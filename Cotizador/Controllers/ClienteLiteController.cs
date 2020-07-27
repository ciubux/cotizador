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
    public class ClienteLiteController : ParentController
    {

        [HttpGet]
        public ActionResult List()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaClientesLite;


            if (this.Session[Constantes.VAR_SESSION_CLIENTE_LITE_BUSQUEDA] == null)
            {
                instanciarClienteLiteBusqueda();
            }
            

            Cliente objSearch = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE_LITE_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaClientesLite;
            ViewBag.clienteLite = objSearch;

            return View();
        }

        private void instanciarClienteLiteBusqueda()
        {
            this.Session[Constantes.VAR_SESSION_CLIENTE_LITE_BUSQUEDA] = instanciarClienteLiteBusquedaBasic();
        }

        private Cliente instanciarClienteLiteBusquedaBasic()
        {
            Cliente cliente = new Cliente();
            cliente.idCliente = Guid.Empty;
            cliente.ciudad = new Ciudad();
            cliente.ciudad.idCiudad = Guid.Empty;
            cliente.codigo = String.Empty;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            cliente.IdUsuarioRegistro = usuario.idUsuario;
            cliente.usuario = usuario;
            cliente.textoBusqueda = "";

            return cliente;
        }

        public String SearchList()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaClientesLite;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            Cliente obj = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE_LITE_BUSQUEDA];

            ClienteBL bL = new ClienteBL();
            List<Cliente> list = bL.getClientesLite(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_CLIENTE_LITE_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);

        }

        /*public String Show()
        {
            int idOrigen = int.Parse(Request["idOrigen"].ToString());
            OrigenBL bL = new OrigenBL();
            Origen obj = bL.getOrigenById(idOrigen);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado =  JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_ORIGEN_VER] = obj;
            return resultado;
        }
        */

        //[HttpGet]
        //public ActionResult ExportLastSearchExcel()
        //{
        //    List<Origen> list = (List<Origen>)this.Session[Constantes.VAR_SESSION_ORIGEN_LISTA];

        //    OrigenSearch excel = new OrigenSearch();
        //    return excel.generateExcel(list, (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO]);
        //}

        

        // GET: Origen
        [HttpGet]
        public ActionResult Index()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.visualizaClientes)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();

        }

        /*
        public void iniciarEdicionClienteLite()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Origen obj = (Origen)this.Session[Constantes.VAR_SESSION_ORIGEN_VER];
            this.Session[Constantes.VAR_SESSION_ORIGEN] = obj;
        }

      */

        public String ChangeIdCiudad()
        {
            Cliente cliente = this.ClienteLiteSession;

            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }

            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
            cliente.ciudad = ciudadNueva;
            this.ClienteLiteSession = cliente;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";

        }
        public void ChangeInputString()
        {
            Cliente obj = (Cliente) this.ClienteLiteSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.ClienteLiteSession = obj;
        }

        public void ChangeInputInt()
        {
            Cliente obj = (Cliente)this.ClienteLiteSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.ClienteLiteSession = obj;
        }

        public void ChangeInputDecimal()
        {
            Cliente obj = (Cliente)this.ClienteLiteSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Decimal.Parse(this.Request.Params["valor"]));
            this.ClienteLiteSession = obj;
        }
        public void ChangeInputBoolean()
        {
            Cliente obj = (Cliente)this.ClienteLiteSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.ClienteLiteSession = obj;
        }


        private Cliente ClienteLiteSession
        {
            get
            {
                Cliente obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaClientesLite: obj = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE_LITE_BUSQUEDA]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaClientesLite: this.Session[Constantes.VAR_SESSION_CLIENTE_LITE_BUSQUEDA] = value; break;
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