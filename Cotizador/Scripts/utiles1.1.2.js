
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


function sumarHoras(hora, cantidad) {
    var hora = hora.split(":");

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