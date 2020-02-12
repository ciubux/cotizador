using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Venta : Transaccion
    {
        public int idAsistente { get; set; }

        public int idOrigen { get; set; }

        public int perteneceCanalLima { get; set; }

        public int perteneceCanalMultiregional { get; set; }

        public int perteneceCanalPcp { get; set; }

        public int perteneceCanalProvincia { get; set; }

        public int idResponsableComercial { get; set; }

        public int idSupervisorComercial { get; set; }

    } 
}
