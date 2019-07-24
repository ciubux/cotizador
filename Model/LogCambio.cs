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


        /*------------------------------------------------*/

        public int tablaId { get; set; }

        public int estadoTabla { get; set; }

        public string nombreTabla { get; set; }

        public int estadoCatalogo { get; set; }

        public string codigo { get; set; }
        public string nombre { get; set; }
        public int puede_persistir { get; set; }

        public int id_catalogo_campo { get; set; }

        public String tabla_referencia { get; set; }

        public int orden { get; set; }

        public String campo_referencia { get; set; }
    }
}

