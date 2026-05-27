

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

        // Inicializa todos los tooltips en la página
        $('[data-toggle="popover"]').popover({
            animation: true,
            delay: { "show": 100, "hide": 100 }
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


