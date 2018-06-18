



var appcomm = {
    refreshFormValidator: function (form) {
        form.removeData("validator");
        form.removeData("unobtrusiveValidation");
        $.validator.unobtrusive.parse(form);
        form.validateBootstrap();
    },
    isNull: function (value) {
        if (value !== null && value !== 'undefined' && value !== '') {
            return false;
        }
        return true;
    },
    pageSize: 10,
    pageList: [10, 20, 30, 50, 100],

    submitform: function (form, actionurl, beforecall, redirecturl, aftercall, successfun) {

        form.submit(function () {

            var index = 0;
            if (form.valid()) {
                //询问框
                layer.confirm('确认执行该操作吗？', {
                    btn: ['确认', '取消'] //按钮
                }, function () {
                    $.ajax({
                        url: actionurl,
                        type: "post",
                        beforeSend: function (XHR) {

                            index = layer.load(1, {
                                shade: [0.5, '#fff'] //透明度的白色背景
                            });
                            var type = typeof beforecall === 'function';

                            if (type) {

                                beforecall();
                            }

                        },

                        data: form.serialize(),
                        success: function (result, status, xhr) {
                            layer.close(index);
                            if (result.status === 1) {

                                var okfunc = typeof successfun === 'function';
                                if (okfunc) {
                                    successfun();
                                }

                                var indexs = msgbox.success(result.msg, function () {
                                    layer.close(indexs);
                                    if (!appcomm.isNull(redirecturl)) {
                                        location.href = redirecturl;
                                    }

                                });

                            } else {
                                var indef = msgbox.fail(result.msg, function () {

                                    layer.close(indef);
                                });
                            }

                            var type = typeof aftercall === 'function';
                            if (type) {
                                aftercall();
                            }

                        },
                        error: function (xhr, status, error) {
                            layer.close(index);
                            var type = typeof aftercall === 'function';
                            if (type) {
                                aftercall();
                            }
                            var fff = msgbox.fail("请求错误 " + status, function () {
                                layer.close(fff);
                            });
                        }
                    });
                });
            }
            return false;
        });

    },
    buttonsubmit: function (form, actionurl, beforecall, redirecturl, aftercall, successfun) {
        var index = 0;
        if (form.valid()) {
            //询问框
            layer.confirm('确认执行该操作吗？', {
                btn: ['确认', '取消'] //按钮
            }, function () {

                $.ajax({
                    url: actionurl,
                    type: "post",
                    beforeSend: function (XHR) {
                        index = layer.load(1, {
                            shade: [0.5, '#fff'] //透明度的白色背景
                        });
                        var type = typeof beforecall === 'function';
                        if (type) {
                            beforecall();
                        }

                    },

                    data: form.serialize(),
                    success: function (result, status, xhr) {
                        layer.close(index);
                        if (result.status === 1) {

                            var okfunc = typeof successfun === 'function';
                            if (okfunc) {
                                successfun();
                            }
                            var indexsu = msgbox.success(result.msg, function () {

                                layer.close(indexsu);
                                if (!appcomm.isNull(redirecturl)) {
                                    location.href = redirecturl;
                                }

                            });

                        } else {
                            var indesf = msgbox.fail(result.msg, function () {

                                layer.close(indesf);
                            });
                        }

                        var type = typeof aftercall === 'function';
                        if (type) {
                            aftercall();
                        }

                    },
                    error: function (xhr, status, error) {
                        layer.close(index);
                        var type = typeof aftercall === 'function';
                        if (type) {
                            aftercall();
                        }
                        var ff = msgbox.fail("请求错误 " + status, function () {
                            layer.close(ff);
                        });
                    }
                });
            });
        }
    },

    ajaxSubmit: function (actionurl, submitdata, beforecall, aftercall, successfun) {
        var index = 0;
        $.ajax({
            url: actionurl,
            type: "post",
            beforeSend: function (XHR) {
                index = layer.load(1, {
                    shade: [0.5, '#fff'] //透明度的白色背景
                });
                var type = typeof beforecall === 'function';

                if (type) {
                    beforecall();
                }

            },

            data: submitdata,
            success: function (result, status, xhr) {
                layer.close(index);
                if (result.status === 1) {

                    var okfunc = typeof successfun === 'function';
                    if (okfunc) {
                        successfun();
                    }

                    var indexm = msgbox.success(result.msg, function () {

                        var type = typeof aftercall === 'function';
                        if (type) {
                            aftercall();
                        }
                        layer.close(indexm);
                    });

                } else {
                    var indexmf = msgbox.fail(result.msg, function () {

                        var type = typeof aftercall === 'function';
                        if (type) {
                            aftercall();

                        }
                        layer.close(indexmf);
                    });
                }



            },
            error: function (xhr, status, error) {
                layer.close(index);
                var type = typeof aftercall === 'function';
                if (type) {
                    aftercall();
                }
                var inmmf = msgbox.fail("请求错误 " + status, function () {
                    layer.close(inmmf);
                });
            }
        });
    }

};