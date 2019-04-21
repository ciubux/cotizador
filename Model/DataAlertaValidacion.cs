using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Model
{
    public class DataAlertaValidacion 
    {
        public string tipo { get; set; }
        public string ObjData { get; set; }
        public string PrevData { get; set; }
        public string PostData { get; set; }
    }
}