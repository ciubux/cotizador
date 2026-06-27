using BusinessLayer;
using Cotizador.ExcelExport;
using Cotizador.Models;
using Cotizador.Models.OBJsFiltro;
using DataLayer;
using Model;
using Model.UTILES;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class ProductoControlStockController : ParentController
    {
        [HttpGet]
        public ActionResult ControlAlertaStock()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ControlStock;

            if (this.Logueado == null || !this.Logueado.visualizaControlStock)
            {
                return RedirectToAction("Login", "Account");
            }

            List<ProductoControlStock> lista = new List<ProductoControlStock>();

            ProductoControlStockBL bl = new ProductoControlStockBL();

            ControlStockFiltro filtro = (ControlStockFiltro)this.Session["s_controlStockFiltro"];
            if (filtro == null)
            {
                filtro = instanciarFiltroControlStock();
                this.Session["s_controlStockFiltro"] = filtro;
            }

            lista = bl.SelectProductosControlStock(Logueado.idUsuario, 1, filtro.idCiudad, filtro.sku, filtro.proveedor, filtro.familia, filtro.stockVerde, filtro.stockAmbar, filtro.stockRojo);

            ParametroBL parametroBL = new ParametroBL();
            int diasConsiderarPedidosAnt = int.Parse(parametroBL.getParametro("STOCK_DIAS_PEDIDOS_ENTREGA_VENCIDA"));
            int diasConsiderarPedidosPost = int.Parse(parametroBL.getParametro("STOCK_DIAS_PEDIDOS_ENTREGA_PENDIENTE"));

            ViewBag.pagina = (int)Constantes.paginas.ControlStock;
            ViewBag.lista = lista;
            ViewBag.filtro = filtro;
            ViewBag.usuario = this.Logueado;
            ViewBag.diasConsiderarPedidosAnt = diasConsiderarPedidosAnt;
            ViewBag.diasConsiderarPedidosPost = diasConsiderarPedidosPost;

            List<object> listaExport = new List<object>();

            foreach (ProductoControlStock item in lista)
            {
                listaExport.Add(new
                {
                    sku = item.producto.sku,
                    producto = item.producto.descripcion,
                    unidad = item.unidadPresentacion,
                    //cantMin = String.Format(Constantes.formatoDosDecimales, item.cantidadMinimaPresentacion),
                    //cantMax = String.Format(Constantes.formatoDosDecimales, item.cantidadMaximaPresentacion),
                    //cantAle = String.Format(Constantes.formatoDosDecimales, item.cantidadAlertaPresentacion),
                    cantMin = item.cantidadMinimaPresentacion,
                    cantMax = item.cantidadMaximaPresentacion,
                    cantAle = item.cantidadAlertaPresentacion,
                    fechaStock = item.FechaCsDesc,
                    //stockReal = String.Format(Constantes.formatoDosDecimales, item.cantidadPresentacion),
                    //stockVirtual = String.Format(Constantes.formatoDosDecimales, item.cantidadVirtualPresentacion),
                    //sugeridoPedir = String.Format(Constantes.formatoDosDecimales, item.cantidadSugeridaPedirPresentacion),
                    stockReal = item.cantidadPresentacion,
                    stockVirtual = item.cantidadVirtualPresentacion,
                    sugeridoPedir = item.cantidadSugeridaPedirPresentacion,
                    estadoStockReal = item.estadoStockReal,
                    estadoStockVirtual = item.estadoStockVirtual
                });
            }

            ViewBag.jsonLista = JsonConvert.SerializeObject(listaExport);

            return View();
        }

        [HttpPost]
        public string CalcularStock()
        {
            int success = 0;

            ProductoControlStockBL bl = new ProductoControlStockBL();
            //bl.UpdateAjusteEstadoAprobado(obj);

            return JsonConvert.SerializeObject(new
            {
                success = success
            });
        }

        [HttpPost]
        public string RegistroMasivoAlertas(Guid idCiudad, List<string> skus, List<int> cMins, List<int> cMaxs, List<int> cAlers)
        {
            if (this.Logueado == null || !this.Logueado.visualizaControlStock)
            {
                return JsonConvert.SerializeObject(new { success = 0, message = "No autorizado o sesión expirada" });
            }

            
            ProductoBL blProducto = new ProductoBL();
            List<Producto> productos = blProducto.GetProductosBySKU(skus);

            List<ProductoControlStock> alertas = new List<ProductoControlStock>();

            for (int i = 0; i < skus.Count; i++)
            {
                Producto prod = productos.Where(p => p.sku.Equals(skus.ElementAt(i))).FirstOrDefault();
                if (prod != null)
                {
                    ProductoControlStock item = new ProductoControlStock();
                    item.producto = prod;
                    item.cantidadMaxima = cMaxs.ElementAt(i);
                    item.cantidadMinima = cMins.ElementAt(i);
                    item.cantidadAlerta = cAlers.ElementAt(i);

                    alertas.Add(item);
                }
            }
            ProductoControlStockBL bl = new ProductoControlStockBL();
            bl.InsertProductosControlStock(this.Logueado.idUsuario, idCiudad, alertas);
            
            return JsonConvert.SerializeObject(new
            {
                success = 1
            });
        }


        public ControlStockFiltro instanciarFiltroControlStock()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            ControlStockFiltro obj = new ControlStockFiltro();
            obj.proveedor = "Todos";
            obj.familia = "Todas";
            this.Session["proveedor"] = obj.proveedor;
            this.Session["familia"] = obj.familia;

            obj.sku = string.Empty;

            if (this.Logueado != null)
            {
                obj.idCiudad = this.Logueado.sedeMP.idCiudad;
                obj.ciudad = this.Logueado.sedeMP;
            }
            else {
                obj.idCiudad = Guid.Empty;
                obj.ciudad = new Ciudad();
                obj.ciudad.idCiudad = Guid.Empty;
                obj.ciudad.nombre = "TODOS"; 
            }            

            obj.stockVerde = true;
            obj.stockAmbar = true;
            obj.stockRojo = true;

            return obj;
        }

        [HttpPost]
        public string ActualizarControlStock(string idCiudad, List<Guid> controlStockIds)
        {
            int success = 0;

            Guid ciudadId = Guid.Empty;

            if (!idCiudad.Equals(string.Empty)) { ciudadId = Guid.Parse(idCiudad); }

            ProductoControlStockBL bl = new ProductoControlStockBL();

            if (controlStockIds == null) { controlStockIds = new List<Guid>(); }

            bl.CalcularControlStock(Logueado.idUsuario, ciudadId, controlStockIds);
            success = 1;

            return JsonConvert.SerializeObject(new
            {
                success = success
            });
        }


        public ActionResult LimpiarFiltroControlStock()
        {
            ControlStockFiltro obj = (ControlStockFiltro)this.Session["s_controlStockFiltro"];

            obj = instanciarFiltroControlStock();
            this.Session["s_controlStockFiltro"] = obj;

            return RedirectToAction("ControlAlertaStock", "ProductoControlStock");
        }

        public void changeFiltroControlStock(string propiedad, string valor, string tipo)
        {
            ControlStockFiltro obj = (ControlStockFiltro)this.Session["s_controlStockFiltro"];

            obj.changeDatoParametro(propiedad, valor, tipo);
            
            if (propiedad.Equals("proveedor"))
            {
                this.Session["proveedor"] = valor;
            }

            if (propiedad.Equals("familia"))
            {
                this.Session["familia"] = valor;
            }

            /*
            if (propiedad.Equals("idCiudad") && obj.idCiudad != null && !obj.idCiudad.Equals(Guid.Empty))
            {
                CiudadBL blCiudad = new CiudadBL();
                obj.ciudad = blCiudad.getCiudad(obj.idCiudad);
            }
            else
            {
                if (propiedad.Equals("idCiudad"))
                {
                    obj.ciudad = new Ciudad();
                    obj.ciudad.idCiudad = Guid.Empty;
                    obj.ciudad.nombre = "TODOS";
                }
            }*/

            this.Session["s_controlStockFiltro"] = obj;
        }
    }
}