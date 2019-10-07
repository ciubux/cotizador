using Cotizador.Models;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Newtonsoft.Json;
using System.Reflection;

namespace Cotizador.Controllers
{
    public class VendedorController : Controller
    {
        /*
        // GET: Vendedor
        public ActionResult Index()
        {
            return View();
        }
        */

        public ActionResult GetResponsablesComerciales(string vendedorSelectId, string selectedValue = null, string disabled = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            var model = new VendedorViewModels
            {
                Data = usuario.responsableComercialList,
                VendedorSelectId = vendedorSelectId,
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_Vendedor", model);
        }

        public ActionResult GetSupervisoresComerciales(string vendedorSelectId, string selectedValue = null, string disabled = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            var model = new VendedorViewModels
            {
                Data = usuario.supervisorComercialList,
                VendedorSelectId = vendedorSelectId,
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_Vendedor", model);
        }

        public ActionResult GetAsistentesServicioCliente(string vendedorSelectId, string selectedValue = null, string disabled = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            var model = new VendedorViewModels
            {
                Data = usuario.asistenteServicioClienteList,
                VendedorSelectId = vendedorSelectId,
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_Vendedor", model);
        }


        /*-----------------------------------------------------------*/

        [HttpGet]
        public ActionResult Index()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaVendedor)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }


        [HttpGet]
        public ActionResult Lista()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaVendedores;
            if (this.Session[Constantes.VAR_SESSION_VENDEDOR_BUSQUEDA] == null)
            {
                instanciarVendedorBusqueda();
            }

            Vendedor objSearch = (Vendedor)this.Session[Constantes.VAR_SESSION_VENDEDOR_BUSQUEDA];
            ViewBag.pagina = (int)Constantes.paginas.BusquedaVendedores;
            ViewBag.vendedor = objSearch;

            return View();
        }

        private void instanciarVendedorBusqueda()
        {
            Vendedor obj = new Vendedor();
            obj.estado = 1;
            obj.codigo = String.Empty;
            obj.descripcion = String.Empty;
            obj.email = String.Empty;
            obj.contacto = String.Empty;
            obj.pass = String.Empty;
            obj.idCiudad = Guid.Empty;

            this.Session[Constantes.VAR_SESSION_VENDEDOR_BUSQUEDA] = obj;
        }


        public String SearchList()
        {

            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaVendedores;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            Vendedor obj = (Vendedor)this.Session[Constantes.VAR_SESSION_VENDEDOR_BUSQUEDA];
            VendedorBL bL = new VendedorBL();
            List<Vendedor> list = bL.getVendedores(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_VENDEDOR_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
        }

        [HttpPost]
        public String Create()
        {
            VendedorBL bL = new VendedorBL();
            Vendedor obj = (Vendedor)this.Session[Constantes.VAR_SESSION_VENDEDOR];

            obj = bL.insertVendedor(obj);
            this.Session[Constantes.VAR_SESSION_VENDEDOR] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public String Update()
        {
            VendedorBL bL = new VendedorBL();
            Vendedor obj = (Vendedor)this.Session[Constantes.VAR_SESSION_VENDEDOR];

            if (obj.idVendedor == 0)
            {
                obj = bL.insertVendedor(obj);
                this.Session[Constantes.VAR_SESSION_VENDEDOR] = null;
            }
            else
            {
                if (obj.esSupervisorComercial == true)
                {
                    obj.supervisor.idVendedor = 0;
                    obj.esAsistenteServicioCliente = false;
                }
                obj = bL.updateVendedor(obj);
                this.Session[Constantes.VAR_SESSION_VENDEDOR] = null;
            }
            String resultado = JsonConvert.SerializeObject(obj);

            return resultado;
        }

        public void ChangeInputString()
        {
            Vendedor obj = (Vendedor)this.VendedorSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.VendedorSession = obj;
        }

        public void ChangeInputInt()
        {

            Vendedor obj = (Vendedor)this.VendedorSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.VendedorSession = obj;
        }


        public void ChangeInputBoolean()
        {

            Vendedor obj = (Vendedor)this.VendedorSession;            
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Boolean.Parse(this.Request.Params["valor"]));

            this.VendedorSession = obj;
        }



        public void ChangeInputDecimal()
        {
            Vendedor obj = (Vendedor)this.VendedorSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Decimal.Parse(this.Request.Params["valor"]));
            this.VendedorSession = obj;
        }

        public String ChangeIdCiudad()
        {
            Vendedor vendedor = (Vendedor)this.VendedorSession;

            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }

            //CiudadBL ciudadBL = new CiudadBL();
            //Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
            vendedor.idCiudad = idCiudad;
            this.VendedorSession = vendedor;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";

        }

        private Vendedor VendedorSession
        {
            get
            {
                Vendedor obj = null;

                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {

                    case Constantes.paginas.BusquedaVendedores: obj = (Vendedor)this.Session[Constantes.VAR_SESSION_VENDEDOR_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoVendedores: obj = (Vendedor)this.Session[Constantes.VAR_SESSION_VENDEDOR]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaVendedores: this.Session[Constantes.VAR_SESSION_VENDEDOR_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoVendedores: this.Session[Constantes.VAR_SESSION_VENDEDOR] = value; break;
                }
            }
        }

        public ActionResult Editar(int? idVendedor = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoVendedores;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaVendedor)
            {
                return RedirectToAction("Login", "Account");
            }
            if ((Vendedor)this.Session[Constantes.VAR_SESSION_VENDEDOR] == null && idVendedor == null)
            {
                instanciarVendedor();
            }

            Vendedor obj = (Vendedor)this.Session[Constantes.VAR_SESSION_VENDEDOR];

            if (idVendedor != null)
            {
                VendedorBL bL = new VendedorBL();
                obj = bL.getVendedorById(idVendedor.Value);
                obj.usuario.idUsuario = usuario.idUsuario;
                obj.usuario = usuario;

                this.Session[Constantes.VAR_SESSION_VENDEDOR] = obj;
            }
            int existeUsuario = 0;
            if (obj.supervisor.usuario.idUsuario != Guid.Empty)
            {
                existeUsuario = 1;
            }
            int existeSupervisor = 0;
            if (obj.supervisor.idUsuarioVendedor != Guid.Empty)
            {
                existeSupervisor = 1;
            }


            ViewBag.existeUsuario = existeUsuario;
            ViewBag.existeSupervisor = existeSupervisor;
            ViewBag.vendedor = obj;
            return View();

        }

        private void instanciarVendedor()
        {
            Vendedor obj = new Vendedor();
            obj.usuario = new Usuario();
            obj.idVendedor = 0;
            obj.estado = 1;
            obj.cargo = " ";
            Usuario user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.usuario = user;
            obj.supervisor = new Vendedor();
            obj.supervisor.usuario = new Usuario();
            obj.esResponsableComercial = true;
            this.Session[Constantes.VAR_SESSION_VENDEDOR] = obj;
        }


        public ActionResult CancelarCreacionVendedor()
        {
            this.Session[Constantes.VAR_SESSION_VENDEDOR] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("Lista", "Vendedor");

        }

        public void iniciarEdicionVendedor()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Vendedor obj = (Vendedor)this.Session[Constantes.VAR_SESSION_VENDEDOR_VER];
            this.Session[Constantes.VAR_SESSION_VENDEDOR] = obj;
        }

        public String ConsultarSiExisteVendedor()
        {
            int idVendedor = int.Parse(Request["idVendedor"].ToString());
            VendedorBL bL = new VendedorBL();
            Vendedor obj = bL.getVendedorById(idVendedor);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_VENDEDOR_VER] = obj;

            obj = (Vendedor)this.Session[Constantes.VAR_SESSION_VENDEDOR];
            if (obj == null)
                return "{\"existe\":\"false\",\"idVendedor\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"idVendedor\":\"" + obj.idVendedor + "\"}";
        }

        /*-------------------------------------------------*/


        public String SearchUsuario()
        {
            String data = this.Request.Params["data[q]"];
            UsuarioBL usuarioBL = new UsuarioBL();
            return usuarioBL.searchUsuariosVenta(data);
        }

        public String SearchVendedor()
        {
            String data = this.Request.Params["data[q]"];
            VendedorBL vendedorBL = new VendedorBL();
            return vendedorBL.getVendedoresBusqueda(data);
        }


        public String GetUsuario()
        {

            Vendedor vendedor = (Vendedor)this.Session[Constantes.VAR_SESSION_VENDEDOR];
            vendedor.supervisor.usuario = new Usuario();
            Guid idVendedor = Guid.Parse(Request["idVendedor"].ToString());
            UsuarioBL clienteBl = new UsuarioBL();
            vendedor.supervisor.usuario = clienteBl.getUsuarioVendedor(idVendedor);
            vendedor.cargo = vendedor.supervisor.usuario.cargo;
            vendedor.descripcion = vendedor.supervisor.usuario.nombre;
            vendedor.idCiudad = vendedor.supervisor.usuario.sedeMP.idCiudad;
            vendedor.contacto = vendedor.supervisor.usuario.contacto;
            String resultado = JsonConvert.SerializeObject(vendedor.supervisor.usuario);
            this.Session[Constantes.VAR_SESSION_VENDEDOR] = vendedor;

            return resultado;
        }

        public String GetVendedorSupervisor()
        {
            Vendedor vendedor = (Vendedor)this.Session[Constantes.VAR_SESSION_VENDEDOR];
            Usuario vende = vendedor.supervisor.usuario;
            Guid idSupervisor = Guid.Parse(Request["idUsuarioSupervisor"].ToString());
            VendedorBL supervidorBl = new VendedorBL();
            vendedor.supervisor = supervidorBl.getSupervisor(idSupervisor);
            vendedor.supervisor.usuario = vende;
            String resultado = JsonConvert.SerializeObject(vendedor.supervisor);

            this.Session[Constantes.VAR_SESSION_VENDEDOR] = vendedor;
            return resultado;
        }
    }
}