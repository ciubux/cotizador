jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición del mensaje; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        $("#btnBusquedaParametro").click();
    });   

    $("#btnBusquedaParametro").click(function () {

        $("#btnBusquedaParametro").attr("disabled", "disabled");
        $.ajax({
            url: "/Parametro/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusquedaParametro").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusquedaParametro").removeAttr("disabled");
                $("#tableParametro > tbody").empty();
                $("#tableParametro").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < list.length; i++) {

                    var ItemRow = '<tr data-expanded="true">' +

                        '<td>  ' + list[i].idParametro +
                        '<td>  <b id="codigoParametro">' + list[i].codigo +'</b> : <span id="descripcionEditable">'+ list[i].descripcion + '</span></td>' +
                        '<td id="valorEditable">' + list[i].valor + '</td>' +                       
                        '<td>' +

                        '<button type="button" id="' + list[i].idParametro + '" value="' + list[i].valor + '" name="' + list[i].descripcion+'" class="btnEditarParametro btn btn-primary">Editar</button>' +
                       
                        '</td>' +
                        '</tr>';

                    $("#tableParametro").append(ItemRow);

                }
            }
        });
    });
    
    $('body').on('click', "button.btnEditarParametro", function () {

        var idParametro = $(this).attr('id');       
        
        var valAnterior = $(this).closest('tr').find('#valorEditable').html();
        $(this).closest('tr').find('#valorEditable').html('<input id="valorFila' + idParametro + '" class="form-control" value="' + valAnterior + '">');

        $('#' + idParametro + '').before('<div id="' + idParametro +'BloqueBotones"><button type="button" style="margin-right:13px; margin-top:5px;" class="' + idParametro + ' btnFinalizarParametro btn btn-success">Finalizar</button>' + '<button type="button" style="margin-top:5px;" class="btnCancelarParametro btn btn-danger">Cancelar</button></div>');
        $('#' + idParametro + '').hide();


        var descripcionParametro = $(this).closest('tr').find('#descripcionEditable').html();        
        $(this).closest('tr').find('#descripcionEditable').html('<input id="descripcionFila' + idParametro + '" class="form-control" value="' + descripcionParametro + '">');

    });
    
    
    $('body').on('click', "button.btnCancelarParametro", function () {
        
        var idParametro = $(this).closest('td').find('button.btnEditarParametro').attr('id');

        var valAnterior = $(this).closest('td').find('button.btnEditarParametro').attr('value');
        var desAnterior = $(this).closest('td').find('button.btnEditarParametro').attr('name');

        $('div#' + idParametro + 'BloqueBotones').remove();
        $('#' + idParametro+'').show();                
        $('#valorFila' + idParametro + '').remove();
        $('#' + idParametro + '').closest('tr').find('#valorEditable').html(valAnterior);
                
        $('#descripcionFila' + idParametro + '').remove();
        $('#' + idParametro + '').closest('tr').find('span#descripcionEditable').html(desAnterior);
    });

    function validarValor(valActual, idParametro)
    {
        if (valActual== "" || valActual == null) {
            $.alert({
                title: "Valor Inválido",
                type: 'orange',
                content: 'Debe ingresar un valor para este parametro.',
                buttons: {                    
                    OK: function ()
                    {
                        $('#valorFila' + idParametro + '').focus();
                    }
                }
            });
            return false;
        }
        return true;
    }



    $('body').on('click', "button.btnFinalizarParametro", function () {

        var idParametro = $(this).closest('td').find('button.btnEditarParametro').attr('id');
        var valActual = $('input#valorFila' + idParametro + '').val();
        var desActual = $('input#descripcionFila' + idParametro + '').val(); 
        
        if (!validarValor(valActual, idParametro))
            return false;
        
        event.preventDefault();
        var codigoParametro = $('#codigoParametro').html();
        
                        $.ajax({
                            url: "/Parametro/Update",
                            type: 'POST',
                            data:
                            {
                                idParametro: idParametro,
                                valActual: valActual,
                                desActual: desActual
                            },
                            error: function () {

                                $.alert({
                                    title: 'Error',
                                    content: 'Se generó un error al intentar modificar el parametro.',
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
                                    content: 'El parametro se modifico correctamente.',
                                    type: 'green',
                                    buttons: {
                                        OK: function () {
                                            $('div#' + idParametro + 'BloqueBotones').remove();
                                            $('#' + idParametro + '').show();
                                            $('#valorFila' + idParametro + '').remove();
                                            $('#descripcionFila' + idParametro + '').remove();

                                            $('#' + idParametro + '').closest('tr').find('#valorEditable').html(valActual);
                                            $('#' + idParametro + '').closest('tr').find('span#descripcionEditable').html(desActual);
                                            
                                            $('#' + idParametro + '').attr('value', valActual);
                                            $('#' + idParametro + '').attr('name', desActual);
                                        }
                                    }
                                });
                            }
                        });         
    });


    $("#parametro_codigo").change(function () {
        changeInputString("codigo", $("#parametro_codigo").val());
    });

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Parametro/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#btnLimpiarBusquedaParametro").click(function () {
        $.ajax({
            url: "/Parametro/CleanBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });

});