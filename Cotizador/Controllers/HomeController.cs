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
using Newtonsoft.Json;

namespace Cotizador.Controllers
{
    public class HomeController : Controller
    {
     

        //Nueva Cotización
       



        public ActionResult Index()
        {


            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //  crearCotizacion();

            if (this.Session["cotizacionBusqueda"] == null)
            {
                Cotizacion cotizacionTmp = new Cotizacion();
                cotizacionTmp.fechaDesde = DateTime.Now.AddDays(-10);
                cotizacionTmp.fechaHasta = DateTime.Now;
                cotizacionTmp.ciudad = new Ciudad();
                cotizacionTmp.cliente = new Cliente();
                cotizacionTmp.grupo = new Grupo();
                cotizacionTmp.seguimientoCotizacion = new SeguimientoCotizacion();
                cotizacionTmp.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Todos;
               // cotizacionTmp.cotizacionDetalleList = new List<CotizacionDetalle>();
                cotizacionTmp.usuario = (Usuario)this.Session["usuario"];
                cotizacionTmp.usuarioBusqueda = cotizacionTmp.usuario;
                this.Session["cotizacionBusqueda"] = cotizacionTmp;
            }

            Cotizacion cotizacionSearch = (Cotizacion)this.Session["cotizacionBusqueda"];


            ViewBag.fechaDesde = cotizacionSearch.fechaDesde.ToString("dd/MM/yyyy");
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

          
            ViewBag.cotizacion = cotizacion;

            ViewBag.cotizacionList =  this.Session["cotizacionList"];
            ViewBag.existeCliente = existeCliente;

            return View();
        }




        /*Ejecución de la búsqueda de cotizaciones*/
        public int SearchCotizaciones()
        {
            //Se recupera la cotizacion de la session
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacionBusqueda"];
            CotizacionBL cotizacionBL = new CotizacionBL();
            List<Cotizacion> cotizacionList = cotizacionBL.GetCotizaciones(cotizacion);
            //Se coloca en session el resultado de la búsqueda
            this.Session["cotizacionList"] = cotizacionList;
            //Se retorna la cantidad de elementos encontrados
            return cotizacionList.Count();
        }


        public ActionResult Exit()
        {
            //Se eliminan todos los datos de Session
            this.Session["usuario"] = null;
            this.Session["cotizacion"] = null;
            this.Session["cotizacionBusqueda"] = null;
            this.Session["cotizacionList"] = null;
            return RedirectToAction("Login", "Account");
        }



        /*Pagina principal, muestra formulario de creación de cotización*/
        


        public ActionResult NuevaCotizacionDesdePrecios()
        {
            this.Session["cotizacion"] = null;
            return RedirectToAction("CotizadorDesdePrecios", "Home");
        }


        public ActionResult CotizadorDesdePrecios()
        {
            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.debug = Constantes.debug;
            ViewBag.Si = "Sí";
            ViewBag.No = "No";
            ViewBag.IGV = Constantes.IGV;


            //Si no se está trabajando con una cotización se crea una y se agrega a la sesion
            if (this.Session["cotizacion"] == null)
            {

                crearCotizacion();
            }

            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];

            ViewBag.cotizacion = cotizacion;
            cotizacion.fecha = DateTime.Now.AddDays(-728);
            ViewBag.fecha = cotizacion.fecha.ToString("dd/MM/yyyy");

            this.Session["cotizacion"] = cotizacion;

            return View();

        }

        

        private void crearCotizacion()
        {
            Cotizacion cotizacionTmp = new Cotizacion();
            cotizacionTmp.mostrarCodigoProveedor = true;
            cotizacionTmp.fecha = DateTime.Now;
            cotizacionTmp.fechaInicioVigenciaPrecios = null;
            cotizacionTmp.fechaFinVigenciaPrecios = null;
            cotizacionTmp.fechaLimiteValidezOferta = cotizacionTmp.fecha.AddDays(Constantes.plazoOfertaDias);
            cotizacionTmp.ciudad = new Ciudad();
            cotizacionTmp.cliente = new Cliente();
            cotizacionTmp.grupo = new Grupo();
            cotizacionTmp.cotizacionDetalleList = new List<CotizacionDetalle>();
            cotizacionTmp.igv = Constantes.IGV;
            cotizacionTmp.flete = 0;
            cotizacionTmp.validezOfertaEnDias = Constantes.plazoOfertaDias;
            cotizacionTmp.considerarCantidades = Cotizacion.OpcionesConsiderarCantidades.Cantidades;
            Usuario usuario = (Usuario)this.Session["usuario"];
            cotizacionTmp.usuario = usuario;
            cotizacionTmp.observaciones = Constantes.observacionesCotizacion;
            cotizacionTmp.incluidoIgv = false;
            cotizacionTmp.seguimientoCotizacion = new SeguimientoCotizacion();

            this.Session["cotizacion"] = cotizacionTmp;
        }

        public ActionResult NuevaCotizacion()
        {
            this.Session["cotizacion"] = null;
            return RedirectToAction("Cotizador", "Home");
        }

        public Boolean ConsultarExisteCotizacion()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            if (cotizacion == null)
                return false;
            else
                return true;
        }

        public ActionResult CancelarCreacionCotizacion()
        {
            this.Session["cotizacion"] = null;
            return RedirectToAction("Index", "Home");
        }


        public ActionResult Cotizador()
        { 
            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.debug = Constantes.debug;
            ViewBag.Si = "Sí";
            ViewBag.No = "No";
            ViewBag.IGV = Constantes.IGV;


            //Si no se está trabajando con una cotización se crea una y se agrega a la sesion

            if (this.Session["cotizacion"] == null)
            {

                crearCotizacion();
            }
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];


            int existeCliente = 0;
            if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
            {
                existeCliente = 1;
            }
            ViewBag.existeCliente = existeCliente;


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


            ViewBag.fecha = cotizacion.fecha.ToString("dd/MM/yyyy");
            ViewBag.fechaLimiteValidezOferta = cotizacion.fecha.ToString("dd/MM/yyyy");
            ViewBag.fechaInicioVigenciaPrecios = cotizacion.fechaInicioVigenciaPrecios == null ? null : cotizacion.fechaInicioVigenciaPrecios.Value.ToString("dd/MM/yyyy");
            ViewBag.fechaFinVigenciaPrecios = cotizacion.fechaFinVigenciaPrecios == null ? null : cotizacion.fechaFinVigenciaPrecios.Value.ToString("dd/MM/yyyy");

            //Se agrega el viewbag numero para poder mostrar el campo vacío cuando no se está creando una cotización
            ViewBag.numero = cotizacion.codigo;
            
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
            cotizacion.considerarCantidades = (Cotizacion.OpcionesConsiderarCantidades)considerarCantidades;

            if (cotizacion.considerarCantidades != Cotizacion.OpcionesConsiderarCantidades.Observaciones)
            {
                List<CotizacionDetalle> detalles = cotizacion.cotizacionDetalleList;

                foreach (CotizacionDetalle cotizacionDetalle in detalles)
                {
                    //Si el detalle de cotización tiene items con cantidad cero entonces se coloca 1 para que se recalcule el subtotal por item y por cotización
                    if (cotizacionDetalle.cantidad == 0)
                        cotizacionDetalle.cantidad = 1;
                }

            }
                
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

        public void updateCodigoCotizacionBusqueda()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacionBusqueda"];
            try
            {
                cotizacion.codigo = int.Parse(this.Request.Params["codigo"]);
            }
            catch (Exception ex)
            {
                cotizacion.codigo = 0;
            }
            this.Session["cotizacionBusqueda"] = cotizacion;
        }


        public void updateEstadoCotizacionBusqueda()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacionBusqueda"];
            try
            {
                cotizacion.seguimientoCotizacion.estado = (SeguimientoCotizacion.estadosSeguimientoCotizacion)int.Parse(this.Request.Params["estado"]);
            }
            catch (Exception ex)
            {
            }
            this.Session["cotizacionBusqueda"] = cotizacion;
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


        #region CONTROL DE FECHAS

        public void updateFecha()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            String[] fecha = this.Request.Params["fecha"].Split('/');
            cotizacion.fecha = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.Session["cotizacion"] = cotizacion;
        }

        public void updateMostrarValidezOfertaEnDias()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.mostrarValidezOfertaEnDias = int.Parse(this.Request.Params["mostrarValidezOfertaEnDias"]);
            this.Session["cotizacion"] = cotizacion;
        }


        public void updateValidezOfertaEnDias()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.validezOfertaEnDias = int.Parse(this.Request.Params["validezOfertaEnDias"]);
            this.Session["cotizacion"] = cotizacion;
        }


        public void updateFechaLimiteValidezOferta()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            String[] fecha = this.Request.Params["fechaLimiteValidezOferta"].Split('/');
            cotizacion.fechaLimiteValidezOferta = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.Session["cotizacion"] = cotizacion;
        }



        public void updateFechaInicioVigenciaPrecios()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            if (this.Request.Params["fechaInicioVigenciaPrecios"] == null || this.Request.Params["fechaInicioVigenciaPrecios"].Equals(""))
                cotizacion.fechaInicioVigenciaPrecios = null;
            else
            { 
                String[] fecha = this.Request.Params["fechaInicioVigenciaPrecios"].Split('/');
                cotizacion.fechaInicioVigenciaPrecios = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            }
            this.Session["cotizacion"] = cotizacion;
        }

        public void updateFechaFinVigenciaPrecios()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            if (this.Request.Params["fechaFinVigenciaPrecios"] == null || this.Request.Params["fechaFinVigenciaPrecios"].Equals(""))
                cotizacion.fechaFinVigenciaPrecios = null;
            else
            {
                String[] fecha = this.Request.Params["fechaFinVigenciaPrecios"].Split('/');
                cotizacion.fechaFinVigenciaPrecios = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            }
            this.Session["cotizacion"] = cotizacion;
        }


        public void updateFechaDesde()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacionBusqueda"];
            String[] fecha = this.Request.Params["fechaDesde"].Split('/');
            cotizacion.fechaDesde = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.Session["cotizacionBusqueda"] = cotizacion;
        }

        public void updateFechaHasta()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacionBusqueda"];
            String[] fecha = this.Request.Params["fechaHasta"].Split('/');
           
            cotizacion.fechaHasta = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]), 23, 59, 59);//.AddDays(1);
            this.Session["cotizacionBusqueda"] = cotizacion;
        }

        public void updateUsuario()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacionBusqueda"];
            cotizacion.usuarioBusqueda = new Usuario { idUsuario = Guid.Parse(this.Request.Params["usuario"]) };
            this.Session["cotizacionBusqueda"] = cotizacion;
        }

        #endregion





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

        public String updateIdCiudadBusqueda()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacionBusqueda"];
            Guid idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);

            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
            cotizacion.cliente = new Cliente();
            cotizacion.grupo = new Grupo();
            cotizacion.ciudad = ciudadNueva;
            this.Session["cotizacionBusqueda"] = cotizacion;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";

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
                    cotizacionDetalle.observacion = cotizacionDetalleJson.observacion;

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



        public String GetClientesBusqueda()
        {

            String data = this.Request.Params["data[q]"];

            ClienteBL clienteBL = new ClienteBL();
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacionBusqueda"];

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
            //Se identifica la pagina 
            Constantes.paginas pagina = (Constantes.paginas)Int32.Parse((Request["pagina"].ToString()));

            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            //Si la pagina es de busqueda se obtiene la cotización de busqueda y se trabaja con la cotización de búsqueda
            if (pagina == Constantes.paginas.misCotizaciones)
                cotizacion = (Cotizacion)this.Session["cotizacionBusqueda"];
           


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


            if (pagina == Constantes.paginas.misCotizaciones)
                this.Session["cotizacionBusqueda"] = cotizacion;
            else
                this.Session["cotizacion"] = cotizacion;


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
            List<Producto> lista = bl.getProductosBusqueda(texto_busqueda, cotizacion.considerarDescontinuados, proveedor, familia, cotizacion.cliente.idCliente, cotizacion.grupo.idGrupo);

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
            Producto producto = bl.getProducto(idProducto, cotizacion.ciudad.esProvincia , cotizacion.incluidoIgv, cotizacion.cliente.idCliente);

            Decimal fleteDetalle = Decimal.Parse(String.Format(Constantes.decimalFormat, producto.costoLista * (cotizacion.flete) / 100));
            Decimal precioUnitario = Decimal.Parse(String.Format(Constantes.decimalFormat, fleteDetalle + producto.precioLista));

            Decimal porcentajeDescuento = 0;
            if (producto.precioNeto != null)
            {

                porcentajeDescuento = 100 - (producto.precioNeto.Value * 100 / producto.precioLista);


            }

            String jsonPrecioLista = JsonConvert.SerializeObject(producto.precioListaList);



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
                "\"porcentajeDescuento\":\"" + porcentajeDescuento + "\"," +
                "\"precioListaList\":" + jsonPrecioLista + "," +

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
            Producto producto = productoBL.getProducto(idProducto, cotizacion.ciudad.esProvincia, cotizacion.incluidoIgv, cotizacion.cliente.idCliente);
            detalle.producto = producto;

            detalle.cantidad = Int32.Parse(Request["cantidad"].ToString());
            detalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
            detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;
            detalle.observacion = Request["observacion"].ToString();
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

            if (cotizacion.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Ambos )
            {
                nombreProducto = nombreProducto + "\\n" + detalle.observacion;
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
                "\"observacion\":\"" + detalle.observacion + "\", " +
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
            String ruta = AppDomain.CurrentDomain.BaseDirectory + "\\pdf\\" + fileName;
            FileStream inStream = new FileStream(ruta, FileMode.Open);
            MemoryStream storeStream = new MemoryStream();

            storeStream.SetLength(inStream.Length);
            inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);

            storeStream.Flush();
            inStream.Close();
           System.IO.File.Delete(ruta);
            //System.IO.File.Delete(fileName);

            FileStreamResult result = new FileStreamResult(storeStream, "application/pdf");
            result.FileDownloadName = fileName;     
            return result;
        }


        public String grabarCotizacion()
        {
            Cotizacion cotizacion = insertarCotizacion();

            return "{ \"codigo\":\""+cotizacion.codigo+"\", \"estado\":\""+ (int)cotizacion.seguimientoCotizacion.estado +"\" }";
        }


        public void generarPlantillaCotizacion()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            Usuario usuario = (Usuario)this.Session["usuario"];
            cotizacion.usuario = usuario;
            CotizacionBL cotizacionBL = new CotizacionBL();
            cotizacion = cotizacionBL.generarPlantillaCotizacion(cotizacion);
            calcularMontosTotales(cotizacion);
            this.Session["cotizacion"] = cotizacion;
        }


        public void editarCotizacion()
        {
            Cotizacion cotizacionSession = (Cotizacion)this.Session["cotizacion"];
            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = Int64.Parse(Request["numero"].ToString());
            cotizacion.fechaModificacion = cotizacionSession.fechaModificacion;
            cotizacion.seguimientoCotizacion = new SeguimientoCotizacion();
            cotizacion.usuario = (Usuario)this.Session["usuario"];
            //Se cambia el estado de la cotizacion a Edición
            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Edicion;
            cotizacionBL.cambiarEstadoCotizacion(cotizacion);

            //Se obtiene los datos de la cotización ya modificada
            cotizacion = cotizacionBL.GetCotizacion(cotizacion);


            this.Session["cotizacion"] = cotizacion;
        }

        public String VerCotizacion()
        {
            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = Int64.Parse(Request["numero"].ToString());
            cotizacion = cotizacionBL.GetCotizacion(cotizacion);
            this.Session["cotizacion"] = cotizacion;

            string jsonCotizacion = JsonConvert.SerializeObject(cotizacion);

            return jsonCotizacion;
        }


        public void updateEstadoSeguimientoCotizacion()
        {
            Cotizacion cotizacionSession = (Cotizacion)this.Session["cotizacion"];

            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = Int64.Parse(Request["codigo"].ToString());
            cotizacion.fechaModificacion = cotizacionSession.fechaModificacion;
            cotizacion.seguimientoCotizacion = new SeguimientoCotizacion();
            cotizacion.seguimientoCotizacion.estado = (SeguimientoCotizacion.estadosSeguimientoCotizacion)Int32.Parse(Request["estado"].ToString());
            cotizacion.seguimientoCotizacion.observacion = Request["observacion"].ToString();
            cotizacion.usuario = (Usuario)this.Session["usuario"];
            cotizacionBL.cambiarEstadoCotizacion(cotizacion);
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
         //   cotizacion.estadoAprobacion = 0;


            //REVISAR
            /*

            ViewBag.fechaLimiteValidezOferta = cotizacion.fecha.ToString("dd/MM/yyyy");
            ViewBag.fechaInicioVigenciaPrecios = cotizacion.fechaInicioVigenciaPrecios == null ? null : cotizacion.fechaInicioVigenciaPrecios.Value.ToString("dd/MM/yyyy");
            ViewBag.fechaFinVigenciaPrecios = cotizacion.fechaFinVigenciaPrecios == null ? null : cotizacion.fechaFinVigenciaPrecios.Value.ToString("dd/MM/yyyy");




            cotizacion.fechaInicioVigenciaPrecios = null;
            cotizacion.fechaFinVigenciaPrecios = null;
            cotizacion.fechaLimiteValidezOferta = cotizacionTmp.fecha.AddDays(Constantes.plazoOfertaDias);




            cotizacion.fechaInicioVigenciaPrecios = cotizacion.fechaFinVigenciaPrecios.AddDays(1);
            cotizacion.fechaFinVigenciaPrecios = cotizacion.fechaFinVigenciaPrecios.AddDays(16);
            */
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