

jQuery(function ($) {
    var pagina = 28


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

    $("#kpiPeriodos").chosen({ placeholder_text: "Seleccione Periodo", no_results_text: "No se encontró"});

    //$("#fechaInicio").datepicker({ dateFormat: "dd/mm/yy" });
    

    $(document).ready(function () {

    });


    function limpiarFormulario() {
        $("#fechaInicio").val("");
        $("#fechaFin").val("");
        $("#skuProducto").val("");

        $('#proveedor').val('Todos');
        $('#proveedor').trigger("chosen:updated");

        $('#rucsCliente option').remove();
        $('#rucsCliente').trigger('chosen:updated');
        $('#codigosCliente option').remove();
        $('#codigosCliente').trigger('chosen:updated');
        $('#codigosGrupoCliente option').remove();
        $('#codigosGrupoCliente').trigger('chosen:updated');
    }

    $("#btnLimpiarFormulario").click(function () {
        limpiarFormulario();
    });

    $("#btnExportExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
    });


});


