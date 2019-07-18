
jQuery(function ($) {
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_VALIDACION_PEDIDO = 'Revisar Datos del Pedido';
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        obtenerConstantes();
      //  setTimeout(autoGuardarPedido, MILISEGUNDOS_AUTOGUARDADO);
        cargarChosenCliente();
        toggleControlesUbigeo();
        toggleControlesDireccionEntrega();
        toggleControlesSolicitante();
        verificarSiExisteNuevaDireccionEntrega();
        verificarSiExisteNuevoSolicitante();
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
        if ($("#esCliente").val() == 0) {
            if ($("#idCliente").val().trim() != "" && $("#pagina").val() == PAGINA_MANTENIMIENTO_PEDIDO_COMPRA)
                $("#idCiudad").attr("disabled", "disabled");
        }
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
            url: "/PedidoCompra/autoGuardarPedido",
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

    function verificarSiExisteNuevoSolicitante() {
        $('#pedido_solicitante option').each(function () {
            if ($(this).val() == GUID_EMPTY) {
                $("#btnAgregarSolicitante").attr("disabled", "disabled");
            }
        });
    }

    $("#btnExcelCompra").click(function () {
        window.location.href = $(this).attr("actionLink");
    });


    /**
     * ################################ INICIO CONTROLES DE CLIENTE
     */

    function cargarChosenCliente() {

        $("#idCliente").chosen({ placeholder_text_single: "Buscar Proveedor", no_results_text: "No se encontró Proveedor" }).on('chosen:showing_dropdown', function (evt, params) {
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
            url: "/PedidoCompra/SearchClientes"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Proveedor", no_results_text: "No se encontró Proveedor" });
    }





    function toggleControlesUbigeo() {
        //Si cliente esta Seleccionado se habilitan la seleccion para la direccion de entrega
        if ($("#idCliente").val().trim() == "") {
            $("#ActualDepartamento").attr('disabled', 'disabled');
            $('#ActualProvincia').attr('disabled', 'disabled');
            $('#ActualDistrito').attr('disabled', 'disabled');
            $("#pedido_direccionEntrega").attr('disabled', 'disabled');
            $("#pedido_solicitante").attr('disabled', 'disabled');
            $("#btnAgregarDireccion").attr("disabled", "disabled");
            $("#btnAgregarSolicitante").attr("disabled", "disabled");
        }
        else {
            $("#idCiudad").attr('disabled', 'disabled');
            $('#ActualDepartamento').removeAttr('disabled');
            $('#ActualProvincia').removeAttr('disabled');
            $('#ActualDistrito').removeAttr('disabled');
            $("#pedido_direccionEntrega").removeAttr('disabled');
            $("#btnAgregarDireccion").removeAttr('disabled');
            $("#pedido_solicitante").removeAttr('disabled');
            $("#btnAgregarSolicitante").removeAttr('disabled');
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
            $("#pedido_direccionEntrega_descripcion").removeAttr("disabled");
            $("#pedido_direccionEntrega_contacto").removeAttr("disabled");
            $("#pedido_direccionEntrega_telefono").removeAttr("disabled");
        }
    }
    
    function toggleControlesSolicitante() {
        var idSolicitante = $('#pedido_solicitante').val();
        if (idSolicitante == "") {
            $("#pedido_solicitante_nombre").attr('disabled', 'disabled');
            $("#pedido_solicitante_telefono").attr('disabled', 'disabled');
            $("#pedido_solicitante_correo").attr('disabled', 'disabled');

        }
        else {
            $("#pedido_solicitante_nombre").removeAttr("disabled");
            $("#pedido_solicitante_telefono").removeAttr("disabled");
            $("#pedido_solicitante_correo").removeAttr("disabled");
        }
    }


    $("#idCliente").change(function () {

        var idCliente = $(this).val();

        $.ajax({
            url: "/PedidoCompra/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            error: function (detalle){ },
            success: function (cliente)
            {
                
                if ($("#pagina").val() == PAGINA_MANTENIMIENTO_PEDIDO_COMPRA)
                    $("#idCiudad").attr("disabled", "disabled");

                $("#idCiudad").attr("disabled", "disabled");


                ////Direccion Entrega
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
                    }));
                }

                //Se limpia controles de Ubigeo
                $("#ActualDepartamento").val("");
                $("#ActualProvincia").val("");
                $("#ActualDistrito").val("");
                toggleControlesUbigeo();





                ///Solicitante
                var solicitanteListTmp = cliente.solicitanteList;

                $('#pedido_solicitante')
                    .find('option')
                    .remove()
                    .end()
                    ;

                $('#pedido_solicitante').append($('<option>', {
                    value: "",
                    text: "Seleccione Solicitante"
                }));


                for (var i = 0; i < solicitanteListTmp.length; i++) {
                    $('#pedido_solicitante').append($('<option>', {
                        value: solicitanteListTmp[i].idSolicitante,
                        text: solicitanteListTmp[i].nombre
                    }));
                }



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
            url: "/PedidoCompra/CreateDireccionTemporal",
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




    $("#btnSaveSolicitante").click(function () {

        if ($("#solicitante_nombre").val().trim() == "") {
            alert("Debe ingresar el nombre del solicitante.");
            $('#solicitante_nombre').focus();
            return false;
        }

        if ($("#solicitante_telefono").val().trim() == "") {
            alert("Debe ingresar el telefono del solicitante.");
            $('#solicitante_telefono').focus();
            return false;
        }

        if ($("#solicitante_correo").val().trim() == "") {
            alert("Debe ingresar el correo del solicitante.");
            $('#solicitante_correo').focus();
            return false;
        }

        var nombre = $("#solicitante_nombre").val();
        var telefono = $("#solicitante_telefono").val();
        var correo = $("#solicitante_correo").val();

        $.ajax({
            url: "/PedidoCompra/CreateSolicitanteTemporal",
            type: 'POST',
            dataType: 'JSON',
            data: {
                nombre : nombre,
                telefono: telefono,
                correo: correo
            },
            error: function (detalle) { alert("Se generó un error al intentar crear el solicitante."); },
            success: function (solicitante) {

                $('#pedido_solicitante').append($('<option>', {
                    value: solicitante.idSolicitante,
                    text: solicitante.nombre
                }));
                $('#pedido_solicitante').val(solicitante.idSolicitante);

                $('#pedido_solicitante_nombre').val(solicitante.nombre);
                $('#pedido_solicitante_telefono').val(solicitante.telefono);
                $('#pedido_solicitante_correo').val(solicitante.correo);
                verificarSiExisteNuevoSolicitante();
                toggleControlesSolicitante();
            }
        });


        $('#btnCancelSolicitante').click();

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

    var fechaCreacionDesde = $("#fechaCreacionDesdetmp").val();
    $("#pedido_fechaCreacionDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaCreacionDesde);

    var fechaCreacionHasta = $("#fechaCreacionHastatmp").val();
    $("#pedido_fechaCreacionHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaCreacionHasta);

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
        $("#divReferenciaCliente").show();
        $("#divCiudadASolicitar").hide();
        if (tipoPedido == TIPO_PEDIDO_COMPRA_COMPRA.charCodeAt(0)
      //      || tipoPedido == TIPO_PEDIDO_COMPRA_TRASLADO_INTERNO_RECIBIDO.charCodeAt(0)
            || tipoPedido == TIPO_PEDIDO_COMPRA_COMODATO_RECIBIDO.charCodeAt(0)
            || tipoPedido == TIPO_PEDIDO_COMPRA_TRANSFERENCIA_GRATUITA_RECIBIDA.charCodeAt(0)
           // || tipoPedido == TIPO_PEDIDO_COMPRA_PRESTAMO_RECIBIDO.charCodeAt(0)
        ) {

                $(".mostrarDatosParaGuia").hide();
            }
            else {
                $(".mostrarDatosParaGuia").show();
            }
            

    }

    $("#pedido_tipoPedido").change(function () { 
        var tipoPedido = $("#pedido_tipoPedido").val();
        validarTipoPedido(tipoPedido);
        

        $.ajax({
            url: "/PedidoCompra/ChangeTipoPedido",
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
            url: "/PedidoCompra/ChangeIdCiudadASolicitar",
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
            url: "/PedidoCompra/ChangeNumeroReferenciaCliente",
            type: 'POST',
            data: {
                numeroReferenciaCliente: $("#pedido_numeroReferenciaCliente").val()
            },
            success: function () { }
        });
    });

    $("#pedido_otrosCargos").change(function () {
        $.ajax({
            url: "/PedidoCompra/ChangeOtrosCargos",
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
            url: "/PedidoCompra/ChangeDireccionEntrega",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idDireccionEntrega: idDireccionEntrega
            },
            success: function (direccionEntrega) {
                
                $("#pedido_direccionEntrega_telefono").val(direccionEntrega.telefono);    
                $("#pedido_direccionEntrega_contacto").val(direccionEntrega.contacto);
                $("#pedido_direccionEntrega_descripcion").val(direccionEntrega.descripcion);
                location.reload()
            }
        })
    });


    $("#pedido_direccionEntrega_descripcion").change(function () {
        $.ajax({
            url: "/PedidoCompra/ChangeDireccionEntregaDescripcion",
            type: 'POST',
            data: {
                direccionEntregaDescripcion: $("#pedido_direccionEntrega_descripcion").val()
            },
            success: function () { }
        });
    });

    $("#pedido_direccionEntrega_contacto").change(function () {
        $.ajax({
            url: "/PedidoCompra/ChangeDireccionEntregaContacto",
            type: 'POST',
            data: {
                direccionEntregaContacto: $("#pedido_direccionEntrega_contacto").val()
            },
            success: function () { }
        });
    });
    
    $("#pedido_direccionEntrega_telefono").change(function () {
        $.ajax({
            url: "/PedidoCompra/ChangeDireccionEntregaTelefono",
            type: 'POST',
            data: {
                direccionEntregaTelefono: $("#pedido_direccionEntrega_telefono").val()
            },
            success: function () { }
        });
    });






    $('#pedido_solicitante').change(function () {
        toggleControlesSolicitante();
        var idSolicitante = $('#pedido_solicitante').val();
        $.ajax({
            url: "/PedidoCompra/ChangeSolicitante",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idSolicitante: idSolicitante
            },
            success: function (solicitante) {
                $("#pedido_solicitante_nombre").val(solicitante.nombre);
                $("#pedido_solicitante_telefono").val(solicitante.telefono);
                $("#pedido_solicitante_correo").val(solicitante.correo);
            }
        })
    });

    $("#pedido_solicitante_nombre").change(function () {
        $.ajax({
            url: "/PedidoCompra/ChangeSolicitanteNombre",
            type: 'POST',
            data: {
                solicitanteNombre: $("#pedido_solicitante_nombre").val()
            },
            success: function () { }
        });
    });


    $("#pedido_solicitante_telefono").change(function () {
        $.ajax({
            url: "/PedidoCompra/ChangeSolicitanteTelefono",
            type: 'POST',
            data: {
                solicitanteTelefono: $("#pedido_solicitante_telefono").val()
            },
            success: function () { }
        });
    });

    $("#pedido_solicitante_correo").change(function () {
        $.ajax({
            url: "/PedidoCompra/ChangeSolicitanteCorreo",
            type: 'POST',
            data: {
                solicitanteCorreo: $("#pedido_solicitante_correo").val()
            },
            success: function () { }
        });
    });




    $(".fechaSolicitud").change(function () {
        var fechaSolicitud = $("#fechaSolicitud").val();
        var horaSolicitud = $("#horaSolicitud").val();
        $.ajax({
            url: "/PedidoCompra/ChangeFechaSolicitud",
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
            url: "/PedidoCompra/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#pedido_sku").change(function () {
        changeInputString("sku", $("#pedido_sku").val())
    });

    $("#pedido_horaEntregaDesde").change(function () {

        var horaEntregaDesde = $("#pedido_horaEntregaDesde").val();
        var horaEntregaHasta = $("#pedido_horaEntregaHasta").val();

        //Si la hora de entrega desde tiene una diferencia menor a dos horas con la hora entrega hasta
        //Se reemplaza la hora de entraga hasta con la hora entrega desde más dos horas
        if (convertirHoraToNumero(horaEntregaDesde) + 2 > convertirHoraToNumero(horaEntregaHasta)) {
            $("#pedido_horaEntregaHasta").val(sumarHoras($("#pedido_horaEntregaDesde").val(),2));
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

    /*
    $("#pedido_observacionesFactura").change(function () {
        changeInputString("observacionesFactura", $("#pedido_observacionesFactura").val())
    });*/

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
        //$('#familia').val("Todas");
        //$('#familia').change();
        //$('#proveedor').val("Todos");
        //$('#proveedor').change();

        //$('#producto').trigger('chosen:activate');
    })


    /////////////CAMPO PRODUCTO 
    $("#producto").change(function () {
        $("#resultadoAgregarProducto").html("");
        desactivarBtnAddProduct();
        $.ajax({
            url: "/PedidoCompra/GetProducto",
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

                for (var i = 0; i < producto.productoPresentacionList.length; i++) {
                    var reg = producto.productoPresentacionList[i];
                    options = options + "<option value='" + reg.IdProductoPresentacion + "'  precioUnitarioAlternativoSinIGV='" + reg.PrecioSinIGV + "' costoAlternativoSinIGV='" + reg.CostoSinIGV + "' >" + reg.Presentacion + "</option>";
                }

                //Limpieza de campos
                $("#costoLista").val(Number(producto.costoLista));
                $("#precioLista").val(Number(producto.precioLista));
                $("#unidad").html(options);
                $("#proveedor").val(producto.proveedor);
                $("#familia").val(producto.familia);
                $('#precioUnitarioSinIGV').val(producto.precioUnitarioSinIGV);
                $('#precioUnitarioAlternativoSinIGV').val(producto.precioUnitarioAlternativoSinIGV);
                $('#costoSiniGV').val(producto.costoSinIGV);
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
            //precioLista = Number($("#precioUnitarioAlternativoSinIGV").val());
            //costoLista = Number($("#costoAlternativoSinIGV").val());

            precioLista = $("#unidad option:selected").attr("precioUnitarioAlternativoSinIGV");
            costoLista = $("#unidad option:selected").attr("costoAlternativoSinIGV");
        }

        if ($("input[name=igv]:checked").val() == 1) {
            precioLista = (precioLista + (precioLista * IGV)).toFixed(cantidadDecimales);
            costoLista = (costoLista + (costoLista * IGV)).toFixed(cantidadDecimales);
        }


        //$("#precioLista").val(precioLista);

        $("#precioLista").val(costoLista);
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

        var idCliente = "";
        var sessionPedido = "pedido"
        if ($("#pagina").val() == PAGINA_BUSQUEDA_PEDIDOS_COMPRA) {
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
        }

        //verIdCliente


        $.ajax({
            url: "/Precio/GetPreciosRegistrados",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: idProducto,
                idCliente: idCliente,
                controller: sessionPedido
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
    


    $("#considerarDescontinuados").change(function () {
        var considerarDescontinuados = $('#considerarDescontinuados').prop('checked');
        $.ajax({
            url: "/PedidoCompra/updateConsiderarDescontinuados",
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
        var esPrecioAlternativo = 0;
        var idProductoPresentacion = Number($("#unidad").val());
        if (idProductoPresentacion > 0)
            esPrecioAlternativo = 1;    
        var subtotal = $("#subtotal").val();
        var incluidoIGV = $("input[name=igv]:checked").val();
        var proveedor = $("#proveedor").val();
        var flete = Number($("#fleteDetalle").val());
        var observacion = $("#observacionProducto").val();
        var costo = $("#costoLista").val();


        $.ajax({
            url: "/PedidoCompra/AddProducto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                cantidad: cantidad,
                porcentajeDescuento: porcentajeDescuento,
                precio: precio,
                costo: costo,
                esPrecioAlternativo: esPrecioAlternativo,
                idProductoPresentacion: idProductoPresentacion,
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
                        && detalle.precioUnitario <= Number(precioLista) + Number(VARIACION_PRECIO_ITEM_PEDIDO))
                    {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right">' + detalle.precioUnitario + '</td>';
                        
                    }
                    else
                    {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right; color: #B9371B; font-weight:bold">' + detalle.precioUnitario + '</td>';

                    }


                }
                else {
                    

                    if (Number(detalle.precioUnitario) >= (Number(detalle.precioUnitarioRegistrado) - Number(VARIACION_PRECIO_ITEM_PEDIDO))
                        && Number(detalle.precioUnitario) <= (Number(detalle.precioUnitarioRegistrado) + Number(VARIACION_PRECIO_ITEM_PEDIDO)))
                    {
                        precios = '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right">' + detalle.precioUnitario + '</td>';
                        
                    }
                    else
                    {
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
                    '<td class="' + detalle.idProducto + ' detbtnMostrarPrecios"> <button  type="button" class="' + detalle.idProducto+' btnMostrarPrecios btn btn-primary bouton-image botonPrecios"></button></td>' +


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
            if (esPrecioAlternativo >= 1) {
                //var nuevoPrecioInicial = Number(Number($("#precioUnitarioAlternativoSinIGV").val()).toFixed(cantidadCuatroDecimales));
                var nuevoPrecioInicial = Number(Number($("#unidad option:selected").attr("precioUnitarioAlternativoSinIGV")).toFixed(cantidadDecimalesPrecioNeto));
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
            url: "/PedidoCompra/obtenerProductosAPartirdePreciosRegistrados",
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
                //window.location = '/PedidoCompra/Cotizador';
            },
            success: function () {
                window.location = '/PedidoCompra/Pedir';
            }
        });

    });

    





    ////////CREAR/EDITAR COTIZACIÓN

    function validarIngresoDatosObligatoriosPedido() {


        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            $("#idCiudad").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe seleccionar la sede MP previamente.',
                buttons: {
                    OK: function () {
                    }
                }
            });
            return false;
        }

        if ($("#idCliente").val().trim() == "") {

            $('#idCliente').trigger('chosen:activate');
            $("#idCiudadAsolicitar").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe seleccionar un cliente.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }


        var tipoPedido = $("#pedido_tipoPedido").val();

        if ($("#pedido_numeroReferenciaCliente").val().length > 20) {
            $("#pedido_numeroReferenciaCliente").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'El número de referencia del cliente no debe contener más de 20 caracteres, si el dato a ingresar es más extenso agreguelo en Observaciones Factura.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }

        /*if ($("#pedido_numeroReferenciaCliente").val().trim() == "") {
            alert('Debe ingresar el número de orden de compra o pedido en el campo "Referencia Doc Cliente".');
            $('#pedido_numeroReferenciaCliente').focus();
            return false;
        }*/

        


        if (tipoPedido != TIPO_PEDIDO_COMPRA_COMPRA.charCodeAt(0)
            //&& tipoPedido != TIPO_PEDIDO_COMPRA_TRASLADO_INTERNO_RECIBIDO.charCodeAt(0)
            && tipoPedido != TIPO_PEDIDO_COMPRA_COMODATO_RECIBIDO.charCodeAt(0)
            && tipoPedido != TIPO_PEDIDO_COMPRA_TRANSFERENCIA_GRATUITA_RECIBIDA.charCodeAt(0)
          //  && tipoPedido != TIPO_PEDIDO_COMPRA_PRESTAMO_RECIBIDO.charCodeAt(0)
        ) {


            if ($("#ActualDepartamento").val().trim().length == 0) {
                $("#ActualDepartamento").focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar el departamento.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }
            if ($("#ActualProvincia").val().trim().length == 0) {
                $("#ActualProvincia").focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar la provincia.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }
            if ($("#ActualDistrito").val().trim().length == 0) {
                $("#ActualDistrito").focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar el distrito.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }


            if ($("#pedido_direccionEntrega").val().trim() == "") {
                $('#pedido_direccionEntrega').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe seleccionar la dirección de entrega.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }

            if ($("#pedido_solicitante").val().trim() == "") {
                $('#pedido_solicitante').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe seleccionar el solicitante.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }

            if ($("#pedido_direccionEntrega_descripcion").val().trim() == "") {
                $('#pedido_direccionEntrega_descripcion').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar la dirección de entrega.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }

            if ($("#pedido_direccionEntrega_contacto").val().trim() == "") {
                $('#pedido_direccionEntrega_contacto').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar el contacto de entrega.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }

            if ($("#pedido_direccionEntrega_telefono").val().trim() == "") {
                $('#pedido_direccionEntrega_telefono').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar una el telefono del contacto de entrega.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }
        }

        var fechaSolicitud = $("#fechaSolicitud").val();
        if (fechaSolicitud.trim() == "") {
            $("#fechaSolicitud").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe ingresar la fecha de la solicitud.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }

        var horaSolicitud = $("#horaSolicitud").val();
        if (horaSolicitud == null || horaSolicitud.trim() == "") {
            $("#horaSolicitud").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe ingresar la hora de la solicitud.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }


        var fechaEntregaDesde = $("#pedido_fechaEntregaDesde").val();
        if (fechaEntregaDesde.trim() == "") {
            $("#pedido_fechaEntregaDesde").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe ingresar la fecha desde cuando se puede realizar la entrega.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }

        var fechaEntregaHasta = $("#pedido_fechaEntregaHasta").val();
        if (fechaEntregaHasta.trim() == "") {
            $("#pedido_fechaEntregaHasta").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe ingresar la fecha hasta cuando se puede realizar la entrega.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }


        //la fecha máxima de entrega no puede ser inferior a la fecha de entrega
        if (convertirFechaNumero(fechaEntregaHasta) < convertirFechaNumero(fechaEntregaDesde)) {
            $("#fechaEntregaHasta").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'La fecha entrega hasta debe ser mayor o igual a la fecha de entrega desde.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }



        if (tipoPedido != TIPO_PEDIDO_COMPRA_COMPRA.charCodeAt(0)
          //  && tipoPedido != TIPO_PEDIDO_COMPRA_TRASLADO_INTERNO_RECIBIDO.charCodeAt(0)
            && tipoPedido != TIPO_PEDIDO_COMPRA_COMODATO_RECIBIDO.charCodeAt(0)
            && tipoPedido != TIPO_PEDIDO_COMPRA_TRANSFERENCIA_GRATUITA_RECIBIDA.charCodeAt(0)
        //    && tipoPedido != TIPO_PEDIDO_COMPRA_PRESTAMO_RECIBIDO.charCodeAt(0)
        ) {

            if ($("#pedido_solicitante_nombre").val().trim() == "") {
                $('#pedido_solicitante_nombre').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar el nombre de la persona que realizó la solicitud.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }



            if ($("#pedido_solicitante_telefono").val().trim() == "" && $("#pedido_solicitante_correo").val().trim() == "") {
                $('#pedido_solicitante_telefono').focus();
                $.alert({
                    title: TITLE_VALIDACION_PEDIDO,
                    content: 'Debe ingresar un telefono y/o correo de contacto de entrega.',
                    buttons: {
                        OK: function () { }
                    }
                });
                return false;
            }
        }


        if ($("#pedido_observacionesGuiaRemision").val().length > 200) {
            $("#pedido_observacionesGuiaRemision").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'El campo observaciones guía de remisión no debe contener más de 200 caracteres.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }

        /*if ($("#pedido_observacionesFactura").val().length > 200) {
            $("#pedido_observacionesFactura").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'El campo observaciones factura no debe contener más de 200 caracteres.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }*/


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
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'Debe ingresar el detalle del pedido.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }

        return true;
    }


    function mostrarMensajeErrorProceso() {
        $.alert({
            //icon: 'fa fa-warning',
            title: 'Error',
            content: MENSAJE_ERROR,
            type: 'red',
            buttons: {
                OK: function () { }
            }
        });       
    }


    $('input[name=filePedidos]').change(function (e) {
        //$('#nombreArchivos').val(e.currentTarget.files);
        var numFiles = e.currentTarget.files.length;
        var nombreArchivos = "";
        for (i = 0; i < numFiles; i++) {
            var fileFound = 0;
            $("#nombreArchivos > li").each(function (index) {

                if ($(this).find("a.descargar")[0].text == e.currentTarget.files[i].name) {
                    alert('El archivo "' + e.currentTarget.files[i].name + '" ya se encuentra agregado.');
                    fileFound = 1;
                }
            });

            if (fileFound == 0) {

                var liHTML = '<a href="javascript:mostrar();" class="descargar">' + e.currentTarget.files[i].name + '</a>' +
                    '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + e.currentTarget.files[i].name + '" class="btnDeleteArchivo" /></a>';

                $('#nombreArchivos').append($('<li />').html(liHTML));


                ///  $('<li />').text(e.currentTarget.files[i].name).appendTo($('#nombreArchivos'));
            }

        }
    });


    $('input:file[multiple]').change(   function (e) {
        //$('#nombreArchivos').val(e.currentTarget.files);

        var numFiles = e.currentTarget.files.length;
        var nombreArchivos = "";
        for (i = 0; i < numFiles; i++) {

            var fileFound = 0;
            $("#nombreArchivos > li").each(function (index) {           

                if ($(this).find("a.descargar")[0].text == e.currentTarget.files[i].name) {
                    alert('El archivo "' + e.currentTarget.files[i].name + '" ya se encuentra agregado.');
                    fileFound = 1;
                }
            });


            if (fileFound == 0)
            {

                var liHTML = '<a href="javascript:mostrar();" class="descargar">' + e.currentTarget.files[i].name + '</a>' +
                    '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + e.currentTarget.files[i].name +'" class="btnDeleteArchivo" /></a>';

                $('#nombreArchivos').append($('<li />').html(liHTML));
                

              ///  $('<li />').text(e.currentTarget.files[i].name).appendTo($('#nombreArchivos'));
            }

        }


        var data = new FormData($('#formularioConArchivos')[0]);

        $.ajax({
            url: "/PedidoCompra/ChangeFiles",
            type: 'POST',
            enctype: 'multipart/form-data',
            contentType: false,
            processData: false,
            data: data,
            error: function (detalle) {    },
            success: function (resultado) {     }
        });



    });


    
    $(document).on('click', ".btnDeleteArchivo", function () {

        var nombreArchivo = event.target.id;




    //$("#btnDeleteArchivo").click(function () {
        $("#files").val("");
        $("#nombreArchivos > li").remove().end();
        $.ajax({
            url: "/PedidoCompra/DescartarArchivos",
            type: 'POST',
            dataType: 'JSON',
            data: { nombreArchivo: nombreArchivo},
            error: function (detalle) { },
            success: function (pedidoAdjuntoList) {

                $("#nombreArchivos > li").remove().end();

                for (var i = 0; i < pedidoAdjuntoList.length; i++) {

                    var liHTML = '<a href="javascript:mostrar();" class="descargar">' + pedidoAdjuntoList[i].nombre + '</a>' +
                        '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + pedidoAdjuntoList[i].nombre + '" class="btnDeleteArchivo" /></a>';

                    $('#nombreArchivos').append($('<li />').html(liHTML));
              //      .appendTo($('#nombreArchivos'));

                }


            }
        });
    });

    $(document).on('click', "a.descargar", function () {

        //var arrrayClass = event.target.getAttribute("class").split(" ");
        var nombreArchivo = event.target.innerHTML;
        //var numeroPedido = arrrayClass[1];

        $.ajax({
            url: "/PedidoCompra/Descargar",
            type: 'POST',
            //  enctype: 'multipart/form-data',
            dataType: 'JSON',
            //  contentType: 'multipart/form-data',
            data: { nombreArchivo: nombreArchivo },
            error: function (detalle) {
                alert(detalle);
            },
            success: function (pedidoAdjunto) {
                var sampleArr = base64ToArrayBuffer(pedidoAdjunto.adjunto);
                saveByteArray(nombreArchivo, sampleArr);
            }
        });
    });

    /*

    */


    function crearPedido(continuarLuego) {
        if (!validarIngresoDatosObligatoriosPedido())
            return false;


        $('body').loadingModal({
            text: 'Creando Pedido...'
        });
        $.ajax({
            url: "/PedidoCompra/Create",
            type: 'POST',
          //  enctype: 'multipart/form-data',
            dataType: 'JSON',
          //  contentType: 'multipart/form-data',
            data: { continuarLuego: continuarLuego},
            error: function (detalle) {
                $('body').loadingModal('hide');
                mostrarMensajeErrorProceso();
            },
            success: function (resultado) {
                $('body').loadingModal('hide')
                $("#pedido_numeroPedido").val(resultado.numeroPedido);
                $("#idPedido").val(resultado.idPedido);

                if (resultado.estado == ESTADO_INGRESADO) {
                    $.alert({
                        title: TITLE_EXITO,
                        type: 'green',
                        content: "El pedido número " + resultado.numeroPedido + " fue ingresado correctamente.",
                        buttons: {
                            OK: function () { window.location = '/PedidoCompra/Index';  }
                        }
                    });
                     
                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    //alert("El pedido número " + resultado.numeroPedido + " fue ingresado correctamente, sin embargo requiere APROBACIÓN")
                    $("#solicitudIngresoComentario").html("El pedido número " + resultado.numeroPedido + " fue ingresado correctamente, sin embargo requiere APROBACIÓN, debe ingresar un comentario.")
                    $("#comentarioPendienteIngreso").val(resultado.observacion);
                    $("#modalComentarioPendienteIngreso").modal({
                        show: true,
                        keyboard: false,
                        backdrop: 'static'
                    });
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    ConfirmDialog("El pedido número " + resultado.numeroPedido + " fue guardado correctamente. ¿Desea continuar editando ahora?", null, '/PedidoCompra/CancelarCreacionPedido');
                }
                else {
                    //alert("El pedido ha tenido problemas para ser procesado; Contacte con el Administrador.");
                    mostrarMensajeErrorProceso();
                    window.location = '/PedidoCompra/Index';
                }

            }
        });
    }

    function editarPedido(continuarLuego) {
        if (!validarIngresoDatosObligatoriosPedido())
            return false;
        $('body').loadingModal({
            text: 'Editando Pedido...'
        });
        $.ajax({
            url: "/PedidoCompra/Update",
            type: 'POST',
            dataType: 'JSON',
            data: {
                continuarLuego: continuarLuego
            },
            error: function (detalle) {
                $('body').loadingModal('hide')
                mostrarMensajeErrorProceso();
            },
            success: function (resultado) {
                $('body').loadingModal('hide')
                $("#pedido_numeroPedido").val(resultado.numeroPedido);
                $("#idPedido").val(resultado.idPedido);

                if (resultado.estado == ESTADO_INGRESADO) {
                    //alert("El pedido número " + resultado.numeroPedido + " fue editado correctamente.");
                    $.alert({
                        title: TITLE_EXITO,
                        type: 'green',
                        content: "El pedido número " + resultado.numeroPedido + " fue editado correctamente.",
                        buttons: {
                            OK: function () { window.location = '/PedidoCompra/Index'; }
                        }
                    });
                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    //alert("El pedido número " + resultado.numeroPedido + " fue editado correctamente, sin embargo requiere APROBACIÓN")
                    $("#solicitudIngresoComentario").html("El pedido número " + resultado.numeroPedido + " fue editado correctamente, sin embargo requiere APROBACIÓN, debe ingresar un comentario.")
                    $("#comentarioPendienteIngreso").val(resultado.observacion);
                    $("#modalComentarioPendienteIngreso").modal({
                        show: true,
                        keyboard: false,
                        backdrop: 'static'
                    });
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    ConfirmDialog("El pedido número " + resultado.numeroPedido + " fue guardado correctamente. ¿Desea continuar editando ahora?", null, '/PedidoCompra/CancelarCreacionPedido');
                }
                else {
                    //alert("El pedido ha tenido problemas para ser procesado; Contacte con el Administrador.");
                    mostrarMensajeErrorProceso();
                    window.location = '/PedidoCompra/Index';
                }
            }
        });
    }

    $("#btnCancelarComentario").click(function () {
        window.location = '/PedidoCompra/CancelarCreacionPedido';
    });

    $("#btnAceptarComentario").click(function () {
        var codigoPedido = $("#pedido_numeroPedido").val();
        var idPedido = $("#idPedido").val();
        var observacion = $("#comentarioPendienteIngreso").val();
        $.ajax({
            url: "/PedidoCompra/updateEstadoPedido",
            data: {
                idPedido: idPedido,
                estado: ESTADO_PENDIENTE_APROBACION,
                observacion: observacion
            },
            type: 'POST',
            error: function () {
                mostrarMensajeErrorProceso();
                $("#btnCancelarComentario").click();
            },
            success: function () {
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El comentario del estado del pedido número: " + codigoPedido + " fue modificado.",
                    buttons: {
                        OK: function () { window.location = '/Pedido/Index'; }
                    }
                });
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



    $("#btnFinalizarCreacionPedido").click(function () {
        crearPedido(0);
    });


    $("#btnFinalizarEdicionPedido").click(function () {
        editarPedido(0);
    });

    
    $("#btnContinuarEditandoLuego").click(function () {
        if ($("#pedido_numeroPedido").val() == "" || $("#pedido_numeroPedido").val() == null) {
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

    function invertirFormatoFecha(fecha)
    {
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
        hora = hora[0] +"."+ hora[1];
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

        $('body').loadingModal({
            text: 'Abriendo Pedido...'
        });
        $('body').loadingModal('show');

        activarBotonesVer();
        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idPedido = arrrayClass[0];
        var numeroPedido = arrrayClass[1];
      //  $("#tableDetalleCotizacion > tbody").empty();
     

        $.ajax({
            url: "/PedidoCompra/Show",
            data: {
                idPedido: idPedido
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                //alert("Ocurrió un problema al obtener el detalle del Pedido N° " + numeroPedido + ".");
                $('body').loadingModal('hide');
                mostrarMensajeErrorProceso();
            },
            success: function (resultado) {
                //var cotizacion = $.parseJSON(respuesta);
                $('body').loadingModal('hide');

                var pedido = resultado.pedido;
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
                $("#verTipoPedido").html(pedido.tiposPedidoCompraString);

                $("#verResponsableComercial").html(pedido.cliente.responsableComercial.codigoDescripcion + " " + pedido.cliente.responsableComercial.usuario.email);
                $("#verSupervisorComercial").html(pedido.cliente.supervisorComercial.codigoDescripcion + " " + pedido.cliente.supervisorComercial.usuario.email);
                $("#verAsistenteServicioCliente").html(pedido.cliente.asistenteServicioCliente.codigoDescripcion + " " + pedido.cliente.asistenteServicioCliente.usuario.email);              


                            
                $("#verFechaHorarioEntrega").html(pedido.fechaHorarioEntrega);

                $("#verCiudad").html(pedido.ciudad.nombre);                
                $("#verIdCliente").val(pedido.cliente.idCliente);
                $("#verCliente").html(pedido.cliente.codigoRazonSocial);

                $("#verGrupoCliente").html(pedido.grupoCliente_nombre == null ? "" : pedido.grupoCliente_nombre);

                $("#verNumeroReferenciaCliente").html(pedido.numeroReferenciaCliente);
                $("#verNumeroReferenciaAdicional").html(pedido.numeroReferenciaAdicional);
                $("#verDireccionEntrega").html(pedido.direccionEntrega.descripcion);
                $("#verTelefonoContactoEntrega").html(pedido.direccionEntrega.telefono);
                $("#verContactoEntrega").html(pedido.direccionEntrega.contacto);

                $("#verUsuarioCreacion").html(pedido.usuario.nombre);
                $("#verFechaHoraRegistro").html(pedido.fechaHoraRegistro);

                $("#verUbigeoEntrega").html(pedido.ubigeoEntrega.ToString);

                $("#verContactoPedido").html(pedido.contactoPedido);
                $("#verTelefonoCorreoContactoPedido").html(pedido.telefonoCorreoContactoPedido);

               


                $("#verFechaHoraSolicitud").html(pedido.fechaHoraSolicitud);

                $("#verEstado").html(pedido.seguimientoPedido.estadoString);
                $("#verModificadoPor").html(pedido.seguimientoPedido.usuario.nombre);
                $("#verObservacionEstado").html(pedido.seguimientoPedido.observacion);

                $("#verEstadoCrediticio").html(pedido.seguimientoCrediticioPedido.estadoString);
                $("#verModificadoCrediticioPor").html(pedido.seguimientoCrediticioPedido.usuario.nombre);
                $("#verObservacionEstadoCrediticio").html(pedido.seguimientoCrediticioPedido.observacion);
                
          
                $("#verObservaciones").html(pedido.observaciones);
                $("#verObservacionesFactura").html(pedido.observacionesFactura);
                $("#verObservacionesGuiaRemision").html(pedido.observacionesGuiaRemision);
                $("#verMontoSubTotal").html(Number(pedido.montoSubTotal).toFixed(cantidadDecimales));
                $("#verMontoIGV").html(Number(pedido.montoIGV).toFixed(cantidadDecimales));
                $("#verMontoTotal").html(Number(pedido.montoTotal).toFixed(cantidadDecimales));
                $("#documentoVenta_observaciones").val(pedido.observacionesFactura);

          /*      $("#verMontoSubTotalVenta").html(Number(pedido.venta.subTotal).toFixed(cantidadDecimales));
                $("#verMontoIGVVenta").html(Number(pedido.venta.igv).toFixed(cantidadDecimales));
                $("#verMontoTotalVenta").html(Number(pedido.venta.total).toFixed(cantidadDecimales));
*/
              //  nombreArchivos

                $("#nombreArchivos > li").remove().end();


                for (var i = 0; i < pedido.pedidoAdjuntoList.length; i++) {
                    var liHTML = '<a href="javascript:mostrar();" class="descargar">' + pedido.pedidoAdjuntoList[i].nombre + '</a>';
                    $('<li />').html(liHTML).appendTo($('#nombreArchivos'));
                }   


                $("#verNombreArchivos > li").remove().end();


                for (var i = 0; i < pedido.pedidoAdjuntoList.length; i++) {
                    var liHTML = '<a href="javascript:mostrar();" class="descargar">' + pedido.pedidoAdjuntoList[i].nombre + '</a>';
                    //$('<li />').html(liHTML).appendTo($('#nombreArchivos'));
                    $('#verNombreArchivos').append($('<li />').html(liHTML));
                }     

              
                $("#tableDetallePedido > tbody").empty();

                FooTable.init('#tableDetallePedido');

                $("#formVerGuiasRemision").html("");
                $("#formVerNotasIngreso").html("");

                var d = '';
                var lista = pedido.pedidoDetalleList;
                for (var i = 0; i < lista.length; i++) {

                    var observacion = lista[i].observacion == null || lista[i].observacion == 'undefined'? '' : lista[i].observacion;

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
                 //       '<td>' + lista[i].precioUnitarioVenta.toFixed(cantidadCuatroDecimales) + '</td>' +
                        '<td>' + lista[i].cantidad + '</td>' +
                        '<td>' + lista[i].cantidadPendienteAtencion + '</td>' +
                        '<td>' + lista[i].subTotal.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + observacion + '</td>' +
                        '<td class="' + lista[i].producto.idProducto + ' detbtnMostrarPrecios"> <button  type="button" class="' + lista[i].producto.idProducto + ' btnMostrarPrecios btn btn-primary bouton-image botonPrecios"></button></td>' +
                        '</tr>';

                   

                }




                $("#verRazonSocialSunat").html(pedido.cliente.razonSocialSunat);
                $("#verRUC").html(pedido.cliente.ruc);
                $("#verDireccionDomicilioLegalSunat").html(pedido.cliente.direccionDomicilioLegalSunat);
                $("#verCodigo").html(pedido.cliente.codigo);

                $("#documentoVenta_observaciones").val(pedido.observacionesFactura);
                $("#verCorreoEnvioFactura").html(pedido.cliente.correoEnvioFactura);


                               
                
                if (pedido.tipoPedidoCompra == TIPO_PEDIDO_COMPRA_COMPRA.charCodeAt(0)
                    //|| pedido.tipoPedidoCompra == TIPO_PEDIDO_COMPRA_TRASLADO_INTERNO_RECIBIDO.charCodeAt(0)
                    || pedido.tipoPedidoCompra == TIPO_PEDIDO_COMPRA_COMODATO_RECIBIDO.charCodeAt(0)
                    || pedido.tipoPedidoCompra == TIPO_PEDIDO_COMPRA_TRANSFERENCIA_GRATUITA_RECIBIDA.charCodeAt(0)
                   // || pedido.tipoPedidoCompra == TIPO_PEDIDO_COMPRA_PRESTAMO_RECIBIDO.charCodeAt(0)
                ) {
                    var notaIngresoList = pedido.guiaRemisionList;
                    for (var j = 0; j < notaIngresoList.length; j++) {
                        $("#tableDetalleNotaIngreso > tbody").empty();
                        var plantilla = $("#plantillaVerNotaIngreso").html();
                        var dGuia = '';
                        var documentoDetalleList = notaIngresoList[j].documentoDetalle;
                        for (var k = 0; k < documentoDetalleList.length; k++) {

                            dGuia += '<tr>' +
                                '<td>' + documentoDetalleList[k].producto.sku + '</td>' +
                                '<td>' + documentoDetalleList[k].cantidad + '</td>' +
                                '<td>' + documentoDetalleList[k].unidad + '</td>' +
                                '<td>' + documentoDetalleList[k].producto.descripcion + '</td>' +
                                '</tr>';
                        }

                        $("#tableDetalleNotaIngreso").append(dGuia);

                        plantilla = $("#plantillaVerNotaIngreso").html();

                        plantilla = plantilla.replace("#serieNumero", notaIngresoList[j].serieNumeroGuia);
                        plantilla = plantilla.replace("#fechaEmisionNotaIngreso", invertirFormatoFecha(notaIngresoList[j].fechaEmision.substr(0, 10)));

                        plantilla = plantilla.replace("tableDetalleNotaIngreso", "tableDetalleNotaIngreso" + j);

                        $("#formVerNotasIngreso").append(plantilla);

                        /*   var tmp = $("#formVerGuiasRemision").html();
                           tmp = tmp+"asas";*/
                    }
                  

                }
                else {

                    var guiaRemisionList = pedido.guiaRemisionList;
                    for (var j = 0; j < guiaRemisionList.length; j++) {
                        $("#tableDetalleGuia > tbody").empty();
                        var plantilla = $("#plantillaVerGuiasRemision").html();
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

                        $("#tableDetalleGuia").append(dGuia);

                        plantilla = $("#plantillaVerGuiasRemision").html();

                        plantilla = plantilla.replace("#serieNumero", guiaRemisionList[j].serieNumeroGuia);
                        plantilla = plantilla.replace("#fechaEmisionGuia", invertirFormatoFecha(guiaRemisionList[j].fechaEmision.substr(0, 10)));

                        plantilla = plantilla.replace("#serieNumeroFactura", guiaRemisionList[j].documentoVenta.serieNumero);
                        if (guiaRemisionList[j].documentoVenta.fechaEmision != null) {
                            plantilla = plantilla.replace("#fechaEmisionFactura", invertirFormatoFecha(guiaRemisionList[j].documentoVenta.fechaEmision.substr(0, 10)));
                        }
                        else
                            plantilla = plantilla.replace("#fechaEmisionFactura", "");


                        plantilla = plantilla.replace("tableDetalleGuia", "tableDetalleGuia" + j);

                        $("#formVerGuiasRemision").append(plantilla);

                        /*   var tmp = $("#formVerGuiasRemision").html();
                           tmp = tmp+"asas";*/
                    }
                  

                }
               

                


              //  
               // sleep
                $("#tableDetallePedido").append(d);

                if (pedido.seguimientoPedido.estado != ESTADO_PROGRAMADO
                    && pedido.seguimientoPedido.estado != ESTADO_ATENDIDO
                    && pedido.seguimientoPedido.estado != ESTADO_FACTURADO
                    && pedido.seguimientoPedido.estado != ESTADO_FACTURADO_PARCIALMENTE
                    && pedido.seguimientoPedido.estado != ESTADO_ATENDIDO_PARCIALMENTE
                    && pedido.seguimientoPedido.estado != ESTADO_RECIBIDO
                    && pedido.seguimientoPedido.estado != ESTADO_RECIBIDO_PARCIALMENTE
                ) {
                    $("#btnEditarPedido").show();
                    if (pedido.seguimientoPedido.estado == ESTADO_EN_EDICION) {
                        $("#btnEditarPedido").html("Continuar Editando");
                    }
                    else {
                        $("#btnEditarPedido").html("Editar");
                    }
                    $("#btnEditarPedido").show();
                }
                else {
                    $("#btnEditarPedido").hide();
                }

                //ACTUALIZAR PEDIDO
                if ( pedido.seguimientoPedido.estado != ESTADO_EN_EDICION
                    && pedido.seguimientoPedido.estado != ESTADO_PENDIENTE_APROBACION
                    && pedido.seguimientoPedido.estado != ESTADO_DENEGADO
                    && pedido.seguimientoPedido.estado != ESTADO_ELIMINADO) {
                    $("#btnActualizarPedido").show();
                }
                else {
                    $("#btnActualizarPedido").hide();
                }






                //APROBAR PEDIDO
                if (
                    (pedido.seguimientoPedido.estado == ESTADO_PENDIENTE_APROBACION ||
                        pedido.seguimientoPedido.estado == ESTADO_DENEGADO)
                ) {

                    $("#btnAprobarIngresoPedido").show();
                }
                else {
                    $("#btnAprobarIngresoPedido").hide();
                }


                //DENEGAR PEDIDO
                if (pedido.seguimientoPedido.estado == ESTADO_PENDIENTE_APROBACION)
                {

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

                    $("#btnLiberarPedido").show();
                }
                else {
                    $("#btnLiberarPedido").hide();
                }

                //BLOQUEAR PEDIDO
                if (
                    (pedido.seguimientoCrediticioPedido.estado == ESTADO_PENDIENTE_LIBERACION ||
                        pedido.seguimientoCrediticioPedido.estado == ESTADO_LIBERADO)
                    && pedido.seguimientoPedido.estado != ESTADO_ELIMINADO
                    && pedido.seguimientoPedido.estado != ESTADO_EN_EDICION
                    && pedido.seguimientoPedido.estado != ESTADO_DENEGADO
                    && pedido.seguimientoPedido.estado != ESTADO_ATENDIDO
                    && pedido.seguimientoPedido.estado != ESTADO_ATENDIDO_PARCIALMENTE
                    && pedido.seguimientoPedido.estado != ESTADO_FACTURADO
                    && pedido.seguimientoPedido.estado != ESTADO_FACTURADO_PARCIALMENTE
                    && pedido.seguimientoPedido.estado != ESTADO_RECIBIDO
                    && pedido.seguimientoPedido.estado != ESTADO_RECIBIDO_PARCIALMENTE
                ) {

                    $("#btnBloquearPedido").show();
                }
                else {
                    $("#btnBloquearPedido").hide();
                }



                //ATENDER PEDIDO
                $("#btnAtenderPedidoCompra").hide();
                $("#btnIngresarPedidoCompra").hide();
                if ((pedido.seguimientoPedido.estado == ESTADO_INGRESADO ||
                    pedido.seguimientoPedido.estado == ESTADO_PROGRAMADO ||
                    pedido.seguimientoPedido.estado == ESTADO_ATENDIDO_PARCIALMENTE
                    )&&
                    pedido.seguimientoCrediticioPedido.estado == ESTADO_LIBERADO
                ) {
                    if (pedido.tipoPedidoCompra == TIPO_PEDIDO_COMPRA_COMPRA.charCodeAt(0)
                     //   || pedido.tipoPedidoCompra == TIPO_PEDIDO_COMPRA_TRASLADO_INTERNO_RECIBIDO.charCodeAt(0)
                        || pedido.tipoPedidoCompra == TIPO_PEDIDO_COMPRA_COMODATO_RECIBIDO.charCodeAt(0)
                        || pedido.tipoPedidoCompra == TIPO_PEDIDO_COMPRA_TRANSFERENCIA_GRATUITA_RECIBIDA.charCodeAt(0)
                      //  || pedido.tipoPedidoCompra == TIPO_PEDIDO_COMPRA_PRESTAMO_RECIBIDO.charCodeAt(0)
                    ) {
                        $("#btnIngresarPedidoCompra").show();
                    }
                    else { $("#btnAtenderPedidoCompra").show(); }
                }

                //CANCELAR PROGRAMACION
                if (pedido.seguimientoPedido.estado == ESTADO_PROGRAMADO)
                {
                    $("#btnCancelarProgramacionPedido").show();
                }
                else {
                    $("#btnCancelarProgramacionPedido").hide();
                }


                //PROGRAMAR PEDIDO
                if ((pedido.seguimientoPedido.estado == ESTADO_ATENDIDO)
                    && (pedido.documentoVenta.numero == "" || pedido.documentoVenta.numero == null )
                    ) {
                    $("#btnEditarVenta").show();
                   // $("#btnFacturarPedido").show();
                }
                else {
                    $("#btnEditarVenta").hide();
                   // $("#btnFacturarPedido").hide();
                }





                if (pedido.seguimientoPedido.estado == ESTADO_RECIBIDO
                    || pedido.seguimientoPedido.estado == ESTADO_RECIBIDO_PARCIALMENTE
                ) {
                    $("#btnVerIngresos").show();
                }
                else {
                    $("#btnVerIngresos").hide();
                }

                if (pedido.seguimientoPedido.estado == ESTADO_ATENDIDO
                    || pedido.seguimientoPedido.estado == ESTADO_ATENDIDO_PARCIALMENTE
                    || pedido.seguimientoPedido.estado == ESTADO_FACTURADO
                    || pedido.seguimientoPedido.estado == ESTADO_FACTURADO_PARCIALMENTE
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
                    && pedido.seguimientoPedido.estado != ESTADO_FACTURADO_PARCIALMENTE
                    && pedido.seguimientoPedido.estado != ESTADO_RECIBIDO
                    && pedido.seguimientoPedido.estado != ESTADO_RECIBIDO_PARCIALMENTE
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

                //  window.location = '/PedidoCompra/Index';
            }
        });
    });



   
    $("#btnCancelarPedido").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/PedidoCompra/CancelarCreacionPedido', null)
    })






    $("#btnFacturarPedido").click(function () {
        $("#facturacion").show(); 
    ;

      /*  $(window).load(function () {
            $("html, body").animate({ scrollTop: $(document).height() }, 1000);
        });*/
        desactivarBotonesVer();
    //    window.scrollTo(0, $("#facturacion").height());
      //  $('html, body').css({ "max-height": $(window).height() , "overflow-y": "auto" });
      // $('#facturacion').css({ "min-height": 312, "overflow-y": "auto" });
    });

    $("#btnCancelarFacturarPedido").click(function () {

        $("#facturacion").hide();
        activarBotonesVer();
    });

    
    $("#btnEditarVenta").click(function () {
        desactivarBotonesVer();

        $.ajax({
            url: "/Venta/iniciarEdicionVenta",
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al iniciar la edición de la venta."); },
            success: function (fileName) {
                window.location = '/Venta/Vender';
            }
        });
        
    });


  


    $("#btnEditarPedido").click(function () {
        desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona
        $.ajax({
            url: "/PedidoCompra/ConsultarSiExistePedido",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/PedidoCompra/iniciarEdicionPedido",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del pedido."); },
                        success: function (fileName) {
                            window.location = '/PedidoCompra/Pedir';
                        }
                    });

                }
                else {
                    if(resultado.numero == 0) {
                        alert('Está creando un nuevo pedido; para continuar por favor diríjase a la página "Crear/Modificar Pedido" y luego haga clic en el botón Cancelar, Finalizar Creación o Guardar y Continuar Editando Luego.');
                    }
                        else {
                        if(resultado.numero == $("#verNumero").html())
                                alert('Ya se encuentra editando el pedido número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Cotización".');
                            else
                                alert('Está editando el pedido número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Pedido" y luego haga clic en el botón Cancelar, Finalizar Edición o Guardar y Continuar Editando Luego.');
                    }
                    activarBotonesVer();
                }
            }
        });
    });

    $("#btnGuardarActualizarPedido").click(function () {

        /*    $("#btnGuardarArchivosAdjuntos").click(function () {
        $.ajax({
            url: "/PedidoCompra/UpdateArchivosAdjuntos",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                mostrarMensajeErrorProceso();
            },
            success: function (resultado) {
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El pedido número " + resultado.numeroPedido + " fue editado correctamente.",
                    buttons: {
                       OK: function() {
                            $("#btnCancelarVerPedido").click();
                        }
                        //OK: function () { window.location = '/PedidoCompra/Index'; }
                    }
                });
            }
        });
    })*/

        if ($("#pedido_numeroReferenciaCliente2").val().length > 20) {
            $("#pedido_numeroReferenciaCliente2").focus();
            $.alert({
                title: TITLE_VALIDACION_PEDIDO,
                content: 'El número de referencia del cliente no debe contener más de 20 caracteres, si el dato a ingresar es más extenso agreguelo en Observaciones Factura.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }   


        var numeroReferenciaCliente = $("#pedido_numeroReferenciaCliente2").val();
        var numeroReferenciaAdicional = $("#pedido_numeroReferenciaAdicional").val();
        var observaciones = $("#pedido_observaciones").val();
        var observacionesFactura = $("#pedido_observacionesFactura").val();
        


        $.ajax({
            url: "/PedidoCompra/UpdatePost",
            data: {
                idPedido: idPedido
            },
            type: 'POST',
            data: {
                numeroReferenciaCliente: numeroReferenciaCliente,
                numeroReferenciaAdicional: numeroReferenciaAdicional,
                observaciones: observaciones,
                observacionesFactura: observacionesFactura
            },
            dataType: 'JSON',
            error: function (detalle) {
                //alert("Ocurrió un problema al obtener el detalle del Pedido N° " + numeroPedido + ".");
                mostrarMensajeErrorProceso();
            },
            success: function (resultado) {
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El pedido número " + resultado.numeroPedido + " fue actualizado correctamente.",
                    buttons: {
                        OK: function () {
                            $("#btnCancelarActualizarPedido").click();
                            window.location = '/PedidoCompra/Index';
                        }
                        //OK: function () { window.location = '/PedidoCompra/Index'; }
                    }
                });


            }
        })
    });
    



    $("#btnActualizarPedido").click(function () {
        $("#verActualizarNumero").html($("#verNumero").html());
        $("#verActualizarCiudad").html($("#verCiudad").html());
        $("#verActualizarCliente").html($("#verCliente").html());

        $("#verActualizarMontoSubTotal").html($("#verMontoSubTotal").html());
        $("#verActualizarMontoIGV").html($("#verMontoIGV").html());
        $("#verActualizarMontoTotal").html($("#verMontoTotal").html());

        $("#pedido_numeroReferenciaCliente2").val($("#verNumeroReferenciaCliente").html());
        $("#pedido_numeroReferenciaAdicional").val($("#verNumeroReferenciaAdicional").html());

        $("#pedido_observacionesFactura").val($("#verObservacionesFactura").html());
        $("#pedido_observaciones").val($("#verObservaciones").html());
        //pe.numero_referencia_adicional,
        //pedido_numeroReferenciaCliente
        //pedido_numeroReferenciaCliente
        
        
        
    });


   





    function limpiarComentario()
    {
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
            url: "/PedidoCompra/updateEstadoPedidoCrediticio",
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
            || $("#modalAprobacionTitle").html() == TITULO_CANCELAR_PROGRAMACION)
        {
            if (comentarioEstado.trim() == "") {
                alert("Debe ingresar un Comentario.");
                return false;
            }
        }
        var codigo = $("#verNumero").html();
        var idPedido = $("#idPedido").val();        

        $.ajax({
            url: "/PedidoCompra/updateEstadoPedido",
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
            url: "/PedidoCompra/updateSeleccionIGV",
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
    $("#considerarCantidades").change( function () {
        var considerarCantidades = $("#considerarCantidades").val();
        $.ajax({
            url: "/PedidoCompra/updateSeleccionConsiderarCantidades",
            type: 'POST',
            data: {
                considerarCantidades: considerarCantidades
            },
            success: function (cantidad)
            {
                if (cantidad > 0)
                {
                    location.reload();
                }
            }
        });


    });



    

    //Mantener en Session cambio de Seleccion de Mostrar Proveedor
    $("input[name=mostrarcodproveedor]").on("click", function () {
        var mostrarcodproveedor = $("input[name=mostrarcodproveedor]:checked").val();
        $.ajax({
            url: "/PedidoCompra/updateMostrarCodigoProveedor",
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
            url: "/PedidoCompra/updateCliente",
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
            url: "/PedidoCompra/updateContacto",
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
        if (flete > 100)
        {
            $("#flete").val("100.00");
            flete = 100;
        }

        var total = Number($("#total").val());
        $('#montoFlete').html("Flete: " + SIMBOLO_SOL + " " + (total * flete / 100).toFixed(cantidadDecimales));
        $('#montoTotalMasFlete').html("Total más Flete: " + SIMBOLO_SOL + " " +  (total + (total * flete / 100)).toFixed(cantidadDecimales));

        


        $.ajax({
            url: "/PedidoCompra/updateFlete",
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
        var mostrarCosto = $('#mostrarCosto').prop('checked') ;
        $.ajax({
            url: "/PedidoCompra/updateMostrarCosto",
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
                                                    url: "/PedidoCompra/DelProducto",
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
        else if (contador > 1)
        {
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
                else
                {
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
        else if (considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES) 
        {
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

            json = json + '{"idProducto":"' + arrId[0] + '", "cantidad":"' + cantidad + '", "porcentajeDescuento":"' + porcentajeDescuento + '", "precio":"' + precio + '", "flete":"' + flete + '",  "costo":"' + costo + '", "observacion":"' + observacion+'"},' 
        });
        json = json.substr(0, json.length - 1) + "]";

    
        /*
        var cotizacionDetalleJson = [
            { "idProducto": "John", "cantidad": "1", "porcentajeDescuento": "0" },
            { "idProducto": "Anna", "cantidad": "1", "porcentajeDescuento": "0" },
            { "idProducto": "Peter", "cantidad": "1", "porcentajeDescuento": "0" }];
        var   json3 = JSON.stringify(cotizacionDetalleJson);*/

        
        $.ajax({
            url: "/PedidoCompra/ChangeDetalle",
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

    

    function calcularSubtotalGrilla(idproducto)
    {
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
        var margen = (1 - (Number(costo) / Number(precio)))*100;
        //Se asigna el margen 
        $("." + idproducto + ".detmargen").text(margen.toFixed(1)+ " %");

        var precioNetoAnterior = Number($("." + idproducto + ".detprecioNetoAnterior").html());        
        var varprecioNetoAnterior = (precio / precioNetoAnterior - 1)*100;
        $("." + idproducto + ".detvarprecioNetoAnterior").text(varprecioNetoAnterior.toFixed(1));

        var costoAnterior = Number($("." + idproducto + ".detcostoAnterior").html());       
        var varcosto = (costo / costoAnterior - 1)*100;
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
        else
        {
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
            url: "/PedidoCompra/CleanBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });


    $("#btnBusquedaPedidos").click(function () {

       
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val();
        var fechaCreacionDesde = $("#pedido_fechaCreacionDesde").val();
        var fechaCreacionHasta = $("#pedido_fechaCreacionHasta").val();
        var fechaEntregaHasta = $("#pedido_fechaEntregaHasta").val();
        var fechaEntregaDesde = $("#pedido_fechaEntregaDesde").val();
        var fechaProgramacionDesde = $("#pedido_fechaProgramacionDesde").val();
        var fechaProgramacionHasta = $("#pedido_fechaProgramacionHasta").val();
        var pedido_numeroPedido = $("#pedido_numeroPedido").val();
        var pedido_numeroGrupoPedido = $("#pedido_numeroGrupoPedido").val();
        var estado = $("#estado").val();
        var estadoCrediticio = $("#estadoCrediticio").val();
        $("#btnBusquedaPedidos").attr("disabled", "disabled");
        $.ajax({
            url: "/PedidoCompra/Search",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                fechaCreacionDesde: fechaCreacionDesde,
                fechaCreacionHasta: fechaCreacionHasta,
                fechaEntregaDesde: fechaEntregaDesde,
                fechaEntregaHasta: fechaEntregaHasta,
                fechaProgramacionDesde: fechaProgramacionDesde,
                fechaProgramacionHasta: fechaProgramacionHasta,
                numero: pedido_numeroPedido,
                numeroGrupo: pedido_numeroGrupoPedido,
                estado: estado,
                estadoCrediticio: estadoCrediticio
            },
            error: function () {
                $("#btnBusquedaPedidos").removeAttr("disabled");
            },

            success: function (pedidoList) {
                $("#btnBusquedaPedidos").removeAttr("disabled");

                $("#tablePedidos > tbody").empty();
                //FooTable.init('#tableCotizaciones');
                $("#tablePedidos").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < pedidoList.length; i++) {

                    var observaciones = pedidoList[i].observaciones == null || pedidoList[i].observaciones == 'undefined' ? '' : pedidoList[i].observaciones;

                    if (pedidoList[i].observaciones != null && pedidoList[i].observaciones.length > 20) {
                        var idComentarioCorto = pedidoList[i].idPedido + "corto";
                        var idComentarioLargo = pedidoList[i].idPedido + "largo";
                        var idVerMas = pedidoList[i].idPedido + "verMas";
                        var idVermenos = pedidoList[i].idPedido + "verMenos";
                        var comentario = pedidoList[i].observaciones.substr(0, 20) + "...";

                        observaciones = '<div id="' + idComentarioCorto + '" style="display:block;">' + comentario + '</div>' +
                            '<div id="' + idComentarioLargo + '" style="display:none;">' + pedidoList[i].observaciones + '</div>' +
                            '<p><a id="' + idVerMas + '" class="' + pedidoList[i].idPedido + ' verMas" href="javascript:mostrar();" style="display:block">Ver Más</a></p>' +
                            '<p><a id="' + idVermenos + '" class="' + pedidoList[i].idPedido + ' verMenos" href="javascript:mostrar();" style="display:none">Ver Menos</a></p>';
                    }

                    var fechaProgramacion = "No Programado";
                    if (pedidoList[i].fechaProgramacion != null && pedidoList[i].fechaProgramacion != "")
                    {
                        fechaProgramacion = invertirFormatoFecha(pedidoList[i].fechaProgramacion.substr(0, 10));
                    }

                  /*  var codigoCliente = pedidoList[i].cliente.codigo;
                    if (codigoCliente == null || codigoCliente == 'null') {
                        codigoCliente = '';
                    }
                    var numeroReferenciaCliente = pedidoList[i].numeroReferenciaCliente;
                    if (numeroReferenciaCliente == null || numeroReferenciaCliente == 'null') {
                        numeroReferenciaCliente = '';
                    }*/
                    var grupoCliente = pedidoList[i].grupoCliente_nombre == null ? "" : pedidoList[i].grupoCliente_nombre;
                    var pedido = '<tr data-expanded="true">' +
                        '<td>  ' + pedidoList[i].idPedido+'</td>' +
                        '<td>  ' + pedidoList[i].numeroPedidoString + '  </td>' +
                        '<td>  ' + pedidoList[i].ciudad.nombre + '  </td>' +
                        '<td>  ' + pedidoList[i].cliente.codigo + ' </td>' +
                        '<td>  ' + pedidoList[i].cliente.razonSocial + '</td>' +
                        '<td>  ' + grupoCliente + '</td>' +
                        '<td>  ' + pedidoList[i].numeroReferenciaCliente+'  </td>' +
                        '<td>  ' + pedidoList[i].usuario.nombre + '  </td>' +
                        '<td>  ' + pedidoList[i].fechaHoraRegistro + '</td>' +
                        //'<td>  ' + pedidoList[i].fechaHoraSolicitud + '</td>' +                        
                        '<td>  ' + pedidoList[i].rangoFechasEntrega + '</td>' +
                        //'<td>  ' + fechaProgramacion+ '</td>' +
                        '<td>  ' + pedidoList[i].montoTotal + '  </td>' +
                        '<td>  ' + pedidoList[i].ubigeoEntrega.Distrito + '  </td>' +
                        '<td>  ' + pedidoList[i].seguimientoPedido.estadoString+'</td>' +
                      //  '<td>  ' + pedidoList[i].seguimientoPedido.usuario.nombre+'  </td>' +
                      //  '<td>  ' + observacion+'  </td>' +
                        '<td>  ' + pedidoList[i].seguimientoCrediticioPedido.estadoString + '</td>' +
                        '<td> ' + observaciones + ' </td>' + 
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
            url: "/PedidoCompra/ChangeFechaSolicitudDesde",
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
            url: "/PedidoCompra/ChangeFechaSolicitudHasta",
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
            || fechaEntregaHasta.trim() == "" ) {
            $("#pedido_fechaEntregaHasta").val(fechaEntregaDesde);
            $("#pedido_fechaEntregaHasta").change();
        }

        $.ajax({
            url: "/PedidoCompra/ChangeFechaEntregaDesde",
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
            url: "/PedidoCompra/ChangeFechaEntregaHasta",
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
            url: "/PedidoCompra/changeNumero",
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
            url: "/PedidoCompra/changeNumeroGrupo",
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
            url: "/PedidoCompra/changeEstado",
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
            url: "/PedidoCompra/changeEstadoCrediticio",
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
            url: "/PedidoCompra/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#ActualProvincia",function () {
        var ubigeoEntregaId = $("#ActualDepartamento").val()+"0000";
        if ($("#ActualProvincia").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualProvincia").val()+"00";
        }
        $.ajax({
            url: "/PedidoCompra/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#ActualDistrito",function () {
        var ubigeoEntregaId = $("#ActualDepartamento").val() + $("#ActualProvincia").val() + "00";
        if ($("#ActualDistrito").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualDistrito").val();
        }
        $.ajax({
            url: "/PedidoCompra/ChangeUbigeoEntrega",
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
            url: "/PedidoCompra/ChangeIdCiudad",
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
            url: "/PedidoCompra/Programar",
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



 


    /****************** FIN PROGRAMACION PEDIDO****************************/
});