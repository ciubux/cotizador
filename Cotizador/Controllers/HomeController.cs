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
        private static Decimal IGV = 0.18M;
        private static String decimalFormat = "{0:0.00}";


        public ActionResult Index()
        {
            ViewBag.Si = "Sí";
            ViewBag.No = "No";
            ViewBag.IGV = IGV;




            /*   String usuario = Request["txtUser"].ToString();
               String clave = Request["txtClave"].ToString();

               UsuarioDal dal = new UsuarioDal();
               Usuario user = dal.LoginUsuario(usuario, clave);
               if (user != null)
               {
                   this.Session["EstablecimientoLogin"] = dal.getEstablecimientoUsuario(user.idUsuario);
                   this.Session["UsuarioLogin"] = user;
                   return RedirectToAction("Index", "Home");
               }
               else
               {
                   if (usuario.Equals("")) this.Session["loginFault"] = 1;
                   else if (clave.Equals("")) this.Session["loginFault"] = 2;
                   else this.Session["loginFault"] = 3;
                   return RedirectToAction("Index", "Login");
               }*/
            if (this.Session["detalles"] == null)
            {
                this.Session["detalles"] = new List<CotizacionDetalle>();
            }

            if (this.Session["usuario"] != null)
            {
              
                Usuario usuario = (Usuario)this.Session["usuario"];
                ViewBag.nombreUsuario = usuario.apellidos + " " + usuario.nombres;

                List<CotizacionDetalle> detalles = (List<CotizacionDetalle>)this.Session["detalles"];
                ViewBag.Detalles = detalles;

                

                /*Ciudad*/
                Guid idCiudad = Guid.Empty;
                if (this.Session["idCiudad"] != null)
                {
                    idCiudad = (Guid)this.Session["idCiudad"];
                }
                else
                {
                    this.Session["idCiudad"] = idCiudad;
                    this.Session["esProvincia"] = false;
                }
                ViewBag.idCiudad = idCiudad;

                
                /*Cliente*/
                /*      String cliente = "";
                      if (this.Session["cliente"] != null)
                      {
                          cliente = (String)this.Session["cliente"];
                      }
                      ViewBag.cliente = cliente;
                      */

                String idCliente = "";
                String descripcionCliente = "";
                int existeCliente = 0;
                if (this.Session["cliente"] != null)
                {
                    Cliente cliente = (Cliente)this.Session["cliente"];
                    idCliente = cliente.idCliente.ToString();
                    descripcionCliente = cliente.ToString();
                    existeCliente = 1;
                }
                ViewBag.idCliente = idCliente;
                ViewBag.existeCliente = existeCliente;
                ViewBag.cliente = descripcionCliente;




                /*Dirigiado a*/
                String contacto = "";
                if (this.Session["contacto"] != null)
                {
                    contacto = (String)this.Session["contacto"];
                }
                ViewBag.contacto = contacto;


                /*IGV*/
                Boolean incluidoIgv = false;
                if (this.Session["incluidoIgv"] != null)
                {
                    incluidoIgv = (Boolean)this.Session["incluidoIgv"];
                }
                else
                {
                    this.Session["incluidoIgv"] = incluidoIgv;
                }

                ViewBag.incluidoIgv = incluidoIgv;

                /*considerarCantidades*/
                Boolean considerarCantidades = true;
                if (this.Session["considerarCantidades"] != null)
                {
                    considerarCantidades = (Boolean)this.Session["considerarCantidades"];
                }
                else
                {
                    this.Session["considerarCantidades"] = considerarCantidades;
                }
                ViewBag.considerarCantidades = considerarCantidades;



                /*Mostrar Codigo Proveedor */
                Boolean mostrarCodProveedor = false;
                if (this.Session["mostrarcodproveedor"] != null)
                {
                    mostrarCodProveedor = (Boolean)this.Session["mostrarcodproveedor"];
                }
                ViewBag.mostrarCodProveedor = mostrarCodProveedor;
                ViewBag.MostrarCodProvSi = "Mostrar";
                ViewBag.MostrarCodProvNo = "No mostrar";


                /*Flete*/
                Decimal flete = 0;
                if (this.Session["flete"] != null)
                {
                    flete = (Decimal)this.Session["flete"];
                }
                ViewBag.flete = flete;



                String observaciones =  "* Condiciones de pago: al contado.\n" +
                                        "* Validez de los precios: 15 días.\n" +
                                        "* Entrega en almacén del cliente, 48 horas luego de la recepción del pedido o la orden de compra.\n" +
                                        "* (para productos no stockeables o primeras compras, consultar plazo).\n";
                if (this.Session["observaciones"] != null)
                {
                    observaciones = this.Session["observaciones"].ToString();
                }
                ViewBag.observaciones = observaciones;
                this.Session["observaciones"] = observaciones;




                String fecha = DateTime.Now.ToString("dd/MM/yyyy");
                if (this.Session["fecha"] != null)
                {
                    fecha = (String)this.Session["fecha"];
                }
                ViewBag.fecha = fecha;





                Decimal total = Decimal.Parse(String.Format(decimalFormat, detalles.AsEnumerable().Sum(o => o.subTotal)));
                Decimal igv = 0;
                Decimal subtotal = 0;


                if (incluidoIgv)
                {
                    subtotal = Decimal.Parse(String.Format(decimalFormat, total / (1 + IGV)));
                    ViewBag.subtotal = subtotal;
                    igv = Decimal.Parse(String.Format(decimalFormat, total - subtotal));
                }
                else
                {
                    subtotal = total;
                    igv = Decimal.Parse(String.Format(decimalFormat, total * IGV));
                    total = Decimal.Parse(String.Format(decimalFormat, subtotal + igv));                  
                }

                ViewBag.igv = igv;
                ViewBag.total = total;
                ViewBag.subtotal = subtotal;
                ViewBag.montoFlete = String.Format(decimalFormat, total * flete / 100);
                ViewBag.montoTotalMasFlete = String.Format(decimalFormat, total + (total * flete / 100));

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public void updateSeleccionMostrarcodproveedor()
        {
            this.Session["mostrarcodproveedor"] = Int32.Parse(this.Request.Params["mostrarcodproveedor"]) == 1;
        }


        public int updateSeleccionIGV()
        {
            int incluidoIGV = Int32.Parse(this.Request.Params["igv"]);
            return actualizarCotizacionDetalles(incluidoIGV, -1);
        }

        

        public int updateSeleccionConsiderarCantidades()
        {       
            int considerarCantidades = Int32.Parse(this.Request.Params["considerarCantidades"]);            
            return actualizarCotizacionDetalles(-1, considerarCantidades);
        }


        private int actualizarCotizacionDetalles(int incluidoIGVInt, int considerarCantidadesInt)
        {

            Boolean incluidoIGV = (Boolean)this.Session["incluidoIgv"];
            Boolean considerarCantidades = (Boolean)this.Session["considerarCantidades"];
            Boolean cambioIncluidoIGV = false;
            Boolean cambioConsiderarCantidades = false;
            if (incluidoIGVInt > -1)
            {
                this.Session["incluidoIgv"] = incluidoIGVInt == 1;
                incluidoIGV = incluidoIGVInt == 1;
                cambioIncluidoIGV = true;
            }
            if (considerarCantidadesInt > -1)
            {
                this.Session["considerarCantidades"] = considerarCantidadesInt == 1;
                considerarCantidades = considerarCantidadesInt == 1;
                cambioConsiderarCantidades = true;
            }
            
            List<CotizacionDetalle> detalles = (List<CotizacionDetalle>)this.Session["detalles"];

            foreach (CotizacionDetalle cotizacionDetalle in detalles)
            {

                if (cambioConsiderarCantidades)
                {
                    if (cotizacionDetalle.cantidad == 0)
                    {
                        cotizacionDetalle.cantidad = 1;
                    }
                }



                    if (incluidoIGV)
                {
                    if (!cotizacionDetalle.esPrecioAlternativo)
                    {
                        cotizacionDetalle.precio = (cotizacionDetalle.precioUnitarioSinIGV + (cotizacionDetalle.precioUnitarioSinIGV * IGV));
                    }
                    else
                    {
                        cotizacionDetalle.precio = (cotizacionDetalle.precioUnitarioAlternativoSinIGV + (cotizacionDetalle.precioUnitarioAlternativoSinIGV * IGV));
                    }
                }
                else
                {
                    if (!cotizacionDetalle.esPrecioAlternativo)
                    {
                        cotizacionDetalle.precio = (cotizacionDetalle.precioUnitarioSinIGV);
                    }
                    else
                    {
                        cotizacionDetalle.precio = (cotizacionDetalle.precioUnitarioAlternativoSinIGV);
                    }
                }
                //Se aplica descuenta al precio y se formatea a dos decimales el precio para un calculo exacto en el subtotal
                cotizacionDetalle.precio = Decimal.Parse(String.Format(decimalFormat, cotizacionDetalle.precio * (100 - cotizacionDetalle.porcentajeDescuento) / 100));
                //Se calcula subtotal
                cotizacionDetalle.subTotal = cotizacionDetalle.cantidad * cotizacionDetalle.precio;




                /*
                //Si el cambio es por cantidades
                if (cambioConsiderarCantidades)
                {
                    if (considerarCantidades)
                    {
                      //  cotizacionDetalle.cantidad = 1;
                      //  cotizacionDetalle.porcentajeDescuento = 0;

                        if (incluidoIGV)
                        {
                            //Si es precio estandar
                            if (!cotizacionDetalle.esPrecioAlternativo)
                            {
                                cotizacionDetalle.precio = cotizacionDetalle.precioUnitarioSinIGV + (cotizacionDetalle.precioUnitarioSinIGV * IGV);
                            }
                            else
                            {
                                cotizacionDetalle.precio = cotizacionDetalle.precioUnitarioAlternativoSinIGV + (cotizacionDetalle.precioUnitarioAlternativoSinIGV * IGV);
                            }
                        }
                        else
                        {
                            if (!cotizacionDetalle.esPrecioAlternativo)
                            {
                                cotizacionDetalle.precio = cotizacionDetalle.precioUnitarioSinIGV;
                            }
                            else
                            {
                                cotizacionDetalle.precio = cotizacionDetalle.precioUnitarioAlternativoSinIGV;
                            }                            
                        }
                        //Se aplica descuenta al precio
                        cotizacionDetalle.precio = cotizacionDetalle.precio * (100 - cotizacionDetalle.porcentajeDescuento) / 100;
                        //Se calcula subtotal
                        cotizacionDetalle.subTotal = cotizacionDetalle.precio * cotizacionDetalle.cantidad;
                    }
                   else
                    {
                        cotizacionDetalle.cantidad = 0;
                        cotizacionDetalle.porcentajeDescuento = 0;
                        cotizacionDetalle.subTotal = 0;
                    }
                }
                else  //Si el cambio es por IGV
                {
                    if (considerarCantidades)
                    {
                        if (incluidoIGV)
                        {
                            if (!cotizacionDetalle.esPrecioAlternativo)
                            {
                                cotizacionDetalle.precio = (cotizacionDetalle.precioUnitarioSinIGV + (cotizacionDetalle.precioUnitarioSinIGV * IGV));
                            }
                            else
                            {
                                cotizacionDetalle.precio = (cotizacionDetalle.precioUnitarioAlternativoSinIGV + (cotizacionDetalle.precioUnitarioAlternativoSinIGV * IGV));
                            }
                        }
                        else
                        {
                            if (!cotizacionDetalle.esPrecioAlternativo)
                            {
                                cotizacionDetalle.precio = (cotizacionDetalle.precioUnitarioSinIGV);
                            }
                            else
                            {
                                cotizacionDetalle.precio = (cotizacionDetalle.precioUnitarioAlternativoSinIGV);
                            }
                        }
                        //Se aplica descuenta al precio
                        cotizacionDetalle.precio = cotizacionDetalle.precio * (100 - cotizacionDetalle.porcentajeDescuento) / 100;
                        //Se calcula subtotal
                        cotizacionDetalle.subTotal = cotizacionDetalle.cantidad * cotizacionDetalle.precio;
                    }
                    else
                    {

                        if (incluidoIGV)
                        {
                            if (!cotizacionDetalle.esPrecioAlternativo)
                            {
                                cotizacionDetalle.precio = (cotizacionDetalle.precioUnitarioSinIGV + (cotizacionDetalle.precioUnitarioSinIGV * IGV));
                            }
                            else
                            {
                                cotizacionDetalle.precio = (cotizacionDetalle.precioUnitarioAlternativoSinIGV + (cotizacionDetalle.precioUnitarioAlternativoSinIGV * IGV));
                            }
                        }
                        else
                        {
                            if (!cotizacionDetalle.esPrecioAlternativo)
                            {
                                cotizacionDetalle.precio = (cotizacionDetalle.precioUnitarioSinIGV);
                            }
                            else
                            {
                                cotizacionDetalle.precio = (cotizacionDetalle.precioUnitarioAlternativoSinIGV);
                            }
                        }
                        //Se aplica descuenta al precio
                        cotizacionDetalle.precio = cotizacionDetalle.precio * (100 - cotizacionDetalle.porcentajeDescuento) / 100;
                        cotizacionDetalle.subTotal = 0;

                    }

                }*/


                /*cotizacionDetalle.porcentajeDescuento = Decimal.Parse(String.Format(decimalFormat, cotizacionDetalle.porcentajeDescuento));*/

                cotizacionDetalle.precio = Decimal.Parse(String.Format(decimalFormat, cotizacionDetalle.precio));
                cotizacionDetalle.subTotal = Decimal.Parse(String.Format(decimalFormat, cotizacionDetalle.subTotal));

            }
          



            return detalles.Count();
        }

        public void updateObservaciones()
        {
            this.Session["observaciones"] = this.Request.Params["observaciones"];
        }

      

        public void updateFlete()
        {
            this.Session["flete"] = Decimal.Parse(this.Request.Params["flete"]);
        }

    /*    public void updateCliente()
        {
            this.Session["cliente"] = this.Request.Params["cliente"];
        }*/

        public void updateContacto()
        {
            this.Session["contacto"] = this.Request.Params["contacto"];
        }

        public void updateFecha()
        {
            this.Session["fecha"] = this.Request.Params["fecha"];
        }

        public String updateIdCiudad()
        {
            Guid idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);     
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);

            
            if (this.Session["detalles"] == null)
            {
                this.Session["detalles"] = new List<CotizacionDetalle>();
            }

            List<CotizacionDetalle> detalles = (List<CotizacionDetalle>)this.Session["detalles"];


            //Para realizar el cambio de ciudad ningun producto debe estar agregado
            if (detalles.Count > 0)
            {
                // throw new Exception("No se puede cambiar de ciudad");
                return "No se puede cambiar de ciudad";
            }
            else
            {
                this.Session["idCiudad"] = idCiudad;
                if (ciudadNueva.orden > 1)
                {
                    this.Session["esProvincia"] = true;
                }
                return "{\"idCiudad\": \"" + idCiudad + "\"}";
            }
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
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
          //  this.Session["idCliente"] = idCliente.ToString();

            ClienteBL clienteBl = new ClienteBL();
            Cliente cliente = clienteBl.getCliente(idCliente);
            this.Session["cliente"] = cliente;
            this.Session["contacto"] = cliente.contacto1;

            String resultado = "{" +
                "\"descripcionCliente\":\"" + cliente.ToString() + "\"," +
                "\"idCliente\":\"" + cliente.idCliente + "\"," +
                "\"contacto\":\"" + cliente.contacto1 + "\"" +
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
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            this.Session["idProducto"] = idProducto.ToString();

            ProductoBL bl = new ProductoBL();
            Producto producto = bl.getProducto(idProducto, Boolean.Parse(this.Session["esProvincia"].ToString()));

            PrecioBL precioBl = new PrecioBL();
            List<PrecioLista> lista = new List<PrecioLista>() ;

            Decimal precioUnitarioSinIGV = producto.precio;
            Decimal precio = producto.precio;
            Decimal valor = producto.valor;

            Decimal precioUnitarioAlternativoSinIGV = 0;
            Decimal valorAlternativo = 0;
            //si la equivalencia es mayor a cero entonces quiere decir que existe una unidad alternativa
            if (producto.equivalencia > 0)
            {
                precioUnitarioAlternativoSinIGV = Decimal.Parse(String.Format(decimalFormat, precioUnitarioSinIGV / producto.equivalencia));
                valorAlternativo = valor / producto.equivalencia;
            }

            //Si esta seleccionado el boton incluido igv entonces hay que realizar el calculo
            //se calculo utilizando el precioUnitarioSinIGV, dado que el precio estandar es el que se selecciona por defecto
            if ((Boolean)this.Session["incluidoIgv"])
            {
                precio = precioUnitarioSinIGV + (precioUnitarioSinIGV * IGV);               
            }

            String resultado = "{" +
                "\"id\":\"" + producto.idProducto + "\"," +
                "\"nombre\":\"" + producto.descripcion + "\"," +
                "\"image\":\"data:image/png;base64, " + Convert.ToBase64String(producto.image) + "\"," +
                "\"unidad\":\"" + producto.unidad + "\"," +
                "\"unidad_alternativa\":\"" + producto.unidad_alternativa + "\"," +
                "\"proveedor\":\"" + producto.proveedor + "\"," +
                "\"precioUnitarioSinIGV\":\"" + precioUnitarioSinIGV + "\"," +
                "\"precioUnitarioAlternativoSinIGV\":\"" + precioUnitarioAlternativoSinIGV + "\"," +
                "\"precio\":\"" + precio+ "\","+
                "\"valor\":\"" + valor + "\"," +
                "\"valorAlternativo\":\"" + valorAlternativo + "\"" +
                "}";
            return resultado;
        }

        public String AddProducto()
        {
            PrecioBL precioBl = new PrecioBL();
            if (this.Session["detalles"] == null)
            {
                this.Session["detalles"] = new List<CotizacionDetalle>();
            }

            List<CotizacionDetalle> detalles = (List<CotizacionDetalle>)this.Session["detalles"];

    

            Guid idProducto = Guid.Parse(this.Session["idProducto"].ToString());
            CotizacionDetalle cotizacionDetalle = detalles.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
            if (cotizacionDetalle != null)
            {
                throw new System.Exception("Producto ya se encuentra en la lista");
            }


            CotizacionDetalle detalle = new CotizacionDetalle();
            ProductoBL productoBL = new ProductoBL();
            Producto producto = productoBL.getProducto(idProducto, Boolean.Parse(this.Session["esProvincia"].ToString()));
            detalle.producto = producto;
            //      detalle.idProducto = producto.idProducto;
            detalle.cantidad = Int32.Parse(Request["cantidad"].ToString());
            detalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
            detalle.precio = Decimal.Parse(Request["precio"].ToString());
            detalle.subTotal = Decimal.Parse(Request["subtotal"].ToString());



            



            detalle.precioUnitarioSinIGV = Decimal.Parse(Request["precioUnitarioSinIGV"].ToString());

            detalle.precioUnitarioAlternativoSinIGV = Decimal.Parse(Request["precioUnitarioAlternativoSinIGV"].ToString());




            detalle.image = producto.image;

            detalles.Add(detalle);

            Decimal total = Decimal.Parse(String.Format(decimalFormat, detalles.AsEnumerable().Sum(o => o.subTotal)));
            Decimal subtotal = 0;
            Decimal igv = 0;


            if ((Boolean)this.Session["incluidoIgv"])
            {

                subtotal = Decimal.Parse(String.Format(decimalFormat, total / (1 + IGV)));
                igv = Decimal.Parse(String.Format(decimalFormat, total - subtotal));
            }
            else
            {
                subtotal = total;
                igv = Decimal.Parse(String.Format(decimalFormat, total * IGV));
                total = Decimal.Parse(String.Format(decimalFormat, subtotal + igv));
            }

            detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;

            detalle.unidad = detalle.producto.unidad;
            detalle.valorUnitario = Decimal.Parse(Request["valor"].ToString());
            //si esPrecioAlternativo es verdadero se mostrará la unidad alternativa
            if (detalle.esPrecioAlternativo)
            {
                detalle.unidad = detalle.producto.unidad_alternativa;
                detalle.valorUnitario = Decimal.Parse(Request["valorAlternativo"].ToString());
            }





            String resultado = "{" +
                "\"idProducto\":\"" + detalle.producto.idProducto + "\"," +
                "\"proveedor\":\"" + detalle.producto.proveedor + "\"," +
                "\"codigoProducto\":\"" + detalle.producto.sku + "\"," +
                "\"nombreProducto\":\"" + detalle.producto.descripcion + "\"," +
                "\"unidad\":\"" + detalle.unidad + "\"," +
                "\"cantidad\":\"" + detalle.cantidad.ToString() + "\"," +
                "\"porcentajeDescuento\":\"" + detalle.porcentajeDescuento.ToString() + "\"," +
                "\"valorUnitario\":\"" + detalle.valorUnitario.ToString() + "\"," +
                "\"precio\":\"" + detalle.precio.ToString() + "\"," +
                "\"precioUnitarioSinIGV\":\"" + detalle.precioUnitarioSinIGV.ToString() + "\"," +
                "\"precioUnitarioAlternativoSinIGV\":\"" + detalle.precioUnitarioAlternativoSinIGV.ToString() + "\"," +
                "\"esPrecioAlternativo\":\"" + (detalle.esPrecioAlternativo ? 1 : 0).ToString() + "\"," +
                "\"subTotal\":\"" + detalle.subTotal.ToString() + "\"," +
                "\"igv\":\"" + igv.ToString() + "\", " +
                "\"subTotal2\":\"" + subtotal.ToString() + "\", " +
                "\"total\":\"" + total.ToString() + "\"}";

            

            this.Session["detalles"] = detalles;
            return resultado;
            
            
        }

        public String DelProducto()
        {
            List<CotizacionDetalle> detalles = (List<CotizacionDetalle>)this.Session["detalles"];
            Guid idProducto = Guid.Parse(this.Request.Params["idProducto"]);
            CotizacionDetalle cotizacionDetalle = detalles.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
            if(cotizacionDetalle != null)
            { 
                detalles.Remove(cotizacionDetalle);
                this.Session["detalles"] = detalles;
            }
            return detalles.AsEnumerable().Sum(o => o.subTotal).ToString();
        }



        public void GenerarPDF2()
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
                        //    IRow irow = sheet.GetRow(row);
                        // Type type = sheet.GetRow(row).GetCell(2).GetType();

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


        public ActionResult GenerarPDF()
        {
            Cotizacion cot = new Cotizacion();

            String[] fecha = Request["fecha"].ToString().Split('/');
            cot.fecha = new DateTime(Convert.ToInt32(fecha[2]), Convert.ToInt32(fecha[1]), Convert.ToInt32(fecha[0]));
            
            cot.idCiudad = Guid.Parse(Request["idCiudad"].ToString());

            Cliente cliente = (Cliente)this.Session["cliente"];        
            cliente.contacto1 = Request["contacto"].ToString();

            cot.considerarCantidades = (Boolean)this.Session["considerarCantidades"];

            cot.igv = IGV;

            cot.incluidoIgv = short.Parse(Request["igv"]);
       //     cot.mostrarCodProveedor = short.Parse(Request["mostrarcodproveedor"]);
       //     cot.idMoneda = Guid.Parse(Request["moneda"].ToString());
            //cot.idTipoCambio = Guid.Parse(Request["tipocambio"].ToString());
       //     cot.idPrecio = Guid.Parse(Request["precio"].ToString());
            cot.flete = Decimal.Parse(Request["flete"].ToString());
          //  cot.moneda = (Moneda)this.Session["moneda"];

            CiudadBL ciudadBl = new CiudadBL();
            cot.ciudad = ciudadBl.getCiudad(cot.idCiudad);

            Usuario usuario = (Usuario)this.Session["usuario"];
            //cot.usuarioCreacion = usuario.apellidos + "_" + usuario.nombres;
            cot.usuario = usuario;
            //   ClienteBL clienteBl = new ClienteBL();
            cot.cliente = cliente; // clienteBl.getCliente(cot.idCliente);*/
            
            List<CotizacionDetalle> detalles = (List<CotizacionDetalle>)this.Session["detalles"];

            cot.detalles = detalles;

            cot.montoSubTotal = Decimal.Parse(Request["montoSubTotal"].ToString());
            cot.montoIGV = Decimal.Parse(Request["montoIGV"].ToString());
            cot.montoTotal = Decimal.Parse(Request["montoTotal"].ToString());
            cot.montoFlete = Decimal.Parse(Request["montoFlete"].ToString());
            cot.montoTotalMasFlete = Decimal.Parse(Request["montoTotalMasFlete"].ToString());

            cot.observaciones = Request["observaciones"].ToString();


            CotizacionBL bl = new CotizacionBL();
            bl.InsertCotizacion(cot);

            GeneradorPDF gen = new GeneradorPDF();
            gen.generarPDFExtended(cot);
            

            return Redirect("/pdfs/" + cot.usuarioCreacion + ".pdf");
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