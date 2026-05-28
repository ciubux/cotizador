

var FABRICANTE_LAST_SEARCH;
jQuery(function ($) {
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    
    $(document).ready(function () {
        $("#tableControlStock").footable({
            "paging": {
                "enabled": true
            }
        });

    });

    $("#idCiudad").change(function () {
        var valor = $(this).val();
        changeInputFiltro("idCiudad", valor, "guid");
    });

    $("#tipoUnidad").change(function () {
        var valor = $(this).val();
        changeInputFiltro("idPresentacionUnidad", valor, "int");
    });

    $("#skuProducto").change(function () {
        var valor = $(this).val();
        changeInputFiltro("sku", valor, "string");
    });

    $("#proveedor").change(function () {
        var valor = $(this).val();
        changeInputFiltro("proveedor", valor, "string");
    });

    $("#familia").change(function () {
        var valor = $(this).val();
        changeInputFiltro("familia", valor, "string");
    });

    $("#filtroStockVerde").change(function () {
        var valor = "false";
        if ($("#filtroStockVerde").is(":checked")) { valor = "true"; }
        changeInputFiltro("stockVerde", valor, "bool");
    });
    
    $("#filtroStockAmbar").change(function () {
        var valor = "false";
        if ($("#filtroStockAmbar").is(":checked")) { valor = "true"; }
        changeInputFiltro("stockAmbar", valor, "bool");
    });

    $("#filtroStockRojo").change(function () {
        var valor = "false";
        if ($("#filtroStockRojo").is(":checked")) { valor = "true"; }
        changeInputFiltro("stockRojo", valor, "bool");
    });

    $("#btnLimpiarBusqueda").click(function () {
        location.href = "/ProductoControlStock/LimpiarFiltroControlStock";
    });

    $("#btnBusqueda").click(function () {
        location.reload();
    });

    $("#btnAculizarControlCiudad").click(function () {
        actualizarControlStock($("#idCiudad").val(), []);
    });
    
    $("#btnAculizarControlResultados").click(function () {
        var cadenaIds = $("#idsControlResultados").val();

        var idsControl = [];

        if (cadenaIds && cadenaIds.trim() !== "") {
            idsControl = cadenaIds.split(';').filter(function (id) {
                return id.trim() !== "";
            });
        }

        actualizarControlStock("", idsControl);
    });

    $("#btnAculizarControlCiudad").click(function () {
        var idsControl = [];

        //coger idsControl y separar ; para generar un array que hay que enviar a la funcion

        actualizarControlStock("", ids);
    });


    $("#btnExportExcel").click(function () {
        const dataExcelDescargar = [["SKU", "PRODUCTO", "UNIDAD", "STOCK MÍNIMO", "STOCK MÁXIMO",
                                    "STOCK ALERTA", "FECHA STOCK", "STOCK REAL", "STOCK VIRTUAL", "PEDIDO SUGERIDO"]];

        var jsonTexto = $("#jsonDataResultados").val();
        var listaData = JSON.parse(jsonTexto);

        for (var i = 0; i < listaData.length; i++) {
            var ItemRow = [listaData[i].sku, listaData[i].producto, listaData[i].unidad, listaData[i].cantMin, listaData[i].cantMax,
                listaData[i].cantAle, listaData[i].fechaStock, listaData[i].stockReal, listaData[i].stockVirtual, listaData[i].sugeridoPedir];
            dataExcelDescargar.push(ItemRow);
        }

        var ciudadNombre = $("#idCiudad option:selected").text();
        
        var nombreArchivo = 'ControlStock_' + ciudadNombre;

        const estilosColumnas = [
            { formatoCelda: "texto", anchoColumna: 10 }, // SKU
            { formatoCelda: "texto", anchoColumna: 70 }, // PRODUCTO
            { formatoCelda: "texto", anchoColumna: 15 }, // UNIDAD
            { formatoCelda: "numero", anchoColumna: 15 }, // STOCK MÍNIMO
            { formatoCelda: "numero", anchoColumna: 15 }, // STOCK MÁXIMO
            { formatoCelda: "numero", anchoColumna: 15 }, // STOCK ALERTA
            { formatoCelda: "fecha", anchoColumna: 18 }, // FECHA STOCK
            { formatoCelda: "numero", anchoColumna: 15 }, // STOCK REAL
            { formatoCelda: "numero", anchoColumna: 15 }, // STOCK VIRTUAL
            { formatoCelda: "numero", anchoColumna: 15 } // PEDIDO SUGERIDO
            //{ formatoCelda: "texto", anchoColumna: 70, colorTexto: "#0000FF" }, 
            //{ formatoCelda: "numero", anchoColumna: 12, colorCelda: "#FFFF00" }, 
        ];

        ExportarTablaExcelJsFormat(dataExcelDescargar, nombreArchivo, ciudadNombre, [], estilosColumnas);
    });


    function changeInputFiltro(propiedad, valor, tipo) {
        $.ajax({
            url: "/ProductoControlStock/changeFiltroControlStock",
            type: 'POST',
            data: {
                tipo: tipo,
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function actualizarControlStock(idCiudad, idsControl) {
        $('body').loadingModal({
            text: 'Calculando stock...'
        });

        $.ajax({
            url: "/ProductoControlStock/ActualizarControlStock",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                controlStockIds: idsControl
            },
            error: function () {
                $.alert({
                    title: "ERROR",
                    type: 'red',
                    content: 'Ocurrió un error al actulizar el stock actual.',
                    buttons: {
                        OK: function () {
                        }
                    }
                });
            },
            success: function (res) {
                if (res.success == 1) {
                    $.alert({
                        title: "OPERACIÓN EXITOSA",
                        type: 'green',
                        content: 'Se actualizó el stock actual.',
                        buttons: {
                            OK: function () {
                                location.reload();
                            }
                        }
                    });
                }
            }
        });
    }
});

