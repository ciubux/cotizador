using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Model;

namespace Cotizador.Controllers
{
    public class CiudadController : Controller
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
       //     ViewBag.numero = Int32.Parse(Request["numero"].ToString());


            CiudadBL ciudadBL = new CiudadBL();

            List<Ciudad> ciudadListTmp = ciudadBL.getCiudades();

            List<Ciudad> ciudadList = new List<Ciudad>();
            Ciudad ciudadDeshabiltiad = new Ciudad { idCiudad = Guid.Empty, nombre = "Seleccione Ciudad", orden = 0 };
            ciudadList.Add(ciudadDeshabiltiad);

            foreach (Ciudad ciudad in ciudadListTmp)
            {
                ciudadList.Add(ciudad);
            }

            List<Guid> ciudadDeshabilitadas = new List<Guid>();
            ciudadDeshabilitadas.Add(ciudadDeshabiltiad.idCiudad);
            ViewBag.List = ciudadDeshabilitadas;
            var model = ciudadList;

                /*new DepartamentoViewModels
            {
                Data = _ubigeoBl.GetDepartamentos(),
                DepartamentoSelectId = departamentoSelectId,
                ProvinciaSelectId = provinciaSelectId,
                DistritoSelectId = distritoSelectId,
                ProvinciaDivId = provinciaDivId,
                DistritoDivId = distritoDivId,
                UrlProvinciasPorDepartamento = Url.Action("GetProvinciasPorDepartamento"),
                UrlDistritosPorProvincia = Url.Action("GetDistritosPorProvincia"),
                SelectedValue = selectedValue
            };
            */
            return PartialView("_SelectCiudad", model);
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
