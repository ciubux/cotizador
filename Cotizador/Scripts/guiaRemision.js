
jQuery(function ($) {


    //CONSTANTES POR DEFECTO
    var cantidadDecimales = 2;
    var IGV = 0.18;
    var SIMBOLO_SOL = "S/";
    var MILISEGUNDOS_AUTOGUARDADO = 5000;

    //Estados para búsqueda de Pedidos
    var ESTADOS_TODOS = -1;
    var ESTADO_PENDIENTE_APROBACION = 0;
    var ESTADO_APROBADA = 1;
    var ESTADO_DENEGADA = 2;
    var ESTADO_ACEPTADA = 3;
    var ESTADO_RECHAZADA = 4;
    var ESTADO_EN_EDICION = 7;

    //Etiquetas de estadps para búsqueda de Pedidos
    var ESTADO_PENDIENTE_APROBACION_STR = "Pendiente de Aprobación";
    var ESTADO_APROBADA_STR = "Aprobada";
    var ESTADO_DENEGADA_STR = "Denegada";
    var ESTADO_ACEPTADA_STR = "Aceptada";
    var ESTADO_RECHAZADA_STR = "Rechazada";
    var ESTADO_EN_EDICION_STR = "En Edición";

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
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición/creación; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";

    $(document).ready(function () {
        obtenerConstantes();
        setTimeout(autoGuardarGuiaRemision, MILISEGUNDOS_AUTOGUARDADO);
        cargarChosenCliente(pagina);      
        verificarSiExisteNuevoTransportista();
        $("#btnBusqueda").click();
    });

    function verificarSiExisteNuevoTransportista() {
        $('#guiaRemision_transportista option').each(function () {
            if ($(this).val() == GUID_EMPTY) {
                $("#btnAgregarTransportista").attr("disabled", "disabled");
            }
        });
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
            }
        });
    }

    function autoGuardarGuiaRemision() {
        $.ajax({
            url: "/Pedido/autoSavePedido",
            type: 'POST',
            error: function () {
                setTimeout(autoGuardarPedido, MILISEGUNDOS_AUTOGUARDADO);
            },
            success: function () {
                setTimeout(autoGuardarPedido, MILISEGUNDOS_AUTOGUARDADO);
            }
        });
    }


    /**
    *################################## INICIO CONTROLES CIUDAD
    */

    function onChangeCiudad(parentController) {
        $.ajax({
            url: "/" + parentController + "/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
            idCiudad: this.value
            }, 
            error: function (detalle) {
                alert('Debe eliminar los productos agregados antes de cambiar de Sede.'); 
                location.reload(); 
            },
            success: function (ciudad) {
                alert(ciudad)
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

    $("#idCliente").change(function () {
      //  $("#contacto").val("");
        var idCliente = $(this).val();

        $.ajax({
            url: "/Pedido/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente) {
                $("#pedido_numeroReferenciaCliente").val("");
                $("#pedido_direccionEntrega").val("");
                $("#pedido_contactoEntrega").val("");
                $("#pedido_telefonoContactoEntrega").val("");
                $("#pedido_contactoPedido").val("");
                $("#pedido_telefonoContactoPedido").val("");
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




    

    /**
     * FIN CONTROLES DE CLIENTE
     */





















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


    var fechaMovimiento = $("#fechaMovimientotmp").val();
    $("#guiaRemision_fechaMovimiento").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaMovimiento);

    var fechaMovimientoDesde = $("#fechaMovimientoDesdetmp").val();
    $("#pedido_fechaMovimientoDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaMovimientoDesde);

    var fechaMovimientoHasta = $("#fechaMovimientoHastatmp").val();
    $("#pedido_fechaMovimientoHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaMovimientoHasta);


    /*
    var fechaEntrega = $("#fechaEntregaTmp").val();
    $("#fechaEntrega").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEntrega);

    var fechaMaximaEntrega = $("#fechaMaximaEntregaTmp").val();
    $("#fechaMaximaEntrega").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaMaximaEntrega);



    

    var fechaEntregaDesde = $("#fechaEntregaDesdetmp").val();
    $("#pedido_fechaEntregaDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEntregaDesde);

    var fechaEntregaHasta = $("#fechaEntregaHastaTmp").val();
    $("#pedido_fechaEntregaHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEntregaHasta);


    var fechaPrecios = $("#fechaPreciostmp").val();
    $("#fechaPrecios").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaPrecios);    

 */



    /**
     * FIN DE CONTROLES DE FECHAS
     */



    /* ################################## INICIO CHANGE CONTROLES */

    function toggleControlesTransportista() {
        var idTransportista = $("#guiaRemision_transportista").val();
        if (idTransportista == "") {
            $("#guiaRemision_transportista_descripcion").attr('disabled', 'disabled');
            $("#guiaRemision_transportista_ruc").attr('disabled', 'disabled');
            $("#guiaRemision_transportista_direccion").attr('disabled', 'disabled');
            $("#guiaRemision_transportista_brevete").attr('disabled', 'disabled');
            
        }
        else {
            /*  $("#pedido_direccionEntrega_telefono").val($('#pedido_direccionEntrega').find(":selected").attr("telefono"));*/
            $("#guiaRemision_transportista_descripcion").removeAttr("disabled");
            $("#guiaRemision_transportista_ruc").removeAttr("disabled");
            $("#guiaRemision_transportista_direccion").removeAttr("disabled");
            $("#guiaRemision_transportista_brevete").removeAttr("disabled");
        }
    }

    $("#guiaRemision_transportista").change(function () {
        toggleControlesTransportista();
        var idTransportista = $("#guiaRemision_transportista").val();

        $.ajax({
            url: "/GuiaRemision/ChangeTransportista",
            type: 'POST',
            dataType: "JSON",
            data: {
                idTransportista: idTransportista
            },
            success: function (transportista) {

                $("#guiaRemision_transportista_descripcion").val(transportista.descripcion);
                $("#guiaRemision_transportista_ruc").val(transportista.ruc);
                $("#guiaRemision_transportista_direccion").val(transportista.direccion);
                $("#guiaRemision_transportista_brevete").val(transportista.brevete);
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


    $("#pedido_direccionEntrega").change(function () {
        $.ajax({
            url: "/Pedido/ChangeDireccionEntrega",
            type: 'POST',
            data: {
                direccionEntrega: $("#pedido_direccionEntrega").val()
            },
            success: function () { }
        });
    });

    $("#pedido_contactoEntrega").change(function () {
        $.ajax({
            url: "/Pedido/ChangeContactoEntrega",
            type: 'POST',
            data: {
                contactoEntrega: $("#pedido_contactoEntrega").val()
            },
            success: function () { }
        });
    });
    
    $("#pedido_telefonoContactoEntrega").change(function () {
        $.ajax({
            url: "/Pedido/ChangeTelefonoContactoEntrega",
            type: 'POST',
            data: {
                telefonoContactoEntrega: $("#pedido_telefonoContactoEntrega").val()
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



    $("#fechaMaximaEntrega").change(function () {
        var fechaMaximaEntrega = $("#fechaMaximaEntrega").val();
        $.ajax({
            url: "/Pedido/ChangeFechaMaximaEntrega",
            type: 'POST',
            data: {
                fechaMaximaEntrega: fechaMaximaEntrega
            },
            success: function () {
            }
        });
    });

    $("#fechaEntrega").change(function () {
        var fechaEntrega = $("#fechaEntrega").val();
        $.ajax({
            url: "/Pedido/ChangeFechaEntrega",
            type: 'POST',
            data: {
                fechaEntrega: fechaEntrega
            },
            success: function () {
            }
        });
    });
  

    $("#pedido_contactoPedido").change(function () {
        $.ajax({
            url: "/Pedido/ChangeContactoPedido",
            type: 'POST',
            data: {
                contactoPedido: $("#pedido_contactoPedido").val()
            },
            success: function () { }
        });
    });

    $("#pedido_telefonoContactoPedido").change(function () {
        $.ajax({
            url: "/Pedido/ChangeTelefonoContactoPedido",
            type: 'POST',
            data: {
                telefonoContactoPedido: $("#pedido_telefonoContactoPedido").val()
            },
            success: function () { }
        });
    });
    
    $("#pedido_observaciones").change(function () {
        $.ajax({
            url: "/Pedido/ChangeObservaciones",
            type: 'POST',
            data: {
                observaciones: $("#pedido_observaciones").val()
            },
            success: function () { }
        });
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
        /*
        //para agregar un producto se debe seleccionar un cliente
        if ($("#idCliente").val().trim() == "") {
            alert("Debe seleccionar previamente un cliente.");
            $('#idCliente').trigger('chosen:activate');
            return false;
        }*/


        //Se limpia el mensaje de resultado de agregar producto
        $("#resultadoAgregarProducto").html("");

        //Se desactiva el boton de agregar producto
        desactivarBtnAddProduct();

        //Se limpian los campos
        $("#unidad").html("");
        $("#imgProducto").attr("src", "images/NoDisponible.gif");
        $('#valor').attr('type', 'text');
        $('#valorAlternativo').attr('type', 'hidden');
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

        //calcularSubtotalProducto();

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
                $("#porcentajeDescuento").val(Number(producto.porcentajeDescuento).toFixed(4));
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
                        '<td>' + Number(producto.precioListaList[i].precioNeto).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(producto.precioListaList[i].flete).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(producto.precioListaList[i].precioUnitario).toFixed(cantidadDecimales) + '</td>' +

                        '</tr>');
                }



                //Activar Botón para agregar producto a la grilla
                activarBtnAddProduct();

                //Se calcula el subtotal del producto
                calcularSubtotalProducto();
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

        precio = precioLista * (100 - porcentajeDescuento) * 0.01;
        precio = precio.toFixed(cantidadDecimales);


        var precioUnitario = Number(precio) + Number($('#fleteDetalle').val());
        $('#precioUnitario').val(precioUnitario.toFixed(cantidadDecimales));


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
            url: "/Precio/GetPreciosRegistrados",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: idProducto,
                idCliente: idCliente,
                controller: "pedido"
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
                        '<td>' + Number(precioListaList[i].precioNeto).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(precioListaList[i].flete).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(precioListaList[i].precioUnitario).toFixed(cantidadDecimales) + '</td>' +

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

                var esRecotizacion = "";
                if ($("#esRecotizacion").val() == "1") {
                    esRecotizacion = '<td class="' + detalle.idProducto + ' detprecioNetoAnterior" style="text-align:right; color: #B9371B">0.00</td>' +
                        '<td class="' + detalle.idProducto + ' detvarprecioNetoAnterior" style="text-align:right; color: #B9371B">0.0 %</td>' +
                        '<td class="' + detalle.idProducto + ' detvarCosto" style="text-align:right; color: #B9371B">0.0 %</td>' +
                        '<td class="' + detalle.idProducto + ' detcostoAnterior" style="text-align:right; color: #B9371B">0.0</td>';
                }

                var observacionesEnDescripcion = "<br /><span class='" + detalle.idProducto + " detproductoObservacion'  style='color: darkred'>" + detalle.observacion + "</span>";

                $('#tableDetalleGuia tbody tr.footable-empty').remove();
                $("#tableDetalleGuia tbody").append('<tr data-expanded="true">' +
                    '<td>' + detalle.idProducto + '</td>' +
                    '<td>' + detalle.codigoProducto + '</td>' +
                    '<td class="' + detalle.idProducto + ' detcantidad" style="text-align:right">' + cantidad + '</td>' +
                    '<td>' + detalle.unidad + '</td>' +
                    '<td>' + detalle.nombreProducto + observacionesEnDescripcion + '</td>' +
                '</tr> ');

                $('#tableDetalleGuia thead tr th.footable-editing').remove();
                $('#tableDetalleGuia tbody tr td.footable-editing').remove();


            /*    $('#montoIGV').html(detalle.igv);
                $('#montoSubTotal').html(detalle.subTotal);
                ///var flete = Number($("#flete").val());
                $('#montoTotal').html(detalle.total);
                $("#total").val(detalle.total);
                var total = Number($("#total").val())
                $('#montoFlete').html((total * flete / 100).toFixed(cantidadDecimales));
                $('#montoTotalMasFlete').html((total + (total * flete / 100)).toFixed(cantidadDecimales));
                */
                cargarTablaDetalle();
                // $('#tablefoottable').footable();
                $('#btnCancelAddProduct').click();




            }, error: function (detalle) {

                $("#resultadoAgregarProducto").html("Producto ya se encuentra en el detalle de la Guía de Remisión.");

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
            nuevoPrecioInicial = Number(Number($("#precioUnitarioSinIGV").val()).toFixed(cantidadDecimales));

            //Si NO es el precio estandar (si es el precio alternativo)
            if (esPrecioAlternativo == 1) {
                var nuevoPrecioInicial = Number(Number($("#precioUnitarioAlternativoSinIGV").val()).toFixed(cantidadDecimales));
            }
        }
        //En caso el calculo se realice al momento de editar un producto en la grilla
        else {
            //El precio inicial se obtiene del precio lista
            var idproducto = $('#idProducto').val();
            var nuevoPrecioInicial = $("." + idproducto + ".detprecioLista").html();
        }

        var nuevoDescuento = 100 - (nuevoPrecioModificado * 100 / nuevoPrecioInicial);
        $('#nuevoPrecio').val(nuevoPrecioModificado.toFixed(cantidadDecimales));
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
            alert("Debe seleccionar la sede MP previamente.");
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


        var contador = 0;
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {
            contador++;
        });

        if (contador > 0) {
            alert("No deben existir productos agregaados a la cotización.");
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

                alert("Ocurrió un problema al generar la cotización a partir de los precios registrados.");
                //window.location = '/Pedido/Cotizador';
            },
            success: function () {
                window.location = '/Pedido/Cotizar';
            }
        });

    });

    





    ////////CREAR/EDITAR COTIZACIÓN
    

    function crearGuiaRemision(continuarLuego) {
        if (!validarIngresoDatosObligatoriosGuiaRemision())
            return false;
        $.ajax({
            url: "/GuiaRemision/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                continuarLuego: continuarLuego
            },
            error: function (detalle) {
                alert(MENSAJE_ERROR);
            },
            success: function (resultado) {
                $("#numero").val(resultado.codigo);

                if (resultado.estado == ESTADO_APROBADA) {
                    alert("La guía de remisión número " + resultado.codigo + " fue creado correctamente.");
                    window.location = '/GuiaRemision/Index';
                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    alert("La guía de remisión número " + resultado.codigo + " fue creado correctamente, sin embargo requiere APROBACIÓN.");
                    window.location = '/GuiaRemision/Index';
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    alert("La guía de remisión " + resultado.codigo + " fue guardado correctamente para seguir editandolo posteriormente.");
                    window.location = '/GuiaRemision/Index';
                }
                else {
                    alert(MENSAJE_ERROR);
                    window.location = '/GuiaRemision/Index';
                }

            }
        });
    }

    function editarGuiaRemision(continuarLuego) {
        if (!validarIngresoDatosObligatoriosPedido())
            return false;

        $.ajax({
            url: "/Pedido/Update",
            type: 'POST',
            dataType: 'JSON',
            data: {
                continuarLuego: continuarLuego
            },
            error: function (detalle) {
                alert(MENSAJE_ERROR);
            },
            success: function (resultado) {
                $("#numero").val(resultado.codigo);

                if (resultado.estado == ESTADO_APROBADA) {
                    alert("El pedido número " + resultado.codigo + " fue editado correctamente.");
                    window.location = '/Pedido/Index';
                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    alert("El pedido número " + resultado.codigo + " fue editado correctamente, sin embargo requiere APROBACIÓN.");
                    window.location = '/Pedido/Index';
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    alert("El pedido número " + resultado.codigo + " fue guardado correctamente para seguir editandola posteriormente.");
                    window.location = '/Pedido/Index';
                }
                else {
                    alert(MENSAJE_ERROR);
                     window.location = '/Pedido/Index';
                }
            }
        });
    }



    $("#btnFinalizarCreacionGuiaRemision").click(function () {
        crearGuiaRemision(0);
    });

    $("#btnContinuarCreandoLuego").click(function () {
        crearGuiaRemision(1);
    });

    $("#btnFinalizarEdicionGuiaRemision").click(function () {
        editarGuiaRemision(0);
    });

    $("#btnContinuarEditandoLuego").click(function () {
        editarGuiaRemision(1);
    });



    function validarIngresoDatosObligatoriosGuiaRemision() {
     

        return true;
    }









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


    /*VER PEDIDO*/
    $(document).on('click', "button.btnVerGuiaRemision", function () {
        
        activarBotonesVer();
        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idPedido = arrrayClass[0];
        var numeroPedido = arrrayClass[1];
      //  $("#tableDetalleCotizacion > tbody").empty();
     

        $.ajax({
            url: "/GuiaRemision/Show",
            data: {
                idPedido: idPedido
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                alert(MENSAJE_ERROR);
            },
            success: function (resultado) {
                //var cotizacion = $.parseJSON(respuesta);
                var pedido = resultado.pedido;
                var usuario = resultado.usuario;

                idPedido
                $("#idPedido").val(pedido.idPedido);

                $("#verNumero").html(pedido.numeroPedidoString);
                $("#verNumeroGrupo").html(pedido.numeroGrupoPedidoString);
                $("#verCotizacionCodigo").html(pedido.cotizacion.numeroCotizacionString);

                $("#verFechaEntrega").html(invertirFormatoFecha(pedido.fechaEntrega.substr(0, 10)));
                $("#verFechaMaximaEntrega").html(invertirFormatoFecha(pedido.fechaMaximaEntrega.substr(0, 10)));

                $("#verCiudad").html(pedido.ciudad.nombre);
                $("#verCliente").html(pedido.cliente.razonSocial);
                $("#verNumeroReferenciaCliente").html(pedido.numeroReferenciaCliente);
                $("#verDireccionEntrega").html(pedido.direccionEntrega);
                $("#verTelefonoContactoEntrega").html(pedido.telefonoContactoEntrega);
                $("#verContactoEntrega").html(pedido.contactoEntrega);
                $("#verContactoPedido").html(pedido.contactoPedido);
                $("#verTelefonoContactoPedido").html(pedido.telefonoContactoPedido);
                $("#verFechaHoraSolicitud").html(pedido.fechaHoraSolicitud);

                $("#verEstado").html(pedido.seguimientoPedido.estadoString);
                $("#verModificadoPor").html(pedido.seguimientoPedido.usuario.nombre);
                $("#verObservacionEstado").html(pedido.seguimientoPedido.observacion);
          
                $("#verObservaciones").html(pedido.observaciones);
                $("#verMontoSubTotal").html(pedido.montoSubTotal);
                $("#verMontoIGV").html(pedido.montoIGV);
                $("#verMontoTotal").html(pedido.montoTotal);

              
                $("#tableDetalleGuia > tbody").empty();

                FooTable.init('#tableDetalleGuia');



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
                        '<td>' + lista[i].precioNeto.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].margen.toFixed(cantidadDecimales) + ' %</td>' +
                        '<td>' + lista[i].flete.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].precioUnitario.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].cantidad + '</td>' +
                        '<td>' + lista[i].subTotal.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + observacion + '</td>' +
                        '</tr>';

                }
              //  
               // sleep
                $("#tableDetalleGuia").append(d);



                /*EDITAR COTIZACIÓN*/
           /*     if (
                    cotizacion.seguimientoCotizacion.estado == ESTADO_PENDIENTE_APROBACION ||
                    cotizacion.seguimientoCotizacion.estado == ESTADO_APROBADA ||
                    cotizacion.seguimientoCotizacion.estado == ESTADO_DENEGADA ||
                    (cotizacion.seguimientoCotizacion.estado == ESTADO_EN_EDICION && usuario.idUsuario == cotizacion.seguimientoCotizacion.usuario.idUsuario)
                ) {
                    $("#btnEditarCotizacion").show();*/
                    if (pedido.seguimientoPedido.estado == ESTADO_EN_EDICION) {
                        $("#btnEditarPedido").html("Continuar Editanto");
                    }
                    else
                    {
                        $("#btnEditarPedido").html("Editar");
                    }
             /*   }
                else {
                    $("#btnEditarCotizacion").hide();
                }
                */




                /*RECOTIZAR
                if (
                    cotizacion.seguimientoCotizacion.estado == ESTADO_APROBADA ||
                    cotizacion.seguimientoCotizacion.estado == ESTADO_ACEPTADA ||
                    cotizacion.seguimientoCotizacion.estado == ESTADO_RECHAZADA
                ) {

                    $("#btnReCotizacion").show();
                }
                else {
                    $("#btnReCotizacion").hide();
                }
                */



                /*APROBAR COTIZACIÓN
                if (
                    (cotizacion.seguimientoCotizacion.estado == ESTADO_PENDIENTE_APROBACION ||
                        cotizacion.seguimientoCotizacion.estado == ESTADO_DENEGADA) &&
                    (
                        usuario.esAprobador && 
                        usuario.maximoPorcentajeDescuentoAprobacion >= cotizacion.maximoPorcentajeDescuentoPermitido)
                ) {

                    $("#btnAprobarCotizacion").show();
                }
                else {
                    $("#btnAprobarCotizacion").hide();
                }
                */


                /*DENEGAR COTIZACIÓN
                if (
                    (cotizacion.seguimientoCotizacion.estado == ESTADO_PENDIENTE_APROBACION) &&
                    (
                        usuario.esAprobador && 
                        usuario.maximoPorcentajeDescuentoAprobacion >= cotizacion.maximoPorcentajeDescuentoPermitido)
                ) {

                    $("#btnDenegarCotizacion").show();
                }
                else {
                    $("#btnDenegarCotizacion").hide();
                }*/

                /*ACEPTAR COTIZACIÓN
                if (
                    cotizacion.seguimientoCotizacion.estado == ESTADO_APROBADA ||
                        cotizacion.seguimientoCotizacion.estado == ESTADO_RECHAZADA
                ) {

                    $("#btnAceptarCotizacion").show();
                }
                else {
                    $("#btnAceptarCotizacion").hide();
                }*/


                /*RECHAZAR COTIZACIÓN
                if (
                    (cotizacion.seguimientoCotizacion.estado == ESTADO_APROBADA)
                ) {

                    $("#btnRechazarCotizacion").show();
                }
                else {
                    $("#btnRechazarCotizacion").hide();
                }
                */

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

    $("#btnCancelarGuiaRemision").click(function () {
        if (confirm(MENSAJE_CANCELAR_EDICION)) {
            window.location = '/GuiaRemision/CancelarCreacionGuiaRemision';
        }
    })



    function desactivarBotonesVer()
    {
      /*  $("#btnCancelarCotizacion").attr('disabled', 'disabled');
        $("#btnEditarCotizacion").attr('disabled', 'disabled');
        $("#btnReCotizacion").attr('disabled', 'disabled');
        $("#btnAprobarCotizacion").attr('disabled', 'disabled');
        $("#btnDenegarCotizacion").attr('disabled', 'disabled');
        $("#btnAceptarCotizacion").attr('disabled', 'disabled');
        $("#btnRechazarCotizacion").attr('disabled', 'disabled');
        $("#btnPDFCotizacion").attr('disabled', 'disabled');*/
    }

    function activarBotonesVer() {
  /*      $("#btnCancelarCotizacion").removeAttr('disabled');
        $("#btnEditarCotizacion").removeAttr('disabled');
        $("#btnReCotizacion").removeAttr('disabled');
        $("#btnAprobarCotizacion").removeAttr('disabled');
        $("#btnDenegarCotizacion").removeAttr('disabled');
        $("#btnAceptarCotizacion").removeAttr('disabled');
        $("#btnRechazarCotizacion").removeAttr('disabled');
        $("#btnPDFCotizacion").removeAttr('disabled');*/
    }

    /*
    $("#btnReCotizacion").click(function () {
        desactivarBotonesVer();
        $.ajax({
            url: "/Pedido/ConsultarSiExisteCotizacion",
            type: 'POST',
            async: false,
            success: function (resultado) {
                if (resultado == "False") {

                    var numero = $("#verNumero").html();
                    $.ajax({
                        url: "/Pedido/recotizacion",
                        data: {
                            numero: numero
                        },
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al obtener el detalle de la cotización N° " + codigo + "."); },
                        success: function (fileName) {
                            window.location = '/Pedido/Cotizar';
                        }
                    });
                }
                else {
                    alert("Existe otra cotización en curso; por favor cancele previamente esa cotización para continuar.");
                    activarBotonesVer();
                }
            }
        })
    });
    */
    /*btnEditarCotizacion desde busqueda*/

    $("#btnEditarPedido").click(function () {
        desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona
        $.ajax({
            url: "/Pedido/ConsultarSiExistePedido",
            type: 'POST',
            async: false,
            success: function (resultado) {
                if (resultado == "False") {

                    $.ajax({
                        url: "/Pedido/iniciarEdicionPedido",
                        type: 'POST',


                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del pedido."); },
                        success: function (fileName) {
                            window.location = '/Pedido/Pedir';
                        }
                    });

                    //window.location = '/Pedido/Cotizador';
                }
                else {
                    alert("Existe otro pedido en curso; por favor vaya a la pantala Pedir, haga clic en cancelar y vuelva a intentarlo.");
                    activarBotonesVer();
                }
            }
        });

        


    });



    $("#btnAtenderPedido").click(function () {
        var idPedido = $("#idPedido").val();
        //desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona
        $.ajax({
            url: "/GuiaRemision/IniciarAtencion",
            data: {
                idPedido: idPedido
            },
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                alert("Ocurrió un problema al iniciar la atención del pedido.");
                //Si se genera error se cierra la ventana modal
                $("#btnCancelarAtencion").click();
            },
            success: function (resultado) {

                var transportistaList = resultado.transportistaList;
                var guiaRemision = resultado.guiaRemision;

                $("#guiaRemision_fechaMovimiento").val(guiaRemision.fechaMovimiento);
                $("#guiaRemision_pedido_numeroPedido").val(guiaRemision.pedido.numeroPedidoString);
                $("#guiaRemision_ciudadOrigen_nombre").val(guiaRemision.ciudadOrigen.nombre);

                $('#mySelect')
                    .find('option')
                    .remove()
                    .end()
                    .val(GUID_EMPTY)
                    ;
                   // .append('<option value="' + GUID_EMPTY+'">Seleccione Transportista</option>')
                   

                $('#guiaRemision_transportista').append($('<option>', {
                    value: GUID_EMPTY,
                    text: "Nuevo Transportista",
                }));

                for (var i = 0; i < transportistaList.length; i++) {
                    $('#guiaRemision_transportista').append($('<option>', {
                        value: transportistaList[i].idTransportista,
                        text: transportistaList[i].descripcion,
                    }));

                }




                $('#motivoTraslado').val(guiaRemision.motivoTraslado);

               // window.location = '/Pedido/Pedir';
            }
        });

    });



    $("#btnAceptarAtencion").click(function () {
        desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona

        $.ajax({
            url: "/GuiaRemision/Create",
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al iniciar la atención del pedido."); },
            success: function (fileName) {
                window.location = '/Pedido/Pedir';
            }
        });

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

        $("#labelNuevoEstado").html(ESTADO_ACEPTADA_STR);
        $("#estadoId").val(ESTADO_ACEPTADA);
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
        var codigo = $("#verNumero").html();

        $.ajax({
            url: "/Pedido/updateEstadoCotizacion",
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
                alert("El estado de la cotización número: " + codigo + " se cambió correctamente.");
                //$("#btnCancelarCambioEstado").click();
                $("#btnBusqueda").click();
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
        var $modal = $('#tableDetalleGuia'),
            $editor = $('#tableDetalleGuia'),
            $editorTitle = $('#tableDetalleGuia');

     
        ft = FooTable.init('#tableDetalleGuia', {
            editing: {
                enabled: true,
                addRow: function () {
                    if (confirm(MENSAJE_CANCELAR_EDICION)) {
                        location.reload();
                    }
                },
                editRow: function (row) {
                    var values = row.val();
                    var idProducto = values.idProducto;
                    alert(idProducto);
                },
                deleteRow: function (row) {
                    var values = row.val();
                    var idProducto = values.idProducto;
                    row.delete();
                }
            }
        });
        
    }
    cargarTablaDetalle();


 

    


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

        
        /*
        var codigo = $("#numero").val();
        if (codigo == "") {
            $("#btnContinuarCreandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarCreacionGuiaRemision").attr('disabled', 'disabled');
            $("#btnCancelPedido").attr('disabled', 'disabled');
        }
        else {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarEdicionGuiaRemision").attr('disabled', 'disabled');
            $("#btnCancelPedido").attr('disabled', 'disabled');
        }
        */
        
      //  $("input[name=mostrarcodproveedor]").attr('disabled', 'disabled');


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
    
    });






    /*Evento que se dispara cuando se hace clic en FINALIZAR en la edición de la grilla*/
    $(document).on('click', "button.footable-hide", function () {

        var json = '[ ';
        var $j_object = $("td.detcantidad");

        $.each($j_object, function (key, value) {
            var arrId = value.getAttribute("class").split(" ");

            /*Se elimina control input en columna cantidad*/
            var cantidad = $("." + arrId[0] + ".detincantidad").val();
            value.innerText = cantidad;

            /*Se elimina control input en columna porcentaje descuento*/
            var porcentajeDescuento = 0;
            var margen = 0;
            var precio = 0;
            var flete = 0;
            var costo = 0;
            var observacion = "";

            json = json + '{"idProducto":"' + arrId[0] + '", "cantidad":"' + cantidad + '", "porcentajeDescuento":"' + porcentajeDescuento + '", "precio":"' + precio + '", "flete":"' + flete + '",  "costo":"' + costo + '", "observacion":"' + observacion + '"},';
        });

        json = json.substr(0, json.length - 1) + ']';

    
        
        $.ajax({
            url: "/GuiaRemision/ChangeDetalle",
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
        var precio = Number((precioLista * (100 - porcentajeDescuento) / 100).toFixed(cantidadDecimales));
        //Se asigna el precio calculculado en la columna precio
        $("." + idproducto + ".detprecio").html(precio);
        //se obtiene la cantidad
        var cantidad = Number($("." + idproducto + ".detincantidad").val());
        //Se define el precio Unitario 
        var precioUnitario = flete + precio
        $("." + idproducto + ".detprecioUnitario").html(precioUnitario.toFixed(cantidadDecimales));
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

        var costoAnterior = Number($("." + idproducto + ".detcostoAnterior").text());       
        var varcosto = (costo / costoAnterior - 1)*100;
        $("." + idproducto + ".detvarCosto").text(varcosto.toFixed(1) + " %");


        //Se actualiza el subtotal de la cotizacion

        var $j_object = $("td.detcantidad");

        var subTotal = 0;
        var igv = 0;
        var total = 0;
       
        $.each($j_object, function (key, value) {
            var arrId = value.getAttribute("class").split(" ");
            var precioUnitario = Number($("." + arrId[0] + ".detprecioUnitario").text());
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



    $("#btnBusqueda").click(function () {
        //sede MP
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val(); 

        var numeroDocumento = $("#guiaRemision_numeroDocumento").val();
        var fechaMovimientoDesde = $("#guiaRemision_fechaMovimientoDesde").val();
        var fechaMovimientoHasta = $("#guiaRemision_fechaMovimientoHasta").val();
        var estado = $("#estado").val();

        $.ajax({
            url: "/GuiaRemision/Search",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                numeroDocumento: numeroDocumento,
                fechaMovimientoDesde: fechaMovimientoDesde,
                fechaMovimientoHasta: fechaMovimientoHasta,
                estado: estado
            },
            success: function (guiaRemisionList) {

                $("#tableGuiasRemision > tbody").empty();
                //FooTable.init('#tableCotizaciones');
                $("#tableGuiasRemision").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < guiaRemisionList.length; i++) {

                    var guiaRemision = "";

                    /*
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

                    var pedido = '<tr data-expanded="true">' +
                        '<td>  ' + pedidoList[i].idPedido + '</td>' +
                        '<td>  ' + pedidoList[i].numeroPedidoString + '  </td>' +
                        '<td>  ' + pedidoList[i].numeroGrupoPedidoString + '  </td>' +
                        '<td>  ' + pedidoList[i].cliente.razonSocial + '</td>' +
                        '<td>  ' + pedidoList[i].cliente.ruc + ' </td>' +
                        '<td>  ' + pedidoList[i].ciudad.nombre + '  </td>' +
                        '<td>  ' + pedidoList[i].usuario.nombre + '  </td>' +
                        '<td>  ' + pedidoList[i].fechaHoraSolicitud + '</td>' +
                        '<td>  ' + pedidoList[i].rangoFechasEntrega + '</td>' +
                        '<td>  ' + pedidoList[i].montoTotal + '  </td>' +
                        '<td>  ' + pedidoList[i].seguimientoPedido.estadoString + '</td>' +
                        '<td>  ' + pedidoList[i].seguimientoPedido.usuario.nombre + '  </td>' +
                        '<td>  ' + observacion + '  </td>' +
                        '<td>  ' + pedidoList[i].seguimientoCrediticioPedido.estadoString + '</td>' +
                        '<td>' +
                        '<button type="button" class="' + pedidoList[i].idPedido + ' ' + pedidoList[i].numeroPedido + ' btnVerPedido btn btn-primary ">Ver</button>' +
                        '</td>' +
                        '</tr>';
                    */
                    $("#tableGuiasRemision").append(guiaRemision);
                }


                if (guiaRemisionList.length > 0)
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

    
    $("#idCiudad").change(function () {
        var idCiudad = $("#idCiudad").val();

        $.ajax({
            url: "/GuiaRemision/ChangeIdCiudad",
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

                $("#guiaRemision_ciudadOrigen_direccionPuntoPartida").val(ciudad.direccionPuntoPartida);

            }
        });
    });  


    function ConfirmDialogAtencionParcial(message) {
        $('<div></div>').appendTo('body')
            .html('<div><h6>' + message + '</h6></div>')
            .dialog({
                modal: true, title: 'Confirmación', zIndex: 10000, autoOpen: true,
                width: 'auto', resizable: false,
                buttons: {
                    Si: function () {
                        changeUltimaAtencionParcial(1);
                        $(this).dialog("close");

                    },
                    No: function () {
                        changeUltimaAtencionParcial(0);
                        $(this).dialog("close");
                    }
                },
                close: function (event, ui) {
                    $(this).remove();
                }
            });
        document.body.scrollTop = default_scrollTop;
    }

    function changeUltimaAtencionParcial(ultimaAtencionParcial) {
        if (ultimaAtencionParcial == 1) {
            $("#descripcionUltimaAtencionParcial").html("(Última Atención Parcial)");
        }
        else {
            $("#descripcionUltimaAtencionParcial").html("");
        }

        $.ajax({
            url: "/GuiaRemision/ChangeUltimaAtencionParcial",
            type: 'POST',
            data: {
                ultimaAtencionParcial: ultimaAtencionParcial
            },
            success: function () { }
        });
    }



    $("#guiaRemision_atencionParcial").change(function () {

        var atencionParcial = 1;

        if (!$('#guiaRemision_atencionParcial').prop('checked')) {
            $("#descripcionUltimaAtencionParcial").html("");
            atencionParcial = 0;
        }


        var estado = $("#guiaRemision_atencionParcial").val();
        $.ajax({
            url: "/GuiaRemision/ChangeAtencionParcial",
            type: 'POST',
            data: {
                atencionParcial: atencionParcial
            },
            success: function () { }
        });


        
        if ($('#guiaRemision_atencionParcial').prop('checked')) {
            ConfirmDialogAtencionParcial("¿Está atención parcial finaliza la atención del pedido?")
        }
        

    });

    $("#btnSaveTransportista").click(function () {

        if ($("#transportista_descripcion").val().trim() == "") {
            alert("Debe ingresar la descripción del transportista.");
            $('#transportista_descripcion').focus();
            return false;
        }

        if ($("#transportista_direccion").val().trim() == "") {
            alert("Debe ingresar la dirección del transportista.");
            $('#transportista_direccion').focus();
            return false;
        }

        if ($("#transportista_ruc").val().trim() == "") {
            alert("Debe ingresar el RUC del transportista.");
            $('#transportista_ruc').focus();
            return false;
        }

        if ($("#transportista_telefono").val().trim() == "") {
            alert("Debe ingresar el telefono del transportista.");
            $('#transportista_telefono').focus();
            return false;
        }


        

        var descripcion = $("#transportista_descripcion").val();
        var direccion = $("#transportista_direccion").val();
        var ruc = $("#transportista_ruc").val();
        var telefono = $("#transportista_telefono").val();

        $.ajax({
            url: "/GuiaRemision/CreateTransportistaTemporal",
            type: 'POST',
            dataType: 'JSON',
            data: {
                descripcion: descripcion,
                direccion: direccion,
                ruc: ruc,
                telefono: telefono
            },
            error: function (detalle) { alert("Se generó un error al intentar crear el transportista."); },
            success: function (transportista) {

                $('#guiaRemision_transportista').append($('<option>', {
                    value: transportista.idTransportista,
                    text: transportista.descripcion
                }));
                $('#guiaRemision_transportista').val(transportista.idTransportista);

                $('#guiaRemision_transportista_descripcion').val(transportista.descripcion);
                $('#guiaRemision_transportista_direccion').val(transportista.direccion);
                $('#guiaRemision_transportista_ruc').val(transportista.ruc);
                $('#guiaRemision_transportista_telefono').val(transportista.telefono);
                verificarSiExisteNuevoTransportista();
                toggleControlesTransportista();
            }
        });


        $('#btnCancelTransportista').click();

    });





});