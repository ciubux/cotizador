
jQuery(function ($) {

    var pagina = 2;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        $("#btnBusqueda").click();
        //cargarChosenCliente();
        mostrarCamposSegunTipoDocIdentidad();
        verificarSiExisteCliente();
    });

    function verificarSiExisteCliente() {
        if ($("#idCliente").val().trim() != GUID_EMPTY) {
            $("#idCiudad").attr("disabled", "disabled");
            $("#tipoDocumentoIdentidad").attr("disabled", "disabled");
            $("#cliente_ruc").attr("disabled", "disabled");
            $("#btnFinalizarEdicionCliente").html('Finalizar Edición');            
        }
        else {
            $("#btnFinalizarEdicionCliente").html('Finalizar Creación');
        }

    }

    function mostrarCamposParaClienteConRUC() {

    }


    $("#tipoDocumentoIdentidad").change(function () {


        var tipoDocumentoIdentidad = $("#tipoDocumentoIdentidad").val();

        if ($("#cliente_codigo").val().length > 0) {
            $.confirm({
                title: 'Confirmación',
                content: '¿Está seguro de cambiar de tipo de Documento?',
                type: 'orange',
                buttons: {           
                    confirm: {
                        text: 'Sí',
                        btnClass: 'btn-red',
                        action: function () {
                            cambiarTipoDocumentoIdentidad(tipoDocumentoIdentidad);
                            limpiarFormulario();
                            mostrarCamposSegunTipoDocIdentidad();
                        }
                    },
                    cancel: {
                        text: 'No',
                        //btnClass: 'btn-blue',
//                        keys: ['enter', 'shift'],
                        action: function () {
                            location.reload();
                        }
                    }
                },
               
            });

           
        }
        else {
            cambiarTipoDocumentoIdentidad(tipoDocumentoIdentidad);
            limpiarFormulario();
            mostrarCamposSegunTipoDocIdentidad();
        }
        
    });

    function cambiarTipoDocumentoIdentidad(tipoDocumentoIdentidad) {
        $.ajax({
            url: "/Cliente/changeTipoDocumentoIdentidad",
            type: 'POST',
            data: { tipoDocumentoIdentidad: tipoDocumentoIdentidad },
            success: function () {

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

    }

    function mostrarCamposSegunTipoDocIdentidad() {
        var tipoDocumentoIdentidad = $("#tipoDocumentoIdentidad").val();
        if (tipoDocumentoIdentidad == CONS_TIPO_DOC_CLIENTE_DNI.charCodeAt(0)
            || tipoDocumentoIdentidad == CONS_TIPO_DOC_CLIENTE_CARNET_EXTRANJERIA.charCodeAt(0)) {
            $("#labelClienteNombre").html("Nombres y Apellidos");
            $("#fieldSetDatosSunat").hide();
            $("#btnRecuperarDatosSunat").hide();
        }
        else if (tipoDocumentoIdentidad == CONS_TIPO_DOC_CLIENTE_RUC.charCodeAt(0)) {
            $("#labelClienteNombre").html("Nombre Comercial");
            $("#fieldSetDatosSunat").show();
            $("#btnRecuperarDatosSunat").show();

        }

    }


    function limpiarFormulario() {

        $("#cliente_ruc").val("");
        $("#cliente_razonSocial").val("");
        $("#cliente_nombreComercial").val("");
        $("#cliente_domicilioLegal").val("");
        $("#cliente_contacto1").val("");
        $("#cliente_correoEnvioFactura").val("");
        $("#cliente_razonSocialSunat").val("");
        $("#cliente_nombreComercialSunat").val("");
        $("#cliente_direccionDomicilioLegalSunat").val("");
        $("#cliente_estadoContribuyente").val("");
        $("#cliente_condicionContribuyente").val("");

        $("#cliente_ubigeo_Departamento").val("");
        $("#cliente_ubigeo_Provincia").val("");
        $("#cliente_ubigeo_Distrito").val("");

        $("#cliente_plazoCredito").val("");
        $("#tipoPagoCliente").val("0");
        $("#formaPagoCliente").val("0");
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
                $("#cliente_nombreComercial").val(cliente.nombreComercial);                
                $("#cliente_nombreComercialSunat").val(cliente.nombreComercialSunat);
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


    
    /**
     * ################################ INICIO CONTROLES DE CLIENTE
     */

    function cargarChosenCliente() {

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

        //verificarSiExisteCliente();
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
                limpiarFormulario();
                
                $('body').loadingModal('hide')

                $("#cliente_codigo").val(cliente.codigo);
                $("#cliente_ruc").val(cliente.ruc);
                $("#cliente_razonSocial").val(cliente.razonSocial);
                $("#cliente_nombreComercial").val(cliente.nombreComercial);
                $("#cliente_domicilioLegal").val(cliente.domicilioLegal);
                $("#cliente_contacto1").val(cliente.contacto1);                
                $("#cliente_correoEnvioFactura").val(cliente.correoEnvioFactura);
                $("#cliente_razonSocialSunat").val(cliente.razonSocialSunat);
                $("#cliente_nombreComercialSunat").val(cliente.nombreComercialSunat);
                $("#cliente_direccionDomicilioLegalSunat").val(cliente.direccionDomicilioLegalSunat);
                $("#cliente_estadoContribuyente").val(cliente.estadoContribuyente);
                $("#cliente_condicionContribuyente").val(cliente.condicionContribuyente);

                $("#cliente_ubigeo_Departamento").val(cliente.ubigeo.Departamento);
                $("#cliente_ubigeo_Provincia").val(cliente.ubigeo.Provincia);
                $("#cliente_ubigeo_Distrito").val(cliente.ubigeo.Distrito);

                $("#tipoDocumentoIdentidad").val(cliente.tipoDocumentoIdentidad);
                $("#formaPagoCliente").val(cliente.formaPagoFactura);

                /*Plazos de Crédito*/
                $("#plazoCreditoSolicitado").val(cliente.plazoCreditoSolicitado);
                $("#tipoPagoCliente").val(cliente.tipoPagoFactura);
                $("#cliente_sobrePlazo").val(cliente.sobrePlazo);

                /*Montos de Crédito*/
                $("#cliente_creditoSolicitado").val(cliente.creditoSolicitado);
                $("#cliente_creditoAprobado").val(cliente.creditoAprobado);
                $("#cliente_sobreGiro").val(cliente.sobreGiro);

                /*Vendedores*/

                if (cliente.usuario.defineResponsableComercial || !cliente.vendedoresAsignados)
                {
                    $("#idResponsableComercial").removeAttr("disabled");
                }
                else
                {
                    $("#idResponsableComercial").attr("disabled", "disabled");
                }

                if (cliente.vendedoresAsignados) {
                    $("#spanVendedoresAsignados").show();
                    $("#spanVendedoresNoAsignados").hide();
                }
                else {
                    $("#spanVendedoresAsignados").hide();
                    $("#spanVendedoresNoAsignados").show();
                }

                $("#idResponsableComercial").val(cliente.responsableComercial.idVendedor);
                $("#idSupervisorComercial").val(cliente.supervisorComercial.idVendedor);
                $("#idAsistenteServicioCliente").val(cliente.asistenteServicioCliente.idVendedor);

                $("#cliente_observacionesCredito").val(cliente.observacionesCredito);
                $("#cliente_observaciones").val(cliente.observaciones);




                verificarSiExisteCliente();
                mostrarCamposSegunTipoDocIdentidad();
            }
        });
      
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


        var tipoDocumentoIdentidad = $("#tipoDocumentoIdentidad").val();
        var ruc = $("#cliente_ruc").val();

        if (tipoDocumentoIdentidad == CONS_TIPO_DOC_CLIENTE_DNI.charCodeAt(0))
        {
            if (ruc.length != 8) {
                $.alert({
                    title: "DNI Inválido", type: 'orange',
                    content: 'El número de DNI debe tener 8 dígitos.',
                    buttons: { OK: function () { $('#cliente_ruc').focus(); }
                    }
                });
            }

            if ($("#cliente_nombreComercial").val().length == 0) {
                $.alert({
                    title: "Nombre Cliente Inválido",
                    type: 'orange',
                    content: 'No existe Nombre Cliente, debe ingresar el Nombre del Cliente"',
                    buttons: {
                        OK: function () { $('#cliente_razonSocialSunat').focus(); }
                    }
                });
                return false;
            }
        }
        else if (tipoDocumentoIdentidad == CONS_TIPO_DOC_CLIENTE_RUC.charCodeAt(0)) {
            if (ruc.length != 11) {
                $.alert({
                    title: "DNI Inválido", type: 'orange',
                    content: 'El número de DNI debe tener 8 dígitos.',
                    buttons: {
                        OK: function () { $('#cliente_ruc').focus(); }
                    }
                });
            }

            if ($("#cliente_correoEnvioFactura").val().trim().length < 8) {
                $.alert({
                    title: "Correo Electrónico Inválido",
                    type: 'orange',
                    content: 'Se debe agregar el correo electrónico para el envío de factura',
                    buttons: {
                        OK: function () { $('#cliente_correoEnvioFactura').focus(); }
                    }
                });

                return false;
            }

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
        else if (tipoDocumentoIdentidad == CONS_TIPO_DOC_CLIENTE_CARNET_EXTRANJERIA.charCodeAt(0)) {
            if (ruc.length > 12 || ruc.length < 0) {
                $.alert({
                    title: "Carnet Extranjería Inválido", type: 'orange',
                    content: 'El número de Carnet Extranjería debe tener 12 caracteres como máximo.',
                    buttons: {
                        OK: function () { $('#cliente_ruc').focus(); }
                    }
                });
            }

            if ($("#cliente_nombreComercial").val().length == 0) {
                $.alert({
                    title: "Nombre Cliente Inválido",
                    type: 'orange',
                    content: 'No existe Nombre Cliente, debe ingresar el Nombre del Cliente"',
                    buttons: {
                        OK: function () { $('#cliente_razonSocialSunat').focus(); }
                    }
                });
                return false;
            }
        }

        if ($("#plazoCreditoSolicitado").is(':enabled')) {

            if ($("#plazoCreditoSolicitado").val() == "0") {
                $.alert({
                    title: "Plazo Crédito Solicitado Inválido",
                    type: 'orange',
                    content: 'Debe indicar el plazo de crédito Solicitado',
                    buttons: {
                        OK: function () { $('#cliente_razonSocialSunat').focus(); }
                    }
                });
                return false;
            }
        }

      /*  if ($("#idResponsableComercial").val().trim() == 0) {
            $.alert({
                title: "Responsable Comercial Inválido",
                type: 'orange',
                content: 'Debe seleccionar al Responsable Comercial',
                buttons: {
                    OK: function () { $('#idResponsableComercial').focus(); }
                }
            });
            return false;
        }        */

        return true;

    }


    $("#btnFinalizarEdicionCliente").click(function () {
        /*Si no tiene codigo el cliente se está creando*/
        if ($("#cliente_codigo").val().length == 0) {
            crearCliente();
        }
        else {
            editarCliente();
        }
    });

    

    function crearCliente() {
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
                            window.location = '/Cliente/Index';
                        }
                    }
                });
            }
        });

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
                            window.location = '/Cliente/Index';
                        }
                    }
                });
            }
        });
    }

    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/Cliente/ChangeInputInt",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#cliente_sobrePlazo").change(function () {
        changeInputInt("sobrePlazo", $("#cliente_sobrePlazo").val())
    });

    function changeInputDecimal(propiedad, valor) {
        $.ajax({
            url: "/Cliente/ChangeInputDecimal",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#cliente_creditoSolicitado").change(function () {
        changeInputDecimal("creditoSolicitado", $("#cliente_creditoSolicitado").val())
    });

    $("#cliente_creditoAprobado").change(function () {
        changeInputDecimal("creditoAprobado", $("#cliente_creditoAprobado").val())
    });

    $("#cliente_sobreGiro").change(function () {
        changeInputDecimal("sobreGiro", $("#cliente_sobreGiro").val())
    });




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



   

    $("#cliente_contacto1").change(function () {
        changeInputString("contacto1", $("#cliente_contacto1").val())
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

    $("#cliente_observacionesCredito").change(function () {
        changeInputString("observacionesCredito", $("#cliente_observacionesCredito").val())
    });

    $("#cliente_observaciones").change(function () {
        changeInputString("observaciones", $("#cliente_observaciones").val())
    });

    $("#cliente_textoBusqueda").change(function () {
        changeInputString("textoBusqueda", $("#cliente_textoBusqueda").val())
    });

    $("#cliente_codigo").change(function () {
        changeInputString("codigo", $("#cliente_codigo").val())
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


    $("#plazoCreditoSolicitado").change(function () {
        var plazoCreditoSolicitado = $("#plazoCreditoSolicitado").val();
        $.ajax({
            url: "/Cliente/ChangePlazoCreditoSolicitado",
            type: 'POST',
            data: {
                plazoCreditoSolicitado: plazoCreditoSolicitado
            },
            success: function () { }
        });
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




    $("#btnCancelarCliente").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/Cliente/CancelarCreacionCliente', null)
    })


    

    




    $('#modalAprobacion').on('shown.bs.modal', function (e) {
        limpiarComentario();
    });



    
    var ft = null;


    
    
    

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


    

    /*####################################################
    EVENTOS BUSQUEDA COTIZACIONES
    #####################################################*/

  
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
                location.reload();
            },
            success: function (ciudad) {
            }
        });
    });  


    $("#idGrupoCliente").change(function () {
        var idGrupoCliente = $("#idGrupoCliente").val();

        $.ajax({
            url: "/Cliente/ChangeIdGrupoCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idGrupoCliente: idGrupoCliente
            },
            error: function (detalle) {
                location.reload();
            },
            success: function (GrupoCliente) {
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


    /*VENDEDORES*/

    $("#idResponsableComercial").change(function () {
        var idResponsableComercial = $("#idResponsableComercial").val();
        $.ajax({
            url: "/Cliente/ChangeIdResponsableComercial", type: 'POST', 
            data: {
                idResponsableComercial: idResponsableComercial
            },
            error: function () { location.reload();      },
            success: function () {     }
        });
    });

    $("#idSupervisorComercial").change(function () {
        var idSupervisorComercial = $("#idSupervisorComercial").val();
        $.ajax({
            url: "/Cliente/ChangeIdSupervisorComercial", type: 'POST',
            data: {
                idSupervisorComercial: idSupervisorComercial
            },
            error: function () { location.reload(); },
            success: function () { }
        });
    });


    $("#idAsistenteServicioCliente").change(function () {
        var idAsistenteServicioCliente = $("#idAsistenteServicioCliente").val();
        $.ajax({
            url: "/Cliente/ChangeIdAsistenteServicioCliente", type: 'POST',
            data: {
                idAsistenteServicioCliente: idAsistenteServicioCliente
            },
            error: function () { location.reload(); },
            success: function () { }
        });
    });

       
    

    $("input[name=cliente_bloqueadoBusqueda]").on("click", function () {
        var valor = $("input[name=cliente_bloqueadoBusqueda]:checked").val();
        changeInputBoolean('bloqueado', valor)
    });

    $("#cliente_bloqueado").change(function () {
        var valor = 1;
        if (!$('#cliente_bloqueado').prop('checked')) {
            valor = 0;
        }
        changeInputBoolean('bloqueado', valor)
    });

    $("#cliente_CanalMultireginal").change(function () {
        var valor = 1;
        if (!$('#cliente_CanalMultireginal').prop('checked')) {
            valor = 0;
        }
        changeInputBoolean('perteneceCanalMultiregional', valor)
    });

    $("#cliente_CanalLima").change(function () {
        var valor = 1;
        if (!$('#cliente_CanalLima').prop('checked')) {
            valor = 0;
        }
        changeInputBoolean('perteneceCanalLima', valor)
    });

    $("#cliente_CanalProvincias").change(function () {
        var valor = 1;
        if (!$('#cliente_CanalProvincia').prop('checked')) {
            valor = 0;
        }
        changeInputBoolean('perteneceCanalProvincias', valor)
    });

    $("#cliente_CanalPCP").change(function () {
        var valor = 1;
        if (!$('#cliente_CanalPCP').prop('checked')) {
            valor = 0;
        }
        changeInputBoolean('perteneceCanalPCP', valor)
    });

    $("#cliente_esSubdistribuidor").change(function () {
        var valor = 1;
        if (!$('#cliente_esSubdistribuidor').prop('checked')) {
            valor = 0;
        }
        changeInputBoolean('esSubDistribuidor', valor)
    });

    function changeInputBoolean(propiedad, valor) {
        $.ajax({
            url: "/Cliente/ChangeInputBoolean",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }



    $("input[name=cliente_sinPlazoCredito]").on("click", function () {
        var sinPlazoCredito = $("input[name=cliente_sinPlazoCredito]:checked").val();
        $.ajax({
            url: "/Cliente/ChangeSinPlazoCreditoAprobado",
            type: 'POST',
            data: {
                sinPlazoCredito: sinPlazoCredito
            },
            success: function () {
            }
        });
    });


    

    $("#btnBusqueda").click(function () {
        if ($("#cliente_textoBusqueda").val().length < 4 &&
            $("#idResponsableComercial").val() == 0 &&
            $("#idSupervisorComercial").val() == 0 &&
            $("#idAsistenteServicioCliente").val() == 0 &&
            $("input[name=cliente_bloqueadoBusqueda]:checked").val() == 0 &&
            $("input[name=cliente_sinPlazoCredito]:checked").val() == 0
            ) {
            $.alert({
                title: 'Validacion',
                content: 'Si no ha seleccionado en los criterios de búsqueda algún responsable, un grupo o no ha marcado la opción solo bloqueados o solo sin plazo de crédito entonces debe ingresar el texto a buscar utilizando 3 o más caracteres en el campo "N° Doc / Razón Social / Nombre".',
                type: 'orange',
                buttons: {
                    OK: function () {

                        $("#cliente_textoBusqueda").focus();
                    }
                }
            });
            $("#tableClientes > tbody").empty();
            $("#tableClientes").footable({
                "paging": {
                    "enabled": true
                }
            });
            return false;
        }

        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/Cliente/Search",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (clienteList) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableClientes > tbody").empty();
                $("#tableClientes").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < clienteList.length; i++) {

                    var textoBloqueado = "";
                    if (clienteList[i].bloqueado == true)
                        textoBloqueado = "Bloqueado";


                    var clienteRow = '<tr data-expanded="true">' +
                        '<td>  ' + clienteList[i].idPedido + '</td>' +
                        '<td>  ' + clienteList[i].codigo + '  </td>' +
                        '<td>  ' + clienteList[i].razonSocialSunat + '  </td>' +
                        '<td>  ' + clienteList[i].nombreComercial + ' </td>' +
                        '<td>  ' + clienteList[i].tipoDocumentoIdentidadToString + '</td>' +
                        '<td>  ' + clienteList[i].ruc + '  </td>' +
                        '<td>  ' + clienteList[i].ciudad.nombre + '  </td>' +
                        '<td>  ' + clienteList[i].responsableComercial.descripcion + '</td>' +
                        '<td>  ' + clienteList[i].supervisorComercial.descripcion + '</td>' +
                        '<td>  ' + clienteList[i].asistenteServicioCliente.descripcion + '</td>' +
                        '<td>  ' + clienteList[i].tipoPagoFacturaToString + '</td>' +
                        '<td>  ' + clienteList[i].creditoAprobado.toFixed(cantidadDecimales) + '  </td>' +
                        '<td>  ' + textoBloqueado + '  </td>' +
                        '<td>' +
                        '<button type="button" class="' + clienteList[i].idCliente + ' ' + clienteList[i].codigo + ' btnVerCliente btn btn-primary ">Ver</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tableClientes").append(clienteRow);

                }

                if (clienteList.length > 0)
                    $("#msgBusquedaSinResultados").hide();
                else
                    $("#msgBusquedaSinResultados").show();
            }
        });
    });

    $(document).on('click', "button.btnVerCliente", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idCliente = arrrayClass[0];
        var codigoCliente = arrrayClass[1];

        $.ajax({
            url: "/Cliente/Show",
            data: {
                idCliente: idCliente
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                mostrarMensajeErrorProceso();
            },
            success: function (cliente) {
                $('body').loadingModal('hide')
                $("#verCiudadNombre").html(cliente.ciudad.nombre);
                $("#verCodigo").html(cliente.codigo);
                $("#verRuc").html(cliente.ruc);
                $("#verTipoDocumentoIdentidad").html(cliente.tipoDocumentoIdentidadToString);
                $("#verNumeroDocumento").html(cliente.ruc);
                $("#verNombreComercial").html(cliente.nombreComercial);
                $("#verContacto").html(cliente.contacto1);
                $("#verCorreoEnvioFactura").html(cliente.correoEnvioFactura);
                if (cliente.bloqueado) {
                    $("#verBloqueado").html("Sí");
                }
                else {
                    $("#verBloqueado").html("No");
                }

                $("#verObservaciones").html(cliente.observaciones);

                /*Plazo Crédito*/
                $("#verPlazoCreditoSolicitado").html(cliente.plazoCreditoSolicitadoToString);
                $("#verPlazoCreditoAprobado").html(cliente.tipoPagoFacturaToString);
                $("#verSobrePlazo").html(cliente.sobrePlazo);               

                /*Montos de Crédito*/
                $("#verCreditoSolicitado").html(cliente.creditoSolicitado.toFixed(cantidadDecimales));
                $("#verCreditoAprobado").html(cliente.creditoAprobado.toFixed(cantidadDecimales));
                $("#verSobreGiro").html(cliente.sobreGiro.toFixed(cantidadDecimales));

                $("#verObservacionesCredito").html(cliente.observacionesCredito);
                $("#verFormaPagoFactura").html(cliente.formaPagoFacturaToString);

                /*Datos Sunat*/
                $("#verRazonSocialSunat").html(cliente.razonSocialSunat);       
                $("#verNombreComercialSunat").html(cliente.nombreComercialSunat);
                $("#verDireccionDomicilioLegalSunat").html(cliente.direccionDomicilioLegalSunat);



                $("#verEstadoContribuyente").html(cliente.estadoContribuyente);
                $("#verCondicionContribuyente").html(cliente.condicionContribuyente);

                /*
                $("#cliente_ubigeo_Departamento").val(cliente.ubigeo.Departamento);
                $("#cliente_ubigeo_Provincia").val(cliente.ubigeo.Provincia);
                $("#cliente_ubigeo_Distrito").val(cliente.ubigeo.Distrito);*/
                

                /*Vendedores*/
                $("#verResponsableComercial").html(cliente.responsableComercial.descripcion);
                $("#verSupervisorComercial").html(cliente.supervisorComercial.descripcion);
                $("#verAsistenteServicioCliente").html(cliente.asistenteServicioCliente.descripcion);



             //   $("#btnEditarCliente").show();
                
                $("#modalVerCliente").modal('show');
                
            }
        });
    });

    $("#btnEditarCliente").click(function () {
      //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona
        $.ajax({
            url: "/Cliente/ConsultarSiExisteCliente",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/Cliente/iniciarEdicionCliente",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del cliente."); },
                        success: function (fileName) {
                            window.location = '/Cliente/Editar';
                        }
                    });

                }
                else {
                    if (resultado.codigo == 0) {
                        alert('Está creando un nuevo cliente; para continuar por favor diríjase a la página "Crear/Modificar Cliente" y luego haga clic en el botón Cancelar.');
                    }
                    
                    else {
                        if (resultado.codigo == $("#verCodigo").html())
                            alert('Ya se encuentra editando el cliente con código ' + resultado.codigo + '; para continuar por favor dirigase a la página "Crear/Modificar Cliente".');
                        else
                            alert('Está editando el pedido número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Cliente" y luego haga clic en el botón Cancelar.');
                    }
                }
            }
        });
    });



});