var JsExt = {
    isRef: function (obj) {
        return obj && (obj.constructor == Object || obj.constructor == Array || obj.constructor == Date);
    },
    isArray: function (obj) {
        return obj && obj.constructor == Array;
    },
    isObject: function (obj) {
        return obj && obj.constructor == Object;
    },
    isArrayOrObject: function (obj) {
        return this.isArray(obj) || this.isObject(obj);
    },
    clone: function (obj, deep) {
        if (!JsExt.isRef(obj)) return obj;
        if (obj.constructor == Date) return new Date(obj);
        var clones = obj.constructor();
        for (var p in obj) {
            if (obj === obj[p]) {//循环引用
                clones[p] = clones;
                continue;
            }
            clones[p] = deep ? JsExt.clone(obj[p], deep) : obj[p];
        }
        return clones;
    },
    isDefined: function (obj, prop) {
        if (obj == undefined || obj == null) return false;
        return obj[prop] != undefined;
    },
    map: function (source, target, baseTypeAutoConversion) {
        if (source == undefined || source == null || source == NaN || target == NaN) return target;
        if (target == null || target == undefined) target = source.constructor();

        if (!JsExt.isRef(source) && !JsExt.isRef(target)) {
            if (source.constructor != target.constructor) {
                if (!baseTypeAutoConversion) return target;
                if (target.constructor == Boolean) {
                    if (source.constructor == String) {
                        if (!isNaN(Number(source)))
                            return target = target.constructor(Number(source));
                        return target = (source.toLowerCase() == "true");
                    }
                }
            }
            return target = target.constructor(source);
        }

        if (target.constructor == Date) {
            if (source.constructor != target.constructor && !baseTypeAutoConversion) return target;
            return target = new Date(source);
        }

        if (source.constructor != target.constructor) return target;

        if (target.constructor == Array) {
            if (target.length == 0) return target;
            var template = target[0];
            target.splice(0);

            for (var i = 0; i < source.length; i++) {
                target.push(JsExt.clone(template, true));
                target[i] = JsExt.map(source[i], target[i], baseTypeAutoConversion);
            }
            return target;
        }

        for (var p in target) {
            if (JsExt.isDefined(source, p))
                target[p] = JsExt.map(source[p], target[p], baseTypeAutoConversion);
        }
        return target;
    },
    mergeObjectProps: function (obj1, obj2) {
        if (!obj1 || !this.isArrayOrObject(obj1) || arguments.length < 2) return obj1;
        if (this.isArray(obj1) && obj1.length == 0) return obj1;

        for (var i = 1; i < arguments.length; i++) {
            var arg = arguments[i];
            if (!arg || arg.constructor != obj1.constructor) continue;

            if (this.isArray(arg)) {
                if (arg.length == 0) continue;
                for (var j = 0; j < obj1.length; j++) {
                    this.mergeObjectProps(obj1[j], arg[0]);
                }
                continue;
            }

            for (var p in arg) {
                if (!this.isDefined(obj1, p)) {
                    obj1[p] = this.clone(arg[p], true);
                    continue;
                }
                if (this.isArrayOrObject(arg[p])) {
                    this.mergeObjectProps(obj1[p], arg[p]);
                    continue;
                }
                if (obj1[p].constructor == arg[p].constructor)
                    obj1[p] = this.clone(arg[p], true);
            }
        }
        return obj1;
    }
};

Array.prototype.contains = function (value) {
    for (var i = 0; i < this.length; i++) {
        if (value.constructor == Function) {
            if (value(this[i])) return true;
        } else {
            if (this[i] == value) return true;
        }
    }
    return false;
}

Array.prototype.remove = function (value) {
    for (var i = 0; i < this.length; i++) {
        if ((value.constructor == Function && value(this[i])) || this[i] == value)
            this.splice(i--, 1);
    }
    return this;
}

Array.prototype.distinct = function (comparer) {
    if (comparer && comparer.constructor != Function) return JsExt.clone(this);
    var newArray = [];
    for (var i = 0; i < this.length; i++) {
        var item = this[i];
        if (comparer) {
            if (!newArray.contains(function (item2) { return comparer(item2, item); })) {
                newArray.push(item);
            }
        } else if (!newArray.contains(item)) {
            newArray.push(item);
        }
    }
    return newArray;
}

Array.prototype.select = function (selector) {
    var newArray = [];
    for (var i = 0; i < this.length; i++) {
        if (selector.constructor == Function) newArray.push(selector(this[i]));
        else newArray.push(this[i]);
    }
    return newArray;
}

Array.prototype.first = function (value) {
    if (!value) return this.length > 0 ? this[0] : null;
    for (var i = 0; i < this.length; i++) {
        if ((value.constructor == Function && value(this[i])) || this[i] == value)
            return this[i];
    }
}

Date.prototype.format = function (format) {
    var o = {
        "M+": this.getMonth() + 1, //month 
        "d+": this.getDate(), //day 
        "h+": this.getHours(), //hour 
        "m+": this.getMinutes(), //minute 
        "s+": this.getSeconds(), //second 
        "q+": Math.floor((this.getMonth() + 3) / 3), //quarter 
        "f": this.getMilliseconds() //millisecond 
    }

    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, this.getFullYear().toString().substr(4 - RegExp.$1.length));
    }

    for (var p in o) {
        if (new RegExp("(" + p + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[p] : ("00" + o[p]).substr(o[p].toString().length));
        }
    }
    return format;
}

String.prototype.toDate = function () {
    if (!this) return;

    var formats = {
        jsonFormat: /^\/Date\((\d{13}(\+\d{4})?)\)\/$/gi,   //字符串格式： /Date(1529026624687)/   或  /Date(1410019200000+0800)/
        jsFormat: /^\"?(\d{4}-\d{1,2}-\d{1,2}T\d{1,2}:\d{1,2}:\d{1,2}(\.\d{1,3})?)Z?\"?$/gi    //字符串格式：2018-06-15T01:37:04.687    或   "2018-06-15T01:37:04.687Z"
    };

    if (formats.jsonFormat.test(this)) {
        return eval(this.replace(formats.jsonFormat, "new Date($1)"));
    }

    if (formats.jsFormat.test(this)) {
        return new Date(this);
    }

    return null;
}

String.prototype.format = function (params) {
    var str = this;
    if (arguments.length == 0) return str;

    if (arguments.length == 1 && params.constructor == Object) {
        for (key in params) {
            var reg = new RegExp("\\{" + key + "\\}", "g");
            str = str.replace(reg, params[key] == undefined ? "" : params[key]);
        }
        return str;
    }

    for (var i = 0; i < arguments.length; i++) {
        var reg = new RegExp("\\{" + i + "\\}", "g");
        str = str.replace(reg, arguments[i] == undefined ? "" : arguments[i]);
    }
    return str;
}
