

//CONSTANTES POR DEFECTO
var cantidadDecimales = 2;
var cantidadCuatroDecimales = 4;
var IGV = 0.18;
var SIMBOLO_SOL = "S/";
var MILISEGUNDOS_AUTOGUARDADO = 5000;
var VARIACION_PRECIO_ITEM_PEDIDO = 0.01;





/**
 * Constantes para toma de pedidos
 */
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
var ESTADO_FACTURADO_PARCIALMENTE = 9;


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


var CONS_TIPO_DOC_CLIENTE_RUC = "6";
var CONS_TIPO_DOC_CLIENTE_DNI= "1";




var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";



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