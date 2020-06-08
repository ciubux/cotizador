jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición del mensaje; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';


    $(document).ready(function () {
        $("#btnBusquedaMensaje").click();
        $("#btnBandejaMensaje").click();
        verificarSiExisteMensaje();
        cargarChosenUsuarioMensaje();
        cargarTooltipLista();
    });

    $.datepicker.regional['es'] = {
        closeText: 'Cerrar',
        prevText: '< Ant',
        nextText: 'Sig >',
        currentText: 'Hoy',
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
        dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
        dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
        dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };

    $.datepicker.setDefaults($.datepicker.regional["es"]);

    var fechaInicio = $("#mensaje_fechaInicioMensaje").val();

    var fechaCreacionDesde = $("#mensaje_fechaCreacionDesde").val();
    var fechaCreacionHasta = $("#mensaje_fechaCreacionHasta").val();

    var fechaVencimientoHastaDesde = $("#mensaje_fechaVencimientoDesde").val();
    var fechaVencimientoHastaHasta = $("#mensaje_fechaVencimientoHasta").val();

    var fechaRecibidosDesde = $("#mensaje_fechaMensajeEntradaDesde").val();
    var fechaRecibidosHasta = $("#mensaje_fechaMensajeEntradaHasta").val();


    var fechaVencimientoedit = $("#mensaje_fechaVencimientoMensaje_edit").val();



    $("#mensaje_fechaMensajeEntradaDesde").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaRecibidosDesde);
    $("#mensaje_fechaMensajeEntradaHasta").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaRecibidosHasta);



    $("#mensaje_fechaVencimientoMensaje_edit").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaVencimientoedit);

    $("#mensaje_fechaVencimientoDesde").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaVencimientoHastaDesde);

    $("#mensaje_fechaVencimientoHasta").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaVencimientoHastaHasta);

    $("#mensaje_fechaInicioMensaje").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaInicio);

    $("#mensaje_fechaCreacionDesde").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaCreacionDesde);

    $("#mensaje_fechaCreacionHasta").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaCreacionHasta);


    $("#btnEnviarMensaje").click(function () {

        if ($("#idMensaje").val() == "00000000-0000-0000-0000-000000000000") {
            crearMensaje();
        }
        else {
            editarMensaje();
        }

    });

    function verificarSiExisteMensaje() {

        if ($("#idMensaje").val() != "00000000-0000-0000-0000-000000000000") {
            $("#btnEnviarMensaje").html('Finalizar Edición');
        }
        else {
            $("#btnEnviarMensaje").html('Finalizar Creación');
        }

    }

    ActulizarMensaje();
    /*
    setInterval(function ()
    {       
        $('.ModalMensajeAlerta').modal('hide');          
        $('div').removeClass('modal-backdrop fade in');
        $('.ModalMensajeAlerta').remove(); 
        ActulizarMensaje();
    }, 5000);
    */

    function validacionMensaje() {
        if ($("input:checkbox:checked").length == 0 && $("#idUsuarioBusquedaMensaje").val().length == 0) {
            $.alert({
                title: "Rol o Usuario Inválido",
                type: 'orange',
                content: 'Debe ingresar al menos un Rol o un Usuario para enviar el mensaje.',
                buttons: {
                    OK: function () { $('input:checkbox:checked').focus(); }
                }
            });
            return false;
        }

        if ($("#titulo").val() == "" || $("#titulo").val() == null) {
            $.alert({
                title: "Titulo Inválido",
                type: 'orange',
                content: 'Debe ingresar un titulo válido.',
                buttons: {
                    OK: function () { $('#titulo').focus(); }
                }
            });
            return false;
        }

        if ($("#txtMensaje").val() == "" || $("#txtMensaje").val() == null) {
            $.alert({
                title: "Mensaje Inválido",
                type: 'orange',
                content: 'Debe ingresar un mensaje válido.',
                buttons: {
                    OK: function () { $('#txtMensaje').focus(); }
                }
            });
            return false;
        }

        var fechaVencimiento = $("#mensaje_fechaVencimientoMensaje_edit").val();
        var fechaInicio = $("#mensaje_fechaInicioMensaje").val();

        function validate_fechaMayorQue(fechaInicial, fechaFinal) {
            valuesStart = fechaInicial.split("/");
            valuesEnd = fechaFinal.split("/");

            var dateStart = new Date(valuesStart[2], (valuesStart[1] - 1), valuesStart[0]);
            var dateEnd = new Date(valuesEnd[2], (valuesEnd[1] - 1), valuesEnd[0]);
            if (dateStart > dateEnd) {
                return 0;
            }
            return 1;
        }


        if (validate_fechaMayorQue(fechaInicio, fechaVencimiento) == 0) {

            $.alert({
                title: "Fecha Vencimiento Inválida",
                type: 'orange',
                content: 'Debe ingresar una fecha posterior o igual a la de inicio.',
                buttons: {
                    OK: function () { $('#mensaje_fechaVencimientoMensaje').focus(); }
                }
            });
            return false;
        }

        return true;

    }


    $("#txtMensaje").change(function () {

        changeInputString("mensaje", $("#txtMensaje").val());
    });

    $("#titulo").change(function () {
        changeInputString("titulo", $("#titulo").val());
    });

    $("#vendedor_mensaje_alta").change(function () {
        changeInputString("mensaje", "Alta");
    });

    $("#vendedor_mensaje_alta").change(function () {
        changeInputString("mensaje", "Normal");
    });

    function crearMensaje() {

        if (!validacionMensaje())
            return false;

        $('body').loadingModal({
            text: 'Creando Mensaje...'
        });
        $.ajax({
            url: "/Mensaje/Create",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {

                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear el mensaje.',
                    type: 'red',
                    buttons: {
                        OK: function () {

                            window.location = '/Mensaje/Editar';
                        }
                    }
                });
            },
            success: function (resultado) {

                $.alert({
                    title: TITLE_EXITO,
                    content: 'El mensaje se creó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Mensaje/Lista';
                        }
                    }
                });
            }
        });

    }

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Mensaje/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    $("#btnLimpiarBusquedaMensaje").click(function () {
        $.ajax({
            url: "/Mensaje/Limpiar",
            type: 'POST',
            success: function () {
                var a = $('#mensaje_bandeja_enviados').prop('checked');
                location.reload();
                if (a == true) {
                    $('#mensaje_bandeja_enviados').click();
                }
            }
        });
    });

    $("#btnLimpiarMensaje").click(function () {
        $.ajax({
            url: "/Mensaje/Limpiar",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });

    $(".chk-rol.MensajeEditar").change(function () {
        var valor = 0;
        if ($(this).prop("checked")) {
            valor = 1;
        }
        changeInputPermiso($(this).attr("id"), valor);
    });

    function changeInputPermiso(rol, valor) {
        $.ajax({
            url: "/Mensaje/ChangeRol",
            type: 'POST',
            data: {
                rol: rol,
                valor: valor
            },
            success: function () { }
        });
    }
    
    $(".navbar-header").on("click", "a.btnModal", function () {
        $('.ModalMensajeAlerta').first().modal('show');    
    });
    
    function ActulizarMensaje() {

        var idUsuario = $('#usuario_idUsuario').val();

        if (idUsuario != "00000000-0000-0000-0000-000000000000") {

            $.ajax({
                url: "/Mensaje/ShowMensaje",
                type: 'POST',
                dataType: 'JSON',
                data: {
                    idUsuario: idUsuario
                },
                success: function (list) {
                                       
                    if (list.length != 0) {                        
                        $('.btnModal').remove();
                        $("#imagenMP").before('<a data-notifications="' + list.length + '" class="btnModal" href="javascript:void()"></a>');

                        var verAutomaticamente = false;
                        var esVisible = $(".ModalMensajeAlerta").is(":visible");

                        for (var i = 0; i < list.length; i++) {
                            if (list[i].importancia == 'Alta') {
                                verAutomaticamente = true;
                            }

                            var numeroModal = i + 1;
                            
                            var imagenAdvertencia = list[i].importancia == "Alta" ? '<img src = "/images/advertencia.svg" style="vertical-align: middle; margin-bottom:5px;  margin-right:10px;" width = "20px" height = "20px">' +
                                '<svg width="1px" height="1px" xmlns="https://www.w3.org/2000/svg"></svg>' : "";
                            var date = new Date(list[i].fechaCreacionMensaje);
                            hora = (date.getHours() < 10 ? '0' : '') + date.getHours();
                            minuto = (date.getMinutes() < 10 ? '0' : '') + date.getMinutes();
                            segundo = (date.getSeconds() < 10 ? '0' : '') + date.getSeconds();
                            horaImprimible = hora + ":" + minuto + ":" + segundo;

                            var ItemRow =
                                '<div id="Mensaje' + numeroModal + '" class="modal fade ModalMensajeAlerta ModalMensaje" tabindex="-1" role="dialog">' +
                                '<div class="modal-dialog" id="MensajeDialog' + numeroModal + '">' +
                                '<div class="modal-content">' +
                                '<div class="modal-header">' +

                                '<div class="btn-group" style="float:right">' +
                                '<button  type="button"  class="Leido btn btn-primary ' + list[i].id_mensaje + ' "></button>' +
                                '</div><br><br>' +
                                '</div>' +
                                '<div class="modal-body">' +

                                '<h5> <b>DE: </b>' + list[i].usuario_creacion + ' - <b> FECHA: </b>' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaInicioMensaje)) + ' ' + horaImprimible + '</h5>' +

                                '<h4 style="display: inline-block;">' + imagenAdvertencia + '<u>' + list[i].titulo + '</u></h4>' +
                                '<p style="margin-bottom: 0.11in"><font size="3"><span lang="es-PE">' + list[i].mensaje + '</span></font></p>' +

                                '<textarea class="form-control" id ="respuesta' + numeroModal + '" rows="4" style="display:none" placeholder="Respuesta..."></textarea>' +


                                '<div class="row form-group" style="display:none" id="addUsuarios' + numeroModal + '"> ' +
                                '<label class="col-form-label col-md-3">Agregar destinatario(s):</label>' +
                                '<div class="col-md-9">' +
                                '<select multiple class="form-control form-control" id="idUsuarioBusquedaRespuestaMensajeModal' + numeroModal +'" name="UsuarioMensajeRapido" required>' +
                                '</select>' +
                                '</div>' +
                                '</div>' +


                                '<div class="btn-group" style="float:right">' +
                                '<button  type="button"  class="btn btn-success Responder" id="' + numeroModal + '"">Responder</button>' +
                                '</div>' +

                                '<div class="botonesRespuesta' + numeroModal + ' pull-right btn-group" style="display:none">' +

                                '<button   class="cerrarRespuesta btn btn-default" id="' + numeroModal + '">Cancelar</button>' +
                                '<button   class="EnviarRespuesta btn btn-primary ' + list[i].id_mensaje + '">Aceptar</button>' +
                                '</div>' +
                                '</div>' +

                                '<div class="VerContenedor modal-body" id="modalHistorialMensaje' + numeroModal + '">' +

                                '</div>' +


                                '</div>' +
                                '</div>';

                            var divs = $('.Leido');
                            var igual = false;
                            divs.each(function (index, element) {
                                var div = $(element).attr('class').split(" ");
                                if (div[3] == list[i].id_mensaje)
                                    return igual = true;
                            });  

                            if (igual == false) {
                                $("body").append(ItemRow); 
                            }   
                            
                        }
                        if ($('[id="Mensaje1"]').toArray().length == 2) {
                            location.reload();
                        }
                        
                        $('.Leido:not(:last)').html('Marcar como leído y mostrar siguiente');
                        $('.Leido:last').html('Marcar como leído');

                        if (verAutomaticamente && esVisible == false) {
                            setTimeout(function () {
                                $('.ModalMensajeAlerta').first().modal('show');
                            }, 2000);
                        }
                    } else {
                        $(".btnModal").remove();
                    }
                }
            });


        }
    }    
    
    $('body').on('show.bs.modal', '.ModalMensajeAlerta', function () {
        var arrayClass = $(this).find('.Leido').attr('class').split(" ");
        var idMensaje = arrayClass[3];
        var numeroModal = $(this).find('.Responder').attr('id');
        $.ajax({
            url: "/Mensaje/verHiloMensaje",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idMensaje: idMensaje
            },
            success: function (list) {
                $('#modalHistorialMensaje' + numeroModal + '').empty();
                for (var i = 0; i < list.length; i++) {

                    var date = new Date(list[i].fechaCreacionMensaje);
                    hora = (date.getHours() < 10 ? '0' : '') + date.getHours();
                    minuto = (date.getMinutes() < 10 ? '0' : '') + date.getMinutes();
                    segundo = (date.getSeconds() < 10 ? '0' : '') + date.getSeconds();
                    horaImprimible = hora + ":" + minuto + ":" + segundo;
                    var ultimoSalto = i + 1 == list.length ? '' : '<br>';
                    var a =

                        '<b><P STYLE="margin-bottom: 0.11in"><A NAME="_GoBack"></A><SPAN LANG="es-PE">' + list[i].user.nombre + '</SPAN></b><FONT SIZE=3><SPAN LANG="es-PE"> </SPAN></FONT> <FONT SIZE=2 > <SPAN LANG="es-PE">' +
                        '</SPAN></FONT> <FONT COLOR="#808080"><FONT SIZE=2><SPAN LANG="es-PE">'
                        + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaCreacionMensaje)) + ' ' + horaImprimible + '</SPAN></FONT></FONT > <FONT COLOR="#808080"><FONT SIZE=2 STYLE="font-size: 9pt"><SPAN LANG="es-PE">' +
                        '</SPAN></FONT></FONT>' +
                        '</P>' +
                        '<P STYLE="margin-bottom: 0.11in"><FONT SIZE=3><SPAN LANG="es-PE">' + list[i].mensaje + '' + '</SPAN></FONT></P>' +
                        ultimoSalto + '';

                    if (i > 3) {
                        $('[id="scrollRemober"]').remove();
                        $('#modalHistorialMensaje' + numeroModal + '').before('<br id="scrollRemober">');
                        $('#modalHistorialMensaje' + numeroModal + '').attr('style', 'height: 300px;overflow: auto;');
                    }
                    $('#modalHistorialMensaje' + numeroModal + '').append(a);
                }
            }
        });

    });

    $('body').on("click", "button.Responder", function () {
        var num = $(this).attr('id');
        $('#respuesta' + num + '').show();
        $('.botonesRespuesta' + num + '').show();
        $('#addUsuarios' + num + '').show();
        $('.Responder').hide();
        $('#respuesta' + num + '').after('<br id="saltoLinea">');
        $('#respuesta' + num + '').focus();
    });
        
        $('body').on("click", "button.EnviarRespuesta", function () {
            var num = $(this).prev('button').attr('id');
            var txtRespuesta = $('#respuesta' + num + '').val();
            var arrayClass = $(this).attr('class').split(" ");
            var idMensaje = arrayClass[3];
            var idUsuarios = $('#idUsuarioBusquedaRespuestaMensajeModal'+num+'').val();

            if (txtRespuesta.trim() === "") {
                $(this).closest('.modal-content').find('button.Leido').click();
            }
            else {
                
                $.ajax({
                    url: "/Mensaje/msnVistoRespuesta",
                    type: 'POST',                    
                    data: {
                        idUsuarios: idUsuarios,
                        idMensaje: idMensaje,
                        respuesta: txtRespuesta
                    },
                    success: function () {                       
                        $.alert({
                            title: TITLE_EXITO,
                            content: 'Mensaje enviado.',
                            type: 'green',
                            buttons: {
                                OK: function () {
                                    let dialog = $('.' + idMensaje + '').closest('.modal');
                                    var btnFinal = dialog.find('.Leido').html();
                                    if (btnFinal == "Marcar como leído") {
                                        dialog.modal('hide');
                                        location.reload();
                                    }
                                    else {
                                        dialog.modal('hide');
                                        dialog.next().modal('show');
                                    }
                                    $('.' + idMensaje + '').closest('#MensajeDialog').find('#modalRespuesta' + num + '').empty();
                                    $('#idUsuarioBusquedaRespuestaMensajeModal'+num+'').val('').trigger('chosen:updated');
                                    $('#respuesta' + num + '').empty;                                    
                                    $('#respuesta' + num + '').hide();
                                    $('.botonesRespuesta' + num + '').hide();
                                    $('#respuesta' + num + '').val("");
                                    $('.Responder').show();
                                    $('#Mensaje' + num + '').attr('class', '');
                                    ActulizarMensaje();
                                }
                            }
                        });
                    }, error(error) {
                        $.alert({
                            title: 'Error',
                            content: 'Se generó un error al intentar enviar la respuesta. | ' + error+'',
                            type: 'red',
                            buttons: {
                                OK: function () {


                                }
                            }
                        });
                        }
                });
            }
        });
    

    $('body').on("click", "button.cerrarRespuesta", function () {
        var num = $(this).attr('id');
        $('#respuesta' + num + '').hide();
        $('.botonesRespuesta' + num + '').hide();           
        $('#addUsuarios' + num + '').hide();
        $('#respuesta' + num + '').val(""); 
        $('#idUsuarioBusquedaRespuestaMensajeModal'+num+'').val('').trigger('chosen:updated');
        $('.Responder').show();
        $('#saltoLinea').remove();
    });

    $('body').on("click", "button.Leido", function () {
        var num = $(this).closest('.modal-content').find("button.Responder").attr('id');
        var arrrayClass = $(this).attr('class').split(" ");
        var idMensaje = arrrayClass[3];

        $.ajax({
            url: "/Mensaje/UpdateMensajeVisto",
            type: 'POST',
            data: {
                idMensaje: idMensaje
            },
            success: function () {

                let dialog = $('.' + idMensaje + '').closest('.modal');
                var btnFinal = dialog.find('.Leido').html();
                if (btnFinal == "Marcar como leído") {
                    dialog.modal('hide');
                    location.reload();
                }
                else {
                    dialog.modal('hide');
                    dialog.next().modal('show');
                }
                $('.Responder').show();
                $('#idUsuarioBusquedaRespuestaMensajeModal'+num+'').val('').trigger('chosen:updated');
                $('.' + idMensaje + '').closest('#MensajeDialog').find('#modalRespuesta' + num + '').empty();
                $('#Mensaje' + num + '').attr('class','');
                ActulizarMensaje();                
            }
        });
    });

    $("#mensaje_fechaInicioMensaje").change(function () {
        var fechaInicio = $("#mensaje_fechaInicioMensaje").val();
        $.ajax({
            url: "/Mensaje/ChangeFechaInicio",
            type: 'POST',
            data: {
                fechaInicio: fechaInicio
            },
            success: function () {
            }
        });
    });


    $("#mensaje_fechaVencimientoDesde").change(function () {
        var valor = $("#mensaje_fechaVencimientoDesde").val();
        if ($("#mensaje_fechaVencimientoHasta").val() == null || $("#mensaje_fechaVencimientoHasta").val() == undefined || $("#mensaje_fechaVencimientoHasta").val() == "") {
            $("#mensaje_fechaVencimientoHasta").val(valor);
        }
        var valor2 = $("#mensaje_fechaVencimientoHasta").val();
        ChangefechaVencimiento('fechaVencimientoMensajeDesde', valor);
        ChangefechaVencimiento('fechaVencimientoMensajeHasta', valor2);
        ChangefechaCreacion('fechaCreacionMensajeDesde', null);
        ChangefechaCreacion('fechaCreacionMensajeHasta', null);
        $("#mensaje_fechaCreacionDesde").val(null);
        $("#mensaje_fechaCreacionHasta").val(null);
    });

    $("#mensaje_fechaVencimientoHasta").change(function () {
        var valor = $("#mensaje_fechaVencimientoHasta").val();
        if ($("#mensaje_fechaVencimientoDesde").val() == null || $("#mensaje_fechaVencimientoDesde").val() == undefined || $("#mensaje_fechaVencimientoDesde").val() == "") {
            $("#mensaje_fechaVencimientoDesde").val(valor);
        }
        var valor2 = $("#mensaje_fechaVencimientoDesde").val();
        ChangefechaVencimiento('fechaVencimientoMensajeHasta', valor);
        ChangefechaVencimiento('fechaVencimientoMensajeDesde', valor2);
        ChangefechaCreacion('fechaCreacionMensajeDesde', null);
        ChangefechaCreacion('fechaCreacionMensajeHasta', null);
        $("#mensaje_fechaCreacionDesde").val(null);
        $("#mensaje_fechaCreacionHasta").val(null);
    });

    function ChangefechaVencimiento(propiedad, valor) {

        $.ajax({
            url: "/Mensaje/ChangeFechaVencimiento",
            type: 'POST',
            data: {
                fechaVencimiento: valor,
                propiedad: propiedad
            },
            success: function () {
            }
        });
    }

    $("#mensaje_fechaMensajeEntradaDesde").change(function () {
        var valor = $("#mensaje_fechaMensajeEntradaDesde").val();
        Changefecha('fechaMensajeEntradaDesde', valor);
    });

    $("#mensaje_fechaMensajeEntradaHasta").change(function () {
        var valor = $("#mensaje_fechaMensajeEntradaHasta").val();
        Changefecha('fechaMensajeEntradaHasta', valor);
    });


    $("#mensaje_fechaCreacionDesde").change(function () {
        var valor = $("#mensaje_fechaCreacionDesde").val();

        if ($("#mensaje_fechaCreacionHasta").val() == null || $("#mensaje_fechaCreacionHasta").val() == undefined || $("#mensaje_fechaCreacionHasta").val() == "") {
            $("#mensaje_fechaCreacionHasta").val(valor);
        }
        var valor2 = $("#mensaje_fechaCreacionHasta").val();
        ChangefechaCreacion('fechaCreacionMensajeDesde', valor);
        ChangefechaCreacion('fechaCreacionMensajeHasta', valor2);
        ChangefechaVencimiento('fechaVencimientoMensajeDesde', null);
        ChangefechaVencimiento('fechaVencimientoMensajeHasta', null);
        $("#mensaje_fechaVencimientoDesde").val(null);
        $("#mensaje_fechaVencimientoHasta").val(null);


    });

    $("#mensaje_fechaCreacionHasta").change(function () {
        var valor = $("#mensaje_fechaCreacionHasta").val();

        if ($("#mensaje_fechaCreacionDesde").val() == null || $("#mensaje_fechaCreacionDesde").val() == undefined || $("#mensaje_fechaCreacionDesde").val() == "") {
            $("#mensaje_fechaCreacionDesde").val(valor);
        }
        var valor2 = $("#mensaje_fechaCreacionDesde").val();
        ChangefechaCreacion('fechaCreacionMensajeHasta', valor);
        ChangefechaCreacion('fechaCreacionMensajeDesde', valor2);
        ChangefechaVencimiento('fechaVencimientoMensajeDesde', null);
        ChangefechaVencimiento('fechaVencimientoMensajeHasta', null);
        $("#mensaje_fechaVencimientoDesde").val(null);
        $("#mensaje_fechaVencimientoHasta").val(null);
    });

    function Changefecha(propiedad, valor) {

        $.ajax({
            url: "/Mensaje/ChangeFecha",
            type: 'POST',
            data: {
                fecha: valor,
                propiedad: propiedad
            },
            success: function () {
            }
        });
    }


    function ChangefechaCreacion(propiedad, valor) {

        $.ajax({
            url: "/Mensaje/ChangeFechaCreacion",
            type: 'POST',
            data: {
                fechaCreacion: valor,
                propiedad: propiedad
            },
            success: function () {
            }
        });
    }

    $("#mensaje_fechaVencimientoMensaje_edit").change(function () {
        var fechaVencimiento = $("#mensaje_fechaVencimientoMensaje_edit").val();

        $.ajax({
            url: "/Mensaje/ChangeFechaVencimientoEdit",
            type: 'POST',
            data: {
                fechaVencimiento: fechaVencimiento
            },
            success: function () {
            }
        });
    });

    $("#btnBusquedaTodosMensajes").click(function () {

        $("#btnBusquedaTodosMensajes").attr("disabled", "disabled");
        $.ajax({
            url: "/Mensaje/BusquedaMensaje",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusquedaTodosMensajes").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusquedaTodosMensajes").removeAttr("disabled");
                $("#tableMensaje > tbody").empty();
                $("#tableMensaje").footable({
                    "paging": {
                        "enabled": true
                    }
                });


                for (var i = 0; i < list.length; i++) {

                    var ItemRow = '<tr data-expanded="true">' +

                        '<td>  ' + list[i].id_mensaje + '  </td>' +
                        '<td>  ' + list[i].titulo + '  </td>' +
                        //'<td>  ' + list[i].mensaje + '  </td>' +
                        '<td>  ' + list[i].usuario_creacion + '  </td>' +
                        '<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaCreacionMensaje)) + '  </td>' +
                        '<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaInicioMensaje)) + '  </td>' +
                        '<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaVencimientoMensaje)) + '  </td>' +
                        '<td>' +
                        '<button type="button" style="margin-right:13px;" class="' + list[i].id_mensaje + ' btnEditarMensaje btn btn-primary" > Editar</button >' +
                        '<button type="button" class="' + list[i].id_mensaje + ' btnVerMensaje btn btn-success">Ver</button>' +

                        '</td>' +
                        '</tr>';

                    $("#tableMensaje").append(ItemRow);

                }
            }
        });
    });

    $("#btnBusquedaMensaje").click(function () {

        $("#btnBusquedaBandejaMensaje").attr("disabled", "disabled");
        $.ajax({
            url: "/Mensaje/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusquedaBandejaMensaje").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusquedaBandejaMensaje").removeAttr("disabled");
                $("#tableMensaje > tbody").empty();
                $("#tableMensaje").footable({
                    "paging": {
                        "enabled": true
                    }
                });


                for (var i = 0; i < list.length; i++) {
                    //var btnEditar = $("#mensaje_bandeja_enviados").prop('checked') ? '<button type = "button" style = "margin-right:13px;" class="' + list[i].id_mensaje + ' btnEditarMensaje btn btn-primary" >Editar</button >' : '';
                    var ItemRow = '<tr data-expanded="true">' +

                        '<td>  ' + list[i].id_mensaje + '  </td>' +
                        '<td>  ' + list[i].titulo + '  </td>' +
                        //'<td>  ' + list[i].mensaje + '  </td>' +
                        '<td>  ' + list[i].usuario_creacion + '  </td>' +
                        //'<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaCreacionMensaje)) + '  </td>' +
                        '<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaInicioMensaje)) + '  </td>' +
                        //'<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaVencimientoMensaje)) + '  </td>' +
                        '<td>' +
                        // btnEditar +
                        '<button type="button" class="' + list[i].id_mensaje + ' btnVerMensaje btn btn-success">Ver</button>' +

                        '</td>' +
                        '</tr>';

                    $("#tableMensaje").append(ItemRow);

                }
            }
        });
    });
    function eliminarDuplicadosUsuario(arrayIn) {

        var arrayOut = new Array();
        var unicos = arrayIn.filter(function (e) {
            return arrayOut[e.idUsuario] ? false : (arrayOut[e.idUsuario] = true);
        });
        return unicos;
    }
    $('body').on('click', "button.btnVerMensaje", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idMensaje = arrrayClass[0];
        $('#idMensajeRespuesta').val(idMensaje);
        $.ajax({
            url: "/Mensaje/verUsuariosRespuesta",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idMensaje: idMensaje
            },
            success: function (resultado) {
                resultado = eliminarDuplicadosUsuario(resultado);
                $('[id^=VerContenedor]').remove();
                $('#UsuariosRespuesta').empty();
                $('#UsuariosRespuesta').append('<option value=" ">Selecciona un Usuario</option>');
                $('#UsuariosRespuesta').prop("disabled", false);
                for (var i = 0; i < resultado.length; i++) {
                    //var a = i == 0 ? 'id="EliminarUsuario"' : '';
                    var listaUsuario = '<option value="' + resultado[i].idUsuario + '">' + resultado[i].nombre + '</option>';
                    $('#UsuariosRespuesta').append(listaUsuario);
                }
                $('#respuestasUsuarios').empty();
                $('[id^=respuestasUser]').remove();
                $('#Prueba2').remove();
                $('#modalListado').modal('show');


                if ($('#mensaje_bandeja_recibidos').prop('checked') == true) {

                    $('#UsuariosRespuesta>option').eq(1).prop('selected', true).change();
                    $('#UsuariosRespuesta').prop("disabled", true);
                }

                if ($('#mensaje_bandeja_enviados').prop('checked') == true) {
                    //$('#EliminarUsuario').remove();
                }                
            }
        });
    });

    $('body').on('change', "#UsuariosRespuesta", function () {

        var idUsuario = $('#UsuariosRespuesta').val();
        var idMensaje = $('#idMensajeRespuesta').val();
        if (idUsuario == " ") {
            $('[id^=respuestasUser]').remove();
            $('#Prueba2').remove();
            $('[id^=saltoEmilinar]').remove();
        }
        else {
            $.ajax({
                url: "/Mensaje/verRespuestaUsuario",
                type: 'POST',
                async: false,
                dataType: 'JSON',
                data: {
                    idMensaje: idMensaje,
                    idUsuario: idUsuario
                },
                success: function (list) {
                    $('[id^=respuestasUser]').remove();
                    $('[id^=saltoEmilinar]').remove();
                    $('#Prueba2').remove();


                    var imagenAdvertencia = list[list.length - 1].importancia == "Alta" ? '<img src = "/images/advertencia.svg" style="vertical-align: middle; margin-bottom:5px;  margin-right:10px;" width = "20px" height="20px">' +
                        '<svg width="1px" height="1px" xmlns="https://www.w3.org/2000/svg"></svg>' : "";

                    var titulo = '<h4 id="Prueba2" class="float-left">' + imagenAdvertencia + '<u>' + list[list.length - 1].titulo + '</u></h4>';

                    $('#Prueba').after(titulo);
                    $('[id^=VerContenedor]').remove();
                    $('#Prueba2').after('<div id="VerContenedor" class="VerContenedor">');

                    var arrayOut = new Array();
                    var unicos = list.filter(function (e) {
                        return arrayOut[e.user.nombre] ? false : (arrayOut[e.user.nombre] = true);
                    });

                    for (var i = 0; i < list.length; i++) {

                        var color2 = unicos[0].user.nombre == list[i].user.nombre ? 'style="background-color: #D6F4FF; border-style: solid; border-width: 2px; border-radius: 8px; border-color: #009FDA; padding-top: 0px; padding-bottom: 0px;padding-left: 10px;padding-right: 10px; "'
                            : 'style="padding-top: 0px; padding-bottom: 0px;padding-left: 10px;padding-right: 10px; background-color: #E0FFD6; border-style: solid; border-width: 2px; border-radius: 8px; border-color: #33D200;"';

                        var date = new Date(list[i].fechaCreacionMensaje);
                        hora = (date.getHours() < 10 ? '0' : '') + date.getHours();
                        minuto = (date.getMinutes() < 10 ? '0' : '') + date.getMinutes();
                        segundo = (date.getSeconds() < 10 ? '0' : '') + date.getSeconds();
                        horaImprimible = hora + ":" + minuto + ":" + segundo;
                        var salto = list.length - 1 == i ? '' : '<br id="saltoEmilinar">';
                        var espacio = unicos[0].user.nombre == list[i].user.nombre ?
                            '' :
                            '<div class="col-lg-5"></div>';
                        var a =
                            '<div class="row">' +
                            espacio +
                            '<div class=col-lg-7><div id = "respuestasUser" ' + color2 + ' >' +
                            '<b><P STYLE="margin-bottom: 0.11in;font-size:11px;"><SPAN LANG="es-PE">' + list[i].user.nombre + '</SPAN></b><FONT SIZE=3><SPAN LANG="es-PE"> </SPAN></FONT> <FONT SIZE=2 > <SPAN LANG="es-PE">' +
                            '</SPAN></FONT> <FONT COLOR="#808080"><SPAN LANG="es-PE">'
                            + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaCreacionMensaje)) + ' ' + horaImprimible + '</SPAN></FONT></FONT > <FONT COLOR="#808080"><FONT SIZE=2 STYLE="font-size: 9pt"><SPAN LANG="es-PE">' +
                            '</SPAN></FONT></P>' +
                            '<P STYLE="margin-bottom: 0.11in"><FONT SIZE=3><SPAN LANG="es-PE">' + list[i].mensaje + '' + '</SPAN></FONT></P>' +
                            '</div></div>' + espacio + '</div>' + salto;

                        $('#VerContenedor').append(a);
                        if (i > 3) {
                            $('#VerContenedor').attr('style', 'height:400px; overflow:auto;overflow-x: hidden;');
                        }
                    }
                }
            });
        }
    });

    $('body').on('click', "button.btnEditarMensaje", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idMensaje = arrrayClass[0];

        $.ajax({
            url: "/Mensaje/ConsultarSiExisteMensaje",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idMensaje: idMensaje
            },
            success: function (resultado) {

                if (resultado.existe == "false") {
                    $.ajax({
                        url: "/Mensaje/iniciarEdicionMensaje",
                        type: 'POST',
                        async: false,
                        error: function (detalle) {
                            alert("Ocurrió un problema al iniciar la edición del mensaje.");
                        },
                        success: function (fileName) {
                            window.location = '/Mensaje/Editar';
                            ConsultaMensajeLeido();
                        }
                    });
                }
                else {
                    if (resultado.idVendedor == 0) {
                        alert('Está creando un nuevo mensaje; para continuar por favor diríjase a la página "Crear/Modificar Vendedor" y luego haga clic en el botón Cancelar.');
                    }

                    else {
                        alert('Ya se encuentra editando un mensaje para continuar por favor dirigase a la página "Crear/Modificar Mensaje".');
                    }
                }
            }
        });
    });

    ConsultaMensajeLeido();
    function ConsultaMensajeLeido() {

        if (window.location.pathname == '/Mensaje/Editar') {
            var idMensaje = $('#idMensaje').val();

            $.ajax({
                url: "/Mensaje/ConsultarSiMensajeLeido",
                type: 'POST',
                async: true,
                dataType: 'JSON',
                data: {
                    idMensaje: idMensaje
                },
                success: function (resultado) {

                    if (resultado.leido == 1) {
                        alert('El mensaje no se puede modificar debido a que ya fue leido por un receptor');
                        $("#vendedor_importancia_si").prop('disabled', true);
                        $("#vendedor_importancia_no").prop('disabled', true);
                        $("#mensaje_fechaInicioMensaje").prop('disabled', true);
                        $("#mensaje_fechaVencimientoMensaje_edit").prop('disabled', true);
                        $("#titulo").prop('disabled', true);
                        $("#txtMensaje").prop('disabled', true);
                        $('#idUsuarioBusquedaMensaje').prop('disabled', true).trigger("chosen:updated");
                        $('[id*=rol_]').prop('disabled', true);
                        $('#btnEnviarMensaje').remove();

                    }
                }
            });
        }
    }


    $("#btnCancelarMensaje").click(function () {

        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/Mensaje/CancelarCreacionMensaje', null);
    });

    function ConfirmDialog(message, redireccionSI, redireccionNO) {
        $('<div></div>').appendTo('body')
            .html('<div><h6>' + message + '</h6></div>')
            .dialog({
                modal: true, title: 'Confirmación', zIndex: 10000, autoOpen: true,
                width: 'auto', resizable: false,
                buttons: {
                    Si: function () {
                        if (redireccionSI != null)
                            window.location = redireccionSI;
                        $(this).dialog("close");

                    },
                    No: function () {
                        if (redireccionNO != null)
                            window.location = redireccionNO;
                        $(this).dialog("close");
                    }
                },
                close: function (event, ui) {
                    $(this).remove();
                }
            });
        document.body.scrollTop = default_scrollTop;
    }

    $("#mensaje_estado_si").click(function () {
        var valCheck = 1;
        changeInputInt("estado", valCheck);
    });

    $("#mensaje_estado_no").click(function () {
        var valCheck = 0;
        changeInputInt("estado", valCheck);
    });


    $("#mensaje_bandeja_enviados").click(function () {
        var valCheck = 1;
        changeInputInt("bandeja", valCheck);
        location.reload();
    });

    $("#mensaje_bandeja_recibidos").click(function () {
        var valCheck = 0;
        changeInputInt("bandeja", valCheck);
        location.reload();
    });


    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/Mensaje/ChangeInputInt",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    function editarMensaje() {

        if (!validacionMensaje())
            return false;


        $('body').loadingModal({
            text: 'Editando Mensaje...'
        });
        $.ajax({
            url: "/Mensaje/UpdateMensaje",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar el mensaje.',
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });
            },
            success: function (resultado) {
                $('body').loadingModal('hide');

                $.alert({
                    title: TITLE_EXITO,
                    content: 'El mensaje se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Mensaje/Lista';
                        }
                    }
                });
            }
        });
    }



    function cargarChosenUsuarioMensaje() {


        $("#idUsuarioBusquedaMensaje").chosen({ placeholder_text: "Buscar Usuario", no_results_text: "No se encontró Usuario", allow_single_deselect: true }).on('chosen:showing_dropdown');


        $("#idUsuarioBusquedaMensaje").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 0,
            afterTypeDelay: 300,
            cache: false,
            url: "/Usuario/SearchUsuarios"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Usuario", no_results_text: "No se encontró Usuario" });
    }

    setTimeout(function () {
        $('body').on('click', '#MensajeRapidoMenu', function (event) {
            event.preventDefault();
            var url = $('#MensajeRapido').data('url');
            $("#MensajeRapidoDialog").load(url, function (data) { $("#MensajeRapido").modal("show"); });
        });
    }, 3000);


    $('body').on('change', '#mensaje_importancia_si_modal', function (event) {
        changeInputString("mensaje", "Alta");
    });
    $('body').on('change', '#mensaje_importancia_no_modal', function (event) {
        changeInputString("mensaje", "Normal");
    });
    $(document).on('change', '#tituloModal', function (event) {
        changeInputString("titulo", $("#tituloModal").val());
    });
    $(document).on('change', '#txtMensajeModal', function (event) {
        changeInputString("mensaje", $("#txtMensajeModal").val());
    });


    $('body').on('click', "#btnCancelarMensajeModal", function () {
        $("#MensajeRapido").modal("hide");
    });


    $('body').on('shown.bs.modal', '.ModalMensajeAlerta', function () {
        $('select[id^=idUsuarioBusquedaRespuestaMensajeModal]', this).chosen('destroy').chosen({ placeholder_text: "Buscar Usuario", no_results_text: "No se encontró Usuario", allow_single_deselect: true, width: '100%' }).on('chosen:showing_dropdown');
        $('select[id^=idUsuarioBusquedaRespuestaMensajeModal]', this).ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 3,
            afterTypeDelay: 300,
            cache: false,
            url: "/Usuario/SearchUsuarios"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Usuario", no_results_text: "No se encontró Usuario" });
    });

    $('body').on('shown.bs.modal', '#MensajeRapido', function () {
        cargarTooltipLista();
        $('#idUsuarioBusquedaMensajeModal', this).chosen('destroy').chosen({ placeholder_text: "Buscar Usuario", no_results_text: "No se encontró Usuario", allow_single_deselect: true }).on('chosen:showing_dropdown');
        $('#mensajeFechaInicioMensajeModal').datepicker().datepicker("setDate", new Date());
        $('#mensajeFechaVencimientoMensajeModal').datepicker().datepicker("setDate", '+7');
        ConteoUsuario();
        $('#idUsuarioBusquedaMensajeModal', this).ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 3,
            afterTypeDelay: 300,
            cache: false,
            url: "/Usuario/SearchUsuarios"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Usuario", no_results_text: "No se encontró Usuario" });
    });



    $(document).on('change', "#mensajeFechaInicioMensajeModal", function () {
        var fechaInicio = $("#mensajeFechaInicioMensajeModal").val();
        $.ajax({
            url: "/Mensaje/ChangeFechaInicio",
            type: 'POST',
            data: {
                fechaInicio: fechaInicio
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#mensajeFechaVencimientoMensajeModal", function () {
        var fechaVencimiento = $("#mensajeFechaVencimientoMensajeModal").val();

        $.ajax({
            url: "/Mensaje/ChangeFechaVencimientoEdit",
            type: 'POST',
            data: {
                fechaVencimiento: fechaVencimiento
            },
            success: function () {
            }
        });
    });


    $(document).on('change', ".RolMensajeModal", function () {
        var valor = 0;

        if ($(this).prop("checked")) {
            valor = 1;
        }

        changeInputPermiso($(this).attr("id"), valor);

        ConteoUsuario();

    });

    $(document).on('change', "#idUsuarioBusquedaMensajeModal", function () {
        var idUsuario = $("#idUsuarioBusquedaMensajeModal").val();

        $.ajax({
            url: "/Mensaje/ChangeUsuarioMensaje",
            type: 'POST',
            data: {
                idUsuario: idUsuario
            },
            success: function () {
            }
        });
        ConteoUsuario();
    });

    function eliminateDuplicados(arrayIn) {

        var arrayOut = new Array();
        var unicos = arrayIn.filter(function (e) {
            return arrayOut[e] ? false : (arrayOut[e] = true);
        });
        return unicos;
    }

    function ConteoUsuario() {
        var numUsuarios = $("#idUsuarioBusquedaMensajeModal").val();
        var listRol = new Array();

        var idRoles = $('.RolMensajeModal:checkbox:checked');
        if (idRoles.length != 0) {
            for (i = 0; i < idRoles.length; i++) {
                listRol.push($(idRoles[i]).val());
            }
        } else { listRol = 0; }
        AjaxNumeroUsuarios(listRol, numUsuarios);
    }
    function AjaxNumeroUsuarios(listRol, numUsuarios) {
        $.ajax({
            url: "/Rol/ListUsuariosRoles",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data:
            {
                ListRol: listRol
            },
            success: function (list) {

                for (i = 0; i < list.length; i++) {
                    numUsuarios.push(list[i].idUsuario);
                }
                numUsuarios = eliminateDuplicados(numUsuarios);
                numerofinal = numUsuarios.length;

                $('#etiquetaUsuariosModalNumero').remove();

                var etiqueta = '<div class="row form-group" id="etiquetaUsuariosModalNumero"> <label class="col-form-label col-md-3"></label> <div class="col-md-9"> <h5 class="text-danger"><b>Este mensaje se enviará a <span id="numerosa" style="position:absolute; font-size: 1em;" class="labelModificado label-danger">' + numerofinal + '</span><span class="labelModificado label-danger" id="numerosaShadow" style="font-size: 1em;">' + numerofinal + '</span> usuario(s)</b></h5></div></div>';
                etiqueta = numerofinal == 0 ? $('#etiquetaUsuariosModalNumero').remove() : etiqueta;
                $('#etiquetaRolesModal').before(etiqueta);


                var element = document.getElementById("numerosa");
                var elementShadow = document.getElementById("numerosaShadow");
                if (element != null && elementShadow != null) {
                    element.innerHTML = numerofinal;
                    elementShadow.innerHTML = numerofinal;
                    element.classList.remove("parpadea");

                    setTimeout(function () {
                        element.classList.add("parpadea");
                        setTimeout(function () {
                            element.classList.remove("parpadea");
                        }, 300);
                    }, 100);
                }
            }
        });
    }

    $(document).on('click', "#btnEnviarMensajeModal", function () {
        crearMensajeModal();
    });

    function crearMensajeModal() {
        if (!validacionMensajeModal())
            return false;

        event.preventDefault();
        $.ajax({
            url: "/Mensaje/Create",
            type: 'POST',
            error: function (error) {

                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar enviar el mensaje. | '+error+'',
                    type: 'red',
                    buttons: {
                        OK: function () {
                            $("#MensajeRapido").modal("hide");
                        }
                    }
                });
            },
            success: function () {

                $.alert({
                    title: TITLE_EXITO,
                    content: 'Mensaje enviado.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            $("#MensajeRapido").modal("hide");
                            ActulizarMensaje();
                        }
                    }
                });
            }
        });
    }

    function validacionMensajeModal() {
        if ($("input:checkbox:checked").length == 0 && $("#idUsuarioBusquedaMensajeModal").val().length == 0) {
            $.alert({
                title: "Rol o Usuario Inválido",
                type: 'orange',
                content: 'Debe ingresar al menos un Rol o un Usuario para enviar el mensaje.',
                buttons: {
                    OK: function () { $('input:checkbox:checked').focus(); }
                }
            });
            return false;
        }

        if ($("#tituloModal").val().trim() == "" || $("#tituloModal").val() == null) {
            $.alert({
                title: "Titulo Inválido",
                type: 'orange',
                content: 'Debe ingresar un titulo válido.',
                buttons: {
                    OK: function () { $('#tituloModal').focus(); }
                }
            });
            return false;
        }

        if ($("#txtMensajeModal").val().trim() == "" || $("#txtMensajeModal").val() == null) {
            $.alert({
                title: "Mensaje Inválido",
                type: 'orange',
                content: 'Debe ingresar un mensaje válido.',
                buttons: {
                    OK: function () { $('#txtMensajeModal').focus(); }
                }
            });
            return false;
        }

        var fechaVencimiento = $("#mensajeFechaVencimientoMensajeModal").val();
        var fechaInicio = $("#mensajeFechaInicioMensajeModal").val();

        function validate_fechaMayorQue(fechaInicial, fechaFinal) {
            valuesStart = fechaInicial.split("/");
            valuesEnd = fechaFinal.split("/");

            var dateStart = new Date(valuesStart[2], (valuesStart[1] - 1), valuesStart[0]);
            var dateEnd = new Date(valuesEnd[2], (valuesEnd[1] - 1), valuesEnd[0]);
            if (dateStart > dateEnd) {
                return 0;
            }
            return 1;
        }


        if (validate_fechaMayorQue(fechaInicio, fechaVencimiento) == 0) {

            $.alert({
                title: "Fecha Vencimiento Inválida",
                type: 'orange',
                content: 'Debe ingresar una fecha posterior a la de inicio.',
                buttons: {
                    OK: function () { $('#mensajeFechaVencimientoMensajeModal').focus(); }
                }
            });
            return false;
        }

        function rangoDias(start, end) {
            var days = (end - start) / (1000 * 60 * 60 * 24);
            return days;

        }

        if (rangoDias($("#mensajeFechaInicioMensajeModal").datepicker("getDate"), $("#mensajeFechaVencimientoMensajeModal").datepicker("getDate")) < 4) {
            $.alert({
                title: "Rango de Fecha Inválida",
                type: 'orange',
                content: 'Debe ingresar una fecha con un rango minimo de 4 dias a la de inicio.',
                buttons: {
                    OK: function () { $('#mensajeFechaVencimientoMensajeModal').focus(); }
                }
            });
            return false;
        }


        return true;

    }

    function cargarTooltipLista() {
        var idRol;
        $(".listaUsuarioRoles").each(function () {
            idRol = $(this).attr('id');
            idRol = idRol.replace('tooltip_', '');
            UsuariosRoles(idRol);
        });
    }

    function UsuariosRoles(idRol) {
        $.ajax({
            url: "/Rol/ListUsuarios",
            type: 'POST',
            dataType: 'JSON',
            data:
            {
                idRol: idRol
            },
            success: function (list) {

                var lista = "";
                for (var i = 0; i < list.length; i++) {
                    lista += list[i].nombre + ' (' + list[i].email + ')' + '</br> ';
                }
                $('#tooltip_' + idRol + '').attr('data-tipso', lista);
                $('#tooltip_' + idRol + '').tipso(
                    {
                        titleContent: 'DESTINATARIOS',
                        titleBackground: '#428bca',
                        titleColor: '#ffffff',
                        background: '#ffffff',
                        color: '#686868',
                        width: 400
                    });
            }
        });
    }

});

