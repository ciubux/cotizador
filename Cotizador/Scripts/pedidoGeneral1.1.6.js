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

$("#btnAtenderDiferidoPedidoVenta").click(function () {
    btnAtenderPedido('VD');
});


$("#btnAtenderPedidoVenta").click(function () {
    btnAtenderPedido('V');
});

$("#btnAtenderPedidoCompra").click(function () {
    btnAtenderPedido('C');
});

$("#btnAtenderPedidoAlmacen").click(function () {
    btnAtenderPedido('A');
});



function btnAtenderPedido(tipo) {
    desactivarBotonesVer();
    var idPedido = $("#idPedido").val();

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
                    url: "/GuiaRemision/iniciarAtencionDesdePedido",
                    type: 'POST',
                    data: { tipo: tipo },
                    error: function (detalle) {
                        $('body').loadingModal('hide')
                        alert("Ocurrió un problema al iniciar la atención del pedido.");
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

$("#btnIngresarPedidoVenta").click(function () {
    btnIngresarPedido('V');
});

$("#btnIngresarPedidoCompra").click(function () {
    btnIngresarPedido('C');
});

$("#btnIngresarPedidoAlmacen").click(function () {
    btnIngresarPedido('A');
});

function btnIngresarPedido(tipo) {
    desactivarBotonesVer();
    var idPedido = $("#idPedido").val();

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
                    url: "/NotaIngreso/iniciarAtencionDesdePedido",
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
    $("#btnCancelarVerPedido").attr('disabled', 'disabled');
    $("#btnEditarPedido").attr('disabled', 'disabled');
    $("#btnActualizarPedido").attr('disabled', 'disabled');
    $("#btnAprobarIngresoPedido").attr('disabled', 'disabled');
    $("#btnDenegarIngresoPedido").attr('disabled', 'disabled');
    $("#btnProgramarPedido").attr('disabled', 'disabled');
    $("#btnFacturarPedido").attr('disabled', 'disabled');
    $("#btnIngresarPedidoVenta").attr('disabled', 'disabled');
    $("#btnAtenderPedidoVenta").attr('disabled', 'disabled');
    $("#btnIngresarPedidoCompra").attr('disabled', 'disabled');
    $("#btnAtenderPedidoCompra").attr('disabled', 'disabled');
    $("#btnCancelarProgramacionPedido").attr('disabled', 'disabled');
    $("#btnLiberarPedido").attr('disabled', 'disabled');
    $("#btnBloquearPedido").attr('disabled', 'disabled');
    $("#btnVerAtenciones").attr('disabled', 'disabled');
    $("#btnEliminarPedido").attr('disabled', 'disabled');
    $("#btnEditarVenta").attr('disabled', 'disabled');

}

function activarBotonesVer() {
    $("#btnCancelarVerPedido").removeAttr('disabled');
    $("#btnEditarPedido").removeAttr('disabled');
    $("#btnActualizarPedido").removeAttr('disabled');
    $("#btnAprobarIngresoPedido").removeAttr('disabled');
    $("#btnDenegarIngresoPedido").removeAttr('disabled');
    $("#btnIngresarPedidoVenta").removeAttr('disabled');
    $("#btnAtenderPedidoVenta").removeAttr('disabled');
    $("#btnIngresarPedidoCompra").removeAttr('disabled');
    $("#btnAtenderPedidoCompra").removeAttr('disabled');
    $("#btnProgramarPedido").removeAttr('disabled');
    $("#btnFacturarPedido").removeAttr('disabled');
    $("#btnCancelarProgramacionPedido").removeAttr('disabled');
    $("#btnLiberarPedido").removeAttr('disabled');
    $("#btnBloquearPedido").removeAttr('disabled');
    $("#btnVerAtenciones").removeAttr('disabled');
    $("#btnEliminarPedido").removeAttr('disabled');
    $("#btnEditarVenta").removeAttr('disabled');
}



$("#tipoPedidoBusqueda").change(function () {
    var tipoPedidoBusqueda = $("#tipoPedidoBusqueda").val();
    var pagina = $("#pagina").val();

    var Controlador = "";
    if (pagina == PAGINA_BUSQUEDA_PEDIDOS_VENTA)
        var Controlador = "Pedido";
    else if (pagina == PAGINA_BUSQUEDA_PEDIDOS_COMPRA)
        var Controlador = "PedidoCompra";
    else
        var Controlador = "PedidoAlmacen";

    $.ajax({
        url: "/" + Controlador + "/ChangeTipoPedidoBusqueda",
        type: 'POST',
        data: {
            tipoPedidoBusqueda: tipoPedidoBusqueda
        },
        success: function () { }
    });
});