using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class Ciudad : Auditoria
    {
        public Guid idCiudad { get; set; }

        public String nombre { get; set; }

        public int orden { get; set; }

        [Display(Name = "Punto Partida:")]
        public String direccionPuntoPartida { get; set; }

        [Display(Name = "Punto Llegada:")]
        public String direccionPuntoLlegada { get; set; }

        public bool esProvincia { get; set; }  

        public List<Transportista> transportistaList { get; set; }

        public String serieGuiaRemision { get; set; }

        public int siguienteNumeroGuiaRemision { get; set; }

        public String serieNotaIngreso { get; set; }

        public int siguienteNumeroNotaIngreso { get; set; }
        public String sede { get; set; }

        public List<SerieDocumentoElectronico> serieDocumentoElectronicoList { get; set; }

        public String correoCoordinador { get; set; }

        /* ADICIONALES */
        public Guid idClienteRelacionado { get; set; }
    }
}