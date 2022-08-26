using BusinessLayer;
using Cotizador.Models;
using Cotizador.Models.OBJsFiltro;
using Model;
using Model.UTILES;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Cotizador.ExcelExport;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace Cotizador.Controllers
{
    public class ReporteController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaGrupoClientes;


           
            return View();

        }

        public ReportePendientesAtencionFiltro instanciarFiltroProductosPendienteAtencion ()
        {
            ReportePendientesAtencionFiltro obj = new ReportePendientesAtencionFiltro();
            obj.proveedor = "Todos";
            obj.familia = "Todas";

            ParametroBL parametroBL = new ParametroBL();
            int diasPasado = int.Parse(parametroBL.getParametro("STOCK_DIAS_PEDIDOS_ENTREGA_VENCIDA"));
            int diasFuturo = int.Parse(parametroBL.getParametro("STOCK_DIAS_PEDIDOS_ENTREGA_PENDIENTE"));

            obj.fechaEntregaInicio = DateTime.Now.AddDays(-1 * diasPasado);
            obj.fechaEntregaFin = DateTime.Now.AddDays(diasFuturo);
            obj.sku = string.Empty;
            obj.descripcion = string.Empty;
            obj.idCiudad = Guid.Empty;

            return obj;
        }
        public ActionResult ProductosPendientesAtencion()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ReporteProductosPendientesAtencion;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.visualizaReporteSellOutPersonalizado)
            {
                return RedirectToAction("Login", "Account");
            }

            ReportePendientesAtencionFiltro obj = (ReportePendientesAtencionFiltro) this.Session["s_rProductosPendientesAtencion"];
            if (obj == null)
            {
                obj = instanciarFiltroProductosPendienteAtencion();
                this.Session["s_rProductosPendientesAtencion"] = obj;
            }

            ViewBag.filtros = obj;
            
            //ViewBag.producto = this.ProductoBusquedaSession;
            return View();
        }

        public void changeFiltroProductosPendienteAtencion(string propiedad, string valor, string tipo)
        {
            ReportePendientesAtencionFiltro obj = (ReportePendientesAtencionFiltro)this.Session["s_rProductosPendientesAtencion"];
            
            PropertyInfo propertyInfo = obj.GetType().GetProperty(propiedad);

            switch (tipo)
            {
                case "string":
                    propertyInfo.SetValue(obj, valor);
                    break;
                case "date":
                    if (!valor.Trim().Equals(""))
                    {
                        String[] fecha = valor.Split('/');
                        propertyInfo.SetValue(obj, new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0])));
                    }
                    break;
                case "guid":
                    if (!valor.Trim().Equals(""))
                    {
                        propertyInfo.SetValue(obj, Guid.Parse(valor));
                    }
                    break;
            }

            this.Session["s_rProductosPendientesAtencion"] = obj;
        }

        public ActionResult SellOutPersonalizado()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ReporteSellOutPersonalizado;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.visualizaReporteSellOutPersonalizado)
            {
                return RedirectToAction("Login", "Account");
            }

            // GET PARAMETROS
            ParametroBL parametroBL = new ParametroBL();
            ViewBag.sku = parametroBL.getParametro("PARAM_SELLOUT_PERSONALIZADO_SKU");
            string proveedor = parametroBL.getParametro("PARAM_SELLOUT_PERSONALIZADO_PROVEEDOR");

            this.Session["proveedor"] = proveedor == "" ? "Todos" : proveedor;

            string fechaInicio = parametroBL.getParametro("PARAM_SELLOUT_PERSONALIZADO_FECHA_INICIO");
            string fechaFin = parametroBL.getParametro("PARAM_SELLOUT_PERSONALIZADO_FECHA_FIN");
            fechaInicio = fechaInicio == null ? "" : fechaInicio;
            fechaFin = fechaFin == null ? "" : fechaFin;
            string txtRucs = parametroBL.getParametro("PARAM_SELLOUT_PERSONALIZADO_RUCS");
            string txtCodigosCliente = parametroBL.getParametro("PARAM_SELLOUT_PERSONALIZADO_CODIGOS_CLIENTE");
            string txtCodigosGrupoCliente = parametroBL.getParametro("PARAM_SELLOUT_PERSONALIZADO_CODIGOS_GRUPO_CLIENTE");

            txtRucs = txtRucs == null ? "" : txtRucs;
            txtCodigosCliente = txtCodigosCliente == null ? "" : txtCodigosCliente;
            txtCodigosGrupoCliente = txtCodigosGrupoCliente == null ? "" : txtCodigosGrupoCliente;

            String[] fiv = fechaInicio.Split('-');
            if (fiv.Length == 3)
            {
                fechaInicio = fiv[2] + "/" + fiv[1] + "/" + fiv[0];
            } else
            {
                fechaInicio = "";
            }

            String[] ffv = fechaFin.Split('-');
            if (ffv.Length == 3)
            {
                fechaFin = ffv[2] + "/" + ffv[1] + "/" + ffv[0];
            }
            else
            {
                fechaFin = "";
            }

            ViewBag.fechaInicio = fechaInicio;
            ViewBag.fechaFin = fechaFin;
            ViewBag.rucs = txtRucs.Split(';');
            ViewBag.codigosCliente = txtCodigosCliente.Split(';');
            ViewBag.codigosGrupoCliente = txtCodigosGrupoCliente.Split(';');

            //ViewBag.producto = this.ProductoBusquedaSession;
            return View();
        }

        [HttpPost]
        public String ActualizarPametrosSellOutPersonalizado(String sku, String proveedor, String fechaInicio, String fechaFin, String[] rucs, String[] codigosCliente, String[] codigosGrupoCliente)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ReporteSellOutPersonalizado;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (usuario != null && usuario.visualizaReporteSellOutPersonalizado)
            {
                string txtRucs = rucs != null ? String.Join(";", rucs) : "";
                string txtCodigosCliente = codigosCliente != null ? String.Join(";", codigosCliente) : "";
                string txtCodigosGrupoCliente = codigosGrupoCliente != null ?  String.Join(";", codigosGrupoCliente) : "";

                String[] fiv = fechaInicio.Split('/');
                if (fiv.Length == 3)
                {
                    fechaInicio = fiv[2] + "-" + fiv[1] + "-" + fiv[0];
                } else
                {
                    fechaInicio = "";
                }

                String[] ffv = fechaFin.Split('/');
                if (ffv.Length == 3)
                {
                    fechaFin = ffv[2] + "-" + ffv[1] + "-" + ffv[0];
                } else
                {
                    fechaFin = "";
                }


                ParametroBL parametroBL = new ParametroBL();
                parametroBL.updateParametro("PARAM_SELLOUT_PERSONALIZADO_FECHA_INICIO", fechaInicio);
                parametroBL.updateParametro("PARAM_SELLOUT_PERSONALIZADO_FECHA_FIN", fechaFin);
                parametroBL.updateParametro("PARAM_SELLOUT_PERSONALIZADO_SKU", sku);
                parametroBL.updateParametro("PARAM_SELLOUT_PERSONALIZADO_RUCS", txtRucs);
                parametroBL.updateParametro("PARAM_SELLOUT_PERSONALIZADO_CODIGOS_CLIENTE", txtCodigosCliente);
                parametroBL.updateParametro("PARAM_SELLOUT_PERSONALIZADO_CODIGOS_GRUPO_CLIENTE", txtCodigosGrupoCliente);
                parametroBL.updateParametro("PARAM_SELLOUT_PERSONALIZADO_PROVEEDOR", proveedor);
            }

            // GET PARAMETROS

            //ViewBag.producto = this.ProductoBusquedaSession;
            return "";
        }



        [HttpGet]
        public ActionResult ExportPlantillaStock()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.realizaCargaMasivaStock)
            {
                return RedirectToAction("Login", "Account");
            }

            Producto obj = (Producto)this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_BUSQUEDA];
           
            obj.familia = this.Session["familia"].ToString();
            obj.proveedor = this.Session["proveedor"].ToString();

            PlantillaCargaStock excel = new PlantillaCargaStock();
            ProductoBL bl = new ProductoBL();
            List<Producto> lista = bl.getProductosPlantillaStock(obj);
            

            return excel.generateExcel(lista, usuario);
        }



        public ActionResult ReporteStock(string grupoClienteSelectId, string selectedValue = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ReporteStock;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.visualizaReporteGlobalStock)
            {
                return RedirectToAction("Login", "Account");
            }


            return View();
        }

        
    }
}