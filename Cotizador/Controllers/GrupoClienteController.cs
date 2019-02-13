using BusinessLayer;
using Cotizador.Models;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class GrupoClienteController : Controller
    {
        // GET: GrupoCliente
        public ActionResult Index()
        {
            return View();
        }


        private GrupoCliente GrupoClienteSession
        {
            get
            {
                GrupoCliente grupoCliente = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaClientes: grupoCliente = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoCliente: grupoCliente = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE]; break;
                }
                return grupoCliente;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaClientes: this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoCliente: this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE] = value; break;
                }
            }
        }

        private void instanciarGrupoCliente()
        {
            GrupoCliente grupoCliente = new GrupoCliente();
            grupoCliente.idGrupoCliente = 0;
            grupoCliente.ciudad = new Ciudad();
            grupoCliente.codigo = String.Empty;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            grupoCliente.IdUsuarioRegistro = usuario.idUsuario;
            grupoCliente.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE] = grupoCliente;
        }



        public ActionResult GetGruposCliente(string grupoClienteSelectId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            GrupoClienteBL grupoClienteBL = new GrupoClienteBL();
            List<GrupoCliente> grupoClienteList = grupoClienteBL.getGruposCliente();


            var model = new GrupoClienteViewModels
            {
                Data = grupoClienteList,
                GrupoClienteSelectId = grupoClienteSelectId,
                SelectedValue = selectedValue
            };

            return PartialView("_GrupoCliente", model);
        }


        public ActionResult Editar(int? idGrupoCliente = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoGrupoCliente;
            Usuario usuario = null;
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                if (!usuario.modificaMaestroClientes)
                {
                    return RedirectToAction("Login", "Account");
                }
            }


            if (this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE] == null || idGrupoCliente == 0)
            {

                instanciarGrupoCliente();
            }

            GrupoCliente grupoCliente = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE];

            List<GrupoCliente> grupoClienteList = new List<GrupoCliente>();

            if (idGrupoCliente != null && idGrupoCliente > 0)
            {
                GrupoClienteBL clienteBL = new GrupoClienteBL();
                grupoCliente = clienteBL.getGrupo(idGrupoCliente.Value);
                grupoCliente.IdUsuarioRegistro = usuario.idUsuario;
                grupoCliente.usuario = usuario;
                this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE] = grupoCliente;
            }

            ViewBag.grupoCliente = grupoCliente;
            return View();

        }

        [HttpPost]
        public String Create()
        {
            GrupoClienteBL grupoClienteBL = new GrupoClienteBL();
            GrupoCliente grupoCliente = this.GrupoClienteSession;
            grupoCliente = grupoClienteBL.insertGrupoCliente(grupoCliente);
            if (grupoCliente.codigo != null && !grupoCliente.codigo.Equals(""))
            {
                this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            }
            String resultado = JsonConvert.SerializeObject(grupoCliente);
            return resultado;
        }

        [HttpPost]
        public String Update()
        {
            GrupoClienteBL grupoClienteBL = new GrupoClienteBL();
            GrupoCliente grupoCliente = this.GrupoClienteSession;


            /*if (grupoCliente.idGrupoCliente == 0)
            {
                grupoCliente = clienteBL.insertClienteSunat(grupoCliente);
                if (grupoCliente.codigo != null && !grupoCliente.codigo.Equals(""))
                {
                    this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
                }
            }
            else
            {*/
            grupoCliente = grupoClienteBL.updateGrupoCliente(grupoCliente);
            this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
         //   }
            String resultado = JsonConvert.SerializeObject(grupoCliente);
            //this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            return resultado;
        }

        public ActionResult CancelarCreacionGrupoCliente()
        {
            this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            return RedirectToAction("Index", "GrupoCliente");
        }

        public String ChangeIdCiudad()
        {
            GrupoCliente grupoCliente = this.GrupoClienteSession;
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
            grupoCliente.ciudad = ciudadNueva;
            this.GrupoClienteSession = grupoCliente;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";
        }


        public void ChangeInputString()
        {
            GrupoCliente grupoCliente = this.GrupoClienteSession;
            PropertyInfo propertyInfo = grupoCliente.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(grupoCliente, this.Request.Params["valor"]);
            this.GrupoClienteSession = grupoCliente;
        }

        public void ChangeInputInt()
        {
            GrupoCliente grupoCliente = this.GrupoClienteSession;
            PropertyInfo propertyInfo = grupoCliente.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(grupoCliente, Int32.Parse(this.Request.Params["valor"]));
            this.GrupoClienteSession = grupoCliente;
        }

        public void ChangeInputDecimal()
        {
            GrupoCliente grupoCliente = this.GrupoClienteSession;
            PropertyInfo propertyInfo = grupoCliente.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(grupoCliente, Decimal.Parse(this.Request.Params["valor"]));
            this.GrupoClienteSession = grupoCliente;
        }

    }
}