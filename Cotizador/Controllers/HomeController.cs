using BusinessLayer;
using Model;
using cotizadorPDF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;

namespace Cotizador.Controllers
{
    public class HomeController : Controller
    {
     
        public ActionResult New()
        {
            this.Session["cotizacion"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Search()
        {
            ViewBag.fecha = DateTime.Now.AddDays(-10).ToString("dd/MM/yyyy");

            if (this.Session["cotizacionList"] == null)
            {
                this.Session["cotizacionList"] = new List<Cotizacion>();
            }

            ViewBag.cotizacionList = this.Session["cotizacionList"];

            return View();
        }

        public int SearchCotizaciones()
        {
            Cotizacion cotizacion = new Cotizacion();

            //Se agrega Ciudad en la busqueda
            cotizacion.ciudad = new Ciudad();
            if (this.Request.Params["idCiudad"].ToString() == "00000000-0000-0000-0000-000000000000")
            {
                cotizacion.ciudad.idCiudad = Guid.Empty;
            }
            else
            {
                cotizacion.ciudad.idCiudad = Guid.Parse(this.Request.Params["idCiudad"].ToString());
            }

            //Se agrega fecha en la busqueda
            String[] fecha = this.Request.Params["fecha"].Split('/');
            cotizacion.fecha = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));

            //Se agrega número/codigo en la busqueda
            if (this.Request.Params["numero"].ToString() != "")
            {
                cotizacion.codigo = Int64.Parse(this.Request.Params["numero"].ToString());
            }

            //Se agrega cliente en la busqueda
            cotizacion.cliente = new Cliente();
            if (this.Request.Params["idCliente"].ToString().Trim() != "")
            {
                cotizacion.cliente.idCliente = Guid.Parse(this.Request.Params["idCliente"].ToString());
            }


            CotizacionBL cotizacionBL = new CotizacionBL();
            List<Cotizacion> cotizacionList = cotizacionBL.GetCotizaciones(cotizacion);

            this.Session["cotizacionList"] = cotizacionList;

            return cotizacionList.Count();
        }


        public ActionResult Exit()
        {
            this.Session["usuario"] = null;
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Index()
        {
            ViewBag.Si = "Sí";
            ViewBag.No = "No";
            ViewBag.IGV = Constantes.IGV;

            //Si no se está trabajando con una cotización se crea una

            if (this.Session["cotizacion"] == null)
            {

                Cotizacion cotizacionTmp = new Cotizacion();
                cotizacionTmp.fecha = DateTime.Now;
                cotizacionTmp.fechaVigenciaLimite = cotizacionTmp.fecha.AddDays(15);
                cotizacionTmp.ciudad = new Ciudad();
                cotizacionTmp.cliente = new Cliente();
                cotizacionTmp.cotizacionDetalleList = new List<CotizacionDetalle>();
                cotizacionTmp.igv = Constantes.IGV;
                cotizacionTmp.flete = 0;
                cotizacionTmp.considerarCantidades = true;
                cotizacionTmp.observaciones = "* Condiciones de pago: al contado.\n" +
                                       "* Entrega en almacén del cliente, 48 horas luego de la recepción del pedido o la orden de compra.\n" +
                                       "* (para productos no stockeables o primeras compras, consultar plazo).\n";



                this.Session["cotizacion"] = cotizacionTmp;
            }
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];


            ///Modificar
            int existeCliente = 0;
            if (cotizacion.cliente.idCliente != Guid.Empty)
            {
                existeCliente = 1;
            }




            if (this.Session["usuario"] != null)
            {
              
                Usuario usuario = (Usuario)this.Session["usuario"];
                ViewBag.nombreUsuario = usuario.apellidos + " " + usuario.nombres;

               // List<CotizacionDetalle> detalles = (List<CotizacionDetalle>)this.Session["detalles"];
                //ViewBag.Detalles = cotizacion.cotizacionDetalleList;

                ViewBag.numero = cotizacion.codigo;
                ViewBag.idCiudad = cotizacion.ciudad.idCiudad;

                ViewBag.idCliente = cotizacion.cliente.idCliente;
                ViewBag.cliente = cotizacion.cliente.ToString();
                ViewBag.existeCliente = existeCliente;
                /*Dirigido a*/
                ViewBag.contacto = cotizacion.contacto;
                ViewBag.incluidoIgv = cotizacion.incluidoIgv;
                ViewBag.considerarCantidades = cotizacion.considerarCantidades;

                //  ViewBag.mostrarCodProveedor = mostrarCodProveedor;
                //  ViewBag.MostrarCodProvSi = "Mostrar";
                //  ViewBag.MostrarCodProvNo = "No mostrar";
                /*Flete*/

                ViewBag.flete = cotizacion.flete;
                ViewBag.observaciones = cotizacion.observaciones;
                ViewBag.fecha = cotizacion.fecha.ToString("dd/MM/yyyy");
                ViewBag.fechaVigencia = cotizacion.fechaVigenciaLimite.ToString("dd/MM/yyyy");
                ViewBag.mostrarCodigoProveedor = cotizacion.mostrarCodigoProveedor;
                Decimal total = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacion.cotizacionDetalleList.AsEnumerable().Sum(o => o.subTotal)));
         


                ViewBag.igv = cotizacion.montoIGV;
                ViewBag.total = cotizacion.montoTotal;
                ViewBag.subtotal = cotizacion.montoSubTotal;


                ViewBag.cotizacion = cotizacion;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }


        public int updateSeleccionIGV()
        {
            int incluidoIGV = Int32.Parse(this.Request.Params["igv"]);
            return actualizarCotizacionDetalles(incluidoIGV, true);
        }

        

        public int updateSeleccionConsiderarCantidades()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            int considerarCantidades = Int32.Parse(this.Request.Params["considerarCantidades"]);
            cotizacion.considerarCantidades = considerarCantidades == 1;
            this.Session["cotizacion"] = cotizacion;
            return 1;
        }


        private int actualizarCotizacionDetalles(int incluidoIGVInt, bool recalcularIGV = false)//, int considerarCantidadesInt)
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];

            if (incluidoIGVInt > -1)
            {
                cotizacion.incluidoIgv = incluidoIGVInt == 1;
            }
    
            
            List<CotizacionDetalle> detalles = cotizacion.cotizacionDetalleList;

            foreach (CotizacionDetalle cotizacionDetalle in detalles)
            {
                Decimal precioNeto = cotizacionDetalle.producto.precioSinIgv;
                Decimal costo = cotizacionDetalle.producto.costoSinIgv;

                //Se calcula el precio con Flete
                precioNeto = precioNeto + (precioNeto * cotizacion.flete / 100);

                if (cotizacion.incluidoIgv)
                {
                    precioNeto = precioNeto + (precioNeto * Constantes.IGV);
                    costo = costo + (costo * Constantes.IGV);
                    //Si recalcularIGV es true quiere decir que se cambio la opcion de considerar IGV
                    //El cambio afecta el precio y el costo Anterior.
                    if (recalcularIGV)
                    {
                        cotizacionDetalle.precioNetoAnterior = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacionDetalle.precioNetoAnterior + cotizacionDetalle.precioNetoAnterior * Constantes.IGV));
                        cotizacionDetalle.costoAnterior = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacionDetalle.costoAnterior + cotizacionDetalle.costoAnterior * Constantes.IGV));
                    }
                }
                else
                {
                    //Si recalcularIGV es true quiere decir que se cambio la opcion de considerar IGV
                    //El cambio afecta el precio y el costo Anterior.
                    if (recalcularIGV)
                    {
                        cotizacionDetalle.precioNetoAnterior = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacionDetalle.precioNetoAnterior / (1 + Constantes.IGV)));
                        cotizacionDetalle.costoAnterior = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacionDetalle.costoAnterior / (1 + Constantes.IGV)));
                    }
                }

                
                //El precioLista no tiene el calculo de equivalencia
                //El flete tambien afecta al precioLista
                cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.decimalFormat, precioNeto));
                //Se define el costoLista del producto como el costo calculado

                //El precio y el costo se setean al final dado que si es equivalente en cada get se hará el recalculo


                cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.decimalFormat, costo));
                //Se aplica descuenta al precio y se formatea a dos decimales el precio para un calculo exacto en el subtotal
                cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.decimalFormat, precioNeto * (100 - cotizacionDetalle.porcentajeDescuento) / 100));
              
            }
            calcularMontosTotales(cotizacion);
            this.Session["cotizacion"] = cotizacion;
            return detalles.Count();
        }

        public void updateObservaciones()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.observaciones = this.Request.Params["observaciones"];
            this.Session["cotizacion"] = cotizacion;
        }

      

        public void updateFlete()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.flete = Decimal.Parse(this.Request.Params["flete"]);
            actualizarCotizacionDetalles(cotizacion.incluidoIgv?1:0);

        }


        public void updateMostrarCodigoProveedor()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.mostrarCodigoProveedor = Int32.Parse(this.Request.Params["mostrarcodproveedor"])==1;
            this.Session["cotizacion"] = cotizacion;
        }

        public void updateContacto()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.contacto = this.Request.Params["contacto"];
            this.Session["cotizacion"] = cotizacion;
        }


        public void updateFecha()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            String[] fecha = this.Request.Params["fecha"].Split('/');
            cotizacion.fecha = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.Session["cotizacion"] = cotizacion;
        }


        public void updateFechaVigencia()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            String[] fecha = this.Request.Params["fechaVigencia"].Split('/');
            cotizacion.fechaVigenciaLimite = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.Session["cotizacion"] = cotizacion;
        }

        public String updateIdCiudad()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            Guid idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);     

            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);


            //Para realizar el cambio de ciudad ningun producto debe estar agregado
            if (cotizacion.cotizacionDetalleList.Count > 0)
            {
                // throw new Exception("No se puede cambiar de ciudad");
                return "No se puede cambiar de ciudad";
            }
            else
            {
                cotizacion.ciudad = ciudadNueva;
                this.Session["cotizacion"] = cotizacion;
                return "{\"idCiudad\": \"" + idCiudad + "\"}";
            }
            
        }

  
        [HttpPost]
        public String updateCotizacionDetalles(List<CotizacionDetalleJson> cotizacionDetalleJsonList)
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];

            if (cotizacionDetalleJsonList != null)
            {

                List<CotizacionDetalle> detalles = cotizacion.cotizacionDetalleList;

                List<Guid> idProductos = new List<Guid>();
                foreach (CotizacionDetalleJson cotizacionDetalleJson in cotizacionDetalleJsonList)
                {
                    idProductos.Add(Guid.Parse(cotizacionDetalleJson.idProducto));
                }

                List<CotizacionDetalle> detallesActualizados = detalles.Where(s => idProductos.Contains(s.producto.idProducto)).ToList();


                List<CotizacionDetalle> detallesOrdenados = new List<CotizacionDetalle>();
                //Se realiza este foreach para considerar el ordenamiento
                foreach (Guid idProducto in idProductos)
                {
                    CotizacionDetalleJson cotizacionDetalleJson = cotizacionDetalleJsonList.Where(s => s.idProducto == idProducto.ToString()).FirstOrDefault();
                    CotizacionDetalle cotizacionDetalle = detallesActualizados.Where(s => s.producto.idProducto == idProducto).FirstOrDefault();

                    cotizacionDetalle.cantidad = cotizacionDetalleJson.cantidad;

                    cotizacionDetalle.precioNeto = cotizacionDetalleJson.precio;

                    if (cotizacionDetalle.esPrecioAlternativo)
                    {
                        cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacionDetalleJson.precio * cotizacionDetalle.producto.equivalencia));
                    }
  
                    cotizacionDetalle.porcentajeDescuento = cotizacionDetalleJson.porcentajeDescuento;

                    detallesOrdenados.Add(cotizacionDetalle);

                }


                cotizacion.cotizacionDetalleList = detallesOrdenados;
            }
            else
            {
                cotizacion.cotizacionDetalleList = new List<CotizacionDetalle>();
            }

            calcularMontosTotales(cotizacion);


            this.Session["cotizacion"] = cotizacion;
            return "{\"cantidad\":\""+ cotizacion.cotizacionDetalleList.Count + "\"}";
        }


        private void calcularMontosTotales(Cotizacion cotizacion)
        {
            Decimal total = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacion.cotizacionDetalleList.AsEnumerable().Sum(o => o.subTotal)));
            Decimal subtotal = 0;
            Decimal igv = 0;

            if (cotizacion.incluidoIgv)
            {
                subtotal = Decimal.Parse(String.Format(Constantes.decimalFormat, total / (1 + Constantes.IGV)));
                igv = Decimal.Parse(String.Format(Constantes.decimalFormat, total - subtotal));
            }
            else
            {
                subtotal = total;
                igv = Decimal.Parse(String.Format(Constantes.decimalFormat, total * Constantes.IGV));
                total = Decimal.Parse(String.Format(Constantes.decimalFormat, subtotal + igv));
            }

            cotizacion.montoTotal = total;
            cotizacion.montoSubTotal = subtotal;
            cotizacion.montoIGV = igv;
        }



        public String GetClientes()
        {

            String data = this.Request.Params["data[q]"];

            ClienteBL clienteBL = new ClienteBL();
            List<Cliente> clienteList = clienteBL.getCLientesBusqueda(data);

            String resultado = "{\"q\":\"" + data + "\",\"results\":[";
            Boolean existeCliente = false;
            foreach (Cliente cliente in clienteList)
            {
                resultado += "{\"id\":\"" + cliente.idCliente + "\",\"text\":\"" + cliente.ToString() + "\"},";
                existeCliente = true;
            }

            if (existeCliente)
                resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
            else
                resultado = resultado.Substring(0, resultado.Length) + "]}";

            return resultado;
        }





        public String GetCliente()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            cotizacion.cliente = clienteBl.getCliente(idCliente);
            cotizacion.contacto = cotizacion.cliente.contacto1;

            String resultado = "{" +
                "\"descripcionCliente\":\"" + cotizacion.cliente.ToString() + "\"," +
                "\"idCliente\":\"" + cotizacion.cliente.idCliente + "\"," +
                "\"contacto\":\"" + cotizacion.cliente.contacto1 + "\"" +
                "}";
            return resultado;
        }




        public String GetProductos()
        {
            
            String texto_busqueda = this.Request.Params["data[q]"];

            ProductoBL bl = new ProductoBL();
            List<Producto> lista = bl.getProductosBusqueda(texto_busqueda);

            String resultado = "{\"q\":\"" + texto_busqueda + "\",\"results\":[";
            Boolean existe = false;
            foreach (Producto prod in lista)
            {
                resultado += "{\"id\":\"" + prod.idProducto + "\",\"text\":\"" + prod.ToString() + "\"},";
                existe = true;
            }

            if (existe)
                resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
            else
                resultado = resultado.Substring(0, resultado.Length) + "]}";

            return resultado;
        }

        public String GetProducto()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            //Se recupera el producto y se guarda en Session
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            this.Session["idProducto"] = idProducto.ToString();

    
            //Para recuperar el producto se envia si la sede seleccionada es provincia o no
            ProductoBL bl = new ProductoBL();
            Producto producto = bl.getProducto(idProducto, cotizacion.ciudad.esProvincia , cotizacion.incluidoIgv, cotizacion.flete);

            String resultado = "{" +
                "\"id\":\"" + producto.idProducto + "\"," +
                "\"nombre\":\"" + producto.descripcion + "\"," +
                "\"image\":\"data:image/png;base64, " + Convert.ToBase64String(producto.image) + "\"," +
                "\"unidad\":\"" + producto.unidad + "\"," +
                "\"unidad_alternativa\":\"" + producto.unidad_alternativa + "\"," +
                "\"proveedor\":\"" + producto.proveedor + "\"," +
                "\"precioUnitarioSinIGV\":\"" + producto.precioSinIgv + "\"," +
                "\"precioUnitarioAlternativoSinIGV\":\"" + producto.precioAlternativoSinIgv + "\"," +
                "\"precioLista\":\"" + producto.precioLista + "\"," +
                "\"costoSinIGV\":\"" + producto.costoSinIgv + "\"," +
                "\"costoAlternativoSinIGV\":\"" + producto.costoAlternativoSinIgv + "\"," +
                "\"costoLista\":\"" + producto.costoLista + "\"" +
                "}";
            return resultado;
        }






        public String AddProducto()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            Guid idProducto = Guid.Parse(this.Session["idProducto"].ToString());
            CotizacionDetalle cotizacionDetalle = cotizacion.cotizacionDetalleList.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
            if (cotizacionDetalle != null)
            {
                throw new System.Exception("Producto ya se encuentra en la lista");
            }

            CotizacionDetalle detalle = new CotizacionDetalle();
            ProductoBL productoBL = new ProductoBL();
            Producto producto = productoBL.getProducto(idProducto, cotizacion.ciudad.esProvincia, cotizacion.incluidoIgv , cotizacion.flete);
            detalle.producto = producto;

            detalle.cantidad = Int32.Parse(Request["cantidad"].ToString());
            detalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
            detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;
            decimal precioNeto = Decimal.Parse(Request["precio"].ToString());
            decimal costo = Decimal.Parse(Request["costo"].ToString());
            if (detalle.esPrecioAlternativo)
            {

                detalle.precioNeto = Decimal.Parse(String.Format(Constantes.decimalFormat, precioNeto * producto.equivalencia));
            }
            else
            {
                detalle.precioNeto = precioNeto;
            }

            cotizacion.cotizacionDetalleList.Add(detalle);

            calcularMontosTotales(cotizacion);

           
            detalle.unidad = detalle.producto.unidad;
            //si esPrecioAlternativo  se mostrará la unidad alternativa
            if (detalle.esPrecioAlternativo)
            {
                detalle.unidad = detalle.producto.unidad_alternativa;
            }

            var nombreProducto = detalle.producto.descripcion;
            if (cotizacion.mostrarCodigoProveedor)
            {
                nombreProducto = detalle.producto.skuProveedor + " - " + detalle.producto.descripcion;
            }

            String resultado = "{" +
                "\"idProducto\":\"" + detalle.producto.idProducto + "\"," +
                "\"codigoProducto\":\"" + detalle.producto.sku + "\"," +
                "\"nombreProducto\":\"" + nombreProducto + "\"," +
                "\"unidad\":\"" + detalle.unidad + "\"," +
                "\"igv\":\"" + cotizacion.montoIGV.ToString() + "\", " +
                "\"subTotal\":\"" + cotizacion.montoSubTotal.ToString() + "\", " +
                "\"margen\":\"" + detalle.margen + "\", " +
                "\"total\":\"" + cotizacion.montoTotal.ToString() + "\"}";

            this.Session["cotizacion"] = cotizacion ;
            return resultado;
            
            
        }

        public String DelProducto()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            List<CotizacionDetalle> detalles = cotizacion.cotizacionDetalleList;
            Guid idProducto = Guid.Parse(this.Request.Params["idProducto"]);
            CotizacionDetalle cotizacionDetalle = detalles.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
            if(cotizacionDetalle != null)
            { 
                detalles.Remove(cotizacionDetalle);
                this.Session["detalles"] = detalles;
            }
            this.Session["cotizacion"] = cotizacion;
            return detalles.AsEnumerable().Sum(o => o.subTotal).ToString();
        }



        public void LoadClientes()
        {
            HSSFWorkbook hssfwb;
            using (FileStream file = new FileStream(@"C:\PENTAHO\PRODUCTOS\Clientes.xls", FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }

            ISheet sheet = hssfwb.GetSheet("Hoja1");
            int row = 1;
            for ( row = 1; row <= 6044; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    try
                    {
                        ClienteStaging clienteStaging = new ClienteStaging();
                        clienteStaging.PlazaId = sheet.GetRow(row).GetCell(0).ToString();
                        clienteStaging.Plaza = sheet.GetRow(row).GetCell(1).ToString();
                        clienteStaging.Id = sheet.GetRow(row).GetCell(2).ToString();
                        clienteStaging.nombre = sheet.GetRow(row).GetCell(3).ToString();
                        clienteStaging.documento = sheet.GetRow(row).GetCell(4).ToString();
                        clienteStaging.codVe = sheet.GetRow(row).GetCell(5).ToString();
                        clienteStaging.nombreComercial = sheet.GetRow(row).GetCell(6).ToString();
                        clienteStaging.domicilioLegal = sheet.GetRow(row).GetCell(7).ToString();
                        clienteStaging.distrito = sheet.GetRow(row).GetCell(8).ToString();
                        clienteStaging.direccionDespacho = sheet.GetRow(row).GetCell(9).ToString();
                        clienteStaging.distritoDespacho = sheet.GetRow(row).GetCell(10).ToString();
                        clienteStaging.rubro = sheet.GetRow(row).GetCell(11).ToString();

                        ClienteBL clienteBL = new ClienteBL();
                        clienteBL.setClienteStaging(clienteStaging);

                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ee);


                    }
                }
            }
            row = row;

        }

        public void LoadProductos()
        {
            HSSFWorkbook hssfwb;
            using (FileStream file = new FileStream(@"C:\PENTAHO\PRODUCTOS\Productos3.xls", FileMode.Open, FileAccess.Read))
            {
                hssfwb = new HSSFWorkbook(file);
            }

            ISheet sheet = hssfwb.GetSheet("Sheet1");
            int row = 1;
            for (row = 3; row <= 68; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    try
                    {
                        ProductoStaging productoStaging = new ProductoStaging();
                        //B
                        productoStaging.proveedor = sheet.GetRow(row).GetCell(1).ToString();
                        //C
                        productoStaging.codigo = sheet.GetRow(row).GetCell(2).ToString();
                        //D
                        productoStaging.codigoProveedor = sheet.GetRow(row).GetCell(3).ToString();
                        //E
                        productoStaging.unidad = sheet.GetRow(row).GetCell(4).ToString();
                        //H
                        productoStaging.unidadAlternativa = sheet.GetRow(row).GetCell(7).ToString();
                        //J
                        productoStaging.equivalencia = Int32.Parse(sheet.GetRow(row).GetCell(8).ToString());
                        //K
                        productoStaging.descripcion = sheet.GetRow(row).GetCell(9).ToString();

                        Double? costo = sheet.GetRow(row).GetCell(18).NumericCellValue;
                        productoStaging.costo = Convert.ToDecimal(costo);
                        //K
                        Double? precioLima = sheet.GetRow(row).GetCell(22).NumericCellValue;
                        productoStaging.precioLima = Convert.ToDecimal(precioLima);
                        //K
                        Double? precioProvincias = sheet.GetRow(row).GetCell(25).NumericCellValue;
                        productoStaging.precioProvincias = Convert.ToDecimal(precioProvincias);

                        /*
                        //A
                        productoStaging.codigo = sheet.GetRow(row).GetCell(0).ToString();
                        //B
                        productoStaging.familia = sheet.GetRow(row).GetCell(1).ToString();
                        //C
                        productoStaging.clase = sheet.GetRow(row).GetCell(2).ToString();
                        //D
                        productoStaging.Marca = sheet.GetRow(row).GetCell(3).ToString();
                        //F
                        productoStaging.unidad = sheet.GetRow(row).GetCell(5).ToString();
                        //G
                        productoStaging.descripcion = sheet.GetRow(row).GetCell(6).ToString();
                        //K
                        productoStaging.codigoProveedor = sheet.GetRow(row).GetCell(10).ToString();
                        //N
                        productoStaging.unidadAlternativa = sheet.GetRow(row).GetCell(13).ToString();
                        //M
                        productoStaging.unidadAlternativa = sheet.GetRow(row).GetCell(14).ToString();

                        Double? costo = sheet.GetRow(row).GetCell(19).NumericCellValue;
                        //T
                        productoStaging.costo = Convert.ToDecimal(costo);

                        Double? precio = sheet.GetRow(row).GetCell(21).NumericCellValue;
                        //V
                        productoStaging.precio = Convert.ToDecimal(precio);
                        //Y
                        productoStaging.proveedor = sheet.GetRow(row).GetCell(24).ToString();
                        */
                        ProductoBL productoBL = new ProductoBL();
                        productoBL.setProductoStaging(productoStaging);
                    }
                    catch (Exception ee)
                    {
                        Console.WriteLine("An error occurred: '{0}'", ee);


                    }
                }
            }
            row = row;

        }

        [HttpGet]
        public ActionResult DownLoadFile(String fileName)
        {
            FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\pdf\\"+ fileName, FileMode.Open);
            MemoryStream storeStream = new MemoryStream();

            storeStream.SetLength(inStream.Length);
            inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);

            storeStream.Flush();
            inStream.Close();
            System.IO.File.Delete(AppDomain.CurrentDomain.BaseDirectory + "\\pdf\\" + fileName);

            FileStreamResult result = new FileStreamResult(storeStream, "application/pdf");
            result.FileDownloadName = fileName;     
            return result;
        }


        public String grabarCotizacion()
        {
            Cotizacion cotizacion = insertarCotizacion();
            return "{ \"codigo\":\""+cotizacion.codigo+"\", \"estadoAprobacion\":\""+ cotizacion.estadoAprobacion+"\" }";
        }


        public void GetCotizacion()
        {
            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();
            //Agregar Try Catch
            cotizacion.codigo = Int64.Parse(Request["numero"].ToString());
            cotizacion = cotizacionBL.GetCotizacion(cotizacion);
            this.Session["cotizacion"] = cotizacion;
        }

        public void recotizacion()
        {
            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();

            //Agregar Try Catch
            cotizacion.codigo = Int64.Parse(Request["numero"].ToString());
            cotizacion.esRecotizacion = true;
            cotizacion = cotizacionBL.GetCotizacion(cotizacion);
            

            




            this.Session["cotizacion"] = cotizacion;
        }


        



        public Cotizacion insertarCotizacion()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            Usuario usuario = (Usuario)this.Session["usuario"];
            cotizacion.usuario = usuario;
            CotizacionBL bl = new CotizacionBL();

            if (cotizacion.codigo > 0 && !cotizacion.esRecotizacion)
            {
                bl.UpdateCotizacion(cotizacion);
            }
            else
            { 
                bl.InsertCotizacion(cotizacion);
            }
            return cotizacion;
        }


      /*  public String GenerarPDF()
        {
            Cotizacion cotizacion = insertarCotizacion();
        //    Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            GeneradorPDF gen = new GeneradorPDF();
            String ruta =   gen.generarPDFExtended(cotizacion);
            return ruta;
        }*/



        [HttpPost]
        public String GenerarPDFdesdeIdCotizacion()
        {
            Int64 codigo = Int64.Parse(this.Request.Params["codigo"].ToString());

            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = codigo;
            cotizacion = cotizacionBL.GetCotizacion(cotizacion);

            GeneradorPDF gen = new GeneradorPDF();
            String ruta = gen.generarPDFExtended(cotizacion);
            return ruta;
        }


        public String TestPDF()
        {
            //Guid idFamilia = Guid.Parse(Request["idFamilia"].ToString());
            GeneradorPDF gen = new GeneradorPDF();
       //     gen.generar();
            return "";
        }

    }
}