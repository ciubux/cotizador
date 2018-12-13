
jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        $("#btnBusqueda").click();
        //cargarChosenCliente();
        verificarSiExisteCliente();
    });

    function verificarSiExisteCliente() {
        if ($("#idProducto").val().trim() != GUID_EMPTY) {
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

    $("#cliente_creditoSolicitado").change(function () {
        changeInputDecimal("creditoSolicitado", $("#cliente_creditoSolicitado").val())
    });

    $("#cliente_creditoAprobado").change(function () {
        changeInputDecimal("creditoAprobado", $("#cliente_creditoAprobado").val())
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
    
    $("#producto_exoneradoIgv").change(function () {
        var valor = 1;
        if (!$('#producto_exoneradoIgv').prop('checked')) {
            valor = 0;
        }
        changeInputBoolean('exoneradoIgv', valor)
    });

    $("#producto_inafecto").change(function () {
        var valor = 1;
        if (!$('#producto_inafecto').prop('checked')) {
            valor = 0;
        }
        changeInputBoolean('inafecto', valor)
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
                $("#verImagen").attr("src", "data:image/png;base64," + producto.image);
                $("#verSku").html(producto.sku);
                $("#verSkuProveedor").html(producto.skuProveedor);
                $("#verFamilia").html(producto.familia);
                $("#verProveedor").html(producto.proveedor);
                $("#verDescripcion").html(producto.descripcion);
                $("#verUnidad").html(producto.unidad);
                $("#verUnidadProveedor").html(producto.unidadProveedor);
                $("#verUnidadAlternativa").html(producto.unidad_alternativa);
                $("#verCosto").html(Number(producto.costoSinIgv).toFixed(cantidadCuatroDecimales));
                $("#verPrecio").html(Number(producto.precioSinIgv).toFixed(cantidadCuatroDecimales));
                $("#verPrecioProvincia").html(Number(producto.precioProvinciaSinIgv).toFixed(cantidadCuatroDecimales));
                $("#verEquivalencia").html(producto.equivalencia);
                $("#verEquivalenciaProveedor").html(producto.equivalenciaProveedor);
                $("#verUnidadEstandarInternacional").html(producto.unidadEstandarInternacional);

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


    
    $("#btnExportExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
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

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list[i].idProducto + '</td>' +
                        '<td>  ' + list[i].sku + '  </td>' +
                        '<td>  ' + list[i].skuProveedor + '  </td>' +
                        '<td>  ' + list[i].proveedor + ' </td>' +
                        '<td>  ' + list[i].familia + '</td>' +
                        '<td>  ' + list[i].descripcion + '  </td>' +
                        '<td>  ' + list[i].unidad + '  </td>' +
                        '<td>  ' + list[i].unidad_alternativa + '  </td>' +
                        '<td>  ' + list[i].equivalencia + '</td>' +
                        '<td>  ' + list[i].unidadProveedor + '</td>' +
                        '<td>  ' + list[i].equivalenciaProveedor + '</td>' +
                        '<td>  ' + Number(list[i].precioSinIgv).toFixed(cantidadCuatroDecimales) + '  </td>' +
                        '<td>  ' + Number(list[i].precioProvinciaSinIgv).toFixed(cantidadCuatroDecimales) + '  </td>' +
                        '<td>  ' + Number(list[i].costoSinIgv).toFixed(cantidadCuatroDecimales) + '  </td>' +
                        '<td>' +
                        '<button type="button" class="' + list[i].idProducto + ' ' + list[i].sku + ' btnVerProducto btn btn-primary ">Ver</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tableProductos").append(ItemRow);

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
    });
    
    $(document).on('click', "button.btnVerCliente", function () {
        $('body').loadingModal({
            text: 'Abriendo Cliente...'
        });
        $('body').loadingModal('show');

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
                $('body').loadingModal('hide');
                mostrarMensajeErrorProceso();
            },
            success: function (result) {
                var cliente = result.cliente;
                idClienteView = idCliente;
                $('body').loadingModal('hide')
                $("#verCiudadNombre").html(cliente.ciudad.nombre);
                $("#verCodigo").html(cliente.codigo);
                $("#verRuc").html(cliente.ruc);
                $("#verTipoDocumentoIdentidad").html(cliente.tipoDocumentoIdentidadToString);
                $("#verNumeroDocumento").html(cliente.ruc);
                $("#verNombreComercial").html(cliente.nombreComercial);
                $("#verContacto").html(cliente.contacto1);
                $("#verTelefonoContacto").html(cliente.telefonoContacto1);
                $("#verEmailContacto").html(cliente.emailContacto1);
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
                
                $("#verHoraInicioPrimerTurnoEntrega").html(cliente.horaInicioPrimerTurnoEntregaFormat);
                $("#verHoraFinPrimerTurnoEntrega").html(cliente.horaFinPrimerTurnoEntregaFormat);
                $("#verHoraInicioSegundoTurnoEntrega").html(cliente.horaInicioSegundoTurnoEntregaFormat);
                $("#verHoraFinSegundoTurnoEntrega").html(cliente.horaFinSegundoTurnoEntregaFormat);

                /*Vendedores*/
                $("#verResponsableComercial").html(cliente.responsableComercial.descripcion);
                $("#verSupervisorComercial").html(cliente.supervisorComercial.descripcion);
                $("#verAsistenteServicioCliente").html(cliente.asistenteServicioCliente.descripcion);

        
                $("#modalVerCliente").modal('show');
                        
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
        var maxSizeMb = fileInput.data('max-size-mb');

        if (fileInput.get(0).files.length) {
            var fileSize = fileInput.get(0).files[0].size; // in bytes

            if (fileSize > maxSize) {
                $.alert({
                    title: "Imagen Inválida",
                    type: 'red',
                    content: 'El tamaño del archivo debe ser como maximo ' + maxSizeMb + 'MB.',
                    buttons: {
                        OK: function () { }
                    }
                });

              
                event.preventDefault();
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
            event.preventDefault();
        }
    });

    $('#imgProductUpload').change(function (event) {
        
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
        
    });
});
