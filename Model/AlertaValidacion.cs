using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class AlertaValidacion
    {
        public Guid IdAlertaValidacion { get; set; }

        public string nombreTabla { get; set; }

        public string tipo { get; set; }
        public string idRegistro { get; set; }

        [Display(Name = "Estado:")]
        public int Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
        public Guid IdUsuarioCreacion { get; set; }
        public DateTime FechaValidacion { get; set; }
        public Guid IdUsuarioValidacion { get; set; }
        public Usuario UsuarioCreacion { get; set; }
        public Usuario usuario { get; set; }

        public DataAlertaValidacion data { get; set; }


        public string EstadoDesc => Estado == 1 ? "Validado" : "No Validado";

        public bool EstadoCheck
        {
            get { return Estado == 1; }
            set
            {
                Estado = value ? 1 : 0;
            }
        }

        public string FechaCreacionDesc
        {
            get { return FechaCreacion.ToString("dd/MM/yyyy HH:mm:ss"); }
        }

        public string FechaCreacionFormatoFecha
        {
            get { return FechaCreacion.ToString(Constantes.formatoFecha); }
        }

        public string FechaValidacionDesc
        {
            get { return FechaValidacion.ToString("dd/MM/yyyy HH:mm:ss"); }
        }

        public string FechaValidacionFormatoFecha
        {
            get { return FechaValidacion.ToString(Constantes.formatoFecha); }
        }

        public string CambioDesc
        {
            get {
                if (this.tipo == AlertaValidacion.CAMBIA_GRUPO_CLIENTE)
                {
                    return "Se cambio el Grupo del cliente \"" + this.data.ObjData + "\" de \"" + this.data.PrevData + "\" por \"" + this.data.PostData + "\" ";
                }

                if (this.tipo == AlertaValidacion.CREA_GRUPO_CLIENTE)
                {
                    return "Se creó el Grupo Cliente \"" + this.data.ObjData + "\" ";
                }
                return "";
            }
            
        }


        public static String CAMBIA_GRUPO_CLIENTE = "CAMBIA_GRUPO_CLIENTE";
        public static String CREA_GRUPO_CLIENTE = "CREA_GRUPO_CLIENTE";
    }
}
