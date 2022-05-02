using BusinessLayer;
using Cotizador.Models;
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
using System.IO;
using Cotizador.Models.DTOshow;
using Cotizador.Models.DTOsShow;

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
                    case Constantes.paginas.ReporteStock: producto = (Producto)this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_REPORTE]; break;
                }
                return producto;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.DescargaPlantillMasivaStock: this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_BUSQUEDA] = value; break;
                    case Constantes.paginas.ReporteStock: this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_REPORTE] = value; break;
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

            this.ProductoBusquedaSession = producto;
        }

        [HttpGet]
        public ActionResult CargaMasivaStock()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.CargaMasivaStock;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.realizaCargaMasivaStock)
            {
                return RedirectToAction("Login", "Account");
            }


            return View();

        }

        public ActionResult PlantillaStock(string grupoClienteSelectId, string selectedValue = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.DescargaPlantillMasivaStock;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.realizaCargaMasivaStock)
            {
                return RedirectToAction("Login", "Account");
            }

            if (this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_BUSQUEDA] == null)
            {
                instanciarProductoBusqueda();
            }


            ViewBag.pagina = (int)this.Session[Constantes.VAR_SESSION_PAGINA];
            ViewBag.producto = this.ProductoBusquedaSession;
            return View();
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

            if (this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_REPORTE] == null)
            {
                instanciarProductoBusqueda();
            }

            ViewBag.pagina = (int) this.Session[Constantes.VAR_SESSION_PAGINA];
            ViewBag.producto = this.ProductoBusquedaSession;

            return View();
        }

        [HttpGet]
        public ActionResult ExportReporteStock()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Guid idSede = Guid.Parse(this.Request.Params["idCiudad"].ToString());
            int ajusteMercaderiaTransito = int.Parse(this.Request.Params["ajusteMercaderiaTransito"].ToString());
            if (idSede == null) idSede = Guid.Empty;

            int tipoUnidad = int.Parse(this.Request.Params["tipoUnidad"].ToString());
            String[] fiv = this.Request.Params["fechaStock"].Split('/');
            DateTime fechaStock = new DateTime(Int32.Parse(fiv[2]), Int32.Parse(fiv[1]), Int32.Parse(fiv[0]));
            
            
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.visualizaReporteGlobalStock)
            {
                return RedirectToAction("Login", "Account");
            }

            Producto obj = (Producto)this.Session[Constantes.VAR_SESSION_STOCK_PRODUCTO_REPORTE];

            obj.familia = this.Session["familia"].ToString();
            obj.proveedor = this.Session["proveedor"].ToString();

            ReporteStock excel = new ReporteStock();
            ProductoBL bl = new ProductoBL();
            List<RegistroCargaStock> lista = bl.InventarioStock(ajusteMercaderiaTransito, fechaStock, usuario.idUsuario, idSede, obj);


            return excel.generateExcel(lista, usuario, tipoUnidad, fechaStock);
        }

        public String ReporteStockProducto(Guid idCiudad, String sku)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            ProductoBL bl = new ProductoBL();
            List<RegistroCargaStock> stocks = bl.StockProducto(sku, usuario.idUsuario);

            return JsonConvert.SerializeObject(stocks); 
        }

        public String ReporteStockProductoKardex(Guid idCiudad, Guid idProducto, int idProductoPresentacion, string fechaInicio)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            ProductoBL bl = new ProductoBL();
            DateTime? dateFechaInicio = null;
            if (!fechaInicio.Trim().Equals(""))
            {
                String[] fiv = fechaInicio.Split('/');
                dateFechaInicio = new DateTime(Int32.Parse(fiv[2]), Int32.Parse(fiv[1]), Int32.Parse(fiv[0]));
            }
            MovimientoKardexCabecera kardex = bl.StockProductoKardex(usuario.idUsuario, idCiudad, idProducto, idProductoPresentacion, dateFechaInicio);

            return JsonConvert.SerializeObject(kardex);
        }

        [HttpGet]
        public ActionResult ReporteStockProductoKardexExcel(Guid idCiudad, Guid idProducto, int idProductoPresentacion, string fechaInicio)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            ProductoBL bl = new ProductoBL();
            DateTime? dateFechaInicio = null;
            if (!fechaInicio.Trim().Equals(""))
            {
                String[] fiv = fechaInicio.Split('/');
                dateFechaInicio = new DateTime(Int32.Parse(fiv[2]), Int32.Parse(fiv[1]), Int32.Parse(fiv[0]));
            }
            MovimientoKardexCabecera kardex = bl.StockProductoKardex(usuario.idUsuario, idCiudad, idProducto, idProductoPresentacion, dateFechaInicio);

            ReporteKardexProducto excel = new ReporteKardexProducto();

            return excel.generateExcel(kardex, dateFechaInicio);
        }



        public ActionResult CargasStock()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            ProductoBL bl = new ProductoBL();
            List<CierreStock> cargas = bl.CargasStock(usuario.idUsuario, Guid.Empty);

            ViewBag.cargas = cargas;

            return View();
        }

        [HttpPost]
        public String GetStockProductos(string ids, string idCiudad)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return "";
            }

            List<Guid> idProductos = new List<Guid>();
            string[] idsItems = ids.Split(';');

            foreach (string id in idsItems)
            {
                idProductos.Add(Guid.Parse(id));
            }

            ProductoBL bl = new ProductoBL();
            List<RegistroCargaStock> stocks = bl.StockProductosSede(idProductos, Guid.Parse(idCiudad), usuario.idUsuario);

            return JsonConvert.SerializeObject(stocks);
        }


        [HttpPost]
        public String GetCierreStock(Guid idCierreStock)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return "";
            }

            
            CierreStockBL bl = new CierreStockBL();
            CierreStock cierre = bl.SelectCierreStock(usuario.idUsuario, idCierreStock);
            string jsonCotizacion = JsonConvert.SerializeObject(ParserDTOsShow.CierreStockDTO(cierre));

            return jsonCotizacion;
        }

        [HttpPost]
        public String EjecutarReporteValidacionStock(Guid idCierreStock)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return "";
            }


            CierreStockBL bl = new CierreStockBL();
            bl.GenerarReporteValidacionStock(usuario.idUsuario, idCierreStock);

            var obj = new { success = 1 };
        
            return JsonConvert.SerializeObject(obj);
        }

        public ActionResult Load(HttpPostedFileBase file)
        {

            Usuario usuario = (Usuario)this.Session["usuario"];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.realizaCargaMasivaStock)
            {
                return RedirectToAction("Login", "Account");
            }


            int tipoCarga = int.Parse(this.Request.Params["tipoCarga"].ToString());
            Guid idSede = Guid.Parse(this.Request.Params["idCiudad"].ToString());
            String[] fiv = this.Request.Params["fechaCierre"].Split('/');
            DateTime fechaCierre = new DateTime(Int32.Parse(fiv[2]), Int32.Parse(fiv[1]), Int32.Parse(fiv[0]));

            ArchivoAdjuntoBL arcBL = new ArchivoAdjuntoBL();
            ArchivoAdjunto arAd = new ArchivoAdjunto();
            Stream inputStream = file.InputStream;

            if (file != null)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }

                arAd.idArchivoAdjunto = Guid.Empty;
                arAd.usuario = usuario;
                arAd.origen = "CIERRE_STOCK";
                arAd.idRegistro = Guid.Empty.ToString();
                arAd.estado = 1;
                arAd.nombre = file.FileName;
                arAd.adjunto = memoryStream.ToArray();
                arAd = arcBL.InsertArchivoGenerico(arAd);
            }


            HSSFWorkbook hssfwb;

            ProductoBL productoBL = new ProductoBL();
            inputStream.Position = 0;

            hssfwb = new HSSFWorkbook(inputStream);

            ISheet sheet = hssfwb.GetSheetAt(0);
            int row = 1;
            int cantidad = sheet.LastRowNum;

            int posicionInicial = 0;
            int pos = 0;
            int invalidosConsecutivos = 0;
            bool agregar = false;

            List<RegistroCargaStock> listaStock = new List<RegistroCargaStock>();

            for (row = 1; row <= cantidad; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    agregar = true;
                    RegistroCargaStock nuevo = new RegistroCargaStock();
                    //try
                    //{
                        pos = posicionInicial + 1;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            nuevo.sku = null;
                            agregar = false;
                        }
                        else
                        {
                            nuevo.sku = sheet.GetRow(row).GetCell(pos).ToString();
                            if (nuevo.sku.Trim().Equals(""))
                            {
                                agregar = false;
                            }
                        }

                        pos = posicionInicial + 7;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            nuevo.cantidadProveedor = 0;
                        }
                        else
                        {
                            try
                            {
                                nuevo.cantidadProveedor = Int32.Parse(sheet.GetRow(row).GetCell(pos).ToString());
                            }
                            catch (Exception ex)
                            {
                                nuevo.cantidadProveedor = 0;
                            }
                        }

                        pos = posicionInicial + 10;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            nuevo.cantidadMp = 0;
                        }
                        else
                        {
                            try
                            {
                                nuevo.cantidadMp = Int32.Parse(sheet.GetRow(row).GetCell(pos).ToString());
                            }
                            catch (Exception ex)
                            {
                                nuevo.cantidadMp = 0;
                            }
                        }


                        pos = posicionInicial + 13;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            nuevo.cantidadAlternativa = 0;
                        }
                        else
                        {
                            try
                            {
                                nuevo.cantidadAlternativa = Int32.Parse(sheet.GetRow(row).GetCell(pos).ToString());
                            }
                            catch (Exception ex)
                            {
                                nuevo.cantidadAlternativa = 0;
                            }
                        }

                        

                       
                        if (agregar)
                        {
                            listaStock.Add(nuevo);
                            //if ((nuevo.cantidadAlternativa + nuevo.cantidadMp + nuevo.cantidadProveedor) > 0)
                            //{
                            //    listaStock.Add(nuevo);
                            //}
                            invalidosConsecutivos = 0;
                        } else
                        {
                            invalidosConsecutivos++;
                            if (invalidosConsecutivos > 5)
                            {
                                row = sheet.LastRowNum;
                            }
                        }
                    //}
                    //catch (Exception ex)
                    //{
                    //    Log log = new Log(ex.ToString() + " paso:" + pos, TipoLog.Error, usuario);
                    //    LogBL logBL = new LogBL();
                    //    logBL.insertLog(log);
                    //}
                }
            }

            
            productoBL.RegistroCierreStock(listaStock, fechaCierre, idSede, usuario.idUsuario, arAd.idArchivoAdjunto, tipoCarga);

            ViewBag.tipoCarga = "archivo";

            return View("CargaCorrecta");
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