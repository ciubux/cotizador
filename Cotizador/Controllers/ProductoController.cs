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
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.CUProducto;
            Usuario usuario = null;
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                if (!usuario.modificaMaestroClientes && !usuario.modificaProducto)
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
        public ActionResult Load(HttpPostedFileBase file)
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
                        try
                        {


                            if (sheet.GetRow(row).GetCell(0+ posicionInicial) == null)
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
                                productoStaging.unidadAlternativaSunat = "";
                            }
                            else
                            {
                                productoStaging.unidadAlternativaSunat = sheet.GetRow(row).GetCell(29 + posicionInicial).ToString();
                            }

                            productoBL.setProductoStaging(productoStaging);
                        }
                        catch (Exception ex)
                        {

                            Usuario usuario = (Usuario)this.Session["usuario"];
                            Log log = new Log(ex.ToString() + " paso:" + paso, TipoLog.Error, usuario);
                            LogBL logBL = new LogBL();
                            logBL.insertLog(log);


                        }
                    }
                }
                productoBL.mergeProductoStaging();
                row = row;
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



        public ActionResult GetDescuentos(string productoSelectId, string selectedValue = null, string disabled = null)
        {
            //Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            var model = new DescuentoViewModels
            {
                Data = Constantes.DESCUENTOS_LIST,
                ProductoSelectId = productoSelectId,
                SelectedValue = selectedValue,
                Disabled = disabled == null || disabled != "disabled" ? false : true
            };

            return PartialView("_Descuento", model);
        }
        private Producto ProductoSession
        {
            get
            {
                Producto producto = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaProductos: producto = (Producto)this.Session[Constantes.VAR_SESSION_PRODUCTO_BUSQUEDA]; break;
                    case Constantes.paginas.CUProducto: producto = (Producto)this.Session[Constantes.VAR_SESSION_PRODUCTO]; break;
                }
                return producto;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaProductos: this.Session[Constantes.VAR_SESSION_PRODUCTO_BUSQUEDA] = value; break;
                    case Constantes.paginas.CUProducto: this.Session[Constantes.VAR_SESSION_PRODUCTO] = value; break;
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