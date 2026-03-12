var CONSOLIDADO_LAST_SEARCH;
var GUID_EMPTY = "00000000-0000-0000-0000-000000000000";
var CODIGO_PAGINA_REGISTRO = 701;

const GridSelector = {
    isSelectionMode: false,
    data: [],

    init: async function () {
        this.cacheDOM();
        this.bindEvents();
        //this.render();
        await this.fetchFromDB();
    },

    cacheDOM: function () {
        this.container = document.getElementById('gs-wrapper');
        this.body = document.getElementById('gs-body');
        this.btn = document.getElementById('gs-toggle-btn');
        this.title = document.getElementById('gs-title');
        this.checkHeaders = document.querySelectorAll('.gs-col-check');
    },

    bindEvents: function () {
        this.btn.addEventListener('click', () => this.toggleMode());
    },

    // Simulación de llamada AJAX
    fetchFromDB: async function () {
 
        try {
            this.body.innerHTML = `<div class="grid-selector-cell" style="grid-column: 1 / -1; justify-content: center; padding: 20px;">Cargando datos...</div>`;

            const response = await fetch('/ConsolidadoAtencion/GetPedidosConsolidar', {
                method: 'POST', 
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Error al consultar Pedidos disponibles');
            }

            // Convertimos la respuesta a JSON
            const rawData = await response.json();

            this.data = rawData.map(item => ({
                id: item.idPedido,
                numero: item.numeroPedidoNumeroGrupoString,
                cliente: item.cliente.nombreCliente,
                fecha: item.rangoFechasEntrega,
                estado: item.stockConfirmado,
                obs: item.observaciones || "",
                selected: item.IUEstadoSeleccion 
            }));

            this.render();

        } catch (error) {
            console.error('Error al obtener datos:', error);
            this.body.innerHTML = `<div class="grid-selector-cell" style="grid-column: 1 / -1; justify-content: center; padding: 20px; color: red;">Error al conectar con la base de datos</div>`;
        }

        /*this.data = [
            { id: 1, cliente: "Inversiones Lima", fecha: "2026-01-10", estado: "Completo", obs: "Prioridad alta", selected: false },
            { id: 2, cliente: "Tech Solutions", fecha: "2026-01-15", estado: "Completo", obs: "Revisar contrato", selected: true },
            { id: 3, cliente: "Logística S.A.", fecha: "2026-01-20", estado: "Incompleto", obs: "Pendiente de pago", selected: false }
        ];*/
        this.render();
    },

    toggleSelection: function (id) {
        const item = this.data.find(d => d.id === id);
        if (item) item.selected = !item.selected;
        this.updateButtonLabel();
    },

    updateButtonLabel: function () {
        const count = this.data.filter(d => d.selected).length;
        this.btn.innerText = this.isSelectionMode ? `Terminar Selección (${count})` : "Seleccionar Pedidos";
    },

    toggleMode: async function () {
        this.isSelectionMode = !this.isSelectionMode;
        if (!this.isSelectionMode) {
            const selectedIds = this.data
                .filter(item => item.selected)
                .map(item => item.id);

            const response = await fetch('/ConsolidadoAtencion/SetPedidosSeleccionados', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                // El arreglo debe convertirse a una cadena JSON
                body: JSON.stringify({
                    listaIds: selectedIds
                })
            });
            
            if (!response.ok) {
                throw new Error('Error al guardar pedidos seleccionados');
            }
        }

        this.render();
    },

    render: function () {
        this.body.innerHTML = '';

        var botonGuardar = document.getElementById('btnFinalizarEdicionConsolidado');

        if (this.isSelectionMode) {
            this.container.classList.remove('no-check');
            this.checkHeaders.forEach(el => el.classList.remove('grid-selector-hidden'));
            this.title.innerText = "Selección de Pedidos";
            
            botonGuardar.style.visibility = 'hidden';
        } else {
            this.container.classList.add('no-check');
            this.checkHeaders.forEach(el => el.classList.add('grid-selector-hidden'));
            this.title.innerText = "Pedidos Seleccionados";

            botonGuardar.style.visibility = 'visible';
        }

        this.updateButtonLabel();

        // 2. Filtrar items
        const visibleItems = this.isSelectionMode ? this.data : this.data.filter(d => d.selected);

        if (visibleItems.length === 0) {
            this.body.innerHTML = `<div class="grid-selector-cell" style="grid-column: 1 / -1; justify-content: center; padding: 40px; color: var(--gs-text-muted);">No hay registros para mostrar</div>`;
            return;
        }

        // 3. Crear fragmento para rendimiento
        visibleItems.forEach(item => {
            const rowContent = `
                    <div class="grid-selector-cell gs-col-check ${this.isSelectionMode ? '' : 'grid-selector-hidden'}">
                        <input type="checkbox" class="grid-selector-check-input" ${item.selected ? 'checked' : ''} onchange="GridSelector.toggleSelection('${item.id}')">
                    </div>
                    <div class="grid-selector-cell"><b>${item.numero}</b></div>
                    <div class="grid-selector-cell">${item.cliente}</div>
                    <div class="grid-selector-cell">${item.fecha}</div>
                    <div class="grid-selector-cell">
                        <span class="grid-selector-badge ${item.estado == 1 ? 'grid-selector-badge-active' : 'grid-selector-badge-partial'}">
                            ${item.estado == 1 ? 'Completo' : 'Imcompleto'}
                        </span>
                    </div>
                    <div class="grid-selector-cell" title="${item.obs}">${item.obs}</div>
                `;

            const rowWrapper = document.createElement('div');
            rowWrapper.style.display = 'contents';
            rowWrapper.className = 'grid-selector-row';
            rowWrapper.innerHTML = rowContent;
            this.body.appendChild(rowWrapper);
        });
    }
};

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


jQuery(function($) {
    $(document).ready(function() { $("#btnBusqueda").click(); });

    
    var pagina = $("#pagina").val();
    if (pagina == CODIGO_PAGINA_REGISTRO) {
        
        
        if (document.getElementById('consolidado_fecha')) {
            $("#consolidado_fecha").datepicker({ dateFormat: "dd/mm/yy" });
            var idCiudad = $("#consolidado_idCiudad").val();
            var fecha = $("#consolidado_fecha").val();

            if (idCiudad != GUID_EMPTY && fecha != "") {
                GridSelector.init();
            }
        }

        if (document.getElementById('search_fecha')) {
            $("#search_fecha").datepicker({ dateFormat: "dd/mm/yy" });
        }
    }
    function limpiarFormulario() {
        $("#consolidado_fecha").val(new Date().toISOString().split('T')[0]);
        $("#consolidado_idVehiculo").val("");
        $("#consolidado_idChofer").val("");
        $("#consolidado_idAsistente").val("");
        $("#consolidado_observaciones").val("");
    }

    $("#consolidado_idCiudad").change(function () {
        var valor = $("#consolidado_idCiudad").val();
        changeInputForm("ciudad", "ciudad", valor);
        setTimeout(function () { location.reload(); }, 200);
    }); 

    $("#consolidado_idVehiculo").change(function () {
        var valor = $("#consolidado_idVehiculo").val();
        changeInputForm("vehiculo", "vehiculo", valor);
    });  

    $("#consolidado_idChofer").change(function () {
        var valor = $("#consolidado_idChofer").val();
        changeInputForm("chofer", "chofer", valor);
    });

    $("#consolidado_idAsistente").change(function () {
        var valor = $("#consolidado_idAsistente").val();
        changeInputForm("asistente", "asistente", valor);
    });

    $("#consolidado_fecha").change(function () {
        var valor = $("#consolidado_fecha").val();
        changeInputForm("date", "fecha", valor);
        setTimeout(function () { location.reload(); }, 200);
    });
    
    $("#consolidado_observaciones").change(function () {
        var valor = $("#consolidado_observaciones").val();
        changeInputForm("string", "observaciones", valor);
    });

    $("#search_fecha").change(function () {
        var valor = $("#search_fecha").val();
        changeInputForm("date", "fecha", valor);
    });

    $("#search_ciudad").change(function () {
        var valor = $("#search_ciudad").val();
        changeInputForm("ciudad", "ciudad", valor);
    }); 

    function changeInputForm(tipo, propiedad, valor) {
        $.ajax({
            url: "/ConsolidadoAtencion/ChangeInputForm",
            type: 'POST',
            data: {
                tipo: tipo,
                propiedad: propiedad,
                valor: valor
            },
            success: function () { }
        });
    }


    $("select#idVehiculo").chosen({ placeholder_text_single: "Buscar Vehiculo", no_results_text: "No se encontró Vehículo" }).on('chosen:showing_dropdown', function (evt, params) {
        if ($("#idCiudad").val() == "" || $("#idCiudad").val() == null) {
            alert("Debe seleccionar la sede MP previamente.");
            $("#idVehiculo").trigger('chosen:close');
            $("#idCiudad").focus();
            return false;
        }
    });

    

    $("#btnFinalizarEdicionConsolidado").click(function() {
        var id = $("#idConsolidadoAtencion").val();
        var url = (id == GUID_EMPTY) ? "/ConsolidadoAtencion/Create" : "/ConsolidadoAtencion/Update";

        var data = {
            idConsolidadoAtencion: id,
            fecha: $("#consolidado_fecha").val(),
            idVehiculo: $("#consolidado_idVehiculo").val(),
            idChofer: $("#consolidado_idChofer").val(),
            idAsistente: $("#consolidado_idAsistente").val(),
            observaciones: $("#consolidado_observaciones").val()
        };


        if (data.idVehiculo == "" || data.idVehiculo == "0") {
             $.alert({
                title: 'Datos Incompletos',
                content: 'Debe seleccionar un vehículo.',
                type: 'orange',
                buttons: {
                    OK: function () { }
                }
            });
            $("#consolidado_idVehiculo").focus();
            return;
        }

        if (data.idChofer == "" || data.idChofer == "0") {
            $.alert({
                title: 'Datos Incompletos',
                content: 'Debe seleccionar un chofer.',
                type: 'orange',
                buttons: {
                    OK: function () { }
                }
            });
            $("#consolidado_idChofer").focus();
            return;
        }

        $('body').loadingModal({ text: 'Guardando...' });
        $.post(url, data, function(res) {
            $('body').loadingModal('hide');
            window.location = '/ConsolidadoAtencion/List';
        }, 'JSON');
    });

    $("#btnBusqueda").click(function () {
        var data = {
            idCiudad: $("#search_ciudad").val(),
            fecha: $("#search_fecha").val(),
        };

        if (data.idCiudad == "" || data.idCiudad == "0" || data.fecha == "") {
            $.alert({
                title: 'Datos Incompletos',
                content: 'Debe seleccionar una Sede y una Fecha.',
                type: 'orange',
                buttons: {
                    OK: function () { }
                }
            });

            return;
        }

        $.post("/ConsolidadoAtencion/SearchList", data, function(list) {
            CONSOLIDADO_LAST_SEARCH = list;
            var rows = "";
            $("#tableConsolidados > tbody").empty();
            $("#tableConsolidados").footable({
                "paging": {
                    "enabled": true
                }
            });

            if (list.length > 0) {
                $("#btnExportarExcelRutasLista").show();
                $("#btnExportarExcelConsolidadoRepartoLista").show();
            } else {
                $("#btnExportarExcelRutasLista").hide();
                $("#btnExportarExcelConsolidadoRepartoLista").hide();
            }

            for (var i = 0; i < list.length; i++) {
                var c = list[i];
                
                rows += '<tr idConsolidadoAtencion="' + c.idConsolidadoAtencion + '">' +
                    '<td>' + c.fechaDesc + '</td>' +
                    '<td>' + (c.vehiculo ? c.vehiculo.placa : "") + '</td>' +
                    '<td>' + (c.chofer ? c.chofer.apellidoPaterno + ' ' + c.chofer.nombres : "") + '</td>' +
                    '<td>' + (c.asistente ? c.asistente.apellidoPaterno + ' ' + c.asistente.nombres : "") + '</td>' +
                    '<td><button type="button" class="btn btn-primary btnVerConsolidadoAtencion" idConsolidadoAtencion="' + c.idConsolidadoAtencion + '">Ver</button></td>' +
                    '</tr>';
            }
            $("#tableConsolidados tbody").html(rows);
        }, 'JSON');
    });

    $(document).on('click', '.btnEditar', function() {
        var id = $(this).data("id");
        var c = CONSOLIDADO_LAST_SEARCH.find(x => x.idConsolidadoAtencion == id);
        if (c) {
            $("#consolidado_id").val(c.idConsolidadoAtencion);
            // Formatear fecha para input date (yyyy-mm-dd)
            var dateObj = new Date(parseInt(c.fecha.substr(6)));
            $("#consolidado_fecha").val(dateObj.toISOString().split('T')[0]);
            
            $("#consolidado_idVehiculo").val(c.vehiculo ? c.vehiculo.idVehiculo : "");
            $("#consolidado_idChofer").val(c.chofer ? c.chofer.idPersonalAlmacen : "");
            $("#consolidado_idAsistente").val(c.asistente ? c.asistente.idPersonalAlmacen : "");
            $("#consolidado_observaciones").val(c.observaciones);

            $("#modalTitle").text("Editar Consolidado");
            $("#modalEditarConsolidado").modal("show");
        }
    });

    $(document).on('click', "button.btnVerConsolidadoAtencion", function () {
        $('body').loadingModal({
            text: 'Abriendo Consolidado Atención...'
        });
        $('body').loadingModal('show');

        var idConsolidado = event.target.getAttribute("idConsolidadoAtencion");
        
        $.ajax({
            url: "/ConsolidadoAtencion/Show",
            data: {
                idConsolidadoAtencion: idConsolidado
            },
            type: 'POST',
            dataType: 'JSON',
            error: function (detalle) {
                $('body').loadingModal('hide');
                $.alert({
                    title: 'ERROR',
                    content: 'OCURRIÓ UN ERROR AL CONSULTAR EL CONSOLIDADO DE ATENCIÓN.',
                    type: 'red',
                    buttons: {
                        OK: function () { }
                    }
                });
            },
            success: function (result) {
                var obj = result.consolidadoAtencion;
                $('body').loadingModal('hide')
                $("#ver_idConsolidadoAtencion").html(obj.idConsolidadoAtencion);
                $("#ver_ciudad").html(obj.ciudad.nombre);
                $("#ver_fecha").html(obj.fechaDesc);
                $("#ver_vehiculo").html(obj.vehiculo.placa);
                $("#ver_chofer").html(obj.chofer.nombreCompleto);
                $("#ver_asistente").html(obj.asistente.nombreCompleto);
                $("#ver_observaciones").html(obj.observaciones);
             
                var pedidosList = obj.pedidos;

                if (obj.esEditable) {
                    $("#btnEditar").show();
                } else {
                    $("#btnEditar").hide();
                }

                $("#tablePedidosConsolidado > tbody").empty();
                for (var i = 0; i < pedidosList.length; i++) {
                    var estadoStock = pedidosList.stockConfirmado == 1 ? 'Completo' : 'Imcompleto';

                    var clienteRow = '<tr data-expanded="true">' +
                        '<td>  ' + pedidosList[i].idPedido + '</td>' +
                        '<td>  ' + pedidosList[i].numeroPedido + '  </td>' +
                        '<td>  ' + pedidosList[i].cliente.nombreCliente + '  </td>' +
                        '<td>  ' + pedidosList[i].rangoFechasEntrega + ' </td>' +
                        '<td>  ' + estadoStock + '</td>' +
                        '<td>  ' + pedidosList[i].observaciones + '  </td>' +
                        '</tr>';

                    $("#tablePedidosConsolidado").append(clienteRow);

                }
                FooTable.init('#tablePedidosConsolidado');


                $("#modalVerConsolidadoAtencion").modal('show');
            }
        });
    });


    $("#btnExportarExcelConsolidadoRepartoLista").click(function () {
        var listaIds = [];

        $("#tableConsolidados > tbody tr").each(function () {
            listaIds.push($(this).attr("idConsolidadoAtencion"));
        });


        if (listaIds.length > 0) {
            GenerarExcelReparto(listaIds);
        }
    });

    function GenerarExcelReparto(listaIds) {
        var data = {
            idsConsolidados: listaIds
        };

        $.post("/ConsolidadoAtencion/DataReparto", data, function (res) {
            var lista = res.lista;
            if (!lista || lista.length === 0) return;

            var dataExcel = [];

            var maxPedidos = 0;
            var nombreArchivo = "";
            lista.forEach(function (c) {
                if (c.pedidos && c.pedidos.length > maxPedidos) {
                    maxPedidos = c.pedidos.length;
                }

                nombreArchivo = "CONSOLIDADO_REPARTO_" + c.fechaDesc.split('/').reverse().join('') + c.ciudad.nombre;
            });

            maxPedidos = maxPedidos > 10 ? maxPedidos : 10;

            lista.forEach(function (c) {
                let cabecera = {
                    placaVehiculo: c.vehiculo.placa,
                    fecha: c.fechaDesc,
                    numero: "",
                    responsable: c.chofer.nombres + ' ' + c.chofer.apellidoPaterno,
                    asistente: c.asistente ? c.asistente.nombres + ' ' + c.asistente.apellidoPaterno : "-",
                    unidad: c.vehiculo.placa
                };

                let productos = [];


                c.resumenDetalleGuias.forEach(function (p) {
                    let producto = {
                        sku: p.producto.sku,
                        cantidad: p.cantidad,
                        unidad: p.unidad,
                        descripcion: p.producto.descripcion
                    };

                    productos.push(producto);
                });

                let guias = [];

                c.guias.forEach(function (g) {
                    let guia = {
                        numero: g.serieDocumento + '-' + g.numeroDocumento,
                        cliente: g.clienteVer.razonSocial
                    };

                    guias.push(guia);
                });

                dataExcel.push(
                    {
                        cabecera: cabecera,
                        productos: productos, 
                        guias: guias
                    }
                );
            });


            const urlLogo = "http://localhost:55996/images/logos/logo_MP.png"; 
            ExportarExcelConsolidadoReparto(dataExcel, urlLogo, nombreArchivo);

        }, 'JSON');

        // Estructura de prueba con 2 consolidados 
        /*const misConsolidados = [
            {
                cabecera: {
                    placaVehiculo: "ABC-123", 
                    fecha: "27-Ene",
                    numero: "1024",
                    responsable: "Luis Tapahuasco",
                    asistente: "Miguel Campos",
                    unidad: "ABC-123"
                },
                productos: [
                    { sku: "HT2S41", cantidad: 4, unidad: "CAJA X 4", descripcion: "PT Scott JRT Airflex Ahorramax nvo. emp. (20 gr.), rollo x 200 mts" },
                    { sku: "HT0S32", cantidad: 4, unidad: "CAJA X 25", descripcion: "PT interf. Scott Airflex 1 ply (28 gr.), pqte x 150 hjs" },
                    { sku: "HJ3F40", cantidad: 1, unidad: "SACHET", descripcion: "Jabón espuma Scott Pure Supreme antib. sin triclosán, sachet x 800 ml" },
                    { sku: "HH2S80", cantidad: 12, unidad: "ROLLO", descripcion: "PH Scott JRT Rindemax gofrado con pre-corte, rollo x 550 mts" }
                ],
                guias: [
                    { numero: "11486", cliente: "J & H INVERSIONES PUNTO AZUL S.A.C." },
                    { numero: "11487", cliente: "PESCADOS PERUANOS P & A S.A.C." }
                ]
            },
            {
                cabecera: {
                    placaVehiculo: "XYZ-789", 
                    fecha: "27-Ene",
                    numero: "1025",
                    responsable: "Xavier Rivas",
                    asistente: "Moises Atienza",
                    unidad: "XYZ-789"
                },
                productos: [
                    { sku: "HT0S32", cantidad: 13, unidad: "CAJA X 25", descripcion: "PT interf. Scott Airflex 1 ply (28 gr.), pqte x 150 hjs" },
                    { sku: "HH2S80", cantidad: 3, unidad: "CAJA X 12", descripcion: "PH Scott JRT Rindemax gofrado con pre-corte, rollo x 550 mts" },
                    { sku: "HJ1F10", cantidad: 6, unidad: "SACHET", descripcion: "Jabón spray Scott Handlotion Rindemax, sachet x 800 ml" },
                    { sku: "HT2S41", cantidad: 90, unidad: "CAJA X 4", descripcion: "PT Scott JRT Airflex Ahorramax nvo. emp. (20 gr.), rollo x 200 mts" }
                ],
                guias: [
                    { numero: "11492", cliente: "UNION PAK DEL PERU S.A." },
                    { numero: "", cliente: "SERVICIOS GASTRONOMICOS P&A S.A.C." }, 
                    { numero: "1218", cliente: "DISTRIPLUS SOCIEDAD ANONIMA CERRADA" }
                ]
            }
        ];

        const urlLogo = "http://localhost:55996/images/logos/logo_MP.png"; 

        ExportarExcelConsolidadoReparto(misConsolidados, urlLogo, "ConsolidadoReparto_");*/
    }

    async function ExportarExcelConsolidadoReparto(listaConsolidados, urlLogo, nombreArchivo) {
        const workbook = new ExcelJS.Workbook();

        let imageId = null;
        let originalWidth = 0;
        let originalHeight = 0;

        const imgData = obtenerBase64DeImagen('imagenMP');

        if (imgData) {
            imageId = workbook.addImage({
                base64: imgData.base64,
                extension: 'png',
            });

            originalWidth = imgData.width;
            originalHeight = imgData.height;
        }

        const bordeFino = { style: 'thin', color: { argb: 'FF000000' } };
        const bordesCompletos = { top: bordeFino, left: bordeFino, bottom: bordeFino, right: bordeFino };
        const fondoGrisClaro = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'ff0066cc' } };
        const fondoGrisOscuro = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'ff0066cc' } };

        let nroHoja = 0;
        listaConsolidados.forEach((consolidado) => {
            nroHoja = nroHoja + 1;
            const nombreHoja = consolidado.cabecera.placaVehiculo || `Consolidado_${nroHoja}`;

            const worksheet = workbook.addWorksheet(nombreHoja);

            worksheet.getColumn('A').width = 3;
            worksheet.getColumn('B').width = 18;
            worksheet.getColumn('C').width = 12;
            worksheet.getColumn('D').width = 25;
            worksheet.getColumn('E').width = 70;

            worksheet.views = [{ state: 'frozen', xSplit: 0, ySplit: 13 }];

            if (imageId !== null) {
                const anchoDeseado = 200;
                const altoProporcional = (originalHeight / originalWidth) * anchoDeseado;

                worksheet.addImage(imageId, {
                    tl: { col: 4.9, row: 0.2 },
                    ext: { width: anchoDeseado, height: altoProporcional },
                    editAs: 'oneCell'
                });
            }

            worksheet.getCell('B2').value = "FECHA:";
            worksheet.getCell('B2').font = { bold: true };
            worksheet.getCell('B2').alignment = { horizontal: 'right' };

            worksheet.getCell('C2').value = consolidado.cabecera.fecha;
            worksheet.getCell('C2').font = { bold: true };
            worksheet.getCell('C2').border = bordesCompletos;
            worksheet.getCell('C2').alignment = { horizontal: 'center' };

            worksheet.getCell('B3').value = "N°";
            worksheet.getCell('B3').font = { bold: true };
            worksheet.getCell('B3').alignment = { horizontal: 'right' };

            worksheet.getCell('C3').value = consolidado.cabecera.numero;
            worksheet.getCell('C3').border = bordesCompletos;
            worksheet.getCell('C3').alignment = { horizontal: 'center' };

            worksheet.mergeCells('B5:E5');
            const celdaTitulo = worksheet.getCell('B5');
            celdaTitulo.value = "CONSOLIDADO DE CARGA";
            celdaTitulo.alignment = { horizontal: 'center' };
            celdaTitulo.font = { bold: true, color: { argb: 'FFFFFFFF' } };  
            celdaTitulo.fill = fondoGrisOscuro;

            worksheet.getRow(8).height = 5;
            worksheet.getRow(10).height = 5;

            const etiquetas = [
                { fila: 7, label: "RESPONSABLE", valor: consolidado.cabecera.responsable },
                { fila: 9, label: "ASISTENTE", valor: consolidado.cabecera.asistente },
                { fila: 11, label: "UNIDAD", valor: consolidado.cabecera.unidad }
            ];

            etiquetas.forEach(item => {
                let celdaLabel = worksheet.getCell(`B${item.fila}`);
                celdaLabel.value = item.label;
                celdaLabel.font = { bold: true, color: { argb: 'FFFFFFFF' } };  
                celdaLabel.fill = fondoGrisClaro;

                let celdaValor = worksheet.getCell(`C${item.fila}`);
                celdaValor.value = item.valor;
                celdaValor.font = { bold: true };
            });

            const filaCabeceraProd = worksheet.getRow(13);
            filaCabeceraProd.values = [null, "SKU", "CANT.", "UNIDAD", "DESCRIPCIÓN"];

            ['B', 'C', 'D', 'E'].forEach(col => {
                let celda = worksheet.getCell(`${col}13`);
                celda.fill = fondoGrisOscuro;
                celda.font = { bold: true, color: { argb: 'FFFFFFFF' } };
                celda.alignment = { horizontal: 'center', vertical: 'middle' };
                celda.border = bordesCompletos;
            });

            worksheet.autoFilter = 'B13:E13';

            let filaActual = 14;
            consolidado.productos.forEach(prod => {
                const row = worksheet.getRow(filaActual);
                row.values = [null, prod.sku, prod.cantidad, prod.unidad, prod.descripcion];

                ['B', 'C', 'D', 'E'].forEach(col => {
                    row.getCell(col).border = { left: bordeFino, right: bordeFino };
                });

                row.getCell('C').alignment = { horizontal: 'center' };
                filaActual++;
            });

            ['B', 'C', 'D', 'E'].forEach(col => {
                worksheet.getCell(`${col}${filaActual - 1}`).border = {
                    left: bordeFino, right: bordeFino, bottom: bordeFino
                };
            });

            filaActual += 2;

            worksheet.getCell(`B${filaActual}`).value = "GUÍA N°";
            worksheet.getCell(`B${filaActual}`).font = { bold: true };

            worksheet.getCell(`C${filaActual}`).value = "CLIENTE";
            worksheet.getCell(`C${filaActual}`).font = { bold: true };
            filaActual++;

            consolidado.guias.forEach(guia => {
                const row = worksheet.getRow(filaActual);
                row.values = [null, guia.numero, guia.cliente];
                row.getCell('B').alignment = { horizontal: 'center' };
                filaActual++;
            });
        }); 

        const buffer = await workbook.xlsx.writeBuffer();
        const blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });

        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = `${nombreArchivo}.xlsx`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }

    $("#btnExportarExcelRutasLista").click(function () {
        var listaIds = [];

        $("#tableConsolidados > tbody tr").each(function () {
            listaIds.push($(this).attr("idConsolidadoAtencion"));
        });


        if (listaIds.length > 0) {
            GenerarExcelRutas(listaIds);
        }        
    });

    function GenerarExcelRutas(listaIds) {
        var data = {
            idsConsolidados: listaIds
        };

        $.post("/ConsolidadoAtencion/DataRutas", data, function (res) {
            var lista = res.lista;
            if (!lista || lista.length === 0) return;

            var dataExcel = [];

            var maxPedidos = 0;
            var nombreArchivo = "";
            lista.forEach(function (c) {
                if (c.pedidos && c.pedidos.length > maxPedidos) {
                    maxPedidos = c.pedidos.length;
                }

                nombreArchivo = "RUTAS_" + c.fechaDesc.split('/').reverse().join('') + c.ciudad.nombre;
            });

            maxPedidos = maxPedidos > 10 ? maxPedidos : 10;

            var fila1 = [""];
            var fila2 = [""];

            lista.forEach(function (c) {
                var nombreChoferAsistente = c.chofer.nombres + ' ' + c.chofer.apellidoPaterno;

                if (c.asistente) {
                    nombreChoferAsistente = nombreChoferAsistente + '/' + c.asistente.nombres + ' ' + c.asistente.apellidoPaterno;
                }
                
                fila1.push(c.vehiculo.placa || "", "", nombreChoferAsistente || "", "");
                fila2.push("N°", "N° Pedido", "Cliente", "");
            });

            dataExcel.push([""]);
            dataExcel.push(fila1);
            dataExcel.push(fila2);

            for (var i = 0; i < maxPedidos; i++) {
                var filaDatos = [""];

                lista.forEach(function (c) {
                    if (c.pedidos && c.pedidos[i]) {
                        var ped = c.pedidos[i];
                        filaDatos.push((i + 1) || "", ped.numeroPedido || "", ped.cliente.nombreCliente || "", "");
                    } else {
                        filaDatos.push("", "", "", "");
                    }
                });

                dataExcel.push(filaDatos);
            }

            ExportarExcelRutas(dataExcel, nombreArchivo, "RUTA", lista.length);

        }, 'JSON');
    }

    async function ExportarExcelRutas(dataExcel, nombreArchivo, nombreHoja, cantidadConsolidados) {
        const workbook = new ExcelJS.Workbook();
        const worksheet = workbook.addWorksheet(nombreHoja);

        dataExcel.forEach(row => worksheet.addRow(row));

        for (let i = 0; i < cantidadConsolidados; i++) {
            let colInicio = (i * 4) + 2;      
            let colFin = colInicio + 1;        
            worksheet.mergeCells(2, colInicio, 2, colFin); 
        }

        [2, 3].forEach(numFila => {
            worksheet.getRow(numFila).eachCell((cell, colNumber) => {
                cell.alignment = { horizontal: 'center', vertical: 'middle' }; 

                if ((colNumber - 1) % 4 === 0) {
                    cell.font = { bold: true, color: { argb: 'FF000000' } };
                    cell.fill = { type: 'pattern', pattern: 'none' };
                } else {
                    cell.font = { bold: true, color: { argb: 'FFFFFFFF' } };  
                    cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'ff0066cc' }  
                    };

                }
            });
        });


        const patronAnchos = [4, 6, 12, 40];

        for (let i = 1; i <= worksheet.columnCount; i++) {
            let indicePatron = (i - 1) % patronAnchos.length;

            worksheet.getColumn(i).width = patronAnchos[indicePatron];
        }

        const bordeNegro = { style: 'thin', color: { argb: 'FF000000' } };

        worksheet.eachRow((row, rowNumber) => {
            row.eachCell((cell, colNumber) => {
                if (!((colNumber - 1) % 4 === 0) && rowNumber > 1) {
                    if (rowNumber === 2 || rowNumber === 3) {
                        cell.border = {
                            top: bordeNegro,
                            left: bordeNegro,
                            bottom: bordeNegro,
                            right: bordeNegro
                        };
                    } else {
                        let bordesDatos = {
                            left: bordeNegro,
                            right: bordeNegro
                        };


                        if (rowNumber === worksheet.rowCount) {
                            bordesDatos.bottom = bordeNegro;
                        }

                        cell.border = bordesDatos;
                        cell.fill = { type: 'pattern', pattern: 'solid', fgColor: { argb: 'ffffffff' } };
                    }
                }
            });
        });

        /*
        worksheet.columns.forEach(col => {
            col.width = 16;
        });
        */
        // Obtener la fecha y hora actual para el nombre del archivo
        const now = new Date();
        const formattedDate = now.toISOString().slice(0, 10).replace(/-/g, "");  // yyyymmdd
        const formattedTime = now.toTimeString().slice(0, 8).replace(/:/g, "");  // hhmmss
        const fileName = `${nombreArchivo}_${formattedDate}${formattedTime}.xlsx`;

        // Generar el archivo Excel como un blob
        const buffer = await workbook.xlsx.writeBuffer();
        const blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });

        // Crear un enlace de descarga y simular el clic
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }

    async function obtenerImagenBase64(url) {
        try {
            const response = await fetch(url);
            const blob = await response.blob();
            return new Promise((resolve, reject) => {
                const reader = new FileReader();
                reader.readAsDataURL(blob);
                reader.onloadend = () => resolve(reader.result);
                reader.onerror = reject;
            });
        } catch (error) {
            console.error("Error al cargar la imagen:", error);
            return null;
        }
    }

    function obtenerBase64DeImagen(imgId) {
        const img = document.getElementById(imgId);
        if (!img) return null;

        const canvas = document.createElement('canvas');
        canvas.width = img.naturalWidth || img.width;
        canvas.height = img.naturalHeight || img.height;

        const ctx = canvas.getContext('2d');
        ctx.drawImage(img, 0, 0);

        // Ahora devolvemos un objeto con la imagen y sus medidas
        return {
            base64: canvas.toDataURL('image/png'),
            width: canvas.width,
            height: canvas.height
        };
    }
    /*
    function obtenerBase64DeImagen(imgId) {
        const img = document.getElementById(imgId);
        if (!img) {
            console.error("No se encontró la imagen con el ID:", imgId);
            return null;
        }

        // Crear un canvas para dibujar la imagen y extraer su código Base64
        const canvas = document.createElement('canvas');
        canvas.width = img.naturalWidth || img.width;
        canvas.height = img.naturalHeight || img.height;

        const ctx = canvas.getContext('2d');
        ctx.drawImage(img, 0, 0);

        // Retorna el string en base64
        return canvas.toDataURL('image/png');
    }*/

    /*
    async function ExportarExcelRutas(dataExcel, nombreArchivo, nombreHoja) {
        const workbook = new ExcelJS.Workbook();
        const worksheet = workbook.addWorksheet(nombreHoja);

        dataExcel.forEach(row => worksheet.addRow(row));

        // Estilos para la cabecera
        worksheet.getRow(2).eachCell((cell) => {
            cell.font = { bold: true, color: { argb: 'FFFFFFFF' } };  // Texto blanco en negrita
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'ff0066cc' }  // Fondo azul
            };
            cell.alignment = { horizontal: 'center' };  // Alineación centrada
        });
        worksheet.getRow(3).eachCell((cell) => {
            cell.font = { bold: true, color: { argb: 'FFFFFFFF' } };  // Texto blanco en negrita
            cell.fill = {
                type: 'pattern',
                pattern: 'solid',
                fgColor: { argb: 'ff0066cc' }  // Fondo azul
            };
            cell.alignment = { horizontal: 'center' };  // Alineación centrada
        });

        

        // Obtener la fecha y hora actual para el nombre del archivo
        const now = new Date();
        const formattedDate = now.toISOString().slice(0, 10).replace(/-/g, "");  // yyyymmdd
        const formattedTime = now.toTimeString().slice(0, 8).replace(/:/g, "");  // hhmmss
        const fileName = `${nombreArchivo}_${formattedDate}${formattedTime}.xlsx`;

        // Generar el archivo Excel como un blob
        const buffer = await workbook.xlsx.writeBuffer();
        const blob = new Blob([buffer], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });

        // Crear un enlace de descarga y simular el clic
        const link = document.createElement('a');
        link.href = URL.createObjectURL(blob);
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    }*/
});