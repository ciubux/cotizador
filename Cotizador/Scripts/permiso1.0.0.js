

$(document).ready(function () {
    $(function () {
        $("#menuCategoriaPermisos").menu();
        $(".menuUsuarios").menu();
    });



    $('#menuCategoriaPermisos li ul li').click(function (e) {

        var idPermiso = $(this).find("div").attr("id");
        var permiso = $(this).find("div").html();

        $.ajax({
            url: "/Permiso/consultaSiExistenCambios",
            type: 'POST',
            error: function (detalle) {
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function (resultado) {
                //alert(resultado)
                if (resultado == 'True') {
                    $.confirm({
                        title: "EXISTEN CAMBIOS",
                        type: 'orange',
                        content: 'Se modificó la asignación de usuarios de este permiso, si selecciona otro permiso, se perderán los cambios aplicados en este. ¿Desea Continuar?',
                        buttons: {
                            aplica: {
                                text: 'SI',
                                btnClass: 'btn-success',
                                action: function () {
                                    changePermiso(idPermiso);
                                }
                            },
                            cancelar: {
                                text: 'NO',
                                btnClass: 'btn-danger',
                                action: function () {
                                    location.reload();
                                }
                            }
                        }
                    });
                }
                else {
                    changePermiso(idPermiso);
                }
            }
        });


      
    });

    function changePermiso(idPermiso) {
        $.ajax({
            url: "/Permiso/changeIdPermiso",
            type: 'POST',
            data: {
                idPermiso: idPermiso
            },
            error: function (detalle) {
                mostrarMensajeErrorProceso(detalle.responseText);

            },
            success: function () {
                location.reload();
            }
        });

    }


    /*$(".menuUsuariosConPemirso").on('click', 'li', function (e) {

    $(".menuUsuariosSinPemirso").on('click', 'li', function (e) {*/

    $(".menuUsuarios").on('click', 'li', function (e) {
        if (e.ctrlKey || e.metaKey) {
            $(this).toggleClass("selected");
        } else {
            $(this).addClass("selected").siblings().removeClass('selected');
        }
    }).sortable({
        connectWith: "ul",
        delay: 150, //Needed to prevent accidental drag when trying to select
        revert: 0,
        helper: function (e, item) {
            var helper = $('<li/>');
            if (!item.hasClass('selected')) {
                item.addClass('selected').siblings().removeClass('selected');
            }
            var elements = item.parent().children('.selected').clone();
            item.data('multidrag', elements).siblings('.selected').remove();
            return helper.append(elements);
        },
        stop: function (e, info) {
            info.item.after(info.item.data('multidrag')).remove();
          /*  alert(e.currentTarget.name)
            alert(info.detail)*/
            changeUsuarios();
        }

        });


    function changeUsuarios() {
        $('body').loadingModal({
            text: 'Actualizando...'
        });
        var idsUsuario = [];

        $('#usuariosConPermiso li').each(function (i) {
            idsUsuario.push($(this).attr("id"));
        });

        var yourWindow;
        $.ajax({
            url: "/Permiso/changeUsuarios",
            type: 'POST',
            data: {
                idsUsuario: idsUsuario
            },
            error: function (detalle) {
                $('body').loadingModal('hide');
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function () {
                $('body').loadingModal('hide');
               
            }
        });
    }


    $("#btnCancelarAsignacionPermisos").click(function () {
        $.ajax({
            url: "/Permiso/deshacerCambiosUsuario",
            type: 'POST',
            error: function (detalle) {
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function () {
                location.reload();
            }
        });
    });
    







    $("#btnFinalizarAsignacionPermisos").click(function () {    
        $.ajax({
            url: "/Permiso/UpdatePermiso",
            type: 'POST',
            error: function (detalle) {
                mostrarMensajeErrorProceso(detalle.responseText);
            },
            success: function () {
                $.alert({
                    title: TITLE_EXITO,
                    content: "La asignación del permiso a los usuarios se realizó correctamente.",
                    type: 'green',
                    buttons: {
                        OK: function () {
                            location.reload();
                        }
                    }
                });
            }
        });


    });

});

