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
        [HttpGet]
        public ActionResult Index()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaGrupoClientes;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA] == null)
            {
                instanciarGrupoClienteBusqueda();
            }

            GrupoCliente grupoClienteSearch = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaGrupoClientes;
            ViewBag.grupoCliente = grupoClienteSearch;
            ViewBag.Si = Constantes.MENSAJE_SI;
            ViewBag.No = Constantes.MENSAJE_NO;
            return View();

        }

        private GrupoCliente GrupoClienteSession
        {
            get
            {
                GrupoCliente grupoCliente = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaGrupoClientes: grupoCliente = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoGrupoCliente: grupoCliente = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE]; break;
                }
                return grupoCliente;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaGrupoClientes: this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoGrupoCliente: this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE] = value; break;
                }
            }
        }

        private void instanciarGrupoCliente()
        {
            GrupoCliente grupoCliente = new GrupoCliente();
            grupoCliente.idGrupoCliente = 0;
            grupoCliente.ciudad = new Ciudad();
            grupoCliente.codigo = String.Empty;
            grupoCliente.nombre = String.Empty;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            grupoCliente.IdUsuarioRegistro = usuario.idUsuario;
            grupoCliente.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE] = grupoCliente;
        }

        private void instanciarGrupoClienteBusqueda()
        {
            GrupoCliente grupoCliente = new GrupoCliente();
            grupoCliente.idGrupoCliente = 0;
            grupoCliente.ciudad = new Ciudad();
            grupoCliente.codigo = String.Empty;
            grupoCliente.nombre = String.Empty;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            grupoCliente.IdUsuarioRegistro = usuario.idUsuario;
            grupoCliente.usuario = usuario;
            grupoCliente.Estado = 1;

            this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA] = grupoCliente;
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
                this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE] = null;
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
            this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE] = null;
         //   }
            String resultado = JsonConvert.SerializeObject(grupoCliente);
            //this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            return resultado;
        }

        [HttpPost]
        public String AddCliente()
        {
            int success = 1;
            string message = "";
            ClienteBL clienteBl = new ClienteBL();
            GrupoCliente grupoCliente = this.GrupoClienteSession;
            
            Guid idCliente = Guid.Parse(this.Request.Params["idCliente"]);

            foreach (Cliente cli in grupoCliente.miembros) { 
                if (cli.idCliente == idCliente)
                {
                    success = 0;
                    message = "El cliente ya es miembro del grupo.";
                }
            }

            Cliente cliente = null;
            if (success == 1)
            {
                cliente = clienteBl.getCliente(idCliente);

                cliente.grupoCliente = grupoCliente;
                cliente.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                clienteBl.updateClienteSunat(cliente);

                grupoCliente.miembros.Add(cliente);
                cliente.grupoCliente = null;
                message = "Se agregó el cliente al grupo.";
                this.GrupoClienteSession = grupoCliente;
            }

            String clienteJson = JsonConvert.SerializeObject(cliente);

            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\", \"cliente\":" + clienteJson + "}";
        }

        public ActionResult CancelarCreacionGrupoCliente()
        {
            this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            return RedirectToAction("Index", "GrupoCliente");
        }

        public void CleanBusqueda()
        {
            instanciarGrupoClienteBusqueda();
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

        public String Search()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaGrupoClientes;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            GrupoCliente grupoCliente = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA];
            GrupoClienteBL bl = new GrupoClienteBL();
            List<GrupoCliente> list = bl.getGruposCliente(grupoCliente);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
            //return pedidoList.Count();
        }


        public String GetGrupoCliente()
        {
            GrupoCliente grupoCliente = this.GrupoClienteSession;
            int idGrupoCliente = int.Parse(Request["idGrupoCliente"].ToString());
            GrupoClienteBL bl = new GrupoClienteBL();
            Ciudad ciudad = grupoCliente.ciudad;
            grupoCliente = bl.getGrupo(idGrupoCliente);
            grupoCliente.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            String resultado = JsonConvert.SerializeObject(grupoCliente);
            this.GrupoClienteSession = grupoCliente;
            return resultado;
        }

        public String Show()
        {
            int idGrupoCliente = int.Parse(Request["idGrupoCliente"].ToString());

            GrupoClienteBL bl = new GrupoClienteBL();
            GrupoCliente grupoCliente = bl.getGrupo(idGrupoCliente);
            grupoCliente.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            List<DocumentoDetalle> listaPrecios = bl.getPreciosVigentesGrupoCliente(idGrupoCliente);
            List<Cliente> clientes = bl.getClientesGrupo(idGrupoCliente);

            grupoCliente.miembros = clientes;

            String resultado = "{\"grupoCliente\":" + JsonConvert.SerializeObject(grupoCliente) + ", \"precios\":" + JsonConvert.SerializeObject(listaPrecios)  + "}";
            
            this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_VER] = grupoCliente;
            
            return resultado;
        }


        public String ConsultarSiExisteGrupoCliente()
        {
            GrupoCliente grupoCliente = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE];
            if (grupoCliente == null)
                return "{\"existe\":\"false\",\"codigo\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"codigo\":\"" + grupoCliente.codigo + "\"}";
        }


        public void iniciarEdicionGrupoCliente()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            GrupoCliente grupoClienteVer = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_VER];
            this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE] = grupoClienteVer;
        }

        public void ChangePlazoCreditoAprobado()
        {
            this.GrupoClienteSession.plazoCreditoAprobado = (DocumentoVenta.TipoPago)Int32.Parse(this.Request.Params["plazoCreditoAprobado"]);
        }


        public void ChangePlazoCreditoSolicitado()
        {
            this.GrupoClienteSession.plazoCreditoSolicitado = (DocumentoVenta.TipoPago)Int32.Parse(this.Request.Params["plazoCreditoSolicitado"]);
        }

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();

            return clienteBL.getCLientesBusqueda(data);
        }

        [HttpPost]
        public String QuitarClienteGrupo()
        {
            int success = 0;
            string message = "";
            ClienteBL clienteBl = new ClienteBL();
            GrupoCliente grupoCliente = this.GrupoClienteSession;

            Guid idCliente = Guid.Parse(this.Request.Params["idCliente"]);
            int removeAt = -1;
            foreach (Cliente cli in grupoCliente.miembros)
            {
                if (cli.idCliente == idCliente)
                {
                    success = 1;
                    removeAt = grupoCliente.miembros.IndexOf(cli);
                }
            }

            Cliente cliente = null;
            if (success == 1)
            {
                cliente = clienteBl.getCliente(idCliente);

                cliente.grupoCliente = null;
                cliente.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                clienteBl.updateClienteSunat(cliente);

                grupoCliente.miembros.RemoveAt(removeAt);
                message = "Se retiró el cliente del grupo.";
                this.GrupoClienteSession = grupoCliente;
            } else
            {
                message = "El cliente no existe o no es miembro del grupo.";
            }

            String clienteJson = JsonConvert.SerializeObject(cliente);

            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\"}";
        }
    }
}