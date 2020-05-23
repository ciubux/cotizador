using System;
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

        public ActionResult cargarArchivo(string origen, Guid idRegistro)
        {
            Usuario user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            ArchivoAdjunto obj = new ArchivoAdjunto();
            obj.usuario = new Usuario();
            obj.usuario = user;
            obj.origen = origen;
            obj.idRegistro = idRegistro;
            ArchivoAdjuntoBL arcBL = new ArchivoAdjuntoBL();
            List<ArchivoAdjunto> ArchivoAdjuntoList = arcBL.getListArchivoAdjuntoByIdRegistro(obj.idRegistro);
            
            var model = ArchivoAdjuntoList;
            this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_EDIT] = ArchivoAdjuntoList;
            this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO] = obj;
            return PartialView("_LoadFiles", model);
        }


        [HttpPost]
        public String ChangeFiles(HttpPostedFileBase file)
        {            
            List<ArchivoAdjunto> objs = (List<ArchivoAdjunto>)this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_EDIT];           
            ArchivoAdjuntoBL arcBL = new ArchivoAdjuntoBL();
            ArchivoAdjunto arAd = (ArchivoAdjunto)this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO];
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
                    obj.usuario= arAd.usuario;
                    obj.origen = arAd.origen;
                    obj.idRegistro = arAd.idRegistro;
                    obj.estado = 1;
                    obj.nombre = file.FileName;
                    obj.adjunto = memoryStream.ToArray();
                    objs.Add(obj);
                    }
                    arcBL.InsertArchivoGenerico(obj);                    
                }      
            this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_EDIT] = objs;
            return obj.idArchivoAdjunto.ToString();
        }

        public String DescartarArchivos()
        {
            List<ArchivoAdjunto> arcAdj = (List<ArchivoAdjunto>)this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_EDIT];
            ArchivoAdjunto obj = (ArchivoAdjunto)this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO];
            Guid idArcAdj = Guid.Parse(Request["idArchivo"].ToString());
            ArchivoAdjunto arAd = new ArchivoAdjunto();
            arAd = (ArchivoAdjunto)this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO];
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
                }               
            }
            this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_EDIT] = listArchivos;           
            return JsonConvert.SerializeObject(listArchivos);
        }       

}
}