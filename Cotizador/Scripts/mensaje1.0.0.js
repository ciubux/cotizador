﻿jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición del mensaje; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';


    $(document).ready(function () {
        $("#btnBusquedaMensaje").click();
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


    var fechaVencimientoedit = $("#mensaje_fechaVencimientoMensaje_edit").val();

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

    $("#vendedor_prioridad_si").click(function () {
        changeInputString("prioridad","si");
    });

    $("#vendedor_prioridad_no").click(function () {
        changeInputString("prioridad", "no");
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
                location.reload();
            }
        });
    });

    $(".chk-rol").change(function () {
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
        $('#Mensaje1').modal('show');
    });


    function eliminateDuplicates(arrayIn) {

        var arrayOut = {};
        var unicos = arrayIn.filter(function (e) {
            return arrayOut[e.id_mensaje] ? false : (arrayOut[e.id_mensaje] = true);
        });
        return unicos;
    }

    //var idUsuario;

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

                    list = eliminateDuplicates(list);
                    if (list.length != 0) {

                        $("#imagenMP").before('<a data-notifications="' + list.length + '" class="btnModal" href="javascript:void()"></a>');

                        var verAutomaticamente = false;
                        var esVisible = $(".ModalMensajeAlerta").is(":visible");

                        for (var i = 0; i < list.length; i++) {
                            if (list[i].prioridad == 'si') {
                                verAutomaticamente = true;
                            }
                            var BtnLabel = list.length === 1 ? "Marcar como leído" : "Marcar como leído y mostrar siguiente";
                            var numeroModal = i + 1;
                            if (list.length - 1 === i) {
                                BtnLabel = "Marcar como leído";
                            }
                            var imagenAdvertencia = list[i].prioridad == "si" ? '<img src = "/images/advertencia.svg" style="vertical-align: middle; margin-bottom:5px;  margin-right:10px;" width = "20px" height = "20px"  >' +
                                    '<svg width="1px" height="1px" xmlns="https://www.w3.org/2000/svg"></svg>' : "";
                            
                            var ItemRow =
                                '<div id="Mensaje' + numeroModal + '" class="modal fade ModalMensajeAlerta" tabindex="-1" role="dialog">' +
                                '<div class="modal-dialog" id="MensajeDialog' + numeroModal + '">' +
                                '<div class="modal-content">' +
                                '<div class="modal-header">' +
                                '<div class="btn-group" style="float:left">' +
                                '<button  type="button"  class="btn btn-default Responder" id="' + numeroModal + '"">Responder</button>' +
                                '</div>' +
                                '<div class="btn-group" style="float:right">' +
                                '<button  type="button"  class="Leido btn btn-primary ' + list[i].id_mensaje + ' ">' + BtnLabel + '</button>' +
                                '</div><br><br>' +
                                '</div>' +                               
                                '<div class="modal-body">' +
                               
                                 '<h5> <b>DE: </b>' + list[i].usuario_creacion + ' - <b> FECHA: </b>' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaInicioMensaje)) + '</h5>' +
                                
                                '<h4 style="display: inline-block;">' + imagenAdvertencia +'<u>' + list[i].titulo + '</u></h4>' +
                                '<p>' + list[i].mensaje + '</p>' +
                                '<textarea class="form-control" id ="respuesta' + numeroModal + '" rows="4" style="display:none" placeholder="Respuesta..."> </textarea>' +
                                '<div class="botonesRespuesta' + numeroModal + ' pull-right btn-group" style="display:none">' +

                                '<button   class="cerrarRespuesta btn btn-default" id="' + numeroModal + '">Cancelar</button>' +
                                '<button   class="EnviarRespuesta btn btn-primary ' + list[i].id_mensaje + '">Aceptar</button>' +
                                '</div>' +
                                '</div>' +
                                
                                '<div class="modal-body" id="modalHistorialMensaje' + numeroModal + '">' +

                                '</div>' +
                                

                                '</div>' +
                                '</div>';

                            if (esVisible == false) {
                                $("body").append(ItemRow);
                            }
                        }

                        if (verAutomaticamente && esVisible === false) {
                            setTimeout(function () {
                                $('#Mensaje1').modal('show');
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
                $('#modalHistorialMensaje'+numeroModal+'').empty();
                for (var i = 0; i < list.length; i++) {

                    var date = new Date(list[i].fechaCreacionMensaje);
                    hora = (date.getHours() < 10 ? '0' : '') + date.getHours();
                    minuto = (date.getMinutes() < 10 ? '0' : '') + date.getMinutes();
                    segundo = (date.getSeconds() < 10 ? '0' : '') + date.getSeconds();
                    horaImprimible = hora + ":" + minuto + ":" + segundo;

                    var a =  

                        '<b><P STYLE="margin-bottom: 0.11in"><A NAME="_GoBack"></A><SPAN LANG="es-PE">' + list[i].user.nombre+'</SPAN></b><FONT SIZE=3><SPAN LANG="es-PE"> </SPAN></FONT> <FONT SIZE=2 > <SPAN LANG="es-PE">' +
                        '</SPAN></FONT> <FONT COLOR="#808080"><FONT SIZE=2><SPAN LANG="es-PE">'
                        + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaCreacionMensaje)) +' '+ horaImprimible +'</SPAN></FONT></FONT > <FONT COLOR="#808080"><FONT SIZE=2 STYLE="font-size: 9pt"><SPAN LANG="es-PE">' +
                        '</SPAN></FONT></FONT>' +
                        '</P>' +
                        '<P STYLE="margin-bottom: 0.11in"><FONT SIZE=3><SPAN LANG="es-PE">' + list[i].mensaje + '' +'</SPAN></FONT></P></br>';                       
                        

                    $('#modalHistorialMensaje'+numeroModal+'').append(a);
                }
            }
        });

    });



    $('body').on("click", "button.Responder", function () {
        var num = $(this).attr('id');
        $('#respuesta' + num + '').show();
        $('.botonesRespuesta' + num + '').show();
        $('.Responder').hide();

    });

    $('body').on("click", "button.EnviarRespuesta", function () {
        var num = $(this).prev('button').attr('id');
        var txtRespuesta = $('#respuesta' + num + '').val();
        var arrayClass = $(this).attr('class').split(" ");
        var idMensaje = arrayClass[3];

        if (txtRespuesta.trim() === "") {
            $(this).closest('.modal-content').find('button.Leido').click();
        }
        else {
            $.ajax({
                url: "/Mensaje/MensajeVistoRespuesta",
                type: 'POST',
                data: {
                    idMensaje: idMensaje,
                    respuesta: txtRespuesta
                },
                success: function () {
                    let dialog = $('.' + idMensaje + '').closest('.modal');
                    var btnFinal = dialog.find('.Leido').html();
                    if (btnFinal == "Marcar como leído") {
                        dialog.modal('hide');
                    }
                    else {
                        dialog.modal('hide');
                        dialog.next().modal('show');
                    }
                    $('.' + idMensaje + '').closest('#MensajeDialog').find('#modalRespuesta' + num + '').empty();
                    ActulizarMensaje();

                }
            });
        }

    });

    $('body').on("click", "button.cerrarRespuesta", function () {
        var num = $(this).attr('id');
        $('#respuesta' + num + '').hide();
        $('.botonesRespuesta' + num + '').hide();
        //$('#botonesRespuesta' + num + '').hide();
        $('#respuesta' + num + '').val("");
        $('.Responder').show();

    });

    $('body').on("click", "button.Leido", function () {
        var num = $(this).closest('.modal-header').find(".Responder").attr('id');
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
                }
                else {
                    dialog.modal('hide');
                    dialog.next().modal('show');
                }
                $('.' + idMensaje + '').closest('#MensajeDialog').find('#modalRespuesta' + num + '').empty();
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

    $("#btnBusquedaMensaje").click(function () {

        $("#btnBusquedaMensaje").attr("disabled", "disabled");
        $.ajax({
            url: "/Mensaje/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusquedaMensaje").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusquedaMensaje").removeAttr("disabled");
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
                        '<td>  ' + list[i].usuario_creacion + '  </td>' +
                        '<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaCreacionMensaje)) + '  </td>' +
                        '<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaInicioMensaje)) + '  </td>' +
                        '<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaVencimientoMensaje)) + '  </td>' +
                        '<td>' +
                        '<div class="btn-group">' +
                        '<button type="button" class="' + list[i].id_mensaje + ' btnEditarMensaje btn btn-primary">Editar</button>' +
                        '<button type="button" id ="' + list[i].id_mensaje + '" class="btnVerMensaje btn btn-success">Ver</button>' +
                        '</div>' +
                        '</td>' +
                        '</tr>';

                    $("#tableMensaje").append(ItemRow);

                }
            }
        });
    });

    $('body').on('click', "button.btnVerMensaje", function () {
        var idMensaje = $(this).attr('id');
        $.ajax({
            url: "/Mensaje/verHiloMensaje",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idMensaje: idMensaje
            },
            success: function (list) {
                $("#tableMensajeVer > tbody").empty();
                $('#tituloMensaje').html('Titulo de Mensaje:' + list[list.length - 1].mensaje + '');
                list = eliminateDuplicates(list);
                function eliminateDuplicates(arrayIn) {

                    var arrayOut = {};
                    var unicos = arrayIn.filter(function (e) {
                        return arrayOut[e.fechaCreacionMensaje] ? false : (arrayOut[e.fechaCreacionMensaje] = true);
                    });
                    return unicos;
                }

                for (var i = 0; i < list.length; i++) {
                    var date = new Date(list[i].fechaCreacionMensaje);
                    hora = (date.getHours() < 10 ? '0' : '') + date.getHours();
                    minuto = (date.getMinutes() < 10 ? '0' : '') + date.getMinutes();
                    segundo = (date.getSeconds() < 10 ? '0' : '') + date.getSeconds();
                    horaImprimible = hora + ":" + minuto + ":" + segundo;
                    var numero = list.length - i;
                    var ItemRow =
                        '<tr data-expanded="true"> ' +
                        '<td style="display: table-cell;">  ' + numero + '  </td>' +
                        '<td style="display: table-cell;">  ' + list[i].user.nombre + '  </td>' +
                        '<td style="display: table-cell;">  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaCreacionMensaje)) + ' ' + horaImprimible + '  </td>' +
                        '<td style="display: table-cell;">  ' + list[i].mensaje + '  </td>' +
                        '</tr>';
                    $("#tableMensajeVer").append(ItemRow);
                }
            }
        });
        $('#modalListado').modal('show');
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
                        error: function (detalle) {
                            alert("Ocurrió un problema al iniciar la edición del mensaje.");
                        },
                        success: function (fileName) {
                            window.location = '/Mensaje/Editar';

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

    $("#idUsuarioBusquedaMensaje").change(function () {

        var idUsuario = $("#idUsuarioBusquedaMensaje").val();

        $.ajax({
            url: "/Mensaje/ChangeUsuarioMensaje",
            type: 'POST',
            data: {
                idUsuario: idUsuario
            },
            success: function () {
            }
        });

    });


    $(document).on('click', '#MensajeRapidoMenu', function (event) {
        event.preventDefault();

        var url = $('#MensajeRapido').data('url');

        $("#MensajeRapido").modal("show");

        $("#MensajeRapidoDialog").load(url, function (data) {

            $("select[name=UsuarioMensajeRapido]").chosen();

        });
    });



    $(document).on('change', '#importanciaModal', function (event) {
        changeInputString("prioridad", $("#importanciaModal").val());
    });
    $(document).on('change', '#tituloModal', function (event) {
        changeInputString("titulo", $("#tituloModal").val());
    });
    $(document).on('change', '#txtMensajeModal', function (event) {
        changeInputString("mensaje", $("#txtMensajeModal").val());
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
    });

    $(document).on('click', "#btnCancelarMensajeModal", function () {
        $("#MensajeRapido").modal("hide");
    });

    setTimeout(function () {
        $(document).on('shown.bs.modal', '#MensajeRapido', function () {
            $('#idUsuarioBusquedaMensajeModal', this).chosen('destroy').chosen({ placeholder_text: "Buscar Usuario", no_results_text: "No se encontró Usuario", allow_single_deselect: true }).on('chosen:showing_dropdown');

            //$('#idUsuarioBusquedaMensajeModal', this).chosen({ placeholder_text: "Buscar Usuario", no_results_text: "No se encontró Usuario", allow_single_deselect: true }).on('chosen:showing_dropdown');
            $('#mensajeFechaInicioMensajeModal').datepicker().datepicker("setDate", new Date());
            $('#mensajeFechaVencimientoMensajeModal').datepicker().datepicker("setDate", '+7');

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
    }, 2000);


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
    });

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
            error: function () {

                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear el mensaje.',
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
                    content: 'El mensaje se creó correctamente.',
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

        if ($("#tituloModal").val() === "" || $("#tituloModal").val() === null) {
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

        if ($("#txtMensajeModal").val() == "" || $("#txtMensajeModal").val() == null) {
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
                content: 'Debe ingresar una fecha posterior o igual a la de inicio.',
                buttons: {
                    OK: function () { $('#mensajeFechaVencimientoMensajeModal').focus(); }
                }
            });
            return false;
        }

        return true;

    }

    function cargarTooltipLista()
    {
        var idRol;      
        $(".listaUsuarioRoles").each(function () {
            idRol = $(this).attr('id');
            idRol = idRol.replace('tooltip_','');
            UsuariosRoles(idRol);            
        });
        
        function UsuariosRoles(idRol)
        {
            $.ajax({
                url: "/Rol/ListUsuarios",
                type: 'POST',
                dataType: 'JSON',
            data: 
            {
                idRol: idRol
            },
                success: function (list)
                {                  
                    var lista="";
                    for (var i = 0; i < list.length;i++)
                    {
                        lista += list[i].nombre + ' (' + list[i].email+')'+'</br> ';                        
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
    }

});



