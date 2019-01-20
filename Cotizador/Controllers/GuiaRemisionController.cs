using BusinessLayer;
using Cotizador.ExcelExport;
using Model;
using Model.EXCEPTION;
using Newtonsoft.Json;
using NPOI.HSSF.Model;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class GuiaRemisionController : Controller
    {

        private GuiaRemision GuiaRemisionSession
        {
            get
            {

                GuiaRemision guiaRemision = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaGuiasRemision: guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoGuiaRemision: guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA]; break;
                    case Constantes.paginas.BusquedaGuiasRemisionConsolidarFactura: guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_FACTURA_CONSOLIDADA]; break;
                }
                return guiaRemision;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaGuiasRemision: this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoGuiaRemision: this.Session[Constantes.VAR_SESSION_GUIA] = value; break;
                    case Constantes.paginas.BusquedaGuiasRemisionConsolidarFactura: this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_FACTURA_CONSOLIDADA] = value; break;
                }
            }
        }

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            return clienteBL.getCLientesBusqueda(data, guiaRemision.ciudadOrigen.idCiudad);
        }

        public String GetCliente()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            guiaRemision.pedido.cliente = clienteBl.getCliente(idCliente);
            String resultado = JsonConvert.SerializeObject(guiaRemision.pedido.cliente);
            this.GuiaRemisionSession = guiaRemision;
            return resultado;
        }

        public String GetGrupoCliente()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            int idGrupoCliente = int.Parse(Request["idGrupoCliente"].ToString());

            guiaRemision.pedido.idGrupoCliente = idGrupoCliente;

            this.GuiaRemisionSession = guiaRemision;
            return "";
        }

        public String GetMovimientosAlmacenExtornantes()
        {
            MovimientoAlmacen movimientoAlmacen = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_VER];
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            List<MovimientoAlmacen> movimientoAlmacenExtornanteList = movimientoAlmacenBL.GetMovimientosAlmacenExtornantes(movimientoAlmacen);
            String resultado = JsonConvert.SerializeObject(movimientoAlmacenExtornanteList);
            return resultado;
        }



        #region Busqueda Guias

        private void instanciarGuiaRemisionBusqueda()
        {
            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.motivoTrasladoBusqueda = GuiaRemision.motivosTrasladoBusqueda.Todos;
            guiaRemision.seguimientoMovimientoAlmacenSalida = new SeguimientoMovimientoAlmacenSalida();
            guiaRemision.seguimientoMovimientoAlmacenSalida.estado = SeguimientoMovimientoAlmacenSalida.estadosSeguimientoMovimientoAlmacenSalida.Enviado;
            guiaRemision.ciudadOrigen = new Ciudad();
            guiaRemision.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            guiaRemision.pedido = new Pedido();
            guiaRemision.pedido.cliente = new Cliente();
            guiaRemision.pedido.cliente.idCliente = Guid.Empty;
            guiaRemision.pedido.buscarSedesGrupoCliente = false;

            guiaRemision.fechaTrasladoDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
            guiaRemision.fechaTrasladoHasta = DateTime.Now.AddDays(0);
            guiaRemision.estaFacturado = true;

            this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] = guiaRemision;
        }

        public void CleanBusqueda()
        {
            instanciarGuiaRemisionBusqueda();
        }

        public ActionResult Index(Guid? idMovimientoAlmacen = null)
        {


            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoGuiaRemision;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] == null)
            {
                instanciarGuiaRemisionBusqueda();
            }

            if (this.Session[Constantes.VAR_SESSION_GUIA_LISTA] == null)
            {
                this.Session[Constantes.VAR_SESSION_GUIA_LISTA] = new List<GuiaRemision>();
            }

            GuiaRemision guiaRemisionSearch = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA];

            ViewBag.guiaRemision = guiaRemisionSearch;
            ViewBag.guiaRemisionList = this.Session[Constantes.VAR_SESSION_GUIA_LISTA];
            ViewBag.pagina = (int)Constantes.paginas.BusquedaGuiasRemision;

            ViewBag.fechaTrasladoDesde = guiaRemisionSearch.fechaTrasladoDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaTrasladoHasta = guiaRemisionSearch.fechaTrasladoHasta.ToString(Constantes.formatoFecha);
            ViewBag.Si = Constantes.MENSAJE_SI;
            ViewBag.No = Constantes.MENSAJE_NO;

            int existeCliente = 0;
            if (guiaRemisionSearch.pedido.cliente.idCliente != Guid.Empty)
            {
                existeCliente = 1;
            }

            Pedido pedido = new Pedido();

            ViewBag.pedido = pedido;

            DocumentoVenta documentoVenta = new DocumentoVenta();
            ViewBag.documentoVenta = documentoVenta;

            ViewBag.existeCliente = existeCliente;


            /*     int mostrarGuia = 0;
                 if (idMovimientoAlmacen != null)
                 {
                     mostrarGuia = 1;
                 }

                 ViewBag.mostrarGuia = mostrarGuia;*/
            ViewBag.idMovimientoAlmacen = idMovimientoAlmacen;

            return View();
        }

        public void UpdateMarcaNoEntregado()
        {
            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.idMovimientoAlmacen = Guid.Parse(this.Request.Params["idMovimientoAlmacen"]);
            guiaRemision.estaNoEntregado = Int32.Parse(this.Request.Params["noEntregado"])==1;
            guiaRemision.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            movimientoAlmacenBL.UpdateMarcaNoEntregado(guiaRemision);
        }


        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<GuiaRemision> list = (List<GuiaRemision>)this.Session[Constantes.VAR_SESSION_GUIA_LISTA];

            GuiaRemisionSearch excel = new GuiaRemisionSearch();
            return excel.generateExcel(list);
        }

        public String Search()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaGuiasRemision;

            //Se recupera el pedido Búsqueda de la session
            GuiaRemision guiaRemision = this.GuiaRemisionSession;

            String[] movDesde = this.Request.Params["fechaTrasladoDesde"].Split('/');
            guiaRemision.fechaTrasladoDesde = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]),0,0,0);

            String[] movHasta = this.Request.Params["fechaTrasladoHasta"].Split('/');
            guiaRemision.fechaTrasladoHasta = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);


            if (this.Request.Params["numeroDocumento"] == null || this.Request.Params["numeroDocumento"].Trim().Length == 0)
            {
                guiaRemision.numeroDocumento = 0;
            }
            else
            {
                guiaRemision.numeroDocumento = int.Parse(this.Request.Params["numeroDocumento"]);
            }

            if (this.Request.Params["numeroPedido"] == null || this.Request.Params["numeroPedido"].Trim().Length == 0)
            {
                guiaRemision.pedido.numeroPedido = 0;
            }
            else
            {
                guiaRemision.pedido.numeroPedido = int.Parse(this.Request.Params["numeroPedido"]);
            }

            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();


            List<GuiaRemision> guiaRemisionList = movimientoAlmacenBL.GetGuiasRemision(guiaRemision);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_GUIA_LISTA] = guiaRemisionList;
            this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] = guiaRemision;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(guiaRemisionList);
            //return pedidoList.Count();
        }

        #endregion



        #region Busqueda Factura Consolidada

        private void instanciarGuiaRemisionBusquedaFacturaConsolidada()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaGuiasRemisionConsolidarFactura;

            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.seguimientoMovimientoAlmacenSalida = new SeguimientoMovimientoAlmacenSalida();
            guiaRemision.seguimientoMovimientoAlmacenSalida.estado = SeguimientoMovimientoAlmacenSalida.estadosSeguimientoMovimientoAlmacenSalida.Enviado;
            guiaRemision.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            guiaRemision.ciudadOrigen = guiaRemision.usuario.sedesMP.Where(c => !c.esProvincia).FirstOrDefault();
            guiaRemision.pedido = new Pedido();
            guiaRemision.pedido.cliente = new Cliente();
            guiaRemision.pedido.cliente.idCliente = Guid.Empty;
            //Busca hasta 45 días atrás
            guiaRemision.fechaTrasladoDesde = DateTime.Now.AddDays(-45);
            guiaRemision.fechaTrasladoHasta = DateTime.Now.AddDays(1);
            this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_FACTURA_CONSOLIDADA] = guiaRemision;
        }     

        public void CleanBusquedaFacturaConsolidada()
        {
            instanciarGuiaRemisionBusquedaFacturaConsolidada();
        }
        
       


        public ActionResult ConsolidarFactura()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaGuiasRemisionConsolidarFactura;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_FACTURA_CONSOLIDADA] == null)
            {
                instanciarGuiaRemisionBusquedaFacturaConsolidada();
            }

            if (this.Session[Constantes.VAR_SESSION_GUIA_LISTA_FACTURA_CONSOLIDADA] == null)
            {
                this.Session[Constantes.VAR_SESSION_GUIA_LISTA_FACTURA_CONSOLIDADA] = new List<GuiaRemision>();
            }

            GuiaRemision guiaRemisionSearch = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_FACTURA_CONSOLIDADA];

            ViewBag.guiaRemision = guiaRemisionSearch;
            ViewBag.guiaRemisionList = this.Session[Constantes.VAR_SESSION_GUIA_LISTA_FACTURA_CONSOLIDADA];
            ViewBag.pagina = (int)Constantes.paginas.BusquedaGuiasRemisionConsolidarFactura;

            ViewBag.fechaTrasladoDesde = guiaRemisionSearch.fechaTrasladoDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaTrasladoHasta = guiaRemisionSearch.fechaTrasladoHasta.ToString(Constantes.formatoFecha);
            ViewBag.Si = Constantes.MENSAJE_SI;
            ViewBag.No = Constantes.MENSAJE_NO;

            int existeCliente = 0;
            if (guiaRemisionSearch.pedido.cliente.idCliente != Guid.Empty)
            {
                existeCliente = 1;
            }

            Pedido pedido = new Pedido();

            ViewBag.pedido = pedido;

            DocumentoVenta documentoVenta = new DocumentoVenta();
            ViewBag.documentoVenta = documentoVenta;

            ViewBag.existeCliente = existeCliente;
            //ViewBag.movimientoAlmacenIdList = this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS];
            return View();
        }




        public String SearchParaFacturaConsolidada()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaGuiasRemisionConsolidarFactura;
  
            //Se recupera el pedido Búsqueda de la session
            GuiaRemision guiaRemision = this.GuiaRemisionSession;

            String[] movDesde = this.Request.Params["fechaTrasladoDesde"].Split('/');
            guiaRemision.fechaTrasladoDesde = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]));

            String[] movHasta = this.Request.Params["fechaTrasladoHasta"].Split('/');
            guiaRemision.fechaTrasladoHasta = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);

            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();

            guiaRemision.estaAnulado = false;
            guiaRemision.estaFacturado = false;
            guiaRemision.numeroDocumento = 0;
        //    guiaRemision.pedido.numeroPedido  = 0;

            List<GuiaRemision> guiaRemisionList = movimientoAlmacenBL.GetGuiasRemisionGrupoCliente(guiaRemision);
            List<Guid>  movimientoAlmacenIdList =  new List<Guid>();


            this.Session[Constantes.VAR_SESSION_GUIA_CONSOLIDADA] = null;
            if (guiaRemisionList.Count > 0)
            {
                this.Session[Constantes.VAR_SESSION_GUIA_CONSOLIDADA] = guiaRemisionList[0];
            }

            foreach (GuiaRemision guiaRemisionId in guiaRemisionList)
            {
                movimientoAlmacenIdList.Add(guiaRemisionId.idMovimientoAlmacen);
            }
            this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS] = movimientoAlmacenIdList;


            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_GUIA_LISTA_FACTURA_CONSOLIDADA] = guiaRemisionList;
            this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_FACTURA_CONSOLIDADA] = guiaRemision;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(guiaRemisionList);
        }



        [HttpPost]
        public String consolidarAtenciones(List<MovimientoAlmacenJson> MovimientoAlmacenJsonList)
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_FACTURA_CONSOLIDADA];

            String idMovimientoAlmacenList = " '";
            List<Guid> movimientoAlmacenIdList = new List<Guid>();

            foreach (MovimientoAlmacenJson movimientoAlmacenJson in MovimientoAlmacenJsonList)
            {

                idMovimientoAlmacenList = idMovimientoAlmacenList + movimientoAlmacenJson.idMovimientoAlmacen+"','";
                movimientoAlmacenIdList.Add(Guid.Parse(movimientoAlmacenJson.idMovimientoAlmacen));
            }

            idMovimientoAlmacenList = idMovimientoAlmacenList.Substring(0, idMovimientoAlmacenList.Length - 2);

            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS] = movimientoAlmacenIdList;            
            DocumentoVenta documentoVenta = movimientoAlmacenBL.obtenerResumenConsolidadoAtenciones(idMovimientoAlmacenList);
            this.Session[Constantes.VAR_SESSION_RESUMEN_CONSOLIDADO] = documentoVenta;
            String resultado = JsonConvert.SerializeObject(documentoVenta);
            return resultado;

            /*this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
            return "{\"cantidad\":\"" + guiaRemision.pedido.documentoDetalle.Count + "\"}";*/
        }


        public String validarPreciosVentaConsolidada()
        {
            String idMovimientoAlmacenList = " '";
            List<Guid> movimientoAlmacenIdList = new List<Guid>();

            List<Guid> guidMovimientoAlmacenList = (List<Guid>)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS];
            foreach (Guid idMovimientoAlmacen in guidMovimientoAlmacenList)
            {

                idMovimientoAlmacenList = idMovimientoAlmacenList + idMovimientoAlmacen + "','";
            }
            idMovimientoAlmacenList = idMovimientoAlmacenList.Substring(0, idMovimientoAlmacenList.Length - 2);


            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            List<GuiaRemision> guiaRemisionList = movimientoAlmacenBL.obtenerDetalleConsolidadoAtenciones(idMovimientoAlmacenList);

            DocumentoVenta documentoVenta = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_RESUMEN_CONSOLIDADO];

            List<GuiaRemision> guiaRemisionListBusqueda = (List<GuiaRemision>)this.Session[Constantes.VAR_SESSION_GUIA_LISTA_FACTURA_CONSOLIDADA];
            this.Session[Constantes.VAR_SESSION_GUIA_CONSOLIDADA] = guiaRemisionListBusqueda.Where(g => g.idMovimientoAlmacen == guiaRemisionList[0].idMovimientoAlmacen).FirstOrDefault();

            List<Producto> productoList = new List<Producto>();


            foreach (VentaDetalle ventaDetalle in documentoVenta.ventaDetalleList)
            {
                Producto producto = new Producto();
                producto.idProducto = ventaDetalle.producto.idProducto;
                productoList.Add(producto);
            }

            Boolean existeDiferenciaDePrecios = false;

            //Se recorren las guias
            foreach (GuiaRemision guiaRemision in guiaRemisionList)
            {
                //Se recorren los productos para buscar si existe en la guía
                foreach (Producto producto in  productoList)
                {
                     DocumentoDetalle documentoDetalle = guiaRemision.documentoDetalle.Where(d => d.producto.idProducto == producto.idProducto).FirstOrDefault();
                    if (documentoDetalle != null)
                    {
                        if (producto.precioSinIgv == 0)
                        {
                            producto.precioSinIgv = documentoDetalle.precioNeto;
                        }
                        else if (documentoDetalle.precioNeto != producto.precioSinIgv)
                        {
                            existeDiferenciaDePrecios = true;
                            break;

                        }
                        
                    }

                }

                if (existeDiferenciaDePrecios)
                {
                    break;
                }

            }

            String resultado = String.Empty;
            if (existeDiferenciaDePrecios)
                resultado = "Existe Diferencia Precios, descargue el reporte detallado de las atenciones haciendo clic en el botón Generar Reporte Detallado.";
            return resultado;
        }





        [HttpGet]
        public ActionResult obtenerDetalleAtenciones()
        {
            //GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_FACTURA_CONSOLIDADA];

            String serieUnidadesAlternativas = Request["serieUnidadesAlternativas"].ToString();


            DocumentoVenta documentoVenta = (DocumentoVenta)this.Session[Constantes.VAR_SESSION_RESUMEN_CONSOLIDADO];


            Dictionary<String, Boolean> mostrarUnidadAlternativaList = new Dictionary<string, bool>();

            //List<bool> mostrarUnidadAlternativaList = new List<bool>();
            int u = 0;
            foreach (char mostrarUnidadAlternativa in serieUnidadesAlternativas.ToCharArray())
            {
                mostrarUnidadAlternativaList.Add(documentoVenta.ventaDetalleList[u].producto.sku, mostrarUnidadAlternativa == '1');
                u++;
            }


            String idMovimientoAlmacenList = " '";
            List<Guid> movimientoAlmacenIdList = new List<Guid>();

            List<Guid> guidMovimientoAlmacenList = (List<Guid>)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA_LISTA_IDS];
            foreach (Guid idMovimientoAlmacen in guidMovimientoAlmacenList)
            {

                idMovimientoAlmacenList = idMovimientoAlmacenList + idMovimientoAlmacen + "','";
            }
            idMovimientoAlmacenList = idMovimientoAlmacenList.Substring(0, idMovimientoAlmacenList.Length - 2);


            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            List<GuiaRemision> guiaRemisionList = movimientoAlmacenBL.obtenerDetalleConsolidadoAtenciones(idMovimientoAlmacenList, mostrarUnidadAlternativaList);

            


      

            HSSFWorkbook wb;
          //  Dictionary<String, ICellStyle> styles = CreateExcelStyles(wb);
            HSSFSheet sheet;
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());


                HSSFFont titleFont = (HSSFFont)wb.CreateFont();
                titleFont.FontHeightInPoints = (short)11;
                titleFont.FontName = "Arial";
                titleFont.Color = IndexedColors.Black.Index;
                titleFont.IsBold = true;
              //  HSSFColor color = new HSSFColor(); // (new byte[] { 184, 212, 249 });
           //     color.RGB.SetValue(new byte[] { 184, 212, 249 },0);
                HSSFCellStyle titleCellStyle = (HSSFCellStyle)wb.CreateCellStyle();
                titleCellStyle.SetFont(titleFont);
                titleCellStyle.FillPattern = FillPattern.SolidForeground;
                titleCellStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index;
                
                IDataFormat format = wb.CreateDataFormat();
                ICellStyle dateFormatStyle = wb.CreateCellStyle();
                dateFormatStyle.DataFormat = format.GetFormat("yyyy-mm-dd");
        
                // create sheet
                sheet = (HSSFSheet)wb.CreateSheet("Atenciones");
               
                /*guiaRemision,fecha_emision, ma.direccion_entrega, ub.distrito, 
                 * ub.provincia,  ub.departamento, ma.observaciones,*/

                /*Cabecera, Sub total*/
                int rTotal = (guiaRemisionList.Count) + 4;
                int cTotal = 9 + (documentoVenta.ventaDetalleList.Count()*2);

                /*Se crean todas las celdas*/
                for (int r = 0; r < rTotal; r++)
                {
                    var row = sheet.CreateRow(r);
                    for (int c = 0; c < cTotal; c++)
                    {
                        row.CreateCell(c);
                    }
                }

                int i = 10;
                foreach (VentaDetalle ventaDetalle in documentoVenta.ventaDetalleList)
                {
                 
                    //¿Es alternativo?
                    if (mostrarUnidadAlternativaList[ventaDetalle.producto.sku])
                    {
                        UtilesHelper.setValorCelda(sheet, 1, i, ventaDetalle.producto.sku + " - " + ventaDetalle.producto.unidad_alternativa, titleCellStyle);
                    }
                    else
                    {
                        UtilesHelper.setValorCelda(sheet, 1, i, ventaDetalle.producto.sku + " - " + ventaDetalle.producto.unidad, titleCellStyle);
                    }

                    

                    UtilesHelper.setValorCelda(sheet, 2, i, "Cantidad", titleCellStyle);
                    UtilesHelper.setValorCelda(sheet, 2, i+1, "Precio (S/)", titleCellStyle);
                   

                    var mergeCell = new NPOI.SS.Util.CellRangeAddress(0, 0, i - 1, i);
                    sheet.AddMergedRegion(mergeCell);

                    i = i + 2;
                }

                UtilesHelper.setValorCelda(sheet, 2, "A", "Guía Remisión:", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "B", "N° Pedido:", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "C", "N° Grupo Pedido:", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "D", "Fecha", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "E", "Dirección Entrega", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "F", "Distrito", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "G", "Provincia", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "H", "Departamento", titleCellStyle);
                UtilesHelper.setValorCelda(sheet, 2, "I", "Observaciones", titleCellStyle);






                i = 3;

              /*  for (int iii = 0; iii<50;iii++)
                { */

                foreach (GuiaRemision guiaRemision in guiaRemisionList)
                {
                    UtilesHelper.setValorCelda(sheet, i, "A", guiaRemision.serieNumeroGuia);
                    UtilesHelper.setValorCelda(sheet, i, "B", guiaRemision.pedido.numeroPedidoString);
                    UtilesHelper.setValorCelda(sheet, i, "C", guiaRemision.pedido.numeroGrupoPedidoString);
                    UtilesHelper.setValorCelda(sheet, i, "D", guiaRemision.fechaEmision, dateFormatStyle);
                    UtilesHelper.setValorCelda(sheet, i, "E", guiaRemision.direccionEntrega);
                    UtilesHelper.setValorCelda(sheet, i, "F", guiaRemision.ubigeoEntrega.Distrito);
                    UtilesHelper.setValorCelda(sheet, i, "G", guiaRemision.ubigeoEntrega.Provincia);
                    UtilesHelper.setValorCelda(sheet, i, "H", guiaRemision.ubigeoEntrega.Departamento);
                    UtilesHelper.setValorCelda(sheet, i, "I", guiaRemision.observaciones);

                    int ic = 10;
                    foreach (VentaDetalle ventaDetalle in documentoVenta.ventaDetalleList)
                    {

                        DocumentoDetalle documentoDetalle = guiaRemision.documentoDetalle.Where(d => d.producto.idProducto == ventaDetalle.producto.idProducto).FirstOrDefault();

                        if (documentoDetalle != null)
                        {

                            // UtilesHelper.setValorCelda(sheet, 1, ic, ventaDetalle.producto.sku + " - " + ventaDetalle.producto.unidad);
                            UtilesHelper.setValorCelda(sheet, i, ic, Convert.ToDouble(documentoDetalle.cantidadDecimal));
                            UtilesHelper.setValorCelda(sheet, i, ic + 1, Convert.ToDouble(documentoDetalle.precioNeto) );
                        }
                        ic = ic + 2;
                    }


                    i++;
                }
                //}


                int flag = 0;
                for (int cf = 9; cf < cTotal; cf++)
                {
                    sheet.GetRow(i - 1).GetCell(cf).CellStyle = titleCellStyle;
                    sheet.GetRow(i - 1).GetCell(cf).SetCellType(CellType.Formula);
                    if (flag == 0)
                    {
                        sheet.GetRow(i - 1).GetCell(cf).CellFormula = "SUM(" + UtilesHelper.columnas[cf] + "3:" + UtilesHelper.columnas[cf] + (i - 1) + ")";
                        flag++;
                    }
                    else {
                        sheet.GetRow(i - 1).GetCell(cf).CellFormula = "AVERAGE(" + UtilesHelper.columnas[cf] + "3:" + UtilesHelper.columnas[cf] + (i - 1) + ")";
                        flag = 0;
                    }
                }
                i++;
                flag = 0;
                for (int cf = 9; cf < cTotal; cf++)
                {
                    sheet.GetRow(i - 1).GetCell(cf).CellStyle = titleCellStyle;
               
                    if (flag == 0)
                    {
                       // sheet.GetRow(i - 1).GetCell(cf).SetCellValue("SUB TOTAL:");
                        flag++;
                    }
                    else
                    {
                        sheet.GetRow(i - 1).GetCell(cf).SetCellType(CellType.Formula);
                        sheet.GetRow(i - 1).GetCell(cf).CellFormula = UtilesHelper.columnas[cf-1] + (i - 1) + "*" + UtilesHelper.columnas[cf] + (i - 1);
                        flag = 0;
                    }

                }





                MemoryStream ms = new MemoryStream();
                using (MemoryStream tempStream = new MemoryStream())
                {
                    wb.Write(tempStream);
                    var byteArray = tempStream.ToArray();
                    ms.Write(byteArray, 0, byteArray.Length);
                    ms.Flush();
                    ms.Position = 0;
                    FileStreamResult result = new FileStreamResult(ms, "application/vnd.ms-excel");

                    GuiaRemision guiaRemision = this.GuiaRemisionSession;
                    result.FileDownloadName = "Detalle de Atenciones - "+ guiaRemision.pedido.cliente.razonSocial + " .xls";

                
                  


                    return result;
                }

              

            }
            
        }


        #endregion




        #region Crear Guia

        public Boolean ConsultarSiExisteGuiaRemision()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
            if (guiaRemision == null)
            {
                return false;
            }
            else
                return true;
        }

        private void instanciarGuiaRemision()
        {
            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.fechaEmision = DateTime.Now;
            guiaRemision.fechaTraslado = DateTime.Now;
            guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.Venta;
            guiaRemision.transportista = new Transportista();
            guiaRemision.ciudadOrigen = new Ciudad();
            guiaRemision.ciudadOrigen.idCiudad = Guid.Empty;
            guiaRemision.pedido = new Pedido();
            guiaRemision.pedido.pedidoDetalleList = new List<PedidoDetalle>();
            guiaRemision.pedido.ciudad = new Ciudad();
            guiaRemision.pedido.ubigeoEntrega = new Ubigeo();
            guiaRemision.ciudadOrigen.transportistaList = new List<Transportista>();
            guiaRemision.seguimientoMovimientoAlmacenSalida = new SeguimientoMovimientoAlmacenSalida();
            guiaRemision.certificadoInscripcion = ".";
            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;

        }


        public void iniciarAtencionDesdePedido()
        {
            try
            {
                var tipo = Request.Params["tipo"];

                Pedido pedido = null;
                if ((Pedido.ClasesPedido)Char.Parse(tipo) == Pedido.ClasesPedido.Venta)
                {
                    pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_VER];
                }
                else if ((Pedido.ClasesPedido)Char.Parse(tipo) == Pedido.ClasesPedido.Compra)
                {
                    pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_COMPRA_VER];
                }
                else if ((Pedido.ClasesPedido)Char.Parse(tipo) == Pedido.ClasesPedido.Almacen)
                {
                    pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_ALMACEN_VER];
                }




                if (this.Session[Constantes.VAR_SESSION_GUIA] == null)
                {
                    instanciarGuiaRemision();
                }
                GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
                guiaRemision.pedido = pedido;

                if ((Pedido.ClasesPedido)Char.Parse(tipo) == Pedido.ClasesPedido.Venta)
                {
                    guiaRemision.motivoTraslado = (GuiaRemision.motivosTraslado)(char)pedido.tipoPedido;
                }
                else if ((Pedido.ClasesPedido)Char.Parse(tipo) == Pedido.ClasesPedido.Compra)
                {
                    guiaRemision.motivoTraslado = (GuiaRemision.motivosTraslado)(char)pedido.tipoPedidoCompra;
                }
                else if ((Pedido.ClasesPedido)Char.Parse(tipo) == Pedido.ClasesPedido.Almacen)
                {
                    guiaRemision.motivoTraslado = (GuiaRemision.motivosTraslado)(char)pedido.tipoPedidoAlmacen;
                }


                guiaRemision.transportista = new Transportista();
                // guiaRemision.ciudadOrigen = pedido.ciudad;

                /*  foreach (DocumentoDetalle documentoDetalle in guiaRemision.pedido.documentoDetalle)
                  {
                      documentoDetalle.cantidadPendienteAtencion = documentoDetalle.cantidad;
                      documentoDetalle.cantidadPorAtender = documentoDetalle.cantidad;
                  }*/
                guiaRemision.observaciones = String.Empty;
                bool existeOrdenCompra = false;
                if (pedido.numeroReferenciaCliente != null && !pedido.numeroReferenciaCliente.Trim().Equals(String.Empty))
                {
                    if (pedido.numeroReferenciaCliente.Contains("O/C"))
                    {
                        guiaRemision.observaciones = pedido.numeroReferenciaCliente.Trim();
                    }
                    else
                    {
                        guiaRemision.observaciones = "O/C N° " + pedido.numeroReferenciaCliente.Trim();
                    }
                    existeOrdenCompra = true;
                }
                if (pedido.observacionesGuiaRemision != null && !pedido.observacionesGuiaRemision.Trim().Equals(String.Empty))
                {
                    if (existeOrdenCompra)
                    {
                        guiaRemision.observaciones = guiaRemision.observaciones + " / ";
                    }
                    guiaRemision.observaciones = guiaRemision.observaciones + pedido.observacionesGuiaRemision;
                }
                
                    

                CiudadBL ciudadBL = new CiudadBL();
                Ciudad ciudadOrigen = ciudadBL.getCiudad(pedido.ciudad.idCiudad);
                guiaRemision.ciudadOrigen = ciudadOrigen;

                guiaRemision.transportista = new Transportista();
                guiaRemision.serieDocumento = ciudadOrigen.serieGuiaRemision;
                guiaRemision.numeroDocumento = ciudadOrigen.siguienteNumeroGuiaRemision;
                TransportistaBL transportistaBL = new TransportistaBL();
                guiaRemision.ciudadOrigen.transportistaList = transportistaBL.getTransportistas(pedido.ciudad.idCiudad);
                guiaRemision.documentoDetalle = guiaRemision.pedido.documentoDetalle;





            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }

        }

        public void iniciarAtencionDesdeNotaIngreso()
        {
            try
            {
                instanciarGuiaRemision();
                GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];

                //new NotaIngreso();
                //notaIngreso.tipoNotaIngreso = (NotaIngreso.TiposNotaIngreso)Char.Parse(Request.Params["tipoNotaIngreso"]);
                //notaIngreso.guiaRemisionAExtornar = new GuiaRemision();
                
                guiaRemision.notaIngresoAExtornar = (NotaIngreso)this.Session[Constantes.VAR_SESSION_NOTA_INGRESO_VER];

                guiaRemision.pedido = guiaRemision.notaIngresoAExtornar.pedido;

                ClienteBL clienteBL = new ClienteBL();            
                //Revisar si es necesario recuperar el cliente
                guiaRemision.pedido.cliente = clienteBL.getCliente(guiaRemision.pedido.cliente.idCliente);

                //Se obtiene la lista de direccioines de entrega registradas para el cliente
                DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
                guiaRemision.pedido.cliente.direccionEntregaList = direccionEntregaBL.getDireccionesEntrega(guiaRemision.pedido.cliente.idCliente);

                SolicitanteBL solicitanteBL = new SolicitanteBL();
                guiaRemision.pedido.cliente.solicitanteList = solicitanteBL.getSolicitantes(guiaRemision.pedido.cliente.idCliente);

                guiaRemision.pedido.direccionEntrega = new DireccionEntrega();

                //Se limpia el ubigeo de entrega
                guiaRemision.pedido.ubigeoEntrega = new Ubigeo();
                guiaRemision.pedido.ubigeoEntrega.Id = Constantes.UBIGEO_VACIO;

              

                if (guiaRemision.notaIngresoAExtornar.motivoTraslado == NotaIngreso.motivosTraslado.Compra)
                {
                    guiaRemision.pedido.clasePedido = Pedido.ClasesPedido.Compra;
                    guiaRemision.pedido.tipoPedidoCompra = Pedido.tiposPedidoCompra.Compra;
                    guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.DevolucionCompra;
                }
                else if (guiaRemision.notaIngresoAExtornar.motivoTraslado == NotaIngreso.motivosTraslado.ComodatoRecibido)
                {
                    guiaRemision.pedido.clasePedido = Pedido.ClasesPedido.Compra;
                    guiaRemision.pedido.tipoPedido = Pedido.tiposPedido.ComodatoEntregado;
                    guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.DevolucionComodatoRecibido;
                }
                else if (guiaRemision.notaIngresoAExtornar.motivoTraslado == NotaIngreso.motivosTraslado.TransferenciaGratuitaRecibida)
                {
                    guiaRemision.pedido.clasePedido = Pedido.ClasesPedido.Compra;
                    guiaRemision.pedido.tipoPedido = Pedido.tiposPedido.TransferenciaGratuitaEntregada;
                    guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.DevolucionCompra;
                }
                else if (guiaRemision.notaIngresoAExtornar.motivoTraslado == NotaIngreso.motivosTraslado.PrestamoRecibido)
                {
                    guiaRemision.pedido.clasePedido = Pedido.ClasesPedido.Almacen;
                    guiaRemision.pedido.tipoPedidoAlmacen = Pedido.tiposPedidoAlmacen.PrestamoRecibido;
                    guiaRemision.motivoTraslado = GuiaRemision.motivosTraslado.DevolucionPrestamoRecibido;
                }


                guiaRemision.observaciones = String.Empty;
                guiaRemision.documentoDetalle = guiaRemision.notaIngresoAExtornar.documentoDetalle;

                foreach (DocumentoDetalle documentoDetalle in guiaRemision.documentoDetalle)
                {
                    documentoDetalle.cantidadPendienteAtencion = documentoDetalle.cantidadSolicitada;
                    documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadSolicitada;
                }



              CiudadBL ciudadBL = new CiudadBL();
                Ciudad ciudadDestino = ciudadBL.getCiudad(guiaRemision.notaIngresoAExtornar.ciudadDestino.idCiudad);
                ciudadDestino.direccionPuntoLlegada = ciudadDestino.direccionPuntoPartida;
                guiaRemision.ciudadOrigen = ciudadDestino;
                guiaRemision.transportista = new Transportista();
                guiaRemision.serieDocumento = ciudadDestino.serieGuiaRemision;
                guiaRemision.numeroDocumento = ciudadDestino.siguienteNumeroGuiaRemision;
                TransportistaBL transportistaBL = new TransportistaBL();
                guiaRemision.ciudadOrigen.transportistaList = transportistaBL.getTransportistas(guiaRemision.notaIngresoAExtornar.ciudadDestino.idCiudad);
                                
                

                this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }
        }
        public ActionResult Guiar()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoGuiaRemision;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                if (!usuario.creaGuias)
                {
                    return RedirectToAction("Login", "Account");
                }
            }



            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoGuiaRemision;
            try
            {
                if (this.Session[Constantes.VAR_SESSION_GUIA] == null)
                {
                    return View("GuiarEmpty");
                    //instanciarGuiaRemision();
                }
                GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];

                ViewBag.fechaTrasladotmp = guiaRemision.fechaTraslado.ToString(Constantes.formatoFecha);
                ViewBag.fechaEmisiontmp = guiaRemision.fechaEmision.ToString(Constantes.formatoFecha);
                ViewBag.guiaRemision = guiaRemision;
            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }

            ViewBag.pagina = (int)Constantes.paginas.MantenimientoGuiaRemision;
            return View();
        }


        public String Create()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoGuiaRemision;

            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());


            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            guiaRemision.usuario = usuario;

            String error = String.Empty;
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            try
            {
                movimientoAlmacenBL.InsertMovimientoAlmacenSalida(guiaRemision);
            }
            catch (DuplicateNumberDocumentException ex)
            {
                error = ex.Message;
            }

            long numeroGuiaRemision = guiaRemision.numero;
            Guid idGuiaRemision = guiaRemision.idMovimientoAlmacen;
            String serieNumeroGuia = guiaRemision.serieNumeroGuia;

            int estado = (int)guiaRemision.seguimientoMovimientoAlmacenSalida.estado;

            String jsonGuiaRemisionValidacion = JsonConvert.SerializeObject(guiaRemision.guiaRemisionValidacion);

            if(guiaRemision.guiaRemisionValidacion.tipoErrorValidacion == GuiaRemisionValidacion.TiposErrorValidacion.NoExisteError)
            { 

                this.GuiaRemisionSession = null;
            }

            String resultado = "{ \"serieNumeroGuia\":\"" + serieNumeroGuia + "\", \"idGuiaRemision\":\"" + idGuiaRemision + "\", \"error\":\"" + error + "\",     \"guiaRemisionValidacion\": " + jsonGuiaRemisionValidacion + "  }";
            return resultado;
        }

        public String CreateTransportistaTemporal()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            Transportista transportista = new Transportista();
            transportista.descripcion = Request["descripcion"];
            transportista.direccion = Request["direccion"];
            transportista.ruc = Request["ruc"];
            transportista.telefono = Request["telefono"];
            transportista.idTransportista = Guid.Empty;
            GuiaRemisionSession.ciudadOrigen.transportistaList.Add(transportista);
            GuiaRemisionSession.transportista = transportista;
            this.GuiaRemisionSession = guiaRemision;
            return JsonConvert.SerializeObject(transportista);
        }

        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> documentoDetalleList)
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];

            foreach (DocumentoDetalleJson documentoDetalleJson in documentoDetalleList)
            {
                DocumentoDetalle documentoDetalle = guiaRemision.documentoDetalle.Where(d => d.producto.idProducto == Guid.Parse(documentoDetalleJson.idProducto)).FirstOrDefault();
                documentoDetalle.cantidadPorAtender = documentoDetalleJson.cantidad;
            }
            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
            return "{\"cantidad\":\"" + guiaRemision.pedido.documentoDetalle.Count + "\"}";
        }

         public string ChangeDireccionEntrega()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];

            if (this.Request.Params["idDireccionEntrega"] == null || this.Request.Params["idDireccionEntrega"].Equals(String.Empty))
            {
                guiaRemision.pedido.direccionEntrega = new DireccionEntrega();
            }
            else
            { 
                Guid idDireccionEntrega = Guid.Parse(this.Request.Params["idDireccionEntrega"]);
                guiaRemision.pedido.direccionEntrega = guiaRemision.pedido.cliente.direccionEntregaList.Where(d => d.idDireccionEntrega == idDireccionEntrega).FirstOrDefault();
                guiaRemision.pedido.ubigeoEntrega = guiaRemision.pedido.direccionEntrega.ubigeo;

            }

            guiaRemision.pedido.existeCambioDireccionEntrega = false;
            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
            return JsonConvert.SerializeObject(guiaRemision.pedido.direccionEntrega);
        }
        #endregion


        #region Acciones en Guia

        public String Show()
        {
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            GuiaRemision guiaRemision = new GuiaRemision();
            guiaRemision.idMovimientoAlmacen = Guid.Parse(Request["idMovimientoAlmacen"].ToString());
            guiaRemision = movimientoAlmacenBL.GetGuiaRemision(guiaRemision);
            this.Session[Constantes.VAR_SESSION_GUIA_VER] = guiaRemision;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonGuiaRemision = JsonConvert.SerializeObject(guiaRemision);
            String json = "{\"usuario\":" + jsonUsuario + ", \"guiaRemision\":" + jsonGuiaRemision + "}";
            return json;
        }

        public ActionResult Print()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.ImprimirGuiaRemision;

            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_VER];

            ViewBag.guiaRemision = guiaRemision;
            ViewBag.pagina = this.Session[Constantes.VAR_SESSION_PAGINA];

            return View("Print" + guiaRemision.ciudadOrigen.sede.ToUpper().Substring(0, 1));

        }

        public String Anular()
        {
            

         //   if (guiaRemision.fechaEmision.Month == DateTime.Now.Month)
         //   {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_VER];
            guiaRemision.comentarioAnulado = Request["comentarioAnulado"];
            MovimientoAlmacenBL movimientoAlmacenBL = new MovimientoAlmacenBL();
            movimientoAlmacenBL.AnularMovimientoAlmacen(guiaRemision);
          //  }


            return JsonConvert.SerializeObject(guiaRemision);
        }



        #endregion

        


        #region Changes



        public void ChangeEstaAnulado()
        {
            this.GuiaRemisionSession.estaAnulado = Int32.Parse(this.Request.Params["estaAnulado"]) == 1;
        }

        public void ChangeEstaFacturado()
        {
            this.GuiaRemisionSession.estaFacturado = Int32.Parse(this.Request.Params["estaFacturado"]) == 1;
        }

        public void ChangeFechaTrasladoDesde()
        {
            String[] movDesde = this.Request.Params["fechaTrasladoDesde"].Split('/');
            this.GuiaRemisionSession.fechaTrasladoDesde = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]));
        }

        public void ChangeFechaTraslado()
        {
            String[] movHasta = this.Request.Params["fechaTraslado"].Split('/');
            this.GuiaRemisionSession.fechaTraslado = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);
        }

        public void ChangeFechaEmision()
        {
            String[] movDesde = this.Request.Params["fechaEmision"].Split('/');
            this.GuiaRemisionSession.fechaEmision = new DateTime(Int32.Parse(movDesde[2]), Int32.Parse(movDesde[1]), Int32.Parse(movDesde[0]));
        }

        public void ChangeFechaTrasladoHasta()
        {
            String[] movHasta = this.Request.Params["fechaTrasladoHasta"].Split('/');
            this.GuiaRemisionSession.fechaTrasladoHasta = new DateTime(Int32.Parse(movHasta[2]), Int32.Parse(movHasta[1]), Int32.Parse(movHasta[0]), 23, 59, 59);
        }


        public String ChangeIdCiudad()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadOrigen = ciudadBL.getCiudad(idCiudad);
            guiaRemision.transportista = new Transportista();
            TransportistaBL transportistaBL = new TransportistaBL();
            ciudadOrigen.transportistaList = transportistaBL.getTransportistas(idCiudad);
            guiaRemision.ciudadOrigen = ciudadOrigen;
            this.GuiaRemisionSession = guiaRemision;
            return JsonConvert.SerializeObject(guiaRemision.ciudadOrigen);
        }

        public void ChangeInputString()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            PropertyInfo propertyInfo = guiaRemision.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(guiaRemision, this.Request.Params["valor"]);
            this.GuiaRemisionSession = guiaRemision;
        }


        public void ChangeInputInt()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            PropertyInfo propertyInfo = guiaRemision.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(guiaRemision, Int32.Parse(this.Request.Params["valor"]));
        }

        public void ChangeBuscarSedesGrupoCliente()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            if (this.Request.Params["buscarSedesGrupoCliente"] != null && Int32.Parse(this.Request.Params["buscarSedesGrupoCliente"]) == 1)
            {
                guiaRemision.pedido.buscarSedesGrupoCliente = true;
            }
            else
            {
                guiaRemision.pedido.buscarSedesGrupoCliente = false;
            }
            this.GuiaRemisionSession = guiaRemision;
        }

        public void ChangeInputStringTransportista()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            PropertyInfo propertyInfo = guiaRemision.transportista.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(guiaRemision.transportista, this.Request.Params["valor"]);
            this.GuiaRemisionSession = guiaRemision;
        }

        public void ChangeInputStringPedido()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            PropertyInfo propertyInfo = guiaRemision.pedido.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(guiaRemision.pedido, this.Request.Params["valor"]);
            this.GuiaRemisionSession = guiaRemision;
        }

        public void ChangeInputIntPedido()
        {
            GuiaRemision guiaRemision = this.GuiaRemisionSession;
            PropertyInfo propertyInfo = guiaRemision.pedido.GetType().GetProperty(this.Request.Params["propiedad"]);
            try
            {
                propertyInfo.SetValue(guiaRemision.pedido, Int64.Parse(this.Request.Params["valor"]));
            }
            catch (Exception e)
            {
                propertyInfo.SetValue(guiaRemision.pedido, 0);
            }
            this.GuiaRemisionSession = guiaRemision;
        }


        public void ChangeAtencionParcial()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
            guiaRemision.atencionParcial = Int32.Parse( this.Request.Params["atencionParcial"])==1;

            if (!guiaRemision.atencionParcial)
            {
                foreach (DocumentoDetalle documentoDetalle in guiaRemision.pedido.documentoDetalle)
                {
                    documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadPendienteAtencion;
                }

            }
            


            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
        }

        /*
        public void ChangeUltimaAtencionParcial()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
            guiaRemision.ultimaAtencionParcial = Int32.Parse(this.Request.Params["ultimaAtencionParcial"]) == 1;


            if (!guiaRemision.atencionParcial && guiaRemision.ultimaAtencionParcial)
            {
                foreach (DocumentoDetalle documentoDetalle in guiaRemision.documentoDetalle)
                {
                    documentoDetalle.cantidadPorAtender = documentoDetalle.cantidadPendienteAtencion;
                }

            }

            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
        }

        */


        public String ChangeTransportista()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
         
            if (this.Request.Params["idTransportista"] == null || this.Request.Params["idTransportista"].Equals(String.Empty))
            {
                guiaRemision.transportista = new Transportista();
            }
            else
            {
                Guid idTransportista = Guid.Parse(this.Request.Params["idTransportista"]);
                guiaRemision.transportista = guiaRemision.ciudadOrigen.transportistaList.Where(t => t.idTransportista == idTransportista).FirstOrDefault();
            }
           
            this.Session[Constantes.VAR_SESSION_GUIA] = guiaRemision;
            String jsonTransportista = JsonConvert.SerializeObject(guiaRemision.transportista);
            return jsonTransportista;
        }

        public void ChangeMotivoTraslado()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA];
            guiaRemision.motivoTrasladoBusqueda = (GuiaRemision.motivosTrasladoBusqueda)(Char)Int32.Parse(this.Request.Params["motivoTraslado"]);
            this.Session[Constantes.VAR_SESSION_GUIA_BUSQUEDA] = guiaRemision;
        }


        #endregion


        #region ChangesConsolidacionGuias

        public void ChangeGuiaRemisionFacturaConsolidada()
        {
            Guid idGuiaRemision = Guid.Parse(this.Request.Params["idGuiaRemision"]);
            List<GuiaRemision> guiaRemisionList = (List<GuiaRemision>)this.Session[Constantes.VAR_SESSION_GUIA_LISTA_FACTURA_CONSOLIDADA];
            GuiaRemision guiaRemision = guiaRemisionList.Where(g => g.idMovimientoAlmacen == idGuiaRemision).FirstOrDefault();
            this.Session[Constantes.VAR_SESSION_GUIA_CONSOLIDADA] = guiaRemision;
        }

        public void ChangeNumeroOrdenCompraFacturaConsolidada()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_CONSOLIDADA];
            guiaRemision.pedido.numeroReferenciaCliente = this.Request.Params["numeroOrdenCompra"];
            this.Session[Constantes.VAR_SESSION_GUIA_CONSOLIDADA] = guiaRemision;
        }

        public void ChangeNumeroReferenciaAdicionalFacturaConsolidada()
        {
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_CONSOLIDADA];
            guiaRemision.pedido.numeroReferenciaAdicional  = this.Request.Params["numeroReferenciaAdicional"];
            this.Session[Constantes.VAR_SESSION_GUIA_CONSOLIDADA] = guiaRemision;
        }

        #endregion


        public ActionResult CancelarCreacionGuiaRemision()
        {
            this.Session[Constantes.VAR_SESSION_GUIA] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("Index");
        }


        public String Descargar()
        {
            String nombreArchivo = Request["nombreArchivo"].ToString();
            GuiaRemision guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA];
            if (guiaRemision == null)
                guiaRemision = (GuiaRemision)this.Session[Constantes.VAR_SESSION_GUIA_VER];

            Pedido pedido = guiaRemision.pedido; ;

            //      pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            PedidoAdjunto pedidoAdjunto = pedido.pedidoAdjuntoList.Where(p => p.nombre.Equals(nombreArchivo)).FirstOrDefault();

            if (pedidoAdjunto != null)
            {
                return JsonConvert.SerializeObject(pedidoAdjunto);
            }
            else
            {
                return null;
            }
        }
    }
}