using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    public class Usuario
    {
        public int idUsuario { get; set; }//] [int] NOT NULL,

        [Display(Name = "Login:")]
        public string login { get; set; }//] [varchar](50) NULL,

        [Display(Name = "Apellido Paterno:")]
        [DataType(DataType.Text)]
        public string apellidoPaterno { get; set; }//] [varchar](100) NULL,

        [Display(Name = "Apellido Materno:")]
        [DataType(DataType.Text)]
        public string apellidoMaterno { get; set; }//] [varchar](100) NULL,

        [Display(Name = "Nombres:")]
        [DataType(DataType.Text)]
        public string nombres { get; set; }//] [varchar](100) NULL,

        [Display(Name = "Iniciales:")]
        [DataType(DataType.Text)]
        public string iniciales { get; set; }//] [varchar](100) NULL,
    }
}

    