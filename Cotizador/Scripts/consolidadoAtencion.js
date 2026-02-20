var CONSOLIDADO_LAST_SEARCH;
var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";

jQuery(function($) {
    $(document).ready(function() { $("#btnBusqueda").click(); });

    function limpiarFormulario() {
        $("#consolidado_fecha").val(new Date().toISOString().split('T')[0]);
        $("#consolidado_idVehiculo").val("");
        $("#consolidado_idChofer").val("");
        $("#consolidado_idAsistente").val("");
        $("#consolidado_observaciones").val("");
    }

    $("#consolidado_idCiudad").change(function () {
        var valor = $("#consolidado_idCiudad").val();
        changeInputForm("ciudad", "ciudad", valor);
        setTimeout(function () { location.reload(); }, 200);
    }); 

    $("#consolidado_idVehiculo").change(function () {
        var valor = $("#consolidado_idVehiculo").val();
        changeInputForm("vehiculo", "vehiculo", valor);
    });  

    $("#consolidado_idChofer").change(function () {
        var valor = $("#consolidado_idChofer").val();
        changeInputForm("chofer", "chofer", valor);
    });

    $("#consolidado_idAsistente").change(function () {
        var valor = $("#consolidado_idAsistente").val();
        changeInputForm("asistente", "asistente", valor);
    });

    $("#consolidado_fecha").change(function () {
        var valor = $("#consolidado_fecha").val();
        changeInputForm("date", "fecha", valor);
    });
    
    $("#consolidado_observaciones").change(function () {
        var valor = $("#consolidado_observaciones").val();
        changeInputForm("string", "observaciones", valor);
    });

    function changeInputForm(tipo, propiedad, valor) {
        $.ajax({
            url: "/ConsolidadoAtencion/ChangeInputForm",
            type: 'POST',
            data: {
                tipo: tipo,
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }
    
    $("select#idVehiculo").chosen({ placeholder_text_single: "Buscar Vehiculo", no_results_text: "No se encontró Vehículo" }).on('chosen:showing_dropdown', function (evt, params) {
        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar la sede MP previamente.");
            $("#idVehiculo").trigger('chosen:close');
            $("#idCiudad").focus();
            return false;
        }
    });

    

    $("#btnGuardarConsolidado").click(function() {
        var id = $("#consolidado_id").val();
        var url = (id == GUID_EMPTY) ? "/ConsolidadoAtencion/Create" : "/ConsolidadoAtencion/Update";

        var data = {
            idConsolidadoAtencion: id,
            fecha: $("#consolidado_fecha").val(),
            idVehiculo: $("#consolidado_idVehiculo").val(),
            idChofer: $("#consolidado_idChofer").val(),
            idAsistente: $("#consolidado_idAsistente").val(),
            observaciones: $("#consolidado_observaciones").val()
        };

        if (!data.idVehiculo || !data.idChofer) {
            alert("Vehículo y Chofer son obligatorios");
            return;
        }

        $('body').loadingModal({ text: 'Guardando...' });
        $.post(url, data, function(res) {
            $('body').loadingModal('hide');
            window.location.reload();
        }, 'JSON');
    });

    $("#btnBusqueda").click(function() {
        $.post("/ConsolidadoAtencion/SearchList", function(list) {
            CONSOLIDADO_LAST_SEARCH = list;
            var rows = "";
            $("#tableConsolidados > tbody").empty();
            $("#tableConsolidados").footable({
                "paging": {
                    "enabled": true
                }
            });

            for (var i = 0; i < list.length; i++) {
                var c = list[i];
                // Formatear fecha de JSON
                var fechaStr = c.fecha ? new Date(parseInt(c.fecha.substr(6))).toLocaleDateString() : "";
                
                rows += '<tr>' +
                    '<td>' + fechaStr + '</td>' +
                    '<td>' + (c.vehiculo ? c.vehiculo.placa : "") + '</td>' +
                    '<td>' + (c.chofer ? c.chofer.apellidoPaterno + ' ' + c.chofer.nombres : "") + '</td>' +
                    '<td>' + (c.asistente ? c.asistente.apellidoPaterno + ' ' + c.asistente.nombres : "") + '</td>' +
                    '<td><button type="button" class="btn btn-primary btnEditar" data-id="' + c.idConsolidadoAtencion + '">Editar</button></td>' +
                    '</tr>';
            }
            $("#tableConsolidados tbody").html(rows);
        }, 'JSON');
    });

    $(document).on('click', '.btnEditar', function() {
        var id = $(this).data("id");
        var c = CONSOLIDADO_LAST_SEARCH.find(x => x.idConsolidadoAtencion == id);
        if (c) {
            $("#consolidado_id").val(c.idConsolidadoAtencion);
            // Formatear fecha para input date (yyyy-mm-dd)
            var dateObj = new Date(parseInt(c.fecha.substr(6)));
            $("#consolidado_fecha").val(dateObj.toISOString().split('T')[0]);
            
            $("#consolidado_idVehiculo").val(c.vehiculo ? c.vehiculo.idVehiculo : "");
            $("#consolidado_idChofer").val(c.chofer ? c.chofer.idPersonalAlmacen : "");
            $("#consolidado_idAsistente").val(c.asistente ? c.asistente.idPersonalAlmacen : "");
            $("#consolidado_observaciones").val(c.observaciones);

            $("#modalTitle").text("Editar Consolidado");
            $("#modalEditarConsolidado").modal("show");
        }
    });

    $("#btnAgregarConsolidado").click(function() {
        limpiarFormulario();
        $("#modalTitle").text("Nuevo Consolidado");
        $("#modalEditarConsolidado").modal("show");
    });
});