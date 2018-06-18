


;
var msgbox = {
    baseAlert: function (iconIndex, content, callback) {
        return layer.alert(content, {
            icon: iconIndex,
            skin: 'layer-ext-moon'
        }, callback);
    },
    //警告
    warn: function (content, callback) {

        return this.baseAlert(0, content, callback);
    },
    //完成 
    ok: function (content, callback) {

        return this.baseAlert(1, content, callback);
    },

    error: function (content, callback) {
        return this.baseAlert(2, content, callback);
    },

    question: function (content, callback) {

        return this.baseAlert(3, content, callback);
    },

    fail: function (content, callback) {

        return this.baseAlert(5, content, callback);
    },

    success: function (content, callback) {

        return this.baseAlert(6, content, callback);
    }

};

var tipslayer = {
    baseTips: function (iconIndex, content) {
        return layer.msg(content, { icon: iconIndex, time: 2000 });
    },

    ok: function (content) {

        return this.baseTips(1, content);
    }

};
