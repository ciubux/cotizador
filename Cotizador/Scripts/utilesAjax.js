
$(document).on('click', "#lnkMostrarLogPrecioProducto", function () {
    var idProducto = $(this).attr("idProducto");
    var actionUrl = "GetLogPrecioProducto";
    $("#verProductoLPP").html($(this).attr("nombreProducto"));
    $("#verCodigoProductoLPP").html($(this).attr("sku"));

    $.ajax({
        url: "/Precio/" + actionUrl,
        type: 'POST',
        dataType: 'JSON',
        data: {
            idProducto: idProducto
        },
        success: function (res) {
            var historial = res.historial;
            
            

            $("#tableLogPrecioProducto > tbody").empty();

            for (var i = 0; i < historial.length; i++) {
                var addRow = '<td>' + historial[i].fechaInicioVigencia + '</td>';

                if (historial[i].dato1 != null && historial[i].dato1 != '') {
                    var addRow = addRow + '<td>' + Number(historial[i].dato1).toFixed(2) + '<br/><span class="mini-info-text">' + historial[i].fechaModificacion1 + '<span></td>';
                } else {
                    var addRow = addRow + '<td></td>';
                }

                if (historial[i].dato2 != null && historial[i].dato2 != '') {
                    var addRow = addRow + '<td>' + Number(historial[i].dato2).toFixed(2) + '<br/><span class="mini-info-text">' + historial[i].fechaModificacion2 + '<span></td>';
                } else {
                    var addRow = addRow + '<td></td>';
                }

                if (res.mostrarCostos == 1) {
                    if (historial[i].dato3 != null && historial[i].dato3 != '') {
                        var addRow = addRow + '<td>' + Number(historial[i].dato3).toFixed(2) + '<br/><span class="mini-info-text">' + historial[i].fechaModificacion3 + '<span></td>';
                    } else {
                        var addRow = addRow + '<td></td>';
                    }
                }

                $("#tableLogPrecioProducto").append('<tr data-expanded="true">' + addRow + '</tr>');
            }

            FooTable.init('#tableLogPrecioProducto');
        }
    });
});