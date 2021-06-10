using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class CierreStock : Auditoria
    {
        public Guid idCierreStock { get; set; }

        public Ciudad ciudad { get; set; }

        public DateTime fecha { get; set; }
        public ArchivoAdjunto archivo{ get; set; }

    }
}