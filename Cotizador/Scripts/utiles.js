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

var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
var TITLE_EXITO = 'Operación Realizada';

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