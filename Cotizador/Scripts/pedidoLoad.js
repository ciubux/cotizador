jQuery(function ($) {

    $("#btnActualizarRUC").click(function () {
        $('body').loadingModal({
            text: 'Actualizando RUC para carga masiva'
        });
        $.ajax({
            url: "/General/actualizarParametroRUCCargaMasiva",
            type: 'POST',
            data: {
                ruc: $("#ruc").val()
            },
            error: function (detalle) {
                $('body').loadingModal('hide')
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function () {
                $('body').loadingModal('hide')
                $.alert({
                    title: "RUC Actualizado",
                    type: 'green',
                    content: 'Se actualizó el RUC correctamente. Ya puede obtener los datos del cliente desde el archivo Excel.',
                    buttons: {
                        OK: function () { }
                    }
                });


            }
        });


    })


})