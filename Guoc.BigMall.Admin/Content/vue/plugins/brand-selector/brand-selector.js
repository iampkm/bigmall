Vue.component("brand-selector", {
    template:
        "<span>"
            + "<el-input    v-model='displayText'  v-bind:placeholder='placeholder' v-on:clear='onClear' readonly clearable>"
                + "<el-button slot='append' icon='el-icon-search' v-on:click='firstLoad'></el-button>"
            + "</el-input>"
            + "<el-dialog v-bind:title='placeholder' v-bind:width='dialogWidth' custom-class='dialog_size' v-bind:visible.sync='dialogVisible'>"
                + "<el-input v-model='code' placeholder='输入编码' clearable style='max-width:49%;'></el-input>"
                + "<el-input v-model='name' placeholder='输入品牌' clearable style='max-width:49%;margin-left:10px;margin-bottom:10px'></el-input>"

                 + "<el-table class='form-group' ref='brandtable' v-bind:data='brandData' border style='width: 100%' v-bind:height='dialogHeight' highlight-current-row "
                 + " v-on:current-change='onCurrentRowChange'"
                 + " v-on:select-all='onSelectAll'"
                 + " v-on:select='onSelect'"
                 //+ "  v-on:selection-change='handleSelectionChange'"
                 + ">"
                        + "<el-table-column type='selection' width='55' v-if='multiSelect'></el-table-column>"
                        + "<el-table-column type='index'>" + "</el-table-column>"
                        + "<el-table-column prop='Code' label='编码' width='150'>" + "</el-table-column>"
                        + "<el-table-column prop='Name' column-key label='品牌'>" + "</el-table-column>"
                 + "   </el-table>"

                  + "  <el-pagination v-bind:current-page='page.pageIndex' v-bind:page-size='page.pageSize' v-bind:total='page.total'  v-bind:layout='page.layout' v-on:size-change='onPageSizeChange' v-on:current-change='onPageChange' background>"
                  + "  </el-pagination>"

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
        selectedBrands: { type: Array, default: [] },
        placeholder: { type: String, default: "选择品牌" },
        multiSelect: { type: Boolean, default: true },
        url: { type: String, default: "/Brand/LoadData" },
        dialogWidth: { type: String, default: "50%" },
        dialogHeight: { type: String, default: "300px" },
        displayTemplate: { type: [String, Function], default: "[{Code}]{Name}" },
        searchArgs: { type: Object, default: { name: "", code: "" } }
    },

    data: function () {
        return {
            displayText: [],
            dialogVisible: false,
            name: "",
            code: "",
            brandData: [],
            page: { total: 0, pageIndex: 1, pageSize: global_config.page.pageSize, isPaging: true },
            //multipleSelection: [],
            tempContainer: [],
            innerEvent: false
        };
    },

    methods: {
        //handleSelectionChange: function (val) {
        //    this.multipleSelection = val;
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

            this.brandData.forEach(function (row) {
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
            this.selectedBrands = [JsExt.clone(currentRow, true)];
        },


        onPageChange: function (page) {
            this.page.pageIndex = page;
            this.loadData();
        },
        onPageSizeChange: function (pageSize) {
            this.page.pageSize = pageSize;
            this.loadData();
        },

        onClear: function () {
            //this.displayText = [];
            //this.value = "";
            //this.multipleSelection = [];
            //this.$refs.brandtable.clearSelection();
            this.selectedBrands = [];
            if (this.$refs.brandtable)
                this.$refs.brandtable.clearSelection();
        },

        firstLoad: function () {
            this.tempContainer = [];
            this.page.pageIndex = 1;
            this.loadData();
            this.dialogVisible = true;
        },

        validate: function (checkedIds) {
            if (!this.multiSelect && checkedIds.length > 1) {//单选
                this.selectedBrands = [];
                this.$message.error("brand-selector仅允许单选！");
                return false;
            }
            return true;
        },

        loadData: function () {
            if (!this.url) return;
            var $this = this;
            this.brandData = [];

            var args = Object.assign({}, this.searchArgs, this.page);

            $.post(this.url, args, function (result) {
                if (result.success) {
                    $this.brandData = result.data;
                    $this.page.total = result.total;
                } else {
                    $this.$message.error(result.error);
                }
            }, "json");
        },

        setInitialValue: function () {
            var values = this.value.toString().trim();
            this.onClear();
            if (!values) return;
            if (!this.validate(values.split(","))) return;

            //this.onClear();
            var $this = this;
            var args = Object.assign({}, this.searchArgs, this.page, { isPaging: false, ids: values });
            $.post(this.url, args, function (result) {
                if (result.success) {
                    $this.selectedBrands = result.data.distinct(function (item1, item2) { return item1.Id == item2.Id; });
                } else {
                    $this.$message.error(result.error);
                }
            }, "json");
        },

        complete: function () {
            var $this = this;
            this.dialogVisible = false;
            this.selectedBrands = JsExt.clone(this.tempContainer, true);
            //延迟执行callback，保证callback事件执行时watch里面的selectedBrands已经执行（即value与页面控件的v-model已经被更新）
            setTimeout(function () {
                $this.$emit("callback", $this.selectedBrands, $this.dataKey);
            }, 10);
        },
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
        selectedBrands: function () {
            var $this = this;
            var selectedIds = [];
            this.displayText = [];
            this.selectedBrands.forEach(function (item) {
                selectedIds.push(item.Id);
                if ($this.displayTemplate.constructor == Function) {
                    $this.displayText.push($this.displayTemplate(JsExt.clone(item, true)));
                } else {
                    var textTemplate = $this.displayTemplate.toString();
                    for (var p in item)
                        textTemplate = textTemplate.replace("{" + p + "}", item[p]);
                    $this.displayText.push(textTemplate);
                }
            });
            this.innerEvent = true;
            this.value = selectedIds.toString();
            this.$emit("value-change", this.value);
            this.$emit("update:selectedBrands", this.selectedBrands);
        }
    },
    created: function () {
        this.setInitialValue();
        //this.loadData();
        this.$emit("initialize", this, this.dataKey);
    }
});