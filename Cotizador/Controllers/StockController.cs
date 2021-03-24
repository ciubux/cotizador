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
    public class StockController : Controller
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
                instanciarProductoBusqueda();
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

        private Producto ProductoBusquedaSession
        {
            get
            {
                Producto producto = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.DescargaPlantillMasivaStock: producto = (Producto)this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_BUSQUEDA]; break;
                }
                return producto;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.DescargaPlantillMasivaStock: this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_BUSQUEDA] = value; break;
                }
            }
        }

        private void instanciarProductoBusqueda()
        {
            Producto producto = new Producto();
            producto.idProducto = Guid.Empty;
            producto.sku = String.Empty;
            producto.skuProveedor = String.Empty;
            producto.descripcion = String.Empty;
            producto.familia = "Todas";
            producto.proveedor = "Todos";
            producto.Estado = 1;
            producto.tipoProductoVista = 0;
            producto.tipoVentaRestringidaBusqueda = -1;
            producto.descontinuado = -1;
            producto.ConImagen = -1;

            this.Session["familia"] = "Todas";
            this.Session["proveedor"] = "Todos";

            this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_BUSQUEDA] = producto;
        }


        public ActionResult PlantillaStock(string grupoClienteSelectId, string selectedValue = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.DescargaPlantillMasivaStock;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            //if (!usuario.visualizaGrupoClientes && !usuario.esVendedor)
            //{
            //    return RedirectToAction("Login", "Account");
            //}

            if (this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_BUSQUEDA] == null)
            {
                instanciarProductoBusqueda();
            }



            ViewBag.producto = this.ProductoBusquedaSession;
            return View();
        }


        [HttpGet]
        public ActionResult ExportPlantillaStock()
        {
            Producto obj = (Producto)this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_BUSQUEDA];
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            obj.familia = this.Session["familia"].ToString();
            obj.proveedor = this.Session["proveedor"].ToString();

            PlantillaCargaStock excel = new PlantillaCargaStock();
            ProductoBL bl = new ProductoBL();
            List<Producto> lista = bl.getProductosPlantillaStock(obj);
            

            return excel.generateExcel(lista, usuario);
        }

        public void CleanBusquedaProducto()
        {
            instanciarProductoBusqueda();
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


        public void ChangeInputString()
        {
            Producto producto = (Producto)this.ProductoBusquedaSession;
            PropertyInfo propertyInfo = producto.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(producto, this.Request.Params["valor"]);
            this.ProductoBusquedaSession = producto;
        }

        public void ChangeInputInt()
        {
            Producto producto = (Producto)this.ProductoBusquedaSession;
            PropertyInfo propertyInfo = producto.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(producto, Int32.Parse(this.Request.Params["valor"]));
            this.ProductoBusquedaSession = producto;
        }

        public void ChangeInputDecimal()
        {
            Producto producto = (Producto)this.ProductoBusquedaSession;
            PropertyInfo propertyInfo = producto.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(producto, Decimal.Parse(this.Request.Params["valor"]));
            this.ProductoBusquedaSession = producto;
        }
        public void ChangeInputBoolean()
        {
            Producto producto = (Producto)this.ProductoBusquedaSession;
            PropertyInfo propertyInfo = producto.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(producto, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.ProductoBusquedaSession = producto;
        }

        public void ChangeVentaRestringida()
        {
            int valor = Int32.Parse(this.Request.Params["ventaRestringida"]);
            if (valor > -1)
            {
                ProductoBusquedaSession.ventaRestringida = (Producto.TipoVentaRestringida)valor;
            }
        }

    }
}