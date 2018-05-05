using BusinessLayer;
using Model;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class ProductoController : Controller
    {

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
            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();

        }


        [HttpPost]
        public ActionResult Load(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);
            }


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
            for (row = 1; row <= cantidad; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {

                    ProductoStaging productoStaging = new ProductoStaging();
                    int paso = 1;
                    try
                    {


                        if (sheet.GetRow(row).GetCell(0) == null)
                        {
                            productoStaging.familia = "No proporcionada";
                        }
                        else
                        {
                            //A
                            productoStaging.familia = sheet.GetRow(row).GetCell(0).ToString();
                        }


                        paso = 2;
                        if (sheet.GetRow(row).GetCell(1) == null)
                        {
                            productoStaging.proveedor = null;
                        }
                        else
                        {
                            //B
                            productoStaging.proveedor = sheet.GetRow(row).GetCell(1).ToString();
                        }


                        paso = 3;
                        if (sheet.GetRow(row).GetCell(2) == null)
                        {
                            productoStaging.codigo = null;
                        }
                        else
                        {
                            //C
                            productoStaging.codigo = sheet.GetRow(row).GetCell(2).ToString();
                        }

                        paso = 4;
                        //D
                        if (sheet.GetRow(row).GetCell(3) == null)
                        {
                            productoStaging.codigoProveedor = null;
                        }
                        else
                        {
                            productoStaging.codigoProveedor = sheet.GetRow(row).GetCell(3).ToString();
                        }

                        paso = 5;
                        //E
                        if (sheet.GetRow(row).GetCell(4) == null)
                        {
                            productoStaging.unidad = null;
                        }
                        else
                        {
                            productoStaging.unidad = sheet.GetRow(row).GetCell(4).ToString();
                        }

                        paso = 6;
                        //F
                        if (sheet.GetRow(row).GetCell(5) == null)
                        {
                            productoStaging.unidadProveedor = null;
                        }
                        else
                        {
                            productoStaging.unidadProveedor = sheet.GetRow(row).GetCell(5).ToString();
                        }


                        paso = 7;
                        //G
                        if (sheet.GetRow(row).GetCell(6) == null)
                        {
                            productoStaging.equivalenciaProveedor = 0;
                        }
                        else
                        {
                            productoStaging.equivalenciaProveedor = Int32.Parse(sheet.GetRow(row).GetCell(6).ToString());
                        }

                        
                        paso = 8;
                        //H
                        if (sheet.GetRow(row).GetCell(7) == null)
                        {
                            productoStaging.unidad = null;
                        }
                        else
                        {
                            productoStaging.unidadAlternativa = sheet.GetRow(row).GetCell(7).ToString();
                        }

                        paso = 9;
                        //J
                        if (sheet.GetRow(row).GetCell(8) == null)
                        {
                            productoStaging.equivalencia = 1;
                        }
                        else
                        {
                            productoStaging.equivalencia = Int32.Parse(sheet.GetRow(row).GetCell(8).ToString());
                        }

                        paso = 10;
                        //K
                        if (sheet.GetRow(row).GetCell(9) == null)
                        {
                            productoStaging.descripcion = null;
                        }
                        else
                        {
                            productoStaging.descripcion = sheet.GetRow(row).GetCell(9).ToString();
                        }


                        paso = 11;
                        //S
                        try
                        {
                            productoStaging.monedaProveedor = sheet.GetRow(row).GetCell(18).ToString();
                        }
                        catch (Exception e)
                        {
                            productoStaging.monedaProveedor = "S";
                        }



                        paso = 12;
                        //T
                        try
                        {
                            Double? costo = sheet.GetRow(row).GetCell(19).NumericCellValue;
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
                            productoStaging.monedaMP = sheet.GetRow(row).GetCell(23).ToString();
                        }
                        catch (Exception e)
                        {
                            productoStaging.monedaMP = "S";
                        }

                        paso = 14;
                        try
                        {
                            //Y
                            Double? precioLima = sheet.GetRow(row).GetCell(24).NumericCellValue;
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
                            Double? precioProvincias = sheet.GetRow(row).GetCell(27).NumericCellValue;
                            productoStaging.precioProvincias = Convert.ToDecimal(precioProvincias);
                        }
                        catch (Exception e)
                        {
                            productoStaging.precioProvincias = 0;
                        }


                        paso = 16;
                        //AC
                        if (sheet.GetRow(row).GetCell(28) == null)
                        {
                            productoStaging.unidadSunat = "NIU";
                        }
                        else
                        {
                            productoStaging.unidadSunat = sheet.GetRow(row).GetCell(28).ToString();
                        }

                        paso = 17;
                        //AD
                        if (sheet.GetRow(row).GetCell(29) == null)
                        {
                            productoStaging.unidadAlternativaSunat = "NIU";
                        }
                        else
                        {
                            productoStaging.unidadAlternativaSunat = sheet.GetRow(row).GetCell(29).ToString();
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
            //productoBL.mergeProductoStaging();
            row = row;
            return RedirectToAction("Index", "Home");
        }

    }
}