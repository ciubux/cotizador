
jQuery(function ($) {
    var pagina = 28

    var ID_SEDE_TODOS = "00000000-0000-0000-0000-000000000000";

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


    $("#fechaCierre").datepicker({ dateFormat: "dd/mm/yy" });
    $("#fechaStock").datepicker({ dateFormat: "dd/mm/yy" });
    

    $(document).ready(function () {

        if ($("#pagina").val() == 722) {
            if ($('#idCiudad option').length > 3) {
                $('#idCiudad').append('<option value = "' + ID_SEDE_TODOS + '" >TODOS</option>');
            }
        }

        $('#tableRVS').footable({
            "paging": {
                "size": 50
            }
        });

        FooTable.init('#tableCargasStock');
    });


    function limpiarFormulario() {
        $("#rubro_codigo").val("");
        $("#rubro_nombre").val("");
    }


    $("#producto_estado_si").click(function () {
        var valCheck = 1;
        changeInputInt("Estado", valCheck)
    });

    $("#producto_estado_no").click(function () {
        var valCheck = 0;
        changeInputInt("Estado", valCheck)
    });

    $("#producto_estado_todos").click(function () {
        var valCheck = -1;
        changeInputInt("Estado", valCheck)
    });

    $("#producto_sku").change(function () {
        changeInputString("sku", $("#producto_sku").val());
    });

    $("#producto_descripcion").change(function () {
        changeInputString("descripcion", $("#producto_descripcion").val());
    });

    $("#tipoVentaRestringidaBusqueda").change(function () {
        changeInputInt("tipoVentaRestringidaBusqueda", $("#tipoVentaRestringidaBusqueda").val());
    });

    function changeInputInt(propiedad, valor) {
        $.ajax({
            url: "/Stock/ChangeInputInt",
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
            url: "/Stock/ChangeInputDecimal",
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
            url: "/Stock/ChangeInputString",
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
            url: "/Stock/ChangeInputBoolean",
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

    $("#btnReporteStockExcel").click(function () {

        var fechaStock = $("#fechaStock").val();
        var tipoUnidad = $("#tipoUnidad").val();
        var ajusteMercaderiaTransito = 0;
        if ($("#chkAjusteMercaderiaTransito").prop('checked')) {
            ajusteMercaderiaTransito = 1;
        }

        var idCiudad = $("#idCiudad").val();

        if (idCiudad == null || idCiudad == "") {
            $.alert({
                title: "FALTA INGRESAR DATOS",
                type: 'orange',
                content: 'Seleccione una Sede',
                buttons: {
                    OK: function () { }
                }
            });
        } else {
            window.location.href = $(this).attr("actionLink") + '?tipoUnidad=' + tipoUnidad + '&fechaStock=' + fechaStock + '&idCiudad=' + idCiudad + '&ajusteMercaderiaTransito=' + ajusteMercaderiaTransito;
        }
        
    });

    $("#btnCargarStock").click(function () {
        var idCiudad = $("#idCiudad").val();
        var sede = $("#idCiudad option:selected").text();
        var fecha = $("#fechaCierre").val();

        var tipoCarga = $("#tipoCarga").val();
        var valido = true;

        if (fecha.trim() == "") {
            alert("Debe ingresar la fecha de cierre stock");
            valido = false;
        }

        if (idCiudad.trim() == "") {
            alert("Debe seleccionar la sede");
            valido = false;
        }

        if (tipoCarga.trim() == "") {
            alert("Debe seleccionar un tipo de inventario.");
            valido = false;
        }

        if (valido) {
            $.confirm({
                title: 'Confirmar Carga',
                content: '¿Está seguro de subir el cierre de stock del almacén de ' + sede + ' a la fecha ' + fecha + '?',
                type: 'orange',
                buttons: {
                    aplica: {
                        text: 'SI',
                        btnClass: 'btn-success',
                        action: function () {
                            $('body').loadingModal({
                                text: 'Leyendo Archivo...'
                            });
                            $('body').loadingModal('show');
                            $("#formCargarStock").submit();
                        }
                    },
                    cancelar: {
                        text: 'Cancelar',
                        btnClass: '',
                        action: function () {
                        }
                    }
                }
            });
        }

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

    $('body').on('click', ".btnVerRVS", function () {
        var idCierreStock = $(this).attr('idCierreStock');

        verRporteCierreStock(idCierreStock);
    });


    $("#btnEjecutarRVS").click(function () {
        var idCierreStock = $(this).attr('idCierreStock');

        ejecutarReporteValidacionStock(idCierreStock);
    });

    function verRporteCierreStock(idCierreStock) {
        $('body').loadingModal({
            text: 'Cargando Reporte Validación Stock...'
        });
        $('body').loadingModal('show');

        $.ajax({
            url: "/Stock/GetCierreStock",
            type: 'POST',
            dataType: 'JSON',
            data: { idCierreStock: idCierreStock },
            error: function () {
                $('body').loadingModal('hide');
                $.alert({
                    title: "ERROR",
                    type: 'red',
                    content: 'Ocurrio un error.',
                    buttons: {
                        OK: function () { }
                    }
                });
            },
            success: function (obj) {

                $("#verSedeRVS").html(obj.sede);
                $("#verFechaRVS").html(obj.fechaDesc);
                $("#verUsuarioValidacionRVS").attr("idArchivoAdjunto", obj.idArchivoAdjunto);
                $("#verUsuarioValidacionRVS").html(obj.usuarioRVS);
                $("#verFechaValidacionRVS").html(obj.fechaRVSDesc);

                $("#btnEjecutarRVS").attr("idCierreStock", idCierreStock);

                var d = "";
                var diferencia;
                var stockZAS;
                var lista = obj.detalles;


                
                $("#tableRVS > tbody").empty();
                FooTable.init('#tableRVS');

                for (var i = 0; i < lista.length; i++) {

                    diferencia = 0;
                    stockZAS = 0;

                    if (lista[i].stockValidable == 1) {
                        stockZAS = lista[i].cantidadConteo + lista[i].diferenciaCantidadValidacion;

                        if (lista[i].diferenciaCantidadValidacion > 0) {
                            diferencia = "+" + lista[i].diferenciaCantidadValidacion;
                        }

                        if (lista[i].diferenciaCantidadValidacion < 0) {
                            diferencia = lista[i].diferenciaCantidadValidacion;
                        }

                        if (lista[i].diferenciaCantidadValidacion == 0) {
                            diferencia = 0;
                        }
                    } else {
                        diferencia = "No Validable";
                        stockZAS = "No Disponible";
                    }

                    d += '<tr>' +
                        '<td>' + lista[i].producto_sku + " " + lista[i].producto_descripcion + '</td>' +
                        '<td>' + lista[i].unidadConteo + '</td>' +
                        '<td>' + lista[i].cantidadConteo + '</td>' +
                        '<td>' + stockZAS + '</td>' +
                        '<td data-sort-value="' + lista[i].diferenciaCantidadValidacion + '"><h4>' + diferencia + '<h4></td>' +
                        '</tr>';

                }

                
                $("#tableRVS").append(d);

                $("#modalReporteValidacionStock").modal('show');

                $('body').loadingModal('hide');
            }
        });
    }

    function ejecutarReporteValidacionStock(idCierreStock) {
        $('body').loadingModal({
            text: 'Ejecutando Validación Stock...'
        });

        setTimeout(function () {
            $('body').loadingModal('show');
        }, 300);

        $.ajax({
            url: "/Stock/EjecutarReporteValidacionStock",
            type: 'POST',
            dataType: 'JSON',
            data: { idCierreStock: idCierreStock },
            error: function () {
                $('body').loadingModal('hide');

                $.alert({
                    title: "ERROR",
                    type: 'red',
                    content: 'Ocurrio un error.',
                    buttons: {
                        OK: function () { }
                    }
                });
            },
            success: function (result) {
                if (result.success == 1) {
                    $('body').loadingModal('hide');

                    $.alert({
                        title: "Ejecución Exitosa",
                        type: 'green',
                        content: 'Se ejecuto el reporte de validación de stock de forma correcta.',
                        buttons: {
                            OK: function () {            
                                verRporteCierreStock(idCierreStock);
                                //volver a cargar vista de reporte de stock
                            }
                        }
                    });
                } else {
                    $('body').loadingModal('hide');
                }
            }
        });
    }
});


