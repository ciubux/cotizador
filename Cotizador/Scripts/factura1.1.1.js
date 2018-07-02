
jQuery(function ($) {


    //CONSTANTES POR DEFECTO
    var cantidadDecimales = 2;
    var IGV = 0.18;
    var SIMBOLO_SOL = "S/";
    var MILISEGUNDOS_AUTOGUARDADO = 5000;
    var DESCARGAR_XML = 0;

    //Estados para búsqueda de Pedidos
    /*var ESTADOS_TODOS = -1;
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
    var ESTADO_EN_EDICION_STR = "En Edición";*/

  
    var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
    

    
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición/creación; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_MENSAJE_BUSQUEDA = "Ingresar datos solicitados";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        obtenerConstantes();
        cargarChosenCliente();
        $("#btnBusqueda").click();
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
                        OK: function () { location.reload();}
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

   
       



    function desactivarBotonesVer()
    {
      /*  $("#btnCancelarCotizacion").attr('disabled', 'disabled');
        $("#btnEditarCotizacion").attr('disabled', 'disabled');*/
    }

    function activarBotonesVer() {
  /*      $("#btnCancelarCotizacion").removeAttr('disabled');
        $("#btnEditarCotizacion").removeAttr('disabled');;*/
    }
    


    

    function limpiarComentario()
    {
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
        //modalAnulacion.modal();
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
        $("#btnAceptarAnulacion").attr("disabled", "disabled");
        $.ajax({
            url: "/Factura/Anular",
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
                $("#btnAceptarAnulacion").removeAttr("disabled");
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "La factura: " + serieNumero + " se encuentra pendiente de aprobación para anulación.",
                    buttons: {
                        OK: function () { location.reload(); }
                    }
                })
            }
        });
    });

    $(document).on('click', "#btnAprobarAnulacion", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idDocumentoVenta = $("#idDocumentoVenta").val();
        var serieNumero = $("#serieNumero").val();
        var comentarioAprobacionAnulacion = $("#comentarioAprobacionAnulacion").val();
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
                mostrarMensajeErrorProceso();
            },
            success: function (documentoVenta) {
                $("#btnAprobarAnulacion").removeAttr("disabled");
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "La factura: " + serieNumero + " se encuentra en proceso de anulación.",
                    buttons: {
                        OK: function () { location.reload(); }
                    }
                })
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

    function mostrarMensajeErrorProceso() {
        $.alert({
            //icon: 'fa fa-warning',
            title: 'Error',
            content: MENSAJE_ERROR,
            type: 'red',
            buttons: {
                OK: function () { }
            }
        });
    }


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
        if ((idCiudad == "" || idCiudad == GUID_EMPTY)  && $("#documentoVenta_numero").val() != "") {
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
                estadoDocumentoSunatBusqueda: estadoDocumentoSunatBusqueda
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
                        && (facturaList[i].estadoDocumentoSunat == 102 ||  facturaList[i].estadoDocumentoSunat == 103)
                    ) {
                        botonAnular = '<button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnAprobarAnulacion  btn btn-danger" data-toggle="modal" data-target="#modalAprobacionAnulacion">Aprobar Anulación</button >';
                    }
                    else if  (facturaList[i].solicitadoAnulacion) {
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


                    var factura = '<tr data-expanded="false">'+
                        '<td>  ' + facturaList[i].idDocumentoVenta + '</td>' +
                        '<td>  ' + facturaList[i].serieNumero + '</td>' +
                        '<td>  ' + facturaList[i].pedido.numeroPedidoString + '</td>' +
                        '<td>  ' + facturaList[i].guiaRemision.serieNumeroGuia + '</td>' +
                        '<td>  ' + facturaList[i].usuario.nombre + '</td>' +
                        '<td>  ' + invertirFormatoFecha(facturaList[i].fechaEmision.substr(0, 10)) + '</td>' +
                        '<td>  ' + facturaList[i].cliente.razonSocial + '</td>' +
                        '<td>  ' + facturaList[i].cliente.ruc + '</td>' +
                        '<td>  ' + facturaList[i].ciudad.nombre + '</td>' +
                        '<td>  ' + facturaList[i].total + '</td>' +
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


                if (facturaList.length > 0)
                    $("#msgBusquedaSinResultados").hide();
                else
                    $("#msgBusquedaSinResultados").show();

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
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
                activarBotonesFacturar();
            },
            success: function (documentoVenta) {

                
                if (documentoVenta.solicitadoAnulacion == true) {
                    $('#btnSolicitarAnulacion').hide();
                    $('#btnIniciarAprobacion').show();
                }
                else {
                    $('#btnSolicitarAnulacion').show();
                    $('#btnIniciarAprobacion').hide();
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
                    $("#vpOBSERVACIONES").html(documentoVenta.cPE_DAT_ADIC_BEList[1].TXT_DESC_ADIC_SUNAT);  
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


  
    $("#btnContinuarGenerandoNotaCredito").click(function () {
        $("#btnCancelarNotaCredito").click();

        var tipoNotaCredito = $('input:radio[name=tipoNotaCredito]:checked').val();

        var idDocumentoVenta =  $("#idDocumentoVenta").val();

        var yourWindow;
        $.ajax({
            url: "/NotaCredito/iniciarCreacionNotaCredito",
            type: 'POST',
            data: {
                idDocumentoVenta: idDocumentoVenta,
                tipoNotaCredito: tipoNotaCredito
            },
            error: function (detalle) { alert("Ocurrió un problema al iniciar la edición de la nota de crédito."); },
            success: function (fileName) {
                window.location = '/NotaCredito/Crear';
            }
        });

    });
    
});