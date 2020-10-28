﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Model;
using Newtonsoft.Json;
using Cotizador.Models;
using System.Reflection;
using System.IO;



namespace Cotizador.Controllers
{
    public class ArchivoAdjuntoController : Controller
    {
        // GET: ArchivoAdjunto
        public ActionResult index()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaArchivoAdjunto;

            if (this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_BUSQUEDA] == null)
            {
                instanciarArchivoAdjuntoBusqueda();
            }

            ArchivoAdjunto objSearch = (ArchivoAdjunto)this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaArchivoAdjunto;
            ViewBag.archivoAdjunto = objSearch;

            return View();
        }
        private void instanciarArchivoAdjuntoBusqueda()
        {
            ArchivoAdjunto obj = new ArchivoAdjunto();
            obj.nombre = String.Empty;
            obj.origenBusqueda = ArchivoAdjunto.origenesBusqueda.opcion;
            this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_BUSQUEDA] = obj;
        }

        public String SearchList()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaArchivoAdjunto;
            ArchivoAdjunto archivoGrupo = (ArchivoAdjunto)this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_BUSQUEDA];
            ArchivoAdjuntoBL bl = new ArchivoAdjuntoBL();
            List<ArchivoAdjunto> list = bl.getListArchivoAdjunto(archivoGrupo);
            return JsonConvert.SerializeObject(list);
            //return pedidoList.Count();
        }


        public String DescargarArchivo()
        {
            Guid idArchivo = Guid.Parse(Request["idArchivo"].ToString());
            ArchivoAdjunto obj = new ArchivoAdjunto();
            obj.idArchivoAdjunto = idArchivo;
            ArchivoAdjunto archivoAdjunto = new ArchivoAdjunto();
            ArchivoAdjuntoBL archivoAdjuntoBL = new ArchivoAdjuntoBL();
            archivoAdjunto = archivoAdjuntoBL.GetArchivoAdjunto(obj);
            return JsonConvert.SerializeObject(archivoAdjunto);
        }

        public void ChangeInputOrigen()
        {
            ArchivoAdjunto objSearch = (ArchivoAdjunto)this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_BUSQUEDA];
            Char origen = Convert.ToChar(Int32.Parse(this.Request.Params["origenBusqueda"]));
            objSearch.origenBusqueda = (ArchivoAdjunto.origenesBusqueda)origen;
            this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_BUSQUEDA] = objSearch;
        }

        public void ChangeInputString()
        {
            ArchivoAdjunto obj = (ArchivoAdjunto)this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_BUSQUEDA];
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_BUSQUEDA] = obj;
        }

        public void Limpiar()
        {
            instanciarArchivoAdjuntoBusqueda();
        }


        /*****************************************************************************/

        public PartialViewResult cargarArchivo(string origen, String idRegistro)
        {
            Usuario user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            ArchivoAdjunto obj = new ArchivoAdjunto();
            obj.usuario = new Usuario();
            obj.usuario = user;
            obj.origen = origen;
            obj.idRegistro = idRegistro;
            ArchivoAdjuntoBL arcBL = new ArchivoAdjuntoBL();
            List<ArchivoAdjunto> ArchivoAdjuntoList = arcBL.getListArchivoAdjuntoByIdRegistro(idRegistro);

            List<ArchivoAdjunto> listaArchivos = new List<ArchivoAdjunto>();
            if (this.Session["ARCHIVO_ADJUNTO_EDIT_" + origen] != null)
            {
                listaArchivos = (List<ArchivoAdjunto>)this.Session["ARCHIVO_ADJUNTO_EDIT_" + origen];
            }
            
            bool agregar = false;
            foreach (ArchivoAdjunto adj in ArchivoAdjuntoList)
            {
                agregar = true;
                foreach (ArchivoAdjunto item in listaArchivos)
                {
                    if (adj.idArchivoAdjunto == item.idArchivoAdjunto)
                    {
                        agregar = false;
                    }
                }

                if (agregar)
                {
                    listaArchivos.Add(adj);
                }
            }

            var model = listaArchivos;
            this.Session["ARCHIVO_ADJUNTO_EDIT_" + origen] = listaArchivos;
            this.Session["ARCHIVO_ADJUNTO_" + origen] = obj;
            this.Session["ARCHIVO_ADJUNTO_CAMBIADOS_" + origen] = new List<ArchivoAdjunto>();
            ViewBag.origen = origen;            
            return PartialView("_LoadFiles", model);
        }

        public PartialViewResult verArchivos(string origen, String idRegistro)
        {
            Usuario user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            ArchivoAdjunto obj = new ArchivoAdjunto();
            obj.usuario = new Usuario();
            obj.usuario = user;
            obj.origen = origen;
            obj.idRegistro = idRegistro;
            ArchivoAdjuntoBL arcBL = new ArchivoAdjuntoBL();
            List<ArchivoAdjunto> listaArchivos = arcBL.getListArchivoAdjuntoByIdRegistro(idRegistro);

            var model = listaArchivos;

            ViewBag.origen = origen;
            return PartialView("_ViewFiles", model);
        }


        [HttpPost]
        public String ChangeFiles(HttpPostedFileBase file, String origen)
        {           
            ArchivoAdjunto arAd = (ArchivoAdjunto)this.Session["ARCHIVO_ADJUNTO_" + origen];
            List<ArchivoAdjunto> objs = (List<ArchivoAdjunto>)this.Session["ARCHIVO_ADJUNTO_EDIT_" + origen];
            List<ArchivoAdjunto> listCambioArchivo = (List<ArchivoAdjunto>)this.Session["ARCHIVO_ADJUNTO_CAMBIADOS_" + origen];
            ArchivoAdjuntoBL arcBL = new ArchivoAdjuntoBL();
            
            ArchivoAdjunto obj = new ArchivoAdjunto();
            if (file != null && file.ContentLength > 0)
            {
                using (Stream inputStream = file.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    obj.usuario = arAd.usuario;
                    obj.origen = arAd.origen;
                    obj.idRegistro = arAd.idRegistro;
                    obj.estado = 1;
                    obj.nombre = file.FileName;
                    obj.adjunto = memoryStream.ToArray();
                    obj = arcBL.InsertArchivoGenerico(obj);
                }
                objs.Add(obj);
                listCambioArchivo.Add(obj);
            }            
            this.Session["ARCHIVO_ADJUNTO_EDIT_" + origen] = objs;
            this.Session["ARCHIVO_ADJUNTO_CAMBIADOS_" + origen] = listCambioArchivo;
            return obj.idArchivoAdjunto.ToString();
        }

        public String DescartarArchivos(String origen)
        {
            ArchivoAdjunto arAd = (ArchivoAdjunto)this.Session["ARCHIVO_ADJUNTO_" + origen];
            List<ArchivoAdjunto> arcAdj = (List<ArchivoAdjunto>)this.Session["ARCHIVO_ADJUNTO_EDIT_" + origen];
            List<ArchivoAdjunto> listCambioArchivo = (List<ArchivoAdjunto>)this.Session["ARCHIVO_ADJUNTO_CAMBIADOS_" + origen];
            Guid idArcAdj = Guid.Parse(Request["idArchivo"].ToString());           
            List<ArchivoAdjunto> listArchivos = new List<ArchivoAdjunto>(arcAdj);
            foreach (ArchivoAdjunto archivoAdjunto in arcAdj)
            {                
                if (archivoAdjunto.idArchivoAdjunto.Equals(idArcAdj))
                {
                    archivoAdjunto.origen = arAd.origen;
                    archivoAdjunto.usuario = new Usuario();
                    archivoAdjunto.usuario = arAd.usuario;
                    archivoAdjunto.idRegistro = arAd.idRegistro;
                    archivoAdjunto.estado = 0;
                    ArchivoAdjuntoBL arcBL = new ArchivoAdjuntoBL();
                    arcBL.InsertArchivoGenerico(archivoAdjunto);
                    listArchivos.Remove(archivoAdjunto);
                    listCambioArchivo.Add(archivoAdjunto);
                    this.Session["ARCHIVO_ADJUNTO_CAMBIADOS_" + origen] = listCambioArchivo;
                }               
            }            
            this.Session["ARCHIVO_ADJUNTO_EDIT_" + origen] = listArchivos;           
            return JsonConvert.SerializeObject(listArchivos);
        }        
    }
}