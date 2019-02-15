Vue.component("product-selector", {
    template:
        "<span>"
            + "<el-input    v-model='displayText'  v-bind:placeholder='placeholder' v-on:clear='onClear' v-on:keyup.native.enter='searchProduct' v-bind:readonly='readonly' clearable>"
                + "<el-button slot='append' icon='el-icon-search' v-on:click='firstLoad'></el-button>"
            + "</el-input>"
            + "<el-dialog v-bind:title='placeholder' v-bind:width='dialogWidth' custom-class='dialog_size' v-bind:visible.sync='dialogVisible'>"
                + "<el-input v-model='name' placeholder='输入名称' clearable style='max-width:49%;'></el-input>"
                + "<el-input v-model='code' placeholder='输入编码' clearable style='max-width:49%;margin-left:10px;margin-bottom:10px'></el-input>"

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
        url: { type: String, default: "/Product/LoadData" },
        dialogWidth: { type: String, default: "50%" },
        dialogHeight: { type: String, default: "300px" },
        displayTemplate: { type: [String, Function], default: "[{Code}]{Name}" },
        columns: { type: Array, default: [["Code", "商品编码", 100], ["Name", "商品名称"], ["InventoryQuantity", "库存", 100], ["CostPrice", "采购价", 100]] },
        searchArgs: { type: Object, default: { name: "", code: "", brandIds: "", categoryId: "", storeId: "", supplierId: "" } }
    },

    data: function () {
        return {
            displayText: "",
            dialogVisible: false,
            name: "",
            code: "",
            productData: [],
            page: { total: 0, pageIndex: 1, pageSize: global_config.page.pageSize, layout: global_config.page.layout, IsPaging: true },
            tempContainer: [],
            innerEvent: false
        };
    },

    methods: {
        ////多选，复选框触发
        //onSelectAll: function (selection) {
        //    var rowData;
        //    var behavior;
        //    var $this = this;
        //    function comparator(item) { return item.Id == rowData.Id; }

        //    if (selection.length > 0) {//全选
        //        behavior = function (row) {
        //            if (!$this.selectedProducts.contains(comparator))
        //                $this.selectedProducts.push(JsExt.clone(row, true));
        //        }
        //    } else {//全不选
        //        behavior = function () {
        //            $this.selectedProducts.remove(comparator);
        //        }
        //    }

        //    this.productData.forEach(function (row) {
        //        rowData = row;
        //        behavior(row);
        //    });
        //},
        ////多选，复选框触发
        //onSelect: function (selection, row) {
        //    function comparator(item) { return item.Id == row.Id; }
        //    var isChecked = selection.contains(comparator);
        //    if (isChecked) {
        //        if (!this.selectedProducts.contains(comparator))
        //            this.selectedProducts.push(JsExt.clone(row, true));
        //    } else {
        //        this.selectedProducts.remove(comparator);
        //    }
        //},
        //多选，复选框触发
        onSelectAll: function (selection) {
            var rowData;
            var behavior;
            var $this = this;
            function comparator(item) { return item.Id == rowData.Id; }

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
            function comparator(item) { return item.Id == row.Id; }
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
                if (!this.columns.contains(function (col) { return col[0] == columnArray[i][0]; }))
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

        validate: function (checkedIds) {
            if (!this.multiSelect && checkedIds.length > 1) {//单选
                this.selectedProducts = [];
                this.$message.error("store-selector仅允许单选！");
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
            this.onClear(true);

            var values = this.value.toString().trim();
            if (!values) return;
            if (!this.validate(values.split(","))) return;

            var $this = this;
            //var args = Object.assign({}, this.page, { name: "", code: "", brandIds: "", categoryId: "", ids: this.value.toString() });
            var args = Object.assign({}, this.searchArgs, this.page, { IsPaging: false, ids: values });
            $.post(this.url, args, function (result) {
                if (result.success) {
                    $this.selectedProducts = result.data.distinct(function (item1, item2) { return item1.Id == item2.Id; });
                    //alert($this.selectedProducts);
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

        searchProduct: function () {
            if (!this.displayText || !this.displayText.trim()) return;
            var $this = this;
            var args = Object.assign({}, this.searchArgs, this.page, { IsPaging: false, code: this.displayText.trim() });
            $.post(this.url, args, function (result) {
                if (result.success) {
                    if (!result.data || result.data.length == 0) {
                        $this.$message.error("商品不存在！");
                        //$this.selectedProducts = [];
                    } else {
                        $this.selectedProducts = result.data.distinct(function (item1, item2) { return item1.Id == item2.Id; });
                        //多选模式下，触发回调事件
                        if ($this.multiSelect)
                            $this.$emit("callback", $this.selectedProducts, $this.dataKey);
                    }
                } else {
                    $this.$message.error(result.error);
                }
            }, "json");
        }
    },
    watch: {
        name: function (val) {
            this.searchArgs.name = this.name;
            this.page.pageIndex = 1;
            this.loadData();

        },
        code: function (val) {
            this.searchArgs.code = this.code;
            this.page.pageIndex = 1;
            this.loadData();
        },
        value: function () {
            var checkedIds = this.value.toString().split(",");
            if (this.innerEvent) {
                this.innerEvent = false;
                this.validate(checkedIds);
            } else {
                //外部改变value值时，组件内设置选中
                this.setInitialValue();
            }
        },
        selectedProducts: function () {
            var $this = this;
            var selectedIds = [];
            this.displayText = "";
            this.selectedProducts.forEach(function (item) {
                selectedIds.push(item.Id);
            });
            //仅显示第一个选择项的信息
            if (selectedIds.length > 0) {
                var item = this.selectedProducts[0];
                if (this.displayTemplate.constructor == Function) {
                    this.displayText = this.displayTemplate(JsExt.clone(item, true));
                } else {
                    var textTemplate = this.displayTemplate.toString();
                    for (var p in item)
                        textTemplate = textTemplate.replace("{" + p + "}", item[p]);
                    this.displayText = textTemplate;
                }
            }
            this.innerEvent = true;
            this.value = selectedIds.toString();
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