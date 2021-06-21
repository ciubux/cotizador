
jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';


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

    var fecha = $("#fechaInicioVigenciatmp").val();
    $("#fechaInicioVigencia").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fecha);

    $("#fechaInicioVigenciaTC").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fecha);

    var fechaCreacionDesde = $("#fechaCreacionDesdetmp").val();
    $("#producto_fechaCreacionDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaCreacionDesde);

    var fechaCreacionHasta = $("#fechaCreacionHastatmp").val();
    $("#producto_fechaCreacionHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaCreacionHasta);

    $(document).ready(function () {

        $("#btnBusqueda").click();
        //cargarChosenCliente();
        verificarSiExisteCliente();
    });

    function verificarSiExisteCliente() {
        var id = $("#idProducto").val();
        if (id.trim() != GUID_EMPTY) {
            //$("#idCiudad").attr("disabled", "disabled");
            //$("#tipoDocumentoIdentidad").attr("disabled", "disabled");
            //$("#cliente_ruc").attr("disabled", "disabled");
            $("#btnFinalizarEdicionProducto").html('Finalizar Edición');
        }
        else {
            $("#btnFinalizarEdicionProducto").html('Finalizar Creación');
        }

    }

    function limpiarFormulario() {
        $("#cliente_ruc").val("");
        $("#cliente_razonSocial").val("");
        $("#cliente_nombreComercial").val("");
        $("#cliente_domicilioLegal").val("");
        $("#cliente_contacto1").val("");
        $("#cliente_telefonoContacto1").val("");
        $("#cliente_emailContacto1").val("");
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


    function validacionDatosProducto() {
       
        if ($("#producto_sku").val().length < 4) {
            $.alert({
                title: "Código Inválido",
                type: 'orange',
                content: 'Debe ingresar un Código de producto válido.',
                buttons: {
                    OK: function () { $('#producto_sku').focus(); }
                }
            });
            return false;
        }
        
        if ($("#producto_skuProveedor").val().length < 4) {
            $.alert({
                title: "Código Proveedor Inválido",
                type: 'orange',
                content: 'Debe ingresar un Código proveedor válido.',
                buttons: {
                    OK: function () { $('#producto_skuProveedor').focus(); }
                }
            });
            return false;
        }

        if ($("#producto_descripcion").val().length < 2) {
            $.alert({
                title: "Descripción Inválida",
                type: 'orange',
                content: 'Debe ingresar una descripción del producto válida.',
                buttons: {
                    OK: function () { $('#producto_descripcion').focus(); }
                }
            });
            return false;
        }

        if ($("#producto_unidad").val().length < 1) {
            $.alert({
                title: "Unidad Inválida",
                type: 'orange',
                content: 'Debe ingresar una unidad válida.',
                buttons: {
                    OK: function () { $('#producto_unidad').focus(); }
                }
            });
            return false;
        }

        return true;

    }


    $("#btnFinalizarEdicionProducto").click(function () {
        /*Si no tiene codigo el cliente se está creando*/
        if ($("#producto_idProducto").val() == '00000000-0000-0000-0000-000000000000') {
            crearProducto();
        }
        else {
            editarProducto();
        }
    });



    function crearProducto() {
        if (!validacionDatosProducto())
            return false;

        $('body').loadingModal({
            text: 'Creando Producto...'
        });
        $.ajax({
            url: "/Producto/Create",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear el producto.',
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
                    content: 'El producto se creó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Producto/List';
                        }
                    }
                });
            }
        });

    }

    function editarProducto() {

        if (!validacionDatosProducto())
            return false;


        $('body').loadingModal({
            text: 'Editando Producto...'
        });
        $.ajax({
            url: "/Producto/Update",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar el producto.',
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
                    content: 'El producto se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/Producto/List';
                        }
                    }
                });
            }
        });
    }

    $("#producto_fechaCreacionDesde").change(function () {
        var fechaDesde = $("#producto_fechaCreacionDesde").val();
        var fechaHasta = $("#producto_fechaCreacionHasta").val();

        //Si fecha de entrega hasta es superior a fecha de entrega desde o la fecha de entrega hasta es vacío
        //se reemplaza el valor por la fecha de entrega desde
        if (convertirFechaNumero(fechaHasta) < convertirFechaNumero(fechaDesde)
            || fechaHasta.trim() == "") {
            $("#producto_fechaCreacionHasta").val(fechaDesde);
            $("#producto_fechaCreacionHasta").change();
        }

        $.ajax({
            url: "/Producto/ChangeFechaRegistroDesde",
            type: 'POST',
            data: {
                fechaDesde: fechaDesde
            },
            success: function () {
            }
        });
    });

    $("#producto_fechaCreacionHasta").change(function () {
        var fechaHasta = $("#producto_fechaCreacionHasta").val();
        $.ajax({
            url: "/Producto/ChangeFechaRegistroHasta",
            type: 'POST',
            data: {
                fechaHasta: fechaHasta
            },
            success: function () {
            }
        });
    });


    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/Producto/ChangeInputInt",
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
            url: "/Producto/ChangeInputDecimal",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#producto_compraRestringida_si").click(function () {
        var valCheck = 1;
        changeInputInt("compraRestringida", valCheck)
    });

    $("#producto_compraRestringida_no").click(function () {
        var valCheck = 0;
        changeInputInt("compraRestringida", valCheck)
    });

    $("#producto_estado_si").click(function () {
        var valCheck = 1;
        changeInputInt("Estado", valCheck)
    });

    $("#producto_estado_no").click(function () {
        var valCheck = 0;
        changeInputInt("Estado", valCheck)
    });
    
    $("#producto_estado_todos").click(function () {
        var valCheck = -1;
        changeInputInt("Estado", valCheck)
    });

    $("#producto_conImagen_si").click(function () {
        var valCheck = 1;
        changeInputInt("ConImagen", valCheck)
    });

    $("#producto_conImagen_no").click(function () {
        var valCheck = 0;
        changeInputInt("ConImagen", valCheck)
    });

    $("#producto_conImagen_todos").click(function () {
        var valCheck = -1;
        changeInputInt("ConImagen", valCheck)
    });


    
    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Producto/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function ChangeTipoProducto(tipoProducto) {
        $.ajax({
            url: "/Producto/ChangeTipoProducto",
            type: 'POST',
            data: { tipoProducto: tipoProducto },
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

    function ChangeTipoVentaRestringida(tipoVentaRestringida) {
        $.ajax({
            url: "/Producto/ChangeVentaRestringida",
            type: 'POST',
            data: { ventaRestringida: tipoVentaRestringida },
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

    $("#motivoRestriccion").change(function () {
        changeInputString("motivoRestriccion", $("#motivoRestriccion").val());
    });

    $("#producto_skuProveedor").change(function () {
        changeInputString("skuProveedor", $("#producto_skuProveedor").val());
    });

    $("#producto_sku").change(function () {
        changeInputString("sku", $("#producto_sku").val());
    });

    $("#producto_descripcion").change(function () {
        changeInputString("descripcion", $("#producto_descripcion").val());
    });

    $("#producto_unidad").change(function () {
        changeInputString("unidad", $("#producto_unidad").val());
    });

    $("#producto_unidadProveedor").change(function () {
        changeInputString("unidadProveedor", $("#producto_unidadProveedor").val());
    });

    $("#producto_unidad_alternativa").change(function () {
        changeInputString("unidad_alternativa", $("#producto_unidad_alternativa").val());
    });

    $("#producto_unidadPedidoProveedor").change(function () {
        changeInputString("unidadPedidoProveedor", $("#producto_unidadPedidoProveedor").val());
    });
    

    $("#producto_equivalenciaAlternativa").change(function () {
        changeInputInt("equivalenciaAlternativa", $("#producto_equivalenciaAlternativa").val());
    });

    $("#producto_equivalenciaProveedor").change(function () {
        changeInputInt("equivalenciaProveedor", $("#producto_equivalenciaProveedor").val());
    });

    $("#producto_equivalenciaUnidadEstandarUnidadConteo").change(function () {
        changeInputInt("equivalenciaUnidadEstandarUnidadConteo", $("#producto_equivalenciaUnidadEstandarUnidadConteo").val());
    });

    $("#producto_equivalenciaUnidadAlternativaUnidadConteo").change(function () {
        changeInputInt("equivalenciaUnidadAlternativaUnidadConteo", $("#producto_equivalenciaUnidadAlternativaUnidadConteo").val());
    });

    $("#producto_equivalenciaUnidadProveedorUnidadConteo").change(function () {
        changeInputInt("equivalenciaUnidadProveedorUnidadConteo", $("#producto_equivalenciaUnidadProveedorUnidadConteo").val());
    });

    $("#producto_equivalenciaUnidadPedidoProveedor").change(function () {
        changeInputInt("equivalenciaUnidadPedidoProveedor", $("#producto_equivalenciaUnidadPedidoProveedor").val());
    });

    $("#producto_cantidadMaximaPedidoRestringido").change(function () {
        changeInputInt("cantidadMaximaPedidoRestringido", $("#producto_cantidadMaximaPedidoRestringido").val());
    });


    $("#producto_unidadEstandarInternacional").change(function () {
        changeInputString("unidadEstandarInternacional", $("#producto_unidadEstandarInternacional").val());
    });

    $("#producto_unidadAlternativaInternacional").change(function () {
        changeInputString("unidadAlternativaInternacional", $("#producto_unidadAlternativaInternacional").val());
    });

    $("#producto_unidadProveedorInternacional").change(function () {
        changeInputString("unidadProveedorInternacional", $("#producto_unidadProveedorInternacional").val());
    });

    $("#tipoProducto").change(function () {
        ChangeTipoProducto($("#tipoProducto").val());
    });

    $("#producto_monedaMP").change(function () {
        changeInputString("monedaMP", $("#producto_monedaMP").val());
    });

    $("#producto_monedaProveedor").change(function () {
        changeInputString("monedaProveedor", $("#producto_monedaProveedor").val());
    });

    $("#producto_unidadConteo").change(function () {
        changeInputString("unidadConteo", $("#producto_unidadConteo").val());
    });

    $("#producto_codigoSunat").change(function () {
        changeInputString("codigoSunat", $("#producto_codigoSunat").val());
    });

    $("#producto_descripcionLarga").change(function () {
        changeInputString("descripcionLarga", $("#producto_descripcionLarga").val());
    });
    

    $("#tipoVentaRestringidaBusqueda").change(function () {
        changeInputInt("tipoVentaRestringidaBusqueda", $("#tipoVentaRestringidaBusqueda").val());
    });



    $("#producto_exoneradoIgv").change(function () {
        var valor = 1;
        if (!$('#producto_exoneradoIgv').prop('checked')) {
            valor = 0;
        }
        changeInputBoolean('exoneradoIgv', valor)
    });

    $("#chkAgregarDescripcionCotizacion").change(function () {
        var valor = 1;
        if (!$('#chkAgregarDescripcionCotizacion').prop('checked')) {
            valor = 0;
        }
        changeInputInt('agregarDescripcionCotizacion', valor)
    });
    

    $("#producto_precioOriginal").change(function () {
        changeInputDecimal("precioOriginal", $("#producto_precioOriginal").val());
    });

    $("#producto_precioProvinciasOriginal").change(function () {
        changeInputDecimal("precioProvinciasOriginal", $("#producto_precioProvinciasOriginal").val());
    });

    $("#producto_costoOriginal").change(function () {
        changeInputDecimal("costoOriginal", $("#producto_costoOriginal").val());
    });

    $("#producto_costoReferencialOriginal").change(function () {
        changeInputDecimal("costoReferencialOriginal", $("#producto_costoReferencialOriginal").val());
    });

    $("#producto_tipoCambio").change(function () {
        changeInputDecimal("tipoCambio", $("#producto_tipoCambio").val());
    });

    $("#producto_topeDescuento").change(function () {
        changeInputDecimal("topeDescuento", $("#producto_topeDescuento").val());
    });

    

    $("#producto_precioSinIgv").change(function () {
        changeInputDecimal("precioSinIgv", $("#producto_precioSinIgv").val());
    });

    $("#producto_precioProvinciaSinIgv").change(function () {
        changeInputDecimal("precioProvinciaSinIgv", $("#producto_precioProvinciaSinIgv").val());
    });

    $("#producto_costoSinIgv").change(function () {
        changeInputDecimal("costoSinIgv", $("#producto_costoSinIgv").val());
    });

    $("#producto_inafecto").change(function () {
        var valor = 1;
        if (!$('#producto_inafecto').prop('checked')) {
            valor = 0;
        }
        changeInputBoolean('inafecto', valor)
    });

    
    $("#ventaRestringida").change(function () {
        ChangeTipoVentaRestringida($("#ventaRestringida").val());

        var valor = parseInt($("#ventaRestringida").val());
        
        if (valor > 0) {
            $("#motivoRestriccion").show();
        } else {
            $("#motivoRestriccion").hide();
        }
    });

    $(".chk_campo_registra").change(function () {
        if ($(this).prop('checked')) {
            $(this).closest("tr").find(".chk_campo_persiste").removeAttr("disabled");
        } else {
            $(this).closest("tr").find(".chk_campo_persiste").attr("disabled", "disabled");
            $(this).closest("tr").find(".chk_campo_persiste").prop('checked', false);
        }

    });
    
    


    $("#btnCancelarProducto").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/Producto/CancelarCreacionProducto', null)
    })




    var ft = null;

    
    $(document).on('click', "button.btnVerProducto", function () {
        $('body').loadingModal({
            text: 'Abriendo Producto...'
        });
        $('body').loadingModal('show');

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idProducto = arrrayClass[0];

        $.ajax({
            url: "/Producto/Show",
            data: {
                idProducto: idProducto
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                mostrarMensajeErrorProceso();
            },
            success: function (result) {
                var producto = result;
                $('body').loadingModal('hide')
                $("#verIdProducto").html(producto.idProducto);

                if (result.Estado == 1) {
                    $("#verEstado").html('<span style="color: green; font-weight: bold;">Activo</span>');
                } else {
                    $("#verEstado").html('<span style="color: red; font-weight: bold;">Inactivo</span>');
                }
                $("#verImagen").attr("src", "data:image/png;base64," + producto.image);
                $("#verSku").html(producto.sku);
                $("#verSkuProveedor").html(producto.skuProveedor);
                $("#verFamilia").html(producto.familia);
                $("#verProveedor").html(producto.proveedor);
                $("#verDescripcion").html(producto.descripcion);
                $("#verDescripcionLarga").html(producto.descripcionLarga);

                $("#verUnidadConteo").html(producto.unidadConteo);
                $("#verUnidadAlternativaInternacional").html(producto.unidadAlternativaInternacional);
                $("#verUnidadProveedorInternacional").html(producto.unidadProveedorInternacional);
                $("#verUnidadPedidoProveedor").html(producto.unidadPedidoProveedor);

                $("#verEquivalenciaUnidadEstandarUnidadConteo").html(producto.equivalenciaUnidadEstandarUnidadConteo);
                $("#verEquivalenciaUnidadAlternativaUnidadConteo").html(producto.equivalenciaUnidadAlternativaUnidadConteo);
                $("#verEquivalenciaUnidadProveedorUnidadConteo").html(producto.equivalenciaUnidadProveedorUnidadConteo);
                $("#verEquivalenciaUnidadPedidoProveedor").html(producto.equivalenciaUnidadPedidoProveedor);

                $("#verUnidad").html(producto.unidad);
                $("#verUnidadProveedor").html(producto.unidadProveedor);
                $("#verUnidadAlternativa").html(producto.unidad_alternativa);
                $("#verCosto").html(Number(producto.costoSinIgv).toFixed(cantidadCuatroDecimales));
                $("#verPrecio").html(Number(producto.precioSinIgv).toFixed(cantidadCuatroDecimales));
                $("#verPrecioProvincia").html(Number(producto.precioProvinciaSinIgv).toFixed(cantidadCuatroDecimales));
                $("#verEquivalenciaAlternativa").html(producto.equivalenciaAlternativa);
                $("#verEquivalenciaProveedor").html(producto.equivalenciaProveedor);
                $("#verUnidadEstandarInternacional").html(producto.unidadEstandarInternacional);
                $("#verTipo").html(producto.tipoProductoToString);


                $("#verTipoCambio").html(Number(producto.tipoCambio).toFixed(cantidadDecimales));

                if (producto.monedaMP == "D") {
                    $("#verMonedaMP").html("DÓLARES");
                }
                else {
                    $("#verMonedaMP").html("SOLES");
                }

                if (producto.monedaProveedor == "D") {
                    $("#verMonedaProveedor").html("DÓLARES");
                }
                else {
                    $("#verMonedaProveedor").html("SOLES");
                }

                $("#verCodigoSunat").html(producto.codigoSunat);

                $("#verCostoOriginal").html(Number(producto.costoOriginal).toFixed(cantidadCuatroDecimales));
                $("#verCostoReferencialOriginal").html(Number(producto.costoReferencialOriginal).toFixed(cantidadCuatroDecimales));
                
                $("#verPrecioOriginal").html(Number(producto.precioOriginal).toFixed(cantidadCuatroDecimales));
                $("#verPrecioProvinciasOriginal").html(Number(producto.precioProvinciasOriginal).toFixed(cantidadCuatroDecimales));

                $("#verTopeDescuento").html(Number(producto.topeDescuento).toFixed(cantidadDecimales) + " %");

                if (producto.descontinuado) {
                    var spnMotivoRestriccion = "";
                    if (producto.motivoRestriccion != null) {
                        spnMotivoRestriccion = '<br/><span style="font-style: italic;">' + producto.motivoRestriccion + '</span>';
                    }
                    $("#verDescontinuado").html(producto.tipoVentaRestingidaToString + spnMotivoRestriccion);
                }
                else {
                    $("#verDescontinuado").html("No");
                }

                if (producto.exoneradoIgv) {
                    $("#verExoneradoIgv").html("Sí");
                }
                else {
                    $("#verExoneradoIgv").html("No");
                }

                if (producto.inafecto) {
                    $("#verInafecto").html("Sí");
                }
                else {
                    $("#verInafecto").html("No");
                }

                if (producto.agregarDescripcionCotizacion) {
                    $("#verAgregarDescripcionCotizacion").html("Sí");
                }
                else {
                    $("#verAgregarDescripcionCotizacion").html("No");
                }

                if (producto.compraRestringida == 1) {
                    $("#verCompraRestringida").html("Sí");
                }
                else {
                    $("#verCompraRestringida").html("No");
                }

                $("#verCantidadMaximaPedidoRestringido").html(producto.cantidadMaximaPedidoRestringido);

                $(".verModalStockProducto").attr("sku", producto.sku);
                

                $("#modalVerProducto").modal('show');        
            }
        });
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
        else if (considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES) {
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


    function changeInputBoolean(propiedad, valor) {
        $.ajax({
            url: "/Producto/ChangeInputBoolean",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }
    
    
    $("#lnkActualizarTodos").click(function () {
        $("#tipoCambioTC").val($("#tipoCambio").val());
        $("#fechaInicioVigenciaTC").val($("#fechaInicioVigencia").val());
    });


    $("#btnActualizarTipoCambio").click(function () {
        $("#formActualizarTipoCambio").submit();
    });
    

    $("#btnExportExcel, #btnExportUploadExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
    });

    $("#btnCargarProductos").click(function () {
        var listaTextosCampos = "";
        $(".chk_campo_registra").each(function () {
            if (!$(this).prop('checked') && !$(this)[0].hasAttribute('disabled')) {
                if (listaTextosCampos != "") listaTextosCampos = listaTextosCampos + "<br/>";
                listaTextosCampos = listaTextosCampos + $(this).closest("tr").find("td.nombreCampo").html();
            }
        });

        if (listaTextosCampos != "") {
            $.confirm({
                title: 'Registro Parcial',
                content: '<div><div class="col-sm-12"><b>Los siguientes campos no se seleccionaron: </b></div><br/><div class="col-sm-12">' + listaTextosCampos + '</div><br/><div class="col-sm-12"><b>¿Desea continuar? </b></div></div>',
                type: 'orange',
                buttons: {
                    aplica: {
                        text: 'SI',
                        btnClass: 'btn-success',
                        action: function () {
                            $('body').loadingModal({
                                text: 'Leyendo Archivo...'
                            });
                            $("#formCargarProductos").submit();
                        }
                    },
                    cancelar: {
                        text: 'Cancelar',
                        btnClass: '',
                        action: function () {
                        }
                    }
                }
            });
        } else {
            $.confirm({
                title: 'Información Actualización',
                content: 'Se realizará la actualización de todos los campos seleccionados. ¿Desea continuar?',
                type: 'orange',
                buttons: {
                    aplica: {
                        text: 'SI',
                        btnClass: 'btn-success',
                        action: function () {
                            $('body').loadingModal({
                                text: 'Leyendo Archivo...'
                            });
                            $("#formCargarProductos").submit();
                        }
                    },
                    cancelar: {
                        text: 'Cancelar',
                        btnClass: '',
                        action: function () {
                        }
                    }
                }
            });
        }
        
    });

    

    $("#btnBusqueda").click(function () {

        
        if ($("#producto_sku").val().length < 2 &&
            $("#producto_skuProveedor").val().length < 2 &&
            $("#producto_descripcion").val().length < 2 &&
            $("#familia").val() == 'Todas' && $("#proveedor").val() == 'Todos'
            ) {
            $.alert({
                title: 'Ingresar texto a buscar',
                content: 'Debe ingresar el texto a buscar utilizando 2 o más caracteres en los campos Código, Código Proveedor y Descripión"',
                type: 'orange',
                buttons: {
                    OK: function () {
                        $("#producto_sku").focus();
                    }
                }
            });
            $("#tableProductos > tbody").empty();
            $("#tableProductos").footable({
                "paging": {
                    "enabled": true
                }
            });

            
            return false;
        }

        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/Producto/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableProductos > tbody").empty();
                $("#tableProductos").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < list.length; i++) {

                    var estado = "";
                    if (list[i].Estado == 1) {
                        estado = '<span style="color: green; font-weight: bold;">' + list[i].EstadoDesc+'</span>';
                    } else {
                        estado = '<span style="color: red; font-weight: bold;">' + list[i].EstadoDesc+'</span>';
                    }

                    var descontinuadoHTML = "";
                    if (list[i].descontinuado == 1) {
                        descontinuadoHTML = "<br/>" + $("#spnProductoDescontinuado").html();

                        if (list[i].motivoRestriccion != null) {
                            list[i].motivoRestriccion = list[i].motivoRestriccion.trim();
                            descontinuadoHTML = descontinuadoHTML.replace("_DATA_TIPSO_", list[i].motivoRestriccionCompuesto);

                            if (list[i].motivoRestriccion != "") {
                                descontinuadoHTML = descontinuadoHTML.replace("_CLASS_TOOLTIP_", "tooltip-motivo-restriccion");
                            }
                        }
                    }

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list[i].idProducto + '</td>' +
                        '<td>  ' + list[i].sku + descontinuadoHTML + '  </td>' +
                        '<td>  ' + list[i].skuProveedor + '  </td>' +
                        '<td>  ' + list[i].proveedor + ' </td>' +
                        '<td>  ' + list[i].familia + '</td>' +
                        '<td>  ' + list[i].tipoProductoToString + '</td>' +
                        '<td>  ' + list[i].descripcion + '  </td>' +
                        '<td>  ' + list[i].unidad + '  </td>' +
                        '<td>  ' + list[i].unidad_alternativa + '  </td>' +
                        '<td>  ' + list[i].equivalenciaAlternativa + '</td>' +
                        '<td>  ' + list[i].unidadProveedor + '</td>' +
                        '<td>  ' + list[i].equivalenciaProveedor + '</td>' +
                        '<td>  ' + Number(list[i].precioSinIgv).toFixed(cantidadCuatroDecimales) + '  </td>' +
                        '<td>  ' + Number(list[i].precioProvinciaSinIgv).toFixed(cantidadCuatroDecimales) + '  </td>' +
                        '<td>  ' + Number(list[i].costoSinIgv).toFixed(cantidadCuatroDecimales) + '  </td>' +
                        '<td>  ' + estado + '  </td>' +
                        '<td>  ' + list[i].FechaEdicionFormatoFecha + '  </td>' +
                        '<td>' +
                        '<button type="button" class="' + list[i].idProducto + ' ' + list[i].sku + ' btnVerProducto btn btn-primary ">Ver</button>' +
                        '<button type="button" idProducto="' + list[i].idProducto + '" skuProducto="' + list[i].sku + '" nombreProducto="' + list[i].descripcion + '" class="btnVerHistorialProducto btn btn-success ">Historial</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tableProductos").append(ItemRow);

                }

                /*
                setTimeout(function () {
                    $('.tooltip-motivo-restriccion').tipso(
                        {
                            titleContent: 'MOTIVO',
                            titleBackground: '#f0ad4e',
                            titleColor: '#111111',
                            background: '#ffffff',
                            color: '#000000',
                            width: 300
                        });
                }, 1000);
                */
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

    $("#btnEditarProducto").click(function () {
      //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona
        $.ajax({
            url: "/Producto/ConsultarSiExisteProducto",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/Producto/iniciarEdicionProducto",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del Producto."); },
                        success: function (fileName) {
                            window.location = '/Producto/Editar';
                        }
                    });

                }
                else {
                    if (resultado.idProducto == "00000000-0000-0000-0000-000000000000") {
                        alert('Está creando un nuevo producto; para continuar por favor diríjase a la página "Crear/Modificar Producto" y luego haga clic en el botón Cancelar.');
                    }
                    
                    else {
                        alert('Ya se encuentra editando un producto para continuar por favor dirigase a la página "Crear/Modificar Producto".');
                    }
                }
            }
        });
    });



});


$(document).ready(function () {
    $('#imgProductUpload').change(function (event) {
        
        var fileInput = $(event.target);
        var maxSize = fileInput.data('max-size');
        var maxSizeText = fileInput.data('max-size-text');
        var imagenValida = true;
        if (fileInput.get(0).files.length) {
            var fileSize = fileInput.get(0).files[0].size; // in bytes

            if (fileSize > maxSize) {
                $.alert({
                    title: "Imagen Inválida",
                    type: 'red',
                    content: 'El tamaño del archivo debe ser como maximo ' + maxSizeText + '.',
                    buttons: {
                        OK: function () { }
                    }
                });
                imagenValida = false;
            }


        } else {
            $.alert({
                title: "Imagen Inválida",
                type: 'red',
                content: 'Seleccione una imagen por favor.',
                buttons: {
                    OK: function () { }
                }
            });
            imagenValida = false;
        }

        if (imagenValida) {
            $('body').loadingModal({
                text: '...'
            });

            var that = document.getElementById('imgProductUpload');
            var file = that.files[0];
            var form = new FormData();
            var url = $(that).data("urlSetImage");
            var reader = new FileReader();
            var mime = file.type;

            reader.onload = function (e) {
                // get loaded data and render thumbnail.
                document.getElementById("verImagen").src = e.target.result;
                var img = new Image();

                img.onload = function () {
                    var width = img.width;
                    var height = img.height;
                };
                img.src = e.target.result;

                $.ajax({
                    url: url,
                    type: 'POST',
                    cache: false,
                    data: {
                        imgBase: $("#verImagen").attr("src")
                    },
                    success: function () { }

                }).done(function () {
                    $('body').loadingModal('hide');
                });
            };

            // read the image file as a data URL.
            reader.readAsDataURL(file);

            form.append('image', file);
        }
    });
});
