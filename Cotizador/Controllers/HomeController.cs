﻿using BusinessLayer;
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
            if (this.Session["cotizacionBusqueda"] == null)
            {
                Cotizacion cotizacionTmp = new Cotizacion();
                cotizacionTmp.fecha = DateTime.Now.AddDays(-10);
                cotizacionTmp.fechaHasta = DateTime.Now;
                cotizacionTmp.ciudad = new Ciudad();
                cotizacionTmp.cliente = new Cliente();
                cotizacionTmp.grupo = new Grupo();
                cotizacionTmp.estadoAprobacion = -1;
                this.Session["cotizacionBusqueda"] = cotizacionTmp;
            }

            Cotizacion cotizacionSearch = (Cotizacion)this.Session["cotizacionBusqueda"];

            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.fecha = cotizacionSearch.fecha.ToString("dd/MM/yyyy");
            ViewBag.fechaHasta = cotizacionSearch.fechaHasta.ToString("dd/MM/yyyy");


            if (this.Session["cotizacionList"] == null)
            {
                this.Session["cotizacionList"] = new List<Cotizacion>();
            }

            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacionBusqueda"];
            int existeCliente = 0;
            //  if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
            if(cotizacion.cliente.idCliente != Guid.Empty)
            {
                existeCliente = 1;
            }

            if (cotizacion.cliente.idCliente != Guid.Empty)
            {
                ViewBag.idClienteGrupo = cotizacion.cliente.idCliente;
                ViewBag.clienteGrupo = cotizacion.cliente.ToString();
            }
         /*   else
            {
                ViewBag.idClienteGrupo = cotizacion.grupo.idGrupo;
                ViewBag.clienteGrupo = cotizacion.grupo.ToString();
            }
            */

            ViewBag.cotizacionList = this.Session["cotizacionList"];
            ViewBag.usuario = this.Session["usuario"];
            ViewBag.cotizacion = cotizacion;
            ViewBag.numero = cotizacion.codigo;
            ViewBag.idCiudad = cotizacion.ciudad.idCiudad;
            
            
            ViewBag.existeCliente = existeCliente;

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

            //Se agrega fechaHasta en la busqueda
            String[] fechaHasta = this.Request.Params["fechaHasta"].Split('/');
            cotizacion.fechaHasta = new DateTime(Int32.Parse(fechaHasta[2]), Int32.Parse(fechaHasta[1]), Int32.Parse(fechaHasta[0]),23,59,59);

            cotizacion.estadoAprobacion = short.Parse(this.Request.Params["estadoAprobacion"].ToString());


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

            this.Session["cotizacionBusqueda"] = cotizacion;

            return cotizacionList.Count();
        }


        public ActionResult Exit()
        {
            this.Session["usuario"] = null;
            this.Session["cotizacion"] = null;
            return RedirectToAction("Login", "Account");
        }

        public ActionResult Index()
        {

            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            Usuario usuario = (Usuario)this.Session["usuario"];

            ViewBag.debug = 1;
            ViewBag.Si = "Sí";
            ViewBag.No = "No";
            ViewBag.IGV = Constantes.IGV;

            //Si no se está trabajando con una cotización se crea una

            if (this.Session["cotizacion"] == null)
            {

                Cotizacion cotizacionTmp = new Cotizacion();
                cotizacionTmp.mostrarCodigoProveedor = true;
                cotizacionTmp.fecha = DateTime.Now;
                cotizacionTmp.fechaVigenciaInicio = DateTime.Now;
                cotizacionTmp.fechaVigenciaLimite = cotizacionTmp.fecha.AddDays(15);
                cotizacionTmp.ciudad = new Ciudad();
                cotizacionTmp.cliente = new Cliente();
                cotizacionTmp.grupo = new Grupo();
                cotizacionTmp.cotizacionDetalleList = new List<CotizacionDetalle>();
                cotizacionTmp.igv = Constantes.IGV;
                cotizacionTmp.flete = 0;
                cotizacionTmp.diasVigencia = Constantes.diasVigencia;
                cotizacionTmp.considerarCantidades = true;
                cotizacionTmp.usuario = usuario;
                cotizacionTmp.observaciones = "* Condiciones de pago: al contado.\n" +
                                       "* Entrega en almacén del cliente, 48 horas luego de la recepción del pedido o la orden de compra.\n" +
                                       "* (para productos no stockeables o primeras compras, consultar plazo).\n";



                this.Session["cotizacion"] = cotizacionTmp;
            }
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];


            int existeCliente = 0;
            if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
            {
                existeCliente = 1;
            }

            if (cotizacion.cliente.idCliente != Guid.Empty)
            {
                ViewBag.idClienteGrupo = cotizacion.cliente.idCliente;
                ViewBag.clienteGrupo = cotizacion.cliente.ToString();
            }
            else
            {
                ViewBag.idClienteGrupo = cotizacion.grupo.idGrupo;
                ViewBag.clienteGrupo = cotizacion.grupo.ToString();
            }

            ViewBag.nombreUsuario = usuario.nombre;

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


            ViewBag.flete = cotizacion.flete;
            ViewBag.observaciones = cotizacion.observaciones;
            ViewBag.fecha = cotizacion.fecha.ToString("dd/MM/yyyy");
            ViewBag.fechaVigencia = cotizacion.fechaVigenciaLimite.ToString("dd/MM/yyyy");
            ViewBag.fechaVigenciaInicio = cotizacion.fechaVigenciaInicio.ToString("dd/MM/yyyy");
            ViewBag.mostrarCodigoProveedor = cotizacion.mostrarCodigoProveedor;
            Decimal total = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacion.cotizacionDetalleList.AsEnumerable().Sum(o => o.subTotal)));
         


            ViewBag.igv = cotizacion.montoIGV;
            ViewBag.total = cotizacion.montoTotal;
            ViewBag.subtotal = cotizacion.montoSubTotal;


            ViewBag.cotizacion = cotizacion;

            return View();
            
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
                //cotizacionDetalle.incluyeIGV = cotizacion.incluidoIgv;

                Decimal precioNeto = cotizacionDetalle.producto.precioSinIgv;
                Decimal costo = cotizacionDetalle.producto.costoSinIgv;

                //Se calcula el precio con Flete
             //   precioNeto = precioNeto + (precioNeto * cotizacion.flete / 100);

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
                //El flete no afecta al precioLista
                //cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.decimalFormat, precioNeto));
                //Se define el costoLista del producto como el costo calculado

                //El precio y el costo se setean al final dado que si es equivalente en cada get se hará el recalculo

                cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.decimalFormat, precioNeto));
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


        public void updateMostrarCosto()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.mostrarCosto = Boolean.Parse(this.Request.Params["mostrarCosto"]);
            this.Session["cotizacion"] = cotizacion;
        }

        public void updateConsiderarDescontinuados()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.considerarDescontinuados = Boolean.Parse(this.Request.Params["considerarDescontinuados"]);
            this.Session["cotizacion"] = cotizacion;
        }
        


        public void updateFlete()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.flete = Decimal.Parse(this.Request.Params["flete"]);
            //actualizarCotizacionDetalles(cotizacion.incluidoIgv?1:0);
            this.Session["cotizacion"] = cotizacion;
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

        public void updateTipoVigencia()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.tipoVigencia = int.Parse(this.Request.Params["tipoVigencia"]);
            this.Session["cotizacion"] = cotizacion;
        }

        public void updateFechaVigenciaInicio()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            String[] fecha = this.Request.Params["fechaVigenciaInicio"].Split('/');
            cotizacion.fechaVigenciaInicio = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.Session["cotizacion"] = cotizacion;
        }

        public void updateFechaVigencia()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            String[] fecha = this.Request.Params["fechaVigencia"].Split('/');
            cotizacion.fechaVigenciaLimite = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.Session["cotizacion"] = cotizacion;
        }


        public void updateDiasVigencia()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.diasVigencia = int.Parse(this.Request.Params["diasVigencia"]);
            this.Session["cotizacion"] = cotizacion;
        }

        public String updateIdCiudad()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            Guid idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);     

            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
            cotizacion.cliente = new Cliente();
            cotizacion.grupo = new Grupo();


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


        public void updateProveedor()
        {
            this.Session["proveedor"] = this.Request.Params["proveedor"];
        }

        public void updateFamilia()
        {
            this.Session["familia"] = this.Request.Params["familia"];
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
                    cotizacionDetalle.flete = cotizacionDetalleJson.flete;

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
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];

            List<Cliente> clienteList = clienteBL.getCLientesBusqueda(data, cotizacion.ciudad.idCiudad);

   
            


            GrupoBL grupoBL = new GrupoBL();
            List<Grupo> grupoList = grupoBL.getGruposBusqueda(data);

            String resultado = "{\"q\":\"" + data + "\",\"results\":[";
            Boolean existeClienteGrupo = false;
            foreach (Cliente cliente in clienteList)
            {
                resultado += "{\"id\":\"c" + cliente.idCliente + "\",\"text\":\"" + cliente.ToString() + "\"},";
                existeClienteGrupo = true;
            }
            foreach (Grupo grupo in grupoList)
            {
                resultado += "{\"id\":\"g" + grupo.idGrupo + "\",\"text\":\"" + grupo.ToString() + "\"},";
                existeClienteGrupo = true;
            }

            if (existeClienteGrupo)
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

            //Se crea un grupo vacío para no considerarlo al momento de grabar la cotización
            cotizacion.grupo = new Grupo();

            String resultado = "{" +
                "\"descripcionCliente\":\"" + cotizacion.cliente.ToString() + "\"," +
                "\"idCliente\":\"" + cotizacion.cliente.idCliente + "\"," +
                "\"contacto\":\"" + cotizacion.cliente.contacto1 + "\"" +
                "}";
            return resultado;
        }

        public String GetGrupo()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            Guid idGrupo = Guid.Parse(Request["idGrupo"].ToString());
            GrupoBL grupoBl = new GrupoBL();
            cotizacion.grupo = grupoBl.getGrupo(idGrupo);            
            cotizacion.contacto = cotizacion.grupo.contacto;

            //Se crea un cliente vacío para no considerarlo al momento de grabar la cotización
            cotizacion.cliente = new Cliente();

            String resultado = "{" +
                "\"descripcionGrupo\":\"" + cotizacion.grupo.ToString() + "\"," +
                "\"idGrupo\":\"" + cotizacion.grupo.idGrupo + "\"," +
                "\"contacto\":\"" + cotizacion.grupo.contacto + "\"" +
                "}";
            return resultado;
        }




        public String GetProductos()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            String texto_busqueda = this.Request.Params["data[q]"];

            String proveedor = "Todos";
            String familia = "Todas";
            if (this.Session["proveedor"] != null)
            {
                proveedor = (String)this.Session["proveedor"];
            }

            if (this.Session["familia"] != null)
            {
                familia = (String)this.Session["familia"];
            }


            ProductoBL bl = new ProductoBL();
            List<Producto> lista = bl.getProductosBusqueda(texto_busqueda, cotizacion.considerarDescontinuados, proveedor, familia);

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
            Producto producto = bl.getProducto(idProducto, cotizacion.ciudad.esProvincia , cotizacion.incluidoIgv);

            Decimal fleteDetalle = Decimal.Parse(String.Format(Constantes.decimalFormat, producto.costoLista * (cotizacion.flete) / 100));
            Decimal precioUnitario = Decimal.Parse(String.Format(Constantes.decimalFormat, fleteDetalle + producto.precioLista));

            String resultado = "{" +
                "\"id\":\"" + producto.idProducto + "\"," +
                "\"nombre\":\"" + producto.descripcion + "\"," +
                "\"image\":\"data:image/png;base64, " + Convert.ToBase64String(producto.image) + "\"," +
                "\"unidad\":\"" + producto.unidad + "\"," +
                "\"unidad_alternativa\":\"" + producto.unidad_alternativa + "\"," +
                "\"proveedor\":\"" + producto.proveedor + "\"," +
                "\"familia\":\"" + producto.familia + "\"," +
                "\"precioUnitarioSinIGV\":\"" + producto.precioSinIgv + "\"," +
                "\"precioUnitarioAlternativoSinIGV\":\"" + producto.precioAlternativoSinIgv + "\"," +
                "\"precioLista\":\"" + producto.precioLista + "\"," +
                "\"costoSinIGV\":\"" + producto.costoSinIgv + "\"," +
                "\"costoAlternativoSinIGV\":\"" + producto.costoAlternativoSinIgv + "\"," +
                "\"fleteDetalle\":\"" + fleteDetalle + "\"," +
                "\"precioUnitario\":\"" + precioUnitario + "\"," +
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
            Producto producto = productoBL.getProducto(idProducto, cotizacion.ciudad.esProvincia, cotizacion.incluidoIgv );
            detalle.producto = producto;

            detalle.cantidad = Int32.Parse(Request["cantidad"].ToString());
            detalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
            detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;
            decimal precioNeto = Decimal.Parse(Request["precio"].ToString());
            decimal costo = Decimal.Parse(Request["costo"].ToString());
            decimal flete = Decimal.Parse(Request["flete"].ToString());
            if (detalle.esPrecioAlternativo)
            {
                //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                //dado que cuando se hace get al precioNeto se recupera diviendo entre la equivalencia
                detalle.precioNeto = Decimal.Parse(String.Format(Constantes.decimalFormat, precioNeto * producto.equivalencia));
            }
            else
            {
                detalle.precioNeto = precioNeto;
            }
            detalle.flete = flete;
            cotizacion.cotizacionDetalleList.Add(detalle);

            //Calcula los montos totales de la cabecera de la cotizacion
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
                "\"precioUnitario\":\"" + detalle.precioUnitario + "\", " +
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


        [HttpGet]
        public ActionResult LoadClientes()
        {
            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();

        }


        [HttpPost]
        public ActionResult LoadClientesFile(HttpPostedFileBase file)
        {

            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);
            }

        
            HSSFWorkbook hssfwb;

            ClienteBL clienteBL = new ClienteBL();
           
            hssfwb = new HSSFWorkbook(file.InputStream);

            ISheet sheet = hssfwb.GetSheetAt(0);
            int row = 1;
            //sheet.LastRowNum

            int cantidad = Int32.Parse(Request["cantidadRegistros"].ToString());
            String sede = Request["sede"].ToString();

            clienteBL.truncateClienteStaging(sede);

            if (cantidad == 0)
                cantidad = sheet.LastRowNum;

            for ( row = 4; row <= cantidad; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    int paso = 0;
                    try
                    {
                        ClienteStaging clienteStaging = new ClienteStaging();
                        //C
                        clienteStaging.codigo = sheet.GetRow(row).GetCell(2).ToString();
                        //D
                        paso = 1;
                        clienteStaging.nombre = sheet.GetRow(row).GetCell(3).ToString();
                        //F
                        paso = 2;
                        clienteStaging.documento = sheet.GetRow(row).GetCell(5).ToString();
                        //G
                        paso = 3;
                        clienteStaging.domicilioLegal = sheet.GetRow(row).GetCell(6).ToString();
                        //H
                        paso = 4;
                        clienteStaging.distrito = sheet.GetRow(row).GetCell(7).ToString();
                        //J
                        paso = 5;
                        clienteStaging.codVe = sheet.GetRow(row).GetCell(9).ToString();
                        //M
                        paso = 6;
                        clienteStaging.direccionDespacho = sheet.GetRow(row).GetCell(12).ToString();
                        //T
                        paso = 7;
                        clienteStaging.nombreComercial = sheet.GetRow(row).GetCell(19).ToString();
                        //U
                        paso = 8;
                        clienteStaging.rubro = sheet.GetRow(row).GetCell(20).ToString();

                        clienteStaging.sede = sede;

                            /*
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
                        clienteStaging.rubro = sheet.GetRow(row).GetCell(11).ToString();*/

                        clienteBL.setClienteStaging(clienteStaging);

                    }
                    catch (Exception ex)
                    {
                        Usuario usuario = (Usuario)this.Session["usuario"];
                        Log log = new Log(ex.ToString()+" paso: " + paso , TipoLog.Error, usuario);
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
        public ActionResult LoadPrecios()
        {
            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();

        }


        [HttpPost]
        public ActionResult LoadPreciosFile(HttpPostedFileBase file)
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
        public ActionResult LoadProductos()
        {
            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();

        }


        [HttpPost]
        public ActionResult LoadProductosFile(HttpPostedFileBase file)
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
            int cantidad = Int32.Parse(Request["cantidad"].ToString());
            if (cantidad == 0)
                cantidad = sheet.LastRowNum;

            cantidad = 2008;
            //sheet.LastRowNum
            for (row = 2; row <= cantidad; row++)
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
                        //H
                        if (sheet.GetRow(row).GetCell(7) == null)
                        {
                            productoStaging.unidad = null;
                        }
                        else
                        {
                            productoStaging.unidadAlternativa = sheet.GetRow(row).GetCell(7).ToString();
                        }

                        paso = 7;
                        //J
                        if (sheet.GetRow(row).GetCell(8) == null)
                        {
                            productoStaging.equivalencia = 1;
                        }
                        else
                        {
                            productoStaging.equivalencia = Int32.Parse(sheet.GetRow(row).GetCell(8).ToString());
                        }

                        paso = 8;
                        //K
                        if (sheet.GetRow(row).GetCell(9) == null)
                        {
                            productoStaging.descripcion = null;
                        }
                        else
                        {
                            productoStaging.descripcion = sheet.GetRow(row).GetCell(9).ToString();
                        }

                        paso = 9;
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

                        paso = 10;
                        try {
                            //Y
                            Double? precioLima = sheet.GetRow(row).GetCell(24).NumericCellValue;
                            productoStaging.precioLima = Convert.ToDecimal(precioLima);
                        }
                        catch (Exception e)
                        {
                            productoStaging.precioLima = 0;
                        }


                        paso = 11;
                        try
                        {
                            //K
                            Double? precioProvincias = sheet.GetRow(row).GetCell(27).NumericCellValue;
                            productoStaging.precioProvincias = Convert.ToDecimal(precioProvincias);
                        }
                        catch (Exception e)
                        {
                            productoStaging.precioProvincias = 0;
                        }
                        
                        productoBL.setProductoStaging(productoStaging);
                    }
                    catch (Exception ex)
                    {
                        
                        Usuario usuario = (Usuario)this.Session["usuario"];
                        Log log = new Log(ex.ToString() + " paso:"+ paso, TipoLog.Error, usuario);
                        LogBL logBL = new LogBL();
                        logBL.insertLog(log);


                    }
                }
            }
            //productoBL.mergeProductoStaging();
            row = row;
            return RedirectToAction("Index", "Home");
        }



        [HttpGet]
        public ActionResult LoadFacturas()
        {
            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();

        }

        [HttpPost]
        public ActionResult LoadFacturasFile(HttpPostedFileBase file)
        {

            if (file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                file.SaveAs(path);
            }


            HSSFWorkbook hssfwb;

            FacturaBL facturaBL = new FacturaBL();
            facturaBL.truncateFacturaStaging();

            hssfwb = new HSSFWorkbook(file.InputStream);

            int numero = 0;

            for (int j = 0; j < 8; j++)
            {

                ISheet sheet = hssfwb.GetSheetAt(j);
                int row = 1;
                //sheet.LastRowNum

                int cantidad = Int32.Parse(Request["cantidad"].ToString());
              //  if (cantidad == 0)
                    cantidad = sheet.LastRowNum;


            

                for (row = 3; row <= cantidad; row++)
                {
                    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                    {
                        try
                        {
                            FacturaStaging facturaStaging = new FacturaStaging();
                            //A
                            facturaStaging.tipoDocumento = sheet.GetRow(row).GetCell(0).ToString();
                            //B
                            facturaStaging.numeroDocumento = Convert.ToInt32(sheet.GetRow(row).GetCell(1).NumericCellValue);
                            //C
                            facturaStaging.fecha = sheet.GetRow(row).GetCell(2).DateCellValue;
                            //D
                            facturaStaging.codigoCliente = sheet.GetRow(row).GetCell(3).ToString();
                            //E
                            //K
                            facturaStaging.valorVenta = Convert.ToDecimal(sheet.GetRow(row).GetCell(8).NumericCellValue);
                            facturaStaging.igv = Convert.ToDecimal(sheet.GetRow(row).GetCell(9).NumericCellValue);
                            facturaStaging.total = Convert.ToDecimal(sheet.GetRow(row).GetCell(10).NumericCellValue);
                            facturaStaging.observacion = sheet.GetRow(row).GetCell(11).ToString();

                            facturaStaging.fechaVencimiento = sheet.GetRow(row).GetCell(13).DateCellValue;
                            try
                            {
                                if (sheet.GetRow(row).GetCell(14) ==  null)
                                    facturaStaging.ruc = null;
                                else
                                    facturaStaging.ruc = sheet.GetRow(row).GetCell(14).ToString();
                            }
                            catch (Exception ex)
                            {
                                facturaStaging.ruc = sheet.GetRow(row).GetCell(14).NumericCellValue.ToString();
                            }
                            
                            //F
                            facturaStaging.razonSocial = sheet.GetRow(row).GetCell(15).StringCellValue;


                            switch (j)
                            {
                                case 0: facturaStaging.sede = "L"; break;
                                case 1: facturaStaging.sede = "A"; break;
                                case 2: facturaStaging.sede = "C"; break;
                                case 3: facturaStaging.sede = "H"; break;
                                case 4: facturaStaging.sede = "O"; break;
                                case 5: facturaStaging.sede = "P"; break;
                                case 6: facturaStaging.sede = "Q"; break;
                                case 7: facturaStaging.sede = "T"; break;                          
                            }

                            if (facturaStaging.tipoDocumento.Trim().Equals("F"))
                            {
                                numero++;
                                facturaStaging.numero = numero;
                            }
                            else
                            {
                                facturaStaging.numero = 0;
                            }
                            

                            facturaBL.setFacturaStaging(facturaStaging);

                        }
                        catch (Exception ex)
                        {
                            Usuario usuario = (Usuario)this.Session["usuario"];
                            Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                            LogBL logBL = new LogBL();
                            logBL.insertLog(log);
                        }
                    }
                }

                row = row;
            }

     //       facturaBL.mergeClienteStaging();
            
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


        public void aprobarCotizacion()
        {
            CotizacionBL cotizacionBL = new CotizacionBL();

            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = Int64.Parse(Request["codigo"].ToString());
            cotizacion.estadoAprobacion = short.Parse(Request["accion"].ToString());
            cotizacion.motivoRechazo = Request["motivoRechazo"].ToString();
            cotizacion.usuario = (Usuario)this.Session["usuario"];
            cotizacion = cotizacionBL.aprobarCotizacion(cotizacion);
           

           // this.Session["cotizacion"] = cotizacion;
        }

        public void recotizacion()
        {
            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = Int64.Parse(Request["numero"].ToString());
            //
            cotizacion.esRecotizacion = true;
            
            cotizacion = cotizacionBL.GetCotizacion(cotizacion);
            //Se seta el codigo y estadoAprobacion en 0 porque una recotización es una nueva cotización
            cotizacion.codigo = 0;
            cotizacion.estadoAprobacion = 0;
            cotizacion.fechaVigenciaInicio = cotizacion.fechaVigenciaLimite.AddDays(1);
            cotizacion.fechaVigenciaLimite = cotizacion.fechaVigenciaLimite.AddDays(16);
            this.Session["cotizacion"] = cotizacion;
        }


        



        public Cotizacion insertarCotizacion()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            Usuario usuario = (Usuario)this.Session["usuario"];
            cotizacion.usuario = usuario;
            CotizacionBL bl = new CotizacionBL();

            // && !cotizacion.esRecotizacion
            if (cotizacion.codigo > 0)
            {
                bl.UpdateCotizacion(cotizacion);
            }
            else
            { 
                bl.InsertCotizacion(cotizacion);
            }
            cotizacion.esRecotizacion = false;
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


        public String AddCliente()
        {
            Cliente cliente = new Cliente();
            Usuario usuario = (Usuario)this.Session["usuario"];
            cliente.IdUsuarioRegistro = usuario.idUsuario;
            cliente.razonSocial = Request["razonSocial"].ToString();
            cliente.nombreComercial = Request["nombreComercial"].ToString();
            cliente.ruc = Request["ruc"].ToString();
            cliente.contacto1 = Request["contacto"].ToString();

            ClienteBL clienteBL = new ClienteBL();
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cliente.ciudad = cotizacion.ciudad;
            cotizacion.cliente = clienteBL.insertCliente(cliente);
            cotizacion.contacto = cotizacion.cliente.contacto1;

            String resultado = "{" +
                "\"idCLiente\":\"" + cotizacion.cliente.idCliente + "\"," +
                "\"codigoAlterno\":\"" + cotizacion.cliente.codigoAlterno + "\"}";

            return resultado;
        }

    }
}