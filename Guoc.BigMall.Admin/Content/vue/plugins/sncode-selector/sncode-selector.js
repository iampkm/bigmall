Vue.component("sncode-selector", {
    template:
        "<span>"
            + "<el-input    v-model='displayText'  v-bind:placeholder='placeholder' v-on:clear='onClear' v-on:keyup.native.enter='searchProduct' v-bind:readonly='readonly' clearable>"
                + "<el-tooltip slot='prepend' content='扫描二维码' placement='right' effect='light'>"
                    + "<el-button  icon='el-icon-view' v-on:click='batchScanning'></el-button>"
                + "</el-tooltip>"
                + "<el-button slot='append' icon='el-icon-search' v-on:click='firstLoad'></el-button>"
            + "</el-input>"
            + "<el-dialog v-bind:title='placeholder' v-bind:width='dialogWidth' custom-class='dialog_size' v-bind:visible.sync='dialogVisible'>"
                + "<el-input v-model='sncode' placeholder='输入串码' clearable style='max-width:32%;margin-bottom:10px'></el-input>"
                + "<el-input v-model='name' placeholder='输入名称' clearable style='max-width:32%;margin-left:2%;'></el-input>"
                + "<el-input v-model='code' placeholder='输入编码' clearable style='max-width:32%;margin-left:2%;'></el-input>"

                 + "<el-table class='form-group' ref='productTable' v-bind:data='productData' border style='width: 100%' v-bind:height='dialogHeight' highlight-current-row "
                 + " v-on:current-change='onCurrentRowChange'"
                 + " v-on:select-all='onSelectAll'"
                 + " v-on:select='onSelect'"
                 + ">"
                        + "<el-table-column type='selection' width='55' v-if='multiSelect'></el-table-column>"
                        + "<el-table-column type='index'></el-table-column>"
                       // + "<el-table-column prop='Code' label='商品编码' width='100'></el-table-column>"
                       // + "<el-table-column prop='Name' label='商品名称'></el-table-column>"
                       // + "<el-table-column prop='InventoryQuantity' label='库存' width='80'></el-table-column>"
                       //// + "<el-table-column prop='BrandName' label='品牌' width='100'></el-table-column>"
                       //// + "<el-table-column prop='CategoryName' label='品类' width='100'></el-table-column>"
                       // + "<el-table-column prop='CostPrice' label='采购价' width='80'></el-table-column>"
                        + "<el-table-column  v-for='column in columns' v-bind:prop='column[0]' v-bind:label='column[1]' v-bind:width='column[2]'></el-table-column>"
                 + "</el-table>"

                 + "<el-pagination v-bind:current-page='page.pageIndex' v-bind:page-size='page.pageSize' v-bind:total='page.total'  v-bind:layout='page.layout' v-on:size-change='onPageSizeChange' v-on:current-change='onPageChange' background></el-pagination>"

                 + "<span slot='footer' class='dialog-footer' v-if='multiSelect'>"
                     + "<el-button v-on:click='dialogVisible = false'>取 消</el-button>"
                     + "<el-button type='primary' v-on:click='complete'>确 定</el-button>"
                 + "</span>"
            + "</el-dialog>"
        + "</span>",

    model: {
        prop: "value",
        event: "value-change"
    },

    props: {
        dataKey: {},
        value: { type: String, default: "" },
        selectedProducts: { type: Array, default: [] },
        placeholder: { type: String, default: "选择商品" },
        readonly: { type: Boolean, default: false },
        multiSelect: { type: Boolean, default: true },
        url: { type: String, default: "/Product/LoadSNCodeData" },
        dialogWidth: { type: String, default: "50%" },
        dialogHeight: { type: String, default: "300px" },
        displayTemplate: { type: [String, Function], default: "[{SNCode}]{Name}" },
        columns: { type: Array, default: [["SNCode", "串号", 120], ["Name", "商品名称"], ["Code", "商品编码", 110], ["InventoryQuantity", "库存", 100]] },
        searchArgs: { type: Object, default: { SNCodes: "", name: "", code: "", brandIds: "", categoryId: "", storeId: "", supplierId: "" } }
    },

    data: function () {
        return {
            displayText: "",
            dialogVisible: false,
            sncode: "",
            name: "",
            code: "",
            productData: [],
            tempContainer: [],
            page: { total: 0, pageIndex: 1, pageSize: global_config.page.pageSize, layout: global_config.page.layout, IsPaging: true }
        };
    },

    methods: {
        //多选，复选框触发
        onSelectAll: function (selection) {
            var rowData;
            var behavior;
            var $this = this;
            function comparator(item) { return (rowData.HasSNCode ? item.SNCode == rowData.SNCode : item.Code == rowData.Code); }

            if (selection.length > 0) {//全选
                behavior = function (row) {
                    if (!$this.tempContainer.contains(comparator))
                        $this.tempContainer.push(JsExt.clone(row, true));
                }
            } else {//全不选
                behavior = function () {
                    $this.tempContainer.remove(comparator);
                }
            }

            this.productData.forEach(function (row) {
                rowData = row;
                behavior(row);
            });
        },
        //多选，复选框触发
        onSelect: function (selection, row) {
            function comparator(item) { return (row.HasSNCode ? item.SNCode == row.SNCode : item.Code == row.Code); }
            var isChecked = selection.contains(comparator);
            if (isChecked) {
                if (!this.tempContainer.contains(comparator))
                    this.tempContainer.push(JsExt.clone(row, true));
            } else {
                this.tempContainer.remove(comparator);
            }
        },
        //点击行触发
        onCurrentRowChange: function (currentRow, oldCurrentRow) {
            if (this.multiSelect) return;//只处理单选
            if (!currentRow) return;
            this.selectedProducts = [JsExt.clone(currentRow, true)];
        },

        //添加显示列
        //参数形式：(["columnName", "columnText", width]) 
        //          (["columnName", "columnText", width], ["columnName", "columnText", width], ...) 
        //          ([["columnName", "columnText", width], ["columnName", "columnText", width], ...])
        addColumns: function (column) {
            if (!column || column.constructor != Array || column.length == 0) return;
            var columnArray = arguments.length > 1 ? arguments : (column[0].constructor == Array ? column : [column]);
            for (var i = 0; i < columnArray.length; i++) {
                this.columns.push(columnArray[i]);
            }
        },
        //移除显示列
        //参数形式：("columnName") 
        //          ("columnName1", "columnName2", ...) 
        //          (["columnName1", "columnName2", ...])
        removeColumns: function (columnName) {
            if (!columnName || (columnName.constructor != String && columnName.constructor != Array)) return;
            var columnNameArray = arguments.length > 1 ? arguments : (columnName.constructor == Array ? columnName : [columnName]);
            for (var i = 0; i < columnNameArray.length; i++) {
                this.columns.remove(function (item) { return item[0] == columnNameArray[i]; });
            }
        },

        onPageChange: function (page) {
            this.page.pageIndex = page;
            this.loadData();
        },
        onPageSizeChange: function (pageSize) {
            this.page.pageSize = pageSize;
            this.loadData();
        },

        onClear: function (noCallback) {
            this.selectedProducts = [];
            if (this.$refs.productTable)
                this.$refs.productTable.clearSelection();
            if (this.multiSelect && !noCallback)
                this.$emit("callback", this.selectedProducts, this.dataKey);
        },

        firstLoad: function () {
            this.tempContainer = [];
            this.page.pageIndex = 1;
            this.loadData();
            this.dialogVisible = true;
        },

        validate: function (checkedSNCodes) {
            if (!this.multiSelect && checkedSNCodes.length > 1) {//单选
                this.selectedProducts = [];
                this.$message.error("sncode-selector仅允许单选！");
                return false;
            }
            return true;
        },

        loadData: function () {
            if (!this.url) return;
            var self = this;
            var _url = this.url;
            this.productData = [];

            //searchArgs: { type: Object, default: { name: "", code: "", brandIds: "", categoryId: "", ids: "" } }
            //var _args = Object.assign({}, self.page, { name: this.name, code: this.code });
            var args = Object.assign({}, this.searchArgs, this.page);

            $.post(this.url, args, function (result) {
                if (result.success) {
                    self.productData = result.data;
                    self.page.total = result.total;
                } else {
                    self.$message.error(result.error);
                }
            }, "json");
        },

        setInitialValue: function () {
            var values = this.value.toString().trim();
            if (!values) return;
            if (!this.validate(values.split(","))) return;

            //alert(values);
            this.onClear(true);
            var $this = this;
            //var args = Object.assign({}, this.page, { name: "", code: "", brandIds: "", categoryId: "", ids: this.value.toString() });
            var args = Object.assign({}, this.searchArgs, this.page, { IsPaging: false, SNCodes: values });
            $.post(this.url, args, function (result) {
                if (result.success) {
                    $this.selectedProducts = result.data;
                } else {
                    $this.$message.error(result.error);
                }
            }, "json");
        },

        complete: function () {
            this.dialogVisible = false;
            this.selectedProducts = JsExt.clone(this.tempContainer, true);
            this.$emit("callback", this.selectedProducts, this.dataKey);
        },

        validateSNCode: function (snCode) {
            if (!snCode || !snCode.trim()) {
                this.$message.error("串码不能为空！");
                return false;
            }

            if (/[^a-zA-Z0-9]/.test(snCode)) {
                this.$message.error("串码不能包含特殊字符！【{0}】".format(snCode));
                return false;
            }

            if (snCode.length > 18) {
                this.$message.error("串码无效，长度超过限制！【{0}】".format(snCode));
                return false;
            }

            return true;
        },

        searchProduct: function (snCodes) {
            var errorSNCodes = [];
            if (!snCodes || snCodes.constructor != Array) {
                snCodes = [this.displayText.trim()];
                if (!this.validateSNCode(snCodes[0])) return snCodes;
            }

            var $this = this;
            var args = Object.assign({}, this.searchArgs, this.page, { IsPaging: false, SNCodes: snCodes.toString() });
            //$.post(this.url, args, function (result) {
            //    if (result.success) {
            //        if (!result.data || result.data.length == 0) {
            //            $this.$message.error("串码商品不存在！");
            //            //$this.selectedProducts = [];
            //        } else {
            //            if (result.data.length != snCodes.length)
            //                $this.$message.error("部分串码商品不存在！");

            //            $this.selectedProducts = result.data;
            //            //多选模式下，触发回调事件
            //            if ($this.multiSelect)
            //                $this.$emit("callback", $this.selectedProducts, $this.dataKey);
            //            success = true;
            //        }
            //    } else {
            //        $this.$message.error(result.error);
            //    }
            //}, "json");
            $.ajax({
                url: this.url,
                async: false,
                type: "POST",
                dataType: "JSON",
                data: args,
                success: function (result) {
                    if (result.success) {
                        if (!result.data || result.data.length == 0) {
                            $this.$message.error("串码商品不存在！");
                            //$this.selectedProducts = [];
                            errorSNCodes = snCodes;
                        } else {
                            if (result.data.length != snCodes.length) {
                                $this.$message.error("部分串码商品不存在！");
                                errorSNCodes = snCodes.filter(function (snCode) { return !result.data.contains(function (product) { return product.SNCode == snCode; }); });
                            }

                            $this.selectedProducts = result.data;
                            //多选模式下，触发回调事件
                            if ($this.multiSelect)
                                $this.$emit("callback", $this.selectedProducts, $this.dataKey);
                        }
                    } else {
                        $this.$message.error(result.error);
                        errorSNCodes = snCodes;
                    }
                }
            });

            return errorSNCodes;
        },

        batchScanning: function () {
            var $this = this;
            function lockSubmit(box, lock) {
                box.showClose = !lock;
                box.closeOnPressEscape = !lock;
                box.closeOnClickModal = !lock;
                box.showCancelButton = !lock;
                box.confirmButtonLoading = lock;
                box.confirmButtonText = lock ? "执行中..." : "确定";
            }

            this.$msgbox({
                title: '批量扫码',
                showInput: true,
                inputType: "textarea",
                inputPlaceholder: "扫描二维码",
                showCancelButton: true,
                showConfirmButton: true,
                confirmButtonText: "确定",
                cancelButtonText: "取消",
                customClass: "snCodeBatchScanningBox",
                beforeClose: function (action, instance, done) {
                    if (action === "confirm") {
                        var value = instance.inputValue;
                        if (!value || !value.trim()) {
                            $this.$message.error("串码不能为空！");
                            return;
                        }

                        value = value.trim();
                        var snCodes = value.toUpperCase().split("\n").select(function (snCode) { return snCode ? snCode.trim() : snCode; }).remove(function (snCode) { return !snCode || !snCode.trim(); }).distinct();
                        if (snCodes.length == 0) {
                            $this.$message.error("串码不能为空！");
                            return;
                        }

                        for (var i = 0; i < snCodes.length; i++) {
                            if ($this.validateSNCode(snCodes[i]) == false) return;
                        }

                        lockSubmit(instance, true);
                        var errorSNCodes = $this.searchProduct(snCodes);
                        if (errorSNCodes && errorSNCodes.length > 0) {
                            instance.inputValue = errorSNCodes.join("\n");
                            lockSubmit(instance, false);
                            return;
                        }
                    }
                    lockSubmit(instance, false);
                    done();
                }
            });
        },

        getDisplayText: function (item) {
            if (this.displayTemplate.constructor == Function)
                return this.displayTemplate(JsExt.clone(item, true));

            var textTemplate = this.displayTemplate.toString();
            for (var p in item)
                textTemplate = textTemplate.replace("{" + p + "}", (item[p] == null ? "" : item[p]));
            return textTemplate
        }
    },
    watch: {
        name: function (val) {
            this.searchArgs.name = this.name;
            this.loadData();

        },
        code: function (val) {
            this.searchArgs.code = this.code;
            this.loadData();
        },
        sncode: function (val) {
            //this.searchArgs.SNCodes = this.sncode;
            this.searchArgs.SNCodes = "";
            this.searchArgs.SNCodeLike = this.sncode;
            this.loadData();
        },
        selectedProducts: function () {
            var $this = this;
            var selectedSNCodes = [];
            var displayTextArray = [];
            this.displayText = "";
            this.selectedProducts.forEach(function (item) {
                if (item.HasSNCode)
                    selectedSNCodes.push(item.SNCode);

                var text = $this.getDisplayText(item);
                if (text)
                    displayTextArray.push(text);
            });
            ////仅显示第一个选择项的信息
            //if (selectedSNCodes.length > 0) {
            //    var item = this.selectedProducts[0];
            //    this.displayText = this.getDisplayText(item);
            //}
            this.displayText = displayTextArray.reverse().toString();//显示所有选择项
            this.value = selectedSNCodes.toString();
            this.$emit("value-change", this.value);
            this.$emit("update:selectedProducts", this.selectedProducts);
        }
    },
    created: function () {
        this.setInitialValue();
        //this.loadData();
        this.$emit("initialize", this, this.dataKey);
    }
});