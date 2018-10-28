
jQuery(function ($) {


    //CONSTANTES POR DEFECTO
    var cantidadDecimales = 2;
    var cantidadCuatroDecimales = 4;
    var IGV = 0.18;
    var SIMBOLO_SOL = "S/";
    var MILISEGUNDOS_AUTOGUARDADO = 5000;

    //Estados para búsqueda de Pedidos
    var ESTADOS_TODOS = -1;
    var ESTADO_PENDIENTE_APROBACION = 0;
    var ESTADO_APROBADA = 1;
    var ESTADO_DENEGADA = 2;
    var ESTADO_ACEPTADA = 3;
    var ESTADO_RECHAZADA = 4;
    var ESTADO_EN_EDICION = 7;

    //Etiquetas de estadps para búsqueda de Pedidos
    var ESTADO_PENDIENTE_APROBACION_STR = "Pendiente de Aprobación";
    var ESTADO_APROBADA_STR = "Aprobada";
    var ESTADO_DENEGADA_STR = "Denegada";
    var ESTADO_ACEPTADA_STR = "Aceptada";
    var ESTADO_RECHAZADA_STR = "Rechazada";
    var ESTADO_EN_EDICION_STR = "En Edición";

    //Eliminar luego 
    var CANT_SOLO_OBSERVACIONES = 0;
    var CANT_SOLO_CANTIDADES = 1;
    var CANT_CANTIDADES_Y_OBSERVACIONES = 2;

    var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";

    /*
     * 2 BusquedaPedidos
       3 CrearPedido
     */

    
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición/creación; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';
    var TITLE_MENSAJE_BUSQUEDA = "Ingresar datos solicitados";

    function hola() {
        alert('as');
    }




    $(document).ready(function () {
    
        obtenerConstantes();
        //setTimeout(autoGuardarGuiaRemision, MILISEGUNDOS_AUTOGUARDADO);
        verificarSiExisteNuevoTransportista();
        esPaginaImpresion();
        cargarChosenCliente();
        $("#btnBusqueda").click();


        if ($("#pagina").val() == 4) {
            if ($("#idMovimientoAlmacen").val() != "") {
                showMovimientoAlmacen($("#idMovimientoAlmacen").val());
            }
        }
        else if ($("#pagina").val() == 19) {
            $("#btnBusquedaGuiasFacturaConsolidada").click();
            
        }

    });

    window.onafterprint = function () {
        if ($("#pagina").val() == 22) {
            window.close();
        }
    };

    function esPaginaImpresion() {
        if ($("#pagina").val() == 22) {
            window.print();
        }
    }

    $("#btnLimpiarBusqueda").click(function () {
        $.ajax({
            url: "/NotaIngreso/CleanBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });

    function verificarSiExisteNuevoTransportista() {
        $('#notaIngreso_transportista option').each(function () {
            if ($(this).val() == GUID_EMPTY) {
                $("#btnAgregarTransportista").attr("disabled", "disabled");
                $("#notaIngreso_transportista_descripcion").attr('disabled', 'disabled');
                $("#notaIngreso_transportista_ruc").attr("disabled", "disabled");
                $("#notaIngreso_transportista_direccion").attr("disabled", "disabled");
                $("#notaIngreso_transportista_brevete").attr("disabled", "disabled");
            }
            else {
                $("#btnAgregarTransportista").removeAttr("disabled");
                $("#notaIngreso_transportista_descripcion").removeAttr('disabled');
                $("#notaIngreso_transportista_ruc").removeAttr("disabled");
                $("#notaIngreso_transportista_direccion").removeAttr("disabled");
                $("#notaIngreso_transportista_brevete").removeAttr("disabled");
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
            url: "/NotaIngreso/SearchClientes"
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
            url: "/NotaIngreso/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente) {
          /*      $("#pedido_numeroReferenciaCliente").val("");
                $("#pedido_direccionEntrega").val("");
                $("#pedido_contactoEntrega").val("");
                $("#pedido_telefonoContactoEntrega").val("");
                $("#pedido_contactoPedido").val("");
                $("#pedido_telefonoContactoPedido").val("");*/
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
    $("#notaIngreso_fechaTraslado").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaTraslado);

    var fechaEmision = $("#fechaEmisiontmp").val();
    $("#notaIngreso_fechaEmision").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEmision);


    var fechaTrasladoDesde = $("#fechaTrasladoDesdetmp").val();
    $("#notaIngreso_fechaTrasladoDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaTrasladoDesde);

    var fechaTrasladoHasta = $("#fechaTrasladoHastatmp").val();
    $("#notaIngreso_fechaTrasladoHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaTrasladoHasta);


    var documentoVenta_fechaEmision = $("#documentoVenta_fechaEmisiontmp").val();
    $("#documentoVenta_fechaEmision").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", documentoVenta_fechaEmision);

    var documentoVenta_fechaVencimiento = $("#documentoVenta_fechaVencimientotmp").val();
    $("#documentoVenta_fechaVencimiento").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", documentoVenta_fechaVencimiento);



    /**
     * FIN DE CONTROLES DE FECHAS
     */






    /* ################################## INICIO CHANGE CONTROLES */

    function toggleControlesTransportista() {
        var idTransportista = $("#notaIngreso_transportista").val();
        if (idTransportista == "") {
            $("#notaIngreso_transportista_descripcion").attr('disabled', 'disabled');
            $("#notaIngreso_transportista_ruc").attr('disabled', 'disabled');
            $("#notaIngreso_transportista_direccion").attr('disabled', 'disabled');
            $("#notaIngreso_transportista_brevete").attr('disabled', 'disabled');
            
        }
        else {
            /*  $("#pedido_direccionEntrega_telefono").val($('#pedido_direccionEntrega').find(":selected").attr("telefono"));*/
            $("#notaIngreso_transportista_descripcion").removeAttr("disabled");
            $("#notaIngreso_transportista_ruc").removeAttr("disabled");
            $("#notaIngreso_transportista_direccion").removeAttr("disabled");
            $("#notaIngreso_transportista_brevete").removeAttr("disabled");
        }
    }

    $("#notaIngreso_transportista").change(function () {
        toggleControlesTransportista();
        var idTransportista = $("#notaIngreso_transportista").val();

        $.ajax({
            url: "/NotaIngreso/ChangeTransportista",
            type: 'POST',
            dataType: "JSON",
            data: {
                idTransportista: idTransportista
            },
            success: function (transportista) {

                $("#notaIngreso_transportista_descripcion").val(transportista.descripcion);
                $("#notaIngreso_transportista_ruc").val(transportista.ruc);
                $("#notaIngreso_transportista_direccion").val(transportista.direccion);
                $("#notaIngreso_transportista_brevete").val(transportista.brevete);
            }
        });
    });


 


    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/NotaIngreso/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function ChangeInputInt(propiedad, valor) {
        $.ajax({
            url: "/NotaIngreso/ChangeInputInt",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    $("#notaIngreso_serieGuiaReferencia").change(function () {
        changeInputString("serieGuiaReferencia", $("#notaIngreso_serieGuiaReferencia").val())
    });

    $("#notaIngreso_numeroGuiaReferencia").change(function () {
        ChangeInputInt("numeroGuiaReferencia", $("#notaIngreso_numeroGuiaReferencia").val())
    });

    $("#notaIngreso_serieDocumentoVentaReferencia").change(function () {
        changeInputString("serieDocumentoVentaReferencia", $("#notaIngreso_serieDocumentoVentaReferencia").val())
    });

    $("#notaIngreso_numeroDocumentoVentaReferencia").change(function () {
        ChangeInputInt("numeroDocumentoVentaReferencia", $("#notaIngreso_numeroDocumentoVentaReferencia").val())
    });

    $("#notaIngreso_sustentoExtorno").change(function () {
        changeInputString("sustentoExtorno", $("#notaIngreso_sustentoExtorno").val())
    });

    $("#tipoDocumentoVentaReferencia").change(function () {
        var tipoDocumentoVentaReferencia = $("#tipoDocumentoVentaReferencia").val();
        $.ajax({
            url: "/NotaIngreso/ChangeTipoDocumentoVentaReferencia",
            type: 'POST',
            data: {
                tipoDocumentoVentaReferencia: tipoDocumentoVentaReferencia
            },
            success: function () { }
        });
    });


    /*
    $("#notaIngreso_certificadoInscripcion").change(function () {
        changeInputString("certificadoInscripcion", $("#notaIngreso_certificadoInscripcion").val())
    });*/

    $("#notaIngreso_observaciones").change(function () {
        changeInputString("observaciones", $("#notaIngreso_observaciones").val())
    });

    function changeInputStringTransportista(propiedad, valor) {
        $.ajax({
            url: "/NotaIngreso/ChangeInputStringTransportista",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#notaIngreso_transportista_ruc").change(function () {
        changeInputStringTransportista("ruc", $("#notaIngreso_transportista_ruc").val())
    });

    $("#notaIngreso_transportista_direccion").change(function () {
        changeInputStringTransportista("direccion", $("#notaIngreso_transportista_direccion").val())
    });

    $("#notaIngreso_transportista_brevete").change(function () {
        changeInputStringTransportista("brevete", $("#notaIngreso_transportista_brevete").val())
    });
    

   /* ################################## FIN CHANGE CONTROLES */

    

    ////////CREAR/EDITAR GUIA REMISION
    

    function crearNotaIngreso() {
        if (!validarIngresoDatosObligatoriosNotaIngreso())
            return false;
        
        desactivarBotonesCreacionModificacion();

        $('body').loadingModal({
            text: 'Creando Nota de Ingreso...'
        });
        $.ajax({
            url: "/NotaIngreso/Create",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal("hide");
                activarBotonesCreacionModificacion();
                alert(MENSAJE_ERROR);
            },
            success: function (resultado) {

                $('body').loadingModal('hide')

                if (resultado.notaIngresoValidacion.tipoErrorValidacion == 0) {

                    if (resultado.generarNotaCredito) {

                        $.alert({
                            //icon: 'fa fa-warning',
                            title: TITLE_EXITO,
                            content: "La nota de ingreso número " + resultado.serieNumeroNotaIngreso + " fue creada PROVISIONALMENTE. A continuación, genere la Nota de Crédito correspondiente para culminar el proceso de extorno.",
                            type: 'orange',
                            buttons: {
                                OK: function () {
                                    $('body').loadingModal({
                                        text: 'Creando Nota de Crédito...'
                                    });
                                    $('body').loadingModal('show');
                                $.ajax({
                                    url: "/NotaCredito/iniciarCreacionNotaCreditoDesdeNotaIngreso",
                                    type: 'POST',
                                    dataType: 'JSON',
                                    //data: { },
                                    error: function (error) {
                                        $('body').loadingModal('hide')
                                        mostrarMensajeErrorProceso(MENSAJE_ERROR);
                                        /*IMPORTANTE: FALTA DEFINIR EL ESTADO DE LA NOTA DE INGRESO CUANDO NO SE CREA CORRECTAMENTE LA NOTA DE CREDITO*/
                                    },
                                    success: function (venta) {
                                        $('body').loadingModal('hide')
                                        if (venta.tipoErrorCrearTransaccion == 0) {
                                            window.location = '/NotaCredito/Crear';
                                        }
                                        else {
                                            mostrarMensajeErrorProceso(MENSAJE_ERROR + "\n" + "Detalle Error: " + venta.descripcionError);
                                        }
                                    }
                                });
                                }
                            }
                        });
                    }
                    else {
                        $.alert({
                            //icon: 'fa fa-warning',
                            title: TITLE_EXITO,
                            content: "La nota de ingreso número " + resultado.serieNumeroNotaIngreso + " fue creada correctamente.",
                            type: 'green',
                            buttons: {
                                OK: function () {
                                    window.location = '/NotaIngreso/Index?idMovimientoAlmacen=' + resultado.idNotaIngreso;
                                }
                            }
                        });
                    }
                }
                else {
                    mostrarMensajeErrorProceso(resultado.notaIngresoValidacion.tipoErrorValidacionString + " " + resultado.notaIngresoValidacion.descripcionError);
                    activarBotonesCreacionModificacion();
                }
            }
        });
    }


    function desactivarBotonesFacturar() {
        $("#btnCancelarFacturarPedido").attr('disabled', 'disabled');
        $("#btnEditarCliente").attr('disabled', 'disabled');
        $("#btnEditarVenta").attr('disabled', 'disabled');
        $("#btnAceptarFacturarPedido").attr('disabled', 'disabled');
        $("#btnConfirmarFacturarPedido").attr('disabled', 'disabled');
        $("#btnCancelarConfirmarFacturarPedido").attr('disabled', 'disabled');
    }

    function activarBotonesFacturar() {
        $("#btnCancelarFacturarPedido").removeAttr('disabled');
        $("#btnEditarCliente").removeAttr('disabled');
        $("#btnEditarVenta").removeAttr('disabled');
        $("#btnAceptarFacturarPedido").removeAttr('disabled');
        $("#btnConfirmarFacturarPedido").removeAttr('disabled');
        $("#btnCancelarConfirmarFacturarPedido").removeAttr('disabled');
    }



    function desactivarBotonesCreacionModificacion() {
        $("#btnFinalizarCreacionNotaIngreso").attr('disabled', 'disabled');
        $("#btnCancelarNotaIngreso").attr('disabled', 'disabled');
    }

    function activarBotonesCreacionModificacion() {
        $("#btnFinalizarCreacionNotaIngreso").removeAttr('disabled');
        $("#btnCancelarNotaIngreso").removeAttr('disabled');
    }


    $("#btnFinalizarCreacionNotaIngreso").click(function () {
        crearNotaIngreso();
    });


    function validarIngresoDatosObligatoriosNotaIngreso() {

        if ($("#notaIngreso_serieGuiaReferencia").length) {
            if ($("#notaIngreso_serieGuiaReferencia").val().length > 4) {
                $.alert({
                    //icon: 'fa fa-warning',
                    title: 'Validación',
                    content: 'La serie de la guía de remisión de referencia no debe contener más de 4 caracteres.',
                    type: 'red',
                    buttons: {
                        OK: function () {
                            $('#notaIngreso_serieGuiaReferencia').focus();
                        }
                    }
                });
                return false;
            }

            if ($("#notaIngreso_numeroGuiaReferencia").val() > 0 && $('#notaIngreso_serieGuiaReferencia') == "") {
                $.alert({
                    //icon: 'fa fa-warning',
                    title: 'Validación',
                    content: 'Debe ingresar la serie de la guía de remisión de referencia.',
                    type: 'red',
                    buttons: {
                        OK: function () {
                            $('#notaIngreso_serieGuiaReferencia').focus();
                        }
                    }
                });
                return false;
            }

            if ($("#notaIngreso_serieDocumentoVentaReferencia").val().length > 4) {
                $.alert({
                    //icon: 'fa fa-warning',
                    title: 'Validación',
                    content: 'La serie del documento de venta de referencia.',
                    type: 'red',
                    buttons: {
                        OK: function () {
                            $('#notaIngreso_serieDocumentoVentaReferencia').focus();
                        }
                    }
                });
                return false;
            }

            if ($("#notaIngreso_numeroDocumentoVentaReferencia").val() > 0 && $("#tipoDocumentoVentaReferencia").val() == 0) {
                $.alert({
                    //icon: 'fa fa-warning',
                    title: 'Validación',
                    content: 'Debe ingresar el tipo de documento de venta de referencia.',
                    type: 'red',
                    buttons: {
                        OK: function () {
                            $('#tipoDocumentoVentaReferencia').focus();
                        }
                    }
                });
                return false;
            }

            if ($("#notaIngreso_numeroDocumentoVentaReferencia").val() > 0 && $('#notaIngreso_numeroDocumentoVentaReferencia') == "") {
                $.alert({
                    //icon: 'fa fa-warning',
                    title: 'Validación',
                    content: 'Debe ingresar la serie del documento de venta de referencia.',
                    type: 'red',
                    buttons: {
                        OK: function () {
                            $('#notaIngreso_serieDocumentoVentaReferencia').focus();
                        }
                    }
                });
                return false;
            }
        }
        else if ($("#notaIngreso_sustentoExtorno").length > 0) {
            if ($("#notaIngreso_sustentoExtorno").val().length <= 25) {
                $.alert({
                    //icon: 'fa fa-warning',
                    title: 'Validación',
                    content: 'El sustento de extorno debe tener más de 25 caracteres.',
                    type: 'red',
                    buttons: {
                        OK: function () {
                            $('#notaIngreso_sustentoExtorno').focus();
                        }
                    }
                });
                
                return false;
            }

        }
       
     
        /*
        var contador = 0;
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {

            if (Number(value.innerHTML) > 0)  
                contador++;
        });

        if (contador > 10) {
            alert('La nota de ingreso debe contener 10 productos como máximo, por favor marque el check de "Atención Parcial", indique que NO es la última atención y modifique la "Cantidad por Atender" a 0 a algunos productos (los productos que tengan 0 no se considerarán en la guía).');
            return false;
        }
        */

        return true;
    }
      




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


    $(document).on('click', "input.chkNoEntregado", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idMovimientoAlmacen = arrrayClass[0];
        var numeroDocumento = arrrayClass[1];

        var noEntregado = 0;
        if (event.target.checked) {
            noEntregado = 1;
        }


        $.ajax({
            url: "/NotaIngreso/UpdateMarcaNoEntregado",
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
    $(document).on('click', "button.btnVerNotaIngreso", function () {
        
        activarBotonesVer();
        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idMovimientoAlmacen = arrrayClass[0];
        var numeroDocumento = arrrayClass[1];     

        showMovimientoAlmacen(idMovimientoAlmacen);
    });

    function showMovimientoAlmacen(idMovimientoAlmacen) {
        $.ajax({
            url: "/NotaIngreso/Show",
            data: {
                idMovimientoAlmacen: idMovimientoAlmacen
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                alert(MENSAJE_ERROR);
            },
            success: function (resultado) {



                $("#comentarioAnulado").val("");
                //var cotizacion = $.parseJSON(respuesta);
                var notaIngreso = resultado.notaIngreso;
                var usuario = resultado.usuario;



                $("#idPedido").val(notaIngreso.pedido.idPedido);
                $("#idMovimientoAlmacen").val(notaIngreso.idMovimientoAlmacen);
                $("#ver_notaIngreso_ciudadDestino_nombre").html(notaIngreso.ciudadDestino.nombre);
                $("#ver_notaIngreso_ciudadDestino_direccionPuntoLlegada").html(notaIngreso.ciudadDestino.direccionPuntoLlegada);

                $("#ver_notaIngreso_fechaTraslado").html(invertirFormatoFecha(notaIngreso.fechaTraslado.substr(0, 10)));
                $("#ver_notaIngreso_fechaEmision").html(invertirFormatoFecha(notaIngreso.fechaEmision.substr(0, 10)));
                $("#ver_notaIngreso_serieNumeroDocumento").html(notaIngreso.serieNumeroNotaIngreso);
                /*
                $("#facturarver_notaIngreso_fechaTraslado").html(invertirFormatoFecha(notaIngreso.fechaTraslado.substr(0, 10)));
                $("#facturarver_notaIngreso_fechaEmision").html(invertirFormatoFecha(notaIngreso.fechaEmision.substr(0, 10)));
                $("#facturarver_notaIngreso_serieNumeroDocumento").html(notaIngreso.serieNumeroGuia);*/


                //   $("#ver_notaIngreso_numeroDocumento").html(guiaRemision.numeroDocumentoString);
                $("#ver_notaIngreso_pedido_numeroPedido").html(notaIngreso.pedido.numeroPedidoString);
                $("#ver_notaIngreso_pedido_cliente").html(notaIngreso.pedido.cliente.razonSocial);
                $("#ver_notaIngreso_pedido_numeroReferenciaCliente").html(notaIngreso.pedido.numeroReferenciaCliente);
                $("#ver_notaIngreso_motivoTraslado").html(notaIngreso.motivoTrasladoString);
                $("#ver_notaIngreso_atencionParcial").html(notaIngreso.atencionParcial);

                /*
                $("#ver_notaIngreso_pedido_ubigeoEntrega").html(notaIngreso.pedido.ubigeoEntrega.ToString);
                $("#ver_notaIngreso_pedido_direccionEntrega").html(notaIngreso.pedido.direccionEntrega.descripcion);
                $("#ver_notaIngreso_transportista_descripcion").html(notaIngreso.transportista.descripcion);
                $("#ver_notaIngreso_transportista_ruc").html(notaIngreso.transportista.ruc);
                $("#ver_notaIngreso_transportista_brevete").html(notaIngreso.transportista.brevete);
                $("#ver_notaIngreso_transportista_direccion").html(notaIngreso.transportista.direccion);
                $("#ver_notaIngreso_placaVehiculo").html(notaIngreso.placaVehiculo);*/
                //$("#ver_notaIngreso_certificadoInscripcion").html(guiaRemision.certificadoInscripcion);
                $("#ver_notaIngreso_observaciones").html(notaIngreso.observaciones);

                $("#ver_notaIngreso_estadoDescripcion").html(notaIngreso.estadoDescripcion);

                if (notaIngreso.estaAnulado == 1) {
                    $("#ver_notaIngreso_estadoDescripcion").attr("style", "color:red")
                    $("#btnAnularNotaIngreso").hide();
                    $("#btnExtornar").hide();
                    $("#btnImprimirNotaIngreso").hide();
                }
                else {
                    $("#ver_notaIngreso_estadoDescripcion").attr("style", "color:black")
                    $("#btnImprimirNotaIngreso").show();

                    //No se requiere agregar este campo en nota de crédito
                    //$("#notaIngreso_tipoExtorno").val(notaIngreso.tipoExtorno);
                    /*Si la nota de ingreso no ha sido extornada, se puede anular, extornar y facturar*/
                    if (notaIngreso.tipoExtorno == MOV_TIPO_EXTORNO_SIN_EXTORNO) {
                        $("#btnAnularNotaIngreso").show();
                        $("#btnExtornar").show();

                        $("#fieldsetTipoExtorno").hide();
                        $("#btnVerGuiasRemisionExtornantes").hide();
                    }
                    /*Si la nota de ingreso ha sido extornada parcialmente, se puede extornar para culminar con el extorno, no se puede anular y tampoco facturar*/
                    else {
                        $("#btnAnularNotaIngreso").hide();
                        $("#btnFacturarGuiaRemision").hide();

                        $("#fieldsetTipoExtorno").show();
                        $("#btnVerGuiasRemisionExtornantes").show();
                        $("#ver_notaIngreso_tipoExtorno").html(notaIngreso.tipoExtornoToString);
                        if (notaIngreso.tipoExtorno == MOV_TIPO_EXTORNO_EXTORNO_PARCIAL) {
                            $("#btnExtornar").show();
                        }
                        else {
                            $("#btnExtornar").hide();
                        }
                    }


                    if (notaIngreso.guiaRemisionAExtornar == null && notaIngreso.guiaRemisionAIngresar == null) {
                        $("#fieldsetDocumentoExtornado").hide();
                        $("#fieldsetDetalleExtorno").hide();
                        $("#fieldsetDocumentoIngresado").hide();
                        $("#fieldsetDocumentosReferencia").show();

                        $("#ver_notaIngreso_serieGuiaReferencia").html(notaIngreso.serieGuiaReferencia);
                        $("#ver_notaIngreso_numeroGuiaReferencia").html(notaIngreso.numeroGuiaReferencia);
                        $("#ver_notaIngreso_serieDocumentoVentaReferencia").html(notaIngreso.serieDocumentoVentaReferencia);
                        $("#ver_notaIngreso_numeroDocumentoVentaReferencia").html(notaIngreso.numeroDocumentoVentaReferencia);
                        $("#ver_notaIngreso_tipoDocumentoVentaReferencia").html(notaIngreso.tipoDocumentoVentaReferenciaString);

                    }
                    else if (notaIngreso.guiaRemisionAIngresar != null) {
                        $("#fieldsetDocumentoExtornado").hide();
                        $("#fieldsetDetalleExtorno").hide();
                        $("#fieldsetDocumentoIngresado").show();
                        $("#fieldsetDocumentosReferencia").hide();
                        $("#ver_notaIngreso_guiaRemisionAIngresar_serieNumeroGuia").html(notaIngreso.guiaRemisionAIngresar.serieNumeroGuia);
                        $("#btnExtornar").hide();
                    }
                    else
                    {
                        $("#fieldsetDocumentoExtornado").show();
                        $("#fieldsetDetalleExtorno").show();
                        $("#fieldsetDocumentosReferencia").hide();        
                        $("#fieldsetDocumentoIngresado").hide();
                        $("#ver_notaIngreso_guiaRemisionAExtornar_serieNumeroGuia").html(notaIngreso.guiaRemisionAExtornar.serieNumeroGuia);
                        $("#ver_notaIngreso_motivoExtornoGuiaRemisionToString").html(notaIngreso.motivoExtornoGuiaRemisionToString);
                        $("#ver_notaIngreso_sustentoExtorno").html(notaIngreso.sustentoExtorno);
                        $("#btnExtornar").hide();
                    }





                    /*Tiene Nota de Crédito*/
                    if (notaIngreso.estaFacturado == 1) {
                        $("#ver_notaIngreso_estadoDescripcion").attr("style", "color:green")
                        $("#btnAnularNotaIngreso").hide();
                    }
                    
                }




                

                if (notaIngreso.atencionParcial) {
                    $("#ver_notaIngreso_atencionParcial").html("Ingreso Parcial");
                }
                else {
                    $("#ver_notaIngreso_atencionParcial").html("Ingreso Final");
                }

              
                


                //invertirFormatoFecha(pedido.fechaMaximaEntrega.substr(0, 10)));

                $("#tableDetalleGuia > tbody").empty();

                FooTable.init('#tableDetalleGuia');


                var d = '';
                var lista = notaIngreso.documentoDetalle;
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


                /*      if (pedido.seguimientoPedido.estado == ESTADO_EN_EDICION) {
                          $("#btnEditarPedido").html("Continuar Editanto");
                      }
                      else
                      {
                          $("#btnEditarPedido").html("Editar");
                      }
            
                  */



                $("#modalVerNotaIngreso").modal('show');

                //  window.location = '/Pedido/Index';
            }
        });
    }





    


    

    $("#btnCancelarNotaIngreso").click(function () {
        if (confirm(MENSAJE_CANCELAR_EDICION)) {
            window.location = '/NotaIngreso/CancelarCreacionNotaIngreso';
        }
    })


    $("#btnVerGuiasRemisionExtornantes").click(function () {
        var idMovimientoAlmacen = $("#idMovimientoAlmacen").val();
        $.ajax({
            url: "/NotaIngreso/GetMovimientosAlmacenExtornantes",
            data: {
                idMovimientoAlmacen: idMovimientoAlmacen
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                mostrarMensajeErrorProceso();
            },
            success: function (guiaRemisionList) {
                $("#formVerGuiasRemision").empty();

                for (var j = 0; j < guiaRemisionList.length; j++) {
                    $("#tableDetalleGuiaRemision > tbody").empty();
                    var plantilla = $("#plantillaVerGuiasRemision").html();
                    var dGuia = '';
                    var documentoDetalleList = guiaRemisionList[j].documentoDetalle;
                    for (var k = 0; k < documentoDetalleList.length; k++) {

                        dGuia += '<tr>' +
                            '<td>' + documentoDetalleList[k].producto.sku + '</td>' +
                            '<td>' + documentoDetalleList[k].cantidad + '</td>' +
                            '<td>' + documentoDetalleList[k].unidad + '</td>' +
                            '<td>' + documentoDetalleList[k].producto.descripcion + '</td>' +
                            '</tr>';
                    }

                    $("#tableDetalleGuiaRemision").append(dGuia);

                    plantilla = $("#plantillaVerGuiaRemision").html();

                    plantilla = plantilla.replace("#serieNumero", guiaRemisionList[j].serieDocumento + '-' + guiaRemisionList[j].numeroDocumento);
                    plantilla = plantilla.replace("#fechaEmisionGuiaRemision", invertirFormatoFecha(guiaRemisionList[j].fechaEmision.substr(0, 10)));
                    /*
                    plantilla = plantilla.replace("#serieNumeroFactura", guiaRemisionList[j].venta.documentoVenta.serieNumero);
                    if (guiaRemisionList[j].venta.documentoVenta.fechaEmision != null) {
                        plantilla = plantilla.replace("#fechaEmisionFactura", invertirFormatoFecha(guiaRemisionList[j].venta.documentoVenta.fechaEmision.substr(0, 10)));
                    }
                    else
                        plantilla = plantilla.replace("#fechaEmisionFactura", "");
                    */

                    plantilla = plantilla.replace("tableDetalleGuia", "tableDetalleGuia" + j);

                    $("#formVerGuiasRemision").append(plantilla);
                }

                $("#modalVerGuiasRemision").modal('show');

            }
        });

    });




    function desactivarBotonesVer()
    {
      /*  $("#btnCancelarCotizacion").attr('disabled', 'disabled');
        $("#btnEditarCotizacion").attr('disabled', 'disabled');
        $("#btnReCotizacion").attr('disabled', 'disabled');
        $("#btnAprobarCotizacion").attr('disabled', 'disabled');
        $("#btnDenegarCotizacion").attr('disabled', 'disabled');
        $("#btnAceptarCotizacion").attr('disabled', 'disabled');
        $("#btnRechazarCotizacion").attr('disabled', 'disabled');
        $("#btnPDFCotizacion").attr('disabled', 'disabled');*/
    }

    function activarBotonesVer() {
  /*      $("#btnCancelarCotizacion").removeAttr('disabled');
        $("#btnEditarCotizacion").removeAttr('disabled');
        $("#btnReCotizacion").removeAttr('disabled');
        $("#btnAprobarCotizacion").removeAttr('disabled');
        $("#btnDenegarCotizacion").removeAttr('disabled');
        $("#btnAceptarCotizacion").removeAttr('disabled');
        $("#btnRechazarCotizacion").removeAttr('disabled');
        $("#btnPDFCotizacion").removeAttr('disabled');*/
    }

   


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
                //var cotizacion = $.parseJSON(respuesta);

                var venta = resultado.venta;
                var pedido = resultado.venta.pedido;
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

                $("#verEstado").html(pedido.seguimientoPedido.estadoString);
                $("#verModificadoPor").html(pedido.seguimientoPedido.usuario.nombre);
                $("#verObservacionEstado").html(pedido.seguimientoPedido.observacion);

                $("#verEstadoCrediticio").html(pedido.seguimientoCrediticioPedido.estadoString);
                $("#verModificadoCrediticioPor").html(pedido.seguimientoCrediticioPedido.usuario.nombre);
                $("#verObservacionEstadoCreiditicio").html(pedido.seguimientoCrediticioPedido.observacion);


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



                




                //  
                // sleep
                $("#tableDetallePedido").append(d);


                $("#modalFacturar").modal('show');

                //  window.location = '/Pedido/Index';
            }
        });


    })


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




    function addZero(i) {
        if (i < 10) {
            i = "0" + i;
        }
        return i;
    }

    function getHoraActual() {
        var d = new Date();
        var h = addZero(d.getHours());
        var m = addZero(d.getMinutes());
        var s = addZero(d.getSeconds());
        return h + ":" + m;
    }




    $('#documentoVenta_fechaEmision').change(function () {
        var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate');
        $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
        calcularFechaVencimiento();

    });

    function calcularFechaVencimiento() {
        var tipoPago = $('#tipoPago').val();
        $('#documentoVenta_fechaVencimiento').attr("disabled","disabled");
        switch (tipoPago) {
            case "0":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate');
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                $('#documentoVenta_fechaVencimiento').removeAttr("disabled");
                break;
            case "1":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate');
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "2":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+7d');
                date2.setDate(date2.getDate() + 7);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "3":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+15d');
                date2.setDate(date2.getDate() + 15);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "4":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+30d');
                date2.setDate(date2.getDate() + 30);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "5":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+60d');
                date2.setDate(date2.getDate() + 60);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "6":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+90d');
                date2.setDate(date2.getDate() + 90);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "7":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+120d');
                date2.setDate(date2.getDate() + 120);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "8":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+20d');
                date2.setDate(date2.getDate() + 20);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "9":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+45d');
                date2.setDate(date2.getDate() + 45);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "10":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+45d');
                date2.setDate(date2.getDate() + 21);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "11":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+45d');
                date2.setDate(date2.getDate() + 25);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;




        }
    }


    $('#tipoPago').change(function () {

        calcularFechaVencimiento();


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


    $("#btnAceptarFacturarPedido").click(function () {
        if ($("#verRazonSocialSunat").html() == "") {
            alert("No se han obtenido los datos del cliente desde SUNAT.");
            return false;
        }

        if ($("#documentoVenta_fechaEmision").val() == "" || $("#documentoVenta_fechaEmision").val() == null) {
            alert("Debe ingresar la fecha de emisión.");
            $("#documentoVenta_fechaEmision").focus();
            return false;
        }

        if ($("#documentoVenta_horaEmision").val() == "" || $("#documentoVenta_horaEmision").val() == null) {
            alert("Debe ingresar la hora de emisión.");
            $("#documentoVenta_horaEmision").focus();
            return false;
        }

        if ($("#documentoVenta_fechaVencimiento").val() == "" || $("#documentoVenta_fechaVencimiento").val() == null) {
            alert("Debe ingresar la fecha de vencimiento.");
            $("#documentoVenta_fechaVencimiento").focus();
            return false;
        }

        if (convertirFechaNumero($("#documentoVenta_fechaEmision").val()) > convertirFechaNumero($("#documentoVenta_fechaVencimiento").val())) {
            alert("La fecha de vencimiento debe ser mayor o igual a la fecha de emisión.");
            $("#documentoVenta_fechaVencimiento").focus();
            return false;
        }


     if ($("#verNumeroReferenciaCliente").html().length > 20) {
            $("#numeroReferenciaCliente").focus();
            $.alert({
                title: 'Validación',
                content: 'El número de referencia del cliente no debe contener más de 20 caracteres, si el dato a ingresar es más extenso agreguelo en observaciones.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }  

        var serie = $("#documentoVenta_serie").val();
        var fechaEmision = $("#documentoVenta_fechaEmision").val();
        var horaEmision = $("#documentoVenta_horaEmision").val();
        var fechaVencimiento = $("#documentoVenta_fechaVencimiento").val();
        var observaciones = $("#documentoVenta_observaciones").val();
        var tipoPago = $("#tipoPago").val();
        var formaPago = $("#formaPago").val();
        var numeroReferenciaCliente = $("#verNumeroReferenciaCliente").html(); 

       

        desactivarBotonesFacturar();
        

        $('body').loadingModal({
            text: 'Creando Factura...'
        });



        var idMovimientoAlmacen = $("#idMovimientoAlmacen").val();


        $.ajax({
            url: "/Factura/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                fechaEmision: fechaEmision,
                horaEmision: horaEmision,
                fechaVencimiento: fechaVencimiento,
                idMovimientoAlmacen: idMovimientoAlmacen,
                tipoPago: tipoPago,
                formaPago: formaPago,
                observaciones: observaciones,
                serie: serie,
                numeroReferenciaCliente: numeroReferenciaCliente
            },
            error: function (resultado) {
                $('body').loadingModal('hide')
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
                activarBotonesFacturar();
            },
            success: function (documentoVenta) {
                $('body').loadingModal('hide')

                if (documentoVenta.cPE_RESPUESTA_BE.CODIGO == "001") {
                    $("#idDocumentoVenta").val(documentoVenta.idDocumentoVenta);
                    /*FECHA HORA EMISIÓN -  SERIE CORRELATIVO*/
                    $("#vpFEC_EMI_HOR_EMI").html(documentoVenta.cPE_CABECERA_BE.FEC_EMI + ' ' + documentoVenta.cPE_CABECERA_BE.HOR_EMI) 
                    $("#vpSERIE_CORRELATIVO").html(documentoVenta.cPE_CABECERA_BE.SERIE + ' ' + documentoVenta.cPE_CABECERA_BE.CORRELATIVO);

                    /*NOMBRE COMERCIAL CLIENTE*/
                    $("#vpNOM_RCT").html(documentoVenta.cPE_CABECERA_BE.NOM_RCT);

                    /*DIRECCION - ORDEN DE COMPRA*/
                    $("#vpDIR_DES_RCT").html(documentoVenta.cPE_CABECERA_BE.DIR_DES_RCT);
                    $("#vpNRO_ORD_COM").html(documentoVenta.cPE_CABECERA_BE.NRO_ORD_COM);

                    /*RUC - NRO GUIA*/
                    $("#vpNRO_DOC_RCT").html(documentoVenta.cPE_CABECERA_BE.NRO_DOC_RCT);
                    $("#vpNRO_GRE").html(documentoVenta.cPE_CABECERA_BE.NRO_GRE);

                    /*OBSERVACIONES*/ /*CODIGO CLIENTE*/
                    $("#vpOBSERVACIONES").html($("#documentoVenta_observaciones").val());                    
                    $("#vpCODIGO_CLIENTE").html(documentoVenta.cliente.codigo);


                    $("#vpCOND_PAGO").html(documentoVenta.tipoPagoString);
                    $("#vpFEC_VCTO").html(documentoVenta.cPE_CABECERA_BE.FEC_VCTO);
                    

                    $("#vpMNT_TOT_GRV").html(documentoVenta.cPE_CABECERA_BE.MNT_TOT_GRV);
                    $("#vpMNT_TOT_GRV_NAC").html(documentoVenta.cPE_CABECERA_BE.MNT_TOT_GRV_NAC);
                    $("#vpMNT_TOT_INF").html(documentoVenta.cPE_CABECERA_BE.MNT_TOT_INF);
                    $("#vpMNT_TOT_EXR").html(documentoVenta.cPE_CABECERA_BE.MNT_TOT_EXR);
                    $("#vpMNT_TOT_GRT").html(documentoVenta.cPE_CABECERA_BE.MNT_TOT_GRT);
                    $("#vpMNT_TOT_VAL_VTA").html(documentoVenta.cPE_CABECERA_BE.MNT_TOT_VAL_VTA);
                    $("#vpMNT_TOT_TRB_IGV").html(documentoVenta.cPE_CABECERA_BE.MNT_TOT_TRB_IGV);
                    $("#pvMNT_TOT_PRC_VTA").html(documentoVenta.cPE_CABECERA_BE.MNT_TOT_PRC_VTA);
                   

                    $("#modalVistaPreviaFactura").modal();


                    $("#tableDetalleFacturaVistaPrevia > tbody").empty();
                    //FooTable.init('#tableCotizaciones');
                    $("#tableDetalleFacturaVistaPrevia").footable();
                    var lineasFactura = documentoVenta.cPE_DETALLE_BEList;

                    for (var i = 0; i < lineasFactura.length; i++) {

                        var lineaFactura = "";

                     /*   var styleEstado = "";
                        if (guiaRemisionList[i].estaAnulado == 1) {
                            styleEstado = "style='color: red'";
                        }
                        else if (guiaRemisionList[i].estaFacturado == 1) {
                            styleEstado = "style='color: green'";
                        }
                        else {
                            styleEstado = "style='color: black'";
                        }*/

                        var lineaFactura = '<tr data-expanded="false">' +
                            '<td>  ' + lineasFactura[i].LIN_ITM + '</td>' +
                            '<td>  ' + lineasFactura[i].COD_ITM + '</td>' +
                            '<td>  ' + lineasFactura[i].CANT_UND_ITM + '</td>' +
                            '<td>  ' + lineasFactura[i].COD_UND_ITM + '</td>' +
                            '<td>  ' + lineasFactura[i].TXT_DES_ITM + '</td>' +
                            '<td>  ' + lineasFactura[i].VAL_UNIT_ITM + '</td>' +
                            '<td>  ' + lineasFactura[i].VAL_VTA_ITM + '</td>' +
                            '<td class="atenuarDetalleFactura">  ' + lineasFactura[i].MNT_IGV_ITM + '</td>' +
                            '<td class="atenuarDetalleFactura">  ' + lineasFactura[i].PRC_VTA_UND_ITM + '</td>' +
                            '<td class="atenuarDetalleFactura">  ' + lineasFactura[i].PRC_VTA_ITEM + '</td>' +
                            '<td class="atenuarDetalleFactura">  ' + lineasFactura[i].POR_IGV_ITM + '</td>' +
                            '<td class="atenuarDetalleFactura">  ' + lineasFactura[i].COD_TIP_AFECT_IGV_ITM + '</td>' +
                            '</tr>';

                        $("#tableDetalleFacturaVistaPrevia").append(lineaFactura);
                    }
                        
                    //documentoVenta.CPE_CABECERA_BE
                    
                        
                  
                    activarBotonesFacturar();
                }
                else {
                    mostrarMensajeErrorProceso(MENSAJE_ERROR + ".\n" + "Detalle Error: " + documentoVenta.cPE_RESPUESTA_BE.DETALLE);
                    //$("#btnAceptarFacturarPedido").removeAttr("disabled");
                    activarBotonesFacturar();
                }





                
            }
        });
        $("btnCancelarFacturarPedido").click();
    });


    $("#btnConfirmarFacturarPedido").click(function () {

        
        if ($("#verRazonSocialSunat").html() == "") {
            alert("No se han obtenido los datos del cliente desde SUNAT.");
            return false;
        }

        if ($("#documentoVenta_fechaEmision").val() == "" || $("#documentoVenta_fechaEmision").val() == null) {
            alert("Debe ingresar la fecha de emisión.");
            $("#documentoVenta_fechaEmision").focus();
            return false;
        }

        if ($("#documentoVenta_horaEmision").val() == "" || $("#documentoVenta_horaEmision").val() == null) {
            alert("Debe ingresar la hora de emisión.");
            $("#documentoVenta_horaEmision").focus();
            return false;
        }

        if ($("#documentoVenta_fechaVencimiento").val() == "" || $("#documentoVenta_fechaVencimiento").val() == null) {
            alert("Debe ingresar la fecha de vencimiento.");
            $("#documentoVenta_fechaVencimiento").focus();
            return false;
        }

        if (convertirFechaNumero($("#documentoVenta_fechaEmision").val()) > convertirFechaNumero($("#documentoVenta_fechaVencimiento").val())) {
            alert("La fecha de vencimiento debe ser mayor o igual a la fecha de emisión.");
            $("#documentoVenta_fechaVencimiento").focus();
            return false;
        }



        var serie = $("#documentoVenta_serie").val();
        var fechaEmision = $("#documentoVenta_fechaEmision").val();
        var horaEmision = $("#documentoVenta_horaEmision").val();
        var fechaVencimiento = $("#documentoVenta_fechaVencimiento").val();
        var observaciones = $("#documentoVenta_observaciones").val();
        var tipoPago = $("#tipoPago").val();
        var formaPago = $("#formaPago").val();



        desactivarBotonesFacturar();


        $('body').loadingModal({
            text: 'Creando Factura...'
        });


        var idMovimientoAlmacen = $("#idMovimientoAlmacen").val();
        var idDocumentoVenta = $("#idDocumentoVenta").val();

        $.ajax({
            url: "/Factura/ConfirmarCreacion",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idDocumentoVenta: idDocumentoVenta
            },
            error: function (resultado) {
                $('body').loadingModal('hide')
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
                activarBotonesFacturar();
            },
            success: function (resultado) {
                $('body').loadingModal('hide')

                if (resultado.CPE_RESPUESTA_BE.CODIGO == "001") {
                    $.alert({
                        //icon: 'fa fa-warning',
                        title: TITLE_EXITO,
                        content: 'Se generó la factura ' + resultado.serieNumero + '.',
                        type: 'green',
                        buttons: {
                            OK: function () { location.reload(); }
                        }
                    });
                    activarBotonesFacturar();
                }
                else {
                    mostrarMensajeErrorProceso(MENSAJE_ERROR + ".\n" + "Detalle Error: " + resultado.CPE_RESPUESTA_BE.DETALLE);
                    //$("#btnAceptarFacturarPedido").removeAttr("disabled");
                    activarBotonesFacturar();
                }

           }
        });
        $("btnCancelarFacturarPedido").click();
    });


    $("#btnAceptarAnulacion").click(function () {

        var comentarioAnulado = $("#comentarioAnulado").val();
        $("#btnAceptarAnulacion").attr("disabled","disabled");
        $.ajax({
            url: "/NotaIngreso/Anular",
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
                alert("La nota de ingreso número " + resultado.serieNumeroNotaIngreso + " fue anulada correctamente.");
                window.location = '/NotaIngreso/Index';
            }
        });

       // window.open("NotaIngreso/Print");
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
        if ($('#notaIngreso_atencionParcial').length) {

            if (!$('#notaIngreso_atencionParcial').prop('checked')) {
                return false;
            }

        }
       




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
            var cantidad = value.innerText;
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
            url: "/NotaIngreso/ChangeDetalle",
            type: 'POST',
            data: json,
            dataType: 'json',
            contentType: 'application/json',
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
    EVENTOS BUSQUEDA GUIAS FACTURA CONSOLIDADA
    #####################################################*/


    $("#btnBusquedaGuiasFacturaConsolidada").click(function () {

        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val();
        var fechaTrasladoDesde = $("#notaIngreso_fechaTrasladoDesde").val();
        var fechaTrasladoHasta = $("#notaIngreso_fechaTrasladoHasta").val();


        $.ajax({
            url: "/NotaIngreso/SearchGuiasRemisionGrupoCliente",
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
                        '<td><input class="' + guiaRemisionList[i].idMovimientoAlmacen + ' ' + guiaRemisionList[i].serieNumeroGuia +'" name="chkMovimientoAlmacen" type="checkbox" checked>' + '</td>' +
                        '<td>  ' + guiaRemisionList[i].serieNumeroGuia + '</td>' +
                        '<td>  ' + guiaRemisionList[i].pedido.numeroPedidoString + '</td>' +
                        '<td>  ' + guiaRemisionList[i].usuario.nombre + '</td>' +
                        '<td>  ' + invertirFormatoFecha(guiaRemisionList[i].fechaEmision.substr(0, 10)) + '</td>' +
                        '<td>  ' + invertirFormatoFecha(guiaRemisionList[i].fechaTraslado.substr(0, 10)) + '</td>' +
                        '<td>  ' + guiaRemisionList[i].pedido.cliente.razonSocial + '</td>' +
                        '<td>  ' + guiaRemisionList[i].pedido.cliente.ruc + '</td>' +
                        '<td>  ' + guiaRemisionList[i].ciudadDestino.nombre + '</td>' +
                        '<td ' + style + '>  ' + guiaRemisionList[i].estadoDescripcion + '</td>' +
                        '<td> <button type="button" class="' + guiaRemisionList[i].idMovimientoAlmacen + ' ' + guiaRemisionList[i].numeroDocumento + ' btnVerNotaIngreso btn btn-primary ">Ver</button></td > ' +
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
            url: "/NotaIngreso/CleanBusquedaFacturaConsolidada",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });


    $("#btnAceptarGenerarVenta").click(function () {


        $("#btnAceptarGenerarVenta").attr("disabled", "disabled");
        var miWindow;
        //Se debe crear la venta Consolidada y abrir la pantalla de edición
        $.ajax({
            url: "/NotaIngreso/generarVentaConsolidada",
            type: 'POST',
            data: json,
            dataType: 'json',
            contentType: 'application/json',
            success: function (respuesta) {

                $("#btnAceptarGenerarVenta").removeAttr("disabled");

                miWindow = window.open(
                    "/Venta/VenderConsolidado",
                    "Edición de Venta Consolidad",
                    "resizable,scrollbars,status"
                );

               

                        


            }
        });

    });
    






    $("#btnConsolidarAtenciones").click(function () {

        // $(document).on('click', "button.btnVerNotaIngreso", function () {});


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
            alert("Debe seleccionar al menos una nota de ingreso.");
            return false;
        }


        json = json.substr(0, json.length - 1) + ']';

        

//        $("#ver_notaIngreso_list").html(ver_notaIngreso_list);


        $.ajax({
            url: "/NotaIngreso/consolidarAtenciones",
            type: 'POST',
            data: json,
            dataType: 'json',
            contentType: 'application/json',
            success: function (respuesta) {

                var ventaDetalleList = respuesta.ventaDetalleList;

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
                        '<td>  ' + ventaDetalleList[i].producto.unidad_alternativa + '</td>' +
                        '<td>  ' + ventaDetalleList[i].sumCantidadUnidadAlternativa + '</td>' +
                        '<td>  ' + ventaDetalleList[i].producto.unidad + '</td>' +
                        '<td>  ' + ventaDetalleList[i].sumCantidadUnidadEstandar + '</td>' +

                /*        '<td>  ' + ventaDetalleList[i].producto.equivalencia + '</td>' +
                        '<td>  ' + ventaDetalleList[i].esPrecioAlternativo + '</td>' +*/

                        '<td>  ' + ventaDetalleList[i].sumPrecioNeto + '</td>' +
                        '<td>  ' + ventaDetalleList[i].sumPrecioUnitario + '</td>' +                        
                        '</tr>';                 

                    $("#tableVentaConsolidada").append(ventaDetalle);
                }

            }
        });

    });





    /*####################################################
    EVENTOS BUSQUEDA GUIAS
    #####################################################*/



    $("#btnBusqueda").click(function () {
        //sede MP
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val(); 

        var numeroDocumento = $("#notaIngreso_numeroDocumento").val();
        var numeroPedido = $("#notaIngreso_pedido_numeroPedido").val();
        var numeroGuiaReferencia = $("#notaIngreso_numeroGuiaReferencia").val();
        var fechaTrasladoDesde = $("#notaIngreso_fechaTrasladoDesde").val();
        var fechaTrasladoHasta = $("#notaIngreso_fechaTrasladoHasta").val();
        //var estado = $("#estado").val();

        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/NotaIngreso/Search",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                numeroDocumento: numeroDocumento,
                numeroPedido: numeroPedido,
                numeroGuiaReferencia: numeroGuiaReferencia,
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
                    else if (guiaRemisionList[i].estaFacturado == 1){
                        styleEstado = "style='color: green'";
                    }
                    else
                    {
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
                    
                     var guiaRemision = '<tr data-expanded="false">'+
                         '<td>  ' + guiaRemisionList[i].idMovimientoAlmacen + '</td>' +
                         '<td>  ' + guiaRemisionList[i].serieNumeroNotaIngreso + '</td>' +
                         '<td>  ' + guiaRemisionList[i].pedido.numeroPedidoString + '</td>' +
                         '<td>  ' + guiaRemisionList[i].motivoTrasladoString + '</td>' +
                         '<td>  ' + guiaRemisionList[i].usuario.nombre + '</td>' +
                         '<td>  ' + invertirFormatoFecha(guiaRemisionList[i].fechaEmision.substr(0, 10)) + '</td>' +
                         '<td>  ' + invertirFormatoFecha(guiaRemisionList[i].fechaTraslado.substr(0, 10)) + '</td>' +
                         '<td>  ' + guiaRemisionList[i].pedido.cliente.razonSocial + '</td>' +
                         '<td>  ' + guiaRemisionList[i].pedido.cliente.ruc + '</td>' +
                         '<td>  ' + guiaRemisionList[i].ciudadDestino.nombre + '</td>' +
                         '<td ' + styleEstado + '>  ' + guiaRemisionList[i].estadoDescripcion + '</td>' +
                         '<td>' + guiaRemisionList[i].tipoExtornoToString + '</td>' +
                         '<td>' + noEntregado + '</td>' +
                         '<td>' + noEntregadoLectura + '</td>' +
                         '<td> <button type="button" class="' + guiaRemisionList[i].idMovimientoAlmacen + ' ' + guiaRemisionList[i].numeroDocumento + ' btnVerNotaIngreso btn btn-primary ">Ver</button></td > ' +
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






    $("input[name=notaIngreso_estaAnulado]").on("click", function () {
        var estaAnulado = $("input[name=notaIngreso_estaAnulado]:checked").val();
        $.ajax({
            url: "/NotaIngreso/ChangeEstaAnulado",
            type: 'POST',
            data: {
                estaAnulado: estaAnulado
            },
            success: function () {
            }
        });
    });

    $("input[name=notaIngreso_estaFacturado]").on("click", function () {
        var estaFacturado = $("input[name=notaIngreso_estaFacturado]:checked").val();
        $.ajax({
            url: "/NotaIngreso/ChangeEstaFacturado",
            type: 'POST',
            data: {
                estaFacturado: estaFacturado
            },
            success: function () {
            }
        });
    });


    $("#notaIngreso_fechaTraslado").change(function () {
        var fechaTraslado = $("#notaIngreso_fechaTraslado").val();
        $.ajax({
            url: "/NotaIngreso/ChangefechaTraslado",
            type: 'POST',
            data: {
                fechaTraslado: fechaTraslado
            },
            success: function () {
            }
        });
    });

    $("#notaIngreso_fechaEmision").change(function () {
        var fechaEmision = $("#notaIngreso_fechaEmision").val();
        $.ajax({
            url: "/NotaIngreso/ChangefechaEmision",
            type: 'POST',
            data: {
                fechaEmision: fechaEmision
            },
            success: function () {
            }
        });
    });



    $("#notaIngreso_fechaTrasladoDesde").change(function () {
        var fechaTrasladoDesde = $("#notaIngreso_fechaTrasladoDesde").val();
        $.ajax({
            url: "/NotaIngreso/ChangefechaTrasladoDesde",
            type: 'POST',
            data: {
                fechaTrasladoDesde: fechaTrasladoDesde
            },
            success: function () {
            }
        });
    });

    $("#notaIngreso_fechaTrasladoHasta").change(function () {
        var fechaTrasladoHasta = $("#notaIngreso_fechaTrasladoHasta").val();
        $.ajax({
            url: "/NotaIngreso/ChangefechaTrasladoHasta",
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
            url: "/NotaIngreso/ChangeMotivoTraslado",
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
            url: "/NotaIngreso/ChangeIdCiudad",
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

                $("#notaIngreso_ciudadOrigen_direccionPuntoPartida").val(ciudad.direccionPuntoPartida);

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
            url: "/NotaIngreso/ChangeUltimaAtencionParcial",
            type: 'POST',
            data: {
                ultimaAtencionParcial: ultimaAtencionParcial
            },
            success: function () {
                location.reload(); 
            }
        });
    }



    $("#notaIngreso_atencionParcial").change(function () {
        
        var atencionParcial = 1;

        if (!$('#notaIngreso_atencionParcial').prop('checked')) {
            //  $("#descripcionUltimaAtencionParcial").html("");
            atencionParcial = 0;
        //    changeUltimaAtencionParcial(0);
        }
      /*  else {
            changeUltimaAtencionParcial(1);
        }*/

   
        
        var estado = $("#notaIngreso_atencionParcial").val();
        $.ajax({
            url: "/NotaIngreso/ChangeAtencionParcial",
            type: 'POST',
            data: {
                atencionParcial: atencionParcial
            },
            success: function () {
               /* if (!$('#notaIngreso_atencionParcial').prop('checked')) {
                    location.reload();
                }*/
                location.reload();
            }
        });


     /*   
        if ($('#notaIngreso_atencionParcial').prop('checked')) {
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
            url: "/NotaIngreso/CreateTransportistaTemporal",
            type: 'POST',
            dataType: 'JSON',
            data: {
                descripcion: descripcion,
                direccion: direccion,
                ruc: ruc,
                telefono: telefono
            },
            error: function (detalle) { alert("Se generó un error al intentar crear el transportista."); },
            success: function (transportista) {

                $('#notaIngreso_transportista').append($('<option>', {
                    value: transportista.idTransportista,
                    text: transportista.descripcion
                }));
                $('#notaIngreso_transportista').val(transportista.idTransportista);

                $('#notaIngreso_transportista_descripcion').val(transportista.descripcion);
                $('#notaIngreso_transportista_direccion').val(transportista.direccion);
                $('#notaIngreso_transportista_ruc').val(transportista.ruc);
                $('#notaIngreso_transportista_telefono').val(transportista.telefono);
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

            if (value.checked == false ) {
                //$("#checkCabecera").attr("checked", "checked");
                countFalse++;
            }        
        });

        if (countFalse > 0)
            $("#checkCabecera").removeAttr("checked");
        else
            $("#checkCabecera").attr("checked", "checked");
    });







    $("#notaIngreso_serieDocumento").change(function () {

        var serieDocumento = $(this).val();

        $.ajax({
            url: "/NotaIngreso/UpdateSerieDocumento",
            type: 'POST',
       //     dataType: 'JSON',
            data: {
                serieDocumento: serieDocumento
            },
            success: function (siguienteNumeroNotaIngreso) {
                $("#notaIngreso_numeroDocumentoString").val(siguienteNumeroNotaIngreso);
            }
        });

    });

    $("#btnImprimirNotaIngreso").click(function () {
        window.open("/NotaIngreso/Print");
    });





    /*GENERACIÓN DE GUIA REMISIÓN POR DEVOLUCIÓN*/
    $("#btnExtornar").click(function () {

        desactivarBotonesVer();

        //Se obtiene el tipo de nota de ingreso seleccionado
    //    var tipoNotaIngreso = $('input:radio[name=tipoNotaIngreso]:checked').val();
        //Se recupera el id de la guía de remisión
        var idMovimientoAlmacen = $("#idMovimientoAlmacen").val();

     /*   if (tipoNotaIngreso == null) {
            mostrarMensajeErrorProceso("Debe seleccionar el Motivo de la Nota de Ingreso.");
            $("#btnContinuarGenerandoNotaIngreso").removeAttr("disabled");
            $("#btnCancelarNotaIngreso").removeAttr("disabled");
            return false;
        }*/

        var yourWindow;
        $.ajax({
            url: "/GuiaRemision/iniciarAtencionDesdeNotaIngreso",
            type: 'POST',
            // dataType: 'JSON',
            data: {
                idMovimientoAlmacen: idMovimientoAlmacen//,
         //       tipoNotaIngreso: tipoNotaIngreso
            },
            error: function (error) {
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
             //   $("#btnContinuarGenerandoNotaIngreso").removeAttr("disabled");
             //   $("#btnCancelarNotaIngreso").removeAttr("disabled");
            },
            success: function (movimientoAlmacen) {
                window.location = '/GuiaRemision/Guiar';
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




});