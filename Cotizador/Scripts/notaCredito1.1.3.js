
jQuery(function ($) {



    //CONSTANTES POR DEFECTO
    var cantidadDecimales = 2;
    var cantidadCuatroDecimales = 4;
    var IGV = 0.18;
    var SIMBOLO_SOL = "S/";
    var MILISEGUNDOS_AUTOGUARDADO = 5000;
    var VARIACION_PRECIO_ITEM_PEDIDO = 0.01;


    //Estados para búsqueda de Pedidos


    var ESTADO_PENDIENTE_APROBACION = 0;
    var ESTADO_INGRESADO = 1;
    var ESTADO_DENEGADO = 2;
    var ESTADO_PROGRAMADO = 3;
    var ESTADO_ATENDIDO = 4;
    var ESTADO_ATENDIDO_PARCIALMENTE = 5;
    var ESTADO_EN_EDICION = 6;
    var ESTADO_ELIMINADO = 7;
    var ESTADO_FACTURADO = 8;


    //Etiquetas de estadps para búsqueda de Pedidos
    var ESTADO_PENDIENTE_APROBACION_STR = "Pendiente de Aprobación de Ingreso";
    var ESTADO_INGRESADO_STR = "Pedido Ingresado";
    var ESTADO_DENEGADO_STR = "Pedido Denegado";
    var ESTADO_PROGRAMADO_STR = "Pedido Programado"
    var ESTADO_ATENDIDO_STR = "Pedido Atendido"
    var ESTADO_ATENDIDO_PARCIALMENTE_STR = "Pedido Atendido Parcialmente"
    var ESTADO_EN_EDICION_STR = "Pedido En Edición";
    var ESTADO_ELIMINADO_STR = "Pedido Eliminado";
    var ESTADO_FACTURADO_STR = "Pedido Facturado";

    var TITULO_CANCELAR_PROGRAMACION = "Cancelar Programación de Pedido";
    var TITULO_DENEGAR_INGRESO = "Denegar Ingreso de Pedido";
    var TITULO_APROBAR_INGRESO = "Aprobar Ingreso de Pedido";
    var TITULO_ELIMINAR = "Eliminar Pedido";

    //Estados Crediticios
    var ESTADO_PENDIENTE_LIBERACION = 0;
    var ESTADO_LIBERADO = 1;
    var ESTADO_BLOQUEADO = 2;

    var ESTADO_PENDIENTE_LIBERACION_STR = "Pedido Pendiente de Liberación";
    var ESTADO_LIBERADO_STR = "Pedido Liberado";
    var ESTADO_BLOQUEADO_STR = "Pedido Bloqueado";




    //Eliminar luego 
    var CANT_SOLO_OBSERVACIONES = 0;
    var CANT_SOLO_CANTIDADES = 1;
    var CANT_CANTIDADES_Y_OBSERVACIONES = 2;

    var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";

    /*
       $("#btnAgregarCliente").click(function () {
   
   
           $("#modalAgregarCliente2").load('/Cliente/Editar');
       });*/

    /*
     * 2 BusquedaPedidos
       3 CrearPedido
     */

    var pagina = 2;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";

    $(document).ready(function () {
        obtenerConstantes();
        setTimeout(autoGuardarPedido, MILISEGUNDOS_AUTOGUARDADO);
        //cargarChosenCliente(pagina);
        toggleControlesUbigeo();
        verificarSiExisteNuevaDireccionEntrega();
        verificarSiExisteDetalle();
        verificarSiExisteCliente();
        $("#btnBusquedaPedidos").click();
        var tipoPedido = $("#pedido_tipoPedido").val();
        validarTipoPedido(tipoPedido);

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
        if ($("#idCliente").val().trim() != "" && $("#pagina").val() == 1)
            $("#idCiudad").attr("disabled", "disabled");
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
                VARIACION_PRECIO_ITEM_PEDIDO = constantes.VARIACION_PRECIO_ITEM_PEDIDO;
            }
        });
    }

    function autoGuardarPedido() {
        $.ajax({
            url: "/Pedido/autoGuardarPedido",
            type: 'POST',
            error: function () {
                setTimeout(autoGuardarPedido, MILISEGUNDOS_AUTOGUARDADO);
            },
            success: function () {
                setTimeout(autoGuardarPedido, MILISEGUNDOS_AUTOGUARDADO);
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
        $('#pedido_direccionEntrega option').each(function () {
            if ($(this).val() == GUID_EMPTY) {
                $("#btnAgregarDireccion").attr("disabled", "disabled");
            }
        });
    }

    /**
     * ################################ INICIO CONTROLES DE CLIENTE
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


    var documentoVenta_fechaEmision = $("#documentoVenta_fechaEmisiontmp").val();
    $("#documentoVenta_fechaEmision").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", documentoVenta_fechaEmision);

    var documentoVenta_fechaVencimiento = $("#documentoVenta_fechaVencimientotmp").val();
    $("#documentoVenta_fechaVencimiento").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", documentoVenta_fechaVencimiento);


    /**
     * FIN DE CONTROLES DE FECHAS
     */


    
    





    $("#documentoVenta_serie").change(function () {

        var serie = $("#documentoVenta_serie").val();
        $.ajax({
            url: "/NotaCredito/ChangeSerie",
            type: 'POST',
            data: {
                serie: serie
            },
            success: function () { }
        });
    });


    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/NotaCredito/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }
    
    $("#venta_sustento").change(function () {
        changeInputString("sustento", $("#venta_sustento").val())
    });

    $("#venta_observaciones").change(function () {
        changeInputString("observaciones", $("#venta_observaciones").val())
    });
    
    

    $(document).on('click', "button.btnMostrarPrecios", function () {

        var idProducto = event.target.getAttribute("class").split(" ")[0];
        var idCliente = $("#idCliente").val();
        if (idCliente.trim() == "") {
            alert("Debe seleccionar un cliente.");
            $('#idCliente').trigger('chosen:activate');
            return false;
        }
        $.ajax({
            url: "/Precio/GetPreciosRegistradosVenta",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: idProducto,
                idCliente: idCliente
            },
            success: function (producto) {

                $("#verProducto").html(producto.nombre);

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

    function activarBotonesNotasCredito() {
        $("#btnFinalizarCreacionNotaCredito").removeAttr("disabled");
        $("#btnCancelarNotaCredito").removeAttr("disabled");
        $(".footable-show").removeAttr("disabled");
    }

    function desactivarBotonesNotasCredito() {
        $("#btnFinalizarCreacionNotaCredito").attr("disabled", "disabled");
        $("#btnCancelarNotaCredito").attr("disabled", "disabled");
        $(".footable-show").attr("disabled", "disabled");
    }

    $(".fechaEmision").change(function () {
        var fechaSolicitud = $("#documentoVenta_fechaEmision").val();
        var horaSolicitud = $("#documentoVenta_horaEmision").val();
        $.ajax({
            url: "/Pedido/ChangeFechaSolicitud",
            type: 'POST',
            data: {
                fechaSolicitud: fechaSolicitud,
                horaSolicitud: horaSolicitud
            },
            success: function () {
            }
        });
    });


    $("#btnFinalizarCreacionNotaCredito").click(function () {
       

        var sustento = $("#venta_sustento").val();
        if (sustento.trim().length < 25) {
            alert("El sustento debe contener al menos 25 caracteres.");
            $("#venta_sustento").focus();
            return false;
        }

        if ($("#documentoVenta_fechaEmision").val() == "" || $("#documentoVenta_fechaEmision").val() == null) {
            alert("Debe ingresar la fecha de emisión.");
            $("#documentoVenta_fechaEmision").focus();
            return false;
        }


        if (convertirFechaNumero($("#documentoVenta_fechaEmision").val()) < convertirFechaNumero($("#venta_documentoReferencia_fechaEmisionFormat").val())) {
            alert("La fecha de emisión de la nota de crédito no puede ser anterior a la fecha de emisión de la factura.");
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

        desactivarBotonesNotasCredito();

        $('body').loadingModal({
            text: 'Creando Nota de Crédito...'
        });
        $('body').loadingModal('show');

        $.ajax({
            url: "/NotaCredito/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                fechaEmision: fechaEmision,
                horaEmision: horaEmision,
                sustento: sustento,
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
                    $("#vpOBSERVACIONES").html($("#venta_observaciones").val());
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




    function desactivarBotonesConfirmarNotasCredito() {
        $("#btnCancelarConfirmarNotaCredito").attr("disabled", "disabled");
        $("#btnConfirmarGeneracionNotaCredito").attr("disabled", "disabled");
    }

    function activarBotonesConfirmarNotasCredito() {
        $("#btnCancelarConfirmarNotaCredito").removeAttr("disabled");
        $("#btnConfirmarGeneracionNotaCredito").removeAttr("disabled");
    }




    $("#btnConfirmarGeneracionNotaCredito").click(function () {


        desactivarBotonesConfirmarNotasCredito();


        $('body').loadingModal({
            text: 'Creando Nota de Crédito...'
        });
        $('body').loadingModal('show');
        
        var idDocumentoVenta = $("#idDocumentoVenta").val();

        $.ajax({
            url: "/NotaCredito/ConfirmarCreacion",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idDocumentoVenta: idDocumentoVenta
            },
            error: function (resultado) {
                $('body').loadingModal('hide')
                mostrarMensajeErrorProceso(MENSAJE_ERROR);
                activarBotonesConfirmarNotasCredito();
            },
            success: function (resultado) {
                $('body').loadingModal('hide')

                if (resultado.CPE_RESPUESTA_BE.CODIGO == "001") {
                    $.alert({
                        //icon: 'fa fa-warning',
                        title: TITLE_EXITO,
                        content: 'Se generó la Nota de Crédito ' + resultado.serieNumero + '.',
                        type: 'green',
                        buttons: {
                            OK: function () {
                                   window.location = '/Factura/Index';
                                    activarBotonesConfirmarNotasCredito();
                            }
                        }
                    });                    
                }
                else {
                    mostrarMensajeErrorProceso(MENSAJE_ERROR + ".\n" + "Detalle Error: " + resultado.CPE_RESPUESTA_BE.DETALLE);
                    //$("#btnAceptarFacturarPedido").removeAttr("disabled");
                    activarBotonesConfirmarNotasCredito();
                }

            }
        });
        $("btnCancelarConfirmarNotaCredito").click();
    });








    /*Evento que se dispara cuando se hace clic en el boton EDITAR en la edición de la grilla*/
    $(document).on('click', "button.footable-show", function () {


        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
            console.log('Esto es un dispositivo móvil');
            return;
        }

        //Cambiar estilos a los botones
        $("button.footable-add").attr("class", "btn btn-default footable-add");
        $("button.footable-hide").attr("class", "btn btn-primary footable-hide");

        $("#btnCancelarNotaCredito").attr('disabled', 'disabled');
        $("#btnFinalizarCreacionNotaCredito").attr('disabled', 'disabled');


        //llama a la función que cambia el estilo de display después de que la tabla se ha redibujado
        //Si se le llama antes el redibujo reemplazará lo definido
    //    window.setInterval(mostrarFlechasOrdenamiento, 600);

        var permiteEditarCantidades = $("#permiteEditarCantidades").val();
        var permiteEditarPrecios = $("#permiteEditarPrecios").val();
        var permiteEliminarLineas = $("#permiteEliminarLineas").val();


        if ($("#permiteEditarCantidades").val() == "True") {
            /*Se agrega control input en columna cantidad*/
            var $j_object = $("td.detcantidad");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var cantidad = value.innerText.trim();
                value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detincantidad form-control' value='" + cantidad + "' type='number'/>";
            });
        }
        else if ($("#permiteEditarPrecios").val() == "True")
        {
            var $j_object = $("td.detprecioUnitario");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var precioUnitario = value.innerText.trim();
                value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detinprecioUnitario form-control' value='" + precioUnitario + "' type='number'/>";
            });
        }
        //else if ($("#permiteEliminarLineas").val == "true") {


        //}

       
    


        /*Se agrega control input en columna porcentaje descuento*/
  /*      var $j_object1 = $("td.detporcentajedescuento");
        $.each($j_object1, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var porcentajedescuento = value.innerText;
            porcentajedescuento = porcentajedescuento.replace("%", "").trim();
            $(".detporcentajedescuentoMostrar." + arrId[0]).html("<div style='width: 150px' ><div style='float:left' ><input style='width: 100px' class='" + arrId[0] + " detinporcentajedescuento form-control' value='" + porcentajedescuento + "' type='number'/></div><div > <button type='button' class='" + arrId[0] + " btnCalcularDescuento btn btn-primary bouton-image monBouton' data-toggle='modal' data-target='#modalCalculadora' ></button ></div></div>");

        });*/


    /*    var $j_objectFlete = $("td.detflete");
        $.each($j_objectFlete, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var flete = value.innerText;
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detinflete form-control' value='" + flete + "' type='number'/>";
        });
        */


    });



    $("#btnCancelarNotaCredito").click(function () {

        window.location = '/Factura/Index';
    });



    /*Evento que se dispara cuando se hace clic en FINALIZAR en la edición de la grilla*/
    $(document).on('click', "button.footable-hide", function () {

        //Se habilitan controles
        $("#btnCancelarNotaCredito").removeAttr('disabled');
        $("#btnFinalizarCreacionNotaCredito").removeAttr('disabled');


        var json = "[ ";

        var permiteEditarCantidades = $("#permiteEditarCantidades").val();
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
        else
        {
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
            url: "/NotaCredito/ChangeDetalle",
            type: 'POST',
            data: json,
            dataType: 'json',
            contentType: 'application/json',
            success: function (respuesta) {
                location.reload();
            }
        });
    });
        

    $("#estado").change(function () {
        var estado = $("#estado").val();
        $.ajax({
            url: "/Pedido/changeEstado",
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
            url: "/Pedido/changeEstadoCrediticio",
            type: 'POST',
            data: {
                estadoCrediticio: estado
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#ActualDepartamento", function () {
        var ubigeoEntregaId = "000000";
        if ($("#ActualDepartamento").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualDepartamento").val() + "0000";
        }
        $.ajax({
            url: "/Pedido/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#ActualProvincia", function () {
        var ubigeoEntregaId = $("#ActualDepartamento").val() + "0000";
        if ($("#ActualProvincia").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualProvincia").val() + "00";
        }
        $.ajax({
            url: "/Pedido/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#ActualDistrito", function () {
        var ubigeoEntregaId = $("#ActualDepartamento").val() + $("#ActualProvincia").val() + "00";
        if ($("#ActualDistrito").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualDistrito").val();
        }
        $.ajax({
            url: "/Pedido/ChangeUbigeoEntrega",
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

            $('#pedido_direccionEntrega')
                .find('option')
                .remove()
                .end()
                ;

            $('#pedido_direccionEntrega').append($('<option>', {
                value: GUID_EMPTY,
                text: "Seleccione Dirección Entrega",
                direccion: "",
                contacto: "",
                telefono: ""
            }));


            for (var i = 0; i < direccionEntregaListTmp.length; i++) {
                $('#pedido_direccionEntrega').append($('<option>', {
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
            url: "/Pedido/ChangeIdCiudad",
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

    /****************** PROGRAMACION PEDIDO****************************/

    $('#modalProgramacion').on('shown.bs.modal', function () {
        var fechaProgramacion = $("#fechaProgramaciontmp").val();
        $("#fechaProgramacion").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaProgramacion);
    })

    $("#btnAceptarProgramarPedido").click(function () {

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
            var respuesta = confirm("¡ATENCIÓN! Está programando la atención del pedido en una fecha fuera del rango solicitado por el cliente.");
            if (!respuesta) {
                $("#fechaProgramacion").focus();
                return false;
            }
        }

        $.ajax({
            url: "/Pedido/Programar",
            type: 'POST',
            // dataType: 'JSON',
            data: {
                fechaProgramacion: fechaProgramacion,
                comentarioProgramacion: comentarioProgramacion
            },
            success: function (resultado) {
                alert('El pedido número ' + $("#verNumero").html() + ' se programó para ser atendido.');
                location.reload();
            }
        });
        $("btnCancelarProgramarPedido").click();
    });




    $('#documentoVenta_fechaEmision').change(function () {
        var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate');
        $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
        calcularFechaVencimiento();

    });


    $('#documentoVenta_observacionesUsoInterno').change(function () {
        var observacionesUsoInterno = $("#documentoVenta_observacionesUsoInterno").val();
        $.ajax({
            url: "/NotaCredito/ChangeObservacionesUsoInterno",
            type: 'POST',
            data: {
                observacionesUsoInterno: observacionesUsoInterno
            },
            success: function () { }
        });
    });

    function calcularFechaVencimiento() {
        var tipoPago = $('#tipoPago').val();
        switch (tipoPago) {
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
        }
    }


    $('#tipoPago').change(function () {

        calcularFechaVencimiento();


    });


    $("#btnAceptarFacturarPedido").click(function () {

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
        /*
         if ($("#documentoVenta_correoEnvio").val() == "" || $("#documentoVenta_correoEnvio").val() == null) {
             alert("Debe ingresar el correo de envío.");
             $("#documentoVenta_correoEnvio").focus();
             return false;
         }
         else
         {
             if (!$("#documentoVenta_correoEnvio").val().match(/^[a-zA-Z0-9\._-]+@[a-zA-Z0-9-]{2,}[.][a-zA-Z]{2,4}$/)) 
             {
                 alert("Debe ingresar un correo de envío válido.");
                 $("#documentoVenta_correoEnvio").focus();
                 return false;
             }
         }
 
         */


        var fechaEmision = $("#documentoVenta_fechaEmision").val();
        var horaEmision = $("#documentoVenta_horaEmision").val();
        var fechaVencimiento = $("#documentoVenta_fechaVencimiento").val();
        var observaciones = $("#documentoVenta_observaciones").val();
        var tipoPago = $("#tipoPago").val();
        var formaPago = $("#formaPago").val();
        /*var correoCopia = $("#documentoVenta_correoCopia").val();
        var correoOculto = $("#documentoVenta_correoOculto").val();*/

        /*
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
            var respuesta = confirm("¡ATENCIÓN! Está programando la atención del pedido en una fecha fuera del rango solicitado por el cliente.");
            if (!respuesta) {
                $("#fechaProgramacion").focus();
                return false;
            }
        }*/

        //   var fechaProgramacion = $('#fechaProgramacion').val();
        $('body').loadingModal({
            text: 'Creando Factura...'
        });
        $.ajax({
            url: "/Factura/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                fechaEmision: fechaEmision,
                horaEmision: horaEmision,
                fechaVencimiento: fechaVencimiento,
                tipoPago: tipoPago,
                formaPago: formaPago,
                observaciones: observaciones
                /*correoCopia: correoCopia,
                correoOculto: correoOculto*/
            },
            error: function (resultado) {
                $('body').loadingModal('hide')
                alert(MENSAJE_ERROR);

            },
            success: function (resultado) {
                $('body').loadingModal('hide')

                if (resultado.CPE_RESPUESTA_BE.CODIGO == "001") {
                    alert('Se generó la factura ' + resultado.serieNumero + '.');
                }
                else {
                    alert(MENSAJE_ERROR);
                }





                location.reload();
            }
        });
        $("btnCancelarFacturarPedido").click();
    });





    $('#modalFacturar').on('shown.bs.modal', function () {




        // $("#documentoVenta_fechaEmision").val();



        /*  $('#familia').focus();
          $('#familia').val("Todas");
          $('#proveedor').val("Todos");
          */
        //$('#producto').trigger('chosen:activate');
    })


    $(document).on('click', "button.btnMostrarPrecios", function () {

        var idProducto = event.target.getAttribute("class").split(" ")[0];

        var idCliente = $("#idCliente").val();

        /*

        if ($("#pagina").val() == 2) {
            idCliente = $("#verIdCliente").val();
            sessionPedido = "pedidoVer";
        }
        else {
            idCliente = $("#idCliente").val();
            if (idCliente.trim() == "") {
                alert("Debe seleccionar un cliente.");
                $('#idCliente').trigger('chosen:activate');
                return false;
            }
        }*/

        //verIdCliente


        $.ajax({
            url: "/Precio/GetPreciosRegistradosVenta",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: idProducto,
                idCliente: idCliente,
                controller: "venta"
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








    /****************** FIN PROGRAMACION PEDIDO****************************/
});