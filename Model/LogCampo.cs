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
    }
}