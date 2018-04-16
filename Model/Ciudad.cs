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

        public bool esProvincia { get; set; }  

        public List<Transportista> transportistaList { get; set; }
    }
}