
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
    $.datepicker.setDefaults($.datepicker.regional['es']);

    $("#fechaEmisionDesde").datepicker();
    $("#fechaEmisionHasta").datepicker();

    $("#tableAjustesAlmacen").footable({
        "paging": {
            "enabled": true
        }
    });


    $(document).ready(function () {
        $("#btnBusqueda").click();
        //cargarChosenCliente();
        verificarSiExisteCliente();

        cargarChosenUsuario();
        $('#tableUsuariosRol').footable();
        //FooTable.init('#tableUsuariosRol');

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



    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/AjusteAlmacen/ChangeInputInt",
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
            url: "/AjusteAlmacen/ChangeInputDecimal",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/AjusteAlmacen/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function changeInputDate(propiedad, valor) {
        $.ajax({
            url: "/AjusteAlmacen/ChangeInputDate",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function changeInputBoolean(propiedad, valor) {
        $.ajax({
            url: "/AjusteAlmacen/ChangeInputBoolean",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#idCiudad").change(function () {
        var idCiudad = $("#idCiudad").val();

        $.ajax({
            url: "/AjusteAlmacen/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad
            },
            error: function (detalle) {
                alert('Debe eliminar los productos agregados antes de cambiar de Sede.');
                location.reload();
            },
            success: function (ciudad) {
            }
        });
    });

    $("#motivoAjuste").change(function () {
        $.ajax({
            url: "/AjusteAlmacen/ChangeIdMotivoAjuste",
            type: 'POST',
            data: {
                idMotivoAjuste: $(this).val()
            },
            success: function () { }
        });
    });

    $("#fechaEmisionDesde").click(function () {
        changeInputDate("fechaEmisionDesde", $(this).val());
    });

    $("#fechaEmisionHasta").click(function () {
        changeInputDate("fechaEmisionHasta", $(this).val());
    });


    
    $("body").on('click', ".btnVerAjusteAlmacen", function () {
        
    });



    

    $("#btnBusqueda").click(function () {

        $('body').loadingModal({
            text: 'Registrando Ajuste Almacén...'
        });
        $('body').loadingModal('show');

        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/AjusteAlmacen/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $('body').loadingModal('hide');
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (list) {
                $('body').loadingModal('hide');
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableAjustesAlmacen > tbody").empty();
                $("#tableAjustesAlmacen").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < list.length; i++) {

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list[i].ciudadOrigen.nombre + '</td>' +
                        '<td>  ' + list[i].fechaEmisionFormatoImpresion + '  </td>' +
                        '<td>  ' + list[i].FechaRegistroDesc + '  </td>' +
                        '<td>  ' + list[i].usuario.nombre + '  </td>' +
                        '<td>  ' + list[i].motivoAjuste.descripcion  + '  </td>' +
                        '<td>' +
                        '<button type="button" idAjuste="' + list[i].idMovimientoAlmacen + '" class="btnVerAjusteAlmacen btn btn-primary">Ver</button>' +
                        /*'&nbsp;&nbsp;&nbsp;<button type="button" idRol="' + list[i].idRol + '" class="btnVerUsuariosRol btn btn-secundary">Usuarios</button>' +*/
                        /*'&nbsp;&nbsp;&nbsp;<button type="button" class="btnVistas btn btn-success">Vista Dashboard</button>'+*/
                        '</td>' +
                        '</tr>';
                    
                    $("#tableAjustesAlmacen").append(ItemRow);

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


    $(document).on('click', "button.btnEditarAjusteAlmacen", function () {
      //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idRol = arrrayClass[0];
        
        $.ajax({
            url: "/AjusteAlmacen/ConsultarSiExisteAjusteAlmacen",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idRol: idRol
            },
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/AjusteAlmacen/iniciarEdicionAjusteAlmacen",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del Ajuste de Almacen."); },
                        success: function (fileName) {
                            window.location = '/AjusteAlmacen/Editar';
                        }
                    });

                }
                else {
                    if (resultado.idRol == 0) {
                        alert('Está creando un nuevo ajuste almacen; para continuar por favor diríjase a la página "Crear/Modificar Ajuste Almacen" y luego haga clic en el botón Cancelar.');
                    }
                    
                    else {
                        alert('Ya se encuentra editando un ajuste almacen para continuar por favor dirigase a la página "Crear/Modificar Ajuste Almacen".');
                    }
                }
            }
        });
    });



});


