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

        public ActionResult LimpiarFiltroSellOutVendedores()
        {
            ReporteSellOutVendedoresFiltro obj = (ReporteSellOutVendedoresFiltro)this.Session["s_rSellOutVendedoresFiltro"];

            obj = instanciarFiltroSellOutVendedores();
            this.Session["s_rSellOutVendedoresFiltro"] = obj;

            return RedirectToAction("SellOutVendedores", "Reporte");
        }

        public void changeFiltroSellOutVendedores(string propiedad, string valor, string tipo)
        {
            ReporteSellOutVendedoresFiltro obj = (ReporteSellOutVendedoresFiltro)this.Session["s_rSellOutVendedoresFiltro"];

            obj.changeDatoParametro(propiedad, valor, tipo);

            if (propiedad.Equals("idCiudad") && obj.idCiudad != null && !obj.idCiudad.Equals(Guid.Empty))
            {
                CiudadBL blCiudad = new CiudadBL();
                obj.ciudad = blCiudad.getCiudad(obj.idCiudad);
            } else {
                if (propiedad.Equals("idCiudad")) { 
                    obj.ciudad = new Ciudad();
                    obj.ciudad.idCiudad = Guid.Empty;
                    obj.ciudad.nombre = "TODOS";
                } 
            }

            this.Session["s_rSellOutVendedoresFiltro"] = obj;
        }

        public ReporteSellOutVendedoresFiltro instanciarFiltroSellOutVendedores()
        {
            ReporteSellOutVendedoresFiltro obj = new ReporteSellOutVendedoresFiltro();
            obj.proveedor = "Todos";
            obj.familia = "Todas";

            ParametroBL parametroBL = new ParametroBL();

            obj.fechaInicio = DateTime.Now;
            obj.fechaFin = DateTime.Now;
            obj.sku = string.Empty;
            obj.idCiudad = Guid.Empty;
            obj.ciudad = new Ciudad();
            obj.ciudad.idCiudad = Guid.Empty;
            obj.ciudad.nombre = "TODOS";
            obj.anio = 0;
            obj.trimestre = 0;

            return obj;
        }

        public ActionResult SellOutVendedores()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ReporteSellOutVendedores;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.visualizaReporteSellOutVendedores)
            {
                return RedirectToAction("Login", "Account");
            }

            ReporteSellOutVendedoresFiltro obj = (ReporteSellOutVendedoresFiltro)this.Session["s_rSellOutVendedoresFiltro"];
            if (obj == null)
            {
                obj = instanciarFiltroSellOutVendedores();
                this.Session["s_rSellOutVendedoresFiltro"] = obj;
            }

            ReporteBL bl = new ReporteBL();
            List<List<String>> resultados = new List<List<String>>();

            if (Request.HttpMethod.Equals("POST"))
            {
                Vendedor responsableComercial = usuario.vendedorList.Where(v => v.idVendedor == obj.idResponsableComercial).FirstOrDefault();
                Vendedor supervisorComercial = usuario.supervisorComercialList.Where(v => v.idVendedor == obj.idSupervisorComercial).FirstOrDefault();
                Vendedor asistenteServicioCliente = usuario.asistenteServicioClienteList.Where(v => v.idVendedor == obj.idAsistenteComercial).FirstOrDefault();
                //Usuario us = usuario.usuarioTomaPedidoList.Where(u => u.idUsuario == obj.idUsuarioCreador).FirstOrDefault();

                

                resultados = bl.sellOutVendedores(obj.sku, obj.familia, obj.proveedor, 
                    responsableComercial != null ? responsableComercial.codigo : "",
                    supervisorComercial != null ? supervisorComercial.codigo : "",
                    asistenteServicioCliente != null ? asistenteServicioCliente.codigo : "",
                    obj.fechaInicio, obj.fechaFin, obj.anio, obj.trimestre, obj.ciudad.nombre, 
                    obj.incluirVentasExcluidas, usuario.idUsuario);
                this.Session["s_rSellOutVendedoresFiltroLastF"] = obj;
                this.Session["s_rSellOutVendedoresFiltroLastS"] = resultados;
            }
            
            ViewBag.filtros = obj;
            ViewBag.resultados = resultados;
            ViewBag.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            ViewBag.usuarios = usuario.usuarioTomaPedidoList;

            return View();
        }

        [HttpGet]
        public ActionResult ExportDetallesReporteSellOutVendedor(String codigoVendedor)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.visualizaReporteProductosPendientesAtencion)
            {
                return RedirectToAction("Login", "Account");
            }

            ReporteBL bl = new ReporteBL();

            ReporteSellOutVendedoresFiltro obj = (ReporteSellOutVendedoresFiltro) this.Session["s_rSellOutVendedoresFiltroLastF"];

            Vendedor responsableComercial = usuario.vendedorList.Where(v => v.idVendedor == obj.idResponsableComercial).FirstOrDefault();
            Vendedor supervisorComercial = usuario.supervisorComercialList.Where(v => v.idVendedor == obj.idSupervisorComercial).FirstOrDefault();
            Vendedor asistenteServicioCliente = usuario.asistenteServicioClienteList.Where(v => v.idVendedor == obj.idAsistenteComercial).FirstOrDefault();

            List<List<String>> resultados = bl.sellOutVendedoresDetalles(codigoVendedor, obj.sku, obj.familia, obj.proveedor,
                    responsableComercial != null ? responsableComercial.codigo : "",
                    supervisorComercial != null ? supervisorComercial.codigo : "",
                    asistenteServicioCliente != null ? asistenteServicioCliente.codigo : "", 
                    obj.fechaInicio, obj.fechaFin, obj.anio, obj.trimestre, obj.ciudad.nombre, 
                    obj.incluirVentasExcluidas, usuario.idUsuario);

            ReporteDetallesSellOutVendedor excel = new ReporteDetallesSellOutVendedor();

            return excel.generateExcel(resultados, obj, usuario);
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
            obj.ciudad = null;
            obj.idProductoPresentacion = 0;

            return obj;
        }

        public ActionResult ProductosPendientesAtencion()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ReporteProductosPendientesAtencion;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.visualizaReporteProductosPendientesAtencion)
            {
                return RedirectToAction("Login", "Account");
            }

            ReportePendientesAtencionFiltro obj = (ReportePendientesAtencionFiltro) this.Session["s_rProductosPendientesAtencionFiltro"];
            if (obj == null)
            {
                obj = instanciarFiltroProductosPendienteAtencion();
                this.Session["s_rProductosPendientesAtencionFiltro"] = obj;
            }

            ReporteBL bl = new ReporteBL();
            List<FilaProductoPendienteAtencion> resultados = new List<FilaProductoPendienteAtencion>();

            if(Request.HttpMethod.Equals("POST"))
            {
                resultados = bl.productosPendientesAtencion(obj.sku, obj.familia, obj.proveedor, obj.fechaEntregaInicio, obj.fechaEntregaFin, obj.idCiudad, usuario.idUsuario);
                this.Session["s_rProductosPendientesAtencionLastF"] = obj;
                this.Session["s_rProductosPendientesAtencionLastS"] = resultados;

            }
            
            ViewBag.filtros = obj;
            ViewBag.resultados = resultados;
            ViewBag.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            //ViewBag.producto = this.ProductoBusquedaSession;
            return View();
        }

        [HttpGet]
        public ActionResult ExportReporteProductosPendientesAtencion()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.visualizaReporteProductosPendientesAtencion)
            {
                return RedirectToAction("Login", "Account");
            }

            ReportePendientesAtencionFiltro obj = (ReportePendientesAtencionFiltro) this.Session["s_rProductosPendientesAtencionLastF"];
            List<FilaProductoPendienteAtencion> resultados = (List<FilaProductoPendienteAtencion>) this.Session["s_rProductosPendientesAtencionLastS"];

            ReporteProdPA excel = new ReporteProdPA();

            return excel.generateExcel(resultados, obj, usuario);
        }

        public void changeFiltroProductosPendienteAtencion(string propiedad, string valor, string tipo)
        {

            ReportePendientesAtencionFiltro obj = (ReportePendientesAtencionFiltro)this.Session["s_rProductosPendientesAtencionFiltro"];

            obj.changeDatoParametro(propiedad, valor, tipo);

            if (propiedad.Equals("idCiudad") && obj.idCiudad != null && !obj.idCiudad.Equals(Guid.Empty))
            {
                CiudadBL blCiudad = new CiudadBL();
                obj.ciudad = blCiudad.getCiudad(obj.idCiudad);
            } else { obj.ciudad = propiedad.Equals("idCiudad") ? null : obj.ciudad; }

            this.Session["s_rProductosPendientesAtencionFiltro"] = obj;
        }

        public ActionResult LimpiarFiltroProductosPendientes()
        {
            ReportePendientesAtencionFiltro obj = (ReportePendientesAtencionFiltro)this.Session["s_rProductosPendientesAtencionFiltro"];
           
            obj = instanciarFiltroProductosPendienteAtencion();
            this.Session["s_rProductosPendientesAtencionFiltro"] = obj;
            
            return RedirectToAction("ProductosPendientesAtencion", "Reporte");
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
            string txtIdSede = parametroBL.getParametro("PARAM_SELLOUT_PERSONALIZADO_SEDE");
            
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
            ViewBag.idSede = txtIdSede;

            //ViewBag.producto = this.ProductoBusquedaSession;
            return View();
        }

        [HttpPost]
        public String ActualizarPametrosSellOutPersonalizado(String sku, String proveedor, String fechaInicio, String fechaFin, String[] rucs, String[] codigosCliente, String[] codigosGrupoCliente, String idSede)
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
                parametroBL.updateParametro("PARAM_SELLOUT_PERSONALIZADO_SEDE", idSede);
            }

            // GET PARAMETROS

            //ViewBag.producto = this.ProductoBusquedaSession;
            return "";
        }



        //[HttpGet]
        //public ActionResult ExportPlantillaStock()
        //{
        //    Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

        //    if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.realizaCargaMasivaStock)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    Producto obj = (Producto)this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_BUSQUEDA];
           
        //    obj.familia = this.Session["familia"].ToString();
        //    obj.proveedor = this.Session["proveedor"].ToString();

        //    PlantillaCargaStock excel = new PlantillaCargaStock();
        //    ProductoBL bl = new ProductoBL();
        //    List<Producto> lista = bl.getProductosPlantillaStock(obj);
            

        //    return excel.generateExcel(lista, usuario);
        //}



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