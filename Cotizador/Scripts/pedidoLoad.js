jQuery(function ($) {
    
    $(document).ready(function () {
        FooTable.init('#tableDetalleStockPedidos');
    });

    $("#btnCargarPedidos").click(function () {
        $("#btnCargarPedidos").attr("disabled", true);
        $('body').loadingModal({
            text: 'Cargando pedidos...'
        });

        $("#formLoad").submit();
    });

    $("#btnActualizarRUC").click(function () {
        $('body').loadingModal({
            text: 'Actualizando RUC para carga masiva'
        });
        var ruc = $("#ruc").val();
        ruc = ruc.trim();
        $("#ruc").val(ruc);

        $.ajax({
            url: "/General/actualizarParametroRUCCargaMasiva",
            type: 'POST',
            data: {
                ruc: ruc
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