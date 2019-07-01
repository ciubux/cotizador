

$(document).ready(function () {
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición; no se guardarán los cambios?';
    var TITLE_EXITO = 'Operación Realizada';

    $(function () {
        $("#menuCategoriaPermisos").menu();
        $(".menuUsuarios").menu();
    });

    $(function () {
        FooTable.init('#tablePermisos');
    });


    $('#permiso_descripcion_corta').keyup(function (e) {
        var textSearch = $('#permiso_descripcion_corta').val();
        
        setTimeout(function () {
            var textNow = $('#permiso_descripcion_corta').val();
            if (textSearch == textNow) {
                filterPermisos(textSearch);
            }
        }, 1000);
    });

    function filterPermisos(textSearch) {
        $("#tablePermisos > tbody").empty();
        $("#tablePermisos").footable({
            "paging": {
                "enabled": true
            }
        });

        $('#dtPermisos tr').each(function () {
            var tr = this;
            var founds = 0;
            $(tr).find('td[isSearchable=1]').each(function () {
                var text = $(this).html();
                if (text.indexOf(textSearch) >= 0) {
                    founds = founds + 1;
                } 
            });

            if (founds > 0) {
                $("#tablePermisos").append('<tr data-expanded="true">' + $(tr).html() + '</tr>');
            }            
        });

        //FooTable.init('#tablePermisos');
    }
   
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
    
    $("#btnFinalizarEdicionPermiso").click(function () {
        if ($("#permiso_idPermiso").val() == '0') {
            $.alert({
                title: "ERROR",
                type: 'red',
                content: 'ERROR',
                buttons: {
                    OK: function () { }
                }
            });
            //crearRol();
        }
        else {
            editarPermiso();
        }
    });

    $("#permiso_descripcion_corta").change(function () {
        changeInputString("descripcion_corta", $("#permiso_descripcion_corta").val());
    });

    $("#permiso_descripcion_larga").change(function () {
        changeInputString("descripcion_larga", $("#permiso_descripcion_larga").val());
    });

    $("#btnCancelarPermiso").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/Permiso/CancelarCreacionPermiso', null)
    })

    $(document).on('click', "button.btnEditarPermiso", function () {
        //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idPermiso = arrrayClass[0];

        $.ajax({
            url: "/Permiso/ConsultarSiExistePermiso",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idPermiso: idPermiso
            },
            success: function (resultado) {
                //if (resultado.existe == "false") {

                    $.ajax({
                        url: "/Permiso/iniciarEdicionPermiso",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del Permiso."); },
                        success: function (fileName) {
                            window.location = '/Permiso/Editar';
                        }
                    });

                /*}
                else {
                    if (resultado.idPermiso == 0) {
                        alert('Está creando un nuevo permiso; para continuar por favor diríjase a la página "Crear/Modificar Permiso" y luego haga clic en el botón Cancelar.');
                    }

                    else {
                        alert('Ya se encuentra editando un permiso para continuar por favor dirigase a la página "Crear/Modificar Permiso".');
                    }
                }*/
            }
        });
    });

    function validacionDatosPermiso() {

        if ($("#permiso_descripcion_corta").val().length < 10) {
            $.alert({
                title: "Nombre Inválido",
                type: 'orange',
                content: 'Debe ingresar un Nombre de permiso válido.',
                buttons: {
                    OK: function () { $('#permiso_descripcion_corta').focus(); }
                }
            });
            return false;
        }

        return true;

    }

    function ConfirmDialog(message, redireccionSI, redireccionNO) {
        $('<div></div>').appendTo('body')
            .html('<div><h6>' + message + '</h6></div>')
            .dialog({
                modal: true, title: 'Confirmación', zIndex: 10000, autoOpen: true,
                width: 'auto', resizable: false,
                buttons: {
                    Si: function () {
                        if (redireccionSI != null)
                            window.location = redireccionSI;
                        $(this).dialog("close");

                    },
                    No: function () {
                        if (redireccionNO != null)
                            window.location = redireccionNO;
                        $(this).dialog("close");
                    }
                },
                close: function (event, ui) {
                    $(this).remove();
                }
            });
        document.body.scrollTop = default_scrollTop;
    };

    function editarPermiso() {
        if (!validacionDatosPermiso())
            return false;

        $('body').loadingModal({
            text: 'Editando Permiso...'
        });
        $.ajax({
            url: "/Permiso/Update",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar el permiso.',
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });
            },
            success: function (resultado) {
                $('body').loadingModal('hide');

                $.alert({
                    title: TITLE_EXITO,
                    content: 'El permiso se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Permiso/List';
                        }
                    }
                });
            }
        });
    }


    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Permiso/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

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

