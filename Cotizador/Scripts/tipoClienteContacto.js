jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición? No se guardarán los cambios';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        buscarLista();
    });   

    function buscarLista() {
        $.ajax({
            url: "/TipoClienteContacto/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $.alert({
                    title: "ERROR",
                    type: 'orange',
                    content: 'No se pudo cargar la lista de Tipos de Contacto Cliente.',
                    buttons: {
                        OK: function () {

                        }
                    }
                });
            },

            success: function (list) {

                $("#tableLista > tbody").empty();
                $("#tableLista").footable({
                    "paging": {
                        "enabled": false
                    }
                });

                for (var i = 0; i < list.length; i++) {

                    if (list[i].descripcion == 'null') list[i].descripcion = "";

                    var ItemRow = '<tr data-expanded="true">' +

                        '<td>' + list[i].idClienteContactoTipo +
                        '<td class="cctNombre" valor="' + list[i].nombre + '">' + list[i].nombre + '</span></td>' +
                        '<td class="cctDesripcion" valor="' + list[i].descripcion + '">' + list[i].descripcion + '</td>' +
                        '<td>' +

                        '<button type="button" id="' + list[i].idClienteContactoTipo + '" class="btnEditar btn btn-primary">Editar</button>' +
                        '<button type="button" id="' + list[i].idClienteContactoTipo + '" class="btnGuardar btn btn-primary" style="display: none;">Guardar</button>' +
                        '&nbsp;<button type="button" id="' + list[i].idClienteContactoTipo + '" class="btnCancelar btn btn-secondary" style="display: none;">Cancelar</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tableLista").append(ItemRow);

                }
            }
        });
    }
    
    $('body').on('click', "button.btnEditar", function () {

        var idTipo = $(this).attr('id');       
        
        $(this).closest('tr').find('.cctNombre').html('<input class="inputCctNombre" class="form-control" value="' + $(this).closest('tr').find('.cctNombre').attr('valor') + '">');
        $(this).closest('tr').find('.cctDesripcion').html('<input class="inputCctDesripcion" class="form-control" value="' + $(this).closest('tr').find('.cctDesripcion').attr('valor') + '">');

        $(this).hide();
        $(this).closest('tr').find('.btnGuardar').show();
        $(this).closest('tr').find('.btnCancelar').show();

    });
    
    
    $('body').on('click', "button.btnCancelar", function () {
        $(this).closest('tr').find('.cctNombre').html($(this).closest('tr').find('.cctNombre').attr("valor"));
        $(this).closest('tr').find('.cctDesripcion').html($(this).closest('tr').find('.cctDesripcion').attr("valor"));

        $(this).hide();
        $(this).closest('tr').find('.btnGuardar').hide();
        $(this).closest('tr').find('.btnEditar').show();        
    });

    $('body').on('click', "button.btnGuardar", function () {

        var idTipo = $(this).attr('id'); 
        var nombre = $(this).closest('tr').find('td.cctNombre .inputCctNombre').val();
        var descripcion = $(this).closest('tr').find('td.cctDesripcion .inputCctDesripcion').val(); 

        var that = this;

        if (nombre == "" || nombre == null) {
            $.alert({
                title: "Nombre Inválido",
                type: 'orange',
                content: 'Debe ingresar un nombre para el tipo de contacto.',
                buttons: {
                    OK: function () {
                        $(that).closest('tr').find('td.cctNombre .inputCctNombre').focus();
                    }
                }
            });
            return false;
        } 

        $.ajax({
            url: "/TipoClienteContacto/Update",
            type: 'POST',
            data:
            {
                idTipo: idTipo,
                nombre: nombre,
                descripcion: descripcion
            },
            error: function () {

                $.alert({
                    title: 'Error',
                    content: 'Ocurrió un error al intentar modificar el tipo de contacto cliente.',
                    type: 'red',
                    buttons: {
                        OK: function () {

                        }
                    }
                });
            },
            success: function () {

                $.alert({
                    title: TITLE_EXITO,
                    content: 'El tipo de contacto cliente se modificó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            $(that).closest('tr').find('.cctNombre').html(nombre);
                            $(that).closest('tr').find('.cctDesripcion').html(descripcion);

                            $(this).closest('tr').find('.cctNombre').attr("valor", nombre);
                            $(this).closest('tr').find('.cctDesripcion').attr("valor", descripcion);

                            $(that).hide();
                            $(that).closest('tr').find('.btnCancelar').hide();
                            $(that).closest('tr').find('.btnEditar').show();
                        }
                    }
                });

                $
            }
        });         
    });

});