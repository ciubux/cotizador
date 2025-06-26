using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model.WEB
{
    public class CotizacionWebMail
    {
        public string codigo { get; set; }
        public string full_name { get; set; }
        public string city { get; set; }
        public string ubigeo { get; set; }
        public string email { get; set; }
        public string phone { get; set; }

        public string message { get; set; }
        public string doc_type { get; set; }
        public string business_name { get; set; }
        public string ruc_number { get; set; }

        public string dni_number { get; set; }
        
        public DateTime apply_date { get; set; }

        public List<ProductoWebMail> products { get; set; }
    }
}