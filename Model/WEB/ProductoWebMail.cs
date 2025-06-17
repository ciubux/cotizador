using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model.WEB
{
    public class ProductoWebMail
    {
        public string name { get; set; }
        public string sku { get; set; }
        public int quantity { get; set; }
        public string brand { get; set; }
    }
}