

var FABRICANTE_LAST_SEARCH;
jQuery(function ($) {
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    
    $(document).ready(function () {
        $("#btnBusqueda").click();
    });


    function limpiarFormularioFabricante() {
        $("#fabricante_id").val("0");
        $("#fabricante_codigo").val("");
        $("#fabricante_nombreUsual").val("");
    }


    function validacionDatosFabricante() {
       
        if ($("#fabricante_codigo").val().length < 2) {
            $.alert({
                title: "Código Inválido",
                type: 'orange',
                content: 'Debe ingresar un Código de fabricante válido.',
                buttons: {
                    OK: function () { $('#fabricante_codigo').focus(); }
                }
            });
            return false;
        }
        
        if ($("#fabricante_nombreUsual").val().length < 4) {
            $.alert({
                title: "Nombre Inválido",
                type: 'orange',
                content: 'Debe ingresar un nombre válido.',
                buttons: {
                    OK: function () { $('#fabricante_nombreUsual').focus(); }
                }
            });
            return false;
        }

        return true;

    }


    $("#btnFinalizarEdicionFabricante").click(function () {
        /*Si no tiene codigo el cliente se está creando*/
        if ($("#fabricante_id").val() == '0') {
            crearFabricante();
        }
        else {
            editarFabricante();
        }
    });



    function crearFabricante() {
        if (!validacionDatosFabricante())
            return false;

        $('body').loadingModal({
            text: 'Creando Fabricante...'
        });
        $.ajax({
            url: "/Fabricante/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                codigo: $("#fabricante_codigo").val(),
                nombreUsual: $("#fabricante_nombreUsual").val()
            },
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear el fabricante.',
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });
            },
            success: function (resultado) {
                $('body').loadingModal('hide');
                $.alert({
                    title: TITLE_EXITO,
                    content: 'El fabricante se creó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Fabricante/List';
                        }
                    }
                });
            }
        });

    }

    function editarFabricante() {
        if (!validacionDatosFabricante())
            return false;


        $('body').loadingModal({
            text: 'Editando Fabricante...'
        });
        $.ajax({
            url: "/Fabricante/Update",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idFabricante: $("#fabricante_id").val(),
                codigo: $("#fabricante_codigo").val(),
                nombreUsual: $("#fabricante_nombreUsual").val()
            },
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar el fabricante.',
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });
            },
            success: function (resultado) {
                $('body').loadingModal('hide');

                $.alert({
                    title: TITLE_EXITO,
                    content: 'El fabricante se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Fabricante/List';
                        }
                    }
                });
            }
        });
    }

    
    $("#btnAgregarFabricante").click(function () {
        limpiarFormularioFabricante();
        $("#modalEditarFabricanteTitle").html("Registrar Nuevo Fabricante");
        $("#btnFinalizarEdicionFabricante").html("Registrar");

        $("#fabricante_codigo").removeAttr("disabled");

        $("#modalEditarFabricante").modal('show');
    });

    $("#btnCancelarFabricante").click(function () {
        $("#modalEditarFabricante").modal('hide');
    });

    
    $("#btnExportExcel").click(function () {
        //window.location.href = $(this).attr("actionLink");

        const dataExcelDescargar = [["CÓDIGO", "NOMBRE"]];

        for (var i = 0; i < FABRICANTE_LAST_SEARCH.length; i++) {
            var ItemRow = [FABRICANTE_LAST_SEARCH[i].codigo, FABRICANTE_LAST_SEARCH[i].nombreUsual];
            dataExcelDescargar.push(ItemRow);
        }

        

        // Crear un libro de trabajo (workbook) y una hoja de trabajo (worksheet)
        const worksheet = XLSX.utils.aoa_to_sheet(dataExcelDescargar);

        const headerStyle = {
            font: { bold: true, color: { rgb: "FFFFFF" } }, // Texto blanco y negrita
            fill: { fgColor: { rgb: "0066CC" } }  // Fondo azul
        };

        const range = XLSX.utils.decode_range(worksheet['!ref']);
        for (let col = range.s.c; col <= range.e.c; col++) {
            const cellRef = XLSX.utils.encode_cell({ r: 0, c: col }); // Encodificar la celda (primera fila)
            if (worksheet[cellRef]) {
                worksheet[cellRef].s = headerStyle;  // Aplicar el estilo
            }
        }

        const workbook = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(workbook, worksheet, "Fabricantes");

        // Generar el archivo Excel
        const excelBuffer = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });

        // Crear un blob y generar el enlace de descarga
        const blob = new Blob([excelBuffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = 'fabricantes.xlsx';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    });

    $("#btnBusqueda").click(function () {
        
        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/Fabricante/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableFabricantes > tbody").empty();
                $("#tableFabricantes").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                FABRICANTE_LAST_SEARCH = list;

                for (var i = 0; i < list.length; i++) {

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td dataAttr="idFabricante">' + list[i].idFabricante + '</td>' +
                        '<td>' + list[i].codigo + '</td>' +
                        '<td>' + list[i].nombreUsual + '</td>' +
                        '<td>' +
                        '<button type="button" class="btnEditarFabricante btn btn-primary ">Editar</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tableFabricantes").append(ItemRow);

                }

                if (ItemRow.length > 0) {
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


    $(document).on('click', "button.btnEditarFabricante", function () {
        
        var idFabricante = $(this).closest("tr").find("td[dataAttr='idFabricante']").html();
        var codigo;
        var nombreUsual;

        for (var i = 0; i < FABRICANTE_LAST_SEARCH.length; i++) {
            if (FABRICANTE_LAST_SEARCH[i].idFabricante == idFabricante) {
                codigo = FABRICANTE_LAST_SEARCH[i].codigo;
                nombreUsual = FABRICANTE_LAST_SEARCH[i].nombreUsual;
            }
        }

        $("#fabricante_id").val(idFabricante);
        $("#fabricante_codigo").val(codigo);
        $("#fabricante_nombreUsual").val(nombreUsual);
        
        $("#modalEditarFabricanteTitle").html("Editar Fabricante");
        $("#btnFinalizarEdicionFabricante").html("Editar");

        $("#fabricante_codigo").attr("disabled", "disabled");

        $("#modalEditarFabricante").modal('show');
    });



});


