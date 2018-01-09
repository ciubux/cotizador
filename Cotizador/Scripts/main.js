
jQuery(function ($) {


    $("#idCliente").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });

    $("#idCliente").ajaxChosen({
        dataType: "json",
        type: "GET",
        minTermLength: 3,
        afterTypeDelay: 300,
        cache: false,
        url: "/Home/GetClientes"
    }, {
            loadingImg: "Content/chosen/images/loading.gif"
        }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });

  /*  if ($('.chosen-container').length > 0) {
        $('.chosen-container').on('touchstart', function (e) {
            e.stopPropagation(); e.preventDefault();
            // Trigger the mousedown event.
            $(this).trigger('mousedown');
        });
    }*/

  

    

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
        $('#nuevoPrecio').focus();
        $('#nuevoPrecio').val($('#precio').val());
        $('#nuevoDescuento').val($('#porcentajeDescuento').val());
    })


    $('#openAgregarProducto').click(function () {

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
            minTermLength: 3,
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

    $("#producto").chosen({ placeholder_text_single: "Buscar producto", no_results_text: "No se encontró Producto" });

    $("#producto").ajaxChosen({
        dataType: "json",
        type: "GET",
        minTermLength: 3,
        afterTypeDelay: 300,
        cache: false,
        url: "/Home/GetProductos"
    }, {
            loadingImg: "Content/chosen/images/loading.gif"
        }, { placeholder_text_single: "Buscar producto", no_results_text: "No se encontró Producto" });





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
                //Se setea el precio estandar
                $('#precio').val(producto.precio);


                //por defecto valor aparece visible
                $('#valor').val(producto.valor);
                $('#valor').attr('type', 'text');
                //por defecto valor alternativo 
                $('#valorAlternativo').val(producto.valorAlternativo);
                $('#valorAlternativo').attr('type', 'hidden');

                
                


                var considerarCantidades = $("input[name=considerarCantidades]:checked").val();
                $("#cantidad").val(considerarCantidades);
                if (considerarCantidades == "1") {
                    $("#subtotal").val(producto.precio);
                }
                else {
                    $("#subtotal").val(0);
                }
                $("#porcentajeDescuento").val(Number(0).toFixed(4));

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

        var esPrecioAlternativo = Number($("#unidad").val());
        //Si es el precio estandar
        var nuevoPrecioInicial = Number(Number($("#precioUnitarioSinIGV").val()).toFixed(cantidadDecimales));

        //Si NO es el precio estandar (si es el precio alternativo)
        if (esPrecioAlternativo == 1) {
            var nuevoPrecioInicial = Number(Number($("#precioUnitarioAlternativoSinIGV").val()).toFixed(cantidadDecimales));
        }
                
        var incluidoIGV = $("input[name=igv]:checked").val();
        var nuevoPrecioModificado = Number($('#nuevoPrecio').val());

        if (incluidoIGV == "1") {
            nuevoPrecioInicial = nuevoPrecioInicial + (nuevoPrecioInicial * IGV);
        }

        var nuevoDescuento = 100 -( nuevoPrecioModificado * 100 / nuevoPrecioInicial);

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
        var esPrecioAlternativo = Number($("#unidad").val());
        //Si es el precio estandar
        var precioUnitarioSinIGV = Number(Number($("#precioUnitarioSinIGV").val()).toFixed(cantidadDecimales));

        //Si NO es el precio estandar (si es el precio alternativo)
        if (esPrecioAlternativo == 1) {
            var precioUnitarioSinIGV = Number(Number($("#precioUnitarioAlternativoSinIGV").val()).toFixed(cantidadDecimales));
        }
        
        var considerarCantidades = $("input[name=considerarCantidades]:checked").val();
        var incluidoIGV = $("input[name=igv]:checked").val();
        var precio = precioUnitarioSinIGV;
        var porcentajeDescuento = parseFloat($("#porcentajeDescuento").val());
        if (considerarCantidades == "1") {
            var cantidad = parseInt($("#cantidad").val());
            

            if (incluidoIGV == "1") {
                precio = precio + (precio * IGV);
            }
            precio = precio * (100 - porcentajeDescuento) * 0.01;
            precio = precio.toFixed(cantidadDecimales);


            var subTotal = precio * cantidad;

    
            $("#subtotal").val(subTotal.toFixed(cantidadDecimales));
        }
        else
        {
            if (incluidoIGV == "1") {
                precio = precio + (precio * IGV);
            }

            precio = precio * (100 - porcentajeDescuento) * 0.01;
            precio = precio.toFixed(cantidadDecimales);
           

        }
        $("#precio").val(precio);


    };


    $("#btnAddProduct").click(function () {
        addProducto();
    });


    $("#btnSaveDescuento").click(function () {
        $("#porcentajeDescuento").val($("#nuevoDescuento").val());
        $("#nuevoPrecio").val($("#precio").val());
        calcularSubtotalProducto();
        $('#btnCancelCalculadora').click();
    });


    $("#generarPDF").click(function () {

        $.ajax({
            url: "/Home/generarPDF",
            type: 'POST',
            data: {
                fecha: $("#fecha").val(),
                idCiudad: $("#idCiudad").val(),
                cliente: $("#cliente").val(),
                contacto: $("#contacto").val(),
                flete: $("#flete").val(),
                igv: $("input[name=igv]:checked").val(),
                considerarCantidades: $("input[name=considerarCantidades]:checked").val(),
                mostrarcodproveedor: $("input[name=mostrarcodproveedor]:checked").val(),

                montoSubTotal: $("#montoSubTotal").html(),
                montoIGV: $("#montoIGV").html(),
                montoTotal: $("#montoTotal").html(),
                montoFlete: $("#montoFlete").html(),
                montoTotalMasFlete: $("#montoTotalMasFlete").html(),
                observaciones: $("#observaciones").html()
            },
            success: function () {
               // row.delete();
            }
        });
    });
    


    function validarNumero()
    {
        //^-?\d+([.]\d{1,2})?$
    }

    
    var ft = null;


    function cargarTablaDetalle() {
        var $modal = $('#tablefoottable'),
            $editor = $('#tablefoottable'),
            $editorTitle = $('#tablefoottable');
            ft = FooTable.init('#tablefoottable', {
                editing: {
                    enabled: true,
                    addRow: function () { },
                    editRow: function (row) { },
                    deleteRow: function (row) {
                        if (confirm('¿Esta seguro de eliminar el producto?')) {
                            var values = row.val();
                            var idProducto = values.idProducto;

                            $.ajax({
                                url: "/Home/DelProducto",
                                type: 'POST',
                                data: {
                                    idProducto: idProducto
                                },
                                success: function (total) {

                                    row.delete();
                                    $('#total').html(total);
                                    location.reload();
                                },
                                error: function (result) { alert("Error al eliminar item, por favor actualice la página.")}                                
                            });
                        }
                    }
                }
            });
    }

    cargarTablaDetalle();


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
            dataType: 'JSON',
            data: {
                flete: flete
            },
            success: function () { }
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



    function addProducto() {

        desactivarBtnAddProduct();       
        var cantidad = parseInt($("#cantidad").val());
        var porcentajeDescuento = parseFloat($("#porcentajeDescuento").val());
        var precio = $("#precio").val();
        var precioUnitarioSinIGV = $("#precioUnitarioSinIGV").val();
        var precioUnitarioAlternativoSinIGV = $("#precioUnitarioAlternativoSinIGV").val();
        var esPrecioAlternativo = Number($("#unidad").val());
        var proveedor = $("#proveedor").val();
        var producto = $("#producto option:selected").val();
        var subtotal = $("#subtotal").val();
        var valor = $("#valor").val();
        var valorAlternativo = $("#valorAlternativo").val();
        var incluidoIGV = $("input[name=igv]:checked").val();

        $.ajax({
            url: "/Home/AddProducto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                cantidad: cantidad,
                porcentajeDescuento: porcentajeDescuento,
                precioUnitarioSinIGV: precioUnitarioSinIGV,
                precioUnitarioAlternativoSinIGV: precioUnitarioAlternativoSinIGV,
                precio: precio,
                subtotal: subtotal,
                valor: valor,
                valorAlternativo: valorAlternativo,
                esPrecioAlternativo: esPrecioAlternativo
            },
            success: function (detalle) {
                $('.table tbody tr.footable-empty').remove();
                var precioLista = 0;
                
                if (esPrecioAlternativo == 0) {
                    precioLista = precioUnitarioSinIGV;
                }
                else {
                    precioLista = precioUnitarioAlternativoSinIGV;
                }

                if (incluidoIGV == 1) {
                    precioLista = precioLista * IGV;
                }
     
                $(".table tbody").append('<tr data-expanded="true">' +
                    '<td>' + detalle.idProducto + '</td>' +
                    '<td>' + detalle.esPrecioAlternativo + '</td>' +
                    '<td>' + detalle.proveedor + '</td>' +
                    '<td>' + detalle.codigoProducto + '</td>' +
                    '<td>' + detalle.nombreProducto + '</td>' +
                    '<td>' + detalle.unidad + '</td>' +
                    '<td class="column-img"><img class="table-product-img" src="' + $("#imgProducto").attr("src") + '"></td>' +
                    '<td>' + precioLista + '</td>' + 
                    '<td>' + detalle.porcentajeDescuento + '%</td>' +
                    '<td>' + detalle.precio + '</td>' +
                    '<td>' + detalle.cantidad + '</td>' +
                    '<td>' + detalle.subTotal + '</td></tr > ');

                    $('.table thead tr th.footable-editing').remove();
                    $('.table tbody tr td.footable-editing').remove();


                    $('#montoIGV').html(detalle.igv);
                    $('#montoSubTotal').html(detalle.subTotal2);
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

    };

});