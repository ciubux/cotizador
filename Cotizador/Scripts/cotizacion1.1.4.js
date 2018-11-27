jQuery(function ($) {


   

    /*
    $("#tableCotizaciones").footable({
        "paging": {
            "enabled": true
        }
    });

    */
    //CONSTANTES:
    var cantidadDecimales = 2;
    var IGV = 0.18;
    var SIMBOLO_SOL = "S/";
    var MILISEGUNDOS_AUTOGUARDADO = 5000;
    var ESTADOS_TODOS = -1;

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


    var CANT_SOLO_OBSERVACIONES = 0;
    var CANT_SOLO_CANTIDADES = 1;
    var CANT_CANTIDADES_Y_OBSERVACIONES = 2;

    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';

    $(document).ready(function () {
        cambiarMostrarValidezOfertaEnDias();
        setTimeout(autoGuardarCotizacion, MILISEGUNDOS_AUTOGUARDADO);
        obtenerConstantes();
        cargarChosenCliente();
        verificarSiExisteDetalle();
        verificiarSiFechaEsModificada();
        verificarSiExisteCliente();
        $("#btnBusquedaCotizaciones").click();
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

    $("#btnAgregarCliente").click(function () {
        window.open(
            "/Cliente/Editar?idCliente=" + GUID_EMPTY,
            "Creación de Cliente",
            "resizable,scrollbars,status"
        );
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
            type: 'POST',
            data: {
                idGrupoCliente: idGrupoCliente
            },
            success: function () {
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

        // $("#proveedor").val(producto.proveedor);
        // $("#familia").val(producto.familia);
        $('#fleteDetalle').val(0);
        $("#costoLista").val(0);
        $("#precioLista").val(0);
        $("#tableMostrarPrecios > tbody").empty();

    }

    $('#btnOpenAgregarProducto').click(function () {

        indicarFamiliaTodas();
        indicarProveedorTodos();
        //Para agregar un producto se debe seleccionar una ciudad
        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar la sede MP previamente.");
            return false;
        }


        //$("#modalAgregarProducto").show();
     //   data - toggle="modal" data- target="#modalAgregarProducto"

        //para agregar un producto se debe seleccionar un cliente
        if ($("#idCliente").val().trim() == "") {
            alert("Debe seleccionar un cliente previamente.");
            $('#idCliente').trigger('chosen:activate');
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

        var sessionCotizacion = "cotizacion"
        if ($("#pagina").val() == "0") {
            idCliente = $("#verIdCliente").val();
            sessionCotizacion = "cotizacionVer";
        }
        else {
            idCliente = $("#idCliente").val();
            if (idCliente.trim() == "") {
                alert("Debe seleccionar un cliente.");
                $('#idCliente').trigger('chosen:activate');
                return false;
            }
        }

        $.ajax({
            url: "/Precio/GetPreciosRegistrados",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: idProducto,
                idCliente: idCliente,
                controller: sessionCotizacion
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
            url: "/Cotizacion/AddProducto",
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
                var considerarCantidades = $("#considerarCantidades").val();
                var esRecotizacion = "";
                if ($("#esRecotizacion").val() == "1") {
                    esRecotizacion = '<td class="' + detalle.idProducto + ' detprecioNetoAnterior" style="text-align:right; color: #B9371B">0.00</td>' +
                        '<td class="' + detalle.idProducto + ' detvarprecioNetoAnterior" style="text-align:right; color: #B9371B">0.0 %</td>' +
                        '<td class="' + detalle.idProducto + ' detvarCosto" style="text-align:right; color: #B9371B">0.0 %</td>' +
                        '<td class="' + detalle.idProducto + ' detcostoAnterior" style="text-align:right; color: #B9371B">0.0</td>';
                }

                var observacionesEnDescripcion = "";
                if (considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES)
                {
                    observacionesEnDescripcion = "<br /><span class='" + detalle.idProducto + " detproductoObservacion'  style='color: darkred'>" + detalle.observacion + "</span>";
                }
            
                $('#tableDetalleCotizacion tbody tr.footable-empty').remove();


                $("#tableDetalleCotizacion tbody").append('<tr data-expanded="false">' +
                    '<td>' + detalle.idProducto + '</td>' +
                    '<td>' + esPrecioAlternativo + '</td>' +
                    '<td>' + proveedor + '</td>' +
                    '<td>' + detalle.codigoProducto + '</td>' +
                    '<td>' + detalle.nombreProducto + observacionesEnDescripcion +'</td>' +
                    '<td>' + detalle.unidad + '</td>' +
                    '<td class="column-img"><img class="table-product-img" src="' + $("#imgProducto").attr("src") + '"></td>' +
                    '<td class="' + detalle.idProducto + ' detprecioLista" style="text-align:right">' + precioLista + '</td>' +
                    '<td class="' + detalle.idProducto + ' detporcentajedescuento" style="text-align:right">' + porcentajeDescuento.toFixed(4) + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detporcentajedescuentoMostrar" style="width:75px; text-align:right;">' + porcentajeDescuento.toFixed(1) + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detprecio" style="text-align:right">' + precio + '</td>' +
                    '<td class="' + detalle.idProducto + ' detcostoLista">' + costoLista + '</td>' +
                    '<td class="' + detalle.idProducto + ' detmargen" style="width:70px; text-align:right; ">' + detalle.margen + ' %</td>' +

                    '<td class="' + detalle.idProducto + ' detflete" style="text-align:right">' + flete.toFixed(2) + '</td>' +
                    '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right">' + detalle.precioUnitario + '</td>' +
                    '<td class="' + detalle.idProducto + ' detcantidad" style="text-align:right">' + cantidad + '</td>' +
                    '<td class="' + detalle.idProducto + ' detsubtotal" style="text-align:right">' + subtotal + '</td>' +
                    '<td class="' + detalle.idProducto + ' detobservacion" style="text-align:left">' + observacion + '</td>' +
                    '<td class="' + detalle.idProducto + ' detbtnMostrarPrecios"> <button  type="button" class="' + detalle.idProducto+' btnMostrarPrecios btn btn-primary bouton-image botonPrecios"></button></td>'+

                    esRecotizacion +

                    '<td class="' + detalle.idProducto + ' detordenamiento"></td>' +
                    '</tr > ');

                $('#tableDetalleCotizacion thead tr th.footable-editing').remove();
                $('#tableDetalleCotizacion tbody tr td.footable-editing').remove();


                $('#montoIGV').html(detalle.igv);
                $('#montoSubTotal').html(detalle.subTotal);
                ///var flete = Number($("#flete").val());
                $('#montoTotal').html(detalle.total);
                $("#total").val(detalle.total);
                var total = Number($("#total").val())
                $('#montoFlete').html((total * flete / 100).toFixed(cantidadDecimales));
                $('#montoTotalMasFlete').html((total + (total * flete / 100)).toFixed(cantidadDecimales));

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







    /**
    * INTERFACE PARA CREAR CLIENTE
    */

    $("#btnSaveCliente").click(function () {

        //ncRazonSocial

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
                controller: "cotizacion"
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




    ////////////GENERAR PLANTILLA DE COTIZACIÓN



    $("#btnAgregarProductosDesdePreciosRegistrados").click(function () {

        var idCiudad = $("#idCiudad").val();
        if (idCiudad == "" || $("#idCiudad").val() == null) {
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


        if (verificarSiExisteDetalle()) {
            alert("No deben existir productos agregados a la cotización.");
            return false;
        }

        indicarFamiliaTodas();
        indicarProveedorTodos();

    });


    $("#btnObtenerProductos").click(function () {
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val();
        var fecha = $("#fechaPrecios").val();
        var familia = $("#familiaBusquedaPrecios").val();
        var proveedor = $("#proveedorBusquedaPrecios").val();


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

                alert("Ocurrió un error al armar el detalle del pedido a partir de los precios registrados.");
                //window.location = '/Cotizacion/Cotizador';
            },
            success: function () {
                window.location = '/Cotizacion/Cotizar';
            }
        });

    });

    





    ////////CREAR/EDITAR COTIZACIÓN


    function validarIngresoDatosObligatoriosCotizacion() {
        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar la sede MP previamente.");
            $("#idCiudad").focus();
            return false;
        }

        if ($("#idCliente").val().trim() == "") {
            alert("Debe seleccionar un cliente.");
            $('#idCliente').trigger('chosen:activate');
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
            }
            else
            {  //Si no está vacía no puede ser menor a la fecha de inicio de vigencia
                if (convertirFechaNumero(fechaFinVigenciaPrecios) < convertirFechaNumero(fecha)) {
                    alert("El fin de vigencia de precios debe ser mayor o igual a la fecha de la cotización.");
                    $("#fechaFinVigenciaPrecios").focus();
                    return false;
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

        var sedePrincipal = parseInt($('#clienteSedePrincipal').val());

        if (continuarLuego == 0 && sedePrincipal == 1) {
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
                        text: 'NO',
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
                alert("Se generó un error al intentar finalizar la creación de la cotización. Si estuvo actualizando, vuelva a buscar la cotización, es posible que este siendo modificada por otro usuario.");
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

        if (continuarLuego == 0 && sedePrincipal == 1) {
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
                        text: 'NO',
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
                alert("Se generó un error al intentar finalizar la edición de la cotización. Si estuvo actualizando, vuelva a buscar la cotización, es posible que este siendo modificada por otro usuario.");
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













    $("#btnCopiar").click(function () {
        /* Get the text field */
        var copyText = document.getElementById("myInput");

        /* Select the text field */
        copyText.select();

        /* Copy the text inside the text field */
        document.execCommand("Copy");

        /* alert the copied text */
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

    $(document).on('click', "button.btnVerCotizacion", function () {

        $('body').loadingModal({
            text: 'Abriendo Cotización...'
        });
        $('body').loadingModal('show');

        activarBotonesVer();
        var codigo = event.target.getAttribute("class").split(" ")[0];
      //  $("#tableDetalleCotizacion > tbody").empty();
     

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

                $("#verCondicionesPago").html(cotizacion.textoCondicionesPago);
                
                $("#verIdCotizacion").val(cotizacion.idCotizacion);
                $("#verIdCliente").val(cotizacion.cliente.idCliente);

                $("#verNumero").html(cotizacion.codigo);
                
                $("#verCiudad").html(cotizacion.ciudad.nombre);
                $("#verCliente").html(cotizacion.cliente.razonSocial);
                $("#verContacto").html(cotizacion.contacto);


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
                
                $("#verEstado").html(cotizacion.seguimientoCotizacion.estadoString);
                $("#verModificadoPor").html(cotizacion.seguimientoCotizacion.usuario.nombre);
                $("#verObservacionEstado").html(cotizacion.seguimientoCotizacion.observacion);

                if (cotizacion.considerarCantidades != CANT_SOLO_OBSERVACIONES) {
                    $("#montosTotalesDiv").show();
                }
                else
                    $("#montosTotalesDiv").hide();

                $("#verObservaciones").html(cotizacion.observaciones);
                if (cotizacion.aplicaSedes === true) {
                    $("#verSedesAplica").html("Esta cotización aplicará también para las sedes: " + cotizacion.cliente.sedeListWebString.replace(new RegExp('<br>', 'g'), ', '));
                }
                $("#verMontoSubTotal").html(cotizacion.montoSubTotal);
                $("#verMontoIGV").html(cotizacion.montoIGV);
                $("#verMontoTotal").html(cotizacion.montoTotal);


                $("#tableDetalleCotizacion > tbody").empty();

                FooTable.init('#tableDetalleCotizacion');



                var d = '';
                var lista = cotizacion.cotizacionDetalleList;
                for (var i = 0; i < cotizacion.cotizacionDetalleList.length; i++) {

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
                        '<td>' + lista[i].producto.costoLista.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].margen.toFixed(cantidadDecimales) + ' %</td>' +
                        '<td>' + lista[i].flete.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].precioUnitario.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].cantidad + '</td>' +
                        '<td>' + lista[i].subTotal.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + observacion + '</td>' +
                        '<td class="' + lista[i].producto.idProducto + ' detbtnMostrarPrecios"> <button  type="button" class="' + lista[i].producto.idProducto + ' btnMostrarPrecios btn btn-primary bouton-image botonPrecios"></button></td>' +
                        '</tr>';

                }
              //  
               // sleep
                $("#tableDetalleCotizacion").append(d);



                /*EDITAR COTIZACIÓN*/
                if (
                    cotizacion.seguimientoCotizacion.estado == ESTADO_PENDIENTE_APROBACION ||
                    cotizacion.seguimientoCotizacion.estado == ESTADO_APROBADA ||
                    cotizacion.seguimientoCotizacion.estado == ESTADO_DENEGADA ||
                    (cotizacion.seguimientoCotizacion.estado == ESTADO_EN_EDICION && usuario.idUsuario == cotizacion.seguimientoCotizacion.usuario.idUsuario)
                ) {
                    $("#btnEditarCotizacion").show();
                    if (cotizacion.seguimientoCotizacion.estado == ESTADO_EN_EDICION) {
                        $("#btnEditarCotizacion").html("Continuar Editando");
                    }
                    else
                    {
                        $("#btnEditarCotizacion").html("Editar");
                    }
                }
                else {
                    $("#btnEditarCotizacion").hide();
                }





                /*RECOTIZAR*/
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




                /*APROBAR COTIZACIÓN*/
                if (
                    (cotizacion.seguimientoCotizacion.estado == ESTADO_PENDIENTE_APROBACION ||
                        cotizacion.seguimientoCotizacion.estado == ESTADO_DENEGADA) &&
                    (
                        usuario.apruebaCotizaciones && 
                        usuario.maximoPorcentajeDescuentoAprobacion >= cotizacion.maximoPorcentajeDescuentoPermitido)
                ) {

                    $("#btnAprobarCotizacion").show();
                }
                else {
                    $("#btnAprobarCotizacion").hide();
                }



                /*DENEGAR COTIZACIÓN*/
                if (
                    (cotizacion.seguimientoCotizacion.estado == ESTADO_PENDIENTE_APROBACION) &&
                    (
                        usuario.apruebaCotizaciones && 
                        usuario.maximoPorcentajeDescuentoAprobacion >= cotizacion.maximoPorcentajeDescuentoPermitido)
                ) {

                    $("#btnDenegarCotizacion").show();
                }
                else {
                    $("#btnDenegarCotizacion").hide();
                }

                /*ACEPTAR COTIZACIÓN*/
                if (
                    cotizacion.seguimientoCotizacion.estado == ESTADO_APROBADA ||
                        cotizacion.seguimientoCotizacion.estado == ESTADO_RECHAZADA
                ) {

                    $("#btnAceptarCotizacion").show();
                }
                else {
                    $("#btnAceptarCotizacion").hide();
                }


                /*RECHAZAR COTIZACIÓN*/
                if (
                    (cotizacion.seguimientoCotizacion.estado == ESTADO_APROBADA)
                ) {

                    $("#btnRechazarCotizacion").show();
                }
                else {
                    $("#btnRechazarCotizacion").hide();
                }


                /*PDF*/
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

                if (
                    cotizacion.seguimientoCotizacion.estado == ESTADO_ACEPTADA 
                ) {

                    $("#btnGenerarPedido").show();
                }
                else {
                    $("#btnGenerarPedido").hide();
                }

                /*Eliminar Cotizacion*/
                if (
                    cotizacion.seguimientoCotizacion.estado != ESTADO_ACEPTADA &&
                    cotizacion.seguimientoCotizacion.estado != ESTADO_RECHAZADA &&
                    cotizacion.seguimientoCotizacion.estado != ESTADO_ELIMINADA
                ) {

                    $("#btnEliminarCotizacion").show();
                }
                else {
                    $("#btnEliminarCotizacion").hide();
                }



                $("#modalVerCotizacion").modal('show');

                //  window.location = '/Cotizacion/Index';
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
        $("#btnReCotizacion").attr('disabled', 'disabled');
        $("#btnAprobarCotizacion").attr('disabled', 'disabled');
        $("#btnDenegarCotizacion").attr('disabled', 'disabled');
        $("#btnAceptarCotizacion").attr('disabled', 'disabled');
        $("#btnRechazarCotizacion").attr('disabled', 'disabled');
        $("#btnEliminarCotizacion").attr('disabled', 'disabled');
        $("#btnPDFCotizacion").attr('disabled', 'disabled');
        $("btnGenerarPedido").attr('disabled', 'disabled');
    }

    function activarBotonesVer() {
        $("#btnCancelarCotizacion").removeAttr('disabled');
        $("#btnEditarCotizacion").removeAttr('disabled');
        $("#btnReCotizacion").removeAttr('disabled');
        $("#btnAprobarCotizacion").removeAttr('disabled');
        $("#btnDenegarCotizacion").removeAttr('disabled');
        $("#btnAceptarCotizacion").removeAttr('disabled');
        $("#btnRechazarCotizacion").removeAttr('disabled');
        $("#btnPDFCotizacion").removeAttr('disabled');
        $("#btnEliminarCotizacion").removeAttr('disabled');
        $("btnGenerarPedido").removeAttr('disabled');
    }


    $("#btnReCotizacion").click(function () {
        desactivarBotonesVer();
        $.ajax({
            url: "/Cotizacion/ConsultarSiExisteCotizacion",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            success: function (resultado) {
                if (resultado.existe == "false") {

                    var numero = $("#verNumero").html();
                    $.ajax({
                        url: "/Cotizacion/recotizacion",
                        data: {
                            numero: numero
                        },
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al obtener el detalle de la cotización N° " + codigo + "."); },
                        success: function (fileName) {
                            window.location = '/Cotizacion/Cotizar';
                        }
                    });
                }
                else {
                    mostrarMensajeCotizacionEnCurso(resultado);
                }
            }
        })
    });

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

    function mostrarMensajeCotizacionEnCurso(resultado) {
        if (resultado.numero == 0) {
            alert('Está creando una nueva cotización; para continuar por favor diríjase a la página "Crear/Modificar Cotización" y luego haga clic en el botón Cancelar, Finalizar Creación o Guardar (si elige Guardar indique No cuando se le consulte si desea continuar editanto ahora).');
        }
        else {
            if (resultado.numero == $("#verNumero").html())
                alert('Ya se encuentra editando la cotización número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Cotización".');
            else
                alert('Está editando la cotización número ' + resultado.numero + '; para continuar por favor dirigase a la página "Crear/Modificar Cotización" y luego haga clic en el botón Cancelar, Finalizar Edición o Guardar (si elige Guardar indique No cuando se le consulte si desea continuar editanto ahora).');
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
                        if (resultado.numero == $("#verNumero").html())
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
        var numero = $("#verNumero").html();
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
        var codigo = $("#verNumero").html();

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
                $("#historial_titulo_numero_cotizacion").html($("#verNumero").html());
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


        $("#btnBusquedaCotizaciones").attr("disabled","disabled");
        $.ajax({
            url: "/Cotizacion/SearchCotizaciones",
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

                    var observacion = cotizacionList[i].seguimientoCotizacion.observacion == null ? "" : cotizacionList[i].seguimientoCotizacion.observacion;

                    if (cotizacionList[i].seguimientoCotizacion.observacion != null && cotizacionList[i].seguimientoCotizacion.observacion.length > 20) {
                        var idComentarioCorto = cotizacionList[i].idCotizacion + "corto";
                        var idComentarioLargo = cotizacionList[i].idCotizacion + "largo";
                        var idVerMas = cotizacionList[i].idCotizacion + "verMas";
                        var idVermenos = cotizacionList[i].idCotizacion + "verMenos";
                        var comentario = cotizacionList[i].seguimientoCotizacion.observacion.substr(0, 20) + "...";

                        observacion = '<div id="'+idComentarioCorto+'" style="display:block;">'+comentario+'</div>'+
                        '<div id="' + idComentarioLargo + '" style="display:none;">' + cotizacionList[i].seguimientoCotizacion.observacion+'</div>'+
                            '<p><a id="' + idVerMas + '" class="' + cotizacionList[i].idCotizacion+' verMas" href="javascript:mostrar();" style="display:block">Ver Más</a></p>'+
                        '<p><a id="' + idVermenos + '" class="' + cotizacionList[i].idCotizacion+' verMenos" href="javascript:mostrar();" style="display:none">Ver Menos</a></p>';
                     }
         

                    var cotizacion = '<tr data-expanded="false">' +
                        '<td>' + cotizacionList[i].idCotizacion + '</td>' +
                        '<td>' + cotizacionList[i].codigo + '</td>' +
                        '<td>' + cotizacionList[i].usuario.nombre + '</td>' +
                        //ToString("dd/MM/yyyy")
                        '<td>' + invertirFormatoFecha(cotizacionList[i].fecha.substr(0, 10)) + '</td>' +
                        '<td>' + cotizacionList[i].cliente.razonSocial + '</td>' +
                        '<td>' + cotizacionList[i].cliente.ruc + '</td>' +
                        '<td>' + cotizacionList[i].ciudad.nombre + '</td>' +
                        '<td>' + Number(cotizacionList[i].montoSubTotal).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(cotizacionList[i].montoIGV).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(cotizacionList[i].montoTotal).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(cotizacionList[i].maximoPorcentajeDescuentoPermitido).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + Number(cotizacionList[i].minimoMargen).toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + cotizacionList[i].seguimientoCotizacion.estadoString + '</td>' +
                        '<td>' + cotizacionList[i].seguimientoCotizacion.usuario.nombre + '</td>' +
                        '<td>' + observacion + '</td>' +
                        '<td><button type="button" class="' + cotizacionList[i].codigo + ' btnVerCotizacion btn btn-primary ">Ver</button></td></tr>';
                    //ToString("dd/MM/yyyy")
                    $("#tableCotizaciones").append(cotizacion);
                    /*+
                        '<td>' + cotizacionList[i].idCotizacion + '</td>' +
                        '<td>' + cotizacionList[i].codigo + '</td>' +
                        '<td>' + cotizacionList[i].usuario.nombre + '</td>' +
                    
                        '<td>' + cotizacionList[i].fecha + '</td>' +
                        '<td>' + cotizacionList[i].cliente.razonSocial + '</td>' +
                        '<td>' + cotizacionList[i].cliente.ruc + '</td>' +
                        '<td>' + cotizacionList[i].ciudad.nombre + '</td>' +
                        '<td>' + cotizacionList[i].montoSubTotal + '</td>' +
                        '<td>' + cotizacionList[i].montoIGV + '</td>' +
                        '<td>' + cotizacionList[i].montoTotal + '</td>' +
                        '<td>' + cotizacionList[i].maximoPorcentajeDescuentoPermitido + '</td>' +
                        '<td>' + cotizacionList[i].seguimientoCotizacion.estadoString + '</td>' +
                        '<td>' + cotizacionList[i].seguimientoCotizacion.usuario.nombre + '</td>' +
                        '<td>asas</td>' +
                        '<td><button type="button" class="' + cotizacionList[i].codigo + ' btnVerCotizacion btn btn-primary ">Ver</button></td></tr>');
                    */
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