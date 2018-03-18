



jQuery(function ($) {

    $(document).ready(function () {

        cambiarMostrarValidezOfertaEnDias();
        cargarChosenCliente();
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

    var fechaLimiteValidezOferta = $("#fechaLimiteValidezOfertaTmp").val();
    $("#fechaLimiteValidezOferta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaLimiteValidezOferta);
    
    var fechaInicioVigenciaPrecios = $("#fechaInicioVigenciaPreciosTmp").val();
    $("#fechaInicioVigenciaPrecios").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaInicioVigenciaPrecios);

    var fechaFinVigenciaPrecios = $("#fechaFinVigenciaPreciosTmp").val();
    $("#fechaFinVigenciaPrecios").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaFinVigenciaPrecios);
    
    $("#fecha").change(function () {
        var fecha = $("#fecha").val();
        $.ajax({
            url: "/Home/updateFecha",
            type: 'POST',
            data: {
                fecha: fecha
            },
            success: function () {    }
        });
    });

    $("#mostrarValidezOfertaEnDias").change(function () {
            var mostrarValidezOfertaEnDias = $("#mostrarValidezOfertaEnDias").val();
            $.ajax({
                url: "/Home/updateMostrarValidezOfertaEnDias",
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
                url: "/Home/updateValidezOfertaEnDias",
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
                url: "/Home/updateFechaLimiteValidezOferta",
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
                url: "/Home/updateFechaInicioVigenciaPrecios",
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
                url: "/Home/updateFechaFinVigenciaPrecios",
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


    function cargarChosenCliente() {

        $("#idCliente").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" }).on('chosen:showing_dropdown', function (evt, params) {
            if ($("#idCiudad").val() == "00000000-0000-0000-0000-000000000000") {
                alert("Debe seleccionar una ciudad previamente.");
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
            url: "/Home/GetClientes"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });
    }
    
    $("#idCliente").change(function () {
        $("#contacto").val("");
        var idClienteGrupo = $(this).val();

        var tipoCliente = idClienteGrupo.substr(0,1);

        idClienteGrupo = idClienteGrupo.substr(1);

        if (tipoCliente == "c") {
            $.ajax({
                url: "/Home/GetCliente",
                type: 'POST',
                dataType: 'JSON',
                data: {
                    idCliente: idClienteGrupo
                },
                success: function (cliente) {
                    $("#contacto").val(cliente.contacto);
                }
            });
        }
        else
        {
            $.ajax({
                url: "/Home/GetGrupo",
                type: 'POST',
                dataType: 'JSON',
                data: {
                    idGrupo: idClienteGrupo
                },
                success: function (grupo) {
                    $("#contacto").val(grupo.contacto);
                }
            });

        }
    });
    
    $('#modalAgregarCliente').on('shown.bs.modal', function () {

        if ($("#idCiudad").val() == "00000000-0000-0000-0000-000000000000") {
            alert("Debe seleccionar una ciudad previamente.");
            $("#idCiudad").focus();
            $('#btnCancelCliente').click();
            return false;
        }


    });
    
    /**
     * FIN CONTROLES DE CLIENTE
     */













    //POR MEJORAR:
    //Se debe obtener estos valores de la tabla de paramétros y se debe recalcular en el servidor

    var cantidadDecimales = 2;
    var IGV = 0.18;

    /**
     * ################################ INICIO CONTROLES DE AGREGAR PRODUCTO
     */

    ////////////////ABRIR AGREGAR PRODUCTO
    $('#openAgregarProducto').click(function () {

        //Para agregar un producto se debe seleccionar una ciudad
        if ($("#idCiudad").val() == "00000000-0000-0000-0000-000000000000") {
            alert("Debe seleccionar previamente una ciudad.");
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
        $("#porcentajeDescuento").val(Number(0).toFixed(4));
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
            url: "/Home/GetProductos"
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
        //$('#producto').trigger('chosen:activate');
    })


    /////////////CAMPO PRODUCTO 
    $("#producto").change(function () {
        $("#resultadoAgregarProducto").html("");
        desactivarBtnAddProduct();
        $.ajax({
            url: "/Home/GetProducto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: $(this).val(),
            },
            success: function (producto) {
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
                $("#porcentajeDescuento").val(Number(0).toFixed(4));
                $("#cantidad").val(1);

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
        if (descuento > 100) {
            descuento = 100;
        }
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


        /**
        considerarCantidades == 0 => Solo Cantidades
        considerarCantidades == 1 => Solo Observaciones
        considerarCantidades == 2 => Cantidades y Observaciones
        */
        var considerarCantidades = $("#considerarCantidades").val();
        //Controles de Cantidad
        if (considerarCantidades == "0" || considerarCantidades == "2") {
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
        if (considerarCantidades == "1" || considerarCantidades == "2") {

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

    }

    function desactivarBtnAddProduct() {
        $("#btnAddProduct").attr('disabled', 'disabled');
        $('#btnCalcularDescuento').attr('disabled', 'disabled');
    }



    $("#considerarDescontinuados").change(function () {
        var considerarDescontinuados = $('#considerarDescontinuados').prop('checked');
        $.ajax({
            url: "/Home/updateConsiderarDescontinuados",
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
            url: "/Home/AddProducto",
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


                $('.table tbody tr.footable-empty').remove();
                $(".table tbody").append('<tr data-expanded="true">' +
                    '<td>' + detalle.idProducto + '</td>' +
                    '<td>' + esPrecioAlternativo + '</td>' +

                    '<td>' + proveedor + '</td>' +
                    '<td>' + detalle.codigoProducto + '</td>' +
                    '<td>' + detalle.nombreProducto + '</td>' +
                    '<td>' + detalle.unidad + '</td>' +
                    '<td class="column-img"><img class="table-product-img" src="' + $("#imgProducto").attr("src") + '"></td>' +
                    '<td class="' + detalle.idProducto + ' detprecioLista" style="text-align:right">' + precioLista + '</td>' +
                    '<td class="' + detalle.idProducto + ' detporcentajedescuento" style="text-align:right">' + porcentajeDescuento.toFixed(4) + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detporcentajedescuentoMostrar" style="width:75px; text-align:right;">' + porcentajeDescuento.toFixed(2) + ' %</td>' +
                    '<td class="' + detalle.idProducto + ' detprecio" style="text-align:right">' + precio + '</td>' +
                    '<td class="' + detalle.idProducto + ' detcostoLista">' + costoLista + '</td>' +
                    '<td class="' + detalle.idProducto + ' detmargen" style="width:70px; text-align:right; ">' + detalle.margen + ' %</td>' +

                    '<td class="' + detalle.idProducto + ' detflete" style="text-align:right">' + flete.toFixed(2) + '</td>' +
                    '<td class="' + detalle.idProducto + ' detprecioUnitario" style="text-align:right">' + detalle.precioUnitario + '</td>' +
                    '<td class="' + detalle.idProducto + ' detcantidad" style="text-align:right">' + cantidad + '</td>' +
                    '<td class="' + detalle.idProducto + ' detsubtotal" style="text-align:right">' + subtotal + '</td>' +
                    '<td class="' + detalle.idProducto + ' detObservacion" style="text-align:right">' + observacion + '</td>' +


                    esRecotizacion +

                    '<td class="' + detalle.idProducto + ' detordenamiento"></td>' +
                    '</tr > ');

                $('.table thead tr th.footable-editing').remove();
                $('.table tbody tr td.footable-editing').remove();


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

        if ($("#ncRUC").val().trim() == "" ) {
            alert("Debe ingresar el RUC.");
            $('#ncRUC').focus();
            return false;
        }

        var razonSocial = $("#ncRazonSocial").val();
        var nombreComercial = $("#ncNombreComercial").val();
        var ruc = $("#ncRUC").val();
        var contacto = $("#ncContacto").val();

        $.ajax({
            url: "/Home/AddCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                razonSocial: razonSocial,
                nombreComercial: nombreComercial,
                ruc: ruc,
                contacto: contacto            
            },
            error: function (detalle) { alert("Se generó un error al intentar crear el cliente."); },
            success: function (resultado) {

                alert("Se creó cliente con Código Temporal: " + resultado.codigoAlterno + ".");

                location.reload();

            }
        });


        $('#btnCancelCliente').click();

    });














  

  

    ////////GUARDAR COTIZACIÓN

    $("#grabarCotizacion").click(function () {

        if ($("#idCiudad").val() == "00000000-0000-0000-0000-000000000000") {
            alert("Debe seleccionar una ciudad previamente.");
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

        var contador = 0;
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {
            contador++;
        });

        if (contador == 0) {
            alert("Debe ingresar el detalle de la cotización.");
            return false;
        }
        
        $.ajax({
            url: "/Home/grabarCotizacion",
            //contentType: 'application/pdf',
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) { alert("Se generó un error al intentar crear/actualizar la cotización."); },
            success: function (resultado) {
                $("#numero").val(resultado.codigo);

                if (resultado.estado == 1) {
                    $("#generarPDF").removeAttr('disabled');
                    if ($("#grabarCotizacion").text() == "Generar Cotización") {
                        alert("La cotización número " + resultado.codigo+" fue creada correctamente.");
                    }
                    else {
                        alert("La cotización número " + resultado.codigo + " fue actualizada correctamente.");
                    }
                    generarPDF();
                }
                else
                {
                    $("#generarPDF").attr('disabled', 'disabled');
                    if ($("#grabarCotizacion").text() == "Generar Cotización") {
                        alert("La cotización número " + resultado.codigo +" fue creada correctamente, sin embargo requiere APROBACIÓN.");
                    }
                    else {
                        alert("La cotización número " + resultado.codigo +" fue actualizada correctamente, sin embargo requiere APROBACIÓN.");
                    }
                   
                    window.location = '/Home/Index';
                }
                $("#grabarCotizacion").text('Actualizar Cotización');
               
            }
        });
    });
















    ////////VER COTIZACIÓN

    $(document).on('click', "button.btnVerCotizacion", function () {
        FooTable.init('#tableDetalleCotizacion');
        activarBotonesVer();
        var codigo = event.target.getAttribute("class").split(" ")[0];

        $.ajax({
            url: "/Home/VerCotizacion",
            data: {
                numero: codigo
            },
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al obtener el detalle de la cotización N° " + codigo + "."); },
            success: function (respuesta) {
                var cotizacion = $.parseJSON(respuesta);


                $("#verNumero").html(cotizacion.codigo);
                $("#verCiudad").html(cotizacion.ciudad.nombre);
                $("#verCliente").html(cotizacion.cliente.razonSocial);
                $("#verContacto").html(cotizacion.contacto);


                $("#verFechaCreacion").html(cotizacion.fecha.substr(0, 10));
                $("#verValidezOferta").html(cotizacion.fechaLimiteValidezOferta.substr(0, 10));
                $("#verFechaInicitoVigenciaPrecios").html(cotizacion.fechaInicioVigenciaPrecios.substr(0, 10));
                $("#verFechaFinVigenciaPrecios").html(cotizacion.fechaFinVigenciaPrecios.substr(0, 10));

                $("#verEstado").html(cotizacion.seguimientoCotizacion.estadoString);
                $("#verModificadoPor").html(cotizacion.seguimientoCotizacion.usuario.nombre);
                $("#verObservacionEstado").html(cotizacion.seguimientoCotizacion.observacion);

                if (cotizacion.considerarCantidades != "1") {
                    $("#montosTotalesDiv").show();
                }
                else
                    $("#montosTotalesDiv").hide();

                $("#verObservaciones").html(cotizacion.observaciones);
                $("#verMontoSubTotal").html(cotizacion.montoSubTotal);
                $("#verMontoIGV").html(cotizacion.montoIGV);
                $("#verMontoTotal").html(cotizacion.montoTotal);
                verMontoTotal

                var d = '';
                var lista = cotizacion.cotizacionDetalleList;
                for (var i = 0; i < cotizacion.cotizacionDetalleList.length; i++) {

                    var observacion = lista[i].observacion == null ? '' : lista[i].observacion;

                    d += '<tr>' +
                        '<td>' + lista[i].producto.proveedor + '</td>' +
                        '<td>' + lista[i].producto.sku + '</td>' +
                        '<td>' + lista[i].producto.descripcion + '</td>' +
                        '<td>' + lista[i].unidad + '</td>' +
                        '<td class="column-img"><img class="table-product-img" src="data:image/png;base64,' + lista[i].producto.image + '"> </td>' +
                        '<td>' + lista[i].porcentajeDescuentoMostrar.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].precioNeto.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].margen.toFixed(cantidadDecimales) + ' %</td>' +
                        '<td>' + lista[i].flete.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].precioUnitario.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + lista[i].cantidad + '</td>' +
                        '<td>' + lista[i].subTotal.toFixed(cantidadDecimales) + '</td>' +
                        '<td>' + observacion + '</td>' +
                        '</tr>';

                }
                $("#tableDetalleCotizacion > tbody").empty();
                $("#tableDetalleCotizacion").append(d);


                /*
              Pendiente = 0,

              Aprobada = 1,

              Denegada = 2,

              Aceptada = 3,

              Rechazada = 4*/


                /*Si está pendiente de aprobacion
                    si esta aprobada
                    si está denegada
                */
                /*EDITAR COTIZACIÓN*/
                if (
                    cotizacion.seguimientoCotizacion.estado == 0 ||
                    cotizacion.seguimientoCotizacion.estado == 1 ||
                    cotizacion.seguimientoCotizacion.estado == 2 ||
                    (cotizacion.seguimientoCotizacion.estado == 5 && cotizacion.usuario.idUsuario == cotizacion.seguimientoCotizacion.usuario.idUsuario)
                ) {

                    $("#btnEditarCotizacion").show();
                }
                else {
                    $("#btnEditarCotizacion").hide();
                }

                /*RECOTIZAR*/
                if (
                    cotizacion.seguimientoCotizacion.estado == 1 ||
                    cotizacion.seguimientoCotizacion.estado == 3
                ) {

                    $("#btnReCotizacion").show();
                }
                else {
                    $("#btnReCotizacion").hide();
                }


                /*APROBAR COTIZACIÓN*/
                if (
                    (cotizacion.seguimientoCotizacion.estado == 0 ||
                        cotizacion.seguimientoCotizacion.estado == 2) &&
                    (
                        cotizacion.usuario.maximoPorcentajeDescuentoAprobacion >= cotizacion.maximoPorcentajeDescuentoPermitido)
                ) {

                    $("#btnAprobarCotizacion").show();
                }
                else {
                    $("#btnAprobarCotizacion").hide();
                }

                /*DENEGAR COTIZACIÓN*/
                if (
                    (cotizacion.seguimientoCotizacion.estado == 0 ||
                        cotizacion.seguimientoCotizacion.estado == 1) &&
                    (
                        cotizacion.usuario.maximoPorcentajeDescuentoAprobacion >= cotizacion.maximoPorcentajeDescuentoPermitido)
                ) {

                    $("#btnDenegarCotizacion").show();
                }
                else {
                    $("#btnDenegarCotizacion").hide();
                }

                /*ACEPTAR COTIZACIÓN*/
                if (
                    (cotizacion.seguimientoCotizacion.estado == 1 ||
                        cotizacion.seguimientoCotizacion.estado == 4)
                ) {

                    $("#btnAceptarCotizacion").show();
                }
                else {
                    $("#btnAceptarCotizacion").hide();
                }


                /*RECHAZAR COTIZACIÓN*/
                if (
                    (cotizacion.seguimientoCotizacion.estado == 1)
                ) {

                    $("#btnRechazarCotizacion").show();
                }
                else {
                    $("#btnRechazarCotizacion").hide();
                }

                $("#modalVerCotizacion").modal('show');

                //  window.location = '/Home/Index';
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
            url: "/Home/GenerarPDFdesdeIdCotizacion",
            data: {
                codigo: codigo
            },
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al descargar la cotización N° " + codigo+" en formato PDF.");},
            success: function (fileName) {
                //Se descarga el PDF y luego se limpia el formulario
                window.open('/Home/DownLoadFile?fileName=' + fileName);
                window.location = '/Home/Index';
            }
        });
}
//);





    function desactivarBotonesVer()
    {
        $("#btnCancelarCotizacion").attr('disabled', 'disabled');
        $("#btnEditarCotizacion").attr('disabled', 'disabled');
        $("#btnReCotizacion").attr('disabled', 'disabled');
        $("#btnAprobarCotizacion").attr('disabled', 'disabled');
        $("#btnDenegarCotizacion").attr('disabled', 'disabled');
        $("#btnAceptarCotizacion").attr('disabled', 'disabled');
        $("#btnRechazarCotizacion").attr('disabled', 'disabled');
    }

    function activarBotonesVer() {
        $("#btnCancelarCotizacion").removeAttr('disabled');
        $("#btnEditarCotizacion").removeAttr('disabled');
        $("#btnReCotizacion").removeAttr('disabled');
        $("#btnAprobarCotizacion").removeAttr('disabled');
        $("#btnDenegarCotizacion").removeAttr('disabled');
        $("#btnAceptarCotizacion").removeAttr('disabled');
        $("#btnRechazarCotizacion").removeAttr('disabled');
    }


    $("#btnReCotizacion").click(function () {
        //$(document).on('click', "button.btnReCotizacion", function () {
        // var codigo = event.target.getAttribute("class").split(" ")[0];
        desactivarBotonesVer();
        var numero = $("#verNumero").html();
        $.ajax({
            url: "/Home/recotizacion",
            data: {
                numero: numero
            },
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al obtener el detalle de la cotización N° " + codigo + "."); },
            success: function (fileName) {
                window.location = '/Home/Cotizador';
            }
        });
    });
    
    $("#btnEditarCotizacion").click(function () {
        //$(document).on('click', "button.btnReCotizacion", function () {
        // var codigo = event.target.getAttribute("class").split(" ")[0];
        desactivarBotonesVer();
        var numero = $("#verNumero").html();
        $.ajax({
            url: "/Home/getCotizacion",
            data: {
                numero: numero
            },
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al obtener el detalle de la cotización N° " + codigo + "."); },
            success: function (fileName) {
                window.location = '/Home/Cotizador';
            }
        });
    });

    $("#btnPDFCotizacion").click(function () {
        //$(document).on('click', "button.btnReCotizacion", function () {
        // var codigo = event.target.getAttribute("class").split(" ")[0];
     //   desactivarBotonesVer();
        var numero = $("#verNumero").html();
        $.ajax({
            url: "/Home/GenerarPDFdesdeIdCotizacion",
            data: {
                codigo: numero
            },
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al descargar la cotización en formato PDF."); },         
            success: function (fileName) {
                window.location = '/Home/DownLoadFile?fileName=' + fileName;
            }
        });
    });




    
    var ft = null;



    //Mantener en Session cambio de Seleccion de IGV
    $("input[name=igv]").on("click", function () {
        var igv = $("input[name=igv]:checked").val();
        $.ajax({
            url: "/Home/updateSeleccionIGV",
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
            url: "/Home/updateSeleccionConsiderarCantidades",
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


    $("#observaciones").change(function () {

        $.ajax({
            url: "/Home/updateObservaciones",
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
            url: "/Home/updateMostrarCodigoProveedor",
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
            url: "/Home/updateCliente",
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
            url: "/Home/updateContacto",
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
        $('#montoFlete').html("Flete: S/ "+ (total * flete / 100).toFixed(cantidadDecimales));
        $('#montoTotalMasFlete').html("Total más Flete: S/ " +  (total + (total * flete / 100)).toFixed(cantidadDecimales));

        


        $.ajax({
            url: "/Home/updateFlete",
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
            url: "/Home/updateMostrarCosto",
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
        var $modal = $('#tablefoottable'),
            $editor = $('#tablefoottable'),
            $editorTitle = $('#tablefoottable');

     
        ft = FooTable.init('#tablefoottable', {
            editing: {
                enabled: true,
                addRow: function () {
                    if (confirm('¿Está seguro de no guardar los cambios?')) {
                        location.reload();
                    }
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
                                                    url: "/Home/DelProducto",
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
        $("#openAgregarProducto").attr('disabled', 'disabled');
        $("#generarPDF").attr('disabled', 'disabled');
        $("#grabarCotizacion").attr('disabled', 'disabled');
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
        if (considerarCantidades == "1") {
            /*Se agrega control input en columna observacion*/
            var $j_object = $("td.detobservacion");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var observacion = value.innerText;
                value.innerHTML = "<textarea class='" + arrId[0] + " detobservacionarea form-control'/>" + observacion + "</textarea>";
            });
        }
        else if (considerarCantidades == "2") 
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
        $("#openAgregarProducto").removeAttr('disabled');
        $("#generarPDF").removeAttr('disabled');
        $("#grabarCotizacion").removeAttr('disabled');
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
            url: "/Home/updateCotizacionDetalles",
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



    $("#btnCancelCotizacion").click(function () {
         if (confirm('¿Está seguro de no guardar los cambios?')) {
             window.location = '/Home/Index';
                    }

    })






    /*####################################################
    EVENTOS BUSQUEDA COTIZACIONES
    #####################################################*/



    $("#btnBusquedaCotizaciones").click(function () {
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val().trim();
        var fechaDesde = $("#fechaDesde").val();
        var fechaHasta = $("#fechaHasta").val();
        var numero = $("#numero").val();
        var estado = $("#estado").val();

        $.ajax({
            url: "/Home/SearchCotizaciones",
            type: 'POST',
            //   dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                fechaDesde: fechaDesde,
                fechaHasta: fechaHasta,
                numero: numero,
                estado: estado
            },
            success: function (resultado) {

                if (resultado == "0") {
                    alert("No se encontraron Cotizaciones");
                }
                location.reload();
            }
        });
    });






  
    $('#modalAprobacion').on('shown.bs.modal', function (e) {

        var $trigger = $(e.relatedTarget);
        var codigo = $trigger.data('codigo');
        var montoTotal = $trigger.data('monto');

        $("#numeroAprobacion").val(codigo);
        $("#totalAprobacion").val(montoTotal);

        mostrarMotivoRechazoDiv();
    });


    $("#accion").change(function () {

        mostrarMotivoRechazoDiv();

    });

    function mostrarMotivoRechazoDiv() {

        if ($("#accion").val() == "1") {
            $("#motivoRechazoDiv").hide();
            $("#btnAceptarAprobacionRechazo").text("Aprobar");
        }
        else {
            $("#motivoRechazoDiv").show();
            $("#btnAceptarAprobacionRechazo").text("Rechazar");
        }

    }


    $("#btnAceptarAprobacionRechazo").click(function () {

        var motivoRechazo = $("#motivoRechazo").val();
        var codigo = $("#numeroAprobacion").val();
        var accion = $("#accion").val();

        if (accion == "2") {
            if (motivoRechazo.trim() == "") {
                alert("Debe ingresar Motivo de Rechazo.");
                return false;
            }
        }

        

        
        


        $.ajax({
            url: "/Home/updateEstadoSeguimientoCotizacion",
            data: {
                codigo: codigo,
                accion: accion,
                motivoRechazo: motivoRechazo
            },
            type: 'POST',
            error: function () { alert("Ocurrió un problema al intentar aprobar la cotización."); },
            success: function () {
                if (accion == "1") {
                    alert("La cotización número "+codigo+" se aprobó correctamente.");
                }
                else {
                    alert("La cotización número "+codigo+" se rechazó correctamente.");
                }
                $("#btnBusquedaCotizaciones").click();
            }
        });
    });


});