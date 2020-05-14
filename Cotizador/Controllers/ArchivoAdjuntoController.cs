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

        [HttpPost]
        public void ChangeFiles(List<HttpPostedFileBase> files)
        {            
            List<ArchivoAdjunto> obj = new List<ArchivoAdjunto>();
            obj = (List<ArchivoAdjunto>)this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_EDIT];

            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (obj.Where(p => p.nombre.Equals(file.FileName)).FirstOrDefault() != null)
                    {
                        continue;
                    }


                    ArchivoAdjunto arAd = new ArchivoAdjunto();
                    using (Stream inputStream = file.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        arAd.nombre = file.FileName;
                        arAd.adjunto = memoryStream.ToArray();
                    }
                    obj.Add(arAd);
                        FilesGuardarSession(obj);
                }
            }

        }
        [HttpPost]
        public void FilesGuardarSession(List<ArchivoAdjunto> files)
        {       
            if (files.Count == 0)
            {
                this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_EDIT] = null;
            }
            else
            {
                switch (files[0].origen)
                {
                    case "Pedido":
                        Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];
                        pedido.listArchivoAjunto = files;                        
                        this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_EDIT] = files;
                        break;
                    case "Factura":

                    case "Producto":

                    case "GuiaRemision":

                        break;
                }
            }
            }
        
        
        public String DescartarArchivos()
        {            
            List<ArchivoAdjunto> arcAdj = (List<ArchivoAdjunto>)this.Session[Constantes.VAR_SESSION_ARCHIVO_ADJUNTO_EDIT];
           
            String nombreArchivo = Request["nombreArchivo"].ToString();
            Usuario user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            List<ArchivoAdjunto> pedidoAdjuntoList = new List<ArchivoAdjunto>();
            foreach (ArchivoAdjunto pedidoAdjunto in arcAdj)
            {
                if (pedidoAdjunto.nombre==nombreArchivo)
                {
                    pedidoAdjunto.estado = 0;
                    pedidoAdjuntoList.Add(pedidoAdjunto);
                }
                if (!pedidoAdjunto.nombre.Equals(nombreArchivo))
                {
                    pedidoAdjunto.estado = 1;
                    pedidoAdjuntoList.Add(pedidoAdjunto);
                }                
            }
            

            arcAdj = pedidoAdjuntoList;
            FilesGuardarSession(arcAdj);
            return JsonConvert.SerializeObject(arcAdj);
        }
        


       
    }
}