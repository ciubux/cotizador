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
using Newtonsoft.Json;

namespace Cotizador.Controllers
{
    public class CotizacionController : Controller
    {

        //Busqueda de Cotizacion
        public ActionResult Index()
        {
            this.Session["pagina"] = Constantes.BUSQUEDA_COTIZACION;

            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //  crearCotizacion();

            if (this.Session["cotizacionBusqueda"] == null)
            {
                Cotizacion cotizacionTmp = new Cotizacion();
                cotizacionTmp.fechaDesde = DateTime.Now.AddDays(-10);
                DateTime fechaHasta = DateTime.Now;
                cotizacionTmp.fechaHasta = new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day,23,59,59);
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


            ViewBag.fechaDesde = cotizacionSearch.fechaDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaHasta = cotizacionSearch.fechaHasta.ToString(Constantes.formatoFecha);


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






        private void instanciarCotizacion()
        {
            Cotizacion cotizacionTmp = new Cotizacion();
            cotizacionTmp.idCotizacion = Guid.Empty;
            cotizacionTmp.mostrarCodigoProveedor = true;
            cotizacionTmp.fecha = DateTime.Now;
            cotizacionTmp.fechaInicioVigenciaPrecios = null;
            cotizacionTmp.fechaFinVigenciaPrecios = null;
            cotizacionTmp.fechaLimiteValidezOferta = cotizacionTmp.fecha.AddDays(Constantes.PLAZO_OFERTA_DIAS);
            cotizacionTmp.fechaPrecios = cotizacionTmp.fecha.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);
            cotizacionTmp.ciudad = new Ciudad();
            cotizacionTmp.cliente = new Cliente();
            cotizacionTmp.grupo = new Grupo();
            cotizacionTmp.cotizacionDetalleList = new List<CotizacionDetalle>();
            cotizacionTmp.igv = Constantes.IGV;
            cotizacionTmp.flete = 0;
            cotizacionTmp.validezOfertaEnDias = Constantes.PLAZO_OFERTA_DIAS;
            cotizacionTmp.considerarCantidades = Cotizacion.OpcionesConsiderarCantidades.Observaciones;
            Usuario usuario = (Usuario)this.Session["usuario"];
            cotizacionTmp.usuario = usuario;
            cotizacionTmp.observaciones = Constantes.OBSERVACION;
            cotizacionTmp.incluidoIGV = false;
            cotizacionTmp.seguimientoCotizacion = new SeguimientoCotizacion();
            this.Session["cotizacion"] = cotizacionTmp;
        }

        public Boolean ConsultarSiExisteCotizacion()
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
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session["usuario"];
            usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("Index", "Cotizacion");
        }


        public ActionResult Cotizar()
        {
            this.Session["pagina"] = Constantes.MANTENIMIENTO_COTIZACION;

            try
            {
                //Si no hay usuario, se dirige el logueo
                if (this.Session["usuario"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                ViewBag.debug = Constantes.DEBUG;
                ViewBag.Si = Constantes.MENSAJE_SI;
                ViewBag.No = Constantes.MENSAJE_NO;
                ViewBag.IGV = Constantes.IGV;


                //Si no se está trabajando con una cotización se crea una y se agrega a la sesion

                if (this.Session["cotizacion"] == null)
                {

                    instanciarCotizacion();
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

                ViewBag.fechaPrecios = cotizacion.fechaPrecios.ToString(Constantes.formatoFecha);
                ViewBag.fecha = cotizacion.fecha.ToString(Constantes.formatoFecha);
                ViewBag.fechaLimiteValidezOferta = cotizacion.fechaLimiteValidezOferta.ToString(Constantes.formatoFecha);
                ViewBag.fechaInicioVigenciaPrecios = cotizacion.fechaInicioVigenciaPrecios == null ? null : cotizacion.fechaInicioVigenciaPrecios.Value.ToString(Constantes.formatoFecha);
                ViewBag.fechaFinVigenciaPrecios = cotizacion.fechaFinVigenciaPrecios == null ? null : cotizacion.fechaFinVigenciaPrecios.Value.ToString(Constantes.formatoFecha);

                //Se agrega el viewbag numero para poder mostrar el campo vacío cuando no se está creando una cotización
                ViewBag.numero = cotizacion.codigo;

                ViewBag.cotizacion = cotizacion;

            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session["usuario"];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }

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
                cotizacion.incluidoIGV = incluidoIGVInt == 1;
            }
    
            
            List<CotizacionDetalle> detalles = cotizacion.cotizacionDetalleList;

            foreach (CotizacionDetalle cotizacionDetalle in detalles)
            {
                //cotizacionDetalle.incluyeIGV = cotizacion.incluidoIgv;

                Decimal precioNeto = cotizacionDetalle.producto.precioSinIgv;
                Decimal costo = cotizacionDetalle.producto.costoSinIgv;

                //Se calcula el precio con Flete
             //   precioNeto = precioNeto + (precioNeto * cotizacion.flete / 100);

                if (cotizacion.incluidoIGV)
                {
                    precioNeto = precioNeto + (precioNeto * Constantes.IGV);
                    costo = costo + (costo * Constantes.IGV);
                    //Si recalcularIGV es true quiere decir que se cambio la opcion de considerar IGV
                    //El cambio afecta el precio y el costo Anterior.
                    if (recalcularIGV)
                    {
                        cotizacionDetalle.precioNetoAnterior = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.precioNetoAnterior + cotizacionDetalle.precioNetoAnterior * Constantes.IGV));
                        cotizacionDetalle.costoAnterior = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.costoAnterior + cotizacionDetalle.costoAnterior * Constantes.IGV));
                    }
                }
                else
                {
                    //Si recalcularIGV es true quiere decir que se cambio la opcion de considerar IGV
                    //El cambio afecta el precio y el costo Anterior.
                    if (recalcularIGV)
                    {
                        cotizacionDetalle.precioNetoAnterior = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.precioNetoAnterior / (1 + Constantes.IGV)));
                        cotizacionDetalle.costoAnterior = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.costoAnterior / (1 + Constantes.IGV)));
                    }
                }

                
                //El precioLista no tiene el calculo de equivalencia
                //El flete no afecta al precioLista
                //cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto));
                //Se define el costoLista del producto como el costo calculado

                //El precio y el costo se setean al final dado que si es equivalente en cada get se hará el recalculo

                cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto));
                cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costo));
                //Se aplica descuenta al precio y se formatea a dos decimales el precio para un calculo exacto en el subtotal
                cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto * (100 - cotizacionDetalle.porcentajeDescuento) / 100));
              
            }
            HelperDocumento.calcularMontosTotales(cotizacion);
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

        public void updateFechaEsModificada()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.fechaEsModificada = int.Parse(this.Request.Params["fechaEsModificada"]) == 1 ;
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





        public String ChangeIdCiudad()
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

        public String ChangeIdCiudadBusqueda()
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
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            IDocumento documento = (Cotizacion)this.Session["cotizacion"];
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            HelperDocumento.calcularMontosTotales(documento);
            this.Session["cotizacion"] = documento;
            return "{\"cantidad\":\""+ documento.documentoDetalle.Count + "\"}";
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






        public String GetProducto()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            //Se recupera el producto y se guarda en Session
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            this.Session["idProducto"] = idProducto.ToString();
    
            //Para recuperar el producto se envia si la sede seleccionada es provincia o no
            ProductoBL bl = new ProductoBL();
            Producto producto = bl.getProducto(idProducto, cotizacion.ciudad.esProvincia , cotizacion.incluidoIGV, cotizacion.cliente.idCliente);

            Decimal fleteDetalle = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.costoLista * (cotizacion.flete) / 100));
            Decimal precioUnitario = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, fleteDetalle + producto.precioLista));

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
            Producto producto = productoBL.getProducto(idProducto, cotizacion.ciudad.esProvincia, cotizacion.incluidoIGV, cotizacion.cliente.idCliente);
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
                detalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto * producto.equivalencia));
            }
            else
            {
                detalle.precioNeto = precioNeto;
            }
            detalle.flete = flete;
            cotizacion.cotizacionDetalleList.Add(detalle);

            //Calcula los montos totales de la cabecera de la cotizacion
            HelperDocumento.calcularMontosTotales(cotizacion);

           
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

     /*       if (cotizacion.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Ambos )
            {
                nombreProducto = nombreProducto + "\\n" + detalle.observacion;
            }*/

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

        


        public String Create()
        {

            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session["usuario"];
            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.usuario = usuario;
            CotizacionBL bl = new CotizacionBL();
            bl.InsertCotizacion(cotizacion);
            long codigo = cotizacion.codigo;
            int estado = (int)cotizacion.seguimientoCotizacion.estado;
            if (continuarLuego == 1)
            {
                SeguimientoCotizacion.estadosSeguimientoCotizacion estadosSeguimientoCotizacion = SeguimientoCotizacion.estadosSeguimientoCotizacion.Edicion;
                estado = (int)estadosSeguimientoCotizacion;
                String observacion = "Se continuará editando luego";
                updateEstadoSeguimientoCotizacion(codigo, estadosSeguimientoCotizacion, observacion);
            }
            cotizacion = null;
            this.Session["cotizacion"] = null;


            usuarioBL.updateCotizacionSerializada(usuario, null);
            String resultado = "{ \"codigo\":\"" + codigo + "\", \"estado\":\"" + estado + "\" }";
            return resultado;
        }


        public String Update()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session["usuario"];

            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            cotizacion.usuario = usuario;
            CotizacionBL bl = new CotizacionBL();
            bl.UpdateCotizacion(cotizacion);
            long codigo = cotizacion.codigo;
            int estado = (int)cotizacion.seguimientoCotizacion.estado; 
            if (continuarLuego == 1)
            {
                SeguimientoCotizacion.estadosSeguimientoCotizacion estadosSeguimientoCotizacion = SeguimientoCotizacion.estadosSeguimientoCotizacion.Edicion;
                estado = (int)estadosSeguimientoCotizacion;
                String observacion = "Se continuará editando luego";
                updateEstadoSeguimientoCotizacion(codigo, estadosSeguimientoCotizacion, observacion);
            }
            cotizacion = null;
            this.Session["cotizacion"] = null;


            usuarioBL.updateCotizacionSerializada(usuario, null);
            String resultado = "{ \"codigo\":\"" + codigo + "\", \"estado\":\"" + estado + "\" }";
            return resultado;
        }



        public void obtenerProductosAPartirdePreciosRegistrados()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
            Usuario usuario = (Usuario)this.Session["usuario"];
            cotizacion.usuario = usuario;
            CotizacionBL cotizacionBL = new CotizacionBL();

            String[] fechaPrecios = this.Request.Params["fecha"].Split('/');
            cotizacion.fechaPrecios = new DateTime(Int32.Parse(fechaPrecios[2]), Int32.Parse(fechaPrecios[1]), Int32.Parse(fechaPrecios[0]),0,0,0);

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

            cotizacion = cotizacionBL.obtenerProductosAPartirdePreciosRegistrados(cotizacion, familia, proveedor);
            HelperDocumento.calcularMontosTotales(cotizacion);
            this.Session["cotizacion"] = cotizacion;
        }


        public void iniciarEdicionCotizacion()
        {
            Cotizacion cotizacionVer = (Cotizacion)this.Session["cotizacionVer"];
            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = cotizacionVer.codigo;
            cotizacion.fechaModificacion = cotizacionVer.fechaModificacion;
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
            this.Session["cotizacionVer"] = cotizacion;

            Usuario usuario = (Usuario)this.Session["usuario"];
            string jsonUsuario = JsonConvert.SerializeObject(usuario);
            string jsonCotizacion = JsonConvert.SerializeObject(cotizacion);

            String json = "{\"usuario\":" + jsonUsuario + ", \"cotizacion\":" + jsonCotizacion + "}";

            return json;
        }


        public void updateEstadoCotizacion()
        {
            Int64 codigo = Int64.Parse(Request["codigo"].ToString());
            SeguimientoCotizacion.estadosSeguimientoCotizacion estadosSeguimientoCotizacion = (SeguimientoCotizacion.estadosSeguimientoCotizacion)Int32.Parse(Request["estado"].ToString());
            String observacion = Request["observacion"].ToString();
            updateEstadoSeguimientoCotizacion(codigo, estadosSeguimientoCotizacion, observacion);
        }


        private void updateEstadoSeguimientoCotizacion(Int64 codigo, SeguimientoCotizacion.estadosSeguimientoCotizacion estado, String observacion)
        {
            Cotizacion cotizacionSession = (Cotizacion)this.Session["cotizacion"];
            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = codigo;
            //REVISAR
            cotizacion.fechaModificacion = DateTime.Now;// cotizacionSession.fechaModificacion;
            cotizacion.seguimientoCotizacion = new SeguimientoCotizacion();
            cotizacion.seguimientoCotizacion.estado = estado;
            cotizacion.seguimientoCotizacion.observacion = observacion;
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
            cotizacion.usuario = (Usuario)this.Session["usuario"];
            //Se seta el codigo y estadoAprobacion en 0 porque una recotización es una nueva cotización
            cotizacion.codigo = 0;
         //   cotizacion.estadoAprobacion = 0;


            //REVISAR
            /*

            ViewBag.fechaLimiteValidezOferta = cotizacion.fecha.ToString(Constantes.formatoFecha);
            ViewBag.fechaInicioVigenciaPrecios = cotizacion.fechaInicioVigenciaPrecios == null ? null : cotizacion.fechaInicioVigenciaPrecios.Value.ToString(Constantes.formatoFecha);
            ViewBag.fechaFinVigenciaPrecios = cotizacion.fechaFinVigenciaPrecios == null ? null : cotizacion.fechaFinVigenciaPrecios.Value.ToString(Constantes.formatoFecha);




            cotizacion.fechaInicioVigenciaPrecios = null;
            cotizacion.fechaFinVigenciaPrecios = null;
            cotizacion.fechaLimiteValidezOferta = cotizacionTmp.fecha.AddDays(Constantes.PLAZO_OFERTA_DIAS);




            cotizacion.fechaInicioVigenciaPrecios = cotizacion.fechaFinVigenciaPrecios.AddDays(1);
            cotizacion.fechaFinVigenciaPrecios = cotizacion.fechaFinVigenciaPrecios.AddDays(16);
            */
            this.Session["cotizacion"] = cotizacion;
        }


        





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
       




        public void autoGuardarCotizacion()
        {
            if (this.Session["cotizacion"] != null)
            {
                Cotizacion cotizacion = (Cotizacion)this.Session["cotizacion"];
                UsuarioBL usuarioBL = new UsuarioBL();
                Usuario usuario = (Usuario)this.Session["usuario"];


                String cotizacionSerializada = JsonConvert.SerializeObject(cotizacion);


                usuarioBL.updateCotizacionSerializada(usuario, cotizacionSerializada);
            }

        }


    }
}