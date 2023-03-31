

jQuery(function ($) {
    var pagina = 28

    $("#rucsCliente").chosen({ placeholder_text: "Agregar RUCs", no_results_text: "No se encontró", allow_single_deselect: true }).on('chosen:showing_dropdown');
    $("#rucsCliente").ajaxChosen({
        dataType: "json",
        type: "GET",
        minTermLength: 0,
        afterTypeDelay: 300,
        cache: false,
        url: "/General/ChosenDinamico"
    }, {
        loadingImg: "Content/chosen/images/loading.gif"
    }, { placeholder_text_single: "Agregar RUCs", no_results_text: "No se encontró RUC" });

    $("#codigosCliente").chosen({ placeholder_text: "Agregar Codigos Cliente", no_results_text: "No se encontró", allow_single_deselect: true }).on('chosen:showing_dropdown');
    $("#codigosCliente").ajaxChosen({
        dataType: "json",
        type: "GET",
        minTermLength: 0,
        afterTypeDelay: 300,
        cache: false,
        url: "/General/ChosenDinamico"
    }, {
        loadingImg: "Content/chosen/images/loading.gif"
    }, { placeholder_text_single: "Agregar Codigos Cliente", no_results_text: "No se encontró Código Cliente" });

    $("#codigosGrupoCliente").chosen({ placeholder_text: "Agregar Codigos Grupo Cliente", no_results_text: "No se encontró", allow_single_deselect: true }).on('chosen:showing_dropdown');
    $("#codigosGrupoCliente").ajaxChosen({
        dataType: "json",
        type: "GET",
        minTermLength: 0,
        afterTypeDelay: 300,
        cache: false,
        url: "/General/ChosenDinamico"
    }, {
        loadingImg: "Content/chosen/images/loading.gif"
    }, { placeholder_text_single: "Agregar Codigos Grupo Cliente", no_results_text: "No se encontró Código Grupo Cliente" });


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


    $("#fechaInicio").datepicker({ dateFormat: "dd/mm/yy" });
    $("#fechaFin").datepicker({ dateFormat: "dd/mm/yy" });
    

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


    $("#btnActualizarVista").click(function () {
        var rucs = $("#rucsCliente").val();
        var codsCliente = $("#codigosCliente").val();
        var codsGrupoCliente = $("#codigosGrupoCliente").val();
        var fechaInicio = $("#fechaInicio").val();
        var fechaFin = $("#fechaFin").val();
        var sku = $("#skuProducto").val();
        var proveedor = $("#proveedor").val();
        var idSede = $("#idSede").val();

        $.ajax({
            url: "/Reporte/ActualizarPametrosSellOutPersonalizado",
            type: 'POST',
            data: {
                sku: sku,
                proveedor: proveedor,
                fechaInicio: fechaInicio,
                fechaFin: fechaFin,
                rucs: rucs,
                codigosCliente: codsCliente,
                codigosGrupoCliente: codsGrupoCliente,
                idSede: idSede
            },
            success: function () {
                $.alert({
                    title: "Actualización Exitosa",
                    type: 'green',
                    content: "Los parámetros fueron actualizados.",
                    buttons: {
                        OK: function () {
                        }
                    }
                });
            }
        });
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


});


