Vue.component("menu-selector", {
    template:
        "<span>"
            + "<el-input    v-model='displayText' v-bind:placeholder='placeholder' v-on:clear='onClear' style='width:386.5px;' readonly clearable>"
                + "<el-button slot='append' icon='el-icon-search' v-on:click='storeDialogVisible = true'></el-button>"
            + "</el-input>"
            + "<el-dialog title='选择菜单' v-bind:width='dialogWidth' custom-class='dialog_size' v-bind:visible.sync='storeDialogVisible'>"
                + "<el-input v-model='name' placeholder='输入菜单名称' clearable style='width:150px;margin-bottom:10px'></el-input>"
             
                 + "  <el-table class='form-group' ref='table' v-bind:data='menuData'   border style='width: 100%' height='450'  "
                 + "   highlight-current-row  v-on:current-change='handleCurrentChange'>"//  v:bind:filter-chang=''>"
            
                 
                    + "<el-table-column type='index'></el-table-column>"
                      + "<el-table-column prop='Id'   label='Id' width='100'>" + "</el-table-column>"
                    + "<el-table-column prop='Name' column-key label='菜单名' width='100'>" + "</el-table-column>"
                    + "<el-table-column prop='Url' label='链接' width='150'>" + "</el-table-column>"
                    + "<el-table-column prop='Icon' label='图标' width='100' sortable>" + "</el-table-column>"
                    + "<el-table-column prop='DisplayOrder' label='排序' width='250'>" + "</el-table-column>"
                 + "   </el-table>"
                  + "  <el-pagination v-bind:current-page='page.pageIndex' v-bind:page-size='page.pageSize' v-bind:total='page.total'  layout='total,sizes,prev,pager,next,jumper' v-on:size-change='onPageSizeChange' v-on:current-change='onPageChange' background>"
                + "  </el-pagination>"
            + "</el-dialog>"
        + "</span>",

    props: {
        value: "",
        placeholder: "选择父菜单",
        url: { type: String, default: "/Menu/LoadData" },
        dialogWidth: { type: String, default: "50%" },
        dialogHeight: { type: String, default: "300px" },
      
    },

    data: function () {
        return {
           
            displayText: "",
            storeDialogVisible: false,
            name: "",
            menuData: [],
            checkDate:[],
        page: { total: 0, pageIndex: 1, pageSize: global_config.page.pageSize },
        multipleSelection:null
        };
    },

    methods: {
        //选中行
        handleCurrentChange: function (val) {
            this.multipleSelection = val;
            this.displayText = this.multipleSelection.Name;
            this.value = this.multipleSelection.Id;
            //alert(this.value);
            demo.ruleForm.parentId = this.multipleSelection.Id;
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
            this.multipleSelection = null;
            this.displayText = "";
            this.value = "";
           
        },
        loadData: function (callback) {
            if (!this.url) return;
            var self = this;
            var _url = this.url;
            this.menuData = [];
           

            var _pageArgs = {
                pageIndex: this.page.pageIndex,
                pageSize: this.page.pageSize,
                isPaging: true
            };
            var _args = Object.assign({}, self.page, { name: this.name });
            //$.ajax({
            //    url: _url, type: "get", cache: false,data:_args, dataType: "json",
            //    success: function (result) {
            //        if (result.success) {
            //            self.menuData = result.data;
            //            self.page.total = result.total;
                       
            //        }
            //    },
              
               
            //});
            $.post(this.url, _args, function (result) {
                if (result.success) {
                    self.menuData = result.data;
                    self.page.total = result.total;
                }
                callback(self.menuData);
            }, "json");
        }
    },
   watch: {
        name: function (val) {
            this.page.pageIndex = 1;
            this.loadData(null);
           
        },
        checkDate: function () {
            var $this = this;
            this.value = "";
            this.displayText = "";
            //$this.displayTex=checkDate[0].Id;
            //$this.value=checkDate[0].Name
            this.checkDate.forEach(function (item, index) {
                $this.displayText=item.Name;
                $this.value=item.Id;
            });
        
        }
      
    },
   created: function () {
      
       var $this = this;
       var checkedKeys = this.value;

       function getDefaultCheckedNodes(nodes) {
           if (nodes) {
               nodes.forEach(function (node) {
                   if (checkedKeys == node.Id)
                       $this.checkDate.push(node);
                  
               });
           }
       }
       this.loadData(getDefaultCheckedNodes);
    }
});