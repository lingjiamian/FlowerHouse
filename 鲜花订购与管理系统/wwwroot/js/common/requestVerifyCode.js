//请求手机验证码的js代码

; (function ($) {
    /**
     * 禁用按钮并设置倒计时   
     * @param {jqObj} jqElem 禁用发送验证码按钮
     */
    function timekeeping(jqElem) {
        var time = 60;
        jqElem.attr("disabled", "disabled");
        var interval = setInterval(function () {
            jqElem.val(time + '秒后可获取');
            if (time === 0) {
                clearInterval(interval);
                jqElem.removeAttr("disabled");
                jqElem.val("重新发送");
            }
            time--;
        }, 1000);
    }

    
    $.requestPhoneCode = requestPhoneCode;

    /**
     * 请求手机验证码
     * @param {any} val 手机号
     * @param {jqObj} jqGetCodeBtn 按钮的jqeury对象，比如$(this)   发送验证码的按钮，点击后需要禁用该按钮并设置倒计时
     */
    function requestPhoneCode(val, url, jqGetCodeBtn) {
        var data = {};
        data['phone'] = val;
        $.ajax({
            url: url,
            method: 'POST',
            data: data
        }).done(function (data) {
            //如果status不为0，则请求成功
            if (data.status) {
                layer.msg(data.msg, { icon: 1 });
                timekeeping(jqGetCodeBtn);
            }
            else {
                layer.msg(data.msg, { icon: 5 });
            }
        });
    };
})(jQuery);