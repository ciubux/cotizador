
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
$("#editLogFechaVigencia").datepicker({ dateFormat: "dd/mm/yy" });

var idProductoViewMostrarLog = "";
var nombreProductoViewMostrarLog = "";
var skuProductoViewMostrarLog = "";

$(document).on('click', "#lnkMostrarLogPrecioProducto", function () {
    idProductoViewMostrarLog = $(this).attr("idProducto");
    nombreProductoViewMostrarLog = $(this).attr("nombreProducto");
    skuProductoViewMostrarLog = $(this).attr("sku");
    cargarVistaLogPreciosHistoricos();
});

$(document).on('click', ".btnVerHistorialProducto", function () {
    idProductoViewMostrarLog = $(this).attr("idProducto");
    nombreProductoViewMostrarLog = $(this).attr("nombreProducto");
    skuProductoViewMostrarLog = $(this).attr("skuProducto");
    cargarVistaLogPreciosHistoricos();
    $('#modalLogPrecioProducto').modal('show');
});

function cargarVistaLogPreciosHistoricos() {
    var actionUrl = "GetLogPrecioProducto";
    $("#verProductoLPP").html(nombreProductoViewMostrarLog);
    $("#verCodigoProductoLPP").html(skuProductoViewMostrarLog);

    $.ajax({
        url: "/Precio/" + actionUrl,
        type: 'POST',
        dataType: 'JSON',
        data: {
            idProducto: idProductoViewMostrarLog
        },
        success: function (res) {
            var historial = res.historial;



            $("#tableLogPrecioProducto > tbody").empty();

            for (var i = 0; i < historial.length; i++) {
                var addRow = '<td>' + historial[i].fechaInicioVigencia + '</td>';

                if (historial[i].dato01 != null) {
                    var addRow = addRow + '<td idHistorico="' + historial[i].dato01.idRegistroCambio + '" valor="' + historial[i].dato01.valor + '">'
                        + Number(historial[i].dato01.valor).toFixed(2) + '<br/><span class="mini-info-text">' + historial[i].dato01.fechaModificacion + '<span>';
                    if (res.editaDatoHistorico && historial[i].dato01.editable) {
                        addRow = addRow + "<br/>" + '<button class="btn btn-link lnkAction lnkActionLabel lnkLogEditarDatoHistorico"'
                            + ' data-toggle="modal" data-target="#modalLogEditarDatoHistorico" >Editar</button>';
                    }
                    addRow = addRow + "</td>";

                } else {
                    var addRow = addRow + '<td></td>';
                }

                if (historial[i].dato02 != null) {
                    var addRow = addRow + '<td idHistorico="' + historial[i].dato02.idRegistroCambio + '" valor="' + historial[i].dato02.valor + '">'
                        + Number(historial[i].dato02.valor).toFixed(2) + '<br/><span class="mini-info-text">' + historial[i].dato02.fechaModificacion + '<span>';
                    if (res.editaDatoHistorico && historial[i].dato02.editable) {
                        addRow = addRow + "<br/>" + '<button class="btn btn-link lnkAction lnkActionLabel lnkLogEditarDatoHistorico"'
                            + ' data-toggle="modal" data-target="#modalLogEditarDatoHistorico" >Editar</button>';
                    }
                    addRow = addRow + "</td>";
                } else {
                    var addRow = addRow + '<td></td>';
                }

                if (res.mostrarCostos == 1) {
                    if (historial[i].dato03 != null) {
                        var addRow = addRow + '<td idHistorico="' + historial[i].dato03.idRegistroCambio + '" valor="' + historial[i].dato03.valor + '">'
                            + Number(historial[i].dato03.valor).toFixed(2) + '<br/><span class="mini-info-text">' + historial[i].dato03.fechaModificacion + '<span>';
                        if (res.editaDatoHistorico && historial[i].dato03.editable) {
                            addRow = addRow + "<br/>" + '<button class="btn btn-link lnkAction lnkActionLabel lnkLogEditarDatoHistorico"'
                                + ' data-toggle="modal" data-target="#modalLogEditarDatoHistorico" >Editar</button>';
                        }
                        addRow = addRow + "</td>";
                    } else {
                        var addRow = addRow + '<td></td>';
                    }
                }

                $("#tableLogPrecioProducto").append('<tr data-expanded="true" fechaInicioVigencia="' + historial[i].fechaInicioVigencia + '">' + addRow + '</tr>');
            }

            FooTable.init('#tableLogPrecioProducto');
        }
    });
}

$(document).on('click', ".lnkLogEditarDatoHistorico", function () {
    $("#editLogFechaVigencia").val($(this).closest("tr").attr("fechaInicioVigencia"));
    $("#editLogIdLog").val($(this).closest("td").attr("idHistorico"));
    $("#editLogValor").val($(this).closest("td").attr("valor"));
});

$(document).on('click', "#btnLogActualizarDatoHistorico", function () {
    var fechaVigencia = $("#editLogFechaVigencia").val();
    var idLog = $("#editLogIdLog").val();
    var valor = $("#editLogValor").val();

    $.ajax({
        url: "/LogCambio/UpdateRegistro",
        type: 'POST',
        dataType: 'JSON',
        data: {
            fechaVigencia: fechaVigencia,
            idLog: idLog,
            valor: valor
        },
        success: function (res) {
            if (res.success == 1) {
                $.alert({
                    title: 'Actualización exitosa',
                    content: 'Se actualizó el dato correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            $('#modalLogEditarDatoHistorico').modal('hide');
                            cargarVistaLogPreciosHistoricos();
                        }
                    }
                });
                
            } else {
                $.alert({
                    title: 'Ocurrió un error',
                    content: 'Por favor contacte con el administrador.',
                    type: 'red',
                    buttons: {
                        OK: function () {
                        }
                    }
                });
            }
        }
    });
});


