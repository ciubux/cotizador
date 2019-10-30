jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición del mensaje; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';


    $(document).ready(function () {
        $("#btnBusquedaMensaje").click();
        verificarSiExisteMensaje();
        cargarChosenUsuarioMensaje();
    });

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
          
    var fechaInicio = $("#mensaje_fechaInicioMensaje").val();

    var fechaCreacionDesde = $("#mensaje_fechaCreacionDesde").val();
    var fechaCreacionHasta = $("#mensaje_fechaCreacionHasta").val();

    var fechaVencimientoHastaDesde = $("#mensaje_fechaVencimientoDesde").val();
    var fechaVencimientoHastaHasta = $("#mensaje_fechaVencimientoHasta").val();


    var fechaVencimientoedit = $("#mensaje_fechaVencimientoMensaje_edit").val();

    $("#mensaje_fechaVencimientoMensaje_edit").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaVencimientoedit);

    $("#mensaje_fechaVencimientoDesde").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaVencimientoHastaDesde);

    $("#mensaje_fechaVencimientoHasta").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaVencimientoHastaHasta);

    $("#mensaje_fechaInicioMensaje").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaInicio);

    $("#mensaje_fechaCreacionDesde").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaCreacionDesde);

    $("#mensaje_fechaCreacionHasta").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaCreacionHasta);


    $("#btnEnviarMensaje").click(function () {

        if ($("#idMensaje").val() == "00000000-0000-0000-0000-000000000000") {
            crearMensaje();
        }
        else {
            editarMensaje();
        }

    });

    function verificarSiExisteMensaje() {

        if ($("#idMensaje").val() != "00000000-0000-0000-0000-000000000000") {
            $("#btnEnviarMensaje").html('Finalizar Edición');
        }
        else {
            $("#btnEnviarMensaje").html('Finalizar Creación');
        }

    }

    ActulizarMensaje();
    function validacionMensaje() {        
        if ($("input:checkbox:checked").length == 0 && $("#idUsuarioBusquedaMensaje").val().length==0)    
        {
            $.alert({
                title: "Rol o Usuario Inválido",
                type: 'orange',
                content: 'Debe ingresar al menos un Rol o un Usuario para enviar el mensaje.',
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
                content: 'Debe ingresar una Importacia.',
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

        var fechaVencimiento = $("#mensaje_fechaVencimientoMensaje_edit").val();
        var fechaInicio = $("#mensaje_fechaInicioMensaje").val();

        function validate_fechaMayorQue(fechaInicial, fechaFinal) {
            valuesStart = fechaInicial.split("/");
            valuesEnd = fechaFinal.split("/");

            var dateStart = new Date(valuesStart[2], (valuesStart[1] - 1), valuesStart[0]);
            var dateEnd = new Date(valuesEnd[2], (valuesEnd[1] - 1), valuesEnd[0]);
            if (dateStart > dateEnd) {
                return 0;
            }
            return 1;
        }


        if (validate_fechaMayorQue(fechaInicio, fechaVencimiento) == 0) {

            $.alert({
                title: "Fecha Vencimiento Inválida",
                type: 'orange',
                content: 'Debe ingresar una fecha posterior o igual a la de inicio.',
                buttons: {
                    OK: function () { $('#mensaje_fechaVencimientoMensaje').focus(); }
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
            error: function (detalle) {

                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear el mensaje.',
                    type: 'red',
                    buttons: {
                        OK: function () {

                            window.location = '/Mensaje/Editar';
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
                            window.location = '/Mensaje/Lista';
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


    $("#btnLimpiarBusquedaMensaje").click(function () {
        $.ajax({
            url: "/Mensaje/Limpiar",
            type: 'POST',
            success: function () {
                location.reload();
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


    function eliminateDuplicates(arrayIn) {

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

                    list = eliminateDuplicates(list);
                    if (list.length != 0) {
                        $("#imagenMP").before('<a data-notifications="' + list.length + '" class="btnModal" href="javascript:void()"></a>');

                        var verAutomaticamente = false;
                       
                        for (var i = 0; i < list.length; i++) {

                            if (list[i].importancia == 'Alta') {
                                verAutomaticamente = true;
                            }
                            
                            var ItemRow =
                                '</br >' +
                                '<div class="modal-content">' +
                                '<div class="modal-header">' +
                                '<h5> <b>DE: </b>' + list[i].usuario_creacion + ' - <b> FECHA: </b>' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaInicioMensaje)) + '</h5>' +
                                '<h4>' + list[i].titulo + '</h4>' +
                                '</div>' +
                                '<div class="modal-body">' +
                                '<p>' + list[i].mensaje + '<p>' +
                                '</div>' +
                                '<div class="modal-footer">' +
                                '<a  class="Leido btn btn-default ' + list[i].id_mensaje + '">Leido</a>';                                +
                                

                                    $("#MensajeDialog").append(ItemRow);
                        }
                       

                        if (verAutomaticamente) {                           
                            setTimeout(function () {
                                $('#Mensaje').modal('show');
                            }, 2000);
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
            url: "/Mensaje/UpdateMensajeVisto",
            type: 'POST',
            data: {
                idMensaje: idMensaje,
                idUsuario: idUsuario
            },
            success: function () {           
                $('#Mensaje').modal('hide');
                $("#MensajeDialog").empty(); 
                ActulizarMensaje();
            }
        });


    });

    $("#mensaje_fechaInicioMensaje").change(function () {
        var fechaInicio = $("#mensaje_fechaInicioMensaje").val();
        $.ajax({
            url: "/Mensaje/ChangeFechaInicio",
            type: 'POST',
            data: {
                fechaInicio: fechaInicio
            },
            success: function () {
            }
        });
    });
    

    $("#mensaje_fechaVencimientoDesde").change(function () {   
        var valor = $("#mensaje_fechaVencimientoDesde").val();        
        if ($("#mensaje_fechaVencimientoHasta").val() == null || $("#mensaje_fechaVencimientoHasta").val() == undefined || $("#mensaje_fechaVencimientoHasta").val() == "")
        {
            $("#mensaje_fechaVencimientoHasta").val(valor);
        }
        var valor2 = $("#mensaje_fechaVencimientoHasta").val();  
        ChangefechaVencimiento('fechaVencimientoMensajeDesde', valor);   
        ChangefechaVencimiento('fechaVencimientoMensajeHasta', valor2); 
        ChangefechaCreacion('fechaCreacionMensajeDesde', null);
        ChangefechaCreacion('fechaCreacionMensajeHasta', null);
        $("#mensaje_fechaCreacionDesde").val(null);
        $("#mensaje_fechaCreacionHasta").val(null);  
    });

    $("#mensaje_fechaVencimientoHasta").change(function () {
        var valor = $("#mensaje_fechaVencimientoHasta").val();        
        if ($("#mensaje_fechaVencimientoDesde").val() == null || $("#mensaje_fechaVencimientoDesde").val() == undefined || $("#mensaje_fechaVencimientoDesde").val() == "")
        {
            $("#mensaje_fechaVencimientoDesde").val(valor);
        }
        var valor2 = $("#mensaje_fechaVencimientoDesde").val();
        ChangefechaVencimiento('fechaVencimientoMensajeHasta', valor);  
        ChangefechaVencimiento('fechaVencimientoMensajeDesde', valor2); 
        ChangefechaCreacion('fechaCreacionMensajeDesde', null);
        ChangefechaCreacion('fechaCreacionMensajeHasta', null);
        $("#mensaje_fechaCreacionDesde").val(null);
        $("#mensaje_fechaCreacionHasta").val(null);   
    });

    function ChangefechaVencimiento(propiedad, valor) {

        $.ajax({
            url: "/Mensaje/ChangeFechaVencimiento",
            type: 'POST',
            data: {
                fechaVencimiento: valor,
                propiedad: propiedad
            },
            success: function () {
            }
        });
    }


    $("#mensaje_fechaCreacionDesde").change(function () {
        var valor = $("#mensaje_fechaCreacionDesde").val(); 
       
        if ($("#mensaje_fechaCreacionHasta").val() == null || $("#mensaje_fechaCreacionHasta").val() == undefined || $("#mensaje_fechaCreacionHasta").val() == "")
        {
            $("#mensaje_fechaCreacionHasta").val(valor);
        }
        var valor2 = $("#mensaje_fechaCreacionHasta").val();
        ChangefechaCreacion('fechaCreacionMensajeDesde', valor);
        ChangefechaCreacion('fechaCreacionMensajeHasta', valor2);
        ChangefechaVencimiento('fechaVencimientoMensajeDesde', null);
        ChangefechaVencimiento('fechaVencimientoMensajeHasta', null);       
        $("#mensaje_fechaVencimientoDesde").val(null);
        $("#mensaje_fechaVencimientoHasta").val(null); 


    });

    $("#mensaje_fechaCreacionHasta").change(function () {
        var valor = $("#mensaje_fechaCreacionHasta").val();
       
        if ($("#mensaje_fechaCreacionDesde").val() == null || $("#mensaje_fechaCreacionDesde").val() == undefined || $("#mensaje_fechaCreacionDesde").val() == "")
        {
            $("#mensaje_fechaCreacionDesde").val(valor);
        }
        var valor2 = $("#mensaje_fechaCreacionDesde").val();
        ChangefechaCreacion('fechaCreacionMensajeHasta', valor);
        ChangefechaCreacion('fechaCreacionMensajeDesde', valor2);
        ChangefechaVencimiento('fechaVencimientoMensajeDesde', null);
        ChangefechaVencimiento('fechaVencimientoMensajeHasta', null); 
        $("#mensaje_fechaVencimientoDesde").val(null);
        $("#mensaje_fechaVencimientoHasta").val(null);  
    });

    function ChangefechaCreacion(propiedad,valor) {
      
        $.ajax({
            url: "/Mensaje/ChangeFechaCreacion",
            type: 'POST',
            data: {
                fechaCreacion: valor,
                propiedad:propiedad
            },
            success: function () {
            }
        });
    }

    $("#mensaje_fechaVencimientoMensaje_edit").change(function () {
        var fechaVencimiento = $("#mensaje_fechaVencimientoMensaje_edit").val();

        $.ajax({
            url: "/Mensaje/ChangeFechaVencimientoEdit",
            type: 'POST',
            data: {
                fechaVencimiento: fechaVencimiento
            },
            success: function () {
            }
        });
    });




    $("#btnBusquedaMensaje").click(function () {

        $("#btnBusquedaMensaje").attr("disabled", "disabled");
        $.ajax({
            url: "/Mensaje/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusquedaMensaje").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusquedaMensaje").removeAttr("disabled");
                $("#tableMensaje > tbody").empty();
                $("#tableMensaje").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < list.length; i++) {

                    var ItemRow = '<tr data-expanded="true">' +

                        '<td>  ' + list[i].id_mensaje + '  </td>' +
                        '<td>  ' + list[i].titulo + '  </td>' +
                        '<td>  ' + list[i].usuario_creacion + '  </td>' +
                        '<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaCreacionMensaje)) + '  </td>' +
                        '<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaInicioMensaje)) + '  </td>' +
                        '<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaVencimientoMensaje)) + '  </td>' +
                        '<td>' +
                        '<button type="button" class="' + list[i].id_mensaje + ' btnEditarMensaje btn btn-primary ">Ver</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tableMensaje").append(ItemRow);

                }



            }
        });
    });


    $(document).on('click', "button.btnEditarMensaje", function () {
        //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idMensaje = arrrayClass[0];

        $.ajax({
            url: "/Mensaje/ConsultarSiExisteMensaje",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idMensaje: idMensaje
            },
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/Mensaje/iniciarEdicionMensaje",
                        type: 'POST',
                        error: function (detalle) {
                            alert("Ocurrió un problema al iniciar la edición del mensaje.");
                        },
                        success: function (fileName) {
                            window.location = '/Mensaje/Editar';

                        }
                    });

                }
                else {
                    if (resultado.idVendedor == 0) {
                        alert('Está creando un nuevo mensaje; para continuar por favor diríjase a la página "Crear/Modificar Vendedor" y luego haga clic en el botón Cancelar.');
                    }

                    else {
                        alert('Ya se encuentra editando un mensaje para continuar por favor dirigase a la página "Crear/Modificar Mensaje".');
                    }
                }
            }
        });


    });

    $("#btnCancelarMensaje").click(function () {

        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/Mensaje/CancelarCreacionMensaje', null);
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

    $("#mensaje_estado_si").click(function () {
        var valCheck = 1;
        changeInputInt("estado", valCheck);
    });

    $("#mensaje_estado_no").click(function () {
        var valCheck = 0;
        changeInputInt("estado", valCheck);
    });

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


    function editarMensaje() {

        if (!validacionMensaje())
            return false;


        $('body').loadingModal({
            text: 'Editando Mensaje...'
        });
        $.ajax({
            url: "/Mensaje/UpdateMensaje",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar el mensaje.',
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
                    content: 'El mensaje se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Mensaje/Lista';
                        }
                    }
                });
            }
        });
    }



    function cargarChosenUsuarioMensaje() {

       
        $("#idUsuarioBusquedaMensaje").chosen({ placeholder_text: "Buscar Usuario", no_results_text: "No se encontró Usuario", allow_single_deselect: true }).on('chosen:showing_dropdown');


        $("#idUsuarioBusquedaMensaje").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 0,
            afterTypeDelay: 300,
            cache: false,
            url: "/Usuario/SearchUsuarios"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Usuario", no_results_text: "No se encontró Usuario" });
    }

    $("#idUsuarioBusquedaMensaje").change(function () {

        var idUsuario = $("#idUsuarioBusquedaMensaje").val();       

        $.ajax({
            url: "/Mensaje/ChangeUsuarioMensaje",
            type: 'POST',
            data: {
                idUsuario: idUsuario
            },
            success: function () {
            }
        });

    });

    
});