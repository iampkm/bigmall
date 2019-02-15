Vue.component("area-selector", {
    template:
        "<span>"
        + "<el-input v-model='displayText' v-bind:placeholder='placeholder' v-on:clear='onClear' readonly clearable>"
        + "<el-button slot='append' icon='el-icon-search' v-on:click='dialogVisible = true'></el-button>"
        + "</el-input>"
        + "<el-dialog title='选择区域' v-bind:width='dialogWidth' custom-class='dialog_size' v-bind:visible.sync='dialogVisible'>"
        + "<el-input v-model='filterText' placeholder='输入区域名称或编码进行过滤' clearable></el-input>"
        + "<el-tree ref='categoryTree' node-key='Key' v-bind:data='treeData' v-bind:props='defaultProps' v-bind:default-checked-keys='value.toString().split(\",\")' v-bind:default-expanded-keys='value.toString().split(\",\")' v-bind:show-checkbox='multiSelect' highlight-current empty-text='没有区域数据。' "
        + " v-bind:style='treeStyle'"
        + " v-bind:filter-node-method='filterNode'"
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
        placeholder: { type: String, default: "选择区域" },
        value: { type: String, default: "" },
        selectLevel: { type: Array, default: [] },
        multiSelect: { type: Boolean, default: false },
        returnSimplified: { type: Boolean, default: false },//多选模式下：简化返回节点，当子节点全部选中时，只返回父节点（selectLevel为空时有效）
        url: { type: String, default: "/Area/LoadAreaTree" },
        dialogWidth: { type: String, default: "50%" },
        dialogHeight: { type: String, default: "300px" }
    },

    data: function () {
        return {
            checkedNodes: [],
            displayText: [],
            dialogVisible: false,
            filterText: "",
            treeData: [],
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
            //alert(this.value);
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
            this.$refs.categoryTree.filter(val);
        },
        checkedNodes: function () {
            var $this = this;
            //this.value = [];
            var checkedKeys = [];
            this.displayText = [];
            this.checkedNodes.forEach(function (item, index) {
                $this.displayText.push(item.Label);
                //$this.value.push(item.Key);
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
        onCheck: function (node, checkedState) {
            if (!this.multiSelect) return;
            this.checkedNodes = [];
            var nodes = checkedState.checkedNodes;
            for (var i = 0; i < nodes.length; i++) {
                var item = nodes[i];
                if (this.returnSimplified == false || this.selectLevel.length > 0) {
                    if (this.canSelect(item))
                        this.checkedNodes.push(item);
                    continue;
                }
                //简化返回，子级全选仅返回父级节点，否则返回子节点
                if (item.ParentCode && nodes.contains(function (n) { return n.Code == item.ParentCode; })) continue;//有父级，且父级节点被选中，则不返回该节点
                this.checkedNodes.push(item);
            }
        },
        onClick: function (node) {
            if (this.multiSelect) return;
            if (this.canSelect(node))
                this.checkedNodes = [node];
        },
        canSelect: function (node) {
            return this.selectLevel.length == 0 || this.selectLevel.indexOf(node.Level) > -1;
        },
        onClear: function () {
            if (this.$refs.categoryTree)
                this.$refs.categoryTree.setCheckedNodes([]);
            this.checkedNodes = [];
        },
        validate: function (checkedKeys) {
            if (!this.multiSelect && checkedKeys.length > 1) {  //单选
                this.checkedNodes = [];
                this.$message.error("仅允许单选！");
                return false;
            } else {  //是否可选
                var matchedNodes = this.findNodes(checkedKeys);
                for (var i = 0; i < matchedNodes.length; i++) {
                    var node = matchedNodes[i];
                    if (!this.canSelect(node)) {
                        this.$message.error("绑定的值包含不可选的节点！");
                        this.checkedNodes = [];
                        return false;
                    }
                }
            }
            return true;
        },
        loadData: function (callback) {
            if (!this.url) return;
            var $this = this;
            this.treeData = [];
            var checkedKeys = this.value.toString().split(",");
            this.checkedNodes = [];
            $.post(this.url, null, function (data) {
                if (data) $this.treeData = data;
                $this.setCheckedNodes(checkedKeys);
            }, "json");
        },
        setCheckedNodes: function (keys) {
            this.onClear();
            if (this.validate(keys)) {
                var matchedNodes = this.findNodes(keys);
                this.checkedNodes = matchedNodes;
            }
        },
        findNodes: function (keys, nodes) {
            if (!nodes) nodes = this.treeData;
            var matchedNodes = [];
            if (nodes) {
                var $this = this;
                nodes.forEach(function (node) {
                    if (keys.contains(node.Key))
                        matchedNodes.push(node);
                    if (node.Children) {
                        var matchedChildNodes = $this.findNodes(keys, node.Children);
                        matchedNodes = matchedNodes.concat(matchedChildNodes);
                    }
                });
            }
            return matchedNodes;
        }
    },
    created: function () {
        this.loadData();
    }
});