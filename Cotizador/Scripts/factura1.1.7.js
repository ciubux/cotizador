
jQuery(function ($) {


    //CONSTANTES POR DEFECTO
    var cantidadDecimales = 2;
    var IGV = 0.18;
    var SIMBOLO_SOL = "S/";
    var MILISEGUNDOS_AUTOGUARDADO = 5000;
    var DESCARGAR_XML = 0;




    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición/creación; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_MENSAJE_BUSQUEDA = "Ingresar datos solicitados";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        obtenerConstantes();
        cargarChosenCliente();
        $("#btnBusqueda").click();
        calcularFechaVencimiento();
    });

    window.onafterprint = function () {
        if ($("#pagina").val() == 14) {
            window.close();
        }
    };


    $("#btnLimpiarBusqueda").click(function () {
        $.ajax({
            url: "/Factura/CleanBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });

    function obtenerConstantes() {
        $.ajax({
            url: "/General/GetConstantes",
            type: 'POST',
            dataType: 'JSON',
            success: function (constantes) {
                IGV = constantes.IGV;
                SIMBOLO_SOL = constantes.SIMBOLO_SOL;
                MILISEGUNDOS_AUTOGUARDADO = constantes.MILISEGUNDOS_AUTOGUARDADO;
                DESCARGAR_XML = constantes.DESCARGAR_XML;
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
            url: "/Factura/SearchClientes"
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
            url: "/Factura/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente) {
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
            url: "/Factura/GetGrupoCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idGrupoCliente: idGrupoCliente
            },
            success: function (cliente) {
            }
        });

    });


    $("#buscarSedesGrupoCliente").change(function () {
        var valor = $("input[name=buscarSedesGrupoCliente]:checked").val();
        $.ajax({
            url: "/Factura/ChangeBuscarSedesGrupoCliente",
            type: 'POST',
            data: {
                buscarSedesGrupoCliente: valor
            },
            success: function () {
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

    var fechaEmisionDesde = $("#fechaEmisionDesdetmp").val();
    $("#documentoVenta_fechaEmisionDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEmisionDesde);

    var fechaEmisionHasta = $("#fechaEmisionHastatmp").val();
    $("#documentoVenta_fechaEmisionHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEmisionHasta);


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


    $("#documentoVenta_fechaEmisionDesde").change(function () {
        var fechaEmisionDesde = $("#documentoVenta_fechaEmisionDesde").val();
        $.ajax({
            url: "/Factura/ChangeFechaEmisionDesde",
            type: 'POST',
            data: {
                fechaEmisionDesde: fechaEmisionDesde
            },
            success: function () {
            }
        });
    });

    $("#documentoVenta_fechaEmisionHasta").change(function () {
        var fechaEmisionHasta = $("#documentoVenta_fechaEmisionHasta").val();
        $.ajax({
            url: "/Factura/ChangeFechaEmisionHasta",
            type: 'POST',
            data: {
                fechaEmisionHasta: fechaEmisionHasta
            },
            success: function () {
            }
        });
    });




    /* ################################## FIN CHANGE CONTROLES */







    ////////VER COTIZACIÓN  --- CAMBIO DE ESTADO

    function invertirFormatoFecha(fecha) {
        var fechaInvertida = fecha.split("-");
        fecha = fechaInvertida[2] + "/" + fechaInvertida[1] + "/" + fechaInvertida[0];
        return fecha
    }

    function convertirFechaNumero(fecha) {
        var fechaInvertida = fecha.split("/");
        fecha = fechaInvertida[2] + fechaInvertida[1] + fechaInvertida[0];
        return Number(fecha)
    }


    /*VER PEDIDO*/



    $("#btnCancelarGuiaRemision").click(function () {
        if (confirm(MENSAJE_CANCELAR_EDICION)) {
            window.location = '/GuiaRemision/CancelarCreacionGuiaRemision';
        }
    })


    $(document).on('click', "button.btnActualizarEstado", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idDocumentoVenta = arrrayClass[0];
        var serieNumero = arrrayClass[1];


        $.ajax({
            url: "/Factura/consultarEstadoDocumentoVenta",
            type: 'POST',
            data: {
                idDocumentoVenta: idDocumentoVenta
            },
            error: function (detalle) {
                mostrarMensajeErrorProceso();
                location.reload();
            },
            success: function () {
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El estado de la factura " + serieNumero + " fue actualizado correctamente.",
                    buttons: {
                        OK: function () { location.reload(); }
                    }
                });

            }
        });
    });



    function base64ToArrayBuffer(base64) {
        var binaryString = window.atob(base64);
        var binaryLen = binaryString.length;
        var bytes = new Uint8Array(binaryLen);
        for (var i = 0; i < binaryLen; i++) {
            var ascii = binaryString.charCodeAt(i);
            bytes[i] = ascii;
        }
        return bytes;
    }

    function saveByteArray(fileName, byte) {
        var blob = new Blob([byte]);
        var link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        // var fileName = reportName + ".pdf";
        link.download = fileName;
        link.click();
    };



    $(document).on('click', "button.btnDescargarPDF", function () {
        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idDocumentoVenta = arrrayClass[0];
        var serieNumero = arrrayClass[1];
        descargarPDF(idDocumentoVenta, serieNumero);
    });

    $("#btnDescargarPDF").click(function () {

        //var arrrayClass = event.target.getAttribute("class").split(" ");
        var idDocumentoVenta = $("#idDocumentoVenta").val();
        var serieNumero = $("#vpSERIE_CORRELATIVO").html();
        descargarPDF(idDocumentoVenta, serieNumero);
    });


    function descargarPDF(idDocumentoVenta, serieNumero) {
        $.ajax({
            url: "/Factura/descargarArchivoDocumentoVenta",
            data: {
                idDocumentoVenta: idDocumentoVenta
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                alert("Ocurrió un problema al descargar la factura " + serieNumero + " en formato PDF.");
            },
            success: function (documentos) {


                var filePDF = base64ToArrayBuffer(documentos.pdf);
                saveByteArray(documentos.nombreArchivo + ".pdf", filePDF);

                /*     if (DESCARGAR_XML == 1) {
                         var fileCPE = base64ToArrayBuffer(documentos.cpe);
                         var fileCDR = base64ToArrayBuffer(documentos.cdr);
                         saveByteArray(serieNumero + ".xml", fileCPE);
                         saveByteArray('R-' + serieNumero + ".xml", fileCDR);
                     }*/

                //Se descarga el PDF y luego se limpia el formulario
                //        window.open('/General/DownLoadFile?fileName=' + fileName);
                // window.location = '/Cotizacion/CancelarCreacionCotizacion';
            }
        });




    }


    $("#btnDescargarXML").click(function () {

        var idDocumentoVenta = $("#idDocumentoVenta").val();
        var serieNumero = $("#vpSERIE_CORRELATIVO").html();



        $.ajax({
            url: "/Factura/descargarArchivoDocumentoVenta",
            data: {
                idDocumentoVenta: idDocumentoVenta
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                alert("Ocurrió un problema al descargar los archivos XML de la factura " + serieNumero + ".");
            },
            success: function (documentos) {


                //   var filePDF = base64ToArrayBuffer(documentos.pdf);
                //    saveByteArray(serieNumero + ".pdf", filePDF);

                //  if (DESCARGAR_XML == 1) {
                var fileCPE = base64ToArrayBuffer(documentos.cpe);
                var fileCDR = base64ToArrayBuffer(documentos.cdr);
                saveByteArray(documentos.nombreArchivo + ".xml", fileCPE);
                saveByteArray('R-' + documentos.nombreArchivo + ".xml", fileCDR);
                //     }



                //Se descarga el PDF y luego se limpia el formulario
                //        window.open('/General/DownLoadFile?fileName=' + fileName);
                // window.location = '/Cotizacion/CancelarCreacionCotizacion';
            }
        });



    });






    function desactivarBotonesVer() {
        $("#btnContinuarGenerandoNotaCredito").attr("disabled", "disabled");
        $("#btnContinuarGenerandoNotaDebito").attr("disabled", "disabled");
        $("#btnCancelarNotaCredito").attr("disabled", "disabled");
        $("#btnCancelarNotaDebito").attr("disabled", "disabled");
        $("#btnCancelarFactura").attr("disabled", "disabled");
        $("btnSolicitarAnulacion").attr("disabled", "disabled");
        $("btnIniciarNotaCredito").attr("disabled", "disabled");
        $("btnIniciarNotaDebito").attr("disabled", "disabled");
        $("btnIniciarAprobacion").attr("disabled", "disabled");
        $("btnIniciarRefacturacion").attr("disabled", "disabled");
    }

    function activarBotonesVer() {
        $("#btnContinuarGenerandoNotaCredito").removeAttr("disabled");
        $("#btnContinuarGenerandoNotaDebito").removeAttr("disabled");
        $("#btnCancelarNotaCredito").removeAttr("disabled");
        $("#btnCancelarNotaDebito").removeAttr("disabled");
        $("#btnCancelarFactura").removeAttr("disabled");
        $("btnSolicitarAnulacion").removeAttr("disabled");
        $("btnIniciarNotaCredito").removeAttr("disabled");
        $("btnIniciarNotaDebito").removeAttr("disabled");
        $("btnIniciarAprobacion").removeAttr("disabled");
        $("btnIniciarRefacturacion").removeAttr("disabled");
    }





    function limpiarComentario() {
        $("#comentarioEstado").val("");
        $("#comentarioEstado").focus();
    }





    $('#modalAprobacion').on('shown.bs.modal', function (e) {
        limpiarComentario();
    });




    $('#btnSolicitarAnulacion').click(function () {

        //var arrrayClass = event.target.getAttribute("class").split(" ");
        //$("#idDocumentoVenta").val(arrrayClass[0]);
        $("#serieNumero").val($("#vpSERIE_CORRELATIVO").html());
        $("#modalAnulacion").modal();
        //modalAnulacion.modal();
    });

    $('#btnIniciarNotaCredito').click(function () {

        //var arrrayClass = event.target.getAttribute("class").split(" ");
        //$("#idDocumentoVenta").val(arrrayClass[0]);
        $("#serieNumeroFacturaParaNotaCredito").val($("#vpSERIE_CORRELATIVO").html());
        $("#modalGenerarNotaCredito").modal();
        $("#divProductoDescuento").hide();
        //modalAnulacion.modal();
    });


    $('#btnIniciarNotaDebito').click(function () {

        //var arrrayClass = event.target.getAttribute("class").split(" ");
        //$("#idDocumentoVenta").val(arrrayClass[0]);
        $("#serieNumeroFacturaParaNotaDebito").val($("#vpSERIE_CORRELATIVO").html());
        $("#modalGenerarNotaDebito").modal();
        $("#divProductoCargo").hide();

        //modalAnulacion.modal();
    });


    $('#btnIniciarRefacturacion').click(function () {

        //   $("#btnContinuarGenerandoNotaDebito").attr("disabled", "disabled");
        //       $("#btnCancelarNotaDebito").attr("disabled", "disabled");
        //      var tipoNotaDebito = $('input:radio[name=tipoNotaDebito]:checked').val();

        var idDocumentoVenta = $("#idDocumentoVenta").val();


        /*     if (tipoNotaDebito == null) {
                 mostrarMensajeErrorProceso("Debe seleccionar el Motivo de la Nota de Débito.");
                 $("#btnContinuarGenerandoNotaDebito").removeAttr("disabled");
                 $("#btnCancelarNotaDebito").removeAttr("disabled");
                 return false;
             }*/

        var yourWindow;
        $.ajax({
            url: "/Factura/iniciarRefacturacion",
            type: 'POST',
            dataType: 'JSON',
            data: {

                idDocumentoVenta: idDocumentoVenta
            },
            error: function (error) {
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
                //   $("#btnContinuarGenerandoNotaDebito").removeAttr("disabled");
                //    $("#btnCancelarNotaDebito").removeAttr("disabled");
            },
            success: function (venta) {


                if (venta.tipoErrorCrearTransaccion == 0) {
                    window.location = '/Factura/Crear';
                }
                else {
                    mostrarMensajeErrorProceso(MENSAJE_ERROR + "\n" + "Detalle Error: " + venta.descripcionError);
                    //    $("#btnContinuarGenerandoNotaDebito").removeAttr("disabled");
                    //      $("#btnCancelarNotaDebito").removeAttr("disabled");
                }


            }
        });


    });



    $('#btnIniciarAprobacion').click(function () {
        $("#serieNumeroAprobacionAnulacion").val($("#vpSERIE_CORRELATIVO").html());
        $("#modalAprobacionAnulacion").modal();
    });



    $(document).on('click', "button.btnAprobarAnulacion", function () {

        /*    var arrrayClass = event.target.getAttribute("class").split(" ");
            $("#idDocumentoVenta").val(arrrayClass[0]);
            $("#serieNumeroAprobacionAnulacion").val(arrrayClass[1]);
            */

        //   modalAnulacion.modal();
    });



    $(document).on('click', "#btnAceptarAnulacion", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idDocumentoVenta = $("#idDocumentoVenta").val();
        var serieNumero = $("#serieNumero").val();
        var comentarioAnulado = $("#comentarioAnulado").val();

        if (comentarioAnulado.trim().length < 25) {
            alert("El motivo de la anulación debe contener al menos 25 caracteres.");
            $("#comentarioAnulado").focus();
            return false;
        }


        $("#btnAceptarAnulacion").attr("disabled", "disabled");
        $.ajax({
            url: "/Factura/SolicitarAnulacion",
            type: 'POST',
            dataType: 'JSON',
            data: {
                comentarioAnulado: comentarioAnulado,
                idDocumentoVenta: idDocumentoVenta
            },
            error: function () {
                $("#btnAceptarAnulacion").removeAttr("disabled");
                mostrarMensajeErrorProceso();
            },
            success: function (documentoVenta) {

                if (documentoVenta.tipoErrorSolicitudAnulacion == 0) {
                    $.alert({
                        title: TITLE_EXITO,
                        type: 'green',
                        content: "Se ha solicitado la anulación del documento: " + serieNumero + ", por favor envíe correo a facturacion@mpinstitucional.com para completar la solicitud.",
                        buttons: {
                            OK: function () { location.reload(); }
                        }
                    })
                }
                else {
                    mostrarMensajeErrorProceso(MENSAJE_ERROR + "\n" + "Detalle Error: " + documentoVenta.descripcionError);
                }

                $("#btnAceptarAnulacion").removeAttr("disabled");


            }
        });
    });

    $(document).on('click', "#btnAprobarAnulacion", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idDocumentoVenta = $("#idDocumentoVenta").val();
        var serieNumero = $("#serieNumero").val();
        var comentarioAprobacionAnulacion = $("#comentarioAprobacionAnulacion").val();


        if (comentarioAprobacionAnulacion.trim().length < 25) {
            alert("El motivo de aprobación debe contener al menos 25 caracteres (Este motivo será enviado a SUNAT).");
            $("#comentarioAprobacionAnulacion").focus();
            return false;
        }



        $("#btnAprobarAnulacion").attr("disabled", "disabled");
        $.ajax({
            url: "/Factura/AprobarAnulacion",
            type: 'POST',
            dataType: 'JSON',
            data: {
                comentarioAprobacionAnulacion: comentarioAprobacionAnulacion,
                idDocumentoVenta: idDocumentoVenta
            },
            error: function () {
                $("#btnAprobarAnulacion").removeAttr("disabled");
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
            },
            success: function (documentoVenta) {
                $("#btnAprobarAnulacion").removeAttr("disabled");
                if (documentoVenta.rPTA_BE.CODIGO == "001") {
                    $.alert({
                        title: TITLE_EXITO,
                        type: 'green',
                        content: "Se realizó la Solicitud de Anulación del documento: " + documentoVenta.cPE_CABECERA_BE.SERIE + "-" + documentoVenta.cPE_CABECERA_BE.CORRELATIVO + ".",
                        buttons: {
                            OK: function () { location.reload(); }
                        }
                    })
                }
                else {
                    mostrarMensajeErrorProceso(documentoVenta.rPTA_BE.DETALLE);
                }

            }
        });
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
    EVENTOS BUSQUEDA FACTURA
    #####################################################*/

    $("#btnExportExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
    });

    $("input[name=documentoVenta_solicitadoAnulacion]").on("click", function () {
        var solicitadoAnulacion = $("input[name=documentoVenta_solicitadoAnulacion]:checked").val();
        $.ajax({
            url: "/Factura/ChangeSolicitadoAnulacion",
            type: 'POST',
            data: {
                solicitadoAnulacion: solicitadoAnulacion
            },
            success: function () {
            }
        });
    });


    $("#btnBusqueda").click(function () {

        var idCiudad = $("#idCiudad").val();
        if ((idCiudad == "" || idCiudad == GUID_EMPTY) && $("#documentoVenta_numero").val() != "") {
            $("#idCiudad").focus();
            $.alert({
                title: TITLE_MENSAJE_BUSQUEDA,
                content: 'Para realizar una búsqueda con número de Factura debe indicar la sede MP.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }



        //sede MP

        var idCliente = $("#idCliente").val();
        var numero = $("#documentoVenta_numero").val();

        var numeroPedido = $("#documentoVenta_pedido_numeroPedido").val();
        var numeroGuiaRemision = $("#documentoVenta_guiaRemision_numeroDocumento").val();

        var fechaEmisionDesde = $("#documentoVenta_fechaEmisionDesde").val();
        var fechaEmisionHasta = $("#documentoVenta_fechaEmisionHasta").val();
        //var estado = $("#estado").val();

        var estadoDocumentoSunatBusqueda = $("#documentoVenta_estadoDocumentoSunatBusqueda").val();
        var tipoDocumento = $("#documentoVenta_tipoDocumento").val();

        var sku = $("#documentoVenta_sku").val();


        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/Factura/Search",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                numero: numero,
                numeroPedido: numeroPedido,
                numeroGuiaRemision: numeroGuiaRemision,
                fechaEmisionDesde: fechaEmisionDesde,
                fechaEmisionHasta: fechaEmisionHasta,
                estadoDocumentoSunatBusqueda: estadoDocumentoSunatBusqueda,
                tipoDocumento: tipoDocumento,
                sku: sku
            },
            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
                mostrarMensajeErrorProceso();
            },
            success: function (facturaList) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableFacturas > tbody").empty();
                //FooTable.init('#tableCotizaciones');
                $("#tableFacturas").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < facturaList.length; i++) {

                    var styleEstado = "";
                    var botonAnular = "";
                    var botonGenerarNotaCredito = "";

                    switch (facturaList[i].estadoDocumentoSunat) {
                        case 105: case 104: styleEstado = "style='color: red;font-weight: normal;'";

                            break;
                        case 102: styleEstado = "style='color: green;font-weight: normal;'";
                            botonAnular = '<button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnAnular  btn btn-danger" data-toggle="modal" data-target="#modalAnulacion">Anular</button >';
                            break;
                        case 103: styleEstado = "style='color: orange;font-weight: normal;'";
                            botonAnular = '<button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnAnular  btn btn-danger" data-toggle="modal" data-target="#modalAnulacion">Anular</button >';
                            break;
                        case 0: styleEstado = "style='color: orange;font-weight: normal;'";

                            break;
                        default: styleEstado = "style='color: black'"; break;
                    }

                    if (facturaList[i].usuario.apruebaAnulaciones && facturaList[i].solicitadoAnulacion
                        && (facturaList[i].estadoDocumentoSunat == 102 || facturaList[i].estadoDocumentoSunat == 103)
                    ) {
                        botonAnular = '<button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnAprobarAnulacion  btn btn-danger" data-toggle="modal" data-target="#modalAprobacionAnulacion">Aprobar Anulación</button >';
                    }
                    else if (facturaList[i].solicitadoAnulacion) {
                        botonAnular = '';
                    }

                    if (facturaList[i].usuario.creaNotasCredito &&
                        (facturaList[i].estadoDocumentoSunat == 102 || facturaList[i].estadoDocumentoSunat == 103)
                    ) {
                        botonGenerarNotaCredito = '<button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnGenerarNotaCredito  btn btn-danger" data-toggle="modal" data-target="#modalGenerarNotaCredito">Generar Nota Crédito</button >';
                    }



                    var botonDescargarXML = '';
                    if (DESCARGAR_XML == 1)
                        botonDescargarXML = '<button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnDescargarXML btn btn- primary">Descargar XML</button>';

                    var movimientoAlmacen = "";
                    if (facturaList[i].guiaRemision != null) {
                        movimientoAlmacen = facturaList[i].guiaRemision.serieNumeroGuia;
                    }
                    else if (facturaList[i].notaIngreso != null) {
                        movimientoAlmacen = facturaList[i].notaIngreso.serieNumeroNotaIngreso;
                    }

                    var factura = '<tr data-expanded="false">' +
                        '<td>  ' + facturaList[i].idDocumentoVenta + '</td>' +
                        '<td>  ' + facturaList[i].serieNumero + '</td>' +
                        '<td>  ' + facturaList[i].pedido.numeroPedidoString + '</td>' +


                        '<td>  ' + movimientoAlmacen + '</td>' +



                        '<td>  ' + facturaList[i].usuario.nombre + '</td>' +
                        '<td>  ' + invertirFormatoFecha(facturaList[i].fechaEmision.substr(0, 10)) + '</td>' +
                        '<td>  ' + invertirFormatoFecha(facturaList[i].fechaVencimiento.substr(0, 10)) + '</td>' +
                        '<td>  ' + facturaList[i].cliente.codigo + '</td>' +
                        '<td>  ' + facturaList[i].cliente.razonSocial + '</td>' +
                        '<td>  ' + facturaList[i].cliente.ruc + '</td>' +
                        '<td>  ' + facturaList[i].ciudad.nombre + '</td>' +
                        '<td>  ' + facturaList[i].total + '</td>' +
                        '<td>' + facturaList[i].cliente.responsableComercial.codigo + '</td>' +
                        '<td ' + styleEstado + ' > ' + facturaList[i].estadoDocumentoSunatString + '</td>' +
                        '<td>  ' + facturaList[i].comentarioSolicitudAnulacion + '</td>' +


                        '<td> <button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnVerDocumentoVenta btn btn-primary">Ver</button>' +
                        '<button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnDescargarPDF btn btn-primary bouton-image pdfBoton">PDF</button>' +
                        '<button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnActualizarEstado  btn btn-success">Act. Estado</button >' +
                        /*      '<td> <button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnDescargarPDF btn btn-primary">Descargar PDF</button>' +
                              botonDescargarXML +
                              '<button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnActualizarEstado  btn btn-primary">Act. Estado</button >' +
                              botonAnular +
                              botonGenerarNotaCredito +
                              '</td> ' +*/
                        '</tr>';

                    $("#tableFacturas").append(factura);
                }


                if (facturaList.length > 0) {
                    $("#msgBusquedaSinResultados").hide();
                    $("#divExportButton").show();
                }
                else {
                    $("#divExportButton").hide();
                    $("#msgBusquedaSinResultados").show();
                }
            }
        });
    });


    $("#idCiudad").change(function () {
        var idCiudad = $("#idCiudad").val();

        $.ajax({
            url: "/Factura/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad
            },
            error: function (detalle) {
                alert(MENSAJE_ERROR);
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
            success: function () { }
        });
    }














    $(document).on('click', "button.btnVerDocumentoVenta", function () {

        $('body').loadingModal({
            text: 'Abriendo Documento de Venta...'
        });
        $('body').loadingModal('show');

        activarBotonesVer();
        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idDocumentoVenta = arrrayClass[0];


        /*   $('body').loadingModal({
               text: 'Creando Factura...'
           });
           */


        $.ajax({
            url: "/Factura/Show",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idDocumentoVenta: idDocumentoVenta
            },
            error: function (resultado) {
                $('body').loadingModal('hide');
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
                activarBotonesFacturar();
            },
            success: function (documentoVenta) {
                $('body').loadingModal('hide');
                /*Habilitar/Deshabilitar botones para ver notas de crédito/débito*/


                $("#btnVerNotaCredito").hide();
                if (documentoVenta.tieneNotaCredito) {
                    $("#btnVerNotaCredito").show();
                }

                $("#btnVerNotaDebito").hide();
                if (documentoVenta.tieneNotaDebito) {
                    $("#btnVerNotaDebito").show();
                }


                /*Solicitar Anulación */
                if (documentoVenta.solicitadoAnulacion == false
                    &&
                    (documentoVenta.estadoDocumentoSunat == '102'
                        || documentoVenta.estadoDocumentoSunat == '103')
                ) {
                    $('#btnSolicitarAnulacion').show();

                }
                else {
                    $('#btnSolicitarAnulacion').hide();
                }

                /*Aprobar anulacion*/
                if (documentoVenta.solicitadoAnulacion == true && (documentoVenta.estadoDocumentoSunat == '102'
                    || documentoVenta.estadoDocumentoSunat == '103')
                ) {
                    $('#btnIniciarAprobacion').show();
                }
                else {
                    $('#btnIniciarAprobacion').hide();
                }


                //Nota de Crédito
                if ((documentoVenta.estadoDocumentoSunat == '102'
                    || documentoVenta.estadoDocumentoSunat == '103')
                    && (documentoVenta.tipoDocumento == CONS_TIPO_DOC_FACTURA //|| 
                        //documentoVenta.tipoDocumento == CONS_TIPO_DOC_BOLETA
                    )
                    && documentoVenta.solicitadoAnulacion == false
                ) {
                    $('#btnIniciarNotaCredito').show();
                }
                else {
                    $('#btnIniciarNotaCredito').hide();
                }


                if ((documentoVenta.estadoDocumentoSunat == '102'
                    || documentoVenta.estadoDocumentoSunat == '103')
                    && (documentoVenta.tipoDocumento == CONS_TIPO_DOC_FACTURA //||
                        //documentoVenta.tipoDocumento == CONS_TIPO_DOC_BOLETA
                    )
                    && documentoVenta.solicitadoAnulacion == false
                ) {
                    $('#btnIniciarNotaDebito').show();
                }
                else {
                    $('#btnIniciarNotaDebito').hide();
                }


                if ((documentoVenta.estadoDocumentoSunat == '102'
                    || documentoVenta.estadoDocumentoSunat == '103')
                    && (documentoVenta.tipoDocumento == CONS_TIPO_DOC_FACTURA //||
                        //documentoVenta.tipoDocumento == CONS_TIPO_DOC_BOLETA
                    )
                    && documentoVenta.solicitadoAnulacion == false
                ) {
                    $('#btnIniciarRefacturacion').show();
                }
                else {
                    $('#btnIniciarRefacturacion').hide();
                }



                if (documentoVenta.tipoDocumento == CONS_TIPO_DOC_NOTA_CREDITO
                    || documentoVenta.tipoDocumento == CONS_TIPO_DOC_NOTA_DEBITO) {

                    $('.datosNotaCreditoDebito').show();
                    if (documentoVenta.tipoDocumento == CONS_TIPO_DOC_NOTA_CREDITO) {
                        $("#pvMOTIVO").html(documentoVenta.tipoNotaCreditoString);
                    }
                    else {
                        $("#pvMOTIVO").html(documentoVenta.tipoNotaDebitoString);
                    }
                    $("#pvDES_MTVO_NC_ND").html(documentoVenta.cPE_CABECERA_BE.DES_MTVO_NC_ND);

                    /*Documento Referencia*/

                    $("#vpREFERENCIA_FECHA_EMISION").html(documentoVenta.cPE_DOC_REF_BEList[0].FEC_DOC_REF);
                    var numeroReferencia = documentoVenta.cPE_DOC_REF_BEList[0].NUM_SERIE_CPE_REF + "-"
                        + documentoVenta.cPE_DOC_REF_BEList[0].NUM_CORRE_CPE_REF;
                    $("#vpREFERENCIA_SERIE_CORRELATIVO").html(numeroReferencia);
                }
                else {

                    $('.datosNotaCreditoDebito').hide();
                }

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
                if (documentoVenta.cPE_DAT_ADIC_BEList.length > 0) {
                    $("#vpCODIGO_CLIENTE").html(documentoVenta.cPE_DAT_ADIC_BEList[0].TXT_DESC_ADIC_SUNAT);
                    if (documentoVenta.cPE_DAT_ADIC_BEList.length > 1) {
                        $("#vpOBSERVACIONES").html(documentoVenta.cPE_DAT_ADIC_BEList[1].TXT_DESC_ADIC_SUNAT);
                    }
                }
                else {
                    $("#vpCODIGO_CLIENTE").html("");
                    $("#vpOBSERVACIONES").html("");
                }
                $("#vpCORREO").html(documentoVenta.cPE_CABECERA_BE.CORREO_ENVIO);
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

                $("#modalVerFactura").modal();


                $("#tableDetalleFacturaVistaPrevia > tbody").empty();
                //FooTable.init('#tableCotizaciones');
                $("#tableDetalleFacturaVistaPrevia").footable();
                var lineasFactura = documentoVenta.cPE_DETALLE_BEList;

                for (var i = 0; i < lineasFactura.length; i++) {

                    var lineaFactura = "";



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

            }
        });
    });


    $("input:radio[name=tipoNotaCredito]").change(function () {
        var tipoNotaCredito = $('input:radio[name=tipoNotaCredito]:checked').val();

        if (tipoNotaCredito == TIPO_NOTA_CREDITO_DESCUENTO_GLOBAL) {
            $("#divProductoDescuento").show();
        }
        else {
            $("#divProductoDescuento").hide();
        }
    });


    $("#btnContinuarGenerandoNotaCredito").click(function () {
        desactivarBotonesVer();

        var tipoNotaCredito = $('input:radio[name=tipoNotaCredito]:checked').val();
        /*
        if (tipoNotaCredito == TIPO_NOTA_CREDITO_ANULACION_DE_LA_OPERACION) {
            /*alert("Anulacion de la operacion");
        }*/


        var idDocumentoVenta = $("#idDocumentoVenta").val();
        var idProducto = $("#idProducto").val();

        if (tipoNotaCredito == null) {
            mostrarMensajeErrorProceso("Debe seleccionar el Motivo de la Nota de Crédito.");
            $("#btnContinuarGenerandoNotaCredito").removeAttr("disabled");
            $("#btnCancelarNotaCredito").removeAttr("disabled");
            return false;
        }

        var yourWindow;
        $.ajax({
            url: "/NotaCredito/iniciarCreacionNotaCredito",
            type: 'POST',
            dataType: 'JSON',
            data: {

                idDocumentoVenta: idDocumentoVenta,
                tipoNotaCredito: tipoNotaCredito,
                idProducto: idProducto
            },
            error: function (error) {
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
                $("#btnContinuarGenerandoNotaCredito").removeAttr("disabled");
                $("#btnCancelarNotaCredito").removeAttr("disabled");
            },
            success: function (venta) {


                if (venta.tipoErrorCrearTransaccion == 0) {
                    window.location = '/NotaCredito/Crear';
                }
                else {
                    mostrarMensajeErrorProceso(MENSAJE_ERROR + "\n" + "Detalle Error: " + venta.descripcionError);
                    $("#btnContinuarGenerandoNotaCredito").removeAttr("disabled");
                    $("#btnCancelarNotaCredito").removeAttr("disabled");
                }


            }
        });

    });


    $("input:radio[name=tipoNotaDebito]").change(function () {
        var tipoNotaDebito = $('input:radio[name=tipoNotaDebito]:checked').val();

        if (tipoNotaDebito != TIPO_NOTA_DEBITO_AUMENTO_VALOR) {
            $("#divProductoCargo").show();
        }
        else {
            $("#divProductoCargo").hide();
        }



    });


    $("#btnContinuarGenerandoNotaDebito").click(function () {

        desactivarBotonesVer();
        var cargos = [];
        $('#ProductoSelectIds option').each(function (i) {
            if (this.selected == true) {
                cargos.push(this.value);
            }
        });

        //PENDIENTE de validacion
        //if (cargos.length)

        var tipoNotaDebito = $('input:radio[name=tipoNotaDebito]:checked').val();
        var idDocumentoVenta = $("#idDocumentoVenta").val();

        if (tipoNotaDebito == null) {
            mostrarMensajeErrorProceso("Debe seleccionar el Motivo de la Nota de Débito.");
            $("#btnContinuarGenerandoNotaDebito").removeAttr("disabled");
            $("#btnCancelarNotaDebito").removeAttr("disabled");
            return false;
        }

        var yourWindow;
        $.ajax({
            url: "/NotaDebito/iniciarCreacion",
            type: 'POST',
            dataType: 'JSON',
            data: {

                idDocumentoVenta: idDocumentoVenta,
                tipoNotaDebito: tipoNotaDebito,
                cargos: cargos
            },
            error: function (error) {
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
                $("#btnContinuarGenerandoNotaDebito").removeAttr("disabled");
                $("#btnCancelarNotaDebito").removeAttr("disabled");
                desactivarBotonesVer();
            },
            success: function (venta) {


                if (venta.tipoErrorCrearTransaccion == 0) {
                    window.location = '/NotaDebito/Crear';
                }
                else {
                    mostrarMensajeErrorProceso(MENSAJE_ERROR + "\n" + "Detalle Error: " + venta.descripcionError);
                    $("#btnContinuarGenerandoNotaDebito").removeAttr("disabled");
                    $("#btnCancelarNotaDebito").removeAttr("disabled");
                }


            }
        });

    });





    $("#btnFinalizarCreacionFactura").click(function () {

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

        var observaciones = $("#venta_observaciones").val();
        var fechaEmision = $("#documentoVenta_fechaEmision").val();
        var horaEmision = $("#documentoVenta_horaEmision").val();

        desactivarBotonesFactura();

        $.ajax({
            url: "/Factura/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                fechaEmision: fechaEmision,
                horaEmision: horaEmision,

                observaciones: observaciones
            },
            error: function (resultado) {
                // $('body').loadingModal('hide')
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
                activarBotonesNotasCredito();
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

                    $("#pvMOTIVO").html(documentoVenta.tipoNotaCreditoString);
                    $("#pvDES_MTVO_NC_ND").html(documentoVenta.cPE_CABECERA_BE.DES_MTVO_NC_ND);



                    /*Documento Referencia*/

                    $("#vpREFERENCIA_FECHA_EMISION").html(documentoVenta.cPE_DOC_REF_BEList[0].FEC_DOC_REF);
                    var numeroReferencia = documentoVenta.cPE_DOC_REF_BEList[0].NUM_SERIE_CPE_REF + "-"
                        + documentoVenta.cPE_DOC_REF_BEList[0].NUM_CORRE_CPE_REF;
                    $("#vpREFERENCIA_SERIE_CORRELATIVO").html(numeroReferencia);




                    $("#modalVistaPreviaCPE").modal();


                    $("#tableDetalleCPEVistaPrevia > tbody").empty();
                    //FooTable.init('#tableCotizaciones');
                    $("#tableDetalleCPEVistaPrevia").footable();
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

                        $("#tableDetalleCPEVistaPrevia").append(lineaFactura);
                    }

                    //documentoVenta.CPE_CABECERA_BE



                    activarBotonesNotasCredito();
                }
                else {
                    mostrarMensajeErrorProceso(MENSAJE_ERROR + ".\n" + "Detalle Error: " + documentoVenta.cPE_RESPUESTA_BE.DETALLE);
                    //$("#btnAceptarFacturarPedido").removeAttr("disabled");
                    activarBotonesNotasCredito();
                }

            }

        });
        $("btnCancelarFacturarPedido").click();
    });

    $("#btnVerNotaCredito").click(function () {


        /*
        var guiaRemisionList = pedido.guiaRemisionList;
        for (var j = 0; j < guiaRemisionList.length; j++) {
            $("#tableDetalleGuia > tbody").empty();
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

            $("#tableDetalleGuia").append(dGuia);

            plantilla = $("#plantillaVerGuiasRemision").html();

            plantilla = plantilla.replace("#serieNumero", guiaRemisionList[j].serieNumeroGuia);
            plantilla = plantilla.replace("#fechaEmisionGuia", invertirFormatoFecha(guiaRemisionList[j].fechaEmision.substr(0, 10)));

            plantilla = plantilla.replace("#serieNumeroFactura", guiaRemisionList[j].documentoVenta.serieNumero);
            if (guiaRemisionList[j].documentoVenta.fechaEmision != null) {
                plantilla = plantilla.replace("#fechaEmisionFactura", invertirFormatoFecha(guiaRemisionList[j].documentoVenta.fechaEmision.substr(0, 10)));
            }
            else
                plantilla = plantilla.replace("#fechaEmisionFactura", "");


            plantilla = plantilla.replace("tableDetalleGuia", "tableDetalleGuia" + j);

            $("#formVerGuiasRemision").append(plantilla);
            
        }*/



    });

    $("#btnVerNotaDebito").click(function () {

    });

    function activarBotonesFactura() {
        $("#btnFinalizarCreacionFactura").removeAttr("disabled");
        $("#btnCancelarFactura").removeAttr("disabled");
        $(".footable-show").removeAttr("disabled");
    }

    function desactivarBotonesFactura() {
        $("#btnFinalizarCreacionFactura").attr("disabled", "disabled");
        $("#btnCancelarFactura").attr("disabled", "disabled");
        $(".footable-show").attr("disabled", "disabled");
    }



    function desactivarBotonesConfirmarFactura() {
        $("#btnCancelarConfirmarFactura").attr("disabled", "disabled");
        $("#btnConfirmarGeneracionFactura").attr("disabled", "disabled");
    }

    function activarBotonesConfirmarFactura() {
        $("#btnCancelarConfirmarFactura").removeAttr("disabled");
        $("#btnConfirmarGeneracionFactura").removeAttr("disabled");
    }




    $("#btnCancelarFactura").click(function () {

        window.location = '/Factura/Index';
    });



    /*Evento que se dispara cuando se hace clic en FINALIZAR en la edición de la grilla*/
    $(document).on('click', "button.footable-hide", function () {

        //Se habilitan controles
        $("#btnCancelarNotaDebito").removeAttr('disabled');
        $("#btnFinalizarCreacionNotaDebito").removeAttr('disabled');


        var json = "[ ";

        //   var permiteEditarCantidades = $("#permiteEditarCantidades").val();
        var permiteEditarPrecios = $("#permiteEditarPrecios").val();
        var permiteEliminarLineas = $("#permiteEliminarLineas").val();

        if (permiteEditarPrecios == "True") {

            var $j_object = $("td.detcantidad");
            $.each($j_object, function (key, value) {
                var arrId = value.getAttribute("class").split(" ");
                /*Se elimina control input en columna cantidad*/
                var cantidad = $("." + arrId[0] + ".detcantidad").html();
                value.innerText = cantidad;
                /*Se elimina control input en columna porcentaje descuento*/
                var porcentajeDescuento = 0;
                var flete = 0;
                var precio = $("." + arrId[0] + ".detinprecioUnitario ").val();
                var costo = 0;
                var observacion = "";
                json = json + '{"idProducto":"' + arrId[0] + '", "cantidad":"' + cantidad + '", "porcentajeDescuento":"' + porcentajeDescuento + '", "precio":"' + precio + '", "flete":"' + flete + '",  "costo":"' + costo + '", "observacion":"' + observacion + '"},'
            });
        }
        else {
            var $j_object = $("td.detprecioUnitario");
            $.each($j_object, function (key, value) {
                var arrId = value.getAttribute("class").split(" ");
                /*Se elimina control input en columna cantidad*/
                var precio = $("." + arrId[0] + ".detprecioUnitario ").html();
                value.innerText = cantidad;
                /*Se elimina control input en columna porcentaje descuento*/
                var porcentajeDescuento = 0;
                var flete = 0;
                var cantidad = $("." + arrId[0] + ".detincantidad").val();
                var costo = 0;
                var observacion = "";
                json = json + '{"idProducto":"' + arrId[0] + '", "cantidad":"' + cantidad + '", "porcentajeDescuento":"' + porcentajeDescuento + '", "precio":"' + precio + '", "flete":"' + flete + '",  "costo":"' + costo + '", "observacion":"' + observacion + '"},'
            });



        }






        json = json.substr(0, json.length - 1) + "]";



        $.ajax({
            url: "/Factura/ChangeDetalle",
            type: 'POST',
            data: json,
            dataType: 'json',
            contentType: 'application/json',
            success: function (respuesta) {
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

});