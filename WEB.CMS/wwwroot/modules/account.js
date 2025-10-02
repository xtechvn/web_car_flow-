var _login = {
    Token: '',
    DotText:'',
    OnLoggingText: '<i class="fa fa-sign-in"></i>Đang đăng nhập',
    LoginSucess: false,
    Initialization: function () {
        $('#user-name').keyup(function (e) {
            $('#validate-username').html('')
            $('#validate-general').html('')
        });
        $('#password').keyup(function (e) {
            $('#validate-password').html('')
            $('#validate-general').html('')
        });
    },

    Login: function () {
        _login.LoopDisplayLoading()
        $('.btn-login').attr('disabled', true);
        var model = {
            UserName: $('#user-name').val(),
            Password: $('#password').val(),
            RememberMe: false,
            ReturnUrl: $('#form-login').attr('data-url')
        }
        if ($('#remember-me').is(":checked")) {
            model.RememberMe=true
        }
        if (model.UserName == undefined || model.UserName.trim() == '') {
            $('#validate-username').html('Tài khoản không được để trống, vui lòng thử lại')
            return false;
        }
        if (model.Password == undefined || model.Password.trim() == '') {
            $('#validate-password').html('Mật khẩu không được để trống, vui lòng thử lại')
            return false;
        }
        $.ajax({
            url: "ConfirmLogin",
            type: "post",
            data: { model: model },
            success: function (result) {
                _login.LoginSucess = true
                $('.btn-login').removeAttr('disabled');
                if (result != undefined && result.status == 0) {
                    window.location.href = result.direct;
                }
                else {
                    $('#validate-general').html(result.msg);
                    $('.img_loading_summit').remove()

                }

            }
        });
    },
    LoopDisplayLoading: function () {
        setTimeout(function () {
            if (_login.DotText.trim() == '...') _login.DotText = ''
            $('.btn-login').html(_login.OnLoggingText + _login.DotText)
            _login.DotText = _login.DotText + '.'
            if (!_login.LoginSucess) _login.LoopDisplayLoading()
            else {
                $('.btn-login').html('<i class="fa fa-sign-in"></i>Đăng nhập');
            }
        }, 1000)
    },

}
const express = require('express');
const fetch = require('node-fetch');
const app = express();

app.get('/api/tts', async (req, res) => {
    const text = req.query.text || 'Xin chào';
    const url = `https://translate.google.com/translate_tts?ie=UTF-8&tl=vi&client=tw-ob&q=${encodeURIComponent(text)}`;

    const response = await fetch(url, {
        headers: {
            'User-Agent': 'Mozilla/5.0'
        }
    });

    if (!response.ok) {
        return res.status(500).send("Google TTS error");
    }

    res.set('Content-Type', 'audio/mpeg');
    response.body.pipe(res);
});

app.listen(3000, () => console.log('Server chạy ở http://localhost:3000'));