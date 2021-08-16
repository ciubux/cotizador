jQuery(function ($) {


   

    //CONSTANTES:
    var cantidadDecimales = 2;
    var cantidadDecimalesPrecioNeto = 2;
    var IGV = 0.18;
    var SIMBOLO_SOL = "S/";
    var MILISEGUNDOS_AUTOGUARDADO = 5000;
    var ESTADOS_TODOS = -1;
    var DIAS_MAX_VALIDEZ_OFERTA_COTIZACION_PUNTUAL = 15;
    var DIAS_DEFECTO_VALIDEZ_OFERTA_COTIZACION_PUNTUAL = 15;

    var TRABAJAR_CON_UNIDADES = true;

    var ESTADO_PENDIENTE_APROBACION = 0;
    var ESTADO_APROBADA = 1;
    var ESTADO_DENEGADA = 2;
    var ESTADO_ACEPTADA = 3;
    var ESTADO_RECHAZADA = 4;
    var ESTADO_EN_EDICION = 5;
    var ESTADO_ELIMINADA = 6;

    var ESTADO_PENDIENTE_APROBACION_STR = "Pendiente de Aprobación";
    var ESTADO_APROBADA_STR = "Aprobada";
    var ESTADO_DENEGADA_STR = "Denegada";
    var ESTADO_ACEPTADA_STR = "Aceptada";
    var ESTADO_ELIMINADA_STR = "Eliminada";
    var ESTADO_RECHAZADA_STR = "Rechazada";
    var ESTADO_EN_EDICION_STR = "En Edición";

    var DIRIGIDO_A_CLIENTE = "cliente";
    var DIRIGIDO_A_GRUPO = "grupo";


    var CANT_SOLO_OBSERVACIONES = 0;
    var CANT_SOLO_CANTIDADES = 1;
    var CANT_CANTIDADES_Y_OBSERVACIONES = 2;

    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';

    $(document).ready(function () {
        if ($("#productosInactivosRemovidos").length) {
            var removioProductosInactivos = $("#productosInactivosRemovidos").val();
            if (removioProductosInactivos == '1') {
                $.alert({
                    title: "Productos Inactivos Removidos",
                    type: 'orange',
                    content: 'La cotización original tenía productos inactivos que fueron removidos.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }
        }

        cambiarMostrarValidezOfertaEnDias();
  //      setTimeout(autoGuardarCotizacion, MILISEGUNDOS_AUTOGUARDADO);
        obtenerConstantes();
        cargarChosenCliente();
        verificarSiExisteDetalle();
        verificiarSiFechaEsModificada();
        verificarSiExisteCliente();
        ajustarFormularioPorTipoCotizacion();
        $("#btnBusquedaCotizaciones").click();

        $('#tipoCotizacion option[value="2"]').remove();

        
    });

    function Alerta(message) {
        $('<div ></div>').appendTo('body')
            .html('<div><h6>' + message + '</h6></div>')
            .dialog({
                modal: true, title: 'Mensaje', top: 0,
                left: 0, zIndex: 10000, autoOpen: true, 
                width: 'auto', resizable: false,
                buttons: {
                    Aceptar: function () {
                        $(this).dialog("close");
                    }
                },
                close: function (event, ui) {
                    $(this).remove();
                }
            });
        document.body.scrollTop = default_scrollTop;
    };

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
    
    function ConfirmDialog(message,redireccionSI, redireccionNO) {
        $('<div></div>').appendTo('body')
            .html('<div><h6>' + message + '</h6></div>')
            .dialog({
                modal: true, title: 'Confirmación', zIndex: 10000, autoOpen: true,
                width: 'auto', resizable: false,
                buttons: {
                    Si: function () {
                        if (redireccionSI!=null)
                            window.location = redireccionSI;
                        $(this).dialog("close");
                        
                    },
                    No: function () {
                        if (redireccionNO!=null)
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
        if ($("#idCliente").val().trim() != "" && $("#pagina").val() == PAGINA_MANTENIMIENTO_COTIZACION)
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
                DIAS_MAX_VALIDEZ_OFERTA_COTIZACION_PUNTUAL = constantes.DIAS_MAX_VALIDEZ_OFERTA_COTIZACION_PUNTUAL;
                DIAS_DEFECTO_VALIDEZ_OFERTA_COTIZACION_PUNTUAL = constantes.DIAS_DEFECTO_VALIDEZ_OFERTA_COTIZACION_PUNTUAL;
            }
        });
    }

    function autoGuardarCotizacion() {
        $.ajax({
            url: "/Cotizacion/autoGuardarCotizacion",
            type: 'POST',
            error: function () {
                setTimeout(autoGuardarCotizacion, MILISEGUNDOS_AUTOGUARDADO);
            },
            success: function () {
                setTimeout(autoGuardarCotizacion, MILISEGUNDOS_AUTOGUARDADO);
            }
        });
    }


    function verificiarSiFechaEsModificada() {
        if ($('#chkFechaEsModificada').prop('checked')) {
            $("#fecha").removeAttr("disabled");
        }
        else {
            $("#fecha").attr('disabled', 'disabled');
        }
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

  

    function abrirEditorCliente() {
        window.open(
            "/Cliente/Editar?idCliente=" + GUID_EMPTY,
            "Creación de Cliente",
            "resizable,scrollbars,status"
        );
    }

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


    var fecha = $("#fechatmp").val();
    $("#fecha").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fecha);

    var fechaDesde = $("#fechaDesdetmp").val();
    $("#fechaDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaDesde);

    var fechaHasta = $("#fechaHastatmp").val();
    $("#fechaHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaHasta);

    var fechaPrecios = $("#fechaPreciostmp").val();
    $("#fechaPrecios").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaPrecios);    

    var fechaLimiteValidezOferta = $("#fechaLimiteValidezOfertaTmp").val();
    $("#fechaLimiteValidezOferta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaLimiteValidezOferta);

    var fechaInicioVigenciaPrecios = $("#fechaInicioVigenciaPreciosTmp").val();
    $("#fechaInicioVigenciaPrecios").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaInicioVigenciaPrecios);

    var fechaFinVigenciaPrecios = $("#fechaFinVigenciaPreciosTmp").val();
    $("#fechaFinVigenciaPrecios").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaFinVigenciaPrecios);

    $("#fecha").change(function () {
        var fecha = $("#fecha").val();
        $.ajax({
            url: "/Cotizacion/updateFecha",
            type: 'POST',
            data: {
                fecha: fecha
            },
            success: function () { }
        });
    });

    $('#chkFechaEsModificada').change(function () {
        var fechaEsModificada = 1;
        if ($('#chkFechaEsModificada').prop('checked')) {
            $("#fecha").removeAttr("disabled");
        }
        else {
            $("#fecha").attr('disabled', 'disabled');
            $("#fecha").datepicker().datepicker("setDate", new Date());
            fechaEsModificada = 0;
        }
        $.ajax({
            url: "/Cotizacion/updateFechaEsModificada",
            type: 'POST',
            data: {
                fechaEsModificada: fechaEsModificada
            },
            success: function () { }
        });
        



    });



    function changeInputBoolean(propiedad, valor) {
        $.ajax({
            url: "/Cotizacion/ChangeInputBoolean",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#cotizacion_sku").change(function () {
        changeInputString("sku", $("#cotizacion_sku").val())
    });

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Cotizacion/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }



    $("#mostrarValidezOfertaEnDias").change(function () {
        var mostrarValidezOfertaEnDias = $("#mostrarValidezOfertaEnDias").val();
        $.ajax({
            url: "/Cotizacion/updateMostrarValidezOfertaEnDias",
            type: 'POST',
            data: {
                mostrarValidezOfertaEnDias: mostrarValidezOfertaEnDias
            },
            success: function () {
                cambiarMostrarValidezOfertaEnDias();
            }
        });

    });

    $("#validezOfertaEnDias").change(function () {
        var validezOfertaEnDias = $("#validezOfertaEnDias").val();
        $.ajax({
            url: "/Cotizacion/updateValidezOfertaEnDias",
            type: 'POST',
            data: {
                validezOfertaEnDias: validezOfertaEnDias
            },
            success: function () {

            }
        });

    });

    $("#fechaLimiteValidezOferta").change(function () {
        var fechaLimiteValidezOferta = $("#fechaLimiteValidezOferta").val();
        $.ajax({
            url: "/Cotizacion/updateFechaLimiteValidezOferta",
            type: 'POST',
            data: {
                fechaLimiteValidezOferta: fechaLimiteValidezOferta
            },
            success: function () {
            }
        });
    });



    function cambiarMostrarValidezOfertaEnDias() {
        var mostrarValidezOfertaEnDias = $('#mostrarValidezOfertaEnDias').val();

        if (mostrarValidezOfertaEnDias == "0") {
            $('#diasVigenciadiv').show();
            $('#fechaVigenciadiv').hide();
        }
        else {
            $('#diasVigenciadiv').hide();
            $('#fechaVigenciadiv').show();
        }
    }

    $("#fechaInicioVigenciaPrecios").change(function () {
        var fechaInicioVigenciaPrecios = $("#fechaInicioVigenciaPrecios").val();
        $.ajax({
            url: "/Cotizacion/updateFechaInicioVigenciaPrecios",
            type: 'POST',
            data: {
                fechaInicioVigenciaPrecios: fechaInicioVigenciaPrecios
            },
            success: function () {
            }
        });
    });

    $("#fechaFinVigenciaPrecios").change(function () {
        var fechaFinVigenciaPrecios = $("#fechaFinVigenciaPrecios").val();
        $.ajax({
            url: "/Cotizacion/updateFechaFinVigenciaPrecios",
            type: 'POST',
            data: {
                fechaFinVigenciaPrecios: fechaFinVigenciaPrecios
            },
            success: function () {
            }
        });
    });





    /**
     * FIN DE CONTROLES DE FECHAS
     */


    /**
     * ################################ INICIO CONTROLES DE CLIENTE
     */



    $("#idGrupoCliente").change(function () {
        var idGrupoCliente = $("#idGrupoCliente").val();
        $.ajax({
            url: "/Cotizacion/updateIdGrupoCliente",
            dataType: 'JSON',
            type: 'POST',
            data: {
                idGrupoCliente: idGrupoCliente
            },
            success: function (grupoCliente) {
                $("#cotizacion_textoCondicionesPago").val(grupoCliente.textoCondicionesPago);
                $("#contacto").val(grupoCliente.contacto);
            }
        });
    });


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
            url: "/Cotizacion/SearchClientes"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });

    }

    $("#idCliente").change(function () {
        $("#contacto").val("");
        var idClienteGrupo = $(this).val();

        var tipoCliente = idClienteGrupo.substr(0, 1);

        idClienteGrupo = idClienteGrupo.substr(1);

        if (tipoCliente == "c") {
            $.ajax({
                url: "/Cotizacion/GetCliente",
                type: 'POST',
                dataType: 'JSON',
                data: {
                    idCliente: idClienteGrupo
                },
                success: function (cliente) {

                    if ($("#pagina").val() == 1) {
                        $("#idCiudad").attr("disabled", "disabled");
                        
                    }
                    $("#cotizacion_textoCondicionesPago").val(cliente.textoCondicionesPago);
                    $("#contacto").val(cliente.contacto);
                    $("#clienteSedePrincipal").val(cliente.sedePrincipal);
                    listaTextoSedesCliente = cliente.sedesString;
                }
            });
        }
        else {
            $.ajax({
                url: "/Cotizacion/GetGrupo",
                type: 'POST',
                dataType: 'JSON',
                data: {
                    idGrupo: idClienteGrupo
                },
                success: function (grupo) {
                    $("#idCiudad").attr("disabled", "disabled");
                    $("#contacto").val(grupo.contacto);
                }
            });

        }
    });

    $("#btnAgregarCliente").click(function () {
        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar la sede MP previamente.");
            $("#idCiudad").focus();
            $('#btnCancelCliente').click();
            return false;
        }


        $.confirm({
            title: 'REGISTRO NUEVO CLIENTE',
            content: 'Seleccione tipo de registro de cliente que desea realizar:',
            type: 'orange',
            buttons: {
                aplica: {
                    text: 'DATOS COMPLETOS',
                    btnClass: 'btn-success',
                    action: function () {
                        abrirEditorCliente();
                    }
                },
                noAplica: {
                    text: 'DATOS BÁSICOS (Cliente potencial)',
                    btnClass: 'btn-warning',
                    action: function () {
                        $('#modalAgregarClienteLite').modal('show')
                    }
                },
                cancelar: {
                    text: 'CANCELAR',
                    btnClass: '',
                    action: function () {

                    }
                }
            }
        });
    });


    $('#modalAgregarCliente').on('shown.bs.modal', function () {

        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar la sede MP previamente.");
            $("#idCiudad").focus();
            $('#btnCancelCliente').click();
            return false;
        }


    });

    $('#modalAgregarClienteLite').on('shown.bs.modal', function () {

        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar la sede MP previamente.");
            $("#idCiudad").focus();
            $('#btnCancelClienteLite').click();
            return false;
        }


    });

    $("#btnSaveClienteLite").click(function () {
        var cliente = $("#nclCliente").val().trim();
        var idOrigen = $("#nclIdOrigen").val().trim();
        var idRubro = $("#nclIdRubro").val().trim();
        var nombre = $("#nclContacto1").val().trim();
        var telefono = $("#nclTelefonoContacto1").val().trim();
        var email = $("#nclEmailContacto1").val().trim();
        var observaciones = $("#ncObservaciones").val().trim();
        
        if (nombre == "") {
            $.alert({
                title: "Error de validación",
                type: 'orange',
                content: 'Debe digitar un nombre.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }

        if (telefono == "") {
            $.alert({
                title: "Error de validación",
                type: 'orange',
                content: 'Debe digitar un número de teléfono.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }


        $.ajax({
            url: '/Cotizacion/CreateClienteLite',
            type: 'POST',
            dataType: 'JSON',
            data: {
                cliente: cliente,
                idOrigen: idOrigen,
                idRubro: idRubro,
                nombreContacto: nombre,
                telefonoContacto: telefono,
                emailContacto: email,
                observaciones: observaciones
            }
        });

        window.location = '/Cotizacion/Cotizar';
    });

    /**
     * FIN CONTROLES DE CLIENTE
     */











    function indicarFamiliaTodas() {
        $.ajax({
            url: '/Familia/ChangeFamilia',
            type: 'POST',
            dataType: 'JSON',
            data: {
                familia: "Todas"
            }
        });
        $('#familia').val("Todas");
        $('#familiaBusquedaPrecios').val("Todas");
        

    }

    function indicarProveedorTodos() {

        $.ajax({
            url: '/Proveedor/ChangeProveedor',
            type: 'POST',
            dataType: 'JSON',
            data: {
                familia: "Todos"
            }
        });
        $('#proveedor').val("Todos");
        $('#proveedorBusquedaPrecios').val("Todos");
    }


    /**
     * ################################ INICIO CONTROLES DE AGREGAR PRODUCTO
     */

    ////////////////ABRIR AGREGAR PRODUCTO

    function limpiarCamposAgregarProductos() {
        $("#unidad").html("");
        $("#imgProducto").attr("src", "images/NoDisponible.gif");
        $("#precioUnitarioSinIGV").val(0);
        $("#precioUnitarioAlternativoSinIGV").val(0);
        $('#costoSinIGV').val(0);
        $('#costoAlternativoSinIGV').val(0);
        $("#subtotal").val(0);
        $("#porcentajeDescuento").val(Number(0).toFixed(4));
        $('#valor').val(0);
        $('#valorAlternativo').val(0);
        $('#observacionProducto').val("");
        $('#valor').attr('type', 'text');
        $('#valorAlternativo').attr('type', 'hidden');
        $('#precio').val(0);
        $('#cantidad').val(1);
        $('#spnProductoDescontinuado').hide();
        

        // $("#proveedor").val(producto.proveedor);
        // $("#familia").val(producto.familia);
        $('#fleteDetalle').val(0);
        $("#costoLista").val(0);
        $("#precioLista").val(0);
        $("#tableMostrarPrecios > tbody").empty();

    }



    $("#btnModalCargarProductos").click(function () {
        if (!validarSeleccionClienteOGrupo()) {
            return false;
        }
    });

    $('#btnOpenAgregarProducto').click(function () {
        $("#btnStockUnidadAddProduct").attr('disabled', 'disabled');
        indicarFamiliaTodas();
        indicarProveedorTodos();
        if (!validarSeleccionClienteOGrupo()) {
            return false;
        }
        //Se limpia el mensaje de resultado de agregar producto
        $("#resultadoAgregarProducto").html("");

        //Se desactiva el boton de agregar producto
        desactivarBtnAddProduct();

        //Se limpian los campos
        limpiarCamposAgregarProductos();

        //Se agrega chosen al campo PRODUCTO
        $("#producto").chosen({ placeholder_text_single: "Seleccione el producto", no_results_text: "No se encontró Producto" });

        $("#producto").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            allow_single_deselect: true,
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
        


        limpiarCamposAgregarProductos
        $('#familia').focus();





        //$('#producto').trigger('chosen:activate');
    })

    var agregarDescripcionLargaCotizacion = 0;

    /////////////CAMPO PRODUCTO 
    $("#producto").change(function () {
        $("#resultadoAgregarProducto").html("");
        desactivarBtnAddProduct();
        $.ajax({
            url: "/Cotizacion/GetProducto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: $(this).val(),
            },
            success: function (producto) {
                $("#imgProducto").attr("src", "data:image/png;base64," + producto.image);
                /*Temporalmente se permitirá trabajar en ambos modos hasta que se compruebe que no se presentan errores*/
                var options = "<option value='0' selected>" + producto.unidad + "</option>";
                if (TRABAJAR_CON_UNIDADES) {
                    for (var i = 0; i < producto.productoPresentacionList.length; i++) {
                        var reg = producto.productoPresentacionList[i];
                        options = options + "<option value='" + reg.IdProductoPresentacion + "'  precioUnitarioAlternativoSinIGV='" + reg.PrecioSinIGV + "' costoAlternativoSinIGV='" + reg.CostoSinIGV + "' >" + reg.Presentacion + "</option>";
                    }
                }
                else {
                        //Se agrega el precio estandar
                        
                        if (producto.unidad_alternativa != "") {
                            //Se agrega el precio alternativo
                            options = options + "<option value='1'>" + producto.unidad_alternativa + "</option>";
                        }
                }
                $("#btnStockUnidadAddProduct").removeAttr("disabled");

                //Limpieza de campos
                $("#costoLista").val(Number(producto.costoLista));
                $("#precioLista").val(Number(producto.precioLista));
                $('#costoSinIGV').val(producto.costoSinIGV);
                $('#precioUnitarioSinIGV').val(producto.precioUnitarioSinIGV);
                $("#unidad").html(options);
                $("#proveedor").val(producto.proveedor);
                $("#familia").val(producto.familia);

                var idCiudad = $("#idCiudad").val();
                $("#btnStockUnidadAddProduct").attr("sku", producto.sku);
                $("#btnStockUnidadAddProduct").attr("idCiudad", idCiudad);
                $("#btnStockUnidadAddProduct").attr("idProductoPresentacion", "0");

                $('#fleteDetalle').val(producto.fleteDetalle);
                $("#porcentajeDescuento").val(Number(producto.porcentajeDescuento).toFixed(4));
                $("#cantidad").val(1);

                agregarDescripcionLargaCotizacion = producto.agregarDescripcionCotizacion;
                if (producto.descontinuado == 1) {
                    $("#spnProductoDescontinuado").show();

                    if (producto.motivoRestriccion != null) {
                        producto.motivoRestriccion = producto.motivoRestriccion.trim();

                        $("#spnProductoDescontinuado .lblAlertaProductoDescontinuado ").removeClass("tooltip-label");
                        if (producto.motivoRestriccion != "") {
                            $("#spnProductoDescontinuado .lblAlertaProductoDescontinuado ").addClass("tooltip-motivo-restriccion");
                            $("#spnProductoDescontinuado .lblAlertaProductoDescontinuado .tooltip-label-text").html(producto.motivoRestriccion);
                        }
                    }
                } else {
                    $("#spnProductoDescontinuado").hide();
                }

                $('#precioUnitarioAlternativoSinIGV').val(producto.precioUnitarioAlternativoSinIGV);
                $('#costoAlternativoSinIGV').val(producto.costoAlternativoSinIGV);

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
                        '<td>' + Number(producto.precioListaList[i].precioNeto).toFixed(cantidadDecimalesPrecioNeto) + '</td>' +
                        '<td>' + Number(producto.precioListaList[i].flete).toFixed(cantidadDecimalesPrecioNeto) + '</td>' +
                        '<td>' + Number(producto.precioListaList[i].precioUnitario).toFixed(cantidadDecimalesPrecioNeto) + '</td>' +
                        '</tr>');
                }

                //Activar Botón para agregar producto a la grilla
                activarBtnAddProduct();
                //Se calcula el subtotal del producto
                calcularSubtotalProducto();

                $('#modalAgregarProducto #observacionProducto').val(producto.observaciones);
            }
        });
    });

    $("#btnAgregarLiteCliente").click(function () {
        
    });

    ///////////////////CAMPO PRESENTACIÓN
    $("#unidad").change(function () {

        //0 es precio estandar 
        //1 es precio alternativo
        var codigoPrecioAlternativo = Number($("#unidad").val());
        //$("#esPrecioAlternativo").val(codigoPrecioAlternativo);

        var precioLista = 0;
        var costoLista = 0;

        $("#btnStockUnidadAddProduct").attr("idProductoPresentacion", codigoPrecioAlternativo);

        if (codigoPrecioAlternativo == 0) {
            precioLista = Number($("#precioUnitarioSinIGV").val());
            costoLista = Number($("#costoSinIGV").val());
        }
        else {
            if (TRABAJAR_CON_UNIDADES) {
                precioLista = $("#unidad option:selected").attr("precioUnitarioAlternativoSinIGV");

                costoLista = $("#unidad option:selected").attr("costoAlternativoSinIGV");
            }
            else {
                precioLista = Number($("#precioUnitarioAlternativoSinIGV").val());
                costoLista = Number($("#costoAlternativoSinIGV").val());
            }
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
            $("#fleteDetalle").val(fleteDetalle.toFixed(cantidadDecimalesPrecioNeto));
        }
    });


    /////////////////////////CAMPOS PORCENTAJE DESCUENTO y CANTIDAD 
    $("#porcentajeDescuento, #cantidad").change(function () {

        var descuento = Number($("#porcentajeDescuento").val());
        /*      if (descuento > 100) {
                  descuento = 100;
              }*/
        $("#porcentajeDescuento").val(descuento.toFixed(4));
        $("#cantidad").val(Number($("#cantidad").val()).toFixed());
        calcularSubtotalProducto();
    });


    /////////////////////////CAMPO FLETE

    $("#fleteDetalle").change(function () {
        var precioUnitario = Number($('#precio').val()) + Number($('#fleteDetalle').val());
        $('#precioUnitario').val(precioUnitario.toFixed(cantidadDecimalesPrecioNeto));
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

        precio = precioLista * (100 - porcentajeDescuento) * 0.01;
        precio = precio.toFixed(cantidadDecimalesPrecioNeto);


        var precioUnitario = Number(precio) + Number($('#fleteDetalle').val());
        $('#precioUnitario').val(precioUnitario.toFixed(cantidadDecimalesPrecioNeto));


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
        var idCliente = GUID_EMPTY;
        var idGrupoCliente = 0;
        var sessionCotizacion = "cotizacion";
        var actionUrl = "GetPreciosRegistrados";

        if ($("#pagina").val() == "0") {
            idCliente = $("#verIdCliente").val();
            idGrupoCliente = parseInt($("#verIdGrupoCliente").val());
            if (idGrupoCliente > 0) {
                actionUrl = "GetPreciosRegistradosGrupoCliente";
            }
            sessionCotizacion = "cotizacionVer";
        }
        else {
            idCliente = $("#idCliente").val();
            idGrupoCliente = $("#idGrupoCliente").val();
            
            if (idGrupoCliente > 0) {
                actionUrl = "GetPreciosRegistradosGrupoCliente";
            } else {
                if (idCliente.trim() == "") {
                    alert("Debe seleccionar un cliente.");
                    $('#idCliente').trigger('chosen:activate');
                    return false;
                }
            }
        }

        $.ajax({
            url: "/Precio/" + actionUrl,
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: idProducto,
                idCliente: idCliente,
                idGrupoCliente: idGrupoCliente,
                controller: sessionCotizacion
            },
            success: function (producto) {

                $("#verProducto").html(producto.nombre);
                $("#verCodigoProducto").html(producto.sku);

                $("#lnkMostrarLogPrecioProducto").attr("nombreProducto", producto.nombre);
                $("#lnkMostrarLogPrecioProducto").attr("sku", producto.sku);
                $("#lnkMostrarLogPrecioProducto").attr("idProducto", idProducto);
                
                $("#verUnidadProveedor").html(producto.unidadProveedor);
                $("#verUnidadMP").html(producto.unidad);
                $("#verUnidadAlternativa").html(producto.unidadAlternativa);

                $("#verPrecioProveedor").html(producto.precioProveedor);
                $("#verPrecioMP").html(producto.precio);
                $("#verPrecioAlternativa").html(producto.precioAlternativa);

                $("#verPrecioProvinciaProveedor").html(producto.precioProvinciaProveedor);
                $("#verPrecioProvinciaMP").html(producto.precioProvincia);
                $("#verPrecioProvinciaAlternativa").html(producto.precioProvinciaAlternativa);

                if ($("#verCostoMP").length) {
                    $("#verCostoProveedor").html(producto.costoProveedor);
                    $("#verCostoMP").html(producto.costo);
                    $("#verCostoAlternativa").html(producto.costoAlternativa);
                }
                
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
                        '<td>' + Number(precioListaList[i].precioNeto).toFixed(cantidadDecimalesPrecioNeto) + '</td>' +
                        '<td>' + Number(precioListaList[i].flete).toFixed(cantidadDecimalesPrecioNeto) + '</td>' +
                        '<td>' + Number(precioListaList[i].precioUnitario).toFixed(cantidadDecimalesPrecioNeto) + '</td>' +

                        '</tr>');
                }
            }
        });
        $("#modalMostrarPrecios").modal();

    });
    


    $("#considerarDescontinuados").change(function () {
        var considerarDescontinuados = $('#considerarDescontinuados').prop('checked');
        $.ajax({
            url: "/Cotizacion/updateConsiderarDescontinuados",
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

        if (agregarDescripcionLargaCotizacion == 1) {
            var considerarCantidades = $('#considerarCantidades').val();

            if (considerarCantidades == 1) {
                $.alert({
                    title: "Observaciones obligatorias",
                    type: 'orange',
                    content: 'Debe cambiar la visualización de "Solo cantidades" a "Cantidades y Observaciones" para agregar este producto a la cotización.',
                    buttons: {
                        OK: function () { }
                    }
                });

                return false;
            }
        }

        var cantidad = parseInt($("#cantidad").val());
        var porcentajeDescuento = parseFloat($("#porcentajeDescuento").val());
        var precio = $("#precio").val();
        var precioLista = $("#precioLista").val();
        var costoLista = $("#costoLista").val();
        var idProductoPresentacion = Number($("#unidad").val());
        var esPrecioAlternativo = 0;
        if (idProductoPresentacion > 0)
            esPrecioAlternativo = 1;       
        
        var subtotal = $("#subtotal").val();
        var incluidoIGV = $("input[name=igv]:checked").val();
        var proveedor = $("#proveedor").val();
        var flete = Number($("#fleteDetalle").val());
        var observacion = $("#observacionProducto").val();
        var costo = $("#costoLista").val();
        var idCiudad = $("#idCiudad").val();

        $.ajax({
            url: "/Cotizacion/AddProducto",
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
                var considerarCantidades = $("#considerarCantidades").val();
                //     var esRecotizacion = "";
                /*       if ($("#esRecotizacion").val() == "1") {
                           esRecotizacion = '<td class="' + detalle.idProducto + ' detprecioNetoAnterior" style="text-align:right; color: #B9371B">0.00</td>' +
                               '<td class="' + detalle.idProducto + ' detvarprecioNetoAnterior" style="text-align:right; color: #B9371B">0.0 %</td>' +
                               '<td class="' + detalle.idProducto + ' detvarPrecioLista" style="text-align:right; color: #B9371B">0.0 %</td>' +
                               '<td class="' + detalle.idProducto + ' detvarCosto" style="text-align:right; color: #B9371B">0.0 %</td>' +
                               '<td class="' + detalle.idProducto + ' detcostoAnterior" style="text-align:right; color: #B9371B">0.0</td>';
                       }
                       else {*/
                var esRecotizacion = '<td class="' + detalle.idProducto + ' detprecioNetoAnterior" style="text-align:right; color: #B9371B">' + detalle.precioNetoAnt + '</td>' +
                    '<td class="' + detalle.idProducto + ' detvarprecioNetoAnterior" style="text-align:right; color: #B9371B">' + detalle.varprecioNetoAnterior + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detvarPrecioLista" style="text-align:right; color: #B9371B">' + detalle.variacionPrecioListaAnterior + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detvarCosto" style="text-align:right; color: #B9371B">' + detalle.variacionCosto + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detcostoAnterior" style="text-align:right; color: #B9371B">' + detalle.costoListaAnterior + '</td>';
                //    }

                var observacionesEnDescripcion = "";
                if (considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES) {
                    observacionesEnDescripcion = "<br /><span class='" + detalle.idProducto + " detproductoObservacion'  style='color: darkred'>" + detalle.observacion + "</span>";
                }

                $('#tableDetalleCotizacion tbody tr.footable-empty').remove();

                var descontinuadoLabel = "";
                if (detalle.descontinuado == 1) {
                    descontinuadoLabel = "<br/>" + $("#spnProductoDescontinuado").html(); 

                    if (detalle.motivoRestriccion != null) {
                        detalle.motivoRestriccion = detalle.motivoRestriccion.trim();
                        descontinuadoLabel = descontinuadoLabel.replace("_DATA_TIPSO_", detalle.motivoRestriccion);

                        if (detalle.motivoRestriccion != "") {
                            descontinuadoLabel = descontinuadoLabel.replace("_CLASS_TOOLTIP_", "tooltip-motivo-restriccion");
                        }
                    }
                }

                $("#tableDetalleCotizacion tbody").append('<tr data-expanded="false">' +
                    '<td>' + detalle.idProducto + '</td>' +
                    '<td>' + esPrecioAlternativo + '</td>' +
                    '<td>' + proveedor + '</td>' +
                    '<td>' + detalle.codigoProducto + descontinuadoLabel + '</td>' +
                    '<td>' + detalle.nombreProducto + observacionesEnDescripcion +'</td>' +
                    '<td>' + detalle.unidad + '</td>' +
                    '<td class="column-img"><img class="table-product-img" src="' + $("#imgProducto").attr("src") + '"></td>' +
                    '<td class="' + detalle.idProducto + ' detprecioLista" style="text-align:right">' + precioLista + '</td>' +
                    '<td class="' + detalle.idProducto + ' detporcentajedescuento" style="text-align:right">' + porcentajeDescuento.toFixed(4) + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detporcentajedescuentoMostrar" style="width:75px; text-align:right;">' + porcentajeDescuento.toFixed(1) + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detprecio" style="text-align:right">' + detalle.precioUnitario + '</td>' +
                    '<td class="' + detalle.idProducto + ' detcostoLista">' + costoLista + '</td>' +
                    '<td class="' + detalle.idProducto + ' detmargen" style="width:70px; text-align:right; ">' + detalle.margen + ' %</td>' +

                    '<td class="' + detalle.idProducto + ' detflete" style="text-align:right">' + flete.toFixed(2) + '</td>' +
                    '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right">' + detalle.precioUnitario + '</td>' +
                    
                    '<td class="' + detalle.idProducto + ' detcantidad" style="text-align:right">' + cantidad + '</td>' +
                    '<td class="' + detalle.idProducto + ' detsubtotal" style="text-align:right">' + detalle.subTotalItem + '</td>' +
                    '<td class="' + detalle.idProducto + ' detobservacion" style="text-align:left">' + observacion + '</td>' +
                    '<td class="' + detalle.idProducto + ' detbtnMostrarPrecios">' +
                    '<button  type="button" class="' + detalle.idProducto + ' btnMostrarPrecios btn btn-primary bouton-image botonPrecios"></button>' +
                    '<br/><button type="button" title="Consultar Stock" class="verModalStockProducto btn" sku="' + detalle.codigoProducto + '" idProductoPresentacion="' + 
                        detalle.idProductoPresentacion + '" idCiudad="' + idCiudad + '" style="margin-top: 7px;">' +
                    '<img src="/images/icon_stock.png" height="25" />' +
                    '</button>' +
                    '</td > '+

                    esRecotizacion +

                    '<td class="' + detalle.idProducto + ' detordenamiento"></td>' +
                    '</tr > ');

                $('#tableDetalleCotizacion thead tr th.footable-editing').remove();
                $('#tableDetalleCotizacion tbody tr td.footable-editing').remove();

                $("#spnProductoDescontinuado").hide();

                $('#montoIGV').html(detalle.igv);
                $('#montoSubTotal').html(detalle.subTotal);
                ///var flete = Number($("#flete").val());
                $('#montoTotal').html(detalle.total);
                $("#total").val(detalle.total);
                var total = Number($("#total").val())
                $('#montoFlete').html((total * flete / 100).toFixed(cantidadDecimalesPrecioNeto));
                $('#montoTotalMasFlete').html((total + (total * flete / 100)).toFixed(cantidadDecimalesPrecioNeto));

                cargarTablaDetalle()
                // $('#tablefoottable').footable();
                $('#btnCancelAddProduct').click();




            }, error: function (detalle) {

                $("#resultadoAgregarProducto").html("Producto ya se encuentra en el detalle de la cotización.");

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
            nuevoPrecioInicial = Number(Number($("#precioUnitarioSinIGV").val()).toFixed(cantidadDecimalesPrecioNeto));
            
            //Si NO es el precio estandar (si es el precio alternativo)
            if (esPrecioAlternativo >= 1) {
                var nuevoPrecioInicial = Number(Number($("#unidad option:selected").attr("precioUnitarioAlternativoSinIGV")).toFixed(cantidadDecimalesPrecioNeto));
                //costoLista = $("#unidad option:selected").attr("costoAlternativoSinIGV");
                //var nuevoPrecioInicial = Number(Number($("#precioUnitarioAlternativoSinIGV").val()).toFixed(cantidadDecimalesPrecioNeto));
            }
        }
        //En caso el calculo se realice al momento de editar un producto en la grilla
        else {
            //El precio inicial se obtiene del precio lista
            var idproducto = $('#idProducto').val();
            var nuevoPrecioInicial = $("." + idproducto + ".detprecioLista").html();
        }

        var nuevoDescuento = 100 - (nuevoPrecioModificado * 100 / nuevoPrecioInicial);
        $('#nuevoPrecio').val(nuevoPrecioModificado.toFixed(cantidadDecimalesPrecioNeto));
        $('#nuevoDescuento').val(nuevoDescuento.toFixed(4));
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
            $("." + idproducto + ".detprecio").text(precio.toFixed(cantidadDecimalesPrecioNeto));
            //Se asigna el descuento en el campo descuento
            $("." + idproducto + ".detinporcentajedescuento").val($("#nuevoDescuento").val());

            calcularSubtotalGrilla(idproducto);

        }

        $('#btnCancelCalculadora').click();

    });








    ////////////GENERAR PLANTILLA DE COTIZACIÓN


    function validarSeleccionClienteOGrupo() {
        /*Se identifica si la cotización es a un cliente o a un grupo*/
        if ($("#pagina").val() == PAGINA_MANTENIMIENTO_COTIZACION) {
            var idCiudad = $("#idCiudad").val();
            if (idCiudad == "" || $("#idCiudad").val() == null) {
                $.alert({
                    title: "Seleccionar Sede",
                    type: 'orange',
                    content: 'Debe seleccionar la sede MP previamente.',
                    buttons: {
                        OK: function () { }
                    }
                });
                $("#idCiudad").focus();
                $("#btnCancelarObtenerProductos").click();
                return false;
            }

            var codigoMoneda = $("#codigoMoneda").val();
            if (codigoMoneda == "" || $("#codigoMoneda").val() == null) {
                $.alert({
                    title: "Seleccionar Moneda",
                    type: 'orange',
                    content: 'Debe seleccionar la moneda a utilizar en la cotización.',
                    buttons: {
                        OK: function () { }
                    }
                });
                $("#codigoMoneda").focus();
                $("#btnCancelarObtenerProductos").click();
                return false;
            }

            var idCliente = $("#idCliente").val();
            if (idCliente.trim() == "") {
                $.alert({
                    title: "Seleccionar Cliente",
                    type: 'orange',
                    content: 'Debe seleccionar un Cliente.',
                    buttons: {
                        OK: function () { }
                    }
                });
                $('#idCliente').trigger('chosen:activate');
                $("#btnCancelarObtenerProductos").click();
                return false;
            }

        }
        else {
            var idGrupoCliente = $("#idGrupoCliente").val();
            if (idGrupoCliente.trim() == "") {
                $.alert({
                    title: "Seleccionar Grupo",
                    type: 'orange',
                    content: 'Debe seleccionar un Grupo.',
                    buttons: {
                        OK: function () { }
                    }
                });
                $('#idGrupoCliente').trigger('chosen:activate');
                $("#btnCancelarObtenerProductos").click();
                return false;
            }

            var codigoMoneda = $("#codigoMoneda").val();
            if (codigoMoneda == "" || $("#codigoMoneda").val() == null) {
                $.alert({
                    title: "Seleccionar Moneda",
                    type: 'orange',
                    content: 'Debe seleccionar la moneda a utilizar en la cotización.',
                    buttons: {
                        OK: function () { }
                    }
                });
                $("#codigoMoneda").focus();
                $("#btnCancelarObtenerProductos").click();
                return false;
            }
        }
        return true;
    }




    $("#btnAgregarProductosDesdePreciosRegistrados").click(function () {

        if (!validarSeleccionClienteOGrupo()) {
            return false;
        }

        if (verificarSiExisteDetalle()) {
            alert("No deben existir productos agregados a la cotización.");
            return false;
        }

        indicarFamiliaTodas();
        indicarProveedorTodos();
    });

    $("#btnAgregarCanasta").click(function () {

        if (!validarSeleccionClienteOGrupo()) {
            return false;
        }

        $.confirm({
            title: 'IMPORTAR PRODUCTOS COTIZADOS',
            content: 'Seleccione una de las opciones para importar los productos cotizados.',
            type: 'orange',
            buttons: {
                aplica: {
                    text: 'PRECIOS VIGENTES',
                    btnClass: 'btn-success',
                    action: function () {
                        window.location = '/Cotizacion/CargarProductosCanasta?tipo=1';
                    }
                },
                noAplica: {
                    text: 'CANASTA HABITUAL',
                    btnClass: 'btn-warning',
                    action: function () {
                        window.location = '/Cotizacion/CargarProductosCanasta?tipo=2';
                    }
                },
                cancelar: {
                    text: 'CANCELAR',
                    btnClass: '',
                    action: function () {
                        
                    }
                }
            }
        });

    });


    $("#btnObtenerProductosParaGrupo").click(function () {
        var idGrupoCliente = $("#idGrupoCliente").val();
        var fecha = $("#fechaPrecios").val();
        var familia = $("#familiaBusquedaPrecios").val();
        var proveedor = $("#proveedorBusquedaPrecios").val();

        $.ajax({
            url: "/Cotizacion/obtenerProductosAPartirdePreciosRegistradosParaGrupo",
            data: {
                idGrupoCliente: idGrupoCliente,
                fecha: fecha,
                familia: familia,
                proveedor: proveedor
            },
            type: 'POST',
            error: function () {
                alert("Ocurrió un error al armar el detalle del pedido a partir de los precios registrados.");
            },
            success: function () {

           /*     $.alert({
                    title: '¡Atención!',
                    type: 'orange',
                    content: "Los productos importados no consideran los precios registrados para un grupo.",
                    buttons: {
                        OK: function () {*/
                            window.location = '/Cotizacion/CotizarGrupo';

          /*              }
                    }
                });
                */
               
            }
        });
    });


    $("#btnObtenerProductos").click(function () {
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val();
        var fecha = $("#fechaPrecios").val();
        var familia = $("#familiaBusquedaPrecios").val();
        var proveedor = $("#proveedorBusquedaPrecios").val();
        $('body').loadingModal({
            text: 'Obteniedo Productos y Precios...'
        });
      

        $.ajax({
            url: "/Cotizacion/obtenerProductosAPartirdePreciosRegistrados",
            data: {
                idCliente: idCliente,
                idCiudad: idCiudad,
                fecha: fecha,
                familia: familia, 
                proveedor: proveedor
            },
            type: 'POST',
            error: function () {
                $('body').loadingModal('hide')
                alert("Ocurrió un error al armar el detalle del pedido a partir de los precios registrados.");
                //window.location = '/Cotizacion/Cotizador';
            },
            success: function () {
                $('body').loadingModal('hide')
                /*     $.alert({
                title: '¡Atención!',
                type: 'orange',
                content: "Los productos importados no consideran los precios registrados para un grupo.",
                buttons: {
                    OK: function () {*/
                window.location = '/Cotizacion/Cotizar';

          /*              }
                    }
                });
                */
             
            }
        });

    });

    





    ////////CREAR/EDITAR COTIZACIÓN


    function validarIngresoDatosObligatoriosCotizacion() {

        if (!validarSeleccionClienteOGrupo()) {
            return false;
        }

        if ($("#contacto").val().trim() == "") {
            alert("Debe ingresar un contacto.");
            $("#contacto").focus();
            return false;
        }

        var fecha = $("#fecha").val();
        if ($("#fecha").val().trim() == "")
        {
            alert("Debe ingresar la fecha de la cotización.");
            $("#fecha").focus();
            return false;
        }
        /*Si la validez de oferta se expresa en días*/
        if ($("#mostrarValidezOfertaEnDias").val() == 0) {
            //No puede ser menor a uno
            if ($("#validezOfertaEnDias").val() < 1) {
                alert("La cantidad de días de validez de oferta debe ser mayor o igual a uno.");
                $("#validezOfertaEnDias").focus();
                return false;
            }
        }
        else {
            //la fecha de validez de oferta no debe estar vacía
            var fechaLimiteValidezOferta = $("#fechaLimiteValidezOferta").val();
            if (fechaLimiteValidezOferta.trim() == "") {
                alert("Debe ingresar la fecha de Validez Oferta.");
                $("#fechaLimiteValidezOferta").focus();
                return false;
            } //Si no está vacía no puede ser menor a la fecha
            else if (convertirFechaNumero(fechaLimiteValidezOferta) < convertirFechaNumero(fecha)) {
                alert("EL fin de Validez de Oferta debe ser mayor o igual a la fecha de la cotización.");
                $("#fechaLimiteValidezOferta").focus();
                return false;
            }   
        }

        


        if ($('#tipoCotizacion').val() == 1) {

            if ($("#mostrarValidezOfertaEnDias").val() == 0) {
                if ($("#validezOfertaEnDias").val() > parseInt(DIAS_MAX_VALIDEZ_OFERTA_COTIZACION_PUNTUAL)) {
                    alert("La cantidad de días de validez de oferta debe como máximo " + DIAS_MAX_VALIDEZ_OFERTA_COTIZACION_PUNTUAL + ".");
                    $("#validezOfertaEnDias").focus();
                    return false;
                }
            }
            else {
                //la fecha de validez de oferta no debe estar vacía
                var fechaLimiteValidezOferta = $("#fechaLimiteValidezOferta").val();
                var fechaMaxValidezOferta = new Date();
                fechaMaxValidezOferta.setDate(fechaMaxValidezOferta.getDate() + parseInt(DIAS_MAX_VALIDEZ_OFERTA_COTIZACION_PUNTUAL));

                var fechaMaxValidezOfertaTexto = "";
                let day = fechaMaxValidezOferta.getDate()
                let month = fechaMaxValidezOferta.getMonth() + 1
                let year = fechaMaxValidezOferta.getFullYear()

                if (month < 10) {
                    fechaMaxValidezOfertaTexto = `${day}/0${month}/${year}`;
                } else {
                    fechaMaxValidezOfertaTexto = `${day}/${month}/${year}`;
                }

                if (convertirFechaNumero(fechaLimiteValidezOferta) > convertirFechaNumero(fechaMaxValidezOfertaTexto)) {
                    alert("La Fecha de fin de Validez de Oferta debe ser como maximo el " + fechaMaxValidezOfertaTexto + ".");
                    $("#fechaLimiteValidezOferta").focus();
                    return false;
                }
            }
        } else {
            var fechaInicioVigenciaPrecios = $("#fechaInicioVigenciaPrecios").val();
            if (fechaInicioVigenciaPrecios.trim() != "") {

                if (convertirFechaNumero(fechaInicioVigenciaPrecios) < convertirFechaNumero(fecha)) {
                    //Si no está vacía no puede ser menor a la fecha
                    var anioInicioVigencia = convertirFechaNumero(fechaInicioVigenciaPrecios).toString().substr(0, 4);
                    //var anioCotizacion = convertirFechaNumero(fecha).toString().substr(0, 4);
                    var fechatmp = new Date();
                    var anioCotizacion = fechatmp.getFullYear();

                    if (Number(anioInicioVigencia) != anioCotizacion) {
                        alert("El año del inicio de vigencia de precios no puede ser menor al año actual.");
                        $("#fechaInicioVigenciaPrecios").focus();
                        return false;
                    }
                }

            }

            var fechaFinVigenciaPrecios = $("#fechaFinVigenciaPrecios").val();
            if (fechaFinVigenciaPrecios.trim() != "") {


                //Si la fecha de inicio de vigencia no es vacío se compara con la fecha de inicio de vigencia
                if (fechaInicioVigenciaPrecios.trim() != "") {

                    //Si no está vacía no puede ser menor a la fecha de inicio de vigencia
                    if (convertirFechaNumero(fechaFinVigenciaPrecios) < convertirFechaNumero(fechaInicioVigenciaPrecios)) {
                        alert("El fin de vigencia de precios debe ser mayor o igual al inicio de vigencia de precios.");
                        $("#fechaFinVigenciaPrecios").focus();
                        return false;
                    }

                    //Si es una cotización transitoria la fecha de fin de vigencia no puede ser mayor a la fecha de inicio de vigencia por más de 10 días

                }
                else {  //Si no está vacía no puede ser menor a la fecha de inicio de vigencia
                    if (convertirFechaNumero(fechaFinVigenciaPrecios) < convertirFechaNumero(fecha)) {
                        alert("El fin de vigencia de precios debe ser mayor o igual a la fecha de la cotización.");
                        $("#fechaFinVigenciaPrecios").focus();
                        return false;
                    }

                }
            }

        }

        var contador = 0;
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {
            contador++;
        });

        if (contador == 0) {
            alert("Debe ingresar el detalle de la cotización.");
            return false;
        }

        desactivarBotonesFinalizarCreacion();

        return true;
    }


    function activarBotonesFinalizarCreacion()
    {
        $("#btnFinalizarEdicionCotizacion").removeAttr('disabled');
        $("btnFinalizarCreacionCotizacion").removeAttr('disabled');
        $("btnContinuarCreandoLuego").removeAttr('disabled');
        $("btnContinuarEditandoLuego").removeAttr('disabled');
        $("btnCancelCotizacion").removeAttr('disabled');
    }

    function desactivarBotonesFinalizarCreacion()
    {
        $("#btnFinalizarEdicionCotizacion").attr('disabled', 'disabled');
        $("btnFinalizarCreacionCotizacion").attr('disabled', 'disabled');
        $("btnContinuarCreandoLuego").attr('disabled', 'disabled');
        $("btnContinuarEditandoLuego").attr('disabled', 'disabled');
        $("btnCancelCotizacion").attr('disabled', 'disabled');
    }


    function crearCotizacion(continuarLuego) {

        if (!validarIngresoDatosObligatoriosCotizacion())
            return false;

        if ($("#pagina").val() == PAGINA_MANTENIMIENTO_COTIZACION) {
            
            var sedePrincipal = parseInt($('#clienteSedePrincipal').val());

            if (continuarLuego == 0 && sedePrincipal == 1 && $('#tipoCotizacion').val() == 0) {
                $.confirm({
                    title: 'Cliente Multiregional Identificado',
                    content: '<div><div class="col-sm-12"><b>¿Desea que al momento de aceptar esta cotización los precios se registren en las siguientes sedes?</b></div><div class="col-sm-12">' + listaTextoSedesCliente + '</div></div>',
                    type: 'orange',
                    buttons: {
                        aplica: {
                            text: 'SI',
                            btnClass: 'btn-success',
                            action: function () {
                                $('#aplicaSedes').val("1");
                                callCreate(continuarLuego);
                            }
                        },
                        noAplica: {
                            text: 'NO, solo registrar precios para ' + $("#idCiudad option:selected").text(),
                            btnClass: 'btn-danger',
                            action: function () {
                                callCreate(continuarLuego);
                            }
                        },
                        cancelar: {
                            text: 'Cancelar',
                            btnClass: '',
                            action: function () {
                                activarBotonesFinalizarCreacion();
                            }
                        }
                    }
                });
            } else {
                callCreate(continuarLuego);
            }
        }
        else {
            //Espacio para mostrar mensaje de Cliente a los cuales se le aplicará la cotización Grupal
            callCreate(continuarLuego);
        }
    }

    
    function callCreate(continuarLuego) {
        $('body').loadingModal({
            text: 'Creando Cotización...'
        });

        var aplicaSedes = $('#aplicaSedes').val();
        $.ajax({
            url: "/Cotizacion/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                continuarLuego: continuarLuego,
                aplicaSedes: aplicaSedes
            },
            error: function (detalle) {
                $('body').loadingModal('hide')
                activarBotonesFinalizarCreacion();
                //alert("Se generó un error al intentar finalizar la creación de la cotización. Si estuvo actualizando, vuelva a buscar la cotización, es posible que este siendo modificada por otro usuario.");
                mostrarMensajeErrorProceso(detalle.responseText);            

            },
            success: function (resultado) {
                $('body').loadingModal('hide')
                activarBotonesFinalizarCreacion();
                $("#numero").val(resultado.codigo);

                if (resultado.estado == ESTADO_APROBADA) {


                    $.alert({
                        title: TITLE_EXITO,
                        type: 'green',
                        content: "La cotización número " + resultado.codigo + " fue creada correctamente; se APROBÓ automáticamente, no requiere aprobación, ingrese un comentario si lo cree conveniente.",
                        buttons: {
                            OK: function () {
                                $("#comentarioAprobacion").val(resultado.observacion);
                                $("#modalComentarioAprobacion").modal('show');

                            }
                        }
                    });
                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    $("#solicitudIngresoComentario").html("La cotización número " + resultado.codigo + " fue creada correctamente, sin embargo requiere APROBACIÓN, debe ingresar un comentario de lo contrario se mantendrá en estado En Edición.")
                    $("#comentarioPendienteAprobacion").val(resultado.observacion);
                    $("#modalComentarioPendienteAprobacion").modal({
                        show: true,
                        keyboard: false,
                        backdrop: 'static'
                    });
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    $("#numero").val(resultado.codigo);
                    ConfirmDialog("La cotización número " + resultado.codigo + " fue guardada correctamente. ¿Desea continuar editando ahora?", null, '/Cotizacion/CancelarCreacionCotizacion');
                }
                else {
                    alert("La cotización ha tenido problemas para ser procesada; Contacte con el Administrador");
                    // window.location = '/Cotizacion/Index';
                }

            }
        });
    }

    function editarCotizacion(continuarLuego) {
        if (!validarIngresoDatosObligatoriosCotizacion())
            return false;

        var sedePrincipal = parseInt($('#clienteSedePrincipal').val());

        if (continuarLuego == 0 && sedePrincipal == 1 && $('#tipoCotizacion').val() == 0) {
            $.confirm({
                title: 'Cliente Multiregional Identificado',
                content: '<div><div class="col-sm-12"><b>¿Desea que al momento de aceptar esta cotización los precios se registren en las siguientes sedes?</b></div><div class="col-sm-12">' + listaTextoSedesCliente + '</div></div>',
                type: 'orange',
                buttons: {
                    aplica: {
                        text: 'SI',
                        btnClass: 'btn-success',
                        action: function () {
                            $('#aplicaSedes').val("1");
                            callUpdate(continuarLuego);
                        }
                    },
                    noAplica: {
                        text: 'NO, solo registrar precios para ' + $("#idCiudad option:selected").text(),
                        btnClass: 'btn-danger',
                        action: function () {
                            callUpdate(continuarLuego);
                        }
                    },
                    cancelar: {
                        text: 'Cancelar',
                        btnClass: '',
                        action: function () {
                            activarBotonesFinalizarCreacion();
                        }
                    }
                }
            });
        } else {
            callUpdate(continuarLuego);
        }  
    }

    function callUpdate(continuarLuego) {
        $('body').loadingModal({
            text: 'Editando Cotización...'
        });

        var aplicaSedes = $('#aplicaSedes').val();
        $.ajax({
            url: "/Cotizacion/Update",
            type: 'POST',
            dataType: 'JSON',
            data: {
                continuarLuego: continuarLuego,
                aplicaSedes: aplicaSedes
            },
            error: function (detalle) {
                $('body').loadingModal('hide')
                activarBotonesFinalizarCreacion();
                //alert("Se generó un error al intentar finalizar la creación de la cotización. Si estuvo actualizando, vuelva a buscar la cotización, es posible que este siendo modificada por otro usuario.");
                mostrarMensajeErrorProceso(detalle.responseText);

            },
            success: function (resultado) {
                $('body').loadingModal('hide')
                activarBotonesFinalizarCreacion();
                $("#numero").val(resultado.codigo);

                if (resultado.estado == ESTADO_APROBADA) {


                    $.alert({
                        title: TITLE_EXITO,
                        type: 'green',
                        content: "La cotización número " + resultado.codigo + " fue editada correctamente; se APROBÓ automáticamente, no requiere aprobación, ingrese un comentario si lo cree conveniente.",
                        buttons: {
                            OK: function () {
                                $("#comentarioAprobacion").val(resultado.observacion);
                                $("#modalComentarioAprobacion").modal('show');

                            }
                        }
                    });



                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    $("#solicitudIngresoComentario").html("La cotización número " + resultado.codigo + " fue editada correctamente, sin embargo requiere APROBACIÓN, debe ingresar un comentario de lo contrario se mantendrá en estado En Edición.")
                    $("#comentarioPendienteAprobacion").val(resultado.observacion);
                    $("#modalComentarioPendienteAprobacion").modal({
                        show: true,
                        keyboard: false,
                        backdrop: 'static'
                    });
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    $("#numero").val(resultado.codigo);

                    ConfirmDialog("La cotización número " + resultado.codigo + " fue guardada correctamente. ¿Desea continuar editando ahora?", null, '/Cotizacion/CancelarCreacionCotizacion');



                    //alert("La cotización número " + resultado.codigo + " fue guardada correctamente para seguir editandola posteriormente.");
                    //window.location = '/Cotizacion/Index';
                }
                else {
                    alert("La cotización ha tenido problemas para ser procesada; Contacte con el Administrador");
                    // window.location = '/Cotizacion/Index';
                }
            }
        });
    }

    $("#btnFinalizarCreacionCotizacion").click(function () {
        crearCotizacion(0);
    });

    $("#btnFinalizarEdicionCotizacion").click(function () {
        editarCotizacion(0);
    });

    $("#btnContinuarEditandoLuego").click(function () {
        if ($("#numero").val() == "" || $("#numero").val() == null) {
            crearCotizacion(1);
        }
        else
        {
            editarCotizacion(1);
        }        
    });












    /*
    $("#btnCopiar").click(function () {
        // Get the text field 
        var copyText = document.getElementById("myInput");

        //Select the text field 
        copyText.select();

        // Copy the text inside the text field 
        document.execCommand("Copy");

        // alert the copied text
    //    alert("Copied the text: " + copyText.value);
    }); 
    */




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

    $(document).on('click', "button.btnVerCotizacion", function () {

        $('body').loadingModal({
            text: 'Abriendo Cotización...'
        });
        $('body').loadingModal('show');

        activarBotonesVer();
        var codigo = event.target.getAttribute("class").split(" ")[0];


        $.ajax({
            url: "/Cotizacion/Show",
            data: {
                numero: codigo
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                alert("Ocurrió un problema al obtener el detalle de la cotización N° " + codigo + ".");
            },
            success: function (resultado) {
                //var cotizacion = $.parseJSON(respuesta);
                $('body').loadingModal('hide');
                var cotizacion = resultado.cotizacion;
                var usuario = resultado.usuario;
                var simboloMoneda = cotizacion.monedaSimbolo;

                $(".mostrarSimboloMoneda").html(cotizacion.monedaSimbolo);
                $(".mostrarDescripcionMoneda").html(cotizacion.monedaNombre + " (" + cotizacion.monedaSimbolo + ")");

                $("#verCondicionesPago").html(cotizacion.textoCondicionesPago);
                $("#verIdCotizacion").val(cotizacion.idCotizacion);
                $("#verIdCliente").val(cotizacion.cliente_idCliente);

                if (cotizacion.cliente_esClienteLite) {
                    $("#verClienteLite").val("1");
                } else {
                    $("#verClienteLite").val("0");
                }

                var numeroCot = '<span id="codigoCotizacionShow">' + cotizacion.codigo + '</span>';
                if (cotizacion.codigoAntecedente) {
                    numeroCot = numeroCot + " (Recotizado desde " + cotizacion.codigoAntecedente  + ")";
                }

                $("#verIdGrupoCliente").val(cotizacion.grupo_idGrupoCliente);
                $("#verNumero").html(numeroCot);
                $("#verCiudad").html(cotizacion.ciudad_nombre);
                $("#verContacto").html(cotizacion.contacto);

                if (cotizacion.cliente_idCliente == GUID_EMPTY) {
                    $("#labelCliente").hide();
                    $("#labelGrupo").show();
                    $("#spnTitleGrupo").show();
                    $("#verClienteGrupo").html(cotizacion.grupo_codigoNombre);
                }
                else {
                    $("#labelCliente").show();
                    $("#labelGrupo").hide();
                    $("#spnTitleGrupo").hide();
                    $("#verClienteGrupo").html(cotizacion.cliente_codigoRazonSocial);
                }     

                if (cotizacion.tipoCotizacion == 0) {
                    $("#esNormal").show();
                    $("#esTransitoria").hide();
                    $("#esTrivial").hide();
                }
                else if (cotizacion.tipoCotizacion == 1) {
                    $("#esNormal").hide();
                    $("#esTransitoria").show();
                    $("#esTrivial").hide();
                }
                else if (cotizacion.tipoCotizacion == 2) {
                    $("#esNormal").hide();
                    $("#esTransitoria").hide();
                    $("#esTrivial").show();
                }

                $("#verFechaCreacion").html(invertirFormatoFecha(cotizacion.fecha.substr(0, 10)));
                $("#verValidezOferta").html(invertirFormatoFecha(cotizacion.fechaLimiteValidezOferta.substr(0, 10)));

                if (cotizacion.fechaInicioVigenciaPrecios == null)
                    $("#verFechaInicioVigenciaPrecios").html("No Definida");
                else
                    $("#verFechaInicioVigenciaPrecios").html(invertirFormatoFecha(cotizacion.fechaInicioVigenciaPrecios.substr(0, 10)));

                if (cotizacion.fechaFinVigenciaPrecios == null)
                    $("#verFechaFinVigenciaPrecios").html("No Definida");
                else
                    $("#verFechaFinVigenciaPrecios").html(invertirFormatoFecha(cotizacion.fechaFinVigenciaPrecios.substr(0, 10)));

                $("#verEstado").html(cotizacion.seguimientoCotizacion_estadoString);
                $("#verModificadoPor").html(cotizacion.seguimientoCotizacion_usuario_nombre);
                $("#verObservacionEstado").html(cotizacion.seguimientoCotizacion_observacion);

                if (cotizacion.considerarCantidades != CANT_SOLO_OBSERVACIONES) {
                    $("#montosTotalesDiv").show();
                }
                else {
                    $("#montosTotalesDiv").hide();
                }

                $("#verObservaciones").html(cotizacion.observaciones);
                if (cotizacion.aplicaSedes === true) {
                    $("#verSedesAplica").html("Esta cotización aplicará también para las sedes: " + cotizacion.cliente_sedeListWebString.replace(new RegExp('<br>', 'g'), ', '));
                }
                $("#verMontoSubTotal").html(cotizacion.montoSubTotal);
                $("#verMontoIGV").html(cotizacion.montoIGV);
                $("#verMontoTotal").html(cotizacion.montoTotal);


                $("#tableDetalleCotizacion > tbody").empty();

                //FooTable.init('#tableDetalleCotizacion');




                var d = '';
                var lista = cotizacion.cotizacionDetalleList;
                var tieneProductoRestringido = false;
                var tieneDescuentoMayorATope = false;

                for (var i = 0; i < cotizacion.cotizacionDetalleList.length; i++) {

                    var observacion = lista[i].observacion == null || lista[i].observacion == 'undefined' ? '' : lista[i].observacion;

                    var precioUnitarioAnterior = lista[i].producto.precioClienteProducto.precioNeto.toFixed(cantidadDecimalesPrecioNeto);
                    //var costo = lista[i].producto.costoLista.toFixed(cantidadDecimales);
                    if (lista[i].esPrecioAlternativo) {
                        precioUnitarioAnterior = lista[i].producto.precioClienteProducto.precioNetoAlternativo.toFixed(cantidadDecimalesPrecioNeto);
                        // costo = lista[i].producto.costoListaAlternativo.toFixed(cantidadDecimales)
                    }

                    var descontinuadoLabel = "";
                    if (lista[i].producto.descontinuado == 1) {
                        if (lista[i].cantidad > 1 && lista[i].cantidad > lista[i].producto.cantidadMaximaPedidoRestringido) {
                            tieneProductoRestringido = true;
                        }
                        
                        descontinuadoLabel = "<br/>" + $("#spnProductoDescontinuado").html();

                        if (lista[i].producto.motivoRestriccion != null) {
                            lista[i].producto.motivoRestriccion = lista[i].producto.motivoRestriccion.trim();
                            descontinuadoLabel = descontinuadoLabel.replace("_DATA_TIPSO_", lista[i].producto.motivoRestriccion);

                            if (lista[i].producto.motivoRestriccion != "") {
                                descontinuadoLabel = descontinuadoLabel.replace("_CLASS_TOOLTIP_", "tooltip-motivo-restriccion");
                            }
                        }
                    }

                    if (lista[i].producto.topeDescuento > 0 && lista[i].producto.topeDescuento < lista[i].porcentajeDescuento) {
                        //TODO: Validar que si la cotizacion es puntual el precio no sea vigente
                        tieneDescuentoMayorATope = true;
                    }

                    var inactivoClass = ""
                    if (lista[i].producto.Estado == 0) {
                        inactivoClass = 'producto-inactivo-row';
                    }

                    d += '<tr class="' + inactivoClass + '">' +
                        '<td>' + lista[i].producto.proveedor + '</td>' +
                        '<td>' + lista[i].producto.sku + descontinuadoLabel + '</td>' +
                        '<td>' + lista[i].producto.descripcion + '</td>' +
                        '<td>' + lista[i].unidad + '</td>' +
                        '<td class="column-img"><img class="table-product-img" src="data:image/png;base64,' + lista[i].producto.image + '"> </td>' +
                        '<td>' + lista[i].precioLista.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].porcentajeDescuentoMostrar.toFixed(cantidadDecimales) + ' %</td>' +
                        '<td>' + lista[i].precioNeto.toFixed(cantidadDecimalesPrecioNeto) + '</td>' +
                        '<td>' + lista[i].costoListaVisible.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].margen.toFixed(cantidadDecimales) + ' %</td>' +
                        '<td>' + lista[i].flete.toFixed(cantidadDecimalesPrecioNeto) + '</td>' +
                        '<td>' + lista[i].precioUnitario.toFixed(cantidadDecimalesPrecioNeto) + '</td>' +
                        '<td>' + lista[i].cantidad + '</td>' +
                        '<td>' + lista[i].subTotal.toFixed(cantidadDecimales) + '</td>' +

                        '<td>' + observacion + '</td>' +
                        '<td class="tdprecioUnitarioAnterior">' + precioUnitarioAnterior + '</td>' +
                        '<td class="tdprecioUnitarioAnterior">' + lista[i].variacionPrecioAnterior + ' %</td>' +
                        '<td class="tdprecioUnitarioAnterior">' + lista[i].variacionPrecioListaAnterior + ' %</td>' +
                        '<td class="tdprecioUnitarioAnterior">' + lista[i].variacionCosto + ' %</td>' +
                        '<td class="' + lista[i].producto.idProducto + ' detbtnMostrarPrecios"> <button  type="button" class="' + lista[i].producto.idProducto + ' btnMostrarPrecios btn btn-primary bouton-image botonPrecios"></button>' +
                        '<br/><button type="button" title="Consultar Stock" class="verModalStockProducto btn" sku="' + lista[i].producto.sku + '" idProductoPresentacion="' +
                        lista[i].idProductoPresentación + '" idCiudad="' + cotizacion.ciudad_idCiudad + '" style="margin-top: 7px;">' +
                        '<img src="/images/icon_stock.png" height="25" />' +
                        '</button>' +
                        '</td>' +
                        '</tr>';

                }
                //  
                // sleep
                $("#tableDetalleCotizacion").append(d);


                /*EDITAR COTIZACIÓN*/
                if (
                    cotizacion.seguimientoCotizacion_estado == ESTADO_PENDIENTE_APROBACION ||
                    cotizacion.seguimientoCotizacion_estado == ESTADO_APROBADA ||
                    cotizacion.seguimientoCotizacion_estado == ESTADO_DENEGADA ||
                    (cotizacion.seguimientoCotizacion_estado == ESTADO_EN_EDICION && usuario.idUsuario == cotizacion.seguimientoCotizacion_usuario_idUsuario)
                ) {
                    if (cotizacion.cliente_idCliente == GUID_EMPTY) {
                        $("#btnEditarCotizacion").hide();
                        $("#btnEditarCotizacionGrupal").show();
                    }
                    else {
                        $("#btnEditarCotizacion").show();
                        $("#btnEditarCotizacionGrupal").hide();
                    }
                    if (cotizacion.seguimientoCotizacion_estado == ESTADO_EN_EDICION) {
                        $("#btnEditarCotizacion").html("Continuar Editando");
                        $("#btnEditarCotizacionGrupal").html("Continuar Editando");
                    }
                    else {
                        $("#btnEditarCotizacion").html("Editar");
                        $("#btnEditarCotizacionGrupal").html("Editar");
                    }
                }
                else {
                    $("#btnEditarCotizacionGrupal").hide();
                    $("#btnEditarCotizacion").hide();
                }
                /*RECOTIZAR*/
                if (
                    cotizacion.seguimientoCotizacion_estado == ESTADO_APROBADA ||
                    cotizacion.seguimientoCotizacion_estado == ESTADO_ACEPTADA ||
                    cotizacion.seguimientoCotizacion_estado == ESTADO_RECHAZADA
                ) {
                    if (cotizacion.cliente_idCliente == GUID_EMPTY) {
                        $("#btnReCotizacionGrupal").show();
                        $("#btnReCotizacion").hide();
                    }
                    else {
                        $("#btnReCotizacion").show();
                        $("#btnReCotizacionGrupal").hide();

                    }
                }
                else {
                    $("#btnReCotizacionGrupal").hide();
                    $("#btnReCotizacion").hide();
                }




                /*APROBAR DENEGAR COTIZACIÓN*/

                if (cotizacion.cliente_idCliente == GUID_EMPTY) {
                    if (
                        (cotizacion.seguimientoCotizacion_estado == ESTADO_PENDIENTE_APROBACION ||
                            cotizacion.seguimientoCotizacion_estado == ESTADO_DENEGADA)
                        &&
                        (usuario.apruebaCotizacionesGrupales &&
                            usuario.maximoPorcentajeDescuentoAprobacion >= cotizacion.maximoPorcentajeDescuentoPermitido)
                        &&
                        (!tieneProductoRestringido || (tieneProductoRestringido && usuario.apruebaCotizacionesVentaRestringida))
                        &&
                        (!tieneDescuentoMayorATope || usuario.maximoPorcentajeDescuentoAprobacion >= 100)
                    ) {
                        $("#btnAprobarCotizacion").show();
                    }
                    else {
                        $("#btnAprobarCotizacion").hide();
                    }

                    if (
                        (cotizacion.seguimientoCotizacion_estado == ESTADO_PENDIENTE_APROBACION)
                        &&
                        (usuario.apruebaCotizacionesGrupales &&
                            usuario.maximoPorcentajeDescuentoAprobacion >= cotizacion.maximoPorcentajeDescuentoPermitido)
                    ) {
                        $("#btnDenegarCotizacion").show();
                    }
                    else {
                        $("#btnDenegarCotizacion").hide();
                    }


                }
                else {
                    if (
                        (cotizacion.seguimientoCotizacion_estado == ESTADO_PENDIENTE_APROBACION ||
                            cotizacion.seguimientoCotizacion_estado == ESTADO_DENEGADA)
                        &&
                        (usuario.apruebaCotizaciones &&
                            usuario.maximoPorcentajeDescuentoAprobacion >= cotizacion.maximoPorcentajeDescuentoPermitido)
                        &&
                        (!tieneProductoRestringido || (tieneProductoRestringido && usuario.apruebaCotizacionesVentaRestringida))
                        &&
                        (!tieneDescuentoMayorATope || usuario.maximoPorcentajeDescuentoAprobacion >= 100)
                    ) {
                        $("#btnAprobarCotizacion").show();
                    }
                    else {
                        $("#btnAprobarCotizacion").hide();
                    }

                    if (
                        (cotizacion.seguimientoCotizacion_estado == ESTADO_PENDIENTE_APROBACION)
                        &&
                        (usuario.apruebaCotizaciones &&
                            usuario.maximoPorcentajeDescuentoAprobacion >= cotizacion.maximoPorcentajeDescuentoPermitido)
                    ) {

                        $("#btnDenegarCotizacion").show();
                    }
                    else {
                        $("#btnDenegarCotizacion").hide();
                    }
                }
                
                /*ACEPTAR COTIZACIÓN*/
                if (
                    cotizacion.seguimientoCotizacion_estado == ESTADO_APROBADA ||
                    cotizacion.seguimientoCotizacion_estado == ESTADO_RECHAZADA
                ) {
                    $("#btnAceptarCotizacion").show(); 
                }
                else {
                    $("#btnAceptarCotizacion").hide();
                }


                /*RECHAZAR COTIZACIÓN*/
                if (
                    (cotizacion.seguimientoCotizacion_estado == ESTADO_APROBADA)
                ) {

                    $("#btnRechazarCotizacion").show();
                }
                else {
                    $("#btnRechazarCotizacion").hide();
                }


                /*PDF*/
                if (
                    (cotizacion.seguimientoCotizacion_estado == ESTADO_APROBADA ||
                        cotizacion.seguimientoCotizacion_estado == ESTADO_ACEPTADA /* ||
                        cotizacion.seguimientoCotizacion_estado == ESTADO_RECHAZADA*/
                    )
                ) {

                    $("#btnPDFCotizacion").show();
                }
                else {
                    $("#btnPDFCotizacion").hide();
                }

                FooTable.init('#tableDetalleCotizacion');


                if (
                    cotizacion.seguimientoCotizacion_estado == ESTADO_ACEPTADA
                    && cotizacion.cliente_idCliente != GUID_EMPTY
                ) {

                    $("#btnGenerarPedido").show();
                    //window.setInterval(ocultarPrecioUnitarioAnterior, 5000);                    
                }
                else {
                    $("#btnGenerarPedido").hide();
                    //window.setInterval(mostrarPrecioUnitarioAnterior, 5000);
                }
                /*Eliminar Cotizacion*/
                if (
                   ( cotizacion.seguimientoCotizacion_estado != ESTADO_ACEPTADA &&
                    cotizacion.seguimientoCotizacion_estado != ESTADO_RECHAZADA &&
                        cotizacion.seguimientoCotizacion_estado != ESTADO_ELIMINADA)
                    || (cotizacion.seguimientoCotizacion_estado == ESTADO_ACEPTADA  &&
                        usuario.eliminaCotizacionesAceptadas == true)
                ) {

                    $("#btnEliminarCotizacion").show();
                }
                else {
                    $("#btnEliminarCotizacion").hide();
                }

                
                $("#modalVerCotizacion").modal('show');
                
            }
        });
    });

    function ocultarPrecioUnitarioAnterior() {
        //$(".tdprecioUnitarioAnterior").attr("data-visible", "false");
        $(".tdprecioUnitarioAnterior").attr("style", "display:none;");
    }

    function mostrarPrecioUnitarioAnterior() {
        //$(".tdprecioUnitarioAnterior").attr("data-visible", "true");
        $(".tdprecioUnitarioAnterior").attr("style", "display: table-cell;");
    }


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
            url: "/Cotizacion/GenerarPDFdesdeIdCotizacion",
            data: {
                codigo: codigo
            },
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al descargar la cotización N° " + codigo+" en formato PDF.");},
            success: function (fileName) {
                //Se descarga el PDF y luego se limpia el formulario
                window.open('/General/DownLoadFile?fileName=' + fileName);
                window.location = '/Cotizacion/CancelarCreacionCotizacion';
            }
        });
}




    function desactivarBotonesVer()
    {
        $("#btnCancelarCotizacion").attr('disabled', 'disabled');
        $("#btnEditarCotizacion").attr('disabled', 'disabled');
        $("#btnEditarCotizacionGrupal").attr('disabled', 'disabled');        
        $("#btnReCotizacion").attr('disabled', 'disabled');
        $("#btnAprobarCotizacion").attr('disabled', 'disabled');
        $("#btnDenegarCotizacion").attr('disabled', 'disabled');
        $("#btnAceptarCotizacion").attr('disabled', 'disabled');
        $("#btnRechazarCotizacion").attr('disabled', 'disabled');
        $("#btnEliminarCotizacion").attr('disabled', 'disabled');
        $("#btnPDFCotizacion").attr('disabled', 'disabled');
        $("#btnGenerarPedido").attr('disabled', 'disabled');

    }

    function activarBotonesVer() {
        $("#btnCancelarCotizacion").removeAttr('disabled');
        $("#btnEditarCotizacion").removeAttr('disabled');
        $("#btnEditarCotizacionGrupal").removeAttr('disabled');
        $("#btnReCotizacion").removeAttr('disabled');
        $("#btnAprobarCotizacion").removeAttr('disabled');
        $("#btnDenegarCotizacion").removeAttr('disabled');
        $("#btnAceptarCotizacion").removeAttr('disabled');
        $("#btnRechazarCotizacion").removeAttr('disabled');
        $("#btnPDFCotizacion").removeAttr('disabled');
        $("#btnEliminarCotizacion").removeAttr('disabled');
        $("#btnGenerarPedido").removeAttr('disabled');
    }


    $("#btnReCotizacion").click(function () {
        desactivarBotonesVer();
        preRecotizar(DIRIGIDO_A_CLIENTE);
    });

    $("#btnReCotizacionGrupal").click(function () {
        desactivarBotonesVer();
        preRecotizar(DIRIGIDO_A_GRUPO);
    });

    function preRecotizar(dirigidoA){
        var texto = "<p>ESTA COTIZACIÓN DATA DEL " + $("#verFechaCreacion").html() + ". LOS PRECIOS DE LISTA PODRÍAN HABER VARIADO.</p>" +
            "<p>EN LA NUEVA COTIZACIÓN DESEA:</p>" +
            "<ul><li type='A'>TRASLADAR CUALQUIER CAMBIO EN LOS PRECIOS DE LISTA (MANTENIENDO LOS MISMOS % DE DESCUENTO)</li>" +
            "<li type='A'>MANTENER LOS PRECIOS DE LA COTIZACIÓN ANTERIOR</li></ul>";
        var url = "/Cotizacion/ConsultarSiExisteCotizacion";
        if (dirigidoA == DIRIGIDO_A_GRUPO)
            url = "/Cotizacion/ConsultarSiExisteCotizacionGrupal";

        $.ajax({
            url: url,
            type: 'POST',
            async: false,
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.existe == "false") {
                    $.confirm({
                        title: 'Advertencia sobre precios de lista',
                        content: '<div><div class="col-sm-12">' + texto + '</span></div></div>',
                        type: 'orange',
                        buttons: {
                            cancelar: {
                                text: 'Cancelar',
                                btnClass: '',
                                action: function () {
                                    activarBotonesVer();
                                }
                            },
                            aplica: {
                                text: 'OPCIÓN A',
                                btnClass: 'btn-success',
                                action: function () {
                                    $('body').loadingModal("text", "Recotizando...");
                                    $('body').loadingModal("show");
                                    recotizar(true, dirigidoA);
                                }
                            },
                            noAplica: {
                                text: 'OPCIÓN B',
                                btnClass: 'btn-primary',
                                action: function () {
                                    $('body').loadingModal("text", "Recotizando...");
                                    $('body').loadingModal("show");
                                    recotizar(false, dirigidoA);
                                }
                            }

                        }
                    });
                }
                else {
                    mostrarMensajeCotizacionEnCurso(resultado);
                }
            }
        })
    }
    


    function recotizar(mantenerPorcentajeDescuento, dirigidoA)
    {
        var url = "/Cotizacion/recotizarCliente";
        if (dirigidoA == DIRIGIDO_A_GRUPO) {
            var url = "/Cotizacion/recotizarGrupo";
        }

        var numero = $("#codigoCotizacionShow").html();
        $.ajax({
            url: url,
            data: {
                numero: numero,
                mantenerPorcentajeDescuento: mantenerPorcentajeDescuento
            },
            type: 'POST',
            error: function (detalle) {
                $('body').loadingModal("hide");
                alert("Ocurrió un problema al obtener el detalle de la cotización N° " + codigo + ".");
            },
            success: function (fileName) {
                if (dirigidoA == DIRIGIDO_A_GRUPO) {
                    window.location = '/Cotizacion/CotizarGrupo';
                }
                else {
                    window.location = '/Cotizacion/Cotizar';
                }
            }
        });

    }



    /*btnEditarCotizacion desde busqueda*/

    $("#btnEditarCotizacion").click(function () {
        desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona
        $.ajax({
            url: "/Cotizacion/ConsultarSiExisteCotizacion",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/Cotizacion/iniciarEdicionCotizacion",
                        type: 'POST',

                        error: function (detalle) { alert("Ocurrió un problema al obtener el detalle de la cotización N° " + codigo + "."); },
                        success: function (fileName) {
                            window.location = '/Cotizacion/Cotizar';
                        }
                    });

                    //window.location = '/Cotizacion/Cotizador';
                }
                else {
                    mostrarMensajeCotizacionEnCurso(resultado);
                }
            }
        });

    });


    $("#btnEditarCotizacionGrupal").click(function () {
        desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona
        $.ajax({
            url: "/Cotizacion/ConsultarSiExisteCotizacionGrupal",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/Cotizacion/iniciarEdicionCotizacionGrupal",
                        type: 'POST',

                        error: function (detalle) { alert("Ocurrió un problema al obtener el detalle de la cotización N° " + codigo + "."); },
                        success: function (fileName) {
                            window.location = '/Cotizacion/CotizarGrupo';
                        }
                    });

                    //window.location = '/Cotizacion/Cotizador';
                }
                else {
                    mostrarMensajeCotizacionGrupalEnCurso(resultado);
                }
            }
        });

    });

    function mostrarMensajeCotizacionEnCurso(resultado) {
        if (resultado.numero == 0) {
            alert('Está creando una nueva cotización; para continuar por favor diríjase a la página "Crear/Modificar Cotización" y luego haga clic en el botón Cancelar, Finalizar Creación o Guardar (si elige Guardar indique No cuando se le consulte si desea continuar editanto ahora).');
        }
        else {
            if (resultado.numero == $("#codigoCotizacionShow").html())
                alert('Ya se encuentra editando la cotización número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Cotización".');
            else
                alert('Está editando la cotización número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Cotización" y luego haga clic en el botón Cancelar, Finalizar Edición o Guardar (si elige Guardar indique No cuando se le consulte si desea continuar editanto ahora).');
        }
        activarBotonesVer();

    }

    function mostrarMensajeCotizacionGrupalEnCurso(resultado) {
        if (resultado.numero == 0) {
            alert('Está creando una nueva cotización; para continuar por favor diríjase a la página "Crear/Modificar Cotización Grupal" y luego haga clic en el botón Cancelar, Finalizar Creación o Guardar (si elige Guardar indique No cuando se le consulte si desea continuar editanto ahora).');
        }
        else {
            if (resultado.numero == $("#codigoCotizacionShow").html())
                alert('Ya se encuentra editando la cotización número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Cotización Grupal".');
            else
                alert('Está editando la cotización número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Cotización Grupal" y luego haga clic en el botón Cancelar, Finalizar Edición o Guardar (si elige Guardar indique No cuando se le consulte si desea continuar editanto ahora).');
        }
        activarBotonesVer();

    }


    $("#btnGenerarPedido").click(function () {
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
                        url: "/Pedido/iniciarEdicionPedidoDesdeCotizacion",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del pedido."); },
                        success: function (fileName) {
                            window.location = '/Pedido/Pedir';
                        }
                    });

                    //window.location = '/Cotizacion/Cotizador';
                }
                else {
                    if (resultado.numero == 0) {
                        alert('Está creando un nuevo pedido; para continuar por favor diríjase a la página "Crear/Modificar Pedido" y luego haga clic en el botón Cancelar, Finalizar Creación o Guardar (si elige Guardar indique No cuando se le consulte si desea continuar editanto ahora).');
                    }
                    else {
                        if (resultado.numero == $("#codigoCotizacionShow").html())
                            alert('Ya se encuentra editando el pedido número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Cotización".');
                        else
                            alert('Está editando el pedido número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Pedido" y luego haga clic en el botón Cancelar, Finalizar Edición o Guardar (si elige Guardar indique No cuando se le consulte si desea continuar editanto ahora).');
                    }
                    activarBotonesVer();
                }
            }
        });

    });
    








    $("#btnPDFCotizacion").click(function () {
        //$(document).on('click', "button.btnReCotizacion", function () {
        // var codigo = event.target.getAttribute("class").split(" ")[0];
     //   desactivarBotonesVer();
        var numero = $("#codigoCotizacionShow").html();
        $.ajax({
            url: "/Cotizacion/GenerarPDFdesdeIdCotizacion",
            data: {
                codigo: numero
            },
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al descargar la cotización en formato PDF."); },         
            success: function (fileName) {
                window.location = '/General/DownLoadFile?fileName=' + fileName;
            }
        });
    });


    $("#btnExcelCotizacion").click(function () {
        window.location.href = $(this).attr("actionLink");
    });

    $("#lnkDescargarDetalleFormatoExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
    });
    
    

    function limpiarComentario()
    {
        $("#comentarioEstado").val("");
        $("#comentarioEstado").focus();
    }


    $("#btnAprobarCotizacion").click(function () {
        $("#labelNuevoEstado").html(ESTADO_APROBADA_STR);
        $("#estadoId").val(ESTADO_APROBADA);
        limpiarComentario();
    });

    $("#btnDenegarCotizacion").click(function () {
        $("#labelNuevoEstado").html(ESTADO_DENEGADA_STR);
        $("#estadoId").val(ESTADO_DENEGADA);
        limpiarComentario();
    });


    $("#btnAceptarCotizacion").click(function () {
        var clienteLite = $("#verClienteLite").val();

        if (clienteLite == "1") {
            $.confirm({
                title: 'COMPLETAR REGISTRO CLIENTE',
                content: 'Debe completar los datos del cliente antes de aceptar la cotización:',
                type: 'orange',
                buttons: {
                    aplica: {
                        text: 'COMPLETAR REGISTRO',
                        btnClass: 'btn-success',
                        action: function () {
                            var idCliente = $("#verIdCliente").val();
                            window.location = "/Cliente/Editar?idCliente=" + idCliente;
                        }
                    },
                    cancelar: {
                        text: 'CANCELAR',
                        btnClass: '',
                        action: function () {

                        }
                    }
                }
            });
        } else {
            $('#modalAprobacion').modal('show')
            $("#labelNuevoEstado").html(ESTADO_ACEPTADA_STR);
            $("#estadoId").val(ESTADO_ACEPTADA);
            limpiarComentario();
        }
    });

    $("#btnEliminarCotizacion").click(function () {

        $("#labelNuevoEstado").html(ESTADO_ELIMINADA_STR);
        $("#estadoId").val(ESTADO_ELIMINADA);
        limpiarComentario();
    });

    $("#btnRechazarCotizacion").click(function () {

        $("#labelNuevoEstado").html(ESTADO_RECHAZADA_STR);
        $("#estadoId").val(ESTADO_RECHAZADA);
        limpiarComentario();
    });



 




    $('#modalAprobacion').on('shown.bs.modal', function (e) {
        limpiarComentario();
    });



    $("#btnAceptarCambioEstado").click(function () {

        var estado = $("#estadoId").val();
        var comentarioEstado = $("#comentarioEstado").val();

        if ($("#labelNuevoEstado").html() == ESTADO_DENEGADA_STR || $("#labelNuevoEstado").html() == ESTADO_RECHAZADA_STR) {
            if (comentarioEstado.trim() == "") {
                alert("Cuando Deniega o Rechaza una cotización debe ingresar un Comentario.");
                return false;
            }
        }
        var codigo = $("#codigoCotizacionShow").html();

        $.ajax({
            url: "/Cotizacion/updateEstadoCotizacion",
            data: {
                codigo: codigo,
                estado: estado,
                observacion: comentarioEstado
            },
            type: 'POST',
            error: function () {
                alert("Ocurrió un problema al intentar cambiar el estado de la cotización.")
                $("#btnCancelarCambioEstado").click();
            },
            success: function () {

                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El estado de la cotización número: " + codigo + " se cambió correctamente.",
                    buttons: {
                        OK: function () { location.reload();}
                    }
                });

                
                //$("#btnBusquedaCotizaciones").click();
            }
        });
    });


    $(".chkProductSerchCheckParam").change(function () {
        var param = $(this).attr("paramName");
        var valor = 0;
        if ($(this).is(":checked")) {
            valor = $(this).attr("checkValue");
        } else {
            valor = $(this).attr("unCheckValue");
        }


        $.ajax({
            url: "/Producto/SetSearchParam",
            type: 'POST',
            data: {
                parametro: param,
                valor: valor
            },
            success: function () {

            }
        });

    });

    $("#chkAjusteCalculoPrecios").change(function () {
        if ($("#chkAjusteCalculoPrecios").is(":checked")) {
            $("#chkAjusteCalculoPreciosModal").prop("checked", true);
        } else {
            $("#chkAjusteCalculoPreciosModal").prop("checked", false);
        }

        cambioChkAjusteCalculoPrecios();
    });

    $("#chkAjusteCalculoPreciosModal").change(function () {
        if ($("#chkAjusteCalculoPreciosModal").is(":checked")) {
            $("#chkAjusteCalculoPrecios").prop("checked", true);
        } else {
            $("#chkAjusteCalculoPrecios").prop("checked", false);
        }

        cambioChkAjusteCalculoPrecios();
    });

    
    $("#lblChkAjusteCalculoPrecios").click(function () {
        if ($("#chkAjusteCalculoPrecios").is(":checked")) {
            $("#chkAjusteCalculoPreciosModal").prop("checked", false);
            $("#chkAjusteCalculoPrecios").prop("checked", false);
        } else {
            $("#chkAjusteCalculoPreciosModal").prop("checked", true);
            $("#chkAjusteCalculoPrecios").prop("checked", true);
        }

        cambioChkAjusteCalculoPrecios();
    });

    $("#lblChkAjusteCalculoPreciosModal").click(function () {
        if ($("#chkAjusteCalculoPreciosModal").is(":checked")) {
            $("#chkAjusteCalculoPreciosModal").prop("checked", false);
            $("#chkAjusteCalculoPrecios").prop("checked", false);
        } else {
            $("#chkAjusteCalculoPreciosModal").prop("checked", true);
            $("#chkAjusteCalculoPrecios").prop("checked", true);
        }

        cambioChkAjusteCalculoPrecios();
    });

    function cambioChkAjusteCalculoPrecios() {
        var ajusteCalculoPrecios = $('#chkAjusteCalculoPrecios').prop('checked');
        $.ajax({
            url: "/Cotizacion/updateAjusteCalculoPrecios",
            type: 'POST',
            data: {
                ajusteCalculoPrecios: ajusteCalculoPrecios
            },
            success: function () {

            }
        });

        if ($("#chkAjusteCalculoPrecios").is(":checked")) {
            $("#lblChkAjusteCalculoPreciosModal").addClass("lbl-ajuste-calculo-precios");
            $("#lblChkAjusteCalculoPreciosModal").removeClass("text-muted");

            $("#lblChkAjusteCalculoPrecios").addClass("lbl-ajuste-calculo-precios");
            $("#lblChkAjusteCalculoPrecios").removeClass("text-muted");
        } else {
            $("#lblChkAjusteCalculoPreciosModal").addClass("text-muted");
            $("#lblChkAjusteCalculoPreciosModal").removeClass("lbl-ajuste-calculo-precios");

            $("#lblChkAjusteCalculoPrecios").addClass("text-muted");
            $("#lblChkAjusteCalculoPrecios").removeClass("lbl-ajuste-calculo-precios");
        }
    }
    
    var ft = null;



    //Mantener en Session cambio de Seleccion de IGV
    $("input[name=igv]").on("click", function () {
        var igv = $("input[name=igv]:checked").val();
        $.ajax({
            url: "/Cotizacion/updateSeleccionIGV",
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

    function ajustarFormularioPorTipoCotizacion() {
        var tipoCotizacion = $("#tipoCotizacion").val();

        if (tipoCotizacion == 0) {
            $("#fechaInicioVigenciaPrecios").closest("div.row").show();
        } else {
            $("#fechaInicioVigenciaPrecios").closest("div.row").hide();

        }
    }


    $("#tipoCotizacion").change(function () {
        var tipoCotizacion = $("#tipoCotizacion").val();
     
        if (tipoCotizacion == 0) {
            $("#fechaInicioVigenciaPrecios").closest("div.row").show();
            $("#fechaInicioVigenciaPrecios").removeAttr("disabled");
            $("#fechaFinVigenciaPrecios").removeAttr("disabled");
        }
        else if (tipoCotizacion == 2) {
            //$("#fechaInicioVigenciaPrecios").removeAttr("disabled");
            $("#fechaInicioVigenciaPrecios").attr("disabled", "disabled");
            $("#fechaFinVigenciaPrecios").attr("disabled", "disabled");
            $("#fechaInicioVigenciaPrecios").val("");
            $("#fechaInicioVigenciaPrecios").change();
            $("#fechaFinVigenciaPrecios").val("");
            $("#fechaFinVigenciaPrecios").change();
        }
        else
        {
            $("#fechaInicioVigenciaPrecios").closest("div.row").hide();

            if ($("#mostrarValidezOfertaEnDias").val() == 0) {
                $("#validezOfertaEnDias").val(DIAS_DEFECTO_VALIDEZ_OFERTA_COTIZACION_PUNTUAL);
                $("#validezOfertaEnDias").change();
            }
            else {
                //la fecha de validez de oferta no debe estar vacía
                var fechaDefectoValidezOferta = new Date();
                fechaDefectoValidezOferta.setDate(fechaDefectoValidezOferta.getDate() + parseInt(DIAS_DEFECTO_VALIDEZ_OFERTA_COTIZACION_PUNTUAL));

                var fechaDefectoValidezOfertaTexto = "";
                let day = fechaDefectoValidezOferta.getDate()
                let month = fechaDefectoValidezOferta.getMonth() + 1
                let year = fechaDefectoValidezOferta.getFullYear()

                if (month < 10) {
                    fechaDefectoValidezOfertaTexto = `${day}/0${month}/${year}`;
                } else {
                    fechaDefectoValidezOfertaTexto = `${day}/${month}/${year}`;
                }

                $("#fechaLimiteValidezOferta").datepicker("setDate", fechaDefectoValidezOfertaTexto);
                $("#fechaLimiteValidezOferta").change();
            }
        }

        
        $.ajax({
            url: "/Cotizacion/changeTipoCotizacion",
            type: 'POST',
            data: {
                tipoCotizacion: tipoCotizacion
            },
            success: function (cantidad) {
            }
        });


    });

    //Mantener en Session cambio de Seleccion de IGV
    $("#considerarCantidades").change( function () {
        var considerarCantidades = $("#considerarCantidades").val();
        $.ajax({
            url: "/Cotizacion/updateSeleccionConsiderarCantidades",
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

    $("#chkEsPagoContado").change(function () {
        var valor = 1;
        if (!$('#chkEsPagoContado').prop('checked')) {
            valor = 0;
        }
        $.ajax({
            url: "/Cotizacion/updateEsPagoContado",
            type: 'POST',
            data: {
                esPagoContado: valor
            },
            dataType: 'JSON',
            success: function (result) {
                $("#cotizacion_textoCondicionesPago").val(result.textoCondicionesPago);
            }
        });
    });

    $("#observaciones").change(function () {

        $.ajax({
            url: "/Cotizacion/updateObservaciones",
            type: 'POST',
            data: {
                observaciones: $("#observaciones").val()
            },
            success: function () { }
        });
    });

    

    //Mantener en Session cambio de Seleccion de Mostrar Proveedor
    $("input[name=mostrarcodproveedor]").on("click", function () {
        var mostrarcodproveedor = $("input[name=mostrarcodproveedor]:checked").val();
        $.ajax({
            url: "/Cotizacion/updateMostrarCodigoProveedor",
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
            url: "/Cotizacion/updateCliente",
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
            url: "/Cotizacion/updateContacto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                contacto: $("#contacto").val()
            },
            success: function () { }
        });
    });

    $("#flete").change(function () {

        $("#flete").val(Number($("#flete").val()).toFixed(cantidadDecimalesPrecioNeto))
        var flete = $("#flete").val(); 
        if (flete > 100)
        {
            $("#flete").val("100.00");
            flete = 100;
        }

        var total = Number($("#total").val());
        $('#montoFlete').html("Flete: " + SIMBOLO_SOL + " " + (total * flete / 100).toFixed(cantidadDecimalesPrecioNeto));
        $('#montoTotalMasFlete').html("Total más Flete: " + SIMBOLO_SOL + " " + (total + (total * flete / 100)).toFixed(cantidadDecimalesPrecioNeto));

        


        $.ajax({
            url: "/Cotizacion/updateFlete",
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
            url: "/Cotizacion/updateMostrarCosto",
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
        var $modal = $('#tableDetalleCotizacion'),
            $editor = $('#tableDetalleCotizacion'),
            $editorTitle = $('#tableDetalleCotizacion');

     
        ft = FooTable.init('#tableDetalleCotizacion', {
            editing: {
                enabled: true,
                addRow: function () {
                    ConfirmDialogReload(MENSAJE_CANCELAR_EDICION)
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
                                                    url: "/Cotizacion/DelProducto",
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
            $("#btnContinuarCreandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarCreacionCotizacion").attr('disabled', 'disabled');
            $("#btnCancelCotizacion").attr('disabled', 'disabled');
        }
        else {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarEdicionCotizacion").attr('disabled', 'disabled');
            $("#btnCancelCotizacion").attr('disabled', 'disabled');
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
            if (observacion == null)
                observacion = "";

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
            url: "/Cotizacion/ChangeDetalle",
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
        var precio = Number((precioLista * (100 - porcentajeDescuento) / 100).toFixed(cantidadDecimalesPrecioNeto));
        //Se asigna el precio calculculado en la columna precio
        $("." + idproducto + ".detprecio").html(precio);
        //se obtiene la cantidad
        var cantidad = Number($("." + idproducto + ".detincantidad").val());
        //Se define el precio Unitario 
        var precioUnitario = flete + precio
        $("." + idproducto + ".detprecioUnitario").html(precioUnitario.toFixed(cantidadDecimalesPrecioNeto));
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


  /*      var precioListaAnterior = Number($("." + idproducto + ".detcostoAnterior").html());
        var varcosto = (costo / costoAnterior - 1) * 100;
        $("." + idproducto + ".detvarprecioNetoAnterior").text(varprecioNetoAnterior.toFixed(1));
        */

      /*
        var costoAnterior = Number($("." + idproducto + ".detcostoAnterior").html());       
        var varcosto = (costo / costoAnterior - 1)*100;
        $("." + idproducto + ".detvarCosto").text(varcosto.toFixed(1) + " %");
        */

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



    $("#btnCancelCotizacion").click(function () {

        ConfirmDialog(MENSAJE_CANCELAR_EDICION,'/Cotizacion/CancelarCreacionCotizacion',null)
    })


    $("#lnkVerHistorial").click(function () {
        showHistorial();
    })

    function showHistorial() {
        $('body').loadingModal({
            text: 'Obteniendo Historial...'
        });
        
        var idCotizacion = $("#verIdCotizacion").val();
        
        $.ajax({
            url: "/Cotizacion/GetHistorial",
            data: {
                id: idCotizacion
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) { $('body').loadingModal('hide'); alert("Ocurrió un problema al obtener el historial de la cotización."); },
            success: function (resultado) {
                $("#historial_titulo_numero_cotizacion").html($("#codigoCotizacionShow").html());
                $("#tableHistorialCotizacion > tbody").empty();

                FooTable.init('#tableHistorialCotizacion');
                
                var d = '';
                var lista = resultado.result;
                for (var i = 0; i < resultado.result.length; i++) {

                    var observacion = lista[i].observacion == null || lista[i].observacion == 'undefined' ? '' : lista[i].observacion;

                    d += '<tr>' +
                        '<td>' + lista[i].FechaRegistroDesc + '</td>' +
                        '<td>' + lista[i].usuario.nombre + '</td>' +
                        '<td>' + lista[i].estadoString + '</td>' +
                        '<td>' + observacion + '</td>' +
                        '</tr>';

                }
                //  
                // sleep
                $("#tableHistorialCotizacion").append(d);

                


                $("#modalVerHistorialCotizacion").modal('show');
                $('body').loadingModal('hide'); 
                //  window.location = '/Cotizacion/Index';
            }
        });
    }




    /*####################################################
    EVENTOS BUSQUEDA COTIZACIONES
    #####################################################*/

    $("#btnLimpiarBusquedaCotizaciones").click(function () {
        $.ajax({
            url: "/Cotizacion/CleanBusquedaCotizaciones",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });
    
    $("#btnExportExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
    });



    $("#btnBusquedaCotizaciones").click(function () {
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val();
        var fechaDesde = $("#fechaDesde").val();
        var fechaHasta = $("#fechaHasta").val();
        var numero = $("#numero").val();
        var estado = $("#estado").val();


        $("#btnBusquedaCotizaciones").attr("disabled", "disabled");
        $.ajax({
            url: "/Cotizacion/Search",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                fechaDesde: fechaDesde,
                fechaHasta: fechaHasta,
                numero: numero,
                estado: estado
            },
            error: function () {
                $("#btnBusquedaCotizaciones").removeAttr("disabled");
            },
            success: function (cotizacionList) {
                $("#btnBusquedaCotizaciones").removeAttr("disabled");

                $("#tableCotizaciones > tbody").empty();
                //FooTable.init('#tableCotizaciones');
                $("#tableCotizaciones").footable({
                    "paging": {
                        "enabled": true
                    }
                });



                for (var i = 0; i < cotizacionList.length; i++) {

                    var observacion = cotizacionList[i].seguimientoCotizacion_observacion == null ? "" : cotizacionList[i].seguimientoCotizacion_observacion;

                    if (cotizacionList[i].seguimientoCotizacion_observacion != null && cotizacionList[i].seguimientoCotizacion_observacion.length > 20) {
                        var idComentarioCorto = cotizacionList[i].idCotizacion + "corto";
                        var idComentarioLargo = cotizacionList[i].idCotizacion + "largo";
                        var idVerMas = cotizacionList[i].idCotizacion + "verMas";
                        var idVermenos = cotizacionList[i].idCotizacion + "verMenos";
                        var comentario = cotizacionList[i].seguimientoCotizacion_observacion.substr(0, 20) + "...";

                        observacion = '<div id="' + idComentarioCorto + '" style="display:block;">' + comentario + '</div>' +
                            '<div id="' + idComentarioLargo + '" style="display:none;">' + cotizacionList[i].seguimientoCotizacion_observacion + '</div>' +
                            '<p><a id="' + idVerMas + '" class="' + cotizacionList[i].idCotizacion + ' verMas" href="javascript:mostrar();" style="display:block">Ver Más</a></p>' +
                            '<p><a id="' + idVermenos + '" class="' + cotizacionList[i].idCotizacion + ' verMenos" href="javascript:mostrar();" style="display:none">Ver Menos</a></p>';
                    }


                    var grupo = '';
                    var clienteRazonSocial = '';
                    var clienteRUC = '';
                    var creadoPara = '';
                    grupo = cotizacionList[i].grupo_nombre;
                    if (cotizacionList[i].cliente_idCliente == GUID_EMPTY) {
                        creadoPara = 'Grupo';
                    }
                    else {
                        creadoPara = 'Cliente';
                        clienteRazonSocial = cotizacionList[i].cliente_razonSocial;
                        clienteRUC = cotizacionList[i].cliente_ruc;
                    }

                    var cotizacion = '<tr data-expanded="false">' +
                        '<td>' + cotizacionList[i].idCotizacion + '</td>' +
                        '<td>' + cotizacionList[i].codigo + '</td>' +
                        '<td>' + cotizacionList[i].usuario_nombre + '</td>' +
                        //ToString("dd/MM/yyyy")
                        '<td>' + invertirFormatoFecha(cotizacionList[i].fecha.substr(0, 10)) + '</td>' +


                        '<td>' + creadoPara + '</td>' +
                        '<td>' + clienteRazonSocial + '</td>' +
                        //'<td>' + clienteRUC + '</td>' +
                        '<td>' + grupo + '</td>' +
                        '<td>' + cotizacionList[i].ciudad_nombre + '</td>' +
                        '<td>' + Number(cotizacionList[i].montoSubTotal).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(cotizacionList[i].montoIGV).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(cotizacionList[i].montoTotal).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(cotizacionList[i].maximoPorcentajeDescuentoPermitido).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(cotizacionList[i].minimoMargen).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + cotizacionList[i].seguimientoCotizacion_estadoString + '</td>' +
                        '<td>' + cotizacionList[i].seguimientoCotizacion_usuario_nombre + '</td>' +
                        '<td>' + observacion + '</td>' +
                        '<td><button type="button" class="' + cotizacionList[i].codigo + ' btnVerCotizacion btn btn-primary ">Ver</button></td></tr>';
                    //ToString("dd/MM/yyyy")
                    $("#tableCotizaciones").append(cotizacion);                  
                }

                if (cotizacionList.length > 0) {
                    $("#msgBusquedaSinResultados").hide();
                    $("#divExportButton").show();
                }
                else {
                    $("#msgBusquedaSinResultados").show();
                    $("#divExportButton").hide();
                }

                //  location.reload();
            }
        });
    });

    $("#fechaDesde").change(function () {
        var fechaDesde = $("#fechaDesde").val();
        $.ajax({
            url: "/Cotizacion/updateFechaDesde",
            type: 'POST',
            data: {
                fechaDesde: fechaDesde
            },
            success: function () {
            }
        });
    });

    $("#fechaHasta").change(function () {
        var fechaHasta = $("#fechaHasta").val();
        $.ajax({
            url: "/Cotizacion/updateFechaHasta",
            type: 'POST',
            data: {
                fechaHasta: fechaHasta
            },
            success: function () {
            }
        });
    });

    $("#numero").change(function () {
        var codigo = $("#numero").val();
        $.ajax({
            url: "/Cotizacion/updateCodigoCotizacionBusqueda",
            type: 'POST',
            data: {
                codigo: codigo
            },
            success: function () {
            }
        });
    });

    $("#estado").change(function () {
        var estado = $("#estado").val();
        $.ajax({
            url: "/Cotizacion/updateEstadoCotizacionBusqueda",
            type: 'POST',
            data: {
                estado: estado
            },
            success: function () {
            }
        });
    });

    
    $("#idCiudad").change(function () {
        var idCiudad = $("#idCiudad").val();

        $.ajax({
            url: "/Cotizacion/ChangeIdCiudad",
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

    $("#codigoMoneda").change(function () {
        var moneda = $("#codigoMoneda").val();

        $.ajax({
            url: "/Cotizacion/ChangeMoneda",
            type: 'POST',
            dataType: 'JSON',
            data: {
                codigoMoneda: moneda
            },
            error: function (detalle) {
                $.alert({
                    title: "ERROR",
                    type: 'red',
                    content: 'Ocurrió un error al intentar cambiar la moneda, por favor inténtelo de nuevo.',
                    buttons: {
                        OK: function () { }
                    }
                });
                location.reload();
            },
            success: function (res) {
                if (res.success == 1) {
                    $(".mostrarSimboloMoneda").html(res.simbolo);
                } else {
                    $.alert({
                        title: "ERROR",
                        type: 'red',
                        content: res.message,
                        buttons: {
                            OK: function () { }
                        }
                    });
                }
                
            }
        });
    });


    $("#buscarSedesGrupoCliente").change(function () {
        var valor = $("input[name=buscarSedesGrupoCliente]:checked").val();
        $.ajax({
            url: "/Cotizacion/updateBuscarSedesGrupoCliente",
            type: 'POST',
            data: {
                buscarSedesGrupoCliente: valor
            },
            success: function () {
            }
        });
    });

    $("#buscarSoloCotizacionesGrupales").change(function () {
        var valor = $("input[name=buscarSoloCotizacionesGrupales]:checked").val();
        $.ajax({
            url: "/Cotizacion/updateBuscarSoloCotizacionesGrupales",
            type: 'POST',
            data: {
                buscarSoloCotizacionesGrupales: valor
            },
            success: function () {
            }
        });
    });
    


    $("#btnCancelarComentario").click(function()
    {
        window.location = '/Cotizacion/CancelarCreacionCotizacion';
    });

    $("#btnCancelarComentarioAprobacion").click(function () {
        generarPDF();
    });


    $("#btnAceptarComentarioAprobacion").click(function () {
        var codigoCotizacion = $("#numero").val();
        var observacion = $("#comentarioAprobacion").val();
        $.ajax({
            url: "/Cotizacion/updateEstadoCotizacion",
            data: {
                codigo: codigoCotizacion,
                estado: ESTADO_APROBADA,
                observacion: observacion
            },
            type: 'POST',
            error: function () {
                alert("Ocurrió un problema al intentar agregar un comentario a la cotización.")
                $("#btnCancelarComentarioAprobacion").click();
            },
            success: function () {
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El comentario del estado de la cotización número: " + codigoCotizacion + " se cambió correctamente.",
                    buttons: {
                        OK: function () {
                            $("#btnCancelarComentarioAprobacion").click();
                        }
                    }
                });

                
                
            }
        });

    });


    $("#btnAceptarComentario").click(function () {
        var codigoCotizacion = $("#numero").val();
        var observacion = $("#comentarioPendienteAprobacion").val();
        var observacionEditable = $("#comentarioPendienteAprobacionEditable").val();
        if (observacionEditable.trim().length < 15) {
            $("#comentarioPendienteAprobacionEditable").focus();
            $.alert({
                title: "Agregar Comentario",
                type: 'orange',
                content: 'Debe ingresar al menos 15 caracteres en el comentario.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }   

        $.ajax({
            url: "/Cotizacion/updateEstadoCotizacion",
            data: {
                codigo: codigoCotizacion,
                estado: ESTADO_PENDIENTE_APROBACION,
                observacion: observacionEditable + "\n" + observacion
            },
            type: 'POST',
            error: function () {
                alert("Ocurrió un problema al intentar agregar un comentario a la cotización.")
                //$("#btnCancelarComentario").click();
                window.location = '/Cotizacion/CancelarCreacionCotizacion';
            },
            success: function () {
                
                $.alert({
                    title: TITLE_EXITO,
                    type: 'green',
                    content: "El comentario del estado de la cotización número: " + codigoCotizacion + " se cambió correctamente.",
                    buttons: {
                        OK: function () { window.location = '/Cotizacion/CancelarCreacionCotizacion'; }
                    }
                });


                
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


});



$(document).ready(function () {
    $('#btnUploadProducts').click(function (event) {
        var fileInput = $('#fileUploadExcel');
        var maxSize = fileInput.data('max-size');
        var maxSizeText = fileInput.data('max-size-text');
        var imagenValida = true;
        if (fileInput.get(0).files.length) {
            var fileSize = fileInput.get(0).files[0].size; // in bytes

            if (fileSize > maxSize) {
                $.alert({
                    title: "Archivo Inválido",
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
                title: "Archivo Inválido",
                type: 'red',
                content: 'Seleccione un archivo por favor.',
                buttons: {
                    OK: function () { }
                }
            });
            imagenValida = false;
        }

        if (imagenValida) {

            var that = document.getElementById('fileUploadExcel');
            var file = that.files[0];
            var form = new FormData();
            var url = $(that).data("urlSetFile");
            var reader = new FileReader();
            var mime = file.type;

            // read the image file as a data URL.
            reader.readAsDataURL(file);

            form.append('file', file);

            $('body').loadingModal({
                text: '...'
            });
            $.ajax({
                url: url,
                type: "POST",
                cache: false,
                contentType: false,
                processData: false,
                data: form,
                dataType: 'JSON',
                beforeSend: function () {
                    //$.blockUI();
                },
                success: function (response) {
                    if (response.success == "true") {
                        $.alert({
                            title: "Carga Exitosa!",
                            type: 'green',
                            content: response.message,
                            buttons: {
                                OK: function () {
                                    location.reload();
                                }
                            }
                        });

                        $('#btnBusqueda').click();
                    } else {
                        $.alert({
                            title: "Carga fallida",
                            type: 'red',
                            content: response.message,
                            buttons: {
                                OK: function () { }
                            }
                        });
                    }
                },
                error: function (error) {
                    console.log(error);
                    $.alert({
                        title: "Carga fallida",
                        type: 'red',
                        content: 'Ocurrió un error al subir el archivo.',
                        buttons: {
                            OK: function () { }
                        }
                    });
                }
            }).done(function () {
                $('body').loadingModal('hide')
            });
        }
    });
});
