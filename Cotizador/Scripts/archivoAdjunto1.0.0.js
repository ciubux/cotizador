﻿jQuery(function ($) {
    var pagina = 28;
    var ARCHIVO_ADJUNTO_CANCELAR_EDICION = '¿Está seguro de cancelar la edición del mensaje; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';
    
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
      
    $('body').on('click', ".btnDeleteArchivo", function () {
        var idArchivo = $(this).closest('li').find('a').eq(0).attr('idArchivoAdjunto');           
      
        $("#nombreArchivos > li").remove().end();
        $.ajax({
            url: "/ArchivoAdjunto/DescartarArchivos",
            type: 'POST',
            dataType: 'JSON',
            data: { idArchivo: idArchivo },
            error: function (detalle) { },
            success: function (files) {
                
                $("#nombreArchivos > li").remove().end();

                for (var i = 0; i < files.length; i++) {
                    
                        var liHTML = '<a href="javascript:mostrar();" class="btnDescargarArchivoAdjunto" idArchivoAdjunto="' + files[i].idArchivoAdjunto + '">' + files[i].nombre + '</a>' +
                            '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + files[i].nombre + '" class="btnDeleteArchivo" /></a>';

                        $('#nombreArchivos').append($('<li />').html(liHTML));
                       

                    
                }
            }
        });
    });

});

$('#fileUpload').change(function (e) {   
    if (CargarArchivos(e.currentTarget.files) == 0)
        return;
       
        $.ajax({
            url: "/ArchivoAdjunto/ChangeFiles",
            type: "POST",
            data: function () {
                var data = new FormData();
                data.append("name", jQuery("#fileUpload").val());
                data.append("file", jQuery("#fileUpload").get(0).files[0]);
                return data;
            }(),
            contentType: false,
            processData: false,
            success: function (response) {
                $('#nombreArchivos li:last').find('a').eq(0).attr("idArchivoAdjunto", response);
            },
            error: function (jqXHR, textStatus, errorMessage) {
                console.log(errorMessage);
            }
        });
     });
        



    

function archivosEncontrados(files) {

    $("#nombreArchivos>li").remove().end();

    for (var i = 0; i < files.length; i++) {
        var liHTML = '<a href = "javascript:mostrar();" class="btnDescargarArchivoAdjunto" idArchivoAdjunto="' + files[i].idArchivoAdjunto + '">' + files[i].nombre + '</a > ' +
            '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + files[i].nombre + '" class="btnDeleteArchivo" /></a>';
        $('<li/>').html(liHTML).appendTo($('#nombreArchivos'));
    }

    $("#verNombreArchivos > li").remove().end();


    for (var i = 0; i < files.length; i++) {
        var liHTML = '<a href="javascript:mostrar();" class="btnDescargarArchivoAdjunto" idArchivoAdjunto="' + files[i].idArchivoAdjunto + '">' + files[i].nombre + '</a>';
        //$('<li />').html(liHTML).appendTo($('#nombreArchivos'));
        $('#verNombreArchivos').append($('<li />').html(liHTML));
    }    
}


function CargarArchivos(files) {    
    var numFiles = files.length;
    var fileFound = 1;
    for (i = 0; i < numFiles; i++) {


        $("#nombreArchivos > li").each(function () {
            if ($(this).find("a.btnDescargarArchivoAdjunto")[0].text == files[0].name) {
                fileFound = 0;                
            }
        });
    }
        if (fileFound == 0) {
            alert('El archivo "' + files[0].name + '" ya se encuentra agregado.');  
        }

        if (fileFound == 1) {
            var liHTML = '<a href="javascript:mostrar();" class="btnDescargarArchivoAdjunto" idArchivoAdjunto="' + files[0].idArchivoAdjunto + '">' + files[0].name + '</a>' +
                '<a href="javascript:mostrar();"><img src="/images/icon-close.png"  id="' + files[0].name + '" class="btnDeleteArchivo" /></a>';

            $('#nombreArchivos').append($('<li />').html(liHTML));
            
        }
        return fileFound;
    }




