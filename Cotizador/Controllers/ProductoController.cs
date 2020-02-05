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

namespace Cotizador.Controllers
{
    public class ProductoController : Controller
    {

        [HttpGet]
        public ActionResult List()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaProductos;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (this.Session[Constantes.VAR_SESSION_PRODUCTO_BUSQUEDA] == null)
            {
                instanciarProductoBusqueda();
            }

            this.Session["familia"] = "Todas";
            this.Session["proveedor"] = "Todos";

            Producto productoSearch = (Producto)this.Session[Constantes.VAR_SESSION_PRODUCTO_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaProductos;
            ViewBag.producto = productoSearch;
            ViewBag.Si = Constantes.MENSAJE_SI;
            ViewBag.No = Constantes.MENSAJE_NO;
            return View();

        }

        public String SearchList()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaProductos;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            Producto producto = (Producto)this.Session[Constantes.VAR_SESSION_PRODUCTO_BUSQUEDA];
            producto.familia = this.Session["familia"].ToString();
            producto.proveedor = this.Session["proveedor"].ToString();
            ProductoBL productoBL = new ProductoBL();
            List<Producto> productoList = productoBL.getProductos(producto);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_PRODUCTO_LISTA] = productoList;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(productoList);

        }

        public String Show()
        {
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            ProductoBL productoBL = new ProductoBL();
            Producto producto = productoBL.getProductoById(idProducto);
            producto.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            String resultado =  JsonConvert.SerializeObject(producto);
            this.Session[Constantes.VAR_SESSION_PRODUCTO_VER] = producto;
            return resultado;
        }

        public ActionResult Editar(Guid? idProducto = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoProductos;
            Usuario usuario = null;
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                if (!usuario.modificaMaestroClientes && !usuario.modificaMaestroProductos)
                {
                    return RedirectToAction("Login", "Account");
                }
            }


            if (this.Session[Constantes.VAR_SESSION_PRODUCTO] == null || idProducto == Guid.Empty)
            {
                instanciarProducto();
            }

            Producto producto = (Producto)this.Session[Constantes.VAR_SESSION_PRODUCTO];
            
            if (idProducto != null && idProducto != Guid.Empty)
            {
                ProductoBL productoBL = new ProductoBL();
                producto = productoBL.getProductoById(idProducto.Value);
                producto.IdUsuarioRegistro = usuario.idUsuario;
                producto.usuario = usuario;
                
                this.Session[Constantes.VAR_SESSION_PRODUCTO] = producto;
            }

            this.Session["familia"] = producto.familia;
            this.Session["proveedor"] = producto.proveedor;

            ViewBag.producto = producto;
            return View();

        }

        private void instanciarProducto()
        {
            Producto producto = new Producto();
            producto.idProducto = Guid.Empty;
            producto.sku = String.Empty;
            producto.skuProveedor = String.Empty;
            producto.descripcion = String.Empty;
            producto.familia = String.Empty;
            producto.proveedor = String.Empty;
            producto.costoSinIgv = 0;
            producto.familia = String.Empty;
            producto.proveedor = String.Empty;
            producto.unidad = String.Empty;
            producto.unidadProveedor = String.Empty;
            producto.unidad_alternativa = String.Empty;
            producto.unidadEstandarInternacional = String.Empty;
            FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
            MemoryStream storeStream = new MemoryStream();
            storeStream.SetLength(inStream.Length);
            inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
            storeStream.Flush();
            inStream.Close();
            producto.image = storeStream.GetBuffer();
            producto.tipoProductoVista = (int) producto.tipoProducto;

            this.Session["familia"] = "Todas";
            this.Session["proveedor"] = "Todos";

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            producto.IdUsuarioRegistro = usuario.idUsuario;
            producto.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_PRODUCTO] = producto;
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
            producto.descontinuado = -1;

            this.Session["familia"] = "Todas";
            this.Session["proveedor"] = "Todos";

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            producto.IdUsuarioRegistro = usuario.idUsuario;
            producto.usuario = usuario;
            this.Session[Constantes.VAR_SESSION_PRODUCTO_BUSQUEDA] = producto;
        }


        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<Producto> list = (List<Producto>)this.Session[Constantes.VAR_SESSION_PRODUCTO_LISTA];

            ProductoSearch excel = new ProductoSearch();
            return excel.generateExcel(list, (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO]);
        }
        
        [HttpGet]
        public ActionResult ExportLastSearchUploadExcel()
        {
            List<Producto> list = (List<Producto>)this.Session[Constantes.VAR_SESSION_PRODUCTO_LISTA];

            ProductoSearch excel = new ProductoSearch();
            return excel.generateUploadExcel(list, (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO]);
        }

        public String Search()
        {
            String texto_busqueda = this.Request.Params["data[q]"];
            ProductoBL bl = new ProductoBL();
            String resultado = bl.getProductosBusqueda(texto_busqueda, false, this.Session["proveedor"] != null ? (String)this.Session["proveedor"] : "Todos", this.Session["familia"] != null ? (String)this.Session["familia"] : "Todas");
            return resultado;
        }


      

        // GET: Producto
        [HttpGet]
        public ActionResult Index()
        {

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                if (!usuario.modificaMaestroProductos)
                {
                    return RedirectToAction("Login", "Account");
                }
            }

            LogCampoBL logCamboBl = new LogCampoBL();
            List<LogCampo> campos = logCamboBl.getCampoLogPorTabla(Producto.NOMBRE_TABLA);

            List<CampoPersistir> persitirCampos = Producto.obtenerCampos(campos);
            ParametroBL parametrobl = new ParametroBL();
            Decimal tipoCambio = parametrobl.getParametroDecimal("TIPO_CAMBIO");

            ViewBag.persitirCampos = persitirCampos;
            ViewBag.tipoCambio = tipoCambio;
            
            return View();

        }

        public String ConsultarSiExisteProducto()
        {
            Producto producto = (Producto)this.Session[Constantes.VAR_SESSION_PRODUCTO];
            if (producto == null)
                return "{\"existe\":\"false\",\"idProducto\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"idProducto\":\"" + producto.idProducto + "\"}";
        }


        public void iniciarEdicionProducto()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Producto productoVer = (Producto)this.Session[Constantes.VAR_SESSION_PRODUCTO_VER];
            this.Session[Constantes.VAR_SESSION_PRODUCTO] = productoVer;
        }

        public String Create()
        {
            ProductoBL productoBL = new ProductoBL();
            Producto producto = (Producto)this.Session[Constantes.VAR_SESSION_PRODUCTO];
            producto.familia = this.Session["familia"].ToString();
            producto.proveedor = this.Session["proveedor"].ToString();
            producto = productoBL.insertProducto(producto);
            this.Session[Constantes.VAR_SESSION_PRODUCTO] = null;
            String resultado = JsonConvert.SerializeObject(producto);
            return resultado;
        }


        public String Update()
        {
            ProductoBL productoBL = new ProductoBL();
            Producto producto = (Producto)this.Session[Constantes.VAR_SESSION_PRODUCTO];
            producto.familia = this.Session["familia"].ToString();
            producto.proveedor = this.Session["proveedor"].ToString();
            if (producto.idProducto == Guid.Empty)
            {
                producto = productoBL.insertProducto(producto);
                this.Session[Constantes.VAR_SESSION_PRODUCTO] = null;
            }
            else
            {
                producto = productoBL.updateProducto(producto);
                this.Session[Constantes.VAR_SESSION_PRODUCTO] = null;
            }
            String resultado = JsonConvert.SerializeObject(producto);
            //this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            return resultado;
        }

        [HttpPost]
        public ActionResult Load1(HttpPostedFileBase file)
        {
            /*     if (file.ContentLength > 0)
                 {
                     var fileName = Path.GetFileName(file.FileName);
                     var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                     file.SaveAs(path);
                 }
                 */

            try
            {

                HSSFWorkbook hssfwb;

                ProductoBL productoBL = new ProductoBL();
                productoBL.truncateProductoStaging();

                hssfwb = new HSSFWorkbook(file.InputStream);

                ISheet sheet = hssfwb.GetSheetAt(0);
                int row = 1;
                int cantidad = Int32.Parse(Request["cantidadLineas"].ToString());
                if (cantidad == 0)
                    cantidad = sheet.LastRowNum;

                //   cantidad = 2008;
                //sheet.LastRowNum
                int posicionInicial = 2;

                for (row = 1; row <= cantidad; row++)
                {
                    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                    {

                        ProductoStaging productoStaging = new ProductoStaging();
                        int paso = 1;
                      /*  try
                        {*/


                            if (sheet.GetRow(row).GetCell(0 + posicionInicial) == null)
                            {
                                productoStaging.familia = "No proporcionada";
                            }
                            else
                            {
                                //A
                                productoStaging.familia = sheet.GetRow(row).GetCell(0 + posicionInicial).ToString();
                            }


                            paso = 2;
                            if (sheet.GetRow(row).GetCell(1 + posicionInicial) == null)
                            {
                                productoStaging.proveedor = null;
                            }
                            else
                            {
                                //B
                                productoStaging.proveedor = sheet.GetRow(row).GetCell(1 + posicionInicial).ToString();
                            }


                            paso = 3;
                            if (sheet.GetRow(row).GetCell(2 + posicionInicial) == null)
                            {
                                productoStaging.codigo = null;
                            }
                            else
                            {
                                //C
                                productoStaging.codigo = sheet.GetRow(row).GetCell(2 + posicionInicial).ToString();
                            }

                            paso = 4;
                            //D
                            if (sheet.GetRow(row).GetCell(3 + posicionInicial) == null)
                            {
                                productoStaging.codigoProveedor = null;
                            }
                            else
                            {
                                productoStaging.codigoProveedor = sheet.GetRow(row).GetCell(3 + posicionInicial).ToString();
                            }

                            paso = 5;
                            //E
                            if (sheet.GetRow(row).GetCell(4 + posicionInicial) == null)
                            {
                                productoStaging.unidad = null;
                            }
                            else
                            {
                                productoStaging.unidad = sheet.GetRow(row).GetCell(4 + posicionInicial).ToString();
                            }

                            paso = 6;
                            //F
                            if (sheet.GetRow(row).GetCell(5 + posicionInicial) == null)
                            {
                                productoStaging.unidadProveedor = null;
                            }
                            else
                            {
                                productoStaging.unidadProveedor = sheet.GetRow(row).GetCell(5 + posicionInicial).ToString();
                            }


                            paso = 7;
                            //G
                            if (sheet.GetRow(row).GetCell(6 + posicionInicial) == null)
                            {
                                productoStaging.equivalenciaProveedor = 0;
                            }
                            else
                            {
                                productoStaging.equivalenciaProveedor = Int32.Parse(sheet.GetRow(row).GetCell(6 + posicionInicial).ToString());
                            }


                            paso = 8;
                            //H
                            if (sheet.GetRow(row).GetCell(7 + posicionInicial) == null)
                            {
                                productoStaging.unidad = null;
                            }
                            else
                            {
                                productoStaging.unidadAlternativa = sheet.GetRow(row).GetCell(7 + posicionInicial).ToString();
                            }

                            paso = 9;
                            //J
                            if (sheet.GetRow(row).GetCell(8 + posicionInicial) == null)
                            {
                                productoStaging.equivalencia = 1;
                            }
                            else
                            {
                                productoStaging.equivalencia = Int32.Parse(sheet.GetRow(row).GetCell(8 + posicionInicial).ToString());
                            }

                            paso = 10;
                            //K
                            if (sheet.GetRow(row).GetCell(9 + posicionInicial) == null)
                            {
                                productoStaging.descripcion = null;
                            }
                            else
                            {
                                productoStaging.descripcion = sheet.GetRow(row).GetCell(9 + posicionInicial).ToString();
                            }


                            paso = 11;
                            //S
                            try
                            {
                                productoStaging.monedaProveedor = sheet.GetRow(row).GetCell(18 + posicionInicial).ToString();
                            }
                            catch (Exception e)
                            {
                                productoStaging.monedaProveedor = "S";
                            }



                            paso = 12;
                            //T
                            try
                            {
                                Double? costo = sheet.GetRow(row).GetCell(19 + posicionInicial).NumericCellValue;
                                productoStaging.costo = Convert.ToDecimal(costo);
                            }
                            catch (Exception e)
                            {
                                productoStaging.costo = 0;
                            }

                            paso = 13;
                            try
                            {
                                //X
                                productoStaging.monedaMP = sheet.GetRow(row).GetCell(23 + posicionInicial).ToString();
                            }
                            catch (Exception e)
                            {
                                productoStaging.monedaMP = "S";
                            }

                            paso = 14;
                            try
                            {
                                //Y
                                Double? precioLima = sheet.GetRow(row).GetCell(24 + posicionInicial).NumericCellValue;
                                productoStaging.precioLima = Convert.ToDecimal(precioLima);
                            }
                            catch (Exception e)
                            {
                                productoStaging.precioLima = 0;
                            }

                            paso = 15;
                            try
                            {
                                //AB
                                Double? precioProvincias = sheet.GetRow(row).GetCell(27 + posicionInicial).NumericCellValue;
                                productoStaging.precioProvincias = Convert.ToDecimal(precioProvincias);
                            }
                            catch (Exception e)
                            {
                                productoStaging.precioProvincias = 0;
                            }


                            paso = 16;
                            //AC
                            if (sheet.GetRow(row).GetCell(28 + posicionInicial) == null)
                            {
                                productoStaging.unidadSunat = "";
                            }
                            else
                            {
                                productoStaging.unidadSunat = sheet.GetRow(row).GetCell(28 + posicionInicial).ToString();
                            }

                            paso = 17;
                            //AD
                            if (sheet.GetRow(row).GetCell(29 + posicionInicial) == null)
                            {
                                productoStaging.unidadProveedorSunat = "";
                            }
                            else
                            {
                                productoStaging.unidadProveedorSunat = sheet.GetRow(row).GetCell(29 + posicionInicial).ToString();
                            }


                            paso = 18;
                            //AD
                            if (sheet.GetRow(row).GetCell(30 + posicionInicial) == null)
                            {
                                productoStaging.unidadAlternativaSunat = "";
                            }
                            else
                            {
                                productoStaging.unidadAlternativaSunat = sheet.GetRow(row).GetCell(30 + posicionInicial).ToString();
                            }

                            productoBL.setProductoStaging(productoStaging);
                        /*}
                        catch (Exception ex)
                        {

                            Usuario usuario = (Usuario)this.Session["usuario"];
                            Log log = new Log(ex.ToString() + " paso:" + paso, TipoLog.Error, usuario);
                            LogBL logBL = new LogBL();
                            logBL.insertLog(log);


                        }*/
                    }
                }
                productoBL.mergeProductoStaging();
                return View("CargaCorrecta");
            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session["usuario"];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
                return View("CargaIncorrecta");
            }
        }


        [HttpPost]
        public ActionResult Load(HttpPostedFileBase file)
        {

            Usuario usuario = (Usuario)this.Session["usuario"];
            LogCampoBL logCambioBl = new LogCampoBL();
            List<LogCampo> campos = logCambioBl.getCampoLogPorTabla(Producto.NOMBRE_TABLA);

            List<CampoPersistir> registrarCampos = Producto.obtenerCampos(campos);
            //List<CampoPersistir> registrarCampos = new List<CampoPersistir>();
            int select = 0;

            foreach (CampoPersistir cp in registrarCampos)
            {
                cp.registra = false;
                cp.persiste = false;

                //si ha sido seleccionado o es un campo no actualizable en la carga masiva debe agregarse a la lista de cmapos a registrar
                if (Request["registra_" + cp.campo.nombre] != null || !Producto.esCampoActualizableCargaMasiva(cp.campo.nombre))
                {

                    if (!Producto.esCampoActualizableCargaMasiva(cp.campo.nombre))
                    {
                        select = 1;
                    } else
                    {
                        select = Int32.Parse(Request["registra_" + cp.campo.nombre].ToString());
                    }

                    if (select == 1)
                    {
                        cp.registra = true;

                        if (Request["persiste_" + cp.campo.nombre] != null)
                        {
                            int persiste = Int32.Parse(Request["registra_" + cp.campo.nombre].ToString());
                            cp.persiste = persiste == 1 ? true : false;
                        }
                    }
                }
            }

            LogCambioBL logCambiobl = new LogCambioBL();
            ParametroBL parametrobl = new ParametroBL();

            //Decimal tipoCambio = parametrobl.getParametroDecimal("TIPO_CAMBIO");
            Decimal tipoCambio = Decimal.Parse(this.Request.Params["tipo_cambio"].ToString());
            String[] fiv = this.Request.Params["fechaInicioVigencia"].Split('/');
            DateTime fechaInicioVigencia = new DateTime(Int32.Parse(fiv[2]), Int32.Parse(fiv[1]), Int32.Parse(fiv[0]));

            int nFIV = (fechaInicioVigencia.Year * 10000) + (fechaInicioVigencia.Month * 100) + fechaInicioVigencia.Day;
            int nFT = (DateTime.Now.Year * 10000) + (DateTime.Now.Month * 100) + DateTime.Now.Day;
            int nFR = 0;
            bool isNew = false;
            HSSFWorkbook hssfwb;

            ProductoBL productoBL = new ProductoBL();

            hssfwb = new HSSFWorkbook(file.InputStream);

            ISheet sheet = hssfwb.GetSheetAt(0);
            int row = 1;
            int cantidad = Int32.Parse(Request["cantidadLineas"].ToString());
            if (cantidad == 0)
                cantidad = sheet.LastRowNum;

            //   cantidad = 2008;
            int lastrow = sheet.LastRowNum;
            int posicionInicial = 0;
            int pos = 0;
            int contInsert = 0;
            int contUpdate = 0;
            bool agregar = false;

            for (row = 1; row <= cantidad; row++)
            {
                int a = 1;
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    isNew = false;

                    Producto productoStaging = new Producto();
                    agregar = true;
                    try
                    {
                        pos = posicionInicial + 0;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.sku = null;
                            agregar = false;
                        }
                        else
                        {
                            productoStaging.sku = sheet.GetRow(row).GetCell(pos).ToString();
                            if (productoStaging.sku.Trim().Equals(""))
                            {
                                agregar = false;
                            }
                        }

                        pos = posicionInicial + 1;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.skuProveedor = null;
                        }
                        else
                        {
                            productoStaging.skuProveedor = sheet.GetRow(row).GetCell(pos).ToString();
                        }
                        
                        pos = posicionInicial + 2;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.proveedor = null;
                        }
                        else
                        {
                            productoStaging.proveedor = sheet.GetRow(row).GetCell(pos).ToString();
                        }

                        pos = posicionInicial + 3;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.familia = "No proporcionada";
                        }
                        else
                        {
                            productoStaging.familia = sheet.GetRow(row).GetCell(pos).ToString();
                        }

                        pos = posicionInicial + 4;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.descripcion = null;
                        }
                        else
                        {
                            productoStaging.descripcion = sheet.GetRow(row).GetCell(pos).ToString();
                        }

                        pos = posicionInicial + 5;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.unidad = null;
                        }
                        else
                        {
                            productoStaging.unidad = sheet.GetRow(row).GetCell(pos).ToString();
                        }

                        pos = posicionInicial + 6;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.unidadProveedor = null;
                        }
                        else
                        {
                            productoStaging.unidadProveedor = sheet.GetRow(row).GetCell(pos).ToString();
                        }

                        pos = posicionInicial + 7;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.equivalenciaProveedor = 0;
                        }
                        else
                        {
                            productoStaging.equivalenciaProveedor = Int32.Parse(sheet.GetRow(row).GetCell(pos).ToString());
                        }

                        pos = posicionInicial + 8;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.unidad_alternativa = null;
                        }
                        else
                        {
                            productoStaging.unidad_alternativa = sheet.GetRow(row).GetCell(pos).ToString();
                        }

                        pos = posicionInicial + 9;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.equivalenciaAlternativa = 0;
                        }
                        else
                        {
                            productoStaging.equivalenciaAlternativa = Int32.Parse(sheet.GetRow(row).GetCell(pos).ToString());
                        }

                        pos = posicionInicial + 10;
                        try
                        {
                            productoStaging.monedaProveedor = sheet.GetRow(row).GetCell(pos).ToString().Trim();
                        }
                        catch (Exception e)
                        {
                            productoStaging.monedaProveedor = "S";
                        }

                        pos = posicionInicial + 11;
                        try
                        {
                            Double? val = sheet.GetRow(row).GetCell(pos).NumericCellValue;
                            productoStaging.costoOriginal = Convert.ToDecimal(val);
                        }
                        catch (Exception e)
                        {
                            productoStaging.costoOriginal = 0;
                        }

                        pos = posicionInicial + 12; // El costo es calculado 

                        pos = posicionInicial + 13;
                        try
                        {
                            productoStaging.monedaMP = sheet.GetRow(row).GetCell(pos).ToString().Trim();
                        }
                        catch (Exception e)
                        {
                            productoStaging.monedaMP = "S";
                        }

                        pos = posicionInicial + 14;
                        try
                        {
                            Double? val = sheet.GetRow(row).GetCell(pos).NumericCellValue;
                            productoStaging.precioOriginal = Convert.ToDecimal(val);
                        }
                        catch (Exception e)
                        {
                            productoStaging.precioOriginal = 0;
                        }

                        pos = posicionInicial + 15; // El precio lima es calculado

                        pos = posicionInicial + 16;
                        try
                        {
                            Double? val = sheet.GetRow(row).GetCell(pos).NumericCellValue;
                            productoStaging.precioProvinciasOriginal = Convert.ToDecimal(val);
                        }
                        catch (Exception e)
                        {
                            productoStaging.precioProvinciasOriginal = 0;
                        }

                        pos = posicionInicial + 17; // El precio provincias es calculado

                        pos = posicionInicial + 18;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.unidadConteo = null;
                        }
                        else
                        {
                            productoStaging.unidadConteo = sheet.GetRow(row).GetCell(pos).ToString();
                        }

                        pos = posicionInicial + 19;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.unidadEstandarInternacional = null;
                        }
                        else
                        {
                            productoStaging.unidadEstandarInternacional = sheet.GetRow(row).GetCell(pos).ToString();
                        }

                        pos = posicionInicial + 20;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.equivalenciaUnidadEstandarUnidadConteo = 0;
                        }
                        else
                        {
                            productoStaging.equivalenciaUnidadEstandarUnidadConteo = Int32.Parse(sheet.GetRow(row).GetCell(pos).ToString());
                        }

                        pos = posicionInicial + 21;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.unidadProveedorInternacional = null;
                        }
                        else
                        {
                            productoStaging.unidadProveedorInternacional = sheet.GetRow(row).GetCell(pos).ToString();
                        }

                        pos = posicionInicial + 22;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.equivalenciaUnidadProveedorUnidadConteo = 0;
                        }
                        else
                        {
                            productoStaging.equivalenciaUnidadProveedorUnidadConteo = Int32.Parse(sheet.GetRow(row).GetCell(pos).ToString());
                        }

                        pos = posicionInicial + 23;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.unidadAlternativaInternacional = null;
                        }
                        else
                        {
                            productoStaging.unidadAlternativaInternacional = sheet.GetRow(row).GetCell(pos).ToString();
                        }

                        pos = posicionInicial + 24;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.equivalenciaUnidadAlternativaUnidadConteo = 0;
                        }
                        else
                        {
                            productoStaging.equivalenciaUnidadAlternativaUnidadConteo = Int32.Parse(sheet.GetRow(row).GetCell(pos).ToString());
                        }

                        pos = posicionInicial + 25;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            productoStaging.codigoSunat = null;
                        }
                        else
                        {
                            productoStaging.codigoSunat = sheet.GetRow(row).GetCell(pos).ToString();
                        }
                        
                        pos = posicionInicial + 26;
                        try
                        {
                            productoStaging.exoneradoIgv = sheet.GetRow(row).GetCell(pos).ToString().Trim().ToUpper() == "SI" ? true : false;
                        }
                        catch (Exception e)
                        {
                            productoStaging.exoneradoIgv = false;
                        }

                        pos = posicionInicial + 27;
                        try
                        {
                            productoStaging.inafecto = sheet.GetRow(row).GetCell(pos).ToString().Trim().ToUpper() == "SI" ? true : false;
                        }
                        catch (Exception e)
                        {
                            productoStaging.inafecto = false;
                        }

                        pos = posicionInicial + 28;
                        try
                        {
                            string name = sheet.GetRow(row).GetCell(pos).ToString().Trim();
                            
                            productoStaging.tipoProducto = (Producto.TipoProducto) Enum.Parse(typeof(Producto.TipoProducto), name);
                        }
                        catch (Exception e)
                        {
                            productoStaging.tipoProducto = Producto.TipoProducto.Bien;
                        }

                        pos = posicionInicial + 29; // Tipo de cambio se sube en el formulario

                        pos = posicionInicial + 30;
                        try
                        {
                            productoStaging.Estado = sheet.GetRow(row).GetCell(pos).ToString().Trim().ToUpper().Equals("SI") ? 1 : 0;
                        }
                        catch (Exception e)
                        {
                            productoStaging.Estado = 0;
                        }

                        //UtilesHelper.setValorCelda(sheet, 1, "AC", Producto.nombreAtributo("tipoProducto"), titleCellStyle);

                        
                        Guid idRegistro = productoBL.getAllProductoId(productoStaging.sku);
                            

                        if (idRegistro == Guid.Empty)
                        {
                            //TO DO: Realizar nuevo registro en el proceso de aplicar cambios 
                            idRegistro = Guid.NewGuid();
                            isNew = true;
                        }

                        // estos productos no se toman en cuenta?:'SG7A08','YXDM600'

                        productoStaging.costoSinIgv = productoStaging.costoOriginal / (productoStaging.equivalenciaProveedor == 0 ? 1 : productoStaging.equivalenciaProveedor);
                        if (productoStaging.monedaProveedor == "D")
                        {
                            productoStaging.costoSinIgv = productoStaging.costoSinIgv * tipoCambio;
                        }

                        if (productoStaging.monedaMP == "D")
                        {
                            productoStaging.precioSinIgv = productoStaging.precioOriginal * tipoCambio;
                        }
                        else
                        {
                            productoStaging.precioSinIgv = productoStaging.precioOriginal;
                        }

                        if (productoStaging.monedaMP == "D")
                        {
                            productoStaging.precioProvinciaSinIgv = productoStaging.precioProvinciasOriginal * tipoCambio;
                        }
                        else
                        {
                            productoStaging.precioProvinciaSinIgv = productoStaging.precioProvinciasOriginal;
                        }

                        productoStaging.tipoCambio = tipoCambio;
                        productoStaging.idProducto = idRegistro;
                        productoStaging.usuario = usuario;
                        productoStaging.fechaInicioVigencia = fechaInicioVigencia;
                        if (agregar)
                        {
                            if (isNew)
                            {
                                contInsert++;
                            } else
                            {
                                contUpdate++;
                            }

                            if (nFIV >= nFT)
                            {
                                //Fecha Inicio vigencia es mayor o igual a la fecha actual
                                if (isNew)
                                {
                                    //Si es un nuevo registro se guarda el log con todos los campos
                                    logCambiobl.insertLogCambiosPogramados(productoStaging.obtenerLogProgramado(registrarCampos, true));
                                }
                                else
                                {
                                    logCambiobl.insertLogCambiosPogramados(productoStaging.obtenerLogProgramado(registrarCampos));
                                }
                            }
                            else
                            {
                                //Fecha Inicio vigencia es menor a la fecha actual
                                Producto existente = productoBL.getProductoById(idRegistro);

                                if (existente.fechaInicioVigencia != null)
                                {
                                    //En caso exista y tenga una fecha de inicio de vigencia se toma la fecha de inicio de vigencia del registro existente
                                    nFR = (existente.fechaInicioVigencia.Year * 10000) + (existente.fechaInicioVigencia.Month * 100) + existente.fechaInicioVigencia.Day;
                                }
                                else
                                {
                                    nFR = int.MinValue;
                                }

                                if (nFR <= nFIV)
                                {
                                    //Si la fecha de inicio de vigencia del registro es menor a la fecha de inicio de vigencia se manda todo al log programado
                                    if (isNew)
                                    {
                                        logCambiobl.insertLogCambiosPogramados(productoStaging.obtenerLogProgramado(registrarCampos, true));
                                    }
                                    else
                                    {
                                        logCambiobl.insertLogCambiosPogramados(productoStaging.obtenerLogProgramado(registrarCampos));
                                    }
                                }
                                else
                                {
                                    //Si la fecha de inicio de vigencia del registro es mayor a la fecha de inicio de vigencia se manda todo al log normal
                                    if (isNew)
                                    {
                                        //Registrar
                                        productoBL.insertProducto(productoStaging);
                                    }
                                    else
                                    {
                                        //Registrar log
                                        logCambiobl.insertLogCambios(productoStaging.obtenerLogProgramado(registrarCampos));
                                    }
                                }
                            }
                        } else
                        {
                            row = sheet.LastRowNum;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log log = new Log(ex.ToString() + " paso:" + pos, TipoLog.Error, usuario);
                        LogBL logBL = new LogBL();
                        logBL.insertLog(log);
                    }
                }
            }

            //this.Session["ImportProducts_Inserts"] = contInsert;
            //this.Session["ImportProducts_Updates"] = contUpdate;

            if (nFIV <= nFT)
            {
                logCambiobl.aplicarLogCambios();
            }

            ViewBag.tipoCarga = "archivo";
            ViewBag.contInsert = contInsert;
            ViewBag.contUpdate = contUpdate;
            return View("CargaCorrecta");
        }


        public ActionResult ActualizarTodosPorTipoCambio()
        {

            // Obtener productos con moneda compra y/o venta en dolares 
            // Actualizar tipo de cambio, hacer calculo y mandar al log programado
            // aplicar log programado
            
            Usuario usuario = (Usuario)this.Session["usuario"];
            LogCampoBL logCambioBl = new LogCampoBL();
            List<LogCampo> campos = logCambioBl.getCampoLogPorTabla(Producto.NOMBRE_TABLA);

            List<CampoPersistir> registrarCampos = Producto.obtenerCampos(campos);
            List<CampoPersistir> camposHalitados = new List<CampoPersistir>();
            int select = 0;
            foreach (CampoPersistir cp in registrarCampos)
            {
                cp.registra = false;
                cp.persiste = false;
                if (Request["registra_tc_" + cp.campo.nombre] != null && Producto.esCampoCalculado(cp.campo.nombre))
                {
                    select = Int32.Parse(Request["registra_tc_" + cp.campo.nombre].ToString());
                    
                    if (select == 1)
                    {
                        cp.registra = true;
                        cp.persiste = true;
                        camposHalitados.Add(cp);
                    }
                }
            }
            
            LogCambioBL logCambiobl = new LogCambioBL();
            ParametroBL parametrobl = new ParametroBL();

            //Decimal tipoCambio = parametrobl.getParametroDecimal("TIPO_CAMBIO");
            Decimal tipoCambio = Decimal.Parse(this.Request.Params["tipo_cambio"].ToString());
            String[] fiv = this.Request.Params["fechaInicioVigencia"].Split('/');
            DateTime fechaInicioVigencia = new DateTime(Int32.Parse(fiv[2]), Int32.Parse(fiv[1]), Int32.Parse(fiv[0]));

            int nFIV = (fechaInicioVigencia.Year * 10000) + (fechaInicioVigencia.Month * 100) + fechaInicioVigencia.Day;
            int nFT = (DateTime.Now.Year * 10000) + (DateTime.Now.Month * 100) + DateTime.Now.Day;
            
            ProductoBL productoBL = new ProductoBL();

            productoBL.actualizaTipoCambioCatalogo(tipoCambio, camposHalitados, fechaInicioVigencia, usuario.idUsuario);

            if (nFIV <= nFT)
            {
                logCambiobl.aplicarLogCambios();
            }

            ViewBag.tipoCarga = "tipo_cambio";
            return View("CargaCorrecta");
        }

        public ActionResult GetDescuentos(string productoSelectId, string selectedValue = null, string disabled = null)
        {
            //Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            var model = new ProductoViewModels
            {
                Data = Constantes.DESCUENTOS_LIST,
                ProductoSelectId = productoSelectId,
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_Producto", model);
        }

        public ActionResult GetCargos(string productoSelectId, string selectedValue = null, string disabled = null)
        {
            //Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            var model = new ProductoMultipleViewModels
            {
                Data = Constantes.CARGOS_LIST,
                ProductoSelectIds = new List<String> { productoSelectId },
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_ProductoMultiple", model);
        }
        private Producto ProductoSession
        {
            get
            {
                Producto producto = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaProductos: producto = (Producto)this.Session[Constantes.VAR_SESSION_PRODUCTO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoProductos: producto = (Producto)this.Session[Constantes.VAR_SESSION_PRODUCTO]; break;
                }
                return producto;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaProductos: this.Session[Constantes.VAR_SESSION_PRODUCTO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoProductos: this.Session[Constantes.VAR_SESSION_PRODUCTO] = value; break;
                }
            }
        }

        public void ChangeInputString()
        {
            Producto producto = (Producto) this.ProductoSession;
            PropertyInfo propertyInfo = producto.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(producto, this.Request.Params["valor"]);
            this.ProductoSession = producto;
        }

        public void ChangeInputInt()
        {
            Producto producto = (Producto)this.ProductoSession;
            PropertyInfo propertyInfo = producto.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(producto, Int32.Parse(this.Request.Params["valor"]));
            this.ProductoSession = producto;
        }

        public void ChangeInputDecimal()
        {
            Producto producto = (Producto)this.ProductoSession;
            PropertyInfo propertyInfo = producto.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(producto, Decimal.Parse(this.Request.Params["valor"]));
            this.ProductoSession = producto;
        }
        public void ChangeInputBoolean()
        {
            Producto producto = (Producto)this.ProductoSession;
            PropertyInfo propertyInfo = producto.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(producto, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.ProductoSession = producto;
        }

        public void ChangeImage()
        {
            Producto producto = (Producto)this.ProductoSession;
            String imgBase = this.Request.Params["imgBase"].ToString();
            string[] sepImgBase = imgBase.Split(new string[] { "base64," }, StringSplitOptions.None);

            if (sepImgBase.Count() == 2)
            {
                imgBase = sepImgBase[1];
                producto.image = Convert.FromBase64String(imgBase);
            } 

            this.ProductoSession = producto;
        }

        public void ChangeTipoProducto()
        {
            int valor = Int32.Parse(this.Request.Params["tipoProducto"]);
            if (valor > 0)
            {
                ProductoSession.tipoProducto = (Producto.TipoProducto) valor;
            }

            ProductoSession.tipoProductoVista = valor;
        }

        public ActionResult CancelarCreacionProducto()
        {
            this.Session[Constantes.VAR_SESSION_PRODUCTO] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("List", "Producto");

        }
    }


}