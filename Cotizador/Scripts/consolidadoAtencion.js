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
                throw new Error('Error en la respuesta del servidor');
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
                selected: false 
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
        this.btn.innerText = this.isSelectionMode ? `Ver Seleccionados (${count})` : "Seleccionar Pedidos";
    },

    toggleMode: function () {
        this.isSelectionMode = !this.isSelectionMode;
        this.render();
    },

    render: function () {
        this.body.innerHTML = '';

        // 1. Manejar visualización de columnas
        if (this.isSelectionMode) {
            this.container.classList.remove('no-check');
            this.checkHeaders.forEach(el => el.classList.remove('grid-selector-hidden'));
            this.title.innerText = "Selección de Pedidos";
        } else {
            this.container.classList.add('no-check');
            this.checkHeaders.forEach(el => el.classList.add('grid-selector-hidden'));
            this.title.innerText = "Pedidos Seleccionados";
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


jQuery(function($) {
    $(document).ready(function() { $("#btnBusqueda").click(); });

    var pagina = $("#pagina").val();
    if (pagina == CODIGO_PAGINA_REGISTRO) {
        var idCiudad = $("#consolidado_idCiudad").val();
        var fecha = $("#consolidado_fecha").val();

        if (idCiudad != GUID_EMPTY && fecha != "") {
            GridSelector.init();
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
    });
    
    $("#consolidado_observaciones").change(function () {
        var valor = $("#consolidado_observaciones").val();
        changeInputForm("string", "observaciones", valor);
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

    

    $("#btnGuardarConsolidado").click(function() {
        var id = $("#consolidado_id").val();
        var url = (id == GUID_EMPTY) ? "/ConsolidadoAtencion/Create" : "/ConsolidadoAtencion/Update";

        var data = {
            idConsolidadoAtencion: id,
            fecha: $("#consolidado_fecha").val(),
            idVehiculo: $("#consolidado_idVehiculo").val(),
            idChofer: $("#consolidado_idChofer").val(),
            idAsistente: $("#consolidado_idAsistente").val(),
            observaciones: $("#consolidado_observaciones").val()
        };

        if (!data.idVehiculo || !data.idChofer) {
            alert("Vehículo y Chofer son obligatorios");
            return;
        }

        $('body').loadingModal({ text: 'Guardando...' });
        $.post(url, data, function(res) {
            $('body').loadingModal('hide');
            window.location.reload();
        }, 'JSON');
    });

    $("#btnBusqueda").click(function() {
        $.post("/ConsolidadoAtencion/SearchList", function(list) {
            CONSOLIDADO_LAST_SEARCH = list;
            var rows = "";
            $("#tableConsolidados > tbody").empty();
            $("#tableConsolidados").footable({
                "paging": {
                    "enabled": true
                }
            });

            for (var i = 0; i < list.length; i++) {
                var c = list[i];
                // Formatear fecha de JSON
                var fechaStr = c.fecha ? new Date(parseInt(c.fecha.substr(6))).toLocaleDateString() : "";
                
                rows += '<tr>' +
                    '<td>' + fechaStr + '</td>' +
                    '<td>' + (c.vehiculo ? c.vehiculo.placa : "") + '</td>' +
                    '<td>' + (c.chofer ? c.chofer.apellidoPaterno + ' ' + c.chofer.nombres : "") + '</td>' +
                    '<td>' + (c.asistente ? c.asistente.apellidoPaterno + ' ' + c.asistente.nombres : "") + '</td>' +
                    '<td><button type="button" class="btn btn-primary btnEditar" data-id="' + c.idConsolidadoAtencion + '">Editar</button></td>' +
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

    $("#btnAgregarConsolidado").click(function() {
        limpiarFormulario();
        $("#modalTitle").text("Nuevo Consolidado");
        $("#modalEditarConsolidado").modal("show");
    });
});