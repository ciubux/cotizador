
jQuery(function ($) {

    

    $("#idCliente").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });

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



    /*
    if ($('.chosen-container').length > 0) {
        $('.chosen-container').on('touchstart', function (e) {
            e.stopPropagation(); e.preventDefault();
            // Trigger the mousedown event.
            $(this).trigger('mousedown');
        });
    }

    */

  

    

    $("#idCliente").change(function () {
        $("#contacto").val("");
        var idCliente = $(this).val();
        $.ajax({
            url: "/Home/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente) {
                $("#contacto").val(cliente.contacto);                
            }
        });
    });
    


    var cantidadDecimales = 2;
    var IGV = 0.18;


    $('#modalCalculadora').on('show', function () {
        $('#modalAgregarProducto').css('opacity', .5);
        $('#modalAgregarProducto').unbind();
    });
    $('#modalCalculadora').on('hidden', function () {
        $('#modalAgregarProducto').css('opacity', 1);
        $('#modalAgregarProducto').removeData("modal").modal({});
    });




    function activarBtnAddProduct() {
        $('#btnAddProduct').removeAttr('disabled');
        $('#btnCalcularDescuento').removeAttr('disabled');

    }

    function desactivarBtnAddProduct() {
        $("#btnAddProduct").attr('disabled', 'disabled');
        $('#btnCalcularDescuento').attr('disabled', 'disabled');
    }

    $('#modalAgregarProducto').on('shown.bs.modal', function () {
        $('#producto').trigger('chosen:activate');
    })

    $('#modalCalculadora').on('shown.bs.modal', function () {

        var modalAgregarProductoIsShown = ($("#modalAgregarProducto").data('bs.modal') || { isShown: false }).isShown;
        if (modalAgregarProductoIsShown)
        {
            //El precio se obtiene de la pantalla de agregar producto
            $('#nuevoPrecio').val($('#precio').val());
            $('#nuevoDescuento').val($('#porcentajeDescuento').val());
        }
        else
        {
            var idproducto = $('#idProducto').val();
            var precio = $("." + idproducto + ".detprecio").html();

            var porcentajedescuento = $("." + idproducto + ".detinporcentajedescuento").val();
            //El precio se obtiene del elemento de la grilla
            $('#nuevoPrecio').val(precio);
            $('#nuevoDescuento').val(porcentajedescuento);  
        }
        $('#nuevoPrecio').focus();

       



    })


    $('#openAgregarProducto').click(function () {

        if ($("#idCiudad").val() == "00000000-0000-0000-0000-000000000000")
        {
            alert("Debe seleccionar una ciudad previamente.");
            return false;
        }

            



        desactivarBtnAddProduct();
        $("#proveedor").val("");
        $("#unidad").html("");
        $("#imgProducto").attr("src", "images/NoDisponible.gif");

        var considerarCantidades = $("input[name=considerarCantidades]:checked").val();

        $("#precioUnitarioSinIGV").val(0);
        $("#precioUnitarioAlternativoSinIGV").val(0);

        if (considerarCantidades == "1") {
            $("#cantidad").removeAttr('disabled');
            //$("#porcentajeDescuento").removeAttr('disabled');
        }
        else {
            $("#cantidad").attr('disabled', 'disabled');
            //$("#porcentajeDescuento").attr('disabled', 'disabled');
        }

        $("#cantidad").val(considerarCantidades);
        $("#subtotal").val(0);
        $("#porcentajeDescuento").val(Number(0).toFixed(4));
        $('#valor').val(0);
        $('#valorAlternativo').val(0);

        $('#valor').attr('type', 'text');
        $('#valorAlternativo').attr('type', 'hidden');
        $('#precio').val(0);



        calcularSubtotalProducto();

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

    });



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

 //   $("#producto").chosen({ placeholder_text_single: "Buscar producto", no_results_text: "No se encontró Producto" });
/*
    $("#producto").ajaxChosen({
        dataType: "json",
        type: "GET",
        minTermLength: 5,
        afterTypeDelay: 300,
        cache: false,
        url: "/Home/GetProductos"
    }, {
            loadingImg: "Content/chosen/images/loading.gif"
        }, { placeholder_text_single: "Buscar producto", no_results_text: "No se encontró Producto" });

    */



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

                $("#unidad").html(options);
                $("#proveedor").val(producto.proveedor);
                $('#precioUnitarioSinIGV').val(producto.precioUnitarioSinIGV);
                $('#precioUnitarioAlternativoSinIGV').val(producto.precioUnitarioAlternativoSinIGV);
                $("#porcentajeDescuento").val(Number(0).toFixed(4));
                $("#cantidad").val(1);
                calcularSubtotalProducto();
                activarBtnAddProduct();
            }
        });
    });


    $("#unidad").change(function () {
        var esPrecioAlternativo = Number($("#unidad").val());
        $("#esPrecioAlternativo").val(esPrecioAlternativo);
        if (esPrecioAlternativo == 0) {
            $('#valor').attr('type', 'text');
            $('#valorAlternativo').attr('type', 'hidden');
        }
        else
        {
            $('#valor').attr('type', 'hidden');
            $('#valorAlternativo').attr('type', 'text');
        }
        calcularSubtotalProducto();
    });




    $("#nuevoPrecio").change(function () {


        var incluidoIGV = $("input[name=igv]:checked").val();
        var nuevoPrecioModificado = Number($('#nuevoPrecio').val());
        var nuevoPrecioInicial = 0;

        var modalAgregarProductoIsShown = ($("#modalAgregarProducto").data('bs.modal') || { isShown: false }).isShown;
        if (modalAgregarProductoIsShown) {

            var esPrecioAlternativo = Number($("#unidad").val());
            //Si es el precio estandar
            nuevoPrecioInicial = Number(Number($("#precioUnitarioSinIGV").val()).toFixed(cantidadDecimales));

            //Si NO es el precio estandar (si es el precio alternativo)
            if (esPrecioAlternativo == 1) {
                var nuevoPrecioInicial = Number(Number($("#precioUnitarioAlternativoSinIGV").val()).toFixed(cantidadDecimales));
            }

        }
        else
        {
            //El precio inicial se obtiene del precio lista
            var idproducto = $('#idProducto').val();
            var nuevoPrecioInicial = $("." + idproducto + ".detprecioLista").html();
        }

        var nuevoDescuento = 100 - (nuevoPrecioModificado * 100 / nuevoPrecioInicial);
        $('#nuevoPrecio').val(nuevoPrecioModificado.toFixed(cantidadDecimales));
        $('#nuevoDescuento').val(nuevoDescuento.toFixed(4));  
    });








    $("#porcentajeDescuento, #cantidad, #precio").change(function () {

        var descuento = Number($("#porcentajeDescuento").val());
        if (descuento > 100)
        {
            descuento = 100;
        }
        $("#porcentajeDescuento").val(descuento.toFixed(4));       
        $("#cantidad").val(Number($("#cantidad").val()).toFixed());
        calcularSubtotalProducto();
    });
    
    function calcularSubtotalProducto() {

        //Si es 0 quiere decir que es precio standar, si es 1 es el precio alternativo
        var esPrecioAlternativo = Number($("#unidad").val());
        var precio = 0;
        //Los precios Lista se muestran sin considerar Impuesto
        if (esPrecioAlternativo == 1)
        {
            precio = Number(Number($("#precioUnitarioAlternativoSinIGV").val()).toFixed(cantidadDecimales));
        }
        else
        {
            precio = Number(Number($("#precioUnitarioSinIGV").val()).toFixed(cantidadDecimales));
        }
        

        //Se identifica si se considera o no las cantidades y se recuperar los valores necesarios
        //para los calculos
        var considerarCantidades = $("input[name=considerarCantidades]:checked").val();
        var porcentajeDescuento = parseFloat($("#porcentajeDescuento").val());
        var incluidoIGV = $("input[name=igv]:checked").val();

        if (incluidoIGV == "1") {
            precio = precio + (precio * IGV);
        }
        $('#precioLista').val(precio.toFixed(cantidadDecimales));


        precio = precio * (100 - porcentajeDescuento) * 0.01;
        precio = precio.toFixed(cantidadDecimales);


        if (considerarCantidades == "1") {
            $("#cantidadiv").show();
            $('#subtotaldiv').show();
            var cantidad = parseInt($("#cantidad").val());
            //se calcula el subtotal
            var subTotal = precio * cantidad;
            $("#subtotal").val(subTotal.toFixed(cantidadDecimales));

        }
        else {
            $("#cantidadiv").hide();
            $('#subtotaldiv').hide();
            $("#subtotal").val(0);
            $('#cantidad').val(0);
        }
        $("#precio").val(precio);
    };





    $("#btnSaveDescuento").click(function () {

        var modalAgregarProductoIsShown = ($("#modalAgregarProducto").data('bs.modal') || { isShown: false }).isShown;
        if (modalAgregarProductoIsShown) {
            $("#porcentajeDescuento").val($("#nuevoDescuento").val());
            //Revisar si se puede comentar
            $("#nuevoPrecio").val($("#precio").val());
            calcularSubtotalProducto();
            
        }
        else
        {
            //Se recupera el precio calculado
            var precio = Number($("#nuevoPrecio").val());
            //Se asigna el precio calculculado en la columna precio
            $("." + idproducto + ".detprecio").html(precio.toFixed(cantidadDecimales));


            var idproducto = $('#idProducto').val();
            //Se asigna el descuento en el campo descuento
            $("." + idproducto + ".detinporcentajedescuento").val($("#nuevoDescuento").val());


            //se obtiene la cantidad el subtotal
            var cantidad = Number($("." + idproducto + ".detincantidad").val());

            //Se calcula el subtotal
            var subTotal = precio * cantidad;
            //Se asigna el subtotal 
            $("." + idproducto + ".detsubtotal").html(subTotal.toFixed(cantidadDecimales));

        }



        $('#btnCancelCalculadora').click();
        
    });

  
    function desactivarBtnAddProduct() {
            $("#btnAddProduct").attr('disabled', 'disabled');
    };

  






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

        if (Number($("#montoSubTotal").html()) == 0) {
            alert("Debe ingresar el detalle de la cotización.");
            return false;
        }

        
        $.ajax({
            url: "/Home/grabarCotizacion",
            //contentType: 'application/pdf',
            type: 'POST',
            error: function (detalle) { alert("Incorrecto"); },
            success: function (numeroCotizacion) {

                $("#numero").val(numeroCotizacion);
                //window.location 
            }
        });
    });




    $("#generarPDF").click(function () {
          $.ajax({
            url: "/Home/generarPDF",
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al descargar la cotización N° " + codigo+" en formato PDF.");},
            success: function (fileName) {
                window.location = '/Home/DownLoadFile?fileName=' + fileName;
            }
        });
    });


    $(document).on('click', "button.btnEditarCotizacion", function () {

        var codigo = event.target.getAttribute("class").split(" ")[0];

        $.ajax({
            url: "/Home/getCotizacion",
            data: {
                numero: codigo
            },
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al obtener el detalle de la cotización N° " + codigo +"."); },
            success: function (fileName) {
                window.location = '/Home/Index';
            }
        });
    });

    $(document).on('click', "button.btnVerCotizacion", function () {

        var codigo = event.target.getAttribute("class").split(" ")[0];
    
        $.ajax({
            url: "/Home/GenerarPDFdesdeIdCotizacion",
            data: {
                codigo: codigo
            },
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al descargar la cotización en formato PDF."); },
            success: function (fileName) {
                window.location = '/Home/DownLoadFile?fileName=' + fileName;
            }
        });
    });


    



    function validarNumero()
    {
        //^-?\d+([.]\d{1,2})?$
    }

    
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
    $("input[name=considerarCantidades]").on("click", function () {
        var considerarCantidades = $("input[name=considerarCantidades]:checked").val();
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
            url: "/Home/updateSeleccionMostrarcodproveedor",
            type: 'POST',
            data: {
                mostrarcodproveedor: mostrarcodproveedor
            },
            success: function () { }
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

    $("#fecha").change(function () {
        var fecha = $("#fecha").val();
        $.ajax({
            url: "/Home/updateFecha",
            type: 'POST',
            dataType: 'JSON',
            data: {
                fecha: fecha
            },
            success: function () { }
        });
    });


    $("#btnAddProduct").click(function ()  {
        //Se desactiva el boton mientras se agrega el producto
        desactivarBtnAddProduct();       
        var cantidad = parseInt($("#cantidad").val());
        var porcentajeDescuento = parseFloat($("#porcentajeDescuento").val());
        var precio = $("#precio").val();
        var precioLista = $("#precioLista").val();
        var esPrecioAlternativo = Number($("#unidad").val());
        var subtotal = $("#subtotal").val();
        var incluidoIGV = $("input[name=igv]:checked").val();
        var proveedor = $("#proveedor").val();

        $.ajax({
            url: "/Home/AddProducto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                cantidad: cantidad,
                porcentajeDescuento: porcentajeDescuento,
                precio: precio,
                esPrecioAlternativo: esPrecioAlternativo,
                subtotal: subtotal              
            },
            success: function (detalle) {
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
                    '<td class="' + detalle.idProducto + ' detporcentajedescuento" style="text-align:right">' + porcentajeDescuento + '%</td>' +
                    '<td class="' + detalle.idProducto + ' detprecio" style="text-align:right">' + precio + '</td>' +
                    '<td class="' + detalle.idProducto + ' " style="text-align:right">%</td>' +
                    '<td class="' + detalle.idProducto + ' detcantidad" style="text-align:right">' + cantidad + '</td>' +
                    '<td class="' + detalle.idProducto + ' detsubtotal" style="text-align:right">' + subtotal + '</td>'+
                   // '<td><div class="container-img"><div class="up"/> <div class="down" /></div></td>' +
                    '</tr > ');

                    $('.table thead tr th.footable-editing').remove();
                    $('.table tbody tr td.footable-editing').remove();


                    $('#montoIGV').html(detalle.igv);
                    $('#montoSubTotal').html(detalle.subTotal);
                    var flete = Number($("#flete").val());
                    $('#montoTotal').html(detalle.total);
                    $("#total").val(detalle.total);
                    var total = Number($("#total").val())
                    $('#montoFlete').html((total * flete / 100).toFixed(cantidadDecimales));
                    $('#montoTotalMasFlete').html((total + (total * flete / 100)).toFixed(cantidadDecimales));

                    cargarTablaDetalle()
                   // $('#tablefoottable').footable();
                    $('#btnCancelAddProduct').click();
            }, error: function (detalle) {

                $("#resultadoAgregarProducto").html("Producto ya se encuentra en la lista");

               // alert($("#resultadoAgregarProducto").html(detalle.responseText).closest("title"));

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
                        /*        $('#total').html(total);
                                location.reload();
                            },
                            error: function (result) { alert("Error al eliminar item, por favor actualice la página.")}                                
                        });*/
               //     }
                }
            }
        });
    }
    cargarTablaDetalle();

    /*Evento que se dispara cuando se hace clic en el boton EDITAR en la edición de la grilla*/
    $(document).on('click', "button.footable-show", function () {

   /*     $("#tablefoottable thead tr").append('<td>Colasas</td>');
        $("#tablefoottable tbody tr").append('<td>Colasas</td>');
        
     
        */

    //    $(".footable-editing").html("")


        //Cambiar estilos a los botones
        $("button.footable-add").attr("class","btn btn-default footable-add");
        $("button.footable-hide").attr("class", "btn btn-primary footable-hide");
        

        //Se deshabilitan controles
        $("input[name=considerarCantidades]").attr('disabled', 'disabled');
        $("input[name=igv]").attr('disabled', 'disabled');
        $("#flete").attr('disabled', 'disabled');
        $("#openAgregarProducto").attr('disabled', 'disabled');
        $("#generarPDF").attr('disabled', 'disabled');
        $("#grabarCotizacion").attr('disabled', 'disabled');




        /*Se agrega control input en columna cantidad*/
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var cantidad = value.innerText;
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detincantidad form-control' value='" + cantidad + "' type='number'/>";
        });

        /*Se agrega control input en columna porcentaje descuento*/
        var $j_object1 = $("td.detporcentajedescuento");
        $.each($j_object1, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var porcentajedescuento = value.innerText;
            porcentajedescuento = porcentajedescuento.replace("%", "").trim();

        //    value.innerHTML = "<div class='form-group'  style='width: 150px'  ><div class='col-xs-8'><input class='" + arrId[0] + " detinporcentajedescuento form-control' value='" + porcentajedescuento + "' type='number'/></div><div class='col-xs-4'> <button type='button' class='" + arrId[0] + " btnCalcularDescuento btn btn-primary bouton-image monBouton' data-toggle='modal' data-target='#modalCalculadora' ></button ></div></div>";
            value.innerHTML = "<div style='width: 150px' ><div style='float:left' ><input style='width: 100px' class='" + arrId[0] + " detinporcentajedescuento form-control' value='" + porcentajedescuento + "' type='number'/></div><div > <button type='button' class='" + arrId[0] + " btnCalcularDescuento btn btn-primary bouton-image monBouton' data-toggle='modal' data-target='#modalCalculadora' ></button ></div></div>";

        });

    });


    /*Evento que se dispara cuando se hace clic en FINALIZAR en la edición de la grilla*/
    $(document).on('click', "button.footable-hide", function () {

        //Se habilitan controles
        $("input[name=considerarCantidades]").removeAttr('disabled');
        $("input[name=igv]").removeAttr('disabled');
        $("#flete").removeAttr('disabled');
        $("#openAgregarProducto").removeAttr('disabled');
        $("#generarPDF").removeAttr('disabled');
        $("#grabarCotizacion").removeAttr('disabled');


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



            var precio = $("." + arrId[0] + ".detprecio").text();
            var subtotal = $("." + arrId[0] + ".detsubtotal").text();


            json = json + '{"idProducto":"' + arrId[0] + '", "cantidad":"' + cantidad + '", "porcentajeDescuento":"' + porcentajeDescuento + '", "precio":"' + precio +'", "subTotal":"'+subtotal+'" },' 
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

    

    function calcularSubtotalGrilla(idproducto)
    {
        //Se obtiene el porcentaje descuento 
        var porcentajeDescuento = Number($("." + idproducto + ".detinporcentajedescuento").val());
        //Se obtiene el precio lista
        var precioLista = Number($("." + idproducto + ".detprecioLista").html());
        //Se calculo el precio con descuento 
        var precio = (precioLista * (100 - porcentajeDescuento) / 100).toFixed(cantidadDecimales);
        //Se asigna el precio calculculado en la columna precio
        $("." + idproducto + ".detprecio").html(precio);
        //se obtiene la cantidad el subtotal
        var cantidad = Number($("." + idproducto + ".detincantidad").val());
        //Se calcula el subtotal
        var subTotal = precio * cantidad;
        //Se asigna el subtotal 
        $("." + idproducto + ".detsubtotal").html(subTotal.toFixed(cantidadDecimales));


        //Se actualiza el subtotal de la cotizacion

        var $j_object = $("td.detcantidad");

        var subTotal = 0;
        var igv = 0;
        var total = 0;
       
        $.each($j_object, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");

            var precio = Number($("." + arrId[0] + ".detprecio").text());
            var cantidad = Number($("." + arrId[0] + ".detincantidad").val());
            subTotal = subTotal + Number(Number((precio * cantidad)).toFixed(cantidadDecimales));
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



    $("#btnBusquedaCotizaciones").click(function () {
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val().trim();
        var fecha = $("#fecha").val();
        var numero = $("#numero").val();

        $.ajax({
            url: "/Home/SearchCotizaciones",
            type: 'POST',
            //   dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                fecha: fecha,
                numero: numero
            },
            success: function (resultado) {

                if (resultado == "0") {
                    alert("No se encontraron Cotizaciones");
                }
                location.reload();

            }
        });
    });




});