jQuery(function ($) {
    $('body').on('click', "button#password_modal_save", function () {

        $('#avisoCambioPasswordActual').hide();
        $('#avisoCambioPasswordConfirmacion').hide();

        var passActual = $("#passActual").val();
        var passNuevo = $("#passNuevo").val();
        var passConfirmacion = $("#passNuevoConfirmacion").val();
        var errorActual;
        var errorNuevo;

        if (passNuevo != passConfirmacion || passNuevo == '' || passConfirmacion == '') {
            errorNuevo = 0;
        }
        else {
            errorNuevo = 1;
        }

        $.ajax({
            url: "/Usuario/confirmarPasswordActual",
            type: 'POST',
            datatype: "bool",
            async: false,
            data: {
                passActual: passActual
            },
            success: function (resp) {
                if (resp == 1) {
                    errorActual = 1;
                }
                else {
                    errorActual = 0;
                }
            }
        });
        if (errorActual == 0 && errorNuevo == 0) {
            $('#avisoCambioPasswordActual').show();
            $('#avisoCambioPasswordConfirmacion').show();
        }
        if (errorNuevo == 0) {
            $('#avisoCambioPasswordConfirmacion').show();
        }
        if (errorActual == 0) {
            $('#avisoCambioPasswordActual').show();
        }
        if (errorActual == 1 && errorNuevo == 1) {
            $.ajax({
                url: "/Usuario/cambiarPassword",
                type: 'POST',
                datatype: "bool",
                data: {
                    passNuevo: passNuevo
                },
                success: function (resp) {
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

                },
                error: function () {
                    $.alert({
                        title: 'Error',
                        content: 'Se generó un error al intentar cambiar la contraseña.',
                        type: 'red',
                        buttons:
                        {
                            OK: function () { }
                        }
                    });
                }
            });
        }
    });
});



$("body").on('mouseenter', ".tooltip-motivo-restriccion", function () {
    $(this).removeClass('tooltip-motivo-restriccion');
    $(this).addClass('tooltip-label');
});


