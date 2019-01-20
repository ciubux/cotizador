
jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

    $(document).ready(function () {
        $("#btnBusqueda").click();
        //cargarChosenCliente();
        verificarSiExisteCliente();
    });

    function verificarSiExisteCliente() {
        if ($("#idSubDistribuidor").val().trim() != "0") {
            $("#btnFinalizarEdicionSubDistribuidor").html('Finalizar Edición');
        }
        else {
            $("#btnFinalizarEdicionSubDistribuidor").html('Finalizar Creación');
        }

    }

    function limpiarFormulario() {
        $("#subDistribuidor_codigo").val("");
        $("#subDistribuidor_nombre").val("");
    }




    function ConfirmDialogReload(message) {
        $('<div></div>').appendTo('body')
            .html('<div><h6>' + message + '</h6></div>')
            .dialog({
                modal: true, title: 'Confirmación', zIndex: 10000, autoOpen: true,
                width: 'auto', resizable: false,
                buttons: {
                    Si: function () {
                        location.reload();
                        $(this).dialog("close");
                    },
                    No: function () {
                        $(this).dialog("close");
                    }
                },
                close: function (event, ui) {
                    $(this).remove();
                }
            });
        document.body.scrollTop = default_scrollTop;
    };

    function ConfirmDialog(message, redireccionSI, redireccionNO) {
        $('<div></div>').appendTo('body')
            .html('<div><h6>' + message + '</h6></div>')
            .dialog({
                modal: true, title: 'Confirmación', zIndex: 10000, autoOpen: true,
                width: 'auto', resizable: false,
                buttons: {
                    Si: function () {
                        if (redireccionSI != null)
                            window.location = redireccionSI;
                        $(this).dialog("close");

                    },
                    No: function () {
                        if (redireccionNO != null)
                            window.location = redireccionNO;
                        $(this).dialog("close");
                    }
                },
                close: function (event, ui) {
                    $(this).remove();
                }
            });
        document.body.scrollTop = default_scrollTop;
    };


    function validacionDatosSubDistribuidor() {
       
        if ($("#subDistribuidor_codigo").val().length < 2) {
            $.alert({
                title: "Código Inválido",
                type: 'orange',
                content: 'Debe ingresar un Código de subdistribuidor válido.',
                buttons: {
                    OK: function () { $('#subDistribuidor_codigo').focus(); }
                }
            });
            return false;
        }
        
        if ($("#subDistribuidor_nombre").val().length < 4) {
            $.alert({
                title: "Nombre Inválido",
                type: 'orange',
                content: 'Debe ingresar un nombre válido.',
                buttons: {
                    OK: function () { $('#subDistribuidor_nombre').focus(); }
                }
            });
            return false;
        }

        return true;

    }


    $("#btnFinalizarEdicionSubDistribuidor").click(function () {
        /*Si no tiene codigo el cliente se está creando*/
        if ($("#subDistribuidor_idSubDistribuidor").val() == '0') {
            crearSubDistribuidor();
        }
        else {
            editarSubDistribuidor();
        }
    });



    function crearSubDistribuidor() {
        if (!validacionDatosSubDistribuidor())
            return false;

        $('body').loadingModal({
            text: 'Creando SubDistribuidor...'
        });
        $.ajax({
            url: "/SubDistribuidor/Create",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar crear el subdistribuidor.',
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });
            },
            success: function (resultado) {
                $('body').loadingModal('hide');
                $.alert({
                    title: TITLE_EXITO,
                    content: 'El subdistribuidor se creó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/SubDistribuidor/List';
                        }
                    }
                });
            }
        });

    }

    function editarSubDistribuidor() {

        if (!validacionDatosSubDistribuidor())
            return false;


        $('body').loadingModal({
            text: 'Editando SubDistribuidor...'
        });
        $.ajax({
            url: "/SubDistribuidor/Update",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar editar el subdistribuidor.',
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });
            },
            success: function (resultado) {
                $('body').loadingModal('hide');

                $.alert({
                    title: TITLE_EXITO,
                    content: 'El subdistribuidor se editó correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/SubDistribuidor/List';
                        }
                    }
                });
            }
        });
    }

    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/SubDistribuidor/ChangeInputInt",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    function changeInputDecimal(propiedad, valor) {
        $.ajax({
            url: "/SubDistribuidor/ChangeInputDecimal",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#subDistribuidor_estado_si").click(function () {
        var valCheck = 1;
        changeInputInt("Estado", valCheck)
    });

    $("#subDistribuidor_estado_no").click(function () {
        var valCheck = 0;
        changeInputInt("Estado", valCheck)
    });

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/SubDistribuidor/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    $("#subDistribuidor_codigo").change(function () {
        changeInputString("codigo", $("#subDistribuidor_codigo").val());
    });

    $("#subDistribuidor_nombre").change(function () {
        changeInputString("nombre", $("#subDistribuidor_nombre").val());
    });

    
    $("#btnCancelarSubDistribuidor").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/SubDistribuidor/CancelarCreacionSubDistribuidor', null)
    })




    var ft = null;

    

    /*Evento que se dispara cuando se hace clic en el boton EDITAR en la edición de la grilla*/
    $(document).on('click', "button.footable-show", function () {

        //Cambiar estilos a los botones
        $("button.footable-add").attr("class", "btn btn-default footable-add");
        $("button.footable-hide").attr("class", "btn btn-primary footable-hide");


        //Se deshabilitan controles que recargan la página o que interfieren con la edición del detalle
        $("#considerarCantidades").attr('disabled', 'disabled');
        $("input[name=igv]").attr('disabled', 'disabled');
        $("#flete").attr('disabled', 'disabled');
        $("#btnOpenAgregarSubDistribuidor").attr('disabled', 'disabled');

        var codigo = $("#numero").val();
        if (codigo == "") {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarCreacionPedido").attr('disabled', 'disabled');
            $("#btnCancelPedido").attr('disabled', 'disabled');
        }
        else {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarEdicionPedido").attr('disabled', 'disabled');
            $("#btnCancelPedido").attr('disabled', 'disabled');
        }


        $("input[name=mostrarcodproveedor]").attr('disabled', 'disabled');


        if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
            console.log('Esto es un dispositivo móvil');
            return;
        }


        //llama a la función que cambia el estilo de display después de que la tabla se ha redibujado
        //Si se le llama antes el redibujo reemplazará lo definido
        window.setInterval(mostrarFlechasOrdenamiento, 600);

        /*Se agrega control input en columna cantidad*/
        var $j_object = $("td.detcantidad");
        $.each($j_object, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var cantidad = value.innerText;
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detincantidad form-control' value='" + cantidad + "' type='number'/>";
        });
        

        //   @cotizacionDetalle.producto.idProducto detproductoObservacion"




        /*Se agrega control input en columna porcentaje descuento*/
        var $j_object1 = $("td.detporcentajedescuento");
        $.each($j_object1, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var porcentajedescuento = value.innerText;
            porcentajedescuento = porcentajedescuento.replace("%", "").trim();
            $(".detporcentajedescuentoMostrar." + arrId[0]).html("<div style='width: 150px' ><div style='float:left' ><input style='width: 100px' class='" + arrId[0] + " detinporcentajedescuento form-control' value='" + porcentajedescuento + "' type='number'/></div><div > <button type='button' class='" + arrId[0] + " btnCalcularDescuento btn btn-primary bouton-image monBouton' data-toggle='modal' data-target='#modalCalculadora' ></button ></div></div>");

        });


        var $j_objectFlete = $("td.detflete");
        $.each($j_objectFlete, function (key, value) {

            var arrId = value.getAttribute("class").split(" ");
            var flete = value.innerText;
            value.innerHTML = "<input style='width: 100px' class='" + arrId[0] + " detinflete form-control' value='" + flete + "' type='number'/>";
        });



    });


    function changeInputBoolean(propiedad, valor) {
        $.ajax({
            url: "/SubDistribuidor/ChangeInputBoolean",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    
    $("#btnExportExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
    });

    $("#btnBusqueda").click(function () {
        
        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/SubDistribuidor/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (list) {
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableSubDistribuidores > tbody").empty();
                $("#tableSubDistribuidores").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < list.length; i++) {

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list[i].idSubDistribuidor + '</td>' +
                        '<td>  ' + list[i].codigo + '  </td>' +
                        '<td>  ' + list[i].nombre + '  </td>' +
                        '<td>' +
                        '<button type="button" class="' + list[i].idSubDistribuidor + ' btnEditarSubDistribuidor btn btn-primary ">Editar</button>' +
                        '</td>' +
                        '</tr>';

                    $("#tableSubDistribuidores").append(ItemRow);

                }

                if (ItemRow.length > 0) {
                    $("#msgBusquedaSinResultados").hide();
                    $("#divExportButton").show();
                }
                else {
                    $("#msgBusquedaSinResultados").show();
                    $("#divExportButton").hide();
                }

            }
        });
    });


    $(document).on('click', "button.btnEditarSubDistribuidor", function () {
      //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idSubDistribuidor = arrrayClass[0];
        
        $.ajax({
            url: "/SubDistribuidor/ConsultarSiExisteSubDistribuidor",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idSubDistribuidor: idSubDistribuidor
            },
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/SubDistribuidor/iniciarEdicionSubDistribuidor",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del SubDistribuidor."); },
                        success: function (fileName) {
                            window.location = '/SubDistribuidor/Editar';
                        }
                    });

                }
                else {
                    if (resultado.idSubDistribuidor == 0) {
                        alert('Está creando un nuevo subdistribuidor; para continuar por favor diríjase a la página "Crear/Modificar SubDistribuidor" y luego haga clic en el botón Cancelar.');
                    }
                    
                    else {
                        alert('Ya se encuentra editando un subdistribuidor para continuar por favor dirigase a la página "Crear/Modificar SubDistribuidor".');
                    }
                }
            }
        });
    });



});


