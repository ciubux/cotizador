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
            this.ubigeo = new Ubigeo();
            this.responsableComercial = new Vendedor();
            this.asistenteServicioCliente = new Vendedor();
            this.responsabelPortafolio = new Vendedor();
            this.supervisorComercial = new Vendedor();
            this.tipoDocumentoIdentidad = DocumentoVenta.TiposDocumentoIdentidad.RUC;
            this.tipoPagoSolicitado = DocumentoVenta.TipoPago.NoAsignado;
        }

        public Guid idCliente { get; set; }

        [Display(Name = "Responsable Comercial:")]
        public Vendedor responsableComercial { get; set; }
        [Display(Name = "Asistente Servicio Cliente:")]
        public Vendedor asistenteServicioCliente { get; set; }
        [Display(Name = "Responsable Portafolio:")]
        public Vendedor responsabelPortafolio { get; set; }
        [Display(Name = "Supervisor Comercial:")]
        public Vendedor supervisorComercial { get; set; }        

    }

}