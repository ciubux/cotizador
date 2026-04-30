

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
});


