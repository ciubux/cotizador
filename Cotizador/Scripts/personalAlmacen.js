var PERSONAL_LAST_SEARCH;
jQuery(function ($) {
    $(document).ready(function () { $("#btnBusqueda").click(); });

    function limpiarFormulario() {
        $("#personal_id").val("0");
        $("#personal_nombres").val("");
        $("#personal_paterno").val("");
        $("#personal_materno").val("");
        $("#personal_documento").val("");
        $("#personal_brevete").val("");
        $("#personal_tipo").val("");
        $("#personal_idSede").val("");
    }

    function validar() {
        if ($("#personal_documento").val().length < 8) {
            alert("Ingrese un documento válido"); return false;
        }
        if ($("#personal_nombres").val().trim() == "") {
            alert("Ingrese los nombres"); return false;
        }
        if ($("#personal_idSede").val() == "") {
            alert("Seleccione una sede"); return false;
        }
        return true;
    }

    $("#btnFinalizarEdicionPersonal").click(function () {
        var url = $("#personal_id").val() == "0" ? "/PersonalAlmacen/Create" : "/PersonalAlmacen/Update";
        if (!validar()) return;

        var data = {
            idPersonalAlmacen: $("#personal_id").val(),
            nombres: $("#personal_nombres").val(),
            apellidoPaterno: $("#personal_paterno").val(),
            apellidoMaterno: $("#personal_materno").val(),
            nroDocumento: $("#personal_documento").val(),
            brevete: $("#personal_brevete").val(),
            tipo: $("#personal_tipo").val(),
            idSede: $("#personal_idSede").val()
        };

        $.post(url, data, function (res) {
            window.location.reload();
        }, 'JSON');
    });

    $("#btnBusqueda").click(function () {
        $.post("/PersonalAlmacen/SearchList", function (list) {
            PERSONAL_LAST_SEARCH = list;
            $("#tablePersonal > tbody").empty();
            $("#tablePersonal").footable({
                "paging": {
                    "enabled": true
                }
            });

            var rows = "";
            for (var i = 0; i < list.length; i++) {
                var p = list[i];
                rows += '<tr>' +
                    '<td>' + p.idPersonalAlmacen + '</td>' +
                    '<td>' + (p.sedePrincipal ? p.sedePrincipal.nombre : "-") + '</td>' +
                    '<td>' + p.nroDocumento + '</td>' +
                    '<td>' + p.apellidoPaterno + ' ' + p.apellidoMaterno + ', ' + p.nombres + '</td>' +
                    '<td>' + p.tipo + '</td>' +
                    '<td><button type="button" class="btn btn-primary btnEditar" data-id="' + p.idPersonalAlmacen + '">Editar</button></td>' +
                    '</tr>';
            }
            $("#tablePersonal tbody").html(rows);
        }, 'JSON');
    });

    $(document).on('click', '.btnEditar', function () {
        var id = $(this).data("id");
        var p = PERSONAL_LAST_SEARCH.find(x => x.idPersonalAlmacen == id);
        if (p) {
            $("#personal_id").val(p.idPersonalAlmacen);
            $("#personal_nombres").val(p.nombres);
            $("#personal_paterno").val(p.apellidoPaterno);
            $("#personal_materno").val(p.apellidoMaterno);
            $("#personal_documento").val(p.nroDocumento);
            $("#personal_brevete").val(p.brevete);
            $("#personal_tipo").val(p.tipo);
            $("#personal_idSede").val(p.sedePrincipal ? p.sedePrincipal.idCiudad : "");

            $("#modalEditarPersonalTitle").text("Editar Personal");
            $("#btnFinalizarEdicionPersonal").text("Actualizar");
            $("#modalEditarPersonal").modal("show");
        }
    });

    $("#btnAgregarPersonal").click(function () {
        limpiarFormulario();
        $("#modalEditarPersonalTitle").text("Registrar Personal");
        $("#btnFinalizarEdicionPersonal").text("Registrar");
        $("#modalEditarPersonal").modal("show");
    });

    $("#btnExportExcel").click(function () {
        const dataExcelDescargar = [["SEDE", "NRO. DOC.", "NOMBRES", "APELLIDO PATERNO", "APELLIDO MATERNO", "TIPO", "BREVETE"]];
        for (var i = 0; i < PERSONAL_LAST_SEARCH.length; i++) {
            var obj = PERSONAL_LAST_SEARCH[i];
            var ItemRow = [(obj.sedePrincipal ? obj.sedePrincipal.nombre : "-"), obj.nroDocumento, obj.nombres, obj.apellidoPaterno, obj.apellidoMaterno, obj.tipo, obj.brevete];
            dataExcelDescargar.push(ItemRow);
        }
        ExportarTablaExcelJs(dataExcelDescargar, "Personal_Alamcen", "PERSONAL_ALMACEN");
    });
});
