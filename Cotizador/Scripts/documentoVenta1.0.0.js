jQuery(function ($) {
   
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';
    
    $(document).ready(function () {

        $.ajax({
            url: "/documentoVenta/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                
            },

            success: function (list) {
                
                $("#tableNotificacionAnulacion > tbody").empty();
                $("#tableNotificacionAnulacion").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < list.length; i++) {                                       
                    var boton;
                    if (list[i].estado_anulacion == 1)
                    {                                               
                        boton = '<button type="button" id="boton" class="' + list[i].idDocumentoVenta + " " + list[i].estado_anulacion + ' btnEditarEstadoNotficacion btn btn-secondary" disabled>Notificado</button>';
                    }
                    if (list[i].estado_anulacion == 0)
                    {                        
                        boton = '<button type="button" id="boton" class="' + list[i].idDocumentoVenta + " " + list[i].estado_anulacion + ' btnEditarEstadoNotficacion btn btn-primary ">Notificado</button>';
                    }
                    if (list[i].telefonoContacto == null)
                    {
                        list[i].telefonoContacto = " ";
                    }
                    if (list[i].contacto == null) {
                        list[i].contacto = " ";
                    }
                    if (list[i].correoEnvio == null) {
                        list[i].correoEnvio = " ";
                    }


                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list[i].idDocumentoVenta + '  </td>' +
                        '<td>  ' + list[i].serie + "-" + list[i].numero_factura+ '  </td>' +
                        '<td>  ' + list[i].razon_social + '  </td>' +
                        '<td>  ' + list[i].ruc + '  </td>' +  
                        '<td>  ' + list[i].nombre + '  </td>' +
                        '<td>  ' + "Nombre:" + list[i].contacto + "<br/> Correo:" + list[i].correoEnvio + "<br/> Telefono:" +  list[i].telefonoContacto + '  </td>' +  
                        '<td>  ' + list[i].fecha_solicitud + '  </td>' +
                        '<td>  ' + list[i].monto + '  </td>' +
                        '<td>  ' + " " + '  </td>' +
                        '<td>' + boton +                                   
                        '</td>' +
                        '</tr>';

                    $("#tableNotificacionAnulacion").append(ItemRow);

                }

                if (ItemRow.length > 0) {
                    $("#msgBusquedaSinResultados").hide();
                }
                else {
                    $("#msgBusquedaSinResultados").show();
                }

            }
        });
    });






    $("#tableNotificacionAnulacion").on("click", "button.btnEditarEstadoNotficacion", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idDocumentoVenta = arrrayClass[0];
        var preestado = arrrayClass[1];
        var estadofinal;
        if (preestado == 0) { estadofinal = 1;}

       
        
        $.confirm({
            title: 'Confirmación de cambio',
            content: '¿Está seguro de cambiar el estado de la notificación a Notificado?',
            type: 'orange',
            buttons: {
                confirm: {
                    text: 'Sí',
                    action: function () {
                        $.ajax({
                            url: "/DocumentoVenta/Update",
                            type: 'POST',
                            async: false,
                            dataType: 'JSON',
                            data: {
                                idDocumentoVenta: idDocumentoVenta,
                                estado_anulacion: estadofinal
                            },
                            error: function (resultado) {
                                $.alert({
                                    title: "Error",
                                    content: "Se generó un error al editar el estado de la notificacion.",
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
                                    content: 'El estado de la notificación se editó correctamente.',
                                    type: 'green',
                                    buttons: {
                                        OK: function () {

                                            location.reload();

                                        }
                                    }
                                });
                            }
                        });
                    }
                },
                cancel: {                   
                    text: 'No',
                    action: function () {


                    }
                }
            }
        });
    });
});


    


