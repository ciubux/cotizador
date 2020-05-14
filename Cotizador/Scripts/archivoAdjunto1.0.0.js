jQuery(function ($) {
    var pagina = 28;
    var ARCHIVO_ADJUNTO_CANCELAR_EDICION = '¿Está seguro de cancelar la edición del mensaje; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';


    $('body').ready(function () {

    });


    $("#btnLimpiarBusquedaArchivosAdjuntos").click(function () {
        $.ajax({
            url: "/ArchivoAdjunto/Limpiar",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });

    $("#archivoAdjunto_nombre").change(function () {
        changeInputString("nombre", $("#archivoAdjunto_nombre").val());
    });


    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/ArchivoAdjunto/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#origen_archivo_adjunto").change(function () {
        changeInputOrigen($("#origen_archivo_adjunto").val());
    });

    function changeInputOrigen(origenBusqueda) {
        $.ajax({
            url: "/ArchivoAdjunto/ChangeInputOrigen",
            type: 'POST',
            data: {
                origenBusqueda: origenBusqueda
            },
            success: function () { }
        });
    }


    $("#btnBusquedaArchivosAdjuntos").click(function () {

        $("#btnBusquedaArchivosAdjuntos").attr("disabled", "disabled");
        $.ajax({
            url: "/ArchivoAdjunto/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusquedaArchivosAdjuntos").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusquedaArchivosAdjuntos").removeAttr("disabled");
                $("#tableBusquedaArchivosAdjuntos > tbody").empty();
                $("#tableBusquedaArchivosAdjuntos").footable({
                    "paging": {
                        "enabled": true
                    }
                });


                for (var i = 0; i < list.length; i++) {

                    var date = new Date(list[i].fechaCreacion);
                    hora = (date.getHours() < 10 ? '0' : '') + date.getHours();
                    minuto = (date.getMinutes() < 10 ? '0' : '') + date.getMinutes();
                    horaImprimible = hora + ":" + minuto;


                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>' + list[i].idArchivoAdjunto + '</td>' +
                        '<td>' + list[i].nombre + '</td>' +
                        '<td>' + $.datepicker.formatDate('dd/mm/yy', new Date(list[i].fechaCreacion)) + ' ' + horaImprimible + '</td>' +
                        '<td>' + list[i].usuario.nombre + '</td>' +
                        '<td>' +
                        '<button type="button" class="btnDescargarArchivoAdjunto btn btn-primary bouton-image botonDownload" idArchivoAdjunto="' + list[i].idArchivoAdjunto + '">Descargar</button >' +
                        '</td>' +
                        '</tr>';

                    $("#tableBusquedaArchivosAdjuntos").append(ItemRow);

                }
            }
        });

    });

    $('body').on('click', ".btnDescargarArchivoAdjunto", function () {

        var idArchivo = $(this).attr('idArchivoAdjunto');

        $.ajax({
            url: "/ArchivoAdjunto/DescargarArchivo",
            type: 'POST',
            //  enctype: 'multipart/form-data',
            dataType: 'JSON',
            //  contentType: 'multipart/form-data',
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

    function loadArchivoAdjunto(idRegistro, archivos) {
        var b = idRegistro;

        var a = archivos;
        $.ajax({
            url: "/ArchivoAdjunto/subidaArchivo",
            type: 'POST',
            data: { idRegistro: idRegistro },
            error: function (detalle) {
                alert(detalle);
            },
            success: function (success) {

            }
        });

    }

    $('body').on('change', "#uploadfiles", function (e) {      
        CargarArchivos(e.currentTarget.files);
        var data = new FormData($('#FormWithArchivos')[0]);
                
        $.ajax({
            url: "/ArchivoAdjunto/ChangeFiles",
            type: 'POST',
            enctype: 'multipart/form-data',
            contentType: false,
            processData: false,
            data: data,
            error: function (detalle) { },
            success: function (resultado) { }
        });
    });

    $('body').on('click', ".btnDeleteArchivo", function () {
        var nombreArchivo = event.target.id;
        //eliminarListaArchivos(idArchivo);

        $("#uploadfiles").val("");
        $("#nombreArchivos > li").remove().end();
        $.ajax({
            url: "/ArchivoAdjunto/DescartarArchivos",
            type: 'POST',
            dataType: 'JSON',
            data: { nombreArchivo: nombreArchivo },
            error: function (detalle) { },
            success: function (files) {
                
                $("#nombreArchivos > li").remove().end();

                for (var i = 0; i < files.length; i++) {
                    if (files[i].estado == 1)
                    {
                        var liHTML = '<a href="javascript:mostrar();" class="btnDescargarArchivoAdjunto" idArchivoAdjunto="' + files[i].idArchivoAdjunto + '">' + files[i].nombre + '</a>' +
                            '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + files[i].nombre + '" class="btnDeleteArchivo" /></a>';

                        $('#nombreArchivos').append($('<li />').html(liHTML));
                        //      .appendTo($('#nombreArchivos'));

                    }
                }
            }
        });





    });
});


function archivosEncontrados(files) {

    $("#nombreArchivos>li").remove().end();

    for (var i = 0; i < files.length; i++) {
        var liHTML = '<a href = "javascript:mostrar();" class="btnDescargarArchivoAdjunto" idArchivoAdjunto="' + files[i].idArchivoAdjunto + '"> ' + files[i].nombre + '</a > ' +
            '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + files[i].nombre + '" class="btnDeleteArchivo" /></a>';
        $('<li/>').html(liHTML).appendTo($('#nombreArchivos'));
    }

    $("#verNombreArchivos > li").remove().end();


    for (var i = 0; i < files.length; i++) {
        var liHTML = '<a href="javascript:mostrar();" class="btnDescargarArchivoAdjunto" idArchivoAdjunto="' + files[i].idArchivoAdjunto + '">' + files[i].nombre + '</a>';
        //$('<li />').html(liHTML).appendTo($('#nombreArchivos'));
        $('#verNombreArchivos').append($('<li />').html(liHTML));
    }

    var pathname = window.location.pathname;
    pathname=pathname.split('/');
    for (var i = 0; i < files.length; i++) {
        files[i].origen = pathname[1];
        files[i].metaData = pathname[3];
        files[i].metaData = files[i].metadata == undefined ? 'Index' : files[i].metadata;
    }
    $.ajax({
        type: "POST",
        url: '/ArchivoAdjunto/FilesGuardarSession',
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(files),
        dataType: "json",
        success: function () { },
        error: function () { }
    });
}

   

/*
function obsAdjuntoarchivosEncontrados() {

    $("#nombreArchivos>li").remove().end();

    for (var i = 0; i < archivosfinal.length; i++) {
        var liHTML = '<a href = "javascript:mostrar();" class="btnDescargarArchivoAdjunto" idArchivoAdjunto="' + archivosfinal[i].idArchivoAdjunto + '"> ' + archivosfinal[i].nombre + '</a > ' +
            '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + archivosfinal[i].nombre + '" class="btnDeleteArchivo" /></a>';
        $('<li/>').html(liHTML).appendTo($('#nombreArchivos'));
    }
}
*/

function eliminarListaArchivos(idArchivo)
{
    archivosfinal = $.grep(archivosfinal, function (e) {
        return e.idArchivoAdjunto != idArchivo;
    });
    obsAdjuntoarchivosEncontrados();
}


function CargarArchivos(files) {
    //$('#nombreArchivos').val(e.currentTarget.files);
    var numFiles = files.length;
    var nombreArchivos = "";
    for (i = 0; i < numFiles; i++) {
        var fileFound = 0;
        $("#nombreArchivos > li").each(function (index) {

            if ($(this).find("a.btnDescargarArchivoAdjunto")[0].text == files[i].name) {
                alert('El archivo "' + files[i].name + '" ya se encuentra agregado.');
                fileFound = 1;
            }
        });

        if (fileFound == 0) {

            var liHTML = '<a href="javascript:mostrar();" class="btnDescargarArchivoAdjunto" idArchivoAdjunto="' + files[i].idArchivoAdjunto + '">' + files[i].name + '</a>' +
                '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + files[i].name + '" class="btnDeleteArchivo" /></a>';

            $('#nombreArchivos').append($('<li />').html(liHTML));


            ///  $('<li />').text(e.currentTarget.files[i].name).appendTo($('#nombreArchivos'));
        }

    }
}



