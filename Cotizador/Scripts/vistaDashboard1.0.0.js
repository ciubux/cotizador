jQuery(function ($) {
    var pagina = 28;
    var VISTA_DASHBOARD_CANCELAR_EDICION = '¿Está seguro de cancelar la edición del mensaje; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';


    $('body').ready(function () {
        $("#btnVistaDashboardBusqueda").click();  
       verificarSiExisteVistaDashboard();
    });

    $("#btnLimpiarVistaDashboardBusqueda").click(function () {
     
    });

    $("#vistaDashboard_codigo").change(function () {
        changeInputString("codigo", $("#vistaDashboard_codigo").val());
    });

    $("#vistaDashboard_nombre").change(function () {
        changeInputString("nombre", $("#vistaDashboard_nombre").val());
    });

    $("#vistaDashboard_descripcion").change(function () {
        changeInputString("descripcion", $("#vistaDashboard_descripcion").val());
    });

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/VistaDashboard/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }    
   

    $("#btnVistaDashboardBusqueda").click(function () {

        $("#btnVistaDashboardBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/VistaDashboard/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnVistaDashboardBusqueda").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnVistaDashboardBusqueda").removeAttr("disabled");
                $("#tableVistaDashboard > tbody").empty();
                $("#tableVistaDashboard").footable({
                    "paging": {
                        "enabled": true
                    }
                });


                for (var i = 0; i < list.length; i++) {

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list[i].idVistaDashboard + '  </td>' +
                        '<td>  ' + list[i].codigo + '  </td>' +                      
                        '<td>  ' + list[i].nombre + '  </td>' +                       
                        '<td>' +
                        '<button type="button" style="margin-right:13px;" class="btnEditarVistaDashboard btn btn-primary" idVistaDashboard="' + list[i].idVistaDashboard + '" idTipoVistaDashboard="' + list[i].idTipoVistaDashboard+'"> Editar</button >' +
                        '</td>' +
                        '</tr>';

                    $("#tableVistaDashboard").append(ItemRow);

                }
            }
        });
    });

    $('body').on('click', "button.btnEditarVistaDashboard", function () {

        var idVistaDashboard = $(this).attr('idVistaDashboard');
        var idTipoVistaDashboard = $(this).attr('idTipoVistaDashboard');

        $.ajax({
            url: "/VistaDashboard/ConsultarSiExisteVistaDashboard",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idVistaDashboard: idVistaDashboard
            },
            success: function (resultado) {

                if (resultado.existe == "false") {
                    $.ajax({
                        url: "/VistaDashboard/iniciarEdicionVistaDashboard",
                        type: 'POST',
                        async: false,
                        error: function (detalle) {
                            alert("Ocurrió un problema al iniciar la edición de la vista Dashboard.");
                        },
                        success: function (fileName) {
                            window.location = '/VistaDashboard/Editar';                            
                        }
                    });
                }
                else {
                    if (resultado.idVendedor == 0) {
                        alert('Está creando una nueva vista de Dashboard; para continuar por favor diríjase a la página "Crear/Modificar Vista Dashboard" y luego haga clic en el botón Cancelar.');
                    }

                    else {
                        alert('Ya se encuentra editando una vista de Dashboard para continuar por favor dirigase a la página "Crear/Modificar Vista Dashboard".');
                    }
                }
            }
        });
    });

    $("#btnCrearVistaDashboard").click(function () {
        if ($("#idVistaDashboard").val() == 0 || $("#idVistaDashboard").val()==null) {
            crearVistaDashboard();
        }
        else {
            editarVistaDashboard();
        }

    });


    $("#btnCancelarVistaDashboard").click(function () {
        ConfirmDialog(VISTA_DASHBOARD_CANCELAR_EDICION, '/VistaDashboard/CancelarCreacionDashboard', null);
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
    }

    function verificarSiExisteVistaDashboard() {

        if ($("#idVistaDashboard").val() != 0) {
            $("#btnCrearVistaDashboard").html('Finalizar Edición');
        }
        else {
            $("#btnCrearVistaDashboard").html('Finalizar Creación');
        }

    }


    function crearVistaDashboard() {

        if (!validacionVistaDashboard())
            return false;
            
        $('body').loadingModal({
            text: 'Creando Vista de Dashboard...'
        });
        $.ajax({
            url: "/VistaDashboard/Create",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {

                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear la vista del  Dashboard.',
                    type: 'red',
                    buttons: {
                        OK: function () {

                            window.location = '/VistaDashboard/Editar';
                        }
                    }
                });
            },
            success: function (resultado) {

                $.alert({
                    title: TITLE_EXITO,
                    content: 'La vista de Dashboard se creó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/VistaDashboard/Lista';
                        }
                    }
                });
            }
        });

    }

    function editarVistaDashboard() {

        if (!validacionVistaDashboard())
            return false;
           

        $('body').loadingModal({
            text: 'Editando vista del  Dashboard...'
        });
        $.ajax({
            url: "/VistaDashboard/Update",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar la vista del Dashboard.',
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
                    content: 'El Dashboard se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/VistaDashboard/Lista';
                        }
                    }
                });
            }
        });
    }


    $("#vistaDashboard_bloquesAncho").change(function () {      
        changeInputInt("bloquesAncho", $("#vistaDashboard_bloquesAncho").val());       
    });
    $("#vistaDashboard_altoPx").change(function () {
        changeInputInt("altoPx", $("#vistaDashboard_altoPx").val());
    });
    $("#idTipoVistaDashboard").change(function () {
        changeInputInt("idTipoVistaDashboard",$("#idTipoVistaDashboard").val());
    });  


    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/VistaDashboard/ChangeInputInt",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }
    
    function validacionVistaDashboard()
    {
        if ($("#vistaDashboard_altoPx").val() == "" || $("#vistaDashboard_bloquesAncho").val() == "")
        {
            $.alert({
                title: "Ancho o Alto Inválido",
                type: 'orange',
                content: 'Debe ingresar minimo 0 en el alto y ancho.',
                buttons: {
                    OK: function () { $('#vistaDashboard_bloquesAncho').focus(); }
                }
            });
           return false;
        }
        
        if ($("#idTipoVistaDashboard").val() == "")
        {
            $.alert({
                title: "Tipo Vista Dashboard Inválido",
                type: 'orange',
                content: 'Debe seleccionar un tipo de vista Dashboard valido.',
                buttons: {
                    OK: function () { $('#idTipoVistaDashboard').focus(); }
                }
            });
           return false;
        }
      
        if ($("#vistaDashboard_codigo").val() == "") {
            $.alert({
                title: "Código Inválido",
                type: 'orange',
                content: 'Debe ingresar un código valido.',
                buttons: {
                    OK: function () { $('#vistaDashboard_codigo').focus(); }
                }
            });
            return false;
        }

        return true;
    }


    

});