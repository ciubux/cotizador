
jQuery(function ($) {
    var pagina = 28

    var ID_SEDE_TODOS = "00000000-0000-0000-0000-000000000000";

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


    $("#fechaCierre").datepicker({ dateFormat: "dd/mm/yy" });
    $("#fechaStock").datepicker({ dateFormat: "dd/mm/yy" });
    

    $(document).ready(function () {

        if ($("#pagina").val() == 722) {
            if ($('#idCiudad option').length > 3) {
                $('#idCiudad').append('<option value = "' + ID_SEDE_TODOS + '" >TODOS</option>');
            }
        }

        FooTable.init('#tableCargasStock');
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

    $("#btnReporteStockExcel").click(function () {
        var fechaStock = $("#fechaStock").val();
        var tipoUnidad = $("#tipoUnidad").val();
        var idCiudad = $("#idCiudad").val();
        window.location.href = $(this).attr("actionLink") + '?tipoUnidad=' + tipoUnidad + '&fechaStock=' + fechaStock + '&idCiudad=' + idCiudad;
    });

    $("#btnCargarStock").click(function () {
        var idCiudad = $("#idCiudad").val();
        var sede = $("#idCiudad option:selected").text();
        var fecha = $("#fechaCierre").val();

        var valido = true;

        if (fecha.trim() == "") {
            alert("Debe ingresar la fecha de cierre stock");
            valido = false;
        }

        if (idCiudad.trim() == "") {
            alert("Debe seleccionar la sede");
            valido = false;
        }

        if (valido) {
            $.confirm({
                title: 'Confirmar Carga',
                content: '¿Está seguro de subir el cierre de stock del almacén de ' + sede + ' a la fecha ' + fecha + '?',
                type: 'orange',
                buttons: {
                    aplica: {
                        text: 'SI',
                        btnClass: 'btn-success',
                        action: function () {
                            $('body').loadingModal({
                                text: 'Leyendo Archivo...'
                            });
                            $("#formCargarStock").submit();
                        }
                    },
                    cancelar: {
                        text: 'Cancelar',
                        btnClass: '',
                        action: function () {
                        }
                    }
                }
            });
        }

    });


    $('body').on('click', ".btnDescargarArchivoAdjunto", function () {

        var idArchivo = $(this).attr('idArchivoAdjunto');

        $.ajax({
            url: "/ArchivoAdjunto/DescargarArchivo",
            type: 'POST',
            dataType: 'JSON',
            data: { idArchivo: idArchivo },
            error: function (detalle) {
                alert(detalle);
            },
            success: function (archivoAdjunto) {
                var sampleArr = base64ToArrayBuffer(archivoAdjunto.adjunto);
                saveByteArray(archivoAdjunto.nombre, sampleArr);
            }
        });
    });

});


