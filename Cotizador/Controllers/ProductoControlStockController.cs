using BusinessLayer;
using Cotizador.Models;
using Model;
using Model.UTILES;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Cotizador.ExcelExport;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Cotizador.Controllers
{
    public class ProductoControlStockController : ParentController
    {
        [HttpGet]
        public ActionResult ControlAlertaStock()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ControlStock;

            if (this.Logueado == null || !this.Logueado.visualizaControlStock)
            {
                return RedirectToAction("Login", "Account");
            }

            List<ProductoControlStock> lista = new List<ProductoControlStock>();

            ProductoControlStockBL bl = new ProductoControlStockBL();
            lista = bl.SelectProductosControlStock(Logueado.idUsuario, 1, Guid.Empty);

            ViewBag.pagina = (int)Constantes.paginas.ControlStock;
            ViewBag.lista = lista;

            return View();
        }

        [HttpPost]
        public string CalcularStock()
        {
            int success = 0;

            ProductoControlStockBL bl = new ProductoControlStockBL();
            //bl.UpdateAjusteEstadoAprobado(obj);

            return JsonConvert.SerializeObject(new
            {
                success = success
            });
        }
    }
}