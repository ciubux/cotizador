jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        $("#btnBusqueda").click();
        verificarSiExisteVendedor();
    });

    function verificarSiExisteVendedor() {
        if ($("#idVendedor").val().trim() != "0") {
            $("#btnFinalizarEdicionVendedor").html('Finalizar Edición');

        }
        else {
            $("#btnFinalizarEdicionVendedor").html('Finalizar Creación');
        }

    }


    $("#btnBusqueda").click(function () {

        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/Vendedor/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableVendedores > tbody").empty();
                $("#tableVendedores").footableita
                ({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < list.length; i++) {
                    var act;
                    if (list[i].estado == 1) { act = "Activo" };
                    if (list[i].estado == 0) { act = "Inactivo" };
                    var ItemRow = '<tr data-expanded="true">' +

                        '<td>  ' + list[i].idVendedor + '  </td>' +
                        '<td>  ' + list[i].descripcion + '  </td>' +
                        '<td>  ' + list[i].codigo + '  </td>' +
                        '<td>  ' + list[i].cargo + '  </td>' +
                        '<td>  ' + act + '  </td>' +
                        '<td>  ' + list[i].email + '  </td>' +
                        '<td>' +
                        '<button type="button" class="' + list[i].idVendedor + ' btnEditarVendedor btn btn-primary ">Editar</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tableVendedores").append(ItemRow);

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

    $("#vendedor_estado_si").click(function () {
        var valCheck = 1;
        changeInputInt("estado", valCheck)
    });

    $("#vendedor_estado_no").click(function () {
        var valCheck = 0;
        changeInputInt("estado", valCheck)
    });

    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/Vendedor/ChangeInputInt",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }



    $(document).on('click', "button.btnEditarVendedor", function () {
        //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idVendedor = arrrayClass[0];

        $.ajax({
            url: "/Vendedor/ConsultarSiExisteVendedor",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idVendedor: idVendedor
            },
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/Vendedor/iniciarEdicionVendedor",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del Vendedor."); },
                        success: function (fileName) {
                            window.location = '/Vendedor/Editar';
                        }
                    });

                }
                else {
                    if (resultado.idVendedor == 0) {
                        alert('Está creando un nuevo vendedor; para continuar por favor diríjase a la página "Crear/Modificar Vendedor" y luego haga clic en el botón Cancelar.');
                    }

                    else {
                        alert('Ya se encuentra editando un vendedor para continuar por favor dirigase a la página "Crear/Modificar Vendedor".');
                    }
                }
            }
        });


    });


    $("#btnCancelarVendedor").click(function () {

        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/Vendedor/CancelarCreacionVendedor', null);
    });








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




    function crearVendedor() {
        if (!validacionDatosVendedor())
            return false;

        $('body').loadingModal({
            text: 'Creando Vendedor...'
        });
        $.ajax({
            url: "/Vendedor/Create",
            type: 'POST',
            dataType: 'JSON',

            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear el vendedor.',
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
                    content: 'El vendedor se creó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Vendedor/Lista';
                        }
                    }
                });
            }
        });

    }

    $("#btnFinalizarEdicionVendedor").click(function () {
        /*Si no tiene codigo el cliente se está creando*/
        if ($("#idVendedor").val() == '0') {
            crearVendedor();
        }
        else {
            editarVendedor();
        }
    });



    function editarVendedor() {

        if (!validacionDatosVendedor2())
            return false;


        $('body').loadingModal({
            text: 'Editando Vendedor...'
        });
        $.ajax({
            url: "/Vendedor/Update",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar el vendedor.',
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
                    content: 'El vendedor se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Vendedor/Lista';
                        }
                    }
                });
            }
        });
    }


    function validacionDatosVendedor() {

        if ($("#vendedor_codigo").val().indexOf(" ") !== -1 || $("#vendedor_codigo").val().length == 1 || $("#vendedor_codigo").val().length > 2 || $("#vendedor_codigo").val().length == "") {
            $.alert({
                title: "Código Inválido",
                type: 'orange',
                content: 'Debe ingresar un Código de Vendedor válido.',
                buttons: {
                    OK: function () { $('#vendedor_codigo').focus(); }
                }
            });
            return false;
        }

        if ($("#vendedor_descripcion").val().length < 4) {
            $.alert({
                title: "Nombre Inválido",
                type: 'orange',
                content: 'Debe ingresar un nombre válido.',
                buttons: {
                    OK: function () { $('#vendedor_descripcion').focus(); }
                }
            });
            return false;
        }
        if ($("#vendedor_pass").val().length < 5) {
            $.alert({
                title: "Contraseña Inválida",
                type: 'orange',
                content: 'Debe ingresar una contraseña válida.',
                buttons: {
                    OK: function () { $('#vendedor_pass').focus(); }
                }
            });
            return false;
        }

        if ($("#vendedor_idCiudad").val() === "") {
            $.alert({
                title: "Ciudad Inválida",
                type: 'orange',
                content: 'Debe ingresar una Sede para el usuario.',
                buttons: {
                    OK: function () { $('#vendedor_idCiudad').focus(); }
                }
            });
            return false;
        }
        var reg = /^[0-9]\d*(\.\d+)?$/;
        var var2 = $("#vendedor_maxdesapro").val();

        if (reg.test(var2) == false || $("#vendedor_maxdesapro").val() > 100) {
            $.alert({
                title: "Maximo descuento aprobado Inválido",
                type: 'orange',
                content: 'Debe ingresar un numero correcto.',
                buttons: {
                    OK: function () { $('#vendedor_maxdesapro').focus(); }
                }
            });
            return false;
        }

        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        var var1 = $("#vendedor_email").val();

        if (regex.test(var1) !== true || $("#vendedor_email").val().indexOf(" ") !== -1 || $("#vendedor_email").val().indexOf("@") == -1 || $("#vendedor_email").val().length < 12) {
            $.alert({
                title: "Email Inválido",
                type: 'orange',
                content: 'Debe ingresar un Email válido. Ejem:prueba@mpinstitucional.com',
                buttons: {
                    OK: function () { $('#vendedor_email').focus(); }
                }
            });
            return false;
        }

        return true;

    }

    function validacionDatosVendedor2() {

        if ($("#vendedor_codigo").val().indexOf(" ") !== -1 || $("#vendedor_codigo").val().length == 1 || $("#vendedor_codigo").val().length > 2 || $("#vendedor_codigo").val().length == "") {
            $.alert({
                title: "Código Inválido",
                type: 'orange',
                content: 'Debe ingresar un Código de Vendedor válido.',
                buttons: {
                    OK: function () { $('#vendedor_codigo').focus(); }
                }
            });
            return false;
        }

        if ($("#vendedor_descripcion").val().length < 4) {
            $.alert({
                title: "Nombre Inválido",
                type: 'orange',
                content: 'Debe ingresar un nombre válido.',
                buttons: {
                    OK: function () { $('#vendedor_descripcion').focus(); }
                }
            });
            return false;
        }

        if ($("#vendedor_idCiudad").val() === "") {
            $.alert({
                title: "Ciudad Inválida",
                type: 'orange',
                content: 'Debe ingresar una sede para el usuario.',
                buttons: {
                    OK: function () { $('#vendedor_idCiudad').focus(); }
                }
            });
            return false;
        }


        var reg = /^[0-9]\d*(\.\d+)?$/;
        var var2 = $("#vendedor_maxdesapro").val()
        if (reg.test(var2) == false || $("#vendedor_maxdesapro").val() > 100) {
            $.alert({
                title: "Maximo descuento aprobado Inválido",
                type: 'orange',
                content: 'Debe ingresar un numero correcto.',
                buttons: {
                    OK: function () { $('#vendedor_maxdesapro').focus(); }
                }
            });
            return false;
        }

        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        var var1 = $("#vendedor_email").val();

        if (regex.test(var1) !== true || $("#vendedor_email").val().indexOf(" ") !== -1 || $("#vendedor_email").val().indexOf("@") == -1 || $("#vendedor_email").val().length < 12) {
            $.alert({
                title: "Email Inválido",
                type: 'orange',
                content: 'Debe ingresar un Email válido. Ejem:prueba@mpinstitucional.com',
                buttons: {
                    OK: function () { $('#vendedor_email').focus(); }
                }
            });
            return false;
        }


        return true;

    }

    $("#vendedor_idVendedor").change(function () {
        ChangeInputInt("idVendedor", $("#vendedor_idVendedor").val());
    });

    $("#vendedor_descripcion").change(function () {
        changeInputString("descripcion", $("#vendedor_descripcion").val());
    });

    $("#vendedor_codigo").change(function () {
        changeInputString("codigo", $("#vendedor_codigo").val());
    });
    $("#vendedor_email").change(function () {

        changeInputString("email", $("#vendedor_email").val());
    });


    $("#vendedor_pass").change(function () {
        changeInputString("pass", $("#vendedor_pass").val());
    });

    $("#vendedor_contacto").change(function () {
        changeInputString("contacto", $("#vendedor_contacto").val());
    });

    $("#vendedor_cargo").change(function () {
        changeInputString("cargo", $("#vendedor_cargo").val());
    });

    $("#vendedor_maxdesapro").change(function () {
        changeInputDecimal("maxdesapro", $("#vendedor_maxdesapro").val());
    });


    $("#vendedor_idCiudad").change(function () {
        var idCiudad = $("#vendedor_idCiudad").val();
        var textCiudad = $("#vendedor_idCiudad option:selected").text();
        $.ajax({
            url: "/Vendedor/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad
            },
            error: function (detalle) {
                location.reload();
            },
            success: function (ciudad) {
            }
        });
    });



    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Vendedor/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function changeInputDecimal(propiedad, valor) {
        $.ajax({
            url: "/Vendedor/ChangeInputDecimal",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }




});