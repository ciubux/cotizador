
jQuery(function ($) {

    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_VALIDACION_OCC = 'Revisar Datos de la Orden de Compra';
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        obtenerConstantes();
      //  setTimeout(autoGuardarOrdenCompraCliente, MILISEGUNDOS_AUTOGUARDADO);
        cargarChosenCliente();
        toggleControlesSolicitante();
        verificarSiExisteNuevoSolicitante();
        verificarSiExisteDetalle();
        verificarSiExisteCliente();
        $("#btnBusquedaOrdenCompraClientes").click();
        var tipoOrdenCompraCliente = $("#occ_tipoOrdenCompraCliente").val();
        validarTipoOrdenCompraCliente(tipoOrdenCompraCliente);

        if ($("#pagina").val() == 2) {
            if ($("#idOrdenCompraCliente").val() != "") {
                showOrdenCompraCliente($("#idOrdenCompraCliente").val());
            }
        }

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
            if ($("#idCliente").val().trim() != "" && $("#pagina").val() == PAGINA_MANTENIMIENTO_OCC)
                $("#idCiudad").attr("disabled", "disabled");
        }
    }
   

    function obtenerConstantes() {
        $.ajax({
            url: "/General/GetConstantes",
            type: 'POST',
            dataType: 'JSON',
            success: function (constantes) {
                IGV = constantes.IGV;
                SIMBOLO_SOL = constantes.SIMBOLO_SOL;
                MILISEGUNDOS_AUTOGUARDADO = constantes.MILISEGUNDOS_AUTOGUARDADO;
                VARIACION_PRECIO_ITEM_OCC = constantes.VARIACION_PRECIO_ITEM_OCC;
            }
        });
    }

    function autoGuardarOrdenCompraCliente() {
        $.ajax({
            url: "/OrdenCompraCliente/autoGuardarOrdenCompraCliente",
            type: 'POST',
            error: function () {
                setTimeout(autoGuardarOrdenCompraCliente, MILISEGUNDOS_AUTOGUARDADO);
            },
            success: function () {
                setTimeout(autoGuardarOrdenCompraCliente, MILISEGUNDOS_AUTOGUARDADO);
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


    function verificarSiExisteNuevaDireccionEntrega() {
        $('#occ_direccionEntrega option').each(function () {
            if ($(this).val() == GUID_EMPTY) {
                $("#btnAgregarDireccion").attr("disabled", "disabled");
            }
        });
    }

    function verificarSiExisteNuevoSolicitante() {
        $('#occ_solicitante option').each(function () {
            if ($(this).val() == GUID_EMPTY) {
                $("#btnAgregarSolicitante").attr("disabled", "disabled");
            }
        });
    }

    /**
     * ################################ INICIO CONTROLES DE CLIENTE
     */


    $("#occ_direccionEntrega").chosen({ placeholder_text_single: "Seleccione la Dirección de Entrega", no_results_text: "No se encontró Dirección de Entrega" }).on('chosen:showing_dropdown');

    function cargarChosenCliente() {

        $("#idCliente").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" }).on('chosen:showing_dropdown', function (evt, params) {
           
        });

        $("#idCliente").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/OrdenCompraCliente/SearchClientes"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });
    }





    function toggleControlesUbigeo() {
        //Si cliente esta Seleccionado se habilitan la seleccion para la direccion de entrega

    //    if ($("#idCliente").val() == "") {
        var varTmp = $('#occ_cliente_habilitadoModificarDireccionEntrega').val();
        var habilitadoModificarDireccionEntrega = varTmp == 'True' || varTmp == 'true';

        if ($("#idCliente").val().trim() != "") {
            //$("#idCiudad").attr('disabled', 'disabled');
            if (habilitadoModificarDireccionEntrega) {
                $('#ActualDepartamento').removeAttr('disabled');
                $('#ActualProvincia').removeAttr('disabled');
                $('#ActualDistrito').removeAttr('disabled');
                $("#occ_direccionEntrega").removeAttr('disabled');
                $("#btnAgregarDireccion").removeAttr('disabled');
            }
            else {
                $("#ActualDepartamento").attr('disabled', 'disabled');
                $('#ActualProvincia').attr('disabled', 'disabled');
                $('#ActualDistrito').attr('disabled', 'disabled');
                $("#occ_direccionEntrega").attr('disabled', 'disabled');
                $("#btnAgregarDireccion").attr('disabled', 'disabled');
            }
          
            $("#occ_solicitante").removeAttr('disabled');
            $("#btnAgregarSolicitante").removeAttr('disabled');
        }
        else {
            $("#ActualDepartamento").attr('disabled', 'disabled');
            $('#ActualProvincia').attr('disabled', 'disabled');
            $('#ActualDistrito').attr('disabled', 'disabled');
            $("#occ_direccionEntrega").attr('disabled', 'disabled');
            $("#occ_solicitante").attr('disabled', 'disabled');
            $("#btnAgregarDireccion").attr("disabled", "disabled");
            $("#btnAgregarSolicitante").attr("disabled", "disabled");


            }
            toggleControlesDireccionEntrega();


        

    //    }
    }




    function toggleControlesDireccionEntrega() {
        //alert("W");
        var idDireccionEntrega = $('#occ_direccionEntrega').val();
        var varTmp = $('#occ_cliente_habilitadoModificarDireccionEntrega').val();
        var habilitadoModificarDireccionEntrega = varTmp == 'True' || varTmp == 'true';
        if (idDireccionEntrega != "" && habilitadoModificarDireccionEntrega) {
            $("#occ_direccionEntrega_descripcion").removeAttr("disabled");
            $("#occ_direccionEntrega_contacto").removeAttr("disabled");
            $("#occ_direccionEntrega_telefono").removeAttr("disabled");


        }
        else {
            $("#occ_direccionEntrega_descripcion").attr('disabled', 'disabled');
            $("#occ_direccionEntrega_contacto").attr('disabled', 'disabled');
            $("#occ_direccionEntrega_telefono").attr('disabled', 'disabled');
        }

        if (habilitadoModificarDireccionEntrega) {
            $("#btnAgregarDireccion").show();
            $("#ActualDepartamento").removeAttr("disabled");
            $("#ActualDepartamento").removeAttr("disabled");
            $("#ActualDistrito").removeAttr("disabled");
        }
        else {
            $("#btnAgregarDireccion").hide();
            $("#ActualDepartamento").attr("disabled");
            $("#ActualDepartamento").attr("disabled");
            $("#ActualDistrito").attr("disabled");
        }

    }
    
    function toggleControlesSolicitante() {
        var idSolicitante = $('#occ_solicitante').val();
        if (idSolicitante == "") {
            $("#occ_solicitante_nombre").attr('disabled', 'disabled');
            $("#occ_solicitante_telefono").attr('disabled', 'disabled');
            $("#occ_solicitante_correo").attr('disabled', 'disabled');

        }
        else {
            /*  $("#occ_direccionEntrega_telefono").val($('#occ_direccionEntrega').find(":selected").attr("telefono"));*/
            $("#occ_solicitante_nombre").removeAttr("disabled");
            $("#occ_solicitante_telefono").removeAttr("disabled");
            $("#occ_solicitante_correo").removeAttr("disabled");
        }
    }


    $("#idCliente").change(function () {

        var idCliente = $(this).val();

        $.ajax({
            url: "/OrdenCompraCliente/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente)
            {

                ///Solicitante
                var solicitanteListTmp = cliente.solicitanteList;

                $('#occ_solicitante')
                    .find('option')
                    .remove()
                    .end()
                    ;

                $('#occ_solicitante').append($('<option>', {
                    value: "",
                    text: "Seleccione Solicitante"
                }));


                for (var i = 0; i < solicitanteListTmp.length; i++) {
                    $('#occ_solicitante').append($('<option>', {
                        value: solicitanteListTmp[i].idSolicitante,
                        text: solicitanteListTmp[i].nombre
                    }));
                }

                
               // alert("asas")
            //    return;
                location.reload();

                //window.setInterval($("#occ_direccionEntrega").trigger("chosen:updated"), 5000);     
                //$("#occ_direccionEntrega").chosen({ placeholder_text_single: "Seleccione la Dirección de Entrega", no_results_text: "No se encontró Dirección de Entrega" }).on('chosen:showing_dropdown');
                //$("#occ_direccionEntrega").trigger("chosen:updated");
          /*      $('#occ_direccionEntrega')
                    .val(1)
                    .trigger('liszt:update')
                    .removeClass('chzn-done');
                */
              /*  $('#test_chzn').remove();


                $("#test").chosen({
                    width: "220px",
                    no_results_text: "test"
                });*/
              //  $('#occ_direccionEntrega').chosen("destroy").chosen();
           //     window.setInterval($("#occ_direccionEntrega").trigger("chosen:updated"), 15000);     
             //   $("#occ_direccionEntrega").chosen({ placeholder_text_single: "Seleccione la Dirección de Entrega", no_results_text: "No se encontró Dirección de Entrega" }).on('chosen:showing_dropdown');
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
   /*     $('#occ_direccionEntrega option').each(function () {

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
                controller: "ordenCompraCliente"
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


    $("#btnSaveDireccion").click(function () {

        if ($("#direccionEntrega_descripcion").val().trim() == "") {
            alert("Debe ingresar la dirección de entrega.");
            $('#direccionEntrega_descripcion').focus();
            return false;
        }

        if ($("#direccionEntrega_contacto").val().trim() == "") {
            alert("Debe ingresar el nombre del contacto de entrega.");
            $('#direccionEntrega_contacto').focus();
            return false;
        }

        if ($("#direccionEntrega_telefono").val().trim() == "") {
            alert("Debe ingresar el telefono del contacto de entrega.");
            $('#direccionEntrega_telefono').focus();
            return false;
        }

        var direccion = $("#direccionEntrega_descripcion").val();
        var contacto = $("#direccionEntrega_contacto").val();
        var telefono = $("#direccionEntrega_telefono").val();

        $.ajax({
            url: "/OrdenCompraCliente/CreateDireccionTemporal",
            type: 'POST',
            dataType: 'JSON',
            data: {
                direccion: direccion,
                contacto: contacto,
                telefono: telefono
            },
            error: function (detalle) { alert("Se generó un error al intentar crear la dirección."); },
            success: function (direccion) {

                $('#occ_direccionEntrega').append($('<option>', {
                    value: direccion.idDireccionEntrega,
                    text: direccion.descripcion
                }));
                $('#occ_direccionEntrega').val(direccion.idDireccionEntrega);

                $('#occ_direccionEntrega_descripcion').val(direccion.descripcion);
                $('#occ_direccionEntrega_contacto').val(direccion.contacto);
                $('#occ_direccionEntrega_telefono').val(direccion.telefono);
                verificarSiExisteNuevaDireccionEntrega();
                toggleControlesDireccionEntrega();
            }
        });


        $('#btnCancelDireccion').click();

    });




    $("#btnSaveSolicitante").click(function () {

        if ($("#solicitante_nombre").val().trim() == "") {
            alert("Debe ingresar el nombre del solicitante.");
            $('#solicitante_nombre').focus();
            return false;
        }

        if ($("#solicitante_telefono").val().trim() == "") {
            alert("Debe ingresar el telefono del solicitante.");
            $('#solicitante_telefono').focus();
            return false;
        }

        if ($("#solicitante_correo").val().trim() == "") {
            alert("Debe ingresar el correo del solicitante.");
            $('#solicitante_correo').focus();
            return false;
        }

        var nombre = $("#solicitante_nombre").val();
        var telefono = $("#solicitante_telefono").val();
        var correo = $("#solicitante_correo").val();

        $.ajax({
            url: "/OrdenCompraCliente/CreateSolicitanteTemporal",
            type: 'POST',
            dataType: 'JSON',
            data: {
                nombre : nombre,
                telefono: telefono,
                correo: correo
            },
            error: function (detalle) { alert("Se generó un error al intentar crear el solicitante."); },
            success: function (solicitante) {

                $('#occ_solicitante').append($('<option>', {
                    value: solicitante.idSolicitante,
                    text: solicitante.nombre
                }));
                $('#occ_solicitante').val(solicitante.idSolicitante);

                $('#occ_solicitante_nombre').val(solicitante.nombre);
                $('#occ_solicitante_telefono').val(solicitante.telefono);
                $('#occ_solicitante_correo').val(solicitante.correo);
                verificarSiExisteNuevoSolicitante();
                toggleControlesSolicitante();
            }
        });


        $('#btnCancelSolicitante').click();

    });

















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
    $("#occ_fechaCreacionDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaCreacionDesde);

    var fechaCreacionHasta = $("#fechaCreacionHastatmp").val();
    $("#occ_fechaCreacionHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaCreacionHasta);
    
    var fechaEntregaDesde = $("#fechaEntregaDesdetmp").val();
    $("#occ_fechaEntregaDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEntregaDesde);

    var fechaEntregaHasta = $("#fechaEntregaHastatmp").val();
    $("#occ_fechaEntregaHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEntregaHasta);
    

    var fechaProgramacionDesde = $("#fechaProgramacionDesdetmp").val();
    $("#occ_fechaProgramacionDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaProgramacionDesde);

    var fechaProgramacionHasta = $("#fechaProgramacionHastatmp").val();
    $("#occ_fechaProgramacionHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaProgramacionHasta);


    var fechaPrecios = $("#fechaPreciostmp").val();
    $("#fechaPrecios").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaPrecios);    

    //var fechaProgramacion = $("#fechaProgramaciontmp").val();
    $("#fechaProgramacion").datepicker({ dateFormat: "dd/mm/yy" });//.datepicker("setDate", fechaProgramacion);    

    var documentoVenta_fechaEmision = $("#documentoVenta_fechaEmisiontmp").val();
    $("#documentoVenta_fechaEmision").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", documentoVenta_fechaEmision);

    var documentoVenta_fechaVencimiento = $("#documentoVenta_fechaVencimientotmp").val();
    $("#documentoVenta_fechaVencimiento").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", documentoVenta_fechaVencimiento);


    $("#occ_fechaEntregaExtendida").datepicker({ dateFormat: "dd/mm/yy", minDate: 0 });


    /**
     * FIN DE CONTROLES DE FECHAS
     */



    /* ################################## INICIO CHANGE CONTROLES */

    function validarTipoOrdenCompraCliente(tipoOrdenCompraCliente) {
        //Si el tipo de ordenCompraCliente es traslado interno (84->'T')
      /*  if (tipoOrdenCompraCliente == TIPO_OCC_VENTA_TRASLADO_INTERNO_ENTREGADO.charCodeAt(0)) {
            $("#divReferenciaCliente").hide();
            $("#divCiudadASolicitar").show();
            $(".mostrarDatosParaGuia").show();
        }
        else {*/
            $("#divReferenciaCliente").show();
            $("#divCiudadASolicitar").hide();

            if (tipoOrdenCompraCliente == TIPO_OCC_VENTA_VENTA.charCodeAt(0)
              //  || tipoOrdenCompraCliente == TIPO_OCC_VENTA_TRASLADO_INTERNO_ENTREGADO.charCodeAt(0)
                || tipoOrdenCompraCliente == TIPO_OCC_VENTA_COMODATO_ENTREGADO.charCodeAt(0)
                || tipoOrdenCompraCliente == TIPO_OCC_VENTA_TRANSFERENCIA_GRATUITA_ENTREGADA.charCodeAt(0)
                //   || tipoOrdenCompraCliente == TIPO_OCC_VENTA_PRESTAMO_ENTREGADO.charCodeAt(0)
            ) {
                $(".mostrarDatosParaGuia").show();
            }
            else {
                $(".mostrarDatosParaGuia").hide();
            }
       // }


        
        




    }

    $("#occ_tipoOrdenCompraCliente").change(function () { 
        var tipoOrdenCompraCliente = $("#occ_tipoOrdenCompraCliente").val();
        validarTipoOrdenCompraCliente(tipoOrdenCompraCliente);
        

        $.ajax({
            url: "/OrdenCompraCliente/ChangeTipoOrdenCompraCliente",
            type: 'POST',
            data: {
                tipoOrdenCompraCliente: tipoOrdenCompraCliente
            },
            success: function () { }
        });
    });

    $("#idCiudadASolicitar").change(function () {
        var idCiudadASolicitar = $("#idCiudadASolicitar").val();

        $.ajax({
            url: "/OrdenCompraCliente/ChangeIdCiudadASolicitar",
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


    $("#occ_sku").change(function () {
        changeInputString("sku", $("#occ_sku").val())
    });

    $("#occ_numeroReferenciaCliente").change(function () {
        changeInputString("numeroReferenciaCliente", $("#occ_numeroReferenciaCliente").val());
    });


    $("#occ_numeroRequerimiento").change(function () {
        changeInputString("numeroRequerimiento", $("#occ_numeroRequerimiento").val());
    });



    $("#occ_otrosCargos").change(function () {
        $.ajax({
            url: "/OrdenCompraCliente/ChangeOtrosCargos",
            type: 'POST',
            data: {
                otrosCargos: $("#occ_otrosCargos").val()
            },
            success: function () { }
        });
    });

    $('#occ_direccionEntrega').change(function () {
        toggleControlesDireccionEntrega();

       /*
        if ($("#occ_numeroOrdenCompraCliente").val() != "") {
            $.alert({
                title: 'Advertencia',
                type: 'orange',
                content: 'Asegurese de modificar también las observaciones de guía y factura de ser necesario.',
                buttons: {
                    OK: function () {
                        var idDireccionEntrega = $('#occ_direccionEntrega').val();
                        $.ajax({
                            url: "/OrdenCompraCliente/ChangeDireccionEntrega",
                            type: 'POST',
                            dataType: 'JSON',
                            data: {
                                idDireccionEntrega: idDireccionEntrega
                            },
                            success: function (direccionEntrega) {

                                $("#occ_direccionEntrega_telefono").val(direccionEntrega.telefono);
                                $("#occ_direccionEntrega_contacto").val(direccionEntrega.contacto);
                                $("#occ_direccionEntrega_descripcion").val(direccionEntrega.descripcion);
                                location.reload()
                            }
                        })
                    }
                }
            });
        }

        else {*/
            var idDireccionEntrega = $('#occ_direccionEntrega').val();
            $.ajax({
                url: "/OrdenCompraCliente/ChangeDireccionEntrega",
                type: 'POST',
                dataType: 'JSON',
                data: {
                    idDireccionEntrega: idDireccionEntrega
                },
                success: function (direccionEntrega) {

               /*     $("#occ_direccionEntrega_telefono").val(direccionEntrega.telefono);
                    $("#occ_direccionEntrega_contacto").val(direccionEntrega.contacto);
                    $("#occ_direccionEntrega_descripcion").val(direccionEntrega.descripcion);*/
                    location.reload()
                }
            })
        //}
    });





    $("#occ_direccionEntrega_descripcion").change(function () {
        $.ajax({
            url: "/OrdenCompraCliente/ChangeDireccionEntregaDescripcion",
            type: 'POST',
            data: {
                direccionEntregaDescripcion: $("#occ_direccionEntrega_descripcion").val()
            },
            success: function () { }
        });
    });

    $("#occ_direccionEntrega_contacto").change(function () {
        $.ajax({
            url: "/OrdenCompraCliente/ChangeDireccionEntregaContacto",
            type: 'POST',
            data: {
                direccionEntregaContacto: $("#occ_direccionEntrega_contacto").val()
            },
            success: function () { }
        });
    });
    
    $("#occ_direccionEntrega_telefono").change(function () {
        $.ajax({
            url: "/OrdenCompraCliente/ChangeDireccionEntregaTelefono",
            type: 'POST',
            data: {
                direccionEntregaTelefono: $("#occ_direccionEntrega_telefono").val()
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
            url: "/OrdenCompraCliente/ChangeEsPagoContado",
            type: 'POST',
            data: {
                esPagoContado: valor
            },
            dataType: 'JSON',
            success: function (result) {
                $("#occ_textoCondicionesPago").val(result.textoCondicionesPago);
            }
        });
    });



    $('#occ_solicitante').change(function () {
        toggleControlesSolicitante();
        var idSolicitante = $('#occ_solicitante').val();
        $.ajax({
            url: "/OrdenCompraCliente/ChangeSolicitante",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idSolicitante: idSolicitante
            },
            success: function (solicitante) {
                $("#occ_solicitante_nombre").val(solicitante.nombre);
                $("#occ_solicitante_telefono").val(solicitante.telefono);
                $("#occ_solicitante_correo").val(solicitante.correo);
            }
        })
    });

    $("#occ_solicitante_nombre").change(function () {
        $.ajax({
            url: "/OrdenCompraCliente/ChangeSolicitanteNombre",
            type: 'POST',
            data: {
                solicitanteNombre: $("#occ_solicitante_nombre").val()
            },
            success: function () { }
        });
    });


    $("#occ_solicitante_telefono").change(function () {
        $.ajax({
            url: "/OrdenCompraCliente/ChangeSolicitanteTelefono",
            type: 'POST',
            data: {
                solicitanteTelefono: $("#occ_solicitante_telefono").val()
            },
            success: function () { }
        });
    });

    $("#occ_solicitante_correo").change(function () {
        $.ajax({
            url: "/OrdenCompraCliente/ChangeSolicitanteCorreo",
            type: 'POST',
            data: {
                solicitanteCorreo: $("#occ_solicitante_correo").val()
            },
            success: function () { }
        });
    });




    $(".fechaSolicitud").change(function () {
        var fechaSolicitud = $("#fechaSolicitud").val();
        var horaSolicitud = $("#horaSolicitud").val();
        $.ajax({
            url: "/OrdenCompraCliente/ChangeFechaSolicitud",
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
            url: "/OrdenCompraCliente/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#occ_horaEntregaDesde").change(function () {

        var horaEntregaDesde = $("#occ_horaEntregaDesde").val();
        var horaEntregaHasta = $("#occ_horaEntregaHasta").val();

        //Si la hora de entrega desde tiene una diferencia menor a dos horas con la hora entrega hasta
        //Se reemplaza la hora de entraga hasta con la hora entrega desde más dos horas
        if (convertirHoraToNumero(horaEntregaDesde) + 1 > convertirHoraToNumero(horaEntregaHasta)) {
            $("#occ_horaEntregaHasta").val(sumarHoras($("#occ_horaEntregaDesde").val(),1));
            $("#occ_horaEntregaHasta").change();
        }
       
        changeInputString("horaEntregaDesde", $("#occ_horaEntregaDesde").val());
    });

    $("#occ_horaEntregaHasta").change(function () {
        var horaEntregaDesde = $("#occ_horaEntregaDesde").val();
        var horaEntregaHasta = $("#occ_horaEntregaHasta").val();

        //Si la hora de entrega desde tiene una diferencia menor a dos horas con la hora entrega hasta
        //Se reemplaza la hora de entraga hasta con la hora entrega desde más dos horas
        if (convertirHoraToNumero(horaEntregaDesde) + 1 > convertirHoraToNumero(horaEntregaHasta)) {
            $("#occ_horaEntregaDesde").val(sumarHoras($("#occ_horaEntregaHasta").val(), -1));
            $("#occ_horaEntregaDesde").change();
        }
        changeInputString("horaEntregaHasta", $("#occ_horaEntregaHasta").val());

    });

    $("#occ_horaEntregaAdicionalDesde").change(function () {

        var horaEntregaDesde = $("#occ_horaEntregaAdicionalDesde").val();
        var horaEntregaHasta = $("#occ_horaEntregaAdicionalHasta").val();

        //Si la hora de entrega desde tiene una diferencia menor a dos horas con la hora entrega hasta
        //Se reemplaza la hora de entraga hasta con la hora entrega desde más dos horas
        if (convertirHoraToNumero(horaEntregaDesde) + 1 > convertirHoraToNumero(horaEntregaHasta)) {
            $("#occ_horaEntregaAdicionalHasta").val(sumarHoras($("#occ_horaEntregaAdicionalDesde").val(), 1));
            $("#occ_horaEntregaAdicionalHasta").change();
        }

        changeInputString("horaEntregaAdicionalDesde", $("#occ_horaEntregaAdicionalDesde").val());
    });

    $("#occ_horaEntregaAdicionalHasta").change(function () {

        var horaEntregaDesde = $("#occ_horaEntregaAdicionalDesde").val();
        var horaEntregaHasta = $("#occ_horaEntregaAdicionalHasta").val();

        //Si la hora de entrega desde tiene una diferencia menor a dos horas con la hora entrega hasta
        //Se reemplaza la hora de entraga hasta con la hora entrega desde más dos horas
        if (convertirHoraToNumero(horaEntregaDesde) + 1 > convertirHoraToNumero(horaEntregaHasta)) {
            $("#occ_horaEntregaAdicionalDesde").val(sumarHoras($("#occ_horaEntregaAdicionalHasta").val(), -1));
            $("#occ_horaEntregaAdicionalDesde").change();
        }

        changeInputString("horaEntregaAdicionalHasta", $("#occ_horaEntregaAdicionalHasta").val());
    });

    $("#occ_numeroReferenciaCliente").change(function () {
        changeInputString("numeroReferenciaCliente", $("#occ_numeroReferenciaCliente").val())
    });

    $("#occ_observacionesFactura").change(function () {
        changeInputString("observacionesFactura", $("#occ_observacionesFactura").val())
    });

    $("#occ_observacionesGuiaRemision").change(function () {
        changeInputString("observacionesGuiaRemision", $("#occ_observacionesGuiaRemision").val())
    });


    

    $("#occ_contactoOrdenCompraCliente").change(function () {
        changeInputString("contactoOrdenCompraCliente", $("#occ_contactoOrdenCompraCliente").val())
    });

    $("#occ_telefonoContactoOrdenCompraCliente").change(function () {
        changeInputString("telefonoContactoOrdenCompraCliente", $("#occ_telefonoContactoOrdenCompraCliente").val())
    });

    $("#occ_correoContactoOrdenCompraCliente").change(function () {
        changeInputString("correoContactoOrdenCompraCliente", $("#occ_correoContactoOrdenCompraCliente").val())
    });

    $("#occ_observaciones").change(function () {
        changeInputString("observaciones", $("#occ_observaciones").val())
    });

    /**
     * ################################ INICIO CONTROLES DE AGREGAR PRODUCTO
     */

    ////////////////ABRIR AGREGAR PRODUCTO
    $('#btnOpenAgregarProducto').click(function () {

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
        $('#spnProductoDescontinuado').hide();

        //Se agrega chosen al campo PRODUCTO
        $("#producto").chosen({ placeholder_text_single: "Seleccione el producto", no_results_text: "No se encontró Producto" });

        $("#producto").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/OrdenCompraCliente/SearchProductos"
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
            url: "/OrdenCompraCliente/GetProducto",
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

                if (producto.descontinuado == 1) {
                    $("#spnProductoDescontinuado").show();

                    if (producto.motivoRestriccion != null) {
                        producto.motivoRestriccion = producto.motivoRestriccion.trim();

                        $("#spnProductoDescontinuado .lblAlertaProductoDescontinuado ").removeClass("tooltip-label");
                        if (producto.motivoRestriccion != "") {
                            $("#spnProductoDescontinuado .lblAlertaProductoDescontinuado ").addClass("tooltip-motivo-restriccion");
                            $("#spnProductoDescontinuado .lblAlertaProductoDescontinuado .tooltip-label-text").html(producto.motivoRestriccion);
                        }
                    }
                } else {
                    $("#spnProductoDescontinuado").hide();
                }

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
        var sessionOrdenCompraCliente = "ordenCompraCliente"
        if ($("#pagina").val() == PAGINA_BUSQUEDA_OCCS_VENTA) {
            idCliente = $("#verIdCliente").val();
            sessionOrdenCompraCliente = "ordenCompraClienteVer";
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
                controller: sessionOrdenCompraCliente
            },
            success: function (producto) {

                $("#verProducto").html(producto.nombre);
                $("#verCodigoProducto").html(producto.sku);
                
                $("#verUnidadProveedor").html(producto.unidadProveedor);
                $("#verUnidadMP").html(producto.unidad);
                $("#verUnidadAlternativa").html(producto.unidadAlternativa);

                $("#verPrecioProveedor").html(producto.precioProveedor);
                $("#verPrecioMP").html(producto.precio);
                $("#verPrecioAlternativa").html(producto.precioAlternativa);

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
    

    $(".chkOrdenCompraClienteSerchProductCheckParam").change(function () {
        var param = $(this).attr("paramName");
        var valor = 0;
        if ($(this).is(":checked")) {
            valor = $(this).attr("checkValue");
        } else {
            valor = $(this).attr("unCheckValue");
        }


        $.ajax({
            url: "/OrdenCompraCliente/SetSearchProductParam",
            type: 'POST',
            data: {
                parametro: param,
                valor: valor
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
            url: "/OrdenCompraCliente/AddProducto",
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

                    if (detalle.precioUnitario >= Number(precioLista) - Number(VARIACION_PRECIO_ITEM_OCC)
                        && detalle.precioUnitario <= Number(precioLista) + Number(VARIACION_PRECIO_ITEM_OCC))
                    {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right">' + detalle.precioUnitario + '</td>';
                        
                    }
                    else
                    {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right; color: #B9371B; font-weight:bold">' + detalle.precioUnitario + '</td>';

                    }


                }
                else {
                    

                    if (Number(detalle.precioUnitario) >= (Number(detalle.precioUnitarioRegistrado) - Number(VARIACION_PRECIO_ITEM_OCC))
                        && Number(detalle.precioUnitario) <= (Number(detalle.precioUnitarioRegistrado) + Number(VARIACION_PRECIO_ITEM_OCC)))
                    {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right">' + detalle.precioUnitario + '</td>';
                        
                    }
                    else
                    {
                    precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right; color: #B9371B; font-weight:bold">' + detalle.precioUnitario + '</td>';

                    }
                }

                var descontinuadoLabel = "";
                if (detalle.descontinuado == 1) {
                    descontinuadoLabel = "<br/>" + $("#spnProductoDescontinuado").html(); 

                    if (detalle.motivoRestriccion != null) {
                        detalle.motivoRestriccion = detalle.motivoRestriccion.trim();
                        descontinuadoLabel = descontinuadoLabel.replace("_DATA_TIPSO_", detalle.motivoRestriccion);

                        if (detalle.motivoRestriccion != "") {
                            descontinuadoLabel = descontinuadoLabel.replace("_CLASS_TOOLTIP_", "tooltip-motivo-restriccion");
                        }
                    }
                }


                $('#tableDetalleOrdenCompraCliente tbody tr.footable-empty').remove();
                $("#tableDetalleOrdenCompraCliente tbody").append('<tr data-expanded="true">' +
                    '<td>' + detalle.idProducto + '</td>' +
                    '<td>' + esPrecioAlternativo + '</td>' +

                    '<td>' + proveedor + '</td>' +
                    '<td>' + detalle.codigoProducto + descontinuadoLabel + '</td>' +
                     

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

                $('#tableDetalleOrdenCompraCliente thead tr th.footable-editing').remove();
                $('#tableDetalleOrdenCompraCliente tbody tr td.footable-editing').remove();

                $("#spnProductoDescontinuado").hide();

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
                            window.location = '/OrdenCompraCliente/Pedir';
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
            alert("Debe seleccionar una ciudad previamente.");
            $("#idCiudad").focus();
            $("#btnCancelarObtenerProductos").click();
            return false;
        }
        var idCliente = $("#idCliente").val();
        if (idCliente.trim() == "") {
            alert("Debe seleccionar un cliente.");
            $('#idCliente').trigger('chosen:activate');
            $("#btnCancelarObtenerProductos").click();
            return false;
        }


        if (verificarSiExisteDetalle()) {
            alert("No deben existir productos agregados al ordenCompraCliente.");
            return false;
        }

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
            url: "/OrdenCompraCliente/obtenerProductosAPartirdePreciosRegistrados",
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
                alert("Ocurrió un error al armar el detalle del ordenCompraCliente a partir de los precios registrados.");
                //window.location = '/OrdenCompraCliente/Cotizador';
            },
            success: function () {
                $('body').loadingModal('hide')
           /*     $.alert({
                    title: '¡Atención!',
                    type: 'orange',
                    content: "Los productos importados no consideran los precios registrados para un grupo.",
                    buttons: {
                        OK: function () {*/
                            window.location = '/OrdenCompraCliente/Pedir';
               /*         }
                    }
                });*/
                
            }
        });

    });


    $("#btnAgregarCanasta").click(function () {
        var idCliente = $("#idCliente").val();

        if (idCliente == "" || idCliente == '00000000-0000-0000-0000-000000000000') {
            $.alert({
                title: "Error",
                type: "red", 
                content: 'Debe seleccionar un cliente.',
                buttons: {
                    OK: function () {
                    }
                }
            });       
            return false;
        }

        $.confirm({
            title: 'IMPORTAR PRODUCTOS COTIZADOS',
            content: 'Seleccione una de las opciones para importar los productos cotizados.',
            type: 'orange',
            buttons: {
                aplica: {
                    text: 'PRECIOS VIGENTES',
                    btnClass: 'btn-success',
                    action: function () {
                        window.location = '/OrdenCompraCliente/CargarProductosCanasta?tipo=1';
                    }
                },
                noAplica: {
                    text: 'CANASTA HABITUAL',
                    btnClass: 'btn-warning',
                    action: function () {
                        window.location = '/OrdenCompraCliente/CargarProductosCanasta?tipo=2';
                    }
                },
                cancelar: {
                    text: 'CANCELAR',
                    btnClass: '',
                    action: function () {

                    }
                }
            }
        });

    });





    ////////CREAR/EDITAR COTIZACIÓN


    function validarIngresoDatosObligatoriosOrdenCompraCliente() {

        if ($("#idCliente").val().trim() == "") {

            $('#idCliente').trigger('chosen:activate');
            $("#idCiudadAsolicitar").focus();
            $.alert({
                title: TITLE_VALIDACION_OCC,
                content: 'Debe seleccionar un cliente.',
                buttons: {
                    OK: function () { }
                }
            });              
            return false;
        }


        //Si el tipo de ordenCompraCliente es traslado interno (84->'T')
      /*  if (tipoOrdenCompraCliente == TIPO_OCC_VENTA_TRASLADO_INTERNO_ENTREGADO.charCodeAt(0)) {
            if ($("#idCiudadASolicitar").val() == "" || $("#idCiudadASolicitar").val() == null) {
                $("#idCiudadAsolicitar").focus();
                $.alert({
                    title: TITLE_VALIDACION_OCC,
                    content: 'Debe seleccionar a que ciudad se solicita el traslado interno.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }
        }*/


        if ($("#occ_numeroReferenciaCliente").val().length > 20) {
            $("#occ_numeroReferenciaCliente").focus();
            $.alert({
                title: TITLE_VALIDACION_OCC,
                content: 'El número de referencia del cliente no debe contener más de 20 caracteres, si el dato a ingresar es más extenso agreguelo en Observaciones Factura.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }   

        /*if ($("#occ_numeroReferenciaCliente").val().trim() == "") {
            alert('Debe ingresar el número de orden de compra o ordenCompraCliente en el campo "Referencia Doc Cliente".');
            $('#occ_numeroReferenciaCliente').focus();
            return false;
        }*/






        if ($("#occ_solicitante").val().trim() == "") {
            $('#occ_solicitante').focus();
            $.alert({
                title: TITLE_VALIDACION_OCC,
                content: 'Debe seleccionar el solicitante.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }

        
        var fechaSolicitud = $("#fechaSolicitud").val();
        if (fechaSolicitud.trim() == "") {
            $("#fechaSolicitud").focus();
            $.alert({
                title: TITLE_VALIDACION_OCC,
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
                title: TITLE_VALIDACION_OCC,
                content: 'Debe ingresar la hora de la solicitud.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }


        
        if ($("#occ_solicitante_nombre").val().trim() == "") {
            $('#occ_solicitante_nombre').focus();
            $.alert({
                title: TITLE_VALIDACION_OCC,
                content: 'Debe ingresar el nombre de la persona que realizó la solicitud.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }



        if ($("#occ_solicitante_telefono").val().trim() == "" && $("#occ_solicitante_correo").val().trim() == "") {
            $('#occ_solicitante_telefono').focus();
            $.alert({
                title: TITLE_VALIDACION_OCC,
                content: 'Debe ingresar un telefono y/o correo de contacto de entrega.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }
        
        /*
        if ($("#occ_correoContactoOrdenCompraCliente").val().trim() != "" && !$("#occ_correoContactoOrdenCompraCliente").val().match(/^[a-zA-Z0-9\._-]+@[a-zA-Z0-9-]{2,}[.][a-zA-Z]{2,4}$/)) {
            alert("Debe ingresar un correo válido.");
            $("#occ_correoContactoOrdenCompraCliente").focus();
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
                title: TITLE_VALIDACION_OCC,
                content: 'Debe ingresar el detalle de la orden de compra.',
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


    $('input[name=fileOrdenCompraClientes]').change(function (e) {
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
            url: "/OrdenCompraCliente/ChangeFiles",
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
            url: "/OrdenCompraCliente/DescartarArchivos",
            type: 'POST',
            dataType: 'JSON',
            data: { nombreArchivo: nombreArchivo},
            error: function (detalle) { },
            success: function (ordenCompraClienteAdjuntoList) {

                $("#nombreArchivos > li").remove().end();

                for (var i = 0; i < ordenCompraClienteAdjuntoList.length; i++) {

                    var liHTML = '<a href="javascript:mostrar();" class="descargar">' + ordenCompraClienteAdjuntoList[i].nombre + '</a>' +
                        '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + ordenCompraClienteAdjuntoList[i].nombre + '" class="btnDeleteArchivo" /></a>';

                    $('#nombreArchivos').append($('<li />').html(liHTML));
              //      .appendTo($('#nombreArchivos'));

                }


            }
        });
    });

    $(document).on('click', "a.descargar", function () {

        //var arrrayClass = event.target.getAttribute("class").split(" ");
        var nombreArchivo = event.target.innerHTML;
        //var numeroOrdenCompraCliente = arrrayClass[1];

        $.ajax({
            url: "/OrdenCompraCliente/Descargar",
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


    function crearOrdenCompraCliente(continuarLuego) {
        if (!validarIngresoDatosObligatoriosOrdenCompraCliente())
            return false;


        $('body').loadingModal({
            text: 'Creando Orden de Compra...'
        });
        $.ajax({
            url: "/OrdenCompraCliente/Create",
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
                $("#occ_numeroOrdenCompraCliente").val(resultado.numeroOrdenCompraCliente);
                $("#idOrdenCompraCliente").val(resultado.idOrdenCompraCliente);

                if (resultado.estado == ESTADO_INGRESADO) {
                    $.alert({
                        title: TITLE_EXITO,
                        type: 'green',
                        content: "El ordenCompraCliente número " + resultado.numeroOrdenCompraCliente + " fue ingresado correctamente.",
                        buttons: {
                            OK: function () { window.location = '/OrdenCompraCliente/Index';  }
                        }
                    });
                     
                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    //alert("El ordenCompraCliente número " + resultado.numeroOrdenCompraCliente + " fue ingresado correctamente, sin embargo requiere APROBACIÓN")
                    $("#solicitudIngresoComentario").html("El ordenCompraCliente número " + resultado.numeroOrdenCompraCliente + " fue ingresado correctamente, sin embargo requiere APROBACIÓN, debe ingresar un comentario.")
                    $("#comentarioPendienteIngreso").val(resultado.observacion);
                    $("#modalComentarioPendienteIngreso").modal({
                        show: true,
                        keyboard: false,
                        backdrop: 'static'
                    });
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    ConfirmDialog("El ordenCompraCliente número " + resultado.numeroOrdenCompraCliente + " fue guardado correctamente. ¿Desea continuar editando ahora?", null, '/OrdenCompraCliente/CancelarCreacionOrdenCompraCliente');
                }
                else {
                    mostrarMensajeErrorProceso("El ordenCompraCliente ha tenido problemas para ser procesado; Contacte con el Administrador.");
                    window.location = '/OrdenCompraCliente/Index';
                }

            }
        });
    }

    function editarOrdenCompraCliente(continuarLuego) {
        if (!validarIngresoDatosObligatoriosOrdenCompraCliente())
            return false;
        $('body').loadingModal({
            text: 'Editando Orden de Compra...'
        });
        $.ajax({
            url: "/OrdenCompraCliente/Update",
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
                $("#occ_numeroOrdenCompraCliente").val(resultado.numeroOrdenCompraCliente);
                $("#idOrdenCompraCliente").val(resultado.idOrdenCompraCliente);

                if (resultado.estado == ESTADO_INGRESADO) {
                    //alert("El ordenCompraCliente número " + resultado.numeroOrdenCompraCliente + " fue editado correctamente.");
                    $.alert({
                        title: TITLE_EXITO,
                        type: 'green',
                        content: "El ordenCompraCliente número " + resultado.numeroOrdenCompraCliente + " fue editado correctamente.",
                        buttons: {
                            OK: function () { window.location = '/OrdenCompraCliente/Index'; }
                        }
                    });
                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    //alert("El ordenCompraCliente número " + resultado.numeroOrdenCompraCliente + " fue editado correctamente, sin embargo requiere APROBACIÓN")
                    $("#solicitudIngresoComentario").html("El ordenCompraCliente número " + resultado.numeroOrdenCompraCliente + " fue editado correctamente, sin embargo requiere APROBACIÓN, debe ingresar un comentario.")
                    $("#comentarioPendienteIngreso").val(resultado.observacion);
                    $("#modalComentarioPendienteIngreso").modal({
                        show: true,
                        keyboard: false,
                        backdrop: 'static'
                    });
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    ConfirmDialog("El ordenCompraCliente número " + resultado.numeroOrdenCompraCliente + " fue guardado correctamente. ¿Desea continuar editando ahora?", null, '/OrdenCompraCliente/CancelarCreacionOrdenCompraCliente');
                }
                else {
                    //alert("El ordenCompraCliente ha tenido problemas para ser procesado; Contacte con el Administrador.");
                    mostrarMensajeErrorProceso("El ordenCompraCliente ha tenido problemas para ser procesado; Contacte con el Administrador.");
                    window.location = '/OrdenCompraCliente/Index';
                }
            }
        });
    }

    $("#btnCancelarComentario").click(function () {
        window.location = '/OrdenCompraCliente/CancelarCreacionOrdenCompraCliente';
    });

    $("#btnAceptarComentario").click(function () {
        var codigoOrdenCompraCliente = $("#occ_numeroOrdenCompraCliente").val();
        var idOrdenCompraCliente = $("#idOrdenCompraCliente").val();
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
            url: "/OrdenCompraCliente/updateEstadoOrdenCompraCliente",
            data: {
                idOrdenCompraCliente: idOrdenCompraCliente,
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
                    content: "El comentario del estado del ordenCompraCliente número: " + codigoOrdenCompraCliente + " fue modificado.",
                    buttons: {
                        OK: function () { window.location = '/OrdenCompraCliente/Index';  }
                    }
                });
                $("#btnCancelarComentario").click();
            }
        });

    });
    /*
        $("#btnAceptarComentarioCrediticio").click(function () {
            var codigoOrdenCompraCliente = $("#occ_numeroOrdenCompraCliente").val();
            var observacion = $("#comentarioPendienteIngreso").val();
            $.ajax({
                url: "/Cotizacion/updateEstadoOrdenCompraClienteCrediticio",
                data: {
                    codigo: codigoOrdenCompraCliente,
                    estado: ESTADO_PENDIENTE_APROBACION,
                    observacion: observacion
                },
                type: 'POST',
                error: function () {
                    alert("Ocurrió un problema al intentar agregar un comentario al ordenCompraCliente.")
                    $("#btnCancelarComentario").click();
                },
                success: function () {
                    alert("El comentario del estado del ordenCompraCliente número: " + codigoOrdenCompraCliente + " se cambió correctamente.");
                    $("#btnCancelarComentario").click();
                }
            });

        });*/



    $("#btnFinalizarCreacionOrdenCompraCliente").click(function () {
        crearOrdenCompraCliente(0);
    });


    $("#btnFinalizarEdicionOrdenCompraCliente").click(function () {
        editarOrdenCompraCliente(0);
    });

    
    $("#btnContinuarEditandoLuego").click(function () {
        if ($("#occ_numeroOrdenCompraCliente").val() == "" || $("#occ_numeroOrdenCompraCliente").val() == null) {
            crearOrdenCompraCliente(1);
        }
        else {
            editarOrdenCompraCliente(1);
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









    /*VER OCC*/
    $(document).on('click', "button.btnVerOrdenCompraCliente", function () {

        $('body').loadingModal({
            text: 'Abriendo Orden de Compra...'
        });
        $('body').loadingModal('show');
        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idOrdenCompraCliente = arrrayClass[0];
        var numeroOrdenCompraCliente = arrrayClass[1];
      //  $("#tableDetalleCotizacion > tbody").empty();
        showOrdenCompraCliente(idOrdenCompraCliente);
     
    });

    var viendoOrdenCompraClienteRestringido = false;
    var ordenCompraClienteItemsRestringidos = [];


    function showOrdenCompraCliente(idOrdenCompraCliente) {
        $.ajax({
            url: "/OrdenCompraCliente/Show",
            data: {
                idOrdenCompraCliente: idOrdenCompraCliente
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                //alert("Ocurrió un problema al obtener el detalle del OrdenCompraCliente N° " + numeroOrdenCompraCliente + ".");
                $('body').loadingModal('hide');
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function (resultado) {
                $('body').loadingModal('hide');
                //var cotizacion = $.parseJSON(respuesta);
                var ordenCompraCliente = resultado.occ;
                var usuario = resultado.usuario;
                var serieDocumentoElectronicoList = resultado.serieDocumentoElectronicoList;
                viendoOrdenCompraClienteRestringido = false;
                ordenCompraClienteItemsRestringidos = [];
                //  var usuario = resultado.usuario;

                $("#verIdOrdenCompraCliente").val(ordenCompraCliente.idOrdenCompraCliente);


                $("#idOrdenCompraCliente").val(ordenCompraCliente.idOrdenCompraCliente);

                $("#verNumeroOrdenCompraCliente").html(ordenCompraCliente.numeroOrdenCompraClienteString);


                $("#verClienteSunat").html(ordenCompraCliente.clienteSunat.razonSocial);

                $("#verNumeroReferenciaCliente").html(ordenCompraCliente.numeroReferenciaCliente);

                $("#verFechaEntregaExtendida").val(ordenCompraCliente.fechaEntregaExtendidaString);


                $("#verTelefonoContactoEntrega").html(ordenCompraCliente.direccionEntrega_telefono);
                $("#verContactoEntrega").html(ordenCompraCliente.direccionEntrega_contacto);

                $("#verUsuarioCreacion").html(ordenCompraCliente.usuario.nombre);
                $("#verFechaHoraRegistro").html(ordenCompraCliente.fechaHoraRegistro);

                $("#verContactoOrdenCompraCliente").html(ordenCompraCliente.contactoOrdenCompraCliente);
                $("#verTelefonoCorreoContactoOrdenCompraCliente").html(ordenCompraCliente.telefonoCorreoContactoOrdenCompraCliente);


                $("#verFechaHoraSolicitud").html(ordenCompraCliente.fechaHoraSolicitud);

                $("#verModificadoPor").html(ordenCompraCliente.seguimientoOrdenCompraCliente_usuario_nombre);
                $("#verObservacionEstado").html(ordenCompraCliente.seguimientoOrdenCompraCliente_observacion);

                $("#verEstadoCrediticio").html(ordenCompraCliente.seguimientoCrediticioOrdenCompraCliente_estadoString);
                $("#verModificadoCrediticioPor").html(ordenCompraCliente.seguimientoCrediticioOrdenCompraCliente_usuario_nombre);
                $("#verObservacionEstadoCrediticio").html(ordenCompraCliente.seguimientoCrediticioOrdenCompraCliente_observacion);


                $("#verObservaciones").html(ordenCompraCliente.observaciones);
                $("#verMontoSubTotal").html(Number(ordenCompraCliente.montoSubTotal).toFixed(cantidadDecimales));
                $("#verMontoIGV").html(Number(ordenCompraCliente.montoIGV).toFixed(cantidadDecimales));
                $("#verMontoTotal").html(Number(ordenCompraCliente.montoTotal).toFixed(cantidadDecimales));

                /*      $("#verMontoSubTotalVenta").html(Number(ordenCompraCliente.venta.subTotal).toFixed(cantidadDecimales));
                      $("#verMontoIGVVenta").html(Number(ordenCompraCliente.venta.igv).toFixed(cantidadDecimales));
                      $("#verMontoTotalVenta").html(Number(ordenCompraCliente.venta.total).toFixed(cantidadDecimales));
      */
                //  nombreArchivos


                //$("#nombreArchivos > li").remove().end();

                //for (var i = 0; i < ordenCompraCliente.ordenCompraClienteAdjuntoList.length; i++) {
                //    var liHTML = '<a href="javascript:mostrar();" class="descargar">' + ordenCompraCliente.ordenCompraClienteAdjuntoList[i].nombre + '</a>';
                //    $('<li />').html(liHTML).appendTo($('#nombreArchivos'));
                //}

                //$("#verNombreArchivos > li").remove().end();


                //for (var i = 0; i < ordenCompraCliente.ordenCompraClienteAdjuntoList.length; i++) {
                //    var liHTML = '<a href="javascript:mostrar();" class="descargar">' + ordenCompraCliente.ordenCompraClienteAdjuntoList[i].nombre + '</a>';
                //    //$('<li />').html(liHTML).appendTo($('#nombreArchivos'));
                //    $('#verNombreArchivos').append($('<li />').html(liHTML));
                //}


                $("#tableDetalleOrdenCompraCliente > tbody").empty();

                FooTable.init('#tableDetalleOrdenCompraCliente');

                $("#formVerGuiasRemision").html("");
                $("#formVerNotasIngreso").html("");

                var d = '';
                var lista = ordenCompraCliente.detalleList;
                var tieneProductoRestringido = false;
                var tienePendienteAtencion = false;
                for (var i = 0; i < lista.length; i++) {

                    var imgIndicadorAprobacion = '<a data-toggle="tooltip" title="Aprobado"> <img class="table-product-img"  src="/images/semaforo_verde_small.png"  srcset="semaforo_verde_min.png 2x"/></a>';
                    if (lista[i].indicadorAprobacion == 2)
                        imgIndicadorAprobacion = '<a data-toggle="tooltip" title="Pendiente Aprobación"> <img class="table-product-img" src="/images/semaforo_naranja_small.png" srcset="semaforo_naranja_min.png 2x"/></a>';
                    else if (lista[i].indicadorAprobacion == 3)
                        imgIndicadorAprobacion = '<a data-toggle="tooltip" title="Pendiente Aprobación"><img class="table-product-img " src="/images/semaforo_rojo_small.png" srcset="semaforo_rojo_min.png 2x"/></a>';


                    var observacion = lista[i].observacion == null || lista[i].observacion == 'undefined' ? '' : lista[i].observacion;

                    var descontinuadoLabel = "";
                    if (lista[i].producto.descontinuado == 1) {
                        tieneProductoRestringido = true;

                        if (lista[i].producto.motivoRestriccion != null) {
                            lista[i].producto.motivoRestriccion = lista[i].producto.motivoRestriccion.trim();

                            $("#spnProductoDescontinuado .lblAlertaProductoDescontinuado ").removeClass("tooltip-motivo-restriccion");
                            if (lista[i].producto.motivoRestriccion != "") {
                                $("#spnProductoDescontinuado .lblAlertaProductoDescontinuado ").addClass("tooltip-motivo-restriccion");
                                $("#spnProductoDescontinuado .lblAlertaProductoDescontinuado .tooltip-label-text").html(lista[i].producto.motivoRestriccion);
                            }
                        }

                        descontinuadoLabel = "<br/>" + $("#spnProductoDescontinuado").html();
                    }

                    itemOrdenCompraCliente = {
                        idOrdenCompraClienteDetalle: lista[i].idOrdenCompraClienteDetalle, sku: lista[i].producto.sku, producto: lista[i].producto.descripcion, unidad: lista[i].unidad,
                        idProducto: lista[i].producto.idProducto, cantidad: lista[i].cantidad, cantidadPendienteAtencion: lista[i].cantidadPendienteAtencion,
                        cantidadPermitida: lista[i].cantidadPermitida, observacionRestriccion: lista[i].observacionRestriccion
                    };


                    ordenCompraClienteItemsRestringidos.push(itemOrdenCompraCliente);

                    if (lista[i].cantidadPendienteAtencion > 0) {
                        tienePendienteAtencion = true;
                    }

                    d += '<tr>' +
                        '<td>' + imgIndicadorAprobacion + '</td>' +
                        '<td>' + lista[i].producto.proveedor + '</td>' +
                        '<td>' + lista[i].producto.sku + descontinuadoLabel + '</td>' +
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

                var sedes = ordenCompraCliente.sedesClienteSunat;
                var sedesText = '<option value="">Seleccione una Sede</option>';
                for (var k = 0; k < sedes.length; k++) {
                    sedesText = sedesText + '<option value="' + sedes[k].idClienteRelacionado + '">' + sedes[k].nombre + '</option>';
                }
                $("#occ_generar_pedido_ciudad").html(sedesText);

                $("#verRazonSocialSunat").html(ordenCompraCliente.cliente_razonSocialSunat);
                $("#verRUC").html(ordenCompraCliente.cliente_ruc);
                $("#verDireccionDomicilioLegalSunat").html(ordenCompraCliente.cliente_direccionDomicilioLegalSunat);
                $("#verCodigo").html(ordenCompraCliente.cliente_codigo);




               
                //var guiaRemisionList = ordenCompraCliente.guiaRemisionList;
                //for (var j = 0; j < guiaRemisionList.length; j++) {
                //    $("#tableDetalleGuia > tbody").empty();
                //    var plantilla = $("#plantillaVerGuiasRemision").html();
                //    var dGuia = '';
                //    var documentoDetalleList = guiaRemisionList[j].documentoDetalle;
                //    for (var k = 0; k < documentoDetalleList.length; k++) {

                //        dGuia += '<tr>' +
                //            '<td>' + documentoDetalleList[k].producto.sku + '</td>' +
                //            '<td>' + documentoDetalleList[k].cantidad + '</td>' +
                //            '<td>' + documentoDetalleList[k].unidad + '</td>' +
                //            '<td>' + documentoDetalleList[k].producto.descripcion + '</td>' +
                //            '</tr>';
                //    }

                //    $("#tableDetalleGuia").append(dGuia);

                //    plantilla = $("#plantillaVerGuiasRemision").html();

                //    plantilla = plantilla.replace("#serieNumero", guiaRemisionList[j].serieNumeroGuia);
                //    plantilla = plantilla.replace("#fechaEmisionGuia", invertirFormatoFecha(guiaRemisionList[j].fechaEmision.substr(0, 10)));

                //    plantilla = plantilla.replace("#serieNumeroFactura", guiaRemisionList[j].documentoVenta.serieNumero);
                //    if (guiaRemisionList[j].documentoVenta.fechaEmision != null) {
                //        plantilla = plantilla.replace("#fechaEmisionFactura", invertirFormatoFecha(guiaRemisionList[j].documentoVenta.fechaEmision.substr(0, 10)));
                //    }
                //    else
                //        plantilla = plantilla.replace("#fechaEmisionFactura", "");


                //    plantilla = plantilla.replace("tableDetalleGuia", "tableDetalleGuia" + j);

                //    $("#formVerGuiasRemision").append(plantilla);

                //    /*   var tmp = $("#formVerGuiasRemision").html();
                //        tmp = tmp+"asas";*/
                //}


               
                    //var notaIngresoList = ordenCompraCliente.guiaRemisionList;
                    //for (var j = 0; j < notaIngresoList.length; j++) {
                    //    $("#tableDetalleNotaIngreso > tbody").empty();
                    //    var plantilla = $("#plantillaVerNotaIngreso").html();
                    //    var dGuia = '';
                    //    var documentoDetalleList = notaIngresoList[j].documentoDetalle;
                    //    for (var k = 0; k < documentoDetalleList.length; k++) {

                    //        dGuia += '<tr>' +
                    //            '<td>' + documentoDetalleList[k].producto.sku + '</td>' +
                    //            '<td>' + documentoDetalleList[k].cantidad + '</td>' +
                    //            '<td>' + documentoDetalleList[k].unidad + '</td>' +
                    //            '<td>' + documentoDetalleList[k].producto.descripcion + '</td>' +
                    //            '</tr>';
                    //    }

                    //    $("#tableDetalleNotaIngreso").append(dGuia);

                    //    plantilla = $("#plantillaVerNotaIngreso").html();

                    //    plantilla = plantilla.replace("#serieNumero", notaIngresoList[j].serieNumeroGuia);
                    //    plantilla = plantilla.replace("#fechaEmisionNotaIngreso", invertirFormatoFecha(notaIngresoList[j].fechaEmision.substr(0, 10)));

                    //    plantilla = plantilla.replace("tableDetalleNotaIngreso", "tableDetalleNotaIngreso" + j);

                    //    $("#formVerNotasIngreso").append(plantilla);

                    //    /*   var tmp = $("#formVerGuiasRemision").html();
                    //       tmp = tmp+"asas";*/
                    //}
            

                //  
                // sleep
                $("#tableDetalleOrdenCompraCliente").append(d);



                


                //APROBAR OCC
                //if ((ordenCompraCliente.seguimientoOrdenCompraCliente_estado == ESTADO_PENDIENTE_APROBACION ||
                //    ordenCompraCliente.seguimientoOrdenCompraCliente_estado == ESTADO_DENEGADO)
                //    &&
                //    (!tieneProductoRestringido || (tieneProductoRestringido && usuario.apruebaOrdenCompraClientesVentaRestringida))
                //) {

                //    $("#btnAprobarIngresoOrdenCompraCliente").show();
                //}
                //else {
                //    $("#btnAprobarIngresoOrdenCompraCliente").hide();
                //}

                viendoOrdenCompraClienteRestringido = tieneProductoRestringido;

                //ATENDER 
                
               

                $("#modalVerOrdenCompraCliente").modal('show');

                //  window.location = '/OrdenCompraCliente/Index';
            }
        });
    }




   
    $("#btnCancelarOrdenCompraCliente").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/OrdenCompraCliente/CancelarCreacionOrdenCompraCliente', null)
    })


    

    $("#btnFacturarOrdenCompraCliente").click(function () {
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

    $("#btnCancelarFacturarOrdenCompraCliente").click(function () {

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


  


    $("#btnEditarOrdenCompraCliente").click(function () {
        desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona
        $.ajax({
            url: "/OrdenCompraCliente/ConsultarSiExisteOrdenCompraCliente",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/OrdenCompraCliente/iniciarEdicionOrdenCompraCliente",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del ordenCompraCliente."); },
                        success: function (fileName) {
                            window.location = '/OrdenCompraCliente/Pedir';
                        }
                    });

                }
                else {
                    if(resultado.numero == 0) {
                        alert('Está creando un nuevo ordenCompraCliente; para continuar por favor diríjase a la página "Crear/Modificar OrdenCompraCliente" y luego haga clic en el botón Cancelar, Finalizar Creación o Guardar y Continuar Editando Luego.');
                    }
                        else {
                        if(resultado.numero == $("#verNumero").html())
                                alert('Ya se encuentra editando el ordenCompraCliente número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar OrdenCompraCliente".');
                            else
                                alert('Está editando el ordenCompraCliente número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar OrdenCompraCliente" y luego haga clic en el botón Cancelar, Finalizar Edición o Guardar y Continuar Editando Luego.');
                    }
                    activarBotonesVer();
                }
            }
        });
    });


    $("#btnGuardarActualizarOrdenCompraCliente").click(function () {

        /*    $("#btnGuardarArchivosAdjuntos").click(function () {
        $.ajax({
            url: "/OrdenCompraCliente/UpdateArchivosAdjuntos",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                mostrarMensajeErrorProceso();
            },
            success: function (resultado) {
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El ordenCompraCliente número " + resultado.numeroOrdenCompraCliente + " fue editado correctamente.",
                    buttons: {
                       OK: function() {
                            $("#btnCancelarVerOrdenCompraCliente").click();
                        }
                        //OK: function () { window.location = '/OrdenCompraCliente/Index'; }
                    }
                });
            }
        });
    })*/

        if ($("#occ_numeroReferenciaCliente2").val().length > 20) {
            $("#occ_numeroReferenciaCliente2").focus();
            $.alert({
                title: TITLE_VALIDACION_OCC,
                content: 'El número de referencia del cliente no debe contener más de 20 caracteres, si el dato a ingresar es más extenso agreguelo en Observaciones Factura.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }   


        var numeroReferenciaCliente = $("#occ_numeroReferenciaCliente2").val();
        var numeroReferenciaAdicional = $("#occ_numeroReferenciaAdicional").val();

        var fechaEntregaExtendida = $("#occ_fechaEntregaExtendida").val();

        var observaciones = $("#occ_observaciones").val();
        var observacionesGuiaRemision = $("#occ_observacionesGuiaRemision").val();
        var observacionesFactura = $("#occ_observacionesFactura").val();

        var ordenCompraClienteNumeroGrupo = $("#occ_numeroGrupo").val();
        


        $.ajax({
            url: "/OrdenCompraCliente/UpdatePost",
         /*   data: {
                idOrdenCompraCliente: idOrdenCompraCliente
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
                ordenCompraClienteNumeroGrupo: ordenCompraClienteNumeroGrupo
            },
            dataType: 'JSON',
            error: function (detalle) {
                //alert("Ocurrió un problema al obtener el detalle del OrdenCompraCliente N° " + numeroOrdenCompraCliente + ".");
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function (resultado) {
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El ordenCompraCliente número " + resultado.numeroOrdenCompraCliente + " fue actualizado correctamente.",
                    buttons: {
                        OK: function () {
                            $("#btnCancelarActualizarOrdenCompraCliente").click();
                            window.location = '/OrdenCompraCliente/Index';
                        }
                        //OK: function () { window.location = '/OrdenCompraCliente/Index'; }
                    }
                });


            }
        })
    });
    



    $("#btnActualizarOrdenCompraCliente").click(function () {
        $("#verActualizarNumero").html($("#verNumero").html());
        $("#verActualizarCiudad").html($("#verCiudad").html());
        $("#verActualizarCliente").html($("#verCliente").html());

        $("#verActualizarMontoSubTotal").html($("#verMontoSubTotal").html());
        $("#verActualizarMontoIGV").html($("#verMontoIGV").html());
        $("#verActualizarMontoTotal").html($("#verMontoTotal").html());

        $("#occ_numeroReferenciaCliente2").val($("#verNumeroReferenciaCliente").html());
        $("#occ_numeroReferenciaAdicional").val($("#verNumeroReferenciaAdicional").html());

        $("#occ_fechaEntregaExtendida").val($("#verFechaEntregaExtendida").val());

        $("#occ_observacionesFactura").val($("#verObservacionesFactura").html());
        $("#occ_observacionesGuiaRemision").val($("#verObservacionesGuiaRemision").html());
        $("#occ_observaciones").val($("#verObservaciones").html());
        
    });



    function mostrarItemsGenerarPedido() {
        $(".tableProductosGenerarPedido tbody").empty();
        /*if (viendoOrdenCompraClienteRestringido) {*/
        $(".divProductosGenerarPedido").show();
        fLen = ordenCompraClienteItemsRestringidos.length;

        text = "";
        for (i = 0; i < fLen; i++) {
            var cantidadRestringir = parseInt(ordenCompraClienteItemsRestringidos[i].cantidad) - parseInt(ordenCompraClienteItemsRestringidos[i].cantidadPermitida);
            var cantidadAtendida = parseInt(ordenCompraClienteItemsRestringidos[i].cantidad) - parseInt(ordenCompraClienteItemsRestringidos[i].cantidadPendienteAtencion);
            text += '<tr idOrdenCompraClienteDetalle="' + ordenCompraClienteItemsRestringidos[i].idProducto + '">';
            text += '<td>' + ordenCompraClienteItemsRestringidos[i].sku + ' ' + ordenCompraClienteItemsRestringidos[i].producto + '</td>';
            text += '<td>' + ordenCompraClienteItemsRestringidos[i].unidad + '</td>';
            text += '<td class="celdaItemCantidad">' + ordenCompraClienteItemsRestringidos[i].cantidad + '</td>';
            text += '<td class="celdaItemCantidadRestante">' + ordenCompraClienteItemsRestringidos[i].cantidad + '</td>';
            text += '<td>' + 
                '<input class="form-control inputItemAtender" type="number" min="0" max="' + ordenCompraClienteItemsRestringidos[i].cantidad + '" step="1" value="0">' +
                    '</td>';
            text += '<td class=""><input type="text" class="inputItemObervacionDetalle form-control" value=""></td>';
            text += '</tr>';
        }

        $(".tableProductosGenerarPedido tbody").html(text);

        FooTable.init('.tableProductosGenerarPedido');
       /* } else {
            $(".divProductosAprobarOrdenCompraCliente").hide();
        }*/
    }


    $('.divProductosGenerarPedido').on('change', 'tr td .inputItemAtender', function (e) {
        var cantidadAtender = parseInt($(this).val());
        var cantidad = parseInt($(this).closest('tr').find('td.celdaItemCantidadRestante').html());
        
        if (cantidadAtender > cantidad) {
            cantidadAtender = cantidad;
            $(this).val(cantidadAtender);
        }

        if (cantidadAtender < 0) {
            cantidadAtender = 0;
            $(this).val(cantidadAtender);
        }

        var idDetalle = $(this).closest('tr').attr('idOrdenCompraClienteDetalle');
        var comentario = $(this).closest('tr').find('td .inputItemObervacionDetalle').val();

        ActualizarDetalleGenerarPedido(idDetalle, cantidadAtender, comentario);
    });


    $('.divProductosGenerarPedido').on('change', 'tr td .inputItemObervacionDetalle', function (e) {
        var idDetalle = $(this).closest('tr').attr('idOrdenCompraClienteDetalle');
        var cantidadAtender = $(this).closest('tr').find('td .inputItemAtender').val();
        var comentario = $(this).val();

        ActualizarDetalleGenerarPedido(idDetalle, cantidadAtender, comentario);
    });

    $('.divProductosGenerarPedido').on('change', '#occ_generar_pedido_ciudad', function (e) {
        var idClienteSede = $(this).val();

        ActualizarIdClienteGenerarPedido(idClienteSede);
    });

    function ActualizarDetalleGenerarPedido(idDetalle, cantidad, comentario) {
        $.ajax({
            url: "/OrdenCompraCliente/SetDetalleGenerarPedido",
            type: 'POST',
            data: {
                idProducto: idDetalle,
                cantidad: cantidad,
                comentario: comentario
            },
            success: function () {
            }
        });
    }

    function ActualizarIdClienteGenerarPedido(idCliente) {
        $.ajax({
            url: "/OrdenCompraCliente/ChangeIdSedePedidoGenerar",
            type: 'POST',
            data: {
                idCliente: idCliente
            },
            success: function () {
            }
        });
    }


    $("#btnRegistrarPedido").click(function () {

        var idClienteSede = $('#occ_generar_pedido_ciudad').val();

        if (idClienteSede == "") {
            $.alert({
                title: 'Advertencia',
                content: "Seleccione una sede",
                type: 'orange',
                buttons: { 
                    OK: function () {

                    }
                }
            });

            return false;
        }

        //Se identifica si existe pedido en curso, la consulta es sincrona
        $.ajax({
            url: "/Pedido/ConsultarSiExistePedido",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/Pedido/iniciarEdicionPedidoDesdeOC",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del pedido."); },
                        success: function (fileName) {
                            window.location = '/Pedido/Pedir';
                        }
                    });

                }
                else {
                    if (resultado.numero == 0) {
                        alert('Ya está creando un nuevo pedido; para continuar por favor diríjase a la página "Crear/Modificar Pedido" y luego haga clic en el botón Cancelar, Finalizar Creación o Guardar (si elige Guardar indique No cuando se le consulte si desea continuar editanto ahora).');
                    }
                    else {
                        if (resultado.numero == $("#verNumero").html())
                            alert('Ya se encuentra editando el pedido número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Cotización".');
                        else
                            alert('Ya está editando el pedido número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Pedido" y luego haga clic en el botón Cancelar, Finalizar Edición o Guardar (si elige Guardar indique No cuando se le consulte si desea continuar editanto ahora).');
                    }
                }
            }
        });

    });


    function limpiarComentario()
    {
        $("#comentarioEstado").val("");
        $("#comentarioEstado").focus();
    }



    $("#btnLiberarOrdenCompraCliente").click(function () {

        $("#labelNuevoEstadoCrediticio").html(ESTADO_LIBERADO_STR);
        $("#estadoCrediticioId").val(ESTADO_LIBERADO);
        limpiarComentario();
    });

    $("#btnBloquearOrdenCompraCliente").click(function () {

        $("#labelNuevoEstadoCrediticio").html(ESTADO_BLOQUEADO_STR);
        $("#estadoCrediticioId").val(ESTADO_BLOQUEADO);
        limpiarComentario();
    });

    $("#btnAprobarIngresoOrdenCompraCliente").click(function () {
        $("#modalAprobacionTitle").html(TITULO_APROBAR_INGRESO);
        $("#labelNuevoEstado").html(ESTADO_INGRESADO_STR);
        $("#estadoId").val(ESTADO_INGRESADO);
        limpiarComentario();
        mostrarItemsRestringidos();
    });

    
    $("#btnGenerarPedido").click(function () {
        mostrarItemsGenerarPedido();
    });


    $("#btnEliminarOrdenCompraCliente").click(function () {
        $("#modalAprobacionTitle").html(TITULO_ELIMINAR);
        $("#labelNuevoEstado").html(ESTADO_ELIMINADO_STR);
        $("#estadoId").val(ESTADO_ELIMINADO);
        limpiarComentario();
    });

    $("#btnDenegarIngresoOrdenCompraCliente").click(function () {
        $("#modalAprobacionTitle").html(TITULO_DENEGAR_INGRESO);
        $("#labelNuevoEstado").html(ESTADO_DENEGADO_STR);
        $("#estadoId").val(ESTADO_DENEGADO);
        limpiarComentario();
    });



    $("#btnCancelarProgramacionOrdenCompraCliente").click(function () {
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
                $.alert({
                    title: 'Advertencia',
                    content: "Cuando Bloquea un ordenCompraCliente debe ingresar un Comentario.",
                    type: 'orange',
                    buttons: {

                        OK: function () {

                        }
                    }
                });

                return false;
            }
        }
        var codigo = $("#verNumero").html();
        var idOrdenCompraCliente = $("#idOrdenCompraCliente").val();

        $.ajax({
            url: "/OrdenCompraCliente/updateEstadoOrdenCompraClienteCrediticio",
            data: {
                idOrdenCompraCliente: idOrdenCompraCliente,
                estado: estado,
                observacion: comentarioEstado
            },
            type: 'POST',
            error: function () {
                $.alert({
                    title: 'Error',
                    content: "Ocurrió un problema al intentar cambiar el estado del ordenCompraCliente.",
                    type: 'red',
                    buttons: {

                        OK: function () {

                        }
                    }
                });
                
                $("#btnCancelarCambioEstadoCrediticio").click();
            },
            success: function () {
                $.alert({
                    title: 'Registro Correcto',
                    content: "El estado crediticio del ordenCompraCliente número: " + codigo + " se cambió correctamente.",
                    type: 'green',
                    buttons: {

                        OK: function () {

                        }
                    }
                });
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
                $.alert({
                    title: 'Advertencia',
                    content: "Debe ingresar un Comentario.",
                    type: 'orange',
                    buttons: {

                        OK: function () {

                        }
                    }
                });
                return false;
            }
        }
        var codigo = $("#verNumero").html();
        var idOrdenCompraCliente = $("#idOrdenCompraCliente").val();        

        $.ajax({
            url: "/OrdenCompraCliente/updateEstadoOrdenCompraCliente",
            data: {
                idOrdenCompraCliente: idOrdenCompraCliente,
                estado: estado,
                observacion: comentarioEstado
            },
            type: 'POST',
            error: function () {
                $.alert({
                    title: 'Error',
                    content: "Ocurrió un problema al intentar cambiar el estado del ordenCompraCliente.",
                    type: 'red',
                    buttons: {

                        OK: function () {

                        }
                    }
                });
                $("#btnCancelarCambioEstado").click();
            },
            success: function () {
                $.alert({
                    title: 'Registro Correcto',
                    content: "El estado del ordenCompraCliente número: " + codigo + " se cambió correctamente.",
                    type: 'green',
                    buttons: {

                        OK: function () {

                        }
                    }
                });
                location.reload();
            }
        });
    });


    $("#btnAceptarDetallesRestriccion").click(function () {
        var codigo = $("#verNumero").html();
        var idOrdenCompraCliente = $("#idOrdenCompraCliente").val(); 
        $.ajax({
            url: "/OrdenCompraCliente/UpdateDetallesRestriccion",
            data: {
                idOrdenCompraCliente: idOrdenCompraCliente
            },
            type: 'POST',
            error: function () {
                $.alert({
                    title: 'Error',
                    content: "Ocurrió un problema al actualizar la restricción de atención del ordenCompraCliente.",
                    type: 'red',
                    buttons: {

                        OK: function () {

                        }
                    }
                });
                $("#btnCancelarDetallesRestriccion").click();
            },
            success: function () {
                $.alert({
                    title: 'Registro Correcto',
                    content: "El ordenCompraCliente número: " + codigo + " registró la restricción de la atención correctamente.",
                    type: 'green',
                    buttons: {

                        OK: function () {

                        }
                    }
                });
                location.reload();
            }
        });
    });



    $("#btnCancelarCambioEstado").click(function () {
        $("#divProductosAprobarOrdenCompraCliente").hide()
    });

    
    var ft = null;



    //Mantener en Session cambio de Seleccion de IGV
    $("input[name=igv]").on("click", function () {
        var igv = $("input[name=igv]:checked").val();
        $.ajax({
            url: "/OrdenCompraCliente/updateSeleccionIGV",
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
            url: "/OrdenCompraCliente/updateSeleccionConsiderarCantidades",
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
            url: "/OrdenCompraCliente/updateMostrarCodigoProveedor",
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
            url: "/OrdenCompraCliente/updateCliente",
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
            url: "/OrdenCompraCliente/updateContacto",
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
            url: "/OrdenCompraCliente/updateFlete",
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
            url: "/OrdenCompraCliente/changeMostrarCosto",
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
        var $modal = $('#tableDetalleOrdenCompraCliente'),
            $editor = $('#tableDetalleOrdenCompraCliente'),
            $editorTitle = $('#tableDetalleOrdenCompraCliente');

     
        ft = FooTable.init('#tableDetalleOrdenCompraCliente', {
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
                                                    url: "/OrdenCompraCliente/DelProducto",
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

        var codigo = $("#occ_numeroOrdenCompraCliente").val();
        if (codigo == "") {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarCreacionOrdenCompraCliente").attr('disabled', 'disabled');
            $("#btnCancelarOrdenCompraCliente").attr('disabled', 'disabled');
        }
        else {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarEdicionOrdenCompraCliente").attr('disabled', 'disabled');
            $("#btnCancelarOrdenCompraCliente").attr('disabled', 'disabled');
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
            url: "/OrdenCompraCliente/ChangeDetalle",
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


    $("#btnLimpiarBusquedaOrdenCompraClientes").click(function () {
        $.ajax({
            url: "/OrdenCompraCliente/CleanBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });


    $(document).on('click', "input.chkStockConfirmado", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idPEdido = arrrayClass[0];
        var numeroOrdenCompraCliente = arrrayClass[1];

        var stockConfirmado = 0;
        if (event.target.checked) {
            stockConfirmado = 1;
        }


        $.ajax({
            url: "/OrdenCompraCliente/UpdateStockConfirmado",
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


    $("#btnBusquedaOrdenCompraClientes").click(function () {

       
        var idCiudad = $("#idCiudad").val();
        var idClienteSunat = $("#idClienteSunat").val();
        var fechaCreacionDesde = $("#occ_fechaCreacionDesde").val();
        var fechaCreacionHasta = $("#occ_fechaCreacionHasta").val();
        var ordenCompraCliente_numeroOrdenCompraCliente = $("#occ_numeroOrdenCompraCliente").val();
        $("#btnBusquedaOrdenCompraClientes").attr("disabled", "disabled");
        $.ajax({
            url: "/OrdenCompraCliente/Search",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idClienteSunat: idClienteSunat,
                fechaCreacionDesde: fechaCreacionDesde,
                fechaCreacionHasta: fechaCreacionHasta,
                numero: ordenCompraCliente_numeroOrdenCompraCliente
            },
            error: function () {
                $("#btnBusquedaOrdenCompraClientes").removeAttr("disabled");
            },

            success: function (ordenCompraClienteList) {
                $("#btnBusquedaOrdenCompraClientes").removeAttr("disabled");

                $("#tableOrdenCompraClientes > tbody").empty();
                //FooTable.init('#tableCotizaciones');
                $("#tableOrdenCompraClientes").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < ordenCompraClienteList.length; i++) {


                    var observaciones = ordenCompraClienteList[i].observaciones == null || ordenCompraClienteList[i].observaciones == 'undefined' ? '' : ordenCompraClienteList[i].observaciones;

                    if (ordenCompraClienteList[i].observaciones != null && ordenCompraClienteList[i].observaciones.length > 20) {
                        var idComentarioCorto = ordenCompraClienteList[i].idOrdenCompraCliente + "corto";
                        var idComentarioLargo = ordenCompraClienteList[i].idOrdenCompraCliente + "largo";
                        var idVerMas = ordenCompraClienteList[i].idOrdenCompraCliente + "verMas";
                        var idVermenos = ordenCompraClienteList[i].idOrdenCompraCliente + "verMenos";
                        var comentario = ordenCompraClienteList[i].observaciones.substr(0, 20) + "...";

                        observaciones = '<div id="' + idComentarioCorto + '" style="display:block;">' + comentario + '</div>' +
                            '<div id="' + idComentarioLargo + '" style="display:none;">' + ordenCompraClienteList[i].observaciones + '</div>' +
                            '<p><a id="' + idVerMas + '" class="' + ordenCompraClienteList[i].idOrdenCompraCliente + ' verMas" href="javascript:mostrar();" style="display:block">Ver Más</a></p>' +
                            '<p><a id="' + idVermenos + '" class="' + ordenCompraClienteList[i].idOrdenCompraCliente + ' verMenos" href="javascript:mostrar();" style="display:none">Ver Menos</a></p>';
                    }

                    
                    var grupoCliente = ordenCompraClienteList[i].grupoCliente_nombre == null ? "" : ordenCompraClienteList[i].grupoCliente_nombre;
                    var ordenCompraCliente = '<tr data-expanded="true">' +
                        '<td>  ' + ordenCompraClienteList[i].idOrdenCompraCliente + '</td>' +
                        '<td>  ' + ordenCompraClienteList[i].numeroOrdenCompraCliente + '  </td>' +
                        '<td>  ' + ordenCompraClienteList[i].numeroReferenciaCliente + '  </td>' +
                        '<td>  ' + ordenCompraClienteList[i].cliente_ruc + ' </td>' +
                        '<td>  ' + ordenCompraClienteList[i].cliente_razonSocial + '</td>' +
                        '<td>  ' + ordenCompraClienteList[i].usuario_nombre + '  </td>' +
                        '<td>  ' + ordenCompraClienteList[i].fechaHoraRegistro + '</td>' +
                        '<td>  ' + ordenCompraClienteList[i].montoTotal + '  </td>' +
                        '<td> ' + observaciones + ' </td>' + 
                        '<td>' +
                        '<button type="button" class="' + ordenCompraClienteList[i].idOrdenCompraCliente + ' ' + ordenCompraClienteList[i].numeroOrdenCompraCliente + ' btnVerOrdenCompraCliente btn btn-primary ">Ver</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tableOrdenCompraClientes").append(ordenCompraCliente);
                 
                }

                if (ordenCompraClienteList.length > 0) {
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



    $("#occ_fechaSolicitudDesde").change(function () {
        var fechaSolicitudDesde = $("#occ_fechaSolicitudDesde").val();
        $.ajax({
            url: "/OrdenCompraCliente/ChangeFechaSolicitudDesde",
            type: 'POST',
            data: {
                fechaSolicitudDesde: fechaSolicitudDesde
            },
            success: function () {
            }
        });
    });

    $("#occ_fechaSolicitudHasta").change(function () {
        var fechaSolicitudHasta = $("#occ_fechaSolicitudHasta").val();
        $.ajax({
            url: "/OrdenCompraCliente/ChangeFechaSolicitudHasta",
            type: 'POST',
            data: {
                fechaSolicitudHasta: fechaSolicitudHasta
            },
            success: function () {
            }
        });
    });
    

    $("#occ_numeroOrdenCompraCliente").change(function () {
        var numero = $("#occ_numeroOrdenCompraCliente").val();
        $.ajax({
            url: "/OrdenCompraCliente/changeNumero",
            type: 'POST',
            data: {
                numero: numero
            },
            success: function () {
            }
        });
    });

    $("#occ_numeroGrupoOrdenCompraCliente").change(function () {
        var numeroGrupo = $("#occ_numeroGrupoOrdenCompraCliente").val();
        $.ajax({
            url: "/OrdenCompraCliente/changeNumeroGrupo",
            type: 'POST',
            data: {
                numeroGrupo: numeroGrupo
            },
            success: function () {
            }
        });
    });

    $("#occ_idGrupoCliente").change(function () {
        var idGrupoCliente = $("#occ_idGrupoCliente").val();
        $.ajax({
            url: "/OrdenCompraCliente/ChangeIdGrupoCliente",
            type: 'POST',
            data: {
                idGrupoCliente: idGrupoCliente
            },
            success: function () {
            }
        });
    });
    

    $("#occ_buscarSedesGrupoCliente").change(function () {
        var valor = $("input[name=ordenCompraCliente_buscarSedesGrupoCliente]:checked").val();
        $.ajax({
            url: "/OrdenCompraCliente/ChangeBuscarSedesGrupoCliente",
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
            url: "/OrdenCompraCliente/changeEstado",
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
            url: "/OrdenCompraCliente/changeEstadoCrediticio",
            type: 'POST',
            data: {
                estadoCrediticio: estado
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

        var idOrdenCompraCliente = $("#verIdOrdenCompraCliente").val();

        $.ajax({
            url: "/OrdenCompraCliente/GetHistorial",
            data: {
                id: idOrdenCompraCliente
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) { $('body').loadingModal('hide'); alert("Ocurrió un problema al obtener el historial del ordenCompraCliente."); },
            success: function (resultado) {

                $("#historial_titulo_numero_ordenCompraCliente").html($("#verNumero").html());

                $("#tableHistorialOrdenCompraCliente > tbody").empty();

                FooTable.init('#tableHistorialOrdenCompraCliente');

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
                $("#tableHistorialOrdenCompraCliente").append(d);




                $("#modalVerHistorialOrdenCompraCliente").modal('show');
                $('body').loadingModal('hide');
                //  window.location = '/Cotizacion/Index';
            }
        });
    };

    function showHistorialCrediticio() {
        $('body').loadingModal({
            text: 'Obteniendo Historial Crediticio...'
        });

        var idOrdenCompraCliente = $("#verIdOrdenCompraCliente").val();

        $.ajax({
            url: "/OrdenCompraCliente/GetHistorialCrediticio",
            data: {
                id: idOrdenCompraCliente
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) { $('body').loadingModal('hide'); alert("Ocurrió un problema al obtener el historial crediticio del ordenCompraCliente."); },
            success: function (resultado) {
                $("#historial_crediticio_titulo_numero_ordenCompraCliente").html($("#verNumero").html());
                $("#tableHistorialCrediticioOrdenCompraCliente > tbody").empty();

                FooTable.init('#tableHistorialCrediticioOrdenCompraCliente');

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
                $("#tableHistorialCrediticioOrdenCompraCliente").append(d);




                $("#modalVerHistorialCrediticioOrdenCompraCliente").modal('show');
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
            url: "/OrdenCompraCliente/ChangeUbigeoEntrega",
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
            url: "/OrdenCompraCliente/ChangeUbigeoEntrega",
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
            url: "/OrdenCompraCliente/ChangeUbigeoEntrega",
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

            $('#occ_direccionEntrega')
                .find('option')
                .remove()
                .end()
                ;

            $('#occ_direccionEntrega').append($('<option>', {
                value: GUID_EMPTY,
                text: "Seleccione Dirección Entrega",
                direccion: "",
                contacto: "",
                telefono: ""
            }));


            for (var i = 0; i < direccionEntregaListTmp.length; i++) {
                $('#occ_direccionEntrega').append($('<option>', {
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
            url: "/OrdenCompraCliente/ChangeIdCiudad",
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

    /****************** PROGRAMACION OCC****************************/

    $('#modalProgramacion').on('shown.bs.modal', function () {
        var fechaProgramacion = $("#fechaProgramaciontmp").val();
        $("#fechaProgramacion").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaProgramacion);    
    })

    $("#btnExportExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
    });

    $("#btnAceptarProgramarOrdenCompraCliente").click(function () {

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
            var respuesta = confirm("¡ATENCIÓN! Está programando la atención del ordenCompraCliente en una fecha fuera del rango solicitado por el cliente.");
            if (!respuesta) {
                $("#fechaProgramacion").focus();
                return false;
            }
        }      

        $.ajax({
            url: "/OrdenCompraCliente/Programar",
            type: 'POST',
           // dataType: 'JSON',
            data: {
                fechaProgramacion: fechaProgramacion,
                comentarioProgramacion: comentarioProgramacion
            },
            success: function (resultado) {
                alert('El ordenCompraCliente número ' + $("#verNumero").html() + ' se programó para ser atendido.');
                location.reload();
            }
        });
        $("btnCancelarProgramarOrdenCompraCliente").click();
    });



 


    /****************** FIN PROGRAMACION OCC****************************/
});