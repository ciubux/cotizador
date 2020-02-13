using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class ClienteReasignacionHistorico : Auditoria
    {
        public Guid idClienteReasignacionHistorico { get; set; }
        public Guid idCliente { get; set; }
        public String campo { get; set; }
        public String valor { get; set; }
        public String observacion { get; set; }

        public String preValor { get; set; }

        public String dataA { get; set; }
        public String dataB { get; set; }
        public String dataC { get; set; }
        public String dataD { get; set; }
        public String dataE { get; set; }

        public void defaultValues(String campo)
        {
            this.campo = campo;
            this.observacion = "";
            this.fechaInicioVigencia = DateTime.Now;
        }
    }
}

