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
    public class SubDistribuidorController : ParentController
    {
        public ActionResult GetSubDistribuidores(string subDistribuidorSelectId, string selectedValue = null, string disabled = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            SubDistribuidor obj = new SubDistribuidor();
            obj.Estado = 1;

            SubDistribuidorBL bL = new SubDistribuidorBL();
            List<SubDistribuidor> list = bL.getSubDistribuidores(obj);

            var model = new SubDistribuidorViewModels
            {
                Data = list,
                SubDistribuidorSelectId = subDistribuidorSelectId,
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_SubDistribuidor", model);
        }


        [HttpGet]
        public ActionResult List()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaSubDistribuidores;
            
            if (this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR_BUSQUEDA] == null)
            {
                instanciarSubDistribuidorBusqueda();
            }
            

            SubDistribuidor objSearch = (SubDistribuidor)this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaSubDistribuidores;
            ViewBag.subDistribuidor = objSearch;

            return View();
        }

        public String SearchList()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaSubDistribuidores;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            SubDistribuidor obj = (SubDistribuidor)this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR_BUSQUEDA];

            SubDistribuidorBL bL = new SubDistribuidorBL();
            List<SubDistribuidor> list = bL.getSubDistribuidores(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);

        }

        public String Show()
        {
            int idSubDistribuidor = int.Parse(Request["idSubDistribuidor"].ToString());
            SubDistribuidorBL bL = new SubDistribuidorBL();
            SubDistribuidor obj = bL.getSubDistribuidorById(idSubDistribuidor);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado =  JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR_VER] = obj;
            return resultado;
        }

        public ActionResult Editar(int? idSubDistribuidor = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.CUSubDistribuidor;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaSubDistribuidor)
            {
                return RedirectToAction("Login", "Account");
            }



            if (this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR] == null && idSubDistribuidor == null)
            {
                instanciarSubDistribuidor();
            }

            SubDistribuidor obj = (SubDistribuidor)this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR];
            
            if (idSubDistribuidor != null)
            {
                SubDistribuidorBL bL = new SubDistribuidorBL();
                obj = bL.getSubDistribuidorById(idSubDistribuidor.Value);
                obj.IdUsuarioRegistro = usuario.idUsuario;
                obj.usuario = usuario;
                
                this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR] = obj;
            }
            

            ViewBag.subDistribuidor = obj;
            return View();

        }

        private void instanciarSubDistribuidor()
        {
            SubDistribuidor obj = new SubDistribuidor();
            obj.idSubDistribuidor = 0;
            obj.Estado = 1;
            obj.codigo = String.Empty;
            obj.nombre = String.Empty;
            
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR] = obj;
        }

        private void instanciarSubDistribuidorBusqueda()
        {
            SubDistribuidor obj = new SubDistribuidor();
            obj.idSubDistribuidor = 0;
            obj.Estado = 1;
            obj.codigo = String.Empty;
            obj.nombre = String.Empty;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR_BUSQUEDA] = obj;
        }


        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<SubDistribuidor> list = (List<SubDistribuidor>)this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR_LISTA];

            SubDistribuidorSearch excel = new SubDistribuidorSearch();
            return excel.generateExcel(list, (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO]);
        }

        

        // GET: SubDistribuidor
        [HttpGet]
        public ActionResult Index()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaSubDistribuidor)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();

        }

        public String ConsultarSiExisteSubDistribuidor()
        {
            int idSubDistribuidor = int.Parse(Request["idSubDistribuidor"].ToString());
            SubDistribuidorBL bL = new SubDistribuidorBL();
            SubDistribuidor obj = bL.getSubDistribuidorById(idSubDistribuidor);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR_VER] = obj;

            obj = (SubDistribuidor)this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR];
            if (obj == null)
                return "{\"existe\":\"false\",\"idSubDistribuidor\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"idSubDistribuidor\":\"" + obj.idSubDistribuidor + "\"}";
        }


        public void iniciarEdicionSubDistribuidor()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            SubDistribuidor obj = (SubDistribuidor)this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR_VER];
            this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR] = obj;
        }

        public String Create()
        {
            SubDistribuidorBL bL = new SubDistribuidorBL();
            SubDistribuidor obj = (SubDistribuidor)this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR];

            obj = bL.insertSubDistribuidor(obj);
            this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public String Update()
        {
            SubDistribuidorBL bL = new SubDistribuidorBL();
            SubDistribuidor obj = (SubDistribuidor)this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR];

            if (obj.idSubDistribuidor == 0)
            {
                obj = bL.insertSubDistribuidor(obj);
                this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR] = null;
            }
            else
            {
                obj = bL.updateSubDistribuidor(obj);
                this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR] = null;
            }
            String resultado = JsonConvert.SerializeObject(obj);
            //this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            return resultado;
        }
        
        public void ChangeInputString()
        {
            SubDistribuidor obj = (SubDistribuidor) this.SubDistribuidorSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.SubDistribuidorSession = obj;
        }

        public void ChangeInputInt()
        {
            SubDistribuidor obj = (SubDistribuidor)this.SubDistribuidorSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.SubDistribuidorSession = obj;
        }

        public void ChangeInputDecimal()
        {
            SubDistribuidor obj = (SubDistribuidor)this.SubDistribuidorSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Decimal.Parse(this.Request.Params["valor"]));
            this.SubDistribuidorSession = obj;
        }
        public void ChangeInputBoolean()
        {
            SubDistribuidor obj = (SubDistribuidor)this.SubDistribuidorSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.SubDistribuidorSession = obj;
        }


        private SubDistribuidor SubDistribuidorSession
        {
            get
            {
                SubDistribuidor obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaSubDistribuidores: obj = (SubDistribuidor)this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR_BUSQUEDA]; break;
                    case Constantes.paginas.CUSubDistribuidor: obj = (SubDistribuidor)this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaSubDistribuidores: this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR_BUSQUEDA] = value; break;
                    case Constantes.paginas.CUSubDistribuidor: this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR] = value; break;
                }
            }
        }

        public ActionResult CancelarCreacionSubDistribuidor()
        {
            this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("List", "SubDistribuidor");

        }
    }


}