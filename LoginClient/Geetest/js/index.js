var handler = function (captchaObj) {
    window.captchaObj = captchaObj;
    captchaObj.appendTo('#captcha');
    captchaObj.onReady(function () {
        $("#wait").hide();
    });
    $('#btn-result').click(function () {
        var result = captchaObj.getValidate();
        if (!result) {
            return alert('请完成验证');
        }
        $('#validate').val(result.geetest_validate);
        $('#seccode').val(result.geetest_seccode);
    });
    captchaObj.onSuccess(function () {
        $('#btn-result').click();
        $('#ipt_submit').removeAttr('disabled');
    });
    // 更多前端接口说明请参见：http://docs.geetest.com/install/client/web-front/
};

$('#btn-gen').click(function () {
    $('#text').hide();
    $('#wait').show();
    var gt = $('#gt').val();
    var challenge = $('#challenge').val();
    // 调用 initGeetest 进行初始化
    // 参数1：配置参数
    // 参数2：回调，回调的第一个参数验证码对象，之后可以使用它调用相应的接口
    initGeetest({
        // 以下 4 个配置参数为必须，不能缺少
        gt: gt,
        challenge: challenge,
        offline: false, // 表示用户后台检测极验服务器是否宕机
        new_captcha: true, // 用于宕机时表示是新验证码的宕机

        product: "float", // 产品形式，包括：float，popup
        width: "300px",
        https: true

        // 更多前端配置参数说明请参见：http://docs.geetest.com/install/client/web-front/
    }, handler);
});

window.onload = function () {
    $('#btn-gen').click();
}