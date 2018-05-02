
jQuery(function ($) {


    //CONSTANTES POR DEFECTO
    var cantidadDecimales = 2;
    var IGV = 0.18;
    var SIMBOLO_SOL = "S/";
    var MILISEGUNDOS_AUTOGUARDADO = 5000;

    //Estados para búsqueda de Pedidos
    /*var ESTADOS_TODOS = -1;
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
    var ESTADO_EN_EDICION_STR = "En Edición";*/

  
    var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
    

    
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la edición/creación; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";

    $(document).ready(function () {
        obtenerConstantes();
        cargarChosenCliente();
        $("#btnBusqueda").click();
    });

    window.onafterprint = function () {
        if ($("#pagina").val() == 14) {
            window.close();
        }
    };

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
            url: "/Factura/SearchClientes"
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
            url: "/Factura/GetCliente",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCliente: idCliente
            },
            success: function (cliente) {
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

    var fechaEmisionDesde = $("#fechaEmisionDesdetmp").val();
    $("#documentoVenta_fechaEmisionDesde").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEmisionDesde);

    var fechaEmisionHasta = $("#fechaEmisionHastatmp").val();
    $("#documentoVenta_fechaEmisionHasta").datepicker({ dateFormat: "dd/mm/yy" }).datepicker("setDate", fechaEmisionHasta);




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


    $("#documentoVenta_fechaEmisionDesde").change(function () {
        var fechaEmisionDesde = $("#documentoVenta_fechaEmisionDesde").val();
        $.ajax({
            url: "/Factura/ChangeFechaEmisionDesde",
            type: 'POST',
            data: {
                fechaEmisionDesde: fechaEmisionDesde
            },
            success: function () {
            }
        });
    });

    $("#documentoVenta_fechaEmisionHasta").change(function () {
        var fechaEmisionHasta = $("#documentoVenta_fechaEmisionHasta").val();
        $.ajax({
            url: "/Factura/ChangeFechaEmisionHasta",
            type: 'POST',
            data: {
                fechaEmisionHasta: fechaEmisionHasta
            },
            success: function () {
            }
        });
    });


 
  
   /* ################################## FIN CHANGE CONTROLES */


      




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


    /*VER PEDIDO*/
  
    

    $("#btnCancelarGuiaRemision").click(function () {
        if (confirm(MENSAJE_CANCELAR_EDICION)) {
            window.location = '/GuiaRemision/CancelarCreacionGuiaRemision';
        }
    })


    $(document).on('click', "button.btnActualizarEstado", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idDocumentoVenta = arrrayClass[0];
        var serieNumero = arrrayClass[1];   


        $.ajax({
            url: "/Factura/consultarEstadoDocumentoVenta",
            type: 'POST',
            data: {
                idDocumentoVenta: idDocumentoVenta
            },
            error: function (detalle) {
                alert(MENSAJE_ERROR);
                location.reload();
            },
            success: function (ciudad) {
                alert("El estado de la factura " + serieNumero + " fue actualizado correctamente.");
                location.reload();
            }
        });
    });

    $(document).on('click', "button.btnDescargarPDF", function () {

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idDocumentoVenta = arrrayClass[0];
        var serieNumero = arrrayClass[1];   


        $.ajax({
            url: "/Factura/descargarArchivoDocumentoVenta",
            data: {
                idDocumentoVenta: idDocumentoVenta
            },
            type: 'POST',
            error: function (detalle) { alert("Ocurrió un problema al descargar la factura " + serieNumero + " en formato PDF."); },
            success: function (fileName) {
                //Se descarga el PDF y luego se limpia el formulario
                window.open('/General/DownLoadFile?fileName=' + fileName);
               // window.location = '/Cotizacion/CancelarCreacionCotizacion';
            }
        });


        
    });

   
       



    function desactivarBotonesVer()
    {
      /*  $("#btnCancelarCotizacion").attr('disabled', 'disabled');
        $("#btnEditarCotizacion").attr('disabled', 'disabled');*/
    }

    function activarBotonesVer() {
  /*      $("#btnCancelarCotizacion").removeAttr('disabled');
        $("#btnEditarCotizacion").removeAttr('disabled');;*/
    }
    


    

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

    
    /*####################################################
    EVENTOS BUSQUEDA FACTURA
    #####################################################*/



    $("#btnBusqueda").click(function () {

        var idCiudad = $("#idCiudad").val();
        var numero = $("#documentoVenta_numero").val();
        if (numero.trim().length == 0 && (idCiudad == "" || idCiudad == GUID_EMPTY)) {
            alert("Para realizar una búsqueda por número de factura debe indicar la sede MP.");
            return false;
        }



        //sede MP
        
        var idCliente = $("#idCliente").val();         
        var fechaEmisionDesde = $("#documentoVenta_fechaEmisionDesde").val();
        var fechaEmisionHasta = $("#documentoVenta_fechaEmisionHasta").val();
        //var estado = $("#estado").val();

        $.ajax({
            url: "/Factura/Search",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad,
                idCliente: idCliente,
                numero: numero,
                fechaEmisionDesde: fechaEmisionDesde,
                fechaEmisionHasta: fechaEmisionHasta
            },
            success: function (facturaList) {

                $("#tableFacturas > tbody").empty();
                //FooTable.init('#tableCotizaciones');
                $("#tableFacturas").footable({
                    "paging": {
                        "enabled": true
                    }
                });

                for (var i = 0; i < facturaList.length; i++) {

                    var factura = "";

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
                    var factura = '<tr data-expanded="false">'+
                        '<td>  ' + facturaList[i].idDocumentoVenta + '</td>' +
                        '<td>  ' + facturaList[i].serieNumero + '</td>' +
                        '<td>  ' + facturaList[i].usuario.nombre + '</td>' +
                        '<td>  ' + invertirFormatoFecha(facturaList[i].fechaEmision.substr(0, 10)) + '</td>' +
                        '<td>  ' + facturaList[i].cliente.razonSocial + '</td>' +
                        '<td>  ' + facturaList[i].cliente.ruc + '</td>' +
                        '<td>  ' + facturaList[i].ciudad.nombre + '</td>' +
                        '<td>  ' + facturaList[i].total + '</td>' +
                        '<td> ' + facturaList[i].descripcionEstadoSunat+'</td>' +
                        '<td> <button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnDescargarPDF btn btn-primary">Descargar</button>'+
                        '<button type="button"  class="' + facturaList[i].idDocumentoVenta + ' ' + facturaList[i].serieNumero + ' btnActualizarEstado  btn btn-primary">Act. Estado</button >'+
                        '</td> ' +
                         '</tr>';                
                    
                    $("#tableFacturas").append(factura);
                }


                if (facturaList.length > 0)
                    $("#msgBusquedaSinResultados").hide();
                else
                    $("#msgBusquedaSinResultados").show();

            }
        });
    });


    $("#idCiudad").change(function () {
        var idCiudad = $("#idCiudad").val();

        $.ajax({
            url: "/Factura/ChangeIdCiudad",
            type: 'POST',
            dataType: 'JSON',
            data: {
                idCiudad: idCiudad
            },
            error: function (detalle) {
                alert(MENSAJE_ERROR);
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

    


});