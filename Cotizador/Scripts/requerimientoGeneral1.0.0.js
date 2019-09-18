var TIPO_PEDIDO_VENTA_VENTA = 'V'; 
//var TIPO_PEDIDO_VENTA_TRASLADO_INTERNO_ENTREGADO = 'T'; 
var TIPO_PEDIDO_VENTA_COMODATO_ENTREGADO = 'M';
var TIPO_PEDIDO_VENTA_TRANSFERENCIA_GRATUITA_ENTREGADA = 'G'; 
//var TIPO_PEDIDO_VENTA_PRESTAMO_ENTREGADO = 'P'; 
var TIPO_PEDIDO_VENTA_DEVOLUCION_VENTA = 'D';   //GENERA NOTA DE CREDITO
//var TIPO_PEDIDO_VENTA_DEVOLUCION_PRESTAMO_ENTREGADO = 'E';  
var TIPO_PEDIDO_VENTA_DEVOLUCION_COMODATO_ENTREGADO = 'F'; 
var TIPO_PEDIDO_VENTA_DEVOLUCION_TRANSFERENCIA_GRATUITA_ENTREGADA = 'H'; 


var TIPO_PEDIDO_COMPRA_COMPRA = 'C'; 
//var TIPO_PEDIDO_COMPRA_TRASLADO_INTERNO_RECIBIDO = 'T'; 
var TIPO_PEDIDO_COMPRA_COMODATO_RECIBIDO = 'M';
var TIPO_PEDIDO_COMPRA_TRANSFERENCIA_GRATUITA_RECIBIDA = 'G'; 
//var TIPO_PEDIDO_COMPRA_PRESTAMO_RECIBIDO = 'P'; 
var TIPO_PEDIDO_COMPRA_DEVOLUCION_COMPRA = 'B';   
//var TIPO_PEDIDO_COMPRA_DEVOLUCION_PRESTAMO_RECIBIDO = 'E';  
var TIPO_PEDIDO_COMPRA_DEVOLUCION_COMODATO_RECIBIDO = 'F'; 
var TIPO_PEDIDO_COMPRA_DEVOLUCION_TRANSFERENCIA_GRATUITA_RECIBIDA = 'H'; 


var TIPO_PEDIDO_ALMACEN_TRASLADO_INTERNO_A_ENTREGAR = 'T';
var TIPO_PEDIDO_ALMACEN_TRASLADO_INTERNO_A_RECIBIR = 'I';
var TIPO_PEDIDO_ALMACEN_PRESTAMO_A_ENTREGAR = 'P';
var TIPO_PEDIDO_ALMACEN_DEVOLUCION_PRESTAMO_ENTREGADO = 'D';
var TIPO_PEDIDO_ALMACEN_PRESTAMO_A_RECIBIR = 'R';
var TIPO_PEDIDO_ALMACEN_DEVOLUCION_PRESTAMO_RECIBIDO = 'E';
var TIPO_PEDIDO_ALMACEN_TRASLADO_EXTORNO_GUIA_REMISION = 'X';


/**
 * ######################## INICIO CONTROLES DE FECHAS
 */
/*
jQuery(function ($) {

}
*/


  
$("#btnAgregarCliente").click(function () {
    window.open(
        "/Cliente/Editar?idCliente=" + GUID_EMPTY,
        "Creación de Cliente",
        "resizable,scrollbars,status"
    );
});

$("#btnAtenderRequerimientoVenta").click(function () {
    btnAtenderRequerimiento('V');
});

$("#btnAtenderRequerimientoCompra").click(function () {
    btnAtenderRequerimiento('C');
});

$("#btnAtenderRequerimientoAlmacen").click(function () {
    btnAtenderRequerimiento('A');
});



function btnAtenderRequerimiento(tipo) {
    desactivarBotonesVer();
    var idRequerimiento = $("#idRequerimiento").val();

    $.ajax({
        url: "/GuiaRemision/ConsultarSiExisteGuiaRemision",
        type: 'POST',
        async: false,
        success: function (resultado) {
            activarBotonesVer();
            if (resultado == "False") {
                $('body').loadingModal({
                    text: 'Abriendo Crear Guía Remisión...'
                });
                $.ajax({
                    url: "/GuiaRemision/iniciarAtencionDesdeRequerimiento",
                    type: 'POST',
                    data: { tipo: tipo },
                    error: function (detalle) {
                        $('body').loadingModal('hide')
                        alert("Ocurrió un problema al iniciar la atención del requerimiento.");
                    },
                    success: function (fileName) {
                        $('body').loadingModal('hide')
                        window.location = '/GuiaRemision/Guiar';
                    }
                });
            }
            else {
                alert("Existe una Guia de Remisión abierta; por favor vaya a la pantalla Guia Remisión, haga clic en cancelar y vuelva a intentarlo.");
                activarBotonesVer();
            }
        }
    });
}

$("#btnIngresarRequerimientoVenta").click(function () {
    btnIngresarRequerimiento('V');
});

$("#btnIngresarRequerimientoCompra").click(function () {
    btnIngresarRequerimiento('C');
});

$("#btnIngresarRequerimientoAlmacen").click(function () {
    btnIngresarRequerimiento('A');
});

function btnIngresarRequerimiento(tipo) {
    desactivarBotonesVer();
    var idRequerimiento = $("#idRequerimiento").val();

    $.ajax({
        url: "/NotaIngreso/ConsultarSiExisteNotaIngreso",
        type: 'POST',
        async: false,
        success: function (resultado) {
            activarBotonesVer();
            if (resultado == "False") {
                $('body').loadingModal({
                    text: 'Abriendo Crear Nota Ingreso...'
                });
                $.ajax({
                    url: "/NotaIngreso/iniciarAtencionDesdeRequerimiento",
                    type: 'POST',
                    data: { tipo: tipo },
                    error: function (detalle) {
                        $('body').loadingModal('hide')
                        alert("Ocurrió un problema.");
                    },
                    success: function (fileName) {
                        $('body').loadingModal('hide')
                        window.location = '/NotaIngreso/Ingresar';
                    }
                });
            }
            else {
                alert("Existe una Nota de Ingreso abierta; por favor vaya a la pantalla Nota de Ingreso, haga clic en cancelar y vuelva a intentarlo.");
                activarBotonesVer();
            }
        }
    });
}


function desactivarBotonesVer() {
    $("#btnCancelarVerRequerimiento").attr('disabled', 'disabled');
    $("#btnEditarRequerimiento").attr('disabled', 'disabled');
    $("#btnActualizarRequerimiento").attr('disabled', 'disabled');
    $("#btnAprobarIngresoRequerimiento").attr('disabled', 'disabled');
    $("#btnDenegarIngresoRequerimiento").attr('disabled', 'disabled');
    $("#btnProgramarRequerimiento").attr('disabled', 'disabled');
    $("#btnFacturarRequerimiento").attr('disabled', 'disabled');
    $("#btnIngresarRequerimientoVenta").attr('disabled', 'disabled');
    $("#btnAtenderRequerimientoVenta").attr('disabled', 'disabled');
    $("#btnIngresarRequerimientoCompra").attr('disabled', 'disabled');
    $("#btnAtenderRequerimientoCompra").attr('disabled', 'disabled');
    $("#btnCancelarProgramacionRequerimiento").attr('disabled', 'disabled');
    $("#btnLiberarRequerimiento").attr('disabled', 'disabled');
    $("#btnBloquearRequerimiento").attr('disabled', 'disabled');
    $("#btnVerAtenciones").attr('disabled', 'disabled');
    $("#btnEliminarRequerimiento").attr('disabled', 'disabled');
    $("#btnEditarVenta").attr('disabled', 'disabled');

}

function activarBotonesVer() {
    $("#btnCancelarVerRequerimiento").removeAttr('disabled');
    $("#btnEditarRequerimiento").removeAttr('disabled');
    $("#btnActualizarRequerimiento").removeAttr('disabled');
    $("#btnAprobarIngresoRequerimiento").removeAttr('disabled');
    $("#btnDenegarIngresoRequerimiento").removeAttr('disabled');
    $("#btnIngresarRequerimientoVenta").removeAttr('disabled');
    $("#btnAtenderRequerimientoVenta").removeAttr('disabled');
    $("#btnIngresarRequerimientoCompra").removeAttr('disabled');
    $("#btnAtenderRequerimientoCompra").removeAttr('disabled');
    $("#btnProgramarRequerimiento").removeAttr('disabled');
    $("#btnFacturarRequerimiento").removeAttr('disabled');
    $("#btnCancelarProgramacionRequerimiento").removeAttr('disabled');
    $("#btnLiberarRequerimiento").removeAttr('disabled');
    $("#btnBloquearRequerimiento").removeAttr('disabled');
    $("#btnVerAtenciones").removeAttr('disabled');
    $("#btnEliminarRequerimiento").removeAttr('disabled');
    $("#btnEditarVenta").removeAttr('disabled');
}



$("#tipoRequerimientoBusqueda").change(function () {
    var tipoRequerimientoBusqueda = $("#tipoRequerimientoBusqueda").val();
    var pagina = $("#pagina").val();

    var Controlador = "";
    if (pagina == PAGINA_BUSQUEDA_PEDIDOS_VENTA)
        var Controlador = "Requerimiento";
    else if (pagina == PAGINA_BUSQUEDA_PEDIDOS_COMPRA)
        var Controlador = "RequerimientoCompra";
    else
        var Controlador = "RequerimientoAlmacen";

    $.ajax({
        url: "/" + Controlador + "/ChangeTipoRequerimientoBusqueda",
        type: 'POST',
        data: {
            tipoRequerimientoBusqueda: tipoRequerimientoBusqueda
        },
        success: function () { }
    });
});