using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class ConsolidadoAtencion : Auditoria
    {
        public Guid idConsolidadoAtencion { get; set; }

        [Display(Name = "Fecha:")]
        public DateTime fecha { get; set; }

        [Display(Name = "Observaciones:")]
        public string observaciones { get; set; }

        [Display(Name = "Sede:")]
        public Ciudad ciudad { get; set; }

        [Display(Name = "Vehículo:")]
        public Vehiculo vehiculo { get; set; }

        [Display(Name = "Chofer:")]
        public PersonalAlmacen chofer { get; set; }

        [Display(Name = "Asistente:")]
        public PersonalAlmacen asistente { get; set; }

        public List<Pedido> pedidos { get; set; }
        public List<GuiaRemision> guias { get; set; }

        public List<DocumentoDetalle> resumenDetalleGuias { get; set; }

        public string fechaDesc { 
            get { return fecha.ToString(Constantes.formatoFecha); }
        }

        public bool esEditable
        {
            get { return fecha.Date >= DateTime.Now.Date; }
        }

        public ConsolidadoAtencion()
        {
            vehiculo = new Vehiculo();
            chofer = new PersonalAlmacen();
            asistente = new PersonalAlmacen();
            pedidos = new List<Pedido>();
            guias = new List<GuiaRemision>();
        }
    }
}