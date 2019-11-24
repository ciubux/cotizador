
jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {

        FooTable.init('#tableListaPrecios');

        //$("#btnBusqueda").click();
        //cargarChosenCliente();
        verificarSiExisteCliente();

        if ($("#pagina").val() == '1001') {
            buscar();
        }
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

        var fecha = $("#periodoSolicitud_fechaInicioFormato").val();
        $("#periodoSolicitud_fechaInicioFormato").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fecha);


        
        var today = new Date();
        var initDate = today.getDate() + '/' + (today.getMonth() + 1) + '/' + today.getFullYear();

        var fecha = $("#periodoSolicitud_fechaFinFormato").val();
        $("#periodoSolicitud_fechaFinFormato").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fecha).datepicker("option", "minDate", initDate);
    });

    function verificarSiExisteCliente() {
        if ($("#idPeriodoSolicitud").val().trim() != "0") {
            $("#btnFinalizarEdicionPeriodoSolicitud").html('Finalizar Edición');
        }
        else {
            $("#btnFinalizarEdicionPeriodoSolicitud").html('Finalizar Creación');
        }

    }

    function limpiarFormulario() {
        //$("#periodoSolicitud_nombre").val("");
    }
    $("#chkSoloCanasta").change(function () {
        if ($(this).is(":checked")) {

            $("#tableListaPrecios tbody tr").hide();
            $(".chkCanasta:checked").closest("tr").show();
            $("#lblChkCanasta").removeClass("text-muted");
        } else {
            $("#tableListaPrecios tbody tr").show();
            $("#lblChkCanasta").addClass("text-muted");
        }
    });

    $("#lblChkCanasta").click(function () {
        if ($("#chkSoloCanasta").is(":checked")) {
            $("#chkSoloCanasta").prop("checked", false);
            $("#tableListaPrecios tbody tr").show();
            $("#lblChkCanasta").addClass("text-muted");
        } else {
            $("#chkSoloCanasta").prop("checked", true);
            $("#tableListaPrecios tbody tr").hide();
            $(".chkCanasta:checked").closest("tr").show();
            $("#lblChkCanasta").removeClass("text-muted");
        }
    });


    $(document).on('change', ".chkCanasta", function () {
        var idProducto = $(this).attr("idProducto");
        if ($(this).is(":checked")) {
            $.ajax({
                url: "/PeriodoSolicitud/AgregarProductoACanasta",
                type: 'POST',
                dataType: 'JSON',
                data: {
                    idProducto: idProducto
                },
                success: function () {
                  /*  if (resultado.success == 1) {
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
                    }*/
                }
            });
        } else {
            $.ajax({
                url: "/PeriodoSolicitud/RetirarProductoDeCanasta",
                type: 'POST',
                dataType: 'JSON',
                data: {
                    idProducto: idProducto
                },
                success: function () {
                    /*
                    if (resultado.success == 1) {
                        $.alert({
                            title: "Operación exitosa",
                            type: 'green',
                            content: resultado.message,
                            buttons: {
                                OK: function () { }
                            }
                        });

                        if ($("#chkSoloCanasta").is(":checked")) {
                            $("#tableListaPrecios tbody tr").hide();
                            $(".chkCanasta:checked").closest("tr").show();
                        }
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
                    }*/
                }
            });
        }
    });



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


    function validacionDatosPeriodoSolicitud() {
        if ($("#periodoSolicitud_nombre").val().length < 4) {
            $.alert({
                title: "Nombre Inválido",
                type: 'orange',
                content: 'Debe ingresar un nombre válido.',
                buttons: {
                    OK: function () { $('#periodoSolicitud_nombre').focus(); }
                }
            });
            return false;
        }

        if ($("#periodoSolicitud_fechaInicioFormato").val().length < 2) {
            $.alert({
                title: "Fecha Inicio Inválida",
                type: 'orange',
                content: 'Debe seleccionar una fecha de inicio.',
                buttons: {
                    OK: function () { $('#periodoSolicitud_fechaInicio').focus(); }
                }
            });
            return false;
        }

        if ($("#periodoSolicitud_fechaFinFormato").val().length < 2) {
            $.alert({
                title: "Fecha Fin Inválida",
                type: 'orange',
                content: 'Debe seleccionar una fecha de fin.',
                buttons: {
                    OK: function () { $('#periodoSolicitud_fechaFin').focus(); }
                }
            });
            return false;
        }

        return true;

    }


    $("#btnFinalizarEdicionPeriodoSolicitud").click(function () {
        /*Si no tiene codigo el cliente se está creando*/
        if ($("#periodoSolicitud_idPeriodoSolicitud").val() == '00000000-0000-0000-0000-000000000000') {
            crearPeriodoSolicitud();
        }
        else {
            editarPeriodoSolicitud();
        }
    });



    function crearPeriodoSolicitud() {
        if (!validacionDatosPeriodoSolicitud())
            return false;

        $('body').loadingModal({
            text: 'Creando Periodo...'
        });
        $.ajax({
            url: "/PeriodoSolicitud/Create",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear el periodo.',
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
                    content: 'El periodo se creó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/PeriodoSolicitud/List';
                        }
                    }
                });
            }
        });

    }

    function editarPeriodoSolicitud() {

        if (!validacionDatosPeriodoSolicitud())
            return false;


        $('body').loadingModal({
            text: 'Editando Periodo...'
        });
        $.ajax({
            url: "/PeriodoSolicitud/Update",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar el periodo.',
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
                    content: 'El periodo se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/PeriodoSolicitud/List';
                        }
                    }
                });
            }
        });
    }

    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/PeriodoSolicitud/ChangeInputInt",
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
            url: "/PeriodoSolicitud/ChangeInputDecimal",
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
            url: "/PeriodoSolicitud/ChangeInputString",
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
            url: "/PeriodoSolicitud/ChangeInputDate",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

   
    $("#periodoSolicitud_nombre").change(function () {
        changeInputString("nombre", $("#periodoSolicitud_nombre").val());
    });

  
    $("#periodoSolicitud_fechaInicioFormato").change(function () {
        var fechaInicio = $("#periodoSolicitud_fechaInicioFormato").val();
        var fechaFin = $("#periodoSolicitud_fechaFinFormato").val();

        if (fechaFin.trim() != "") {
            //Si no está vacía no puede ser menor a la fecha de inicio de vigencia
            if (convertirFechaNumero(fechaFin) < convertirFechaNumero(fechaInicio)) {
                alert("El fecha de inicio del periodo debe ser menor o igual a la fecha de fin.");
                $("#periodoSolicitud_fechaInicioFormato").focus();
                $("#periodoSolicitud_fechaInicioFormato").val("");
                return false;
            }
        }

        changeInputDate("fechaInicio", fechaInicio);
    });

    $("#periodoSolicitud_fechaFinFormato").change(function () {
        var fechaInicio = $("#periodoSolicitud_fechaInicioFormato").val();
        var fechaFin = $("#periodoSolicitud_fechaFinFormato").val();

        if (fechaInicio.trim() != "") {
            //Si no está vacía no puede ser menor a la fecha de inicio de vigencia
            if (convertirFechaNumero(fechaFin) < convertirFechaNumero(fechaInicio)) {
                alert("El fecha de fin del periodo debe ser mayor o igual a la fecha de inicio.");
                $("#fechaFinVigenciaPrecios").focus();
                $("#fechaFinVigenciaPrecios").val("");
                return false;
            }
        }

        changeInputDate("fechaFin", fechaFin);
    });

    $("#periodoSolicitud_Estado").change(function () {
        changeInputInt("Estado", $("#periodoSolicitud_Estado").val());
    });



    $("#btnCancelarPeriodoSolicitud").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/PeriodoSolicitud/CancelarCreacionPeriodoSolicitud', null)
    });




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
        $("#btnOpenAgregarPeriodoSolicitud").attr('disabled', 'disabled');

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

        //   @cotizacionDetalle.producto.idProducto detproductoObservacion"

    });


    function changeInputBoolean(propiedad, valor) {
        $.ajax({
            url: "/PeriodoSolicitud/ChangeInputBoolean",
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
        buscar();

    });

    function buscar() {
        $.ajax({
            url: "/PeriodoSolicitud/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tablePeriodosSolicitud > tbody").empty();
                $("#tablePeriodosSolicitud").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < list.length; i++) {

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list[i].idPeriodoSolicitud + '</td>' +
                        '<td>  ' + list[i].nombre + '  </td>' +
                        '<td>  ' + list[i].fechaInicioFormato + '  </td>' +
                        '<td>  ' + list[i].fechaFinFormato + '  </td>' +
                        '<td>  ' + list[i].nombreEstado + '  </td>' +
                        '<td>' +
                        '<button type="button" class="' + list[i].idPeriodoSolicitud + ' btnEditarPeriodoSolicitud btn btn-primary ">Editar</button>' +
                        '&nbsp;&nbsp;<button type="button" class="' + list[i].idPeriodoSolicitud + ' btnEliminarPeriodoSolicitud btn btn-danger ">Eliminar</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tablePeriodosSolicitud").append(ItemRow);

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
    }


    $(document).on('click', "button.btnEditarPeriodoSolicitud", function () {
      //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idPeriodoSolicitud = arrrayClass[0];
        
        $.ajax({
            url: "/PeriodoSolicitud/ConsultarSiExistePeriodoSolicitud",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idPeriodoSolicitud: idPeriodoSolicitud
            },
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/PeriodoSolicitud/iniciarEdicionPeriodoSolicitud",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del PeriodoSolicitud."); },
                        success: function (fileName) {
                            window.location = '/PeriodoSolicitud/Editar';
                        }
                    });

                }
                else {
                    if (resultado.idPeriodoSolicitud == 0) {
                        alert('Está creando un nuevo periodoSolicitud; para continuar por favor diríjase a la página "Crear/Modificar PeriodoSolicitud" y luego haga clic en el botón Cancelar.');
                    }
                    
                    else {
                        alert('Ya se encuentra editando un periodoSolicitud para continuar por favor dirigase a la página "Crear/Modificar PeriodoSolicitud".');
                    }
                }
            }
        });
    });


    $(document).on('click', "button.btnEliminarPeriodoSolicitud", function () {
        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idPeriodoSolicitud = arrrayClass[0];

        $.confirm({
            title: 'Confirmar operación',
            content: '¿Esta seguro que desea eliminar el periodo? Si el periodo tiene requerimientos asignados estos se perderán.',
            type: 'orange',
            buttons: {
                aplica: {
                    text: 'SI',
                    btnClass: 'btn-success',
                    action: function () {
                        $.ajax({
                            url: "/PeriodoSolicitud/EliminarPeriodoSolicitud",
                            type: 'POST',
                            async: false,
                            dataType: 'JSON',
                            data: {
                                idPeriodoSolicitud: idPeriodoSolicitud
                            },
                            success: function (resultado) {
                                if (resultado.success == "1") {
                                    $.alert({
                                        title: "Operación exitosa",
                                        type: 'green',
                                        content: resultado.message,
                                        buttons: {
                                            OK: function () { }
                                        }
                                    });
                                } else {
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
                    text: 'CANCELAR',
                    btnClass: 'btn-danger',
                    action: function () {
                    }
                }
            }
        });

        
    });

});


