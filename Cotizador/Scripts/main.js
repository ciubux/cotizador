/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

jQuery(function ($) {
  /*  $('#modal-content').on('shown.bs.modal', function () {
        // $("#txtname").focus();
        alert("asas");
    })

    $('#modal-content').modal({
        show: true
    });*/

    $('#openBtn').click(function () {
        $('#proveedor').focus();
        $('#proveedor').val(0);
        $('#categoria').val(0);
        $('#familia').val(0);
        $('#imgProducto').attr("src", "images/NoDisponible.gif");
        

        $("#producto").chosen({ placeholder_text_single: "Seleccione el producto", no_results_text: "No existen coincidencias" });

        $("#producto").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 3,
            afterTypeDelay: 300,
            cache: false,
            url: "/Home/GetProductos"
        }, {
            loadingImg: "Content/chosen/images/loading.gif"
        }, { placeholder_text_single: "Seleccione el producto", no_results_text: "No existen coincidencias" });


    /*    $('#modal-content').modal({
            show: true
        });*/
    });

    $.datepicker.regional['es'] = {
        closeText: 'Cerrar',
        prevText: '< Ant',
        nextText: 'Sig >',
        currentText: 'Hoy',
        monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
        monthNamesShort: ['Ene','Feb','Mar','Abr', 'May','Jun','Jul','Ago','Sep', 'Oct','Nov','Dic'],
        dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
        dayNamesShort: ['Dom','Lun','Mar','Mié','Juv','Vie','Sáb'],
        dayNamesMin: ['Do','Lu','Ma','Mi','Ju','Vi','Sá'],
        weekHeader: 'Sm',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
        };
    $.datepicker.setDefaults($.datepicker.regional['es']);
    
    
    $('.table').footable();
    
    $("#fecha").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", new Date());

    $("#categoria").change(function () {
        //alert(1);
        $.ajax({
            url: "/Home/GetFamilias",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCategoria: $(this).val(),
            },
            success: function (res) {
                var selects = '<option selected>Seleccione...</option>';

                res.results.forEach(function (item, index, array) {
                    selects = selects + '<option value="' + item.id + '" >' + item.text + '</option>';
                });

                $('#familia').html(selects);
            }
        });
    });

    $("#proveedor").change(function () {
        $.ajax({
            url: "/Home/SetProveedor",
            type: 'POST',
            dataType: 'HTML',
            data: {
                idProveedor: $(this).val(),
            },
            success: function (res) {
                var a = 0;
            }
        });
    });

    $("#familia").change(function () {
        $.ajax({
            url: "/Home/SetFamilia",
            type: 'POST',
            dataType: 'HTML',
            data: {
                idFamilia: $(this).val(),
            },
            success: function (res) {
                var a = 0;
            }
        });
    });

    $("#producto").chosen({ placeholder_text_single: "Seleccione el producto", no_results_text: "No existen coincidencias" });

    $("#producto").ajaxChosen({
        dataType: "json",
        type: "GET",
        minTermLength: 3,
        afterTypeDelay: 300,
        cache: false,
        url: "/Home/GetProductos"
    }, {
        loadingImg: "Content/chosen/images/loading.gif"
    }, { placeholder_text_single: "Seleccione el producto", no_results_text: "No existen coincidencias" });

    $(".rMoneda").click(function () {
        $.ajax({
            url: "/Home/SetMoneda",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idMoneda: $(this).val(),
            },
            success: function (res) {
                var a = 0;
            }
        });

    });
    $("#producto").change(function () {
        $.ajax({
            url: "/Home/GetProducto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idProducto: $(this).val(),
            },
            success: function (res) {
                $("#presentacion").val(res.presentacion);
                $("#imgProducto").attr("src", res.image);
                $("#cantidad").val(1);
                $("#pdescuento").val(0.0);
               // alert("asas");
                var selectstmp = "<option selected>Seleccione...</option>";
                var selects = "";
                res.precios.forEach(function (item, index, array) {
                  
                    if (item.id == $("#precio").val()) {
                        selects = selects + '<option selected="selected" price="' + item.precio + '" value="' + item.id + '" >' + item.nombre + '</option>';
                        selectstmp = "<option >Seleccione...</option>";
                    }
                    else
                    {
                        selects = selects + '<option price="' + item.precio + '" value="' + item.id + '" >' + item.nombre + '</option>';
                    }
                });
                selects = selectstmp + selects;

                $('#precioProducto').html(selects);
                var price = $('#precioProducto option[value="0b08561c-d257-43b6-bc9f-85b4ab24ee39"]').attr("price");
                $("#valorunitario").val(price);
                calcularProducto();
            }
        });
    });

    $("#precioProducto").change(function () {
        var idPrecio = $(this).val();
        var price = $('#precioProducto option[value="' + idPrecio + '"]').attr("price");
        $("#valorunitario").val(price);
        calcularProducto();
    });

    $("#pdescuento, #cantidad").change(function () {
        calcularProducto();
    });

    $("#btnAddProduct").click(function () {
        addProducto();
    });
    
    $("#btnCancelAddProduct").click(function () {
        
    });

    function calcularProducto() {
        var cantidad = parseInt($("#cantidad").val());
        var pDescuento = parseFloat($("#pdescuento").val());
        var price = parseFloat($("#valorunitario").val());
        var priceFinal = price * (100 - pDescuento) * 0.01;
        var subTotal = priceFinal * cantidad;

        $("#valorunitarioneto").val(priceFinal);
        $("#subtotal").val(subTotal);
    };

    function addProducto() {
        //alert(1);
        var cantidad = parseInt($("#cantidad").val());
        var pDescuento = parseFloat($("#pdescuento").val());
        var idPrice = $("#precioProducto").val();
        var prov = $("#proveedor option:selected").text();
        var cat = $("#categoria option:selected").text();
        var fam = $("#familia option:selected").text();
        var prod = $("#producto option:selected").text();
        //alert(2);
        $.ajax({
            url: "/Home/AddProducto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                cantidad: cantidad,
                pDescuento: pDescuento,
                idPrecioProducto: idPrice,
                proveedor: prov,
                categoria: cat,
                familia: fam
            },
            success: function (res) {
                // alert(res.codigoProducto);
                $('.table tbody tr.footable-empty').remove();
                $(".table tbody").append('<tr data-expanded="true"><td>' + res.proveedor + '</td><td>' + res.codigoProducto + '</td><td>' + res.nombreProducto + '</td><td>' + res.presentacion +
                     '</td><td class="column-img"><img class="table-product-img" src="' + $("#imgProducto").attr("src") + '"></td><td>' + res.moneda + " " + res.valorUnitarioFinal +
                     '</td><td>' + res.cantidad + '</td><td>' + res.nombrePrecio + '</td><td>' + res.porcentajeDescuento + '%</td><td>' + res.moneda + " " + res.subTotal + '</td></tr>');
                $('.table').footable();
                $('#btnCancelAddProduct').click();
                
            }
        });
        
    };
});