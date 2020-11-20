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

    function cargarImagenFirma() {
        $.ajax({
            url: "/Usuario/GetFirmaLogueado",
            type: 'POST',
            datatype: "JSON",
            data: {
            },
            success: function (res) {
                $("#verImagenFirma").attr("src", "data:image/png;base64," + res.image);
            }
        });
        
    }

    var imagenValida = false;

    $('#imgSignUpload').change(function (event) {
        var fileInput = $(event.target);
        var maxSize = fileInput.data('max-size');
        var maxSizeText = fileInput.data('max-size-text');
        var imagenValida = true;
        if (fileInput.get(0).files.length) {
            var fileSize = fileInput.get(0).files[0].size; // in bytes

            if (fileSize > maxSize) {
                $.alert({
                    title: "Imagen Inválida",
                    type: 'red',
                    content: 'El tamaño del archivo debe ser como maximo ' + maxSizeText + '.',
                    buttons: {
                        OK: function () { }
                    }
                });
                imagenValida = false;
            }


        } else {
            $.alert({
                title: "Imagen Inválida",
                type: 'red',
                content: 'Seleccione una imagen por favor.',
                buttons: {
                    OK: function () { }
                }
            });
            imagenValida = false;
        }

        if (imagenValida) {
            $('body').loadingModal({
                text: '...'
            });

            var that = document.getElementById('imgSignUpload');
            var file = that.files[0];
            var form = new FormData();

            var reader = new FileReader();
            var mime = file.type;

            reader.onload = function (e) {
                // get loaded data and render thumbnail.
                document.getElementById("verImagenFirma").src = e.target.result;
                var img = new Image();

                img.onload = function () {
                    var width = img.width;
                    var height = img.height;

                    imagenValida = true;
                    $('body').loadingModal('hide');
                };
                img.src = e.target.result;
            };

            // read the image file as a data URL.
            reader.readAsDataURL(file);

            form.append('image', file);
        }
    });

    $('body').on('click', "button#changesign_modal_save", function () {
        if (imagenValida) {
            $('body').loadingModal({
                text: '...'
            });

            var that = document.getElementById('imgSignUpload');
            var url = $(that).data("urlSetImage");

            $.ajax({
                url: url,
                type: 'POST',
                cache: false,
                data: {
                    imgBase: $("#verImagenFirma").attr("src")
                },
                success: function () { }

            }).done(function () {
                $('body').loadingModal('hide');
            });
        } 
    });
});



$("body").on('mouseenter', ".tooltip-motivo-restriccion", function () {
    $(this).removeClass('tooltip-motivo-restriccion');
    $(this).addClass('tooltip-label');
});


