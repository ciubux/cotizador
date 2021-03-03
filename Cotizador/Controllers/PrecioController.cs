﻿using BusinessLayer;
using Model;
using Model.UTILES;
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
    public class PrecioController : Controller
    {
        // GET: Precio


        public String GetPreciosRegistrados()
        {
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());

            IDocumento documento = (IDocumento)this.Session[Request["controller"].ToString()];
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            //Para recuperar el producto se envia si la sede seleccionada es provincia o no
            PrecioClienteProductoBL precioClienteProductoBL = new PrecioClienteProductoBL();
            List<PrecioClienteProducto> precioClienteProductoList = precioClienteProductoBL.getPreciosRegistrados(idProducto, idCliente);
            String nombreProducto = String.Empty;
            String skuProducto = String.Empty;
          

            foreach (PrecioClienteProducto precio in precioClienteProductoList)
            {
                if (precio.numeroCotizacion == null)
                {
                    precio.numeroCotizacion = "No Identificado";
                } else
                {
                    precio.numeroCotizacion = precio.numeroCotizacion + " (" + precio.tipoCotizacion + ")";
                }
            }

            ProductoBL productoBL = new ProductoBL();
            Producto producto = productoBL.getProductoById(idProducto);

            nombreProducto = producto.descripcion;
            skuProducto = producto.sku;

            String jsonPrecioLista = JsonConvert.SerializeObject(precioClienteProductoList);

            String json = "{\"sku\":\"" + skuProducto + "\", \"nombre\":\"" + nombreProducto +
                "\", \"unidad\":\"" + producto.unidad + "\", \"precio\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioSinIgv) + "\", \"precioProvincia\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioProvinciaSinIgv) +
                "\", \"unidadAlternativa\":\"" + producto.unidad_alternativa + "\", \"precioAlternativa\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioAlternativo) + "\", \"precioProvinciaAlternativa\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioProvinciaAlternativo) +
                "\", \"unidadProveedor\":\"" + producto.unidadProveedor + "\", \"precioProveedor\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioProveedor) + "\", \"precioProvinciaProveedor\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioProvinciaProveedor);

            if (usuario.visualizaCostos)
            {
                json = json + "\", \"costo\":\"" + String.Format(Constantes.formatoDosDecimales, producto.costoSinIgv) + "\", \"costoAlternativa\":\"" + String.Format(Constantes.formatoDosDecimales, producto.costoAlternativo) + "\", \"costoProveedor\":\"" + String.Format(Constantes.formatoDosDecimales, producto.costoProveedor);
            }

            json = json + "\", \"precioLista\": " + jsonPrecioLista + "}";

            return json;
        }

        public String GetPreciosRegistradosGrupoCliente()
        {
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            int idGrupoCliente = int.Parse(Request["idGrupoCliente"].ToString());
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];


            //Para recuperar el producto se envia si la sede seleccionada es provincia o no
            PrecioClienteProductoBL precioClienteProductoBL = new PrecioClienteProductoBL();
            ProductoBL productoBL = new ProductoBL();
            List<PrecioClienteProducto> precioGrupoClienteProductoList = precioClienteProductoBL.getPreciosRegistradosGrupo(idProducto, idGrupoCliente);
            Producto producto = productoBL.getProductoById(idProducto);
            String nombreProducto = producto.descripcion;
            String skuProducto = producto.sku;

            foreach (PrecioClienteProducto precio in precioGrupoClienteProductoList)
            {
                if (precio.numeroCotizacion == null)
                {
                    precio.numeroCotizacion = "No Identificado";
                }
                else
                {
                    precio.numeroCotizacion = precio.numeroCotizacion + " (Grupal)";
                }
            }

            String jsonPrecioLista = JsonConvert.SerializeObject(precioGrupoClienteProductoList);

            String json = "{\"sku\":\"" + skuProducto + "\", \"nombre\":\"" + nombreProducto +
                "\", \"unidad\":\"" + producto.unidad + "\", \"precio\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioSinIgv) + "\", \"precioProvincia\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioProvinciaSinIgv) +
                "\", \"unidadAlternativa\":\"" + producto.unidad_alternativa + "\", \"precioAlternativa\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioAlternativo) + "\", \"precioProvinciaAlternativa\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioProvinciaAlternativo) +
                "\", \"unidadProveedor\":\"" + producto.unidadProveedor + "\", \"precioProveedor\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioProveedor) + "\", \"precioProvinciaProveedor\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioProvinciaProveedor);
            
            if (usuario.visualizaCostos)
            {
                json = json + "\", \"costo\":\"" + String.Format(Constantes.formatoDosDecimales, producto.costoSinIgv) + "\", \"costoAlternativa\":\"" + String.Format(Constantes.formatoDosDecimales, producto.costoAlternativo) + "\", \"costoProveedor\":\"" + String.Format(Constantes.formatoDosDecimales, producto.costoProveedor);
            }

            json = json + "\", \"precioLista\": " + jsonPrecioLista + "}";

            return json;
        }


        public String GetPreciosRegistradosCliente()
        {
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];


            //Para recuperar el producto se envia si la sede seleccionada es provincia o no
            PrecioClienteProductoBL precioClienteProductoBL = new PrecioClienteProductoBL();
            ProductoBL productoBL = new ProductoBL();
            List<PrecioClienteProducto> precioClienteProductoList = precioClienteProductoBL.getPreciosRegistrados(idProducto, idCliente);
            Producto producto = productoBL.getProductoById(idProducto);
            String nombreProducto = producto.descripcion;
            String skuProducto = producto.sku;

            foreach (PrecioClienteProducto precio in precioClienteProductoList)
            {
                if (precio.numeroCotizacion == null)
                {
                    precio.numeroCotizacion = "No Identificado";
                }
                else
                {
                    precio.numeroCotizacion = precio.numeroCotizacion + " (" + precio.tipoCotizacion + ")";
                }
            }

            String jsonPrecioLista = JsonConvert.SerializeObject(precioClienteProductoList);

            String json = "{\"sku\":\"" + skuProducto + "\", \"nombre\":\"" + nombreProducto +
                "\", \"unidad\":\"" + producto.unidad + "\", \"precio\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioSinIgv) +
                "\", \"unidadAlternativa\":\"" + producto.unidad_alternativa + "\", \"precioAlternativa\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioAlternativo) +
                "\", \"unidadProveedor\":\"" + producto.unidadProveedor + "\", \"precioProveedor\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioProveedor);

            if (usuario.visualizaCostos)
            {
                json = json + "\", \"costo\":\"" + String.Format(Constantes.formatoDosDecimales, producto.costoSinIgv) + "\", \"costoAlternativa\":\"" + String.Format(Constantes.formatoDosDecimales, producto.costoAlternativo) + "\", \"costoProveedor\":\"" + String.Format(Constantes.formatoDosDecimales, producto.costoProveedor);
            }

            json = json + "\", \"precioLista\": " + jsonPrecioLista + "}";

            return json;
        }
        public String GetPreciosRegistradosVenta()
        {
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());

            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA];
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];


            IDocumento documento = (IDocumento)venta.pedido;

            //Para recuperar el producto se envia si la sede seleccionada es provincia o no
            PrecioClienteProductoBL precioClienteProductoBL = new PrecioClienteProductoBL();
            List<PrecioClienteProducto> precioClienteProductoList = precioClienteProductoBL.getPreciosRegistrados(idProducto, idCliente);
            String nombreProducto = String.Empty;
            String skuProducto = String.Empty;
            foreach (DocumentoDetalle documentoDetalle in documento.documentoDetalle)
            {
                if (documentoDetalle.producto.idProducto == idProducto)
                {
                    nombreProducto = documentoDetalle.producto.descripcion;
                    skuProducto = documentoDetalle.producto.sku;
                    break;
                }
            }

            ProductoBL productoBL = new ProductoBL();
            Producto producto = productoBL.getProductoById(idProducto);

            String jsonPrecioLista = JsonConvert.SerializeObject(precioClienteProductoList);

            String json = "{\"sku\":\"" + skuProducto + "\", \"nombre\":\"" + nombreProducto +
                "\", \"unidad\":\"" + producto.unidad + "\", \"precio\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioSinIgv) +
                "\", \"unidadAlternativa\":\"" + producto.unidad_alternativa + "\", \"precioAlternativa\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioAlternativo) +
                "\", \"unidadProveedor\":\"" + producto.unidadProveedor + "\", \"precioProveedor\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioProveedor);

            if (usuario.visualizaCostos)
            {
                json = json + "\", \"costo\":\"" + String.Format(Constantes.formatoDosDecimales, producto.costoSinIgv) + "\", \"costoAlternativa\":\"" + String.Format(Constantes.formatoDosDecimales, producto.costoAlternativo) + "\", \"costoProveedor\":\"" + String.Format(Constantes.formatoDosDecimales, producto.costoProveedor);
            }

            json = json + "\", \"precioLista\": " + jsonPrecioLista + "}";

            return json;
        }

        public String GetPreciosRegistradosVentaVer()
        {
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());

            Venta venta = (Venta)this.Session[Constantes.VAR_SESSION_VENTA_VER];



            IDocumento documento = (IDocumento)venta.pedido;

            //Para recuperar el producto se envia si la sede seleccionada es provincia o no
            PrecioClienteProductoBL precioClienteProductoBL = new PrecioClienteProductoBL();
            List<PrecioClienteProducto> precioClienteProductoList = precioClienteProductoBL.getPreciosRegistrados(idProducto, idCliente);
            String nombreProducto = String.Empty;
            String skuProducto = String.Empty;
            foreach (DocumentoDetalle documentoDetalle in documento.documentoDetalle)
            {
                if (documentoDetalle.producto.idProducto == idProducto)
                {
                    nombreProducto = documentoDetalle.producto.descripcion;
                    skuProducto = documentoDetalle.producto.sku;
                    break;
                }
            }

            ProductoBL productoBL = new ProductoBL();
            Producto producto = productoBL.getProductoById(idProducto);

            String jsonPrecioLista = JsonConvert.SerializeObject(precioClienteProductoList);

            String json = "{\"sku\":\"" + skuProducto + "\", \"nombre\":\"" + nombreProducto +
                "\", \"unidad\":\"" + producto.unidad + "\", \"precio\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioSinIgv) +
                "\", \"unidadAlternativa\":\"" + producto.unidad_alternativa + "\", \"precioAlternativa\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioAlternativo) +
                "\", \"unidadProveedor\":\"" + producto.unidadProveedor + "\", \"precioProveedor\":\"" + String.Format(Constantes.formatoDosDecimales, producto.precioProveedor) + 
                "\", \"precioLista\": " + jsonPrecioLista + "}";

            return json;
        }



        public ActionResult LogPrecioProducto()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            ViewBag.usuario = usuario;

            return PartialView();
        }


        public string GetLogPrecioProducto()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());

            ProductoBL bl = new ProductoBL();
            List<LogRegistroCampos> historial = bl.GetHistorialPreciosProductos(idProducto);
            
            if (!usuario.visualizaCostos)
            {
                List<LogRegistroCampos> temp = new List<LogRegistroCampos>();
                foreach (LogRegistroCampos log in historial)
                {
                    log.dato3 = "";
                    if ((log.dato1 == null || log.dato1.Trim().Equals("")) && (log.dato2 == null || log.dato2.Trim().Equals("")))
                    {
                        temp.Add(log);
                    }
                }

                foreach (LogRegistroCampos log in temp)
                {
                    historial.Remove(log);
                }
            }
            String jsonHistorial = JsonConvert.SerializeObject(historial);

            String json = "{\"mostrarCostos\":" + (usuario.visualizaCostos ? "1" : "0") + ", \"historial\": " + jsonHistorial + "}";

            return json;
        }


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

            PrecioBL precioBL = new PrecioBL();

            hssfwb = new HSSFWorkbook(file.InputStream);

            ISheet sheet = hssfwb.GetSheetAt(0);
            int row = 1;
            //sheet.LastRowNum

            int cantidad = Int32.Parse(Request["cantidadRegistros"].ToString());
            String sede = Request["sede"].ToString();

            precioBL.truncatePrecioStaging(sede);

            if (cantidad == 0)
                cantidad = sheet.LastRowNum;

            for (row = 6; row <= cantidad; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    int paso = 0;
                    try
                    {
                        PrecioStaging precioStaging = new PrecioStaging();
                        //B
                        paso = 1;
                        precioStaging.fecha = sheet.GetRow(row).GetCell(1).DateCellValue;
                        //C
                        paso = 2;
                        try
                        {
                            precioStaging.codigoCliente = sheet.GetRow(row).GetCell(2).StringCellValue;
                        }
                        catch (Exception ex)
                        {
                            precioStaging.codigoCliente = sheet.GetRow(row).GetCell(2).NumericCellValue.ToString();
                        }
                        //D
                        paso = 3;
                        try
                        {
                            precioStaging.sku = sheet.GetRow(row).GetCell(3).StringCellValue;
                        }
                        catch (Exception ex)
                        {
                            precioStaging.sku = sheet.GetRow(row).GetCell(3).NumericCellValue.ToString();
                        }
                        //E
                        paso = 4;
                        if (sheet.GetRow(row).GetCell(4) == null)
                        {
                            precioStaging.moneda = "S";
                        }
                        else
                        {
                            precioStaging.moneda = sheet.GetRow(row).GetCell(4).StringCellValue;
                        }
                        //F
                        paso = 5;
                        Double? precio = sheet.GetRow(row).GetCell(5).NumericCellValue;
                        precioStaging.precio = Convert.ToDecimal(precio);
                        //G
                        paso = 6;
                        if (sheet.GetRow(row).GetCell(6) == null)
                        {
                            precioStaging.codigoVendedor = null;
                        }
                        else
                        {
                            precioStaging.codigoVendedor = sheet.GetRow(row).GetCell(6).StringCellValue;
                        }

                        paso = 7;
                        precioStaging.sede = sede;

                        precioBL.setPrecioStaging(precioStaging);

                    }
                    catch (Exception ex)
                    {
                        Usuario usuario = (Usuario)this.Session["usuario"];
                        Log log = new Log(ex.ToString() + " paso: " + paso, TipoLog.Error, usuario);
                        LogBL logBL = new LogBL();
                        logBL.insertLog(log);
                    }
                }
            }

            // clienteBL.mergeClienteStaging();
            row = row;
            return RedirectToAction("Index", "Home");

        }








        [HttpGet]
        public ActionResult LoadPrecioLista()
        {
            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();

        }


        [HttpPost]
        public ActionResult LoadPrecioListaFile(HttpPostedFileBase file)
        {
            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);
            }


            HSSFWorkbook hssfwb;

            PrecioListaBL precioListaBL = new PrecioListaBL();
            precioListaBL.truncatePrecioListaStaging();

            hssfwb = new HSSFWorkbook(file.InputStream);

            ISheet sheet = hssfwb.GetSheetAt(0);
            int row = 1;
            int cantidad = Int32.Parse(Request["cantidad"].ToString());
            if (cantidad == 0)
                cantidad = sheet.LastRowNum;

            cantidad = 1550;

            for (row = 2; row <= cantidad; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {

                    PrecioListaStaging precioListaStaging = new PrecioListaStaging();
                    int paso = 1;
                    try
                    {
                        //Codigo Cliente
                        paso = 2;
                        try
                        {
                            precioListaStaging.codigoCliente = sheet.GetRow(row).GetCell(0).StringCellValue;
                        }
                        catch
                        {
                            precioListaStaging.codigoCliente = sheet.GetRow(row).GetCell(0).ToString();
                        }

                        paso = 3;
                        //Fecha Vigencia Inicio
                        if (sheet.GetRow(row).GetCell(1) == null)
                        {
                            precioListaStaging.fechaVigenciaInicio = null;
                        }
                        else
                        {
                            precioListaStaging.fechaVigenciaInicio = sheet.GetRow(row).GetCell(1).DateCellValue;
                        }

                        //Fecha Vigencia Fin
                        paso = 4;
                        if (sheet.GetRow(row).GetCell(2) == null)
                        {
                            precioListaStaging.fechaVigenciaFin = null;
                        }
                        else
                        {
                            precioListaStaging.fechaVigenciaFin = sheet.GetRow(row).GetCell(2).DateCellValue;
                        }

                        //SKU
                        paso = 5;
                        precioListaStaging.sku = sheet.GetRow(row).GetCell(3).StringCellValue;
                        //Considerar Cantidades
                        paso = 6;
                        precioListaStaging.consideraCantidades = sheet.GetRow(row).GetCell(4).StringCellValue;
                        //Cantidad
                        paso = 7;
                        precioListaStaging.cantidad = Int32.Parse(sheet.GetRow(row).GetCell(5).ToString());
                        //Es Unidad Alternativa
                        paso = 8;
                        precioListaStaging.esAlternativa = sheet.GetRow(row).GetCell(6).StringCellValue;
                        //Unidad
                        paso = 9;
                        precioListaStaging.unidad = sheet.GetRow(row).GetCell(7).StringCellValue;
                        //Precio Lista
                        paso = 10;
                        Double? precioLista = sheet.GetRow(row).GetCell(11).NumericCellValue;
                        precioListaStaging.precioLista = Convert.ToDecimal(precioLista);
                        //Moneda
                        paso = 11;
                        if (sheet.GetRow(row).GetCell(12) == null)
                        {
                            precioListaStaging.moneda = null;
                        }
                        else
                        {
                            precioListaStaging.moneda = sheet.GetRow(row).GetCell(12).StringCellValue;
                        }
                        //Precio Neto
                        paso = 12;
                        Double? precioNeto = sheet.GetRow(row).GetCell(13).NumericCellValue;
                        precioListaStaging.precioNeto = Convert.ToDecimal(precioNeto);
                        //Precio costo
                        paso = 13;
                        Double? costo = sheet.GetRow(row).GetCell(14).NumericCellValue;
                        precioListaStaging.costo = Convert.ToDecimal(costo);
                        //Flete
                        paso = 14;
                        if (sheet.GetRow(row).GetCell(15) == null)
                        {
                            precioListaStaging.flete = null;
                        }
                        else
                        {
                            precioListaStaging.flete = sheet.GetRow(row).GetCell(15).StringCellValue;
                        }
                        //Precio porcentajeDescuento
                        paso = 15;
                        Double? porcentajeDescuento = sheet.GetRow(row).GetCell(16).NumericCellValue;
                        precioListaStaging.porcentajeDescuento = Convert.ToDecimal(porcentajeDescuento);
                        //grupo
                        if (sheet.GetRow(row).GetCell(17) == null)
                        {
                            precioListaStaging.grupo = null;
                        }
                        else
                        {
                            precioListaStaging.grupo = sheet.GetRow(row).GetCell(17).StringCellValue;
                        }

                        precioListaBL.setPrecioListaStaging(precioListaStaging);
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