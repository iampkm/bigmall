(function ($, window) {
    //控件的数据源,数据结构{key:xxx,tabs:[{index:x,title:x,url:x,icon:x,navbarid:x}]}
     var _tabDatas = [];   
    //获取所选的Tab信息
    $.fn.aceselecttab = function (options) {
        // 默认参数
        var defaults = {
        };
        //为了给自定义方法用
        var _self = this;
        //当前标签的ID
        var htmlID = $(_self).attr("id");
        // 插件配置
        $.extend(defaults, options);
        return _getShowTab(htmlID);
    }
    //获取当前显示的数据
    var _getShowTab = function (key) {
        var _tabs = _getTabs(key);
        var selectIndex = 0;
        if ($("#" + key + " > ul > li[class*='dropdown']").size() == 1) {
            if ($("#" + key + " > ul > li > ul >li[class*='active']").attr("id") == undefined) {
                selectIndex = $("#" + key + " > ul > li[class='active']").attr("id").replace(key + "-title-li-", "");
                return _tabs[selectIndex];
            }
            selectIndex = $("#" + key + " > ul > li > ul >li[class*='active']").attr("id").replace(key + "-title-li-", "");
        } else {
            selectIndex = $("#" + key + " > ul > li[class='active']").attr("id").replace(key + "-title-li-", "");
        }
        return _tabs[selectIndex];
    }
    //获取所选的Tab信息
    $.fn.aceshowtab = function (options) {
        // 默认参数
        var defaults = {
            index: 0
        };
        //为了给自定义方法用
        var _self = this;
        //当前标签的ID
        var htmlID = $(_self).attr("id");
        // 插件配置
        var opt = $.extend(defaults, options);
        _showTab(htmlID, opt.index);
    }
    //显示Tab
    var _showTab = function (key, index) {
        //头部
        $("#" + key + " > ul > li").removeClass("active");
        //内容
        $("#" + key + "> div > div").removeClass("active").removeClass("in");
        if ($("#" + key + " > ul > li[class*='dropdown']").size() == 1) {
            $("#" + key + " li[id*='title-li']").removeClass("active");
            //包含隐藏列表的,且所选的不在显示区域内的
            if ($("#" + key + " > ul > li").size() - 1 < index + 1) {
                $("#" + key + " > ul > li").last().addClass("active");
                $("#" + key + "-title-li-" + index).addClass("active");
            } else {
                //不包含因此列表的
                //显示
                $("#" + key + "-title-li-" + index).addClass("active");
            }
        } else {
            //不包含因此列表的
            //显示
            $("#" + key + "-title-li-" + index).addClass("active");
        }


        $("#" + key + "-content-" + index).addClass("active").addClass("in");
    }
    //根据key获取相关tab集合
    var _getTabs = function (key) {
        var tabs = [];
        for (var i = 0; i < _tabDatas.length; i++) {
            if (_tabDatas[i].key == key) {
                tabs = _tabDatas[i].tabs;
                break;
            }
        }
        return tabs;
    }
    // 添加Tab
    $.fn.aceaddtab = function (options) {
        // 默认参数
        var defaults = {
            //tab显示标题
            title: "",
            //地址
            url: "",
            //图标icon-home
            icon: "icon-home",
            //头部Log横向条的ID
            navbarid: "navbar"
        };


        //为了给自定义方法用
        var _self = this;
        //当前标签的ID
        var htmlID = $(_self).attr("id");
        // 插件配置
        var opt = $.extend(defaults, options);


        // 初始化函数
        var _init = function () {
            //加载数据
            var allowDo = _initData();
            if (allowDo == true) {
                // 加载内容
                _loadContent();
                // 事件绑定
                _loadEvent();
                //选择最后一个
                _showTab(htmlID, _getTabs(htmlID).length - 1);


            }
        };



        //加载数据
        var _initData = function () {
            var allowDo = true;
            // 初始化一个首页tab
            if (_tabDatas.length == 0)
            {
                _tabDatas = [{ key: htmlID, tabs: [{ index: 0, title: "首 页",icon:"fa fa-home" }] }];  //初始化一个静态的tab 首页
            }          

            if (_tabDatas.length == 0) {
                var _tabData = { key: htmlID, tabs: [] };
                opt.index = 0;
                _tabData.tabs.push(opt);
                _tabDatas.push(_tabData);
                return allowDo;
            }
            for (var i = 0; i < _tabDatas.length; i++) {
                if (_tabDatas[i].key == htmlID) {
                    //判断打开的是否是重复项
                    var index = -1;
                    for (var j = 0; j < _tabDatas[i].tabs.length; j++) {
                        if (_tabDatas[i].tabs[j].title == opt.title) {
                            index = _tabDatas[i].tabs[j].index;
                            break;
                        }
                    }
                    if (index == -1) {
                        opt.index = _getTabs(htmlID).length;
                        _tabDatas[i].tabs.push(opt);
                    } else {
                        _showTab(htmlID, index);
                        //刷新ifrrame 连接
                        var iframe = $("#" + htmlID + "-content-" + index + "> iframe");
                        iframe.attr('src', iframe.attr('src'));
                        allowDo = false;
                    }
                    break;
                }
            }
            return allowDo;
        };
        //根据索引删除数据
        var _deleteTabs = function (index) {
            var tabs = _getTabs(htmlID);
            tabs.splice(index, 1);
            for (var i = 0; i < tabs.length; i++) {
                tabs[i].index = i;
            }


            for (var i = 0; i < _tabDatas.length; i++) {
                if (_tabDatas[i].key == htmlID) {
                    _tabDatas[i].tabs = tabs;
                    break;
                }
            }
            return tabs;
        }



        //加载内容
        var _loadContent = function () {
            var _tabs = _getTabs(htmlID);
            if (_tabs.length == 1) {
                //添加头部Tab样式
                $(_self).addClass("nav-tabs");  //  tabbable nav-tabs-custom
                //头部的列表标签
                var tabTitles = "<ul class=\"nav nav-tabs padding-12 tab-color-blue background-blue\">";
                //循环体
                tabTitles += "<li class=\"active\" id=\"" + htmlID + "-title-li-" + 0 + "\">";
                //---------------------------------------------------
                tabTitles += "<a data-toggle=\"tab\" href=\"#" + htmlID + "-content-" + 0 + "\">";
                tabTitles += " <i class=\"text-success " + opt.icon + " bigger-110\"></i>";
                tabTitles += opt.title;
                //tabTitles += "<i class=\"red icon-remove bigger-110\"></i>";
                //tabTitles+="<i class=\"fa fa-times\"></i>"
                tabTitles += "</a>";
                //---------------------------------------------------
                tabTitles += "</li>";
                tabTitles += "</ul>"


                //区域的内容
                var tabContents = "<div class=\"tab-content\" style=\"padding: 1px 0px 0px 0px;\">";
                //循环体
                tabContents += "<div id=\"" + htmlID + "-content-" + 0 + "\" class=\"tab-pane in active\">";
                //---------------------------------------------------
                tabContents += "<iframe width=\"100%\" height=\"100%\" src=\"" + opt.url + "\" frameborder=\"0\" marginwidth=\"0\" marginheight=\"0\" scrolling=\"auto\"></iframe>";
                //---------------------------------------------------
                tabContents += "</div>"
                tabContents += "</div>"
                $(_self).append(tabTitles + tabContents);
                return;
            }
            if ($("#" + htmlID + " > ul > li[class*='dropdown']").size() == 1) {
                //有列表头部
                var listHtml = "<li id=\"" + htmlID + "-title-li-" + (_tabs.length - 1) + "\">";
                listHtml += "<a data-toggle=\"tab\" href=\"#" + htmlID + "-content-" + (_tabs.length - 1) + "\">";
                listHtml += " <i class=\"text-success " + opt.icon + " bigger-110\"></i>";
                listHtml += opt.title;
               // listHtml += "<i class=\"red icon-remove bigger-110\" style=\"cursor:pointer\"></i>";
                listHtml += "<i class=\"fa fa-times\" style=\"cursor:pointer\"></i>";
                listHtml += "</a>";
                listHtml += "</li>";
                $("#" + htmlID + " > ul > li > ul").append(listHtml);
            } else {
                //头部
                var tabTitles = "<li id=\"" + htmlID + "-title-li-" + (_tabs.length - 1) + "\">";
                //---------------------------------------------------
                tabTitles += "<a data-toggle=\"tab\" href=\"#" + htmlID + "-content-" + (_tabs.length - 1) + "\">";
                tabTitles += " <i class=\"text-success " + opt.icon + " bigger-110\"></i>";
                tabTitles += opt.title;
               // tabTitles += "<i class=\"red icon-remove bigger-110\" style=\"cursor:pointer\"></i>";
                tabTitles += "<i class=\"fa fa-times bigger-110\" style=\"cursor:pointer\"></i>";
                tabTitles += "</a>";
                //---------------------------------------------------
                tabTitles += "</li>";
                $("#" + htmlID + " > ul").append(tabTitles);
            }
            //内容
            var tabContents = "<div id=\"" + htmlID + "-content-" + (_tabs.length - 1) + "\" class=\"tab-pane\">";
            //---------------------------------------------------  
            tabContents += "<iframe width=\"100%\" height=\"100%\" src=\"" + opt.url + "\" frameborder=\"0\" marginwidth=\"0\" marginheight=\"0\" scrolling=\"auto\"></iframe>";
            //---------------------------------------------------
            tabContents += "</div>"
            $("#" + htmlID + "> div").append(tabContents);

        }


        //事件绑定
        var _loadEvent = function () {
            //主页Tab内容区域的高度
            window.onresize = _autoHieght;
            $(function () {
              _autoHieght();
            });
        };
        //处理Tab页卡太多的问题
        var _tabList = function () {
            //获取当前选中的那个Tab
            var selectTab = _getShowTab(htmlID);
            //所有数据
            var tabs = _getTabs(htmlID);
            if ($("#" + htmlID + " > ul").outerHeight(true) < 50) {
                //正常的情况下
                $("#" + htmlID + " > ul > li").remove();
                for (var i = 0; i < tabs.length; i++) {
                    var tabTitles = "<li id=\"" + htmlID + "-title-li-" + i + "\">";
                    //---------------------------------------------------
                    tabTitles += "<a data-toggle=\"tab\" href=\"#" + htmlID + "-content-" + i + "\">";
                    tabTitles += " <i class=\"text-success " + tabs[i].icon + " bigger-110\"></i>";
                    tabTitles += tabs[i].title;
                    if (i != 0) {
                        // tabTitles += "<i class=\"red icon-remove bigger-110\" style=\"cursor:pointer\"></i>";
                        tabTitles += "<i class=\"fa fa-times\" style=\"cursor:pointer\"></i>";
                    }
                    tabTitles += "</a>";
                    $("#" + htmlID + " > ul").append(tabTitles);
                }
            }
            if ($("#" + htmlID + " > ul").outerHeight(true) > 50) {
                //有几个页卡
                var listCount = $("#" + htmlID + " > ul > li").size();
                for (var i = listCount - 1; i >= 0 ; i--) {
                    $($("#" + htmlID + " > ul > li").get(i)).remove();
                    if ($("#" + htmlID + " > ul").outerHeight(true) < 50) {
                        //正常大小之后
                        break;
                    }
                }
                //添加列表选项卡
                var listTab = "<li class=\"dropdown\" >";
                listTab += "<a style=\"cursor:pointer\" data-toggle=\"dropdown\" class=\"dropdown-toggle\" href=\"#\">";
                listTab += "<i class=\"text-success fa fa-list bigger-110\" ></i>";
                listTab += "</a>";
                listTab += "<ul class=\"dropdown-menu dropdown-info\">";
                listTab += "</ul>";
                listTab += "</li>";
                $("#" + htmlID + " > ul").append(listTab);
                while (true) {
                    if ($("#" + htmlID + " > ul").outerHeight(true) > 50) {
                        $("#" + htmlID + " > ul >li").last().prev().remove();
                    } else {
                        break;
                    }
                }
                //目前还显示几个，-掉一个列表的
                var showCount = $("#" + htmlID + " > ul > li").size() - 1;


                var listHtml = "";
                //把多余的给弄到列表里面
                for (var i = 0; i < tabs.length; i++) {
                    if (i > showCount - 1) {
                        listHtml += "<li id=\"" + htmlID + "-title-li-" + i + "\">";
                        listHtml += "<a data-toggle=\"tab\" href=\"#" + htmlID + "-content-" + i + "\">";
                        listHtml += " <i class=\"text-success " + tabs[i].icon + " bigger-110\"></i>";
                        listHtml += tabs[i].title;
                        // listHtml += "<i class=\"red icon-remove bigger-110\" style=\"cursor:pointer\"></i>";
                        listHtml += "<i class=\"fa fa-times\" style=\"cursor:pointer\"></i>";
                        listHtml += "</a>";
                        listHtml += "</li>";
                    }
                }
                $("#" + htmlID + " > ul > li > ul").append(listHtml);
            }
            _showTab(htmlID, selectTab.index);
            //注册关闭事件
            _closeTab();
        }
        //关闭事件
        var _closeTab = function () {
            // $("#" + htmlID + " a i[class*='icon-remove']").click(function (e) {
            $("#" + htmlID + " a i[class*='fa-times']").click(function (e) {
                //当前显示的Tab
                var showTab = _getShowTab(htmlID);
                //选中要删除的Tab
                var selectIndex = $(this).parent().parent().attr("id").replace(htmlID + "-title-li-", "");
                if (showTab.index == selectIndex) {
                    //当前显示和当前选择要删除的Tab一样，就显示前一个
                    _showTab(htmlID, selectIndex - 1);
                }
                var tabs = _deleteTabs(selectIndex);
                $(this).parent().parent().remove();//li
                $("#" + htmlID + "-content-" + selectIndex).remove();//content
                //删除之后，如果列表里面没有值了就把列表删了
                if ($("#" + htmlID + " > ul > li[class*='dropdown']").size() == 1 && $("#" + htmlID + " > ul > li[class*='dropdown']").children().last().children().size() == 0) {
                    $("#" + htmlID + " > ul > li[class*='dropdown']").remove();
                }
                //重新初始化index
                for (var i = 0; i < tabs.length; i++) {
                    $($("#" + htmlID + " ul li[id*='-title-li-']").get(i)).attr("id", htmlID + "-title-li-" + i);
                    $($("#" + htmlID + " li a[href*='-content-']").get(i)).attr("href", "#" + htmlID + "-content-" + i);
                    $($("#" + htmlID + " > div > div[id*='-content-']").get(i)).attr("id", htmlID + "-content-" + i);
                }
                //删除完成高度有变化
                _autoHieght();
                //阻止事件冒泡
                return false;
            });
        }
        //自动高度
        var _autoHieght = function () {
            //处理Tab页卡太多的问题
            _tabList();

            //可视区域的高度
            var documentHeight = parseInt(document.documentElement.clientHeight);
            //取得浏览器的userAgent字符串
            var userAgent = navigator.userAgent;
            var isOpera = userAgent.indexOf("Opera") > -1;
            if (userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1 && !isOpera) {

                documentHeight = parseInt($("html").outerHeight(true));
                //4为IE版本的一个偏差值
                //减1主要是为了给底部留一条缝隙
                documentHeight -= 5;
            }
            else if (userAgent.indexOf("Firefox") > -1) {
                documentHeight = parseInt(document.documentElement.clientHeight);
                //减3主要是为了给底部留一条缝隙
                documentHeight -= 3;
            }
            else if (userAgent.indexOf("Chrome") > -1) {
                //2为谷歌版本的一个偏差值
                //减1主要是为了给底部留一条缝隙
                documentHeight -= 3;
            }
            else {
                //减1主要是为了给底部留一条缝隙
                documentHeight -= 1;
            }
            //主要由tab-content的padding引起.总高度-头部条高度-tabtitle高度
            var contentHeight = documentHeight - (parseInt($("#" + htmlID + " > ul").outerHeight(true)) + parseInt($("#" + opt.navbarid).outerHeight(true)));
            $("#" + htmlID + "> div > div").css("height", contentHeight + "px");           
            //下拉列表的样式调整
            //如果有最后那个下拉列表

            if ($("#" + htmlID + " > ul > li[class*='dropdown']").size() == 1) {

                //修改显示位置，避免被挡住
                var dropdown = $("#" + htmlID + " > ul > li[class*='dropdown']").children().last()
                //列表的宽度
                var listWidth = $("#" + htmlID + " > ul > li[class*='dropdown']").outerWidth(true);
                //+2border宽度
                dropdown.css("left", "-" + (dropdown.outerWidth(true) - listWidth + 3) + "px");
                //列表高度处理,如果小于他的三分之二就不处理，大于就处理
                var listHeight = dropdown.outerHeight(true);
                var flag = parseInt(contentHeight * 2 / 3);
                if (listHeight > flag) {
                    dropdown.css("height", flag).css("overflow-y", "scroll");
                } else {
                    dropdown.css("height", listHeight);
                }
            }
        }
        // 启动插件
        _init();
        // 链式调用
        return this;
    };

    // 关闭当前tab
    $.fn.closeCurrentTab = function () {
        var _self = this;
        var htmlID = $(_self).attr("id");
        var currentTab = _getShowTab(htmlID);

        // 触发点击关闭事件

        $(_self).find("#main-tab-title-li-" + currentTab.index + " a i[class*='fa-times']").click();

       // $("#main-tab-title-li-" + currentTab.index + " a i[class*='fa-times']").click();

       // $("#" + htmlID + " a i[class*='fa-times']").click();
    }
    // 插件结束
})(jQuery, window);