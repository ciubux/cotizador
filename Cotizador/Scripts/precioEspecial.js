
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

    $("#tableDetalles").footable({
        "paging": {
            "enabled": false
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

    function ChangeGrupoCliente(valor) {
        $.ajax({
            url: "/PrecioEspecial/ChangeGrupoCliente",
            type: 'POST',
            data: {
                valor: valor
            },
            success: function () { }
        });

        $("#tableDetalles > tbody").empty();
        $("#tableDetalles").footable({
            "paging": {
                "enabled": false
            }
        });
    }

    function ChangeClienteSunat(valor) {
        $.ajax({
            url: "/PrecioEspecial/ChangeClienteSunat",
            type: 'POST',
            data: {
                valor: valor
            },
            success: function () { }
        });

        $("#tableDetalles > tbody").empty();
        $("#tableDetalles").footable({
            "paging": {
                "enabled": false
            }
        });
    }

    $("#obj_codigo").change(function () {
        changeInputString("codigo", $(this).val());
    });

    $("#obj_titulo").change(function () {
        changeInputString("titulo", $(this).val());
    });

    $("#tipoNegociacion").change(function () {
        changeInputString("tipoNegociacion", $(this).val());

        $.ajax({
            url: "/PrecioEspecial/LimpiarDetallesPrecios",
            type: 'POST',
            data: {
            },
            success: function () { }
        });

        $("#tableDetalles > tbody").empty();
        $("#tableDetalles").footable({
            "paging": {
                "enabled": false
            }
        });
    });

    $("#obj_observaciones").change(function () {
        changeInputString("observaciones", $(this).val());
    });

    $("#idGrupoCliente").change(function () {
        ChangeGrupoCliente($(this).val());
    });

    $("#idClienteSunat").change(function () {
        ChangeClienteSunat($(this).val());
    });

    $("#fechaVigenciaDesde").change(function () {
        changeInputDate("fechaInicio", $(this).val());
    });

    $("#fechaVigenciaHasta").change(function () {
        changeInputDate("fechaFin", $(this).val());
    });

    $("#fechaInicio").change(function (event) {
        var fechaInicio = $(this).val();
        var fechaFin = $("#fechaFin").val();

        if (compararFechas(fechaInicio, fechaFin) == 1) {
            fechaFin = fechaInicio;
            $("#fechaFin").val(fechaFin);
            changeInputDate("fechaFin", fechaFin);
        }

        changeInputDate("fechaInicio", fechaInicio);
    });

    $("#fechaFin").change(function () {
        var fechaInicio = $("#fechaInicio").val();
        var fechaFin = $(this).val();

        if (compararFechas(fechaInicio, fechaFin) == 1) {
            fechaFin = fechaInicio;
            $("#fechaFin").val(fechaFin);
        }

        changeInputDate("fechaFin", fechaFin);
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

    function compararFechas(fecha1, fecha2) {
        // Convertir las fechas en formato DD/MM/YY a objetos Date de JavaScript
        var partesFecha1 = fecha1.split("/");
        var fechaObj1 = new Date(partesFecha1[2], partesFecha1[1] - 1, partesFecha1[0]);

        var partesFecha2 = fecha2.split("/");
        var fechaObj2 = new Date(partesFecha2[2], partesFecha2[1] - 1, partesFecha2[0]);

        if (fechaObj1 > fechaObj2) {
            return 1;
        } else {
            return 2;
        }
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

    $("#btnCancelarPrecioEspecial").click(function () {
        ConfirmDialog(MENSAJE_CANCELAR_EDICION, '/PrecioEspecial/CancelarCreacion', null)
    })

    $("#btnFinalizarEdicionPrecioEspecial").click(function () {  
        if ($("#idPrecioEspecialCabecera").val() == "00000000-0000-0000-0000-000000000000") {
            procesarCrearPrecioEspecial();
        }
        else {
            //editarPrecioEspecial();
        }
    });

    function procesarCrearPrecioEspecial() {
        if (!validacionDatosPrecioEspecial())
            return false;

        if ($("#tieneDetallesConflicto").val() == 1) {
            $.confirm({
                title: "Precios con conflicto",
                type: 'orange',
                content: 'Tiene precios con conflicot, estos no serán registrados.',
                buttons: {
                    aceptar: {
                        text: 'ACEPTAR',
                        btnClass: 'btn-success',
                        action: function () {
                            crearPrecioEspecial();
                        }
                    },
                    regresar: {
                        text: 'REGRESAR',
                        btnClass: 'btn-danger',
                        action: function () {

                        }
                    }
                }
            });
        } else {
            crearPrecioEspecial();
        }
    }

    function crearPrecioEspecial() {
        $('body').loadingModal({
            text: 'Creando Rol...'
        });
        $.ajax({
            url: "/PrecioEspecial/Create",
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'Error',
                    content: 'Se generó un error al intentar registrar.',
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });
            },
            success: function (resultado) {
                $('body').loadingModal('hide');
                $.alert({
                    title: "Registro Correcto",
                    content: 'Los precios especiales se registraron correctamente.',
                    type: 'green',
                    buttons: {
                        OK: function () {
                            window.location = '/PrecioEspecial/List';
                        }
                    }
                });
            }
        });

    }

    function validacionDatosPrecioEspecial() {

        if ($("#obj_codigo").val().length < 2) {
            $.alert({
                title: "Código Inválido",
                type: 'orange',
                content: 'Debe ingresar un Código válido.',
                buttons: {
                    OK: function () { $('#obj_codigo').focus(); }
                }
            });
            return false;
        }

        if ($("#obj_titulo").val().length < 4) {
            $.alert({
                title: "Título Inválido",
                type: 'orange',
                content: 'Debe ingresar un título válido.',
                buttons: {
                    OK: function () { $('#obj_titulo').focus(); }
                }
            });
            return false;
        }

        if ($("#tipoNegociacion").val().length < 1) {
            $.alert({
                title: "Tipo Negociación Inválido",
                type: 'orange',
                content: 'Debe seleccionar un tipo de negociación.',
                buttons: {
                    OK: function () { $('#obj_titulo').focus(); }
                }
            });
            return false;
        }

        if ($("#tipoNegociacion").val()  == "GRUPO" && $("#idGrupoCliente").val() == 0) {
            $.alert({
                title: "Grupo Inválido",
                type: 'orange',
                content: 'Debe seleccionar un grupo.',
                buttons: {
                    OK: function () { $('#idGrupoCliente').focus(); }
                }
            });
            return false;
        }

        if ($("#tipoNegociacion").val() == "RUC" && $("#idClienteSunat").val() == 0) {
            $.alert({
                title: "Cliente Inválido",
                type: 'orange',
                content: 'Debe seleccionar un Cliente.',
                buttons: {
                    OK: function () { $('#idClienteSunat').focus(); }
                }
            });
            return false;
        }

        if ($("#tieneDetallesValidos").val() == 0) {
            $.alert({
                title: "Ingresar Precios",
                type: 'orange',
                content: 'Debe ingresar precios válidos.',
                buttons: {
                    OK: function () { }
                }
            });
            return false;
        }

        return true;

    }


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


