using BusinessLayer;
using Cotizador.Models;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class ListaPreciosController : ParentController
    {
        private ListaPrecios ListaPreciosSession
        {
            get
            {
                ListaPrecios listaPrecios = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaListaPrecios: listaPrecios = (ListaPrecios)this.Session[Constantes.VAR_SESSION_LISTAPRECIOS_BUSQUEDA]; break;
                    case Constantes.paginas.RegistroListaPrecios: listaPrecios = (ListaPrecios)this.Session[Constantes.VAR_SESSION_LISTAPRECIOS]; break;
                }
                return listaPrecios;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaListaPrecios: this.Session[Constantes.VAR_SESSION_LISTAPRECIOS_BUSQUEDA] = value; break;
                    case Constantes.paginas.RegistroListaPrecios: this.Session[Constantes.VAR_SESSION_LISTAPRECIOS] = value; break;
                }
            }
        }
        private void InstanciarListaPrecios()
        {
            ListaPrecios obj = new ListaPrecios();
            obj.idListaPrecios = Guid.Empty;
            obj.Estado = 1;
            
            if (this.Logueado != null)
            {
                obj.usuario = this.Logueado;
            }


            this.Session[Constantes.VAR_SESSION_LISTAPRECIOS] = obj;
        }


        [HttpGet]
        public ActionResult List() 
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaListaPrecios;

            if (Logueado == null || !Logueado.visualizaListaPrecios)
            {
                return RedirectToAction("Login", "Account"); 
            }

            return View(); 
        }

        [HttpPost]
        public String SearchList() 
        {
            try
            {
                ListaPreciosBL bL = new ListaPreciosBL();
                List<ListaPrecios> list = bL.GetListasPrecios(Logueado.idUsuario);

                return JsonConvert.SerializeObject(list); 
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message }); 
            }
        }

        public void IniciarEdicion()
        {
            ListaPrecios obj = (ListaPrecios)this.Session[Constantes.VAR_SESSION_LISTAPRECIOS_VER];
            this.Session[Constantes.VAR_SESSION_LISTAPRECIOS] = obj;
        }

        public String ConsultarSiExisteEdicionActiva()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            ListaPrecios obj = (ListaPrecios)this.Session[Constantes.VAR_SESSION_LISTAPRECIOS];
            if (obj == null)
                return "{\"existe\":\"false\"}";
            else
                return "{\"existe\":\"true\"}";
        }


        public String Show(Guid idListaPrecios)
        {
            try
            {
                ListaPreciosBL bL = new ListaPreciosBL();

                ListaPrecios obj = bL.GetListaPrecios(idListaPrecios, Logueado.idUsuario);
                this.Session[Constantes.VAR_SESSION_LISTAPRECIOS_VER] = obj;

                return JsonConvert.SerializeObject(new { success = 1, obj = obj });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }


        [HttpGet]
        public ActionResult Editar()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.RegistroListaPrecios;
            Usuario usuario = Logueado;

            if (Logueado == null || !Logueado.modificaMaestroListaPrecios)
            {
                return RedirectToAction("Login", "Account");
            }

            ListaPrecios obj = this.ListaPreciosSession;

            if (obj == null)
            {
                InstanciarListaPrecios();
                obj = this.ListaPreciosSession;
            }

            ViewBag.listaPrecios = obj;

            return View();
        }

        [HttpPost]
        public String AddItem(Guid idProducto)
        {
            try
            {
                ListaPrecios obj = this.ListaPreciosSession;

                if (obj == null)
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "La sesión expiró." });
                }

                if (obj.items.Any(x => x.producto.idProducto == idProducto && x.Estado != 0))
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "El producto ya se encuentra en la lista." });
                }

                ProductoBL productoBL = new ProductoBL();
                Producto producto = productoBL.getProductoById(idProducto);

                ListaPreciosItem item = new ListaPreciosItem();
                item.idListaPreciosItem = Guid.NewGuid(); 
                item.producto = producto;
                item.precio = 0; 
                item.unidad = producto.unidad ?? "";
                item.Estado = 1;

                obj.items.Add(item);
                this.ListaPreciosSession = obj;

                return JsonConvert.SerializeObject(new { success = 1, item = item });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public String ChangeItem(Guid idProducto, decimal precio)
        {
            try
            {
                ListaPrecios obj = this.ListaPreciosSession;

                if (obj == null) return JsonConvert.SerializeObject(new { success = 0, message = "Sesión expirada" });

                var itemEncontrado = obj.items.FirstOrDefault(x => x.producto.idProducto == idProducto);

                if (itemEncontrado != null)
                {
                    itemEncontrado.precio = precio;
                    this.ListaPreciosSession = obj;
                    return JsonConvert.SerializeObject(new { success = 1 });
                }

                return JsonConvert.SerializeObject(new { success = 0, message = "No se encontró el item" });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public String RemoveItem(Guid idProducto)
        {
            try
            {
                ListaPrecios obj = this.ListaPreciosSession;
                if (obj == null) return JsonConvert.SerializeObject(new { success = 0, message = "Sesión expirada" });

                var itemToRemove = obj.items.FirstOrDefault(x => x.producto.idProducto == idProducto);

                if (itemToRemove != null)
                {
                    obj.items.Remove(itemToRemove);
                    this.ListaPreciosSession = obj;
                    return JsonConvert.SerializeObject(new { success = 1 });
                }

                return JsonConvert.SerializeObject(new { success = 0, message = "Item no encontrado." });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public String Guardar(int finalizar = 0)
        {
            /*try
            {*/
                ListaPrecios obj = this.ListaPreciosSession;

                ListaPreciosBL bL = new ListaPreciosBL();
                obj.IdUsuarioRegistro = Logueado.idUsuario;

                obj = bL.GuardarListaPrecios(obj);

                if (finalizar == 1)
                {
                    this.Session[Constantes.VAR_SESSION_LISTAPRECIOS] = null;
                }
                return JsonConvert.SerializeObject(new { success = 1, obj = obj }); 
            /*}
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message }); 
            }*/
        }

        [HttpPost]
        public String Eliminar(Guid idListaPrecios)
        {
            try
            {
                ListaPreciosBL bL = new ListaPreciosBL();

                bL.EliminarListaPrecios(idListaPrecios, Logueado.idUsuario);

                return JsonConvert.SerializeObject(new { success = 1 }); 
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message }); 
            }
        }

        
        public ActionResult CancelarRegistro()
        {
            this.ListaPreciosSession = null;
            return RedirectToAction("List", "ListaPrecios");
        }

        public void ChangeInputForm()
        {
            string tipo = this.Request.Params["tipo"].ToString();

            ListaPrecios obj = this.ListaPreciosSession;
            PropertyInfo propertyInfo = null;

            switch (tipo)
            {
                case "string":
                    propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
                    propertyInfo.SetValue(obj, this.Request.Params["valor"]);
                    break;

                case "int":
                    propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
                    propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
                    break;

                case "date":
                    string fechaParam = Request.Params["valor"].ToString();

                    if (!fechaParam.Trim().Equals(""))
                    {
                        String[] fecha = fechaParam.Split('/');
                        propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
                        propertyInfo.SetValue(obj, new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0])));
                    }
                    break;
            }

            this.ListaPreciosSession = obj;
        }

        
    }
}
