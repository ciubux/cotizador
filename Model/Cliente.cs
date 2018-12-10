using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Cliente : Persona
    {
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
            this.tipoDocumentoIdentidad = DocumentoVenta.TiposDocumentoIdentidad.RUC;
            this.plazoCreditoSolicitado = DocumentoVenta.TipoPago.NoAsignado;
            this.horaInicioPrimerTurnoEntrega = "09:00:00";
            this.horaFinPrimerTurnoEntrega = "18:00:00";
            this.horaInicioSegundoTurnoEntrega = "";
            this.horaFinSegundoTurnoEntrega = "";
            this.clienteAdjuntoList = new List<ClienteAdjunto>();
        }

        public Guid idCliente { get; set; }

        public List<Cliente> sedeList { get; set; }

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

        [Display(Name = "Bloqueado:")]
        public Boolean bloqueado { get; set; }

      //  public Boolean sinPlazoCredito { get; set; }

        public Boolean sinMontoCreditoAprobado { get; set; }

        public Boolean sinPlazoCreditoAprobado { get; set; }
        [Display(Name = "Grupo Cliente:")]
        public GrupoCliente grupoCliente { get; set; }
        [Display(Name = "Pertenece Canal Multiregional")]
        public Boolean perteneceCanalMultiregional { get; set; }
        [Display(Name = "Pertenece Canal Lima")]
        public Boolean perteneceCanalLima { get; set; }
        [Display(Name = "Pertenece Canal Provincia")]
        public Boolean perteneceCanalProvincias { get; set; }
        [Display(Name = "Pertenece Canal PCP")]
        public Boolean perteneceCanalPCP { get; set; }
        [Display(Name = "Pertenece Canal Ordon")]
        public Boolean perteneceCanalOrdon { get; set; }
        [Display(Name = "es Sub Distribuidor:")]
        public Boolean esSubDistribuidor { get; set; }

        [Display(Name = "Cliente habilitado para negociacion multiregional")]
        public Boolean habilitadoNegociacionMultiregional { get { return negociacionMultiregional; } }

        [Display(Name = "Registra cotizaciones multiregionales para este cliente")]
        public Boolean habilitadoSedePrincipal { get { return sedePrincipal; } }

        public String horaInicioPrimerTurnoEntrega { get; set; }

        public String canal { get; set; }

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

        public Usuario usuarioSolicitante { get; set; }

        public List<ClienteAdjunto> clienteAdjuntoList { get; set; }
    }

}