$(document).ready(function () {

    var consulta_documento = function (e) {
        if ($(this)[0].checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
            $(this).addClass('was-validated');
        }
        else {
            e.preventDefault();

            var formulario = $("#form-consulta-documento").serialize();

            $.ajax({
                type: "POST",
                url: "/Facturacion/ConsultaDocumento",
                dataType: "json",
                data: formulario,
                success: function (result) {
                    console.log(result);
                    $('#lista-documentos').empty();
                    $("#myPager").empty();
                    $("#total-registros").html("");

                    if (result.length == 0) {
                        //$("#id-alerta-info").css("display", "block");
                        //$("#id-alerta-info").html("No se encontraron registros.");                        
                        alert("No se encontraron registros");                        
                    }
                    else {
                        //$("#id-alerta-info").css("display", "block");
                        //$("#id-alerta-info").html("Fueron encontrados " + result.length + " registros.")
                        $("#total-registros").html("Total registros: " + result.length);

                        $.each(result, function (key, rs) {
                            var data_fecha = rs.fechaemision.split(" ");

                            $('#lista-documentos').append
                                ("<tr class='tr-dinamico'>" +
                                  "<td><input class='form-check-input select' type='checkbox' value='" + rs.num_cpe + "' style='position:relative;left:11px;top:4px'></td>" +
                                  "<td><button value='" + rs.num_cpe + ".xml' class='btn btn-outline-dark btn-sm class-download'><span class='oi oi-data-transfer-download'></span></button></td>" +
                                  "<td><button value='" + rs.num_cpe + ".pdf' class='btn btn-outline-dark btn-sm class-download'><span class='oi oi-data-transfer-download'></span></button></td>" +
                                  "<td>" + rs.seriecorrelativo + "</td>" +
                                  "<td>" + rs.des_tipodocumento + "</td>" +
                                  "<td>" + data_fecha[0] + "</td>" +
                                  "<td>" + rs.montototal + "</td>" +
                                "</tr>");
                        });
                        $('#tabla').pageMe({ pagerSelector: '#myPager', showPrevNext: true, hidePageNumbers: false, perPage: 4 });
                    }
                }
            });
        }
    }

    $("#form-consulta-documento").submit(consulta_documento);

    //PAGINACION
    $.fn.pageMe = function (opts) {
        var $this = this,
            defaults = {
                perPage: 7,
                showPrevNext: false,
                hidePageNumbers: false
            },
            settings = $.extend(defaults, opts);

        var listElement = $this.find('tbody');
        var perPage = settings.perPage;
        var children = listElement.children();
        var pager = $('.pager');

        if (typeof settings.childSelector != "undefined") {
            children = listElement.find(settings.childSelector);
        }

        if (typeof settings.pagerSelector != "undefined") {
            pager = $(settings.pagerSelector);
        }

        var numItems = children.size();
        var numPages = Math.ceil(numItems / perPage);

        pager.data("curr", 0);

        if (settings.showPrevNext) {
            $('<li class="page-item"><a href="#" class="prev_link page-link">Antes</a></li>').appendTo(pager);
        }

        var curr = 0;
        while (numPages > curr && (settings.hidePageNumbers == false)) {
            $('<li class="page-item"><a href="#" class="page_link page-link">' + (curr + 1) + '</a></li>').appendTo(pager);
            curr++;
        }

        if (settings.showPrevNext) {
            $('<li class="page-item"><a href="#" class="next_link page-link">Siguiente</a></li>').appendTo(pager);
        }

        pager.find('.page_link:first').addClass('active');
        pager.find('.prev_link').hide();
        if (numPages <= 1) {
            pager.find('.next_link').hide();
        }
        pager.children().eq(1).addClass("active");

        children.hide();
        children.slice(0, perPage).show();

        pager.find('li .page_link').click(function () {
            var clickedPage = $(this).html().valueOf() - 1;
            goTo(clickedPage, perPage);
            return false;
        });
        pager.find('li .prev_link').click(function () {
            previous();
            return false;
        });
        pager.find('li .next_link').click(function () {
            next();
            return false;
        });

        function previous() {
            var goToPage = parseInt(pager.data("curr")) - 1;
            goTo(goToPage);
        }

        function next() {
            goToPage = parseInt(pager.data("curr")) + 1;
            goTo(goToPage);
        }

        function goTo(page) {
            var startAt = page * perPage,
                endOn = startAt + perPage;

            children.css('display', 'none').slice(startAt, endOn).show();

            if (page >= 1) {
                pager.find('.prev_link').show();
            }
            else {
                pager.find('.prev_link').hide();
            }

            if (page < (numPages - 1)) {
                pager.find('.next_link').show();
            }
            else {
                pager.find('.next_link').hide();
            }

            pager.data("curr", page);
            pager.children().removeClass("active");
            pager.children().eq(page + 1).addClass("active");

        }
    };

    //Boton download
        $("#lista-documentos").on("click", ".class-download", function () {
            //alert("Soy boton XML con num_cpe= "+$(this).val());
            var var_num_cpe = $(this).val();
            var documento = var_num_cpe.split(".");

            $.ajax({
                type: "POST",
                url: "/Facturacion/DescargaDocumento",
                data: { num_cpe: var_num_cpe},
                dataType: "json",
                success: function (result) {
                    console.log(result);

                    var save = document.createElement('a');
                    save.href = "../MisDocumentos/" + var_num_cpe;
                    save.download = documento[0];
                    var clicEvent = new MouseEvent('click', {
                        'view': window,
                        'bubbles': true,
                        'cancelable': true
                    });
                    save.dispatchEvent(clicEvent);
                    (window.URL || window.webkitURL).revokeObjectURL(save.href);
                }
            });
        });

    //Checkbox

        var arreglo = [];

        $("#lista-documentos").on("click", ".select", function () {
            //alert("Soy CHECKBOX con num_cpe= "+$(this).val());

            var num_cpe = $(this).val();
            var tipo = $(this).parents("tr")[0].cells[4].innerHTML
            var nro_documento = $(this).parents("tr")[0].cells[3].innerHTML
            var fecha = $(this).parents("tr")[0].cells[5].innerHTML
            var monto = $(this).parents("tr")[0].cells[6].innerHTML


            var contador = 0;
            for (var i = 0; i < arreglo.length; i++) {

                if (arreglo[i].Num_Cpe == num_cpe) {
                    contador++;
                    arreglo.splice(i, 1);
                }
            }
            if (contador == 0) {
                var data = {};
                data.num_cpe = num_cpe;
                data.des_tipodocumento = tipo;
                data.seriecorrelativo = nro_documento;
                data.fechaemision = fecha;
                data.monto = monto;

                arreglo.push(data);
            }
        });

    //Enviar correo
        var enviocorreo = function (e) {

            e.preventDefault();

            if (arreglo.length == 0) {
                alert("Seleccione documentos a enviar.");
                $("#mensaje").css("display", "none");
            }
            else {
                $("#mensaje").css("display", "none");
                var var_correo = $("#txt-correo").val();
                $.ajax({
                    type: "POST",
                    url: "/Facturacion/EnvioCorreo",
                    data: { correo: var_correo, array: arreglo },
                    dataType: "json",
                    beforeSend: function (event, xhr, settings) {
                        $("#txt-correo").attr("disabled", "disabled");
                        $("#btn-enviar").attr("disabled", "disabled");
                        $("#btn-enviar").html("Enviando...");
                    },
                    success: function (result) {
                        $("#txt-correo").attr("disabled", false);
                        $("#btn-enviar").attr("disabled", false);
                        $("#btn-enviar").html("Enviar");
                        console.log(result);

                        if (result == true) {
                            $("#mensaje").css("display", "block");
                            $("#mensaje").css("color", "green");
                            $("#mensaje").val("Documentos enviados exitosamente.");
                        }
                        else {
                            $("#mensaje").css("display", "block");
                            $("#mensaje").css("color", "red");
                            $("#mensaje").val("Ocurrió un error en el envió");
                        }
                    }
                });
            }            
        }
        $("#form-correo").submit(enviocorreo);


    //Boton guardar clave o contraseña

        var form_modificar_datos = function (e) {
            var correo_nuevo = $("#txt-correonuevo").val();
            var clave_nueva = $("#txt-clavenueva").val();

            if (correo_nuevo == "" && clave_nueva == "") {
                e.preventDefault();
                $("#form-modifica-datos").removeClass('was-validated');
            }
                
            else if (correo_nuevo != "") {
                if ($("#form-modifica-datos")[0].checkValidity() === false) {
                    e.preventDefault();
                    e.stopPropagation();
                    $("#form-modifica-datos").addClass('was-validated');
                }
                else {
                    var formulario = $("#form-modifica-datos").serialize();
                    $.ajax({
                        type: "POST",
                        url: "/Usuario/ModificaDatos",
                        dataType: "json",
                        data: formulario,
                        success: function (result) {
                            console.log(result);
                            $("#alert-modificadatos").css("display", "block");
                            $("#alert-modificadatos").html(result);
                        }
                    });
                }
            }
            else {
                var formulario = $("#form-modifica-datos").serialize();
                $("#form-modifica-datos").removeClass('was-validated');
                $.ajax({
                    type: "POST",
                    url: "/Usuario/ModificaDatos",
                    dataType: "json",
                    data: formulario,
                    success: function (result) {
                        console.log(result);
                        $("#alert-modificadatos").css("display", "block");
                        $("#alert-modificadatos").html(result);
                    }
                });
            }
            
        }
        $("#btn-guardar-cambios").click(form_modificar_datos);

    //Limpia modal
        var limpia_modal = function () {
            $("#txt-correonuevo").val("");
            $("#txt-clavenueva").val("");
            $("#alert-modificadatos").css("display", "none");
        }
        $("#modal-cambiodatos").click(limpia_modal);
});