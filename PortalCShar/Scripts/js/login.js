$(document).ready(function () {

    //form login
    var form_login = function (e) {
        if ($(this)[0].checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
            $(this).addClass('was-validated');
        }
        else {
            e.preventDefault();

            var formulario = $("#form-login").serialize();

            $.ajax({
                type: "POST",
                url: "/Usuario/Login",
                dataType: "json",
                data: formulario,
                success: function (result) {
                    console.log(result);
                    if (result == false) {
                        $("#id-error").css("display", "block");
                        $("#id-error").html("Usuario o clave incorrectos");
                    } else {
                        document.location = "../Facturacion/ConsultaDocumento";
                    }
                }
            });
        }
    }
    $("#form-login").submit(form_login);

    //form registro
    var form_registro = function (e) {
        if ($(this)[0].checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
            $(this).addClass('was-validated');
        }
        else {
            e.preventDefault();

            var formulario = $("#form-registro").serialize();

            $.ajax({
                type: "POST",
                url: "/Usuario/Registro",
                dataType: "json",
                data: formulario,
                success: function (result) {
                    console.log(result);
                    if (result == true) {
                        $("#id-alerta-exito").css("display", "none");
                        $("#id-alerta-error").css("display", "block");
                        $("#id-alerta-error").html("El ruc/dni ingresado ya se encuentra registrado");
                    }
                    else {
                        $("#id-alerta-error").css("display", "none");
                        $("#id-alerta-exito").css("display", "block");
                        $("#id-alerta-exito").html(result);
                    }                    
                }
            });
        }
    }
    $("#form-registro").submit(form_registro);

    $("#btn-reset").click(function () {
        $("#id-alerta-exito").css("display", "none");
        $("#id-alerta-error").css("display", "none");
    });

});