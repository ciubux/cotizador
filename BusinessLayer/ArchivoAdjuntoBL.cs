﻿
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;
using System.Linq;
using ServiceLayer;
using BusinessLayer.Email;

namespace BusinessLayer
{
    public class ArchivoAdjuntoBL
    {
        
        public ArchivoAdjunto GetArchivoAdjunto(ArchivoAdjunto archivoAdjunto)
        {
            using (var dal = new ArchivoDAL())
            {
                archivoAdjunto = dal.SelectArchivoAdjunto(archivoAdjunto);
            }
            return archivoAdjunto;
        }

    }
}
