using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class Unidad : Auditoria
    {
        public Guid idUnidad { get; set; }
        
        public String descripcion { get; set; }


    }
}