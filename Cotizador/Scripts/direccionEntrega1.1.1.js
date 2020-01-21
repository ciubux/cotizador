
jQuery(function ($) {
    var turnoTimepickerStep = 30;
    var pagina = 2;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';
    var SELECCIONE_DIRECCION_ACOPIO = "Seleccione dirección de entrega";
    
 
    cargarDireccionesEntrega();

    $("#direccionEntrega_esDireccionAcopio").change(
        function () {
            if ($('#direccionEntrega_esDireccionAcopio').is(':checked')) {
             //   $('#idDireccionAlmacen').text($("#direccionEntrega_descripcion").text());
                $("#idDireccionAlmacen").attr('disabled', 'disabled');
                $("#idDireccionAlmacen").val(GUID_EMPTY)
                $("#idDireccionAlmacen").find("option:selected").text($('#direccionEntrega_descripcion').val());
                //$('#idDireccionAlmacen option:contains("Newest")').text('TEMPEST');
            }
            else {
                $("#idDireccionAlmacen").val(GUID_EMPTY)
                $("#idDireccionAlmacen").find("option:selected").text(SELECCIONE_DIRECCION_ACOPIO);
                $("#idDireccionAlmacen").removeAttr('disabled');
            }
        }
    )

    var columns = new Array(
        { name: "idDireccionEntrega", title: "idDireccionEntrega" },
        { name: "idCliente" },
        { name: "idCiudad" },
        { name: "idDomicilioLegal" },
        { name: "codigoUbigeo" },
        { name: "codigo" },
        { name: "sede" },
        { name: "ubigeo" },
        { name: "direccionEntrega" },
        { name: "contacto" },
        { name: "telefono" },
        { name: "codigoCliente" },
        { name: "nombre" },
        { name: "codigoMP" },
        { name: "emailRecepcionFacturas" },
        { name: "direccionDomicilioLegal" }       
    );

    var columnas = '[{ "name": "idDireccionEntrega", "title": "idDireccionEntrega" },' +
        '{ "name": "idCliente" },' +
        '{ "name": "idCiudad" },' +
        '{ "name": "idDomicilioLegal" },' +
        '{ "name": "codigoUbigeo" },' +
        '{ "name": "codigo" },' +
        '{ "name": "sede" },' +
        '{ "name": "ubigeo" },' +
        '{ "name": "direccionEntrega" },' +
        '{ "name": "contacto" },' +
        '{ "name": "telefono" },' +
        '{ "name": "codigoCliente" },' +
        '{ "name": "nombre" },' +
        '{ "name": "codigoMP" },' +
        '{ "name": "emailRecepcionFacturas" },' +
        '{ "name": "direccionDomicilioLegal" }    ]';

    var $modal = $('#modalEdicionDireccionEntrega'),
        $editor = $('#editorDireccionEntrega'),
        $editorTitle = $('#modalEdicionDireccionEntregaTitle'),
        ft = FooTable.init('#tableListaDireccionesEntrega',
          //  { "columns": columns },
            {
            editing: {
                enabled: true,
                addRow: function (row) {

                    $("#idDireccionAlmacen").removeAttr('disabled');
                    $("#direccionEntrega_esDireccionAcopio").prop('checked', false);

                    $editor.find('#idDireccionEntrega').val("");
                    $modal.removeData('row');
                    $editor[0].reset();
                    $editorTitle.text('Agregando Dirección');
                    $modal.modal('show');


                },
                editRow: function (row) {
                    $('body').loadingModal({
                        text: 'Cargando Dirección'
                    });

                    var values = row.val();
                    $editor.find('#idDireccionEntrega').val(values.idDireccionEntrega.trim());
                    $editor.find('#direccionEntrega_codigo').val(values.codigo.trim());
                    //$editor.find('#idCliente').val(values.idCliente.trim());
                    //$editor.find('#direccionEntrega_cliente_ciudad_sede').val(values.cliente.ciudad.sede.trim());  


                    $("#idDireccionAlmacen").removeAttr('disabled');
                    $("#direccionEntrega_esDireccionAcopio").prop('checked', false);


                    $editor.find('#idDireccionAlmacen').val(values.idDireccionAlmacen.trim());


                    $editor.find('#idCiudad').val(values.idCiudad.trim());

                    $editor.find('#Departamento').val(values.codigoUbigeo.trim().substr(0, 2))
                    $("#Departamento").change();
                    window.setTimeout(function () {
                        $editor.find('#Provincia').val(values.codigoUbigeo.trim().substr(0, 4));
                        $("#Provincia").change();
                    }, 2500);
                    window.setTimeout(function () {
                        $editor.find('#Distrito').val(values.codigoUbigeo.trim().substr(0, 6));
                        $('body').loadingModal('hide');
                    }, 5000);
                    $editor.find('#direccionEntrega_descripcion').val(values.direccionEntrega.trim());
                    $editor.find('#direccionEntrega_contacto').val(values.contacto.trim());
                    $editor.find('#direccionEntrega_telefono').val(values.telefono.trim());
                    $editor.find('#direccionEntrega_emailRecepcionFacturas').val(values.emailRecepcionFacturas.trim());
                    $editor.find('#direccionEntrega_codigoCliente').val(values.codigoCliente.trim());
                    $editor.find('#direccionEntrega_codigoMP').val(values.codigoMP.trim());
                    $editor.find('#direccionEntrega_nombre').val(values.nombre.trim());
                    $editor.find('#direccionEntrega_idDomicilioLegal').val(values.idDomicilioLegal.trim());
                    $modal.data('row', row);
                    $editorTitle.text('Editando Dirección de Establecimiento: ' + values.direccionEntrega);
                    $modal.modal('show');
                },
                deleteRow: function (row) {

                    var values = row.val();

                    $.confirm({
                        title: 'Confirmación',
                        content: '¿Está seguro de eliminar la Dirección de Entrega: ' + values.direccionEntrega.trim() + '?',
                        type: 'orange',
                        buttons: {
                            confirm: {
                                text: 'Sí',
                                btnClass: 'btn-red',
                                action: function () {
                                    var values = row.val();
                                    var idDireccionEntrega = values.idDireccionEntrega.trim();
                                    $('body').loadingModal({
                                        text: 'Eliminando Dirección de Entrega'
                                    });

                                    $.ajax({
                                        url: "/DireccionEntrega/Delete",
                                        type: 'POST',
                                        data: {
                                            idDireccionEntrega: idDireccionEntrega
                                        },
                                        error: function (detalle) {
                                            $('body').loadingModal('hide')
                                            mostrarMensajeErrorProceso(detalle.responseText);
                                        },
                                        success: function () {
                                            row.delete();
                                            $.alert({
                                                title: "Operación exitosa",
                                                type: 'green',
                                                content: "Se eliminó la dirección correctamente.",
                                                buttons: {
                                                    OK: function () { }
                                                }
                                            });
                                            $('body').loadingModal('hide')
                                        }
                                    });
                                }
                            },
                            cancel: {
                                text: 'No',
                                action: function () {
                                }
                            }
                        },
                    });
                }
                }, "paging": {
                    "enabled": true
                }
        }),
        // this example does not send data to the server so this variable holds the integer to use as an id for newly
        // generated rows. In production this value would be returned from the server upon a successful ajax call.

     
        uid = 10;



    $editor.on('submit', function (e) {
       
        if (this.checkValidity && !this.checkValidity()) return; // if validation fails exit early and do nothing.
        e.preventDefault(); // stop the default post back from a form submit
        var codigoUbigeo = $editor.find('#Distrito').val();

        var departamento = $('#Departamento option:selected').text();
        var provincia = $('#Provincia option:selected').text();
        var distrito = $('#Distrito option:selected').text();
    
        var direccionEntrega = $editor.find('#direccionEntrega_descripcion').val();
        var idDomicilioLegal = $editor.find('#direccionEntrega_idDomicilioLegal').val();
        var domicilioLegal = $editor.find('#direccionEntrega_idDomicilioLegal option:selected').text();
        //$('#idCiudad').change();
    //    var sede = $editor.find('#idCiudad option:selected').text();
      //  alert(sede)
       // return;


      //  sede = sede.split("(")[1].substr(0, 3);

        var idDireccionAlmacen = $editor.find('#idDireccionAlmacen').val();
        var direccionAlmacen = $('#idDireccionAlmacen option:selected').text();

        var idDireccionEntrega = $editor.find('#idDireccionEntrega').val();

        var esDireccionAcopio = $editor.find('#direccionEntrega_esDireccionAcopio').prop('checked');
     
     //   alert("asa1")
        var idCiudad = $editor.find('#idCiudad').val();
     //   alert("asa2")
        var contacto = $editor.find('#direccionEntrega_contacto').val();
     //   alert("asa3")
        var telefono = $editor.find('#direccionEntrega_telefono').val();
     
        var codigoCliente = $editor.find('#direccionEntrega_codigoCliente').val();
        var nombre = $editor.find('#direccionEntrega_nombre').val();
        var codigo = $editor.find('#direccionEntrega_codigo').val();
        var emailRecepcionFacturas = $editor.find('#direccionEntrega_emailRecepcionFacturas').val();

      /*
        if (idCiudad == null || idCiudad == "") {
            $.alert({
                title: "Validación",
                type: 'orange',
                content: "Debe seleccionar la Sede MP",
                buttons: {
                    OK: function () { }
                }
            });
            $("#Distrito").focus();
            return;
        }*/

        if (codigoUbigeo == null || codigoUbigeo == "") {
            $.alert({
                title: "Validación",
                type: 'orange',
                content: "Debe seleccionar el Departamento, Provincia y Distrito",
                buttons: {
                    OK: function () { }
                }
            });
            $("#Distrito").focus();
            return;
        }

        if (direccionEntrega == null || direccionEntrega.trim().length < 5) {
            $.alert({
                title: "Validación",
                type: 'orange',
                content: "Debe ingresar una dirección válida",
                buttons: {
                    OK: function () { }
                }
            });
            $("#direccionEntrega_descripcion").focus();
            return;
        }

        if (idDireccionAlmacen == GUID_EMPTY && !$('#direccionEntrega_esDireccionAcopio').is(':checked')) {
            $.alert({
                title: "Validación",
                type: 'orange',
                content: "Debe seleccionar la dirección de Acopio",
                buttons: {
                    OK: function () { }
                }
            });
            $("#idDireccionEntrega").focus();
            return;
        }

        if (idDomicilioLegal == null || idDomicilioLegal == "") {
            $.alert({
                title: "Validación",
                type: 'orange',
                content: "Debe seleccionar el Domicilio Legal",
                buttons: {
                    OK: function () { }
                }
            });
            $("#direccionEntrega_idDomicilioLegal").focus();
            return;
        }


                        

        var row = $modal.data('row'),
            values = {
                idDireccionEntrega: idDireccionEntrega,
               // idCliente: idCliente,
                idCiudad: idCiudad,
                idDomicilioLegal: idDomicilioLegal,
                codigoUbigeo: codigoUbigeo,
                codigo: codigo,
              //  sede: sede,
                ubigeo: departamento + ' - ' + provincia + ' - ' + distrito,
                direccionEntrega: direccionEntrega,
                contacto: contacto,
                telefono: telefono,
                codigoCliente: codigoCliente,
                nombre: nombre,
                emailRecepcionFacturas: emailRecepcionFacturas,
                direccionDomicilioLegal: domicilioLegal,
                direccionAlmacen: direccionAlmacen,
                idDireccionAlmacen: idDireccionAlmacen,
                esDireccionAcopio: esDireccionAcopio
                
            };

        if (row instanceof FooTable.Row) {
            $('body').loadingModal({
                text: 'Modificando Dirección de Entrega'
            });
            $.ajax({
                url: "/DireccionEntrega/Update",
                type: 'POST',
                dataType: 'JSON',
                data: {
                    idDireccionEntrega: idDireccionEntrega,
                    ubigeo: codigoUbigeo,
                    direccion: direccionEntrega,
                    contacto: contacto,
                    telefono: telefono,
                    emailRecepcionFacturas: emailRecepcionFacturas,
                    codigoCliente: codigoCliente,
                    nombre: nombre,
                    idDomicilioLegal: idDomicilioLegal,
                    departamento: departamento,
                    provincia: provincia,
                    distrito: distrito,
                    direccionAlmacen: direccionAlmacen,
                    idDireccionAlmacen: idDireccionAlmacen,
                    esDireccionAcopio: esDireccionAcopio
                },
                error: function (detalle) {
                    $('body').loadingModal('hide')
                    mostrarMensajeErrorProceso(detalle.responseText);
                },
                success: function (direccion) {
                    values.sede = direccion.cliente.ciudad.sede
                    row.val(values);
                    $modal.modal('hide');
                    $('body').loadingModal('hide')
                    $.alert({
                        title: "Operación exitosa",
                        type: 'green',
                        content: "Se actualizó la dirección correctamente.",
                        buttons: {
                            OK: function () { }
                        }
                    });


                }
            });

        } else {
            $('body').loadingModal({
                text: 'Creando Dirección de Entrega'
            });
            $.ajax({
                url: "/DireccionEntrega/Create",
                type: 'POST',
                dataType: 'JSON',
                data: {
                    ubigeo: codigoUbigeo,
                    direccion: direccionEntrega,
                    contacto: contacto,
                    telefono: telefono,
                    emailRecepcionFacturas: emailRecepcionFacturas,
                    codigoCliente: codigoCliente,
                    nombre: nombre,
                    idDomicilioLegal: idDomicilioLegal,
                    departamento: departamento,
                    provincia: provincia,
                    distrito: distrito,
                    direccionAlmacen: direccionAlmacen,
                    idDireccionAlmacen: idDireccionAlmacen,
                    esDireccionAcopio: esDireccionAcopio
                },
                error: function (detalle) {
                    $('body').loadingModal('hide')
                    mostrarMensajeErrorProceso(detalle.responseText);
                },
                success: function (direccion) {
                    values.id = uid++;
                    values.idDireccionEntrega = direccion.idDireccionEntrega;
                    values.codigo = direccion.codigo;
                    values.sede = direccion.cliente.ciudad.sede
                    ft.rows.add(values);
                    $modal.modal('hide');
                    $('body').loadingModal('hide')
                    $.alert({
                        title: "Operación exitosa",
                        type: 'green',
                        content: "Se creó la dirección correctamente.",
                        buttons: {
                            OK: function () { }
                        }
                    });
                }
            });



        }
    });



    function loadDireccionesEntrega(arrayDireccionEntrega) {

        ft.rows.load(arrayDireccionEntrega, false);
    }
    





    $(document).ready(function () {
        //cargarChosenCliente();
       
        

        $('.timepicker').timepicker({
            timeFormat: 'HH:mm ',
            interval: turnoTimepickerStep,
            minTime: '6:00',
            maxTime: '22:00',
            startTime: '06:00',
            dynamic: false,
            dropdown: true,
            scrollbar: true,
            change: function (time) {
                $(this).change();
            }
        });

    });

    var fechaDesde = $("#fechaVentasDesdetmp").val();
    $("#fechaVentasDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaDesde);


    function verificiarSiFechaVentasEsModificada() {
        if ($('#chkFechaVentasEsModificada').prop('checked')) {
            $("#fechaVentasDesde").removeAttr("disabled");
        }
        else {
            $("#fechaVentasDesde").attr('disabled', 'disabled');
        }
    }


    $('#direccionEntrega_codigoCliente').change(function () {       
        $('#direccionEntrega_codigoMP').val($("#direccionEntrega_codigoCliente").val())
    });




    $('#chkFechaVentasEsModificada').change(function () {
        var fechaEsModificada = 1;
        if ($('#chkFechaVentasEsModificada').prop('checked')) {
            $("#fechaVentasDesde").removeAttr("disabled");

            //$("#fechaVentasDesde").datepicker().datepicker("setDate", new Date());
        }
        else {
            $("#fechaVentasDesde").attr('disabled', 'disabled');
            $("#fechaVentasDesde").val("");
            fechaEsModificada = 0;
        }
        $("#fechaVentasDesde").change();
        changeInputBoolean("fechaVentasEsModificada", fechaEsModificada)
    });

    $('#fechaVentasDesde').change(function () {
        changeInputDate("fechaVentasDesde", $("#fechaVentasDesde").val())
    });

    function changeInputDate(propiedad, valor) {
        $.ajax({
            url: "/Cliente/ChangeInputDate",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }




    function verificarSiExisteCliente() {
        if ($("#idCliente").val().trim() != GUID_EMPTY) {
            $("#idCiudad").attr("disabled", "disabled");
            $("#tipoDocumentoIdentidad").attr("disabled", "disabled");
            $("#cliente_ruc").attr("disabled", "disabled");
            $("#btnFinalizarEdicionCliente").html('Finalizar Edición');            
        }
        else {
            //Si recién se está creando el cliente, el usuario puede seleccionar el responsable comercial
            $("#idResponsableComercial").removeAttr("disabled");
            $("#btnFinalizarEdicionCliente").html('Finalizar Creación');
        }

    }

    function mostrarCamposParaClienteConRUC() {

    }


    $("#tipoDocumentoIdentidad").change(function () {


        var tipoDocumentoIdentidad = $("#tipoDocumentoIdentidad").val();

        if ($("#cliente_codigo").val().length > 0) {
            $.confirm({
                title: 'Confirmación',
                content: '¿Está seguro de cambiar de tipo de Documento?',
                type: 'orange',
                buttons: {           
                    confirm: {
                        text: 'Sí',
                        btnClass: 'btn-red',
                        action: function () {
                            cambiarTipoDocumentoIdentidad(tipoDocumentoIdentidad);
                            limpiarFormulario();
                            mostrarCamposSegunTipoDocIdentidad();
                        }
                    },
                    cancel: {
                        text: 'No',
                        //btnClass: 'btn-blue',
//                        keys: ['enter', 'shift'],
                        action: function () {
                            location.reload();
                        }
                    }
                },
               
            });

           
        }
        else {
            cambiarTipoDocumentoIdentidad(tipoDocumentoIdentidad);
            limpiarFormulario();
            mostrarCamposSegunTipoDocIdentidad();
        }
        
    });

    function cambiarTipoDocumentoIdentidad(tipoDocumentoIdentidad) {
        $.ajax({
            url: "/Cliente/changeTipoDocumentoIdentidad",
            type: 'POST',
            data: { tipoDocumentoIdentidad: tipoDocumentoIdentidad },
            success: function () {

            },
            error: function () {
                $.alert({
                    title: 'Error',
                    content: MENSAJE_ERROR,
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });
            }
        });

    }

    function mostrarCamposSegunTipoDocIdentidad() {


        var tipoDocumentoIdentidad = $("#tipoDocumentoIdentidad").val();
      
        if (tipoDocumentoIdentidad == CONS_TIPO_DOC_CLIENTE_DNI
            || tipoDocumentoIdentidad == CONS_TIPO_DOC_CLIENTE_CARNET_EXTRANJERIA) {
            $("#labelClienteNombre").html("Nombres y Apellidos");
            $("#fieldSetDatosSunat").hide();
            $("#btnRecuperarDatosSunat").hide();
            $("#divContinueEdit").removeAttr("disabled");
           
        }
        else if (tipoDocumentoIdentidad == CONS_TIPO_DOC_CLIENTE_RUC) {
            $("#labelClienteNombre").html("Nombre Comercial");
            $("#fieldSetDatosSunat").show(); 
            $("#fieldSetDatosSunat").show();
            
       /*     var rSunat = $("#cliente_razonSocialSunat").val();
            if (rSunat.trim() == "") {
                $("#divContinueEdit").attr("disabled", "disabled");
                $("#cliente_ruc").removeAttr("disabled");
                $("#tipoDocumentoIdentidad").removeAttr("disabled");
            } else {
                $("#cliente_ruc").attr("disabled", "disabled");
                $("#tipoDocumentoIdentidad").attr("disabled", "disabled");
                $("#divContinueEdit").removeAttr("disabled");
            }*/
        }


    }


    function limpiarFormulario() {

        $("#cliente_ruc").val("");
        $("#cliente_razonSocial").val("");
        $("#cliente_nombreComercial").val("");
        $("#cliente_domicilioLegal").val("");
        $("#cliente_contacto1").val("");
        $("#cliente_telefonoContacto1").val("");
        $("#cliente_emailContacto1").val("");
        $("#cliente_correoEnvioFactura").val("");
        $("#cliente_razonSocialSunat").val("");
        $("#cliente_nombreComercialSunat").val("");
        $("#cliente_direccionDomicilioLegalSunat").val("");
        $("#cliente_estadoContribuyente").val("");
        $("#cliente_condicionContribuyente").val("");

        $("#cliente_ubigeo_Departamento").val("");
        $("#cliente_ubigeo_Provincia").val("");
        $("#cliente_ubigeo_Distrito").val("");

        $("#cliente_plazoCredito").val("");
        $("#tipoPagoCliente").val("0");
        $("#formaPagoCliente").val("0");


        $("#idGrupoCliente").val("");
        $("#sinPlazoCreditos").removeAttr("checked");
        $("#bloqueadoBusqueda").removeAttr("checked");
        $("#sinAsesorValidado").removeAttr("checked");
    }


    $("#btnLimpiarBusqueda").click(function () {
        $.ajax({
            url: "/Cliente/CleanBusqueda",
            type: 'POST',
            success: function () {
                location.reload();
            }
        });
    });

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

  
   



    $("#btnRecuperarDatosSunat").click(function () {

        var ruc = $("#cliente_ruc").val();
        
        if (ruc.length != 11) {
            $.alert({
                title: "RUC Inválido",
                type: 'orange',
                content: 'Debe ingresar un número de RUC válido.',
                buttons: {
                    OK: function () { }
                }
            });
            $('#cliente_ruc').focus();
            return false;
        }

        $.ajax({
            url: "/Cliente/GetDatosSunat",
            type: 'POST',
            dataType: 'JSON',
            data: { ruc: ruc },
            success: function (cliente) {
                $("#cliente_razonSocialSunat").val(cliente.razonSocialSunat);
                $("#cliente_nombreComercial").val(cliente.nombreComercial);                
                $("#cliente_nombreComercialSunat").val(cliente.nombreComercialSunat);
                $("#cliente_direccionDomicilioLegalSunat").val(cliente.direccionDomicilioLegalSunat);
                $("#cliente_ubigeo_Departamento").val(cliente.ubigeo.Departamento);
                $("#cliente_ubigeo_Provincia").val(cliente.ubigeo.Provincia);
                $("#cliente_ubigeo_Distrito").val(cliente.ubigeo.Distrito);
                $("#cliente_estadoContribuyente").val(cliente.estadoContribuyente);
                $("#cliente_condicionContribuyente").val(cliente.condicionContribuyente);
                $("#divContinueEdit").removeAttr("disabled");
                $("#cliente_ruc").attr("disabled", "disabled");
                $("#tipoDocumentoIdentidad").attr("disabled", "disabled");
               // $("#btnRecuperarDatosSunat").hide();
            },
            error: function () {
                $.alert({
                    title: 'Error',
                    content: MENSAJE_ERROR,
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });       
            }
        });
    });


    
    /**
     * ################################ INICIO CONTROLES DE CLIENTE
     */

   


    function toggleControlesUbigeo() {
        //Si cliente esta Seleccionado se habilitan la seleccion para la direccion de entrega
        if ($("#idCliente").val().trim() == "") {
            $("#ActualDepartamento").attr('disabled', 'disabled');
            $('#ActualProvincia').attr('disabled', 'disabled');
            $('#ActualDistrito').attr('disabled', 'disabled');
            $("#pedido_direccionEntrega").attr('disabled', 'disabled');
            $("#btnAgregarDireccion").attr("disabled", "disabled");
        }
        else {
            $('#ActualDepartamento').removeAttr('disabled');
            $('#ActualProvincia').removeAttr('disabled');
            $('#ActualDistrito').removeAttr('disabled');
            $("#pedido_direccionEntrega").removeAttr('disabled');
            $("#btnAgregarDireccion").removeAttr('disabled');
        }
        toggleControlesDireccionEntrega();
    }


    

    $("#idCliente").change(function () {

        var idCliente = $(this).val();

         $('body').loadingModal({
            text: 'Recuperando Datos del Cliente...'
        });

        $.ajax({
            url: "/Cliente/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente) {
                limpiarFormulario();
                
                $('body').loadingModal('hide')

                $("#cliente_codigo").val(cliente.codigo);
                $("#cliente_ruc").val(cliente.ruc);
                $("#cliente_razonSocial").val(cliente.razonSocial);
                $("#cliente_nombreComercial").val(cliente.nombreComercial);
                $("#cliente_domicilioLegal").val(cliente.domicilioLegal);
                $("#cliente_contacto1").val(cliente.contacto1);         
                $("#cliente_telefonoContacto1").val(cliente.telefonoContacto1);
                $("#cliente_emailContacto1").val(cliente.emailContacto1);
                $("#cliente_correoEnvioFactura").val(cliente.correoEnvioFactura);
                $("#cliente_razonSocialSunat").val(cliente.razonSocialSunat);
                $("#cliente_nombreComercialSunat").val(cliente.nombreComercialSunat);
                $("#cliente_direccionDomicilioLegalSunat").val(cliente.direccionDomicilioLegalSunat);
                $("#cliente_estadoContribuyente").val(cliente.estadoContribuyente);
                $("#cliente_condicionContribuyente").val(cliente.condicionContribuyente);

                $("#cliente_ubigeo_Departamento").val(cliente.ubigeo.Departamento);
                $("#cliente_ubigeo_Provincia").val(cliente.ubigeo.Provincia);
                $("#cliente_ubigeo_Distrito").val(cliente.ubigeo.Distrito);

                $("#tipoDocumentoIdentidad").val(cliente.tipoDocumentoIdentidad);
                $("#formaPagoCliente").val(cliente.formaPagoFactura);

                /*Plazos de Crédito*/
                $("#plazoCreditoSolicitado").val(cliente.plazoCreditoSolicitado);
                $("#tipoPagoCliente").val(cliente.tipoPagoFactura);
                $("#cliente_sobrePlazo").val(cliente.sobrePlazo);

                /*Montos de Crédito*/
                $("#cliente_creditoSolicitado").val(cliente.creditoSolicitado);
                $("#cliente_creditoAprobado").val(cliente.creditoAprobado);
                $("#cliente_sobreGiro").val(cliente.sobreGiro);

                /*Vendedores*/

                if (cliente.usuario.defineResponsableComercial || !cliente.vendedoresAsignados)
                {
                    $("#idResponsableComercial").removeAttr("disabled");
                }
                else
                {
                    $("#idResponsableComercial").attr("disabled", "disabled");
                }

                if (cliente.vendedoresAsignados) {
             
                    $("#spanVendedoresAsignados").show();
                    $("#spanVendedoresNoAsignados").hide();
                }
                else {
                    $("#spanVendedoresAsignados").hide();
                    $("#spanVendedoresNoAsignados").show();
                }

                $("#idResponsableComercial").val(cliente.responsableComercial.idVendedor);
                $("#idSupervisorComercial").val(cliente.supervisorComercial.idVendedor);
                $("#idAsistenteServicioCliente").val(cliente.asistenteServicioCliente.idVendedor);

                $("#cliente_observacionesCredito").val(cliente.observacionesCredito);
                $("#cliente_observaciones").val(cliente.observaciones);




                verificarSiExisteCliente();
                mostrarCamposSegunTipoDocIdentidad();
            }
        });
      
    });



 /*
    $(window).on("paste", function (e) {
        
            $.each(e.originalEvent.clipboardData.items, function () {
                this.getAsString(function (str) {
                    //alert(str);
                    var lineas = str.split("\t");           
                        
                    if (lineas.length <= 1)
                        return false;
                    
              //      if (lineas[1] == $("#cliente_ruc").val()) {
                  //      $("#ncRUC").val(lineas[1]);

                 //       $("#cliente_razonSocialSunat").val(lineas[0]);
                        //$("#ncRUC").val(lineas[1]);
                    var direccionDomicilioLegalSunat = lineas[0]+ " " + lineas[1] + " - " + lineas[2] + " - " + lineas[3];



                        $('body').loadingModal({
                            text: 'Recuperando Ubicación Geográfica...'
                        });
                        $.ajax({
                            url: "/Cliente/ChangeDireccionDomicilioLegalSunat",
                            type: 'POST',
                            dataType: 'JSON',
                            data: {
                                direccionDomicilioLegalSunat: direccionDomicilioLegalSunat
                            },
                            erro: function () {
                                $('body').loadingModal('hide')
                            },

                            success: function (resultado) {
                                $('body').loadingModal('hide')
                                $("#cliente_direccionDomicilioLegalSunat").val(resultado.direccionDomicilioLegalSunat);
                                $("#cliente_ubigeo_Departamento").val(resultado.ubigeo.Departamento);
                                $("#cliente_ubigeo_Provincia").val(resultado.ubigeo.Provincia);
                                $("#cliente_ubigeo_Distrito").val(resultado.ubigeo.Distrito);
                            }
                        });

                        changeInputString("estadoContribuyente", $("#cliente_estadoContribuyente").val())
                        changeInputString("condicionContribuyente", $("#cliente_condicionContribuyente").val())
                
                });
            });

            
    });
    */



    function validacionDatosCliente()
    {
        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            $.alert({
                title: "No selecionó Ciudad",
                type: 'orange',
                content: 'Debe seleccionar la sede MP previamente.',
                buttons: {
                    OK: function () { }
                }
            });
            $("#idCiudad").focus()
            return false;
        }


        var tipoDocumentoIdentidad = $("#tipoDocumentoIdentidad").val();
        var ruc = $("#cliente_ruc").val();

        if (tipoDocumentoIdentidad == CONS_TIPO_DOC_CLIENTE_DNI)
        {
            if (ruc.length != 8) {
                $.alert({
                    title: "DNI Inválido", type: 'orange',
                    content: 'El número de DNI debe tener 8 dígitos.',
                    buttons: { OK: function () { $('#cliente_ruc').focus(); }
                    }
                });
            }

            if ($("#cliente_nombreComercial").val().length == 0) {
                $.alert({
                    title: "Nombre Cliente Inválido",
                    type: 'orange',
                    content: 'No existe Nombre Cliente, debe ingresar el Nombre del Cliente"',
                    buttons: {
                        OK: function () { $('#cliente_razonSocialSunat').focus(); }
                    }
                });
                return false;
            }
        }
        else if (tipoDocumentoIdentidad == CONS_TIPO_DOC_CLIENTE_RUC) {
            if (ruc.length != 11) {
                $.alert({
                    title: "DNI Inválido", type: 'orange',
                    content: 'El número de DNI debe tener 8 dígitos.',
                    buttons: {
                        OK: function () { $('#cliente_ruc').focus(); }
                    }
                });
            }
            /*
            if ($("#cliente_correoEnvioFactura").val().trim().length < 8) {
                $.alert({
                    title: "Correo Electrónico Inválido",
                    type: 'orange',
                    content: 'Se debe agregar el correo electrónico para el envío de factura',
                    buttons: {
                        OK: function () { $('#cliente_correoEnvioFactura').focus(); }
                    }
                });

                return false;
            }*/

            if ($("#cliente_direccionDomicilioLegalSunat").val().length > 120) {
                $.alert({
                    title: "Dirección Inválida",
                    type: 'orange',
                    content: 'La dirección del domicilio legal obtenido de Sunat debe contener solo 120 caracteres.',
                    buttons: {
                        OK: function () { $('#cliente_direccionDomicilioLegalSunat').focus(); }
                    }
                });

                return false;
            }

            if ($("#cliente_razonSocialSunat").val().length == 0) {
                $.alert({
                    title: "No existe Razón Social",
                    type: 'orange',
                    content: 'No existe Razón Social, debe hacer clic en el botón "Obtener Datos Sunat"',
                    buttons: {
                        OK: function () { $('#cliente_razonSocialSunat').focus(); }
                    }
                });

                return false;
            }
        }
        else if (tipoDocumentoIdentidad == CONS_TIPO_DOC_CLIENTE_CARNET_EXTRANJERIA) {
            if (ruc.length > 12 || ruc.length < 0) {
                $.alert({
                    title: "Carnet Extranjería Inválido", type: 'orange',
                    content: 'El número de Carnet Extranjería debe tener 12 caracteres como máximo.',
                    buttons: {
                        OK: function () { $('#cliente_ruc').focus(); }
                    }
                });
            }

            if ($("#cliente_nombreComercial").val().length == 0) {
                $.alert({
                    title: "Nombre Cliente Inválido",
                    type: 'orange',
                    content: 'No existe Nombre Cliente, debe ingresar el Nombre del Cliente"',
                    buttons: {
                        OK: function () { $('#cliente_nombreComercial').focus(); }
                    }
                });
                return false;
            }
        }

        if ($("#plazoCreditoSolicitado").is(':enabled')) {

            if ($("#plazoCreditoSolicitado").val() == "0") {
                $.alert({
                    title: "Plazo Crédito Solicitado Inválido",
                    type: 'orange',
                    content: 'Debe indicar el plazo de crédito Solicitado',
                    buttons: {
                        OK: function () { $('#plazoCreditoSolicitado').focus(); }
                    }
                });
                return false;
            }
        }

      /*  if ($("#idResponsableComercial").val().trim() == 0) {
            $.alert({
                title: "Responsable Comercial Inválido",
                type: 'orange',
                content: 'Debe seleccionar al Responsable Comercial',
                buttons: {
                    OK: function () { $('#idResponsableComercial').focus(); }
                }
            });
            return false;
        }        */
            //Si se ha seleccionado que es subdistribuidor se debe indicar la catergoría
        if ($('#cliente_esSubdistribuidor').prop('checked') && $('#idSubDistribuidor').val() == "") {

            $.alert({
                title: "Categoría Sub Distribuidor no seleccionado",
                type: 'orange',
                content: 'Debe indicar la Categoría del Sub Distribuidor',
                buttons: {
                    OK: function () { $('#idSubDistribuidor').focus(); }
                }
            });
            return false;
        }
      
        


        return true;

    }


    $("#btnFinalizarEdicionCliente").click(function () {
        /*Si no tiene codigo el cliente se está creando*/
        if ($("#cliente_codigo").val().length == 0) {
            crearCliente();
        }
        else {
            editarCliente();
        }
    });

    
    

    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/Cliente/ChangeInputInt",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#cliente_sobrePlazo").change(function () {
        changeInputInt("sobrePlazo", $("#cliente_sobrePlazo").val())
    });

    function changeInputDecimal(propiedad, valor) {
        $.ajax({
            url: "/Cliente/ChangeInputDecimal",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#cliente_creditoSolicitado").change(function () {
        changeInputDecimal("creditoSolicitado", $("#cliente_creditoSolicitado").val())
    });

    $("#cliente_creditoAprobado").change(function () {
        changeInputDecimal("creditoAprobado", $("#cliente_creditoAprobado").val())
    });

    $("#cliente_sobreGiro").change(function () {
        changeInputDecimal("sobreGiro", $("#cliente_sobreGiro").val())
    });

    $("#cliente_observacionHorarioEntrega").change(function () {
        changeInputString("observacionHorarioEntrega", $("#cliente_observacionHorarioEntrega").val())
    });

    /*
    function changeInputTime(propiedad, valor) {
        $.ajax({
            url: "/Cliente/ChangeInputTime",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }*/



    $("#cliente_horaInicioPrimerTurnoEntregaFormat").change(function () {
        var hour = $("#cliente_horaInicioPrimerTurnoEntregaFormat").val().trim();
        var hourPost = $("#cliente_horaFinPrimerTurnoEntregaFormat").val().trim();
        if (validateHourFormat(hour)) {
            changeInputString("horaInicioPrimerTurnoEntrega", hour);


            if (!validateHourFormat(hourPost) || !validateHoraPosterior(hour, hourPost)) {
                hour = hour.split(":");

                var hTime = parseInt(hour[0]);
                var mTime = parseInt(hour[1]);

                var txtHoruPost = "";

                if (hTime >= 23) {
                    var txtHoruPost = "23:59";
                } else {
                    var txtHoruPost = "";
                    hTime = hTime + 1;
                    if (hTime < 10) {
                        txtHoruPost = "0" + hTime;
                    } else {
                        txtHoruPost = hTime;
                    }
                    txtHoruPost = txtHoruPost + ":";
                    if (mTime < 10) {
                        txtHoruPost = txtHoruPost + "0" + mTime;
                    } else {
                        txtHoruPost = txtHoruPost + mTime;
                    }
                }

                $("#cliente_horaFinPrimerTurnoEntregaFormat").val(txtHoruPost);
                changeInputString("horaFinPrimerTurnoEntrega", txtHoruPost);

                $("#cliente_horaInicioSegundoTurnoEntregaFormat").val("");
                changeInputString("horaInicioSegundoTurnoEntrega", "");

                $("#cliente_horaFinSegundoTurnoEntregaFormat").val("");
                changeInputString("horaFinSegundoTurnoEntrega", "");
            }
        } else {
            $("#cliente_horaInicioPrimerTurnoEntregaFormat").val("09:00");
            changeInputString("horaInicioPrimerTurnoEntrega", "09:00");
            $.alert({
                title: "Hora Invalida",
                type: 'orange',
                content: 'Debe seleccionar una hora de la lista.',
                buttons: {
                    OK: function () { }
                }
            });
        }
    });

    $("#cliente_horaFinPrimerTurnoEntregaFormat").change(function () {
        var hourPrev = $("#cliente_horaInicioPrimerTurnoEntregaFormat").val().trim();
        var hour = $("#cliente_horaFinPrimerTurnoEntregaFormat").val().trim();
        var hourPost = $("#cliente_horaInicioSegundoTurnoEntregaFormat").val().trim();

        if (validateHourFormat(hour)) {
            var valid = true;
            if (!validateHourFormat(hourPrev)) {
                valid = false;
                $.alert({
                    title: "Hora Invalida",
                    type: 'orange',
                    content: 'Error en los horarios.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }

            if (!validateHoraPosterior(hourPrev, hour)) {
                valid = false;
                $.alert({
                    title: "Hora Invalida",
                    type: 'orange',
                    content: 'Debe seleccionar una hora mayor a la hora inicial del primer turno.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }

            if (valid) {
                changeInputString("horaFinPrimerTurnoEntrega", $("#cliente_horaFinPrimerTurnoEntregaFormat").val());

                if (!validateHourFormat(hourPost) || !validateHoraPosterior(hour, hourPost)) {
                    $("#cliente_horaInicioSegundoTurnoEntregaFormat").val("");
                    changeInputString("horaInicioSegundoTurnoEntrega", "");

                    $("#cliente_horaFinSegundoTurnoEntregaFormat").val("");
                    changeInputString("horaFinSegundoTurnoEntrega", "");
                }
            } else {
                $("#cliente_horaFinPrimerTurnoEntregaFormat").val("18:00");
                changeInputString("horaFinPrimerTurnoEntrega", "18:00");

                $("#cliente_horaInicioSegundoTurnoEntregaFormat").val("");
                changeInputString("horaInicioSegundoTurnoEntrega", "");

                $("#cliente_horaFinSegundoTurnoEntregaFormat").val("");
                changeInputString("horaFinSegundoTurnoEntrega", "");
            }
        } else {
            $("#cliente_horaFinPrimerTurnoEntregaFormat").val("18:00");
            changeInputString("horaFinPrimerTurnoEntrega", "18:00");
            $.alert({
                title: "Hora Invalida",
                type: 'orange',
                content: 'Debe seleccionar una hora de la lista.',
                buttons: {
                    OK: function () { }
                }
            });
        }
    });


    $("#cliente_horaInicioSegundoTurnoEntregaFormat").change(function () {
        
        var hourPrev = $("#cliente_horaFinPrimerTurnoEntregaFormat").val().trim();
        var hour = $("#cliente_horaInicioSegundoTurnoEntregaFormat").val().trim();
        var hourPost = $("#cliente_horaFinSegundoTurnoEntregaFormat").val().trim();
        
        if (validateHourFormat(hour)) {
            var valid = true;
            if (!validateHourFormat(hourPrev)) {
                valid = false;
                $.alert({
                    title: "Hora Invalida",
                    type: 'orange',
                    content: 'Error en los horarios.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }

            if (!validateHoraPosterior(hourPrev, hour)) {
                valid = false;
                $.alert({
                    title: "Hora Invalida",
                    type: 'orange',
                    content: 'Debe seleccionar una hora mayor a la hora de final del primer turno.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }

            if (valid) {
                changeInputString("horaInicioSegundoTurnoEntrega", $("#cliente_horaInicioSegundoTurnoEntregaFormat").val());

                if (!validateHourFormat(hourPost) || !validateHoraPosterior(hour, hourPost)) {
                    $("#cliente_horaFinSegundoTurnoEntregaFormat").val("");
                    changeInputString("horaFinSegundoTurnoEntrega", "");
                }
            } else {
                $("#cliente_horaInicioSegundoTurnoEntregaFormat").val("");
                changeInputString("horaInicioSegundoTurnoEntrega", "");

                $("#cliente_horaFinSegundoTurnoEntregaFormat").val("");
                changeInputString("horaFinSegundoTurnoEntrega", "");
            }
        } else {
            $("#cliente_horaInicioSegundoTurnoEntregaFormat").val("");
            changeInputString("horaInicioSegundoTurnoEntrega", "");
            $.alert({
                title: "Hora Invalida",
                type: 'orange',
                content: 'Debe seleccionar una hora de la lista.',
                buttons: {
                    OK: function () { }
                }
            });
        }
    });


    $("#cliente_horaFinSegundoTurnoEntregaFormat").change(function () {
        var hourPrev = $("#cliente_horaInicioSegundoTurnoEntregaFormat").val().trim();
        var hour = $("#cliente_horaFinSegundoTurnoEntregaFormat").val().trim();

        if (validateHourFormat(hour)) {
            var valid = true;
            if (!validateHourFormat(hourPrev)) {
                valid = false;
                $.alert({
                    title: "Hora Invalida",
                    type: 'orange',
                    content: 'Error en los horarios.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }

            if (!validateHoraPosterior(hourPrev, hour)) {
                valid = false;
                $.alert({
                    title: "Hora Invalida",
                    type: 'orange',
                    content: 'Debe seleccionar una hora mayor a la hora de inicial del segundo turno.',
                    buttons: {
                        OK: function () { }
                    }
                });
            }

            if (valid) {
                changeInputString("horaFinSegundoTurnoEntrega", $("#cliente_horaFinSegundoTurnoEntregaFormat").val());
            } else {
                $("#cliente_horaFinSegundoTurnoEntregaFormat").val("");
                changeInputString("horaFinSegundoTurnoEntrega", "");
            }
        } else {
            $("#cliente_horaFinSegundoTurnoEntregaFormat").val("");
            $.alert({
                title: "Hora Invalida",
                type: 'orange',
                content: 'Debe seleccionar una hora de la lista.',
                buttons: {
                    OK: function () { }
                }
            });
        }
    });

    function validateHoraPosterior(horaPrev, horaPost) {
        //var horaPrev = $("#" + idHoraPrev).val();
        //var horaPost = $("#" + idHoraPost).val();
        
        horaPrev = horaPrev.split(":");
        horaPost = horaPost.split(":");

        var hPrev = parseInt(horaPrev[0]);
        var mPrev = parseInt(horaPrev[1]);
        var hPost = parseInt(horaPost[0]);
        var mPost = parseInt(horaPost[1]);

        if (hPrev <= hPost) {
            if (hPrev == hPost) {
                if (mPrev >= mPost) {
                    return false;
                } else {
                    return true;
                }
            } else {
                return true;
            }
        } else {
            return false;
        }
    }

    function validateHourFormat(hourString) {
        var hourFormat = /(([0-1][0-9])|(2[0-3])):?[0-5][0-9]/;

        return hourFormat.test(hourString);
    }

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/Cliente/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }



   

    $("#cliente_contacto1").change(function () {
        changeInputString("contacto1", $("#cliente_contacto1").val());
    });

    $("#cliente_telefonoContacto1").change(function () {
        changeInputString("telefonoContacto1", $("#cliente_telefonoContacto1").val());
    });

    $("#cliente_emailContacto1").change(function () {
        changeInputString("emailContacto1", $("#cliente_emailContacto1").val());
    });

    $("#cliente_negociacionMultiregional").change(function () {
        if ($("#cliente_negociacionMultiregional").is(":checked")) {
            $("#cliente_sedePrincipal").removeAttr("disabled");
        } else {
            $("#cliente_sedePrincipal").prop("checked", false);
            changeInputBoolean('sedePrincipal', 0);
            $("#cliente_sedePrincipal").attr("disabled", "");
        }

    });

    $("#idOrigen").change(function () {
        var idOrigen = $("#idOrigen").val();
        $.ajax({
            url: "/Cliente/ChangeIdOrigen", type: 'POST',
            data: {
                idOrigen: idOrigen
            },
            error: function () { location.reload(); },
            success: function () { }
        });

    });

    $("#idSubDistribuidor").change(function () {
        var idSubDistribuidor = $("#idSubDistribuidor").val();
        $.ajax({
            url: "/Cliente/ChangeIdSubDistribuidor", type: 'POST',
            data: {
                idSubDistribuidor: idSubDistribuidor
            },
            error: function () { location.reload(); },
            success: function () { }
        });
    });
    

    $("#cliente_ruc").change(function () {
        changeInputString("ruc", $("#cliente_ruc").val())
    });

    $("#cliente_razonSocial").change(function () {
        changeInputString("razonSocial", $("#cliente_razonSocial").val())
    });

    $("#cliente_domicilioLegal").change(function () {
        changeInputString("domicilioLegal", $("#cliente_domicilioLegal").val())
    });

    $("#cliente_nombreComercial").change(function () {
        changeInputString("nombreComercial", $("#cliente_nombreComercial").val())
    });

    $("#cliente_correoEnvioFactura").change(function () {
        changeInputString("correoEnvioFactura", $("#cliente_correoEnvioFactura").val())
    });
    
    $("#cliente_razonSocialSunat").change(function () {
        changeInputString("razonSocialSunat", $("#cliente_razonSocialSunat").val())
    });

    $("#cliente_nombreComercialSunat").change(function () {
        changeInputString("nombreComercialSunat", $("#cliente_nombreComercialSunat").val())
    });

    $("#cliente_direccionDomicilioLegalSunat").change(function () {
        changeInputString("direccionDomicilioLegalSunat", $("#cliente_direccionDomicilioLegalSunat").val())
    });

    $("#cliente_estadoContribuyente").change(function () {
        changeInputString("estadoContribuyente", $("#cliente_direccliente_estadoContribuyentecionDomicilioLegalSunat").val())
    });

    $("#cliente_condicionContribuyente").change(function () {
        changeInputString("condicionContribuyente", $("#cliente_direcclientecliente_condicionContribuyente_estadoContribuyentecionDomicilioLegalSunat").val())
    });

    $("#cliente_observacionesCredito").change(function () {
        changeInputString("observacionesCredito", $("#cliente_observacionesCredito").val())
    });

    $("#cliente_observaciones").change(function () {
        changeInputString("observaciones", $("#cliente_observaciones").val())
    });

    $("#cliente_textoBusqueda").change(function () {
        changeInputString("textoBusqueda", $("#cliente_textoBusqueda").val())
    });

    $("#cliente_codigo").change(function () {
        changeInputString("codigo", $("#cliente_codigo").val())
    });

    $("#formaPagoCliente").change(function () {
        var formaPagoFactura = $("#formaPagoCliente").val();
        $.ajax({
            url: "/Cliente/ChangeFormaPagoFactura",
            type: 'POST',
            data: {
                formaPagoFactura: formaPagoFactura
            },
            success: function () { }
        });
    });

    $("#tipoPagoCliente").change(function () {
        var tipoPagoFactura = $("#tipoPagoCliente").val();
        $.ajax({
            url: "/Cliente/ChangeTipoPagoFactura",
            type: 'POST',
            data: {
                tipoPagoFactura: tipoPagoFactura
            },
            success: function () { }
        });
    });


    $("#plazoCreditoSolicitado").change(function () {
        var plazoCreditoSolicitado = $("#plazoCreditoSolicitado").val();
        $.ajax({
            url: "/Cliente/ChangePlazoCreditoSolicitado",
            type: 'POST',
            data: {
                plazoCreditoSolicitado: plazoCreditoSolicitado
            },
            success: function () { }
        });
    });
    





    


    

    
    

    $("#btnCopiar").click(function () {
        /* Get the text field */
        var copyText = document.getElementById("myInput");

        /* Select the text field */
        copyText.select();

        /* Copy the text inside the text field */
        document.execCommand("Copy");

        /* Alert the copied text */
    //    alert("Copied the text: " + copyText.value);
    }); 





    ////////VER COTIZACIÓN  --- CAMBIO DE ESTADO

    function invertirFormatoFecha(fecha)
    {
        var fechaInvertida = fecha.split("-");
        fecha = fechaInvertida[2] + "/" + fechaInvertida[1] + "/" + fechaInvertida[0];
        return fecha 
    }

    function convertirFechaNumero(fecha) {
        var fechaInvertida = fecha.split("/");
        fecha = fechaInvertida[2] + fechaInvertida[1] + fechaInvertida[0];
        return Number(fecha)
    }

    function convertirHoraToNumero(hora) {
        var hora = hora.split(":");
        hora = hora[0] +"."+ hora[1];
        return Number(hora)
    }

    function sumarHoras(hora, cantidad) {
        var hora = hora.split(":");

        var horaNumero = Number(hora[0]) + cantidad;


        if (horaNumero < 0)
            hora = "00:00";
        else if (horaNumero > 23)
            hora = "23:59";

        if (horaNumero < 10) {
            hora = "0" + horaNumero + ":" + hora[1];
            // hora = horaNumero + ":" + hora[1];
        }
        else {
            hora = horaNumero + ":" + hora[1];
        }

        return hora
    }



    
   /*####################################################
    EVENTOS DE LA GRILLA
    #####################################################*/


    /**
     * Se definen los eventos de la grilla
     */



 //   $('#tablefoottable').footable()

   /* $("#tablefoottable").onSort(function () {
        alert("Asas");

        /* onSort

    sort.bs.table

        }
    );*/
   


    function mostrarFlechasOrdenamiento() {
        $(".ordenar, .detordenamiento").attr('style', 'display: table-cell');

       // $(".detordenamiento.media").html('<div class="updown"><div class="up"></div> <div class="down"></div></div>');

        //Se identifica cuantas filas existen
        var contador = 0;
        var $j_object = $("td.detordenamiento");
        $.each($j_object, function (key, value) {
            contador++;
        });

        if (contador == 1) {
            $(".detordenamiento").html('');
        }
        else if (contador > 1)
        {
            var contador2 = 0;
            var $j_object = $("td.detordenamiento");
            $.each($j_object, function (key, value) {
                contador2++;
                if (contador2 == 1) {
                    value.innerHTML = '<div class="updown"><div class="down"></div></div>';
                    value.setAttribute("posicion", "primera");
                }
                else if (contador2 == contador) {
                    value.innerHTML = '<div class="updown"><div class="up"></div></div>';
                    value.setAttribute("posicion", "ultima");
                }
                else
                {
                    value.innerHTML = '<div class="updown"><div class="up"></div> <div class="down"></div></div>';
                    value.setAttribute("posicion", "media");
                }
            });
        }






    }


    $(document).on('click', ".up,.down", function () {

        var codigo = event.target.parentElement.parentElement.getAttribute("class").split(" ")[0];
        var row = $(this).parents("tr:first");

        //Mover hacia arriba
        if ($(this).is(".up")) {

            var posicionPrevia = row.prev().find('td.detordenamiento').attr("posicion");
            var htmlPrevio = row.prev().find('td.detordenamiento').html();

            var posicionActual = row.find('td.detordenamiento').attr("posicion");
            var htmlActual = row.find('td.detordenamiento').html(); 

            //intercambio de posicion
            row.prev().find('td.detordenamiento').attr("posicion", posicionActual);
            row.find('td.detordenamiento').attr("posicion", posicionPrevia);
            //intercambio de controles
            row.prev().find('td.detordenamiento').html(htmlActual);
            row.find('td.detordenamiento').html(htmlPrevio);

            row.insertBefore(row.prev());

        } else {
            var posicionPrevia = row.next().find('td.detordenamiento').attr("posicion");
            var htmlPrevio = row.next().find('td.detordenamiento').html();

            var posicionActual = row.find('td.detordenamiento').attr("posicion");
            var htmlActual = row.find('td.detordenamiento').html();

            //intercambio de posicion
            row.next().find('td.detordenamiento').attr("posicion", posicionActual);
            row.find('td.detordenamiento').attr("posicion", posicionPrevia);
            //intercambio de controles
            row.next().find('td.detordenamiento').html(htmlActual);
            row.find('td.detordenamiento').html(htmlPrevio);

            row.insertAfter(row.next());
        }
    });







    

    /*Evento que se dispara cuando se hace clic en el boton EDITAR en la edición de la grilla*/
    $(document).on('click', "button.footable-show", function () {

        //Cambiar estilos a los botones
        $("button.footable-add").attr("class", "btn btn-default footable-add");
        $("button.footable-hide").attr("class", "btn btn-primary footable-hide");


        //Se deshabilitan controles que recargan la página o que interfieren con la edición del detalle
        $("#considerarCantidades").attr('disabled', 'disabled');
        $("input[name=igv]").attr('disabled', 'disabled');
        $("#flete").attr('disabled', 'disabled');
        $("#btnOpenAgregarProducto").attr('disabled', 'disabled');

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

        var considerarCantidades = $("#considerarCantidades").val();
        if (considerarCantidades == CANT_SOLO_OBSERVACIONES) {
            /*Se agrega control input en columna observacion*/
            var $j_object = $("td.detobservacion");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var observacion = value.innerText;
                value.innerHTML = "<textarea class='" + arrId[0] + " detobservacionarea form-control'/>" + observacion + "</textarea>";
            });
        }
        else if (considerarCantidades == CANT_CANTIDADES_Y_OBSERVACIONES) 
        {
            var $j_object = $("span.detproductoObservacion");
            $.each($j_object, function (key, value) {

                var arrId = value.getAttribute("class").split(" ");
                var observacion = value.innerText;
                value.innerHTML = "<textarea class='" + arrId[0] + " detobservacionarea form-control'/>" + observacion + "</textarea>";
            });

        }

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


    

    /*####################################################
    EVENTOS BUSQUEDA COTIZACIONES
    #####################################################*/

  
    $("#estadoCrediticio").change(function () {
        var estado = $("#estadoCrediticio").val();
        $.ajax({
            url: "/Pedido/changeEstadoCrediticio",
            type: 'POST',
            data: {
                estadoCrediticio: estado
            },
            success: function () {
            }
        });
    });

 /*   $(document).on('change', "#ActualDepartamento", function () {
        var ubigeoEntregaId = "000000";
        if ($("#ActualDepartamento").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualDepartamento").val() + "0000";
        }
        $.ajax({
            url: "/Pedido/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#ActualProvincia",function () {
        var ubigeoEntregaId = $("#ActualDepartamento").val()+"0000";
        if ($("#ActualProvincia").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualProvincia").val()+"00";
        }
        $.ajax({
            url: "/Pedido/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
    });

    $(document).on('change', "#ActualDistrito",function () {
        var ubigeoEntregaId = $("#ActualDepartamento").val() + $("#ActualProvincia").val() + "00";
        if ($("#ActualDistrito").val().trim().length > 0) {
            ubigeoEntregaId = $("#ActualDistrito").val();
        }
        $.ajax({
            url: "/Pedido/ChangeUbigeoEntrega",
            type: 'POST',
            data: {
                ubigeoEntregaId: ubigeoEntregaId
            },
            success: function () {
            }
        });
       
    });*/


    $("#idCiudad").change(function () {
        var idCiudad = $("#idCiudad").val();
        var textCiudad = $("#idCiudad option:selected").text();

        if ($("#pagina").val() == PAGINA_MantenimientoCliente) {            

            if (textCiudad.trim().toUpperCase() == 'LIMA') {
                $('#cliente_CanalLima').prop('checked', true);
                $('#cliente_CanalProvincias').prop('checked', false);

                changeInputBoolean('perteneceCanalLima', 1);
                changeInputBoolean('perteneceCanalProvincias', 0);
            } else {
                $('#cliente_CanalProvincias').prop('checked', true);
                $('#cliente_CanalLima').prop('checked', false);

                changeInputBoolean('perteneceCanalLima', 0);
                changeInputBoolean('perteneceCanalProvincias', 1);
            }
        }

        $("#spn_vercliente_mp_registracotizaciones").html($("#idCiudad option:selected").text());

        $.ajax({
            url: "/Cliente/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad
            },
            error: function (detalle) {
                location.reload();
            },
            success: function (ciudad) {
            }
        });
    });  


    $("#idGrupoCliente").change(function () {
        var idGrupoCliente = $("#idGrupoCliente").val();
        if (idGrupoCliente != undefined && idGrupoCliente != "") {
            $("#cliente_habilitadoNegociacionGrupal").closest("label").show();
        } else {
            $("#cliente_habilitadoNegociacionGrupal").closest("label").hide();
            $('#cliente_habilitadoNegociacionGrupal').prop('checked', false);
            changeInputBoolean('habilitadoNegociacionGrupal', 0)
        }

        $.ajax({
            url: "/Cliente/ChangeIdGrupoCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idGrupoCliente: idGrupoCliente
            },
            error: function (detalle) {
                location.reload();
            },
            success: function (GrupoCliente) {
            }
        });
    });  



    /*VENDEDORES*/


    
    

    var idClienteView = "";





    $('#editorDireccionEntrega1').on('submit', function (e) {
      
        /*
        if (idDireccionEntrega == null || idDireccionEntrega == "") {
         
        }
        else {
          
        }*/
    });

    

    

  

    var direccionAcopioList;


    function cargarDireccionesEntrega() {
      
        $.ajax({
            url: "/DireccionEntrega/GetDireccionesEntregaCliente",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                mostrarMensajeErrorProceso();
            },
            success: function (result) {
                /**
                 * DIRECCIONES
                 */

                direccionAcopioList = result.direccionAcopioList;
                $("#idDireccionAlmacen").find('option')
                    .remove()
                    .end()
                    ;

                $('#idDireccionAlmacen').append($('<option>', {
                    value: GUID_EMPTY,
                    text: SELECCIONE_DIRECCION_ACOPIO
                }));

                for (var i = 0; i < direccionAcopioList.length; i++) {

                    $('#idDireccionAlmacen').append($('<option>', {
                        value: direccionAcopioList[i].idDireccionEntrega,
                        text: direccionAcopioList[i].descripcion
                    }));
                }



                var arrayDireccionEntrega = new Array();

                //    $("#tableListaDireccionesEntrega > tbody").empty();
                var direccionEntregaList = result.direccionEntregaList;
                for (i = 0; i < direccionEntregaList.length; i++) {
                    var contacto = direccionEntregaList[i].contacto == null ? "" : direccionEntregaList[i].contacto;
                    var telefono = direccionEntregaList[i].telefono == null ? "" : direccionEntregaList[i].telefono;
                    var codigoCliente = direccionEntregaList[i].codigoCliente == null ? "" : direccionEntregaList[i].codigoCliente;
                    var nombre = direccionEntregaList[i].nombre == null ? "" : direccionEntregaList[i].nombre;
                    var codigoMP = direccionEntregaList[i].codigoMP == null ? "" : direccionEntregaList[i].codigoMP;
                    var ubigeo = direccionEntregaList[i].ubigeo.Id == null ? "000000" : direccionEntregaList[i].ubigeo.Id;
                    var departamento = direccionEntregaList[i].ubigeo.Departamento == null ? "" : direccionEntregaList[i].ubigeo.Departamento;
                    var provincia = direccionEntregaList[i].ubigeo.Provincia == null ? "" : direccionEntregaList[i].ubigeo.Provincia;
                    var distrito = direccionEntregaList[i].ubigeo.Distrito == null ? "" : direccionEntregaList[i].ubigeo.Distrito;
                    var direccionDomicilioLegal = direccionEntregaList[i].direccionDomicilioLegal == null ? "" : direccionEntregaList[i].direccionDomicilioLegal;
                    var emailRecepcionFacturas = direccionEntregaList[i].emailRecepcionFacturas == null ? "" : direccionEntregaList[i].emailRecepcionFacturas;

                    var direccionEntregaRow = '<tr data-expanded="true">' +
                        '<td>' + nombre + '</td>' +
                        '<td>' + codigoCliente + '</td>' +
                        '<td>' + codigoMP + '</td>' +


                        '<td>' + direccionEntregaList[i].idDireccionEntrega + '</td>' +
                        '<td>' + direccionEntregaList[i].cliente.idCliente + '</td>' +
                        '<td>' + direccionEntregaList[i].cliente.ciudad.idCiudad + '</td>' +
                        '<td>' + direccionEntregaList[i].domicilioLegal.idDomicilioLegal + '</td>' +
                        '<td>' + ubigeo + '</td>' +

                        '<td>' + direccionEntregaList[i].codigo + '</td>' +
                        '<td>' + direccionEntregaList[i].cliente.ciudad.sede + '</td>' +
                        '<td>' + departamento + ' - ' + provincia + ' - ' + distrito + '</td>' +
                        '<td>' + direccionEntregaList[i].descripcion + '</td>' +
                        '<td>' + contacto + '</td>' +
                        '<td>' + telefono + '</td>' +
                        
                        
                        '<td>' + emailRecepcionFacturas + '</td>' +
                        '<td>' + direccionDomicilioLegal + '</td>' +
                        '<td>' + direccionEntregaList[i].direccionEntregaAlmacen.descripcion + '</td>' +
                        '<td>' + direccionEntregaList[i].direccionEntregaAlmacen.idDireccionEntrega + '</td>' +
                        '</tr>';

                    //var rowtmp = $modal.data('row'),
                    var values = {
                        idDireccionEntrega: direccionEntregaList[i].idDireccionEntrega,
                        idCliente: direccionEntregaList[i].cliente.idCliente,
                        idCiudad: direccionEntregaList[i].cliente.ciudad.idCiudad,
                        idDomicilioLegal: direccionEntregaList[i].domicilioLegal.idDomicilioLegal,
                        codigoUbigeo: ubigeo,
                        codigo: direccionEntregaList[i].codigo,
                        sede: direccionEntregaList[i].cliente.ciudad.sede,
                        ubigeo: departamento + ' - ' + provincia + ' - ' + distrito,
                        direccionEntrega: direccionEntregaList[i].descripcion,
                        contacto: contacto,
                        telefono: telefono,
                        codigoCliente: codigoCliente,
                        nombre: nombre,
                        codigoMP: codigoMP,
                        emailRecepcionFacturas: emailRecepcionFacturas,
                        direccionDomicilioLegal: direccionDomicilioLegal,
                        direccionAlmacen: direccionEntregaList[i].direccionEntregaAlmacen.descripcion,
                        idDireccionAlmacen: direccionEntregaList[i].direccionEntregaAlmacen.idDireccionEntrega
                    };

                    arrayDireccionEntrega.push(values);
                }

                loadDireccionesEntrega(arrayDireccionEntrega);


                $("#direccionEntrega_idDomicilioLegal").find('option')
                    .remove()
                    .end()
                    ;

                $('#direccionEntrega_idDomicilioLegal').append($('<option>', {
                    value: "",
                    text: "Seleccione Domicilio Legal"
                }));

                var domicilioLegalList = result.domicilioLegalList;

                for (var i = 0; i < domicilioLegalList.length; i++) {

                    //var ubigeo = domicilioLegalList[i].direccionEntrega.ubigeo;
                    $('#direccionEntrega_idDomicilioLegal').append($('<option>', {
                        value: domicilioLegalList[i].idDomicilioLegal,
                        text: domicilioLegalList[i].direccion // + " " + ubigeo.Departamento + " - " + ubigeo.Provincia + " - " + ubigeo.Distrito
                    }));
                }


                if (cliente.vendedoresAsignados) {

                    $("#spanVendedoresAsignados").show();
                    $("#spanVendedoresNoAsignados").hide();
                }
                else {
                    $("#spanVendedoresAsignados").hide();
                    $("#spanVendedoresNoAsignados").show();
                }
                

            }
        });
    };






});





$(document).ready(function () {
    $('#btnUpdateByExcel').click(function (event) {
        var fileInput = $('#fileUploadExcel');
        var maxSize = fileInput.data('max-size');
        var maxSizeText = fileInput.data('max-size-text');
        var imagenValida = true;
        if (fileInput.get(0).files.length) {
            var fileSize = fileInput.get(0).files[0].size; // in bytes

            if (fileSize > maxSize) {
                $.alert({
                    title: "Archivo Inválido",
                    type: 'red',
                    content: 'El tamaño del archivo debe ser como maximo ' + maxSizeText + '.',
                    buttons: {
                        OK: function () { }
                    }
                });
                imagenValida = false;
            }


        } else {
            $.alert({
                title: "Archivo Inválido",
                type: 'red',
                content: 'Seleccione un archivo por favor.',
                buttons: {
                    OK: function () { }
                }
            });
            imagenValida = false;
        }

        if (imagenValida) {

            var that = document.getElementById('fileUploadExcel');
            var file = that.files[0];
            var form = new FormData();
            var url = $(that).data("urlSetFile");
            var reader = new FileReader();
            var mime = file.type;

            // read the image file as a data URL.
            reader.readAsDataURL(file);

            form.append('file', file);

            $('body').loadingModal({
                text: '...'
            });
            $.ajax({
                url: url,
                type: "POST",
                cache: false,
                contentType: false,
                processData: false,
                data: form,
                dataType: 'JSON',
                beforeSend: function () {
                    //$.blockUI();
                },
                success: function (response) {
                    if (response.success == "true") {
                        $.alert({
                            title: "Carga Exitosa!",
                            type: 'green',
                            content: response.message,
                            buttons: {
                                OK: function () { }
                            }
                        });

                        $('#btnBusqueda').click();
                    } else {
                        $.alert({
                            title: "Carga fallida",
                            type: 'red',
                            content: response.message,
                            buttons: {
                                OK: function () { }
                            }
                        });
                    }
                },
                error: function (error) {
                    console.log(error);
                    $.alert({
                        title: "Carga fallida",
                        type: 'red',
                        content: 'Ocurrió un error al subir el archivo.',
                        buttons: {
                            OK: function () { }
                        }
                    });
                }
            }).done(function () {
                $('body').loadingModal('hide')
            });
        }
    });
});
