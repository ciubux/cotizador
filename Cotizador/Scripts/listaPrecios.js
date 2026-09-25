$(document).ready(function () {
    var filaEnProceso = false;

    if ($("#btnBuscar").length > 0) {
        cargarTabla();
    }

    $(document).on("change", ".cabecera-input", function () {
        var $input = $(this); $.ajax({
            url: "/ListaPrecios/ChangeInputForm",
            type: "POST",
            data: {
                propiedad: $input.data("propiedad"),
                tipo: $input.data("tipo"),
                valor: $input.val()
            },
            success: function (res) {

            }
        });
    });

    $(document).on("change", ".input-precio", function () {
        var $input = $(this);
        var idProd = $input.data("idproducto");
        var valPrecio = parseFloat($input.val()) || 0;

        $input.val(valPrecio.toFixed(2));

        $.ajax({
            url: "/ListaPrecios/ChangeItem",
            type: "POST",
            dataType: "JSON",
            data: {
                idProducto: idProd,
                precio: valPrecio
            },
            success: function (res) {
                if (res.success === 0) {
                    alert("Ocurrió un problema: " + res.message);
                }
            }
        });
    });


    $(document).on("click", ".btnEliminarItem", function () {
        var $btn = $(this);
        var idProd = $btn.data("idproducto");

        $.ajax({
            url: "/ListaPrecios/RemoveItem",
            type: "POST",
            dataType: "JSON",
            data: { idProducto: idProd },
            success: function (res) {
                if (res.success === 1) {
                    $btn.closest(".item-row").remove();
                } else {
                    alert(res.message);
                }
            }
        });
    });


    $("#btnAgregarItem").click(function () {
        if (filaEnProceso) {
            alert("Tiene un item pendiente por seleccionar. Escoja un producto o cancele la selección actual.");
            return;
        }

        filaEnProceso = true;

        var htmlFilaVacia = `
            <div class="row item-row fila-busqueda-activa" style="padding: 10px 0; border-bottom: 1px solid #eee; display: flex; align-items: center;">
                <div class="col-xs-2">
                    <select class="form-control select-busqueda-prod" name="producto">
                        <option value=""></option>
                    </select>
                </div>
                <div class="col-xs-3">-</div>
                <div class="col-xs-1">-</div>
                <div class="col-xs-2">-</div>
                <div class="col-xs-2">-</div>
                <div class="col-xs-1">
                    <input type="text" class="form-control" disabled value="0.00" />
                </div>
                <div class="col-xs-1 text-center">
                    <button class="btn btn-warning btn-sm btnCancelarFila" title="Cancelar Agregado">
                        <i class="glyphicon glyphicon-remove"></i>
                    </button>
                </div>
            </div>`;

        $("#contenedorItems").append(htmlFilaVacia);

        var $select = $(".fila-busqueda-activa").last().find(".select-busqueda-prod");
        cargarChosenProductoEnSelect($select);
    });

    $(document).on("click", ".btnCancelarFila", function () {
        $(this).closest(".fila-busqueda-activa").remove();
        filaEnProceso = false;
    });

    function cargarChosenProductoEnSelect($select) {
        $select.chosen({ placeholder_text_single: "Busque y Seleccione...", no_results_text: "No se encontró" });

        $select.ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            allow_single_deselect: true,
            cache: false,
            url: "/Producto/Search"
        }, {
            loadingImg: "Content/chosen/images/loading.gif"
        });

        $select.change(function () {
            var idProd = $(this).val();
            if (!idProd) return;

            var $fila = $(this).closest(".fila-busqueda-activa");

            $.ajax({
                url: "/ListaPrecios/AddItem",
                type: "POST",
                dataType: "JSON",
                data: { idProducto: idProd },
                success: function (res) {
                    if (res.success === 1) {
                        var p = res.item.producto;
                        var pLima = parseFloat(p.precioSinIgv || 0).toFixed(2);
                        var pProv = parseFloat(p.precioProvinciaSinIgv || 0).toFixed(2);

                        $fila.removeClass("fila-busqueda-activa").attr("data-idproducto", p.idProducto);

                        var celdasRenderizadas = `
                            <div class="col-xs-2">${p.sku}</div>
                            <div class="col-xs-3">${p.descripcion}</div>
                            <div class="col-xs-1">${p.unidad || "-"}</div>
                            <div class="col-xs-2">${pLima}</div>
                            <div class="col-xs-2">${pProv}</div>
                            <div class="col-xs-1">
                                <input type="text" class="form-control input-precio" 
                                       onkeypress="return (event.charCode >= 48 && event.charCode <= 57) || event.charCode == 46"
                                       data-idproducto="${p.idProducto}" value="0.00" />
                            </div>
                            <div class="col-xs-1 text-center">
                                <button class="btn btn-danger btn-sm btnEliminarItem" data-idproducto="${p.idProducto}" title="Remover Item">
                                    <i class="glyphicon glyphicon-trash"></i>
                                </button>
                            </div>
                        `;

                        $fila.html(celdasRenderizadas);
                        filaEnProceso = false;
                    } else {
                        alert(res.message);
                        $select.val('').trigger("chosen:updated");
                    }
                }
            });
        });
    }

    $("#btnGuardarAvance").click(function () {
        enviarAlertaYGuardar(0);
    });

    $("#btnFinalizarRegistro").click(function () {
        enviarAlertaYGuardar(1);
    });

    function enviarAlertaYGuardar(finalizar) {
        $.ajax({
            url: "/ListaPrecios/Guardar",
            type: "POST",
            dataType: "JSON",
            data: { finalizar: finalizar },
            success: function (res) {
                if (res.success === 1) {
                    if (finalizar === 1) {
                        alert("Registro finalizado correctamente.");
                        window.location.href = "/ListaPrecios/List";
                    } else {
                        alert("Avance guardado correctamente.");
                    }
                } else {
                    alert("Ocurrió un error: " + res.message);
                }
            }
        });
    }

    $("#btnCancelarRegistroListaPrecios").click(function () {
        $.confirm({
            title: '',
            content: '¿Esta seguro que desea cancelar el registro? No se guardaran los cambios.',
            type: 'orange',
            buttons: {
                SI: {
                    text: 'SI',
                    btnClass: 'btn-danger',
                    action: function () {
                        window.location = '/ListaPrecios/CancelarRegistro';
                    }
                },
                NO: {
                    text: 'NO',
                    btnClass: 'btn-success',
                    action: function () {
                        
                    }
                }
            }
        });
    })


    $("#btnBuscar").click(function () {
        cargarTabla();
    });

    function cargarTabla() {
        $.ajax({
            url: "/ListaPrecios/SearchList",
            type: "POST",
            dataType: "JSON",
            success: function (list) {
                $("#tableListas > tbody").empty();

                // Inicializar Footable
                $("#tableListas").footable({
                    "paging": { "enabled": true }
                });

                var rows = "";
                for (var i = 0; i < list.length; i++) {
                    var p = list[i];
                    rows += '<tr>' +
                        '<td>' + (p.nombre || "-") + '</td>' +
                        '<td>' + (p.comentarios || "-") + '</td>' +
                        '<td>' +
                        '<button type="button" class="btn btn-info btn-sm btnVer" style="margin-right: 5px;" data-id="' + p.idListaPrecios + '">Ver</button>' +
                        '<button type="button" class="btn btn-danger btn-sm btnEliminar" data-id="' + p.idListaPrecios + '">Eliminar</button>' +
                        '</td>' +
                        '</tr>';
                }

                $("#tableListas tbody").html(rows);
            },
            error: function () {
                $.alert('Ocurrió un error al cargar las listas de precios.');
            }
        });
    }

    $(document).on("click", ".btnVer", function () {
        var idListaPrecios = $(this).data("id");

        $.ajax({
            url: "/ListaPrecios/Show",
            type: "POST",
            data: { idListaPrecios: idListaPrecios },
            dataType: "JSON",
            success: function (res) {
                if (res.success === 1) {
                    var obj = res.obj;

                    // Llenar cabeceras
                    $("#lblVerNombre").text(obj.nombre || "-");
                    $("#lblVerComentarios").text(obj.comentarios || "-");

                    // Llenar tabla de items
                    $("#tableVerItems > tbody").empty();
                    $("#tableVerItems").footable({
                        "paging": { "enabled": true }
                    });

                    var rowsItems = "";
                    if (obj.items && obj.items.length > 0) {
                        for (var i = 0; i < obj.items.length; i++) {
                            var item = obj.items[i];
                            // Ignorar eliminados si estuvieran en memoria
                            if (item.Estado === 0) continue;

                            var sku = item.producto ? item.producto.sku : "-";
                            var descripcion = item.producto ? item.producto.descripcion : "-";

                            rowsItems += '<tr>' +
                                '<td>' + sku + '</td>' +
                                '<td>' + descripcion + '</td>' +
                                '<td>' + (item.unidad || "-") + '</td>' +
                                '<td>' + parseFloat(item.precio).toFixed(2) + '</td>' +
                                '</tr>';
                        }
                    }

                    $("#tableVerItems tbody").html(rowsItems);
                    $("#modalVerLista").modal("show");

                } else {
                    $.alert("Error al obtener el detalle: " + res.message);
                }
            }
        });
    });

    $(document).on("click", ".btnEliminar", function () {
        var idListaPrecios = $(this).data("id");

        $.confirm({
            title: 'Confirmación',
            content: '¿Está seguro que desea eliminar esta lista de precios?',
            type: 'red',
            buttons: {
                confirm: {
                    text: 'Eliminar',
                    btnClass: 'btn-red',
                    action: function () {
                        $.ajax({
                            url: "/ListaPrecios/Eliminar",
                            type: "POST",
                            dataType: "JSON",
                            data: { idListaPrecios: idListaPrecios },
                            success: function (res) {
                                if (res.success === 1) {
                                    location.reload();
                                } else {
                                    $.alert("Error: " + res.message);
                                }
                            }
                        });
                    }
                },
                cancel: {
                    text: 'Cancelar'
                }
            }
        });
    });

    $("#btnModalEditar").click(function () {
        $.ajax({
            url: "/ListaPrecios/ConsultarSiExisteEdicionActiva",
            type: "POST",
            dataType: "JSON",
            success: function (res) {
                var boolExiste = (typeof res === 'string') ? JSON.parse(res).existe : res.existe;

                if (boolExiste === "true") {
                    $.alert({
                        title: 'Alerta',
                        content: 'Ya se encuentra realizando un registro o edición. Debe cancelarlo antes de editar otra lista.'
                    });
                } else {
                    $.ajax({
                        url: "/ListaPrecios/IniciarEdicion",
                        type: "POST",
                        success: function () {
                            window.location.href = "/ListaPrecios/Editar";
                        }
                    });
                }
            }
        });
    });

});