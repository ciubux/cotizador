using Model.CONFIGCLASSES;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Cliente : Persona
    {
        public const string NOMBRE_TABLA = "CLIENTE";

        public Cliente()
        {
            //Cada vez que se instancia un cliente se instancia con al menos una dirección de entrega
            this.direccionEntregaList = new List<DireccionEntrega>();
            this.solicitanteList = new List<Solicitante>();
            this.sedeList = new List<Cliente>();

            this.ubigeo = new Ubigeo();
            this.responsableComercial = new Vendedor();
            this.asistenteServicioCliente = new Vendedor();
            this.responsabelPortafolio = new Vendedor();
            this.supervisorComercial = new Vendedor();
            this.origen = new Origen();
            this.subDistribuidor = new SubDistribuidor();
            this.subDistribuidor.idSubDistribuidor = 0;

            this.rubro = new Rubro();
            this.rubro.idRubro = 0;
            this.tipoDocumentoIdentidad = DocumentoVenta.TiposDocumentoIdentidad.RUC;
            this.plazoCreditoSolicitado = DocumentoVenta.TipoPago.NoAsignado;
            this.horaInicioPrimerTurnoEntrega = "09:00:00";
            this.horaFinPrimerTurnoEntrega = "18:00:00";
            this.horaInicioSegundoTurnoEntrega = "";
            this.horaFinSegundoTurnoEntrega = "";
            this.CargaMasiva = false;
            this.clienteAdjuntoList = new List<ClienteAdjunto>();
            this.esClienteLite = false;

            this.chrAsesor = new ClienteReasignacionHistorico();
            this.chrAsesor.fechaInicioVigencia = DateTime.Now;
            this.chrAsesor.preValor = this.responsableComercial.idVendedor.ToString();
            this.chrAsistente = new ClienteReasignacionHistorico();
            this.chrAsistente.fechaInicioVigencia = DateTime.Now;
            this.chrAsistente.preValor = this.asistenteServicioCliente.idVendedor.ToString();
            this.chrSupervisor = new ClienteReasignacionHistorico();
            this.chrSupervisor.fechaInicioVigencia = DateTime.Now;
            this.chrSupervisor.preValor = this.supervisorComercial.idVendedor.ToString();
        }

        public Guid idCliente { get; set; }

        public List<Cliente> sedeList { get; set; }

        public List<DocumentoDetalle> listaPrecios { get; set; }

        [Display(Name = "Asesor Comercial:")]
        public Vendedor responsableComercial { get; set; }
        [Display(Name = "Asistente de Atención al Cliente:")]
        public Vendedor asistenteServicioCliente { get; set; }
        [Display(Name = "Responsable Portafolio:")]
        public Vendedor responsabelPortafolio { get; set; }
        [Display(Name = "Supervisor Comercial:")]
        public Vendedor supervisorComercial { get; set; }
        [Display(Name = "Observaciones (Créditos y Cobranzas):")]
        public String observacionesCredito { get; set; }

        [Display(Name = "N° Doc / Razón Social / Nombre:")]
        public String textoBusqueda { get; set; }

        //  public Boolean sinPlazoCredito { get; set; }

        public Boolean sinMontoCreditoAprobado { get; set; }

        public Boolean esClienteLite { get; set; }
        public int  clienteLiteValue { get { return esClienteLite ? 1 : 0; } }

        public Boolean sinPlazoCreditoAprobado { get; set; }
        [Display(Name = "Grupo Cliente:")]
        public GrupoCliente grupoCliente { get; set; }
        [Display(Name = "Multiregional")]
        public Boolean perteneceCanalMultiregional { get; set; }
        [Display(Name = "Lima")]
        public Boolean perteneceCanalLima { get; set; }
        [Display(Name = "Provincias")]
        public Boolean perteneceCanalProvincias { get; set; }
        [Display(Name = "PCP")]
        public Boolean perteneceCanalPCP { get; set; }
        [Display(Name = "Pertenece Canal Ordon")]
        public Boolean perteneceCanalOrdon { get; set; }
        [Display(Name = "Sub Distribuidor")]
        public Boolean esSubDistribuidor { get; set; }

        [Display(Name = "Cliente habilitado para negociacion multiregional")]
        public Boolean habilitadoNegociacionMultiregional { get { return negociacionMultiregional; } }

        [Display(Name = "Registra cotizaciones multiregionales para este cliente")]
        public Boolean habilitadoSedePrincipal { get { return sedePrincipal; } }

        [Display(Name = "Habilitado para negociación grupal")]
        public Boolean habilitadoNegociacionGrupal { get; set; }

        public String horaInicioPrimerTurnoEntrega { get; set; }

        [Display(Name = "Observaciones Horario Entrega:")]
        public String observacionHorarioEntrega { get; set; }

        public String canal { get; set; }

        public Boolean habilitadoModificarDireccionEntrega { get; set; }

        public String horaInicioPrimerTurnoEntregaFormat { get
            {
                if (horaInicioPrimerTurnoEntrega != null && horaInicioPrimerTurnoEntrega.Length >= 5)
                {
                    return this.horaInicioPrimerTurnoEntrega.Substring(0, 5);
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        [Display(Name = "Nombre Cliente:")]
        public String nombreCliente
        {
            get
            {
                String textNombre = this.nombreComercialSunat;

                if (this.tipoDocumentoIdentidad == DocumentoVenta.TiposDocumentoIdentidad.RUC)
                {
                    textNombre = this.razonSocial + " (" + this.nombreComercial + ")";
                }

                if (this.tipoDocumentoIdentidad == DocumentoVenta.TiposDocumentoIdentidad.DNI)
                {
                    textNombre = this.nombreComercial;
                }

                return textNombre;
            }
        }

        public String horaFinPrimerTurnoEntrega { get; set; }

        public String horaFinPrimerTurnoEntregaFormat
        {
            get
            {
                if (horaFinPrimerTurnoEntrega != null && horaFinPrimerTurnoEntrega.Length >= 5)
                {
                    return this.horaFinPrimerTurnoEntrega.Substring(0, 5);
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        public String horaInicioSegundoTurnoEntrega { get; set; }

        public String horaInicioSegundoTurnoEntregaFormat
        {
            get
            {
                if (horaInicioSegundoTurnoEntrega != null && horaInicioSegundoTurnoEntrega.Length >= 5)
                {
                    return this.horaInicioSegundoTurnoEntrega.Substring(0, 5);
                }
                else
                {
                    return String.Empty;
                }
            }
        }
        public String horaFinSegundoTurnoEntrega { get; set; }

        public String horaFinSegundoTurnoEntregaFormat
        {
            get
            {
                if (horaFinSegundoTurnoEntrega != null && horaFinSegundoTurnoEntrega.Length >= 5)
                {
                    return this.horaFinSegundoTurnoEntrega.Substring(0, 5);
                }
                else
                {
                    return String.Empty;
                }
            }
        }


        [Display(Name = "Primer Turno de Entrega:")]
        public String primerTurnoEntrega
        {
            get
            {
                return "Desde: " + this.horaInicioPrimerTurnoEntrega + " Hasta: " + this.horaFinPrimerTurnoEntrega;
            }
        }


        [Display(Name = "Segundo Turno de Entrega:")]
        public String segundoTurnoEntrega
        {
            get
            {
                return "Desde: " + this.horaInicioSegundoTurnoEntrega + " Hasta: " + this.horaFinSegundoTurnoEntrega;
            }
        }


        [Display(Name = "Genera Cotizaciones Multergionales:")]
        public Boolean sedePrincipal { get; set; }

        public String sedeListWebString
        {
            get
            {
                String webString = "";

                foreach (Cliente sede in this.sedeList)
                {
                    if (sede.ciudad != null)
                    {
                        if (webString.Equals(""))
                        {
                            webString = sede.ciudad.nombre;
                        } else
                        {
                            webString = webString + "<br>" + sede.ciudad.nombre;
                        }
                    }
                }
                return webString;
            }
        }

        [Display(Name = "Negociación Multiregional:")]
        public Boolean negociacionMultiregional { get; set; }

        public Boolean existenCambiosCreditos { get; set; }


        public ClienteConfiguracion configuraciones { get; set; }

        public Usuario usuarioSolicitante { get; set; }

        public List<ClienteAdjunto> clienteAdjuntoList { get; set; }

        [Display(Name = "Origen:")]
        public Origen origen
        {
            get; set;
        }

        [Display(Name = "Categoría:")]
        public SubDistribuidor subDistribuidor
        {
            get; set;
        }

        [Display(Name = "Rubro:")]
        public Rubro rubro
        {
            get; set;
        }

        public ClienteReasignacionHistorico chrAsesor { get; set; }
        public ClienteReasignacionHistorico chrSupervisor { get; set; }
        public ClienteReasignacionHistorico chrAsistente { get; set; }

        public int modificaCanasta 
        {
            get
            {
                int access = 0;
                if (this.usuario != null)
                {
                    access = this.usuario.modificaCanastaCliente ? 1 : 0;
                }
                else
                {
                    return 0;
                }

                return this.usuario.modificaCanastaCliente || this.isOwner ? 1 : 0;
            }
        }

        public int modificaGrupo
        {
            get
            {
                if (this.usuario == null) return 0;

                return this.usuario.modificaMiembrosGrupoCliente || this.isOwner ? 1 : 0;
            }
        }

        public bool isOwner
        {
            get
            {
                if (this.usuario == null) return false;

                if (this.asistenteServicioCliente != null && this.asistenteServicioCliente.usuario != null && this.asistenteServicioCliente.usuario.idUsuario == this.usuario.idUsuario)
                {
                    return true;
                }

                if (this.responsableComercial != null && this.responsableComercial.usuario != null && this.responsableComercial.usuario.idUsuario == this.usuario.idUsuario)
                {
                    return true;
                }

                if (this.supervisorComercial != null && this.supervisorComercial.usuario != null && this.supervisorComercial.usuario.idUsuario == this.usuario.idUsuario)
                {
                    return true;
                }

                if (this.IdUsuarioRegistro == this.usuario.idUsuario
                    && (this.asistenteServicioCliente == null || this.asistenteServicioCliente.usuario == null)
                    && (this.responsableComercial == null || this.responsableComercial.usuario == null)
                    && (this.supervisorComercial == null || this.supervisorComercial.usuario == null))
                {
                    return true;
                }

                return false;
            }
        }
        [Display(Name = "SKU:")]
        public String sku { get; set; }
        [Display(Name = "Fecha Ventas Desde:")]
        public DateTime? fechaVentasDesde { get; set; }

        public Boolean fechaVentasEsModificada { get; set; }
    }

}
