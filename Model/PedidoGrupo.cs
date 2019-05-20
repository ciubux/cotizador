using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class PedidoGrupo : Auditoria
    {
        public int numero { get; set; }
        public DateTime fechaSolicitud { get; set; }

        public String numeroToString
        {
             get { return this.numero.ToString().PadLeft(Constantes.LONGITUD_NUMERO_GRUPO, Constantes.PAD); }
        }

        public String fechaSolicitudToString
        {
            get { return this.fechaSolicitud.ToString(Constantes.formatoFecha); }
        }
        
    }

}
