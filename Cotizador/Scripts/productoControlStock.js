

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

        cargarChosenProducto();
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

    $("#btnRegistroMasivo").click(function () {
        var texto = $("#idCiudad option:selected").text();
        $("#modalCMPCSCiudadtitulo").html(texto);
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
            "STOCK ALERTA", "FECHA STOCK", "STOCK REAL", "STOCK DISPONIBLE", "STOCK VIRTUAL", "PEDIDO SUGERIDO"]];

        var jsonTexto = $("#jsonDataResultados").val();
        var listaData = JSON.parse(jsonTexto);

        for (var i = 0; i < listaData.length; i++) {
            var ItemRow = [listaData[i].sku, listaData[i].producto, listaData[i].unidad, listaData[i].cantMin, listaData[i].cantMax,
                listaData[i].cantAle, listaData[i].fechaStock, listaData[i].stockReal, listaData[i].stockDisponible, listaData[i].stockVirtual, listaData[i].sugeridoPedir];
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
            { formatoCelda: "numero", anchoColumna: 15 }, // STOCK DISPONIBLE
            { formatoCelda: "numero", anchoColumna: 15 }, // STOCK VIRTUAL
            { formatoCelda: "numero", anchoColumna: 15 } // PEDIDO SUGERIDO
            //{ formatoCelda: "texto", anchoColumna: 70, colorTexto: "#0000FF" }, 
            //{ formatoCelda: "numero", anchoColumna: 12, colorCelda: "#FFFF00" }, 
        ];

        ExportarTablaExcelJsFormat(dataExcelDescargar, nombreArchivo, ciudadNombre, [], estilosColumnas);
    });

    $("#btnModalCMPCSAceptar").click(async function () {
        const fileInput = document.getElementById('modalCMPCSExcel');

        if (fileInput.files.length === 0) {
            $.alert({
                title: "VALIDAR",
                type: 'orange',
                content: "Por favor, selecciona un archivo Excel.",
                buttons: {
                    OK: function () {
                    }
                }
            });
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
                const listaCargaStock = [];

                worksheet.eachRow(function (row, rowNumber) {
                    if (rowNumber > 1) {

                        let celdaSku = row.getCell(1).value;         // Columna A
                        let celdaStockMin = row.getCell(4).value;    // Columna D
                        let celdaStockMax = row.getCell(5).value;    // Columna E
                        let celdaStockAle = row.getCell(6).value;    // Columna F

                        if (celdaSku) {
                            listaCargaStock.push({
                                SKU: celdaSku.toString().trim(),
                                CANTIDAD_MINIMA: celdaStockMin ? parseInt(celdaStockMin) : 0,
                                CANTIDAD_MAXIMA: celdaStockMax ? parseInt(celdaStockMax) : 0,
                                CANTIDAD_ALERTA: celdaStockAle ? parseInt(celdaStockAle) : 0
                            });
                        }
                    }
                });

                //console.log("Datos leídos correctamente:", listaCargaStock);

                var idCiudad = $("#idCiudad").val();
                registroMasivoAlertas(idCiudad, listaCargaStock);

            } catch (error) {
                $.alert({
                    title: "ERROR",
                    type: 'red',
                    content: "Ocurrió un error al procesar el archivo Excel.",
                    buttons: {
                        OK: function () {
                        }
                    }
                });
            }
        };

        reader.readAsArrayBuffer(file);
    });

    function registroMasivoAlertas(idCiudad, listaCargaStock) {
        var arraySkus = [];
        var arrayMins = [];
        var arrayMaxs = [];
        var arrayAlers = [];

        listaCargaStock.forEach(function (item) {
            arraySkus.push(item.SKU);
            arrayMins.push(item.CANTIDAD_MINIMA);
            arrayMaxs.push(item.CANTIDAD_MAXIMA);
            arrayAlers.push(item.CANTIDAD_ALERTA);
        });

        $.ajax({
            url: '/ProductoControlStock/RegistroMasivoAlertas', 
            type: 'POST',
            dataType: 'json', 
            data: {
                idCiudad: idCiudad,
                skus: arraySkus,
                cMins: arrayMins,
                cMaxs: arrayMaxs,
                cAlers: arrayAlers
            },
            success: function (response) {
                if (response.success === 1) {
                    $.alert({
                        title: "OPERACIÓN EXITOSA",
                        type: 'green',
                        content: 'Se actualizaron las alertas de stock.',
                        buttons: {
                            OK: function () {
                                location.reload();
                            }
                        }
                    });

                    location.reload();
                } else {
                    $.alert({
                        title: "ERROR",
                        type: 'red',
                        content: "No se pudo realizar el registro. " + (response.message || ""),
                        buttons: {
                            OK: function () {
                            }
                        }
                    });
                }
            },
            error: function (xhr, status, error) {
                $.alert({
                    title: "ERROR",
                    type: 'red',
                    content: "Error desconocido.",
                    buttons: {
                        OK: function () {
                        }
                    }
                });
            }
        });
    }

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


    $("#btnSolicitarRegargaStockReservas").click(function () {
        var jsonTexto = $("#jsonDataResultados").val();
        var listaData = JSON.parse(jsonTexto);
        var idsReservas = [];

        for (var i = 0; i < listaData.length; i++) {
            if (listaData[i].puedeRegistrarSolicitudReservaStock && listaData[i].idClienteProductoReservado !== "00000000-0000-0000-0000-000000000000") {
                idsReservas.push(listaData[i].idClienteProductoReservado);
            }
        }

        if (idsReservas.length === 0) {
            $.alert({ title: "Aviso", type: "orange", content: "No hay registros con reserva asociada en los resultados actuales." });
            return;
        }

        $('body').loadingModal({ text: 'Cargando datos de reservas...' });

        $.ajax({
            url: '/ProductoControlStock/GetDatosReservaSolicitarRecarga',
            type: 'POST',
            dataType: 'json',
            traditional: true, 
            data: { idsReservas: idsReservas },
            success: function (res) {
                $('body').loadingModal('destroy');
                if (res.success === 1) {
                    llenarTablaSolicitudesRecarga(res.data);
                    $("#modalSolicitarRecarga").modal("show");
                } else {
                    $.alert({ title: "ERROR", type: "red", content: res.message });
                }
            },
            error: function () {
                $('body').loadingModal('destroy');
                $.alert({ title: "ERROR", type: "red", content: MENSAJE_ERROR });
            }
        });
    });

    function format2Dec(num) {
        return parseFloat(num).toFixed(2);
    }

    function llenarTablaSolicitudesRecarga(data) {
        var tbody = $("#tablaSolicitudRecarga tbody");
        tbody.empty();
        var tipoUnidadSeleccionada = parseInt($("#tipoUnidad").val());

        data.forEach(function (item) {
            var prod = item.producto;
            var divisor = 1;
            var unidadTexto = prod.unidadConteo;

            if (tipoUnidadSeleccionada === 0) {
                divisor = prod.equivalenciaUnidadEstandarUnidadConteo;
                unidadTexto = prod.unidad;
            } else if (tipoUnidadSeleccionada === 1) {
                divisor = prod.equivalenciaUnidadAlternativaUnidadConteo;
                unidadTexto = prod.unidad_alternativa;
            } else if (tipoUnidadSeleccionada === 2) {
                divisor = prod.equivalenciaUnidadProveedorUnidadConteo;
                unidadTexto = prod.unidadProveedor;
            }

            var cantOrig = item.cantidadOriginal / divisor;
            var cantRsv = item.cantidadReserva / divisor;

            var idReservaStr = item.idClienteProductoReservado;
            var tdSugerido = $("td[idreserva='" + idReservaStr + "']"); 
            var cantSugeridaConvertida = 0;

            if (tdSugerido.length > 0) {
                var cantSugeridaMinima = parseFloat(tdSugerido.attr("cantidadsugerida"));

                if (!isNaN(cantSugeridaMinima) && cantSugeridaMinima > 0) {
                    cantSugeridaConvertida = Math.floor(cantSugeridaMinima / divisor);
                }
            }

            var htmlSolicitado = "";
            if (item.solicitudRecargaActiva && item.solicitudRecargaActiva.cantidadSolicitada > 0) {
                var cantSolActiva = item.solicitudRecargaActiva.cantidadSolicitada / divisor;
                htmlSolicitado = `<br><span style="color: #337ab7; font-size: 85%;">Solicitado: ${format2Dec(cantSolActiva)}</span>`;
            }

            var resultanteInicial = cantRsv + cantSugeridaConvertida;

            var minInput = Math.trunc(cantRsv * -1);

            var tr = `<tr>
                        <td>${item.ciudad.nombre}</td>
                        <td>${prod.sku} - ${prod.descripcion}</td>
                        <td>${unidadTexto}</td>
                        <td>${format2Dec(cantOrig)}</td>
                        <td class="reserva-actual" data-val="${cantRsv}">${format2Dec(cantRsv)}</td>
                        <td>
                            <input type="number" class="form-control input-sm input-solicitud-recarga" 
                                   data-id="${item.idClienteProductoReservado}" 
                                   data-divisor="${divisor}" 
                                   value="${cantSugeridaConvertida > 0 ? cantSugeridaConvertida : ''}"
                                   min="${minInput}" step="1" 
                                   onkeypress="return (event.charCode >= 48 && event.charCode <= 57) || event.charCode <= 45" 
                                   style="width: 100px; display:inline-block;" />
                            ${htmlSolicitado}
                        </td>
                        <td class="cantidad-resultante" style="font-weight:bold;">${format2Dec(resultanteInicial)}</td>
                      </tr>`;

            tbody.append(tr);
        });

        $(".input-solicitud-recarga").on("input", function () {
            var row = $(this).closest("tr");
            var resActual = parseFloat(row.find(".reserva-actual").attr("data-val"));
            var ingresado = parseInt($(this).val());
            var min = parseInt($(this).attr("min"));

            if (isNaN(ingresado)) {
                ingresado = 0;
            } else {
                if (ingresado < min) {
                    ingresado = min;
                    $(this).val(ingresado);
                }
            }

            var resultante = resActual + ingresado;
            row.find(".cantidad-resultante").text(format2Dec(resultante));
        });
    }

    $("#btnRegistrarSolicitudRecarga").click(function () {
        var contNulosOCero = 0;
        var ids = [];
        var cants = [];

        $(".input-solicitud-recarga").each(function () {
            var val = $(this).val();
            var num = parseInt(val);
            var idReserva = $(this).attr("data-id");
            var divisor = parseFloat($(this).attr("data-divisor"));

            if (val.trim() === "" || isNaN(num) || num === 0) {
                contNulosOCero++;
                num = 0;
            }

            var cantidadEnMinima = num * divisor;

            ids.push(idReserva);
            cants.push(cantidadEnMinima);
        });

        var mensajeConfirmacion = "¿Está seguro de registrar estas solicitudes?";
        if (contNulosOCero > 0) {
            mensajeConfirmacion += `<br><br><span style='color:red;'><b>Aviso:</b> Hay ${contNulosOCero} registro(s) con cantidad vacía o en 0. Si tienen una solicitud activa previa, ésta será descartada.</span>`;
        }

        $.confirm({
            title: 'Confirmar Registro',
            content: mensajeConfirmacion,
            type: 'orange',
            buttons: {
                SI: {
                    btnClass: 'btn-warning',
                    action: function () {
                        $('body').loadingModal({ text: 'Registrando solicitudes...' });
                        $.ajax({
                            url: '/ProductoControlStock/RegistrarSolicitudesRecarga',
                            type: 'POST',
                            dataType: 'json',
                            traditional: true, 
                            data: { idsReservas: ids, cantidades: cants },
                            success: function (res) {
                                $('body').loadingModal('destroy');
                                if (res.success === 1) {
                                    $.alert({
                                        title: "ÉXITO",
                                        type: "green",
                                        content: "Las solicitudes de reserva se registraron correctamente.",
                                        buttons: {
                                            OK: function () {
                                                $("#modalSolicitarRecarga").modal("hide");
                                                location.reload(); 
                                            }
                                        }
                                    });
                                } else {
                                    $.alert({ title: "ERROR", type: "red", content: res.message });
                                }
                            },
                            error: function () {
                                $('body').loadingModal('destroy');
                                $.alert({ title: "ERROR", type: "red", content: MENSAJE_ERROR });
                            }
                        });
                    }
                },
                NO: {
                    btnClass: 'btn-success',
                    action: function() {

                    }
                }
            }
        });
    });

    $(document).on("click", ".btnSolicitarRecargaFila", function () {
        var idReserva = $(this).attr("data-idreserva");

        if (!idReserva || idReserva === "00000000-0000-0000-0000-000000000000") {
            return;
        }

        var idsReservas = [idReserva];

        $('body').loadingModal({ text: 'Cargando datos de la reserva...' });

        $.ajax({
            url: '/ProductoControlStock/GetDatosReservaSolicitarRecarga',
            type: 'POST',
            dataType: 'json',
            traditional: true,
            data: { idsReservas: idsReservas },
            success: function (res) {
                $('body').loadingModal('destroy');
                if (res.success === 1) {
                    llenarTablaSolicitudesRecarga(res.data);
                    $("#modalSolicitarRecarga").modal("show");
                } else {
                    $.alert({ title: "ERROR", type: "red", content: res.message });
                }
            },
            error: function () {
                $('body').loadingModal('destroy');
                $.alert({ title: "ERROR", type: "red", content: MENSAJE_ERROR });
            }
        });
    });

    var PRODUCTO_SELECCIONADO_NUEVA_RESERVA = null;

    function cargarChosenProducto() {
        $("#modal_idProducto").chosen({ width: "100%", placeholder_text_single: "Seleccione el producto", no_results_text: "No se encontró Producto" });

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
    }

    $("#btnAgregarNuevaReserva").click(function () {
        var idCiudad = $("#idCiudad").val();
        var nombreCiudad = $("#idCiudad option:selected").text().trim();

        if (!idCiudad || idCiudad === "00000000-0000-0000-0000-000000000000" || idCiudad === "") {
            $.alert({
                title: "Aviso",
                type: "orange",
                content: "Debe seleccionar una Sede específica en los filtros de búsqueda antes de solicitar una reserva."
            });
            return;
        }

        PRODUCTO_SELECCIONADO_NUEVA_RESERVA = null;
        $("#modal_nueva_idCiudad").val(idCiudad);
        $("#modal_nueva_nombreCiudad").val(nombreCiudad);
        $("#modal_nueva_unidad").html('<option value="">Seleccione un producto</option>');
        $("#modal_nueva_cantidad").val("");

        $('#modal_idProducto').empty().append('<option value=""></option>').val('').trigger('chosen:updated');
        $('#modal_idProducto').trigger('liszt:updated');

        $("#modalAgregarNuevaReserva").modal("show");
    });

    $("#modal_idProducto").change(function () {
        var idProd = $(this).val();
        if (!idProd) {
            PRODUCTO_SELECCIONADO_NUEVA_RESERVA = null;
            $("#modal_nueva_unidad").html('<option value="">Seleccione un producto</option>');
            return;
        }

        $.post("/Producto/GetProducto", { idProducto: idProd }, function (res) {
            PRODUCTO_SELECCIONADO_NUEVA_RESERVA = res;

            var eqEstandar = res.equivalenciaUnidadEstandarUnidadConteo || 1;
            var eqAlternativa = res.equivalenciaUnidadAlternativaUnidadConteo || 1;
            var eqProveedor = res.equivalenciaUnidadProveedorUnidadConteo || 1;

            var htmlUnidades = "";

            var textoMP = res.unidad || "MP";
            htmlUnidades += `<option value="0" selected>${textoMP}</option>`;

            if (eqAlternativa !== eqEstandar && res.unidad_alternativa) {
                htmlUnidades += `<option value="1">${res.unidad_alternativa}</option>`;
            }

            if (eqProveedor !== eqEstandar && res.unidadProveedor) {
                htmlUnidades += `<option value="2">${res.unidadProveedor}</option>`;
            }

            $("#modal_nueva_unidad").html(htmlUnidades);
            $("#modal_nueva_unidad").val("0"); 
        }, 'JSON');
    });

    $("#btnGuardarNuevaReserva").click(function () {
        var idCiudad = $("#modal_nueva_idCiudad").val();
        var idProducto = $("#modal_idProducto").val();
        var tipoUnidad = $("#modal_nueva_unidad").val();
        var cantIngresada = parseInt($("#modal_nueva_cantidad").val(), 10);

        if (!idProducto || !PRODUCTO_SELECCIONADO_NUEVA_RESERVA) {
            $.alert({ title: "VALIDAR", type: "orange", content: "Por favor, seleccione un producto." });
            return;
        }

        if (isNaN(cantIngresada) || cantIngresada <= 0) {
            $.alert({ title: "VALIDAR", type: "orange", content: "Por favor, ingrese una cantidad entera mayor a 0." });
            return;
        }

        var prod = PRODUCTO_SELECCIONADO_NUEVA_RESERVA;
        var eqEstandarConteo = prod.equivalenciaUnidadEstandarUnidadConteo || 1;
        var eqAlternativaConteo = prod.equivalenciaUnidadAlternativaUnidadConteo || 1;
        var eqProveedorConteo = prod.equivalenciaUnidadProveedorUnidadConteo || 1;

        if (eqAlternativaConteo === 0) eqAlternativaConteo = 1;

        var factorElegidaAConteo = 1;
        if (tipoUnidad === "0") {
            factorElegidaAConteo = eqEstandarConteo;
        } else if (tipoUnidad === "1") {
            factorElegidaAConteo = eqAlternativaConteo;
        } else if (tipoUnidad === "2") {
            factorElegidaAConteo = eqProveedorConteo;
        }

        var cantidadEnUnidadAlternativa = Math.round((cantIngresada * factorElegidaAConteo) / eqAlternativaConteo);

        if (cantidadEnUnidadAlternativa <= 0) {
            $.alert({ title: "VALIDAR", type: "orange", content: "La cantidad convertida a la unidad alternativa debe ser al menos 1." });
            return;
        }

        var nombreUnidadSeleccionada = $("#modal_nueva_unidad option:selected").text();

        $.confirm({
            title: 'Confirmar Solicitud',
            content: `¿Está seguro de registrar la solicitud de reserva por <b>${cantIngresada} ${nombreUnidadSeleccionada}</b> para el producto seleccionado?`,
            type: 'orange',
            buttons: {
                SI: {
                    btnClass: 'btn-warning',
                    action: function () {
                        $('body').loadingModal({ text: 'Registrando solicitud de reserva...' });

                        $.ajax({
                            url: '/ProductoControlStock/RegistrarNuevaReservaConSolicitud',
                            type: 'POST',
                            dataType: 'json',
                            data: {
                                idCiudad: idCiudad,
                                idProducto: idProducto,
                                cantidadSolicitada: cantidadEnUnidadAlternativa
                            },
                            success: function (res) {
                                $('body').loadingModal('destroy');
                                if (res.success === 1) {
                                    $.alert({
                                        title: "ÉXITO",
                                        type: "green",
                                        content: "La solicitud de reserva del nuevo producto se registró correctamente.",
                                        buttons: {
                                            OK: function () {
                                                $("#modalAgregarNuevaReserva").modal("hide");
                                                location.reload();
                                            }
                                        }
                                    });
                                } else {
                                    $.alert({ title: "ERROR", type: "red", content: res.message || MENSAJE_ERROR });
                                }
                            },
                            error: function () {
                                $('body').loadingModal('destroy'); $.alert({ title: "ERROR", type: "red", content: MENSAJE_ERROR });
                            }
                        });
                    }
                },
                NO: {
                    btnClass: 'btn-success',
                    action: function () { }
                }
            }
        });
    });
});

