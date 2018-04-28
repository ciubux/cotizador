
jQuery(function ($) {


    //CONSTANTES POR DEFECTO
    var cantidadDecimales = 2;
    var IGV = 0.18;
    var SIMBOLO_SOL = "S/";
    var MILISEGUNDOS_AUTOGUARDADO = 5000;

    //Estados para búsqueda de Pedidos
    var ESTADOS_TODOS = -1;
    var ESTADO_PENDIENTE_APROBACION = 0;
    var ESTADO_APROBADA = 1;
    var ESTADO_DENEGADA = 2;
    var ESTADO_ACEPTADA = 3;
    var ESTADO_RECHAZADA = 4;
    var ESTADO_EN_EDICION = 7;

    //Etiquetas de estadps para búsqueda de Pedidos
    var ESTADO_PENDIENTE_APROBACION_STR = "Pendiente de Aprobación";
    var ESTADO_APROBADA_STR = "Aprobada";
    var ESTADO_DENEGADA_STR = "Denegada";
    var ESTADO_ACEPTADA_STR = "Aceptada";
    var ESTADO_RECHAZADA_STR = "Rechazada";
    var ESTADO_EN_EDICION_STR = "En Edición";

    //Eliminar luego 
    var CANT_SOLO_OBSERVACIONES = 0;
    var CANT_SOLO_CANTIDADES = 1;
    var CANT_CANTIDADES_Y_OBSERVACIONES = 2;

    var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";

    /*
     * 2 BusquedaPedidos
       3 CrearPedido
     */

    
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición/creación; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";

    $(document).ready(function () {
        obtenerConstantes();
        //setTimeout(autoGuardarGuiaRemision, MILISEGUNDOS_AUTOGUARDADO);
        verificarSiExisteNuevoTransportista();
        esPaginaImpresion();
        cargarChosenCliente();
        $("#btnBusqueda").click();
    });

    window.onafterprint = function () {
        if ($("#pagina").val() == 14) {
            window.close();
        }
    };

    function esPaginaImpresion() {
        if ($("#pagina").val() == 14) {
            window.print();
        }
    }

    function verificarSiExisteNuevoTransportista() {
        $('#guiaRemision_transportista option').each(function () {
            if ($(this).val() == GUID_EMPTY) {
                $("#btnAgregarTransportista").attr("disabled", "disabled");
            }
        });
    }


    function obtenerConstantes() {
        $.ajax({
            url: "/General/GetConstantes",
            type: 'POST',
            dataType: 'JSON',
            success: function (constantes) {
                IGV = constantes.IGV;
                SIMBOLO_SOL = constantes.SIMBOLO_SOL;
                MILISEGUNDOS_AUTOGUARDADO = constantes.MILISEGUNDOS_AUTOGUARDADO;
            }
        });
    }

   /* function autoGuardarGuiaRemision() {
        $.ajax({
            url: "/Pedido/autoSavePedido",
            type: 'POST',
            error: function () {
                setTimeout(autoGuardarPedido, MILISEGUNDOS_AUTOGUARDADO);
            },
            success: function () {
                setTimeout(autoGuardarPedido, MILISEGUNDOS_AUTOGUARDADO);
            }
        });
    }*/


    function cargarChosenCliente() {

        $("#idCliente").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" }).on('chosen:showing_dropdown', function (evt, params) {
            if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
                alert("Debe seleccionar la sede MP previamente.");
                $("#idCliente").trigger('chosen:close');
                $("#idCiudad").focus();
                return false;
            }
        });

        $("#idCliente").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/GuiaRemision/SearchClientes"
        }, {
                loadingImg: "Content/chosen/images/loading.gif"
            }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });
    }


    /**
    *################################## INICIO CONTROLES CIUDAD
    */

    function onChangeCiudad(parentController) {
        $.ajax({
            url: "/" + parentController + "/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
            idCiudad: this.value
            }, 
            error: function (detalle) {
                alert('Debe eliminar los productos agregados antes de cambiar de Sede.'); 
                location.reload(); 
            },
            success: function (ciudad) {
                alert(ciudad)
            }
        }); 
    }  



    /**
     * ################################ INICIO CONTROLES DE CLIENTE
     */



    $("#idCliente").change(function () {
      //  $("#contacto").val("");
        var idCliente = $(this).val();

        $.ajax({
            url: "/GuiaRemision/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente) {
          /*      $("#pedido_numeroReferenciaCliente").val("");
                $("#pedido_direccionEntrega").val("");
                $("#pedido_contactoEntrega").val("");
                $("#pedido_telefonoContactoEntrega").val("");
                $("#pedido_contactoPedido").val("");
                $("#pedido_telefonoContactoPedido").val("");*/
            }
        });
      
    });

    $('#modalAgregarCliente').on('shown.bs.modal', function () {

        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar la sede MP previamente.");
            $("#idCiudad").focus();
            $('#btnCancelCliente').click();
            return false;
        }
    });




    

    /**
     * FIN CONTROLES DE CLIENTE
     */





















    /**
     * ######################## INICIO CONTROLES DE FECHAS
     */
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


    var fechaMovimiento = $("#fechaMovimientotmp").val();
    $("#guiaRemision_fechaMovimiento").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaMovimiento);

    var fechaMovimientoDesde = $("#fechaMovimientoDesdetmp").val();
    $("#guiaRemision_fechaMovimientoDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaMovimientoDesde);

    var fechaMovimientoHasta = $("#fechaMovimientoHastatmp").val();
    $("#guiaRemision_fechaMovimientoHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaMovimientoHasta);




    /**
     * FIN DE CONTROLES DE FECHAS
     */



    /* ################################## INICIO CHANGE CONTROLES */

    function toggleControlesTransportista() {
        var idTransportista = $("#guiaRemision_transportista").val();
        if (idTransportista == "") {
            $("#guiaRemision_transportista_descripcion").attr('disabled', 'disabled');
            $("#guiaRemision_transportista_ruc").attr('disabled', 'disabled');
            $("#guiaRemision_transportista_direccion").attr('disabled', 'disabled');
            $("#guiaRemision_transportista_brevete").attr('disabled', 'disabled');
            
        }
        else {
            /*  $("#pedido_direccionEntrega_telefono").val($('#pedido_direccionEntrega').find(":selected").attr("telefono"));*/
            $("#guiaRemision_transportista_descripcion").removeAttr("disabled");
            $("#guiaRemision_transportista_ruc").removeAttr("disabled");
            $("#guiaRemision_transportista_direccion").removeAttr("disabled");
            $("#guiaRemision_transportista_brevete").removeAttr("disabled");
        }
    }

    $("#guiaRemision_transportista").change(function () {
        toggleControlesTransportista();
        var idTransportista = $("#guiaRemision_transportista").val();

        $.ajax({
            url: "/GuiaRemision/ChangeTransportista",
            type: 'POST',
            dataType: "JSON",
            data: {
                idTransportista: idTransportista
            },
            success: function (transportista) {

                $("#guiaRemision_transportista_descripcion").val(transportista.descripcion);
                $("#guiaRemision_transportista_ruc").val(transportista.ruc);
                $("#guiaRemision_transportista_direccion").val(transportista.direccion);
                $("#guiaRemision_transportista_brevete").val(transportista.brevete);
            }
        });
    });


 


    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/GuiaRemision/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    $("#guiaRemision_placaVehiculo").change(function () {
        changeInputString("placaVehiculo", $("#guiaRemision_placaVehiculo").val())
    });

    $("#guiaRemision_certificadoInscripcion").change(function () {
        changeInputString("certificadoInscripcion", $("#guiaRemision_certificadoInscripcion").val())
    });

    $("#guiaRemision_observaciones").change(function () {
        changeInputString("observaciones", $("#guiaRemision_observaciones").val())
    });


   /* ################################## FIN CHANGE CONTROLES */

    

    ////////CREAR/EDITAR GUIA REMISION
    

    function crearGuiaRemision(continuarLuego) {
        if (!validarIngresoDatosObligatoriosGuiaRemision())
            return false;
        $.ajax({
            url: "/GuiaRemision/Create",
            type: 'POST',
            dataType: 'JSON',
            data: {
                continuarLuego: continuarLuego
            },
            error: function (detalle) {
                alert(MENSAJE_ERROR);
            },
            success: function (resultado) {
                $("#numero").val(resultado.codigo);

                if (resultado.error == "DuplicateNumberDocumentException")
                {
                    alert("El número de guía de remisión ya fue utilizado, por favor actualice el número de guía, haciendo clic en el botón actualizar.");
                }
                else if (resultado.estado == ESTADO_APROBADA) {
                    alert("La guía de remisión número " + resultado.codigo + " fue creada correctamente.");
                    window.location = '/GuiaRemision/Index';
                }
                else if (resultado.estado == ESTADO_PENDIENTE_APROBACION) {
                    alert("La guía de remisión número " + resultado.codigo + " fue creada correctamente, sin embargo requiere APROBACIÓN.");
                    window.location = '/GuiaRemision/Index';
                }
                else if (resultado.estado == ESTADO_EN_EDICION) {
                    alert("La guía de remisión " + resultado.codigo + " fue guardada correctamente para seguir editandolo posteriormente.");
                    window.location = '/GuiaRemision/Index';
                }
                else {
                    alert(MENSAJE_ERROR);
                    window.location = '/GuiaRemision/Index';
                }

            }
        });
    }

   

    $("#btnFinalizarCreacionGuiaRemision").click(function () {
        crearGuiaRemision(0);
    });

    $("#btnContinuarCreandoLuego").click(function () {
        crearGuiaRemision(1);
    });

    $("#btnFinalizarEdicionGuiaRemision").click(function () {
        editarGuiaRemision(0);
    });

    $("#btnContinuarEditandoLuego").click(function () {
        editarGuiaRemision(1);
    });



    function validarIngresoDatosObligatoriosGuiaRemision() {
     

        return true;
    }
      




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


    /*VER GUIAREMISION*/
    $(document).on('click', "button.btnVerGuiaRemision", function () {
        
        activarBotonesVer();
        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idGuiaRemision = arrrayClass[0];
        var numeroDocumento = arrrayClass[1];     

        $.ajax({
            url: "/GuiaRemision/Show",
            data: {
                idGuiaRemision: idGuiaRemision
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                alert(MENSAJE_ERROR);
            },
            success: function (resultado) {
                //var cotizacion = $.parseJSON(respuesta);
                var guiaRemision = resultado.guiaRemision;
                var usuario = resultado.usuario;
                
                $("#idGuiaRemision").val(guiaRemision.idGuiaRemision);
                $("#ver_guiaRemision_ciudadOrigen_nombre").html(guiaRemision.ciudadOrigen.nombre);
                $("#ver_guiaRemision_ciudadOrigen_direccionPuntoPartida").html(guiaRemision.ciudadOrigen.direccionPuntoPartida);
                
                $("#ver_guiaRemision_fechaMovimiento").html(invertirFormatoFecha(guiaRemision.fechaMovimiento.substr(0, 10))); 
                $("#ver_guiaRemision_serieDocumento").html(guiaRemision.serieDocumento);
                $("#ver_guiaRemision_numeroDocumento").html(guiaRemision.numeroDocumento);
                $("#ver_guiaRemision_pedido_numeroPedido").html(guiaRemision.pedido.numeroPedido);
                $("#ver_guiaRemision_pedido_cliente").html(guiaRemision.pedido.cliente.razonSocial);
                $("#ver_guiaRemision_pedido_numeroReferenciaCliente").html(guiaRemision.pedido.numeroReferenciaCliente);
                $("#ver_guiaRemision_motivoTraslado").html(guiaRemision.motivoTraslado);
                $("#ver_guiaRemision_atencionParcial").html(guiaRemision.atencionParcial);
                $("#ver_guiaRemision_pedido_ubigeoEntrega").html(guiaRemision.pedido.ubigeoEntrega.ToString);
                $("#ver_guiaRemision_pedido_direccionEntrega").html(guiaRemision.pedido.direccionEntrega.descripcion);
                $("#ver_guiaRemision_transportista_descripcion").html(guiaRemision.transportista.descripcion);
                $("#ver_guiaRemision_transportista_ruc").html(guiaRemision.transportista.ruc);
                $("#ver_guiaRemision_transportista_brevete").html(guiaRemision.transportista.brevete);
                $("#ver_guiaRemision_transportista_direccion").html(guiaRemision.transportista.direccion);
                $("#ver_guiaRemision_placaVehiculo").html(guiaRemision.placaVehiculo);
                $("#ver_guiaRemision_certificadoInscripcion").html(guiaRemision.certificadoInscripcion);
                $("#ver_guiaRemision_observaciones").html(guiaRemision.observaciones);

                //invertirFormatoFecha(pedido.fechaMaximaEntrega.substr(0, 10)));


                
              
                $("#tableDetalleGuia > tbody").empty();

                FooTable.init('#tableDetalleGuia');    


                var d = '';
                var lista = guiaRemision.documentoDetalle;
                for (var i = 0; i < lista.length; i++) {

                   // var observacion = lista[i].observacion == null || lista[i].observacion == 'undefined'? '' : lista[i].observacion;

                    d += '<tr>' +
                        '<td>' + lista[i].producto.idProducto + '</td>' +
                        '<td>' + lista[i].producto.sku + '</td>' +
                        '<td>' + lista[i].cantidad + '</td>' +
                        '<td>' + lista[i].unidad + '</td>' +
                        '<td>' + lista[i].producto.descripcion + '</td>' +
                        '</tr>';

                }
             
                $("#tableDetalleGuia").append(d);


              /*      if (pedido.seguimientoPedido.estado == ESTADO_EN_EDICION) {
                        $("#btnEditarPedido").html("Continuar Editanto");
                    }
                    else
                    {
                        $("#btnEditarPedido").html("Editar");
                    }
          
                */

                

                $("#modalVerGuiaRemision").modal('show');

                //  window.location = '/Pedido/Index';
            }
        });
    });

    


    

    $("#btnCancelarGuiaRemision").click(function () {
        if (confirm(MENSAJE_CANCELAR_EDICION)) {
            window.location = '/GuiaRemision/CancelarCreacionGuiaRemision';
        }
    })



    function desactivarBotonesVer()
    {
      /*  $("#btnCancelarCotizacion").attr('disabled', 'disabled');
        $("#btnEditarCotizacion").attr('disabled', 'disabled');
        $("#btnReCotizacion").attr('disabled', 'disabled');
        $("#btnAprobarCotizacion").attr('disabled', 'disabled');
        $("#btnDenegarCotizacion").attr('disabled', 'disabled');
        $("#btnAceptarCotizacion").attr('disabled', 'disabled');
        $("#btnRechazarCotizacion").attr('disabled', 'disabled');
        $("#btnPDFCotizacion").attr('disabled', 'disabled');*/
    }

    function activarBotonesVer() {
  /*      $("#btnCancelarCotizacion").removeAttr('disabled');
        $("#btnEditarCotizacion").removeAttr('disabled');
        $("#btnReCotizacion").removeAttr('disabled');
        $("#btnAprobarCotizacion").removeAttr('disabled');
        $("#btnDenegarCotizacion").removeAttr('disabled');
        $("#btnAceptarCotizacion").removeAttr('disabled');
        $("#btnRechazarCotizacion").removeAttr('disabled');
        $("#btnPDFCotizacion").removeAttr('disabled');*/
    }

   


    $("#btnImprimirGuiaRemision").click(function () {
        window.open("GuiaRemision/Print");
    });

    


    $("#btnAceptarAtencion").click(function () {
        desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona

        $.ajax({
            url: "/GuiaRemision/Create",
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al iniciar la atención del pedido."); },
            success: function (fileName) {
                window.location = '/Pedido/Pedir';
            }
        });

    });


    

    function limpiarComentario()
    {
        $("#comentarioEstado").val("");
        $("#comentarioEstado").focus();
    }

    



    $('#modalAprobacion').on('shown.bs.modal', function (e) {
        limpiarComentario();
    });



    $("#btnAceptarCambioEstado").click(function () {

        var estado = $("#estadoId").val();
        var comentarioEstado = $("#comentarioEstado").val();

        if ($("#labelNuevoEstado").html() == ESTADO_DENEGADA_STR || $("#labelNuevoEstado").html() == ESTADO_RECHAZADA_STR) {
            if (comentarioEstado.trim() == "") {
                alert("Cuando Deniega o Rechaza una cotización debe ingresar un Comentario.");
                return false;
            }
        }
        var codigo = $("#verNumero").html();

        $.ajax({
            url: "/Pedido/updateEstadoCotizacion",
            data: {
                codigo: codigo,
                estado: estado,
                observacion: comentarioEstado
            },
            type: 'POST',
            error: function () {
                alert("Ocurrió un problema al intentar cambiar el estado de la cotización.")
                $("#btnCancelarCambioEstado").click();
            },
            success: function () {
                alert("El estado de la cotización número: " + codigo + " se cambió correctamente.");
                //$("#btnCancelarCambioEstado").click();
                $("#btnBusqueda").click();
            }
        });
    });




    
    var ft = null;



    //Mantener en Session cambio de Seleccion de IGV
    $("input[name=igv]").on("click", function () {
        var igv = $("input[name=igv]:checked").val();
        $.ajax({
            url: "/Pedido/updateSeleccionIGV",
            type: 'POST',
            data: {
                igv: igv
            },
            success: function (cantidad) {
                if (cantidad > 0) {
                    location.reload();
                }
            }
        });
    });

    //Mantener en Session cambio de Seleccion de IGV
    $("#considerarCantidades").change( function () {
        var considerarCantidades = $("#considerarCantidades").val();
        $.ajax({
            url: "/Pedido/updateSeleccionConsiderarCantidades",
            type: 'POST',
            data: {
                considerarCantidades: considerarCantidades
            },
            success: function (cantidad)
            {
                if (cantidad > 0)
                {
                    location.reload();
                }
            }
        });


    });



    

    //Mantener en Session cambio de Seleccion de Mostrar Proveedor
    $("input[name=mostrarcodproveedor]").on("click", function () {
        var mostrarcodproveedor = $("input[name=mostrarcodproveedor]:checked").val();
        $.ajax({
            url: "/Pedido/updateMostrarCodigoProveedor",
            type: 'POST',
            data: {
                mostrarcodproveedor: mostrarcodproveedor
            },
            success: function () {
                location.reload();
            }
        });
    });

    //Mantener en Session cambio de Cliente
    $("#cliente").change(function () {

        $.ajax({
            url: "/Pedido/updateCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                cliente: $("#cliente").val()
            },
            success: function () { }
        });
    });




    $("#contacto").change(function () {

        $.ajax({
            url: "/Pedido/updateContacto",
            type: 'POST',
            dataType: 'JSON',
            data: {
                contacto: $("#contacto").val()
            },
            success: function () { }
        });
    });

    $("#flete").change(function () {

        $("#flete").val(Number($("#flete").val()).toFixed(cantidadDecimales))
        var flete = $("#flete").val(); 
        if (flete > 100)
        {
            $("#flete").val("100.00");
            flete = 100;
        }

        var total = Number($("#total").val());
        $('#montoFlete').html("Flete: " + SIMBOLO_SOL + " " + (total * flete / 100).toFixed(cantidadDecimales));
        $('#montoTotalMasFlete').html("Total más Flete: " + SIMBOLO_SOL + " " +  (total + (total * flete / 100)).toFixed(cantidadDecimales));

        


        $.ajax({
            url: "/Pedido/updateFlete",
            type: 'POST',
            data: {
                flete: flete
            },
            success: function () {
                location.reload();
            }
        });
    });

    

    $("#mostrarCosto").change(function () {
        var mostrarCosto = $('#mostrarCosto').prop('checked') ;
        $.ajax({
            url: "/Pedido/updateMostrarCosto",
            type: 'POST',
            data: {
                mostrarCosto: mostrarCosto
            },
            success: function () {
                location.reload();
            }
        });
    });

    





    /*####################################################
    EVENTOS DE LA GRILLA
    #####################################################*/


    /**
     * Se definen los eventos de la grilla
     */
    function cargarTablaDetalle() {
        var $modal = $('#tableDetalleGuia'),
            $editor = $('#tableDetalleGuia'),
            $editorTitle = $('#tableDetalleGuia');

     
        ft = FooTable.init('#tableDetalleGuia', {
            editing: {
                enabled: true,
                addRow: function () {
                    if (confirm(MENSAJE_CANCELAR_EDICION)) {
                        location.reload();
                    }
                },
                editRow: function (row) {
                    var values = row.val();
                    var idProducto = values.idProducto;
                    alert(idProducto);
                },
                deleteRow: function (row) {
                    var values = row.val();
                    var idProducto = values.idProducto;
                    row.delete();
                }
            }
        });
        
    }
    cargarTablaDetalle();


 

    


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

        
        /*
        var codigo = $("#numero").val();
        if (codigo == "") {
            $("#btnContinuarCreandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarCreacionGuiaRemision").attr('disabled', 'disabled');
            $("#btnCancelPedido").attr('disabled', 'disabled');
        }
        else {
            $("#btnContinuarEditandoLuego").attr('disabled', 'disabled');
            $("#btnFinalizarEdicionGuiaRemision").attr('disabled', 'disabled');
            $("#btnCancelPedido").attr('disabled', 'disabled');
        }
        */
        
      //  $("input[name=mostrarcodproveedor]").attr('disabled', 'disabled');


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
    
    });






    /*Evento que se dispara cuando se hace clic en FINALIZAR en la edición de la grilla*/
    $(document).on('click', "button.footable-hide", function () {

        var json = '[ ';
        var $j_object = $("td.detcantidad");

        $.each($j_object, function (key, value) {
            var arrId = value.getAttribute("class").split(" ");

            /*Se elimina control input en columna cantidad*/
            var cantidad = $("." + arrId[0] + ".detincantidad").val();
            value.innerText = cantidad;

            /*Se elimina control input en columna porcentaje descuento*/
            var porcentajeDescuento = 0;
            var margen = 0;
            var precio = 0;
            var flete = 0;
            var costo = 0;
            var observacion = "";

            json = json + '{"idProducto":"' + arrId[0] + '", "cantidad":"' + cantidad + '", "porcentajeDescuento":"' + porcentajeDescuento + '", "precio":"' + precio + '", "flete":"' + flete + '",  "costo":"' + costo + '", "observacion":"' + observacion + '"},';
        });

        json = json.substr(0, json.length - 1) + ']';

    
        
        $.ajax({
            url: "/GuiaRemision/ChangeDetalle",
            type: 'POST',
            data: json,
            dataType: 'json',
            contentType: 'application/json',
            success: function (respuesta) {
                location.reload();
            }
        });

    });

    /*Evento que se dispara cuando se hace clic en el boton calcular descuento de la grilla*/
    $(document).on('click', "button.btnCalcularDescuento", function () {
        var idProducto = event.target.getAttribute("class").split(" ")[0];
        $("#idProducto").val(idProducto);
    });



    /*Evento que se dispara cuando se modifica un control de descuento de la grilla*/
    $(document).on('change', "input.detinporcentajedescuento", function () {
        var idproducto = event.target.getAttribute("class").split(" ")[0];
        calcularSubtotalGrilla(idproducto);
    });

    /*Evento que se dispara cuando se modifica un control de cantidad de la grilla*/
    $(document).on('change', "input.detincantidad", function () {
        var idproducto = event.target.getAttribute("class").split(" ")[0];
        calcularSubtotalGrilla(idproducto);
    });


    /*Evento que se dispara cuando se modifica un control de cantidad de la grilla*/
    $(document).on('change', "input.detinflete", function () {
        var idproducto = event.target.getAttribute("class").split(" ")[0];
        calcularSubtotalGrilla(idproducto);
    });

    /*Evento que se dispara cuando se modifica el color en la grilla*/
/*    $(document).on('change', "input.detobservacioncolor", function () {
        var idproducto = event.target.getAttribute("class").split(" ")[0];

        alert(event.target.value);

        /*


        class="@cotizacionDetalle.producto.idProducto fila"

        #00ff40*/
    //});

    

    function calcularSubtotalGrilla(idproducto)
    {
        //Se obtiene el porcentaje descuento 
        var porcentajeDescuento = Number($("." + idproducto + ".detinporcentajedescuento").val());
        //Se obtiene el flete
        var flete = Number($("." + idproducto + ".detinflete").val());
        //Se obtiene el precio lista
        var precioLista = Number($("." + idproducto + ".detprecioLista").html());
        //Se calculo el precio con descuento 
        var precio = Number((precioLista * (100 - porcentajeDescuento) / 100).toFixed(cantidadDecimales));
        //Se asigna el precio calculculado en la columna precio
        $("." + idproducto + ".detprecio").html(precio);
        //se obtiene la cantidad
        var cantidad = Number($("." + idproducto + ".detincantidad").val());
        //Se define el precio Unitario 
        var precioUnitario = flete + precio
        $("." + idproducto + ".detprecioUnitario").html(precioUnitario.toFixed(cantidadDecimales));
        //Se calcula el subtotal
        var subTotal = precioUnitario * cantidad;
        //Se asigna el subtotal 
        $("." + idproducto + ".detsubtotal").html(subTotal.toFixed(cantidadDecimales));
        //Se calcula el margen
        var costo = Number($("." + idproducto + ".detcostoLista").html());
        var margen = (1 - (Number(costo) / Number(precio)))*100;
        //Se asigna el margen 
        $("." + idproducto + ".detmargen").text(margen.toFixed(1)+ " %");

        var precioNetoAnterior = Number($("." + idproducto + ".detprecioNetoAnterior").html());        
        var varprecioNetoAnterior = (precio / precioNetoAnterior - 1)*100;
        $("." + idproducto + ".detvarprecioNetoAnterior").text(varprecioNetoAnterior.toFixed(1));

        var costoAnterior = Number($("." + idproducto + ".detcostoAnterior").text());       
        var varcosto = (costo / costoAnterior - 1)*100;
        $("." + idproducto + ".detvarCosto").text(varcosto.toFixed(1) + " %");


        //Se actualiza el subtotal de la cotizacion

        var $j_object = $("td.detcantidad");

        var subTotal = 0;
        var igv = 0;
        var total = 0;
       
        $.each($j_object, function (key, value) {
            var arrId = value.getAttribute("class").split(" ");
            var precioUnitario = Number($("." + arrId[0] + ".detprecioUnitario").text());
            var cantidad = Number($("." + arrId[0] + ".detincantidad").val());
            subTotal = subTotal + Number(Number((precioUnitario * cantidad)).toFixed(cantidadDecimales));
        });

        

        var incluidoIGV = $("input[name=igv]:checked").val();
        //Si no se etsá incluyendo IGV se le agrega
        if (incluidoIGV == "0") {
            igv = Number((subTotal * IGV).toFixed(cantidadDecimales));
            total = subTotal + (igv);
        }
        //Si se está incluyendo IGV entonces se 
        else
        {
            total = subTotal;
            subTotal = Number((subTotal / (1 + IGV)).toFixed(cantidadDecimales));
            igv = total - subTotal;
        }

        $('#montoSubTotal').html(subTotal.toFixed(cantidadDecimales));
        $('#montoIGV').html(igv.toFixed(cantidadDecimales));
        $('#montoTotal').html(total.toFixed(cantidadDecimales));

    };









    /*####################################################
    EVENTOS BUSQUEDA COTIZACIONES
    #####################################################*/



    $("#btnBusqueda").click(function () {
        //sede MP
        var idCiudad = $("#idCiudad").val();
        var idCliente = $("#idCliente").val(); 

        var numeroDocumento = $("#guiaRemision_numeroDocumento").val();
        var fechaMovimientoDesde = $("#guiaRemision_fechaMovimientoDesde").val();
        var fechaMovimientoHasta = $("#guiaRemision_fechaMovimientoHasta").val();
        //var estado = $("#estado").val();

        $.ajax({
            url: "/GuiaRemision/Search",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                numeroDocumento: numeroDocumento,
                fechaMovimientoDesde: fechaMovimientoDesde,
                fechaMovimientoHasta: fechaMovimientoHasta//,
          //      estado: estado
            },
            success: function (guiaRemisionList) {

                $("#tableGuiasRemision > tbody").empty();
                //FooTable.init('#tableCotizaciones');
                $("#tableGuiasRemision").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < guiaRemisionList.length; i++) {

                    var guiaRemision = "";

               /*   
                    var observacion = pedidoList[i].seguimientoPedido.observacion == null ? "" : pedidoList[i].seguimientoPedido.observacion;

                    if (pedidoList[i].seguimientoPedido.observacion != null && pedidoList[i].seguimientoPedido.observacion.length > 20) {
                        var idComentarioCorto = pedidoList[i].idPedido + "corto";
                        var idComentarioLargo = pedidoList[i].idPedido + "largo";
                        var idVerMas = pedidoList[i].idPedido + "verMas";
                        var idVermenos = pedidoList[i].idPedido + "verMenos";

                        var comentario = pedidoList[i].seguimientoPedido.observacion.substr(0, 20) + "...";
                        observacion = '<div id="' + idComentarioCorto + '" style="display:block;">' + comentario + '</div>' +
                            '<div id="' + idComentarioLargo + '" style="display:none;">' + pedidoList[i].seguimientoPedido.observacion + '</div>' +
                            '<p><a id="' + idVerMas + '" class="' + pedidoList[i].idCotizacion + ' verMas" href="javascript:mostrar();" style="display:block">Ver Más</a></p>' +
                            '<p><a id="' + idVermenos + '" class="' + pedidoList[i].idCotizacion + ' verMenos" href="javascript:mostrar();" style="display:none">Ver Menos</a></p>';
                    }
  */
                     var guiaRemision = '<tr data-expanded="false">'+
                     '<td>  ' + guiaRemisionList[i].idGuiaRemision + '</td>' +
                         '<td>  ' + guiaRemisionList[i].serieNumeroGuia + '</td>' +
                         '<td>  ' + guiaRemisionList[i].usuario.nombre + '</td>' +
                         '<td>  ' + invertirFormatoFecha(guiaRemisionList[i].fechaMovimiento.substr(0, 10)) + '</td>' +
                         '<td>  ' + guiaRemisionList[i].pedido.cliente.razonSocial + '</td>' +
                         '<td>  ' + guiaRemisionList[i].pedido.cliente.ruc + '</td>' +
                         '<td>  ' + guiaRemisionList[i].ciudadOrigen.nombre + '</td>' +
                         '<td>  ' + guiaRemisionList[i].estaAnuladoDescripcion + '</td>' +
                         '<td> <button type="button" class="' + guiaRemisionList[i].idGuiaRemision + ' ' + guiaRemisionList[i].numeroDocumento + ' btnVerGuiaRemision btn btn-primary ">Ver</button></td > ' +
                         '</tr>';
                
                    
                    $("#tableGuiasRemision").append(guiaRemision);
                }


                if (guiaRemisionList.length > 0)
                    $("#msgBusquedaSinResultados").hide();
                else
                    $("#msgBusquedaSinResultados").show();

            }
        });
    });


    $("input[name=guiaRemision_estaAnulado]").on("click", function () {
        var estaAnulado = $("input[name=guiaRemision_estaAnulado]:checked").val();
        $.ajax({
            url: "/GuiaRemision/ChangeEstaAnulado",
            type: 'POST',
            data: {
                estaAnulado: estaAnulado
            },
            success: function () {
            }
        });
    });


    $("#guiaRemision_fechaMovimientoDesde").change(function () {
        var fechaMovimientoDesde = $("#guiaRemision_fechaMovimientoDesde").val();
        $.ajax({
            url: "/GuiaRemision/ChangeFechaMovimientoDesde",
            type: 'POST',
            data: {
                fechaMovimientoDesde: fechaMovimientoDesde
            },
            success: function () {
            }
        });
    });

    $("#guiaRemision_fechaMovimientoHasta").change(function () {
        var fechaMovimientoHasta = $("#guiaRemision_fechaMovimientoHasta").val();
        $.ajax({
            url: "/GuiaRemision/ChangeFechaMovimientoHasta",
            type: 'POST',
            data: {
                fechaMovimientoHasta: fechaMovimientoHasta
            },
            success: function () {
            }
        });
    });
    
    
    $("#idCiudad").change(function () {
        var idCiudad = $("#idCiudad").val();

        $.ajax({
            url: "/GuiaRemision/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad
            },
            error: function (detalle) {
                alert('Debe eliminar los productos agregados antes de cambiar de Sede.');
                location.reload();
            },
            success: function (ciudad) {

                $("#guiaRemision_ciudadOrigen_direccionPuntoPartida").val(ciudad.direccionPuntoPartida);

            }
        });
    });  


    function ConfirmDialogAtencionParcial(message) {
        $('<div></div>').appendTo('body')
            .html('<div><h6>' + message + '</h6></div>')
            .dialog({
                modal: true, title: 'Confirmación', zIndex: 10000, autoOpen: true,
                width: 'auto', resizable: false,
                buttons: {
                    Si: function () {
                        changeUltimaAtencionParcial(1);
                        $(this).dialog("close");

                    },
                    No: function () {
                        changeUltimaAtencionParcial(0);
                        $(this).dialog("close");
                    }
                },
                close: function (event, ui) {
                    $(this).remove();
                }
            });
        document.body.scrollTop = default_scrollTop;
    }

    function changeUltimaAtencionParcial(ultimaAtencionParcial) {
        if (ultimaAtencionParcial == 1) {
            $("#descripcionUltimaAtencionParcial").html("(Última Atención Parcial)");
        }
        else {
            $("#descripcionUltimaAtencionParcial").html("");
        }

        $.ajax({
            url: "/GuiaRemision/ChangeUltimaAtencionParcial",
            type: 'POST',
            data: {
                ultimaAtencionParcial: ultimaAtencionParcial
            },
            success: function () { }
        });
    }



    $("#guiaRemision_atencionParcial").change(function () {

        var atencionParcial = 1;

        if (!$('#guiaRemision_atencionParcial').prop('checked')) {
            $("#descripcionUltimaAtencionParcial").html("");
            atencionParcial = 0;
        }


        var estado = $("#guiaRemision_atencionParcial").val();
        $.ajax({
            url: "/GuiaRemision/ChangeAtencionParcial",
            type: 'POST',
            data: {
                atencionParcial: atencionParcial
            },
            success: function () { }
        });


        
        if ($('#guiaRemision_atencionParcial').prop('checked')) {
            ConfirmDialogAtencionParcial("¿Está atención parcial finaliza la atención del pedido?")
        }
        

    });

    $("#btnSaveTransportista").click(function () {

        if ($("#transportista_descripcion").val().trim() == "") {
            alert("Debe ingresar la descripción del transportista.");
            $('#transportista_descripcion').focus();
            return false;
        }

        if ($("#transportista_direccion").val().trim() == "") {
            alert("Debe ingresar la dirección del transportista.");
            $('#transportista_direccion').focus();
            return false;
        }

        if ($("#transportista_ruc").val().trim() == "") {
            alert("Debe ingresar el RUC del transportista.");
            $('#transportista_ruc').focus();
            return false;
        }

        if ($("#transportista_telefono").val().trim() == "") {
            alert("Debe ingresar el telefono del transportista.");
            $('#transportista_telefono').focus();
            return false;
        }


        

        var descripcion = $("#transportista_descripcion").val();
        var direccion = $("#transportista_direccion").val();
        var ruc = $("#transportista_ruc").val();
        var telefono = $("#transportista_telefono").val();

        $.ajax({
            url: "/GuiaRemision/CreateTransportistaTemporal",
            type: 'POST',
            dataType: 'JSON',
            data: {
                descripcion: descripcion,
                direccion: direccion,
                ruc: ruc,
                telefono: telefono
            },
            error: function (detalle) { alert("Se generó un error al intentar crear el transportista."); },
            success: function (transportista) {

                $('#guiaRemision_transportista').append($('<option>', {
                    value: transportista.idTransportista,
                    text: transportista.descripcion
                }));
                $('#guiaRemision_transportista').val(transportista.idTransportista);

                $('#guiaRemision_transportista_descripcion').val(transportista.descripcion);
                $('#guiaRemision_transportista_direccion').val(transportista.direccion);
                $('#guiaRemision_transportista_ruc').val(transportista.ruc);
                $('#guiaRemision_transportista_telefono').val(transportista.telefono);
                verificarSiExisteNuevoTransportista();
                toggleControlesTransportista();
            }
        });


        $('#btnCancelTransportista').click();

    });





});