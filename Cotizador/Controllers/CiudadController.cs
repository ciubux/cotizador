using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Cotizador.Models;
using Model;

namespace Cotizador.Controllers
{
    public class CiudadController : Controller
    {

        CiudadBL ciudadBL = new CiudadBL();
        // GET: Ciudad
        public ActionResult Index()
        {
            return View();
        }

        // GET: Ciudad/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }


        public ActionResult GetCiudades(string ciudadSelectId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            List<Ciudad> ciudades = usuario.sedesMP;
            bool verSeleccione = true;

            if ((int)(Constantes.paginas.BusquedaCotizaciones) == (int)this.Session[Constantes.VAR_SESSION_PAGINA] ||
                (int)(Constantes.paginas.MantenimientoCotizacion) == (int)this.Session[Constantes.VAR_SESSION_PAGINA]
                )
            {
                ciudades = usuario.sedesMPCotizaciones;
            }
            else if ((int)(Constantes.paginas.BusquedaPedidos) == (int)this.Session[Constantes.VAR_SESSION_PAGINA] ||
                (int)(Constantes.paginas.MantenimientoPedido) == (int)this.Session[Constantes.VAR_SESSION_PAGINA]
                )
            {
                ciudades = usuario.sedesMPPedidos;
            }
            else if ((int)(Constantes.paginas.BusquedaGuiasRemision) == (int)this.Session[Constantes.VAR_SESSION_PAGINA] ||
                (int)(Constantes.paginas.MantenimientoGuiaRemision) == (int)this.Session[Constantes.VAR_SESSION_PAGINA]
                )
            {
                ciudades = usuario.sedesMPGuiasRemision;
            }
            else if ((int)(Constantes.paginas.BusquedaFacturas) == (int)this.Session[Constantes.VAR_SESSION_PAGINA] ||
                (int)(Constantes.paginas.MantenimientoFactura) == (int)this.Session[Constantes.VAR_SESSION_PAGINA] ||
                (int)(Constantes.paginas.BusquedaBoletas) == (int)this.Session[Constantes.VAR_SESSION_PAGINA] ||
                (int)(Constantes.paginas.MantenimientoBoleta) == (int)this.Session[Constantes.VAR_SESSION_PAGINA] ||
                (int)(Constantes.paginas.BusquedaFacturas) == (int)this.Session[Constantes.VAR_SESSION_PAGINA] ||
                (int)(Constantes.paginas.MantenimientoFactura) == (int)this.Session[Constantes.VAR_SESSION_PAGINA]||
                (int)(Constantes.paginas.BusquedaFacturas) == (int)this.Session[Constantes.VAR_SESSION_PAGINA] ||
                (int)(Constantes.paginas.MantenimientoFactura) == (int)this.Session[Constantes.VAR_SESSION_PAGINA]
                )
            {
                ciudades = usuario.sedesMPDocumentosVenta;
            } else if ((int)(Constantes.paginas.MantenimientoOrdenCompraCliente) == (int)this.Session[Constantes.VAR_SESSION_PAGINA])
            {
                verSeleccione = false;
                ciudades = usuario.sedesMPPedidos;
            }

           /* if (ciudades.Count == 1)
            {
                selectedValue = ciudades[0].idCiudad.ToString();
            }*/

            var model = new CiudadViewModels
            {
                Data = ciudades,
                CiudadSelectId = ciudadSelectId,
                incluirSeleccione = verSeleccione, 
                SelectedValue = selectedValue
            };

            return PartialView("_Ciudad", model);
        }

        public ActionResult GetCiudadesASolicitar(string ciudadSelectId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            List<Ciudad> ciudades = usuario.sedesMP;
            var model = new CiudadViewModels
            {
                Data = ciudades,
                CiudadSelectId = ciudadSelectId,
                incluirSeleccione = true,
                SelectedValue = selectedValue
            };

            return PartialView("_Ciudad", model);
        }


            // GET: Ciudad/Create
            public ActionResult Create()
        {
            return View();
        }

        // POST: Ciudad/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Ciudad/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Ciudad/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Ciudad/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Ciudad/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
