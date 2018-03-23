﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Model;

namespace Cotizador.Controllers
{
    public class FamiliaController : Controller
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
            List<Familia> familiaListTmp = (List<Familia>)this.Session["familiaList"];


            List<Familia> familiaList = new List<Familia>();
            Familia familiaDeshabilitada = new Familia { nombre = "Todas" };
            familiaList.Add(familiaDeshabilitada);

            foreach (Familia familia in familiaListTmp)
            {
                familiaList.Add(familia);
            }

            /*     List<Guid> ciudadDeshabilitadas = new List<Guid>();
                 ciudadDeshabilitadas.Add(ciudadDeshabiltiad.idCiudad);
                 ViewBag.List = ciudadDeshabilitadas;*/
            var model = familiaList;

            return PartialView("_SelectFamilia", model);
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