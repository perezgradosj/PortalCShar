$(document).ready(function () {
    
    var refresh_auto = function () {
        var aleatorio = "";
        var parametros = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        for (var x = 0; x < 5; x++) {
            var rand = Math.floor(Math.random() * parametros.length);
            aleatorio += parametros.substr(rand, 1);
        }
        //return aleatorio;
        $("#id-captcha").val(aleatorio);
    }
    refresh_auto();

    //Evento para el boton refresh
    var refresh = function (e) {
        e.preventDefault();
        //alert("refresh");

        var aleatorio = "";
        var parametros = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        for (var x = 0; x < 5; x++) {
            var rand = Math.floor(Math.random() * parametros.length);
            aleatorio += parametros.substr(rand, 1);
        }
        //return aleatorio;
        $("#id-captcha").val(aleatorio);
    };
    $('#refresh').click(refresh);

    var descarga_documento = function (e) {
        if ($("#form-descarga-documentos")[0].checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
            $("#form-descarga-documentos").addClass('was-validated');
        }
        else {

            e.preventDefault();

            var id_input_captcha = $("#id-input-captcha").val();
            var id_captcha = $("#id-captcha").val();
            if (id_input_captcha !== id_captcha) {
                $("#id-alerta-captcha").css("display", "block");
                $("#id-input-captcha").focus();
            }
            else {
                $("#id-alerta-captcha").css("display", "none");

                
                var button_value = $(this).val();
                if (button_value == ".xml")
                    $("#hidden").val("xml");
                else
                    $("#hidden").val("pdf");

                var formulario = $("#form-descarga-documentos").serialize();                

                var ruc = "20302241598";
                var tipo_documento = $("#id-tipo-documento").val();
                var serie = $("#id-serie").val();
                var ndocumento = $("#id-ndocumento").val();
            
                var documento = ruc + "-" + tipo_documento + "-" + serie + "-" + ndocumento;

                $.ajax({                    
                    type: "POST",
                    url: "/Facturacion/Index",
                    dataType: "json",
                    data: formulario,
                    success: function (result) {
                        console.log(result);
                        if (result == true) {
                            //Descargo el documento
                            var save = document.createElement('a');

                            save.href = "../MisDocumentos/" + documento + button_value;

                            save.download = documento;
                            var clicEvent = new MouseEvent('click', {
                                'view': window,
                                'bubbles': true,
                                'cancelable': true
                            });
                            save.dispatchEvent(clicEvent);
                            (window.URL || window.webkitURL).revokeObjectURL(save.href);
                        }
                        else {
                            //Informo que no existe el documento en bd
                            alert("No existe documento solicitado");
                        }
                    }
                });
            }
        }
    }
    $("#btn-xml").click(descarga_documento);
    $("#btn-pdf").click(descarga_documento);
})