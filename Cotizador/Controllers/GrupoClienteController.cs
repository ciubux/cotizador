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
using Cotizador.ExcelExport;

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

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.visualizaGrupoClientes && !usuario.esVendedor)
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

            DateTime fechaConsultaPrecios = new DateTime(DateTime.Now.AddDays(-720).Year, 1, 1, 0, 0, 0);
            this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA_FECHA_PRECIOS_VER] = fechaConsultaPrecios;
            ViewBag.fechaConsultaPrecios = fechaConsultaPrecios;

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
            grupoCliente.sinPlazoCreditoAprobado = false;
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
            List<GrupoCliente> grupoClienteList = grupoClienteBL.getGruposCliente(usuario.idUsuario);


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
                if (!usuario.modificaGrupoClientes && !usuario.esVendedor)
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


        public ActionResult Miembros(int? idGrupoCliente)
        {
            GrupoClienteBL bl = new GrupoClienteBL();
            GrupoCliente grupoCliente = new GrupoCliente();

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            
            if (idGrupoCliente == null || idGrupoCliente == 0)
            {
                return RedirectToAction("Index", "GrupoCliente");
            }

            grupoCliente = bl.getGrupo(idGrupoCliente.Value);
            grupoCliente.usuario = usuario;
            grupoCliente.IdUsuarioRegistro = usuario.idUsuario;
            grupoCliente.miembros = bl.getClientesGrupo(grupoCliente.idGrupoCliente);

            if (!usuario.modificaMiembrosGrupoCliente && !grupoCliente.isOwner)
            {
                return RedirectToAction("Index", "GrupoCliente");
            }

            ViewBag.grupoCliente = grupoCliente;
            return View();
        }

        [HttpGet]
        public ActionResult ExportLastShowCanasta(int tipoDescarga)
        {
            GrupoCliente obj = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_VER];

            CanastaGrupoCliente excel = new CanastaGrupoCliente();
            GrupoClienteBL bl = new GrupoClienteBL();
            DateTime fechaPrecios = (DateTime)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA_FECHA_PRECIOS_VER];

            bool soloCanastaHabitual = false;
            switch (tipoDescarga)
            {
                case 1: obj.listaPrecios = bl.getPreciosVigentesGrupoCliente(obj.idGrupoCliente); break;
                case 2: soloCanastaHabitual = true; break;
                case 3: obj.listaPrecios = bl.getPreciosHistoricoGrupoCliente(obj.idGrupoCliente); break;
                case 4: obj.listaPrecios = bl.getPreciosVigentesGrupoCliente(obj.idGrupoCliente, fechaPrecios); break;
            }

            return excel.generateExcel(obj, soloCanastaHabitual);
        }


        [HttpPost]
        public String Create()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaGrupoClientes && !usuario.esVendedor)
            {
                return "";
            }

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

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaGrupoClientes && !grupoCliente.isOwner)
            {
                return "";
            }

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
            GrupoClienteBL bl = new GrupoClienteBL();


            Guid idCliente = Guid.Parse(this.Request.Params["idCliente"]);
            int idGrupoCliente = int.Parse(this.Request.Params["idGrupoCliente"]);
            int heredaPrecios = int.Parse(this.Request.Params["heredaPrecios"]);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            GrupoCliente grupoCliente = bl.getGrupo(idGrupoCliente);
            grupoCliente.miembros = bl.getClientesGrupo(grupoCliente.idGrupoCliente);
            grupoCliente.usuario = usuario;

            if (!usuario.modificaMiembrosGrupoCliente && !grupoCliente.isOwner)
            {
                return "";
            }
            

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
                cliente.usuario = usuario;
                cliente.habilitadoNegociacionGrupal = heredaPrecios == 1 ? true : false;
                clienteBl.updateClienteSunatAsync(cliente);

                grupoCliente.miembros.Add(cliente);
                cliente.grupoCliente = null;
                message = "Se agregó el cliente al grupo.";
            }

            String clienteJson = JsonConvert.SerializeObject(cliente);

            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\", \"cliente\":" + clienteJson + "}";
        }

        [HttpPost]
        public String AddClientesRUC()
        {
            int success = 1;
            string message = "";
            ClienteBL clienteBl = new ClienteBL();
            GrupoClienteBL bl = new GrupoClienteBL();

            int encontrados = 0;
            int agregados = 0;
            int existentes = 0;
            bool existe = false;
            string ruc = this.Request.Params["ruc"].ToString();
            int idGrupoCliente = int.Parse(this.Request.Params["idGrupoCliente"]);
            int heredaPrecios = int.Parse(this.Request.Params["heredaPrecios"]);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            GrupoCliente grupoCliente = bl.getGrupo(idGrupoCliente);
            grupoCliente.miembros = bl.getClientesGrupo(grupoCliente.idGrupoCliente);
            grupoCliente.usuario = usuario;

            if (!usuario.modificaMiembrosGrupoCliente && !grupoCliente.isOwner)
            {
                return "";
            }
            
            List<Cliente> lista = clienteBl.getClientesByRUC(ruc);
            List<Cliente> listaAgregados = new List<Cliente>();

            encontrados = lista.Count;


            foreach (Cliente cli in lista)
            {
                existe = false;
                foreach (Cliente miembro in grupoCliente.miembros)
                {
                    if (miembro.idCliente == cli.idCliente)
                    {
                        existe = true;
                    }
                }

                if (existe)
                {
                    existentes = existentes + 1;
                } else
                {
                    cli.grupoCliente = grupoCliente;
                    cli.usuario = usuario;
                    cli.habilitadoNegociacionGrupal = heredaPrecios == 1 ? true : false;
                    clienteBl.updateClienteSunatAsync(cli);
                    cli.grupoCliente = null;
                    grupoCliente.miembros.Add(cli);

                    listaAgregados.Add(cli);
                }
            }

            agregados = encontrados - existentes;

            
            message = "Se encontraron " + encontrados.ToString() + " clientes con el RUC ingresado";

            if (agregados == encontrados)
            {
                message = message + ". Se agregaron todos satisfactoriamente al grupo.";
            } else
            {
                if (existentes == encontrados)
                {
                    message = message + ". Todos ya pertenecían al grupo.";
                } else
                {
                    message = message + ", " + existentes.ToString() + " ya pertenecían al grupo. Se agregaron " + agregados.ToString() + " clientes al grupo.";
                }
            }
            

            String clientesJson = JsonConvert.SerializeObject(listaAgregados);

            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\", \"agregados\":" + clientesJson + "}";
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

        public void ChangeSinPlazoCreditoAprobado()
        {
            GrupoCliente grupoCliente = this.GrupoClienteSession;
            grupoCliente.sinPlazoCreditoAprobado = Int32.Parse(this.Request.Params["sinPlazoCreditoAprobado"]) ==1 ? true:false;          
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

            DateTime fechaPrecios = (DateTime)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA_FECHA_PRECIOS_VER];

            GrupoClienteBL bl = new GrupoClienteBL();
            GrupoCliente grupoCliente = bl.getGrupo(idGrupoCliente);
            grupoCliente.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            List<DocumentoDetalle> listaPrecios = bl.getPreciosVigentesGrupoCliente(idGrupoCliente, fechaPrecios);
            List<Cliente> clientes = bl.getClientesGrupo(idGrupoCliente);

            grupoCliente.miembros = clientes;
            grupoCliente.listaPrecios = listaPrecios;

            String resultado = "{\"grupoCliente\":" + JsonConvert.SerializeObject(grupoCliente) + ", \"precios\":" + JsonConvert.SerializeObject(listaPrecios)  + "}";
            
            this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_VER] = grupoCliente;
            
            return resultado;
        }

        public String ConsultaPreciosGrupo()
        {
            int idGrupoCliente = int.Parse(Request["idGrupoCliente"].ToString());
            DateTime fechaPrecios = (DateTime) this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA_FECHA_PRECIOS_VER];

            GrupoClienteBL bl = new GrupoClienteBL();
            List<DocumentoDetalle> listaPrecios = bl.getPreciosVigentesGrupoCliente(idGrupoCliente, fechaPrecios);


            String resultado = "{\"precios\":" + JsonConvert.SerializeObject(listaPrecios) + "}";


            return resultado;
        }

        public void ChangeFechaVigenciaPrecios()
        {
            if (this.Request.Params["val"] != null && this.Request.Params["val"] != String.Empty)
            {
                String[] fiv = this.Request.Params["val"].Split('/');
                this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA_FECHA_PRECIOS_VER] = new DateTime(Int32.Parse(fiv[2]), Int32.Parse(fiv[1]), Int32.Parse(fiv[0]), 0, 0, 0);
            }
        }

        [HttpPost]
        public String ActualizarSKUCliente()
        {
            int success = 1;
            string message = "";

            GrupoCliente grupoCliente = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_VER];
            GrupoClienteBL bl = new GrupoClienteBL();

            string skuCliente = this.Request.Params["skuCliente"].ToString();
            Guid idProducto = Guid.Parse(this.Request.Params["idProducto"]);
            int replicaMiembros = int.Parse(this.Request.Params["replicaMiembros"]);
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (grupoCliente.modificaCanasta == 1)
            {
                if (grupoCliente.listaPrecios.Where(p => p.producto.idProducto == idProducto).FirstOrDefault() != null && bl.setSKUCliente(skuCliente, grupoCliente.idGrupoCliente, usuario.idUsuario, idProducto, replicaMiembros))
                {
                    message = "Se registró el SKU del cliente en el Grupo y sus miembros que heredan precios.";
                }
                else
                {
                    success = 0;
                    message = "No se pudo registrar el SKU.";
                }
            }
            else
            {
                success = 0;
                message = "No tiene permiso para realizar esta acción.";
            }


            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\"}";
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
        public String SearchClientesRuc()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();

            return clienteBL.getCLientesBusquedaRUC(data);
        }

        [HttpPost]
        public String QuitarClienteGrupo()
        {
            int success = 0;
            string message = "";
            ClienteBL clienteBl = new ClienteBL();
            GrupoClienteBL bl = new GrupoClienteBL();


            Guid idCliente = Guid.Parse(this.Request.Params["idCliente"]);
            int idGrupoCliente = int.Parse(this.Request.Params["idGrupoCliente"]);

            GrupoCliente grupoCliente = bl.getGrupo(idGrupoCliente);
            grupoCliente.miembros = bl.getClientesGrupo(grupoCliente.idGrupoCliente);
            

            int removeAt = -1;
            foreach (Cliente cli in grupoCliente.miembros)
            {
                if (cli.idCliente == idCliente)
                {
                    success = 1;
                    removeAt = grupoCliente.miembros.IndexOf(cli);
                }
            }

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            grupoCliente.usuario = usuario;

            if (!usuario.modificaMiembrosGrupoCliente && !grupoCliente.isOwner)
            {
                success = 0;
            }
            

            Cliente cliente = null;
            if (success == 1)
            {
                cliente = clienteBl.getCliente(idCliente);

                cliente.grupoCliente = null;
                cliente.usuario = usuario;
                clienteBl.updateClienteSunatAsync(cliente);

                grupoCliente.miembros.RemoveAt(removeAt);
                message = "Se retiró el cliente del grupo.";
            } else
            {
                message = "El cliente no existe o no es miembro del grupo.";
            }

            String clienteJson = JsonConvert.SerializeObject(cliente);

            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\"}";
        }

        [HttpPost]
        public String UpdateMiembro()
        {
            int success = 0;
            string message = "";
            ClienteBL clienteBl = new ClienteBL();
            GrupoClienteBL bl = new GrupoClienteBL();


            Guid idCliente = Guid.Parse(this.Request.Params["idCliente"]);
            int idGrupoCliente = int.Parse(this.Request.Params["idGrupoCliente"]);
            int heredaPrecios = int.Parse(this.Request.Params["heredaPrecios"]);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            GrupoCliente grupoCliente = bl.getGrupo(idGrupoCliente);
            grupoCliente.miembros = bl.getClientesGrupo(grupoCliente.idGrupoCliente);
            grupoCliente.usuario = usuario;

            if (!usuario.modificaMiembrosGrupoCliente && !grupoCliente.isOwner)
            {
                return "";
            }
            

            foreach (Cliente cli in grupoCliente.miembros)
            {
                if (cli.idCliente == idCliente)
                {
                    success = 1;
                }
            }

            Cliente cliente = null;
            if (success == 1)
            {
                cliente = clienteBl.getCliente(idCliente);

                cliente.usuario = usuario;
                cliente.habilitadoNegociacionGrupal = heredaPrecios == 1 ? true : false;
                clienteBl.updateClienteSunatAsync(cliente);
                
                message = "Se actualizó el cliente.";
            } else
            {
                message = "El cliente no es miembro del grupo.";
            }
            String clienteJson = JsonConvert.SerializeObject(cliente);

            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\", \"cliente\":" + clienteJson + "}";
        }


        [HttpPost]
        public String LimpiaCanasta()
        {
            int success = 1;
            string message = "";
            GrupoClienteBL bl = new GrupoClienteBL();
            GrupoCliente grupoCliente = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_VER];

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            int aplicaMiembros = int.Parse(this.Request.Params["aplicaMiembros"]);

            if (usuario.modificaCanastaGrupoCliente)
            {
                if (bl.limpiaCanasta(grupoCliente.idGrupoCliente, aplicaMiembros))
                {
                    message = "Se limpió la canasta habitual de grupo";
                    if (aplicaMiembros == 1)
                    {
                        message = message + " y de los miembros que heredan precios";
                    }
                    message = message + ".";

                }
                else
                {
                    success = 0;
                    message = "Ocurrió un error al limpiar la canasta habitual de grupo.";
                }
            }
            else
            {
                success = 0;
                message = "No tiene permiso para realizar esta acción.";
            }

            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\"}";
        }

        [HttpPost]
        public String AgregarProductoACanasta()
        {
            int success = 1;
            string message = "";
            GrupoClienteBL bl = new GrupoClienteBL();
            GrupoCliente grupoCliente = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_VER];

            Guid idProducto = Guid.Parse(this.Request.Params["idProducto"]);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (usuario.modificaCanastaGrupoCliente) { 
                if (bl.agregarProductoCanasta(grupoCliente.idGrupoCliente, idProducto, usuario))
                {
                    message = "Se agregó el producto a la canasta habitual de grupo y de los miembros que heredan precios.";
                }
                else
                {
                    success = 0;
                    message = "No se pudo agregar el producto a la canasta.";
                }
            }
            else
            {
                success = 0;
                message = "No tiene permiso para realizar esta acción.";
            }

            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\"}";
        }


        [HttpPost]
        public String RetirarProductoDeCanasta()
        {
            int success = 1;
            string message = "";
            GrupoClienteBL bl = new GrupoClienteBL();
            GrupoCliente grupoCliente = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_VER];

            Guid idProducto = Guid.Parse(this.Request.Params["idProducto"]);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (usuario.modificaCanastaGrupoCliente)
            {
                if (bl.retiraProductoCanasta(grupoCliente.idGrupoCliente, idProducto, usuario))
                {
                    message = "Se retiró el producto a la canasta habitual de grupo y de los miembros que heredan precios.";
                }
                else
                {
                    success = 0;
                    message = "No se pudo retirar el producto de la canasta.";
                }
            }
            else
            {
                success = 0;
                message = "No tiene permiso para realizar esta acción.";
            }

            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\"}";
        }

        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<GrupoCliente> list = (List<GrupoCliente>)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_LISTA];

            GrupoClienteSearch excel = new GrupoClienteSearch();
           return excel.generateExcel(list);
        }

        [HttpGet]
        public ActionResult ExportarMienbros()
        {
            GrupoCliente grupoCliente = (GrupoCliente)this.Session[Constantes.VAR_SESSION_GRUPO_CLIENTE_BUSQUEDA];
            List<GrupoCliente> list = new List<GrupoCliente>();
            GrupoClienteBL bl = new GrupoClienteBL();
            list = bl.getGruposMienbrosExportar(grupoCliente);            
            GrupoClienteSearch excel = new GrupoClienteSearch();
            return excel.mienbrosGruposExcel(list);
        }
    }
}