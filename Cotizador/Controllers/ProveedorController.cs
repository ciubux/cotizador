using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Model;

namespace Cotizador.Controllers
{
    public class ProveedorController : Controller
    {
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


        public ActionResult list()
        {
            List<Proveedor> proveedorListTmp = (List<Proveedor>)this.Session["proveedorList"];


            List<Proveedor> proveedorList = new List<Proveedor>();
            Proveedor proveedorTodos = new Proveedor { nombre = "Todos" };

            if (this.Session["proveedor"] == null)
            { this.Session["proveedor"] = "Todos"; }

            proveedorList.Add(proveedorTodos);

            foreach (Proveedor proveedor in proveedorListTmp)
            {
                proveedorList.Add(proveedor);
            }
            var model = proveedorList;

            return PartialView("_SelectProveedor", model);
        }

        public ActionResult listBusquedaPrecios()
        {
            List<Proveedor> proveedorListTmp = (List<Proveedor>)this.Session["proveedorList"];


            List<Proveedor> proveedorList = new List<Proveedor>();
            Proveedor proveedorTodos = new Proveedor { nombre = "Todos" };

            if (this.Session["proveedor"] == null)
            { this.Session["proveedor"] = "Todos"; }

            proveedorList.Add(proveedorTodos);

            foreach (Proveedor proveedor in proveedorListTmp)
            {
                proveedorList.Add(proveedor);
            }
            var model = proveedorList;

            return PartialView("_SelectProveedorBusquedaPrecios", model);
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
