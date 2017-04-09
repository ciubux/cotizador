/* 
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */


jQuery(function($){
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
                var selects = "<option selected>Seleccione...</option>";
                res.precios.forEach(function (item, index, array) {
                    selects = selects + '<option price="' + item.precio + '" value="' + item.id + '" >' + item.nombre + '</option>';
                });
                $('#precioProducto').html(selects);
            }
        });
    });

    $("#precioProducto").change(function () {
        var idPrecio = $(this).val();
        var price = $('#precioProducto option[value="' + idPrecio + '"]').attr("price");

        $("#valorunitario").val(price);
    });

    $("#pdescuento, #cantidad").change(function () {
        var cantidad = parseInt($("#cantidad").val());
        var pDescuento = parseFloat($("#pdescuento").val());
        var price = parseFloat($("#valorunitario").val());
        var priceFinal = price * (100 - pDescuento) * 0.01;
        var subTotal = priceFinal * cantidad;

        $("#valorunitarioneto").val(priceFinal);
        $("#subtotal").val(subTotal);
    });
    
});