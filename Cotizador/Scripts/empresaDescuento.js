

var EMPRESADESCUENTO_LAST_SEARCH;
var EMPRESADESCUENTO_LAST_SEARCH_EMPRESA;
var EMPRESADESCUENTO_LAST_SEARCH_TIPO_DESCUENTO;
var EMPRESADESCUENTO_LAST_SEARCH_SEDES;
jQuery(function ($) {
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    
    $(document).ready(function () {
        $("#btnBusqueda").click();
    });


    function limpiar() {
        $("#fabricante_id").val("0");
        $("#fabricante_codigo").val("");
        $("#fabricante_nombreUsual").val("");
    }

    
    $("#btnExportExcel").click(function () {
        //window.location.href = $(this).attr("actionLink");

        const dataExcelDescargar = [["SEDE", "SKU", "PRODUCTO", "TIPO DESCUENTO", "DESCUENTO"]];

        for (var i = 0; i < EMPRESADESCUENTO_LAST_SEARCH.length; i++) {
            var ItemRow = [
                EMPRESADESCUENTO_LAST_SEARCH[i].ciudad.nombre,
                EMPRESADESCUENTO_LAST_SEARCH[i].producto.sku,
                EMPRESADESCUENTO_LAST_SEARCH[i].producto.descripcion,
                EMPRESADESCUENTO_LAST_SEARCH[i].tipoDescuento,
                EMPRESADESCUENTO_LAST_SEARCH[i].descuento,
            ];
            dataExcelDescargar.push(ItemRow);
        }

        var dataTiposExcel = [];

        var sedes = "Todos";
        for (i = 0; i < EMPRESADESCUENTO_LAST_SEARCH_SEDES.length; i++) {
            if (sedes.length > 0) { sedes = sedes + ","; }
            sedes = sedes + EMPRESADESCUENTO_LAST_SEARCH_SEDES[i].nombre;
        }
        sedes = '"' + sedes + '"';
        dataTiposExcel.push([1, false, sedes, "Seleccione una sede de la lista."]);


        var tiposDesc = "";
        for (i = 0; i < EMPRESADESCUENTO_LAST_SEARCH_TIPO_DESCUENTO.length; i++) {
            if (tiposDesc.length > 0) { tiposDesc = tiposDesc + ","; }
            tiposDesc = tiposDesc + EMPRESADESCUENTO_LAST_SEARCH_TIPO_DESCUENTO[i];
        }
        tiposDesc = '"' + tiposDesc + '"';
        dataTiposExcel.push([4, false, tiposDesc, "Seleccione un tipo de descuento de la lista."]);

        
        ExportarTablaExcelJs(dataExcelDescargar, "Descuentos", "Descuentos_" + EMPRESADESCUENTO_LAST_SEARCH_EMPRESA, dataTiposExcel);
    });


    $("#btnBusqueda").click(function () {
        
        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/EmpresaDescuento/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (res) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableDescuentosEmpresa > tbody").empty();
                $("#tableDescuentosEmpresa").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                EMPRESADESCUENTO_LAST_SEARCH = res.lista;
                EMPRESADESCUENTO_LAST_SEARCH_TIPO_DESCUENTO = res.tiposDescuento;
                EMPRESADESCUENTO_LAST_SEARCH_SEDES = res.ciudades;

                list = res.lista;
                for (var i = 0; i < list.length; i++) {

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td dataAttr="idEmpresadescuento">' + list[i].idEmpresadescuento + '</td>' +
                        '<td>' + list[i].ciudad.nombre + '</td>' +
                        '<td>' + list[i].producto.sku + ' - ' + list[i].producto.descripcion + '</td>' +
                        '<td>' + list[i].descuentoDesc + '</td>' +
                        '</tr>';

                    $("#tableDescuentosEmpresa").append(ItemRow);

                }

                if (list.length > 0) {
                    $("#msgBusquedaSinResultados").hide();
                    $("#divExportButton").show();
                }
                else {
                    $("#msgBusquedaSinResultados").show();
                    $("#divExportButton").hide();
                }

            }
        });
    });


});


