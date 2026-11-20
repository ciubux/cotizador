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
using Model.UTILES;
using NPOI.POIFS.FileSystem;

namespace Cotizador.Controllers
{
    public class PortafolioProductosController : ParentController
    {
        [HttpGet]
        public ActionResult List()
        {
            if (this.Logueado == null || !this.Logueado.modificaPortafolioProductos)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPortafolioProductos;

            return View();
        }

        [HttpPost]
        public String SearchList(string nombre, int estado = 1)
        {
            try
            {
                if (this.Logueado == null || !this.Logueado.modificaPortafolioProductos)
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "Sesión expirada." });
                }

                PortafolioProductosBL bl = new PortafolioProductosBL();
                List<PortafolioProductos> list = bl.getPortafoliosProductos(nombre ?? string.Empty, estado);

                return JsonConvert.SerializeObject(list);
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public String Show(Guid idPortafolioProductos)
        {
            try
            {
                if (this.Logueado == null || !this.Logueado.modificaPortafolioProductos)
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "Sesión expirada." });
                }

                PortafolioProductosBL bl = new PortafolioProductosBL();
                PortafolioProductos obj = bl.getPortafolioProductos(idPortafolioProductos);

                this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS_VER] = obj;

                return JsonConvert.SerializeObject(new { success = 1, obj = obj });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        protected PortafolioProductos InstanciarPortafolio()
        {
            PortafolioProductos obj = new PortafolioProductos();
            obj.idPortafolioProductos = Guid.Empty;
            obj.Estado = 1;

            return obj;
        }


        [HttpPost]
        public String IniciarNuevo()
        {
            PortafolioProductos obj = InstanciarPortafolio();
            this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = obj;

            return JsonConvert.SerializeObject(new { success = 1 });
        }

        [HttpPost]
        public String PrepararAccion(Guid idPortafolioProductos, bool desdeVer = false)
        {
            try
            {
                PortafolioProductosBL bl = new PortafolioProductosBL();

                if(desdeVer)
                {
                    this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS_VER];
                } else
                {
                    PortafolioProductos obj = bl.getPortafolioProductos(idPortafolioProductos);
                    this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = obj;
                }

                

                return JsonConvert.SerializeObject(new { success = 1 });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult Editar()
        {
            if (this.Logueado == null || !this.Logueado.modificaPortafolioProductos)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPortafolioProductos;
            PortafolioProductos obj = (PortafolioProductos)this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS];

            if (obj == null)
            {
                obj = InstanciarPortafolio();
                this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = obj;
            }

            ViewBag.portafolio = obj;
            return View();
        }

        [HttpPost]
        public void ChangeInputForm(string propiedad, string valor)
        {
            PortafolioProductos obj = (PortafolioProductos)this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS];
            if (obj == null) return;

            switch (propiedad)
            {
                case "nombre":
                    obj.nombre = valor?.Trim();
                    break;
                case "comentarios":
                    obj.comentarios = valor?.Trim();
                    break;
                case "Estado":
                    int est = 1;
                    int.TryParse(valor, out est);
                    obj.Estado = est;
                    break;
            }

            this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = obj;
        }

        [HttpPost]
        public String AddItem(Guid idProducto)
        {
            try
            {
                PortafolioProductos obj = (PortafolioProductos)this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS];
                if (obj == null)
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "La sesión expiró." });
                }

                // Validar si el producto ya se encuentra en el portafolio
                if (obj.items.Any(x => x.producto.idProducto == idProducto && x.Estado != 0))
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "El producto ya se encuentra en el portafolio." });
                }

                ProductoBL productoBL = new ProductoBL();
                Producto producto = productoBL.getProductoById(idProducto);

                if (producto == null || producto.idProducto == Guid.Empty)
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "No se encontró el producto seleccionado." });
                }

                PortafolioProductosItem item = new PortafolioProductosItem();
                item.idPortafolioProductosItem = Guid.NewGuid();
                item.producto = producto;
                item.Estado = 1;

                obj.items.Add(item);
                this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = obj;

                return JsonConvert.SerializeObject(new { success = 1, item = item });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public String AddItemsMasivo(List<string> skus)
        {
            try
            {
                PortafolioProductos obj = (PortafolioProductos)this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS];
                if (obj == null)
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "La sesión expiró." });
                }

                if (skus == null || skus.Count == 0)
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "No se enviaron códigos SKU." });
                }

                ProductoBL productoBL = new ProductoBL();
                List<Producto> productosEncontrados = productoBL.GetProductosBySKU(skus);

                List<PortafolioProductosItem> nuevosItems = new List<PortafolioProductosItem>();
                int agregados = 0;

                foreach (Producto prod in productosEncontrados)
                {
                    if (!obj.items.Any(x => x.producto.idProducto == prod.idProducto && x.Estado != 0))
                    {
                        PortafolioProductosItem item = new PortafolioProductosItem();
                        item.idPortafolioProductosItem = Guid.NewGuid();
                        item.producto = prod;
                        item.Estado = 1;

                        obj.items.Add(item);
                        nuevosItems.Add(item);
                        agregados++;
                    }
                }

                this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = obj;

                return JsonConvert.SerializeObject(new
                {
                    success = 1,
                    agregados = agregados,
                    items = nuevosItems
                });
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
                PortafolioProductos obj = (PortafolioProductos)this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS];
                if (obj == null)
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "La sesión expiró." });
                }

                var itemToRemove = obj.items.FirstOrDefault(x => x.producto.idProducto == idProducto);
                if (itemToRemove != null)
                {
                    obj.items.Remove(itemToRemove);
                    this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = obj;
                    return JsonConvert.SerializeObject(new { success = 1 });
                }

                return JsonConvert.SerializeObject(new { success = 0, message = "Ítem no encontrado." });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public String Guardar(int finalizar = 0)
        {
            try
            {
                if (this.Logueado == null || !this.Logueado.modificaPortafolioProductos)
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "La sesión expiró." });
                }

                PortafolioProductos obj = (PortafolioProductos)this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS];
                if (obj == null)
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "No hay un portafolio activo en sesión." });
                }

                if (string.IsNullOrWhiteSpace(obj.nombre))
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "Debe ingresar el nombre del portafolio." });
                }

                List<string> skus = obj.items
                    .Where(x => x.Estado != 0 && x.producto != null && !string.IsNullOrWhiteSpace(x.producto.sku))
                    .Select(x => x.producto.sku.Trim())
                    .Distinct()
                    .ToList();

                PortafolioProductosBL bl = new PortafolioProductosBL();
                obj = bl.insertOrUpdatePortafolioProductos(obj, skus, this.Logueado.idUsuario);

                if (finalizar == 1)
                {
                    this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = null;
                }
                else
                {
                    this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = obj;
                }

                return JsonConvert.SerializeObject(new { success = 1, obj = obj });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        [HttpPost]
        public String CancelarEdicion()
        {
            this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = null;
            return JsonConvert.SerializeObject(new { success = 1 });
        }

        [HttpGet]
        public ActionResult Usuarios()
        {
            if (this.Logueado == null || !this.Logueado.modificaPortafolioProductos)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPortafolioProductos;
            PortafolioProductos obj = (PortafolioProductos)this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS];

            if (obj == null || obj.idPortafolioProductos == Guid.Empty)
            {
                return RedirectToAction("List");
            }

            ViewBag.portafolio = obj;
            return View();
        }

        [HttpPost]
        public String GuardarUsuarios(Guid idPortafolioProductos, List<Guid> idsUsuarios)
        {
            try
            {
                if (this.Logueado == null || !this.Logueado.modificaPortafolioProductos)
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "La sesión expiró." });
                }

                PortafolioProductosBL bl = new PortafolioProductosBL();
                bl.updatePortafolioProductosUsuarios(
                    idPortafolioProductos,
                    this.Logueado.idUsuario,
                    idsUsuarios ?? new List<Guid>()
                );

                this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = null;

                return JsonConvert.SerializeObject(new { success = 1 });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult Roles()
        {
            if (this.Logueado == null || !this.Logueado.modificaPortafolioProductos)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPortafolioProductos;
            PortafolioProductos obj = (PortafolioProductos)this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS];

            if (obj == null || obj.idPortafolioProductos == Guid.Empty)
            {
                return RedirectToAction("List");
            }

            RolBL rolbl = new RolBL();
            Rol rol = new Rol();
            rol.Estado = 1;
            List<Rol> roles = rolbl.getRoles(rol);

            ViewBag.portafolio = obj;
            ViewBag.roles = roles;
            return View();
        }

        [HttpPost]
        public String GuardarRoles(Guid idPortafolioProductos, List<int> idsRoles)
        {
            try
            {
                if (this.Logueado == null || !this.Logueado.modificaPortafolioProductos)
                {
                    return JsonConvert.SerializeObject(new { success = 0, message = "La sesión expiró." });
                }

                PortafolioProductosBL bl = new PortafolioProductosBL();
                bl.updatePortafolioProductosRoles(
                    idPortafolioProductos,
                    this.Logueado.idUsuario,
                    idsRoles ?? new List<int>()
                );

                this.Session[Constantes.VAR_SESSION_PORTAFOLIO_PRODUCTOS] = null;

                return JsonConvert.SerializeObject(new { success = 1 });
            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = ex.Message });
            }
        }
    }
}