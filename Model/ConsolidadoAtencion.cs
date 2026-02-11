using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class ConsolidadoAtencion : Auditoria
    {
        public Guid idConsolidadoAtencion { get; set; }
        public DateTime fecha { get; set; }
        public string observaciones { get; set; }
        public Vehiculo vehiculo { get; set; }
        public PersonalAlmacen chofer { get; set; }
        public PersonalAlmacen asistente { get; set; }

        public ConsolidadoAtencion()
        {
            vehiculo = new Vehiculo();
            chofer = new PersonalAlmacen();
            asistente = new PersonalAlmacen();
        }
    }
}