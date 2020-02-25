jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        $("#btnBusqueda").click();
        cargarChosenUsuarioList();
        cargarChosenVendedorList();
        verificarSiExisteVendedor();
        verificarSiEsSupervisor();
    });

    function verificarSiEsSupervisor() {

        if ($("#supervisor_vendedor").val() == 1)
        {
            $("#idVendedorBusquedaList").prop('disabled', true).trigger("chosen:updated"); 
        }       

    }

    function verificarSiExisteVendedor() {
        if ($("#idVendedor").val().trim() != "0") {
            $("#idUsuarioBusquedaList").prop('disabled', true).trigger("chosen:updated");
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
                $("#tableVendedores").footable
                    ({
                        "paging": {
                            "enabled": true
                        }
                    });

                for (var i = 0; i < list.length; i++) {
                    var act = list[i].estado == 1 ? "Activo" : "Inactivo" ;  
                    list[i].cargo = list[i].cargo == null ? "" : list[i].cargo;

                    var ItemRow = '<tr data-expanded="true">' +

                        '<td>  ' + list[i].idVendedor + '  </td>' +
                        '<td>  ' + list[i].descripcion + '  </td>' +
                        '<td>  ' + list[i].codigo + '  </td>' +
                        '<td>  ' + list[i].cargo + '  </td>' +
                        '<td>  ' + act + '  </td>' +
                        '<td>  ' + list[i].email + '  </td>' +
                        '<td>  ' + list[i].ciudad.nombre + '  </td>' +
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

    $("#btnLimpiarBusquedaVendedor").click(function () {
        $.ajax({
            url: "/Vendedor/CleanBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
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

        if (!validacionDatosVendedor())
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

        if ($("#vendedor_idCiudad").val() === "") {
            $.alert({
                title: "Ciudad Inválida",
                type: 'orange',
                content: 'Debe ingresar una Sede para el vendedor.',
                buttons: {
                    OK: function () { $('#vendedor_idCiudad').focus(); }
                }
            });
            return false;
        }
      
        if ($("#idUsuarioBusquedaList").val() === "") {
            $.alert({
                title: "Usuario Incorrecto",
                type: 'orange',
                content: 'Debe ingresar un Usuario.',
                buttons: {
                    OK: function () { $('#idUsuarioBusquedaList').focus(); }
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

        if ($("#idVendedorBusquedaList").val() == undefined || $("#idVendedorBusquedaList").val() == "")
        {
            $("#idVendedorBusquedaList").val("");
        }             
     
        if ($("#supervisor_vendedor").prop('checked') == false)
        {
            if ($("#responsable_comercial_vendedor").prop('checked') && $("#idVendedorBusquedaList").val() == "" || $("#atencion_cliente_vendedor").prop('checked') && $("#idVendedorBusquedaList").val() == "") 
            {
            $.alert({
                title: "Supervisor Inválido",
                type: 'orange',
                content: 'Debe asignar un Supervisor para el usuario.',
                buttons: {
                    OK: function () { $('#idVendedorBusquedaList').focus(); }
                }
            });
            return false;
            }            
        }

        if ($("#responsable_comercial_vendedor").prop('checked') == false && $("#supervisor_vendedor").prop('checked') == false && $("#atencion_cliente_vendedor").prop('checked') == false)
        {
            $.alert({
                title: "Tipo de Vendedor Inválido",
                type: 'orange',
                content: 'Debe elegir al menos un tipo de Vendedor.',
                buttons: {
                    OK: function () { $('#responsable_comercial_vendedor').focus(); }
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
   
    

    $("#vendedor_contacto").change(function () {
        changeInputString("contacto", $("#vendedor_contacto").val());
    });

    $("#vendedor_cargo").change(function () {
        changeInputString("cargo", $("#vendedor_cargo").val());
    });

    $("#vendedor_maxdesapro").change(function () {
        changeInputDecimal("maxdesapro", $("#vendedor_maxdesapro").val());
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


    function cargarChosenUsuarioList() {

        $("#idUsuarioBusquedaList").chosen({ placeholder_text_single: "Buscar Usuario", no_results_text: "No se encontró Usuario", allow_single_deselect: true }).on('chosen:showing_dropdown');


        $("#idUsuarioBusquedaList").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 1,
            afterTypeDelay: 300,
            cache: false,
            url: "/Vendedor/SearchUsuario"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Usuario", no_results_text: "No se encontró Usuario" });

    }

    function cargarChosenVendedorList() {

        $("#idVendedorBusquedaList").chosen({ placeholder_text_single: "Buscar Supervisor", no_results_text: "No se encontró Supervisor", allow_single_deselect: true }).on('chosen:showing_dropdown');


        $("#idVendedorBusquedaList").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 1,
            afterTypeDelay: 300,
            cache: false,
            url: "/Vendedor/SearchVendedor"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Supervisor", no_results_text: "No se encontró Supervisor" });
    }

    $('body').on('change', "#supervisor_vendedor", function () {       
        $("#idVendedorBusquedaList").prop('disabled', true).trigger("chosen:updated"); 
        $('#idVendedorBusquedaList').val('').trigger("chosen:updated");
        var a = $("#supervisor_vendedor").prop('checked');
        changeInputBoolean("esSupervisorComercial", a);        
        if ($("#supervisor_vendedor").prop('checked') == false)
        {
            $("#idVendedorBusquedaList").removeAttr('disabled').trigger("chosen:updated");
        }
    });

    $('body').on('change', "#atencion_cliente_vendedor", function () {
        var a = $("#atencion_cliente_vendedor").prop('checked');
        changeInputBoolean("esAsistenteServicioCliente", a);        
        if ($("#supervisor_vendedor").prop('checked') == false)
        {
            $("#idVendedorBusquedaList").removeAttr('disabled').trigger("chosen:updated");
        }
           
       
    });

    $('body').on('change', "#responsable_comercial_vendedor", function () {  
        var a = $('#responsable_comercial_vendedor').prop('checked');
        changeInputBoolean("esResponsableComercial", a);
        if ($("#supervisor_vendedor").prop('checked') == false)
        {
            $("#idVendedorBusquedaList").removeAttr('disabled').trigger("chosen:updated");
        }
        
    });


    function changeInputBoolean(propiedad, valor) {
        $.ajax({
            url: "/Vendedor/ChangeInputBoolean",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    var ciudadOriginal;
    $("#idUsuarioBusquedaList").change(function () {
        //  $("#contacto").val("");
        var idVendedor = $("#idUsuarioBusquedaList").val();
        idVendedor = idVendedor == "" ? "00000000-0000-0000-0000-000000000000" : idVendedor;

        $.ajax({
            url: "/Vendedor/GetUsuario",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idVendedor: idVendedor
            },
            success: function (usuario) {                                               
                $("#vendedor_idCiudad").val(usuario.sedeMP.idCiudad); 
                ciudadOriginal = usuario.sedeMP.idCiudad;
                $("#vendedor_cargo").val(usuario.cargo);
                $("#vendedor_descripcion").val(usuario.nombre);
                $("#vendedor_contacto").val(usuario.contacto);
                $("#vendedor_maxdesapro").val(usuario.maximoPorcentajeDescuentoAprobacion);  
            }
        });
        
    });

    $("#vendedor_idCiudad").change(function () {        
        var idCiudad = $("#vendedor_idCiudad").val();
        var textCiudad = $("#vendedor_idCiudad option:selected").text();
        if (ciudadOriginal != idCiudad && ciudadOriginal != undefined)
        {
            $.alert({
                title: "Cambio de sede a Usuario",
                type: 'orange',
                content: 'Si cambia la sede tambien cambiara la sede del usuario asignado.',
                buttons: {
                    OK: function () { $('#responsable_comercial_vendedor').focus(); }
                }
            });
        }

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



    $("#idVendedorBusquedaList").change(function () {
        //  $("#contacto").val("");
        var idUsuarioSupervisor = $(this).val();

        if (idUsuarioSupervisor == "") {
            idUsuarioSupervisor = "00000000-0000-0000-0000-000000000000";
        }

        $.ajax({
            url: "/Vendedor/GetVendedorSupervisor",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idUsuarioSupervisor: idUsuarioSupervisor
            },
            success: function (cliente) {
            }
        });

    });

});
