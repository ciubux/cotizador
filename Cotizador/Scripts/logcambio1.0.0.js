jQuery(function ($) {

    var TITLE_EXITO = 'Operación Realizada';

    var estadoPersistencia;
    var valestadoPersistencia;
    var catalogoId;
    var nombreCatalogo;


    $(document).ready(function () {
        if ($("#catalogo_tablaId").val() == "") {
            $("#btnBusqueda").attr("disabled", "disabled");
        }
        $(".form-control").on("change", function () {
            var a = $("#catalogo_tablaId").val();
            if (a == "") {
                $("#btnBusqueda").attr("disabled", "disabled");
            } else {
                $("#btnBusqueda").removeAttr('disabled');
            }
        });
    });

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

                $("#btnBusqueda").removeAttr("disabled");
                $("#tableCatalogo > tbody").empty();
                $("#tableCatalogo").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < list.length; i++) {

                    if (list[i].estadoCatalogo == 1) {
                        var label = '<input class="1 estadoCatalogo ' + list[i].catalogoId + " " + list[i].nombre + ' radio-input" checked type="radio" name="catalogo_estado ' + list[i].catalogoId + '" id="catalogo_estado_si" value="1"><span>Activo</span>';
                    }
                    else {
                        label = '<input class="1 estadoCatalogo ' + list[i].catalogoId + " " + list[i].nombre + 'radio-input" type="radio" name="catalogo_estado ' + list[i].catalogoId + '" id="catalogo_estado_si" value="1"><span>Activo</span>';
                    };

                    if (list[i].estadoCatalogo == 0) {
                        var label2 = '<input class="0 estadoCatalogo ' + list[i].catalogoId + " " + list[i].nombre + ' radio-input" checked type="radio" name="catalogo_estado ' + list[i].catalogoId + '" id="catalogo_estado_no" value="0"><span>Inactivo</span>';
                    }
                    else {
                        label2 = '<input class="0 estadoCatalogo ' + list[i].catalogoId + " " + list[i].nombre + ' radio-input" type="radio" name="catalogo_estado ' + list[i].catalogoId + '" id="catalogo_estado_no" value="0"><span>Inactivo</span>';
                    };
                    if (list[i].puede_persistir == 1) {
                        var label3 = '<input class="1 puede_persistir ' + list[i].catalogoId + " " + list[i].nombre + ' radio-input" checked type="radio" name="puede_persistir ' + list[i].catalogoId + '" id="catalogo_persiste_si" value="1"><span>Si</span>';
                    }
                    else {
                        label3 = '<input class="1 puede_persistir ' + list[i].catalogoId + " " + list[i].nombre + ' radio-input" type="radio" name="puede_persistir ' + list[i].catalogoId + '" id="catalogo_persiste_si" value="1"><span>Si</span>';
                    };

                    if (list[i].puede_persistir == 0) {
                        var label4 = '<input class="0 puede_persistir ' + list[i].catalogoId + " " + list[i].nombre + ' radio-input" checked type="radio" name="puede_persistir ' + list[i].catalogoId + '" id="catalogo_persiste_no" value="0"><span>No</span>';
                    }
                    else {
                        label4 = '<input class="0 puede_persistir ' + list[i].catalogoId + " " + list[i].nombre + ' radio-input" type="radio" name="puede_persistir ' + list[i].catalogoId + '" id="catalogo_persiste_no" value="0"><span>No</span>';
                    };
                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list[i].catalogoId + '  </td>' +
                        '<td>  ' + list[i].catalogoId + '  </td>' +
                        '<td>  ' + list[i].nombre + '  </td>' +
                        '<td> <div class="radio radio-inline"><label class="radio-label">' + label + '</label></div><div class="radio radio-inline"><label class="radio-label">' + label2 + '</label></div> </td>' +
                        '<td> <div class="radio radio-inline"><label class="radio-label">' + label3 + '</label></div><div class="radio radio-inline"><label class="radio-label">' + label4 + '</label></div> </td>' +
                        '<td>  ' + list[i].codigo + '  </td>' +
                        '<td>  ' + list[i].tabla_referencia + '  </td>' +
                        '<td>  ' + list[i].campo_referencia + '  </td>' +
                        '</tr>';

                    $("#tableCatalogo").append(ItemRow);


                }
            }
        });
    });

    $("#tableCatalogo").on("change", "input", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        valestadoPersistencia = arrrayClass[0];
        estadoPersistencia = arrrayClass[1];
        catalogoId = arrrayClass[2];
        nombreCatalogo = arrrayClass[3];

        if ("estadoCatalogo" == estadoPersistencia) {
            
            confirmacionEstado();

        }
        if ("puede_persistir" == estadoPersistencia) {
            
            confirmacionPersistencia();
        }

    });
    
    var opcion;
    function confirmacionEstado() {
        if (valestadoPersistencia == 1) { opcion = "Activo" }
        if (valestadoPersistencia == 0) { opcion = "Inactivo" }
        $.confirm({
            title: 'Confirmación de cambio',
            content: '¿Está seguro de cambiar el estado de la tabla ' + nombreCatalogo + ' a ' + opcion+ '?',
            type: 'orange',
            buttons: {
                confirm: {
                    text: 'Sí',
                    action: function () {
                        $.ajax({
                            url: "/LogCambio/Update",
                            type: 'POST',
                            data: {
                                catalogoId: catalogoId, 
                                propiedad: "estadoCatalogo",
                                valor: valestadoPersistencia                              

                            },
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
                                            
                                                $("#btnBusqueda").click();
                                            
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

                        
                            $("#btnBusqueda").click();

                        
                    }
                }
            }
        })

    }
    
    function confirmacionPersistencia() {
        if (valestadoPersistencia == 1) {opcion = "Si" }
        if (valestadoPersistencia == 0) { opcion = "No" }
        $.confirm({
            title: 'Confirmación de cambio',
            content: '¿Está seguro de cambiar la persistencia de la tabla ' + nombreCatalogo + ' a ' + opcion+ '?',
            type: 'orange',
            buttons: {
                confirm: {
                    text: 'Sí',
                    action: function () {
                        $.ajax({
                            url: "/LogCambio/Update",
                            type: 'POST',
                            data: {
                                catalogoId: catalogoId,
                                propiedad: "puede_persistir",
                                valor: valestadoPersistencia

                            },
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
                                            
                                                $("#btnBusqueda").click();
                                            
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

                        
                            $("#btnBusqueda").click();
                        

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

    $("#catalogo_tablaId").change(function () {
        var idTabla = $("#catalogo_tablaId").val();

      
        $.ajax({
            url: "/LogCambio/ChangeIdTabla",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idTabla: idTabla
            },
            error: function (detalle) {
                $("#btnBusqueda").removeAttr("disabled");
            },
            success: function (idTabla) {
                
            }
        });
    });
});
