
jQuery(function ($) {
    
    var TITLE_EXITO = 'Operación Realizada';

    //CONSTANTES POR DEFECTO
    var cantidadDecimales = 2;
    var cantidadCuatroDecimales = 4;
    var IGV = 0.18;
    var SIMBOLO_SOL = "S/";
    var MILISEGUNDOS_AUTOGUARDADO = 5000;
    var VARIACION_PRECIO_ITEM_PEDIDO = 0.01;


    //Estados para búsqueda de Pedidos


    var ESTADO_PENDIENTE_APROBACION = 0;
    var ESTADO_INGRESADO = 1;
    var ESTADO_DENEGADO = 2;
    var ESTADO_PROGRAMADO = 3;
    var ESTADO_ATENDIDO = 4;
    var ESTADO_ATENDIDO_PARCIALMENTE = 5;
    var ESTADO_EN_EDICION = 6;
    var ESTADO_ELIMINADO = 7;
    var ESTADO_FACTURADO = 8;


    //Etiquetas de estadps para búsqueda de Pedidos
    var ESTADO_PENDIENTE_APROBACION_STR = "Pendiente de Aprobación de Ingreso";
    var ESTADO_INGRESADO_STR = "Pedido Ingresado";
    var ESTADO_DENEGADO_STR = "Pedido Denegado";
    var ESTADO_PROGRAMADO_STR = "Pedido Programado"
    var ESTADO_ATENDIDO_STR = "Pedido Atendido"
    var ESTADO_ATENDIDO_PARCIALMENTE_STR = "Pedido Atendido Parcialmente"
    var ESTADO_EN_EDICION_STR = "Pedido En Edicion";
    var ESTADO_ELIMINADO_STR = "Pedido Eliminado";
    var ESTADO_FACTURADO_STR = "Pedido Facturado";

    var TITULO_CANCELAR_PROGRAMACION = "Cancelar Programación de Pedido";
    var TITULO_DENEGAR_INGRESO = "Denegar Ingreso de Pedido";
    var TITULO_APROBAR_INGRESO = "Aprobar Ingreso de Pedido";
    var TITULO_ELIMINAR = "Eliminar Pedido";

    //Estados Crediticios
    var ESTADO_PENDIENTE_LIBERACION = 0;
    var ESTADO_LIBERADO = 1;
    var ESTADO_BLOQUEADO = 2;

    var ESTADO_PENDIENTE_LIBERACION_STR = "Pedido Pendiente de Liberación";
    var ESTADO_LIBERADO_STR = "Pedido Liberado";
    var ESTADO_BLOQUEADO_STR = "Pedido Bloqueado";




    //Eliminar luego 
    var CANT_SOLO_OBSERVACIONES = 0;
    var CANT_SOLO_CANTIDADES = 1;
    var CANT_CANTIDADES_Y_OBSERVACIONES = 2;

    var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";

    /*
       $("#btnAgregarCliente").click(function () {
   
   
           $("#modalAgregarCliente2").load('/Cliente/Editar');
       });*/

    /*
     * 2 BusquedaPedidos
       3 CrearPedido
     */

    var pagina = 2;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";

    $(document).ready(function () {
        obtenerConstantes();
        setTimeout(autoGuardarPedido, MILISEGUNDOS_AUTOGUARDADO);
        //cargarChosenCliente(pagina);
        toggleControlesUbigeo();
        verificarSiExisteNuevaDireccionEntrega();
        verificarSiExisteDetalle();
        verificarSiExisteCliente();
        $("#btnBusquedaPedidos").click();
        var tipoPedido = $("#pedido_tipoPedido").val();
        validarTipoPedido(tipoPedido);

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

    function verificarSiExisteCliente() {
        if ($("#idCliente").val().trim() != "" && $("#pagina").val() == 1)
            $("#idCiudad").attr("disabled", "disabled");
    }


    function obtenerConstantes() {
        $.ajax({
            url: "/General/GetConstantes",
            type: 'POST',
            dataType: 'JSON',
            success: function (constantes) {
                IGV = constantes.IGV;
                SIMBOLO_SOL = constantes.SIMBOLO_SOL;
                MILISEGUNDOS_AUTOGUARDADO = constantes.MILISEGUNDOS_AUTOGUARDADO;
                VARIACION_PRECIO_ITEM_PEDIDO = constantes.VARIACION_PRECIO_ITEM_PEDIDO;
            }
        });
    }

    function autoGuardarPedido() {
        $.ajax({
            url: "/Pedido/autoGuardarPedido",
            type: 'POST',
            error: function () {
                setTimeout(autoGuardarPedido, MILISEGUNDOS_AUTOGUARDADO);
            },
            success: function () {
                setTimeout(autoGuardarPedido, MILISEGUNDOS_AUTOGUARDADO);
            }
        });
    }


    function verificarSiExisteDetalle() {
        //Si existen productos agregados no se puede obtener desde precios registrados
        var contador = 0;
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {
            contador++;
        });

        if (contador > 0) {
            $("#btnAgregarProductosDesdePreciosRegistrados").attr('disabled', 'disabled');
            return true;
        }
        else {
            $("#btnAgregarProductosDesdePreciosRegistrados").removeAttr('disabled');
            return false;
        }
    }


    function verificarSiExisteNuevaDireccionEntrega() {
        $('#pedido_direccionEntrega option').each(function () {
            if ($(this).val() == GUID_EMPTY) {
                $("#btnAgregarDireccion").attr("disabled", "disabled");
            }
        });
    }

    /**
     * ################################ INICIO CONTROLES DE CLIENTE
     */

    function cargarChosenCliente(pagina) {

        $("#idCliente").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" }).on('chosen:showing_dropdown', function (evt, params) {
            if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
                alert("Debe seleccionar la sede MP previamente.");
                $("#idCliente").trigger('chosen:close');
                $("#idCiudad").focus();
                return false;
            }
        });

        $("#idCliente").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/Pedido/SearchClientes"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });
    }





    function toggleControlesUbigeo() {
        //Si cliente esta Seleccionado se habilitan la seleccion para la direccion de entrega
        if ($("#idCliente").val().trim() == "") {
            $("#ActualDepartamento").attr('disabled', 'disabled');
            $('#ActualProvincia').attr('disabled', 'disabled');
            $('#ActualDistrito').attr('disabled', 'disabled');
            $("#pedido_direccionEntrega").attr('disabled', 'disabled');
            $("#btnAgregarDireccion").attr("disabled", "disabled");
        }
        else {
            $('#ActualDepartamento').removeAttr('disabled');
            $('#ActualProvincia').removeAttr('disabled');
            $('#ActualDistrito').removeAttr('disabled');
            $("#pedido_direccionEntrega").removeAttr('disabled');
            $("#btnAgregarDireccion").removeAttr('disabled');
        }
        toggleControlesDireccionEntrega();
    }




    function toggleControlesDireccionEntrega() {
        var idDireccionEntrega = $('#pedido_direccionEntrega').val();
        if (idDireccionEntrega == "") {
            $("#pedido_direccionEntrega_descripcion").attr('disabled', 'disabled');
            $("#pedido_direccionEntrega_contacto").attr('disabled', 'disabled');
            $("#pedido_direccionEntrega_telefono").attr('disabled', 'disabled');

        }
        else {
            /*  $("#pedido_direccionEntrega_telefono").val($('#pedido_direccionEntrega').find(":selected").attr("telefono"));*/
            $("#pedido_direccionEntrega_descripcion").removeAttr("disabled");
            $("#pedido_direccionEntrega_contacto").removeAttr("disabled");
            $("#pedido_direccionEntrega_telefono").removeAttr("disabled");
        }
    }




    $("#idCliente").change(function () {

        var idCliente = $(this).val();

        $.ajax({
            url: "/Pedido/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente) {
                if ($("#pagina").val() == 3)
                    $("#idCiudad").attr("disabled", "disabled");

                $("#idCiudad").attr("disabled", "disabled");
                var direccionEntregaListTmp = cliente.direccionEntregaList;

                $('#pedido_direccionEntrega')
                    .find('option')
                    .remove()
                    .end()
                    ;

                $('#pedido_direccionEntrega').append($('<option>', {
                    value: "",
                    text: "Seleccione Dirección de Entrega"
                }));


                for (var i = 0; i < direccionEntregaListTmp.length; i++) {
                    $('#pedido_direccionEntrega').append($('<option>', {
                        value: direccionEntregaListTmp[i].idDireccionEntrega,
                        text: direccionEntregaListTmp[i].descripcion
                        /*,
                        direccion: direccionEntregaListTmp[i].descripcion,
                        contacto: direccionEntregaListTmp[i].contacto,
                        telefono: direccionEntregaListTmp[i].telefono*/
                    }));
                }

                //Se limpia controles de Ubigeo
                $("#ActualDepartamento").val("");
                $("#ActualProvincia").val("");
                $("#ActualDistrito").val("");


                toggleControlesUbigeo();
            }
        });

    });






    $('#modalAgregarCliente').on('shown.bs.modal', function () {



        $("#ncRazonSocial").val("");
        $("#ncRUC").val("");
        $("#ncNombreComercial").val("");
        $("#ncDireccion").val("");
        $("#ncTelefono").val("");

        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar una ciudad previamente.");
            $("#idCiudad").focus();
            $('#btnCancelCliente').click();
            return false;
        }



    });


    $('#modalAgregarDireccion').on('shown.bs.modal', function () {
        $('#direccionEntrega_descripcion').focus();
        /*     $('#pedido_direccionEntrega option').each(function () {
     
                 if ($(this).val() == GUID_EMPTY) {
                     alert("");
                 }
                 alert();
     
                 return true;
             });*/
    });


    /*
    $(window).on("paste", function (e) {
        var modalAgregarClienteIsShown = ($("#modalAgregarCliente").data('bs.modal') || { isShown: false }).isShown;
        if (modalAgregarClienteIsShown) {
            $.each(e.originalEvent.clipboardData.items, function () {
                this.getAsString(function (str) {
                   // alert(str);
                    var lineas = str.split("\t");

                    $("#ncRazonSocial").val(lineas[0]);
                    $("#ncRUC").val(lineas[1]);
                    $("#ncNombreComercial").val(lineas[2]);
                    $("#ncDireccion").val(lineas[3]);
                    $("#ncTelefono").val(lineas[4]);
                });
            });

        }
       
    });*/



    $("#btnSaveCliente").click(function () {

        if ($("#ncRazonSocial").val().trim() == "" && $("#ncNombreComercial").val().trim() == "") {
            alert("Debe ingresar la Razón Social o el Nombre Comercial.");
            $('#ncRazonSocial').focus();
            return false;
        }

        if ($("#ncRUC").val().trim() == "") {
            alert("Debe ingresar el RUC.");
            $('#ncRUC').focus();
            return false;
        }

        var razonSocial = $("#ncRazonSocial").val();
        var nombreComercial = $("#ncNombreComercial").val();
        var ruc = $("#ncRUC").val();
        var contacto = $("#ncContacto").val();

        $.ajax({
            url: "/Cliente/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                razonSocial: razonSocial,
                nombreComercial: nombreComercial,
                ruc: ruc,
                contacto: contacto,
                controller: "pedido"
            },
            error: function (detalle) {
                alert("Se generó un error al intentar crear el cliente.");
            },
            success: function (resultado) {
                alert("Se creó cliente con Código Temporal: " + resultado.codigoAlterno + ".");
                location.reload();
            }
        });


        $('#btnCancelCliente').click();

    });


    /**
     * FIN CONTROLES DE CLIENTE
     */


    $("#btnSaveDireccion").click(function () {

        if ($("#direccionEntrega_descripcion").val().trim() == "") {
            alert("Debe ingresar la dirección de entrega.");
            $('#direccionEntrega_descripcion').focus();
            return false;
        }

        if ($("#direccionEntrega_contacto").val().trim() == "") {
            alert("Debe ingresar el nombre del contacto de entrega.");
            $('#direccionEntrega_contacto').focus();
            return false;
        }

        if ($("#direccionEntrega_telefono").val().trim() == "") {
            alert("Debe ingresar el telefono del contacto de entrega.");
            $('#direccionEntrega_telefono').focus();
            return false;
        }

        var direccion = $("#direccionEntrega_descripcion").val();
        var contacto = $("#direccionEntrega_contacto").val();
        var telefono = $("#direccionEntrega_telefono").val();

        $.ajax({
            url: "/Pedido/CreateDireccionTemporal",
            type: 'POST',
            dataType: 'JSON',
            data: {
                direccion: direccion,
                contacto: contacto,
                telefono: telefono
            },
            error: function (detalle) { alert("Se generó un error al intentar crear la dirección."); },
            success: function (direccion) {

                $('#pedido_direccionEntrega').append($('<option>', {
                    value: direccion.idDireccionEntrega,
                    text: direccion.descripcion
                }));
                $('#pedido_direccionEntrega').val(direccion.idDireccionEntrega);

                $('#pedido_direccionEntrega_descripcion').val(direccion.descripcion);
                $('#pedido_direccionEntrega_contacto').val(direccion.contacto);
                $('#pedido_direccionEntrega_telefono').val(direccion.telefono);
                verificarSiExisteNuevaDireccionEntrega();
                toggleControlesDireccionEntrega();
            }
        });


        $('#btnCancelDireccion').click();

    });


















    /**
     * ######################## INICIO CONTROLES DE FECHAS
     */
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


    var fechaSolicitud = $("#fechaSolicitudTmp").val();
    $("#fechaSolicitud").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaSolicitud);

    var fechaSolicitudDesde = $("#fechaSolicitudDesdetmp").val();
    $("#pedido_fechaSolicitudDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaSolicitudDesde);

    var fechaSolicitudHasta = $("#fechaSolicitudHastatmp").val();
    $("#pedido_fechaSolicitudHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaSolicitudHasta);

    var fechaEntregaDesde = $("#fechaEntregaDesdetmp").val();
    $("#pedido_fechaEntregaDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEntregaDesde);

    var fechaEntregaHasta = $("#fechaEntregaHastatmp").val();
    $("#pedido_fechaEntregaHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEntregaHasta);



    var fechaProgramacionDesde = $("#fechaProgramacionDesdetmp").val();
    $("#pedido_fechaProgramacionDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaProgramacionDesde);

    var fechaProgramacionHasta = $("#fechaProgramacionHastatmp").val();
    $("#pedido_fechaProgramacionHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaProgramacionHasta);


    var fechaPrecios = $("#fechaPreciostmp").val();
    $("#fechaPrecios").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaPrecios);

    //var fechaProgramacion = $("#fechaProgramaciontmp").val();
    $("#fechaProgramacion").datepicker({ dateFormat: "dd/mm/yy" });//.datepicker("setDate", fechaProgramacion);    

    var documentoVenta_fechaEmision = $("#documentoVenta_fechaEmisiontmp").val();
    $("#documentoVenta_fechaEmision").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", documentoVenta_fechaEmision);

    var documentoVenta_fechaVencimiento = $("#documentoVenta_fechaVencimientotmp").val();
    $("#documentoVenta_fechaVencimiento").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", documentoVenta_fechaVencimiento);


    /**
     * FIN DE CONTROLES DE FECHAS
     */



    /* ################################## INICIO CHANGE CONTROLES */

    function validarTipoPedido(tipoPedido) {
        //Si el tipo de pedido es traslado interno (84->'T')
        if (tipoPedido == "84") {
            $("#divReferenciaCliente").hide();
            $("#divCiudadASolicitar").show();
        }
        else {
            $("#divReferenciaCliente").show();
            $("#divCiudadASolicitar").hide();
        }

    }

    $("#pedido_tipoPedido").change(function () {
        var tipoPedido = $("#pedido_tipoPedido").val();
        validarTipoPedido(tipoPedido);


        $.ajax({
            url: "/Pedido/ChangeTipoPedido",
            type: 'POST',
            data: {
                tipoPedido: tipoPedido
            },
            success: function () { }
        });
    });

    $("#idCiudadASolicitar").change(function () {
        var idCiudadASolicitar = $("#idCiudadASolicitar").val();

        $.ajax({
            url: "/Pedido/ChangeIdCiudadASolicitar",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudadASolicitar: idCiudadASolicitar
            },
            error: function (detalle) {
                alert('Debe eliminar los productos agregados antes de cambiar de Sede.');
                location.reload();
            },
            success: function (ciudad) {
            }
        });
    });



    $("#pedido_numeroReferenciaCliente").change(function () {
        $.ajax({
            url: "/Pedido/ChangeNumeroReferenciaCliente",
            type: 'POST',
            data: {
                numeroReferenciaCliente: $("#pedido_numeroReferenciaCliente").val()
            },
            success: function () { }
        });
    });

    $("#pedido_otrosCargos").change(function () {
        $.ajax({
            url: "/Pedido/ChangeOtrosCargos",
            type: 'POST',
            data: {
                otrosCargos: $("#pedido_otrosCargos").val()
            },
            success: function () { }
        });
    });

    $('#pedido_direccionEntrega').change(function () {
        toggleControlesDireccionEntrega();
        var idDireccionEntrega = $('#pedido_direccionEntrega').val();
        $.ajax({
            url: "/Pedido/ChangeDireccionEntrega",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idDireccionEntrega: idDireccionEntrega
            },
            success: function (direccionEntrega) {

                $("#pedido_direccionEntrega_telefono").val(direccionEntrega.telefono);
                $("#pedido_direccionEntrega_contacto").val(direccionEntrega.contacto);
                $("#pedido_direccionEntrega_descripcion").val(direccionEntrega.descripcion);

            }
        })
    });


    $("#pedido_direccionEntrega_descripcion").change(function () {
        $.ajax({
            url: "/Pedido/ChangeDireccionEntregaDescripcion",
            type: 'POST',
            data: {
                direccionEntregaDescripcion: $("#pedido_direccionEntrega_descripcion").val()
            },
            success: function () { }
        });
    });

    $("#pedido_direccionEntrega_contacto").change(function () {
        $.ajax({
            url: "/Pedido/ChangeDireccionEntregaContacto",
            type: 'POST',
            data: {
                direccionEntregaContacto: $("#pedido_direccionEntrega_contacto").val()
            },
            success: function () { }
        });
    });

    $("#pedido_direccionEntrega_telefono").change(function () {
        $.ajax({
            url: "/Pedido/ChangeDireccionEntregaTelefono",
            type: 'POST',
            data: {
                direccionEntregaTelefono: $("#pedido_direccionEntrega_telefono").val()
            },
            success: function () { }
        });
    });


    $(".fechaSolicitud").change(function () {
        var fechaSolicitud = $("#fechaSolicitud").val();
        var horaSolicitud = $("#horaSolicitud").val();
        $.ajax({
            url: "/Pedido/ChangeFechaSolicitud",
            type: 'POST',
            data: {
                fechaSolicitud: fechaSolicitud,
                horaSolicitud: horaSolicitud
            },
            success: function () {
            }
        });
    });









    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Pedido/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#pedido_horaEntregaDesde").change(function () {

        var horaEntregaDesde = $("#pedido_horaEntregaDesde").val();
        var horaEntregaHasta = $("#pedido_horaEntregaHasta").val();

        //Si la hora de entrega desde tiene una diferencia menor a dos horas con la hora entrega hasta
        //Se reemplaza la hora de entraga hasta con la hora entrega desde más dos horas
        if (convertirHoraToNumero(horaEntregaDesde) + 2 > convertirHoraToNumero(horaEntregaHasta)) {
            $("#pedido_horaEntregaHasta").val(sumarHoras($("#pedido_horaEntregaDesde").val(), 2));
            $("#pedido_horaEntregaHasta").change();
        }

        changeInputString("horaEntregaDesde", $("#pedido_horaEntregaDesde").val())
    });

    $("#pedido_horaEntregaHasta").change(function () {
        var horaEntregaDesde = $("#pedido_horaEntregaDesde").val();
        var horaEntregaHasta = $("#pedido_horaEntregaHasta").val();

        //Si la hora de entrega desde tiene una diferencia menor a dos horas con la hora entrega hasta
        //Se reemplaza la hora de entraga hasta con la hora entrega desde más dos horas
        if (convertirHoraToNumero(horaEntregaDesde) + 2 > convertirHoraToNumero(horaEntregaHasta)) {
            $("#pedido_horaEntregaDesde").val(sumarHoras($("#pedido_horaEntregaHasta").val(), -2));
            $("#pedido_horaEntregaDesde").change();
        }
        changeInputString("horaEntregaHasta", $("#pedido_horaEntregaHasta").val())

    });

    $("#pedido_numeroReferenciaCliente").change(function () {
        changeInputString("numeroReferenciaCliente", $("#pedido_numeroReferenciaCliente").val())
    });

    $("#pedido_observacionesFactura").change(function () {
        changeInputString("observacionesFactura", $("#pedido_observacionesFactura").val())
    });

    $("#pedido_observacionesGuiaRemision").change(function () {
        changeInputString("observacionesGuiaRemision", $("#pedido_observacionesGuiaRemision").val())
    });




    $("#pedido_contactoPedido").change(function () {
        changeInputString("contactoPedido", $("#pedido_contactoPedido").val())
    });

    $("#pedido_telefonoContactoPedido").change(function () {
        changeInputString("telefonoContactoPedido", $("#pedido_telefonoContactoPedido").val())
    });

    $("#pedido_correoContactoPedido").change(function () {
        changeInputString("correoContactoPedido", $("#pedido_correoContactoPedido").val())
    });

    $("#pedido_observaciones").change(function () {
        changeInputString("observaciones", $("#pedido_observaciones").val())
    });

    /**
     * ################################ INICIO CONTROLES DE AGREGAR PRODUCTO
     */

    ////////////////ABRIR AGREGAR PRODUCTO
    $('#btnOpenAgregarProducto').click(function () {

        //Para agregar un producto se debe seleccionar una ciudad
        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar la sede MP previamente.");
            return false;
        }

        //para agregar un producto se debe seleccionar un cliente
        if ($("#idCliente").val().trim() == "") {
            alert("Debe seleccionar previamente un cliente.");
            $('#idCliente').trigger('chosen:activate');
            return false;
        }


        //Se limpia el mensaje de resultado de agregar producto
        $("#resultadoAgregarProducto").html("");

        //Se desactiva el boton de agregar producto
        desactivarBtnAddProduct();

        //Se limpian los campos
        $("#unidad").html("");
        $("#imgProducto").attr("src", "images/NoDisponible.gif");
        $("#precioUnitarioSinIGV").val(0);
        $("#precioUnitarioAlternativoSinIGV").val(0);
        $("#subtotal").val(0);
        $("#porcentajeDescuento").val(Number(0).toFixed(10));
        $('#valor').val(0);
        $('#valorAlternativo').val(0);
        $('#observacionProducto').val("");
        $('#valor').attr('type', 'text');
        $('#valorAlternativo').attr('type', 'hidden');
        $('#precio').val(0);
        $('#cantidad').val(1);


        //Se agrega chosen al campo PRODUCTO
        $("#producto").chosen({ placeholder_text_single: "Seleccione el producto", no_results_text: "No se encontró Producto" });

        $("#producto").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/Producto/Search"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Seleccione el producto", no_results_text: "No se encontró Producto" });

        $('#producto').val('').trigger('chosen:updated');
        $('#producto').val('').trigger('liszt:updated');

        $('#producto')
            .find('option:first-child').prop('selected', true)
            .end().trigger('chosen:updated');

        calcularSubtotalProducto();

    });

    //EVENTO CUANDO SE ABRE VENTANA DE AGREGAR PRODUCTO
    $('#modalAgregarProducto').on('shown.bs.modal', function () {
        $('#familia').focus();
        $('#familia').val("Todas");
        $('#proveedor').val("Todos");

        //$('#producto').trigger('chosen:activate');
    })


    /////////////CAMPO PRODUCTO 
    $("#producto").change(function () {
        $("#resultadoAgregarProducto").html("");
        desactivarBtnAddProduct();
        $.ajax({
            url: "/Pedido/GetProducto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: $(this).val(),
            },
            success: function (producto) {
                // var producto = $.parseJSON(respuesta);


                $("#imgProducto").attr("src", producto.image);

                //Se agrega el precio estandar
                var options = "<option value='0' selected>" + producto.unidad + "</option>";
                if (producto.unidad_alternativa != "") {
                    //Se agrega el precio alternativo
                    options = options + "<option value='1'>" + producto.unidad_alternativa + "</option>";
                }

                //Limpieza de campos
                $("#costoLista").val(Number(producto.costoLista));
                $("#precioLista").val(Number(producto.precioLista));
                $("#unidad").html(options);
                $("#proveedor").val(producto.proveedor);
                $("#familia").val(producto.familia);
                $('#precioUnitarioSinIGV').val(producto.precioUnitarioSinIGV);
                $('#precioUnitarioAlternativoSinIGV').val(producto.precioUnitarioAlternativoSinIGV);
                $('#costoSinIGV').val(producto.costoSinIGV);
                $('#costoAlternativoSinIGV').val(producto.costoAlternativoSinIGV);
                $('#observacionProducto').val("");
                $('#fleteDetalle').val(producto.fleteDetalle);
                $("#porcentajeDescuento").val(Number(producto.porcentajeDescuento).toFixed(10));
                $("#cantidad").val(1);

                $("#tableMostrarPrecios > tbody").empty();

                $("#verProducto").html(producto.nombre);

                FooTable.init('#tableMostrarPrecios');
                for (var i = 0; i < producto.precioListaList.length; i++) {
                    var fechaInicioVigencia = producto.precioListaList[i].fechaInicioVigencia;
                    var fechaFinVigencia = producto.precioListaList[i].fechaFinVigencia;

                    if (fechaInicioVigencia == null)
                        fechaInicioVigencia = "No Definida";
                    else
                        fechaInicioVigencia = invertirFormatoFecha(producto.precioListaList[i].fechaInicioVigencia.substr(0, 10));

                    if (fechaFinVigencia == null)
                        fechaFinVigencia = "No Definida";
                    else
                        fechaFinVigencia = invertirFormatoFecha(producto.precioListaList[i].fechaFinVigencia.substr(0, 10));

                    var numeroCotizacion = producto.precioListaList[i].numeroCotizacion;
                    if (numeroCotizacion == null)
                        numeroCotizacion = "No Identificado";



                    $("#tableMostrarPrecios").append('<tr data-expanded="true">' +

                        '<td>' + numeroCotizacion + '</td>' +
                        '<td>' + fechaInicioVigencia + '</td>' +
                        '<td>' + fechaFinVigencia + '</td>' +
                        '<td>' + producto.precioListaList[i].unidad + '</td>' +
                        '<td>' + Number(producto.precioListaList[i].precioNeto).toFixed(cantidadCuatroDecimales) + '</td>' +
                        '<td>' + Number(producto.precioListaList[i].flete).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(producto.precioListaList[i].precioUnitario).toFixed(cantidadCuatroDecimales) + '</td>' +

                        '</tr>');
                }



                //Activar Botón para agregar producto a la grilla
                activarBtnAddProduct();

                //Se calcula el subtotal del producto
                calcularSubtotalProducto();

                $('#precioRegistrado').val($('#precio').val());



            }
        });
    });






    ///////////////////CAMPO PRESENTACIÓN
    $("#unidad").change(function () {

        //0 es precio estandar 
        //1 es precio alternativo
        var esPrecioAlternativo = Number($("#unidad").val());
        $("#esPrecioAlternativo").val(esPrecioAlternativo);

        var precioLista = 0;
        var costoLista = 0;

        if (esPrecioAlternativo == 0) {
            precioLista = Number($("#precioUnitarioSinIGV").val());
            costoLista = Number($("#costoSinIGV").val());
        }
        else {
            precioLista = Number($("#precioUnitarioAlternativoSinIGV").val());
            costoLista = Number($("#costoAlternativoSinIGV").val());
        }

        if ($("input[name=igv]:checked").val() == 1) {
            precioLista = (precioLista + (precioLista * IGV)).toFixed(cantidadDecimales);
            costoLista = (costoLista + (costoLista * IGV)).toFixed(cantidadDecimales);
        }

        $("#precioLista").val(precioLista);
        $("#costoLista").val(costoLista);

        calcularSubtotalProducto();

        var flete = Number($("#flete").val());
        if (flete > 0) {
            alert("Tener en cuenta que al cambiar de unidad se recalcula el monto del flete.")

            var fleteDetalle = costoLista * flete / 100;
            $("#fleteDetalle").val(fleteDetalle.toFixed(cantidadDecimales));
        }
    });

    /*
    $("#porcentajeDescuento").change(function () {
        
        var descuento = Number($("#porcentajeDescuento").val());
        $("#porcentajeDescuento").val(descuento.toFixed(4));

        var precioLista = Number($("#precioLista").val());

        var porcentajeDescuento = parseFloat($("#porcentajeDescuento").val());



        var precio = $("#precio").val();

        var precio = precioLista * (100 - porcentajeDescuento) * 0.01;
        precio = precio.toFixed(cantidadDecimales);

    //    precioLista * (100 - porcentajeDescuento) * 0.01;
     //   precio = precio.toFixed(cantidadDecimales);


        calcularSubtotalProducto();
    });*/


    /////////////////////////CAMPOS PORCENTAJE DESCUENTO y CANTIDAD 
    $("#porcentajeDescuento, #cantidad").change(function () {

        var descuento = Number($("#porcentajeDescuento").val());
        /*      if (descuento > 100) {
                  descuento = 100;
              }*/
        $("#porcentajeDescuento").val(descuento.toFixed(10));
        $("#cantidad").val(Number($("#cantidad").val()).toFixed());
        calcularSubtotalProducto();
    });


    /////////////////////////CAMPO FLETE

    $("#fleteDetalle").change(function () {
        var precioUnitario = Number($('#precio').val()) + Number($('#fleteDetalle').val());
        $('#precioUnitario').val(precioUnitario.toFixed(cantidadDecimales));
        calcularSubtotalProducto();
    });


    /**
    *Función de Cálculo 
    */


    function calcularSubtotalProducto() {
        //Si es 0 quiere decir que es precio standar, si es 1 es el precio alternativo
        var esPrecioAlternativo = Number($("#unidad").val());

        //Se recuperan los valores de precioLista y costoLista
        var precioLista = Number($("#precioLista").val());
        var costoLista = Number($("#costoLista").val());

        //Se identifica si se considera o no las cantidades y se recuperar los valores necesarios
        //para los calculos

        var porcentajeDescuento = parseFloat($("#porcentajeDescuento").val());

        var precio = precioLista * (100 - porcentajeDescuento) * 0.01;
        precio = precio.toFixed(cantidadCuatroDecimales);


        var precioUnitario = Number(precio) + Number($('#fleteDetalle').val());
        $('#precioUnitario').val(precioUnitario.toFixed(cantidadCuatroDecimales));


        var considerarCantidades = $("#considerarCantidades").val();
        //Controles de Cantidad
        if (considerarCantidades == CANT_SOLO_CANTIDADES || considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES) {
            $("#cantidadiv").show();
            $('#subtotaldiv').show();
            var cantidad = parseInt($("#cantidad").val());
            //se calcula el subtotal con el precioUnitario que ya incluye el flete
            var subTotal = precioUnitario * cantidad;
            $("#subtotal").val(subTotal.toFixed(cantidadDecimales));
        }
        else {
            $("#cantidadiv").hide();
            $('#subtotaldiv').hide();
            $("#subtotal").val(0);
            $('#cantidad').val(0);
        }

        //Controles de Observaciones
        $('#observacionProducto').val("");
        if (considerarCantidades == CANT_SOLO_OBSERVACIONES || considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES) {

            $('#observacionProductoDiv').show();
        }
        else {
            $('#observacionProductoDiv').hide();
        }

        $("#precio").val(precio);

        //Se calcula margen
        margen = (1 - (Number($("#costoLista").val()) / Number(precio))) * 100;
        $("#margen").val(margen.toFixed(cantidadDecimales));

    };




    /////////EVENTO CUANDO SE ABRE CALCULADORA DE DESCUENTO
    $('#modalCalculadora').on('shown.bs.modal', function () {
        var modalAgregarProductoIsShown = ($("#modalAgregarProducto").data('bs.modal') || { isShown: false }).isShown;
        if (modalAgregarProductoIsShown) {
            //El precio se obtiene de la pantalla de agregar producto
            $('#nuevoPrecio').val($('#precio').val());
            $('#nuevoDescuento').val($('#porcentajeDescuento').val());
        }
        else {
            var idproducto = $('#idProducto').val();
            var precio = $("." + idproducto + ".detprecio").html();
            var porcentajedescuento = $("." + idproducto + ".detinporcentajedescuento").val();
            //El precio se obtiene del elemento de la grilla
            $('#nuevoPrecio').val(precio);
            $('#nuevoDescuento').val(porcentajedescuento);
        }
        $('#nuevoPrecio').focus();
    })




    //////CONTROL DE BOTONES PARA AGREGAR PRODUCTO A LA GRILLA

    function activarBtnAddProduct() {
        $('#btnAddProduct').removeAttr('disabled');
        $('#btnCalcularDescuento').removeAttr('disabled');
        $('#btnMostrarPrecios').removeAttr('disabled');


    }

    function desactivarBtnAddProduct() {
        $("#btnAddProduct").attr('disabled', 'disabled');
        $('#btnCalcularDescuento').attr('disabled', 'disabled');
        $('#btnMostrarPrecios').attr('disabled', 'disabled');
    }

    $(document).on('click', "button.btnMostrarPrecios", function () {

        var idProducto = event.target.getAttribute("class").split(" ")[0];
        var idCliente = $("#idCliente").val();
        if (idCliente.trim() == "") {
            alert("Debe seleccionar un cliente.");
            $('#idCliente').trigger('chosen:activate');
            return false;
        }
        $.ajax({
            url: "/Precio/GetPreciosRegistradosVenta",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: idProducto,
                idCliente: idCliente
            },
            success: function (producto) {

                $("#verProducto").html(producto.nombre);

                var precioListaList = producto.precioLista;

                // var producto = $.parseJSON(respuesta);
                $("#tableMostrarPrecios > tbody").empty();

                FooTable.init('#tableMostrarPrecios');
                for (var i = 0; i < precioListaList.length; i++) {
                    var fechaInicioVigencia = precioListaList[i].fechaInicioVigencia;
                    var fechaFinVigencia = precioListaList[i].fechaFinVigencia;

                    if (fechaInicioVigencia == null)
                        fechaInicioVigencia = "No Definida";
                    else
                        fechaInicioVigencia = invertirFormatoFecha(precioListaList[i].fechaInicioVigencia.substr(0, 10));

                    if (fechaFinVigencia == null)
                        fechaFinVigencia = "No Definida";
                    else
                        fechaFinVigencia = invertirFormatoFecha(precioListaList[i].fechaFinVigencia.substr(0, 10));

                    var numeroCotizacion = precioListaList[i].numeroCotizacion;
                    if (numeroCotizacion == null)
                        numeroCotizacion = "No Identificado";

                    $("#tableMostrarPrecios").append('<tr data-expanded="true">' +

                        '<td>' + numeroCotizacion + '</td>' +
                        '<td>' + fechaInicioVigencia + '</td>' +
                        '<td>' + fechaFinVigencia + '</td>' +
                        '<td>' + precioListaList[i].unidad + '</td>' +
                        '<td>' + Number(precioListaList[i].precioNeto).toFixed(cantidadCuatroDecimales) + '</td>' +
                        '<td>' + Number(precioListaList[i].flete).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(precioListaList[i].precioUnitario).toFixed(cantidadCuatroDecimales) + '</td>' +

                        '</tr>');
                }
            }
        });
        $("#modalMostrarPrecios").modal();

    });



    $("#considerarDescontinuados").change(function () {
        var considerarDescontinuados = $('#considerarDescontinuados').prop('checked');
        $.ajax({
            url: "/Pedido/updateConsiderarDescontinuados",
            type: 'POST',
            data: {
                considerarDescontinuados: considerarDescontinuados
            },
            success: function () {
            }
        });
    });


    $("#btnAddProduct").click(function () {
        //Se desactiva el boton mientras se agrega el producto
        desactivarBtnAddProduct();
        var cantidad = parseInt($("#cantidad").val());
        var porcentajeDescuento = parseFloat($("#porcentajeDescuento").val());
        var precio = $("#precio").val();
        var precioLista = $("#precioLista").val();
        var costoLista = $("#costoLista").val();
        var esPrecioAlternativo = Number($("#unidad").val());
        var subtotal = $("#subtotal").val();
        var incluidoIGV = $("input[name=igv]:checked").val();
        var proveedor = $("#proveedor").val();
        var flete = Number($("#fleteDetalle").val());
        var observacion = $("#observacionProducto").val();
        var costo = $("#costoLista").val();


        $.ajax({
            url: "/Pedido/AddProducto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                cantidad: cantidad,
                porcentajeDescuento: porcentajeDescuento,
                precio: precio,
                costo: costo,
                esPrecioAlternativo: esPrecioAlternativo,
                flete: flete,
                subtotal: subtotal,
                observacion: observacion
            },
            success: function (detalle) {

                /*   var esRecotizacion = "";
                   if ($("#esRecotizacion").val() == "1") {
                       esRecotizacion = '<td class="' + detalle.idProducto + ' detprecioNetoAnterior" style="text-align:right; color: #B9371B">0.00</td>' +
                           '<td class="' + detalle.idProducto + ' detvarprecioNetoAnterior" style="text-align:right; color: #B9371B">0.0 %</td>' +
                           '<td class="' + detalle.idProducto + ' detvarCosto" style="text-align:right; color: #B9371B">0.0 %</td>' +
                           '<td class="' + detalle.idProducto + ' detcostoAnterior" style="text-align:right; color: #B9371B">0.0</td>';
                   }*/

                var observacionesEnDescripcion = "<br /><span class='" + detalle.idProducto + " detproductoObservacion'  style='color: darkred'>" + detalle.observacion + "</span>";


                var precios = "";

                if (detalle.precioUnitarioRegistrado == 0) {

                    if (detalle.precioUnitario >= Number(precioLista) - Number(VARIACION_PRECIO_ITEM_PEDIDO)
                        && detalle.precioUnitario <= Number(precioLista) + Number(VARIACION_PRECIO_ITEM_PEDIDO)) {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right">' + detalle.precioUnitario + '</td>';

                    }
                    else {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right; color: #B9371B; font-weight:bold">' + detalle.precioUnitario + '</td>';

                    }


                }
                else {


                    if (Number(detalle.precioUnitario) >= (Number(detalle.precioUnitarioRegistrado) - Number(VARIACION_PRECIO_ITEM_PEDIDO))
                        && Number(detalle.precioUnitario) <= (Number(detalle.precioUnitarioRegistrado) + Number(VARIACION_PRECIO_ITEM_PEDIDO))) {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right">' + detalle.precioUnitario + '</td>';

                    }
                    else {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right; color: #B9371B; font-weight:bold">' + detalle.precioUnitario + '</td>';

                    }
                }


                $('#tableDetallePedido tbody tr.footable-empty').remove();
                $("#tableDetallePedido tbody").append('<tr data-expanded="true">' +
                    '<td>' + detalle.idProducto + '</td>' +
                    '<td>' + esPrecioAlternativo + '</td>' +

                    '<td>' + proveedor + '</td>' +
                    '<td>' + detalle.codigoProducto + '</td>' +
                    '<td>' + detalle.nombreProducto + observacionesEnDescripcion + '</td>' +
                    '<td>' + detalle.unidad + '</td>' +
                    '<td class="column-img"><img class="table-product-img" src="' + $("#imgProducto").attr("src") + '"></td>' +
                    '<td class="' + detalle.idProducto + ' detprecioLista" style="text-align:right">' + precioLista + '</td>' +
                    '<td class="' + detalle.idProducto + ' detporcentajedescuento" style="text-align:right">' + porcentajeDescuento.toFixed(10) + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detporcentajedescuentoMostrar" style="width:75px; text-align:right;">' + porcentajeDescuento.toFixed(1) + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detprecio" style="text-align:right">' + precio + '</td>' +
                    '<td class="' + detalle.idProducto + ' detcostoLista">' + costoLista + '</td>' +
                    '<td class="' + detalle.idProducto + ' detmargen" style="width:70px; text-align:right; ">' + detalle.margen + ' %</td>' +

                    '<td class="' + detalle.idProducto + ' detflete" style="text-align:right">' + flete.toFixed(2) + '</td>' +
                    precios +
                    '<td class="' + detalle.idProducto + ' detprecioUnitarioRegistrado" style="text-align:right">' + detalle.precioUnitarioRegistrado + '</td>' +
                    '<td class="' + detalle.idProducto + ' detcantidad" style="text-align:right">' + cantidad + '</td>' +
                    '<td class="' + detalle.idProducto + ' detsubtotal" style="text-align:right">' + subtotal + '</td>' +
                    '<td class="' + detalle.idProducto + ' detobservacion" style="text-align:left">' + observacion + '</td>' +
                    '<td class="' + detalle.idProducto + ' detbtnMostrarPrecios"> <button  type="button" class="' + detalle.idProducto + ' btnMostrarPrecios btn btn-primary bouton-image botonPrecios"></button></td>' +


                    //   esRecotizacion +

                    '<td class="' + detalle.idProducto + ' detordenamiento"></td>' +
                    '</tr > ');

                $('#tableDetallePedido thead tr th.footable-editing').remove();
                $('#tableDetallePedido tbody tr td.footable-editing').remove();


                $('#montoIGV').html(detalle.igv);
                $('#montoSubTotal').html(detalle.subTotal);
                ///var flete = Number($("#flete").val());
                $('#montoTotal').html(detalle.total);
                $("#total").val(detalle.total);
                var total = Number($("#total").val())
                $('#montoFlete').html((total * flete / 100).toFixed(cantidadDecimales));
                $('#montoTotalMasFlete').html((total + (total * flete / 100)).toFixed(cantidadDecimales));

                cargarTablaDetalle();
                // $('#tablefoottable').footable();
                $('#btnCancelAddProduct').click();




            }, error: function (detalle) {

                $("#resultadoAgregarProducto").html("Producto ya se encuentra en el detalle del pedido.");

                // alert($("#resultadoAgregarProducto").html(detalle.responseText).closest("title"));

            }


        });

    });












    /**
    * INTERFACE PARA CALCULO DE DESCUENTO
    */
    $('#modalCalculadora').on('show', function () {
        $('#modalAgregarProducto').css('opacity', .5);
        $('#modalAgregarProducto').unbind();
    });

    $('#modalCalculadora').on('hidden', function () {
        $('#modalAgregarProducto').css('opacity', 1);
        $('#modalAgregarProducto').removeData("modal").modal({});
    });


    $("#nuevoPrecio").change(function () {
        var incluidoIGV = $("input[name=igv]:checked").val();
        var nuevoPrecioModificado = Number($('#nuevoPrecio').val());
        var nuevoPrecioInicial = 0;
        var modalAgregarProductoIsShown = ($("#modalAgregarProducto").data('bs.modal') || { isShown: false }).isShown;

        //En caso el calculo se realice al momento de agregar un producto
        if (modalAgregarProductoIsShown) {

            var esPrecioAlternativo = Number($("#unidad").val());
            //Si es el precio estandar
            nuevoPrecioInicial = Number(Number($("#precioUnitarioSinIGV").val()).toFixed(cantidadCuatroDecimales));

            //Si NO es el precio estandar (si es el precio alternativo)
            if (esPrecioAlternativo == 1) {
                var nuevoPrecioInicial = Number(Number($("#precioUnitarioAlternativoSinIGV").val()).toFixed(cantidadCuatroDecimales));
            }
        }
        //En caso el calculo se realice al momento de editar un producto en la grilla
        else {
            //El precio inicial se obtiene del precio lista
            var idproducto = $('#idProducto').val();
            var nuevoPrecioInicial = $("." + idproducto + ".detprecioLista").html();
        }

        var nuevoDescuento = 100 - (nuevoPrecioModificado * 100 / nuevoPrecioInicial);
        $('#nuevoPrecio').val(nuevoPrecioModificado.toFixed(cantidadCuatroDecimales));
        $('#nuevoDescuento').val(nuevoDescuento.toFixed(10));
    });


    $("#btnSaveDescuento").click(function () {

        var modalAgregarProductoIsShown = ($("#modalAgregarProducto").data('bs.modal') || { isShown: false }).isShown;
        if (modalAgregarProductoIsShown) {
            $("#porcentajeDescuento").val($("#nuevoDescuento").val());
            //Revisar si se puede comentar

            $("#nuevoPrecio").val($("#precio").val());
            calcularSubtotalProducto();

        }
        else {
            //REVISAR CALCULO DE MARGEN Y PRECIO UNITARIO

            var idproducto = $('#idProducto').val();

            //Se recupera el precio calculado
            var precio = Number($("#nuevoPrecio").val());
            //Se asigna el precio calculculado en la columna precio
            $("." + idproducto + ".detprecio").text(precio.toFixed(cantidadDecimales));
            //Se asigna el descuento en el campo descuento
            $("." + idproducto + ".detinporcentajedescuento").val($("#nuevoDescuento").val());

            calcularSubtotalGrilla(idproducto);

        }

        $('#btnCancelCalculadora').click();

    });








    ////////////GENERAR PLANTILLA DE COTIZACIÓN



    $("#btnAgregarProductosDesdePreciosRegistrados").click(function () {

        var idCiudad = $("#idCiudad").val();
        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar una ciudad previamente.");
            $("#idCiudad").focus();
            $("#btnCancelarObtenerProductos").click();
            return false;
        }
        var idCliente = $("#idCliente").val();
        if (idCliente.trim() == "") {
            alert("Debe seleccionar un cliente.");
            $('#idCliente').trigger('chosen:activate');
            $("#btnCancelarObtenerProductos").click();
            return false;
        }


        if (verificarSiExisteDetalle()) {
            alert("No deben existir productos agregados al pedido.");
            return false;
        }

    });




    $("#btnObtenerProductos").click(function () {
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val();
        var fecha = $("#fechaPrecios").val();
        var familia = $("#familiaBusquedaPrecios").val();
        var proveedor = $("#proveedorBusquedaPrecios").val();


        $.ajax({
            url: "/Pedido/obtenerProductosAPartirdePreciosRegistrados",
            data: {
                idCliente: idCliente,
                idCiudad: idCiudad,
                fecha: fecha,
                familia: familia,
                proveedor: proveedor
            },
            type: 'POST',
            error: function () {

                alert("Ocurrió un error al armar el detalle del pedido a partir de los precios registrados.");
                //window.location = '/Pedido/Cotizador';
            },
            success: function () {
                window.location = '/Pedido/Pedir';
            }
        });

    });







    ////////CREAR/EDITAR COTIZACIÓN


    function validarIngresoDatosObligatoriosPedido() {
        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar una ciudad previamente.");
            $("#idCiudad").focus();
            return false;
        }
        var tipoPedido = $("#pedido_tipoPedido").val();
        //Si el tipo de pedido es traslado interno (84->'T')
        if (tipoPedido == "84") {
            if ($("#idCiudadASolicitar").val() == "" || $("#idCiudadASolicitar").val() == null) {
                alert("Debe seleccionar a que ciudad se solicita el traslado interno.");
                $("#idCiudadAsolicitar").focus();
                return false;
            }
        }


        if ($("#idCliente").val().trim() == "") {
            alert("Debe seleccionar un cliente.");
            $('#idCliente').trigger('chosen:activate');
            return false;
        }

        /*     if ($("#pedido_numeroReferenciaCliente").val().trim() == "") {
                 alert('Debe ingresar el número de orden de compra o pedido en el campo "Referencia Doc Cliente".');
                 $('#pedido_numeroReferenciaCliente').focus();
                 return false;
             }*/

        if ($("#ActualDepartamento").val().trim().length == 0) {
            alert('Debe ingresar el departamento.');
            $("#ActualDepartamento").focus();
            return false;
        }
        if ($("#ActualProvincia").val().trim().length == 0) {
            alert('Debe ingresar la provincia.');
            $("#ActualProvincia").focus();
            return false;
        }
        if ($("#ActualDistrito").val().trim().length == 0) {
            alert('Debe ingresar el distrito.');
            $("#ActualDistrito").focus();
            return false;
        }


        if ($("#pedido_direccionEntrega").val().trim() == "") {
            alert("Debe seleccionar la dirección de entrega.");
            $('#pedido_direccionEntrega').focus();
            return false;
        }

        if ($("#pedido_direccionEntrega_descripcion").val().trim() == "") {
            alert("Debe ingresar la dirección de entrega.");
            $('#pedido_direccionEntrega_descripcion').focus();
            return false;
        }

        if ($("#pedido_direccionEntrega_contacto").val().trim() == "") {
            alert("Debe ingresar el contacto de entrega.");
            $('#pedido_direccionEntrega_contacto').focus();
            return false;
        }

        if ($("#pedido_direccionEntrega_telefono").val().trim() == "") {
            alert("Debe ingresar una el telefono del contacto de entrega.");
            $('#pedido_direccionEntrega_telefono').focus();
            return false;
        }
        /*
        if (!$("#documentoVenta_correoEnvio").val().match(/^[a-zA-Z0-9\._-]+@[a-zA-Z0-9-]{2,}[.][a-zA-Z]{2,4}$/)) {
            alert("Debe ingresar un correo de envío válido.");
            $("#documentoVenta_correoEnvio").focus();
            return false;
        }*/

        var fechaSolicitud = $("#fechaSolicitud").val();
        if (fechaSolicitud.trim() == "") {
            alert("Debe ingresar la fecha de la solicitud.");
            $("#fechaSolicitud").focus();
            return false;
        }

        var horaSolicitud = $("#horaSolicitud").val();
        if (horaSolicitud == null || horaSolicitud.trim() == "") {
            alert("Debe ingresar la hora de la solicitud.");
            $("#horaSolicitud").focus();
            return false;
        }


        var fechaEntregaDesde = $("#pedido_fechaEntregaDesde").val();
        if (fechaEntregaDesde.trim() == "") {
            alert("Debe ingresar la fecha desde cuando se puede realizar la entrega .");
            $("#pedido_fechaEntregaDesde").focus();
            return false;
        }

        var fechaEntregaHasta = $("#pedido_fechaEntregaHasta").val();
        if (fechaEntregaHasta.trim() == "") {
            alert("Debe ingresar la fecha hasta cuando se puede realizar la entrega .");
            $("#pedido_fechaEntregaHasta").focus();
            return false;
        }


        //la fecha máxima de entrega no puede ser inferior a la fecha de entrega
        if (convertirFechaNumero(fechaEntregaHasta) < convertirFechaNumero(fechaEntregaDesde)) {
            alert("La fecha entrega hasta debe ser mayor o igual a la fecha de entrega desde.");
            $("#fechaEntregaHasta").focus();
            return false;
        }



        if ($("#pedido_contactoPedido").val().trim() == "") {
            alert("Debe ingresar el nombre de la persona que realizó la solicitud.");
            $('#pedido_contactoPedido').focus();
            return false;
        }



        if ($("#pedido_telefonoContactoPedido").val().trim() == "" && $("#pedido_correoContactoPedido").val().trim() == "") {
            alert("Debe ingresar un telefono y/o correo de contacto de entrega.");
            $('#pedido_telefonoContactoPedido').focus();
            return false;
        }

        /*
        if ($("#pedido_correoContactoPedido").val().trim() != "" && !$("#pedido_correoContactoPedido").val().match(/^[a-zA-Z0-9\._-]+@[a-zA-Z0-9-]{2,}[.][a-zA-Z]{2,4}$/)) {
            alert("Debe ingresar un correo válido.");
            $("#pedido_correoContactoPedido").focus();
            return false;
        }


        */
        var contador = 0;
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {
            contador++;
        });

        if (contador == 0) {
            alert("Debe ingresar el detalle del pedido.");
            return false;
        }

        return true;
    }

    function crearPedido(continuarLuego) {
        if (!validarIngresoDatosObligatoriosPedido())
            return false;

        $('body').loadingModal({
            text: 'Creando Pedido...'
        });
        $.ajax({
            url: "/Pedido/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                continuarLuego: continuarLuego
            },
            error: function (detalle) {
                $('body').loadingModal('hide')
                alert("Se generó un error al intentar finalizar la edición del pedido. Si estuvo actualizando, vuelva a buscar el pedido, es posible que este siendo modificado por otro usuario.");
            },
            success: function (resultado) {
                $('body').loadingModal('hide')
                $("#pedido_numeroPedido").val(resultado.numeroPedido);
                $("#idPedido").val(resultado.idPedido);

                if (resultado.estado == ESTADO_INGRESADO) {
                    alert("El pedido número " + resultado.numeroPedido + " fue ingresado correctamente.");
                    window.location = '/Pedido/Index';


                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    alert("El pedido número " + resultado.numeroPedido + " fue ingresado correctamente, sin embargo requiere APROBACIÓN")
                    $("#comentarioPendienteIngreso").val(resultado.observacion);
                    $("#modalComentarioPendienteIngreso").modal('show');
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    ConfirmDialog("El pedido número " + resultado.numeroPedido + " fue guardado correctamente. ¿Desea continuar editando ahora?", null, '/Pedido/CancelarCreacionPedido');
                }
                else {
                    alert("El pedido ha tenido problemas para ser procesado; Contacte con el Administrador.");
                    window.location = '/Pedido/Index';
                }

            }
        });
    }

    function editarVenta() {

        $('body').loadingModal({
            text: 'Editando Venta...'
        });
        $.ajax({
            url: "/Venta/Update",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide')
                alert("Se generó un error al intentar finalizar la edición de la venta. Si estuvo actualizando, vuelva a buscar el pedido, es posible que este siendo modificado por otro usuario.");
            },
            success: function (resultado) {
                $('body').loadingModal('hide')
                $("#pedido_numeroPedido").val(resultado.numeroPedido);
                $("#idPedido").val(resultado.idPedido);
                alert("Se modificó la venta correctamente.");
                /*
                if (resultado.estado == ESTADO_INGRESADO) {
                    alert("El pedido número " + resultado.numeroPedido + " fue editado correctamente.");
                    window.location = '/Pedido/Index';
                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    alert("El pedido número " + resultado.numeroPedido + " fue editado correctamente, sin embargo requiere APROBACIÓN")
                    $("#comentarioPendienteIngreso").val(resultado.observacion);
                    $("#modalComentarioPendienteIngreso").modal('show');
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    ConfirmDialog("El pedido número " + resultado.numeroPedido + " fue guardado correctamente. ¿Desea continuar editando ahora?", null, '/Pedido/CancelarCreacionPedido');
                }
                else {
                    alert("El pedido ha tenido problemas para ser procesado; Contacte con el Administrador.");
                    window.location = '/Pedido/Index';
                }*/
                // window.location = '/Pedido/Index';

                window.close();
            }
        });
    }

    $("#btnCancelarVenta").click(function () {
        window.close();
    });



    $("#btnCancelarComentario").click(function () {
        window.location = '/Pedido/CancelarCreacionPedido';
    });

    $("#btnAceptarComentario").click(function () {
        var codigoPedido = $("#pedido_numeroPedido").val();
        var idPedido = $("#idPedido").val();
        var observacion = $("#comentarioPendienteIngreso").val();
        $.ajax({
            url: "/Pedido/updateEstadoPedido",
            data: {
                idPedido: idPedido,
                estado: ESTADO_PENDIENTE_APROBACION,
                observacion: observacion
            },
            type: 'POST',
            error: function () {
                alert("Ocurrió un problema al intentar agregar un comentario al pedido.");
                $("#btnCancelarComentario").click();
            },
            success: function () {
                alert("El comentario del estado del pedido número: " + codigoPedido + " se cambió correctamente.");;
                $("#btnCancelarComentario").click();
            }
        });

    });
    /*
        $("#btnAceptarComentarioCrediticio").click(function () {
            var codigoPedido = $("#pedido_numeroPedido").val();
            var observacion = $("#comentarioPendienteIngreso").val();
            $.ajax({
                url: "/Cotizacion/updateEstadoPedidoCrediticio",
                data: {
                    codigo: codigoPedido,
                    estado: ESTADO_PENDIENTE_APROBACION,
                    observacion: observacion
                },
                type: 'POST',
                error: function () {
                    alert("Ocurrió un problema al intentar agregar un comentario al pedido.")
                    $("#btnCancelarComentario").click();
                },
                success: function () {
                    alert("El comentario del estado del pedido número: " + codigoPedido + " se cambió correctamente.");
                    $("#btnCancelarComentario").click();
                }
            });

        });*/


    $("#btnFinalizarEdicionVenta").click(function () {
        editarVenta();
    });


    $("#btnFinalizarCreacionPedido").click(function () {
        crearPedido(0);
    });





    $("#btnContinuarEditandoLuego").click(function () {
        if ($("#pedido_numeroPedido") == "") {
            crearPedido(1);
        }
        else {
            editarPedido(1);
        }
    });















    $("#btnCopiar").click(function () {
        /* Get the text field */
        var copyText = document.getElementById("myInput");

        /* Select the text field */
        copyText.select();

        /* Copy the text inside the text field */
        document.execCommand("Copy");

        /* Alert the copied text */
        //    alert("Copied the text: " + copyText.value);
    });





    ////////VER COTIZACIÓN  --- CAMBIO DE ESTADO

    function invertirFormatoFecha(fecha) {
        var fechaInvertida = fecha.split("-");
        fecha = fechaInvertida[2] + "/" + fechaInvertida[1] + "/" + fechaInvertida[0];
        return fecha
    }

    function convertirFechaNumero(fecha) {
        var fechaInvertida = fecha.split("/");
        fecha = fechaInvertida[2] + fechaInvertida[1] + fechaInvertida[0];
        return Number(fecha)
    }

    function convertirHoraToNumero(hora) {
        var hora = hora.split(":");
        hora = hora[0] + "." + hora[1];
        return Number(hora)
    }

    function sumarHoras(hora, cantidad) {
        var hora = hora.split(":");

        var horaNumero = Number(hora[0]) + cantidad;


        if (horaNumero < 0)
            hora = "00:00";
        else if (horaNumero > 23)
            hora = "23:59";

        if (horaNumero < 10) {
            hora = "0" + horaNumero + ":" + hora[1];
            // hora = horaNumero + ":" + hora[1];
        }
        else {
            hora = horaNumero + ":" + hora[1];
        }

        return hora
    }



    /*VER PEDIDO*/
    $(document).on('click', "button.btnVerPedido", function () {

        activarBotonesVer();
        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idPedido = arrrayClass[0];
        var numeroPedido = arrrayClass[1];
        //  $("#tableDetalleCotizacion > tbody").empty();


        $.ajax({
            url: "/Pedido/Show",
            data: {
                idPedido: idPedido
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) { alert("Ocurrió un problema al obtener el detalle del Pedido N° " + numeroPedido + "."); },
            success: function (resultado) {
                //var cotizacion = $.parseJSON(respuesta);
                var pedido = resultado.pedido;
                var usuario = resultado.usuario;


                $("#fechaEntregaDesdeProgramacion").val(invertirFormatoFecha(pedido.fechaEntregaDesde.substr(0, 10)));
                $("#fechaEntregaHastaProgramacion").val(invertirFormatoFecha(pedido.fechaEntregaHasta.substr(0, 10)));
                $("#fechaProgramaciontmp").val(invertirFormatoFecha(pedido.fechaEntregaDesde.substr(0, 10)));
                //Important

                $("#idPedido").val(pedido.idPedido);

                $("#verNumero").html(pedido.numeroPedidoString);
                $("#verNumeroGrupo").html(pedido.numeroGrupoPedidoString);
                $("#verCotizacionCodigo").html(pedido.cotizacion.numeroCotizacionString);
                $("#verTipoPedido").html(pedido.tiposPedidoString);

                if (pedido.tipoPedido == "84") {
                    $("#divReferenciaCliente").hide();
                    $("#divCiudadSolicitante").show();
                    $("#verCiudadSolicitante").html(pedido.cliente.ciudad.nombre);
                }
                else {
                    $("#divReferenciaCliente").show();
                    $("#divCiudadSolicitante").hide();
                }


                $("#verFechaHorarioEntrega").html(pedido.fechaHorarioEntrega);

                $("#verCiudad").html(pedido.ciudad.nombre);
                $("#verCliente").html(pedido.cliente.razonSocial);
                $("#verNumeroReferenciaCliente").html(pedido.numeroReferenciaCliente);
                $("#verDireccionEntrega").html(pedido.direccionEntrega.descripcion);
                $("#verTelefonoContactoEntrega").html(pedido.direccionEntrega.telefono);
                $("#verContactoEntrega").html(pedido.direccionEntrega.contacto);

                $("#verUbigeoEntrega").html(pedido.ubigeoEntrega.ToString);

                $("#verContactoPedido").html(pedido.contactoPedido);
                $("#verTelefonoCorreoContactoPedido").html(pedido.telefonoCorreoContactoPedido);




                $("#verFechaHoraSolicitud").html(pedido.fechaHoraSolicitud);

                $("#verEstado").html(pedido.seguimientoPedido.estadoString);
                $("#verModificadoPor").html(pedido.seguimientoPedido.usuario.nombre);
                $("#verObservacionEstado").html(pedido.seguimientoPedido.observacion);

                $("#verEstadoCrediticio").html(pedido.seguimientoCrediticioPedido.estadoString);
                $("#verModificadoCrediticioPor").html(pedido.seguimientoCrediticioPedido.usuario.nombre);
                $("#verObservacionEstadoCreiditicio").html(pedido.seguimientoCrediticioPedido.observacion);


                $("#verObservaciones").html(pedido.observaciones);
                $("#verMontoSubTotal").html(pedido.montoSubTotal);
                $("#verMontoIGV").html(pedido.montoIGV);
                $("#verMontoTotal").html(pedido.montoTotal);
                $("#documentoVenta_observaciones").val(pedido.observacionesFactura);







                $("#tableDetallePedido > tbody").empty();

                FooTable.init('#tableDetallePedido');

                $("#formVerGuiasRemision").html("");

                var d = '';
                var lista = pedido.pedidoDetalleList;
                for (var i = 0; i < lista.length; i++) {

                    var observacion = lista[i].observacion == null || lista[i].observacion == 'undefined' ? '' : lista[i].observacion;

                    d += '<tr>' +
                        '<td>' + lista[i].producto.proveedor + '</td>' +
                        '<td>' + lista[i].producto.sku + '</td>' +
                        '<td>' + lista[i].producto.descripcion + '</td>' +
                        '<td>' + lista[i].unidad + '</td>' +
                        '<td class="column-img"><img class="table-product-img" src="data:image/png;base64,' + lista[i].producto.image + '"> </td>' +
                        '<td>' + lista[i].precioLista.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].porcentajeDescuentoMostrar.toFixed(cantidadDecimales) + ' %</td>' +
                        '<td>' + lista[i].precioNeto.toFixed(cantidadCuatroDecimales) + '</td>' +
                        '<td>' + lista[i].margen.toFixed(cantidadDecimales) + ' %</td>' +
                        '<td>' + lista[i].flete.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].precioUnitario.toFixed(cantidadCuatroDecimales) + '</td>' +
                        '<td>' + lista[i].cantidad + '</td>' +
                        '<td>' + lista[i].cantidadPendienteAtencion + '</td>' +
                        '<td>' + lista[i].subTotal.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + observacion + '</td>' +
                        '</tr>';



                }




                $("#verRazonSocialSunat").html(pedido.cliente.razonSocialSunat);
                $("#verRUC").html(pedido.cliente.ruc);
                $("#verDireccionDomicilioLegalSunat").html(pedido.cliente.direccionDomicilioLegalSunat);
                $("#verCodigo").html(pedido.cliente.codigo);

                $("#documentoVenta_observaciones").val(pedido.observacionesFactura);
                $("#verCorreoEnvioFactura").html(pedido.cliente.correoEnvioFactura);




                var guiaRemisionList = pedido.guiaRemisionList;
                for (var j = 0; j < guiaRemisionList.length; j++) {
                    $("#documentoVenta_fechaEmision").val(invertirFormatoFecha(guiaRemisionList[j].fechaEmision.substr(0, 10)));
                    $("#documentoVenta_fechaVencimiento").val(invertirFormatoFecha(guiaRemisionList[j].fechaEmision.substr(0, 10)));
                    $("#documentoVenta_horaEmision").val("18:00");



                    break;
                }


                $("#tipoPago").val(pedido.cliente.tipoPagoFactura);
                calcularFechaVencimiento();
                $("#formaPago").val(pedido.cliente.formaPagoFactura);

                /*
                var guiaRemisionList = pedido.guiaRemisionList;
                for (var j = 0; j < guiaRemisionList.length; j++) {


                    $("#tableDetalleGuia > tbody").empty();

                    FooTable.init('#tableDetalleGuia');

                    var dGuia = '';
                    var documentoDetalleList = guiaRemisionList[j].documentoDetalle;
                    for (var k = 0; k < documentoDetalleList.length; k++) {

                        dGuia += '<tr>' +
                            '<td>' + documentoDetalleList[k].producto.sku + '</td>' +
                            '<td>' + documentoDetalleList[k].cantidad + '</td>' +
                            '<td>' + documentoDetalleList[k].unidad + '</td>' +
                            '<td>' + documentoDetalleList[k].producto.descripcion + '</td>' +
                            '</tr>';
                    }

                    $("#tableDetalleGuia tbody").append(dGuia);

                    var plantilla = $("#plantillaVerGuiasRemision").html();

                    plantilla = plantilla.replace("#serieNumero", guiaRemisionList[j].serieNumeroGuia);
                    plantilla = plantilla.replace("#fechaEmisionGuia", invertirFormatoFecha(guiaRemisionList[j].fechaEmision.substr(0, 10)));
                    


                    plantilla = plantilla.replace("#serieNumeroFactura", guiaRemisionList[j].documentoVenta.serieNumero);
                    if (guiaRemisionList[j].documentoVenta.fechaEmision != null) {
                        plantilla = plantilla.replace("#fechaEmisionFactura", invertirFormatoFecha(guiaRemisionList[j].documentoVenta.fechaEmision.substr(0, 10)));
                    }
                    else
                        plantilla = plantilla.replace("#fechaEmisionFactura", "");
                    

                    plantilla = plantilla.replace("#tableDetalleGuia", "#tableDetalleGuia" + j);


                    $("#formVerGuiasRemision").append(plantilla);
                }
                */





                //  
                // sleep
                $("#tableDetallePedido").append(d);

                if (pedido.seguimientoPedido.estado != ESTADO_PROGRAMADO
                    && pedido.seguimientoPedido.estado != ESTADO_ATENDIDO
                    && pedido.seguimientoPedido.estado != ESTADO_FACTURADO
                    && pedido.seguimientoPedido.estado != ESTADO_ATENDIDO_PARCIALMENTE) {
                    $("#btnEditarPedido").show();
                    if (pedido.seguimientoPedido.estado == ESTADO_EN_EDICION) {
                        $("#btnEditarPedido").html("Continuar Editanto");
                    }
                    else {
                        $("#btnEditarPedido").html("Editar");
                    }
                    $("#btnEditarPedido").show();
                }
                else {
                    $("#btnEditarPedido").hide();
                }


                //APROBAR PEDIDO
                if (
                    (pedido.seguimientoPedido.estado == ESTADO_PENDIENTE_APROBACION ||
                        pedido.seguimientoPedido.estado == ESTADO_DENEGADO) &&
                    (usuario.apruebaPedidos)
                ) {

                    $("#btnAprobarIngresoPedido").show();
                }
                else {
                    $("#btnAprobarIngresoPedido").hide();
                }


                //DENEGAR PEDIDO
                if (pedido.seguimientoPedido.estado == ESTADO_PENDIENTE_APROBACION
                    && usuario.apruebaPedidos) {

                    $("#btnDenegarIngresoPedido").show();
                }
                else {
                    $("#btnDenegarIngresoPedido").hide();
                }

                //LIBERAR PEDIDO
                if (
                    pedido.seguimientoCrediticioPedido.estado == ESTADO_PENDIENTE_LIBERACION ||
                    pedido.seguimientoCrediticioPedido.estado == ESTADO_BLOQUEADO
                ) {

                    $("#btnLiberarPedido").hide();
                }
                else {
                    $("#btnLiberarPedido").hide();
                }

                //BLOQUEAR PEDIDO
                if (
                    (pedido.seguimientoCrediticioPedido.estado == ESTADO_PENDIENTE_LIBERACION ||
                        pedido.seguimientoCrediticioPedido.estado == ESTADO_LIBERADO)
                    && pedido.seguimientoPedido.estado != ESTADO_ATENDIDO
                    && pedido.seguimientoPedido.estado != ESTADO_ATENDIDO_PARCIALMENTE

                ) {

                    $("#btnBloquearPedido").hide();
                }
                else {
                    $("#btnBloquearPedido").hide();
                }


                //PROGRAMAR PEDIDO
                if (pedido.seguimientoPedido.estado == ESTADO_INGRESADO) {

                    $("#btnProgramarPedido").show();
                }
                else {
                    $("#btnProgramarPedido").hide();
                }

                //ATENDER PEDIDO
                if ((pedido.seguimientoPedido.estado == ESTADO_INGRESADO ||
                    pedido.seguimientoPedido.estado == ESTADO_PROGRAMADO ||
                    pedido.seguimientoPedido.estado == ESTADO_ATENDIDO_PARCIALMENTE
                ) &&
                    pedido.seguimientoCrediticioPedido.estado == ESTADO_LIBERADO) {

                    $("#btnAtenderPedido").show();
                }
                else {
                    $("#btnAtenderPedido").hide();
                }

                //CANCELAR PROGRAMACION
                if (pedido.seguimientoPedido.estado == ESTADO_PROGRAMADO &&
                    !pedido.seguimientoPedido.estado == ESTADO_ATENDIDO &&
                    !pedido.seguimientoPedido.estado == ESTADO_ATENDIDO_PARCIALMENTE) {
                    $("#btnCancelarProgramacionPedido").show();
                }
                else {
                    $("#btnCancelarProgramacionPedido").hide();
                }


                //PROGRAMAR PEDIDO
                if ((pedido.seguimientoPedido.estado == ESTADO_ATENDIDO)
                    && (pedido.documentoVenta.numero == "" || pedido.documentoVenta.numero == null)
                ) {
                    $("#btnEditarVenta").show();
                    $("#btnFacturarPedido").show();
                }
                else {
                    $("#btnEditarVenta").hide();
                    $("#btnFacturarPedido").hide();
                }


                if (pedido.seguimientoPedido.estado == ESTADO_ATENDIDO
                    || pedido.seguimientoPedido.estado == ESTADO_ATENDIDO_PARCIALMENTE
                ) {

                    $("#btnVerAtenciones").show();
                }
                else {
                    $("#btnVerAtenciones").hide();
                }

                if (pedido.seguimientoPedido.estado != ESTADO_ATENDIDO
                    && pedido.seguimientoPedido.estado != ESTADO_ATENDIDO_PARCIALMENTE
                    && pedido.seguimientoPedido.estado != ESTADO_ELIMINADO
                    && pedido.seguimientoPedido.estado != ESTADO_FACTURADO
                ) {

                    $("#btnEliminarPedido").show();
                }
                else {
                    $("#btnEliminarPedido").hide();
                }






                /*PDF
                if (
                    (cotizacion.seguimientoCotizacion.estado == ESTADO_APROBADA ||
                        cotizacion.seguimientoCotizacion.estado == ESTADO_ACEPTADA ||
                        cotizacion.seguimientoCotizacion.estado == ESTADO_RECHAZADA
                    )
                ) {

                    $("#btnPDFCotizacion").show();
                }
                else {
                    $("#btnPDFCotizacion").hide();
                }
                */


                $("#modalVerPedido").modal('show');

                //  window.location = '/Pedido/Index';
            }
        });
    });



    function generarPDF() {
        //$("#generarPDF").click(function () {

        var codigo = $("#numero").val();
        if (codigo == "" || codigo == 0) {
            alert("Debe guardar la cotización previamente.");
            return false;
        }

        $.ajax({
            url: "/Pedido/GenerarPDFdesdeIdCotizacion",
            data: {
                codigo: codigo
            },
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al descargar la cotización N° " + codigo + " en formato PDF."); },
            success: function (fileName) {
                //Se descarga el PDF y luego se limpia el formulario
                window.open('/Pedido/DownLoadFile?fileName=' + fileName);
                window.location = '/Pedido/Index';
            }
        });
    }

    $("#btnCancelarPedido").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/Pedido/CancelarCreacionPedido', null)
    })



    function desactivarBotonesVer() {
        $("#btnCancelarVerPedido").attr('disabled', 'disabled');
        $("#btnEditarPedido").attr('disabled', 'disabled');
        $("#btnAprobarIngresoPedido").attr('disabled', 'disabled');
        $("#btnDenegarIngresoPedido").attr('disabled', 'disabled');
        $("#btnProgramarPedido").attr('disabled', 'disabled');
        $("#btnFacturarPedido").attr('disabled', 'disabled');
        $("#btnAtenderPedido").attr('disabled', 'disabled');
        $("#btnCancelarProgramacionPedido").attr('disabled', 'disabled');
        $("#btnLiberarPedido").attr('disabled', 'disabled');
        $("#btnBloquearPedido").attr('disabled', 'disabled');
        $("#btnVerAtenciones").attr('disabled', 'disabled');
        $("#btnEliminarPedido").attr('disabled', 'disabled');

    }

    function activarBotonesVer() {
        $("#btnCancelarVerPedido").removeAttr('disabled');
        $("#btnEditarPedido").removeAttr('disabled');
        $("#btnAprobarIngresoPedido").removeAttr('disabled');
        $("#btnDenegarIngresoPedido").removeAttr('disabled');
        $("#btnAtenderPedido").removeAttr('disabled');
        $("#btnProgramarPedido").removeAttr('disabled');
        $("#btnFacturarPedido").removeAttr('disabled');
        $("#btnCancelarProgramacionPedido").removeAttr('disabled');
        $("#btnLiberarPedido").removeAttr('disabled');
        $("#btnBloquearPedido").removeAttr('disabled');
        $("#btnVerAtenciones").removeAttr('disabled');
        $("#btnEliminarPedido").removeAttr('disabled');
    }


    $("#btnFacturarPedido").click(function () {
        $("#facturacion").show();
        desactivarBotonesVer();
    });

    $("#btnCancelarFacturarPedido").click(function () {

        $("#facturacion").hide();
        activarBotonesVer();
    });






    $("#btnEditarPedido").click(function () {
        desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona
        $.ajax({
            url: "/Pedido/ConsultarSiExistePedido",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/Pedido/iniciarEdicionPedido",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del pedido."); },
                        success: function (fileName) {
                            window.location = '/Pedido/Pedir';
                        }
                    });

                }
                else {
                    if (resultado.numero == 0) {
                        alert('Está creando un nuevo pedido; para continuar por favor diríjase a la página "Crear/Modificar Pedido" y luego haga clic en el botón Cancelar, Finalizar Creación o Guardar y Continuar Editando Luego.');
                    }
                    else {
                        if (resultado.numero == $("#verNumero").html())
                            alert('Ya se encuentra editando el pedido número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Cotización".');
                        else
                            alert('Está editando el pedido número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Pedido" y luego haga clic en el botón Cancelar, Finalizar Edición o Guardar y Continuar Editando Luego.');
                    }
                    activarBotonesVer();
                }
            }
        });
    });



    $("#btnAtenderPedido").click(function () {
        desactivarBotonesVer();
        var idPedido = $("#idPedido").val();

        $.ajax({
            url: "/GuiaRemision/ConsultarSiExisteGuiaRemision",
            type: 'POST',
            async: false,
            success: function (resultado) {
                activarBotonesVer();
                if (resultado == "False") {
                    $('body').loadingModal({
                        text: 'Abriendo Crear Guía Remisión...'
                    });
                    $.ajax({
                        url: "/GuiaRemision/iniciarAtencionDesdePedido",
                        type: 'POST',
                        error: function (detalle) {
                            $('body').loadingModal('hide')
                            alert("Ocurrió un problema al iniciar la atención del pedido.");
                        },
                        success: function (fileName) {
                            $('body').loadingModal('hide')
                            window.location = '/GuiaRemision/Guiar';
                        }
                    });
                }
                else {
                    alert("Existe una Guia de Remisión abierta; por favor vaya a la pantala Guia Remisión, haga clic en cancelar y vuelva a intentarlo.");
                    activarBotonesVer();
                }
            }
        });
    });





    function limpiarComentario() {
        $("#comentarioEstado").val("");
        $("#comentarioEstado").focus();
    }



    $("#btnLiberarPedido").click(function () {

        $("#labelNuevoEstadoCrediticio").html(ESTADO_LIBERADO_STR);
        $("#estadoCrediticioId").val(ESTADO_LIBERADO);
        limpiarComentario();
    });

    $("#btnBloquearPedido").click(function () {

        $("#labelNuevoEstadoCrediticio").html(ESTADO_BLOQUEADO_STR);
        $("#estadoCrediticioId").val(ESTADO_BLOQUEADO);
        limpiarComentario();
    });

    $("#btnAprobarIngresoPedido").click(function () {
        $("#modalAprobacionTitle").html(TITULO_APROBAR_INGRESO);
        $("#labelNuevoEstado").html(ESTADO_INGRESADO_STR);
        $("#estadoId").val(ESTADO_INGRESADO);
        limpiarComentario();
    });

    $("#btnEliminarPedido").click(function () {
        $("#modalAprobacionTitle").html(TITULO_ELIMINAR);
        $("#labelNuevoEstado").html(ESTADO_ELIMINADO_STR);
        $("#estadoId").val(ESTADO_ELIMINADO);
        limpiarComentario();
    });

    $("#btnDenegarIngresoPedido").click(function () {
        $("#modalAprobacionTitle").html(TITULO_DENEGAR_INGRESO);
        $("#labelNuevoEstado").html(ESTADO_DENEGADO_STR);
        $("#estadoId").val(ESTADO_DENEGADO);
        limpiarComentario();
    });



    $("#btnCancelarProgramacionPedido").click(function () {
        $("#modalAprobacionTitle").html(TITULO_CANCELAR_PROGRAMACION);
        $("#labelNuevoEstado").html(ESTADO_INGRESADO_STR);
        $("#estadoId").val(ESTADO_INGRESADO);
        limpiarComentario();
    });





    $('#modalAprobacion').on('shown.bs.modal', function (e) {
        limpiarComentario();
    });




    $("#btnAceptarCambioEstadoCrediticio").click(function () {

        var estado = $("#estadoCrediticioId").val();
        var comentarioEstado = $("#comentarioEstadoCrediticio").val();

        if ($("#labelNuevoEstadoCrediticio").html() == ESTADO_BLOQUEADO_STR) {
            if (comentarioEstado.trim() == "") {
                alert("Cuando Bloquea un pedido debe ingresar un Comentario.");
                return false;
            }
        }
        var codigo = $("#verNumero").html();
        var idPedido = $("#idPedido").val();

        $.ajax({
            url: "/Pedido/updateEstadoPedidoCrediticio",
            data: {
                idPedido: idPedido,
                estado: estado,
                observacion: comentarioEstado
            },
            type: 'POST',
            error: function () {
                alert("Ocurrió un problema al intentar cambiar el estado del pedido.")
                $("#btnCancelarCambioEstadoCrediticio").click();
            },
            success: function () {
                alert("El estado crediticio del pedido número: " + codigo + " se cambió correctamente.");
                location.reload();
            }
        });
    });


    $("#btnAceptarCambioEstado").click(function () {

        var estado = $("#estadoId").val();
        var comentarioEstado = $("#comentarioEstado").val();

        if ($("#labelNuevoEstado").html() == ESTADO_DENEGADO_STR
            || $("#modalAprobacionTitle").html() == TITULO_CANCELAR_PROGRAMACION) {
            if (comentarioEstado.trim() == "") {
                alert("Debe ingresar un Comentario.");
                return false;
            }
        }
        var codigo = $("#verNumero").html();
        var idPedido = $("#idPedido").val();

        $.ajax({
            url: "/Pedido/updateEstadoPedido",
            data: {
                idPedido: idPedido,
                estado: estado,
                observacion: comentarioEstado
            },
            type: 'POST',
            error: function () {
                alert("Ocurrió un problema al intentar cambiar el estado del pedido.")
                $("#btnCancelarCambioEstado").click();
            },
            success: function () {
                alert("El estado del pedido número: " + codigo + " se cambió correctamente.");
                location.reload();
            }
        });
    });





    var ft = null;



    //Mantener en Session cambio de Seleccion de IGV
    $("input[name=igv]").on("click", function () {
        var igv = $("input[name=igv]:checked").val();
        $.ajax({
            url: "/Pedido/updateSeleccionIGV",
            type: 'POST',
            data: {
                igv: igv
            },
            success: function (cantidad) {
                if (cantidad > 0) {
                    location.reload();
                }
            }
        });
    });

    //Mantener en Session cambio de Seleccion de IGV
    $("#considerarCantidades").change(function () {
        var considerarCantidades = $("#considerarCantidades").val();
        $.ajax({
            url: "/Pedido/updateSeleccionConsiderarCantidades",
            type: 'POST',
            data: {
                considerarCantidades: considerarCantidades
            },
            success: function (cantidad) {
                if (cantidad > 0) {
                    location.reload();
                }
            }
        });


    });





    //Mantener en Session cambio de Seleccion de Mostrar Proveedor
    $("input[name=mostrarcodproveedor]").on("click", function () {
        var mostrarcodproveedor = $("input[name=mostrarcodproveedor]:checked").val();
        $.ajax({
            url: "/Pedido/updateMostrarCodigoProveedor",
            type: 'POST',
            data: {
                mostrarcodproveedor: mostrarcodproveedor
            },
            success: function () {
                location.reload();
            }
        });
    });

    //Mantener en Session cambio de Cliente
    $("#cliente").change(function () {

        $.ajax({
            url: "/Pedido/updateCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                cliente: $("#cliente").val()
            },
            success: function () { }
        });
    });




    $("#contacto").change(function () {

        $.ajax({
            url: "/Pedido/updateContacto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                contacto: $("#contacto").val()
            },
            success: function () { }
        });
    });

    $("#flete").change(function () {

        $("#flete").val(Number($("#flete").val()).toFixed(cantidadDecimales))
        var flete = $("#flete").val();
        if (flete > 100) {
            $("#flete").val("100.00");
            flete = 100;
        }

        var total = Number($("#total").val());
        $('#montoFlete').html("Flete: " + SIMBOLO_SOL + " " + (total * flete / 100).toFixed(cantidadDecimales));
        $('#montoTotalMasFlete').html("Total más Flete: " + SIMBOLO_SOL + " " + (total + (total * flete / 100)).toFixed(cantidadDecimales));




        $.ajax({
            url: "/Pedido/updateFlete",
            type: 'POST',
            data: {
                flete: flete
            },
            success: function () {
                location.reload();
            }
        });
    });



    $("#mostrarCosto").change(function () {
        var mostrarCosto = $('#mostrarCosto').prop('checked');
        $.ajax({
            url: "/Pedido/updateMostrarCosto",
            type: 'POST',
            data: {
                mostrarCosto: mostrarCosto
            },
            success: function () {
                location.reload();
            }
        });
    });







    /*####################################################
    EVENTOS DE LA GRILLA
    #####################################################*/


    /**
     * Se definen los eventos de la grilla
     */
    function cargarTablaDetalle() {
        var $modal = $('#tableDetallePedido'),
            $editor = $('#tableDetallePedido'),
            $editorTitle = $('#tableDetallePedido');


        ft = FooTable.init('#tableDetallePedido', {
            editing: {
                enabled: true,
                addRow: function () {
                    ConfirmDialogReload(MENSAJE_CANCELAR_EDICION);
                },
                editRow: function (row) {
                    var values = row.val();
                    var idProducto = values.idProducto;
                    alert(idProducto);
                },
                deleteRow: function (row) {
                    //  if (confirm('¿Esta seguro de eliminar el producto?')) {
                    var values = row.val();
                    var idProducto = values.idProducto;
                    /*
                                                $.ajax({
                                                    url: "/Pedido/DelProducto",
                                                    type: 'POST',
                                                    data: {
                                                        idProducto: idProducto
                                                    },
                                                    success: function (total) {
                                                */
                    row.delete();
                }
            }
        });

        /*.bind({
            'footable_sorted': function (e) {
                /*    var rows = $('#details tbody tr.data');
        
                    rows.each(function () {
                        var personid = $(this).data('row-person');
        
                        var detail = $('#details tbody tr.descriptions[data-detail-person="' + personid + '"]');
                        $(detail).insertAfter($(this));
                    });
                alert("asas");
            }
        });*/
    }
    cargarTablaDetalle();





    //   $('#tablefoottable').footable()

    /* $("#tablefoottable").onSort(function () {
         alert("Asas");
 
         /* onSort
 
     sort.bs.table
 
         }
     );*/



    function mostrarFlechasOrdenamiento() {
        $(".ordenar, .detordenamiento").attr('style', 'display: table-cell');

        // $(".detordenamiento.media").html('<div class="updown"><div class="up"></div> <div class="down"></div></div>');

        //Se identifica cuantas filas existen
        var contador = 0;
        var $j_object = $("td.detordenamiento");
        $.each($j_object, function (key, value) {
            contador++;
        });

        if (contador == 1) {
            $(".detordenamiento").html('');
        }
        else if (contador > 1) {
            var contador2 = 0;
            var $j_object = $("td.detordenamiento");
            $.each($j_object, function (key, value) {
                contador2++;
                if (contador2 == 1) {
                    value.innerHTML = '<div class="updown"><div class="down"></div></div>';
                    value.setAttribute("posicion", "primera");
                }
                else if (contador2 == contador) {
                    value.innerHTML = '<div class="updown"><div class="up"></div></div>';
                    value.setAttribute("posicion", "ultima");
                }
                else {
                    value.innerHTML = '<div class="updown"><div class="up"></div> <div class="down"></div></div>';
                    value.setAttribute("posicion", "media");
                }
            });
        }






    }


    $(document).on('click', ".up,.down", function () {

        var codigo = event.target.parentElement.parentElement.getAttribute("class").split(" ")[0];
        var row = $(this).parents("tr:first");

        //Mover hacia arriba
        if ($(this).is(".up")) {

            var posicionPrevia = row.prev().find('td.detordenamiento').attr("posicion");
            var htmlPrevio = row.prev().find('td.detordenamiento').html();

            var posicionActual = row.find('td.detordenamiento').attr("posicion");
            var htmlActual = row.find('td.detordenamiento').html();

            //intercambio de posicion
            row.prev().find('td.detordenamiento').attr("posicion", posicionActual);
            row.find('td.detordenamiento').attr("posicion", posicionPrevia);
            //intercambio de controles
            row.prev().find('td.detordenamiento').html(htmlActual);
            row.find('td.detordenamiento').html(htmlPrevio);

            row.insertBefore(row.prev());

        } else {
            var posicionPrevia = row.next().find('td.detordenamiento').attr("posicion");
            var htmlPrevio = row.next().find('td.detordenamiento').html();

            var posicionActual = row.find('td.detordenamiento').attr("posicion");
            var htmlActual = row.find('td.detordenamiento').html();

            //intercambio de posicion
            row.next().find('td.detordenamiento').attr("posicion", posicionActual);
            row.find('td.detordenamiento').attr("posicion", posicionPrevia);
            //intercambio de controles
            row.next().find('td.detordenamiento').html(htmlActual);
            row.find('td.detordenamiento').html(htmlPrevio);

            row.insertAfter(row.next());
        }
    });













    /*Evento que se dispara cuando se hace clic en el boton EDITAR en la edición de la grilla*/
    $(document).on('click', "button.footable-show", function () {

        //Cambiar estilos a los botones
        $("button.footable-add").attr("class", "btn btn-default footable-add");
        $("button.footable-hide").attr("class", "btn btn-primary footable-hide");


        //Se deshabilitan controles que recargan la página o que interfieren con la edición del detalle
        $("#considerarCantidades").attr('disabled', 'disabled');
        $("input[name=igv]").attr('disabled', 'disabled');
        $("#flete").attr('disabled', 'disabled');
        $("#btnOpenAgregarProducto").attr('disabled', 'disabled');

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
            var cantidad = value.innerText.trim();
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detincantidad form-control' value='" + cantidad + "' type='number'/>";
        });

        var considerarCantidades = $("#considerarCantidades").val();
        if (considerarCantidades == CANT_SOLO_OBSERVACIONES) {
            /*Se agrega control input en columna observacion*/
            var $j_object = $("td.detobservacion");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var observacion = value.innerText.trim();
                value.innerHTML = "<textarea class='" + arrId[0] + " detobservacionarea form-control'/>" + observacion + "</textarea>";
            });
        }
        else if (considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES) {
            var $j_object = $("span.detproductoObservacion");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var observacion = value.innerText.trim();
                value.innerHTML = "<textarea class='" + arrId[0] + " detobservacionarea form-control'/>" + observacion + "</textarea>";
            });

        }

        //   @cotizacionDetalle.producto.idProducto detproductoObservacion"




        /*Se agrega control input en columna porcentaje descuento*/
        var $j_object1 = $("td.detporcentajedescuento");
        $.each($j_object1, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var porcentajedescuento = value.innerText.trim();
            porcentajedescuento = porcentajedescuento.replace("%", "").trim();
            $(".detporcentajedescuentoMostrar." + arrId[0]).html("<div style='width: 150px' ><div style='float:left' ><input style='width: 100px' class='" + arrId[0] + " detinporcentajedescuento form-control' value='" + porcentajedescuento + "' type='number'/></div><div > <button type='button' class='" + arrId[0] + " btnCalcularDescuento btn btn-primary bouton-image monBouton' data-toggle='modal' data-target='#modalCalculadora' ></button ></div></div>");

        });


        var $j_objectFlete = $("td.detflete");
        $.each($j_objectFlete, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var flete = value.innerText.trim();
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detinflete form-control' value='" + flete + "' type='number'/>";
        });



    });






    /*Evento que se dispara cuando se hace clic en FINALIZAR en la edición de la grilla*/
    $(document).on('click', "button.footable-hide", function () {

        //Se habilitan controles
        $("#considerarCantidades").removeAttr('disabled');
        $("input[name=igv]").removeAttr('disabled');
        $("#flete").removeAttr('disabled');
        $("#btnOpenAgregarProducto").removeAttr('disabled');
        $("input[name=mostrarcodproveedor]").removeAttr('disabled');

        //  $(".ordenar").attr('data-visible', 'false');
        //       $(".updown").hide();
        //       $(".ordenar, .detordenamiento").width('0px');
        // FooTable.init();
        //  <th class="ordenar" data-name="ordenar" data-visible="true"></th>

        var json = "[ ";
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {
            var arrId = value.getAttribute("class").split(" ");

            /*Se elimina control input en columna cantidad*/
            var cantidad = $("." + arrId[0] + ".detincantidad").val();
            value.innerText = cantidad;

            /*Se elimina control input en columna porcentaje descuento*/
            var porcentajeDescuento = $("." + arrId[0] + ".detinporcentajedescuento").val();
            $("." + arrId[0] + ".detporcentajedescuento").text(porcentajeDescuento + " %");

            var margen = $("." + arrId[0] + ".detmargen").text().replace("%", "").trim();
            var precio = $("." + arrId[0] + ".detprecio").text();
            //  var subtotal = $("." + arrId[0] + ".detsubtotal").text();
            var flete = $("." + arrId[0] + ".detinflete").val();

            var costo = $("." + arrId[0] + ".detcostoLista").text();

            var observacion = $("." + arrId[0] + ".detobservacionarea").val();

            json = json + '{"idProducto":"' + arrId[0] + '", "cantidad":"' + cantidad + '", "porcentajeDescuento":"' + porcentajeDescuento + '", "precio":"' + precio + '", "flete":"' + flete + '",  "costo":"' + costo + '", "observacion":"' + observacion + '"},'
        });
        json = json.substr(0, json.length - 1) + "]";


        /*
        var cotizacionDetalleJson = [
            { "idProducto": "John", "cantidad": "1", "porcentajeDescuento": "0" },
            { "idProducto": "Anna", "cantidad": "1", "porcentajeDescuento": "0" },
            { "idProducto": "Peter", "cantidad": "1", "porcentajeDescuento": "0" }];
        var   json3 = JSON.stringify(cotizacionDetalleJson);*/


        $.ajax({
            url: "/Venta/ChangeDetalle",
            type: 'POST',
            data: json,
            dataType: 'json',
            contentType: 'application/json',
            success: function (respuesta) {
                location.reload();
            }
        });
    });

    /*Evento que se dispara cuando se hace clic en el boton calcular descuento de la grilla*/
    $(document).on('click', "button.btnCalcularDescuento", function () {
        var idProducto = event.target.getAttribute("class").split(" ")[0];
        $("#idProducto").val(idProducto);
    });



    /*Evento que se dispara cuando se modifica un control de descuento de la grilla*/
    $(document).on('change', "input.detinporcentajedescuento", function () {
        var idproducto = event.target.getAttribute("class").split(" ")[0];
        calcularSubtotalGrilla(idproducto);
    });

    /*Evento que se dispara cuando se modifica un control de cantidad de la grilla*/
    $(document).on('change', "input.detincantidad", function () {
        var idproducto = event.target.getAttribute("class").split(" ")[0];
        calcularSubtotalGrilla(idproducto);
    });


    /*Evento que se dispara cuando se modifica un control de cantidad de la grilla*/
    $(document).on('change', "input.detinflete", function () {
        var idproducto = event.target.getAttribute("class").split(" ")[0];
        calcularSubtotalGrilla(idproducto);
    });

    /*Evento que se dispara cuando se modifica el color en la grilla*/
    /*    $(document).on('change', "input.detobservacioncolor", function () {
            var idproducto = event.target.getAttribute("class").split(" ")[0];
    
            alert(event.target.value);
    
            /*
    
    
            class="@cotizacionDetalle.producto.idProducto fila"
    
            #00ff40*/
    //});



    function calcularSubtotalGrilla(idproducto) {
        //Se obtiene el porcentaje descuento 
        var porcentajeDescuento = Number($("." + idproducto + ".detinporcentajedescuento").val());
        //Se obtiene el flete
        var flete = Number($("." + idproducto + ".detinflete").val());
        //Se obtiene el precio lista
        var precioLista = Number($("." + idproducto + ".detprecioLista").html());
        //Se calculo el precio con descuento 
        var precio = Number((precioLista * (100 - porcentajeDescuento) / 100).toFixed(cantidadCuatroDecimales));
        //Se asigna el precio calculculado en la columna precio
        $("." + idproducto + ".detprecio").html(precio);
        //se obtiene la cantidad
        var cantidad = Number($("." + idproducto + ".detincantidad").val());
        //Se define el precio Unitario 
        var precioUnitario = flete + precio
        $("." + idproducto + ".detprecioUnitario").html(precioUnitario.toFixed(cantidadCuatroDecimales));
        //Se calcula el subtotal
        var subTotal = precioUnitario * cantidad;
        //Se asigna el subtotal 
        $("." + idproducto + ".detsubtotal").html(subTotal.toFixed(cantidadDecimales));
        //Se calcula el margen
        var costo = Number($("." + idproducto + ".detcostoLista").html());
        var margen = (1 - (Number(costo) / Number(precio))) * 100;
        //Se asigna el margen 
        $("." + idproducto + ".detmargen").text(margen.toFixed(1) + " %");

        var precioNetoAnterior = Number($("." + idproducto + ".detprecioNetoAnterior").html());
        var varprecioNetoAnterior = (precio / precioNetoAnterior - 1) * 100;
        $("." + idproducto + ".detvarprecioNetoAnterior").text(varprecioNetoAnterior.toFixed(1));

        var costoAnterior = Number($("." + idproducto + ".detcostoAnterior").html());
        var varcosto = (costo / costoAnterior - 1) * 100;
        $("." + idproducto + ".detvarCosto").text(varcosto.toFixed(1) + " %");


        //Se actualiza el subtotal de la cotizacion

        var $j_object = $("td.detcantidad");

        var subTotal = 0;
        var igv = 0;
        var total = 0;

        $.each($j_object, function (key, value) {
            var arrId = value.getAttribute("class").split(" ");
            var precioUnitario = Number($("." + arrId[0] + ".detprecioUnitario").html());
            var cantidad = Number($("." + arrId[0] + ".detincantidad").val());
            subTotal = subTotal + Number(Number((precioUnitario * cantidad)).toFixed(cantidadDecimales));
        });



        var incluidoIGV = $("input[name=igv]:checked").val();
        //Si no se etsá incluyendo IGV se le agrega
        if (incluidoIGV == "0") {
            igv = Number((subTotal * IGV).toFixed(cantidadDecimales));
            total = subTotal + (igv);
        }
        //Si se está incluyendo IGV entonces se 
        else {
            total = subTotal;
            subTotal = Number((subTotal / (1 + IGV)).toFixed(cantidadDecimales));
            igv = total - subTotal;
        }

        $('#montoSubTotal').html(subTotal.toFixed(cantidadDecimales));
        $('#montoIGV').html(igv.toFixed(cantidadDecimales));
        $('#montoTotal').html(total.toFixed(cantidadDecimales));

    };

    /*####################################################
    EVENTOS BUSQUEDA COTIZACIONES
    #####################################################*/


    $("#btnLimpiarBusquedaPedidos").click(function () {
        $.ajax({
            url: "/Pedido/CleanBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });


    $("#btnBusquedaPedidos").click(function () {
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val();
        var fechaSolicitudDesde = $("#pedido_fechaSolicitudDesde").val();
        var fechaSolicitudHasta = $("#pedido_fechaSolicitudHasta").val();
        var fechaEntregaDesde = $("#pedido_fechaEntregaDesde").val();
        var fechaEntregaHasta = $("#pedido_fechaEntregaHasta").val();
        var fechaEntregaDesde = $("#pedido_fechaEntregaDesde").val();
        var fechaProgramacionDesde = $("#pedido_fechaProgramacionDesde").val();
        var fechaProgramacionHasta = $("#pedido_fechaProgramacionHasta").val();
        var pedido_numeroPedido = $("#pedido_numeroPedido").val();
        var pedido_numeroGrupoPedido = $("#pedido_numeroGrupoPedido").val();
        var estado = $("#estado").val();
        var estadoCrediticio = $("#estadoCrediticio").val();

        $.ajax({
            url: "/Pedido/Search",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                fechaSolicitudDesde: fechaSolicitudDesde,
                fechaSolicitudHasta: fechaSolicitudHasta,
                fechaEntregaDesde: fechaEntregaDesde,
                fechaEntregaHasta: fechaEntregaHasta,
                fechaProgramacionDesde: fechaProgramacionDesde,
                fechaProgramacionHasta: fechaProgramacionHasta,
                numero: pedido_numeroPedido,
                numeroGrupo: pedido_numeroGrupoPedido,
                estado: estado,
                estadoCrediticio: estadoCrediticio
            },
            success: function (pedidoList) {


                $("#tablePedidos > tbody").empty();
                //FooTable.init('#tableCotizaciones');
                $("#tablePedidos").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < pedidoList.length; i++) {

                    var observacion = pedidoList[i].seguimientoPedido.observacion == null ? "" : pedidoList[i].seguimientoPedido.observacion;

                    if (pedidoList[i].seguimientoPedido.observacion != null && pedidoList[i].seguimientoPedido.observacion.length > 20) {
                        var idComentarioCorto = pedidoList[i].idPedido + "corto";
                        var idComentarioLargo = pedidoList[i].idPedido + "largo";
                        var idVerMas = pedidoList[i].idPedido + "verMas";
                        var idVermenos = pedidoList[i].idPedido + "verMenos";

                        var comentario = pedidoList[i].seguimientoPedido.observacion.substr(0, 20) + "...";
                        observacion = '<div id="' + idComentarioCorto + '" style="display:block;">' + comentario + '</div>' +
                            '<div id="' + idComentarioLargo + '" style="display:none;">' + pedidoList[i].seguimientoPedido.observacion + '</div>' +
                            '<p><a id="' + idVerMas + '" class="' + pedidoList[i].idCotizacion + ' verMas" href="javascript:mostrar();" style="display:block">Ver Más</a></p>' +
                            '<p><a id="' + idVermenos + '" class="' + pedidoList[i].idCotizacion + ' verMenos" href="javascript:mostrar();" style="display:none">Ver Menos</a></p>';
                    }

                    var fechaProgramacion = "No Programado";
                    if (pedidoList[i].fechaProgramacion != null && pedidoList[i].fechaProgramacion != "") {
                        fechaProgramacion = invertirFormatoFecha(pedidoList[i].fechaProgramacion.substr(0, 10));
                    }

                    var pedido = '<tr data-expanded="true">' +
                        '<td>  ' + pedidoList[i].idPedido + '</td>' +
                        '<td>  ' + pedidoList[i].numeroPedidoString + '  </td>' +
                        '<td>  ' + pedidoList[i].numeroGrupoPedidoString + '  </td>' +
                        '<td>  ' + pedidoList[i].cliente.razonSocial + '</td>' +
                        '<td>  ' + pedidoList[i].cliente.codigo + ' </td>' +
                        '<td>  ' + pedidoList[i].ciudad.nombre + '  </td>' +
                        '<td>  ' + pedidoList[i].usuario.nombre + '  </td>' +
                        '<td>  ' + pedidoList[i].fechaHoraSolicitud + '</td>' +
                        '<td>  ' + pedidoList[i].rangoFechasEntrega + '</td>' +
                        '<td>  ' + fechaProgramacion + '</td>' +
                        '<td>  ' + pedidoList[i].montoTotal + '  </td>' +
                        '<td>  ' + pedidoList[i].seguimientoPedido.estadoString + '</td>' +
                        //  '<td>  ' + pedidoList[i].seguimientoPedido.usuario.nombre+'  </td>' +
                        //  '<td>  ' + observacion+'  </td>' +
                        '<td>  ' + pedidoList[i].seguimientoCrediticioPedido.estadoString + '</td>' +
                        '<td>' +
                        '<button type="button" class="' + pedidoList[i].idPedido + ' ' + pedidoList[i].numeroPedido + ' btnVerPedido btn btn-primary ">Ver</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tablePedidos").append(pedido);

                }

                if (pedidoList.length > 0)
                    $("#msgBusquedaSinResultados").hide();
                else
                    $("#msgBusquedaSinResultados").show();
            }
        });
    });

    $("#pedido_fechaSolicitudDesde").change(function () {
        var fechaSolicitudDesde = $("#pedido_fechaSolicitudDesde").val();
        $.ajax({
            url: "/Pedido/ChangeFechaSolicitudDesde",
            type: 'POST',
            data: {
                fechaSolicitudDesde: fechaSolicitudDesde
            },
            success: function () {
            }
        });
    });

    $("#pedido_fechaSolicitudHasta").change(function () {
        var fechaSolicitudHasta = $("#pedido_fechaSolicitudHasta").val();
        $.ajax({
            url: "/Pedido/ChangeFechaSolicitudHasta",
            type: 'POST',
            data: {
                fechaSolicitudHasta: fechaSolicitudHasta
            },
            success: function () {
            }
        });
    });

    $("#pedido_fechaEntregaDesde").change(function () {
        var fechaEntregaDesde = $("#pedido_fechaEntregaDesde").val();
        var fechaEntregaHasta = $("#pedido_fechaEntregaHasta").val();

        //Si fecha de entrega hasta es superior a fecha de entrega desde o la fecha de entrega hasta es vacío
        //se reemplaza el valor por la fecha de entrega desde
        if (convertirFechaNumero(fechaEntregaHasta) < convertirFechaNumero(fechaEntregaDesde)
            || fechaEntregaHasta.trim() == "") {
            $("#pedido_fechaEntregaHasta").val(fechaEntregaDesde);
            $("#pedido_fechaEntregaHasta").change();
        }

        $.ajax({
            url: "/Pedido/ChangeFechaEntregaDesde",
            type: 'POST',
            data: {
                fechaEntregaDesde: fechaEntregaDesde
            },
            success: function () {
            }
        });
    });

    $("#pedido_fechaEntregaHasta").change(function () {
        var fechaEntregaHasta = $("#pedido_fechaEntregaHasta").val();
        $.ajax({
            url: "/Pedido/ChangeFechaEntregaHasta",
            type: 'POST',
            data: {
                fechaEntregaHasta: fechaEntregaHasta
            },
            success: function () {
            }
        });
    });



    $("#pedido_numeroPedido").change(function () {
        var numero = $("#pedido_numeroPedido").val();
        $.ajax({
            url: "/Pedido/changeNumero",
            type: 'POST',
            data: {
                numero: numero
            },
            success: function () {
            }
        });
    });

    $("#pedido_numeroGrupoPedido").change(function () {
        var numeroGrupo = $("#pedido_numeroGrupoPedido").val();
        $.ajax({
            url: "/Pedido/changeNumeroGrupo",
            type: 'POST',
            data: {
                numeroGrupo: numeroGrupo
            },
            success: function () {
            }
        });
    });


    $("#estado").change(function () {
        var estado = $("#estado").val();
        $.ajax({
            url: "/Pedido/changeEstado",
            type: 'POST',
            data: {
                estado: estado
            },
            success: function () {
            }
        });
    });

    $("#estadoCrediticio").change(function () {
        var estado = $("#estadoCrediticio").val();
        $.ajax({
            url: "/Pedido/changeEstadoCrediticio",
            type: 'POST',
            data: {
                estadoCrediticio: estado
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#ActualDepartamento", function () {
        var ubigeoEntregaId = "000000";
        if ($("#ActualDepartamento").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualDepartamento").val() + "0000";
        }
        $.ajax({
            url: "/Pedido/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#ActualProvincia", function () {
        var ubigeoEntregaId = $("#ActualDepartamento").val() + "0000";
        if ($("#ActualProvincia").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualProvincia").val() + "00";
        }
        $.ajax({
            url: "/Pedido/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#ActualDistrito", function () {
        var ubigeoEntregaId = $("#ActualDepartamento").val() + $("#ActualProvincia").val() + "00";
        if ($("#ActualDistrito").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualDistrito").val();
        }
        $.ajax({
            url: "/Pedido/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
        /*obtenerDireccionesEntrega
        */
    });

    /*
    function obtenerDireccionesEntrega() {
    $.ajax({
        url: "/DireccionEntrega/GetDireccionesEntrega",
        type: 'POST',
        dataType: 'JSON',
        data: {
            ubigeo: ubigeo
        },
        success: function (direccionEntregaListTmp) {

            $('#pedido_direccionEntrega')
                .find('option')
                .remove()
                .end()
                ;

            $('#pedido_direccionEntrega').append($('<option>', {
                value: GUID_EMPTY,
                text: "Seleccione Dirección Entrega",
                direccion: "",
                contacto: "",
                telefono: ""
            }));


            for (var i = 0; i < direccionEntregaListTmp.length; i++) {
                $('#pedido_direccionEntrega').append($('<option>', {
                    value: direccionEntregaListTmp[i].idDireccionEntrega,
                    text: direccionEntregaListTmp[i].descripcion,
                    direccion: direccionEntregaListTmp[i].descripcion,
                    contacto: direccionEntregaListTmp[i].contacto,
                    telefono: direccionEntregaListTmp[i].telefono
                }));

            }

            deshabilitarEdicionDireccionEntrega();
            $('#btnNuevaDireccion').show();
            $('#btnModificarDireccion').hide();
            $('#btnCancelarDireccion').hide();
        }
        });
    }*/

    $("#idCiudad").change(function () {
        var idCiudad = $("#idCiudad").val();

        $.ajax({
            url: "/Pedido/ChangeIdCiudad",
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



    $(document).on('click', "a.verMas", function () {
        var idCotizacion = event.target.getAttribute("class").split(" ")[0];
        divCorto = document.getElementById(idCotizacion + "corto");
        divLargo = document.getElementById(idCotizacion + "largo");
        divVerMas = document.getElementById(idCotizacion + "verMas");
        divVerMenos = document.getElementById(idCotizacion + "verMenos");

        divCorto.style.display = 'none';
        divLargo.style.display = 'block';

        divVerMas.style.display = 'none';
        divVerMenos.style.display = 'block';
    });

    $(document).on('click', "a.verMenos", function () {
        var idCotizacion = event.target.getAttribute("class").split(" ")[0];
        divCorto = document.getElementById(idCotizacion + "corto");
        divLargo = document.getElementById(idCotizacion + "largo");
        divVerMas = document.getElementById(idCotizacion + "verMas");
        divVerMenos = document.getElementById(idCotizacion + "verMenos");

        divCorto.style.display = 'block';
        divLargo.style.display = 'none';

        divVerMas.style.display = 'block';
        divVerMenos.style.display = 'none';
    });

    /****************** PROGRAMACION PEDIDO****************************/

    $('#modalProgramacion').on('shown.bs.modal', function () {
        var fechaProgramacion = $("#fechaProgramaciontmp").val();
        $("#fechaProgramacion").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaProgramacion);
    })

    $("#btnAceptarProgramarPedido").click(function () {

        if ($("#fechaProgramacion").val() == "" || $("#fechaProgramacion").val() == null) {
            alert("Debe ingresar la fecha de programación.");
            $("#fechaProgramacion").focus();
            return false;
        }
        var fechaProgramacion = $('#fechaProgramacion').val();
        var comentarioProgramacion = $('#comentarioProgramacion').val();
        var fechaEntregaDesdeProgramacion = $("#fechaEntregaDesdeProgramacion").val();
        var fechaEntregaHastaProgramacion = $("#fechaEntregaHastaProgramacion").val();

        if (convertirFechaNumero(fechaProgramacion) < convertirFechaNumero(fechaEntregaDesdeProgramacion)
            || convertirFechaNumero(fechaProgramacion) > convertirFechaNumero(fechaEntregaHastaProgramacion)
        ) {
            var respuesta = confirm("¡ATENCIÓN! Está programando la atención del pedido en una fecha fuera del rango solicitado por el cliente.");
            if (!respuesta) {
                $("#fechaProgramacion").focus();
                return false;
            }
        }

        $.ajax({
            url: "/Pedido/Programar",
            type: 'POST',
            // dataType: 'JSON',
            data: {
                fechaProgramacion: fechaProgramacion,
                comentarioProgramacion: comentarioProgramacion
            },
            success: function (resultado) {
                alert('El pedido número ' + $("#verNumero").html() + ' se programó para ser atendido.');
                location.reload();
            }
        });
        $("btnCancelarProgramarPedido").click();
    });




    $('#documentoVenta_fechaEmision').change(function () {
        var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate');
        $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
        calcularFechaVencimiento();

    });

    function calcularFechaVencimiento() {
        var tipoPago = $('#tipoPago').val();
        switch (tipoPago) {
            case "1":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate');
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "2":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+7d');
                date2.setDate(date2.getDate() + 7);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "3":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+15d');
                date2.setDate(date2.getDate() + 15);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "4":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+30d');
                date2.setDate(date2.getDate() + 30);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "5":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+60d');
                date2.setDate(date2.getDate() + 60);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "6":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+90d');
                date2.setDate(date2.getDate() + 90);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "7":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+120d');
                date2.setDate(date2.getDate() + 120);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "8":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+20d');
                date2.setDate(date2.getDate() + 20);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
            case "9":
                var date2 = $('#documentoVenta_fechaEmision').datepicker('getDate', '+45d');
                date2.setDate(date2.getDate() + 45);
                $('#documentoVenta_fechaVencimiento').datepicker('setDate', date2);
                break;
        }
    }


    $('#tipoPago').change(function () {

        calcularFechaVencimiento();


    });


    $("#btnAceptarFacturarPedido").click(function () {

        if ($("#documentoVenta_fechaEmision").val() == "" || $("#documentoVenta_fechaEmision").val() == null) {
            alert("Debe ingresar la fecha de emisión.");
            $("#documentoVenta_fechaEmision").focus();
            return false;
        }

        if ($("#documentoVenta_horaEmision").val() == "" || $("#documentoVenta_horaEmision").val() == null) {
            alert("Debe ingresar la hora de emisión.");
            $("#documentoVenta_horaEmision").focus();
            return false;
        }

        if ($("#documentoVenta_fechaVencimiento").val() == "" || $("#documentoVenta_fechaVencimiento").val() == null) {
            alert("Debe ingresar la fecha de vencimiento.");
            $("#documentoVenta_fechaVencimiento").focus();
            return false;
        }

        if (convertirFechaNumero($("#documentoVenta_fechaEmision").val()) > convertirFechaNumero($("#documentoVenta_fechaVencimiento").val())) {
            alert("La fecha de vencimiento debe ser mayor o igual a la fecha de emisión.");
            $("#documentoVenta_fechaVencimiento").focus();
            return false;
        }
        /*
         if ($("#documentoVenta_correoEnvio").val() == "" || $("#documentoVenta_correoEnvio").val() == null) {
             alert("Debe ingresar el correo de envío.");
             $("#documentoVenta_correoEnvio").focus();
             return false;
         }
         else
         {
             if (!$("#documentoVenta_correoEnvio").val().match(/^[a-zA-Z0-9\._-]+@[a-zA-Z0-9-]{2,}[.][a-zA-Z]{2,4}$/)) 
             {
                 alert("Debe ingresar un correo de envío válido.");
                 $("#documentoVenta_correoEnvio").focus();
                 return false;
             }
         }
 
         */


        var fechaEmision = $("#documentoVenta_fechaEmision").val();
        var horaEmision = $("#documentoVenta_horaEmision").val();
        var fechaVencimiento = $("#documentoVenta_fechaVencimiento").val();
        var observaciones = $("#documentoVenta_observaciones").val();
        var tipoPago = $("#tipoPago").val();
        var formaPago = $("#formaPago").val();
        /*var correoCopia = $("#documentoVenta_correoCopia").val();
        var correoOculto = $("#documentoVenta_correoOculto").val();*/

        /*
        if ($("#fechaProgramacion").val() == "" || $("#fechaProgramacion").val() == null) {
            alert("Debe ingresar la fecha de programación.");
            $("#fechaProgramacion").focus();
            return false;
        }
        var fechaProgramacion = $('#fechaProgramacion').val();
        var comentarioProgramacion = $('#comentarioProgramacion').val();
        var fechaEntregaDesdeProgramacion = $("#fechaEntregaDesdeProgramacion").val();
        var fechaEntregaHastaProgramacion = $("#fechaEntregaHastaProgramacion").val();

        if (convertirFechaNumero(fechaProgramacion) < convertirFechaNumero(fechaEntregaDesdeProgramacion)
            || convertirFechaNumero(fechaProgramacion) > convertirFechaNumero(fechaEntregaHastaProgramacion)
        ) {
            var respuesta = confirm("¡ATENCIÓN! Está programando la atención del pedido en una fecha fuera del rango solicitado por el cliente.");
            if (!respuesta) {
                $("#fechaProgramacion").focus();
                return false;
            }
        }*/

        //   var fechaProgramacion = $('#fechaProgramacion').val();
        $('body').loadingModal({
            text: 'Creando Factura...'
        });
        $.ajax({
            url: "/Factura/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                fechaEmision: fechaEmision,
                horaEmision: horaEmision,
                fechaVencimiento: fechaVencimiento,
                tipoPago: tipoPago,
                formaPago: formaPago,
                observaciones: observaciones
                /*correoCopia: correoCopia,
                correoOculto: correoOculto*/
            },
            error: function (resultado) {
                $('body').loadingModal('hide')
                alert(MENSAJE_ERROR);

            },
            success: function (resultado) {
                $('body').loadingModal('hide')

                if (resultado.CPE_RESPUESTA_BE.CODIGO == "001") {
                    alert('Se generó la factura ' + resultado.serieNumero + '.');
                }
                else {
                    alert(MENSAJE_ERROR);
                }


                location.reload();
            }
        });
        $("btnCancelarFacturarPedido").click();
    });





    $('#modalFacturar').on('shown.bs.modal', function () {




        // $("#documentoVenta_fechaEmision").val();



        /*  $('#familia').focus();
          $('#familia').val("Todas");
          $('#proveedor').val("Todos");
          */
        //$('#producto').trigger('chosen:activate');
    })


    $(document).on('click', "button.btnMostrarPrecios", function () {

        var idProducto = event.target.getAttribute("class").split(" ")[0];

        var idCliente = $("#idCliente").val();

        /*

        if ($("#pagina").val() == 2) {
            idCliente = $("#verIdCliente").val();
            sessionPedido = "pedidoVer";
        }
        else {
            idCliente = $("#idCliente").val();
            if (idCliente.trim() == "") {
                alert("Debe seleccionar un cliente.");
                $('#idCliente').trigger('chosen:activate');
                return false;
            }
        }*/

        //verIdCliente


        $.ajax({
            url: "/Precio/GetPreciosRegistradosVenta",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: idProducto,
                idCliente: idCliente,
                controller: "venta"
            },
            success: function (producto) {

                $("#verProducto").html(producto.nombre);
                $("#verCodigoProducto").html(producto.sku);


                var precioListaList = producto.precioLista;

                // var producto = $.parseJSON(respuesta);
                $("#tableMostrarPrecios > tbody").empty();

                FooTable.init('#tableMostrarPrecios');
                for (var i = 0; i < precioListaList.length; i++) {
                    var fechaInicioVigencia = precioListaList[i].fechaInicioVigencia;
                    var fechaFinVigencia = precioListaList[i].fechaFinVigencia;

                    if (fechaInicioVigencia == null)
                        fechaInicioVigencia = "No Definida";
                    else
                        fechaInicioVigencia = invertirFormatoFecha(precioListaList[i].fechaInicioVigencia.substr(0, 10));

                    if (fechaFinVigencia == null)
                        fechaFinVigencia = "No Definida";
                    else
                        fechaFinVigencia = invertirFormatoFecha(precioListaList[i].fechaFinVigencia.substr(0, 10));

                    var numeroCotizacion = precioListaList[i].numeroCotizacion;
                    if (numeroCotizacion == null)
                        numeroCotizacion = "No Identificado";

                    $("#tableMostrarPrecios").append('<tr data-expanded="true">' +

                        '<td>' + numeroCotizacion + '</td>' +
                        '<td>' + fechaInicioVigencia + '</td>' +
                        '<td>' + fechaFinVigencia + '</td>' +
                        '<td>' + precioListaList[i].unidad + '</td>' +
                        '<td>' + Number(precioListaList[i].precioNeto).toFixed(cantidadCuatroDecimales) + '</td>' +
                        '<td>' + Number(precioListaList[i].flete).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(precioListaList[i].precioUnitario).toFixed(cantidadCuatroDecimales) + '</td>' +

                        '</tr>');
                }
            }
        });
        $("#modalMostrarPrecios").modal();

    });
    /****************** FIN PROGRAMACION PEDIDO****************************/

    /********************ANDRE************************************************/

    function validacionBusqueda() {

        var fechaVencimiento = $("#Venta_fechaEmision_Hasta").val();
        var fechaInicio = $("#Venta_fechaEmision_Desde").val();

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
                title: "Fecha Vencimiento Inválido",
                type: 'orange',
                content: 'Debe ingresar una fecha posterior o igual a la de inicio.',
                buttons: {
                    OK: function () { $('#Venta_fechaEmision_Hasta').focus(); }
                }
            });
            return false;
        }        
       
        return true;
    }


    $("#btnBusquedaVentaList").click(function () {

        var tipoDocumento = $("#VentaList_tipoDocumento").val();
        var numero = $("#VentaList_numeroDocumento").val();

        if (!validacionBusqueda())
            return false;

        $("#btnBusquedaVentaList").attr("disabled", "disabled");
        $.ajax({
            url: "/Venta/SearchList",
            type: 'POST',
            dataType: 'JSON',
            data: {
                numero: numero,
                tipoDocumento: tipoDocumento
            },
            error: function () {
                $("#btnBusquedaVentaList").removeAttr("disabled");
            },

            success: function (ventaList) {

                mostrarListVenta(ventaList);
            }
        });
    });

    function mostrarListVenta(ventaList) {
        $("#btnBusquedaVentaList").removeAttr("disabled");
        $("#tableVentaList > tbody").empty();
        $("#tableVentaList").footable({
            "paging": {
                "enabled": true
            }
        });

        for (var i = 0; i < ventaList.length; i++) {
            if (ventaList[i].documentoVenta.numero == "-") { ventaList[i].documentoVenta.numero = " "; }
            if (ventaList[i].cliente.responsableComercial.codigo == null) { ventaList[i].cliente.responsableComercial.codigo = " "; }
            var Venta = '<tr data-expanded="true">' +
                '<td>  ' + ventaList[i].idVenta + '</td>' +
                '<td>  ' + ventaList[i].pedido.numeroPedidoString + '</td>' +
                '<td>  ' + ventaList[i].guiaRemision.serieDocumento + '</td>' +
                '<td>  ' + ventaList[i].documentoVenta.numero + '</td>' +
                //'<td>  ' + ventaList[i].usuario.nombre + '</td>' +
                '<td>  ' + $.datepicker.formatDate('dd/mm/yy', new Date(ventaList[i].guiaRemision.fechaEmision)) + '</td>' +
                '<td>  ' + ventaList[i].cliente.codigo + '</td>' +
                '<td>  ' + ventaList[i].cliente.razonSocial + '</td>' +
                '<td>  ' + ventaList[i].cliente.ruc + '</td>' +
                '<td>  ' + ventaList[i].ciudad.sede + '</td>' +
                '<td>  ' + ventaList[i].total + '</td>' +
                '<td>' + ventaList[i].cliente.responsableComercial.descripcion + '</td>' +

                '<td> <button type="button" class="' + ventaList[i].guiaRemision.idMovimientoAlmacen + ' ' + ventaList[i].idVenta + ' btnVerVentaList btn btn-primary">Ver</button>' +
                '</tr>';
            $("#tableVentaList").append(Venta);
        }


        if (ventaList.length > 0) {
            $("#msgBusquedaSinResultados").hide();
        }
        else {
            $("#msgBusquedaSinResultados").show();
        }
    }

    $("#btnLimpiarBusquedaVentaList").click(function () {
        $.ajax({
            url: "/Venta/CleanBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });


    $("#idCiudadBusquedaList").change(function () {
        var idCiudad = $("#idCiudadBusquedaList").val();

        $.ajax({
            url: "/Venta/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad
            },
            error: function (detalle) {

            },
            success: function (ciudad) {
            }
        });
    });

    $("#venta_guiaRemision_sku").change(function () {
        changeInputStringVenta("sku", $("#venta_guiaRemision_sku").val());
    });

    $("#venta_guiaRemision_numero_list").change(function () {
        changeInputIntGuia("numero", $("#venta_guiaRemision_numero_list").val());
    });

    $("#venta_pedido_numeroPedido").change(function () {
        changeInputIntPedido("numeroPedido", $("#venta_pedido_numeroPedido").val());
    });


    function changeInputIntPedido(propiedad, valor) {
        $.ajax({
            url: "/Venta/ChangeInputIntPedido",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }
    function changeInputIntGuia(propiedad, valor) {
        $.ajax({
            url: "/Venta/ChangeInputIntGuia",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#Venta_fechaEmision_Desde").change(function () {
        var fechaEmisionDesde = $(this).val();
        $.ajax({
            url: "/Venta/ChangeFechaEmisionDesde",
            type: 'POST',
            data: {
                fechaEmisionDesde: fechaEmisionDesde
            },
            success: function () {
            }
        });
    });

    $("#Venta_fechaEmision_Hasta").change(function () {
        var fechaEmisionHasta = $(this).val();
        $.ajax({
            url: "/Venta/ChangeFechaEmisionHasta",
            type: 'POST',
            data: {
                fechaEmisionHasta: fechaEmisionHasta
            },
            success: function () {
            }
        });
    });

    function changeInputStringVenta(propiedad, valor) {
        $.ajax({
            url: "/Venta/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#idClienteBusquedaList").change(function () {
        //  $("#contacto").val("");
        var idCliente = $(this).val();

        $.ajax({
            url: "/Venta/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente) {
            }
        });

    });

    function cargarChosenClienteList() {
        $("#idClienteBusquedaList").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" }).on('chosen:showing_dropdown', function (evt, params) {
            if ($("#idCiudadBusquedaList").val() == "" || $("#idCiudadBusquedaList").val() == null) {
                alert("Debe seleccionar la sede MP previamente.");
                $("#idClienteBusquedaList").trigger('chosen:close');
                $("#idCiudadBusquedaList").focus();
                return false;
            }
        });

        $("#idClienteBusquedaList").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/Venta/SearchClientes"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });
    }

    $(document).ready(function () {

        cargarChosenClienteList();
        $("#VentaList_tipoDocumento").find("option[value='7']").remove();
        $("#VentaList_tipoDocumento").find("option[value='8']").remove();
       
        $("#btnBusquedaVentaList").click();

    });

    var fechaEmisionDesde = $("#Venta_fechaEmision_Desde").val();
    var fechaEmisionHasta = $("#Venta_fechaEmision_Hasta").val();


    $("#Venta_fechaEmision_Desde").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaEmisionDesde);

    $("#Venta_fechaEmision_Hasta").datepicker({
        dateFormat: 'dd/mm/yy'
    }).datepicker("setDate", fechaEmisionHasta);


    $(document).on('click', "button.btnVerVentaList", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idMovimientoAlmacen = arrrayClass[0];
        var idVenta = arrrayClass[1];


        $.ajax({
            url: "/Venta/ShowList",
            data: {
                idMovimientoAlmacen: idMovimientoAlmacen,
                idVenta: idVenta
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                mostrarMensajeErrorVenta();
                detalle.rectificarVenta.rectificar_venta;
            },
            success: function (resultado) {
                mostrarModalVenta(resultado);
            }
        });

    });

    function mostrarModalVenta(resultado) {
        //var cotizacion = $.parseJSON(respuesta);
        var permiso = resultado.PermisoRectificarVenta.Permiso;
        var venta = resultado.venta;
        var pedido = resultado.venta.pedido;
        var guiaRemision = resultado.venta.guiaRemision;
        var serieDocumentoElectronicoList = resultado.serieDocumentoElectronicoList;

        //  var usuario = resultado.usuario;
        

        $("#fechaEntregaDesdeProgramacion").val(invertirFormatoFecha(pedido.fechaEntregaDesde.substr(0, 10)));
        $("#fechaEntregaHastaProgramacion").val(invertirFormatoFecha(pedido.fechaEntregaHasta.substr(0, 10)));
        $("#fechaProgramaciontmp").val(invertirFormatoFecha(pedido.fechaEntregaDesde.substr(0, 10)));
        //Important

        $("#idPedido").val(pedido.idPedido);

        $("#verNumero").html(pedido.numeroPedidoString);
        $("#verNumeroGrupo").html(pedido.numeroGrupoPedidoString);
        $("#verCotizacionCodigo").html(pedido.cotizacion.numeroCotizacionString);
        $("#verTipoPedido").html(pedido.tiposPedidoString);

        $("#verUsuarioNombre").html(pedido.usuario.nombre);
        $("#verFechaHoraSolicitud").html(pedido.fechaHoraSolicitud);

        if (pedido.tipoPedido == "84") {
            $("#divReferenciaCliente").hide();
            $("#divCiudadSolicitante").show();
            $("#verCiudadSolicitante").html(pedido.cliente.ciudad.nombre);
        }
        else {
            $("#divReferenciaCliente").show();
            $("#divCiudadSolicitante").hide();
        }


        $("#verFechaHorarioEntrega").html(pedido.fechaHorarioEntrega);

        $("#verCiudad").html(pedido.ciudad.nombre);
        $("#idClienteFacturacion").val(pedido.cliente.idCliente);
        $("#verCliente").html(pedido.cliente.razonSocial);
        $("#verClienteCodigo").html(pedido.cliente.codigo);
        $("#verNumeroReferenciaCliente").html(pedido.numeroReferenciaCliente);
        $("#verNumeroReferenciaAdicional").html(pedido.numeroReferenciaAdicional);
        $("#verObservacionesPedido").html(pedido.observaciones);

        if (pedido.cliente.tipoDocumento == CONS_TIPO_DOC_CLIENTE_RUC) {
            $("#modalFacturarTitle").html("<b>Crear Factura</b>");
            $("#descripcionDatosDocumento").html("<b>Datos de la Factura</b>");
            $("#observacionesDocumento").html("Observaciones Factura:");
            $("#btnAceptarFacturarPedido").html("Generar Factura");
            $("#pedido_cliente_tipoDocumento").val(CONS_TIPO_DOC_CLIENTE_RUC);
        }
        else if (pedido.cliente.tipoDocumento == CONS_TIPO_DOC_CLIENTE_DNI) {
            $("#modalFacturarTitle").html("<b>Crear Boleta</b>");
            $("#descripcionDatosDocumento").html("<b>Datos de la Boleta</b>");
            $("#observacionesDocumento").html("Observaciones Boleta:");
            $("#btnAceptarFacturarPedido").html("Generar Boleta");
            $("#pedido_cliente_tipoDocumento").val(CONS_TIPO_DOC_CLIENTE_DNI);
        }
        else if (pedido.cliente.tipoDocumento == CONS_TIPO_DOC_CLIENTE_CARNET_EXTRANJERIA) {
            $("#modalFacturarTitle").html("<b>Crear Boleta</b>");
            $("#descripcionDatosDocumento").html("<b>Datos de la Boleta</b>");
            $("#observacionesDocumento").html("Observaciones Boleta:");
            $("#btnAceptarFacturarPedido").html("Generar Boleta");
            $("#pedido_cliente_tipoDocumento").val(CONS_TIPO_DOC_CLIENTE_CARNET_EXTRANJERIA);
        }


        $("#nombreArchivos > li").remove().end();


        for (var i = 0; i < pedido.pedidoAdjuntoList.length; i++) {
            var liHTML = '<a href="javascript:mostrar();" class="descargarDesdeVenta">' + pedido.pedidoAdjuntoList[i].nombre + '</a>';
            $('<li />').html(liHTML).appendTo($('#nombreArchivos'));
        }




        $("#verDireccionEntrega").html(pedido.direccionEntrega.descripcion);
        $("#verTelefonoContactoEntrega").html(pedido.direccionEntrega.telefono);
        $("#verContactoEntrega").html(pedido.direccionEntrega.contacto);

        $("#verUbigeoEntrega").html(pedido.ubigeoEntrega.ToString);

        $("#verContactoPedido").html(pedido.contactoPedido);
        $("#verTelefonoCorreoContactoPedido").html(pedido.telefonoCorreoContactoPedido);






        $("#verFechaHoraSolicitud").html(pedido.fechaHoraSolicitud);
        $("#facturarver_guiaRemision_fechaTraslado").html(invertirFormatoFecha(guiaRemision.fechaTraslado.substr(0, 10)));
        $("#facturarver_guiaRemision_fechaEmision").html(invertirFormatoFecha(guiaRemision.fechaEmision.substr(0, 10)));
        $("#facturarver_guiaRemision_serieNumeroDocumento").html(guiaRemision.serieDocumento);

        $("#verObservaciones").html(pedido.observaciones);
        $("#verMontoSubTotal").html(Number(pedido.montoSubTotal).toFixed(cantidadDecimales));
        $("#verMontoIGV").html(Number(pedido.montoIGV).toFixed(cantidadDecimales));
        $("#verMontoTotal").html(Number(pedido.montoTotal).toFixed(cantidadDecimales));
        $("#documentoVenta_observaciones").val(pedido.observacionesFactura);

        $("#verMontoSubTotalVenta").html(Number(venta.subTotal).toFixed(cantidadDecimales));
        $("#verMontoIGVVenta").html(Number(venta.igv).toFixed(cantidadDecimales));
        $("#verMontoTotalVenta").html(Number(venta.total).toFixed(cantidadDecimales));





        $("#tableDetallePedido > tbody").empty();

        FooTable.init('#tableDetallePedido');
        
        //    $("#formVerGuiasRemision").html("");

        var d = '';
        var lista = pedido.pedidoDetalleList;
        for (var i = 0; i < lista.length; i++) {

            var observacion = lista[i].observacion == null || lista[i].observacion == 'undefined' ? '' : lista[i].observacion;

            var RectificarVenta;
            if (permiso == 1) {

                if (lista[i].excluirVenta == 1 && lista[i].estadoVenta == 1)
                {                    
                    RectificarVenta = '<td><input type="checkbox" checked class="chkRectificarVenta" id="' + lista[i].idVentaDetalle+'"" name="rectificarVenta"></td>';
                }
                else
                    RectificarVenta = '<td><input type="checkbox" class="chkRectificarVenta" id="' + lista[i].idVentaDetalle + '"" name="rectificarVenta"></td>';
            }
            else {
                RectificarVenta = "";
            }


            d += '<tr>' +               
                 RectificarVenta +
                '<td>' + lista[i].producto.proveedor + '</td>' +
                '<td>' + lista[i].producto.sku + '</td>' +
                '<td>' + lista[i].producto.descripcion + '</td>' +
                '<td>' + lista[i].unidad + '</td>' +
                '<td class="column-img"><img class="table-product-img" src="data:image/png;base64,' + lista[i].producto.image + '"> </td>' +
                '<td>' + lista[i].precioLista.toFixed(cantidadDecimales) + '</td>' +
                '<td>' + lista[i].porcentajeDescuentoMostrar.toFixed(cantidadDecimales) + ' %</td>' +
                '<td>' + lista[i].precioNeto.toFixed(cantidadCuatroDecimales) + '</td>' +
                '<td>' + lista[i].margen.toFixed(cantidadDecimales) + ' %</td>' +
                '<td>' + lista[i].flete.toFixed(cantidadDecimales) + '</td>' +
                //             '<td>' + lista[i].precioUnitario.toFixed(cantidadCuatroDecimales) + '</td>' +
                '<td>' + lista[i].precioUnitarioVenta.toFixed(cantidadCuatroDecimales) + '</td>' +
                '<td>' + lista[i].cantidad + '</td>' +
                //     '<td>' + lista[i].cantidadPendienteAtencion + '</td>' +
                '<td>' + lista[i].subTotal.toFixed(cantidadDecimales) + '</td>' +
                '<td>' + observacion + '</td>' +
                '<td class="' + lista[i].producto.idProducto + ' detbtnMostrarPrecios"> <button  type="button" class="' + lista[i].producto.idProducto + ' btnMostrarPreciosVentaList btn btn-primary bouton-image botonPrecios"></button></td>' +
                '</tr>';
        }

        $("#verRazonSocialSunat").html(pedido.cliente.razonSocialSunat);
        $("#verRUC").html(pedido.cliente.ruc);
        $("#verDireccionDomicilioLegalSunat").html(pedido.cliente.direccionDomicilioLegalSunat);
        $("#verCodigo").html(pedido.cliente.codigo);

        $("#documentoVenta_observaciones").val(pedido.observacionesFactura);



        $("#verCorreoEnvioFactura").html(pedido.cliente.correoEnvioFactura);

        //$("#documentoVenta_fechaEmision").val(invertirFormatoFecha(venta.guiaRemision.fechaEmision.substr(0, 10)));

        $("#documentoVenta_fechaEmisionList").val($.datepicker.formatDate('dd/mm/yy', new Date(pedido.documentoVenta.FechaRegistro)));
        $("#documentoVenta_fechaVencimientoList").val($.datepicker.formatDate('dd/mm/yy', new Date(pedido.documentoVenta.fechaVencimiento)));

        var date = new Date(pedido.documentoVenta.horaEmision);
        hora = date.getHours();
        minuto = (date.getMinutes() < 10 ? '0' : '') + date.getMinutes();
        horaImprimible = hora + ":" + minuto;
        $("#documentoVenta_horaEmisionList").val(horaImprimible);





        $("#tipoPagoList").val(pedido.cliente.tipoPagoFactura);
        $("#formaPagoList").val(pedido.cliente.formaPagoFactura);
        $("#documentoVenta_serieList").val(pedido.documentoVenta.serie + '-' + pedido.documentoVenta.numero);



        $("#documentoVenta_fechaEmisionList").attr("disabled", "disabled");
        $("#documentoVenta_fechaVencimientoList").attr("disabled", "disabled");
        $("#documentoVenta_horaEmisionList").attr("disabled", "disabled");
        $("#formaPagoList").attr("disabled", "disabled");
        $("#documentoVenta_serieList").attr("disabled", "disabled");
        $("#tipoPagoList").attr("disabled", "disabled");


        calcularFechaVencimiento();
        $("#formaPago").val(pedido.cliente.formaPagoFactura);


        $("fieldset#NumeroDocumentoVenta").show();
        if (pedido.documentoVenta.idDocumentoVenta == '00000000-0000-0000-0000-000000000000') {
            $("fieldset#NumeroDocumentoVenta").hide();
        }

        $("#tableDetallePedido").append(d);

        $("#modalFacturar").modal('show');
    }

    $(document).on('click', "button#btnExcluirItemsVenta", function () {
        var total = $('.chkRectificarVenta').length;        
        var error = 0;
        var success = 0;
        $('.chkRectificarVenta').each(function () {
            var valor;
           
            var id_detalle_producto;
            if ($(this).prop('checked')) {
                id_detalle_producto = $(this).attr("id");
                 valor = 1;
                AjaxCheck();    
            }
            if ($(this).prop('checked')==false) {
                id_detalle_producto = $(this).attr("id");
                 valor = 0;
                AjaxCheck();
            }
            function AjaxCheck()
            {
                $.ajax({
                    url: "/Venta/RectificarVentaCheck",
                    type: 'POST', 
                    async: false,
                    data: {
                        id_detalle_producto: id_detalle_producto,
                        valor: valor
                    },
                    success:function()
                    {
                        success = success + 1;
                    },
                    error: function()
                    {
                        error = error + 1;
                    }
                });
                
            }
            if (total == success)
            {
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: 'Se guardaron correctamente los cambios.',
                    buttons: {
                        OK: function ()
                        {
                           
                        }
                    }
                });
            }
            if (error == 1) {
                $.alert({
                    title: 'Error',
                    type: 'red',
                    content: 'Ocurrió un problema al guardar los cambios.',
                    buttons: {
                        OK: function () {

                        }
                    }
                });
            }
           

            
        });
    });

    $(document).on('click', "button.btnMostrarPreciosVentaList", function () {

        var idProducto = event.target.getAttribute("class").split(" ")[0];
        var idCliente = $("#idClienteFacturacion").val();

        $.ajax({
            url: "/Precio/GetPreciosRegistradosVentaVer",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: idProducto,
                idCliente: idCliente
            },
            success: function (producto) {

                $("#verProducto").html(producto.nombre);
                $("#verCodigoProducto").html(producto.sku);

                var precioListaList = producto.precioLista;

                // var producto = $.parseJSON(respuesta);
                $("#tableMostrarPrecios > tbody").empty();

                FooTable.init('#tableMostrarPrecios');
                for (var i = 0; i < precioListaList.length; i++) {
                    var fechaInicioVigencia = precioListaList[i].fechaInicioVigencia;
                    var fechaFinVigencia = precioListaList[i].fechaFinVigencia;

                    if (fechaInicioVigencia == null)
                        fechaInicioVigencia = "No Definida";
                    else
                        fechaInicioVigencia = invertirFormatoFecha(precioListaList[i].fechaInicioVigencia.substr(0, 10));

                    if (fechaFinVigencia == null)
                        fechaFinVigencia = "No Definida";
                    else
                        fechaFinVigencia = invertirFormatoFecha(precioListaList[i].fechaFinVigencia.substr(0, 10));

                    var numeroCotizacion = precioListaList[i].numeroCotizacion;
                    if (numeroCotizacion == null)
                        numeroCotizacion = "No Identificado";

                    $("#tableMostrarPrecios").append('<tr data-expanded="true">' +

                        '<td>' + numeroCotizacion + '</td>' +
                        '<td>' + fechaInicioVigencia + '</td>' +
                        '<td>' + fechaFinVigencia + '</td>' +
                        '<td>' + precioListaList[i].unidad + '</td>' +
                        '<td>' + Number(precioListaList[i].precioNeto).toFixed(cantidadCuatroDecimales) + '</td>' +
                        '<td>' + Number(precioListaList[i].flete).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(precioListaList[i].precioUnitario).toFixed(cantidadCuatroDecimales) + '</td>' +

                        '</tr>');
                }
            }
        });
        $("#modalMostrarPrecios").modal();

    });

});