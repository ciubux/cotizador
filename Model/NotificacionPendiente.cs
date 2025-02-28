using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class NotificacionPendiente : Auditoria
    {
        public const String MENSAJE_ZAS_BASE = "MENSAJE_ZAS_BASE";
        public Guid idNotificacionPendiente { get; set; }

        public String codigo { get; set; }
        public String destinatarios { get; set; }
        public String idRegistro { get; set; }

        public String datosCorto { get; set; }

        public String datosLargo { get; set; }

    }
}