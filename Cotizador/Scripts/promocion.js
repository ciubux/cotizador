
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

    $("#promocion_fechaInicioDesc").datepicker({ dateFormat: "dd/mm/yy" });
    $("#promocion_fechaFinDesc").datepicker({ dateFormat: "dd/mm/yy" });

    $(document).ready(function () {
        $("#btnBusqueda").click();
        //cargarChosenCliente();
        verificarSiExistePromocion();

    });

    function verificarSiExistePromocion() {
        if ($("#idPromocion").val().trim() != "0") {
            $("#btnFinalizarEdicionPromocion").html('Finalizar Edición');
        }
        else {
            $("#btnFinalizarEdicionPromocion").html('Finalizar Creación');
        }

    }

    function limpiarFormulario() {
        $("#promocion_codigo").val("");
        $("#promocion_titulo").val("");
        $("#promocion_descripcion").val("");
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


    function validacionDatosPromocion() {
       
        if ($("#promocion_codigo").val().length < 2) {
            $.alert({
                title: "Código Inválido",
                type: 'orange',
                content: 'Debe ingresar un Código de promocion válido.',
                buttons: {
                    OK: function () { $('#promocion_codigo').focus(); }
                }
            });
            return false;
        }
        
        if ($("#promocion_titulo").val().length < 4) {
            $.alert({
                title: "Título Inválido",
                type: 'orange',
                content: 'Debe ingresar un titulo válido.',
                buttons: {
                    OK: function () { $('#promocion_titulo').focus(); }
                }
            });
            return false;
        }

        return true;

    }


    $("#btnFinalizarEdicionPromocion").click(function () {
        /*Si no tiene codigo el cliente se está creando*/
        if ($("#promocion_idPromocion").val() == '0') {
            crearPromocion();
        }
        else {
            editarPromocion();
        }
    });



    function crearPromocion() {
        if (!validacionDatosPromocion())
            return false;

        $('body').loadingModal({
            text: 'Creando Promocion...'
        });
        $.ajax({
            url: "/Promocion/Create",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear la promocion.',
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
                    content: 'El promocion se creó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Promocion/List';
                        }
                    }
                });
            }
        });

    }

    function editarPromocion() {

        if (!validacionDatosPromocion())
            return false;


        $('body').loadingModal({
            text: 'Editando Promocion...'
        });
        $.ajax({
            url: "/Promocion/Update",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar la promocion.',
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
                    content: 'El promocion se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Promocion/List';
                        }
                    }
                });
            }
        });
    }

    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/Promocion/ChangeInputInt",
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
            url: "/Promocion/ChangeInputDecimal",
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
            url: "/Promocion/ChangeDate",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    $("#promocion_estado_si").click(function () {
        var valCheck = 1;
        changeInputInt("Estado", valCheck)
    });

    $("#promocion_estado_no").click(function () {
        var valCheck = 0;
        changeInputInt("Estado", valCheck)
    });

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Promocion/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    $("#promocion_codigo").change(function () {
        changeInputString("codigo", $("#promocion_codigo").val());
    });

    $("#promocion_titulo").change(function () {
        changeInputString("titulo", $("#promocion_titulo").val());
    });

    $("#promocion_descripcion").change(function () {
        changeInputString("descripcion", $("#promocion_descripcion").val());
    });

    $("#promocion_fechaInicioDesc").change(function () {
        changeInputDate("fechaInicio", $("#promocion_fechaInicioDesc").val());
    });

    $("#promocion_fechaFinDesc").change(function () {
        changeInputDate("fechaFin", $("#promocion_fechaFinDesc").val());
    });

    $("#btnCancelarPromocion").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/Promocion/CancelarCreacionPromocion', null)
    })




    var ft = null;

    
    function changeInputBoolean(propiedad, valor) {
        $.ajax({
            url: "/Promocion/ChangeInputBoolean",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    
    //$("#btnExportExcel").click(function () {
    //    window.location.href = $(this).attr("actionLink");
    //});

    $("#btnBusqueda").click(function () {



        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/Promocion/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tablePromociones > tbody").empty();
                

                for (var i = 0; i < list.length; i++) {

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list[i].idPromocion + '</td>' +
                        '<td>  ' + list[i].codigo + '  </td>' +
                        '<td>  ' + list[i].titulo + '  </td>' +
                        '<td>  ' + list[i].fechaInicioDesc + '  </td>' +
                        '<td>  ' + list[i].fechaFinDesc + '  </td>' +
                        '<td>' +
                        '<button type="button" class="' + list[i].idPromocion + ' btnEditarPromocion btn btn-primary ">Editar</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tablePromociones").append(ItemRow);

                }

                $("#tablePromociones").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                if (list.length > 0) {
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


    $(document).on('click', "button.btnEditarPromocion", function () {
      //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idPromocion = arrrayClass[0];
        
        $.ajax({
            url: "/Promocion/ConsultarSiExistePromocion",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idPromocion: idPromocion
            },
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/Promocion/iniciarEdicionPromocion",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del Promoción."); },
                        success: function (fileName) {
                            window.location = '/Promocion/Editar';
                        }
                    });

                }
                else {
                    if (resultado.idPromocion == '00000000-0000-0000-0000-000000000000') {
                        alert('Está creando un nuevo promoción; para continuar por favor diríjase a la página "Crear/Modificar Promoción" y luego haga clic en el botón Cancelar.');
                    }
                    
                    else {
                        alert('Ya se encuentra editando un promoción para continuar por favor dirigase a la página "Crear/Modificar Promoción".');
                    }
                }
            }
        });
    });



});


