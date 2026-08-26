using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Cotizador.Models;
using Newtonsoft.Json;
using Model;

namespace Cotizador.Controllers
{
    public class ClienteProductoReservadoController : ParentController
    {
        [HttpGet]
        public ActionResult List()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (Logueado == null || !Logueado.modificaClienteProductoReservado)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session["proveedor"] = null;
            this.Session["familia"] = null;

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ProductosReservadosCliente;

            if (this.Session[Constantes.VAR_SESSION_CLIENTEPRODUCTORESERVADO_BUSQUEDA] == null) 
            {
                instanciarClienteProductoReservadoBusqueda();
            }


            
            ClienteProductoReservado objSearch = (ClienteProductoReservado)this.Session[Constantes.VAR_SESSION_CLIENTEPRODUCTORESERVADO_BUSQUEDA];

            this.Session["proveedor"] = objSearch.producto.proveedor;

            this.Session["s_idCiudadSearchClientesGlobal"] = objSearch.ciudad.idCiudad;

            ViewBag.pagina = (int)Constantes.paginas.ProductosReservadosCliente;
            ViewBag.search = objSearch;

            return View();
        }

        public String SearchList()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.ProductosReservadosCliente;

            ClienteProductoReservado obj = (ClienteProductoReservado)this.Session[Constantes.VAR_SESSION_CLIENTEPRODUCTORESERVADO_BUSQUEDA];

            ClienteProductoReservadoBL bL = new ClienteProductoReservadoBL();
            List<ClienteProductoReservado> list = bL.getClienteProductosReservados(obj);

            //this.Session[Constantes.VAR_SESSION_CLIENTEPRODUCTORESERVADO_LISTA] = list;
            return JsonConvert.SerializeObject(list);
        }

        public String Search()
        {
            ClienteProductoReservado obj = (ClienteProductoReservado)this.Session[Constantes.VAR_SESSION_CLIENTEPRODUCTORESERVADO_BUSQUEDA];

            if (!string.IsNullOrEmpty(this.Request.Params["idCiudad"]))
            {
                obj.ciudad.idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }
            if (!string.IsNullOrEmpty(this.Request.Params["idCliente"]))
            {
                obj.cliente.idCliente = Guid.Parse(this.Request.Params["idCliente"]);
                ClienteBL blCliente = new ClienteBL();
                obj.cliente = blCliente.getCliente(obj.cliente.idCliente);
            }

            obj.idPresentacionUnidad = string.IsNullOrEmpty(this.Request.Params["idPresentacion"]) ? 1 : int.Parse(this.Request.Params["idPresentacion"]);
            obj.Estado = string.IsNullOrEmpty(this.Request.Params["estado"]) ? 1 : int.Parse(this.Request.Params["estado"]);

            obj.producto.sku = string.IsNullOrEmpty(this.Request.Params["sku"]) ? "" : this.Request.Params["sku"].ToString();
            obj.producto.proveedor = string.IsNullOrEmpty(this.Request.Params["proveedor"]) ? "Todos" : this.Request.Params["proveedor"].ToString();
            obj.filtroTieneSolicitudRecargaActiva = string.IsNullOrEmpty(this.Request.Params["tieneSolicitudRecaga"]) ? -1 : int.Parse(this.Request.Params["tieneSolicitudRecaga"]);


            this.Session["proveedor"] = obj.producto.proveedor;

            ClienteProductoReservadoBL bL = new ClienteProductoReservadoBL();
            List<ClienteProductoReservado> list = bL.getClienteProductosReservados(obj);

            this.Session[Constantes.VAR_SESSION_CLIENTEPRODUCTORESERVADO_BUSQUEDA] = obj;

            return JsonConvert.SerializeObject(list);
        }

        private void instanciarClienteProductoReservadoBusqueda()
        {
            ClienteProductoReservado obj = new ClienteProductoReservado();
            obj.idClienteProductoReservado = Guid.Empty;
            obj.Estado = 1;
            obj.ciudad = new Ciudad();
            obj.cliente = new Cliente();
            obj.producto = new Producto();
            obj.idPresentacionUnidad = 2;
            obj.producto = new Producto();
            obj.producto.sku = "";
            obj.producto.proveedor = "Todos";
            obj.filtroTieneSolicitudRecargaActiva = -1;

            if (this.Logueado != null)
            {
                obj.ciudad = this.Logueado.sedeMP;
                obj.usuario = this.Logueado;
            }
            else
            {
                obj.ciudad = new Ciudad();
                obj.ciudad.idCiudad = Guid.Empty;
                obj.ciudad.nombre = "TODOS";
            }


            this.Session[Constantes.VAR_SESSION_CLIENTEPRODUCTORESERVADO_BUSQUEDA] = obj; 
        }

        public String Create()
        {
            ClienteProductoReservadoBL bL = new ClienteProductoReservadoBL();
            ClienteProductoReservado obj = new ClienteProductoReservado();

            if (Logueado == null || !Logueado.modificaClienteProductoReservado)
            {
                return "";
            }

            obj.cliente.idCliente = Guid.Parse(Request["idCliente"].ToString());
            obj.producto.idProducto = Guid.Parse(Request["idProducto"].ToString());
            obj.ciudad.idCiudad = Guid.Parse(Request["idCiudad"].ToString());
            obj.tipo = Request["tipo"];

            if (!string.IsNullOrEmpty(Request["cantidadOriginal"]))
            {
                obj.cantidadOriginal = int.Parse(Request["cantidadOriginal"].ToString());
            }

            obj.Estado = 1;
            obj.IdUsuarioRegistro = Logueado.idUsuario;

            obj = bL.insertClienteProductoReservado(obj);
            return JsonConvert.SerializeObject(obj);
        }

        public String Update()
        {
            ClienteProductoReservadoBL bL = new ClienteProductoReservadoBL();
            ClienteProductoReservado obj = new ClienteProductoReservado();

            if (Logueado == null || !Logueado.modificaClienteProductoReservado)
            {
                return "";
            }

            string strId = Request["idClienteProductoReservado"];
            obj.idClienteProductoReservado = string.IsNullOrEmpty(strId) ? Guid.Empty : Guid.Parse(strId.ToString());

            obj.tipo = Request["tipo"];

            if (!string.IsNullOrEmpty(Request["cantidadOriginal"]))
            {
                obj.cantidadOriginal = int.Parse(Request["cantidadOriginal"].ToString());
            }

            obj.Estado = string.IsNullOrEmpty(Request["estado"]) ? 1 : int.Parse(Request["estado"].ToString());
            obj.IdUsuarioRegistro = Logueado.idUsuario;

            if (obj.idClienteProductoReservado == Guid.Empty)
            {
                obj.cliente.idCliente = Guid.Parse(Request["idCliente"].ToString());
                obj.producto.idProducto = Guid.Parse(Request["idProducto"].ToString());
                obj.ciudad.idCiudad = Guid.Parse(Request["idCiudad"].ToString());

                obj = bL.insertClienteProductoReservado(obj);
            }
            else
            {
                obj = bL.updateClienteProductoReservado(obj);
            }

            return JsonConvert.SerializeObject(obj);
        }

        [HttpPost]
        public string RegistroMasivo(Guid idCiudad, List<string> rucs, List<string> skus, List<int> cantidades)
        {
            if (this.Logueado == null || !this.Logueado.modificaClienteProductoReservado) 
            {
                return JsonConvert.SerializeObject(new { success = 0, message = "No autorizado o sesión expirada" });
            }

            try
            {
                ProductoBL blProducto = new ProductoBL();
                List<Producto> productos = blProducto.GetProductosBySKU(skus); 


                List<ClienteProductoReservado> registrosMasivos = new List<ClienteProductoReservado>();

                for (int i = 0; i < skus.Count; i++)
                {
                    Producto prod = productos.Where(p => p.sku.Equals(skus.ElementAt(i))).FirstOrDefault();

                    if (prod != null && !rucs.ElementAt(i).Trim().Equals(string.Empty))
                    {
                        ClienteProductoReservado item = new ClienteProductoReservado();
                        item.producto = prod;
                        item.cliente = new Cliente();
                        item.cliente.ruc = rucs.ElementAt(i);
                        item.cantidadOriginal = cantidades.ElementAt(i);

                        registrosMasivos.Add(item);
                    }
                }

                ClienteProductoReservadoBL bl = new ClienteProductoReservadoBL();
                bl.InsertClientesProductosReservadosMasivo(this.Logueado.idUsuario, idCiudad, registrosMasivos);

                return JsonConvert.SerializeObject(new { success = 1 });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public String Eliminar()
        {
            try
            {
                Guid idClienteProductoReservado = Guid.Parse(Request["idClienteProductoReservado"].ToString());
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

                ClienteProductoReservadoBL bl = new ClienteProductoReservadoBL();
                bl.EliminarClienteProductoReservado(idClienteProductoReservado, usuario.idUsuario);

                return JsonConvert.SerializeObject(new { success = 1, message = "Reserva eliminada." });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public String AgregarReserva()
        {
            try
            {
                Guid idClienteProductoReservado = Guid.Parse(Request["idClienteProductoReservado"].ToString());
                int cantidadAgregar = int.Parse(Request["cantidadAgregar"].ToString());

                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

                ClienteProductoReservadoBL bl = new ClienteProductoReservadoBL();
                bl.AgregarCantidadReserva(idClienteProductoReservado, cantidadAgregar, usuario.idUsuario);

                return JsonConvert.SerializeObject(new { success = 1, message = "Reserva actualizada correctamente." });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public String HistorialMovimientosReserva()
        {
            try
            {
                Guid idClienteProductoReservado = Guid.Parse(Request["idClienteProductoReservado"].ToString());

                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

                ClienteProductoReservadoBL bl = new ClienteProductoReservadoBL();
                ClienteProductoReservado obj = bl.getClienteProductoReservadoMovimientos(idClienteProductoReservado, usuario.idUsuario);

                return JsonConvert.SerializeObject(new { success = 1, obj = obj });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }
    }
}
