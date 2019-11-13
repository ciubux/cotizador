jQuery(function ($) {

    $(document).on('click', "button#password_modal_save", function () {

        $('#avisoCambioPasswordActual').hide();
        $('#avisoCambioPasswordConfirmacion').hide();

        var idUsuarioCambioPass = $('#usuario_idUsuario').val();
        var passActual = $("#passActual").val();
        var passNuevo = $("#passNuevo").val();
        var passConfirmacion = $("#passNuevoConfirmacion").val();

        $.ajax({
            url: "/Usuario/cambiarContraseña",
            type: 'POST',
            datatype: "bool",            
            data: {
                idUsuarioCambioPass: idUsuarioCambioPass,
                passActual: passActual,
                passNuevo: passNuevo
            },
            success: function (resp) {
                RespuestaFinal(resp);  
            }
        });
        if (passNuevo != passConfirmacion || passNuevo == '' || passConfirmacion == '')
        {
            $('#avisoCambioPasswordConfirmacion').show();  
        } 

        function RespuestaFinal(resp)
        {
            if (resp == 1) {
                $.alert({
                    title: "Operación Realizada",
                    type: 'green',
                    content: 'La contraseña se cambio satisfactoriamente',
                    buttons: {
                        OK: function () {
                            window.location = '/General/Exit';
                        }
                    }
                });

            }
            else 
            $('#avisoCambioPasswordActual').show();
        }        
    });
});