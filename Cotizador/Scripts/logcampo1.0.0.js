jQuery(function ($) {

    var TITLE_EXITO = 'Operación Realizada';

    var catalogoId;


    $(document).ready(function () {
        if ($("#catalogo_tablaId").val() == "") {
            $("#btnBusquedaLogCatalogo").attr("disabled", "disabled");

        }
        $("#catalogo_tablaId").on("change", function () {
            var a = $("#catalogo_tablaId").val();
            if (a == "") {
                $("#btnBusquedaLogCatalogo").attr("disabled", "disabled");
                $("td").remove();

            } else {
                $("#btnBusquedaLogCatalogo").removeAttr('disabled');
                if ($("td").length) {
                    $("td").remove();
                }
            }
        });

        $("#btnBusquedaLogCatalogo").click();

    });



    $("#btnBusquedaLogCatalogo").click(function () {

        $("#btnBusquedaLogCatalogo").attr("disabled", "disabled");
        $.ajax({
            url: "/LogCampo/SearchList",
            type: 'POST',
            dataType: 'JSON',

            error: function () {
                $("#btnBusquedaLogCatalogo").removeAttr("disabled");
            },

            success: function (list) {

                $("#btnBusquedaLogCatalogo").removeAttr("disabled");

                $("#tableCatalogo > tbody").empty();


                for (var i = 0; i < list.length; i++) {



                    if (list[i].estadoCatalogo == 1) {
                        var label = '<input class="estadoCatalogo ' + list[i].catalogoId + ' radio-input" checked type="radio" name="catalogo_estado_' + list[i].catalogoId + '" value="1"><span>Activo</span>';
                    }
                    else {
                        label = '<input class="estadoCatalogo ' + list[i].catalogoId + ' radio-input" type="radio"  name="catalogo_estado_' + list[i].catalogoId + '" value="1"><span>Activo</span>';
                    }

                    if (list[i].estadoCatalogo == 0) {
                        var label2 = '<input class="estadoCatalogo ' + list[i].catalogoId + ' radio-input" checked type="radio" name="catalogo_estado_' + list[i].catalogoId + '" value="0"><span>Inactivo</span>';
                    }
                    else {
                        label2 = '<input class="estadoCatalogo ' + list[i].catalogoId + ' radio-input" type="radio" name="catalogo_estado_' + list[i].catalogoId + '" value="0"><span>Inactivo</span>';
                    }
                    if (list[i].puede_persistir == 1) {
                        var label3 = '<input class="puede_persistir ' + list[i].catalogoId + ' radio-input" checked type="radio" name="catalogo_persiste_' + list[i].catalogoId + '" value="1"><span>Si</span>';
                    }
                    else {
                        label3 = '<input class="puede_persistir ' + list[i].catalogoId + ' radio-input" type="radio" name="catalogo_persiste_' + list[i].catalogoId + '" value="1"><span>Si</span>';
                    }

                    if (list[i].puede_persistir == 0) {
                        var label4 = '<input class="puede_persistir ' + list[i].catalogoId + ' radio-input" checked type="radio" name="catalogo_persiste_' + list[i].catalogoId + '" value="0"><span>No</span>';
                    }
                    else {
                        label4 = '<input class="puede_persistir ' + list[i].catalogoId + ' radio-input" type="radio" name="catalogo_persiste_' + list[i].catalogoId + '" value="0"><span>No</span>';
                    }


                    if (list[i].estadoCatalogo == null && list[i].puede_persistir == null) {
                        list[i].nombre = list[i].nombre + "<span style='color: darkred;' class='has - activate'><br>*No se encuentra configurado en el LOG</span>";
                        if (list[i].estadoCatalogo == null) {

                            label = '<input class="estadoCatalogo ' + list[i].catalogoId + ' add radio-input" type="radio" name="catalogo_estado_' + list[i].catalogoId + '" value="1"><span>Activo</span>';

                            label2 = '<input class="estadoCatalogo ' + list[i].catalogoId + ' add radio-input" type="radio" name="catalogo_estado_' + list[i].catalogoId + '" value="0"><span>Inactivo</span>';
                        }

                        if (list[i].puede_persistir == null) {


                            label3 = '<input class="puede_persistir ' + list[i].catalogoId + ' add radio-input"  type="radio" name="catalogo_persiste_' + list[i].catalogoId + '" value="1" ><span>Si</span>';

                            label4 = '<input class="puede_persistir ' + list[i].catalogoId + ' add radio-input" type="radio" name="catalogo_persiste_' + list[i].catalogoId + '" value="0"><span>No</span>';
                        }

                    }


                    var ItemRow = '<tr data-expanded="true" id="contenidoTabla">' +


                        '<td class="nombreCampo">' + list[i].nombre + '</td>' +
                        '<td> <div class="radio radio-inline"><label class="radio-label">' + label + '</label></div><div class="radio radio-inline"><label class="radio-label">' + label2 + '</label></div> </td>' +
                        '<td> <div class="radio radio-inline"><label class="radio-label">' + label3 + '</label></div><div class="radio radio-inline"><label class="radio-label">' + label4 + '</label></div> </td>' +
                        '</tr>';



                    $("#tableCatalogo").append(ItemRow);

                }


            }
        });
    });


    var applyEstado = false;
    $("#tableCatalogo").on("change", ".estadoCatalogo", function () {
        if (applyEstado) {

            return;
        }
        applyEstado = true;

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var that = this;
        catalogoId = arrrayClass[1];
        add = arrrayClass[2];
        var valor = $(this).val();
        if (valor == 1) {
            opcion = "Activo";


        } if (valor == 0) {
            opcion = "Inactivo";
        }

        var nombreCampo = $(this).closest("tr").find("td.nombreCampo").text().replace("*No se encuentra configurado en el LOG", "");;


        if (add != "add") {
            $.confirm({
                title: 'Confirmación de cambio',
                content: '¿Está seguro de cambiar el estado del campo ' + nombreCampo + ' a ' + opcion + '?',
                type: 'orange',
                buttons: {
                    confirm: {
                        text: 'Confirmar',
                        action: function () {
                            $.ajax({
                                url: "/LogCampo/Update",
                                type: 'POST',
                                data: {
                                    catalogoId: catalogoId,
                                    propiedad: "estadoCatalogo",
                                    valor: valor

                                },
                                error: function (resultado) {
                                    $.alert({
                                        title: "Error",
                                        content: "Se generó un error al editar el estado del campo.",
                                        type: 'red',
                                        buttons: {
                                            OK: function () {
                                            }
                                        }
                                    });
                                },
                                success: function (resultado) {
                                    $('body').loadingModal('hide');

                                    $.alert({
                                        title: TITLE_EXITO,
                                        content: 'El estado del campo se editó correctamente.',
                                        type: 'green',
                                        buttons: {
                                            OK: function () {



                                            }
                                        }
                                    });
                                }
                            })
                        }
                    },
                    cancel: {
                        text: 'Cancelar',
                        action: function () {
                            if (valor == 0) {

                                $(that).closest("td").find("input[value=0]").prop("checked", false);
                                $(that).closest("td").find("input[value=1]").prop("checked", true);

                            }
                            if (valor == 1) {



                                $(that).closest("td").find("input[value=0]").prop("checked", true);
                                $(that).closest("td").find("input[value=1]").prop("checked", false);
                            }

                        }
                    }
                }
            });

        } else {
            $.confirm({
                title: 'Confirmación de cambio',
                content: '¿Está seguro de cambiar el estado del campo ' + nombreCampo + '(Se insertará a la tabla LogCampo) a ' + opcion + '?',
                type: 'orange',
                buttons: {
                    confirm: {
                        text: 'Confirmar',
                        action: function () {
                            $.ajax({
                                url: "/LogCampo/InsertCampo",
                                type: 'POST',
                                data: {
                                    propiedad: "estadoCatalogo",
                                    valor: valor,
                                    nombre: nombreCampo
                                },
                                error: function (resultado) {
                                    $.alert({
                                        title: "Error",
                                        content: "Se generó un error al editar el estado del campo.",
                                        type: 'red',
                                        buttons: {
                                            OK: function () {
                                                $(that).closest("td").find("input[value=0]").prop("checked", false);
                                                $(that).closest("td").find("input[value=1]").prop("checked", false);
                                            }
                                        }
                                    });
                                },
                                success: function (resultado) {
                                    $('body').loadingModal('hide');

                                    $.alert({
                                        title: TITLE_EXITO,
                                        content: 'El estado del campo se registro y editó correctamente.',
                                        type: 'green',
                                        buttons: {
                                            OK: function () {

                                                $("#btnBusquedaLogCatalogo").click();
                                            }
                                        }
                                    });
                                }
                            })
                        }
                    },
                    cancel: {
                        text: 'Cancelar',
                        action: function () {

                            $(that).closest("td").find("input[value=0]").prop("checked", false);
                            $(that).closest("td").find("input[value=1]").prop("checked", false);

                        }
                    }
                }
            });
        }



        applyEstado = false;

    });

    $("#tableCatalogo").on("change", ".puede_persistir", function () {
        if (applyEstado) {

            return;
        }
        applyEstado = true;

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var that = this;
        catalogoId = arrrayClass[1];
        add = arrrayClass[2];
        var valor = $(this).val();
        if (valor == 1) {
            opcion = "Si";


        } if (valor == 0) {
            opcion = "No";
        }

        var nombreCampo = $(this).closest("tr").find("td.nombreCampo").text().replace("*No se encuentra configurado en el LOG", "");

        if (add != "add") {
            $.confirm({
                title: 'Confirmación de cambio',
                content: '¿Está seguro de cambiar la persistencia del campo ' + nombreCampo + ' a ' + opcion + '?',
                type: 'orange',
                buttons: {
                    confirm: {
                        text: 'Confirmar',
                        action: function () {
                            $.ajax({
                                url: "/LogCampo/Update",
                                type: 'POST',
                                data: {
                                    catalogoId: catalogoId,
                                    propiedad: "puede_persistir",
                                    valor: valor

                                },
                                error: function (resultado) {
                                    $.alert({
                                        title: "Error",
                                        content: "Se generó un error al editar la persistencia del campo.",
                                        type: 'red',
                                        buttons: {
                                            OK: function () {
                                            }
                                        }
                                    });
                                },
                                success: function (resultado) {
                                    $('body').loadingModal('hide');

                                    $.alert({
                                        title: TITLE_EXITO,
                                        content: 'La persistencia del campo se editó correctamente.',
                                        type: 'green',
                                        buttons: {
                                            OK: function () {



                                            }
                                        }
                                    });
                                }
                            })
                        }
                    },
                    cancel: {
                        text: 'Cancelar',
                        action: function () {
                            if (valor == 0) {

                                $(that).closest("td").find("input[value=0]").prop("checked", false);
                                $(that).closest("td").find("input[value=1]").prop("checked", true);

                            }
                            if (valor == 1) {



                                $(that).closest("td").find("input[value=0]").prop("checked", true);
                                $(that).closest("td").find("input[value=1]").prop("checked", false);
                            }

                        }
                    }
                }
            });
        } else {
            $.confirm({
                title: 'Confirmación de cambio',
                content: '¿Está seguro de cambiar la persistencia del campo ' + nombreCampo + '(Se insertará a la tabla LogCampo) a ' + opcion + '?',
                type: 'orange',
                buttons: {
                    confirm: {
                        text: 'Confirmar',
                        action: function () {
                            $.ajax({
                                url: "/LogCampo/InsertCampo",
                                type: 'POST',
                                data: {
                                    propiedad: "puede_persistir",
                                    valor: valor,
                                    nombre: nombreCampo

                                },
                                error: function (resultado) {
                                    $.alert({
                                        title: "Error",
                                        content: "Se generó un error al editar la persistencia del campo.",
                                        type: 'red',
                                        buttons: {
                                            OK: function () {
                                                $(that).closest("td").find("input[value=0]").prop("checked", false);
                                                $(that).closest("td").find("input[value=1]").prop("checked", false);
                                            }
                                        }
                                    });
                                },
                                success: function (resultado) {
                                    $('body').loadingModal('hide');

                                    $.alert({
                                        title: TITLE_EXITO,
                                        content: 'La persistencia del campo se registro y editó correctamente.',
                                        type: 'green',
                                        buttons: {
                                            OK: function () {

                                                $("#btnBusquedaLogCatalogo").click();
                                            }
                                        }
                                    });
                                }
                            })
                        }
                    },
                    cancel: {
                        text: 'Cancelar',
                        action: function () {

                            $(that).closest("td").find("input[value=0]").prop("checked", false);
                            $(that).closest("td").find("input[value=1]").prop("checked", false);

                        }
                    }
                }
            });

        }
        applyEstado = false;

    });


    $("#btnLimpiarLogCatalogo").click(function () {
        $.ajax({
            url: "/LogCampo/LimpiarBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });

    $("#catalogo_tablaId").change(function () {
        var idTabla = $("#catalogo_tablaId").val();


        $.ajax({
            url: "/LogCampo/ChangeIdTabla",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idTabla: idTabla
            },
            error: function (detalle) {
                $("#btnBusquedaLogCatalogo").removeAttr("disabled");
            },
            success: function (idTabla) {

            }
        });
    });
});
