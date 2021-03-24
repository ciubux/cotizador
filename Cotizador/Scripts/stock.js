
jQuery(function ($) {
    var pagina = 28

    $(document).ready(function () {

    });


    function limpiarFormulario() {
        $("#rubro_codigo").val("");
        $("#rubro_nombre").val("");
    }


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

    $("#producto_sku").change(function () {
        changeInputString("sku", $("#producto_sku").val());
    });

    $("#producto_descripcion").change(function () {
        changeInputString("descripcion", $("#producto_descripcion").val());
    });

    $("#tipoVentaRestringidaBusqueda").change(function () {
        changeInputInt("tipoVentaRestringidaBusqueda", $("#tipoVentaRestringidaBusqueda").val());
    });

    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/Stock/ChangeInputInt",
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
            url: "/Stock/ChangeInputDecimal",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Stock/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    function changeInputBoolean(propiedad, valor) {
        $.ajax({
            url: "/Stock/ChangeInputBoolean",
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



});


