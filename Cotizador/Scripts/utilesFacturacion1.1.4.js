$('#documentoVenta_fechaEmision').change(function () {
    var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate');
    $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
    calcularFechaVencimiento();
});

function calcularFechaVencimiento() {
    var tipoPago = $('#tipoPago').val();
    $('#documentoVenta_fechaVencimiento').attr("disabled", "disabled");
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

$('#formaPago').change(function () {
    var tipoPago = $("#tipoPago").val();
    var formaPago = $("#formaPago").val();
    if (tipoPago == 1 && formaPago == 3) {
        $.alert({
            title: 'Forma de Pago Incorrecta',
            type: 'red',
            content: 'La Forma de Pago no puede ser LETRA si ha seleccionado el Tipo de Pago CONTADO.',
            buttons: {
                OK: function () { }
            }
        });
        $("#formaPago").val(0);
    }
    
});


function desactivarBotonesFacturar() {
    $("#btnCancelarFacturarPedido").attr('disabled', 'disabled');
    $("#btnEditarCliente").attr('disabled', 'disabled');
    $("#btnEditarVenta").attr('disabled', 'disabled');
    $("#btnEditarVentaConsolidada").attr('disabled', 'disabled');
    $("#btnAceptarFacturarPedido").attr('disabled', 'disabled');
    $("#btnConfirmarFacturarPedido").attr('disabled', 'disabled');
    $("#btnCancelarConfirmarFacturarPedido").attr('disabled', 'disabled');
    $("#btnConfirmarFacturaConsolidada").attr('disabled', 'disabled');
}

function activarBotonesFacturar() {
    $("#btnCancelarFacturarPedido").removeAttr('disabled');
    $("#btnEditarCliente").removeAttr('disabled');
    $("#btnEditarVenta").removeAttr('disabled');
    $("#btnEditarVentaConsolidada").removeAttr('disabled');
    $("#btnAceptarFacturarPedido").removeAttr('disabled');
    $("#btnConfirmarFacturarPedido").removeAttr('disabled');
    $("#btnCancelarConfirmarFacturarPedido").removeAttr('disabled');
    $("#btnConfirmarFacturaConsolidada").removeAttr('disabled');
}



$("#btnAceptarFacturarPedido").click(function () {

    if ($("#pedido_cliente_tipoDocumento").val() == CONS_TIPO_DOC_CLIENTE_RUC) {
        if ($("#verRazonSocialSunat").html() == "") {
            alert("No se han obtenido los datos del cliente desde SUNAT.");
            return false;
        }
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
    
    if ($("#tipoPago").val() == 0) {
        $("#tipoPago").focus();
        $.alert({
            title: 'Validación',
            type: 'orange',
            content: 'La Condición de Pago NO ASIGNADO es inválido. Conctate con el área de Créditos para que apruebe la Condición de Pago.',
            buttons: {
                OK: function () { }
            }
        });
        return false;
    }
    

    if ($("#verNumeroReferenciaCliente").html().length > 20) {
        $("#numeroReferenciaCliente").focus();
        $.alert({
            title: 'Validación',
            type: 'orange',
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

   /* $('body').loadingModal({
        text: 'Creando Factura...'
    });*/

    $('body').loadingModal('text', 'Creando Factura...');
    $('body').loadingModal('show')

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


                if (documentoVenta.tipoDocumento == CONS_TIPO_DOC_BOLETA) {
                    $("#modalVistaPreviaFacturaTitle").html("<b> Vista Previa Boleta</b>");
                    $("#btnConfirmarFacturarPedido").html("Confirmar Generación Boleta");
                }
                else if (documentoVenta.tipoDocumento == CONS_TIPO_DOC_FACTURA) {
                    $("#modalVistaPreviaFacturaTitle").html("<b> Vista Previa Factura</b>");
                    $("#btnConfirmarFacturarPedido").html("Confirmar Generación Factura");
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


    //Si el tipoi de documento cliente es RUC entonces se debe validar los datos con SUNAT
    if ($("#pedido_cliente_tipoDocumento").val() == CONS_TIPO_DOC_CLIENTE_RUC) {   
        if ($("#verRazonSocialSunat").html() == "") {
        alert("No se han obtenido los datos del cliente desde SUNAT.");
        return false;
        }
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


    $('body').loadingModal('text', 'Confirmando Generación Factura...');
    $('body').loadingModal('show')


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
                    content: 'Se generó el documento electrónico: ' + resultado.serieNumero + '.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            var actionPostCPE = $("#actionPostCPE").val();

                            if (actionPostCPE.length > 3) {
                                execActionPostCPE(actionPostCPE);
                            } else {
                                location.reload();
                            }
                        }
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






$("#btnConfirmarFacturaConsolidada").click(function () {
 
    desactivarBotonesFacturar(); 

   
    $('body').loadingModal('text', 'Confirmando Generación Factura...');
    $('body').show();

    var idMovimientoAlmacen = $("#idMovimientoAlmacen").val();
    var idDocumentoVenta = $("#idDocumentoVenta").val();

    $.ajax({
        url: "/Factura/ConfirmarCreacionFacturaConsolidada",
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
                    content: 'Se generó el documento electrónico: ' + resultado.serieNumero + '.',
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


    //////REVISAR


});


/*Evento que se dispara cuando se modifica un control de cantidad de la grilla*/
$(document).on('change', "input.detincantidad", function () {
    var idproducto = event.target.getAttribute("class").split(" ")[0];
    var cantidad = Number($("." + idproducto + ".detincantidad").val());
    var precioUnitario = $("." + idproducto + ".detprecioUnitario").html();
    var subTotal = precioUnitario * cantidad;
    //Se asigna el subtotal 
    $("." + idproducto + ".detsubtotal").html(subTotal.toFixed(cantidadDecimales));
    var subTotalDocumento = 0;
    var $j_object = $("td.detprecioUnitario");
    $.each($j_object, function (key, value) {
        var arrId = value.getAttribute("class").split(" ");
        var precioUnitarioTmp = Number($("." + arrId[0] + ".detprecioUnitario").html());
        var cantidadTmp = Number($("." + arrId[0] + ".detincantidad").val());
        subTotalDocumento = subTotalDocumento + Number(Number((precioUnitarioTmp * cantidadTmp)).toFixed(cantidadDecimales));
    });
    calcularSubtotalGrilla(idproducto, cantidad, precioUnitario, subTotalDocumento);
});


/*Evento que se dispara cuando se modifica un control de cantidad de la grilla*/
$(document).on('change', "input.detinprecioUnitario", function () {
    var idproducto = event.target.getAttribute("class").split(" ")[0];
    var precioUnitario = Number($("." + idproducto + ".detinprecioUnitario").val());
    var cantidad = $("." + idproducto + ".detcantidad").html();
    var subTotal = precioUnitario * cantidad;
    //Se asigna el subtotal 
    $("." + idproducto + ".detsubtotal").html(subTotal.toFixed(cantidadDecimales));
    var subTotalDocumento = 0;
    var $j_object = $("td.detcantidad");
    $.each($j_object, function (key, value) {
        var arrId = value.getAttribute("class").split(" ");
        var precioUnitarioTmp = Number($("." + arrId[0] + ".detinprecioUnitario").val());
        var cantidadTmp = Number($("." + arrId[0] + ".detcantidad").html());
        subTotalDocumento = subTotalDocumento + Number(Number((precioUnitarioTmp * cantidadTmp)).toFixed(cantidadDecimales));
    });
    calcularSubtotalGrilla(idproducto, cantidad, precioUnitario, subTotalDocumento);
});




function calcularSubtotalGrilla(idproducto, cantidad, precioUnitario, subTotalDocumento) {
    //Se obtiene el porcentaje descuento 
    var porcentajeDescuento = 0;
    var flete = 0;
    var precioLista = 0;
    var precio = precioUnitario;

    //Se calcula el margen
    var costo = 0;// Number($("." + idproducto + ".detcostoLista").html());
    var margen = 0;//(1 - (Number(costo) / Number(precio))) * 100;        

    var subTotal = subTotalDocumento;
    var igv = 0;
    var total = 0;

    /*   var incluidoIGV = $("input[name=igv]:checked").val();
       //Si no se etsá incluyendo IGV se le agrega
       if (incluidoIGV == "0") {*/
    igv = Number((subTotal * IGV).toFixed(cantidadDecimales));
    total = subTotal + (igv);
    /*   }
       //Si se está incluyendo IGV entonces se 
       else {
           total = subTotal;
           subTotal = Number((subTotal / (1 + IGV)).toFixed(cantidadDecimales));
           igv = total - subTotal;
       }*/

    $('#montoSubTotal').html(subTotal.toFixed(cantidadDecimales));
    $('#montoIGV').html(igv.toFixed(cantidadDecimales));
    $('#montoTotal').html(total.toFixed(cantidadDecimales));
};



function execActionPostCPE(actionPostCPE) {
    if (actionPostCPE == "recargarGuia") {
        $('.modal').modal('hide');

        setTimeout(function () {
            var idGuia = $("#actionPostCPE").attr("idGuia");
            $(".btnVerGuiaRemision." + idGuia).click();
        }, 500);
    }
};

