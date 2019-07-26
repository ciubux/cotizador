using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class LogCampo 
    {
        public int idCampo { get; set; }

        public int idTabla { get; set; }

        public String nombre { get; set; }

        public bool puedePersistir { get; set; }

        /*------------------------------------------------*/

        public int catalogoId { get; set; }
        public int tablaId { get; set; }

        public int estadoTabla { get; set; }

        public string nombreTabla { get; set; }

        public int estadoCatalogo { get; set; }

        public string codigo { get; set; }
        
        public int puede_persistir { get; set; }

        public int id_catalogo_campo { get; set; }

        public String tabla_referencia { get; set; }

        public int orden { get; set; }

        public String campo_referencia { get; set; }

    }
}