var VEHICULO_LAST_SEARCH;
jQuery(function ($) {
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";

    $(document).ready(function () {
        $("#btnBusqueda").click();
    });

    function limpiarFormularioVehiculo() {
        $("#vehiculo_id").val("0");
        $("#vehiculo_placa").val("");
        $("#vehiculo_marca").val("");
        $("#vehiculo_modelo").val("");
        $("#vehiculo_idCiudad").val(""); 
    }

    function validacionDatosVehiculo() {
        if ($("#vehiculo_placa").val().length < 6) {
            $.alert({
                title: "Placa Inválida",
                type: 'orange',
                content: 'Debe ingresar una placa válida (mínimo 6 caracteres).',
                buttons: { OK: function () { $('#vehiculo_placa').focus(); } }
            });
            return false;
        }

        if ($("#vehiculo_marca").val().trim() == "") {
            $.alert({
                title: "Marca Requerida",
                type: 'orange',
                content: 'Debe ingresar la marca del vehículo.',
                buttons: { OK: function () { $('#vehiculo_marca').focus(); } }
            });
            return false;
        }

        if ($("#vehiculo_idCiudad").val() == "") {
            $.alert({
                title: "Ciudad Requerida",
                type: 'orange',
                content: 'Debe seleccionar una ciudad.',
                buttons: { OK: function () { $('#vehiculo_idCiudad').focus(); } }
            });
            return false;
        }

        return true;
    }

    $("#btnFinalizarEdicionVehiculo").click(function () {
        if ($("#vehiculo_id").val() == '0') {
            crearVehiculo();
        }
        else {
            editarVehiculo();
        }
    });

    function crearVehiculo() {
        if (!validacionDatosVehiculo()) return false;

        $('body').loadingModal({ text: 'Registrando Vehículo...' });
        $.ajax({
            url: "/Vehiculo/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                placa: $("#vehiculo_placa").val(),
                marca: $("#vehiculo_marca").val(),
                modelo: $("#vehiculo_modelo").val(),
                idCiudad: $("#vehiculo_idCiudad").val()
            },
            error: function () {
                $('body').loadingModal('hide');
                $.alert({ title: 'Error', content: 'Error al intentar crear el vehículo.', type: 'red' });
            },
            success: function () {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Éxito',
                    content: 'El vehículo se registró correctamente.',
                    type: 'green',
                    buttons: { OK: function () { window.location = '/Vehiculo/List'; } }
                });
            }
        });
    }

    function editarVehiculo() {
        if (!validacionDatosVehiculo()) return false;

        $('body').loadingModal({ text: 'Actualizando Vehículo...' });
        $.ajax({
            url: "/Vehiculo/Update",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idVehiculo: $("#vehiculo_id").val(),
                placa: $("#vehiculo_placa").val(),
                marca: $("#vehiculo_marca").val(),
                modelo: $("#vehiculo_modelo").val(),
                idCiudad: $("#vehiculo_idCiudad").val()
            },
            error: function () {
                $('body').loadingModal('hide');
                $.alert({ title: 'Error', content: 'Error al intentar editar el vehículo.', type: 'red' });
            },
            success: function () {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Éxito',
                    content: 'El vehículo se actualizó correctamente.',
                    type: 'green',
                    buttons: { OK: function () { window.location = '/Vehiculo/List'; } }
                });
            }
        });
    }

    $("#btnAgregarVehiculo").click(function () {
        limpiarFormularioVehiculo();
        $("#modalEditarVehiculoTitle").html("Registrar Nuevo Vehículo");
        $("#btnFinalizarEdicionVehiculo").html("Registrar");
        $("#vehiculo_placa").removeAttr("disabled");
        $("#modalEditarVehiculo").modal('show');
    });

    $("#btnCancelarVehiculo").click(function () {
        $("#modalEditarVehiculo").modal('hide');
    });

    $("#btnExportExcel").click(function () {
        const dataExcelDescargar = [["PLACA", "MARCA", "MODELO", "CIUDAD"]];
        for (var i = 0; i < VEHICULO_LAST_SEARCH.length; i++) {
            var v = VEHICULO_LAST_SEARCH[i];
            var ItemRow = [v.placa, v.marca, v.modelo, (v.ciudad ? v.ciudad.nombre : "")];
            dataExcelDescargar.push(ItemRow);
        }
        ExportarTablaExcelJs(dataExcelDescargar, "Vehiculos", "VEHICULOS");
    });

    $("#btnBusqueda").click(function () {
        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/Vehiculo/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () { $("#btnBusqueda").removeAttr("disabled"); },
            success: function (list) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableVehiculos > tbody").empty();
                $("#tableVehiculos").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                VEHICULO_LAST_SEARCH = list;

                for (var i = 0; i < list.length; i++) {
                    var nombreCiudad = list[i].ciudad ? list[i].ciudad.nombre : "N/A";

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td dataAttr="idVehiculo">' + list[i].idVehiculo + '</td>' +
                        '<td>' + nombreCiudad + '</td>' +
                        '<td>' + list[i].placa + '</td>' +
                        '<td>' + list[i].marca + '</td>' +
                        '<td>' + list[i].modelo + '</td>' +
                        '<td>' +
                        '<button type="button" class="btnEditarVehiculo btn btn-primary" idVehiculo="' + list[i].idVehiculo + '">Editar</button>' +
                        '</td>' +
                        '</tr>';
                    $("#tableVehiculos").append(ItemRow);
                }

            }
        });
    });

    $(document).on('click', "button.btnEditarVehiculo", function () {
        var id = $(this).attr("idVehiculo");
        var item = VEHICULO_LAST_SEARCH.find(x => x.idVehiculo == id);

        if (item) {
            $("#vehiculo_id").val(item.idVehiculo);
            $("#vehiculo_placa").val(item.placa);
            $("#vehiculo_marca").val(item.marca);
            $("#vehiculo_modelo").val(item.modelo);
            $("#vehiculo_idCiudad").val(item.ciudad ? item.ciudad.idCiudad : "");

            $("#modalEditarVehiculoTitle").html("Editar Vehículo");
            $("#btnFinalizarEdicionVehiculo").html("Editar");
            $("#modalEditarVehiculo").modal('show');
        }
    });
});