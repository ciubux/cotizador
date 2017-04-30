using cotizadorPDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class PDFController : Controller
    {
        // GET: PDF
        public ActionResult Index()
        {

            GeneradorPDF generadorPDF = new GeneradorPDF();
            generadorPDF.generar();
            return View();
        }
    }
}