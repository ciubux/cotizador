using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class LogCambio
    {
        public Guid idCambio { get; set; }
        public int idTabla { get; set; }
        public int idCampo { get; set; }
        public String idRegistro { get; set; }
        public String valor { get; set; }
        public bool persisteCambio { get; set; }
        public bool estado { get; set; }

        public LogCampo campo { get; set; }
        public LogTabla tabla { get; set; }
        public DateTime fechaInicioVigencia { get; set; }
        public Guid idUsuarioModificacion { get; set; }

        public bool repiteDato { get; set; }

        public int catalogoId { get; set; }


        public DateTime FechaEdicion { get; set; }

        public string FechaEdicionDesc
        {
            get { return FechaEdicion.ToString("dd/MM/yyyy HH:mm:ss"); }
        }
    }
}

