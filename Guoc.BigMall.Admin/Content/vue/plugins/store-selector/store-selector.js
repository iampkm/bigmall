Vue.component("store-selector", {
    template:
        "<span>"
            + "<el-input v-model='displayText' v-bind:placeholder='placeholder' v-on:clear='onClear'  v-bind:disabled='disabled' readonly clearable>"
                + "<el-button slot='append' icon='el-icon-search' v-on:click='storeDialogVisible = true'  v-bind:disabled='disabled'></el-button>"
            + "</el-input>"
            + "<el-dialog title='选择门店' v-bind:width='dialogWidth' custom-class='dialog_size' v-bind:visible.sync='storeDialogVisible'>"
                + "<el-input v-model='filterText' placeholder='输入门店名称或编码进行过滤' clearable></el-input>"
                + "<el-tree ref='storeTree' node-key='Key' v-bind:data='storeData' v-bind:props='defaultProps' v-bind:default-checked-keys='value.toString().split(\",\")' v-bind:default-expanded-keys='value.toString().split(\",\")' v-bind:show-checkbox='multiSelect' highlight-current empty-text='没有门店数据。' "
                + " v-bind:style='treeStyle'"
                + " v-bind:filter-node-method='filterNode'"
                //+ " v-on:check-change='onCheckChange'"
                + " v-on:check='onCheck'"
                + " v-on:node-click='onClick'"
                + ">"
                + "</el-tree>"
            + "</el-dialog>"
        + "</span>",

    model: {
        prop: "value",
        event: "value-change"
    },

    props: {
        value: { type: String, default: "" },
        placeholder: { type: String, default: "选择门店" },
        multiSelect: { type: Boolean, default: false },
        url: { type: String, default: "/Store/LoadStoreTree" },
        dialogWidth: { type: String, default: "50%" },
        dialogHeight: { type: String, default: "300px" },
        disabled: { type: Boolean, default: false },
    },

    data: function () {
        return {
            checkedNodes: [],
            displayText: [],
            storeDialogVisible: false,
            filterText: "",
            storeData: [],
            defaultProps: {
                children: "Children",
                label: "Label"
            },
            treeStyle: {
                overflow: "auto",
                maxHeight: this.dialogHeight
            },
            innerEvent: false,
            eventList: {
                valueChanged: "value-changed"
            }
        };
    },
    watch: {
        value: function () {
            var checkedKeys = this.value.toString().split(",");
            if (this.innerEvent) {
                this.innerEvent = false;
                this.validate(checkedKeys);
            } else {
                //外部改变value值时，组件内设置选中
                this.setCheckedNodes(checkedKeys);
            }
        },
        filterText: function (val) {
            this.$refs.storeTree.filter(val);
        },
        checkedNodes: function () {
            var $this = this;
            var checkedKeys = [];
            this.displayText = [];
            this.checkedNodes.forEach(function (item, index) {
                $this.displayText.push(item.Label);
                checkedKeys.push(item.Key);
            });

            this.innerEvent = true;
            this.value = checkedKeys.toString();
            this.$emit("value-change", this.value);
            this.$emit(this.eventList.valueChanged, JsExt.clone(this.checkedNodes, true));
        }
    },
    methods: {
        filterNode: function (value, data) {
            if (!value) return true;
            return data.Label.indexOf(value) !== -1;
        },
        //onCheckChange: function (node, isChecked, childIsChecked) {
        //    if (!node.IsStore) return;

        //    if (isChecked) {
        //        this.checkedNodes.push(node);
        //    } else {
        //        var $this = this;
        //        this.checkedNodes.forEach(function (item, index) {
        //            if (item == node) {
        //                $this.checkedNodes.splice(index, 1);
        //                return false;
        //            }
        //        });
        //    }
        //},
        onCheck: function (node, checkedState) {
            if (!this.multiSelect) return;
            //alert(checkedState.checkedNodes);
            //alert(checkedState.checkedKeys);
            var $this = this;
            this.checkedNodes = [];
            checkedState.checkedNodes.forEach(function (item) {
                if (item.IsStore)
                    $this.checkedNodes.push(item);
            });
        },
        onClick: function (node) {
            if (this.multiSelect) return;
            if (node.IsStore) {
                if (!this.checkedNodes.contains(node))
                    this.checkedNodes = [node];
            }
        },
        onClear: function () {
            if (this.$refs.storeTree)
                this.$refs.storeTree.setCheckedNodes([]);
            this.checkedNodes = [];
        },
        validate: function (checkedKeys) {
            if (!this.multiSelect && checkedKeys.length > 1) {//单选
                this.checkedNodes = [];
                this.$message.error("store-selector仅允许单选！");
                return false;
            }
            return true;
        },
        loadData: function () {
            if (!this.url) return;
            var $this = this;
            this.storeData = [];
            $.post(this.url, { name: "", code: "", groupId: "" }, function (data) {
                if (data) $this.storeData = data;
                var checkedKeys = $this.value.toString().split(",");
                $this.setCheckedNodes(checkedKeys);
            }, "json");
        },
        setCheckedNodes: function (keys) {
            this.onClear();
            if (this.validate(keys))
                this._setCheckedNodes(this.storeData, keys);
        },
        _setCheckedNodes: function (nodes, keys) {
            if (nodes) {
                var $this = this;
                nodes.forEach(function (node) {
                    if (node.IsStore && keys.contains(node.Key))
                        $this.checkedNodes.push(node);
                    if (node.Children) {
                        $this._setCheckedNodes(node.Children, keys);
                    }
                });
            }
        }
    },
    created: function () {
        this.loadData();
    }
});