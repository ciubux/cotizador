var LAST_SEARCH_DATA;

jQuery(function ($) {
    $(document).ready(function () {
        $("#btnBusqueda").click();

        cargarChosenCliente();
        cargarChosenProducto();
    });
    /*
    function limpiarFormulario() {
        $("#modal_id").val("0"); 
        $("#modal_idCiudad").val("");
        $("#modal_idCliente").val("");
        $("#modal_idProducto").val("");
        $("#modal_cantidadOriginal").val("");
    }*/

    function limpiarFormulario() {
        $("#modal_id").val("0");
        $("#modal_idCiudad").val("");

        $("#modal_idCliente").empty().append('<option value=""></option>').val("").trigger('chosen:updated');
        $("#modal_idProducto").empty().append('<option value=""></option>').val("").trigger('chosen:updated');

        $("#modal_cantidadOriginal").val("");
        $("#modal_unidadConteo").text("");
        $("#modal_divisor").val("1");

        $("#modal_idCiudad").prop("disabled", false);
        $("#modal_idCliente").prop("disabled", false).trigger('chosen:updated');
        $("#modal_idProducto").prop("disabled", false).trigger('chosen:updated');
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
    /*
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
    });*/

    $("#btnFinalizarEdicion").click(function () {
        var isNew = $("#modal_id").val() === "0" || $("#modal_id").val() === "";
        var url = isNew ? "/ClienteProductoReservado/Create" : "/ClienteProductoReservado/Update";

        if (!validar()) return;

        var divisor = parseFloat($("#modal_divisor").val()) || 1;
        var cantidadIngresadaSelec = parseFloat($("#modal_cantidadOriginal").val()) || 0;

        var cantidadOriginalBase = Math.round(cantidadIngresadaSelec * divisor);

        var data = {
            idClienteProductoReservado: isNew ? "" : $("#modal_id").val(),
            idCiudad: $("#modal_idCiudad").val(),
            idCliente: $("#modal_idCliente").val(),
            idProducto: $("#modal_idProducto").val(),
            cantidadOriginal: cantidadOriginalBase, 
            estado: 1
        };

        $.post(url, data, function (res) {
            $("#modalEditarClienteProductoReservado").modal("hide");
            $("#btnBusqueda").click(); 
        }, 'JSON');
    });

    $("#tipoUnidad").change(function () {
        if (LAST_SEARCH_DATA && LAST_SEARCH_DATA.length > 0) {
            renderizarTablaRegistros();
        }
    });

    $("#btnBusqueda").click(function () {
        var dataFiltro = {
            idCiudad: $("#idCiudadBusqueda").val(),
            idCliente: $("#idClienteBusqueda").val(),
            estado: $("#estadoBusqueda").val(),
            idPresentacion: $("#tipoUnidad").val() 
        };

        $.post("/ClienteProductoReservado/Search", dataFiltro, function (list) {
            LAST_SEARCH_DATA = list;
            renderizarTablaRegistros(); 
        }, 'JSON');
    });

    function renderizarTablaRegistros() {
        $("#tableRegistros > tbody").empty();
        $("#tableRegistros").footable({
            "paging": { "enabled": true }
        });

        var tipoUnidad = $("#tipoUnidad").val();
        var rows = "";

        for (var i = 0; i < LAST_SEARCH_DATA.length; i++) {
            var p = LAST_SEARCH_DATA[i];

            var conv = obtenerDatosConvertidos(p, tipoUnidad);

            var etiquetaCantidadSolicitada = "";
            if (p.tieneSolicitudRecargaActiva) {
                etiquetaCantidadSolicitada = '<br/> <span class="label label-warning label-solicitado"> Solicitado: ' + conv.cantidadSolicitada + ' </span> ';
            }

            rows += '<tr>' +
                '<td>' + p.idClienteProductoReservado + '</td>' +
                '<td>' + (p.ciudad ? p.ciudad.nombre : "-") + '</td>' +
                '<td>' + (p.cliente && p.cliente.ruc ? p.cliente.ruc + " - " + p.cliente.razonSocial : "-") + '</td>' +
                '<td>' + (p.producto && p.producto.sku ? p.producto.sku + " - " + p.producto.descripcion : "-") + '</td>' +
                '<td>' + conv.unidadTexto + '</td>' +
                '<td>' + conv.cantidadOriginal + '</td>' +
                '<td>' + conv.cantidadReserva + etiquetaCantidadSolicitada + '</td>' +
                '<td>' + conv.cantidadAtendida + '</td>' +
                '<td>' +
                '<button type="button" class="btn btn-primary btnEditar" data-id="' + p.idClienteProductoReservado + '">Editar Reserva Base</button> ' +
                '<button type="button" class="btn btn-success btnAbrirReserva" data-id="' + p.idClienteProductoReservado + '">+ Reserva</button>' +
                '</td>' +
                '</tr>';
        }

        $("#tableRegistros tbody").html(rows);
    }
    /*
    $("#btnBusqueda").click(function () {
        var dataFiltro = {
            idCiudad: $("#idCiudadBusqueda").val(),
            idCliente: $("#idClienteBusqueda").val(),
            estado: $("#estadoBusqueda").val(),
            idPresentacion: $("#tipoUnidad").val()
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
                    '<button type="button" class="btn btn-primary btnEditar" data-id="' + p.idClienteProductoReservado + '">Editar Reserva Base</button> ' +
                    '<button type="button" class="btn btn-success btnAbrirReserva" data-id="' + p.idClienteProductoReservado + '">+ Reserva</button>' +
                    '</td>' +
                    '</tr>';
            }
            
            $("#tableRegistros tbody").html(rows);
        }, 'JSON');
    });
    */
    $("#modal_idCiudad").change(function () {
        var idCiudad = $(this).val();
        CambiarSedeBuscarClienteGlobal(idCiudad);
    });

    $("#idCiudadBusqueda").change(function () {
        var idCiudad = $(this).val();
        CambiarSedeBuscarClienteGlobal(idCiudad);
    });

    $("#btnLimpiarBusqueda").click(function () {
        $("#idCiudadBusqueda").val("");
        $("#idClienteBusqueda").val("");
        $("#estadoBusqueda").val("1");
        //$("#btnBusqueda").click();
    });

    /*
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
    */

    $(document).on('click', '.btnEditar', function () {
        var id = $(this).data("id");
        var p = LAST_SEARCH_DATA.find(x => x.idClienteProductoReservado == id);

        if (p) {
            limpiarFormulario();

            $("#modal_id").val(p.idClienteProductoReservado);

            var cli = p.cliente || {};
            var idCliente = cli.idCliente || "";

            var prod = p.producto || {};
            var idProducto = prod.idProducto || "";

            var ciu = p.ciudad || {};
            var idCiudad = ciu.idCiudad || "";

            var textoCliente = "R. Social: " + (cli.razonSocial || "") + " - COD: " + (cli.codigo || "") + " - RUC: " + (cli.ruc || "");

            $("#modal_idCliente")
                .html('<option value="' + idCliente + '">' + textoCliente + '</option>')
                .val(idCliente)
                .trigger('chosen:updated');

            var textoProducto = prod.descripcion || "";

            $("#modal_idProducto")
                .html('<option value="' + idProducto + '">' + textoProducto + '</option>')
                .val(idProducto)
                .trigger('chosen:updated');

            $("#modal_idCiudad").val(idCiudad);

            var tipoUnidad = $("#tipoUnidad").val();
            var unidadTexto = p.unidadConteo || "-";
            var divisor = 1;

            if (tipoUnidad == "0") {
                unidadTexto = prod.unidad || "-";
                divisor = prod.equivalenciaUnidadEstandarUnidadConteo || 1;
            } else if (tipoUnidad == "1") {
                unidadTexto = prod.unidad_alternativa || "-";
                divisor = prod.equivalenciaUnidadAlternativaUnidadConteo || 1;
            } else if (tipoUnidad == "2") {
                unidadTexto = prod.unidadProveedor || "-";
                divisor = prod.equivalenciaUnidadProveedorUnidadConteo || 1;
            } else {
                unidadTexto = p.unidadConteo || "-";
                divisor = 1;
            }

            if (divisor === 0) divisor = 1;

            $("#modal_divisor").val(divisor);
            $("#modal_unidadConteo").text(unidadTexto);

            var cantOriginalSelec = Math.floor((p.cantidadOriginal || 0) / divisor);
            $("#modal_cantidadOriginal").val(cantOriginalSelec);

            $("#modal_idCiudad").prop("disabled", true);
            $("#modal_idCliente").prop("disabled", true).trigger('chosen:updated');
            $("#modal_idProducto").prop("disabled", true).trigger('chosen:updated');

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
        const dataExcelDescargar = [["SEDE", "RUC CLIENTE", "CLIENTE", "SKU PRODUCTO", "PRODUCTO", "UNIDAD", "RESERVA BASE", "RESERVA ACTIVA", "CANT. ATENDIDA"]];

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


        $("select#idClienteBusqueda").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" }).on('chosen:showing_dropdown', function (evt, params) {
            if ($("#idCiudadBusqueda").val() == "" || $("#idCiudadBusqueda").val() == null) {
                alert("Debe seleccionar la sede MP previamente.");
                $("#idClienteBusqueda").trigger('chosen:close');
                $("#idCiudadBusqueda").focus();
                return false;
            }
        });

        $("select#idClienteBusqueda").ajaxChosen({
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

    /*
    $("#modal_idProducto").change(function () {
        var data = { idProducto: $("#modal_idProducto").val() };
        $.post("/Producto/GetProducto", data, function (res) {
            $("#modal_unidadConteo").html(res.unidadConteo);
        }, 'JSON');
    });
    */

    $("#modal_idProducto").change(function () {
        var idProd = $(this).val();
        if (!idProd) {
            $("#modal_unidadConteo").text("");
            $("#modal_divisor").val("1");
            return;
        }

        var data = { idProducto: idProd };
        $.post("/Producto/GetProducto", data, function (res) {
            var tipoUnidad = $("#tipoUnidad").val();
            var unidadTexto = res.unidadConteo || "-";
            var divisor = 1;

            if (tipoUnidad == "0") {
                unidadTexto = res.unidad || "-";
                divisor = res.equivalenciaUnidadEstandarUnidadConteo || 1;
            } else if (tipoUnidad == "1") {
                unidadTexto = res.unidad_alternativa || "-";
                divisor = res.equivalenciaUnidadAlternativaUnidadConteo || 1;
            } else if (tipoUnidad == "2") {
                unidadTexto = res.unidadProveedor || "-";
                divisor = res.equivalenciaUnidadProveedorUnidadConteo || 1;
            } else {
                unidadTexto = res.unidadConteo || "-";
                divisor = 1;
            }

            if (divisor === 0) divisor = 1;

            $("#modal_divisor").val(divisor);
            $("#modal_unidadConteo").text(unidadTexto);
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
                        let celdaCantidad = row.getCell(7).value; // Columna G: RESERVA BASE

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


    function obtenerDatosConvertidos(p, tipoUnidad) {
        var res = {
            unidadTexto: p.unidadConteo || "-",
            divisor: 1,
            cantidadOriginal: 0,
            cantidadReserva: 0,
            cantidadAtendida: 0,
            cantidadSolicitada: 0
        };

        var prod = p.producto || {};

        if (tipoUnidad == "0") {
            res.unidadTexto = prod.unidad || "-";
            res.divisor = prod.equivalenciaUnidadEstandarUnidadConteo || 1;
        } else if (tipoUnidad == "1") {
            res.unidadTexto = prod.unidad_alternativa || "-";
            res.divisor = prod.equivalenciaUnidadAlternativaUnidadConteo || 1;
        } else if (tipoUnidad == "2") {
            res.unidadTexto = prod.unidadProveedor || "-";
            res.divisor = prod.equivalenciaUnidadProveedorUnidadConteo || 1;
        } else { // 3
            res.unidadTexto = p.unidadConteo || "-";
            res.divisor = 1;
        }

        if (res.divisor === 0) res.divisor = 1;

        res.cantidadOriginal = Number(((p.cantidadOriginal || 0) / res.divisor).toFixed(2));
        res.cantidadReserva = Number(((p.cantidadReserva || 0) / res.divisor).toFixed(2));

        if (p.tieneSolicitudRecargaActiva && p.solicitudRecargaActiva) {
            res.cantidadSolicitada = Number(((p.solicitudRecargaActiva.cantidadSolicitada || 0) / res.divisor).toFixed(2));
        }

        return res;
    }


    $(document).on('click', '.btnAbrirReserva', function () {
        var id = $(this).data("id");
        var p = LAST_SEARCH_DATA.find(x => x.idClienteProductoReservado == id);

        if (p) {
            var prod = p.producto || {};

            $("#modal_res_id").val(p.idClienteProductoReservado);
            $("#modal_res_id").data("fila_producto", p);

            $("#modal_res_sede").val(p.ciudad ? p.ciudad.nombre : "-");
            $("#modal_res_cliente").val(p.cliente && p.cliente.razonSocial ? p.cliente.razonSocial : "-");
            $("#modal_res_sku").val(prod.sku ? prod.sku : "-");
            $("#modal_res_unidad_conteo").val(p.unidadConteo || "-");

            var eqEstandar = prod.equivalenciaUnidadEstandarUnidadConteo || 1;
            var eqAlternativa = prod.equivalenciaUnidadAlternativaUnidadConteo || 1;
            var eqProveedor = prod.equivalenciaUnidadProveedorUnidadConteo || 1;

            var selectHtml = "";

            if (prod.unidadProveedor && eqEstandar !== eqProveedor) {
                selectHtml += '<option value="2">' + prod.unidadProveedor + '</option>';
            }

            if (prod.unidad && eqEstandar !== eqAlternativa) {
                selectHtml += '<option value="0">' + prod.unidad + '</option>';
            }
            if (prod.unidad_alternativa && eqAlternativa !== 1) {
                selectHtml += '<option value="1">' + prod.unidad_alternativa + '</option>';
            }
            selectHtml += '<option value="3">' + (p.unidadConteo || 'Conteo') + '</option>';

            $("#modal_res_tipoUnidad").html(selectHtml);

            var ordenUnidades = ["2", "0", "1", "3"]; 
            var unidadFiltro = $("#tipoUnidad").val() || "3";

            var startIndex = ordenUnidades.indexOf(unidadFiltro);
            if (startIndex === -1) startIndex = 0; 

            var unidadAsignar = "3"; 

            for (var i = startIndex; i < ordenUnidades.length; i++) {
                if ($("#modal_res_tipoUnidad option[value='" + ordenUnidades[i] + "']").length > 0) {
                    unidadAsignar = ordenUnidades[i];
                    break; 
                }
            }

            $("#modal_res_tipoUnidad").val(unidadAsignar);

            $("#modal_res_cant_original_base").val(p.cantidadOriginal || 0);
            $("#modal_res_cant_actual_base").val(p.cantidadReserva || 0);

            if (p.tieneSolicitudRecargaActiva && p.solicitudRecargaActiva) {
                $("#divSolicitudReservaActivaModal").show();
                $("#modal_res_cant_solicitada_base").val(p.solicitudRecargaActiva.cantidadSolicitada || 0);
            } else {
                $("#divSolicitudReservaActivaModal").hide();
                $("#modal_res_cant_solicitada_base").val(0);
            }

            $("#modal_res_tipoUnidad").trigger("change");

            $("#modalAgregarReserva").modal("show");
        }
    });


    $("#modal_res_tipoUnidad").change(function () {
        var p = $("#modal_res_id").data("fila_producto");
        if (!p) return;

        var tipoUnidad = $(this).val();
        var prod = p.producto || {};
        var divisor = 1;

        if (tipoUnidad == "0") divisor = prod.equivalenciaUnidadEstandarUnidadConteo || 1;
        else if (tipoUnidad == "1") divisor = prod.equivalenciaUnidadAlternativaUnidadConteo || 1;
        else if (tipoUnidad == "2") divisor = prod.equivalenciaUnidadProveedorUnidadConteo || 1;
        else divisor = 1;

        if (divisor === 0) divisor = 1; 
        $("#modal_res_divisor").val(divisor);

        var origBase = parseFloat($("#modal_res_cant_original_base").val()) || 0;
        var actBase = parseFloat($("#modal_res_cant_actual_base").val()) || 0;
        var solBase = parseFloat($("#modal_res_cant_solicitada_base").val()) || 0;

        $("#modal_res_cant_original_selec").val(Number((origBase / divisor).toFixed(2)));
        $("#modal_res_cant_actual_selec").val(Number((actBase / divisor).toFixed(2)));

        var solSelec = Number((solBase / divisor).toFixed(2));
        $("#modal_res_cant_solicitada_selec").val(solSelec);

        var cantSugeridaEntera = Math.floor(solBase / divisor);
        $("#modal_res_cant_agregar_selec").val(cantSugeridaEntera);

        $("#modal_res_cant_agregar_selec").trigger("input");
    });
    
    

    $("#modal_res_cant_agregar_selec").on('input', function () {
        if ($(this).val() === "-") return;

        var divisor = parseFloat($("#modal_res_divisor").val()) || 1;

        var valorIngresado = parseFloat($(this).val()) || 0;
        var agregarSelec = parseInt(valorIngresado, 10);

        if (valorIngresado !== agregarSelec && !isNaN(valorIngresado)) {
            $(this).val(agregarSelec);
        }

        var agregarBase = Math.round(agregarSelec * divisor);
        $("#modal_res_cant_agregar_base").val(agregarBase);

        var actSelec = parseFloat($("#modal_res_cant_actual_selec").val()) || 0;
        var resultanteSelec = Number((actSelec + agregarSelec).toFixed(2));
        $("#modal_res_cant_resultante_selec").val(resultanteSelec);

        var actBase = parseFloat($("#modal_res_cant_actual_base").val()) || 0;
        var resultanteBase = actBase + agregarBase;
        $("#modal_res_cant_resultante_base").val(resultanteBase);

        if (resultanteBase < 0) {
            $("#modal_res_cant_resultante_selec").removeClass("text-success").addClass("text-danger");
            $("#modal_res_cant_resultante_base").removeClass("text-success").addClass("text-danger");
        } else {
            $("#modal_res_cant_resultante_selec").removeClass("text-danger").addClass("text-success");
            $("#modal_res_cant_resultante_base").removeClass("text-danger").addClass("text-success");
        }
    });

    $("#btnGuardarReserva").click(function () {
        var agregarBase = parseInt($("#modal_res_cant_agregar_base").val()) || 0;
        var agregarSelec = parseInt($("#modal_res_cant_agregar_selec").val()) || 0;
        var resultanteBase = parseInt($("#modal_res_cant_resultante_base").val()) || 0;
        var resultanteSelec = $("#modal_res_cant_resultante_selec").val();
        var unidadNombre = $("#modal_res_tipoUnidad option:selected").text();

        if (agregarSelec === 0) {
            $.alert({ title: "VALIDAR", type: 'orange', content: "Debe ingresar una cantidad diferente de 0 para realizar un ajuste." });
            return;
        }

        if (resultanteBase < 0) {
            $.alert({ title: "VALIDAR", type: 'orange', content: "La cantidad resultante de la reserva no puede ser menor a 0." });
            return;
        }

        var accionTexto = agregarSelec > 0 ? "agregar" : "descontar";
        var cantidadMostrar = Math.abs(agregarSelec); 

        $.confirm({
            title: 'Confirmar Ajuste',
            content: '¿Está seguro de ' + accionTexto + ' <b>' + cantidadMostrar + ' ' + unidadNombre + '</b> de la reserva?<br><br>La nueva cantidad en reserva será: <b>' + resultanteSelec + ' ' + unidadNombre + '</b>.',
            type: 'orange',
            buttons: {
                si: {
                    text: 'SI',
                    btnClass: 'btn-warning',
                    action: function () {
                        var data = {
                            idClienteProductoReservado: $("#modal_res_id").val(),
                            cantidadAgregar: agregarBase
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
    

    $("#modal_res_cant_agregar_selec").on('keydown', function (e) {
        if (e.key === '.' || e.key === ',') {
            e.preventDefault();
        }
    });
});