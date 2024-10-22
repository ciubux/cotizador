using BusinessLayer;
using Cotizador.Models;
using Cotizador.ExcelExport;
using Model;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class FabricanteController : ParentController
    {
        [HttpGet]
        public ActionResult List()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaMaestroFabricantes)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaFabricantes;
            
            if (this.Session[Constantes.VAR_SESSION_FABRICANTE_BUSQUEDA] == null)
            {
                instanciarFabricanteBusqueda();
            }
            

            Fabricante objSearch = (Fabricante)this.Session[Constantes.VAR_SESSION_FABRICANTE_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaFabricantes;
            ViewBag.fabricante = objSearch;

            return View();
        }

        public String SearchList()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaFabricantes;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            Fabricante obj = (Fabricante)this.Session[Constantes.VAR_SESSION_FABRICANTE_BUSQUEDA];

            FabricanteBL bL = new FabricanteBL();
            List<Fabricante> list = bL.Listar(usuario.idUsuario, obj.Estado);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_FABRICANTE_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);

        }

        
        

        private void instanciarFabricante()
        {
            Fabricante obj = new Fabricante();
            obj.idFabricante = 0;
            obj.Estado = 1;
            obj.codigo = String.Empty;
            obj.nombreUsual = String.Empty;
            
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_FABRICANTE] = obj;
        }

        private void instanciarFabricanteBusqueda()
        {
            Fabricante obj = new Fabricante();
            obj.idFabricante = 0;
            obj.Estado = 1;
            obj.codigo = String.Empty;
            obj.nombreUsual = String.Empty;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_FABRICANTE_BUSQUEDA] = obj;
        }

        /*
        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<Fabricante> list = (List<Fabricante>)this.Session[Constantes.VAR_SESSION_FABRICANTE_LISTA];

            FabricanteSearch excel = new FabricanteSearch();
            return excel.generateExcel(list, (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO]);
        }
        */
        

        // GET: Fabricante
        [HttpGet]
        public ActionResult Index()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaMaestroFabricantes)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();

        }

        public String Create()
        {
            FabricanteBL bL = new FabricanteBL();
            Fabricante obj = new Fabricante();

            obj.codigo = Request["codigo"].ToString();
            obj.nombreUsual = Request["nombreUsual"].ToString();
            obj.IdUsuarioRegistro = Logueado.idUsuario; 

            obj = bL.Insert(obj);
            this.Session[Constantes.VAR_SESSION_FABRICANTE] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public String Update()
        {
            FabricanteBL bL = new FabricanteBL();
            Fabricante obj = new Fabricante();

            obj.idFabricante = int.Parse(Request["idFabricante"].ToString());  
            obj.codigo = Request["codigo"].ToString();
            obj.nombreUsual = Request["nombreUsual"].ToString();
            obj.IdUsuarioRegistro = Logueado.idUsuario;

            if (obj.idFabricante == 0)
            {
                obj = bL.Insert(obj);
                this.Session[Constantes.VAR_SESSION_FABRICANTE] = null;
            }
            else
            {
                obj = bL.Update(obj);
                this.Session[Constantes.VAR_SESSION_FABRICANTE] = null;
            }
            String resultado = JsonConvert.SerializeObject(obj);
            //this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            return resultado;
        }
        

        private Fabricante FabricanteSession
        {
            get
            {
                Fabricante obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaFabricantes: obj = (Fabricante)this.Session[Constantes.VAR_SESSION_FABRICANTE_BUSQUEDA]; break;
                    //case Constantes.paginas.MantenimientoFabricante: obj = (Fabricante)this.Session[Constantes.VAR_SESSION_FABRICANTE]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaFabricantes: this.Session[Constantes.VAR_SESSION_FABRICANTE_BUSQUEDA] = value; break;
                    //case Constantes.paginas.MantenimientoFabricante: this.Session[Constantes.VAR_SESSION_FABRICANTE] = value; break;
                }
            }
        }
    }
}