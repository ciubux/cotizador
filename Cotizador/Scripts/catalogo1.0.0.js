jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    
    $("#btnBusqueda").click(function () {

        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/Catalogo/SearchList",
            type: 'POST',
            dataType: 'JSON',

            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (list) {
                if (list.catalogoId !== 0)
                {
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
                if (list.estado == 1)
                {
                    label = '<input class="radio-input" checked type="radio" name="catalogo_estado" id="catalogo_estado_si" value="1"><span>Activo</span>'
                }
                else
                {
                    label = '<input class="radio-input" type="radio" name="catalogo_estado" id="catalogo_estado_si" value="1"><span>Activo</span>'
                };

                if (list.estado == 0)
                {
                    label2 = '<input class="radio-input" checked type="radio" name="catalogo_estado" id="catalogo_estado_no" value="0"><span>Inactivo</span>'
                }
                else
                {
                    label2 = '<input class="radio-input" type="radio" name="catalogo_estado" id="catalogo_estado_no" value="0"><span>Inactivo</span>'
                };
                if (list.puede_persistir == 1)
                {
                    label3 = '<input class="radio-input" checked type="radio" name="puede_persistir" id="catalogo_persiste_si" value="1"><span>Si</span>'
                }
                else
                {
                    label3 = '<input class="radio-input" type="radio" name="puede_persistir" id="catalogo_persiste_si" value="1"><span>Si</span>'
                };

                if (list.puede_persistir == 0)
                {
                    label4 = '<input class="radio-input" checked type="radio" name="puede_persistir" id="catalogo_persiste_no" value="0"><span>No</span>'
                }
                else
                {
                    label4 = '<input class="radio-input" type="radio" name="puede_persistir" id="catalogo_persiste_no" value="0"><span>No</span>'
                };

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list.catalogoId + '  </td>' +
                        '<td>  ' + list.catalogoId + '  </td>' +                       
                        '<td>  ' + list.nombre + '  </td>' +
                        '<td > <div class="radio radio-inline" ><label class="radio-label">' + label + '</label></div><div class="radio radio-inline"><label class="radio-label">' + label2 + '</label></div> </td>' +
                        '<td> <div class="radio radio-inline"><label class="radio-label">' + label3 + '</label></div><div class="radio radio-inline"><label class="radio-label">' + label4 + '</label></div> </td>' +
                        '<td>  ' + list.codigo + '  </td>' +
                        '<td>  ' + list.tabla_referencia + '  </td>' +
                        '<td>  ' + list.campo_referencia + '  </td>' +
                        '</tr>';

                    $("#tableCatalogo").append(ItemRow);
                }
                
                
            }
        });
    });

    $("#tableCatalogo").on("change", ".radio ", function () {
        var estado = $('input[name=catalogo_estado]:checked', '#tableCatalogo').val();
        var persiste = $('input[name=puede_persistir]:checked', '#tableCatalogo').val();

        if (estado !== 0 && persiste !== 0)
        {
        changeInputInt("estado", estado);
        changeInputInt("puede_persistir", persiste);
        }
        
        
    
        function changeInputInt(propiedad, valor) {
            $.ajax({
                url: "/Catalogo/ChangeInputInt",
                type: 'POST',
                data: {
                    propiedad: propiedad,
                    valor: valor
                },
                success: function ()
                {           
                    $.confirm({
                        title: 'Confirmación Cambio',
                        content: '¿Está seguro de cambiar el estado y persistencia de la tabla?',
                        type: 'orange',
                        buttons: {
                            confirm: {
                                text: 'Sí',
                                action: function () {
                                    $.ajax({
                                        url: "/Catalogo/Update",
                                        type: 'POST',
                                        error: function (resultado) {
                                            $.alert({
                                                title: "Error",
                                                content: "Se generó un error al editar la tabla.",
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
                                                content: 'El vendedor se editó correctamente.',
                                                type: 'green',
                                                buttons: {
                                                    OK: function () {
                                                        window.location = '/Catalogo/Lista';
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
                                    location.reload();
                                }
                            }
                        }
                    })

                    
                }
            });
        }


    });    


    $("#btnLimpiarBusqueda").click(function () {
        $.ajax({
            url: "/Catalogo/LimpiarBusqueda",
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
            url: "/Catalogo/ChangeIdCiudad",
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

