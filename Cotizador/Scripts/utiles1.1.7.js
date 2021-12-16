

//CONSTANTES POR DEFECTO
var cantidadDecimales = 2;
var cantidadCuatroDecimales = 4;
var IGV = 0.18;
var SIMBOLO_SOL = "S/";
var MILISEGUNDOS_AUTOGUARDADO = 5000;
var VARIACION_PRECIO_ITEM_PEDIDO = 0.01;

var MOTIVO_TRASLADO_SALIDA_VENTA = "V";
var MOTIVO_TRASLADO_SALIDA_TRANSFERENCIA_GRATUITA = "G";
var MOTIVO_TRASLADO_SALIDA_COMODATO = "M";
var MOTIVO_TRASLADO_SALIDA_PRESTAMO = "P";



var MOTIVO_TRASLADO_SALIDA_TRASLADO_INTERNO = "T";
var MOTIVO_TRASLADO_SALIDA_DEVOLUCION_COMPRA = "B";
var MOTIVO_TRASLADO_SALIDA_DEVOLUCION_PRESTAMO_RECIBIDO = "E";
var MOTIVO_TRASLADO_SALIDA_DEVOLUCION_COMODATO_RECIBIDO = "F";
var MOTIVO_TRASLADO_SALIDA_DEVOLUCION_TRANSFERENCIA_GRATUITA_RECIBIDA = "H";


var MOTIVO_TRASLADO_ENTRADA_COMPRA = "C";
var MOTIVO_TRASLADO_ENTRADA_TRANSFERENCIA_GRATUITA = "G";

var MOTIVO_TRASLADO_ENTRADA_DEVOLUCION_VENTA = "D";
var MOTIVO_TRASLADO_ENTRADA_DEVOLUCION_PRESTAMO_ENTREGADO = "P";
var MOTIVO_TRASLADO_ENTRADA_DEVOLUCION_COMODATO_ENTREGADO = "F";
var MOTIVO_TRASLADO_ENTRADA_DEVOLUCION_TRANSFERENCIA_GRATUITA_ENTREGADA = "H";






var EMAIL_REGEX = /^[-\w.%+]{1,64}@(?:[A-Z0-9-]{1,63}\.){1,125}[A-Z]{2,63}$/i;


var TIPO_NOTA_CREDITO_ANULACION_DE_LA_OPERACION = "1";
var TIPO_NOTA_CREDITO_DESCUENTO_GLOBAL = "4";
var TIPO_NOTA_CREDITO_AJUSTES = "13";

var TIPO_NOTA_DEBITO_INTERESES_POR_MORA = "1";
var TIPO_NOTA_DEBITO_AUMENTO_VALOR = "2";
var TIPO_NOTA_DEBITO_PENALIDADES = "3";





var MOV_TIPO_EXTORNO_SIN_EXTORNO = 0;
/*var MOV_TIPO_EXTORNO_EXTORNO_TOTAL = 1;
var MOV_TIPO_EXTORNO_EXTORNO_TOTAL = 1;*/
var MOV_TIPO_EXTORNO_EXTORNO_PARCIAL = 7;

var TITLE_EXITO = 'Operación Realizada';

var PAGINA_BUSQUEDA_COTIZACIONES = 0;
var PAGINA_MANTENIMIENTO_COTIZACION = 1;
var PAGINA_BUSQUEDA_PEDIDOS_VENTA = 2;
var PAGINA_MANTENIMIENTO_PEDIDO_VENTA = 3;
var PAGINA_BusquedaGuiasRemision = 4;
var PAGINA_MantenimientoGuiaRemision = 5;
var PAGINA_BusquedaFacturas = 6;
var PAGINA_MantenimientoFactura = 7;
var PAGINA_BusquedaBoletas = 8;
var PAGINA_MantenimientoBoleta = 9;
var PAGINA_BusquedaNotasCredito = 10;
var PAGINA_MantenimientoNotaCredito = 11;
var PAGINA_BusquedaNotasDebito = 12;
var PAGINA_MantenimientoNotaDebito = 13;
var PAGINA_ImprimirGuiaRemision = 14;
var PAGINA_BusquedaVentas = 15;
var PAGINA_MantenimientoVenta = 16;
var PAGINA_BusquedaClientes = 17;
var PAGINA_MantenimientoCliente = 18;
var PAGINA_BusquedaGuiasRemisionConsolidarFactura = 19;
var PAGINA_BusquedaNotasIngreso = 20;
var PAGINA_MantenimientoNotaIngreso = 21;
var PAGINA_ImprimirNotaIngreso = 22;
var PAGINA_BUSQUEDA_PEDIDOS_COMPRA = 23;
var PAGINA_MANTENIMIENTO_PEDIDO_COMPRA = 24;
var PAGINA_BUSQUEDA_PEDIDOS_ALMACEN = 25;
var PAGINA_MANTENIMIENTO_PEDIDO_ALMACEN = 26;

var PAGINA_MANTENIMIENTO_COTIZACION_GRUPAL = 35;
/**
 * Constantes para toma de pedidos
 */


var PAGINA_BUSQUEDA_ORDEN_COMPRA_CLIENTE = 250;
var PAGINA_MANTENIMIENTO_ORDEN_COMPRA_CLIENTE = 251;







var ESTADO_PENDIENTE_APROBACION = 0;
var ESTADO_INGRESADO = 1;
var ESTADO_DENEGADO = 2;
var ESTADO_PROGRAMADO = 3;
var ESTADO_ATENDIDO = 4;
var ESTADO_ATENDIDO_PARCIALMENTE = 5;
var ESTADO_EN_EDICION = 6;
var ESTADO_ELIMINADO = 7;
var ESTADO_FACTURADO = 8;
var ESTADO_FACTURADO_PARCIALMENTE = 9;
var ESTADO_RECIBIDO = 10;
var ESTADO_RECIBIDO_PARCIALMENTE = 11;


//Etiquetas de estadps para búsqueda de Pedidos
var ESTADO_PENDIENTE_APROBACION_STR = "Pendiente de Aprobación de Ingreso";
var ESTADO_INGRESADO_STR = "Pedido Ingresado";
var ESTADO_DENEGADO_STR = "Pedido Denegado";
var ESTADO_PROGRAMADO_STR = "Pedido Programado"
var ESTADO_ATENDIDO_STR = "Pedido Atendido"
var ESTADO_ATENDIDO_PARCIALMENTE_STR = "Pedido Atendido Parcialmente"
var ESTADO_EN_EDICION_STR = "Pedido En Edicion";
var ESTADO_ELIMINADO_STR = "Pedido Eliminado";
var ESTADO_FACTURADO_STR = "Pedido Facturado";
var ESTADO_FACTURADO_PARCIALMENTE_STR = "Pedido Facturado Parcialmente";
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


/**
 * Constantes para Facturacion
 */


var CONS_TIPO_DOC_NOTA_CREDITO = 7;
var CONS_TIPO_DOC_NOTA_DEBITO = 8;
var CONS_TIPO_DOC_FACTURA = 1;
var CONS_TIPO_DOC_BOLETA = 3;


var CONS_TIPO_DOC_CLIENTE_RUC = 6;
var CONS_TIPO_DOC_CLIENTE_DNI= 1;
var CONS_TIPO_DOC_CLIENTE_CARNET_EXTRANJERIA = 4;



//var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";





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

function saveByteArray(reportName, byte) {
    var blob = new Blob([byte]);
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    var fileName = reportName;
    link.download = fileName;
    link.click();
};


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

var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
var TITLE_EXITO = 'Operación Realizada';



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


function sumarHoras(horatmp, cantidad) {
    var hora = horatmp.split(":");

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


function mostrarMensajeErrorProceso(mensajeError) {
    $.alert({
        //icon: 'fa fa-warning',
        title: 'Error',
        content: mensajeError,
        type: 'red',
        buttons: {
            OK: function () { }
        }
    });
}


function esEntero(numero) {
    return numero % 1 == 0;
}


function cargarArchivosAdjuntos(files) {
    var numFiles = files.length;
    var nombreArchivos = "";
    for (i = 0; i < numFiles; i++) {

        var fileFound = 0;
        $("#nombreArchivos > li").each(function (index) {

            if ($(this).find("a.descargar")[0].text == files[i].name) {
                $.alert({
                    title: 'Error',
                    content: 'El archivo "' + files[i].name + '" ya se encuentra agregado.',
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });

                fileFound = 1;
            }
        });

        if (fileFound == 0) {

            var liHTML = '<a href="javascript:mostrar();" class="descargar">' + files[i].name + '</a>' +
                '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + files[i].name + '" class="btnDeleteArchivo" /></a>';

            $('#nombreArchivos').append($('<li />').html(liHTML));
        }
    }


}