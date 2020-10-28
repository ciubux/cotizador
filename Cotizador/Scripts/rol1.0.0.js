
jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        $("#btnBusqueda").click();
        //cargarChosenCliente();
        verificarSiExisteCliente();

        cargarChosenUsuario();
        FooTable.init('#tableUsuariosRol');

        $('.hasTooltipPermiso').tipso(
            {
                titleContent: 'DESCRIPCIÓN',
                titleBackground: '#428bca',
                titleColor: '#ffffff',
                background: '#ffffff',
                color: '#686868',
                width: 400
            });
    });

    function verificarSiExisteCliente() {
        if ($("#idRol").val().trim() != "0") {
            $("#btnFinalizarEdicionRol").html('Finalizar Edición');
        }
        else {
            $("#btnFinalizarEdicionRol").html('Finalizar Creación');
        }

    }

    function limpiarFormulario() {
        $("#rol_codigo").val("");
        $("#rol_nombre").val("");
    }




    function ConfirmDialogReload(message) {
        $('<div></div>').appendTo('body')
            .html('<div><h6>' + message + '</h6></div>')
            .dialog({
                modal: true, title: 'Confirmación', zIndex: 10000, autoOpen: true,
                width: 'auto', resizable: false,
                buttons: {
                    Si: function () {
                        location.reload();
                        $(this).dialog("close");
                    },
                    No: function () {
                        $(this).dialog("close");
                    }
                },
                close: function (event, ui) {
                    $(this).remove();
                }
            });
        document.body.scrollTop = default_scrollTop;
    };

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


    function validacionDatosRol() {
       
        if ($("#rol_codigo").val().length < 2) {
            $.alert({
                title: "Código Inválido",
                type: 'orange',
                content: 'Debe ingresar un Código de rol válido.',
                buttons: {
                    OK: function () { $('#rol_codigo').focus(); }
                }
            });
            return false;
        }
        
        if ($("#rol_nombre").val().length < 4) {
            $.alert({
                title: "Nombre Inválido",
                type: 'orange',
                content: 'Debe ingresar un nombre válido.',
                buttons: {
                    OK: function () { $('#rol_nombre').focus(); }
                }
            });
            return false;
        }

        return true;

    }


    $("#btnFinalizarEdicionRol").click(function () {
        /*Si no tiene codigo el cliente se está creando*/
        if ($("#rol_idRol").val() == '0') {
            crearRol();
        }
        else {
            editarRol();
        }
    });



    function crearRol() {
        if (!validacionDatosRol())
            return false;

        $('body').loadingModal({
            text: 'Creando Rol...'
        });
        $.ajax({
            url: "/Rol/Create",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear el rol.',
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
                    content: 'El rol se creó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Rol/List';
                        }
                    }
                });
            }
        });

    }

    function editarRol() {

        if (!validacionDatosRol())
            return false;


        $('body').loadingModal({
            text: 'Editando Rol...'
        });
        $.ajax({
            url: "/Rol/Update",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar el rol.',
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
                    content: 'El rol se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Rol/List';
                        }
                    }
                });
            }
        });
    }

    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/Rol/ChangeInputInt",
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
            url: "/Rol/ChangeInputDecimal",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#rol_estado_si").click(function () {
        var valCheck = 1;
        changeInputInt("Estado", valCheck)
    });

    $("#rol_estado_no").click(function () {
        var valCheck = 0;
        changeInputInt("Estado", valCheck)
    });

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Rol/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    function changeInputPermiso(permiso, valor) {
        $.ajax({
            url: "/Rol/ChangePermiso",
            type: 'POST',
            data: {
                permiso: permiso,
                valor: valor
            },
            success: function () { }
        });
    }
    


    $("#rol_codigo").change(function () {
        changeInputString("codigo", $("#rol_codigo").val());
    });

    $("#rol_nombre").change(function () {
        changeInputString("nombre", $("#rol_nombre").val());
    });

    $(".chk-permiso").change(function () {
        var valor = 0;
        if ($(this).prop("checked")) {
            valor = 1;
        }
        changeInputPermiso($(this).attr("id"), valor);
    });


    $("#btnCancelarRol").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/Rol/CancelarCreacionRol', null)
    })

    $("body").on('click', ".btnVerUsuariosRol", function () {
        var idRol = $(this).attr("idRol");
        window.location = '/Rol/Usuarios?idRol=' + idRol;
    });

    $("#btnAgregarUsuarioRol").click(function () {
        //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona

        var idUsuario = $("#idUsuario").val();
        var idRol = $("#idRol").val();
        
        if (idUsuario == "") {
            $.alert({
                title: "Ocurrió un error",
                type: 'red',
                content: "Debe seleccionar un usuario",
                buttons: {
                    OK: function () { }
                }
            });

            return;
        }

        $('body').loadingModal({
            text: 'Agregando Usuario'
        });

        $.ajax({
            url: "/Rol/AddUsuarioRol",
            type: 'POST',
            data: {
                idUsuario: idUsuario,
                idRol: idRol
            },
            type: 'POST',
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.success == 1) {
                    
                    var usuario = resultado.usuario;

                    var clienteRow = '<tr data-expanded="true">' +
                        '<td>  ' + usuario.idUsuario + '</td>' +
                        '<td>  ' + usuario.email + '  </td>' +
                        '<td>  ' + usuario.nombre + '  </td>' +
                        '<td><button type="button" class="btn btn-danger btnQuitarUsuarioRol" idUsuario="' + usuario.idUsuario + '">Remover</button></td>' +
                        '</tr>';

                    $("#tableUsuariosRol").append(clienteRow);

                    FooTable.init('#tableUsuariosRol');

                    $.alert({
                        title: "Operación exitosa",
                        type: 'green',
                        content: resultado.message,
                        buttons: {
                            OK: function () { }
                        }
                    });

                }
                else {
                    $.alert({
                        title: "Ocurrió un error",
                        type: 'red',
                        content: resultado.message,
                        buttons: {
                            OK: function () { }
                        }
                    });
                }
                $('body').loadingModal('hide');
            },
            error: function () {
                $('body').loadingModal('hide');
            }
        });
    });


    $("body").on('click', ".btnQuitarUsuarioRol", function () {
        //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona
        var that = this;
        var emailUsuario = $(this).closest("tr").find("td:nth-child(2)").html();
        var nomUsuario = $(this).closest("tr").find("td:nth-child(3)").html();
        $.confirm({
            title: 'Confirmar operación',
            content: 'Esta seguro que desea remover el usuario: ' + emailUsuario + ' - ' + nomUsuario,
            type: 'orange',
            buttons: {
                aplica: {
                    text: 'SI',
                    btnClass: 'btn-success',
                    action: function () {
                        var idUsuario = $(that).attr("idUsuario");
                        var idRol = $("#idRol").val();
                        $.ajax({
                            url: "/Rol/QuitarUsuarioRol",
                            type: 'POST',
                            data: {
                                idUsuario: idUsuario,
                                idRol: idRol
                            },
                            type: 'POST',
                            dataType: 'JSON',
                            success: function (resultado) {
                                if (resultado.success == 1) {

                                    $(that).closest("tr").remove();
                                    FooTable.init('#tableUsuariosRol');

                                    $.alert({
                                        title: "Operación exitosa",
                                        type: 'green',
                                        content: resultado.message,
                                        buttons: {
                                            OK: function () { }
                                        }
                                    });

                                }
                                else {
                                    $.alert({
                                        title: "Ocurrió un error",
                                        type: 'red',
                                        content: resultado.message,
                                        buttons: {
                                            OK: function () { }
                                        }
                                    });
                                }
                            }
                        });
                    }
                },
                noAplica: {
                    text: 'NO',
                    btnClass: 'btn-danger',
                    action: function () {
                    }
                }
            }
        });


    });


    function cargarChosenUsuario() {
        $("#idUsuario").chosen({ placeholder_text_single: "Buscar Usuario", no_results_text: "No se encontró Usuario" }).on('chosen:showing_dropdown', function (evt, params) {

        });

        $("#idUsuario").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/Usuario/SearchUsuarios"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Usuario", no_results_text: "No se encontró Usuario" });
    }



    var ft = null;
    
    /*Evento que se dispara cuando se hace clic en el boton EDITAR en la edición de la grilla*/
    $(document).on('click', "button.footable-show", function () {

        //Cambiar estilos a los botones
        $("button.footable-add").attr("class", "btn btn-default footable-add");
        $("button.footable-hide").attr("class", "btn btn-primary footable-hide");


        //Se deshabilitan controles que recargan la página o que interfieren con la edición del detalle
        $("#considerarCantidades").attr('disabled', 'disabled');
        $("input[name=igv]").attr('disabled', 'disabled');
        $("#flete").attr('disabled', 'disabled');
        $("#btnOpenAgregarRol").attr('disabled', 'disabled');

        var codigo = $("#numero").val();
        if (codigo == "") {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarCreacionPedido").attr('disabled', 'disabled');
            $("#btnCancelPedido").attr('disabled', 'disabled');
        }
        else {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarEdicionPedido").attr('disabled', 'disabled');
            $("#btnCancelPedido").attr('disabled', 'disabled');
        }


        $("input[name=mostrarcodproveedor]").attr('disabled', 'disabled');


        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
            console.log('Esto es un dispositivo móvil');
            return;
        }


        //llama a la función que cambia el estilo de display después de que la tabla se ha redibujado
        //Si se le llama antes el redibujo reemplazará lo definido
        window.setInterval(mostrarFlechasOrdenamiento, 600);

        /*Se agrega control input en columna cantidad*/
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var cantidad = value.innerText;
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detincantidad form-control' value='" + cantidad + "' type='number'/>";
        });
        

        //   @cotizacionDetalle.producto.idProducto detproductoObservacion"




        /*Se agrega control input en columna porcentaje descuento*/
        var $j_object1 = $("td.detporcentajedescuento");
        $.each($j_object1, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var porcentajedescuento = value.innerText;
            porcentajedescuento = porcentajedescuento.replace("%", "").trim();
            $(".detporcentajedescuentoMostrar." + arrId[0]).html("<div style='width: 150px' ><div style='float:left' ><input style='width: 100px' class='" + arrId[0] + " detinporcentajedescuento form-control' value='" + porcentajedescuento + "' type='number'/></div><div > <button type='button' class='" + arrId[0] + " btnCalcularDescuento btn btn-primary bouton-image monBouton' data-toggle='modal' data-target='#modalCalculadora' ></button ></div></div>");

        });


        var $j_objectFlete = $("td.detflete");
        $.each($j_objectFlete, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var flete = value.innerText;
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detinflete form-control' value='" + flete + "' type='number'/>";
        });



    });


    function changeInputBoolean(propiedad, valor) {
        $.ajax({
            url: "/Rol/ChangeInputBoolean",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    
    $("#btnExportExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
    });

    $("#btnBusqueda").click(function () {
        
        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/Rol/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableRoles > tbody").empty();
                $("#tableRoles").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < list.length; i++) {

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list[i].idRol + '</td>' +
                        '<td>  ' + list[i].codigo + '  </td>' +
                        '<td>  ' + list[i].nombre + '  </td>' +
                        '<td>' +
                        '<button type="button" class="' + list[i].idRol + ' btnEditarRol btn btn-primary ">Editar</button>' +
                        '&nbsp;&nbsp;&nbsp;<button type="button" idRol="' + list[i].idRol + '" class="btnVerUsuariosRol btn btn-secundary">Usuarios</button>' +
                        /*'&nbsp;&nbsp;&nbsp;<button type="button" class="btnVistas btn btn-success">Vista Dashboard</button>'+*/
                        '</td>' +
                        '</tr>';
                    
                    $("#tableRoles").append(ItemRow);

                }

                if (ItemRow.length > 0) {
                    $("#msgBusquedaSinResultados").hide();
                    $("#divExportButton").show();
                }
                else {
                    $("#msgBusquedaSinResultados").show();
                    $("#divExportButton").hide();
                }

            }
        });
    });


    $(document).on('click', "button.btnEditarRol", function () {
      //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idRol = arrrayClass[0];
        
        $.ajax({
            url: "/Rol/ConsultarSiExisteRol",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idRol: idRol
            },
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/Rol/iniciarEdicionRol",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del Rol."); },
                        success: function (fileName) {
                            window.location = '/Rol/Editar';
                        }
                    });

                }
                else {
                    if (resultado.idRol == 0) {
                        alert('Está creando un nuevo rol; para continuar por favor diríjase a la página "Crear/Modificar Rol" y luego haga clic en el botón Cancelar.');
                    }
                    
                    else {
                        alert('Ya se encuentra editando un rol para continuar por favor dirigase a la página "Crear/Modificar Rol".');
                    }
                }
            }
        });
    });



    $('body').on('click', "button.btnVistas", function () {
        var idRol = $(this).closest('td').find('.btnVerUsuariosRol').attr('idRol');  
        window.location = '/Rol/VistaDashboard?idRol=' + idRol; 
    });


    $(".VistaDashboardEditar").change(function () {
        var valor = 0;
        if ($(this).prop("checked")) {
            valor = 1;
        }
        changeInputVistaDashboard($(this).attr("id"), valor);
    });

    function changeInputVistaDashboard(rol, valor) {
        $.ajax({
            url: "/Rol/ChangeVistaDashboard",
            type: 'POST',
            data: {
                rol: rol,
                valor: valor
            },
            success: function () { }
        });
    }


    $('body').on('click', "button#btnCancelarVistaDashboard", function () {
        window.location = '/Rol/List';
    });

    $('body').on('click', "button#btnFinalizarEdicionVistaDashboard", function () {
        $.ajax({
            url: "/Rol/updateRolVistaDashboard",
            success: function (response)
            {
                $.alert({
                    title: TITLE_EXITO,
                    content: 'Se registraron los cambios correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Rol/List';
                        }
                    }
                });                
            }
        });
    });

});


