﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class CampoPersistir
    {
        public LogCampo campo { get; set; }
        public string nombre { get; set; }
        
        public bool persiste { get; set; }
        public bool registra { get; set; }
    }
    
}