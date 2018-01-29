using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Log
    {
        public Log(String descripcion, TipoLog tipo, Usuario usuario)
        {
            this.descripcion = descripcion;
            this.tipo = tipo;
            this.usuario = usuario;
        }

        public String descripcion { get; set; }
        public TipoLog tipo { get; set; }
        public Usuario usuario { get; set; }
    }

    public enum TipoLog
    {
        Informativo = 0,
        Advertencia = 1,
        Error = 2
    }


}
