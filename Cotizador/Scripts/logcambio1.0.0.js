jQuery(function ($) {

    var TITLE_EXITO = 'Operación Realizada';

    var estado_lista;
    var persiste_lista;
    var persiste;
    var estado;

    if ($("#catalogo_catalogoId").val()=="")
    {
        $("#btnBusqueda").attr("disabled", "disabled");
    }
    

    $("#btnBusqueda").click(function () {

                $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/LogCambio/SearchList",
            type: 'POST',
            dataType: 'JSON',

            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (list) {

                estado_lista = list.estado;
                persiste_lista = list.puede_persistir;

               
                    $("#btnBusqueda").removeAttr("disabled");
                    $("#tableCatalogo > tbody").empty();
                    $("#tableCatalogo").footable({
                        "paging": {
                            "enabled": true
                        }
                    });

                    var label;
                    var label2;
                    var label3;
                    var label4;
                    if (list.estado == 1) {
                        label = '<input class="radio-input" checked type="radio" name="catalogo_estado" id="catalogo_estado_si" value="1"><span>Activo</span>'
                    }
                    else {
                        label = '<input class="radio-input" type="radio" name="catalogo_estado" id="catalogo_estado_si" value="1"><span>Activo</span>'
                    };

                    if (list.estado == 0) {
                        label2 = '<input class="radio-input" checked type="radio" name="catalogo_estado" id="catalogo_estado_no" value="0"><span>Inactivo</span>'
                    }
                    else {
                        label2 = '<input class="radio-input" type="radio" name="catalogo_estado" id="catalogo_estado_no" value="0"><span>Inactivo</span>'
                    };
                    if (list.puede_persistir == 1) {
                        label3 = '<input class="radio-input" checked type="radio" name="puede_persistir" id="catalogo_persiste_si" value="1"><span>Si</span>'
                    }
                    else {
                        label3 = '<input class="radio-input" type="radio" name="puede_persistir" id="catalogo_persiste_si" value="1"><span>Si</span>'
                    };

                    if (list.puede_persistir == 0) {
                        label4 = '<input class="radio-input" checked type="radio" name="puede_persistir" id="catalogo_persiste_no" value="0"><span>No</span>'
                    }
                    else {
                        label4 = '<input class="radio-input" type="radio" name="puede_persistir" id="catalogo_persiste_no" value="0"><span>No</span>'
                    };

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list.catalogoId + '  </td>' +
                        '<td>  ' + list.catalogoId + '  </td>' +
                        '<td>  ' + list.nombre + '  </td>' +
                        '<td> <div class="radio radio-inline"><label class="radio-label">' + label + '</label></div><div class="radio radio-inline"><label class="radio-label">' + label2 + '</label></div> </td>' +
                        '<td> <div class="radio radio-inline"><label class="radio-label">' + label3 + '</label></div><div class="radio radio-inline"><label class="radio-label">' + label4 + '</label></div> </td>' +
                        '<td>  ' + list.codigo + '  </td>' +
                        '<td>  ' + list.tabla_referencia + '  </td>' +
                        '<td>  ' + list.campo_referencia + '  </td>' +
                        '</tr>';

                    $("#tableCatalogo").append(ItemRow);
                


            }
        });
    });

    $("#tableCatalogo").on("change", ".radio ", function () {
        persiste = $('input[name=puede_persistir]:checked', '#tableCatalogo').val();
        estado = $('input[name=catalogo_estado]:checked', '#tableCatalogo').val();

        if (estado_lista != estado) {
            changeInputInt("estadoCatalogo", estado);
            confirmacionEstado();

        }
        if (persiste_lista != persiste) {
            changeInputInt("puede_persistir", persiste);
            confirmacionPersistencia();
        }

    });
    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/LogCambio/ChangeInputInt",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () {

            }
        });
    }

    function confirmacionEstado() {
        if (estado == 1) { estado = "Activo" }
        if (estado == 0) { estado = "Inactivo" }
        $.confirm({
            title: 'Confirmación de cambio',
            content: '¿Está seguro de cambiar el estado de la tabla a ' + estado + '?',
            type: 'orange',
            buttons: {
                confirm: {
                    text: 'Sí',
                    action: function () {
                        $.ajax({
                            url: "/LogCambio/Update",
                            type: 'POST',
                            error: function (resultado) {
                                $.alert({
                                    title: "Error",
                                    content: "Se generó un error al editar el estado de la tabla.",
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
                                    content: 'El estado de la tabla se editó correctamente.',
                                    type: 'green',
                                    buttons: {
                                        OK: function () {
                                            $(document).ready(function () {
                                                $("#btnBusqueda").click();
                                            });
                                        }
                                    }
                                });
                            }
                        })
                    }
                },
                cancel: {
                    text: 'No',
                    action: function () {

                        $(document).ready(function () {
                            $("#btnBusqueda").click();

                        });
                    }
                }
            }
        })

    }

    function confirmacionPersistencia() {
        if (persiste == 1) { persiste = "Si" }
        if (persiste == 0) { persiste = "No" }
        $.confirm({
            title: 'Confirmación de cambio',
            content: '¿Está seguro de cambiar la persistencia de la tabla a ' + persiste + '?',
            type: 'orange',
            buttons: {
                confirm: {
                    text: 'Sí',
                    action: function () {
                        $.ajax({
                            url: "/LogCambio/Update",
                            type: 'POST',
                            error: function (resultado) {
                                $.alert({
                                    title: "Error",
                                    content: "Se generó un error al editar la persistencia de la tabla.",
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
                                    content: 'La persistencia de la tabla se editó correctamente.',
                                    type: 'green',
                                    buttons: {
                                        OK: function () {
                                            $(document).ready(function () {
                                                $("#btnBusqueda").click();
                                            });
                                        }
                                    }
                                });
                            }
                        })
                    }
                },
                cancel: {
                    text: 'No',
                    action: function () {

                        $(document).ready(function () {
                            $("#btnBusqueda").click();
                        });

                    }
                }
            }
        })

    }




    $("#btnLimpiarBusqueda").click(function () {
        $.ajax({
            url: "/LogCambio/LimpiarBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });

    $("#catalogo_catalogoId").change(function () {
        var idCiudad = $("#catalogo_catalogoId").val();

        var textCiudad = $("#catalogo_catalogoId option:selected").text();
        $.ajax({
            url: "/LogCambio/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad
            },
            error: function (detalle) {
                $("#btnBusqueda").removeAttr("disabled");
            },
            success: function (idCiudad) {
                location.reload();
            }
        });
    });
});

