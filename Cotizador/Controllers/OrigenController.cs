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
    public class OrigenController : ParentController
    {

        [HttpGet]
        public ActionResult List()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaOrigenes;
            
            if (this.Session[Constantes.VAR_SESSION_ORIGEN_BUSQUEDA] == null)
            {
                instanciarOrigenBusqueda();
            }
            

            Origen objSearch = (Origen)this.Session[Constantes.VAR_SESSION_ORIGEN_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaOrigenes;
            ViewBag.origen = objSearch;

            return View();
        }

        public ActionResult GetOrigenes(string origenSelectId, string selectedValue = null, string disabled = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Origen obj = new Origen();
            obj.Estado = 1;

            OrigenBL bL = new OrigenBL();
            List<Origen> list = bL.getOrigenes(obj);

            var model = new OrigenViewModels
            {
                Data = list,
                OrigenSelectId = origenSelectId,
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_Origen", model);


        }
        public String SearchList()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaOrigenes;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            Origen obj = (Origen)this.Session[Constantes.VAR_SESSION_ORIGEN_BUSQUEDA];

            OrigenBL bL = new OrigenBL();
            List<Origen> list = bL.getOrigenes(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_ORIGEN_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);

        }

        public String Show()
        {
            int idOrigen = int.Parse(Request["idOrigen"].ToString());
            OrigenBL bL = new OrigenBL();
            Origen obj = bL.getOrigenById(idOrigen);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado =  JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_ORIGEN_VER] = obj;
            return resultado;
        }

        public ActionResult Editar(int? idOrigen = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.CUOrigen;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            /*if (!usuario.modificaMaestroClientes && !usuario.modificaProducto)
            {
                return RedirectToAction("Login", "Account");
            }*/
            


            if (this.Session[Constantes.VAR_SESSION_ORIGEN] == null && idOrigen == null)
            {
                instanciarOrigen();
            }

            Origen obj = (Origen)this.Session[Constantes.VAR_SESSION_ORIGEN];
            
            if (idOrigen != null)
            {
                OrigenBL bL = new OrigenBL();
                obj = bL.getOrigenById(idOrigen.Value);
                obj.IdUsuarioRegistro = usuario.idUsuario;
                obj.usuario = usuario;
                
                this.Session[Constantes.VAR_SESSION_ORIGEN] = obj;
            }
            

            ViewBag.origen = obj;
            return View();

        }

        private void instanciarOrigen()
        {
            Origen obj = new Origen();
            obj.idOrigen = 0;
            obj.Estado = 1;
            obj.codigo = String.Empty;
            obj.nombre = String.Empty;
            
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_ORIGEN] = obj;
        }

        private void instanciarOrigenBusqueda()
        {
            Origen obj = new Origen();
            obj.idOrigen = 0;
            obj.Estado = 1;
            obj.codigo = String.Empty;
            obj.nombre = String.Empty;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_ORIGEN_BUSQUEDA] = obj;
        }


        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<Origen> list = (List<Origen>)this.Session[Constantes.VAR_SESSION_ORIGEN_LISTA];

            OrigenSearch excel = new OrigenSearch();
            return excel.generateExcel(list, (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO]);
        }

        

        // GET: Origen
        [HttpGet]
        public ActionResult Index()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            /*if (!usuario.modificaMaestroProductos)
            {
                return RedirectToAction("Login", "Account");
            }*/

            return View();

        }

        public String ConsultarSiExisteOrigen()
        {
            int idOrigen = int.Parse(Request["idOrigen"].ToString());
            OrigenBL bL = new OrigenBL();
            Origen obj = bL.getOrigenById(idOrigen);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_ORIGEN_VER] = obj;

            obj = (Origen)this.Session[Constantes.VAR_SESSION_ORIGEN];
            if (obj == null)
                return "{\"existe\":\"false\",\"idOrigen\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"idOrigen\":\"" + obj.idOrigen + "\"}";
        }


        public void iniciarEdicionOrigen()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Origen obj = (Origen)this.Session[Constantes.VAR_SESSION_ORIGEN_VER];
            this.Session[Constantes.VAR_SESSION_ORIGEN] = obj;
        }

        public String Create()
        {
            OrigenBL bL = new OrigenBL();
            Origen obj = (Origen)this.Session[Constantes.VAR_SESSION_ORIGEN];

            obj = bL.insertOrigen(obj);
            this.Session[Constantes.VAR_SESSION_ORIGEN] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public String Update()
        {
            OrigenBL bL = new OrigenBL();
            Origen obj = (Origen)this.Session[Constantes.VAR_SESSION_ORIGEN];

            if (obj.idOrigen == 0)
            {
                obj = bL.insertOrigen(obj);
                this.Session[Constantes.VAR_SESSION_ORIGEN] = null;
            }
            else
            {
                obj = bL.updateOrigen(obj);
                this.Session[Constantes.VAR_SESSION_ORIGEN] = null;
            }
            String resultado = JsonConvert.SerializeObject(obj);
            //this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            return resultado;
        }
        
        public void ChangeInputString()
        {
            Origen obj = (Origen) this.OrigenSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.OrigenSession = obj;
        }

        public void ChangeInputInt()
        {
            Origen obj = (Origen)this.OrigenSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.OrigenSession = obj;
        }

        public void ChangeInputDecimal()
        {
            Origen obj = (Origen)this.OrigenSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Decimal.Parse(this.Request.Params["valor"]));
            this.OrigenSession = obj;
        }
        public void ChangeInputBoolean()
        {
            Origen obj = (Origen)this.OrigenSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.OrigenSession = obj;
        }


        private Origen OrigenSession
        {
            get
            {
                Origen obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaOrigenes: obj = (Origen)this.Session[Constantes.VAR_SESSION_ORIGEN_BUSQUEDA]; break;
                    case Constantes.paginas.CUOrigen: obj = (Origen)this.Session[Constantes.VAR_SESSION_ORIGEN]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaOrigenes: this.Session[Constantes.VAR_SESSION_ORIGEN_BUSQUEDA] = value; break;
                    case Constantes.paginas.CUOrigen: this.Session[Constantes.VAR_SESSION_ORIGEN] = value; break;
                }
            }
        }

        public ActionResult CancelarCreacionOrigen()
        {
            this.Session[Constantes.VAR_SESSION_ORIGEN] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("List", "Origen");

        }
    }


}