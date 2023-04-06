
jQuery(function ($) {
    var pagina = 28;
    var MENSAJE_CANCELAR_EDICION = '¿Está seguro de cancelar la creación/edición; no se guardarán los cambios?';
    var MENSAJE_ERROR = "La operación no se procesó correctamente; Contacte con el Administrador.";
    var TITLE_EXITO = 'Operación Realizada';

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

    $("#fechaVigenciaDesde").datepicker();
    $("#fechaVigenciaHasta").datepicker();

    $("#fechaFin").datepicker();
    $("#fechaInicio").datepicker();

    $("#tableListasPrecios").footable({
        "paging": {
            "enabled": true
        }
    });


    $(document).ready(function () {
        $("#btnBusqueda").click();
        cargarChosenCliente();
    });


    function limpiarFormulario() {
        $("#rol_codigo").val("");
        $("#rol_nombre").val("");
    }


    function cargarChosenCliente() {

        $("#idClienteSunat").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" }).on('chosen:showing_dropdown', function (evt, params) {

        });

        $("#idClienteSunat").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/Cliente/SearchClienteSunat"
        }, {
            loadingImg: "Content/chosen/images/loading.gif"
        }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });
    }

    function cargarChosenGrupo() {
        $("#idGrupoCliente").chosen({ placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" }).on('chosen:showing_dropdown', function (evt, params) {

        });

        $("#idGrupoCliente").ajaxChosen({
            dataType: "json",
            type: "GET",
            minTermLength: 5,
            afterTypeDelay: 300,
            cache: false,
            url: "/GrupoCliente/SearchClienteSunat"
        }, {
            loadingImg: "Content/chosen/images/loading.gif"
        }, { placeholder_text_single: "Buscar Cliente", no_results_text: "No se encontró Cliente" });
    }

    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/PrecioEspecial/ChangeInputInt",
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
            url: "/PrecioEspecial/ChangeInputDecimal",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function changeInputString(propiedad, valor) {
        $.ajax({
            url: "/PrecioEspecial/ChangeInputString",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function changeInputDate(propiedad, valor) {
        $.ajax({
            url: "/PrecioEspecial/ChangeInputDate",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

    function changeInputBoolean(propiedad, valor) {
        $.ajax({
            url: "/PrecioEspecial/ChangeInputBoolean",
            type: 'POST',
            data: {
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }

   
    $("#fechaVigenciaDesde").change(function () {
        changeInputDate("fechaVigenciaDesde", $(this).val());
    });

    $("#fechaVigenciaHasta").change(function () {
        changeInputDate("fechaVigenciaHasta", $(this).val());
    });

    $("#fechaInicio").change(function () {
        changeInputDate("fechaInicio", $(this).val());
    });

    $("#fechaFin").change(function () {
        changeInputDate("fechaFin", $(this).val());
    });


    $("#btnAprobarAjusteAlmacen").click(function () {
        $('body').loadingModal({
            text: 'Registrando Aprobación...'
        });
        $('body').loadingModal('show');

        var idAjusteAlmacen = $(this).attr("idAjusteAlmacen");
        $.ajax({
            url: "/AjusteAlmacen/AprobarAjusteAlmacen",
            type: 'POST',
            data: {
                idAjusteAlmacen: idAjusteAlmacen
            },
            error: function () {
                $('body').loadingModal('hide');
                $.alert({
                    title: "ERROR",
                    content: 'Ocurrió un error.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                        }
                    }
                });
            },
            success: function () {
                $('body').loadingModal('hide');
                $.alert({
                    title: "REGISTRO EXITOSO",
                    content: 'Se registro la aprobación del Ajuste de almacén.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            location.reload();
                        }
                    }
                });
                
            }
        });
    });
    


    $(document).on('click', "button.btnVerPrecioEspecial", function () {
        var idPrecioEspecial = $(this).attr("idPrecioEspecial");

        showPrecioEspecial(idPrecioEspecial);
    });

    function showPrecioEspecial(idPrecioEspecialCabecera) {
        $('body').loadingModal({
            text: 'Abriendo Lista de Precios Especiales...'
        });
        $('body').loadingModal('show');


        $.ajax({
            url: "/PrecioEspecial/Show",
            data: {
                idPrecioEspecialCabecera: idPrecioEspecialCabecera
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: "ERROR",
                    content: 'Ocurrió un error al intentar visualizar la lista de Precios Especiales.',
                    type: 'red',
                    buttons: {
                        OK: function () {
                        }
                    }
                });
            },
            success: function (resultado) {
                $('body').loadingModal('hide');


                var obj = resultado;
                var usuario = resultado.usuario;

                $("#ver_codigo").html(obj.codigo);
                $("#ver_titulo").html(obj.titulo);
                $("#ver_tipo_negociacion").html(obj.tipoNegociacion);
                $("#ver_grupo").html(obj.grupoCliente.codigoNombre);
                $("#ver_cliente").html(obj.clienteSunat.ruc + " - " + obj.clienteSunat.razonSocial);
                $("#ver_fecha_inicio").html(obj.FechaInicioDesc);
                $("#ver_fecha_fin").html(obj.fechaFinDesc);
                $("#ver_registradoPor").html(obj.UsuarioRegistro.nombre);
                $("#ver_fechaRegistro").html(obj.FechaRegistroDesc);
                $("#ver_observaciones").html(obj.observaciones);

              

                if (obj.tipoNegociacion == "RUC") {
                    $("#divVerGrupo").hide();
                }
                else {
                    $("#divVerGrupo").show();
                }


                $("#tableDetallePreciosEspeciales > tbody").empty();

                FooTable.init('#tableDetallePreciosEspeciales');

                var d = '';
                var lista = obj.precios;
                for (var i = 0; i < lista.length; i++) {

                    d += '<tr>' +
                        '<td>' + lista[i].producto.sku + ' - ' + lista[i].producto.descripcion + '</td>' +
                        '<td>' + lista[i].unidadPrecio.Presentacion + '</td>' +
                        '<td>' + lista[i].unidadPrecio.PrecioSinIGV + '</td>' +
                        '<td>' + lista[i].unidadCosto.Presentacion + '</td>' +
                        '<td>' + lista[i].unidadCosto.CostoSinIGV + '</td>' +
                        '<td>' + lista[i].FechaInicioDesc + '</td>' +
                        '<td>' + lista[i].fechaFinDesc + '</td>' +
                        '<td></td>' +
                        '</tr>';

                }
                $("#tableDetallePreciosEspeciales").append(d);

                $("#modalVerPrecioEspecial").modal('show');
            }
        });
    }

    

    $("#btnBusqueda").click(function () {

        $('body').loadingModal({
            text: 'Buscando Listas de Precios Especiales...'
        });
        $('body').loadingModal('show');

        $("#btnBusqueda").attr("disabled", "disabled");
        $.ajax({
            url: "/PrecioEspecial/SearchList",
            type: 'POST',
            dataType: 'JSON',
            error: function () {
                $('body').loadingModal('hide');
                $("#btnBusqueda").removeAttr("disabled");
            },

            success: function (list) {
                $('body').loadingModal('hide');
                $("#btnBusqueda").removeAttr("disabled");
                $("#tableListasPrecios > tbody").empty();
                $("#tableListasPrecios").footable({
                    "paging": {
                        "enabled": true
                    }
                });


                for (var i = 0; i < list.length; i++) {

                    var ItemRow = '<tr data-expanded="true">' +
                        '<td>  ' + list[i].tipoNegociacion + '</td>' +
                        '<td>  ' + list[i].codigo + '  </td>' +
                        '<td>  ' + list[i].clienteOGrupoDesc + '  </td>' +
                        '<td>  ' + list[i].FechaInicioDesc + '  </td>' +
                        '<td>  ' + list[i].fechaFinDesc + '  </td>' +
                        '<td>' +
                        '<button type="button" idPrecioEspecial="' + list[i].idPrecioEspecialCabecera + '" class="btnVerPrecioEspecial btn btn-primary">Ver</button>' 
                        '</td>' +
                        '</tr>';
                    
                    $("#tableListasPrecios").append(ItemRow);

                }

                if (ItemRow.length > 0) {
                    $("#msgBusquedaSinResultados").hide();
                }
                else {
                    $("#msgBusquedaSinResultados").show();
                }

            }
        });
    });

    $("#btnExportExcel").click(function () {
        window.location.href = $(this).attr("actionLink");
    });


    $(document).on('click', "button.btnEditarAjusteAlmacen", function () {
      //  desactivarBotonesVer();
        //Se identifica si existe cotizacion en curso, la consulta es sincrona

        var arrrayClass = event.target.getAttribute("class").split(" ");
        var idRol = arrrayClass[0];
        
        $.ajax({
            url: "/AjusteAlmacen/ConsultarSiExisteAjusteAlmacen",
            type: 'POST',
            async: false,
            dataType: 'JSON',
            data: {
                idRol: idRol
            },
            success: function (resultado) {
                if (resultado.existe == "false") {

                    $.ajax({
                        url: "/AjusteAlmacen/iniciarEdicionAjusteAlmacen",
                        type: 'POST',
                        error: function (detalle) { alert("Ocurrió un problema al iniciar la edición del Ajuste de Almacen."); },
                        success: function (fileName) {
                            window.location = '/AjusteAlmacen/Editar';
                        }
                    });

                }
                else {
                    if (resultado.idRol == 0) {
                        alert('Está creando un nuevo ajuste almacen; para continuar por favor diríjase a la página "Crear/Modificar Ajuste Almacen" y luego haga clic en el botón Cancelar.');
                    }
                    
                    else {
                        alert('Ya se encuentra editando un ajuste almacen para continuar por favor dirigase a la página "Crear/Modificar Ajuste Almacen".');
                    }
                }
            }
        });
    });



});


