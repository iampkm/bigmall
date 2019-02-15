Vue.component("supplier-selector", {
    template:
        "<span>"
            + "<el-input    v-model='displayText'  v-bind:placeholder='placeholder' v-on:clear='onClear'  v-bind:disabled='disabled' readonly clearable>"
                + "<el-button slot='append' icon='el-icon-search' v-on:click='storeDialogVisible = true' v-bind:disabled='disabled'></el-button>"
            + "</el-input>"
            + "<el-dialog title='选择供应商' v-bind:width='dialogWidth' custom-class='dialog_size' v-bind:visible.sync='storeDialogVisible'>"
                + "<el-input v-model='name' placeholder='输入名称' clearable style='width:150px;'></el-input>"
                + "<el-input v-model='code' placeholder='输入编码' clearable style='width:150px;margin-left:10px;margin-bottom:10px'></el-input>"
                 + "  <el-table class='form-group' ref='brandtable' v-bind:data='brandData'   border style='width: 100%' height='450' "
                 + "  v-on:selection-change='handleSelectionChange'  v-on:current-change='handleCurrentChange'>"//  v:bind:filter-chang=''>"

                   + "<el-table-column type='selection' v-if='multiSelect' width='55'></el-table-column>"

                      + "<el-table-column type='index'   label='Id' width='100'>" + "</el-table-column>"
                    + "<el-table-column prop='Name' column-key label='供应商' width='200'>" + "</el-table-column>"
                    + "<el-table-column prop='Code' label='编码' width='150'>" + "</el-table-column>"
                 + "   </el-table>"
                  + "  <el-pagination v-bind:current-page='page.pageIndex' v-bind:page-size='page.pageSize' v-bind:total='page.total'  layout='total,sizes,prev,pager,next,jumper' v-on:size-change='onPageSizeChange' v-on:current-change='onPageChange' background>"
                + "  </el-pagination>"
            + "</el-dialog>"
        + "</span>",

    model: {
        prop: "value",
        event: "value-change"
    },


    props: {
        value: { type: String, default: "" },
        placeholder: "选择供应商",
        url: { type: String, default: "/Supplier/LoadData" },
        dialogWidth: { type: String, default: "50%" },
        dialogHeight: { type: String, default: "300px" },
        multiSelect: { type: Boolean, default: false },
        disabled: { type: Boolean, default: false },

    },

    data: function () {
        return {

            displayText: [],
            storeDialogVisible: false,
            name: "",
            code: "",
            brandData: [],
            // checkDate:[],
            page: { total: 0, pageIndex: 1, pageSize: global_config.page.pageSize },
            multipleSelection: [],
            signalSelection: null,
            eventList: {
                valueChanged: "value-changed"
            }
        };
    },

    methods: {
        //选中行
        handleCurrentChange: function (val) {
            this.signalSelection = val;
        },
        //多选
        handleSelectionChange: function (val) {
            this.multipleSelection = val;
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
            //this.multipleSelection = [];
            this.displayText = [];
            this.value = "";
            this.multipleSelection = [];
            this.signalSelection = null;
            this.$refs.brandtable.clearSelection();
            this.$refs.brandtable.setCurrentRow();
        },
        loadData: function (callback) {
            if (!this.url) return;
            var self = this;
            var _url = this.url;
            this.brandData = [];


            var _pageArgs = {
                pageIndex: this.page.pageIndex,
                pageSize: this.page.pageSize,
                isPaging: true
            };
            var _args = Object.assign({}, self.page, { name: this.name, code: this.code });

            $.post(this.url, _args, function (result) {
                if (result.success) {
                    self.brandData = result.data;
                    self.page.total = result.total;
                }
                callback(self.brandData);
            }, "json");
        }
    },
    watch: {
        name: function (val) {
            //this.page.pageIndex = 1;
            this.loadData(null);

        },
        code: function (val) {
            // this.page.pageIndex = 1;
            this.loadData(null);
        },
        multipleSelection: function () {
            var $this = this;
            var getvalue = [];
            this.displayText = [];
            this.multipleSelection.forEach(function (item) {
                $this.displayText.push(item.Name);
                getvalue.push(item.Id);
                // vue.table.agrs.brandIds.push(item.Id);
                //demo.suppliers.push(item.Id);
            });
            this.value = getvalue.toString();
            this.$emit("value-change", this.value);
        },
        signalSelection: function () {
            var $this = this;
            var getvalue = [];
            this.displayText = [];

            $this.displayText.push($this.signalSelection.Name);
            getvalue.push($this.signalSelection.Id);
            // vue.table.agrs.brandIds.push(item.Id);
            //demo.suppliers.push($this.signalSelection.Id);
            this.value = getvalue.toString();
            this.$emit("value-change", this.value);
            this.$emit(this.eventList.valueChanged, JsExt.clone(this.signalSelection, true));

        }

    },
    created: function () {

        var $this = this;
        var checkedKeys = this.value.toString().split(',');

        function getDefaultCheckedNodes(nodes) {
            if (nodes) {
                nodes.forEach(function (node) {
                    if (checkedKeys.contains(node.Id)) {
                        $this.multipleSelection.push(node);
                        $this.signalSelection = node;
                    }
                });
            }
        }
        this.loadData(getDefaultCheckedNodes);
    }
});