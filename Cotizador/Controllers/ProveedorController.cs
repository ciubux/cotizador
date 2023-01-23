using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Model;
using Newtonsoft.Json;

namespace Cotizador.Controllers
{
    public class ProveedorController : Controller
    {
        private Proveedor ProveedorSession
        {
            get
            {
                return (Proveedor)this.Session[Constantes.VAR_SESSION_PROVEEDOR];
            }
            set
            {
                this.Session[Constantes.VAR_SESSION_PROVEEDOR] = value;
            }
        }
        // GET: Ciudad
        public ActionResult Index()
        {
            return View();
        }



        public String SearchProveedores()
        {
            String data = this.Request.Params["data[q]"];
            ProveedorBL proveedorBL = new ProveedorBL();
            Proveedor proveedor = (Proveedor)this.Session[Constantes.VAR_SESSION_PROVEEDOR];
            return proveedorBL.getProveedoresBusqueda(data, proveedor.ciudad.idCiudad);
        }


        public String GetProveedor()
        {
            Proveedor proveedor = (Proveedor)this.Session[Constantes.VAR_SESSION_PROVEEDOR];
            Guid idProveedor = Guid.Parse(Request["idCliente"].ToString());
            ProveedorBL proveedorBL = new ProveedorBL();
            Ciudad ciudad = proveedor.ciudad;
            proveedor = proveedorBL.getProveedor(idProveedor);
            proveedor.ciudad = ciudad;
            String resultado = JsonConvert.SerializeObject(proveedor);
            this.ProveedorSession = proveedor;
            return resultado;
        }

        // GET: Ciudad/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        public void ChangeProveedor()
        {
            this.Session["proveedor"] = this.Request.Params["proveedor"];
        }



        public ActionResult list()
        {
            List<Proveedor> proveedorListTmp = (List<Proveedor>)this.Session["proveedorList"];


            List<Proveedor> proveedorList = new List<Proveedor>();
            Proveedor proveedorTodos = new Proveedor { codigo = "Todos", nombre = "Todos" };

            if (this.Session["proveedor"] == null)
            { this.Session["proveedor"] = "Todos"; }

            proveedorList.Add(proveedorTodos);

            foreach (Proveedor proveedor in proveedorListTmp)
            {
                proveedorList.Add(proveedor);
            }
            var model = proveedorList;
            ViewBag.selectProveedor = this.Session["proveedor"];

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
