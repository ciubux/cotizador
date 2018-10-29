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

        public Boolean sinPlazoCredito { get; set; }

        public Boolean sinMontoCreditoAprobado { get; set; }

        public Boolean sinPlazoCreditoAprobado { get; set; }
        [Display(Name = "Grupo Cliente:")]
        public GrupoCliente grupoCliente { get; set; }
        [Display(Name = "Pertenece Canal Multiregional:")]
        public Boolean perteneceCanalMultiregional { get; set; }
        [Display(Name = "Pertenece Canal Lima:")]
        public Boolean perteneceCanalLima { get; set; }
        [Display(Name = "Pertenece Canal Provincia:")]
        public Boolean perteneceCanalProvincia { get; set; }
        [Display(Name = "Pertenece Canal PCP:")]
        public Boolean perteneceCanalPCP { get; set; }
        [Display(Name = "Pertenece Canal Ordon:")]
        public Boolean perteneceCanalOrdon { get; set; }
        [Display(Name = "es Sub Distribuidor:")]
        public Boolean esSubDistribuidor { get; set; }

        [Display(Name = "Es Sede Principal:")]
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
                        if (!webString.Equals(""))
                        {
                            webString = webString + sede.ciudad.nombre + "<br>";
                        } else
                        {
                            webString = webString + sede.ciudad.nombre;
                        }
                    }
                }
                return webString;
            }
        }
    }

}