using BusinessLayer;
using Cotizador.Models;
using Cotizador.ExcelExport;
using Model;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class MonedaController : ParentController
    {
        
        public ActionResult SelectMonedas(string selectId, string selectedValue = null, string disabled = null)
        {

            var model = new MonedaViewModels
            {
                Data = Moneda.ListaMonedas,
                SelectId = selectId,
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_Moneda", model);
        }
    }
}