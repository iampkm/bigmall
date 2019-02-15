Vue.component("subpage-dialog", {
    template:
        "<span>"
            + "<el-button v-bind:type='type' v-on:click='show'>"
                + "{{text}}"
            + "</el-button>"
            + "<el-dialog v-bind:title='title' v-bind:width='dialogWidth' append-to-body='true' v-bind:visible.sync='dialogVisible'>"
                + "<iframe v-bind:src='pageUrl' v-bind:style='iframeStyle' frameborder='0'></iframe>"
                + "<span slot='footer' class='dialog-footer' v-if='innerButtons.length>0'>"
                    + "<el-button v-for='button in innerButtons' v-bind:type='button.type' v-bind:disabled='button.disabled' v-on:click='button.baseClick'>{{button.text}}</el-button>"
                + "</span>"
            + "</el-dialog>"
        + "</span>",

    props: {
        type: { type: String, default: "text" },
        text: { type: String, default: "查看" },
        title: { type: String, default: "" },
        url: { type: String, default: "" },
        dialogWidth: { type: String, default: "70%" },
        dialogHeight: { type: String, default: "450px" },
        buttons: { type: Array, default: [] }//[{ buttonId: "ok", text: "确定", type: null, autolock: true, callback: null }, { buttonId: "cancel", text: "取消", type: null, autolock: true, callback: null }]
    },

    data: function () {
        return {
            pageUrl: "",
            dialogVisible: false,
            iframeStyle: {
                width: "100%",
                height: this.dialogHeight,
                overflow: "auto"
            },
            innerButtons: []
        };
    },

    methods: {
        show: function () {
            window.activeDialog = this;
            this.pageUrl = this.url;
            this.dialogVisible = true;
        },
        close: function () {
            this.dialogVisible = false;
            this.pageUrl = "";
        },
        bindEvent: function (buttonId, eventFunc) {
            if (!buttonId || !eventFunc || (typeof eventFunc) != "function") return;
            var button = this.innerButtons.first(function (btn) { return btn.buttonId == buttonId; });
            if (button) button.onClick = eventFunc;
        }
    },

    created: function () {
        var $this = this;
        var _buttons = [];
        if (this.buttons.length > 0) {
            this.buttons.forEach(function (button) {
                if (!button.buttonId || !button.text) return true;
                if (_buttons.contains(function (btn) { return btn.buttonId == button.buttonId; })) return true;

                var newButton = JsExt.clone(button, true);
                if ((!newButton.autolock && newButton.autolock != false) || newButton.autolock.constructor != Boolean) newButton.autolock = true;
                if ((!newButton.disabled && newButton.disabled != false) || newButton.disabled.constructor != Boolean) newButton.disabled = false;

                newButton.lock = function () {
                    newButton.disabled = true;
                }

                newButton.unlock = function () {
                    newButton.disabled = false;
                }

                newButton.baseClick = function () {
                    if (newButton.autolock) newButton.lock();
                    if (newButton.onClick) {
                        newButton.onClick($this, newButton, newButton.callback);
                    } else {
                        if (newButton.callback)
                            newButton.callback($this, newButton);
                        if (newButton.autolock) newButton.unlock();
                    }
                }
                _buttons.push(newButton);
            });
            this.innerButtons = _buttons;
        }

        if (!window.closeDialog) {
            window.closeDialog = function () {
                if (window.activeDialog)
                    window.activeDialog.close();
            }
        }
    }
});
