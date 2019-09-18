
jQuery(function ($) {

    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_VALIDACION_PEDIDO = 'Revisar Datos del Requerimiento';
    var TITLE_EXITO = 'Operación Realizada';


    function obtenerConstantes() {
        $.ajax({
            url: "/General/GetConstantes",
            type: 'POST',
            dataType: 'JSON',
            success: function (constantes) {
                IGV = constantes.IGV;
                SIMBOLO_SOL = constantes.SIMBOLO_SOL;
                MILISEGUNDOS_AUTOGUARDADO = constantes.MILISEGUNDOS_AUTOGUARDADO;
                VARIACION_PRECIO_ITEM_PEDIDO = constantes.VARIACION_PRECIO_ITEM_PEDIDO;
            }
        });
    }


    function desactivarControles() {
        $("#ActualDepartamento").attr('disabled', 'disabled');
        $('#ActualProvincia').attr('disabled', 'disabled');
        $('#ActualDistrito').attr('disabled', 'disabled');
        $("#requerimiento_direccionEntrega").attr('disabled', 'disabled');
        $("#requerimiento_solicitante").attr('disabled', 'disabled');
        $("#btnAgregarDireccion").attr("disabled", "disabled");
        $("#btnAgregarSolicitante").attr("disabled", "disabled");
        $("#requerimiento_direccionEntrega_descripcion").removeAttr("disabled");
        $("#requerimiento_direccionEntrega_contacto").removeAttr("disabled");
        $("#requerimiento_direccionEntrega_telefono").removeAttr("disabled");
        $("#btnAgregarDireccion").hide();

    }

    $(document).ready(function () {
        obtenerConstantes();   
        desactivarControles();
        verificarSiExisteDetalle();
        
        $("#btnBusquedaRequerimientos").click();
        $("#btnAgregarProductosDesdePreciosRegistrados").click()

        var tipoRequerimiento = $("#requerimiento_tipoRequerimiento").val();

      /*  if ($("#pagina").val() == 2) {
            if ($("#idRequerimiento").val() != "") {
                showRequerimiento($("#idRequerimiento").val());
            }
        }*/

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
        $.datepicker.setDefaults($.datepicker.regional['es']);

        var fechaEntregaDesde = $("#fechaEntregaDesdeTodostmp").val();
        $("#requerimiento_fechaEntregaDesdeTodos").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEntregaDesde);

        var fechaEntregaHasta = $("#fechaEntregaHastaTodostmp").val();
        $("#requerimiento_fechaEntregaHastaTodos").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEntregaHasta);
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

    function verificarSiExisteCliente() {
        if ($("#esCliente").val() == 0) {
            if ($("#idCliente").val().trim() != "" && $("#pagina").val() == PAGINA_MANTENIMIENTO_PEDIDO_VENTA)
                $("#idCiudad").attr("disabled", "disabled");
        }
    }
   

   

    function autoGuardarRequerimiento() {
        $.ajax({
            url: "/Requerimiento/autoGuardarRequerimiento",
            type: 'POST',
            error: function () {
                setTimeout(autoGuardarRequerimiento, MILISEGUNDOS_AUTOGUARDADO);
            },
            success: function () {
                setTimeout(autoGuardarRequerimiento, MILISEGUNDOS_AUTOGUARDADO);
            }
        });
    }
   

    function verificarSiExisteDetalle() {
        //Si existen productos agregados no se puede obtener desde precios registrados
        var contador = 0;
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {
            contador++;
        });

        if (contador > 0) {
            $("#btnAgregarProductosDesdePreciosRegistrados").attr('disabled', 'disabled');
            return true;
        }
        else {
            $("#btnAgregarProductosDesdePreciosRegistrados").removeAttr('disabled');
            return false;
        }
    }



    /**
     * ################################ INICIO CONTROLES DE CLIENTE
     */


    $("#requerimiento_direccionEntrega").chosen({ placeholder_text_single: "Seleccione la Dirección de Entrega", no_results_text: "No se encontró Dirección de Entrega" }).on('chosen:showing_dropdown');

       
   

    $("#idCliente").change(function () {

        var idCliente = $(this).val();

        $.ajax({
            url: "/Requerimiento/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente)
            {
                $("#requerimiento_cliente_habilitadoModificarDireccionEntrega").val(cliente.habilitadoModificarDireccionEntrega)


                if (cliente.correoEnvioFactura == null || cliente.correoEnvioFactura == "") {
                    $.alert({
                        title: '¡Advertencia!',
                        type: 'orange',
                        content: "Cliente no cuenta con correo para enviarle la factura electrónica, edite el cliente e intente seleccionarlo nuevamente.",
                        buttons: {
                            OK: function () {
                                window.location = '/Requerimiento/Pedir';
                            }
                        }
                    });
                    return false;
                }
             
                
                if ($("#pagina").val() == PAGINA_MANTENIMIENTO_PEDIDO_VENTA)
                    $("#idCiudad").attr("disabled", "disabled");

                $("#idCiudad").attr("disabled", "disabled");


                ////Direccion Entrega
                var direccionEntregaListTmp = cliente.direccionEntregaList;




                $('#requerimiento_direccionEntrega')
                    .find('option')
                    .remove()
                    .end()
                    ;

                window.setInterval($("#requerimiento_direccionEntrega").trigger("chosen:updated"), 15000);    
              
                $('#requerimiento_direccionEntrega').append($('<option>', {
                    value: "",
                    text: "Seleccione Dirección de Entrega"
                }));
              

                for (var i = 0; i < direccionEntregaListTmp.length; i++) {
                    $('#requerimiento_direccionEntrega').append($('<option>', {
                        value: direccionEntregaListTmp[i].idDireccionEntrega,
                        text: direccionEntregaListTmp[i].direccionConSede
                    }));
                }
                //$('#requerimiento_direccionEntrega').trigger('liszt:updated');
                $('#requerimiento_direccionEntrega').prop('disabled', false)
                

            /*    window.setInterval($("#requerimiento_direccionEntrega").trigger("liszt:updated"), 15000);  
                window.setInterval($("#requerimiento_direccionEntrega").trigger("chosen:updated"), 15000); */
                $("#requerimiento_direccionEntrega").trigger("liszt:updated")
                $("#requerimiento_direccionEntrega").trigger("chosen:updated")
                $('#requerimiento_direccionEntrega').prop('disabled', false)


                //Se limpia controles de Ubigeo
                $("#ActualDepartamento").val("");
                $("#ActualProvincia").val("");
                $("#ActualDistrito").val("");
               
                toggleControlesUbigeo();
             
                $("#requerimiento_textoCondicionesPago").val(cliente.textoCondicionesPago);



                ///Solicitante
                var solicitanteListTmp = cliente.solicitanteList;

                $('#requerimiento_solicitante')
                    .find('option')
                    .remove()
                    .end()
                    ;

                $('#requerimiento_solicitante').append($('<option>', {
                    value: "",
                    text: "Seleccione Solicitante"
                }));


                for (var i = 0; i < solicitanteListTmp.length; i++) {
                    $('#requerimiento_solicitante').append($('<option>', {
                        value: solicitanteListTmp[i].idSolicitante,
                        text: solicitanteListTmp[i].nombre
                    }));
                }

                //To Do: Set horarios
                $("#requerimiento_horaEntregaDesde").val(cliente.horaInicioPrimerTurnoEntregaFormat);
                $("#requerimiento_horaEntregaHasta").val(cliente.horaFinPrimerTurnoEntregaFormat);
                $("#requerimiento_horaEntregaAdicionalDesde").val(cliente.horaInicioSegundoTurnoEntregaFormat);
                $("#requerimiento_horaEntregaAdicionalHasta").val(cliente.horaFinSegundoTurnoEntregaFormat);

                $("#requerimiento_direccionEntrega_descripcion").val("")
                $("#requerimiento_direccionEntrega_contacto").val("")
                $("#requerimiento_direccionEntrega_telefono").val("")
                $("#requerimiento_direccionEntrega_nombre").val("")
                $("#requerimiento_direccionEntrega_codigoCliente").val("")
                $("#requerimiento_direccionEntrega_direccionDomicilioLegal").val("")  
                $("#ActualDepartamento").val("");
                $("#ActualProvincia").val("");
                $("#ActualDistrito").val("");
                
               // alert("asas")
            //    return;
                location.reload();

                //window.setInterval($("#requerimiento_direccionEntrega").trigger("chosen:updated"), 5000);     
                //$("#requerimiento_direccionEntrega").chosen({ placeholder_text_single: "Seleccione la Dirección de Entrega", no_results_text: "No se encontró Dirección de Entrega" }).on('chosen:showing_dropdown');
                //$("#requerimiento_direccionEntrega").trigger("chosen:updated");
          /*      $('#requerimiento_direccionEntrega')
                    .val(1)
                    .trigger('liszt:update')
                    .removeClass('chzn-done');
                */
              /*  $('#test_chzn').remove();


                $("#test").chosen({
                    width: "220px",
                    no_results_text: "test"
                });*/
              //  $('#requerimiento_direccionEntrega').chosen("destroy").chosen();
           //     window.setInterval($("#requerimiento_direccionEntrega").trigger("chosen:updated"), 15000);     
             //   $("#requerimiento_direccionEntrega").chosen({ placeholder_text_single: "Seleccione la Dirección de Entrega", no_results_text: "No se encontró Dirección de Entrega" }).on('chosen:showing_dropdown');
                //location.reload();
            }
        });
      
    });

    

    $('#modalAgregarCliente').on('shown.bs.modal', function () {



        $("#ncRazonSocial").val("");
        $("#ncRUC").val("");
        $("#ncNombreComercial").val("");
        $("#ncDireccion").val("");
        $("#ncTelefono").val("");

        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar una ciudad previamente.");
            $("#idCiudad").focus();
            $('#btnCancelCliente').click();
            return false;
        }



    });


    $('#modalAgregarDireccion').on('shown.bs.modal', function () {
        $('#direccionEntrega_descripcion').focus();
   /*     $('#requerimiento_direccionEntrega option').each(function () {

            if ($(this).val() == GUID_EMPTY) {
                alert("");
            }
            alert();

            return true;
        });*/
    });
  

    /*
    $(window).on("paste", function (e) {
        var modalAgregarClienteIsShown = ($("#modalAgregarCliente").data('bs.modal') || { isShown: false }).isShown;
        if (modalAgregarClienteIsShown) {
            $.each(e.originalEvent.clipboardData.items, function () {
                this.getAsString(function (str) {
                   // alert(str);
                    var lineas = str.split("\t");

                    $("#ncRazonSocial").val(lineas[0]);
                    $("#ncRUC").val(lineas[1]);
                    $("#ncNombreComercial").val(lineas[2]);
                    $("#ncDireccion").val(lineas[3]);
                    $("#ncTelefono").val(lineas[4]);
                });
            });

        }
       
    });*/



    $("#btnSaveCliente").click(function () {

        if ($("#ncRazonSocial").val().trim() == "" && $("#ncNombreComercial").val().trim() == "") {
            alert("Debe ingresar la Razón Social o el Nombre Comercial.");
            $('#ncRazonSocial').focus();
            return false;
        }

        if ($("#ncRUC").val().trim() == "") {
            alert("Debe ingresar el RUC.");
            $('#ncRUC').focus();
            return false;
        }

        var razonSocial = $("#ncRazonSocial").val();
        var nombreComercial = $("#ncNombreComercial").val();
        var ruc = $("#ncRUC").val();
        var contacto = $("#ncContacto").val();
 
        $.ajax({
            url: "/Cliente/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                razonSocial: razonSocial,
                nombreComercial: nombreComercial,
                ruc: ruc,
                contacto: contacto,
                controller: "requerimiento"
            },
            error: function (detalle) {
                alert("Se generó un error al intentar crear el cliente.");
            },
            success: function (resultado) {
                alert("Se creó cliente con Código Temporal: " + resultado.codigoAlterno + ".");
                location.reload();
            }
        });


        $('#btnCancelCliente').click();

    });


    /**
     * FIN CONTROLES DE CLIENTE
     */


  
    
    /**
     * ######################## INICIO CONTROLES DE FECHAS
     */
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
    $.datepicker.setDefaults($.datepicker.regional['es']);


    var fechaSolicitud = $("#fechaSolicitudTmp").val();
    $("#fechaSolicitud").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaSolicitud);

    var fechaCreacionDesde = $("#fechaCreacionDesdetmp").val();
    $("#requerimiento_fechaCreacionDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaCreacionDesde);

    var fechaCreacionHasta = $("#fechaCreacionHastatmp").val();
    $("#requerimiento_fechaCreacionHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaCreacionHasta);
    
    var fechaEntregaDesde = $("#fechaEntregaDesdetmp").val();
    $("#requerimiento_fechaEntregaDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEntregaDesde);

    var fechaEntregaHasta = $("#fechaEntregaHastatmp").val();
    $("#requerimiento_fechaEntregaHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEntregaHasta);
    

    var fechaProgramacionDesde = $("#fechaProgramacionDesdetmp").val();
    $("#requerimiento_fechaProgramacionDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaProgramacionDesde);

    var fechaProgramacionHasta = $("#fechaProgramacionHastatmp").val();
    $("#requerimiento_fechaProgramacionHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaProgramacionHasta);


    var fechaPrecios = $("#fechaPreciostmp").val();
    $("#fechaPrecios").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaPrecios);    

    //var fechaProgramacion = $("#fechaProgramaciontmp").val();
    $("#fechaProgramacion").datepicker({ dateFormat: "dd/mm/yy" });//.datepicker("setDate", fechaProgramacion);    

    var documentoVenta_fechaEmision = $("#documentoVenta_fechaEmisiontmp").val();
    $("#documentoVenta_fechaEmision").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", documentoVenta_fechaEmision);

    var documentoVenta_fechaVencimiento = $("#documentoVenta_fechaVencimientotmp").val();
    $("#documentoVenta_fechaVencimiento").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", documentoVenta_fechaVencimiento);


    $("#requerimiento_fechaEntregaExtendida").datepicker({ dateFormat: "dd/mm/yy", minDate: 0 });


    /**
     * FIN DE CONTROLES DE FECHAS
     */



    /* ################################## INICIO CHANGE CONTROLES */
    

    $("#requerimiento_tipoRequerimiento").change(function () { 
        var tipoRequerimiento = $("#requerimiento_tipoRequerimiento").val();
 
        $.ajax({
            url: "/Requerimiento/ChangeTipoRequerimiento",
            type: 'POST',
            data: {
                tipoRequerimiento: tipoRequerimiento
            },
            success: function () { }
        });
    });

    $("#idCiudadASolicitar").change(function () {
        var idCiudadASolicitar = $("#idCiudadASolicitar").val();

        $.ajax({
            url: "/Requerimiento/ChangeIdCiudadASolicitar",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudadASolicitar: idCiudadASolicitar
            },
            error: function (detalle) {
                alert('Debe eliminar los productos agregados antes de cambiar de Sede.');
                location.reload();
            },
            success: function (ciudad) {
            }
        });
    });  


    $("#requerimiento_sku").change(function () {
        changeInputString("sku", $("#requerimiento_sku").val())
    });

    $("#requerimiento_numeroReferenciaCliente").change(function () {
        changeInputString("numeroReferenciaCliente", $("#requerimiento_numeroReferenciaCliente").val());
    });


    $("#requerimiento_numeroRequerimiento").change(function () {
        changeInputString("numeroRequerimiento", $("#requerimiento_numeroRequerimiento").val());
    });



    $("#requerimiento_otrosCargos").change(function () {
        $.ajax({
            url: "/Requerimiento/ChangeOtrosCargos",
            type: 'POST',
            data: {
                otrosCargos: $("#requerimiento_otrosCargos").val()
            },
            success: function () { }
        });
    });



    $('#requerimiento_direccionEntrega').change(function () {      
        var idDireccionEntrega = $('#requerimiento_direccionEntrega').val();
        $.ajax({
            url: "/Requerimiento/ChangeDireccionEntrega",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idDireccionEntrega: idDireccionEntrega
            },
            success: function (direccionEntrega) {
                location.reload()
            }
        })
    });





    $("#requerimiento_direccionEntrega_descripcion").change(function () {
        $.ajax({
            url: "/Requerimiento/ChangeDireccionEntregaDescripcion",
            type: 'POST',
            data: {
                direccionEntregaDescripcion: $("#requerimiento_direccionEntrega_descripcion").val()
            },
            success: function () { }
        });
    });

    $("#requerimiento_direccionEntrega_contacto").change(function () {
        $.ajax({
            url: "/Requerimiento/ChangeDireccionEntregaContacto",
            type: 'POST',
            data: {
                direccionEntregaContacto: $("#requerimiento_direccionEntrega_contacto").val()
            },
            success: function () { }
        });
    });
    
    $("#requerimiento_direccionEntrega_telefono").change(function () {
        $.ajax({
            url: "/Requerimiento/ChangeDireccionEntregaTelefono",
            type: 'POST',
            data: {
                direccionEntregaTelefono: $("#requerimiento_direccionEntrega_telefono").val()
            },
            success: function () { }
        });
    });


    $("#chkEsPagoContado").change(function () {
        var valor = 1;
        if (!$('#chkEsPagoContado').prop('checked')) {
            valor = 0;
        }
        $.ajax({
            url: "/Requerimiento/ChangeEsPagoContado",
            type: 'POST',
            data: {
                esPagoContado: valor
            },
            dataType: 'JSON',
            success: function (result) {
                $("#requerimiento_textoCondicionesPago").val(result.textoCondicionesPago);
            }
        });
    });


    




    $(".fechaSolicitud").change(function () {
        var fechaSolicitud = $("#fechaSolicitud").val();
        var horaSolicitud = $("#horaSolicitud").val();
        $.ajax({
            url: "/Requerimiento/ChangeFechaSolicitud",
            type: 'POST',
            data: {
                fechaSolicitud: fechaSolicitud,
                horaSolicitud: horaSolicitud
            },
            success: function () {
            }
        });
    });


    






    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Requerimiento/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#requerimiento_horaEntregaDesde").change(function () {

        var horaEntregaDesde = $("#requerimiento_horaEntregaDesde").val();
        var horaEntregaHasta = $("#requerimiento_horaEntregaHasta").val();

        //Si la hora de entrega desde tiene una diferencia menor a dos horas con la hora entrega hasta
        //Se reemplaza la hora de entraga hasta con la hora entrega desde más dos horas
        if (convertirHoraToNumero(horaEntregaDesde) + 1 > convertirHoraToNumero(horaEntregaHasta)) {
            $("#requerimiento_horaEntregaHasta").val(sumarHoras($("#requerimiento_horaEntregaDesde").val(),1));
            $("#requerimiento_horaEntregaHasta").change();
        }
       
        changeInputString("horaEntregaDesde", $("#requerimiento_horaEntregaDesde").val());
    });

    $("#requerimiento_horaEntregaHasta").change(function () {
        var horaEntregaDesde = $("#requerimiento_horaEntregaDesde").val();
        var horaEntregaHasta = $("#requerimiento_horaEntregaHasta").val();

        //Si la hora de entrega desde tiene una diferencia menor a dos horas con la hora entrega hasta
        //Se reemplaza la hora de entraga hasta con la hora entrega desde más dos horas
        if (convertirHoraToNumero(horaEntregaDesde) + 1 > convertirHoraToNumero(horaEntregaHasta)) {
            $("#requerimiento_horaEntregaDesde").val(sumarHoras($("#requerimiento_horaEntregaHasta").val(), -1));
            $("#requerimiento_horaEntregaDesde").change();
        }
        changeInputString("horaEntregaHasta", $("#requerimiento_horaEntregaHasta").val());

    });

    $("#requerimiento_horaEntregaAdicionalDesde").change(function () {

        var horaEntregaDesde = $("#requerimiento_horaEntregaAdicionalDesde").val();
        var horaEntregaHasta = $("#requerimiento_horaEntregaAdicionalHasta").val();

        //Si la hora de entrega desde tiene una diferencia menor a dos horas con la hora entrega hasta
        //Se reemplaza la hora de entraga hasta con la hora entrega desde más dos horas
        if (convertirHoraToNumero(horaEntregaDesde) + 1 > convertirHoraToNumero(horaEntregaHasta)) {
            $("#requerimiento_horaEntregaAdicionalHasta").val(sumarHoras($("#requerimiento_horaEntregaAdicionalDesde").val(), 1));
            $("#requerimiento_horaEntregaAdicionalHasta").change();
        }

        changeInputString("horaEntregaAdicionalDesde", $("#requerimiento_horaEntregaAdicionalDesde").val());
    });

    $("#requerimiento_horaEntregaAdicionalHasta").change(function () {

        var horaEntregaDesde = $("#requerimiento_horaEntregaAdicionalDesde").val();
        var horaEntregaHasta = $("#requerimiento_horaEntregaAdicionalHasta").val();

        //Si la hora de entrega desde tiene una diferencia menor a dos horas con la hora entrega hasta
        //Se reemplaza la hora de entraga hasta con la hora entrega desde más dos horas
        if (convertirHoraToNumero(horaEntregaDesde) + 1 > convertirHoraToNumero(horaEntregaHasta)) {
            $("#requerimiento_horaEntregaAdicionalDesde").val(sumarHoras($("#requerimiento_horaEntregaAdicionalHasta").val(), -1));
            $("#requerimiento_horaEntregaAdicionalDesde").change();
        }

        changeInputString("horaEntregaAdicionalHasta", $("#requerimiento_horaEntregaAdicionalHasta").val());
    });

    $("#requerimiento_numeroReferenciaCliente").change(function () {
        changeInputString("numeroReferenciaCliente", $("#requerimiento_numeroReferenciaCliente").val())
    });

    $("#requerimiento_observacionesFactura").change(function () {
        changeInputString("observacionesFactura", $("#requerimiento_observacionesFactura").val())
    });

    $("#requerimiento_observacionesGuiaRemision").change(function () {
        changeInputString("observacionesGuiaRemision", $("#requerimiento_observacionesGuiaRemision").val())
    });


    

    $("#requerimiento_contactoRequerimiento").change(function () {
        changeInputString("contactoRequerimiento", $("#requerimiento_contactoRequerimiento").val())
    });

    $("#requerimiento_telefonoContactoRequerimiento").change(function () {
        changeInputString("telefonoContactoRequerimiento", $("#requerimiento_telefonoContactoRequerimiento").val())
    });

    $("#requerimiento_correoContactoRequerimiento").change(function () {
        changeInputString("correoContactoRequerimiento", $("#requerimiento_correoContactoRequerimiento").val())
    });

    $("#requerimiento_observaciones").change(function () {
        changeInputString("observaciones", $("#requerimiento_observaciones").val())
    });

    /**
     * ################################ INICIO CONTROLES DE AGREGAR PRODUCTO
     */

    ////////////////ABRIR AGREGAR PRODUCTO
    $('#btnOpenAgregarProducto').click(function () {

        //Para agregar un producto se debe seleccionar una ciudad
        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar la sede MP previamente.");
            return false;
        }

        //para agregar un producto se debe seleccionar un cliente
        if ($("#idCliente").val().trim() == "") {
            alert("Debe seleccionar previamente un cliente.");
            $('#idCliente').trigger('chosen:activate');
            return false;
        }


        //Se limpia el mensaje de resultado de agregar producto
        $("#resultadoAgregarProducto").html("");

        //Se desactiva el boton de agregar producto
        desactivarBtnAddProduct();

        //Se limpian los campos
        $("#unidad").html("");
        $("#imgProducto").attr("src", "images/NoDisponible.gif");
        $("#precioUnitarioSinIGV").val(0);
        $("#precioUnitarioAlternativoSinIGV").val(0);
        $("#subtotal").val(0);
        $("#porcentajeDescuento").val(Number(0).toFixed(10));
        $('#valor').val(0);
        $('#valorAlternativo').val(0);
        $('#observacionProducto').val("");
        $('#valor').attr('type', 'text');
        $('#valorAlternativo').attr('type', 'hidden');
        $('#precio').val(0);
        $('#cantidad').val(1);


        //Se agrega chosen al campo PRODUCTO
        $("#producto").chosen({ placeholder_text_single: "Seleccione el producto", no_results_text: "No se encontró Producto" });

        $("#producto").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/Requerimiento/SearchProductos"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Seleccione el producto", no_results_text: "No se encontró Producto" });

        $('#producto').val('').trigger('chosen:updated');
        $('#producto').val('').trigger('liszt:updated');

        $('#producto')
            .find('option:first-child').prop('selected', true)
            .end().trigger('chosen:updated');

        calcularSubtotalProducto();

    });

    //EVENTO CUANDO SE ABRE VENTANA DE AGREGAR PRODUCTO
    $('#modalAgregarProducto').on('shown.bs.modal', function () {
        $('#familia').focus();
        //$('#familia').val("Todas");
        //$('#familia').change();
        //$('#proveedor').val("Todos");
        //$('#proveedor').change();

        //$('#producto').trigger('chosen:activate');
    })


    /////////////CAMPO PRODUCTO 
    $("#producto").change(function () {
        $("#resultadoAgregarProducto").html("");
        desactivarBtnAddProduct();
        $.ajax({
            url: "/Requerimiento/GetProducto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: $(this).val(),
            },
            success: function (producto) {
                // var producto = $.parseJSON(respuesta);


                $("#imgProducto").attr("src", producto.image);

                //Se agrega el precio estandar
                var options = "<option value='0' selected>" + producto.unidad + "</option>";

                for (var i = 0; i < producto.productoPresentacionList.length; i++) {
                    var reg = producto.productoPresentacionList[i];
                    options = options + "<option value='" + reg.IdProductoPresentacion + "'  precioUnitarioAlternativoSinIGV='" + reg.PrecioSinIGV + "' costoAlternativoSinIGV='" + reg.CostoSinIGV + "' stock='" + reg.Stock + "' >" + reg.Presentacion + "</option>";
                }

                /*
                if (producto.unidad_alternativa != "") {
                    //Se agrega el precio alternativo
                    options = options + "<option value='1'>" + producto.unidad_alternativa + "</option>";
                }*/

                //Limpieza de campos
                $("#costoLista").val(Number(producto.costoLista));
                //alert($("#costoLista").val());
                $("#precioLista").val(Number(producto.precioLista));
                $("#unidad").html(options);
                $("#proveedor").val(producto.proveedor);
                $("#familia").val(producto.familia);
                $('#precioUnitarioSinIGV').val(producto.precioUnitarioSinIGV);
                $('#precioUnitarioAlternativoSinIGV').val(producto.precioUnitarioAlternativoSinIGV);
                $('#costoSinIGV').val(producto.costoSinIGV);
                $('#costoAlternativoSinIGV').val(producto.costoAlternativoSinIGV);
                $('#observacionProducto').val("");
                $('#fleteDetalle').val(producto.fleteDetalle);
                $("#porcentajeDescuento").val(Number(producto.porcentajeDescuento).toFixed(10));
                $("#cantidad").val(1);
                $("#stock").val(producto.Stock);

                $("#tableMostrarPrecios > tbody").empty();

                $("#verProducto").html(producto.nombre);

                FooTable.init('#tableMostrarPrecios');
                for (var i = 0; i < producto.precioListaList.length; i++) {
                    var fechaInicioVigencia = producto.precioListaList[i].fechaInicioVigencia;
                    var fechaFinVigencia = producto.precioListaList[i].fechaFinVigencia;

                    if (fechaInicioVigencia == null)
                        fechaInicioVigencia = "No Definida";
                    else
                        fechaInicioVigencia = invertirFormatoFecha(producto.precioListaList[i].fechaInicioVigencia.substr(0, 10));

                    if (fechaFinVigencia == null)
                        fechaFinVigencia = "No Definida";
                    else
                        fechaFinVigencia = invertirFormatoFecha(producto.precioListaList[i].fechaFinVigencia.substr(0, 10));

                    var numeroCotizacion = producto.precioListaList[i].numeroCotizacion;
                    if (numeroCotizacion == null)
                        numeroCotizacion = "No Identificado";

                   

                    $("#tableMostrarPrecios").append('<tr data-expanded="true">' +

                        '<td>' + numeroCotizacion + '</td>' +
                        '<td>' + fechaInicioVigencia + '</td>' +
                        '<td>' + fechaFinVigencia + '</td>' +
                        '<td>' + producto.precioListaList[i].unidad + '</td>' +
                        '<td>' + Number(producto.precioListaList[i].precioNeto).toFixed(cantidadCuatroDecimales) + '</td>' +
                        '<td>' + Number(producto.precioListaList[i].flete).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(producto.precioListaList[i].precioUnitario).toFixed(cantidadCuatroDecimales) + '</td>' +

                        '</tr>');
                }



                //Activar Botón para agregar producto a la grilla
                activarBtnAddProduct();

                //Se calcula el subtotal del producto
                calcularSubtotalProducto();

                $('#precioRegistrado').val($('#precio').val());



            }
        });
    });






    ///////////////////CAMPO PRESENTACIÓN
    $("#unidad").change(function () {

        //0 es precio estandar 
        //1 es precio alternativo
        var codigoPrecioAlternativo = Number($("#unidad").val());
        //var esPrecioAlternativo = Number($("#unidad").val());
        //$("#esPrecioAlternativo").val(esPrecioAlternativo);

        var precioLista = 0;
        var costoLista = 0;

        if (codigoPrecioAlternativo == 0) {
            precioLista = Number($("#precioUnitarioSinIGV").val());
            costoLista = Number($("#costoSinIGV").val());
        }
        else {
            //precioLista = Number($("#precioUnitarioAlternativoSinIGV").val());
            //costoLista = Number($("#costoAlternativoSinIGV").val());

            precioLista = $("#unidad option:selected").attr("precioUnitarioAlternativoSinIGV");
            costoLista = $("#unidad option:selected").attr("costoAlternativoSinIGV");
        }

        if ($("input[name=igv]:checked").val() == 1) {
            precioLista = (precioLista + (precioLista * IGV)).toFixed(cantidadDecimales);
            costoLista = (costoLista + (costoLista * IGV)).toFixed(cantidadDecimales);
        }

        $("#precioLista").val(precioLista);
        $("#costoLista").val(costoLista);

        calcularSubtotalProducto();

        var flete = Number($("#flete").val());
        if (flete > 0) {
            alert("Tener en cuenta que al cambiar de unidad se recalcula el monto del flete.")

            var fleteDetalle = costoLista * flete / 100;
            $("#fleteDetalle").val(fleteDetalle.toFixed(cantidadDecimales));
        }
    });

    /*
    $("#porcentajeDescuento").change(function () {
        
        var descuento = Number($("#porcentajeDescuento").val());
        $("#porcentajeDescuento").val(descuento.toFixed(4));

        var precioLista = Number($("#precioLista").val());

        var porcentajeDescuento = parseFloat($("#porcentajeDescuento").val());



        var precio = $("#precio").val();

        var precio = precioLista * (100 - porcentajeDescuento) * 0.01;
        precio = precio.toFixed(cantidadDecimales);

    //    precioLista * (100 - porcentajeDescuento) * 0.01;
     //   precio = precio.toFixed(cantidadDecimales);


        calcularSubtotalProducto();
    });*/


    /////////////////////////CAMPOS PORCENTAJE DESCUENTO y CANTIDAD 
    $("#porcentajeDescuento, #cantidad").change(function () {

        var descuento = Number($("#porcentajeDescuento").val());
        /*      if (descuento > 100) {
                  descuento = 100;
              }*/
        $("#porcentajeDescuento").val(descuento.toFixed(10));
        $("#cantidad").val(Number($("#cantidad").val()).toFixed());
        calcularSubtotalProducto();
    });


    /////////////////////////CAMPO FLETE

    $("#fleteDetalle").change(function () {
        var precioUnitario = Number($('#precio').val()) + Number($('#fleteDetalle').val());
        $('#precioUnitario').val(precioUnitario.toFixed(cantidadDecimales));
        calcularSubtotalProducto();
    });


    /**
    *Función de Cálculo 
    */


    function calcularSubtotalProducto() {
        //Si es 0 quiere decir que es precio standar, si es 1 es el precio alternativo
        var esPrecioAlternativo = Number($("#unidad").val());

        //Se recuperan los valores de precioLista y costoLista
        var precioLista = Number($("#precioLista").val());
        var costoLista = Number($("#costoLista").val());

        //Se identifica si se considera o no las cantidades y se recuperar los valores necesarios
        //para los calculos

        var porcentajeDescuento = parseFloat($("#porcentajeDescuento").val());

        var precio = precioLista * (100 - porcentajeDescuento) * 0.01;
        precio = precio.toFixed(cantidadCuatroDecimales);


        var precioUnitario = Number(precio) + Number($('#fleteDetalle').val());
        $('#precioUnitario').val(precioUnitario.toFixed(cantidadCuatroDecimales));


        var considerarCantidades = $("#considerarCantidades").val();
        //Controles de Cantidad
        if (considerarCantidades == CANT_SOLO_CANTIDADES || considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES) {
            $("#cantidadiv").show();
            $('#subtotaldiv').show();
            var cantidad = parseInt($("#cantidad").val());
            //se calcula el subtotal con el precioUnitario que ya incluye el flete
            var subTotal = precioUnitario * cantidad;
            $("#subtotal").val(subTotal.toFixed(cantidadDecimales));
        }
        else {
            $("#cantidadiv").hide();
            $('#subtotaldiv').hide();
            $("#subtotal").val(0);
            $('#cantidad').val(0);
        }

        //Controles de Observaciones
        $('#observacionProducto').val("");
        if (considerarCantidades == CANT_SOLO_OBSERVACIONES || considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES) {

            $('#observacionProductoDiv').show();
        }
        else {
            $('#observacionProductoDiv').hide();
        }

        $("#precio").val(precio);

        //Se calcula margen
        margen = (1 - (Number($("#costoLista").val()) / Number(precio))) * 100;
        $("#margen").val(margen.toFixed(cantidadDecimales));

    };




    /////////EVENTO CUANDO SE ABRE CALCULADORA DE DESCUENTO
    $('#modalCalculadora').on('shown.bs.modal', function () {
        var modalAgregarProductoIsShown = ($("#modalAgregarProducto").data('bs.modal') || { isShown: false }).isShown;
        if (modalAgregarProductoIsShown) {
            //El precio se obtiene de la pantalla de agregar producto
            $('#nuevoPrecio').val($('#precio').val());
            $('#nuevoDescuento').val($('#porcentajeDescuento').val());
        }
        else {
            var idproducto = $('#idProducto').val();
            var precio = $("." + idproducto + ".detprecio").html();
            var porcentajedescuento = $("." + idproducto + ".detinporcentajedescuento").val();
            //El precio se obtiene del elemento de la grilla
            $('#nuevoPrecio').val(precio);
            $('#nuevoDescuento').val(porcentajedescuento);
        }
        $('#nuevoPrecio').focus();
    })




    //////CONTROL DE BOTONES PARA AGREGAR PRODUCTO A LA GRILLA

    function activarBtnAddProduct() {
        $('#btnAddProduct').removeAttr('disabled');
        $('#btnCalcularDescuento').removeAttr('disabled');
        $('#btnMostrarPrecios').removeAttr('disabled');


    }

    function desactivarBtnAddProduct() {
        $("#btnAddProduct").attr('disabled', 'disabled');
        $('#btnCalcularDescuento').attr('disabled', 'disabled');
        $('#btnMostrarPrecios').attr('disabled', 'disabled');
    }

    $(document).on('click', "button.btnMostrarPrecios", function () {

        var idProducto = event.target.getAttribute("class").split(" ")[0];

        var idCliente = "";
        var sessionRequerimiento = "requerimiento"
        if ($("#pagina").val() == PAGINA_BUSQUEDA_PEDIDOS_VENTA) {
            idCliente = $("#verIdCliente").val();
            sessionRequerimiento = "requerimientoVer";
        }
        else {
            idCliente = $("#idCliente").val();
            if (idCliente.trim() == "") {
                alert("Debe seleccionar un cliente.");
                $('#idCliente').trigger('chosen:activate');
                return false;
            }
        }

        //verIdCliente


        $.ajax({
            url: "/Precio/GetPreciosRegistrados",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: idProducto,
                idCliente: idCliente,
                controller: sessionRequerimiento
            },
            success: function (producto) {

                $("#verProducto").html(producto.nombre);
                $("#verCodigoProducto").html(producto.sku);
                

                var precioListaList = producto.precioLista;

                // var producto = $.parseJSON(respuesta);
                $("#tableMostrarPrecios > tbody").empty();

                FooTable.init('#tableMostrarPrecios');
                for (var i = 0; i < precioListaList.length; i++) {
                    var fechaInicioVigencia = precioListaList[i].fechaInicioVigencia;
                    var fechaFinVigencia = precioListaList[i].fechaFinVigencia;

                    if (fechaInicioVigencia == null)
                        fechaInicioVigencia = "No Definida";
                    else
                        fechaInicioVigencia = invertirFormatoFecha(precioListaList[i].fechaInicioVigencia.substr(0, 10));

                    if (fechaFinVigencia == null)
                        fechaFinVigencia = "No Definida";
                    else
                        fechaFinVigencia = invertirFormatoFecha(precioListaList[i].fechaFinVigencia.substr(0, 10));

                    var numeroCotizacion = precioListaList[i].numeroCotizacion;
                    if (numeroCotizacion == null)
                        numeroCotizacion = "No Identificado";

                    $("#tableMostrarPrecios").append('<tr data-expanded="true">' +

                        '<td>' + numeroCotizacion + '</td>' +
                        '<td>' + fechaInicioVigencia + '</td>' +
                        '<td>' + fechaFinVigencia + '</td>' +
                        '<td>' + precioListaList[i].unidad + '</td>' +
                        '<td>' + Number(precioListaList[i].precioNeto).toFixed(cantidadCuatroDecimales) + '</td>' +
                        '<td>' + Number(precioListaList[i].flete).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(precioListaList[i].precioUnitario).toFixed(cantidadCuatroDecimales) + '</td>' +

                        '</tr>');
                }
            }
        });
        $("#modalMostrarPrecios").modal();

    });
    


    $("#considerarDescontinuados").change(function () {
        var considerarDescontinuados = $('#considerarDescontinuados').prop('checked');
        $.ajax({
            url: "/Requerimiento/updateConsiderarDescontinuados",
            type: 'POST',
            data: {
                considerarDescontinuados: considerarDescontinuados
            },
            success: function () {
            }
        });
    });


    $("#btnAddProduct").click(function () {
        //Se desactiva el boton mientras se agrega el producto
        desactivarBtnAddProduct();
        var cantidad = parseInt($("#cantidad").val());
        var porcentajeDescuento = parseFloat($("#porcentajeDescuento").val());
        var precio = $("#precio").val();
        var precioLista = $("#precioLista").val();
        var costoLista = $("#costoLista").val();
        var esPrecioAlternativo = 0;
        var idProductoPresentacion = Number($("#unidad").val());
        if (idProductoPresentacion > 0)
            esPrecioAlternativo = 1;    
        var subtotal = $("#subtotal").val();
        var incluidoIGV = $("input[name=igv]:checked").val();
        var proveedor = $("#proveedor").val();
        var flete = Number($("#fleteDetalle").val());
        var observacion = $("#observacionProducto").val();
        var costo = $("#costoLista").val();
        

        $.ajax({
            url: "/Requerimiento/AddProducto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                cantidad: cantidad,
                porcentajeDescuento: porcentajeDescuento,
                precio: precio,
                costo: costo,
                esPrecioAlternativo: esPrecioAlternativo,
                idProductoPresentacion: idProductoPresentacion,
                flete: flete,
                subtotal: subtotal,
                observacion: observacion
            },
            success: function (detalle) {

             /*   var esRecotizacion = "";
                if ($("#esRecotizacion").val() == "1") {
                    esRecotizacion = '<td class="' + detalle.idProducto + ' detprecioNetoAnterior" style="text-align:right; color: #B9371B">0.00</td>' +
                        '<td class="' + detalle.idProducto + ' detvarprecioNetoAnterior" style="text-align:right; color: #B9371B">0.0 %</td>' +
                        '<td class="' + detalle.idProducto + ' detvarCosto" style="text-align:right; color: #B9371B">0.0 %</td>' +
                        '<td class="' + detalle.idProducto + ' detcostoAnterior" style="text-align:right; color: #B9371B">0.0</td>';
                }*/

                var observacionesEnDescripcion = "<br /><span class='" + detalle.idProducto + " detproductoObservacion'  style='color: darkred'>" + detalle.observacion + "</span>";


                var precios = "";

                if (detalle.precioUnitarioRegistrado == 0) {

                    if (detalle.precioUnitario >= Number(precioLista) - Number(VARIACION_PRECIO_ITEM_PEDIDO)
                        && detalle.precioUnitario <= Number(precioLista) + Number(VARIACION_PRECIO_ITEM_PEDIDO))
                    {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right">' + detalle.precioUnitario + '</td>';
                        
                    }
                    else
                    {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right; color: #B9371B; font-weight:bold">' + detalle.precioUnitario + '</td>';

                    }


                }
                else {
                    

                    if (Number(detalle.precioUnitario) >= (Number(detalle.precioUnitarioRegistrado) - Number(VARIACION_PRECIO_ITEM_PEDIDO))
                        && Number(detalle.precioUnitario) <= (Number(detalle.precioUnitarioRegistrado) + Number(VARIACION_PRECIO_ITEM_PEDIDO)))
                    {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right">' + detalle.precioUnitario + '</td>';
                        
                    }
                    else
                    {
                    precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right; color: #B9371B; font-weight:bold">' + detalle.precioUnitario + '</td>';

                    }
                }


                $('#tableDetalleRequerimiento tbody tr.footable-empty').remove();
                $("#tableDetalleRequerimiento tbody").append('<tr data-expanded="true">' +
                    '<td>' + detalle.idProducto + '</td>' +
                    '<td>' + esPrecioAlternativo + '</td>' +

                    '<td>' + proveedor + '</td>' +
                    '<td>' + detalle.codigoProducto + '</td>' +
                    '<td>' + detalle.nombreProducto + observacionesEnDescripcion + '</td>' +
                    '<td>' + detalle.unidad + '</td>' +
                    '<td class="column-img"><img class="table-product-img" src="' + $("#imgProducto").attr("src") + '"></td>' +
                    '<td class="' + detalle.idProducto + ' detprecioLista" style="text-align:right">' + precioLista + '</td>' +
                    '<td class="' + detalle.idProducto + ' detporcentajedescuento" style="text-align:right">' + porcentajeDescuento.toFixed(10) + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detporcentajedescuentoMostrar" style="width:75px; text-align:right;">' + porcentajeDescuento.toFixed(1) + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detprecio" style="text-align:right">' + precio + '</td>' +
                    '<td class="' + detalle.idProducto + ' detcostoLista">' + costoLista + '</td>' +
                    '<td class="' + detalle.idProducto + ' detmargen" style="width:70px; text-align:right; ">' + detalle.margen + ' %</td>' +

                    '<td class="' + detalle.idProducto + ' detflete" style="text-align:right">' + flete.toFixed(2) + '</td>' +
                    precios +
                    '<td class="' + detalle.idProducto + ' detprecioUnitarioRegistrado" style="text-align:right">' + detalle.precioUnitarioRegistrado + '</td>' +
                    '<td class="' + detalle.idProducto + ' detcantidad" style="text-align:right">' + cantidad + '</td>' +
                    '<td class="' + detalle.idProducto + ' detsubtotal" style="text-align:right">' + subtotal + '</td>' +
                    '<td class="' + detalle.idProducto + ' detobservacion" style="text-align:left">' + observacion + '</td>' +
                    '<td class="' + detalle.idProducto + ' detbtnMostrarPrecios"> <button  type="button" class="' + detalle.idProducto+' btnMostrarPrecios btn btn-primary bouton-image botonPrecios"></button></td>' +


                 //   esRecotizacion +

                    '<td class="' + detalle.idProducto + ' detordenamiento"></td>' +
                    '</tr > ');

                $('#tableDetalleRequerimiento thead tr th.footable-editing').remove();
                $('#tableDetalleRequerimiento tbody tr td.footable-editing').remove();


                $('#montoIGV').html(detalle.igv);
                $('#montoSubTotal').html(detalle.subTotal);
                ///var flete = Number($("#flete").val());
                $('#montoTotal').html(detalle.total);
                $("#total").val(detalle.total);
                var total = Number($("#total").val())
                $('#montoFlete').html((total * flete / 100).toFixed(cantidadDecimales));
                $('#montoTotalMasFlete').html((total + (total * flete / 100)).toFixed(cantidadDecimales));

                cargarTablaDetalle();
                // $('#tablefoottable').footable();
                $('#btnCancelAddProduct').click();




            }, error: function (detalle) {


                $.alert({
                    title: 'Error al agregar producto',
                    content: detalle.responseText,
                    type: 'orange',
                    buttons: {

                        OK: function () {
                            window.location = '/Requerimiento/Pedir';
                        }
                    }
                });


            }


        });

    });
    
    /**
    * INTERFACE PARA CALCULO DE DESCUENTO
    */
    $('#modalCalculadora').on('show', function () {
        $('#modalAgregarProducto').css('opacity', .5);
        $('#modalAgregarProducto').unbind();
    });

    $('#modalCalculadora').on('hidden', function () {
        $('#modalAgregarProducto').css('opacity', 1);
        $('#modalAgregarProducto').removeData("modal").modal({});
    });


    $("#nuevoPrecio").change(function () {
        var incluidoIGV = $("input[name=igv]:checked").val();
        var nuevoPrecioModificado = Number($('#nuevoPrecio').val());
        var nuevoPrecioInicial = 0;
        var modalAgregarProductoIsShown = ($("#modalAgregarProducto").data('bs.modal') || { isShown: false }).isShown;

        //En caso el calculo se realice al momento de agregar un producto
        if (modalAgregarProductoIsShown) {

            var esPrecioAlternativo = Number($("#unidad").val());
            //Si es el precio estandar
            nuevoPrecioInicial = Number(Number($("#precioUnitarioSinIGV").val()).toFixed(cantidadCuatroDecimales));

            //Si NO es el precio estandar (si es el precio alternativo)
            if (esPrecioAlternativo >= 1) {
                //var nuevoPrecioInicial = Number(Number($("#precioUnitarioAlternativoSinIGV").val()).toFixed(cantidadCuatroDecimales));
                var nuevoPrecioInicial = Number(Number($("#unidad option:selected").attr("precioUnitarioAlternativoSinIGV")).toFixed(cantidadDecimalesPrecioNeto));
            }
        }
        //En caso el calculo se realice al momento de editar un producto en la grilla
        else {
            //El precio inicial se obtiene del precio lista
            var idproducto = $('#idProducto').val();
            var nuevoPrecioInicial = $("." + idproducto + ".detprecioLista").html();
        }

        var nuevoDescuento = 100 - (nuevoPrecioModificado * 100 / nuevoPrecioInicial);
        $('#nuevoPrecio').val(nuevoPrecioModificado.toFixed(cantidadCuatroDecimales));
        $('#nuevoDescuento').val(nuevoDescuento.toFixed(10));
    });


    $("#btnSaveDescuento").click(function () {

        var modalAgregarProductoIsShown = ($("#modalAgregarProducto").data('bs.modal') || { isShown: false }).isShown;
        if (modalAgregarProductoIsShown) {
            $("#porcentajeDescuento").val($("#nuevoDescuento").val());
            //Revisar si se puede comentar

            $("#nuevoPrecio").val($("#precio").val());
            calcularSubtotalProducto();

        }
        else {
            //REVISAR CALCULO DE MARGEN Y PRECIO UNITARIO

            var idproducto = $('#idProducto').val();

            //Se recupera el precio calculado
            var precio = Number($("#nuevoPrecio").val());
            //Se asigna el precio calculculado en la columna precio
            $("." + idproducto + ".detprecio").text(precio.toFixed(cantidadDecimales));
            //Se asigna el descuento en el campo descuento
            $("." + idproducto + ".detinporcentajedescuento").val($("#nuevoDescuento").val());

            calcularSubtotalGrilla(idproducto);

        }

        $('#btnCancelCalculadora').click();

    });








    ////////////GENERAR PLANTILLA DE COTIZACIÓN



    $("#btnAgregarProductosDesdePreciosRegistrados").click(function () {
        var idCiudad = $("#idCiudad").val();
        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
           //alert("Debe seleccionar una dirección de entrega para cargar los productos.");
            $("#idCiudad").focus();
            $("#btnCancelarObtenerProductos").click();
            return false;
        }
        var idCliente = $("#idCliente").val();
        if (idCliente.trim() == "") {
           // alert("Debe seleccionar un cliente.");
            $('#idCliente').trigger('chosen:activate');
            $("#btnCancelarObtenerProductos").click();
            return false;
        }


        if (verificarSiExisteDetalle()) {
            //alert("No deben existir productos agregados al requerimiento.");
            return false;
        }

        $("#btnObtenerProductos").click();

    });

 


    $("#btnObtenerProductos").click(function () {
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val();
        var fecha = $("#fechaPrecios").val();
        var familia = $("#familiaBusquedaPrecios").val();
        var proveedor = $("#proveedorBusquedaPrecios").val();
        $('body').loadingModal({
            text: 'Obteniedo Productos y Precios...'
        });

        $.ajax({
            url: "/Requerimiento/obtenerProductosAPartirdePreciosRegistrados",
            data: {
                idCliente: idCliente,
                idCiudad: idCiudad,
                fecha: fecha,
                familia: familia, 
                proveedor: proveedor
            },
            type: 'POST',
            error: function () {
                $('body').loadingModal('hide')
               // alert("Ocurrió un error al armar el detalle del requerimiento a partir de los precios registrados.");
                //window.location = '/Requerimiento/Cotizador';
            },
            success: function () {
                $('body').loadingModal('hide')
           /*     $.alert({
                    title: '¡Atención!',
                    type: 'orange',
                    content: "Los productos importados no consideran los precios registrados para un grupo.",
                    buttons: {
                        OK: function () {*/
                            window.location = '/Requerimiento/Pedir';
               /*         }
                    }
                });*/
                
            }
        });

    });

    





    ////////CREAR/EDITAR COTIZACIÓN


    function validarIngresoDatosObligatoriosRequerimiento() {


        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            $("#idCiudad").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe seleccionar la sede MP previamente.',
                buttons: {
                    OK: function () {
                    }
                }
            });            
            return false;
        }

        if ($("#idCliente").val().trim() == "") {

            $('#idCliente').trigger('chosen:activate');
            $("#idCiudadAsolicitar").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe seleccionar un cliente.',
                buttons: {
                    OK: function () { }
                }
            });              
            return false;
        }


        var tipoRequerimiento = $("#requerimiento_tipoRequerimiento").val();
        //Si el tipo de requerimiento es traslado interno (84->'T')
      /*  if (tipoRequerimiento == TIPO_PEDIDO_VENTA_TRASLADO_INTERNO_ENTREGADO.charCodeAt(0)) {
            if ($("#idCiudadASolicitar").val() == "" || $("#idCiudadASolicitar").val() == null) {
                $("#idCiudadAsolicitar").focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe seleccionar a que ciudad se solicita el traslado interno.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }
        }*/


        if ($("#requerimiento_numeroReferenciaCliente").val().length > 20) {
            $("#requerimiento_numeroReferenciaCliente").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'El número de referencia del cliente no debe contener más de 20 caracteres, si el dato a ingresar es más extenso agreguelo en Observaciones Factura.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }   

        /*if ($("#requerimiento_numeroReferenciaCliente").val().trim() == "") {
            alert('Debe ingresar el número de orden de compra o requerimiento en el campo "Referencia Doc Cliente".');
            $('#requerimiento_numeroReferenciaCliente').focus();
            return false;
        }*/





        if (tipoRequerimiento == TIPO_PEDIDO_VENTA_VENTA.charCodeAt(0)
            //|| tipoRequerimiento == TIPO_PEDIDO_VENTA_TRASLADO_INTERNO_ENTREGADO.charCodeAt(0)
            || tipoRequerimiento == TIPO_PEDIDO_VENTA_COMODATO_ENTREGADO.charCodeAt(0)
            || tipoRequerimiento == TIPO_PEDIDO_VENTA_TRANSFERENCIA_GRATUITA_ENTREGADA.charCodeAt(0)
        //    || tipoRequerimiento == TIPO_PEDIDO_VENTA_PRESTAMO_ENTREGADO.charCodeAt(0)
        ) {


            if ($("#ActualDepartamento").val().trim().length == 0) {
                $("#ActualDepartamento").focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar el departamento.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }
            if ($("#ActualProvincia").val().trim().length == 0) {
                $("#ActualProvincia").focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar la provincia.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }
            if ($("#ActualDistrito").val().trim().length == 0) {
                $("#ActualDistrito").focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar el distrito.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }


            if ($("#requerimiento_direccionEntrega").val().trim() == "") {
                $('#requerimiento_direccionEntrega').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe seleccionar la dirección de entrega.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }

       /*     if ($("#requerimiento_solicitante").val().trim() == "") {
                $('#requerimiento_solicitante').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe seleccionar el solicitante.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }*/

            if ($("#requerimiento_direccionEntrega_descripcion").val().trim() == "") {
                $('#requerimiento_direccionEntrega_descripcion').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar la dirección de entrega.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }

            if ($("#requerimiento_direccionEntrega_contacto").val().trim() == "") {
                $('#requerimiento_direccionEntrega_contacto').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar el contacto de entrega.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }

            if ($("#requerimiento_direccionEntrega_telefono").val().trim() == "") {
                $('#requerimiento_direccionEntrega_telefono').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar una el telefono del contacto de entrega.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }
        }

        var fechaSolicitud = $("#fechaSolicitud").val();
        if (fechaSolicitud.trim() == "") {
            $("#fechaSolicitud").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe ingresar la fecha de la solicitud.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }

        var horaSolicitud = $("#horaSolicitud").val();
        if (horaSolicitud == null || horaSolicitud.trim() == "") {
            $("#horaSolicitud").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe ingresar la hora de la solicitud.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }


        var fechaEntregaDesde = $("#requerimiento_fechaEntregaDesde").val();
        if (fechaEntregaDesde.trim() == "") {
            $("#requerimiento_fechaEntregaDesde").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe ingresar la fecha desde cuando se puede realizar la entrega.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }

        var fechaEntregaHasta = $("#requerimiento_fechaEntregaHasta").val();
        if (fechaEntregaHasta.trim() == "") {
            $("#requerimiento_fechaEntregaHasta").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe ingresar la fecha hasta cuando se puede realizar la entrega.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }

        
        //la fecha máxima de entrega no puede ser inferior a la fecha de entrega
        if (convertirFechaNumero(fechaEntregaHasta) < convertirFechaNumero(fechaEntregaDesde)) {
            $("#fechaEntregaHasta").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'La fecha entrega hasta debe ser mayor o igual a la fecha de entrega desde.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }


         /*
        if (tipoRequerimiento == TIPO_PEDIDO_VENTA_VENTA.charCodeAt(0)
            //|| tipoRequerimiento == TIPO_PEDIDO_VENTA_TRASLADO_INTERNO_ENTREGADO.charCodeAt(0)
            || tipoRequerimiento == TIPO_PEDIDO_VENTA_COMODATO_ENTREGADO.charCodeAt(0)
            || tipoRequerimiento == TIPO_PEDIDO_VENTA_TRANSFERENCIA_GRATUITA_ENTREGADA.charCodeAt(0)
            //|| tipoRequerimiento == TIPO_PEDIDO_VENTA_PRESTAMO_ENTREGADO.charCodeAt(0)
        ) {

          if ($("#requerimiento_solicitante_nombre").val().trim() == "") {
                $('#requerimiento_solicitante_nombre').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar el nombre de la persona que realizó la solicitud.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }

         

            if ($("#requerimiento_solicitante_telefono").val().trim() == "" && $("#requerimiento_solicitante_correo").val().trim() == "") {
                $('#requerimiento_solicitante_telefono').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar un telefono y/o correo de contacto de entrega.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }
        }
        */

        if ($("#requerimiento_observacionesGuiaRemision").val().length > 200) {
            $("#requerimiento_observacionesGuiaRemision").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'El campo observaciones guía de remisión no debe contener más de 200 caracteres.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }  

        if ($("#requerimiento_observacionesFactura").val().length > 200) {
            $("#requerimiento_observacionesFactura").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'El campo observaciones factura no debe contener más de 200 caracteres.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }  
        
      
        /*
        if ($("#requerimiento_correoContactoRequerimiento").val().trim() != "" && !$("#requerimiento_correoContactoRequerimiento").val().match(/^[a-zA-Z0-9\._-]+@[a-zA-Z0-9-]{2,}[.][a-zA-Z]{2,4}$/)) {
            alert("Debe ingresar un correo válido.");
            $("#requerimiento_correoContactoRequerimiento").focus();
            return false;
        }


        */
        var contador = 0;
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {
            contador++;
        });

        if (contador == 0) {    
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe ingresar el detalle del requerimiento.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }

        return true;
    }


    function mostrarMensajeErrorProceso(texto) {

        $.alert({
            title: 'Error, contacte con TI',
            content: texto,
            type: 'orange',
            buttons: {

                OK: function () {
                   
                }
            }
        });
    }

    /*CARGA Y DESCARGA DE ARCHIVOS*/


    $('input[name=fileRequerimientos]').change(function (e) {
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
    });


    $('input:file[multiple]').change(   function (e) {
        
        cargarArchivosAdjuntos(e.currentTarget.files);
        var data = new FormData($('#formularioConArchivos')[0]);
        $.ajax({
            url: "/Requerimiento/ChangeFiles",
            type: 'POST',
            enctype: 'multipart/form-data',
            contentType: false,
            processData: false,
            data: data,
            error: function (detalle) {    },
            success: function (resultado) {     }
        });

    });


    
    $(document).on('click', ".btnDeleteArchivo", function () {

        var nombreArchivo = event.target.id;




    //$("#btnDeleteArchivo").click(function () {
        $("#files").val("");
        $("#nombreArchivos > li").remove().end();
        $.ajax({
            url: "/Requerimiento/DescartarArchivos",
            type: 'POST',
            dataType: 'JSON',
            data: { nombreArchivo: nombreArchivo},
            error: function (detalle) { },
            success: function (requerimientoAdjuntoList) {

                $("#nombreArchivos > li").remove().end();

                for (var i = 0; i < requerimientoAdjuntoList.length; i++) {

                    var liHTML = '<a href="javascript:mostrar();" class="descargar">' + requerimientoAdjuntoList[i].nombre + '</a>' +
                        '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + requerimientoAdjuntoList[i].nombre + '" class="btnDeleteArchivo" /></a>';

                    $('#nombreArchivos').append($('<li />').html(liHTML));
              //      .appendTo($('#nombreArchivos'));

                }


            }
        });
    });

    $(document).on('click', "a.descargar", function () {

        //var arrrayClass = event.target.getAttribute("class").split(" ");
        var nombreArchivo = event.target.innerHTML;
        //var numeroRequerimiento = arrrayClass[1];

        $.ajax({
            url: "/Requerimiento/Descargar",
            type: 'POST',
            //  enctype: 'multipart/form-data',
            dataType: 'JSON',
            //  contentType: 'multipart/form-data',
            data: { nombreArchivo: nombreArchivo },
            error: function (detalle) {
                alert(detalle);
            },
            success: function (archivoAdjunto) {
                var sampleArr = base64ToArrayBuffer(archivoAdjunto.adjunto);
                saveByteArray(nombreArchivo, sampleArr);
            }
        });
    });








    /*

    */


    function crearRequerimiento(continuarLuego) {
        if (!validarIngresoDatosObligatoriosRequerimiento())
            return false;


        $('body').loadingModal({
            text: 'Creando Requerimiento...'
        });
        $.ajax({
            url: "/Requerimiento/Create",
            type: 'POST',
          //  enctype: 'multipart/form-data',
            dataType: 'JSON',
          //  contentType: 'multipart/form-data',
            data: { continuarLuego: continuarLuego},
            error: function (detalle) {
                $('body').loadingModal('hide');
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function (resultado) {
                $('body').loadingModal('hide')
                $("#requerimiento_numeroRequerimiento").val(resultado.numeroRequerimiento);
                $("#idRequerimiento").val(resultado.idRequerimiento);

                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El Requerimiento número " + resultado.idRequerimiento + " fue ingresado correctamente.",
                    buttons: {
                        OK: function () { window.location = '/Requerimiento/Aprobar'; }
                    }
                });

                /*
                if (resultado.estado == ESTADO_INGRESADO) {
                   
                     
                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    //alert("El requerimiento número " + resultado.numeroRequerimiento + " fue ingresado correctamente, sin embargo requiere APROBACIÓN")
                    $("#solicitudIngresoComentario").html("El requerimiento número " + resultado.numeroRequerimiento + " fue ingresado correctamente, sin embargo requiere APROBACIÓN, debe ingresar un comentario.")
                    $("#comentarioPendienteIngreso").val(resultado.observacion);
                    $("#modalComentarioPendienteIngreso").modal({
                        show: true,
                        keyboard: false,
                        backdrop: 'static'
                    });
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    ConfirmDialog("El Requerimiento número " + resultado.numeroRequerimiento + " fue guardado correctamente. ¿Desea continuar editando ahora?", null, '/Requerimiento/CancelarCreacionRequerimiento');
                }
                else {
                    mostrarMensajeErrorProceso("El requerimiento ha tenido problemas para ser procesado; Contacte con el Administrador.");
                    window.location = '/Requerimiento/Aprobar';
                }
                */
            }
        });
    }

    function editarRequerimiento(continuarLuego) {
        if (!validarIngresoDatosObligatoriosRequerimiento())
            return false;
        $('body').loadingModal({
            text: 'Editando Requerimiento...'
        });
        $.ajax({
            url: "/Requerimiento/Update",
            type: 'POST',
            dataType: 'JSON',
            data: {
                continuarLuego: continuarLuego
            },
            error: function (detalle) {
                $('body').loadingModal('hide')
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function (resultado) {
                $('body').loadingModal('hide')
                $("#requerimiento_numeroRequerimiento").val(resultado.numeroRequerimiento);
                $("#idRequerimiento").val(resultado.idRequerimiento);

                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El Requerimiento número " + resultado.numeroRequerimiento + " fue editado correctamente.",
                    buttons: {
                        OK: function () { window.location = '/Requerimiento/Aprobar'; }
                    }
                });

                /*

                if (resultado.estado == ESTADO_INGRESADO) {
                    //alert("El requerimiento número " + resultado.numeroRequerimiento + " fue editado correctamente.");
                  
                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    //alert("El requerimiento número " + resultado.numeroRequerimiento + " fue editado correctamente, sin embargo requiere APROBACIÓN")
                    $("#solicitudIngresoComentario").html("El requerimiento número " + resultado.numeroRequerimiento + " fue editado correctamente, sin embargo requiere APROBACIÓN, debe ingresar un comentario.")
                    $("#comentarioPendienteIngreso").val(resultado.observacion);
                    $("#modalComentarioPendienteIngreso").modal({
                        show: true,
                        keyboard: false,
                        backdrop: 'static'
                    });
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    ConfirmDialog("El requerimiento número " + resultado.numeroRequerimiento + " fue guardado correctamente. ¿Desea continuar editando ahora?", null, '/Requerimiento/CancelarCreacionRequerimiento');
                }
                else {
                    //alert("El requerimiento ha tenido problemas para ser procesado; Contacte con el Administrador.");
                    mostrarMensajeErrorProceso("El requerimiento ha tenido problemas para ser procesado; Contacte con el Administrador.");
                    window.location = '/Requerimiento/Aprobar';
                }*/
            }
        });
    }

    $("#btnCancelarComentario").click(function () {
        window.location = '/Requerimiento/CancelarCreacionRequerimiento';
    });

    $("#btnAceptarComentario").click(function () {
        var codigoRequerimiento = $("#requerimiento_numeroRequerimiento").val();
        var idRequerimiento = $("#idRequerimiento").val();
        var observacion = $("#comentarioPendienteIngreso").val();
        var observacionEditable = $("#comentarioPendienteIngresoEditable").val();
        if (observacionEditable.trim().length < 15) {
            $("#comentarioPendienteIngresoEditable").focus();
            $.alert({
                title: "AGREGAR COMENTARIO",
                type: 'orange', 
                content: 'Debe ingresar al menos 15 caracteres en el comentario.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }   
        $.ajax({
            url: "/Requerimiento/updateEstadoRequerimiento",
            data: {
                idRequerimiento: idRequerimiento,
                estado: ESTADO_PENDIENTE_APROBACION,
                observacion: observacionEditable + "\n"+ observacion
            },
            type: 'POST',
            error: function (detalle) {
                mostrarMensajeErrorProceso(detalle.responseText);
                $("#btnCancelarComentario").click();
            },
            success: function () {
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El comentario del estado del requerimiento número: " + codigoRequerimiento + " fue modificado.",
                    buttons: {
                        OK: function () { window.location = '/Requerimiento/Index';  }
                    }
                });
                $("#btnCancelarComentario").click();
            }
        });

    });
    /*
        $("#btnAceptarComentarioCrediticio").click(function () {
            var codigoRequerimiento = $("#requerimiento_numeroRequerimiento").val();
            var observacion = $("#comentarioPendienteIngreso").val();
            $.ajax({
                url: "/Cotizacion/updateEstadoRequerimientoCrediticio",
                data: {
                    codigo: codigoRequerimiento,
                    estado: ESTADO_PENDIENTE_APROBACION,
                    observacion: observacion
                },
                type: 'POST',
                error: function () {
                    alert("Ocurrió un problema al intentar agregar un comentario al requerimiento.")
                    $("#btnCancelarComentario").click();
                },
                success: function () {
                    alert("El comentario del estado del requerimiento número: " + codigoRequerimiento + " se cambió correctamente.");
                    $("#btnCancelarComentario").click();
                }
            });

        });*/



    $("#btnFinalizarCreacionRequerimiento").click(function () {
        crearRequerimiento(0);
    });


    $("#btnFinalizarEdicionRequerimiento").click(function () {
        editarRequerimiento(0);
    });

    
    $("#btnContinuarEditandoLuego").click(function () {
        if ($("#requerimiento_numeroRequerimiento").val() == "" || $("#requerimiento_numeroRequerimiento").val() == null) {
            crearRequerimiento(1);
        }
        else {
            editarRequerimiento(1);
        }
    });



    

    









    $("#btnCopiar").click(function () {
        /* Get the text field */
        var copyText = document.getElementById("myInput");

        /* Select the text field */
        copyText.select();

        /* Copy the text inside the text field */
        document.execCommand("Copy");

        /* Alert the copied text */
    //    alert("Copied the text: " + copyText.value);
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









    /*VER PEDIDO*/
    $(document).on('click', "button.btnVerRequerimiento", function () {

        $('body').loadingModal({
            text: 'Abriendo Requerimiento...'
        });
        $('body').loadingModal('show');
        activarBotonesVer();
        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idRequerimiento = arrrayClass[0];
        var numeroRequerimiento = arrrayClass[1];
      //  $("#tableDetalleCotizacion > tbody").empty();
        showRequerimiento(idRequerimiento);
     
    });


    function showRequerimiento(idRequerimiento) {
        $.ajax({
            url: "/Requerimiento/Show",
            data: {
                idRequerimiento: idRequerimiento
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                //alert("Ocurrió un problema al obtener el detalle del Requerimiento N° " + numeroRequerimiento + ".");
                $('body').loadingModal('hide');
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function (resultado) {
                $('body').loadingModal('hide');
                //var cotizacion = $.parseJSON(respuesta);
                var requerimiento = resultado.requerimiento;
                var serieDocumentoElectronicoList = resultado.serieDocumentoElectronicoList;

                //  var usuario = resultado.usuario;

                $("#verIdRequerimiento").val(requerimiento.idRequerimiento);

                $('#requerimiento_numeroGrupo')
                    .find('option')
                    .remove()
                    .end()
                    ;
             


                $("#fechaEntregaDesdeProgramacion").val(invertirFormatoFecha(requerimiento.fechaEntregaDesde.substr(0, 10)));
                $("#fechaEntregaHastaProgramacion").val(invertirFormatoFecha(requerimiento.fechaEntregaHasta.substr(0, 10)));
                $("#fechaProgramaciontmp").val(invertirFormatoFecha(requerimiento.fechaEntregaDesde.substr(0, 10)));
                //Important
              
                $("#idRequerimiento").val(requerimiento.idRequerimiento);

                $("#verNumero").html(requerimiento.numeroRequerimientoString);
                if (requerimiento.numeroGrupoRequerimientoString == "") {
                    $("#verNumero").html(requerimiento.numeroRequerimientoString);
                }
                else {
                    $("#verNumero").html(requerimiento.numeroRequerimientoString);
                }

                if (requerimiento.cotizacion_tipoCotizacion == 0) {
                    $("#verCotizacionCodigo").html(requerimiento.cotizacion_numeroCotizacionString);
                }
                else if (requerimiento.cotizacion_tipoCotizacion == 1) {
                    $("#verCotizacionCodigo").html(requerimiento.cotizacion_numeroCotizacionString + " (Transitoria)");
                }
                else if (requerimiento.cotizacion_tipoCotizacion == 2) {
                    $("#verCotizacionCodigo").html(requerimiento.cotizacion_numeroCotizacionString + " (Trivial)");
                }

              

                $("#verTipoRequerimiento").html(requerimiento.tiposRequerimientoString);

                $("#verResponsableComercial").html(requerimiento.cliente_responsableComercial_codigoDescripcion + " " + requerimiento.cliente_responsableComercial_usuario_email);
                $("#verSupervisorComercial").html(requerimiento.cliente_supervisorComercial_codigoDescripcion + " " + requerimiento.cliente_supervisorComercial_usuario_email);
                $("#verAsistenteServicioCliente").html(requerimiento.cliente_asistenteServicioCliente_codigoDescripcion + " " + requerimiento.cliente_asistenteServicioCliente_usuario_email);

                /*if (requerimiento.tipoRequerimiento == TIPO_PEDIDO_VENTA_TRASLADO_INTERNO_ENTREGADO.charCodeAt(0)) {
                    $("#divReferenciaCliente").hide();
                    $("#divCiudadSolicitante").show();
                    $("#verCiudadSolicitante").html(requerimiento.cliente.ciudad.nombre);
                }
                else {*/
                $("#divReferenciaCliente").show();
                $("#divCiudadSolicitante").hide();
                //}

                $("#verCondicionesPago").html(requerimiento.textoCondicionesPago);

                $("#verFechaHorarioEntrega").html(requerimiento.fechaHorarioEntrega);

                $("#verCiudad").html(requerimiento.ciudad_nombre);
                $("#verIdCliente").val(requerimiento.cliente_idCliente);
                $("#verCliente").html(requerimiento.cliente_codigoRazonSocial);


                $("#verGrupoCliente").html(requerimiento.grupoCliente_nombre == null ? "" : requerimiento.grupoCliente_nombre);
               
                $("#verNumeroReferenciaCliente").html(requerimiento.numeroReferenciaCliente);
                $("#verNumeroReferenciaAdicional").html(requerimiento.numeroReferenciaAdicional);
                $("#verNumeroRequerimiento").html(requerimiento.numeroRequerimiento);
                
                $("#verFechaEntregaExtendida").val(requerimiento.fechaEntregaExtendidaString);

                $("#verDireccionEntrega").html(requerimiento.direccionEntrega_descripcion);
                $("#verTelefonoContactoEntrega").html(requerimiento.direccionEntrega_telefono);
                $("#verContactoEntrega").html(requerimiento.direccionEntrega_contacto);

                $("#verUsuarioCreacion").html(requerimiento.usuario_nombre);
                $("#verFechaHoraRegistro").html(requerimiento.fechaHoraRegistro);

                $("#verUbigeoEntrega").html(requerimiento.ubigeoEntrega.ToString);

                $("#verContactoRequerimiento").html(requerimiento.contactoRequerimiento);
                $("#verTelefonoCorreoContactoRequerimiento").html(requerimiento.telefonoCorreoContactoRequerimiento);

           


                $("#verFechaHoraSolicitud").html(requerimiento.fechaHoraSolicitud);

                $("#verEstado").html(requerimiento.seguimientoRequerimiento_estadoString);
                $("#verModificadoPor").html(requerimiento.seguimientoRequerimiento_usuario_nombre);
                $("#verObservacionEstado").html(requerimiento.seguimientoRequerimiento_observacion);

                $("#verEstadoCrediticio").html(requerimiento.seguimientoCrediticioRequerimiento_estadoString);
                $("#verModificadoCrediticioPor").html(requerimiento.seguimientoCrediticioRequerimiento_usuario_nombre);
                $("#verObservacionEstadoCrediticio").html(requerimiento.seguimientoCrediticioRequerimiento_observacion);


                $("#verObservaciones").html(requerimiento.observaciones);
                $("#verObservacionesFactura").html(requerimiento.observacionesFactura);
                $("#verObservacionesGuiaRemision").html(requerimiento.observacionesGuiaRemision);
                $("#verMontoSubTotal").html(Number(requerimiento.montoSubTotal).toFixed(cantidadDecimales));
                $("#verMontoIGV").html(Number(requerimiento.montoIGV).toFixed(cantidadDecimales));
                $("#verMontoTotal").html(Number(requerimiento.montoTotal).toFixed(cantidadDecimales));
                $("#documentoVenta_observaciones").val(requerimiento.observacionesFactura);

                /*      $("#verMontoSubTotalVenta").html(Number(requerimiento.venta.subTotal).toFixed(cantidadDecimales));
                      $("#verMontoIGVVenta").html(Number(requerimiento.venta.igv).toFixed(cantidadDecimales));
                      $("#verMontoTotalVenta").html(Number(requerimiento.venta.total).toFixed(cantidadDecimales));
      */
                //  nombreArchivos

            
              

                $("#tableDetalleRequerimiento > tbody").empty();

                FooTable.init('#tableDetalleRequerimiento');

                $("#formVerGuiasRemision").html("");
                $("#formVerNotasIngreso").html("");

                var d = '';
                var lista = requerimiento.requerimientoDetalleList;
                for (var i = 0; i < lista.length; i++) {

                    var imgIndicadorAprobacion = '<a data-toggle="tooltip" title="Aprobado"> <img class="table-product-img"  src="/images/semaforo_verde_small.png"  srcset="semaforo_verde_min.png 2x"/></a>';
                    if (lista[i].indicadorAprobacion == 2)
                        imgIndicadorAprobacion = '<a data-toggle="tooltip" title="Pendiente Aprobación"> <img class="table-product-img" src="/images/semaforo_naranja_small.png" srcset="semaforo_naranja_min.png 2x"/></a>';
                    else if (lista[i].indicadorAprobacion == 3)
                        imgIndicadorAprobacion = '<a data-toggle="tooltip" title="Pendiente Aprobación"><img class="table-product-img " src="/images/semaforo_rojo_small.png" srcset="semaforo_rojo_min.png 2x"/></a>';


                    var observacion = lista[i].observacion == null || lista[i].observacion == 'undefined' ? '' : lista[i].observacion;

                    d += '<tr>' +
                        '<td>' + imgIndicadorAprobacion + '</td>' +
                        '<td>' + lista[i].producto.proveedor + '</td>' +
                        '<td>' + lista[i].producto.sku + '</td>' +
                        '<td>' + lista[i].producto.descripcion + '</td>' +
                        '<td>' + lista[i].unidad + '</td>' +
                        '<td class="column-img"><img class="table-product-img" src="data:image/png;base64,' + lista[i].producto.image + '"> </td>' +
                        '<td>' + lista[i].precioLista.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].porcentajeDescuentoMostrar.toFixed(cantidadDecimales) + ' %</td>' +
                        '<td>' + lista[i].precioNeto.toFixed(cantidadCuatroDecimales) + '</td>' +
                        '<td>' + lista[i].margen.toFixed(cantidadDecimales) + ' %</td>' +
                        '<td>' + lista[i].flete.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].precioUnitario.toFixed(cantidadCuatroDecimales) + '</td>' +
                        //       '<td>' + lista[i].precioUnitarioVenta.toFixed(cantidadCuatroDecimales) + '</td>' +
                        '<td>' + lista[i].cantidad + '</td>' +
                        '<td>' + lista[i].cantidadPendienteAtencion + '</td>' +
                        '<td>' + lista[i].subTotal.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + observacion + '</td>' +
                        '<td class="' + lista[i].producto.idProducto + ' detbtnMostrarPrecios"> <button  type="button" class="' + lista[i].producto.idProducto + ' btnMostrarPrecios btn btn-primary bouton-image botonPrecios"></button></td>' +
                        '</tr>';
                }

             


                $("#verRazonSocialSunat").html(requerimiento.cliente_razonSocialSunat);
                $("#verRUC").html(requerimiento.cliente_ruc);
                $("#verDireccionDomicilioLegalSunat").html(requerimiento.cliente_direccionDomicilioLegalSunat);
                $("#verCodigo").html(requerimiento.cliente_codigo);

                $("#documentoVenta_observaciones").val(requerimiento.observacionesFactura);
                $("#verCorreoEnvioFactura").html(requerimiento.cliente_correoEnvioFactura);



                
                //  
                // sleep
                $("#tableDetalleRequerimiento").append(d);

                if (requerimiento.seguimientoRequerimiento_estado != ESTADO_PROGRAMADO
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_ATENDIDO
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_FACTURADO
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_FACTURADO_PARCIALMENTE
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_ATENDIDO_PARCIALMENTE) {
                    $("#btnEditarRequerimiento").show();
                    if (requerimiento.seguimientoRequerimiento_estado == ESTADO_EN_EDICION) {
                        $("#btnEditarRequerimiento").html("Continuar Editando");
                    }
                    else {
                        $("#btnEditarRequerimiento").html("Editar");
                    }
                    $("#btnEditarRequerimiento").show();
                }
                else {
                    $("#btnEditarRequerimiento").hide();
                }

                //ACTUALIZAR PEDIDO
                if (requerimiento.seguimientoRequerimiento_estado != ESTADO_EN_EDICION
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_DENEGADO
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_ELIMINADO) {
                    $("#btnActualizarRequerimiento").show();
                }
                else {
                    $("#btnActualizarRequerimiento").hide();
                }

                
                /**/
                if (requerimiento.seguimientoRequerimiento_estado == ESTADO_ATENDIDO) {
                    $("#requerimiento_observacionesGuiaRemision").attr("disabled", "disabled");
                }
                else {
                    $("#requerimiento_observacionesGuiaRemision").removeAttr("disabled");
                }

                if (requerimiento.seguimientoRequerimiento_estado == ESTADO_FACTURADO) {
                    $("#requerimiento_observacionesFactura").attr("disabled", "disabled");
                }
                else {
                    $("#requerimiento_observacionesFactura").removeAttr("disabled");
                }


                

                //APROBAR PEDIDO
                if (
                    (requerimiento.seguimientoRequerimiento_estado == ESTADO_PENDIENTE_APROBACION ||
                        requerimiento.seguimientoRequerimiento_estado == ESTADO_DENEGADO)
                ) {

                    $("#btnAprobarIngresoRequerimiento").show();
                }
                else {
                    $("#btnAprobarIngresoRequerimiento").hide();
                }

                
                //DENEGAR PEDIDO
                if (requerimiento.seguimientoRequerimiento_estado == ESTADO_PENDIENTE_APROBACION) {

                    $("#btnDenegarIngresoRequerimiento").show();
                }
                else {
                    $("#btnDenegarIngresoRequerimiento").hide();
                }

                //LIBERAR PEDIDO
                if (
                    requerimiento.seguimientoCrediticioRequerimiento_estado == ESTADO_PENDIENTE_LIBERACION ||
                    requerimiento.seguimientoCrediticioRequerimiento_estado == ESTADO_BLOQUEADO
                ) {

                    $("#btnLiberarRequerimiento").show();
                }
                else {
                    $("#btnLiberarRequerimiento").hide();
                }

                //BLOQUEAR PEDIDO
                if (
                    (requerimiento.seguimientoCrediticioRequerimiento_estado == ESTADO_PENDIENTE_LIBERACION ||
                        requerimiento.seguimientoCrediticioRequerimiento_estado == ESTADO_LIBERADO)
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_ELIMINADO
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_EN_EDICION
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_DENEGADO
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_ATENDIDO
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_ATENDIDO_PARCIALMENTE
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_FACTURADO
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_FACTURADO_PARCIALMENTE

                ) {

                    $("#btnBloquearRequerimiento").show();
                }
                else {
                    $("#btnBloquearRequerimiento").hide();
                }
                

                //PROGRAMAR PEDIDO
                if (requerimiento.seguimientoRequerimiento_estado == ESTADO_INGRESADO
                    || requerimiento.seguimientoRequerimiento_estado == ESTADO_ATENDIDO_PARCIALMENTE
                ) {

                    $("#btnProgramarRequerimiento").show();
                }
                else {
                    $("#btnProgramarRequerimiento").hide();
                }

                //ATENDER PEDIDO
                $("#btnAtenderRequerimientoVenta").hide();
                $("#btnIngresarRequerimientoVenta").hide();
                $("#btnVerAtenciones").hide();
                $("#btnVerIngresos").hide();


                if ((requerimiento.seguimientoRequerimiento_estado == ESTADO_INGRESADO ||
                    requerimiento.seguimientoRequerimiento_estado == ESTADO_PROGRAMADO ||
                    requerimiento.seguimientoRequerimiento_estado == ESTADO_ATENDIDO_PARCIALMENTE
                ) &&
                    requerimiento.seguimientoCrediticioRequerimiento_estado == ESTADO_LIBERADO
                ) {
                    if (requerimiento.tipoRequerimiento == TIPO_PEDIDO_VENTA_VENTA.charCodeAt(0)
                        //|| requerimiento.tipoRequerimiento == TIPO_PEDIDO_VENTA_TRASLADO_INTERNO_ENTREGADO.charCodeAt(0)
                        || requerimiento.tipoRequerimiento == TIPO_PEDIDO_VENTA_COMODATO_ENTREGADO.charCodeAt(0)
                        || requerimiento.tipoRequerimiento == TIPO_PEDIDO_VENTA_TRANSFERENCIA_GRATUITA_ENTREGADA.charCodeAt(0)
                        //|| requerimiento.tipoRequerimiento == TIPO_PEDIDO_VENTA_PRESTAMO_ENTREGADO.charCodeAt(0)
                    ) {
                        $("#btnAtenderRequerimientoVenta").show();



                    }
                    else {
                        $("#btnIngresarRequerimientoVenta").show();


                    }
                }

                

                if (requerimiento.seguimientoRequerimiento_estado == ESTADO_RECIBIDO
                    || requerimiento.seguimientoRequerimiento_estado == ESTADO_RECIBIDO_PARCIALMENTE
                ) {
                    $("#btnVerIngresos").show();
                }
                else {
                    $("#btnVerIngresos").hide();
                }

                if (requerimiento.seguimientoRequerimiento_estado == ESTADO_ATENDIDO
                    || requerimiento.seguimientoRequerimiento_estado == ESTADO_ATENDIDO_PARCIALMENTE
                    || requerimiento.seguimientoRequerimiento_estado == ESTADO_FACTURADO
                    || requerimiento.seguimientoRequerimiento_estado == ESTADO_FACTURADO_PARCIALMENTE
                ) {
                    $("#btnVerAtenciones").show();
                }
                else {
                    $("#btnVerAtenciones").hide();
                }

                //CANCELAR PROGRAMACION
                if (requerimiento.seguimientoRequerimiento_estado == ESTADO_PROGRAMADO) {
                    $("#btnCancelarProgramacionRequerimiento").show();
                }
                else {
                    $("#btnCancelarProgramacionRequerimiento").hide();
                }


                //PROGRAMAR PEDIDO
                if ((requerimiento.seguimientoRequerimiento_estado == ESTADO_ATENDIDO)
                    && (requerimiento.documentoVenta_numero == "" || requerimiento.documentoVenta_numero == null)
                ) {
                    $("#btnEditarVenta").show();
                    // $("#btnFacturarRequerimiento").show();
                }
                else {
                    $("#btnEditarVenta").hide();
                    // $("#btnFacturarRequerimiento").hide();
                }


                





                if (requerimiento.seguimientoRequerimiento_estado != ESTADO_ATENDIDO
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_ATENDIDO_PARCIALMENTE
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_ELIMINADO
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_FACTURADO
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_FACTURADO_PARCIALMENTE
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_RECIBIDO
                    && requerimiento.seguimientoRequerimiento_estado != ESTADO_RECIBIDO_PARCIALMENTE
                ) {

                    $("#btnEliminarRequerimiento").show();
                }
                else {
                    $("#btnEliminarRequerimiento").hide();
                }
                $("#modalVerRequerimiento").modal('show');

                //  window.location = '/Requerimiento/Index';
            }
        });
    }




   
    $("#btnCancelarRequerimiento").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/Requerimiento/CancelarCreacionRequerimiento', null)
    })


    

    $("#btnFacturarRequerimiento").click(function () {
        $("#facturacion").show(); 
    ;

      /*  $(window).load(function () {
            $("html, body").animate({ scrollTop: $(document).height() }, 1000);
        });*/
        desactivarBotonesVer();
    //    window.scrollTo(0, $("#facturacion").height());
      //  $('html, body').css({ "max-height": $(window).height() , "overflow-y": "auto" });
      // $('#facturacion').css({ "min-height": 312, "overflow-y": "auto" });
    });

    $("#btnCancelarFacturarRequerimiento").click(function () {

        $("#facturacion").hide();
        activarBotonesVer();
    });

    
    $("#btnEditarVenta").click(function () {
        desactivarBotonesVer();

        $.ajax({
            url: "/Venta/iniciarEdicionVenta",
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al iniciar la edición de la venta."); },
            success: function (fileName) {
                window.location = '/Venta/Vender';
            }
        });
        
    });


  


    $("#btnEditarRequerimiento").click(function () {
        desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona
        $.ajax({
            url: "/Requerimiento/ConsultarSiExisteRequerimiento",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/Requerimiento/iniciarEdicionRequerimiento",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del requerimiento."); },
                        success: function (fileName) {
                            window.location = '/Requerimiento/Pedir';
                        }
                    });

                }
                else {
                    if(resultado.numero == 0) {
                        alert('Está creando un nuevo requerimiento; para continuar por favor diríjase a la página "Crear/Modificar Requerimiento" y luego haga clic en el botón Cancelar, Finalizar Creación o Guardar y Continuar Editando Luego.');
                    }
                        else {
                        if(resultado.numero == $("#verNumero").html())
                                alert('Ya se encuentra editando el requerimiento número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Requerimiento".');
                            else
                                alert('Está editando el requerimiento número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Requerimiento" y luego haga clic en el botón Cancelar, Finalizar Edición o Guardar y Continuar Editando Luego.');
                    }
                    activarBotonesVer();
                }
            }
        });
    });


    $("#btnGuardarActualizarRequerimiento").click(function () {

        /*    $("#btnGuardarArchivosAdjuntos").click(function () {
        $.ajax({
            url: "/Requerimiento/UpdateArchivosAdjuntos",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                mostrarMensajeErrorProceso();
            },
            success: function (resultado) {
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El requerimiento número " + resultado.numeroRequerimiento + " fue editado correctamente.",
                    buttons: {
                       OK: function() {
                            $("#btnCancelarVerRequerimiento").click();
                        }
                        //OK: function () { window.location = '/Requerimiento/Index'; }
                    }
                });
            }
        });
    })*/

        if ($("#requerimiento_numeroReferenciaCliente2").val().length > 20) {
            $("#requerimiento_numeroReferenciaCliente2").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'El número de referencia del cliente no debe contener más de 20 caracteres, si el dato a ingresar es más extenso agreguelo en Observaciones Factura.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }   


        var numeroReferenciaCliente = $("#requerimiento_numeroReferenciaCliente2").val();
        var numeroReferenciaAdicional = $("#requerimiento_numeroReferenciaAdicional").val();

        var fechaEntregaExtendida = $("#requerimiento_fechaEntregaExtendida").val();

        var observaciones = $("#requerimiento_observaciones").val();
        var observacionesGuiaRemision = $("#requerimiento_observacionesGuiaRemision").val();
        var observacionesFactura = $("#requerimiento_observacionesFactura").val();

        var requerimientoNumeroGrupo = $("#requerimiento_numeroGrupo").val();
        


        $.ajax({
            url: "/Requerimiento/UpdatePost",
         /*   data: {
                idRequerimiento: idRequerimiento
            },
           */
            type: 'POST',
            data: {
                numeroReferenciaCliente: numeroReferenciaCliente,
                numeroReferenciaAdicional: numeroReferenciaAdicional,
                fechaEntregaExtendida: fechaEntregaExtendida,
                observaciones: observaciones,
                observacionesGuiaRemision: observacionesGuiaRemision,
                observacionesFactura: observacionesFactura,
                requerimientoNumeroGrupo: requerimientoNumeroGrupo
            },
            dataType: 'JSON',
            error: function (detalle) {
                //alert("Ocurrió un problema al obtener el detalle del Requerimiento N° " + numeroRequerimiento + ".");
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function (resultado) {
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El requerimiento número " + resultado.numeroRequerimiento + " fue actualizado correctamente.",
                    buttons: {
                        OK: function () {
                            $("#btnCancelarActualizarRequerimiento").click();
                            window.location = '/Requerimiento/Index';
                        }
                        //OK: function () { window.location = '/Requerimiento/Index'; }
                    }
                });


            }
        })
    });
    



    $("#btnActualizarRequerimiento").click(function () {
        $("#verActualizarNumero").html($("#verNumero").html());
        $("#verActualizarCiudad").html($("#verCiudad").html());
        $("#verActualizarCliente").html($("#verCliente").html());

        $("#verActualizarMontoSubTotal").html($("#verMontoSubTotal").html());
        $("#verActualizarMontoIGV").html($("#verMontoIGV").html());
        $("#verActualizarMontoTotal").html($("#verMontoTotal").html());

        $("#requerimiento_numeroReferenciaCliente2").val($("#verNumeroReferenciaCliente").html());
        $("#requerimiento_numeroReferenciaAdicional").val($("#verNumeroReferenciaAdicional").html());

        $("#requerimiento_fechaEntregaExtendida").val($("#verFechaEntregaExtendida").val());

        $("#requerimiento_observacionesFactura").val($("#verObservacionesFactura").html());
        $("#requerimiento_observacionesGuiaRemision").val($("#verObservacionesGuiaRemision").html());
        $("#requerimiento_observaciones").val($("#verObservaciones").html());
        
    });


  




    function limpiarComentario()
    {
        $("#comentarioEstado").val("");
        $("#comentarioEstado").focus();
    }



    $("#btnLiberarRequerimiento").click(function () {

        $("#labelNuevoEstadoCrediticio").html(ESTADO_LIBERADO_STR);
        $("#estadoCrediticioId").val(ESTADO_LIBERADO);
        limpiarComentario();
    });

    $("#btnBloquearRequerimiento").click(function () {

        $("#labelNuevoEstadoCrediticio").html(ESTADO_BLOQUEADO_STR);
        $("#estadoCrediticioId").val(ESTADO_BLOQUEADO);
        limpiarComentario();
    });

    $("#btnAprobarIngresoRequerimiento").click(function () {
        $("#modalAprobacionTitle").html(TITULO_APROBAR_INGRESO);
        $("#labelNuevoEstado").html(ESTADO_INGRESADO_STR);
        $("#estadoId").val(ESTADO_INGRESADO);
        limpiarComentario();
    });

    $("#btnEliminarRequerimiento").click(function () {
        $("#modalAprobacionTitle").html(TITULO_ELIMINAR);
        $("#labelNuevoEstado").html(ESTADO_ELIMINADO_STR);
        $("#estadoId").val(ESTADO_ELIMINADO);
        limpiarComentario();
    });

    $("#btnDenegarIngresoRequerimiento").click(function () {
        $("#modalAprobacionTitle").html(TITULO_DENEGAR_INGRESO);
        $("#labelNuevoEstado").html(ESTADO_DENEGADO_STR);
        $("#estadoId").val(ESTADO_DENEGADO);
        limpiarComentario();
    });



    $("#btnCancelarProgramacionRequerimiento").click(function () {
        $("#modalAprobacionTitle").html(TITULO_CANCELAR_PROGRAMACION);
        $("#labelNuevoEstado").html(ESTADO_INGRESADO_STR);
        $("#estadoId").val(ESTADO_INGRESADO);
        limpiarComentario();
    });





    $('#modalAprobacion').on('shown.bs.modal', function (e) {
        limpiarComentario();
    });




    $("#btnAceptarCambioEstadoCrediticio").click(function () {

        var estado = $("#estadoCrediticioId").val();
        var comentarioEstado = $("#comentarioEstadoCrediticio").val();

        if ($("#labelNuevoEstadoCrediticio").html() == ESTADO_BLOQUEADO_STR) {
            if (comentarioEstado.trim() == "") {
                alert("Cuando Bloquea un requerimiento debe ingresar un Comentario.");
                return false;
            }
        }
        var codigo = $("#verNumero").html();
        var idRequerimiento = $("#idRequerimiento").val();

        $.ajax({
            url: "/Requerimiento/updateEstadoRequerimientoCrediticio",
            data: {
                idRequerimiento: idRequerimiento,
                estado: estado,
                observacion: comentarioEstado
            },
            type: 'POST',
            error: function () {
                alert("Ocurrió un problema al intentar cambiar el estado del requerimiento.")
                $("#btnCancelarCambioEstadoCrediticio").click();
            },
            success: function () {
                alert("El estado crediticio del requerimiento número: " + codigo + " se cambió correctamente.");
                location.reload();
            }
        });
    });


    $("#btnAceptarCambioEstado").click(function () {

        var estado = $("#estadoId").val();
        var comentarioEstado = $("#comentarioEstado").val();

        if ($("#labelNuevoEstado").html() == ESTADO_DENEGADO_STR
            || $("#modalAprobacionTitle").html() == TITULO_CANCELAR_PROGRAMACION)
        {
            if (comentarioEstado.trim() == "") {
                alert("Debe ingresar un Comentario.");
                return false;
            }
        }
        var codigo = $("#verNumero").html();
        var idRequerimiento = $("#idRequerimiento").val();        

        $.ajax({
            url: "/Requerimiento/updateEstadoRequerimiento",
            data: {
                idRequerimiento: idRequerimiento,
                estado: estado,
                observacion: comentarioEstado
            },
            type: 'POST',
            error: function () {
                alert("Ocurrió un problema al intentar cambiar el estado del requerimiento.")
                $("#btnCancelarCambioEstado").click();
            },
            success: function () {
                alert("El estado del requerimiento número: " + codigo + " se cambió correctamente.");
                location.reload();
            }
        });
    });




    
    var ft = null;



    //Mantener en Session cambio de Seleccion de IGV
    $("input[name=igv]").on("click", function () {
        var igv = $("input[name=igv]:checked").val();
        $.ajax({
            url: "/Requerimiento/updateSeleccionIGV",
            type: 'POST',
            data: {
                igv: igv
            },
            success: function (cantidad) {
                if (cantidad > 0) {
                    location.reload();
                }
            }
        });
    });

    //Mantener en Session cambio de Seleccion de IGV
    $("#considerarCantidades").change( function () {
        var considerarCantidades = $("#considerarCantidades").val();
        $.ajax({
            url: "/Requerimiento/updateSeleccionConsiderarCantidades",
            type: 'POST',
            data: {
                considerarCantidades: considerarCantidades
            },
            success: function (cantidad)
            {
                if (cantidad > 0)
                {
                    location.reload();
                }
            }
        });


    });



    

    //Mantener en Session cambio de Seleccion de Mostrar Proveedor
    $("input[name=mostrarcodproveedor]").on("click", function () {
        var mostrarcodproveedor = $("input[name=mostrarcodproveedor]:checked").val();
        $.ajax({
            url: "/Requerimiento/updateMostrarCodigoProveedor",
            type: 'POST',
            data: {
                mostrarcodproveedor: mostrarcodproveedor
            },
            success: function () {
                location.reload();
            }
        });
    });

    //Mantener en Session cambio de Cliente
    $("#cliente").change(function () {

        $.ajax({
            url: "/Requerimiento/updateCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                cliente: $("#cliente").val()
            },
            success: function () { }
        });
    });




    $("#contacto").change(function () {

        $.ajax({
            url: "/Requerimiento/updateContacto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                contacto: $("#contacto").val()
            },
            success: function () { }
        });
    });

    $("#flete").change(function () {

        $("#flete").val(Number($("#flete").val()).toFixed(cantidadDecimales))
        var flete = $("#flete").val(); 
        if (flete > 100)
        {
            $("#flete").val("100.00");
            flete = 100;
        }

        var total = Number($("#total").val());
        $('#montoFlete').html("Flete: " + SIMBOLO_SOL + " " + (total * flete / 100).toFixed(cantidadDecimales));
        $('#montoTotalMasFlete').html("Total más Flete: " + SIMBOLO_SOL + " " +  (total + (total * flete / 100)).toFixed(cantidadDecimales));

        


        $.ajax({
            url: "/Requerimiento/updateFlete",
            type: 'POST',
            data: {
                flete: flete
            },
            success: function () {
                location.reload();
            }
        });
    });

    

    $("#mostrarCosto").change(function () {
        var mostrarCosto = $('#mostrarCosto').prop('checked') ;
        $.ajax({
            url: "/Requerimiento/changeMostrarCosto",
            type: 'POST',
            data: {
                mostrarCosto: mostrarCosto
            },
            success: function () {
                location.reload();
            }
        });
    });

    





    /*####################################################
    EVENTOS DE LA GRILLA
    #####################################################*/


    /**
     * Se definen los eventos de la grilla
     */
    function cargarTablaDetalle() {
        var $modal = $('#tableDetalleRequerimiento'),
            $editor = $('#tableDetalleRequerimiento'),
            $editorTitle = $('#tableDetalleRequerimiento');

     
        ft = FooTable.init('#tableDetalleRequerimiento', {
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
                                                    url: "/Requerimiento/DelProducto",
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

        var codigo = $("#requerimiento_numeroRequerimiento").val();
        if (codigo == "") {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarCreacionRequerimiento").attr('disabled', 'disabled');
            $("#btnCancelarRequerimiento").attr('disabled', 'disabled');
        }
        else {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarEdicionRequerimiento").attr('disabled', 'disabled');
            $("#btnCancelarRequerimiento").attr('disabled', 'disabled');
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
            var cantidad = value.innerText.trim();
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detincantidad form-control' value='" + cantidad + "' type='number'/>";
        });

        var considerarCantidades = $("#considerarCantidades").val();
        if (considerarCantidades == CANT_SOLO_OBSERVACIONES) {
            /*Se agrega control input en columna observacion*/
            var $j_object = $("td.detobservacion");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var observacion = value.innerText.trim();
                value.innerHTML = "<textarea class='" + arrId[0] + " detobservacionarea form-control'/>" + observacion + "</textarea>";
            });
        }
        else if (considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES) 
        {
            var $j_object = $("span.detproductoObservacion");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var observacion = value.innerText.trim();
                value.innerHTML = "<textarea class='" + arrId[0] + " detobservacionarea form-control'/>" + observacion + "</textarea>";
            });

        }

     //   @cotizacionDetalle.producto.idProducto detproductoObservacion"




        /*Se agrega control input en columna porcentaje descuento*/
        var $j_object1 = $("td.detporcentajedescuento");
        $.each($j_object1, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var porcentajedescuento = value.innerText.trim();
            porcentajedescuento = porcentajedescuento.replace("%", "").trim();
            $(".detporcentajedescuentoMostrar." + arrId[0]).html("<div style='width: 150px' ><div style='float:left' ><input style='width: 100px' class='" + arrId[0] + " detinporcentajedescuento form-control' value='" + porcentajedescuento + "' type='number'/></div><div > <button type='button' class='" + arrId[0] + " btnCalcularDescuento btn btn-primary bouton-image monBouton' data-toggle='modal' data-target='#modalCalculadora' ></button ></div></div>");

        });


        var $j_objectFlete = $("td.detflete");
        $.each($j_objectFlete, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var flete = value.innerText.trim();
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detinflete form-control' value='" + flete + "' type='number'/>";
        });



    });






    /*Evento que se dispara cuando se hace clic en FINALIZAR en la edición de la grilla*/
    $(document).on('click', "button.footable-hide", function () {

        //Se habilitan controles
        $("#considerarCantidades").removeAttr('disabled');
        $("input[name=igv]").removeAttr('disabled');
        $("#flete").removeAttr('disabled');
        $("#btnOpenAgregarProducto").removeAttr('disabled');
        $("input[name=mostrarcodproveedor]").removeAttr('disabled');

        //  $(".ordenar").attr('data-visible', 'false');
 //       $(".updown").hide();
 //       $(".ordenar, .detordenamiento").width('0px');
       // FooTable.init();
      //  <th class="ordenar" data-name="ordenar" data-visible="true"></th>

        var json = "[ ";
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {
            var arrId = value.getAttribute("class").split(" ");

             /*Se elimina control input en columna cantidad*/
            var cantidad = $("." + arrId[0] + ".detincantidad").val();
            value.innerText = cantidad;

            /*Se elimina control input en columna porcentaje descuento*/
            var porcentajeDescuento = $("." + arrId[0] + ".detinporcentajedescuento").val();
            $("." + arrId[0] + ".detporcentajedescuento").text(porcentajeDescuento + " %");

            var margen = $("." + arrId[0] + ".detmargen").text().replace("%", "").trim();
            var precio = $("." + arrId[0] + ".detprecio").text();
          //  var subtotal = $("." + arrId[0] + ".detsubtotal").text();
            var flete = $("." + arrId[0] + ".detinflete").val();

            var costo = $("." + arrId[0] + ".detcostoLista").text();

            var observacion = $("." + arrId[0] + ".detobservacionarea").val(); 

            json = json + '{"idProducto":"' + arrId[0] + '", "cantidad":"' + cantidad + '", "porcentajeDescuento":"' + porcentajeDescuento + '", "precio":"' + precio + '", "flete":"' + flete + '",  "costo":"' + costo + '", "observacion":"' + observacion+'"},' 
        });
        json = json.substr(0, json.length - 1) + "]";

    
        /*
        var cotizacionDetalleJson = [
            { "idProducto": "John", "cantidad": "1", "porcentajeDescuento": "0" },
            { "idProducto": "Anna", "cantidad": "1", "porcentajeDescuento": "0" },
            { "idProducto": "Peter", "cantidad": "1", "porcentajeDescuento": "0" }];
        var   json3 = JSON.stringify(cotizacionDetalleJson);*/

        
        $.ajax({
            url: "/Requerimiento/ChangeDetalle",
            type: 'POST',
            data: json,
            dataType: 'json',
            contentType: 'application/json',
            success: function (respuesta) {
                location.reload();
            }
        });
    });

    /*Evento que se dispara cuando se hace clic en el boton calcular descuento de la grilla*/
    $(document).on('click', "button.btnCalcularDescuento", function () {
        var idProducto = event.target.getAttribute("class").split(" ")[0];
        $("#idProducto").val(idProducto);
    });



    /*Evento que se dispara cuando se modifica un control de descuento de la grilla*/
    $(document).on('change', "input.detinporcentajedescuento", function () {
        var idproducto = event.target.getAttribute("class").split(" ")[0];
        calcularSubtotalGrilla(idproducto);
    });

    /*Evento que se dispara cuando se modifica un control de cantidad de la grilla*/
    $(document).on('change', "input.detincantidad", function () {
        var idproducto = event.target.getAttribute("class").split(" ")[0];
        calcularSubtotalGrilla(idproducto);
    });


    /*Evento que se dispara cuando se modifica un control de cantidad de la grilla*/
    $(document).on('change', "input.detinflete", function () {
        var idproducto = event.target.getAttribute("class").split(" ")[0];
        calcularSubtotalGrilla(idproducto);
    });

    /*Evento que se dispara cuando se modifica el color en la grilla*/
/*    $(document).on('change', "input.detobservacioncolor", function () {
        var idproducto = event.target.getAttribute("class").split(" ")[0];

        alert(event.target.value);

        /*


        class="@cotizacionDetalle.producto.idProducto fila"

        #00ff40*/
    //});

    

    function calcularSubtotalGrilla(idproducto)
    {
        //Se obtiene el porcentaje descuento 
        var porcentajeDescuento = Number($("." + idproducto + ".detinporcentajedescuento").val());
        //Se obtiene el flete
        var flete = Number($("." + idproducto + ".detinflete").val());
        //Se obtiene el precio lista
        var precioLista = Number($("." + idproducto + ".detprecioLista").html());
        //Se calculo el precio con descuento 
        var precio = Number((precioLista * (100 - porcentajeDescuento) / 100).toFixed(cantidadCuatroDecimales));
        //Se asigna el precio calculculado en la columna precio
        $("." + idproducto + ".detprecio").html(precio);
        //se obtiene la cantidad
        var cantidad = Number($("." + idproducto + ".detincantidad").val());
        //Se define el precio Unitario 
        var precioUnitario = flete + precio
        $("." + idproducto + ".detprecioUnitario").html(precioUnitario.toFixed(cantidadCuatroDecimales));
        //Se calcula el subtotal
        var subTotal = precioUnitario * cantidad;
        //Se asigna el subtotal 
        $("." + idproducto + ".detsubtotal").html(subTotal.toFixed(cantidadDecimales));
        //Se calcula el margen
        var costo = Number($("." + idproducto + ".detcostoLista").html());
        var margen = (1 - (Number(costo) / Number(precio)))*100;
        //Se asigna el margen 
        $("." + idproducto + ".detmargen").text(margen.toFixed(1)+ " %");

        var precioNetoAnterior = Number($("." + idproducto + ".detprecioNetoAnterior").html());        
        var varprecioNetoAnterior = (precio / precioNetoAnterior - 1)*100;
        $("." + idproducto + ".detvarprecioNetoAnterior").text(varprecioNetoAnterior.toFixed(1));

        var costoAnterior = Number($("." + idproducto + ".detcostoAnterior").html());       
        var varcosto = (costo / costoAnterior - 1)*100;
        $("." + idproducto + ".detvarCosto").text(varcosto.toFixed(1) + " %");


        //Se actualiza el subtotal de la cotizacion

        var $j_object = $("td.detcantidad");

        var subTotal = 0;
        var igv = 0;
        var total = 0;
       
        $.each($j_object, function (key, value) {
            var arrId = value.getAttribute("class").split(" ");
            var precioUnitario = Number($("." + arrId[0] + ".detprecioUnitario").html());
            var cantidad = Number($("." + arrId[0] + ".detincantidad").val());
            subTotal = subTotal + Number(Number((precioUnitario * cantidad)).toFixed(cantidadDecimales));
        });

        

        var incluidoIGV = $("input[name=igv]:checked").val();
        //Si no se etsá incluyendo IGV se le agrega
        if (incluidoIGV == "0") {
            igv = Number((subTotal * IGV).toFixed(cantidadDecimales));
            total = subTotal + (igv);
        }
        //Si se está incluyendo IGV entonces se 
        else
        {
            total = subTotal;
            subTotal = Number((subTotal / (1 + IGV)).toFixed(cantidadDecimales));
            igv = total - subTotal;
        }

        $('#montoSubTotal').html(subTotal.toFixed(cantidadDecimales));
        $('#montoIGV').html(igv.toFixed(cantidadDecimales));
        $('#montoTotal').html(total.toFixed(cantidadDecimales));

    };

    /*####################################################
    EVENTOS BUSQUEDA COTIZACIONES
    #####################################################*/


    $("#btnLimpiarBusquedaRequerimientos").click(function () {
        $.ajax({
            url: "/Requerimiento/CleanBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });


    $(document).on('click', "input.chkStockConfirmado", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idPEdido = arrrayClass[0];
        var numeroRequerimiento = arrrayClass[1];

        var stockConfirmado = 0;
        if (event.target.checked) {
            stockConfirmado = 1;
        }


        $.ajax({
            url: "/Requerimiento/UpdateStockConfirmado",
            data: {
                idPEdido: idPEdido,
                stockConfirmado: stockConfirmado
            },
            type: 'POST',
            error: function (detalle) {
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function (resultado) {

            }
        });
    });
    

    $("#btnAprobarSolicitudesPreview").click(function () {
        $.ajax({
            url: "/Requerimiento/AprobarTodosPreview",
            type: 'POST',
            dataType: 'JSON',
            data: {
            },
            error: function () {
            },
            success: function (consolidado) {
                
                $("#modalAprobarTodosPreview #verMontoSubTotal").html(consolidado.subTotal.toFixed(cantidadDecimales));
                $("#modalAprobarTodosPreview #verMontoIGV").html(consolidado.igv.toFixed(cantidadDecimales));
                $("#modalAprobarTodosPreview #verMontoTotal").html(consolidado.total.toFixed(cantidadDecimales));
                requerimientoList = consolidado.requerimientoList;
                //$("#tableAprobarRequerimientos").footable();
                
                $("#tableAprobarRequerimientos > tbody").empty();
                $("#tableAprobarRequerimientos").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                var nro = 1;

                for (var i = 0; i < requerimientoList.length; i++) {
                   
                    var observaciones = requerimientoList[i].observaciones == null || requerimientoList[i].observaciones == 'undefined' ? '' : requerimientoList[i].observaciones;
                    
                    /*if (requerimientoList[i].observaciones != null && requerimientoList[i].observaciones.length > 20) {
                        var idComentarioCorto = requerimientoList[i].idRequerimiento + "corto";
                        var idComentarioLargo = requerimientoList[i].idRequerimiento + "largo";
                        var idVerMas = requerimientoList[i].idRequerimiento + "verMas";
                        var idVermenos = requerimientoList[i].idRequerimiento + "verMenos";
                        var comentario = requerimientoList[i].observaciones.substr(0, 20) + "...";

                        observaciones = '<div id="' + idComentarioCorto + '" style="display:block;">' + comentario + '</div>' +
                            '<div id="' + idComentarioLargo + '" style="display:none;">' + requerimientoList[i].observaciones + '</div>' +
                            '<p><a id="' + idVerMas + '" class="' + requerimientoList[i].idRequerimiento + ' verMas" href="javascript:mostrar();" style="display:block">Ver Más</a></p>' +
                            '<p><a id="' + idVermenos + '" class="' + requerimientoList[i].idRequerimiento + ' verMenos" href="javascript:mostrar();" style="display:none">Ver Menos</a></p>';
                    }*/


                    var cantidad1 = 0
                    var cantidad2 = 0
                    
                    
                    var direcc = requerimientoList[i].direccionEntrega.direccionConSede + ' ' +
                        requerimientoList[i].direccionEntrega.ubigeo.Departamento + ' - ' +
                        requerimientoList[i].direccionEntrega.ubigeo.Provincia + ' - ' +
                        requerimientoList[i].direccionEntrega.ubigeo.Distrito;
                   
                    for (j = 0; j < requerimientoList[i].requerimientoDetalleList.length; j++) {
                        var requerimiento = '<tr data-expanded="true">' +
                            '<td>  ' + nro + '  </td>' +
                            '<td>  ' + direcc + '  </td>' +
                            '<td>  ' + requerimientoList[i].requerimientoDetalleList[j].producto.sku + '  </td>' +
                            '<td>  ' + requerimientoList[i].requerimientoDetalleList[j].producto.descripcion + '  </td>' +
                            '<td>  ' + requerimientoList[i].requerimientoDetalleList[j].unidad + ' </td>' +
                            '<td>  ' + requerimientoList[i].requerimientoDetalleList[j].precioUnitario.toFixed(cantidadDecimales) + '</td>' +
                            '<td>  ' + requerimientoList[i].requerimientoDetalleList[j].cantidad + '</td>' +
                            '<td>  ' + requerimientoList[i].requerimientoDetalleList[j].subTotal.toFixed(cantidadDecimales) + '</td>' 
                            '</tr>';

                            nro = nro + 1;
                            
                        $("#tableAprobarRequerimientos").append(requerimiento);
                            
                    }
                }
            }
        });
    });



    $("#btnGenerarPedidos").click(function () {


        var fechaEntregaDesde = $("#requerimiento_fechaEntregaDesdeTodos").val();
        var fechaEntregaHasta = $("#requerimiento_fechaEntregaHastaTodos").val();
        var horaEntregaDesde = $("#requerimiento_horaEntregaDesde").val();
        var horaEntregaHasta = $("#requerimiento_horaEntregaHasta").val();
        var horaEntregaAdicionalDesde = $("#requerimiento_horaEntregaAdicionalDesde").val();
        var horaEntregaAdicionalHasta = $("#requerimiento_horaEntregaAdicionalHasta").val();

   
        $('body').loadingModal({
            text: 'Creando Pedidos...'
        });
        $.ajax({
            url: "/Requerimiento/CreatePedidos",
            type: 'POST',
            data: {
                fechaEntregaDesde: fechaEntregaDesde,
                fechaEntregaHasta: fechaEntregaHasta,
                horaEntregaDesde: horaEntregaDesde,
                horaEntregaHasta: horaEntregaHasta,
                horaEntregaAdicionalDesde: horaEntregaAdicionalDesde,
                horaEntregaAdicionalHasta: horaEntregaAdicionalHasta
            },            
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function (resultado) {
                $('body').loadingModal('hide')
             /*   $("#requerimiento_numeroRequerimiento").val(resultado.numeroRequerimiento);
                $("#idRequerimiento").val(resultado.idRequerimiento);*/

                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "Los Pedidos fueron creados correctamente.",
                    buttons: {
                        OK: function () { window.location = '/Pedido/Index'; }
                    }
                });

               
            }
        })
    });




    $("#btnBusquedaRequerimientos").click(function () {


        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val();
        var fechaCreacionDesde = $("#requerimiento_fechaCreacionDesde").val();
        var fechaCreacionHasta = $("#requerimiento_fechaCreacionHasta").val();
        var fechaEntregaHasta = $("#requerimiento_fechaEntregaHasta").val();
        var fechaEntregaDesde = $("#requerimiento_fechaEntregaDesde").val();
        var fechaProgramacionDesde = $("#requerimiento_fechaProgramacionDesde").val();
        var fechaProgramacionHasta = $("#requerimiento_fechaProgramacionHasta").val();
        var requerimiento_idRequerimiento = $("#requerimiento_idRequerimiento").val();
        var requerimiento_numeroGrupoRequerimiento = $("#requerimiento_numeroGrupoRequerimiento").val();
        var requerimiento_idGrupoCliente = $("#requerimiento_idGrupoCliente").val();
        var estado = $("#estado").val();
        var estadoCrediticio = $("#estadoCrediticio").val();
        $("#btnBusquedaRequerimientos").attr("disabled", "disabled");
        $.ajax({
            url: "/Requerimiento/Search",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                fechaCreacionDesde: fechaCreacionDesde,
                fechaCreacionHasta: fechaCreacionHasta,
                fechaEntregaDesde: fechaEntregaDesde,
                fechaEntregaHasta: fechaEntregaHasta,
                fechaProgramacionDesde: fechaProgramacionDesde,
                fechaProgramacionHasta: fechaProgramacionHasta,
                numero: requerimiento_idRequerimiento,
                numeroGrupo: requerimiento_numeroGrupoRequerimiento,
                idGrupoCliente: requerimiento_idGrupoCliente,
                estado: estado,
                estadoCrediticio: estadoCrediticio
            },
            error: function () {
                $("#btnBusquedaRequerimientos").removeAttr("disabled");
            },

            success: function (requerimientoList) {

                //var requerimientoSearch = resultado.requerimiento;
                //var requerimientoList = resultado.requerimientoList;
        
                $("#tableProductos").footable();

                $("#btnBusquedaRequerimientos").removeAttr("disabled");

                $("#tableRequerimientos > tbody").empty();
                //FooTable.init('#tableCotizaciones');
                $("#tableRequerimientos").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < requerimientoList.length; i++) {
                

                    var observaciones = requerimientoList[i].observaciones == null || requerimientoList[i].observaciones == 'undefined' ? '' : requerimientoList[i].observaciones;

                    if (requerimientoList[i].observaciones != null && requerimientoList[i].observaciones.length > 20) {
                        var idComentarioCorto = requerimientoList[i].idRequerimiento + "corto";
                        var idComentarioLargo = requerimientoList[i].idRequerimiento + "largo";
                        var idVerMas = requerimientoList[i].idRequerimiento + "verMas";
                        var idVermenos = requerimientoList[i].idRequerimiento + "verMenos";
                        var comentario = requerimientoList[i].observaciones.substr(0, 20) + "...";

                        observaciones = '<div id="' + idComentarioCorto + '" style="display:block;">' + comentario + '</div>' +
                            '<div id="' + idComentarioLargo + '" style="display:none;">' + requerimientoList[i].observaciones + '</div>' +
                            '<p><a id="' + idVerMas + '" class="' + requerimientoList[i].idRequerimiento + ' verMas" href="javascript:mostrar();" style="display:block">Ver Más</a></p>' +
                            '<p><a id="' + idVermenos + '" class="' + requerimientoList[i].idRequerimiento + ' verMenos" href="javascript:mostrar();" style="display:none">Ver Menos</a></p>';
                    }

                  
                

                  
                    var imgIndicadorAprobacion = '<a data-toggle="tooltip" title="Aprobado"> <img class="table-product-img"  src="/images/semaforo_verde_small.png"  srcset="semaforo_verde_min.png 2x"/></a>';
                    if (requerimientoList[i].excedioPresupuesto)
                        imgIndicadorAprobacion = '<a data-toggle="tooltip" title="Pendiente Aprobación"><img class="table-product-img " src="/images/semaforo_rojo_small.png" srcset="semaforo_rojo_min.png 2x"/></a>';
                    /*    imgIndicadorAprobacion = '<a data-toggle="tooltip" title="Pendiente Aprobación"> <img class="table-product-img" src="/images/semaforo_naranja_small.png" srcset="semaforo_naranja_min.png 2x"/></a>';
                    else if (lista[i].indicadorAprobacion == 3)*/
                        
                    //requerimientoList[i].numeroRequerimientoNumeroGrupoString
                    //requerimientoList[i].seguimientoCrediticioRequerimiento_estadoString
                
                    var grupoCliente = requerimientoList[i].grupoCliente_nombre == null ? "" : requerimientoList[i].grupoCliente_nombre;
          
                    var requerimiento = '<tr data-expanded="true">' +
                        '<td>  ' + requerimientoList[i].idRequerimiento + '</td>' +
                        '<td>  ' + requerimientoList[i].idRequerimiento + '  </td>' +
                        '<td>  ' + requerimientoList[i].ciudad_nombre + '  </td>' +
                        '<td>  ' + requerimientoList[i].cliente_codigo + ' </td>' +
                        '<td>  ' + requerimientoList[i].cliente_razonSocial + '</td>' +
                        '<td>  ' + grupoCliente + '</td>' +
                        '<td>  ' + requerimientoList[i].numeroReferenciaCliente + '  </td>' +
                        '<td>  ' + requerimientoList[i].usuario_nombre + '  </td>' +
                        '<td>  ' + requerimientoList[i].fechaHoraRegistro + '</td>' +
                        '<td>  ' + requerimientoList[i].rangoFechasEntrega + '</td>' +
                        '<td>  ' + requerimientoList[i].rangoHoraEntrega + '</td>' +
                        '<td>  ' + requerimientoList[i].ubigeoEntrega_distrito + '  </td>' +

                        '<td>  ' + 'A' + '</td>' +
                        '<td> ' + observaciones + ' </td>';

                    /*    for (var j = 0; j < requerimientoSearch.requerimientoDetalleList.length; j++) 
                        {*/
                        for (j = 0; j < requerimientoList[i].requerimientoDetalleList.length; j++) {                          

                            for (var k = 0; k < requerimientoList[i].requerimientoDetalleList.length; k++)
                            {
                                //alert(requerimientoList[i].requerimientoDetalleList[k].producto.sku + $("#productosku"+k).val())
                                if (requerimientoList[i].requerimientoDetalleList[k].producto.sku == $("#productosku"+j).val())
                                {
                                    //alert("p")
                                    requerimiento = requerimiento + '<td> ' + requerimientoList[i].requerimientoDetalleList[k].cantidad + ' </td>';
                                    break;
                                }                                
                            }
                        };
                  /*      }*/

                    

                        requerimiento = requerimiento + '<td>  ' + requerimientoList[i].montoSubTotal.toFixed(cantidadDecimales) + '  </td>' +
                        '<td>  ' + requerimientoList[i].estadoRequerimientoString + '</td>' +
                        '<td>' + false + '</td>' +
                        '<td>' + false + '</td>' +
                        '<td>' + requerimientoList[i].topePresupuesto.toFixed(cantidadDecimales) + '  </td>' +
                        '<td>' + imgIndicadorAprobacion + '</td>' +
                        '<td>' +
                        '<button type="button" class="' + requerimientoList[i].idRequerimiento + ' ' + requerimientoList[i].idRequerimiento + ' btnVerRequerimiento btn btn-primary ">Ver</button>' +
                        '</td>' +

                        '</tr>';

                    $("#tableRequerimientos").append(requerimiento);

                }

                if (requerimientoList.length > 0) {
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

    $("#requerimiento_fechaSolicitudDesde").change(function () {
        var fechaSolicitudDesde = $("#requerimiento_fechaSolicitudDesde").val();
        $.ajax({
            url: "/Requerimiento/ChangeFechaSolicitudDesde",
            type: 'POST',
            data: {
                fechaSolicitudDesde: fechaSolicitudDesde
            },
            success: function () {
            }
        });
    });

    $("#requerimiento_fechaSolicitudHasta").change(function () {
        var fechaSolicitudHasta = $("#requerimiento_fechaSolicitudHasta").val();
        $.ajax({
            url: "/Requerimiento/ChangeFechaSolicitudHasta",
            type: 'POST',
            data: {
                fechaSolicitudHasta: fechaSolicitudHasta
            },
            success: function () {
            }
        });
    });
    
    $("#requerimiento_fechaEntregaDesde").change(function () {
        var fechaEntregaDesde = $("#requerimiento_fechaEntregaDesde").val();
        var fechaEntregaHasta = $("#requerimiento_fechaEntregaHasta").val();

        //Si fecha de entrega hasta es superior a fecha de entrega desde o la fecha de entrega hasta es vacío
        //se reemplaza el valor por la fecha de entrega desde
        if (convertirFechaNumero(fechaEntregaHasta) < convertirFechaNumero(fechaEntregaDesde)
            || fechaEntregaHasta.trim() == "" ) {
            $("#requerimiento_fechaEntregaHasta").val(fechaEntregaDesde);
            $("#requerimiento_fechaEntregaHasta").change();
        }

        $.ajax({
            url: "/Requerimiento/ChangeFechaEntregaDesde",
            type: 'POST',
            data: {
                fechaEntregaDesde: fechaEntregaDesde
            },
            success: function () {
            }
        });
    });

    $("#requerimiento_fechaEntregaHasta").change(function () {
        var fechaEntregaHasta = $("#requerimiento_fechaEntregaHasta").val();
        $.ajax({
            url: "/Requerimiento/ChangeFechaEntregaHasta",
            type: 'POST',
            data: {
                fechaEntregaHasta: fechaEntregaHasta
            },
            success: function () {
            }
        });
    });



    $("#requerimiento_idRequerimiento").change(function () {
        var numero = $("#requerimiento_idRequerimiento").val();
        $.ajax({
            url: "/Requerimiento/changeNumero",
            type: 'POST',
            data: {
                numero: numero
            },
            success: function () {
            }
        });
    });

    $("#requerimiento_numeroGrupoRequerimiento").change(function () {
        var numeroGrupo = $("#requerimiento_numeroGrupoRequerimiento").val();
        $.ajax({
            url: "/Requerimiento/changeNumeroGrupo",
            type: 'POST',
            data: {
                numeroGrupo: numeroGrupo
            },
            success: function () {
            }
        });
    });

    $("#requerimiento_idGrupoCliente").change(function () {
        var idGrupoCliente = $("#requerimiento_idGrupoCliente").val();
        $.ajax({
            url: "/Requerimiento/ChangeIdGrupoCliente",
            type: 'POST',
            data: {
                idGrupoCliente: idGrupoCliente
            },
            success: function () {
            }
        });
    });
    

    $("#requerimiento_buscarSedesGrupoCliente").change(function () {
        var valor = $("input[name=requerimiento_buscarSedesGrupoCliente]:checked").val();
        $.ajax({
            url: "/Requerimiento/ChangeBuscarSedesGrupoCliente",
            type: 'POST',
            data: {
                buscarSedesGrupoCliente: valor
            },
            success: function () {
            }
        });
    });

    $("#estado").change(function () {
        var estado = $("#estado").val();
        $.ajax({
            url: "/Requerimiento/changeEstado",
            type: 'POST',
            data: {
                estado: estado
            },
            success: function () {
            }
        });
    });

    $("#estadoCrediticio").change(function () {
        var estado = $("#estadoCrediticio").val();
        $.ajax({
            url: "/Requerimiento/changeEstadoCrediticio",
            type: 'POST',
            data: {
                estadoCrediticio: estado
            },
            success: function () {
            }
        });
    });


    $("#periodo").change(function () {
        var periodo = $("#periodo").val();
        $.ajax({
            url: "/Requerimiento/changePeriodo",
            type: 'POST',
            data: {
                periodo: periodo
            },
            success: function () {
            }
        });
    });

    $("#lnkVerHistorial").click(function () {
        showHistorial();
    });

    $("#lnkVerHistorialCrediticio").click(function () {
        showHistorialCrediticio();
    });

    function showHistorial() {
        $('body').loadingModal({
            text: 'Obteniendo Historial...'
        });

        var idRequerimiento = $("#verIdRequerimiento").val();

        $.ajax({
            url: "/Requerimiento/GetHistorial",
            data: {
                id: idRequerimiento
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) { $('body').loadingModal('hide'); alert("Ocurrió un problema al obtener el historial del requerimiento."); },
            success: function (resultado) {

                $("#historial_titulo_numero_requerimiento").html($("#verNumero").html());

                $("#tableHistorialRequerimiento > tbody").empty();

                FooTable.init('#tableHistorialRequerimiento');

                var d = '';
                var lista = resultado.result;
                for (var i = 0; i < resultado.result.length; i++) {

                    var observacion = lista[i].observacion == null || lista[i].observacion == 'undefined' ? '' : lista[i].observacion;

                    d += '<tr>' +
                        '<td>' + lista[i].FechaRegistroDesc + '</td>' +
                        '<td>' + lista[i].usuario.nombre + '</td>' +
                        '<td>' + lista[i].estadoString + '</td>' +
                        '<td>' + observacion + '</td>' +
                        '</tr>';

                }
                //  
                // sleep
                $("#tableHistorialRequerimiento").append(d);




                $("#modalVerHistorialRequerimiento").modal('show');
                $('body').loadingModal('hide');
                //  window.location = '/Cotizacion/Index';
            }
        });
    };

    function showHistorialCrediticio() {
        $('body').loadingModal({
            text: 'Obteniendo Historial Crediticio...'
        });

        var idRequerimiento = $("#verIdRequerimiento").val();

        $.ajax({
            url: "/Requerimiento/GetHistorialCrediticio",
            data: {
                id: idRequerimiento
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) { $('body').loadingModal('hide'); alert("Ocurrió un problema al obtener el historial crediticio del requerimiento."); },
            success: function (resultado) {
                $("#historial_crediticio_titulo_numero_requerimiento").html($("#verNumero").html());
                $("#tableHistorialCrediticioRequerimiento > tbody").empty();

                FooTable.init('#tableHistorialCrediticioRequerimiento');

                var d = '';
                var lista = resultado.result;
                for (var i = 0; i < resultado.result.length; i++) {

                    var observacion = lista[i].observacion == null || lista[i].observacion == 'undefined' ? '' : lista[i].observacion;

                    d += '<tr>' +
                        '<td>' + lista[i].FechaRegistroDesc + '</td>' +
                        '<td>' + lista[i].usuario.nombre + '</td>' +
                        '<td>' + lista[i].estadoString + '</td>' +
                        '<td>' + observacion + '</td>' +
                        '</tr>';

                }
                //  
                // sleep
                $("#tableHistorialCrediticioRequerimiento").append(d);




                $("#modalVerHistorialCrediticioRequerimiento").modal('show');
                $('body').loadingModal('hide');
                //  window.location = '/Cotizacion/Index';
            }
        });
    };

    $(document).on('change', "#ActualDepartamento", function () {
        var ubigeoEntregaId = "000000";
        if ($("#ActualDepartamento").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualDepartamento").val() + "0000";
        }
        $.ajax({
            url: "/Requerimiento/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#ActualProvincia",function () {
        var ubigeoEntregaId = $("#ActualDepartamento").val()+"0000";
        if ($("#ActualProvincia").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualProvincia").val()+"00";
        }
        $.ajax({
            url: "/Requerimiento/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#ActualDistrito",function () {
        var ubigeoEntregaId = $("#ActualDepartamento").val() + $("#ActualProvincia").val() + "00";
        if ($("#ActualDistrito").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualDistrito").val();
        }
        $.ajax({
            url: "/Requerimiento/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
        /*obtenerDireccionesEntrega
        */
    });

    /*
    function obtenerDireccionesEntrega() {
    $.ajax({
        url: "/DireccionEntrega/GetDireccionesEntrega",
        type: 'POST',
        dataType: 'JSON',
        data: {
            ubigeo: ubigeo
        },
        success: function (direccionEntregaListTmp) {

            $('#requerimiento_direccionEntrega')
                .find('option')
                .remove()
                .end()
                ;

            $('#requerimiento_direccionEntrega').append($('<option>', {
                value: GUID_EMPTY,
                text: "Seleccione Dirección Entrega",
                direccion: "",
                contacto: "",
                telefono: ""
            }));


            for (var i = 0; i < direccionEntregaListTmp.length; i++) {
                $('#requerimiento_direccionEntrega').append($('<option>', {
                    value: direccionEntregaListTmp[i].idDireccionEntrega,
                    text: direccionEntregaListTmp[i].descripcion,
                    direccion: direccionEntregaListTmp[i].descripcion,
                    contacto: direccionEntregaListTmp[i].contacto,
                    telefono: direccionEntregaListTmp[i].telefono
                }));

            }

            deshabilitarEdicionDireccionEntrega();
            $('#btnNuevaDireccion').show();
            $('#btnModificarDireccion').hide();
            $('#btnCancelarDireccion').hide();
        }
        });
    }*/

    $("#idCiudad").change(function () {
        var idCiudad = $("#idCiudad").val();

        $.ajax({
            url: "/Requerimiento/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad
            },
            error: function (detalle) {
                alert('Debe eliminar los productos agregados antes de cambiar de Sede.');
                location.reload();
            },
            success: function (ciudad) {
                location.reload();
            }
        });
    });  


    $("#idPeriodo").change(function () {
        var idPeriodo = $("#idPeriodo").val();

        $.ajax({
            url: "/Requerimiento/ChangeIdPeriodo",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idPeriodo: idPeriodo
            },
            error: function (detalle) {
                location.reload();
            },
            success: function (ciudad) {
               // location.reload();
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

    /****************** PROGRAMACION PEDIDO****************************/

    $('#modalProgramacion').on('shown.bs.modal', function () {
        var fechaProgramacion = $("#fechaProgramaciontmp").val();
        $("#fechaProgramacion").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaProgramacion);    
    })

    $("#btnExportExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
    });

    $("#btnExportRequerimientosExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
    });

    $("#btnAceptarProgramarRequerimiento").click(function () {

        if ($("#fechaProgramacion").val() == "" || $("#fechaProgramacion").val() == null) {
            alert("Debe ingresar la fecha de programación.");
            $("#fechaProgramacion").focus();
            return false;
        }
        var fechaProgramacion = $('#fechaProgramacion').val();
        var comentarioProgramacion = $('#comentarioProgramacion').val();
        var fechaEntregaDesdeProgramacion = $("#fechaEntregaDesdeProgramacion").val();
        var fechaEntregaHastaProgramacion = $("#fechaEntregaHastaProgramacion").val();

        if (convertirFechaNumero(fechaProgramacion) < convertirFechaNumero(fechaEntregaDesdeProgramacion)
            || convertirFechaNumero(fechaProgramacion) > convertirFechaNumero(fechaEntregaHastaProgramacion)
        ) {
            var respuesta = confirm("¡ATENCIÓN! Está programando la atención del requerimiento en una fecha fuera del rango solicitado por el cliente.");
            if (!respuesta) {
                $("#fechaProgramacion").focus();
                return false;
            }
        }      

        $.ajax({
            url: "/Requerimiento/Programar",
            type: 'POST',
           // dataType: 'JSON',
            data: {
                fechaProgramacion: fechaProgramacion,
                comentarioProgramacion: comentarioProgramacion
            },
            success: function (resultado) {
                alert('El requerimiento número ' + $("#verNumero").html() + ' se programó para ser atendido.');
                location.reload();
            }
        });
        $("btnCancelarProgramarRequerimiento").click();
    });



 


    /****************** FIN PROGRAMACION PEDIDO****************************/
});