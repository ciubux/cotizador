using Model.CONFIGCLASSES;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class ClienteSunast : Auditoria
    {
        public const string NOMBRE_TABLA = "CLIENTE_SUNAT";

        public ClienteSunast()
        {
            //Cada vez que se instancia un cliente se instancia con al menos una dirección de entrega
            this.direccionEntregaList = new List<DireccionEntrega>();
            this.clientes = new List<Cliente>();

         
        }

        public Guid idCliente { get; set; }

        public List<Cliente> clientes { get; set; }
        public List<DireccionEntrega> direccionEntregaList  { get; set; }

        [Display(Name = "Razón Social / Nombre:")]
        public String textoBusqueda { get; set; }

        public String nombreComercial { get; set; }
        public String razonSocial { get; set; }
        public String ruc { get; set; }

        public ClienteConfiguracion configuraciones { get; set; }


    }

}
