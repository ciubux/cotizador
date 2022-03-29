

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

    $("#kpiPeriodos").chosen({ placeholder_text: "Seleccione Periodo", no_results_text: "No se encontró" });
    $("#kpiKpis").chosen({ placeholder_text: "Seleccione KPI", no_results_text: "No se encontró" });
    $("#kpiUsuarios").chosen({ placeholder_text: "Seleccione Usuario", no_results_text: "No se encontró" });
    FooTable.init('#tableResultadosKPI');
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

    $("#kpiPeriodos").change(function () {
        var idKpiPeriodo = $(this).val();
        $.ajax({
            url: "/Kpi/GetPeriodoKPIs",
            data: {
                idKpiPeriodo: idKpiPeriodo
            },
            type: 'POST',
            dataType: 'JSON',
            error: function () {
            },
            success: function (list) {
                $("#kpiKpis").empty();
                $('#kpiKpis').append('<option value=""></option>');

                for (var i = 0; i < list.length; i++) {

                    var newOption = $('<option value="' + list[i].idKpi + '">  ' + list[i].kpi + '</option>');
                    $('#kpiKpis').append(newOption);
                }

                $("#kpiKpis").trigger("chosen:updated");
            }
        });
    });

    $("#kpiKpis").change(function () {
        var idKpi = $(this).val();
        $.ajax({
            url: "/Kpi/GetPeriodoKpiUsuarios",
            data: {
                idKpi: idKpi
            },
            type: 'POST',
            dataType: 'JSON',
            error: function () {
            },
            success: function (list) {
                $("#kpiUsuarios").empty();
                $('#kpiUsuarios').append('<option value=""></option>');

                if (list.length > 1) {
                    $('#kpiUsuarios').append('<option value="00000000-0000-0000-0000-000000000000">TODOS</option>');
                }

                for (var i = 0; i < list.length; i++) {

                    var newOption = $('<option value="' + list[i].idUsuario + '">  ' + list[i].nombre + '(' + list[i].email + ')</option>');
                    $('#kpiUsuarios').append(newOption);
                }

                $("#kpiUsuarios").trigger("chosen:updated");
            }
        });
    });

    $("#kpiUsuarios").change(function () {
        var idUsuario = $(this).val();
        var idsUsuario = [];
        if (idUsuario == '00000000-0000-0000-0000-000000000000') {
            var pos = 0;
            $('#kpiUsuarios option').each(function () {
                var id = $(this).val();
                if (id != '00000000-0000-0000-0000-000000000000' && id != '') {
                    idsUsuario[pos] = id;
                    pos = pos + 1;
                }
            });
           
        } else {
            idsUsuario[0] = idUsuario;
        }
        

        $.ajax({
            url: "/Kpi/GetResultadosKPIUsuarios",
            data: {
                idsUsuario: idsUsuario
            },
            type: 'POST',
            dataType: 'JSON',
            error: function () {
            },
            success: function (list) {
                $("#tableResultadosKPI > tbody").empty();

                for (var i = 0; i < list.length; i++) {

                    var newOption = $('<tr>' +
                        '<td>' + list[i].usuario.nombre + '</td>' +
                        '<td>' + list[i].valor + '</td>' +
                        '<td>' + list[i].resultado + '</td>' +
                        '</tr>');
                    $('#tableResultadosKPI').append(newOption);

                }

                FooTable.init('#tableResultadosKPI');
            }
        });
    });
});
