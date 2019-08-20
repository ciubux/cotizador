jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    $.datepicker.regional['es'] = {
        closeText: 'Cerrar',
        prevText: '< Ant',
        nextText: 'Sig >',
        currentText: 'Hoy',
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
        dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
        dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
        dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };


    $.datepicker.setDefaults($.datepicker.regional["es"]);

    $("#fechaHastaMensaje").datepicker({
        dateFormat: 'dd/mm/yy',
        firstDay: 1
    }).datepicker("setDate", new Date());



    var hoy = $("#fechaHastaMensaje").val();

    $("#btnEnviarMensaje").click(function () {

        crearMensaje();

    });



    ActulizarMensaje();
    function validacionMensaje() {

        if ($("input:checkbox:checked").length == 0) {
            $.alert({
                title: "Rol Inválido",
                type: 'orange',
                content: 'Debe ingresar al menos un Rol para enviar el mensaje.',
                buttons: {
                    OK: function () { $('input:checkbox:checked').focus(); }
                }
            });
            return false;
        }

        if ($("#importancia").val() == "") {
            $.alert({
                title: "Importancia Inválida",
                type: 'orange',
                content: 'Debe ingresar una Importacia válida.',
                buttons: {
                    OK: function () { $('#importancia').focus(); }
                }
            });
            return false;
        }

        if ($("#titulo").val() == "" || $("#titulo").val() == null) {
            $.alert({
                title: "Titulo Inválida",
                type: 'orange',
                content: 'Debe ingresar un titulo válido.',
                buttons: {
                    OK: function () { $('#titulo').focus(); }
                }
            });
            return false;
        }

        if ($("#txtMensaje").val() == "" || $("#txtMensaje").val() == null) {
            $.alert({
                title: "Mensaje Inválido",
                type: 'orange',
                content: 'Debe ingresar un mensaje válido.',
                buttons: {
                    OK: function () { $('#txtMensaje').focus(); }
                }
            });
            return false;
        }


        if (new Date($("#fechaHastaMensaje").val()).getTime() < new Date(hoy).getTime() || $("#fechaHastaMensaje").val() == null) {
            $.alert({
                title: "Fecha Inválida",
                type: 'orange',
                content: 'Debe ingresar un posterior a la de hoy.',
                buttons: {
                    OK: function () { $('#fechaHastaMensaje').focus(); }
                }
            });
            return false;
        }

        return true;

    }


    $("#txtMensaje").change(function () {

        changeInputString("mensaje", $("#txtMensaje").val());
    });

    $("#titulo").change(function () {
        changeInputString("titulo", $("#titulo").val());
    });

    $("#importancia").change(function () {
        changeInputString("importancia", $("#importancia").val());
    });




    function crearMensaje() {

        if (!validacionMensaje())
            return false;

        $('body').loadingModal({
            text: 'Creando Mensaje...'
        });
        $.ajax({
            url: "/Mensaje/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                hoy: hoy
            },
            error: function (detalle) {

                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear el mensaje.',
                    type: 'red',
                    buttons: {
                        OK: function () {

                            window.location = '/Mensaje/Crear';
                        }
                    }
                });
            },
            success: function (resultado) {

                $.alert({
                    title: TITLE_EXITO,
                    content: 'El mensaje se creó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Mensaje/Crear';
                        }
                    }
                });
            }
        });

    }

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Mensaje/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/Mensaje/ChangeInputInt",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#btnLimpiarBusqueda").click(function () {
        $.confirm({
            title: 'Confirmación de cambio',
            content: '¿Está seguro de borrar el mensaje?',
            type: 'orange',
            buttons: {
                confirm: {
                    text: 'Sí',
                    action: function () {
                        $.ajax({
                            url: "/Mensaje/Limpiar",
                            type: 'POST',
                            success: function () {
                                location.reload();
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

    $(".chk-rol").change(function () {
        var valor = 0;
        if ($(this).prop("checked")) {
            valor = 1;
        }
        changeInputPermiso($(this).attr("id"), valor);
    });

    function changeInputPermiso(rol, valor) {
        $.ajax({
            url: "/Mensaje/ChangeRol",
            type: 'POST',
            data: {
                rol: rol,
                valor: valor
            },
            success: function () { }
        });
    }


    $(".navbar-header").on("click", "a.btnModal", function () {
        $('#Mensaje').modal('show');
    });

<<<<<<< HEAD
    function eliminateDuplicates(arrayIn) {
=======
 function eliminateDuplicates(arrayIn) {
>>>>>>> dc2b83dbb12f30b2156232b3ebfacc8e112bad52
        var arrayOut = {};
        var unicos = arrayIn.filter(function (e) {
            return arrayOut[e.id_mensaje] ? false : (arrayOut[e.id_mensaje] = true);
        });
        return unicos;
    }

    var idUsuario;
    function ActulizarMensaje() {

        idUsuario = $('#usuario_idUsuario').val();

        if (idUsuario != "00000000-0000-0000-0000-000000000000") {

            $.ajax({
                url: "/Mensaje/ShowMensaje",
                type: 'POST',
                dataType: 'JSON',
                data: {
                    idUsuario: idUsuario
                },
                success: function (list) {

                    if (list.length != 0) {
<<<<<<< HEAD

                        list=eliminateDuplicates(list);

=======
                        
                        list=eliminateDuplicates(list);
                        
>>>>>>> dc2b83dbb12f30b2156232b3ebfacc8e112bad52
                        $("#imagenMP").before('<a data-notifications="' + list.length + '" class="btnModal" href="javascript:void()"></a>');




                        for (var i = 0; i < list.length; i++) {  

                            var ItemRow =


                                '<div class="modal-content" id="' + list[i].id_mensaje + '">' +
                                '<div class="modal-header">' +

                                '<h4>' + list[i].titulo + '</h4>' +
                                '<h5> Importancia:' + list[i].importancia + ' | Usuario:' + list[i].user.nombre + ' | Fecha de creación:' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fecha_creacion_mensaje)) + ' | Fecha de vencimiento:' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaVencimiento)) + '</h5>' +
                                '</div>' +
                                '<div class="modal-body">' +
                                '<p>' + list[i].mensaje + '<p>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                '<a  class="Leido btn btn-default ' + list[i].id_mensaje + '">Leido</a>' +
                                '</div>' +
                                '</div>';



                            $("#Mensaje").append(ItemRow);
                        }

                    } else {
                        $(".btnModal").remove();
                    }
                }
            });


        }

    }



    $("body").on("click", "a.Leido", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idMensaje = arrrayClass[3];


        $.ajax({
            url: "/Mensaje/UpdateMensaje",
            type: 'POST',
            data: {
                idMensaje: idMensaje,
                idUsuario: idUsuario
            },
            success: function () {

                /*$("#Mensaje").removeData('bs.modal');*/
                $("#Mensaje").find('.modal-content').empty();
                $('#Mensaje').modal('hide');

                ActulizarMensaje();

            }
        });


    });




    $("#fechaHastaMensaje").change(function () {

        var fechaVencimiento = $(this).val();
        $.ajax({
            url: "/Mensaje/changeFecha",
            type: 'POST',
            data: {
                fechaVencimiento: fechaVencimiento
            },
            success: function () { }
        });
    });



});
