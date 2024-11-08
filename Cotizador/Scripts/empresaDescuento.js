

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
                }
                else {
                    $("#msgBusquedaSinResultados").show();
                }

            }
        });
    });

    $("#btnModalCMEDCancelar").click(function () {
        $("#modalEditarFabricante").modal('hide');
    });

    $('#btnModalCMEDAceptar').click(function (event) {
        var fileInput = $('#modalCMEDExcel');
        var maxSize = fileInput.data('max-size');
        var maxSizeText = fileInput.data('max-size-text');
        var imagenValida = true;
        if (fileInput.get(0).files.length) {
            var fileSize = fileInput.get(0).files[0].size; // in bytes

            if (fileSize > maxSize) {
                $.alert({
                    title: "Archivo Inválido",
                    type: 'red',
                    content: 'El tamaño del archivo debe ser como maximo ' + maxSizeText + '.',
                    buttons: {
                        OK: function () { }
                    }
                });
                imagenValida = false;
            }


        } else {
            $.alert({
                title: "Archivo Inválido",
                type: 'red',
                content: 'Seleccione un archivo por favor.',
                buttons: {
                    OK: function () { }
                }
            });
            imagenValida = false;
        }

        if (imagenValida) {

            var that = document.getElementById('modalCMEDExcel');
            var file = that.files[0];
            var form = new FormData();
            var url = $(that).data("urlSetFile");
            var reader = new FileReader();
            var mime = file.type;

            // read the image file as a data URL.
            //reader.readAsDataURL(file);

            reader.onload = async (e) => {
                const buffer = e.target.result;

                // Crear una instancia de ExcelJS Workbook y cargar el archivo
                const workbook = new ExcelJS.Workbook();
                await workbook.xlsx.load(buffer);

                // Leer la primera hoja del archivo
                const worksheet = workbook.getWorksheet(1);  // La primera hoja se indexa como 1

                // Convertir los datos de la hoja a JSON y mostrarlos
                const jsonData = [];
                worksheet.eachRow({ includeEmpty: true }, (row, rowNumber) => {
                    const rowData = row.values.slice(1);  // Quitar el primer elemento vacío
                    jsonData.push(rowData);
                });

                console.log("Datos leídos del archivo:", jsonData);

                $('body').loadingModal({
                    text: '...'
                });
                $.ajax({
                    url: "/EmpresaDescuento/CargaMasiva",
                    type: "POST",
                    data: {
                        datosCarga: jsonData
                    },
                    dataType: 'JSON',
                    success: function (response) {
                        if (response.success == "true") {
                            $.alert({
                                title: "Carga Exitosa!",
                                type: 'green',
                                content: response.message,
                                buttons: {
                                    OK: function () {
                                        location.reload();
                                    }
                                }
                            });
                        } else {
                            $.alert({
                                title: "Carga fallida",
                                type: 'red',
                                content: response.message,
                                buttons: {
                                    OK: function () { }
                                }
                            });
                        }
                    },
                    error: function (error) {
                        console.log(error);
                        $.alert({
                            title: "Carga fallida",
                            type: 'red',
                            content: 'Ocurrió un error al procesar el archivo.',
                            buttons: {
                                OK: function () { }
                            }
                        });
                    }
                }).done(function () {
                    $('body').loadingModal('hide')
                });

                // Opcional: mostrar los datos en una tabla en el HTML
                //displayData(jsonData);
            };
            reader.readAsArrayBuffer(file);
        }
    });
});



// Función para mostrar los datos en una tabla en el HTML
function displayData(data) {
    const table = document.createElement('table');
    data.forEach(row => {
        const tr = document.createElement('tr');
        row.forEach(cell => {
            const td = document.createElement('td');
            td.textContent = cell || '';  // Manejar celdas vacías
            tr.appendChild(td);
        });
        table.appendChild(tr);
    });
    document.body.appendChild(table);
}

