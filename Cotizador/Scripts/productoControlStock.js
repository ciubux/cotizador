

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
            if (listaData[i].tieneRegistroReserva && listaData[i].idClienteProductoReservado !== "00000000-0000-0000-0000-000000000000") {
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

            var minInput = cantRsv * -1;

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
                                   onkeypress="return event.charCode >= 48 && event.charCode <= 57" 
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

            if (isNaN(ingresado)) ingresado = 0;

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
});

