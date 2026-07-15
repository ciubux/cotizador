var LAST_SEARCH_DATA;

jQuery(function ($) {
    $(document).ready(function () {
        $("#btnBusqueda").click();

        cargarChosenCliente();
        cargarChosenProducto();
    });

    function limpiarFormulario() {
        $("#modal_id").val("0"); 
        $("#modal_idCiudad").val("");
        $("#modal_idCliente").val("");
        $("#modal_idProducto").val("");
        $("#modal_cantidadOriginal").val("");
    }

    function validar() {
        if ($("#modal_idCiudad").val() == "") {
            alert("Seleccione una ciudad"); return false;
        }
        if ($("#modal_idCliente").val() == "") {
            alert("Seleccione un cliente"); return false;
        }
        if ($("#modal_idProducto").val() == "") {
            alert("Seleccione un producto"); return false;
        }
        if ($("#modal_cantidadOriginal").val().trim() === "" || parseInt($("#modal_cantidadOriginal").val()) <= 0) {
            alert("Ingrese una cantidad original mayor a 0"); return false;
        }
        return true;
    }

    $("#btnFinalizarEdicion").click(function () {
        var isNew = $("#modal_id").val() === "0" || $("#modal_id").val() === "";
        var url = isNew ? "/ClienteProductoReservado/Create" : "/ClienteProductoReservado/Update";

        if (!validar()) return;

        var data = {
            idClienteProductoReservado: isNew ? "" : $("#modal_id").val(),
            idCiudad: $("#modal_idCiudad").val(),
            idCliente: $("#modal_idCliente").val(),
            idProducto: $("#modal_idProducto").val(),
            cantidadOriginal: $("#modal_cantidadOriginal").val(),
            estado: 1 
        };

        $.post(url, data, function (res) {
            $("#modalEditar").modal("hide");
            $("#btnBusqueda").click(); 
        }, 'JSON');
    });


    $("#btnBusqueda").click(function () {
        var dataFiltro = {
            idCiudad: $("#idCiudadBusqueda").val(),
            idCliente: $("#idClienteBusqueda").val(),
            estado: $("#estadoBusqueda").val()
        };

        $.post("/ClienteProductoReservado/Search", dataFiltro, function (list) {
            LAST_SEARCH_DATA = list;

            $("#tableRegistros > tbody").empty();
            $("#tableRegistros").footable({
                "paging": { "enabled": true }
            });

            var rows = "";
            for (var i = 0; i < list.length; i++) {
                var p = list[i];

                var etiquetaCantidadSolicitada = "";
                if (p.tieneSolicitudRecargaActiva) {
                    etiquetaCantidadSolicitada = '<br/> <span class="label label-warning label-solicitado"> Solicitado: ' + p.solicitudRecargaActiva.cantidadSolicitada + ' </span> ';
                }

                rows += '<tr>' +
                    '<td>' + p.idClienteProductoReservado + '</td>' +
                    '<td>' + (p.ciudad ? p.ciudad.nombre : "-") + '</td>' +
                    '<td>' + (p.cliente && p.cliente.ruc ? p.cliente.ruc + " - " + p.cliente.razonSocial : "-") + '</td>' +
                    '<td>' + (p.producto && p.producto.sku ? p.producto.sku + " - " + p.producto.descripcion : "-") + '</td>' +
                    '<td>' + p.unidadConteo + '</td>' +
                    '<td>' + p.cantidadOriginal + '</td>' +
                    '<td>' + p.cantidadReserva + etiquetaCantidadSolicitada + '</td>' +
                    '<td>' + p.cantidadAtendida + '</td>' +
                    '<td>' +
                    '<button type="button" class="btn btn-primary btnEditar" data-id="' + p.idClienteProductoReservado + '">Editar</button> ' +
                    '<button type="button" class="btn btn-success btnAbrirReserva" data-id="' + p.idClienteProductoReservado + '">+ Reserva</button>' +
                    '</td>' +
                    '</tr>';
            }
            
            $("#tableRegistros tbody").html(rows);
        }, 'JSON');
    });

    $("#modal_idCiudad").change(function () {
        var idCiudad = $(this).val();
        CambiarSedeBuscarClienteGlobal(idCiudad);
    });

    $("#btnLimpiarBusqueda").click(function () {
        $("#idCiudadBusqueda").val("");
        $("#idClienteBusqueda").val("");
        $("#estadoBusqueda").val("1");
        //$("#btnBusqueda").click();
    });

    $(document).on('click', '.btnEditar', function () {
        var id = $(this).data("id");
        var p = LAST_SEARCH_DATA.find(x => x.idClienteProductoReservado == id);

        if (p) {
            $("#modal_id").val(p.idClienteProductoReservado);
            $("#modal_cantidadOriginal").val(p.cantidadOriginal);

            $("#modalEditarClienteProductoReservadoTitle").text("Editar Registro");
            $("#btnFinalizarEdicion").text("Actualizar");
            $("#modalEditarClienteProductoReservado").modal("show");
        }
    });

    $("#btnAgregar").click(function () {
        limpiarFormulario();
        $("#modalEditarClienteProductoReservadoTitle").text("Registrar Nueva Reserva");
        $("#btnFinalizarEdicion").text("Registrar");
        $("#modalEditarClienteProductoReservado").modal("show");
    });

    $("#btnExportExcel").click(function () {
        const dataExcelDescargar = [["SEDE", "RUC CLIENTE", "CLIENTE", "SKU PRODUCTO", "PRODUCTO", "UNIDAD", "CANT. ORIGINAL", "CANT. RESERVA", "CANT. ATENDIDA"]];

        for (var i = 0; i < LAST_SEARCH_DATA.length; i++) {
            var obj = LAST_SEARCH_DATA[i];
            var ItemRow = [
                (obj.ciudad ? obj.ciudad.nombre : "-"),
                (obj.cliente && obj.cliente.ruc ? obj.cliente.ruc : "-"),
                (obj.cliente && obj.cliente.razonSocial ? obj.cliente.razonSocial : "-"),
                (obj.producto && obj.producto.sku ? obj.producto.sku: "-"),
                (obj.producto && obj.producto.descripcion ? obj.producto.descripcion : "-"),
                obj.unidadConteo,
                obj.cantidadOriginal,
                obj.cantidadReserva,
                obj.cantidadAtendida
            ];
            dataExcelDescargar.push(ItemRow);
        }
        
        ExportarTablaExcelJs(dataExcelDescargar, "Stock_Reservado_Cliente", "STOCK_RESERVADO");
    });

    function cargarChosenCliente() {
        $("select#modal_idCliente").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" }).on('chosen:showing_dropdown', function (evt, params) {
            if ($("#modal_idCiudad").val() == "" || $("#modal_idCiudad").val() == null) {
                alert("Debe seleccionar la sede MP previamente.");
                $("#modal_idCliente").trigger('chosen:close');
                $("#modal_idCiudad").focus();
                return false;
            }
        });

        $("select#modal_idCliente").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/Cliente/SearchClientesGlobal"
        }, {
            loadingImg: "Content/chosen/images/loading.gif"
        }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });

    }

    $("#modal_idProducto").change(function () {
        var data = { idProducto: $("#modal_idProducto").val() };
        $.post("/Producto/GetProducto", data, function (res) {
            $("#modal_unidadConteo").html(res.unidadConteo);
        }, 'JSON');
    });

    function cargarChosenProducto() {
        $("#modal_idProducto").chosen({ placeholder_text_single: "Seleccione el producto", no_results_text: "No se encontró Producto" });

        $("#modal_idProducto").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            allow_single_deselect: true,
            cache: false,
            url: "/Producto/Search"
        }, {
            loadingImg: "Content/chosen/images/loading.gif"
        }, { placeholder_text_single: "Seleccione el producto", no_results_text: "No se encontró Producto" });

        $('#modal_idProducto').val('').trigger('chosen:updated');
        $('#modal_idProducto').val('').trigger('liszt:updated');

        $('#modal_idProducto')
            .find('option:first-child').prop('selected', true)
            .end().trigger('chosen:updated');
    }

    $("#btnAbrirModalCargaMasiva").click(function () {
        var idCiudad = $("#idCiudadBusqueda").val();
        if (idCiudad === "") {
            $.alert({ title: "VALIDAR", type: 'orange', content: "Seleccione una Sede en los filtros de búsqueda antes de importar." });
            return;
        }
        
        var texto = $("#idCiudadBusqueda option:selected").text();
        $("#modalCPRCiudadtitulo").html(texto);

        $("#modalCPRExcelFile").val(""); 
        $("#modalCargaMasivaClienteProductoReservado").modal("show");
    });

    $("#btnModalCargaMasivaCPRAceptar").click(async function () {
        const fileInput = document.getElementById('modalCPRExcelFile');

        if (fileInput.files.length === 0) {
            $.alert({ title: "VALIDAR", type: 'orange', content: "Seleccione un archivo Excel." });
            return;
        }

        const file = fileInput.files[0];
        const reader = new FileReader();

        reader.onload = async (e) => {
            const buffer = e.target.result;
            const workbook = new ExcelJS.Workbook();

            try {
                await workbook.xlsx.load(buffer);
                const worksheet = workbook.getWorksheet(1);
                const listaCarga = [];

                worksheet.eachRow(function (row, rowNumber) {
                    if (rowNumber > 1) { 
                        let celdaRuc = row.getCell(2).value;      // Columna B: RUC CLIENTE
                        let celdaSku = row.getCell(4).value;      // Columna D: SKU PRODUCTO
                        let celdaCantidad = row.getCell(7).value; // Columna G: CANT. ORIGINAL

                        if (celdaSku && celdaRuc) {
                            listaCarga.push({
                                RUC: celdaRuc.toString().trim(),
                                SKU: celdaSku.toString().trim(),
                                CANTIDAD: celdaCantidad ? parseInt(celdaCantidad) : 0
                            });
                        }
                    }
                });

                var idCiudad = $("#idCiudadBusqueda").val();
                enviarRegistroMasivo(idCiudad, listaCarga);

            } catch (error) {
                $.alert({ title: "ERROR", type: 'red', content: "Ocurrió un error al procesar el archivo Excel." });
            }
        };

        reader.readAsArrayBuffer(file);
    });

    function enviarRegistroMasivo(idCiudad, listaCarga) {
        var arrayRucs = [];
        var arraySkus = [];
        var arrayCantidades = [];

        listaCarga.forEach(function (item) {
            arrayRucs.push(item.RUC);
            arraySkus.push(item.SKU);
            arrayCantidades.push(item.CANTIDAD);
        });

        $.ajax({
            url: '/ClienteProductoReservado/RegistroMasivo',
            type: 'POST',
            dataType: 'json',
            data: {
                idCiudad: idCiudad,
                rucs: arrayRucs,
                skus: arraySkus,
                cantidades: arrayCantidades
            },
            success: function (response) {
                if (response.success === 1) {
                    $.alert({
                        title: "OPERACIÓN EXITOSA",
                        type: 'green',
                        content: 'Se cargaron los registros masivos correctamente.',
                        buttons: {
                            OK: function () {
                                $("#modalCargaMasiva").modal("hide");
                                $("#btnBusqueda").click(); 
                            }
                        }
                    });
                } else {
                    $.alert({ title: "ERROR", type: 'red', content: "No se pudo realizar el registro. " + (response.message || "") });
                }
            },
            error: function () {
                $.alert({ title: "ERROR", type: 'red', content: "Error desconocido en el servidor." });
            }
        });
    }


    $(document).on('click', '.btnAbrirReserva', function () {
        var id = $(this).data("id");
        var p = LAST_SEARCH_DATA.find(x => x.idClienteProductoReservado == id);

        if (p) {
            $("#modal_res_id").val(p.idClienteProductoReservado);
            $("#modal_res_sede").val(p.ciudad ? p.ciudad.nombre : "-");
            $("#modal_res_cliente").val(p.cliente && p.cliente.razonSocial ? p.cliente.razonSocial : "-");
            $("#modal_res_sku").val(p.producto && p.producto.sku ? p.producto.sku : "-");
            $("#modal_res_unidad").val(p.unidadConteo);
            $("#modal_res_cant_original").val(p.cantidadOriginal || 0);

            var reservaActual = p.cantidadReserva || 0;
            $("#modal_res_cant_actual").val(reservaActual);

            $("#modal_res_cant_agregar").val(0);
            $("#modal_res_cant_resultante").val(reservaActual);

            if (p.tieneSolicitudRecargaActiva) {
                $("#divSolicitudReservaActiva").show();
                $("#modal_res_cant_solicitada").val(p.solicitudRecargaActiva.cantidadSolicitada);
                $("#modal_res_cant_agregar").val(p.solicitudRecargaActiva.cantidadSolicitada);
                $("#modal_res_cant_resultante").val(reservaActual + p.solicitudRecargaActiva.cantidadSolicitada);
            } else {
                $("#divSolicitudReservaActiva").hide();
            }
            

            $("#modalAgregarReserva").modal("show");
        }
    });

    $("#modal_res_cant_agregar").on('input', function () {
        var actual = parseInt($("#modal_res_cant_actual").val()) || 0;
        var agregar = parseInt($(this).val()) || 0;

        var resultante = actual + agregar;
        $("#modal_res_cant_resultante").val(resultante);
    });

    $("#btnGuardarReserva").click(function () {
        var agregar = parseInt($("#modal_res_cant_agregar").val()) || 0;
        var resultante = $("#modal_res_cant_resultante").val();

        if (agregar <= 0) {
            alert("Debe ingresar una cantidad mayor a 0 para agregar a la reserva.");
            return;
        }

        $.confirm({
            title: 'Confirmar Actualización',
            content: '¿Está seguro de agregar <b>' + agregar + '</b> a la reserva?<br><br>La nueva cantidad en reserva será: <b>' + resultante + '</b>.',
            type: 'orange',
            buttons: {
                si: {
                    text: 'SI',
                    btnClass: 'btn-warning',
                    action: function () {
                        var data = {
                            idClienteProductoReservado: $("#modal_res_id").val(),
                            cantidadAgregar: agregar
                        };

                        $.post('/ClienteProductoReservado/AgregarReserva', data, function (res) {
                            if (res.success === 1) {
                                $("#modalAgregarReserva").modal("hide");
                                $("#btnBusqueda").click(); 
                            } else {
                                $.alert({
                                    title: "ERROR",
                                    type: 'red',
                                    content: "Ocurrió un error: " + res.message
                                });
                            }
                        }, 'JSON');
                    }
                },
                no: {
                    text: 'NO',
                    btnClass: 'btn-success',
                    action: function () {
                    }
                }
            }
        });
    });
});