
jQuery(function ($) {



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
   
    

    //Etiquetas de estadps para búsqueda de Pedidos
    var ESTADO_PENDIENTE_APROBACION_STR = "Pendiente de Aprobación de Ingreso";
    var ESTADO_INGRESADO_STR = "Pedido Ingresado";
    var ESTADO_DENEGADO_STR = "Pedido Denegado";
    var ESTADO_PROGRAMADO_STR = "Pedido Programado"
    var ESTADO_ATENDIDO_STR = "Pedido Atendido"
    var ESTADO_ATENDIDO_PARCIALMENTE_STR = "Pedido Atendido Parcialmente"
    var ESTADO_EN_EDICION_STR = "Pedido En Edicion";
    var ESTADO_ELIMINADO_STR = "Pedido Eliminado";

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
     * 2 BusquedaPedidos
       3 CrearPedido
     */

    var pagina = 2;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
   //     obtenerConstantes();
  //      setTimeout(autoGuardarPedido, MILISEGUNDOS_AUTOGUARDADO);
        cargarChosenCliente(pagina);
  /*      toggleControlesUbigeo();
        verificarSiExisteNuevaDireccionEntrega();
        verificarSiExisteDetalle();
        verificarSiExisteCliente();
        $("#btnBusquedaPedidos").click();
        var tipoPedido = $("#pedido_tipoPedido").val();
        validarTipoPedido(tipoPedido);
        */
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


    $("#btnRecuperarDatosSunat").click(function () {

        var ruc = $("#cliente_ruc").val();
        
        if (ruc.length != 11) {
            $.alert({
                title: "RUC Inválido",
                type: 'orange',
                content: 'Debe ingresar un número de RUC válido.',
                buttons: {
                    OK: function () { }
                }
            });
            $('#cliente_ruc').focus();
            return false;
        }

        $.ajax({
            url: "/Cliente/GetDatosSunat",
            type: 'POST',
            dataType: 'JSON',
            data: { ruc: ruc },
            success: function (cliente) {
                $("#cliente_razonSocialSunat").val(cliente.razonSocialSunat);
                $("#cliente_direccionDomicilioLegalSunat").val(cliente.direccionDomicilioLegalSunat);
                $("#cliente_ubigeo_Departamento").val(cliente.ubigeo.Departamento);
                $("#cliente_ubigeo_Provincia").val(cliente.ubigeo.Provincia);
                $("#cliente_ubigeo_Distrito").val(cliente.ubigeo.Distrito);
                $("#cliente_estadoContribuyente").val(cliente.estadoContribuyente);
                $("#cliente_condicionContribuyente").val(cliente.condicionContribuyente);
            },
            error: function () {
                $.alert({
                    title: 'Error',
                    content: MENSAJE_ERROR,
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });       
            }
        });
    });



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
            url: "/Cliente/SearchClientes"
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

         $('body').loadingModal({
            text: 'Recuperando Datos del Cliente...'
        });

        $.ajax({
            url: "/Cliente/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente) {

                $('body').loadingModal('hide')
                $("#idCiudad").attr("disabled", "disabled");

                //Se limpia controles de Ubigeo
                $("#cliente_codigo").val(cliente.codigo);
                $("#cliente_ruc").val(cliente.ruc);
                $("#cliente_razonSocial").val(cliente.razonSocial);
                $("#cliente_nombreComercial").val(cliente.nombreComercial);
                $("#cliente_domicilioLegal").val(cliente.domicilioLegal);
                $("#cliente_correoEnvioFactura").val(cliente.correoEnvioFactura);
                $("#cliente_razonSocialSunat").val(cliente.razonSocialSunat);
                $("#cliente_nombreComercialSunat").val(cliente.nombreComercialSunat);
                $("#cliente_direccionDomicilioLegalSunat").val(cliente.direccionDomicilioLegalSunat);
                $("#cliente_estadoContribuyente").val(cliente.estadoContribuyente);
                $("#cliente_condicionContribuyente").val(cliente.condicionContribuyente);

                $("#cliente_ubigeo_Departamento").val(cliente.ubigeo.Departamento);
                $("#cliente_ubigeo_Provincia").val(cliente.ubigeo.Provincia);
                $("#cliente_ubigeo_Distrito").val(cliente.ubigeo.Distrito);

                $("#cliente_plazoCredito").val(cliente.plazoCredito);
                $("#tipoPagoCliente").val(cliente.tipoPagoFactura);
                $("#formaPagoCliente").val(cliente.formaPagoFactura);

            /*    $("#direccionDomicilioLegalSunat").val(resultado.direccionDomicilioLegalSunat);
                $("#cliente_ubigeo_Departamento").val(resultado.ubigeo.Departamento);
                $("#cliente_ubigeo_Provincia").val(resultado.ubigeo.Provincia);
                $("#cliente_ubigeo_Distrito").val(resultado.ubigeo.Distrito);
                */
               // toggleControlesUbigeo();
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
  

 
    $(window).on("paste", function (e) {
        
            $.each(e.originalEvent.clipboardData.items, function () {
                this.getAsString(function (str) {
                   // alert(str);
                    var lineas = str.split("\t");           

                    if (lineas.length <= 1)
                        return false;
                    if (lineas[1] == $("#cliente_ruc").val()) {
                        $("#ncRUC").val(lineas[1]);

                        $("#cliente_razonSocialSunat").val(lineas[0]);
                        //$("#ncRUC").val(lineas[1]);
                        $("#cliente_nombreComercialSunat").val(lineas[2]);
                        $("#cliente_direccionDomicilioLegalSunat").val(lineas[3]);
                        $("#cliente_estadoContribuyente").val(lineas[5]);
                        $("#cliente_condicionContribuyente").val(lineas[6]);


                        changeInputString("razonSocialSunat", $("#cliente_razonSocialSunat").val())
                        changeInputString("nombreComercialSunat", $("#cliente_nombreComercialSunat").val())

                        var direccionDomicilioLegalSunat = $("#cliente_direccionDomicilioLegalSunat").val();

                        $('body').loadingModal({
                            text: 'Recuperando Ubicación Geográfica...'
                        });
                        $.ajax({
                            url: "/Cliente/ChangeDireccionDomicilioLegalSunat",
                            type: 'POST',
                            dataType: 'JSON',
                            data: {
                                direccionDomicilioLegalSunat: direccionDomicilioLegalSunat
                            },
                            erro: function () {
                                $('body').loadingModal('hide')
                            },

                            success: function (resultado) {
                                $('body').loadingModal('hide')
                                $("#cliente_direccionDomicilioLegalSunat").val(resultado.direccionDomicilioLegalSunat);
                                $("#cliente_ubigeo_Departamento").val(resultado.ubigeo.Departamento);
                                $("#cliente_ubigeo_Provincia").val(resultado.ubigeo.Provincia);
                                $("#cliente_ubigeo_Distrito").val(resultado.ubigeo.Distrito);
                            }
                        });

                        changeInputString("estadoContribuyente", $("#cliente_estadoContribuyente").val())
                        changeInputString("condicionContribuyente", $("#cliente_condicionContribuyente").val())
                    }
                    else {
                        alert("El RUC que acaba de pegar no coincide con el RUC del cliente.");
                    }
                });
            });

            
    });


    function validacionDatosCliente()
    {


        var ruc = $("#cliente_ruc").val();

        if (ruc.length == 11) {

            if ($("#cliente_direccionDomicilioLegalSunat").val().length > 120) {
                $.alert({
                    title: "Dirección Inválida",
                    type: 'orange',
                    content: 'La dirección del domicilio legal obtenido de Sunat debe contener solo 120 caracteres.',
                    buttons: {
                        OK: function () { $('#cliente_direccionDomicilioLegalSunat').focus(); }
                    }
                });

                return false;
            }

            if ($("#cliente_razonSocialSunat").val().length == 0) {
                $.alert({
                    title: "No existe Razón Social",
                    type: 'orange',
                    content: 'No existe Razón Social, debe hacer clic en el botón "Obtener Datos Sunat"',
                    buttons: {
                        OK: function () { $('#cliente_razonSocialSunat').focus(); }
                    }
                });

                return false;
            }
        }
        else {

            if ($("#cliente_nombreComercial").val().length == 0) {
                $.alert({
                    title: "No existe Nombre Cliente",
                    type: 'orange',
                    content: 'No existe Nombre Cliente, debe ingresar el Nombre del Cliente"',
                    buttons: {
                        OK: function () { $('#cliente_razonSocialSunat').focus(); }
                    }
                });

                return false;
            }
        }


        

        if ($("#cliente_correoEnvioFactura").val().trim().length < 8) {
            $.alert({
                title: "No se ingresó Correo Electrónico",
                type: 'orange',
                content: 'Se debe agregar el correo electrónico para el envío de factura',
                buttons: {
                    OK: function () { $('#cliente_correoEnvioFactura').focus();}
                }
            });
            
            return false;
        }

        return true;

    }



    

    function crearCliente() {

        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            $.alert({
                title: "No selecionó Ciudad",
                type: 'orange',
                content: 'Debe seleccionar la sede MP previamente.',
                buttons: {
                    OK: function () { }
                }
            });
            $("#idCiudad").focus()
            return false;
        }

        if (!validacionDatosCliente())
            return false;       

        $('body').loadingModal({
            text: 'Creando Cliente...'
        });
        $.ajax({
            url: "/Cliente/Create",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear el cliente.',
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
                    content: 'El cliente se creó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Cliente/Editar';
                        }
                    }
                });
            }
        });

        // $('#btnCancelCliente').click();

    }

    function editarCliente() {

        if (!validacionDatosCliente())
            return false;       

    
        $('body').loadingModal({
            text: 'Editando Cliente...'
        });
        $.ajax({
            url: "/Cliente/Update",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar el cliente.',
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
                    content: 'El cliente se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Cliente/Editar';
                        }
                    }
                });
            }
        });


        // $('#btnCancelCliente').click();

    }


    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Cliente/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }
    
    $("#cliente_codigo").change(function () {
        changeInputString("codigo", $("#cliente_codigo").val())
    });

    $("#cliente_ruc").change(function () {
        changeInputString("ruc", $("#cliente_ruc").val())
    });

    $("#cliente_razonSocial").change(function () {
        changeInputString("razonSocial", $("#cliente_razonSocial").val())
    });

    $("#cliente_domicilioLegal").change(function () {
        changeInputString("domicilioLegal", $("#cliente_domicilioLegal").val())
    });

    $("#cliente_nombreComercial").change(function () {
        changeInputString("nombreComercial", $("#cliente_nombreComercial").val())
    });

    $("#cliente_correoEnvioFactura").change(function () {
        changeInputString("correoEnvioFactura", $("#cliente_correoEnvioFactura").val())
    });
    
    $("#cliente_razonSocialSunat").change(function () {
        changeInputString("razonSocialSunat", $("#cliente_razonSocialSunat").val())
    });

    $("#cliente_nombreComercialSunat").change(function () {
        changeInputString("nombreComercialSunat", $("#cliente_nombreComercialSunat").val())
    });

    $("#cliente_direccionDomicilioLegalSunat").change(function () {
        changeInputString("direccionDomicilioLegalSunat", $("#cliente_direccionDomicilioLegalSunat").val())
    });

    $("#cliente_estadoContribuyente").change(function () {
        changeInputString("estadoContribuyente", $("#cliente_direccliente_estadoContribuyentecionDomicilioLegalSunat").val())
    });

    $("#cliente_condicionContribuyente").change(function () {
        changeInputString("condicionContribuyente", $("#cliente_direcclientecliente_condicionContribuyente_estadoContribuyentecionDomicilioLegalSunat").val())
    });

    $("#formaPagoCliente").change(function () {
        var formaPagoFactura = $("#formaPagoCliente").val();
        $.ajax({
            url: "/Cliente/ChangeFormaPagoFactura",
            type: 'POST',
            data: {
                formaPagoFactura: formaPagoFactura
            },
            success: function () { }
        });
    });

    $("#tipoPagoCliente").change(function () {
        var tipoPagoFactura = $("#tipoPagoCliente").val();
        $.ajax({
            url: "/Cliente/ChangeTipoPagoFactura",
            type: 'POST',
            data: {
                tipoPagoFactura: tipoPagoFactura
            },
            success: function () { }
        });
    });

    





    


    $("#btnFinalizarEdicionCliente").click(function () {
        if ($("#cliente_codigo").val().length == 0)
            crearCliente();
        else
            editarCliente();
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

              
                $("#tableDetallePedido > tbody").empty();

                FooTable.init('#tableDetallePedido');

               $("#formVerGuiasRemision").html("");

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
                        '<td>' + lista[i].cantidad + '</td>' +
                        '<td>' + lista[i].cantidadPendienteAtencion + '</td>' +
                        '<td>' + lista[i].subTotal.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + observacion + '</td>' +
                        '</tr>';

                   

                }

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

              //  
               // sleep
                $("#tableDetallePedido").append(d);

                if (pedido.seguimientoPedido.estado != ESTADO_PROGRAMADO
                    && pedido.seguimientoPedido.estado != ESTADO_ATENDIDO
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
                && usuario.apruebaPedidos)
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
                    )&&
                    pedido.seguimientoCrediticioPedido.estado == ESTADO_LIBERADO) {

                    $("#btnAtenderPedido").show();
                }
                else {
                    $("#btnAtenderPedido").hide();
                }

                //CANCELAR PROGRAMACION
                if (pedido.seguimientoPedido.estado == ESTADO_PROGRAMADO &&
                    !pedido.seguimientoPedido.estado == ESTADO_ATENDIDO &&
                    !pedido.seguimientoPedido.estado == ESTADO_ATENDIDO_PARCIALMENTE)
                {
                    $("#btnCancelarProgramacionPedido").show();
                }
                else {
                    $("#btnCancelarProgramacionPedido").hide();
                }


                //PROGRAMAR PEDIDO
                if (pedido.seguimientoPedido.estado == ESTADO_ATENDIDO
                    || pedido.seguimientoPedido.estado == ESTADO_ATENDIDO_PARCIALMENTE
                    ) {

                    $("#btnFacturarPedido").show();
                }
                else {
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



    function generarPDF()
    {
        //$("#generarPDF").click(function () {

        var codigo = $("#numero").val();
        if (codigo == "" || codigo == 0)
        { 
            alert("Debe guardar la cotización previamente.");
            return false;
        }

          $.ajax({
            url: "/Pedido/GenerarPDFdesdeIdCotizacion",
            data: {
                codigo: codigo
            },
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al descargar la cotización N° " + codigo+" en formato PDF.");},
            success: function (fileName) {
                //Se descarga el PDF y luego se limpia el formulario
                window.open('/Pedido/DownLoadFile?fileName=' + fileName);
                window.location = '/Pedido/Index';
            }
        });
}

    $("#btnCancelarCliente").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/Cliente/CancelarCreacionCliente', null)
    })



    function desactivarBotonesVer()
    {
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
                    $.ajax({
                        url: "/GuiaRemision/iniciarAtencionDesdePedido",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la atención del pedido."); },
                        success: function (fileName) {
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
    $("#considerarCantidades").change( function () {
        var considerarCantidades = $("#considerarCantidades").val();
        $.ajax({
            url: "/Pedido/updateSeleccionConsiderarCantidades",
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
        if (flete > 100)
        {
            $("#flete").val("100.00");
            flete = 100;
        }

        var total = Number($("#total").val());
        $('#montoFlete').html("Flete: " + SIMBOLO_SOL + " " + (total * flete / 100).toFixed(cantidadDecimales));
        $('#montoTotalMasFlete').html("Total más Flete: " + SIMBOLO_SOL + " " +  (total + (total * flete / 100)).toFixed(cantidadDecimales));

        


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
        var mostrarCosto = $('#mostrarCosto').prop('checked') ;
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
            var cantidad = value.innerText;
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detincantidad form-control' value='" + cantidad + "' type='number'/>";
        });

        var considerarCantidades = $("#considerarCantidades").val();
        if (considerarCantidades == CANT_SOLO_OBSERVACIONES) {
            /*Se agrega control input en columna observacion*/
            var $j_object = $("td.detobservacion");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var observacion = value.innerText;
                value.innerHTML = "<textarea class='" + arrId[0] + " detobservacionarea form-control'/>" + observacion + "</textarea>";
            });
        }
        else if (considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES) 
        {
            var $j_object = $("span.detproductoObservacion");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var observacion = value.innerText;
                value.innerHTML = "<textarea class='" + arrId[0] + " detobservacionarea form-control'/>" + observacion + "</textarea>";
            });

        }

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
            url: "/Pedido/ChangeDetalle",
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
                    if (pedidoList[i].fechaProgramacion != null && pedidoList[i].fechaProgramacion != "")
                    {
                        fechaProgramacion = invertirFormatoFecha(pedidoList[i].fechaProgramacion.substr(0, 10));
                    }

                    var pedido = '<tr data-expanded="true">' +
                        '<td>  ' + pedidoList[i].idPedido+'</td>' +
                        '<td>  ' + pedidoList[i].numeroPedidoString+'  </td>' +
                        '<td>  ' + pedidoList[i].numeroGrupoPedidoString+'  </td>' +
                        '<td>  ' + pedidoList[i].cliente.razonSocial+'</td>' +
                        '<td>  ' + pedidoList[i].cliente.ruc+' </td>' +
                        '<td>  ' + pedidoList[i].ciudad.nombre+'  </td>' +
                        '<td>  ' + pedidoList[i].usuario.nombre+'  </td>' +
                        '<td>  ' + pedidoList[i].fechaHoraSolicitud+'</td>' +
                        '<td>  ' + pedidoList[i].rangoFechasEntrega + '</td>' +
                        '<td>  ' + fechaProgramacion+ '</td>' +
                        '<td>  ' + pedidoList[i].montoTotal+'  </td>' +
                        '<td>  ' + pedidoList[i].seguimientoPedido.estadoString+'</td>' +
                      //  '<td>  ' + pedidoList[i].seguimientoPedido.usuario.nombre+'  </td>' +
                      //  '<td>  ' + observacion+'  </td>' +
                        '<td>  ' + pedidoList[i].seguimientoCrediticioPedido.estadoString+'</td>' +
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
            || fechaEntregaHasta.trim() == "" ) {
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

    $(document).on('change', "#ActualProvincia",function () {
        var ubigeoEntregaId = $("#ActualDepartamento").val()+"0000";
        if ($("#ActualProvincia").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualProvincia").val()+"00";
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

    $(document).on('change', "#ActualDistrito",function () {
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
            url: "/Cliente/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad
            },
            error: function (detalle) {
               // alert('Debe eliminar los productos agregados antes de cambiar de Sede.');
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


    

    /****************** FIN PROGRAMACION PEDIDO****************************/
});