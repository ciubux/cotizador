using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Cliente : Auditoria
    {
        public Cliente()
        {
            //Cada vez que se instancia un cliente se instancia con al menos una dirección de entrega
            this.direccionEntregaList = new List<DireccionEntrega>();
           /* DireccionEntrega seleccioneDireccionEntrega = new DireccionEntrega();
            seleccioneDireccionEntrega.descripcion = Constantes.LABEL_DIRECCION_ENTREGA_VACIO;
            seleccioneDireccionEntrega.idDireccionEntrega = Guid.Empty;*/
          //  this.direccionEntregaList.Add(seleccioneDireccionEntrega);
        }


        public Guid idCliente { get; set; }

        public String codigo { get; set;  }

        public int codigoAlterno { get; set; }

        public String razonSocial { get; set; }

        public String nombreComercial { get; set; }

        public String ruc { get; set; }

        public String contacto1 { get; set; }

        public String contacto2 { get; set; }

        public Ciudad ciudad { get; set; }

        public String domicilioLegal { get; set; }

        public List<DireccionEntrega> direccionEntregaList { get; set; }

        public override string ToString()
        {
            return "R. Social: " + this.razonSocial + "  -  N. Comercial: "
                + this.nombreComercial + " - Cod: " + this.codigo;// + " **** RUC: " + this.ruc;
        }
    }
}