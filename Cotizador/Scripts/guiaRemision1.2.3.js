/// <reference path="moment.js" />

jQuery(function ($) {
    
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición/creación; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';
    var TITLE_MENSAJE_BUSQUEDA = "Ingresar datos solicitados";

    $(document).ready(function () {
        obtenerConstantes();
        verificarSiExisteNuevoTransportista();
        esPaginaImpresion();
      
        $("#btnBusqueda").click();

        if ($("#pagina").val() == 4) {
            if ($("#idMovimientoAlmacen").val() != "") {
                showMovimientoAlmacen($("#idMovimientoAlmacen").val());
            }
            cargarChosenCliente();
        }
        else if ($("#pagina").val() == 19) {
            $("#btnBusquedaGuiasFacturaConsolidada").click();
            cargarChosenClienteConsolidacion();
        }
        else {
            cargarChosenCliente();
        }

    });

    window.onafterprint = function () {
        if ($("#pagina").val() == 14) {
            window.close();
        }
    };

    function esPaginaImpresion() {
        if ($("#pagina").val() == 14) {
            window.print();
        }
    }

    $("#btnLimpiarBusqueda").click(function () {
        $.ajax({
            url: "/GuiaRemision/CleanBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });

    function verificarSiExisteNuevoTransportista() {
        $('#guiaRemision_transportista option').each(function () {
            if ($(this).val() == GUID_EMPTY) {
                $("#btnAgregarTransportista").attr("disabled", "disabled");
                $("#guiaRemision_transportista_descripcion").attr('disabled', 'disabled');
                $("#guiaRemision_transportista_ruc").attr("disabled", "disabled");
                $("#guiaRemision_transportista_direccion").attr("disabled", "disabled");
                $("#guiaRemision_transportista_brevete").attr("disabled", "disabled");
            }
            else {
                $("#btnAgregarTransportista").removeAttr("disabled");
                $("#guiaRemision_transportista_descripcion").removeAttr('disabled');
                $("#guiaRemision_transportista_ruc").removeAttr("disabled");
                $("#guiaRemision_transportista_direccion").removeAttr("disabled");
                $("#guiaRemision_transportista_brevete").removeAttr("disabled");
            }
        });
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
            }
        });
    }


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
            url: "/GuiaRemision/SearchClientes"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });
    }


    function cargarChosenClienteConsolidacion() {

        $("#idClienteConsolidacion").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" }).on('chosen:showing_dropdown', function (evt, params) {
            if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
                alert("Debe seleccionar la sede MP previamente.");
                $("#idClienteConsolidacion").trigger('chosen:close');
                $("#idCiudad").focus();
                return false;
            }
        });

        $("#idClienteConsolidacion").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/GuiaRemision/SearchClientes"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });
    }
    


    /**
    *################################## INICIO CONTROLES CIUDAD
    */

    function onChangeCiudad(parentController) {
        $.ajax({
            url: "/" + parentController + "/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
            idCiudad: this.value
            }, 
            error: function (detalle) {
                alert('Debe eliminar los productos agregados antes de cambiar de Sede.'); 
                location.reload(); 
            },
            success: function (ciudad) {
                alert(ciudad)
            }
        }); 
    }  



    /**
     * ################################ INICIO CONTROLES DE CLIENTE
     */



    $("#idCliente").change(function () {
      //  $("#contacto").val("");
        var idCliente = $(this).val();

        $.ajax({
            url: "/GuiaRemision/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente) {
            }
        });
      
    });

    $("#idClienteConsolidacion").change(function () {
        //  $("#contacto").val("");
        var idCliente = $(this).val();

        $.ajax({
            url: "/GuiaRemision/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente) {
                $("#tableGuiasRemision > tbody").empty();
                $("#ver_guiaRemision_pedido_cliente_razon_social").html(cliente.razonSocial);
                $("#ver_guiaRemision_pedido_cliente_ruc").html(cliente.ruc);
                $("#ver_venta_cliente_razon_social").html(cliente.razonSocial);
                $("#ver_venta_pedido_cliente_ruc").html(cliente.ruc);               
            }
        });

    });




    $('#modalAgregarCliente').on('shown.bs.modal', function () {

        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar la sede MP previamente.");
            $("#idCiudad").focus();
            $('#btnCancelCliente').click();
            return false;
        }
    });


    $("#idGrupoCliente").change(function () {
        //  $("#contacto").val("");
        var idGrupoCliente = $(this).val();

        $.ajax({
            url: "/GuiaRemision/GetGrupoCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idGrupoCliente: idGrupoCliente
            },
            success: function (cliente) {
            }
        });

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


    var fechaTraslado = $("#fechaTrasladotmp").val();
    $("#guiaRemision_fechaTraslado").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaTraslado);

    var fechaEmision = $("#fechaEmisiontmp").val();
    $("#guiaRemision_fechaEmision").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEmision);


    var fechaTrasladoDesde = $("#fechaTrasladoDesdetmp").val();
    $("#guiaRemision_fechaTrasladoDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaTrasladoDesde);

    var fechaTrasladoHasta = $("#fechaTrasladoHastatmp").val();
    $("#guiaRemision_fechaTrasladoHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaTrasladoHasta);


    var documentoVenta_fechaEmision = $("#documentoVenta_fechaEmisiontmp").val();
    $("#documentoVenta_fechaEmision").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", documentoVenta_fechaEmision);

    var documentoVenta_fechaVencimiento = $("#documentoVenta_fechaVencimientotmp").val();
    $("#documentoVenta_fechaVencimiento").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", documentoVenta_fechaVencimiento);



    /**
     * FIN DE CONTROLES DE FECHAS
     */






    /* ################################## INICIO CHANGE CONTROLES */

    function toggleControlesTransportista() {
        var idTransportista = $("#guiaRemision_transportista").val();
        if (idTransportista == "") {
            $("#guiaRemision_transportista_descripcion").attr('disabled', 'disabled');
            $("#guiaRemision_transportista_ruc").attr('disabled', 'disabled');
            $("#guiaRemision_transportista_direccion").attr('disabled', 'disabled');
            $("#guiaRemision_transportista_brevete").attr('disabled', 'disabled');
            
        }
        else {
            /*  $("#pedido_direccionEntrega_telefono").val($('#pedido_direccionEntrega').find(":selected").attr("telefono"));*/
            $("#guiaRemision_transportista_descripcion").removeAttr("disabled");
            $("#guiaRemision_transportista_ruc").removeAttr("disabled");
            $("#guiaRemision_transportista_direccion").removeAttr("disabled");
            $("#guiaRemision_transportista_brevete").removeAttr("disabled");
        }
    }

    $("#guiaRemision_transportista").change(function () {
        toggleControlesTransportista();
        var idTransportista = $("#guiaRemision_transportista").val();

        $.ajax({
            url: "/GuiaRemision/ChangeTransportista",
            type: 'POST',
            dataType: "JSON",
            data: {
                idTransportista: idTransportista
            },
            success: function (transportista) {

                $("#guiaRemision_transportista_descripcion").val(transportista.descripcion);
                $("#guiaRemision_transportista_ruc").val(transportista.ruc);
                $("#guiaRemision_transportista_direccion").val(transportista.direccion);
                $("#guiaRemision_transportista_brevete").val(transportista.brevete);
            }
        });
    });


    $("#buscarSedesGrupoCliente").change(function () {
        var valor = $("input[name=buscarSedesGrupoCliente]:checked").val();
        $.ajax({
            url: "/GuiaRemision/ChangeBuscarSedesGrupoCliente",
            type: 'POST',
            data: {
                buscarSedesGrupoCliente: valor
            },
            success: function () {
            }
        });
    });
    


    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/GuiaRemision/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#guiaRemision_placaVehiculo").change(function () {
        changeInputString("placaVehiculo", $("#guiaRemision_placaVehiculo").val())
    });


    $("#guiaRemision_observaciones").change(function () {
        changeInputString("observaciones", $("#guiaRemision_observaciones").val())
    });

    function changeInputStringTransportista(propiedad, valor) {
        $.ajax({
            url: "/GuiaRemision/ChangeInputStringTransportista",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#guiaRemision_transportista_ruc").change(function () {
        changeInputStringTransportista("ruc", $("#guiaRemision_transportista_ruc").val())
    });

    $("#guiaRemision_transportista_direccion").change(function () {
        changeInputStringTransportista("direccion", $("#guiaRemision_transportista_direccion").val())
    });

    $("#guiaRemision_transportista_brevete").change(function () {
        changeInputStringTransportista("brevete", $("#guiaRemision_transportista_brevete").val())
    });
    

   /* ################################## FIN CHANGE CONTROLES */

    

    ////////CREAR/EDITAR GUIA REMISION
    

    function crearGuiaRemision(continuarLuego) {
        if (!validarIngresoDatosObligatoriosGuiaRemision())
            return false;

        desactivarBotonesCreacionModificacion();

        $('body').loadingModal({
            text: 'Creando Guía Remisión...'
        });
        $.ajax({
            url: "/GuiaRemision/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                continuarLuego: continuarLuego
            },
            error: function (detalle) {
                $('body').loadingModal("hide");
                activarBotonesCreacionModificacion();
                alert(MENSAJE_ERROR);
            },
            success: function (resultado) {
                $('body').loadingModal('hide')
                if (resultado.guiaRemisionValidacion.tipoErrorValidacion == 0) {
                    $.alert({
                        //icon: 'fa fa-warning',
                        title: TITLE_EXITO,
                        content: "La guía de remisión número " + resultado.serieNumeroGuia + " fue creada correctamente.",
                        type: 'green',
                        buttons: {
                            OK: function () {
                                
                                window.location = '/GuiaRemision/Index?idMovimientoAlmacen=' + resultado.idGuiaRemision; }
                        }
                    });
                }
                else {
                    mostrarMensajeErrorProceso(resultado.guiaRemisionValidacion.tipoErrorValidacionString + " " + resultado.guiaRemisionValidacion.descripcionError);
                    activarBotonesCreacionModificacion();
                }


                
/*
                $('body').loadingModal("hide");
                activarBotonesCreacionModificacion();
                $("#numero").val(resultado.codigo);

                if (resultado.error == "DuplicateNumberDocumentException") {
                    alert("El número de guía de remisión ya fue utilizado, por favor actualice el número de guía, haciendo clic en el botón actualizar.");
                }
                else {
                    alert("La guía de remisión número " + resultado.serieNumeroGuia + " fue creada correctamente.");
                    showMovimientoAlmacen(resultado.idGuiaRemision);
                    window.location = '/GuiaRemision/Index?idMovimientoAlmacen=' + resultado.idGuiaRemision;
                }
                */
                /*
                else if (resultado.estado == ESTADO_APROBADA) {
                    alert("La guía de remisión número " + resultado.codigo + " fue creada correctamente.");
                    window.location = '/GuiaRemision/Index';
                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    alert("La guía de remisión número " + resultado.codigo + " fue creada correctamente, sin embargo requiere APROBACIÓN.");
                    window.location = '/GuiaRemision/Index';
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    alert("La guía de remisión " + resultado.codigo + " fue guardada correctamente para seguir editandolo posteriormente.");
                    window.location = '/GuiaRemision/Index';
                }
                else {
                    alert(MENSAJE_ERROR);
                    window.location = '/GuiaRemision/Index';
                }*/

            }
        });
    }






    function desactivarBotonesCreacionModificacion() {
        $("#btnFinalizarCreacionGuiaRemision").attr('disabled', 'disabled');
        $("#btnFinalizarEdicionGuiaRemision").attr('disabled', 'disabled');
        $("#btnContinuarCreandoLuego").attr('disabled', 'disabled');
        $("#btnCancelarGuiaRemision").attr('disabled', 'disabled');
    }

    function activarBotonesCreacionModificacion() {
        $("#btnFinalizarCreacionGuiaRemision").removeAttr('disabled');
        $("#btnFinalizarEdicionGuiaRemision").removeAttr('disabled');
        $("#btnContinuarCreandoLuego").removeAttr('disabled');
        $("#btnCancelarGuiaRemision").removeAttr('disabled');
    }


    $("#btnFinalizarCreacionGuiaRemision").click(function () {
        crearGuiaRemision(0);
    });

    $("#btnContinuarCreandoLuego").click(function () {
        crearGuiaRemision(1);
    });

    $("#btnFinalizarEdicionGuiaRemision").click(function () {
        editarGuiaRemision(0);
    });

    $("#btnContinuarEditandoLuego").click(function () {
        editarGuiaRemision(1);
    });



    function validarIngresoDatosObligatoriosGuiaRemision() {
             
        if ($("#guiaRemision_transportista").val() == null || $("#guiaRemision_transportista").val().trim() == "") {
            alert('Debe seleccionar el transportista.');
            $('#guiaRemision_transportista').focus();
            return false;
        }

        if ($("#guiaRemision_transportista_descripcion").val() == null || $("#guiaRemision_transportista_descripcion").val().trim() == "") {
            alert('Debe ingresar el nombre del transportista.');
            $('#guiaRemision_transportista_descripcion').focus();
            return false;
        }

        if ($("#guiaRemision_transportista_ruc").val() == null || $("#guiaRemision_transportista_ruc").val().trim() == "") {
            alert('Debe ingresar el ruc del transportista.');
            $('#guiaRemision_transportista_ruc').focus();
            return false;
        }

        if ($("#guiaRemision_transportista_brevete").val() == null || $("#guiaRemision_transportista_brevete").val().trim() == "") {
            alert('Debe ingresar el brevete del transportista.');
            $('#guiaRemision_transportista_brevete').focus();
            return false;
        }

        if ($("#guiaRemision_transportista_direccion").val() == null || $("#guiaRemision_transportista_direccion").val().trim() == "") {
            alert('Debe ingresar la dirección del transportista.');
            $('#guiaRemision_transportista_direccion').focus();
            return false;
        }

        if ($("#guiaRemision_placaVehiculo").val() == null || $("#guiaRemision_placaVehiculo").val().trim() == "") {
            alert('Debe ingresar la placa del vehículo.');
            $('#guiaRemision_placaVehiculo').focus();
            return false;
        }
        /*
        if ($("#guiaRemision_certificadoInscripcion").val() == null || $("#guiaRemision_certificadoInscripcion").val().trim() == "") {
            alert('Debe ingresar el certificado de inscripción.');
            $('#guiaRemision_certificadoInscripcion').focus();
            return false;
        }*/

        if ($("#guiaRemision_pedido_direccionEntrega").length) {
            if ($("#guiaRemision_pedido_direccionEntrega").val().trim() == "") {
                $('#guiaRemision_pedido_direccionEntrega').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe seleccionar la dirección de entrega.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }
        }
      


        var contador = 0;
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {

            if (Number(value.innerHTML) > 0)  
                contador++;
        });

        if (contador > 10) {
            alert('La guía de remisión debe contener 10 productos como máximo, por favor marque el check de "Atención Parcial", indique que NO es la última atención y modifique la "Cantidad por Atender" a 0 a algunos productos (los productos que tengan 0 no se considerarán en la guía).');
            return false;
        }


        return true;
    }
      



    $(document).on('click', "input.chkNoEntregado", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idMovimientoAlmacen = arrrayClass[0];
        var numeroDocumento = arrrayClass[1];

        var noEntregado = 0;
        if (event.target.checked) {
            noEntregado = 1;
        }


        $.ajax({
            url: "/GuiaRemision/UpdateMarcaNoEntregado",
            data: {
                idMovimientoAlmacen: idMovimientoAlmacen,
                noEntregado: noEntregado
            },
            type: 'POST',
          // dataType: 'JSON',
            error: function (detalle) {
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
            },
            success: function (resultado) {

            }
        });
    });
    


    /*VER GUIAREMISION*/
    $(document).on('click', "button.btnVerGuiaRemision", function () {

        $('body').loadingModal({
            text: 'Abriendo Guía de Remisión...'
        });
        $('body').loadingModal('show');

        activarBotonesVer();
        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idMovimientoAlmacen = arrrayClass[0];
        var numeroDocumento = arrrayClass[1];     

        showMovimientoAlmacen(idMovimientoAlmacen);
    });

    function showMovimientoAlmacen(idMovimientoAlmacen) {
        $.ajax({
            url: "/GuiaRemision/Show",
            data: {
                idMovimientoAlmacen: idMovimientoAlmacen
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                alert(MENSAJE_ERROR);
            },
            success: function (resultado) {
                $('body').loadingModal('hide');
                $("#comentarioAnulado").val("");
                //var cotizacion = $.parseJSON(respuesta);
                var guiaRemision = resultado.guiaRemision;
                var usuario = resultado.usuario;



                $("#idPedido").val(guiaRemision.pedido_idPedido);
                $("#idMovimientoAlmacen").val(guiaRemision.idMovimientoAlmacen);
                $("#ver_guiaRemision_ciudadOrigen_nombre").html(guiaRemision.ciudadOrigen_nombre);
                $("#ver_guiaRemision_ciudadOrigen_direccionPuntoPartida").html(guiaRemision.ciudadOrigen_direccionPuntoPartida);

                $("#ver_guiaRemision_fechaTraslado").html(invertirFormatoFecha(guiaRemision.fechaTraslado.substr(0, 10)));
                $("#ver_guiaRemision_fechaEmision").html(invertirFormatoFecha(guiaRemision.fechaEmision.substr(0, 10)));
                $("#ver_guiaRemision_serieNumeroDocumento").html(guiaRemision.serieNumeroGuia);

                $("#facturarver_guiaRemision_fechaTraslado").html(invertirFormatoFecha(guiaRemision.fechaTraslado.substr(0, 10)));
                $("#facturarver_guiaRemision_fechaEmision").html(invertirFormatoFecha(guiaRemision.fechaEmision.substr(0, 10)));
                $("#facturarver_guiaRemision_serieNumeroDocumento").html(guiaRemision.serieNumeroGuia);


                //   $("#ver_guiaRemision_numeroDocumento").html(guiaRemision.numeroDocumentoString);
                $("#ver_guiaRemision_pedido_numeroPedido").html(guiaRemision.pedido_numeroPedidoString);
                $("#ver_guiaRemision_pedido_cliente").html(guiaRemision.pedido_cliente_razonSocial);
                $("#ver_guiaRemision_pedido_numeroReferenciaCliente").html(guiaRemision.pedido_numeroReferenciaCliente);
                $("#ver_guiaRemision_motivoTraslado").html(guiaRemision.motivoTrasladoString);
                $("#ver_guiaRemision_atencionParcial").html(guiaRemision.atencionParcial);
                $("#ver_guiaRemision_pedido_ubigeoEntrega").html(guiaRemision.pedido_ubigeoEntrega.ToString);
                $("#ver_guiaRemision_pedido_direccionEntrega").html(guiaRemision.pedido_direccionEntrega_descripcion);
                $("#ver_guiaRemision_transportista_descripcion").html(guiaRemision.transportista_descripcion);
                $("#ver_guiaRemision_transportista_ruc").html(guiaRemision.transportista_ruc);
                $("#ver_guiaRemision_transportista_brevete").html(guiaRemision.transportista_brevete);
                $("#ver_guiaRemision_transportista_direccion").html(guiaRemision.transportista_direccion);
                $("#ver_guiaRemision_placaVehiculo").html(guiaRemision.placaVehiculo);
                //$("#ver_guiaRemision_certificadoInscripcion").html(guiaRemision.certificadoInscripcion);
                $("#ver_guiaRemision_observaciones").html(guiaRemision.observaciones);

                $("#ver_guiaRemision_estadoDescripcion").html(guiaRemision.estadoDescripcion);



                /*Si la guía de remisión se encuentra ANULADA no se puede extornar, ni imprimir, ni facturar*/
                if (guiaRemision.estaAnulado == 1) {
                    $("#ver_guiaRemision_estadoDescripcion").attr("style", "color:red")
                    $("#btnAnularGuiaRemision").hide();
                    $("#btnExtornar").hide();
                    $("#btnImprimirGuiaRemision").hide();
                    $("#btnFacturarGuiaRemision").hide();
                }
                else {
                    $("#ver_guiaRemision_estadoDescripcion").attr("style", "color:black")
                    $("#btnImprimirGuiaRemision").show();

                    $("#guiaRemision_tipoExtorno").val(guiaRemision.tipoExtorno);
                    /*Si se encuentra NO EXTORNADA*/
                    if (guiaRemision.tipoExtorno == MOV_TIPO_EXTORNO_SIN_EXTORNO) {
                        $("#btnAnularGuiaRemision").show();
                        $("#btnExtornar").show();

                        $("#divTipoExtorno").hide();
                        $("#btnVerNotasIngresoExtornantes").hide();


                    }
                    /*Si se encuentra EXTORNADA no se puede Anular, Ni facturar*/
                    else {
                        $("#btnAnularGuiaRemision").hide();
                        $("#btnFacturarGuiaRemision").hide();

                        $("#divTipoExtorno").show();
                        $("#btnVerNotasIngresoExtornantes").show();

                        $("#ver_guiaRemision_tipoExtorno").html(guiaRemision.tipoExtornoToString);
                        if (guiaRemision.tipoExtorno == MOV_TIPO_EXTORNO_EXTORNO_PARCIAL) {
                            /*Si la guía de remisión ha sido extornada parcialmente, se puede extornar para culminar con el extorno, no se puede anular y tampoco facturar*/
                            $("#btnExtornar").show();
                        }
                        else {
                            $("#btnExtornar").hide();
                        }
                    }

                    //es EXTORNANTE
                    if (guiaRemision.notaIngresoAExtornar == null) {
                        $("#fieldsetDocumentoExtornado").hide();
                        $("#fieldsetDetalleExtorno").hide();
                    }
                    else {
                        $("#btnExtornar").hide();
                        $("#fieldsetDocumentoExtornado").show();
                        $("#fieldsetDetalleExtorno").show();
                        $("#ver_guiaRemision_notaIngresoAExtornar_serieNumeroNotaIngreso").html(guiaRemision.notaIngresoAExtornar.serieNumeroNotaIngreso);
                        $("#ver_guiaRemision_motivoExtornoNotaIngresoToString").html(guiaRemision.motivoExtornoNotaIngresoToString);
                        $("#ver_guiaRemision_sustentoExtorno").html(guiaRemision.sustentoExtorno);
                    }

                    /*FACTURADO*/
                    if (guiaRemision.estaFacturado == 1) {
                        $("#ver_guiaRemision_estadoDescripcion").attr("style", "color:green")
                        $("#btnAnularGuiaRemision").hide();
                        $("#btnFacturarGuiaRemision").hide();
                    }

                   /* if (    guiaRemision.motivoTraslado == MOTIVO_TRASLADO_SALIDA_DEVOLUCION_COMPRA.charCodeAt(0)
                        || guiaRemision.motivoTraslado == MOTIVO_TRASLADO_SALIDA_DEVOLUCION_PRESTAMO_RECIBIDO.charCodeAt(0)
                        || guiaRemision.motivoTraslado == MOTIVO_TRASLADO_SALIDA_DEVOLUCION_COMODATO_RECIBIDO.charCodeAt(0)
                        || guiaRemision.motivoTraslado == MOTIVO_TRASLADO_SALIDA_DEVOLUCION_TRANSFERENCIA_GRATUITA_RECIBIDA.charCodeAt(0)
                    )
                    {
                        $("#btnExtornar").hide();
                    }*/
                  
                }



                var transaccionList = guiaRemision.transaccionList;
                var documentosVenta = "";
                for (var i = 0; i < transaccionList.length; i++) {
                    var documentoVenta = transaccionList[i].documentoVenta;
                    documentosVenta = documentoVenta.serieNumero + ";"
                }
                documentosVenta = documentosVenta.substr(0, documentosVenta.length - 1);

                $("#documentosVenta").val(documentosVenta);

                /*Si la guía de remisión no corresponde a una venta o a una transferencia gratuita no se puede facturar*/
                $("#btnIngresar").hide();
                if (guiaRemision.motivoTraslado == MOTIVO_TRASLADO_SALIDA_TRASLADO_INTERNO.charCodeAt(0)
                    && guiaRemision.ingresado == false
                ) {
                    $("#btnIngresar").show()
                    $("#btnFacturarGuiaRemision").hide();
                    $("#btnExtornar").hide();
                }
                else if (guiaRemision.motivoTraslado != MOTIVO_TRASLADO_SALIDA_VENTA.charCodeAt(0)
                    && guiaRemision.motivoTraslado != MOTIVO_TRASLADO_SALIDA_TRANSFERENCIA_GRATUITA.charCodeAt(0)
                ) {
                    $("#btnFacturarGuiaRemision").hide();
                   
                    $("#btnIngresar").hide();

                    if (guiaRemision.motivoTraslado != MOTIVO_TRASLADO_SALIDA_COMODATO.charCodeAt(0)
                        && guiaRemision.motivoTraslado != MOTIVO_TRASLADO_SALIDA_PRESTAMO.charCodeAt(0)) {
                        $("#btnExtornar").hide();
                    }
                }
                else if (guiaRemision.ingresado) {
                    $("#btnAnularGuiaRemision").hide();
                    $("#btnIngresar").hide();
                }
                

                if (guiaRemision.atencionParcial) {
                    $("#ver_guiaRemision_atencionParcial").html("Atención Parcial");
                }
                else {
                    $("#ver_guiaRemision_atencionParcial").html("Atención Final");
                }

               
                $("#tableDetalleGuia > tbody").empty();

                FooTable.init('#tableDetalleGuia');


                var d = '';
                var lista = guiaRemision.documentoDetalle;
                for (var i = 0; i < lista.length; i++) {

                    // var observacion = lista[i].observacion == null || lista[i].observacion == 'undefined'? '' : lista[i].observacion;

                    d += '<tr>' +
                        '<td>' + lista[i].producto.idProducto + '</td>' +
                        '<td>' + lista[i].producto.sku + '</td>' +
                        '<td class="' + lista[i].producto.idProducto + ' detcantidad" style="text-align:right">' + lista[i].cantidad + '</td>' +
                        '<td class="' + lista[i].producto.idProducto + ' detcantidadSaldo">' + lista[i].cantidad + '</td>' +
                        '<td>' + lista[i].unidad + '</td>' +
                        '<td>' + lista[i].producto.descripcion + '</td>' +
                        '</tr>';

                }

                $("#tableDetalleGuia").append(d);




                $("#modalVerGuiaRemision").modal('show');

                //  window.location = '/Pedido/Index';
            }
        });
    }
    
    

    $("#btnCancelarGuiaRemision").click(function () {
        if (confirm(MENSAJE_CANCELAR_EDICION)) {
            window.location = '/GuiaRemision/CancelarCreacionGuiaRemision';
        }
    })



    function desactivarBotonesVer()
    {
        $("#btnContinuarGenerandoNotaIngreso").attr("disabled", "disabled");
        $("#btnCancelarNotaIngreso").attr("disabled", "disabled");
        $("#btnCancelarVerGuiaRemision").attr("disabled", "disabled");
        $("btnImprimirGuiaRemision").attr("disabled", "disabled");
        $("btnFacturarGuiaRemision").attr("disabled", "disabled");
        $("btnAnularGuiaRemision").attr("disabled", "disabled");
        $("btnExtornar").attr("disabled", "disabled");
        $("btnVerNotasIngresoExtornantes").attr("disabled", "disabled");
        $("btnIngresar").attr("disabled", "disabled");
    }

    function activarBotonesVer() {
        $("#btnContinuarGenerandoNotaIngreso").removeAttr("disabled");
        $("#btnCancelarNotaIngreso").removeAttr("disabled");
        $("#btnCancelarVerGuiaRemision").removeAttr("disabled");
        $("btnImprimirGuiaRemision").removeAttr("disabled");
        $("btnFacturarGuiaRemision").removeAttr("disabled");
        $("btnAnularGuiaRemision").removeAttr("disabled");
        $("btnExtornar").removeAttr("disabled");
        $("btnVerNotasIngresoExtornantes").removeAttr("disabled");
        $("btnIngresar").removeAttr("disabled");
    }

   


    $("#btnImprimirGuiaRemision").click(function () {
        window.open("/GuiaRemision/Print");
    });

    $("#btnVerNotasIngresoExtornantes").click(function () {
        var idMovimientoAlmacen = $("#idMovimientoAlmacen").val();
        $.ajax({
            url: "/GuiaRemision/GetMovimientosAlmacenExtornantes",
            data: {
                idMovimientoAlmacen: idMovimientoAlmacen
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                mostrarMensajeErrorProceso();
            },
            success: function (notaIngresoList) {
                $("#formVerNotasIngreso").empty();

                for (var j = 0; j < notaIngresoList.length; j++) {
                    $("#tableDetalleNotaIngreso > tbody").empty();
                    var plantilla = $("#plantillaVerNotasIngreso").html();
                    var dGuia = '';
                    var documentoDetalleList = notaIngresoList[j].documentoDetalle;
                    for (var k = 0; k < documentoDetalleList.length; k++) {

                        dGuia += '<tr>' +
                            '<td>' + documentoDetalleList[k].producto.sku + '</td>' +
                            '<td>' + documentoDetalleList[k].cantidad + '</td>' +
                            '<td>' + documentoDetalleList[k].unidad + '</td>' +
                            '<td>' + documentoDetalleList[k].producto.descripcion + '</td>' +
                            '</tr>';
                    }

                    $("#tableDetalleNotaIngreso").append(dGuia);

                    plantilla = $("#plantillaVerNotasIngreso").html();

                    plantilla = plantilla.replace("#serieNumero", notaIngresoList[j].serieDocumento + '-' + notaIngresoList[j].numeroDocumento);
                    plantilla = plantilla.replace("#fechaEmisionNotaIngreso", invertirFormatoFecha(notaIngresoList[j].fechaEmision.substr(0, 10)));

                    plantilla = plantilla.replace("#serieNumeroNotaCrédito", notaIngresoList[j].venta.documentoVenta.serieNumero);
                    if (notaIngresoList[j].venta.documentoVenta.fechaEmision != null) {
                        plantilla = plantilla.replace("#fechaEmisionNotaCrédito", invertirFormatoFecha(notaIngresoList[j].venta.documentoVenta.fechaEmision.substr(0, 10)));
                    }
                    else
                        plantilla = plantilla.replace("#fechaEmisionFactura", "");


                    plantilla = plantilla.replace("tableDetalleGuia", "tableDetalleGuia" + j);

                    $("#formVerNotasIngreso").append(plantilla);
                }

                $("#modalVerNotasIngreso").modal('show');

            }
        });
        
    });






    $("#btnFacturarGuiaRemision").click(function () {

        var idMovimientoAlmacen = $("#idMovimientoAlmacen").val();
        $.ajax({
            url: "/Venta/Show",
            data: {
                idMovimientoAlmacen: idMovimientoAlmacen
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                mostrarMensajeErrorProceso();
            },
            success: function (resultado) {
                mostrarModalFacturar(resultado);
            }
        });

    })


    function mostrarModalFacturar(resultado) {
        //var cotizacion = $.parseJSON(respuesta);

        var venta = resultado.venta;
        var pedido = resultado.venta.pedido;
        var guiaRemision = resultado.venta.guiaRemision;
        var serieDocumentoElectronicoList = resultado.serieDocumentoElectronicoList;

        //  var usuario = resultado.usuario;


        $("#fechaEntregaDesdeProgramacion").val(invertirFormatoFecha(pedido.fechaEntregaDesde.substr(0, 10)));
        $("#fechaEntregaHastaProgramacion").val(invertirFormatoFecha(pedido.fechaEntregaHasta.substr(0, 10)));
        $("#fechaProgramaciontmp").val(invertirFormatoFecha(pedido.fechaEntregaDesde.substr(0, 10)));
        //Important

        $("#idPedido").val(pedido.idPedido);

        $("#verNumero").html(pedido.numeroPedidoString);
        $("#verNumeroGrupo").html(pedido.numeroGrupoPedidoString);
        $("#verCotizacionCodigo").html(pedido.cotizacion.numeroCotizacionString);
        $("#verTipoPedido").html(pedido.tiposPedidoString);

        $("#verUsuarioNombre").html(pedido.usuario.nombre);
        $("#verFechaHoraSolicitud").html(pedido.fechaHoraSolicitud);

        if (pedido.tipoPedido == "84") {
            $("#divReferenciaCliente").hide();
            $("#divCiudadSolicitante").show();
            $("#verCiudadSolicitante").html(pedido.cliente.ciudad.nombre);
        }
        else {
            $("#divReferenciaCliente").show();
            $("#divCiudadSolicitante").hide();
        }


        $("#verFechaHorarioEntrega").html(pedido.fechaHorarioEntrega);

        $("#verCiudad").html(pedido.ciudad.nombre);
        $("#idClienteFacturacion").val(pedido.cliente.idCliente);
        $("#verCliente").html(pedido.cliente.razonSocial);
        $("#verClienteCodigo").html(pedido.cliente.codigo);
        $("#verNumeroReferenciaCliente").html(pedido.numeroReferenciaCliente);
        $("#verNumeroReferenciaAdicional").html(pedido.numeroReferenciaAdicional);
        $("#verObservacionesPedido").html(pedido.observaciones);

        if (pedido.cliente.tipoDocumento == CONS_TIPO_DOC_CLIENTE_RUC) {
            $("#modalFacturarTitle").html("<b>Crear Factura</b>");
            $("#descripcionDatosDocumento").html("<b>Datos de la Factura</b>");
            $("#observacionesDocumento").html("Observaciones Factura:");
            $("#btnAceptarFacturarPedido").html("Generar Factura");
            $("#pedido_cliente_tipoDocumento").val(CONS_TIPO_DOC_CLIENTE_RUC);
        }
        else if (pedido.cliente.tipoDocumento == CONS_TIPO_DOC_CLIENTE_DNI) {
            $("#modalFacturarTitle").html("<b>Crear Boleta</b>");
            $("#descripcionDatosDocumento").html("<b>Datos de la Boleta</b>");
            $("#observacionesDocumento").html("Observaciones Boleta:");
            $("#btnAceptarFacturarPedido").html("Generar Boleta");
            $("#pedido_cliente_tipoDocumento").val(CONS_TIPO_DOC_CLIENTE_DNI);
        }
        else if (pedido.cliente.tipoDocumento == CONS_TIPO_DOC_CLIENTE_CARNET_EXTRANJERIA) {
            $("#modalFacturarTitle").html("<b>Crear Boleta</b>");
            $("#descripcionDatosDocumento").html("<b>Datos de la Boleta</b>");
            $("#observacionesDocumento").html("Observaciones Boleta:");
            $("#btnAceptarFacturarPedido").html("Generar Boleta");
            $("#pedido_cliente_tipoDocumento").val(CONS_TIPO_DOC_CLIENTE_CARNET_EXTRANJERIA);
        }


        $("#nombreArchivos > li").remove().end();


        for (var i = 0; i < pedido.pedidoAdjuntoList.length; i++) {
            var liHTML = '<a href="javascript:mostrar();" class="descargarDesdeVenta">' + pedido.pedidoAdjuntoList[i].nombre + '</a>';
            $('<li />').html(liHTML).appendTo($('#nombreArchivos'));
        }




        $("#verDireccionEntrega").html(pedido.direccionEntrega.descripcion);
        $("#verTelefonoContactoEntrega").html(pedido.direccionEntrega.telefono);
        $("#verContactoEntrega").html(pedido.direccionEntrega.contacto);

        $("#verUbigeoEntrega").html(pedido.ubigeoEntrega.ToString);

        $("#verContactoPedido").html(pedido.contactoPedido);
        $("#verTelefonoCorreoContactoPedido").html(pedido.telefonoCorreoContactoPedido);

        $("#verFechaHoraSolicitud").html(pedido.fechaHoraSolicitud);

    /*    $("#verEstado").html(pedido.seguimientoPedido.estadoString);
        $("#verModificadoPor").html(pedido.seguimientoPedido.usuario.nombre);
        $("#verObservacionEstado").html(pedido.seguimientoPedido.observacion);

        $("#verEstadoCrediticio").html(pedido.seguimientoCrediticioPedido.estadoString);
        $("#verModificadoCrediticioPor").html(pedido.seguimientoCrediticioPedido.usuario.nombre);
        $("#verObservacionEstadoCreiditicio").html(pedido.seguimientoCrediticioPedido.observacion);
        *//*

        $("#facturarver_guiaRemision_fechaTraslado").html(invertirFormatoFecha(guiaRemision.fechaTraslado.substr(0, 10)));
        $("#facturarver_guiaRemision_fechaEmision").html(invertirFormatoFecha(guiaRemision.fechaEmision.substr(0, 10)));
        $("#facturarver_guiaRemision_serieNumeroDocumento").html(guiaRemision.serieNumeroGuia);*/

        $("#verObservaciones").html(pedido.observaciones);
        $("#verMontoSubTotal").html(Number(pedido.montoSubTotal).toFixed(cantidadDecimales));
        $("#verMontoIGV").html(Number(pedido.montoIGV).toFixed(cantidadDecimales));
        $("#verMontoTotal").html(Number(pedido.montoTotal).toFixed(cantidadDecimales));
        $("#documentoVenta_observaciones").val(pedido.observacionesFactura);

        $("#verMontoSubTotalVenta").html(Number(venta.subTotal).toFixed(cantidadDecimales));
        $("#verMontoIGVVenta").html(Number(venta.igv).toFixed(cantidadDecimales));
        $("#verMontoTotalVenta").html(Number(venta.total).toFixed(cantidadDecimales));





        $("#tableDetallePedido > tbody").empty();

        FooTable.init('#tableDetallePedido');

        //    $("#formVerGuiasRemision").html("");

        var d = '';
        var lista = pedido.pedidoDetalleList;
        for (var i = 0; i < lista.length; i++) {

            var observacion = lista[i].observacion == null || lista[i].observacion == 'undefined' ? '' : lista[i].observacion;

            d += '<tr>' +
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
                //             '<td>' + lista[i].precioUnitario.toFixed(cantidadCuatroDecimales) + '</td>' +
                '<td>' + lista[i].precioUnitarioVenta.toFixed(cantidadCuatroDecimales) + '</td>' +
                '<td>' + lista[i].cantidad + '</td>' +
                //     '<td>' + lista[i].cantidadPendienteAtencion + '</td>' +
                '<td>' + lista[i].subTotal.toFixed(cantidadDecimales) + '</td>' +
                '<td>' + observacion + '</td>' +
                '<td class="' + lista[i].producto.idProducto + ' detbtnMostrarPrecios"> <button  type="button" class="' + lista[i].producto.idProducto + ' btnMostrarPrecios btn btn-primary bouton-image botonPrecios"></button></td>' +
                '</tr>';
        }




        $("#verRazonSocialSunat").html(pedido.cliente.razonSocialSunat);
        $("#verRUC").html(pedido.cliente.ruc);
        $("#verDireccionDomicilioLegalSunat").html(pedido.cliente.direccionDomicilioLegalSunat);
        $("#verCodigo").html(pedido.cliente.codigo);

        $("#documentoVenta_observaciones").val(pedido.observacionesFactura);
        $("#verCorreoEnvioFactura").html(pedido.cliente.correoEnvioFactura);


        $("#documentoVenta_fechaEmision").val(invertirFormatoFecha(venta.guiaRemision.fechaEmision.substr(0, 10)));
        $("#documentoVenta_fechaVencimiento").val(invertirFormatoFecha(venta.guiaRemision.fechaEmision.substr(0, 10)));
        $("#documentoVenta_horaEmision").val(getHoraActual());

        $("#tipoPago").val(pedido.cliente.tipoPagoFactura);
        $("#tipoPago").attr("disabled", "disabled");
        calcularFechaVencimiento();
        $("#formaPago").val(pedido.cliente.formaPagoFactura);

        $('#documentoVenta_serie')
            .find('option')
            .remove()
            .end()
            ;

        for (var i = 0; i < serieDocumentoElectronicoList.length; i++) {
            $('#documentoVenta_serie').append($('<option>', {
                value: serieDocumentoElectronicoList[i].serie,
                text: serieDocumentoElectronicoList[i].serie
            }));
        }

        $("#tableDetallePedido").append(d);

        $("#modalFacturar").modal('show');
    }






    $(document).on('click', "button.btnMostrarPrecios", function () {

        var idProducto = event.target.getAttribute("class").split(" ")[0];
        var idCliente = $("#idClienteFacturacion").val();

        $.ajax({
            url: "/Precio/GetPreciosRegistradosVentaVer",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: idProducto,
                idCliente: idCliente
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




    




    


    $("#btnEditarCliente").click(function () {
        $("#btnCancelarFacturarPedido").click();
        window.open(
            "/Cliente/Editar?idCliente="+$("#idClienteFacturacion").val(),
            "Edición de Cliente",
            "resizable,scrollbars,status"
        );
    });

    $("#btnEditarVenta").click(function () {

        $("#btnCancelarFacturarPedido").click();
       // desactivarBotonesVer();

      
        var yourWindow;
        $.ajax({
            url: "/Venta/iniciarEdicionVenta",
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al iniciar la edición de la venta."); },
            success: function (fileName) {

                
                yourWindow = window.open(
                    "/Venta/Vender",
                    "Edición de Venta",
                    "resizable,scrollbars,status"
                );
           
          

             
               
                  
               

            }
        });

    });

    function mostrarMensajeErrorProceso(mensaje) {
        $.alert({
            //icon: 'fa fa-warning',
            title: 'Error',
            content: mensaje,
            type: 'red',
            buttons: {
                OK: function () { }
            }
        });
    }







    $("#btnAceptarAnulacion").click(function () {

        var comentarioAnulado = $("#comentarioAnulado").val();
        $("#btnAceptarAnulacion").attr("disabled","disabled");
        $.ajax({
            url: "/GuiaRemision/Anular",
            type: 'POST',
            dataType: 'JSON',
            data: {
                comentarioAnulado: comentarioAnulado
            },
            error: function () {
                $("#btnAceptarAnulacion").removeAttr("disabled");
                alert(MENSAJE_ERROR);
            },
            success: function (resultado) {
                $("#btnAceptarAnulacion").removeAttr("disabled");
                alert("La guía de remisión número " + resultado.serieNumeroGuia + " fue anulada correctamente.");
                window.location = '/GuiaRemision/Index';
            }
        });

       // window.open("GuiaRemision/Print");
    });
        

    function limpiarComentario()
    {
        $("#comentarioEstado").val("");
        $("#comentarioEstado").focus();
    }

    



    $('#modalAprobacion').on('shown.bs.modal', function (e) {
        limpiarComentario();
    });



    $("#btnAceptarCambioEstado").click(function () {

        var estado = $("#estadoId").val();
        var comentarioEstado = $("#comentarioEstado").val();

        if ($("#labelNuevoEstado").html() == ESTADO_DENEGADA_STR || $("#labelNuevoEstado").html() == ESTADO_RECHAZADA_STR) {
            if (comentarioEstado.trim() == "") {
                alert("Cuando Deniega o Rechaza una cotización debe ingresar un Comentario.");
                return false;
            }
        }
        var codigo = $("#verNumero").html();

        $.ajax({
            url: "/Pedido/updateEstadoCotizacion",
            data: {
                codigo: codigo,
                estado: estado,
                observacion: comentarioEstado
            },
            type: 'POST',
            error: function () {
                alert("Ocurrió un problema al intentar cambiar el estado de la cotización.")
                $("#btnCancelarCambioEstado").click();
            },
            success: function () {
                alert("El estado de la cotización número: " + codigo + " se cambió correctamente.");
                //$("#btnCancelarCambioEstado").click();
                $("#btnBusqueda").click();
            }
        });
    });




    
    var ft = null;


    
    


    
    

    
    

    





    /*####################################################
    EVENTOS DE LA GRILLA
    #####################################################*/


    /**
     * Se definen los eventos de la grilla
     */
    function cargarTablaDetalle() {
        var $modal = $('#tableDetalleGuia'),
            $editor = $('#tableDetalleGuia'),
            $editorTitle = $('#tableDetalleGuia');

     
        ft = FooTable.init('#tableDetalleGuia', {
            editing: {
                enabled: true,
                addRow: function () {
                    if (confirm(MENSAJE_CANCELAR_EDICION)) {
                        location.reload();
                    }
                },
                editRow: function (row) {
                    var values = row.val();
                    var idProducto = values.idProducto;
                    alert(idProducto);
                },
                deleteRow: function (row) {
                    var values = row.val();
                    var idProducto = values.idProducto;
                    row.delete();
                }
            }
        });
        
    }
    cargarTablaDetalle();


 

    


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

        /*Si no es una atención parcial no se debe poder editar*/
        if (!$('#guiaRemision_atencionParcial').prop('checked'))
        {
            return false;
        }


        $("#btnCancelarGuiaRemision").attr("disabled", "disabled");
        $("#btnFinalizarCreacionGuiaRemision").attr("disabled", "disabled");   


        //Cambiar estilos a los botones
        $("button.footable-add").attr("class", "btn btn-default footable-add");
        $("button.footable-hide").attr("class", "btn btn-primary footable-hide");    


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
    
    });






    /*Evento que se dispara cuando se hace clic en FINALIZAR en la edición de la grilla*/
    $(document).on('click', "button.footable-hide", function () {

        var json = '[ ';
        var $j_object = $("td.detcantidad");

        $.each($j_object, function (key, value) {
            var arrId = value.getAttribute("class").split(" ");

            /*Se elimina control input en columna cantidad*/
            var cantidad = $("." + arrId[0] + ".detincantidad").val();
            value.innerText = cantidad;

            /*Se elimina control input en columna porcentaje descuento*/
            var porcentajeDescuento = 0;
            var margen = 0;
            var precio = 0;
            var flete = 0;
            var costo = 0;
            var observacion = "";

            json = json + '{"idProducto":"' + arrId[0] + '", "cantidad":"' + cantidad + '", "porcentajeDescuento":"' + porcentajeDescuento + '", "precio":"' + precio + '", "flete":"' + flete + '",  "costo":"' + costo + '", "observacion":"' + observacion + '"},';
        });

        json = json.substr(0, json.length - 1) + ']';

    
        
        $.ajax({
            url: "/GuiaRemision/ChangeDetalle",
            type: 'POST',
            data: json,
            dataType: 'json',
            contentType: 'application/json',
            error: function (error) {

                alert(error)
            },
            success: function (respuesta) {
                location.reload();
            }
        });

    });

    

    /*Evento que se dispara cuando se modifica un control de cantidad de la grilla*/
    $(document).on('change', "input.detincantidad", function () {
        var idproducto = event.target.getAttribute("class").split(" ")[0];
        calcularCantidades(idproducto);
    });

    

    function calcularCantidades(idproducto)
    {
        //Se obtiene cantidad total de pedido
        var cantidadPedido = Number($("." + idproducto + ".detcantidadSaldo").html());
        var cantidad = Number($("." + idproducto + ".detincantidad").val());

        if (cantidad > cantidadPedido) {
            $("." + idproducto + ".detincantidad").val(cantidadPedido);
        }

    };







    /*####################################################
    EVENTOS BUSQUEDA GUIAS
    #####################################################*/



    $("#btnBusqueda").click(function () {
        /*   var idCiudad = $("#idCiudad").val();
           if ((idCiudad == "" || idCiudad == GUID_EMPTY) && $("#guiaRemision_numeroDocumento").val() != "") {
               $("#idCiudad").focus();
               $.alert({
                   title: TITLE_MENSAJE_BUSQUEDA,
                   content: 'Para realizar una búsqueda con número de guía debe indicar la sede MP.',
                   buttons: {
                       OK: function () { }
                   }
               });
               return false;
           }*/
        //sede MP
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val();
        var idGrupoCliente = $("#idGrupoCliente").val(); 

        var numeroDocumento = $("#guiaRemision_numeroDocumento").val();
        var numeroPedido = $("#guiaRemision_pedido_numeroPedido").val();
        var fechaTrasladoDesde = $("#guiaRemision_fechaTrasladoDesde").val();
        var fechaTrasladoHasta = $("#guiaRemision_fechaTrasladoHasta").val();
        //var estado = $("#estado").val();

        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/GuiaRemision/Search",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                idGrupoCliente: idGrupoCliente,
                numeroDocumento: numeroDocumento,
                numeroPedido: numeroPedido,
                fechaTrasladoDesde: fechaTrasladoDesde,
                fechaTrasladoHasta: fechaTrasladoHasta


                //      estado: estado
            },
            error: function () {
                $("#btnBusqueda").removeAttr("disabled");

            },
            success: function (guiaRemisionList) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableGuiasRemision > tbody").empty();
                //FooTable.init('#tableCotizaciones');
                $("#tableGuiasRemision").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < guiaRemisionList.length; i++) {

                    var guiaRemision = "";

                    var styleEstado = "";
                    if (guiaRemisionList[i].estaAnulado == 1) {
                        styleEstado = "style='color: red'";
                    }
                    else if (guiaRemisionList[i].estaFacturado == 1) {
                        styleEstado = "style='color: green'";
                    }
                    else {
                        styleEstado = "style='color: black'";
                    }

                    var noEntregado = '';
                    if (guiaRemisionList[i].estaNoEntregado == 1) {
                        noEntregado = '<input class="' + guiaRemisionList[i].idMovimientoAlmacen + ' ' + guiaRemisionList[i].numeroDocumento + ' chkNoEntregado" type="checkbox" checked></input>'
                    }
                    else {
                        noEntregado = '<input class="' + guiaRemisionList[i].idMovimientoAlmacen + ' ' + guiaRemisionList[i].numeroDocumento + ' chkNoEntregado" type="checkbox"></input>'
                    }

                    var noEntregadoLectura = '';
                    if (guiaRemisionList[i].estaNoEntregado == 1) {
                        noEntregadoLectura = '<input disabled type="checkbox" checked></input>'
                    }
                    else {
                        noEntregadoLectura = '<input disabled type="checkbox"></input>'
                    }

                    var guiaRemision = '<tr data-expanded="false">' +
                        '<td>  ' + guiaRemisionList[i].idMovimientoAlmacen + '</td>' +
                        '<td>  ' + guiaRemisionList[i].serieNumeroGuia + '</td>' +
                        '<td>  ' + guiaRemisionList[i].pedido_numeroPedidoString + '</td>' +
                        '<td>  ' + guiaRemisionList[i].motivoTrasladoString + '</td>' +
                        '<td>  ' + guiaRemisionList[i].usuario_nombre + '</td>' +
                        '<td>  ' + invertirFormatoFecha(guiaRemisionList[i].fechaEmision.substr(0, 10)) + '</td>' +
                        '<td>  ' + invertirFormatoFecha(guiaRemisionList[i].fechaTraslado.substr(0, 10)) + '</td>' +
                        '<td>  ' + guiaRemisionList[i].pedido_cliente_razonSocial + '</td>' +
                        '<td>  ' + guiaRemisionList[i].pedido_cliente_ruc + '</td>' +
                        '<td>  ' + guiaRemisionList[i].ciudadOrigen_nombre + '</td>' +
                        '<td ' + styleEstado + '>  ' + guiaRemisionList[i].estadoDescripcion + '</td>' +
                        '<td>' + guiaRemisionList[i].tipoExtornoToString + '</td>' +
                        '<td>' + noEntregado + '</td>' +
                        '<td>' + noEntregadoLectura + '</td>' +
                        '<td> <button type="button" class="' + guiaRemisionList[i].idMovimientoAlmacen + ' ' + guiaRemisionList[i].numeroDocumento + ' btnVerGuiaRemision btn btn-primary ">Ver</button></td > ' +
                        '</tr>';

                    $("#tableGuiasRemision").append(guiaRemision);
                }


                if (guiaRemisionList.length > 0)
                    $("#msgBusquedaSinResultados").hide();
                else
                    $("#msgBusquedaSinResultados").show();

            }
        });
    });






    $("input[name=guiaRemision_estaAnulado]").on("click", function () {
        var estaAnulado = $("input[name=guiaRemision_estaAnulado]:checked").val();
        $.ajax({
            url: "/GuiaRemision/ChangeEstaAnulado",
            type: 'POST',
            data: {
                estaAnulado: estaAnulado
            },
            success: function () {
            }
        });
    });

    $("input[name=guiaRemision_estaFacturado]").on("click", function () {
        var estaFacturado = $("input[name=guiaRemision_estaFacturado]:checked").val();
        $.ajax({
            url: "/GuiaRemision/ChangeEstaFacturado",
            type: 'POST',
            data: {
                estaFacturado: estaFacturado
            },
            success: function () {
            }
        });
    });


    $("#guiaRemision_fechaTraslado").change(function () {
        var fechaTraslado = $("#guiaRemision_fechaTraslado").val();
        $.ajax({
            url: "/GuiaRemision/ChangefechaTraslado",
            type: 'POST',
            data: {
                fechaTraslado: fechaTraslado
            },
            success: function () {
            }
        });
    });

    $("#guiaRemision_fechaEmision").change(function () {
        var fechaEmision = $("#guiaRemision_fechaEmision").val();
        $.ajax({
            url: "/GuiaRemision/ChangefechaEmision",
            type: 'POST',
            data: {
                fechaEmision: fechaEmision
            },
            success: function () {
            }
        });
    });



    $("#guiaRemision_fechaTrasladoDesde").change(function () {
        var fechaTrasladoDesde = $("#guiaRemision_fechaTrasladoDesde").val();
        $.ajax({
            url: "/GuiaRemision/ChangefechaTrasladoDesde",
            type: 'POST',
            data: {
                fechaTrasladoDesde: fechaTrasladoDesde
            },
            success: function () {
            }
        });
    });

    $("#guiaRemision_fechaTrasladoHasta").change(function () {
        var fechaTrasladoHasta = $("#guiaRemision_fechaTrasladoHasta").val();
        $.ajax({
            url: "/GuiaRemision/ChangefechaTrasladoHasta",
            type: 'POST',
            data: {
                fechaTrasladoHasta: fechaTrasladoHasta
            },
            success: function () {
            }
        });
    });

    $("#motivoTraslado").change(function () {
        var motivoTraslado = $("#motivoTraslado").val();
        $.ajax({
            url: "/GuiaRemision/ChangeMotivoTraslado",
            type: 'POST',
            data: {
                motivoTraslado: motivoTraslado
            },
            success: function () {
            }
        });
    });


    $("#idCiudad").change(function () {
        var idCiudad = $("#idCiudad").val();

        $.ajax({
            url: "/GuiaRemision/ChangeIdCiudad",
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

                $("#guiaRemision_ciudadOrigen_direccionPuntoPartida").val(ciudad.direccionPuntoPartida);

            }
        });
    });


    function ConfirmDialogAtencionParcial(message) {
        $('<div></div>').appendTo('body')
            .html('<div><h6>' + message + '</h6></div>')
            .dialog({
                modal: true, title: 'Confirmación', zIndex: 10000, autoOpen: true,
                width: 'auto', resizable: false,
                buttons: {
                    Si: function () {
                        changeUltimaAtencionParcial(1);

                        $(this).dialog("close");

                    },
                    No: function () {
                        changeUltimaAtencionParcial(0);
                        $(this).dialog("close");
                    }
                },
                close: function (event, ui) {
                    $(this).remove();
                }
            });
        document.body.scrollTop = default_scrollTop;
    }

    function changeUltimaAtencionParcial(ultimaAtencionParcial) {
        if (ultimaAtencionParcial == 1) {
            $("#descripcionUltimaAtencionParcial").html("(Última Atención Parcial)");
        }
        else {
            $("#descripcionUltimaAtencionParcial").html("");
        }

        $.ajax({
            url: "/GuiaRemision/ChangeUltimaAtencionParcial",
            type: 'POST',
            data: {
                ultimaAtencionParcial: ultimaAtencionParcial
            },
            success: function () {
                location.reload();
            }
        });
    }



    $("#guiaRemision_atencionParcial").change(function () {

        var atencionParcial = 1;

        if (!$('#guiaRemision_atencionParcial').prop('checked')) {
            //  $("#descripcionUltimaAtencionParcial").html("");
            atencionParcial = 0;
            //    changeUltimaAtencionParcial(0);
        }
        /*  else {
              changeUltimaAtencionParcial(1);
          }*/



        var estado = $("#guiaRemision_atencionParcial").val();
        $.ajax({
            url: "/GuiaRemision/ChangeAtencionParcial",
            type: 'POST',
            data: {
                atencionParcial: atencionParcial
            },
            success: function () {
                /* if (!$('#guiaRemision_atencionParcial').prop('checked')) {
                     location.reload();
                 }*/
                location.reload();
            }
        });


        /*   
           if ($('#guiaRemision_atencionParcial').prop('checked')) {
               ConfirmDialogAtencionParcial("¿Está atención parcial finaliza la atención del pedido?")
           }*/



    });

    $("#btnSaveTransportista").click(function () {

        if ($("#transportista_descripcion").val().trim() == "") {
            alert("Debe ingresar la descripción del transportista.");
            $('#transportista_descripcion').focus();
            return false;
        }

        if ($("#transportista_direccion").val().trim() == "") {
            alert("Debe ingresar la dirección del transportista.");
            $('#transportista_direccion').focus();
            return false;
        }

        if ($("#transportista_ruc").val().trim() == "") {
            alert("Debe ingresar el RUC del transportista.");
            $('#transportista_ruc').focus();
            return false;
        }

        if ($("#transportista_telefono").val().trim() == "") {
            alert("Debe ingresar el telefono del transportista.");
            $('#transportista_telefono').focus();
            return false;
        }

        var descripcion = $("#transportista_descripcion").val();
        var direccion = $("#transportista_direccion").val();
        var ruc = $("#transportista_ruc").val();
        var telefono = $("#transportista_telefono").val();

        $.ajax({
            url: "/GuiaRemision/CreateTransportistaTemporal",
            type: 'POST',
            dataType: 'JSON',
            data: {
                descripcion: descripcion,
                direccion: direccion,
                ruc: ruc,
                telefono: telefono
          //      estado: estado
            },
            error: function (detalle) { alert("Se generó un error al intentar crear el transportista."); },
            success: function (transportista) {

                $('#guiaRemision_transportista').append($('<option>', {
                    value: transportista.idTransportista,
                    text: transportista.descripcion
                }));
                $('#guiaRemision_transportista').val(transportista.idTransportista);

                $('#guiaRemision_transportista_descripcion').val(transportista.descripcion);
                $('#guiaRemision_transportista_direccion').val(transportista.direccion);
                $('#guiaRemision_transportista_ruc').val(transportista.ruc);
                $('#guiaRemision_transportista_telefono').val(transportista.telefono);
                verificarSiExisteNuevoTransportista();
                toggleControlesTransportista();
            }
        });


        $('#btnCancelTransportista').click();

    });



    $(document).on('change', "#checkCabecera", function () {
        var $j_object = $("input[name='chkMovimientoAlmacen']");
        $.each($j_object, function (key, value) {
            value.checked = event.target.checked;
        });
    });

    $(document).on('change', "input[name='chkMovimientoAlmacen']", function () {

        var countFalse = 0;
        var $j_object = $("input[name='chkMovimientoAlmacen']");
        $.each($j_object, function (key, value) {

            if (value.checked == false) {
                //$("#checkCabecera").attr("checked", "checked");
                countFalse++;
            }
        });

        if (countFalse > 0)
            $("#checkCabecera").removeAttr("checked");
        else
            $("#checkCabecera").attr("checked", "checked");
    });



    $(document).on('click', "a.descargarDesdeVenta", function () {

        //var arrrayClass = event.target.getAttribute("class").split(" ");
        var nombreArchivo = event.target.innerHTML;
        //var numeroPedido = arrrayClass[1];

        $.ajax({
            url: "/Venta/Descargar",
            type: 'POST',
            //  enctype: 'multipart/form-data',
            dataType: 'JSON',
            //  contentType: 'multipart/form-data',
            data: { nombreArchivo: nombreArchivo },
            error: function (detalle) {
            },
            success: function (pedidoAdjunto) {
                var sampleArr = base64ToArrayBuffer(pedidoAdjunto.adjunto);
                saveByteArray(nombreArchivo, sampleArr);
            }
        });
    });

    $(document).on('click', "a.descargar", function () {

        //var arrrayClass = event.target.getAttribute("class").split(" ");
        var nombreArchivo = event.target.innerHTML;
        //var numeroPedido = arrrayClass[1];

        $.ajax({
            url: "/GuiaRemision/Descargar",
            type: 'POST',
            //  enctype: 'multipart/form-data',
            dataType: 'JSON',
            //  contentType: 'multipart/form-data',
            data: { nombreArchivo: nombreArchivo },
            error: function (detalle) {
            },
            success: function (pedidoAdjunto) {
                var sampleArr = base64ToArrayBuffer(pedidoAdjunto.adjunto);
                saveByteArray(nombreArchivo, sampleArr);
            }
        });
    });





    $('#btnIngresar').click(function () {

        desactivarBotonesVer();
        var idMovimientoAlmacen = $("#idMovimientoAlmacen").val();
        /*Si no se cuenta con documentos de venta */
        var yourWindow;
        $.ajax({
            url: "/NotaIngreso/iniciarIngresoDesdeGuiaRemisionTrasladoInterno",
            type: 'POST',
            // dataType: 'JSON',
            data: {
                idMovimientoAlmacen: idMovimientoAlmacen
            },
            error: function (error) {
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
                $("#btnContinuarGenerandoNotaIngreso").removeAttr("disabled");
                $("#btnCancelarNotaIngreso").removeAttr("disabled");
            },
            success: function (movimientoAlmacen) {
                window.location = '/NotaIngreso/Ingresar';
            }
        });
    });






    /*GENERACIÓN DE NOTA DE INGRESO*/
    $('#btnExtornar').click(function () {
        var documentosVentaString = $("#documentosVenta").val();

        $("#li_motivoExtornoGuiaRemision7").hide();
        /*Si no se cuenta con documentos de venta */
        if (documentosVentaString.length == 0) {
            $("#serieNumeroDocumentoVenta").val(documentosVenta);
            $("#divDocumentoVenta").hide();
        }
        else {
            var documentosVenta = documentosVentaString.split(";");
            var documentoSize = documentosVenta.length;
            if (documentosVenta.length > 1) {
                $.alert({
                    //icon: 'fa fa-warning',
                    title: "Error",
                    content: "Existen múltiples Documentos de Venta asocidados a esta Guía de Remisión, no se puede extornar, contacte con TI.",
                    type: 'red',
                    buttons: {
                        OK: function () {

                        }
                    }
                });
                $("#divDocumentoVenta").hide();
                return false;
            }
            else {
                $("#serieNumeroDocumentoVenta").val(documentosVenta);
                $("#divDocumentoVenta").show();
                $("#li_motivoExtornoGuiaRemision7").show();
            }
        }


        $("#serieNumeroDocumentoParaNotaIngreso").val($("#ver_guiaRemision_serieNumeroDocumento").html());
        if ($("#guiaRemision_tipoExtorno").val() == MOV_TIPO_EXTORNO_EXTORNO_PARCIAL) {
            $("#li_motivoExtornoGuiaRemision1").hide();
            $("#li_motivoExtornoGuiaRemision6").hide();
        }
        else {
            $("#li_motivoExtornoGuiaRemision1").show();
            $("#li_motivoExtornoGuiaRemision6").show();
        }



        $("#modalGenerarNotaIngreso").modal();
    });

    $("#btnContinuarGenerandoNotaIngreso").click(function () {

        desactivarBotonesVer();





        //Se obtiene el tipo de nota de ingreso seleccionado
        var motivoExtornoGuiaRemision = $('input:radio[name=motivoExtornoGuiaRemision]:checked').val();
        //Se recupera el id de la guía de remisión
        var idMovimientoAlmacen = $("#idMovimientoAlmacen").val();

        if (motivoExtornoGuiaRemision == null) {
            mostrarMensajeErrorProceso("Debe seleccionar el motivo de extorno de la Guía de Remisión.");
            $("#btnContinuarGenerandoNotaIngreso").removeAttr("disabled");
            $("#btnCancelarNotaIngreso").removeAttr("disabled");
            return false;
        }

        var yourWindow;
        $.ajax({
            url: "/NotaIngreso/iniciarIngresoDesdeGuiaRemision",
            type: 'POST',
            // dataType: 'JSON',
            data: {
                idMovimientoAlmacen: idMovimientoAlmacen,
                motivoExtornoGuiaRemision: motivoExtornoGuiaRemision
            },
            error: function (error) {
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
                $("#btnContinuarGenerandoNotaIngreso").removeAttr("disabled");
                $("#btnCancelarNotaIngreso").removeAttr("disabled");
            },
            success: function (movimientoAlmacen) {
                window.location = '/NotaIngreso/Ingresar';
                /*
                if (movimientoAlmacen.tipoErrorCrearTransaccion == 0) {
                    window.location = '/NotaCredito/Crear';
                }
                else {
                    mostrarMensajeErrorProceso(MENSAJE_ERROR + "\n" + "Detalle Error: " + venta.descripcionError);
                    $("#btnContinuarGenerandoNotaIngreso").removeAttr("disabled");
                    $("#btnCancelarNotaIngreso").removeAttr("disabled");
                }*/
            }
        });

    });

    $('#guiaRemision_pedido_direccionEntrega').change(function () {
        toggleControlesDireccionEntrega();
        var idDireccionEntrega = $('#guiaRemision_pedido_direccionEntrega').val();
        $.ajax({
            url: "/GuiaRemision/ChangeDireccionEntrega",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idDireccionEntrega: idDireccionEntrega
            },
            success: function (direccionEntrega) {

                $("#guiaRemision_pedido_direccionEntrega_telefono").val(direccionEntrega.telefono);
                $("#guiaRemision_pedido_direccionEntrega_contacto").val(direccionEntrega.contacto);
                $("#guiaRemision_pedido_direccionEntrega_descripcion").val(direccionEntrega.descripcion);
                location.reload()
            }
        })
    });



    function toggleControlesDireccionEntrega() {
        var idDireccionEntrega = $('#guiaRemision_pedido_direccionEntrega').val();
        if (idDireccionEntrega == "") {
            $("#guiaRemision_pedido_direccionEntrega_descripcion").attr('disabled', 'disabled');
            $("#guiaRemision_pedido_direccionEntrega_contacto").attr('disabled', 'disabled');
            $("#guiaRemision_pedido_direccionEntrega_telefono").attr('disabled', 'disabled');

        }
        else {
            /*  $("#pedido_direccionEntrega_telefono").val($('#pedido_direccionEntrega').find(":selected").attr("telefono"));*/
            $("#guiaRemision_pedido_direccionEntrega_descripcion").removeAttr("disabled");
            $("#guiaRemision_pedido_direccionEntrega_contacto").removeAttr("disabled");
            $("#guiaRemision_pedido_direccionEntrega_telefono").removeAttr("disabled");
        }
    }



    /*####################################################
    EVENTOS BUSQUEDA GUIAS FACTURA CONSOLIDADA
    #####################################################*/


    $("#btnBusquedaGuiasFacturaConsolidada").click(function () {

        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idClienteConsolidacion").val();
        var fechaTrasladoDesde = $("#guiaRemision_fechaTrasladoDesde").val();
        var fechaTrasladoHasta = $("#guiaRemision_fechaTrasladoHasta").val();


        $.ajax({
            url: "/GuiaRemision/SearchParaFacturaConsolidada",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                fechaTrasladoDesde: fechaTrasladoDesde,
                fechaTrasladoHasta: fechaTrasladoHasta
            },
            success: function (guiaRemisionList) {

                $("#checkCabecera").attr("checked", "checked");

                $("#tableGuiasRemision > tbody").empty();


                //No se pagina
                $("#tableGuiasRemision").footable();

                for (var i = 0; i < guiaRemisionList.length; i++) {

                    var guiaRemision = "";

                    var style = "";
                    if (guiaRemisionList[i].estaAnulado == 1) {
                        style = "style='color: red'";
                    }
                    else if (guiaRemisionList[i].estaFacturado == 1) {
                        style = "style='color: green'";
                    }
                    else {
                        style = "style='color: black'";
                    }

                    var guiaRemision = '<tr data-expanded="false">' +
                        '<td>  ' + guiaRemisionList[i].idMovimientoAlmacen + '</td>' +
                        '<td><input class="' + guiaRemisionList[i].idMovimientoAlmacen + ' ' + guiaRemisionList[i].serieNumeroGuia + ' ' + guiaRemisionList[i].pedido.numeroReferenciaCliente + ' ' + guiaRemisionList[i].ciudadOrigen.nombre +
                        '" numeroPedido="' + guiaRemisionList[i].pedido.numeroPedidoString + '" numeroGrupoPedido="' + guiaRemisionList[i].pedido.numeroGrupoPedidoString+'" name="chkMovimientoAlmacen" type="checkbox" checked>' + '</td>' +
                        '<td>  ' + guiaRemisionList[i].serieNumeroGuia + '</td>' +
                        '<td>  ' + guiaRemisionList[i].pedido.numeroPedidoString + '</td>' +
                        '<td>  ' + guiaRemisionList[i].pedido.numeroGrupoPedidoString + '</td>' +
                        '<td>  ' + guiaRemisionList[i].usuario.nombre + '</td>' +
                        '<td>  ' + invertirFormatoFecha(guiaRemisionList[i].fechaEmision.substr(0, 10)) + '</td>' +
                        '<td>  ' + invertirFormatoFecha(guiaRemisionList[i].fechaTraslado.substr(0, 10)) + '</td>' +
                        '<td>  ' + guiaRemisionList[i].pedido.cliente.razonSocial + '</td>' +
                        '<td>  ' + guiaRemisionList[i].pedido.cliente.ruc + '</td>' +
                        '<td>  ' + guiaRemisionList[i].ciudadOrigen.nombre + '</td>' +
                        '<td ' + style + '>  ' + guiaRemisionList[i].estadoDescripcion + '</td>' +
                        '<td> <button type="button" class="' + guiaRemisionList[i].idMovimientoAlmacen + ' ' + guiaRemisionList[i].numeroDocumento + ' btnVerGuiaRemision btn btn-primary ">Ver</button></td > ' +
                        '</tr>';

                    $("#tableGuiasRemision").append(guiaRemision);
                }


                if (guiaRemisionList.length > 0)
                    $("#msgBusquedaSinResultados").hide();
                else
                    $("#msgBusquedaSinResultados").show();
            }
        });
    });


    $("#btnLimpiarBusquedaGuiasFacturaConsolidada").click(function () {
        $.ajax({
            url: "/GuiaRemision/CleanBusquedaFacturaConsolidada",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });


    $("#btnIniciarGenerarVenta").click(function () {

        $.ajax({
            url: "/GuiaRemision/validarPreciosVentaConsolidada",
            type: 'POST',
          //  dataType: 'json',
           // contentType: 'application/json',
            error: function (detalle) {
            
            },
            success: function (resultado) {
                if (resultado != "") {
                    $.alert({
                        title: "Precios Inválidos",
                        type: "orange",
                        content: resultado,
                        buttons: {
                            OK: function () { }
                        }
                    });

                }
                else {
                    iniciarGenerarVenta()
                }
            }
        });



    });

    function iniciarGenerarVenta() {

        $("#modalIngresarDatosVentaConsolidada").modal();

        $(".selectUnidad").removeAttr("disabled");
        $("#idGuiaRemisionFacturaConsolidada").removeAttr("disabled");
        $("#numeroOrdenCompraFacturaConsolidada").removeAttr("disabled");
        $("#numeroReferenciaAdicionalFacturaConsolidada").removeAttr("disabled");
        $("#numeroReferenciaAdicionalFacturaConsolidada").val("");


        $('#idGuiaRemisionFacturaConsolidada')
            .find('option')
            .remove()
            .end();

        //     $("#guiaRemisionList > li").remove().end();

        var json = '[ ';

        var count = 0;
        var $j_object = $("input[name='chkMovimientoAlmacen']");

        var ordenCompraAsignado = false;
        $.each($j_object, function (key, value) {

            var classChkMovimientoAlmacen = value.getAttribute("class").split(" ");
            var idMovimientoAlmacen = classChkMovimientoAlmacen[0];
            if (value.checked) {

                var numeroOrdenCompraFacturaConsolidada = "";
                if (classChkMovimientoAlmacen[2] != null && classChkMovimientoAlmacen[2] != "null") {
                    numeroOrdenCompraFacturaConsolidada = classChkMovimientoAlmacen[2];
                }

                if (!ordenCompraAsignado) {
                    ordenCompraAsignado = true;
                    $("#numeroOrdenCompraFacturaConsolidada").val(numeroOrdenCompraFacturaConsolidada);
                    $("#ver_guiaRemision_pedido_cliente_ciudad").html(classChkMovimientoAlmacen[3]);

                }

                $('#idGuiaRemisionFacturaConsolidada').append($('<option>', {
                    value: classChkMovimientoAlmacen[0],
                    text: classChkMovimientoAlmacen[1],
                    numeroOrdenCompraFacturaConsolidada: numeroOrdenCompraFacturaConsolidada,
                    ciudad: classChkMovimientoAlmacen[3],
                }));
            }

        });



        $("#tableVentaConsolidadaConfirmacion > tbody").empty();
        $("#tableVentaConsolidadaConfirmacion").footable();

        for (var i = 0; i < ventaDetalleList.length; i++) {

            var unidad = "";
            var cantidad = 1;
            /*Si la equivalencia es 1 quiere decir que no cuenta con unidad alternativa*/
            if (ventaDetalleList[i].producto.equivalencia == 1) {
                unidad = "<select class='form-control selectUnidad'  sku='" + ventaDetalleList[i].producto.sku + "'  idProducto='" + ventaDetalleList[i].producto.idProducto + "'  >" +
                    "<option esUnidadAlternativa='0' cantidad='" + ventaDetalleList[i].sumCantidadUnidadEstandar + "'>" + ventaDetalleList[i].producto.unidad + "</option>" +
                    "</select > ";
                cantidad = ventaDetalleList[i].sumCantidadUnidadEstandar;
            }
            else {
                unidad = "<select class='form-control selectUnidad'  sku='" + ventaDetalleList[i].producto.sku + "'  idProducto='" + ventaDetalleList[i].producto.idProducto + "'  >" +
                    "<option esUnidadAlternativa='0' cantidad='" + ventaDetalleList[i].sumCantidadUnidadEstandar + "'>" + ventaDetalleList[i].producto.unidad + "</option>" +
                    "<option esUnidadAlternativa='1' cantidad='" + ventaDetalleList[i].sumCantidadUnidadAlternativa + "'> " + ventaDetalleList[i].producto.unidad_alternativa + "</option>" +
                    "</select > ";
                cantidad = ventaDetalleList[i].sumCantidadUnidadEstandar;
            }

            var ventaDetalle = '<tr data-expanded="false">' +
                '<td>  ' + ventaDetalleList[i].producto.idProducto + '</td>' +
                '<td>  ' + ventaDetalleList[i].producto.sku + '</td>' +
                '<td>  ' + ventaDetalleList[i].producto.descripcion + '</td>' +
                '<td> ' + unidad + '</td>' +
                '<td style="text-align: right" id="cantidadSeleccionada' + ventaDetalleList[i].producto.idProducto + '"> ' + cantidad + '</td>' +
                '<td style="text-align: right" >  ' + ventaDetalleList[i].sumPrecioUnitario.toFixed(cantidadDecimales) + '</td>' +
                '</tr>';

            $("#tableVentaConsolidadaConfirmacion").append(ventaDetalle);
        }

    }


    $("#btnContinuarGenerandoVenta").click(function () {

        $('body').loadingModal("show");
        $('body').loadingModal({
            text: 'Generando Venta...'
        });

        $("#idMovimientoAlmacen").val($("#idGuiaRemisionFacturaConsolidada").val());

        var unidadInvalida = 0;

        var json = '[ ';

        var $j_object = $(".selectUnidad");
        $.each($j_object, function (key, value) {


            if (!esEntero($(this).find('option:selected').attr("cantidad"))) {

                $.alert({
                    title: "Unidad Inválida",
                    type: "orange",
                    content: 'La cantidad atendida del producto ' + value.getAttribute("sku") + ' expresada en la unidad ' + $(this).val() +' tiene decimales. Debe seleccionar unidad alernativa.',
                    buttons: {
                        OK: function () { }
                    }
                });
                unidadInvalida++;
                $(this).focus();
            }

            json = json + '{"idProducto":"' + value.getAttribute("idProducto") + '", '+
                '"cantidad":' + $(this).find('option:selected').attr("cantidad") + ', ' +
                '"esUnidadAlternativa":' + $(this).find('option:selected').attr("esUnidadAlternativa") +
            '},';
        });

        if (unidadInvalida > 0) {
            $('body').loadingModal("hide");
            return false;
        }

        json = json.substr(0, json.length - 1) + ']';
        


        var isDisabled = $('#numeroOrdenCompraFacturaConsolidada').prop('disabled');

        var controller = "generarVentaConsolidada";
        if (isDisabled)
            controller = "obtenerVentaConsolidada";

        $(".selectUnidad").attr("disabled", "disabled");
        $("#idGuiaRemisionFacturaConsolidada").attr("disabled", "disabled");
        $("#numeroOrdenCompraFacturaConsolidada").attr("disabled", "disabled");
        $("#numeroReferenciaAdicionalFacturaConsolidada").attr("disabled", "disabled");
        

        $.ajax({
            url: "/Venta/" + controller,
            type: 'POST',
            data: json,
            dataType: 'json',
            contentType: 'application/json',
            error: function (detalle) {
                $('body').loadingModal("hide");
                mostrarMensajeErrorProceso();
            },
            success: function (resultado) {
                $('body').loadingModal("hide");
                mostrarModalFacturar(resultado);

                var guiaRemision = resultado.venta.guiaRemision;

                $("#facturarver_guiaRemision_fechaTraslado").html(invertirFormatoFecha(guiaRemision.fechaTraslado.substr(0, 10)));
                $("#facturarver_guiaRemision_fechaEmision").html(invertirFormatoFecha(guiaRemision.fechaEmision.substr(0, 10)));
                $("#facturarver_guiaRemision_serieNumeroDocumento").html(guiaRemision.serieNumeroGuia);
            }
        });


        

    });


    $(document).on('change', ".selectUnidad", function () {
        $("#cantidadSeleccionada" + $(this).attr("idProducto")).html($(this).find('option:selected').attr("cantidad"));
    })



    var ventaDetalleList = null;


    $("#btnConsolidarAtenciones").click(function () {

        // $(document).on('click', "button.btnVerGuiaRemision", function () {});


        $("#guiaRemisionList > li").remove().end();


        var json = '[ ';

        var count = 0;
        var $j_object = $("input[name='chkMovimientoAlmacen']");

        var existeDiferenciaNumeroPedido = false;
        var existeDiferenciaNumeroGrupoPedido = false;
        var numeroPedidoAnterior = "";
        var numeroGrupoPedidoAnterior = "";

        $.each($j_object, function (key, value) {

            var classChkMovimientoAlmacen = value.getAttribute("class").split(" ");
            var idMovimientoAlmacen = classChkMovimientoAlmacen[0];

            if (value.checked) {
                count++;
                $('#guiaRemisionList').append($('<li />').html(classChkMovimientoAlmacen[1]));
                json = json + '{"idMovimientoAlmacen":"' + idMovimientoAlmacen + '"},';

                var numeroPedido = value.getAttribute("numeroPedido");
                var numeroGrupoPedido = value.getAttribute("numeroGrupoPedido");

                if (numeroPedidoAnterior != numeroPedido && numeroPedidoAnterior != "")
                    existeDiferenciaNumeroPedido = true;
                else
                    numeroPedidoAnterior = numeroPedido

                if (numeroGrupoPedidoAnterior != numeroGrupoPedido && numeroGrupoPedidoAnterior != "")
                    existeDiferenciaNumeroGrupoPedido = true;
                else
                    numeroGrupoPedidoAnterior = numeroGrupoPedido

            }

        });

        if (existeDiferenciaNumeroPedido && numeroGrupoPedidoAnterior == "") {
            $.alert({
                //icon: 'fa fa-warning',
                title: "¡Advertencia!",
                content: "Se han seleccionado distintos pedidos.",
                type: 'orange',
                buttons: {
                    OK: function () {
                    }
                }
            });
        }
        else if (existeDiferenciaNumeroGrupoPedido) {
            $.alert({
                //icon: 'fa fa-warning',
                title: "¡Advertencia!",
                content: "Se han seleccionado distintos grupos de pedido.",
                type: 'orange',
                buttons: {
                    OK: function () {
                    }
                }
            });
        }





        if (count == 0) {
            $.alert({
                //icon: 'fa fa-warning',
                title: "Seleccionar Guías Remisión",
                content: "Debe seleccionar al menos una guía de remisión.",
                type: 'orange',
                buttons: {
                    OK: function () {
                    }
                }
            });
            return false;
        }





        json = json.substr(0, json.length - 1) + ']';


        $.ajax({
            url: "/GuiaRemision/consolidarAtenciones",
            type: 'POST',
            data: json,
            dataType: 'json',
            contentType: 'application/json',
            success: function (respuesta) {

                ventaDetalleList = respuesta.ventaDetalleList;

                $("#modalVerVentaConsolidada").modal();

                $("#tableVentaConsolidada > tbody").empty();
                $("#tableVentaConsolidada").footable();

           /*     $("#tableGuiasRemision").footable({
                    "paging": {
                        "enabled": true
                    }
                });
                */
                for (var i = 0; i < ventaDetalleList.length; i++) {
               
                    var ventaDetalle = '<tr data-expanded="false">' +
                        '<td>  ' + ventaDetalleList[i].producto.idProducto + '</td>' +
                        '<td>  ' + ventaDetalleList[i].producto.sku + '</td>' +
                        '<td>  ' + ventaDetalleList[i].producto.descripcion + '</td>' +
                        '<td>  ' + ventaDetalleList[i].producto.unidad + '</td>' +
                        '<td>  ' + ventaDetalleList[i].sumCantidadUnidadEstandar + '</td>' +
                        '<td>  ' + ventaDetalleList[i].producto.unidad_alternativa + '</td>' +
                        '<td>  ' + ventaDetalleList[i].sumCantidadUnidadAlternativa + '</td>' +
                        '<td>  ' + ventaDetalleList[i].producto.equivalencia + '</td>' +
                /*        '<td>  ' + ventaDetalleList[i].producto.equivalencia + '</td>' +
                        '<td>  ' + ventaDetalleList[i].esPrecioAlternativo + '</td>' +*/

                        '<td style="text-align: right" >  ' + ventaDetalleList[i].sumPrecioNeto + '</td>' +
                        '<td style="text-align: right" >  ' + ventaDetalleList[i].sumPrecioUnitario.toFixed(cantidadDecimales) + '</td>' +                      
                        '</tr>';                 

                    $("#tableVentaConsolidada").append(ventaDetalle);
                }

            }
        });

    });



    $("#btnGenerarReporteDetallado").click(function () {
        $("#guiaRemisionList > li").remove().end();
        var json = '[ ';
        var count = 0;
        var $j_object = $("input[name='chkMovimientoAlmacen']");
        $.each($j_object, function (key, value) {
            var classChkMovimientoAlmacen = value.getAttribute("class").split(" ");
            var idMovimientoAlmacen = classChkMovimientoAlmacen[0];
            if (value.checked) {
                count++;
                $('#guiaRemisionList').append($('<li />').html(classChkMovimientoAlmacen[1]));
                json = json + '{"idMovimientoAlmacen":"' + idMovimientoAlmacen + '"},';
            }
        });

        if (count == 0) {
            alert("Debe seleccionar al menos una guía de remisión.");
            return false;
        }

        json = json.substr(0, json.length - 1) + ']';
        window.open('/GuiaRemision/obtenerDetalleAtenciones');
        /*
        $.ajax({
            url: "/GuiaRemision/obtenerDetalleAtenciones",
            type: 'POST',
           // data: json,
            dataType: 'JSON',
            //contentType: 'application/json',
            error: function (detalle) {
                alert(detalle);
            },
            success: function (xls) {
                window.open('/General/DownLoadFile?fileName=' + fileName);
                var sampleArr = base64ToArrayBuffer(pedidoAdjunto.adjunto);
                saveByteArray("Detalle Atenciones.xls", sampleArr);
            }
        });*/

    });

    $('#idGuiaRemisionFacturaConsolidada').change(function () {
        $("#numeroOrdenCompraFacturaConsolidada").val($("#idGuiaRemisionFacturaConsolidada").attr("numeroOrdenCompraFacturaConsolidada"));
        $("#ciudad").val($("#idGuiaRemisionFacturaConsolidada").attr("ciudad"));

        $.ajax({
            url: "/GuiaRemision/ChangeGuiaRemisionFacturaConsolidada",
            type: 'POST',
            data: {
                idGuiaRemision: $("#idGuiaRemisionFacturaConsolidada").val()
            },
            error: function (detalle) {
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
            },
            success: function (resultado) {
            }
        })
    });

    $('#numeroOrdenCompraFacturaConsolidada').change(function () {

        $.ajax({
            url: "/GuiaRemision/ChangeNumeroOrdenCompraFacturaConsolidada",
            type: 'POST',
            data: {
                numeroOrdenCompra: $("#numeroOrdenCompraFacturaConsolidada").val()
            },
            error: function (detalle) {
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
            },
            success: function (resultado) {
            }
        })
    });


    $("#guiaRemision_placaVehiculo").change(function () {
        changeInputString("placaVehiculo", $("#guiaRemision_placaVehiculo").val())
    });


    function changeInputStringPedido(propiedad, valor) {
        $.ajax({
            url: "/GuiaRemision/changeInputStringPedido",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function ChangeInputIntPedido(propiedad, valor) {
        $.ajax({
            url: "/GuiaRemision/changeInputIntPedido",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#guiaRemision_pedido_numeroReferenciaCliente").change(function () {
        changeInputStringPedido("numeroReferenciaCliente", $("#guiaRemision_pedido_numeroReferenciaCliente").val())
    });

    $("#guiaRemision_pedido_numeroGrupoPedido").change(function () {
        ChangeInputIntPedido("numeroGrupoPedido", $("#guiaRemision_pedido_numeroGrupoPedido").val())
    });

    $("#guiaRemision_pedido_numeroPedido").change(function () {
        ChangeInputIntPedido("numeroPedido", $("#guiaRemision_pedido_numeroPedido").val())
    });





    $('#numeroReferenciaAdicionalFacturaConsolidada').change(function () {
        $.ajax({
            url: "/GuiaRemision/ChangeNumeroReferenciaAdicionalFacturaConsolidada",
            type: 'POST',
            data: {
                numeroReferenciaAdicional: $("#numeroReferenciaAdicionalFacturaConsolidada").val()
            },
            error: function (detalle) {
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
            },
            success: function (resultado) {
            }
        })
    });

    $("#btnEditarVentaConsolidada").click(function () {

        $("#btnCancelarFacturarPedido").click();
        // desactivarBotonesVer();


        var yourWindow;
        $.ajax({
            url: "/Venta/iniciarEdicionVentaConsolidada",
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al iniciar la edición de la venta consolidada."); },
            success: function (fileName) {
                yourWindow = window.open(
                    "/Venta/Vender",
                    "Edición de Venta Consolidada",
                    "resizable,scrollbars,status"
                );
            }
        });

    });



});