
jQuery(function ($) {
    var turnoTimepickerStep = 30;
    var pagina = 2;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        $("#btnBusqueda").click();
        //cargarChosenCliente();
        verificarSiExisteGrupoCliente();
    });

    function verificarSiExisteGrupoCliente() {
        if ($("#idGrupoCliente").val().trim() != 0) {
           
            //$("#idCiudad").attr("disabled", "disabled");
            $("#grupoCliente_codigo").attr("disabled", "disabled");
            $("#btnFinalizarEdicionGrupoCliente").html('Finalizar Edición');
        }
        else {
            $("#btnFinalizarEdicionGrupoCliente").html('Finalizar Creación');
        }

    }

    function mostrarCamposParaClienteConRUC() {

    }


    $("#tipoDocumentoIdentidad").change(function () {


        var tipoDocumentoIdentidad = $("#tipoDocumentoIdentidad").val();

        if ($("#grupoCliente_codigo").val().length > 0) {
            $.confirm({
                title: 'Confirmación',
                content: '¿Está seguro de cambiar de tipo de Documento?',
                type: 'orange',
                buttons: {
                    confirm: {
                        text: 'Sí',
                        btnClass: 'btn-red',
                        action: function () {
                            cambiarTipoDocumentoIdentidad(tipoDocumentoIdentidad);
                            limpiarFormulario();
                            mostrarCamposSegunTipoDocIdentidad();
                        }
                    },
                    cancel: {
                        text: 'No',
                        //btnClass: 'btn-blue',
                        //                        keys: ['enter', 'shift'],
                        action: function () {
                            location.reload();
                        }
                    }
                },

            });


        }
        else {
            cambiarTipoDocumentoIdentidad(tipoDocumentoIdentidad);
            limpiarFormulario();
            mostrarCamposSegunTipoDocIdentidad();
        }

    });

    function cambiarTipoDocumentoIdentidad(tipoDocumentoIdentidad) {
        $.ajax({
            url: "/GrupoCliente/changeTipoDocumentoIdentidad",
            type: 'POST',
            data: { tipoDocumentoIdentidad: tipoDocumentoIdentidad },
            success: function () {

            },
            error: function () {
                $.alert({
                    title: 'Error',
                    content: MENSAJE_ERROR,
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });
            }
        });

    }


    function limpiarFormulario() {

        $("#grupoCliente_nombre").val("");
        $("#grupoCliente_contacto").val("");
        $("#grupoCliente_telefonoContacto").val("");
        $("#grupoCliente_emailContacto").val("");

        $("#grupoCliente_plazoCredito").val("");
        $("#tipoPagoCliente").val("0");
        $("#formaPagoCliente").val("0");


        $("#idGrupoCliente").val("");
        $("#sinPlazoCreditos").removeAttr("checked");
        $("#bloqueadoBusqueda").removeAttr("checked");
        $("#sinAsesorValidado").removeAttr("checked");
    }


    $("#btnLimpiarBusqueda").click(function () {
        $.ajax({
            url: "/GrupoCliente/CleanBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });

    function ConfirmDialogReload(message) {
        $('<div></div>').appendTo('body')
            .html('<div><h6>' + message + '</h6></div>')
            .dialog({
                modal: true, title: 'Confirmación', zIndex: 10000, autoOpen: true,
                width: 'auto', resizable: false,
                buttons: {
                    Si: function () {
                        location.reload();
                        $(this).dialog("close");
                    },
                    No: function () {
                        $(this).dialog("close");
                    }
                },
                close: function (event, ui) {
                    $(this).remove();
                }
            });
        document.body.scrollTop = default_scrollTop;
    };

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
    };







    /**
     * ################################ INICIO CONTROLES DE CLIENTE
     */

    function cargarChosenCliente() {

        $("#idCliente").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" }).on('chosen:showing_dropdown', function (evt, params) {
            if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
                alert("Debe seleccionar la sede MP previamente.");
                $("#idCliente").trigger('chosen:close');
                $("#idCiudad").focus();
                return false;
            }
        });

        $("#idCliente").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/GrupoCliente/SearchClientes"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });

        //verificarSiExisteGrupoCliente();
    }





    function toggleControlesUbigeo() {
        //Si cliente esta Seleccionado se habilitan la seleccion para la direccion de entrega
        if ($("#idCliente").val().trim() == "") {
            $("#ActualDepartamento").attr('disabled', 'disabled');
            $('#ActualProvincia').attr('disabled', 'disabled');
            $('#ActualDistrito').attr('disabled', 'disabled');
            $("#pedido_direccionEntrega").attr('disabled', 'disabled');
            $("#btnAgregarDireccion").attr("disabled", "disabled");
        }
        else {
            $('#ActualDepartamento').removeAttr('disabled');
            $('#ActualProvincia').removeAttr('disabled');
            $('#ActualDistrito').removeAttr('disabled');
            $("#pedido_direccionEntrega").removeAttr('disabled');
            $("#btnAgregarDireccion").removeAttr('disabled');
        }
        toggleControlesDireccionEntrega();
    }



    

    $(window).on("paste", function (e) {

        $.each(e.originalEvent.clipboardData.items, function () {
            this.getAsString(function (str) {
                //alert(str);
                var lineas = str.split("\t");

                if (lineas.length <= 1)
                    return false;

                //      if (lineas[1] == $("#grupoCliente_ruc").val()) {
                //      $("#ncRUC").val(lineas[1]);

                //       $("#grupoCliente_razonSocialSunat").val(lineas[0]);
                //$("#ncRUC").val(lineas[1]);
                var direccionDomicilioLegalSunat = lineas[0] + " " + lineas[1] + " - " + lineas[2] + " - " + lineas[3];



                $('body').loadingModal({
                    text: 'Recuperando Ubicación Geográfica...'
                });
                $.ajax({
                    url: "/GrupoCliente/ChangeDireccionDomicilioLegalSunat",
                    type: 'POST',
                    dataType: 'JSON',
                    data: {
                        direccionDomicilioLegalSunat: direccionDomicilioLegalSunat
                    },
                    erro: function () {
                        $('body').loadingModal('hide')
                    },

                    success: function (resultado) {
                        $('body').loadingModal('hide')
                        $("#grupoCliente_direccionDomicilioLegalSunat").val(resultado.direccionDomicilioLegalSunat);
                        $("#grupoCliente_ubigeo_Departamento").val(resultado.ubigeo.Departamento);
                        $("#grupoCliente_ubigeo_Provincia").val(resultado.ubigeo.Provincia);
                        $("#grupoCliente_ubigeo_Distrito").val(resultado.ubigeo.Distrito);
                    }
                });

                changeInputString("estadoContribuyente", $("#grupoCliente_estadoContribuyente").val())
                changeInputString("condicionContribuyente", $("#grupoCliente_condicionContribuyente").val())
                /*   }
                   else {
                       alert("El RUC que acaba de pegar no coincide con el RUC del cliente.");
                   }*/
            });
        });


    });


    function validacionDatosGrupoCliente() {
        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            $.alert({
                title: "No selecionó Ciudad",
                type: 'orange',
                content: 'Debe seleccionar la sede MP previamente.',
                buttons: {
                    OK: function () { }
                }
            });
            $("#idCiudad").focus()
            return false;
        }


      

        if ($("#plazoCreditoSolicitado").is(':enabled')) {

            if ($("#plazoCreditoSolicitado").val() == "0") {
                $.alert({
                    title: "Plazo Crédito Solicitado Inválido",
                    type: 'orange',
                    content: 'Debe indicar el plazo de crédito Solicitado',
                    buttons: {
                        OK: function () { $('#plazoCreditoSolicitado').focus(); }
                    }
                });
                return false;
            }
        }

        /*  if ($("#idResponsableComercial").val().trim() == 0) {
              $.alert({
                  title: "Responsable Comercial Inválido",
                  type: 'orange',
                  content: 'Debe seleccionar al Responsable Comercial',
                  buttons: {
                      OK: function () { $('#idResponsableComercial').focus(); }
                  }
              });
              return false;
          }        */

        return true;

    }


    $("#btnFinalizarEdicionGrupoCliente").click(function () {
        /*Si no tiene codigo el cliente se está creando*/
        if ($("#idGrupoCliente").val() == 0) {
            crearGrupoCliente();
        }
        else {
            editarGrupoCliente();
        }
    });



    function crearGrupoCliente() {
        if (!validacionDatosGrupoCliente())
            return false;

        $('body').loadingModal({
            text: 'Creando Grupo Cliente...'
        });
        $.ajax({
            url: "/GrupoCliente/Create",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear el grupo cliente.',
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
                    content: 'El grupo cliente se creó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/GrupoCliente/Index';
                        }
                    }
                });
            }
        });

    }

    function editarGrupoCliente() {

        if (!validacionDatosGrupoCliente())
            return false;


        $('body').loadingModal({
            text: 'Editando Grupo Cliente...'
        });
        $.ajax({
            url: "/GrupoCliente/Update",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar el cliente.',
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
                    content: 'El Grupo Cliente se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/GrupoCliente/Index';
                        }
                    }
                });
            }
        });
    }

    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/GrupoCliente/ChangeInputInt",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#grupoCliente_sobrePlazo").change(function () {
        changeInputInt("sobrePlazo", $("#grupoCliente_sobrePlazo").val())
    });

    function changeInputDecimal(propiedad, valor) {
        $.ajax({
            url: "/GrupoCliente/ChangeInputDecimal",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#grupoCliente_creditoSolicitado").change(function () {
        changeInputDecimal("creditoSolicitado", $("#grupoCliente_creditoSolicitado").val())
    });

    $("#grupoCliente_creditoAprobado").change(function () {
        changeInputDecimal("creditoAprobado", $("#grupoCliente_creditoAprobado").val())
    });

    $("#grupoCliente_sobreGiro").change(function () {
        changeInputDecimal("sobreGiro", $("#grupoCliente_sobreGiro").val())
    });
    

    /*
    function changeInputTime(propiedad, valor) {
        $.ajax({
            url: "/GrupoCliente/ChangeInputTime",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }*/



    $("#grupoCliente_horaInicioPrimerTurnoEntregaFormat").change(function () {
        var hour = $("#grupoCliente_horaInicioPrimerTurnoEntregaFormat").val().trim();
        var hourPost = $("#grupoCliente_horaFinPrimerTurnoEntregaFormat").val().trim();
        if (validateHourFormat(hour)) {
            changeInputString("horaInicioPrimerTurnoEntrega", hour);


            if (!validateHourFormat(hourPost) || !validateHoraPosterior(hour, hourPost)) {
                hour = hour.split(":");

                var hTime = parseInt(hour[0]);
                var mTime = parseInt(hour[1]);

                var txtHoruPost = "";

                if (hTime >= 23) {
                    var txtHoruPost = "23:59";
                } else {
                    var txtHoruPost = "";
                    hTime = hTime + 1;
                    if (hTime < 10) {
                        txtHoruPost = "0" + hTime;
                    } else {
                        txtHoruPost = hTime;
                    }
                    txtHoruPost = txtHoruPost + ":";
                    if (mTime < 10) {
                        txtHoruPost = txtHoruPost + "0" + mTime;
                    } else {
                        txtHoruPost = txtHoruPost + mTime;
                    }
                }

                $("#grupoCliente_horaFinPrimerTurnoEntregaFormat").val(txtHoruPost);
                changeInputString("horaFinPrimerTurnoEntrega", txtHoruPost);

                $("#grupoCliente_horaInicioSegundoTurnoEntregaFormat").val("");
                changeInputString("horaInicioSegundoTurnoEntrega", "");

                $("#grupoCliente_horaFinSegundoTurnoEntregaFormat").val("");
                changeInputString("horaFinSegundoTurnoEntrega", "");
            }
        } else {
            $("#grupoCliente_horaInicioPrimerTurnoEntregaFormat").val("09:00");
            changeInputString("horaInicioPrimerTurnoEntrega", "09:00");
            $.alert({
                title: "Hora Invalida",
                type: 'orange',
                content: 'Debe seleccionar una hora de la lista.',
                buttons: {
                    OK: function () { }
                }
            });
        }
    });

    $("#grupoCliente_horaFinPrimerTurnoEntregaFormat").change(function () {
        var hourPrev = $("#grupoCliente_horaInicioPrimerTurnoEntregaFormat").val().trim();
        var hour = $("#grupoCliente_horaFinPrimerTurnoEntregaFormat").val().trim();
        var hourPost = $("#grupoCliente_horaInicioSegundoTurnoEntregaFormat").val().trim();

        if (validateHourFormat(hour)) {
            var valid = true;
            if (!validateHourFormat(hourPrev)) {
                valid = false;
                $.alert({
                    title: "Hora Invalida",
                    type: 'orange',
                    content: 'Error en los horarios.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }

            if (!validateHoraPosterior(hourPrev, hour)) {
                valid = false;
                $.alert({
                    title: "Hora Invalida",
                    type: 'orange',
                    content: 'Debe seleccionar una hora mayor a la hora inicial del primer turno.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }

            if (valid) {
                changeInputString("horaFinPrimerTurnoEntrega", $("#grupoCliente_horaFinPrimerTurnoEntregaFormat").val());

                if (!validateHourFormat(hourPost) || !validateHoraPosterior(hour, hourPost)) {
                    $("#grupoCliente_horaInicioSegundoTurnoEntregaFormat").val("");
                    changeInputString("horaInicioSegundoTurnoEntrega", "");

                    $("#grupoCliente_horaFinSegundoTurnoEntregaFormat").val("");
                    changeInputString("horaFinSegundoTurnoEntrega", "");
                }
            } else {
                $("#grupoCliente_horaFinPrimerTurnoEntregaFormat").val("18:00");
                changeInputString("horaFinPrimerTurnoEntrega", "18:00");

                $("#grupoCliente_horaInicioSegundoTurnoEntregaFormat").val("");
                changeInputString("horaInicioSegundoTurnoEntrega", "");

                $("#grupoCliente_horaFinSegundoTurnoEntregaFormat").val("");
                changeInputString("horaFinSegundoTurnoEntrega", "");
            }
        } else {
            $("#grupoCliente_horaFinPrimerTurnoEntregaFormat").val("18:00");
            changeInputString("horaFinPrimerTurnoEntrega", "18:00");
            $.alert({
                title: "Hora Invalida",
                type: 'orange',
                content: 'Debe seleccionar una hora de la lista.',
                buttons: {
                    OK: function () { }
                }
            });
        }
    });


    $("#grupoCliente_horaInicioSegundoTurnoEntregaFormat").change(function () {

        var hourPrev = $("#grupoCliente_horaFinPrimerTurnoEntregaFormat").val().trim();
        var hour = $("#grupoCliente_horaInicioSegundoTurnoEntregaFormat").val().trim();
        var hourPost = $("#grupoCliente_horaFinSegundoTurnoEntregaFormat").val().trim();

        if (validateHourFormat(hour)) {
            var valid = true;
            if (!validateHourFormat(hourPrev)) {
                valid = false;
                $.alert({
                    title: "Hora Invalida",
                    type: 'orange',
                    content: 'Error en los horarios.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }

            if (!validateHoraPosterior(hourPrev, hour)) {
                valid = false;
                $.alert({
                    title: "Hora Invalida",
                    type: 'orange',
                    content: 'Debe seleccionar una hora mayor a la hora de final del primer turno.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }

            if (valid) {
                changeInputString("horaInicioSegundoTurnoEntrega", $("#grupoCliente_horaInicioSegundoTurnoEntregaFormat").val());

                if (!validateHourFormat(hourPost) || !validateHoraPosterior(hour, hourPost)) {
                    $("#grupoCliente_horaFinSegundoTurnoEntregaFormat").val("");
                    changeInputString("horaFinSegundoTurnoEntrega", "");
                }
            } else {
                $("#grupoCliente_horaInicioSegundoTurnoEntregaFormat").val("");
                changeInputString("horaInicioSegundoTurnoEntrega", "");

                $("#grupoCliente_horaFinSegundoTurnoEntregaFormat").val("");
                changeInputString("horaFinSegundoTurnoEntrega", "");
            }
        } else {
            $("#grupoCliente_horaInicioSegundoTurnoEntregaFormat").val("");
            changeInputString("horaInicioSegundoTurnoEntrega", "");
            $.alert({
                title: "Hora Invalida",
                type: 'orange',
                content: 'Debe seleccionar una hora de la lista.',
                buttons: {
                    OK: function () { }
                }
            });
        }
    });


    $("#grupoCliente_horaFinSegundoTurnoEntregaFormat").change(function () {
        var hourPrev = $("#grupoCliente_horaInicioSegundoTurnoEntregaFormat").val().trim();
        var hour = $("#grupoCliente_horaFinSegundoTurnoEntregaFormat").val().trim();

        if (validateHourFormat(hour)) {
            var valid = true;
            if (!validateHourFormat(hourPrev)) {
                valid = false;
                $.alert({
                    title: "Hora Invalida",
                    type: 'orange',
                    content: 'Error en los horarios.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }

            if (!validateHoraPosterior(hourPrev, hour)) {
                valid = false;
                $.alert({
                    title: "Hora Invalida",
                    type: 'orange',
                    content: 'Debe seleccionar una hora mayor a la hora de inicial del segundo turno.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }

            if (valid) {
                changeInputString("horaFinSegundoTurnoEntrega", $("#grupoCliente_horaFinSegundoTurnoEntregaFormat").val());
            } else {
                $("#grupoCliente_horaFinSegundoTurnoEntregaFormat").val("");
                changeInputString("horaFinSegundoTurnoEntrega", "");
            }
        } else {
            $("#grupoCliente_horaFinSegundoTurnoEntregaFormat").val("");
            $.alert({
                title: "Hora Invalida",
                type: 'orange',
                content: 'Debe seleccionar una hora de la lista.',
                buttons: {
                    OK: function () { }
                }
            });
        }
    });

    function validateHoraPosterior(horaPrev, horaPost) {
        //var horaPrev = $("#" + idHoraPrev).val();
        //var horaPost = $("#" + idHoraPost).val();

        horaPrev = horaPrev.split(":");
        horaPost = horaPost.split(":");

        var hPrev = parseInt(horaPrev[0]);
        var mPrev = parseInt(horaPrev[1]);
        var hPost = parseInt(horaPost[0]);
        var mPost = parseInt(horaPost[1]);

        if (hPrev <= hPost) {
            if (hPrev == hPost) {
                if (mPrev >= mPost) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return true;
            }
        } else {
            return false;
        }
    }

    function validateHourFormat(hourString) {
        var hourFormat = /(([0-1][0-9])|(2[0-3])):?[0-5][0-9]/;

        return hourFormat.test(hourString);
    }

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/GrupoCliente/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }





    $("#grupoCliente_contacto").change(function () {
        changeInputString("contacto", $("#grupoCliente_contacto").val());
    });

    $("#grupoCliente_telefonoContacto").change(function () {
        changeInputString("telefonoContacto", $("#grupoCliente_telefonoContacto").val());
    });

    $("#grupoCliente_emailContacto").change(function () {
        changeInputString("emailContacto", $("#grupoCliente_emailContacto").val());
    });


    $("#grupoCliente_observacionesCredito").change(function () {
        changeInputString("observacionesCredito", $("#grupoCliente_observacionesCredito").val())
    });

    $("#grupoCliente_observaciones").change(function () {
        changeInputString("observaciones", $("#grupoCliente_observaciones").val())
    });
    
    $("#grupoCliente_nombre").change(function () {
        changeInputString("nombre", $("#grupoCliente_nombre").val())
    });

    $("#grupoCliente_codigo").change(function () {
        changeInputString("codigo", $("#grupoCliente_codigo").val())
    });

    $("#formaPagoCliente").change(function () {
        var formaPagoFactura = $("#formaPagoCliente").val();
        $.ajax({
            url: "/GrupoCliente/ChangeFormaPagoFactura",
            type: 'POST',
            data: {
                formaPagoFactura: formaPagoFactura
            },
            success: function () { }
        });
    });

    $("#plazoCreditoAprobado").change(function () {
        var plazoCreditoAprobado = $("#plazoCreditoAprobado").val();
        $.ajax({
            url: "/GrupoCliente/ChangePlazoCreditoAprobado",
            type: 'POST',
            data: {
                plazoCreditoAprobado: plazoCreditoAprobado
            },
            success: function () { }
        });
    });


    $("#plazoCreditoSolicitado").change(function () {
        var plazoCreditoSolicitado = $("#plazoCreditoSolicitado").val();
        $.ajax({
            url: "/GrupoCliente/ChangePlazoCreditoSolicitado",
            type: 'POST',
            data: {
                plazoCreditoSolicitado: plazoCreditoSolicitado
            },
            success: function () { }
        });
    });
    






    ////////VER COTIZACIÓN  --- CAMBIO DE ESTADO

    function invertirFormatoFecha(fecha)
    {
        var fechaInvertida = fecha.split("-");
        fecha = fechaInvertida[2] + "/" + fechaInvertida[1] + "/" + fechaInvertida[0];
        return fecha 
    }

    function convertirFechaNumero(fecha) {
        var fechaInvertida = fecha.split("/");
        fecha = fechaInvertida[2] + fechaInvertida[1] + fechaInvertida[0];
        return Number(fecha)
    }

    function convertirHoraToNumero(hora) {
        var hora = hora.split(":");
        hora = hora[0] +"."+ hora[1];
        return Number(hora)
    }

    function sumarHoras(hora, cantidad) {
        var hora = hora.split(":");

        var horaNumero = Number(hora[0]) + cantidad;


        if (horaNumero < 0)
            hora = "00:00";
        else if (horaNumero > 23)
            hora = "23:59";

        if (horaNumero < 10) {
            hora = "0" + horaNumero + ":" + hora[1];
            // hora = horaNumero + ":" + hora[1];
        }
        else {
            hora = horaNumero + ":" + hora[1];
        }

        return hora
    }




    $("#btnCancelarGrupoCliente").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/GrupoCliente/CancelarCreacionGrupoCliente', null)
    })


    
    var ft = null;
    
    

    /*####################################################
    EVENTOS DE LA GRILLA
    #####################################################*/


    /**
     * Se definen los eventos de la grilla
     */
    function cargarTablaDetalle() {
        var $modal = $('#tableDetallePedido'),
            $editor = $('#tableDetallePedido'),
            $editorTitle = $('#tableDetallePedido');

     
        ft = FooTable.init('#tableDetallePedido', {
            editing: {
                enabled: true,
                addRow: function () {
                    ConfirmDialogReload(MENSAJE_CANCELAR_EDICION);
                },
                editRow: function (row) {
                    var values = row.val();
                    var idProducto = values.idProducto;
                    alert(idProducto);
                },
                deleteRow: function (row) {
                    //  if (confirm('¿Esta seguro de eliminar el producto?')) {
                    var values = row.val();
                    var idProducto = values.idProducto;
                    /*
                                                $.ajax({
                                                    url: "/Pedido/DelProducto",
                                                    type: 'POST',
                                                    data: {
                                                        idProducto: idProducto
                                                    },
                                                    success: function (total) {
                                                */
                    row.delete();
                }
            }
        });
        
        /*.bind({
            'footable_sorted': function (e) {
                /*    var rows = $('#details tbody tr.data');
        
                    rows.each(function () {
                        var personid = $(this).data('row-person');
        
                        var detail = $('#details tbody tr.descriptions[data-detail-person="' + personid + '"]');
                        $(detail).insertAfter($(this));
                    });
                alert("asas");
            }
        });*/
    }
    cargarTablaDetalle();


 


 //   $('#tablefoottable').footable()

   /* $("#tablefoottable").onSort(function () {
        alert("Asas");

        /* onSort

    sort.bs.table

        }
    );*/
   


    function mostrarFlechasOrdenamiento() {
        $(".ordenar, .detordenamiento").attr('style', 'display: table-cell');

       // $(".detordenamiento.media").html('<div class="updown"><div class="up"></div> <div class="down"></div></div>');

        //Se identifica cuantas filas existen
        var contador = 0;
        var $j_object = $("td.detordenamiento");
        $.each($j_object, function (key, value) {
            contador++;
        });

        if (contador == 1) {
            $(".detordenamiento").html('');
        }
        else if (contador > 1)
        {
            var contador2 = 0;
            var $j_object = $("td.detordenamiento");
            $.each($j_object, function (key, value) {
                contador2++;
                if (contador2 == 1) {
                    value.innerHTML = '<div class="updown"><div class="down"></div></div>';
                    value.setAttribute("posicion", "primera");
                }
                else if (contador2 == contador) {
                    value.innerHTML = '<div class="updown"><div class="up"></div></div>';
                    value.setAttribute("posicion", "ultima");
                }
                else
                {
                    value.innerHTML = '<div class="updown"><div class="up"></div> <div class="down"></div></div>';
                    value.setAttribute("posicion", "media");
                }
            });
        }






    }


    $(document).on('click', ".up,.down", function () {

        var codigo = event.target.parentElement.parentElement.getAttribute("class").split(" ")[0];
        var row = $(this).parents("tr:first");

        //Mover hacia arriba
        if ($(this).is(".up")) {

            var posicionPrevia = row.prev().find('td.detordenamiento').attr("posicion");
            var htmlPrevio = row.prev().find('td.detordenamiento').html();

            var posicionActual = row.find('td.detordenamiento').attr("posicion");
            var htmlActual = row.find('td.detordenamiento').html(); 

            //intercambio de posicion
            row.prev().find('td.detordenamiento').attr("posicion", posicionActual);
            row.find('td.detordenamiento').attr("posicion", posicionPrevia);
            //intercambio de controles
            row.prev().find('td.detordenamiento').html(htmlActual);
            row.find('td.detordenamiento').html(htmlPrevio);

            row.insertBefore(row.prev());

        } else {
            var posicionPrevia = row.next().find('td.detordenamiento').attr("posicion");
            var htmlPrevio = row.next().find('td.detordenamiento').html();

            var posicionActual = row.find('td.detordenamiento').attr("posicion");
            var htmlActual = row.find('td.detordenamiento').html();

            //intercambio de posicion
            row.next().find('td.detordenamiento').attr("posicion", posicionActual);
            row.find('td.detordenamiento').attr("posicion", posicionPrevia);
            //intercambio de controles
            row.next().find('td.detordenamiento').html(htmlActual);
            row.find('td.detordenamiento').html(htmlPrevio);

            row.insertAfter(row.next());
        }
    });











    

    /*Evento que se dispara cuando se hace clic en el boton EDITAR en la edición de la grilla*/
    $(document).on('click', "button.footable-show", function () {

        //Cambiar estilos a los botones
        $("button.footable-add").attr("class", "btn btn-default footable-add");
        $("button.footable-hide").attr("class", "btn btn-primary footable-hide");


        //Se deshabilitan controles que recargan la página o que interfieren con la edición del detalle
        $("#considerarCantidades").attr('disabled', 'disabled');
        $("input[name=igv]").attr('disabled', 'disabled');
        $("#flete").attr('disabled', 'disabled');
        $("#btnOpenAgregarProducto").attr('disabled', 'disabled');

        var codigo = $("#numero").val();
        if (codigo == "") {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarCreacionPedido").attr('disabled', 'disabled');
            $("#btnCancelPedido").attr('disabled', 'disabled');
        }
        else {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarEdicionPedido").attr('disabled', 'disabled');
            $("#btnCancelPedido").attr('disabled', 'disabled');
        }

        
        $("input[name=mostrarcodproveedor]").attr('disabled', 'disabled');


        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
            console.log('Esto es un dispositivo móvil');
            return;
        }


        //llama a la función que cambia el estilo de display después de que la tabla se ha redibujado
        //Si se le llama antes el redibujo reemplazará lo definido
        window.setInterval(mostrarFlechasOrdenamiento, 600);

        /*Se agrega control input en columna cantidad*/
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var cantidad = value.innerText;
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detincantidad form-control' value='" + cantidad + "' type='number'/>";
        });

        var considerarCantidades = $("#considerarCantidades").val();
        if (considerarCantidades == CANT_SOLO_OBSERVACIONES) {
            /*Se agrega control input en columna observacion*/
            var $j_object = $("td.detobservacion");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var observacion = value.innerText;
                value.innerHTML = "<textarea class='" + arrId[0] + " detobservacionarea form-control'/>" + observacion + "</textarea>";
            });
        }
        else if (considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES) 
        {
            var $j_object = $("span.detproductoObservacion");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var observacion = value.innerText;
                value.innerHTML = "<textarea class='" + arrId[0] + " detobservacionarea form-control'/>" + observacion + "</textarea>";
            });

        }

     //   @cotizacionDetalle.producto.idProducto detproductoObservacion"




        /*Se agrega control input en columna porcentaje descuento*/
        var $j_object1 = $("td.detporcentajedescuento");
        $.each($j_object1, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var porcentajedescuento = value.innerText;
            porcentajedescuento = porcentajedescuento.replace("%", "").trim();
            $(".detporcentajedescuentoMostrar." + arrId[0]).html("<div style='width: 150px' ><div style='float:left' ><input style='width: 100px' class='" + arrId[0] + " detinporcentajedescuento form-control' value='" + porcentajedescuento + "' type='number'/></div><div > <button type='button' class='" + arrId[0] + " btnCalcularDescuento btn btn-primary bouton-image monBouton' data-toggle='modal' data-target='#modalCalculadora' ></button ></div></div>");

        });


        var $j_objectFlete = $("td.detflete");
        $.each($j_objectFlete, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var flete = value.innerText;
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detinflete form-control' value='" + flete + "' type='number'/>";
        });



    });


    

    /*####################################################
    EVENTOS BUSQUEDA COTIZACIONES
    #####################################################*/

  
    

    $("#idCiudad").change(function () {
        var idCiudad = $("#idCiudad").val();
        var textCiudad = $("#idCiudad option:selected").text();

        $.ajax({
            url: "/GrupoCliente/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad
            },
            error: function (detalle) {
                location.reload();
            },
            success: function (ciudad) {
            }
        });
    });  

    

    $(document).on('click', "a.verMas", function () {
        var idCotizacion = event.target.getAttribute("class").split(" ")[0];
        divCorto = document.getElementById(idCotizacion + "corto");
        divLargo = document.getElementById(idCotizacion + "largo");
        divVerMas = document.getElementById(idCotizacion + "verMas");
        divVerMenos = document.getElementById(idCotizacion + "verMenos");

        divCorto.style.display = 'none';
        divLargo.style.display = 'block';

        divVerMas.style.display = 'none';
        divVerMenos.style.display = 'block';
    });

    $(document).on('click', "a.verMenos", function () {
        var idCotizacion = event.target.getAttribute("class").split(" ")[0];
        divCorto = document.getElementById(idCotizacion + "corto");
        divLargo = document.getElementById(idCotizacion + "largo");
        divVerMas = document.getElementById(idCotizacion + "verMas");
        divVerMenos = document.getElementById(idCotizacion + "verMenos");

        divCorto.style.display = 'block';
        divLargo.style.display = 'none';

        divVerMas.style.display = 'block';
        divVerMenos.style.display = 'none';
    });
    

    function changeInputBoolean(propiedad, valor) {
        $.ajax({
            url: "/GrupoCliente/ChangeInputBoolean",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function changeInputBitBoolean(propiedad, valor) {
        $.ajax({
            url: "/GrupoCliente/ChangeInputBitBoolean",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }
    


    $("#sinAsesorValidado").change(function () {
        var valCheck = 0;
        if ($("#sinAsesorValidado").is(":checked")) {
            valCheck = 1;
        }

        $.ajax({
            url: "/GrupoCliente/ChangeSinAsesorValidado",
            type: 'POST',
            data: {
                sinAsesorValidado: valCheck
            },
            success: function () {
            }
        });
    });

    $("#sinPlazoCreditos").change(function () {
        var valCheck = 0;
        if ($("#sinPlazoCreditos").is(":checked")) {
            valCheck = 1;
        }

        $.ajax({
            url: "/GrupoCliente/ChangeSinPlazoCreditoAprobado",
            type: 'POST',
            data: {
                sinPlazoCreditoAprobado: valCheck
            },
            success: function () {
            }
        });
    });

    $("#bloqueadoBusqueda").change(function () {
        var valCheck = 0;
        if ($("#bloqueadoBusqueda").is(":checked")) {
            valCheck = 1;
        }
        changeInputBoolean('bloqueado', valCheck);
    });
    

    $("#btnExportExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
    });

    $("#btnBusqueda").click(function () {
        /*
        if ($("#grupoCliente_nombre").val().length < 3 &&
            $("#sinPlazoCreditos").is(":checked") == 0 &&
            $("#grupoCliente_codigo").val().trim().length == 0 
            ) {
            $.alert({
                title: 'Ingresar texto a buscar',
                content: 'Debe ingresar el texto a buscar utilizando 3 o más caracteres en el campo Nombre',
                type: 'orange',
                buttons: {
                    OK: function () {

                        $("#grupoCliente_nombre").focus();
                    }
                }
            });
            $("#tableGruposCliente > tbody").empty();
            $("#tableGruposCliente").footable({
                "paging": {
                    "enabled": true
                }
            });

            
            return false;
        }*/

        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/GrupoCliente/Search",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableGruposCliente > tbody").empty();
                $("#tableGruposCliente").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < list.length; i++) {



                    var clienteRow = '<tr data-expanded="true">' +
                        '<td>  ' + list[i].idGrupoCliente + '</td>' +
                        '<td>  ' + list[i].codigo + '  </td>' +
                        '<td>  ' + list[i].nombre + '  </td>' +
                        '<td>  ' + list[i].ciudad.nombre + '  </td>' +
                        '<td>  ' + list[i].plazoCreditoAprobadoToString + '</td>' +
                        '<td>  ' + list[i].creditoAprobado.toFixed(cantidadDecimales) + '  </td>' +
                        '<td>' +
                        '<button type="button" class="' + list[i].idGrupoCliente + ' ' + list[i].codigo + ' btnVerGrupoCliente btn btn-primary ">Ver</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tableGruposCliente").append(clienteRow);

                }

                if (clienteList.length > 0) {
                    $("#msgBusquedaSinResultados").hide();
                    $("#divExportButton").show();
                }
                else {
                    $("#msgBusquedaSinResultados").show();
                    $("#divExportButton").hide();
                }

            }
        });
    });
    

    var idGrupoClienteView = "";
    $(document).on('click', "button.btnVerGrupoCliente", function () {
        $('body').loadingModal({
            text: 'Abriendo Grupo Cliente...'
        });
        $('body').loadingModal('show');

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idGrupoCliente = arrrayClass[0];
        var codigoGrupoCliente = arrrayClass[1];

        $.ajax({
            url: "/GrupoCliente/Show",
            data: {
                idGrupoCliente: idGrupoCliente
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                mostrarMensajeErrorProceso();
            },
            success: function (result) {
                var obj = result;
                idGrupoClienteView = idGrupoCliente;
                $('body').loadingModal('hide')
                $("#verCiudadNombre").html(obj.ciudad.nombre);
                $("#verCodigo").html(obj.codigo);
                $("#verNombre").html(obj.nombre);
                $("#verContacto").html(obj.contacto);
                $("#verTelefonoContacto").html(obj.telefonoContacto);
                $("#verEmailContacto").html(obj.emailContacto);
                $("#verObservaciones").html(obj.observaciones);

                /*Plazo Crédito*/
                $("#verPlazoCreditoSolicitado").html(obj.plazoCreditoSolicitadoToString);
                $("#verPlazoCreditoAprobado").html(obj.plazoCreditoAprobadoToString);
                
                /*Montos de Crédito*/
                $("#verCreditoSolicitado").html(obj.creditoSolicitado.toFixed(cantidadDecimales));
                $("#verCreditoAprobado").html(obj.creditoAprobado.toFixed(cantidadDecimales));
                $("#verSobreGiro").html(obj.sobreGiro.toFixed(cantidadDecimales));

                $("#verObservacionesCredito").html(obj.observacionesCredito);
        
                $("#modalVerGrupoCliente").modal('show');                        
            }
        });
    });


    $("#btnEditarGrupoCliente").click(function () {
      //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona
        $.ajax({
            url: "/GrupoCliente/ConsultarSiExisteGrupoCliente",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/GrupoCliente/iniciarEdicionGrupoCliente",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del grupo de cliente."); },
                        success: function (fileName) {
                            window.location = '/GrupoCliente/Editar';
                        }
                    });

                }
                else {
                    if (resultado.codigo == 0) {
                        alert('Está creando un nuevo grupo cliente; para continuar por favor diríjase a la página "Crear/Modificar Grupo Cliente" y luego haga clic en el botón Cancelar.');
                    }
                    
                    else {
                        alert('Ya se encuentra editando el grupo cliente con código ' + resultado.codigo + '; para continuar por favor dirigase a la página "Crear/Modificar Grupo Cliente".');
                    }
                }
            }
        });
    });





     /*CARGA Y DESCARGA DE ARCHIVOS*/

    /*
    $('input[name=filePedidos]').change(function (e) {
        //$('#nombreArchivos').val(e.currentTarget.files);
        var numFiles = e.currentTarget.files.length;
        var nombreArchivos = "";
        for (i = 0; i < numFiles; i++) {
            var fileFound = 0;
            $("#nombreArchivos > li").each(function (index) {

                if ($(this).find("a.descargar")[0].text == e.currentTarget.files[i].name) {
                    alert('El archivo "' + e.currentTarget.files[i].name + '" ya se encuentra agregado.');
                    fileFound = 1;
                }
            });

            if (fileFound == 0) {

                var liHTML = '<a href="javascript:mostrar();" class="descargar">' + e.currentTarget.files[i].name + '</a>' +
                    '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + e.currentTarget.files[i].name + '" class="btnDeleteArchivo" /></a>';

                $('#nombreArchivos').append($('<li />').html(liHTML));


                ///  $('<li />').text(e.currentTarget.files[i].name).appendTo($('#nombreArchivos'));
            }

        }
    });*/


    $('input:file[multiple]').change(function (e) {       

        cargarArchivosAdjuntos(e.currentTarget.files);

        var data = new FormData($('#formularioConArchivos')[0]);

        $.ajax({
            url: "/GrupoCliente/ChangeFiles",
            type: 'POST',
            enctype: 'multipart/form-data',
            contentType: false,
            processData: false,
            data: data,
            error: function (detalle) { },
            success: function (resultado) { }
        });


    });



    $(document).on('click', ".btnDeleteArchivo", function () {

        var nombreArchivo = event.target.id;




        //$("#btnDeleteArchivo").click(function () {
        $("#files").val("");
        $("#nombreArchivos > li").remove().end();
        $.ajax({
            url: "/GrupoCliente/DescartarArchivos",
            type: 'POST',
            dataType: 'JSON',
            data: { nombreArchivo: nombreArchivo },
            error: function (detalle) { },
            success: function (pedidoAdjuntoList) {

                $("#nombreArchivos > li").remove().end();

                for (var i = 0; i < pedidoAdjuntoList.length; i++) {

                    var liHTML = '<a href="javascript:mostrar();" class="descargar">' + pedidoAdjuntoList[i].nombre + '</a>' +
                        '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + pedidoAdjuntoList[i].nombre + '" class="btnDeleteArchivo" /></a>';

                    $('#nombreArchivos').append($('<li />').html(liHTML));
                    //      .appendTo($('#nombreArchivos'));

                }


            }
        });
    });

    $(document).on('click', "a.descargar", function () {

        //var arrrayClass = event.target.getAttribute("class").split(" ");
        var nombreArchivo = event.target.innerHTML;
        //var numeroPedido = arrrayClass[1];

        $.ajax({
            url: "/GrupoCliente/Descargar",
            type: 'POST',
            //  enctype: 'multipart/form-data',
            dataType: 'JSON',
            //  contentType: 'multipart/form-data',
            data: { nombreArchivo: nombreArchivo },
            error: function (detalle) {
                alert(detalle);
            },
            success: function (pedidoAdjunto) {
                var sampleArr = base64ToArrayBuffer(pedidoAdjunto.adjunto);
                saveByteArray(nombreArchivo, sampleArr);
            }
        });
    });




});



$(document).ready(function () {
    $('#btnUpdateByExcel').click(function (event) {
        var fileInput = $('#fileUploadExcel');
        var maxSize = fileInput.data('max-size');
        var maxSizeText = fileInput.data('max-size-text');
        var imagenValida = true;
        if (fileInput.get(0).files.length) {
            var fileSize = fileInput.get(0).files[0].size; // in bytes

            if (fileSize > maxSize) {
                $.alert({
                    title: "Archivo Inválido",
                    type: 'red',
                    content: 'El tamaño del archivo debe ser como maximo ' + maxSizeText + '.',
                    buttons: {
                        OK: function () { }
                    }
                });
                imagenValida = false;
            }


        } else {
            $.alert({
                title: "Archivo Inválido",
                type: 'red',
                content: 'Seleccione un archivo por favor.',
                buttons: {
                    OK: function () { }
                }
            });
            imagenValida = false;
        }

        if (imagenValida) {

            var that = document.getElementById('fileUploadExcel');
            var file = that.files[0];
            var form = new FormData();
            var url = $(that).data("urlSetFile");
            var reader = new FileReader();
            var mime = file.type;

            // read the image file as a data URL.
            reader.readAsDataURL(file);

            form.append('file', file);

            $('body').loadingModal({
                text: '...'
            });
            $.ajax({
                url: url,
                type: "POST",
                cache: false,
                contentType: false,
                processData: false,
                data: form,
                dataType: 'JSON',
                beforeSend: function () {
                    //$.blockUI();
                },
                success: function (response) {
                    if (response.success == "true") {
                        $.alert({
                            title: "Carga Exitosa!",
                            type: 'green',
                            content: response.message,
                            buttons: {
                                OK: function () { }
                            }
                        });

                        $('#btnBusqueda').click();
                    } else {
                        $.alert({
                            title: "Carga fallida",
                            type: 'red',
                            content: response.message,
                            buttons: {
                                OK: function () { }
                            }
                        });
                    }
                },
                error: function (error) {
                    console.log(error);
                    $.alert({
                        title: "Carga fallida",
                        type: 'red',
                        content: 'Ocurrió un error al subir el archivo.',
                        buttons: {
                            OK: function () { }
                        }
                    });
                }
            }).done(function () {
                $('body').loadingModal('hide')
            });
        }
    });
});
