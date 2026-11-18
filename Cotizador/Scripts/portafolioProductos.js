var PORTAFOLIOS_LAST_SEARCH = [];

jQuery(function ($) {
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";

    $(document).ready(function () {
        if ($("#tablePortafolios").length > 0) {
            $("#btnBusqueda").click();
        }
    });

    $("#btnLimpiarBusqueda").click(function () {
        $("#filtro_nombre").val("");
        $("#filtro_estado").val("1");
        $("#btnBusqueda").click();
    });

    $("#btnBusqueda").click(function () {
        var dataFiltro = {
            nombre: $("#filtro_nombre").val(),
            estado: $("#filtro_estado").val()
        };

        $.post("/PortafolioProductos/SearchList", dataFiltro, function (list) {
            PORTAFOLIOS_LAST_SEARCH = list || [];
            renderizarTablaPortafolios();
        }, 'JSON');
    });

    function renderizarTablaPortafolios() {
        $("#tablePortafolios > tbody").empty();
        $("#tablePortafolios").footable({
            "paging": { "enabled": true }
        });

        var rowsHtml = "";

        for (var i = 0; i < PORTAFOLIOS_LAST_SEARCH.length; i++) {
            var item = PORTAFOLIOS_LAST_SEARCH[i];
            var estadoTexto = item.Estado === 1
                ? '<span class="label label-success">Activo</span>'
                : '<span class="label label-default">Inactivo</span>';

            var botonesOpciones =
                '<button type="button" class="btn btn-default btn-sm btnVerPortafolio" data-id="' + item.idPortafolioProductos + '" title="Ver Detalle" style="margin-right:4px;">' +
                '<span class="glyphicon glyphicon-eye-open"></span> Ver' +
                '</button>' +
                '<button type="button" class="btn btn-primary btn-sm btnIrEditarPortafolio" data-id="' + item.idPortafolioProductos + '" title="Editar Productos" style="margin-right:4px;">' +
                '<span class="glyphicon glyphicon-pencil"></span> Editar' +
                '</button>' +
                '<button type="button" class="btn btn-info btn-sm btnIrRolesPortafolio" data-id="' + item.idPortafolioProductos + '" title="Asignar Roles" style="margin-right:4px;">' +
                '<span class="glyphicon glyphicon-lock"></span> Roles' +
                '</button>' +
                '<button type="button" class="btn btn-warning btn-sm btnIrUsuariosPortafolio" data-id="' + item.idPortafolioProductos + '" title="Asignar Usuarios">' +
                '<span class="glyphicon glyphicon-user"></span> Usuarios' +
                '</button>';

            rowsHtml += '<tr>' +
                '<td><b>' + (item.nombre || '') + '</b></td>' +
                '<td>' + estadoTexto + '</td>' +
                '<td>' + botonesOpciones + '</td>' +
                '</tr>';
        }

        $("#tablePortafolios tbody").html(rowsHtml);
    }


    $(document).on("click", ".btnVerPortafolio", function () {
        var idPortafolio = $(this).attr("data-id");

        $.post("/PortafolioProductos/Show", { idPortafolioProductos: idPortafolio }, function (res) {
            if (!res || res.success !== 1 || !res.obj) {
                alert((res && res.message) ? res.message : MENSAJE_ERROR);
                return;
            }

            var p = res.obj;
            $("#ver_idPortafolio").val(p.idPortafolioProductos);
            $("#lblVerNombre").text(p.nombre || "-");
            $("#lblVerEstado").html(p.Estado === 1 ? '<span class="label label-success">Activo</span>' : '<span class="label label-default">Inactivo</span>');
            $("#lblVerComentarios").text(p.comentarios || "-");

            var items = p.items || [];
            $("#badgeCantProductos").text(items.length);
            $("#tableVerProductos > tbody").empty();
            $("#tableVerProductos").footable({ "paging": { "enabled": true, "size": 10 } });

            var rowsProd = "";
            for (var i = 0; i < items.length; i++) {
                var prod = items[i].producto || {};
                rowsProd += '<tr>' +
                    '<td><b>' + (prod.sku || '') + '</b></td>' +
                    '<td>' + (prod.descripcion || '') + '</td>' +
                    '</tr>';
            }
            $("#tableVerProductos tbody").html(rowsProd);

            var roles = p.roles || [];
            $("#badgeCantRoles").text(roles.length);
            $("#tableVerRoles > tbody").empty();
            $("#tableVerRoles").footable({ "paging": { "enabled": true, "size": 10 } });

            var rowsRoles = "";
            for (var j = 0; j < roles.length; j++) {
                rowsRoles += '<tr>' +
                    '<td><b>' + (roles[j].codigo || '') + '</b></td>' +
                    '<td>' + (roles[j].nombre || '') + '</td>' +
                    '</tr>';
            }
            $("#tableVerRoles tbody").html(rowsRoles);

            var usuarios = p.usuarios || [];
            $("#badgeCantUsuarios").text(usuarios.length);
            $("#tableVerUsuarios > tbody").empty();
            $("#tableVerUsuarios").footable({ "paging": { "enabled": true, "size": 10 } });

            var rowsUsuarios = "";
            for (var k = 0; k < usuarios.length; k++) {
                rowsUsuarios += '<tr>' +
                    '<td>' + (usuarios[k].nombre || '') + '</td>' +
                    '<td>' + (usuarios[k].email || '') + '</td>' +
                    '</tr>';
            }
            $("#tableVerUsuarios tbody").html(rowsUsuarios);

            $('#showPortafolioProductos').tab('show');

            $("#modalVerPortafolio").modal("show");
        }, 'JSON');
    });


    $('a[data-toggle="tab"]').on('shown.bs.tab', function () {
        $("#tableVerProductos").trigger('footable_redraw');
        $("#tableVerRoles").trigger('footable_redraw');
        $("#tableVerUsuarios").trigger('footable_redraw');
    });

    $("#btnNuevoPortafolio").click(function () {
        $.post("/PortafolioProductos/IniciarNuevo", {}, function () {
            window.location = "/PortafolioProductos/Editar";
        }, 'JSON');
    });


    function prepararYRedirigir(idPortafolio, urlDestino) {
        $.post("/PortafolioProductos/PrepararAccion", { idPortafolioProductos: idPortafolio }, function (res) {
            if (res && res.success === 1) {
                window.location = urlDestino;
            } else {
                alert((res && res.message) ? res.message : MENSAJE_ERROR);
            }
        }, 'JSON');
    }

    $(document).on("click", ".btnIrEditarPortafolio", function () {
        prepararYRedirigir($(this).attr("data-id"), "/PortafolioProductos/Editar");
    });

    $(document).on("click", ".btnIrRolesPortafolio", function () {
        prepararYRedirigir($(this).attr("data-id"), "/PortafolioProductos/Roles");
    });

    $(document).on("click", ".btnIrUsuariosPortafolio", function () {
        prepararYRedirigir($(this).attr("data-id"), "/PortafolioProductos/Usuarios");
    });

    $("#btnModalIrEditar").click(function () {
        window.location = "/PortafolioProductos/Editar";
    });

    $("#btnModalIrRoles").click(function () {
        window.location = "/PortafolioProductos/Roles";
    });

    $("#btnModalIrUsuarios").click(function () {
        window.location = "/PortafolioProductos/Usuarios";
    });


    
    function actualizarContadorItemsEditar() {
        var total = $("#contenedorItemsPortafolio .item-row").length;
        $("#badgeTotalItemsEditar").text(total);
    }

    $(document).on("change", ".cabecera-portafolio-input", function () {
        var $input = $(this);
        var data = {
            propiedad: $input.attr("data-propiedad"),
            valor: $input.val()
        };
        $.post("/PortafolioProductos/ChangeInputForm", data);
    });

    $("#btnAgregarItemPortafolio").click(function () {
        if ($("#filaNuevaBusquedaProducto").length > 0) {
            $.alert({
                title: "Atención",
                type: "orange",
                content: "Ya tiene una fila pendiente de selección de producto. Seleccione el producto o elimine la fila antes de agregar otra."
            });
            return;
        }

        var nuevaFilaHtml =
            '<div class="row" id="filaNuevaBusquedaProducto" style="padding: 10px 0; border-bottom: 1px solid #eee; display: flex; align-items: center;">' +
            '<div class="col-xs-3">' +
            '<select class="form-control" id="select_nuevo_idProducto">' +
            '<option value=""></option>' +
            '</select>' +
            '</div>' +
            '<div class="col-xs-8"><span class="text-muted">Seleccione un producto para cargar su descripción...</span></div>' +
            '<div class="col-xs-1 text-center">' +
            '<button type="button" class="btn btn-default btn-sm" id="btnCancelarFilaNueva" title="Cancelar fila">' +
            '<i class="glyphicon glyphicon-remove"></i>' +
            '</button>' +
            '</div>' +
            '</div>';

        $("#contenedorItemsPortafolio").append(nuevaFilaHtml);
        inicializarChosenNuevoProducto();
    });

    function inicializarChosenNuevoProducto() {
        $("#select_nuevo_idProducto").chosen({
            placeholder_text_single: "Buscar SKU o producto...",
            no_results_text: "No se encontró Producto"
        });

        $("#select_nuevo_idProducto").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 3,
            afterTypeDelay: 300,
            allow_single_deselect: true,
            cache: false,
            url: "/Producto/Search"
        }, {
            loadingImg: "/Content/chosen/images/loading.gif"
        }, {
            placeholder_text_single: "Buscar SKU o producto...",
            no_results_text: "No se encontró Producto"
        });

        $('#select_nuevo_idProducto').val('').trigger('chosen:updated');
    }

    $(document).on("click", "#btnCancelarFilaNueva", function () {
        $("#filaNuevaBusquedaProducto").remove();
    });

    $(document).on("change", "#select_nuevo_idProducto", function () {
        var idProd = $(this).val();
        if (!idProd) return;

        if ($('#contenedorItemsPortafolio .item-row[data-idproducto="' + idProd + '"]').length > 0) {
            $.alert({
                title: "Producto Duplicado",
                type: "orange",
                content: "El producto seleccionado ya se encuentra en la lista del portafolio."
            });
            $('#select_nuevo_idProducto').val('').trigger('chosen:updated');
            return;
        }

        $.post("/PortafolioProductos/AddItem", { idProducto: idProd }, function (res) {
            if (!res || res.success !== 1 || !res.item) {
                $.alert({
                    title: "Aviso",
                    type: "red",
                    content: (res && res.message) ? res.message : MENSAJE_ERROR
                });
                $('#select_nuevo_idProducto').val('').trigger('chosen:updated');
                return;
            }

            var prod = res.item.producto;
            var filaDefinitivaHtml = armarFilaProductoHtml(prod);

            $("#filaNuevaBusquedaProducto").replaceWith(filaDefinitivaHtml);
            actualizarContadorItemsEditar();
        }, "JSON");
    });

    function armarFilaProductoHtml(prod) {
        return '<div class="row item-row" style="padding: 10px 0; border-bottom: 1px solid #eee; display: flex; align-items: center;" data-idproducto="' + prod.idProducto + '">' +
            '<div class="col-xs-3"><b>' + (prod.sku || '') + '</b></div>' +
            '<div class="col-xs-8">' + (prod.descripcion || '') + '</div>' +
            '<div class="col-xs-1 text-center">' +
            '<button type="button" class="btn btn-danger btn-sm btnEliminarItemPortafolio" data-idproducto="' + prod.idProducto + '" title="Quitar Producto">' +
            '<i class="glyphicon glyphicon-trash"></i>' +
            '</button>' +
            '</div>' +
            '</div>';
    }

    $(document).on("click", ".btnEliminarItemPortafolio", function () {
        var $btn = $(this);
        var idProd = $btn.attr("data-idproducto");

        $.post("/PortafolioProductos/RemoveItem", { idProducto: idProd }, function (res) {
            if (res && res.success === 1) {
                $btn.closest(".item-row").remove();
                actualizarContadorItemsEditar();
            } else {
                $.alert({
                    title: "Error",
                    type: "red",
                    content: (res && res.message) ? res.message : MENSAJE_ERROR
                });
            }
        }, "JSON");
    });

    $("#btnAbrirCargaMasivaSkus").click(function () {
        $("#txtListaSkusMasivo").val("");
        $("#modalCargaMasivaSkus").modal("show");
    });

    $("#btnProcesarSkusMasivo").click(function () {
        var texto = $("#txtListaSkusMasivo").val() || "";
        var partes = texto.split(/[\n,;\t\s]+/);
        var skusLimpios = [];

        for (var i = 0; i < partes.length; i++) {
            var s = $.trim(partes[i]);
            if (s !== "" && $.inArray(s, skusLimpios) === -1) {
                skusLimpios.push(s);
            }
        }

        if (skusLimpios.length === 0) {
            $.alert({ title: "Aviso", type: "orange", content: "Ingrese al menos un código SKU." });
            return;
        }

        $.ajax({
            url: "/PortafolioProductos/AddItemsMasivo",
            type: "POST",
            dataType: "JSON",
            traditional: true,
            data: { skus: skusLimpios },
            success: function (res) {
                if (res && res.success === 1) {
                    var nuevos = res.items || [];
                    for (var j = 0; j < nuevos.length; j++) {
                        $("#contenedorItemsPortafolio").append(armarFilaProductoHtml(nuevos[j].producto));
                    }
                    actualizarContadorItemsEditar();
                    $("#modalCargaMasivaSkus").modal("hide");
                    $.alert({
                        title: "Carga completada",
                        type: "green",
                        content: "Se agregaron " + res.agregados + " producto(s) nuevos al portafolio."
                    });
                } else {
                    $.alert({
                        title: "Error",
                        type: "red",
                        content: (res && res.message) ? res.message : MENSAJE_ERROR
                    });
                }
            }
        });
    });

    function guardarPortafolio(finalizar) {
        var nombre = $.trim($("#portafolio_nombre").val());
        if (nombre === "") {
            $.alert({
                title: "Validación",
                type: "orange",
                content: "Debe ingresar el nombre del portafolio."
            });
            return;
        }

        $.post("/PortafolioProductos/ChangeInputForm", {
            propiedad: "nombre",
            valor: nombre
        }, function () {
            $.post("/PortafolioProductos/ChangeInputForm", {
                propiedad: "comentarios",
                valor: $("#portafolio_comentarios").val()
            }, function () {
                $.post("/PortafolioProductos/Guardar", { finalizar: finalizar }, function (res) {
                    if (res && res.success === 1) {
                        if (finalizar === 1) {
                            window.location = "/PortafolioProductos/List";
                        } else {
                            $("#editar_idPortafolio").val(res.obj.idPortafolioProductos);
                            $("#btnFinalizarPortafolio").text("Finalizar Edición");
                            $.alert({
                                title: "Guardado",
                                type: "green",
                                content: "El avance del portafolio se guardó correctamente."
                            });
                        }
                    } else {
                        $.alert({
                            title: "Error",
                            type: "red",
                            content: (res && res.message) ? res.message : MENSAJE_ERROR
                        });
                    }
                }, "JSON");
            });
        });
    }

    $("#btnGuardarAvancePortafolio").click(function () {
        guardarPortafolio(0);
    });

    $("#btnFinalizarPortafolio").click(function () {
        guardarPortafolio(1);
    });

    $("#btnVolverListado").click(function () {
        $.confirm({
            title: 'Confirmación',
            content: '¿Está seguro de volver al listado? Se perderán los cambios no guardados.',
            type: 'orange',
            buttons: {
                confirm: {
                    text: 'SI',
                    btnClass: 'btn-warning',
                    action: function () {
                        $.post("/PortafolioProductos/CancelarEdicion", {}, function () {
                            window.location = "/PortafolioProductos/List";
                        }, "JSON");
                    }
                },
                cancel: {
                    text: 'Cancelar',
                    action: function () {
                    }
                }
            }
        });
    });

});