using BusinessLayer;
using Model;
using cotizadorPDF;
using Cotizador.ExcelExport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using System.IO;
using NPOI.SS.UserModel;
using Newtonsoft.Json;
using NPOI.HSSF.Model;
using System.Reflection;
using Cotizador.Models.DTOsSearch;
using Cotizador.Models.DTOsShow;
using NLog;

namespace Cotizador.Controllers
{
    public class CotizacionController : ParentController
    {

        private Cotizacion CotizacionSession
        {
            get
            {

                Cotizacion cotizacion = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaCotizaciones: cotizacion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoCotizacion: cotizacion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION]; break;
                    case Constantes.paginas.MantenimientoCotizacionGrupal: cotizacion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION_GRUPAL]; break;
                }
                return cotizacion;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaCotizaciones: this.Session[Constantes.VAR_SESSION_COTIZACION_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoCotizacion: this.Session[Constantes.VAR_SESSION_COTIZACION] = value; break;
                    case Constantes.paginas.MantenimientoCotizacionGrupal: this.Session[Constantes.VAR_SESSION_COTIZACION_GRUPAL] = value; break;
                }
            }
        }


        public void instanciarCotizacionBusqueda()
        {
            Cotizacion cotizacionTmp = new Cotizacion();
            cotizacionTmp.esPagoContado = false;
            cotizacionTmp.tipoCotizacion = Cotizacion.TiposCotizacion.Normal;
            cotizacionTmp.fechaDesde = DateTime.Now.AddDays(-Constantes.DIAS_DESDE_BUSQUEDA);
            DateTime fechaHasta = DateTime.Now;
            cotizacionTmp.fechaHasta = new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);
            cotizacionTmp.ciudad = new Ciudad();
            cotizacionTmp.cliente = new Cliente();
            cotizacionTmp.grupo = new GrupoCliente();
            cotizacionTmp.seguimientoCotizacion = new SeguimientoCotizacion();
            cotizacionTmp.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Todos;
            cotizacionTmp.buscarSedesGrupoCliente = false;
            // cotizacionTmp.cotizacionDetalleList = new List<CotizacionDetalle>();
            cotizacionTmp.usuario = (Usuario)this.Session["usuario"];
            cotizacionTmp.usuarioBusqueda = new Usuario { idUsuario = Guid.Empty };
            cotizacionTmp.aplicaSedes = false;
            this.CotizacionSession = cotizacionTmp;
            this.Session[Constantes.VAR_SESSION_COTIZACION_LISTA] = new List<Cotizacion>();
        }


        //Busqueda de Cotizacion
        public ActionResult Index()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaCotizaciones;
            return RedirectToAction("Index", "Pedido");
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                if (!usuario.creaCotizaciones && !usuario.visualizaCotizaciones)
                {
                    return RedirectToAction("Login", "Account");
                }
            }
            

            //  crearCotizacion();

            if (this.CotizacionSession == null)
            {
                instanciarCotizacionBusqueda();
            }

            Cotizacion cotizacionSearch = this.CotizacionSession;

            //Si existe cotizacion se debe verificar si no existe cliente
            if (this.Session[Constantes.VAR_SESSION_COTIZACION] != null)
            {
                Cotizacion cotizacionEdicion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION];
                if (cotizacionEdicion.ciudad == null || cotizacionEdicion.ciudad.idCiudad == null
                    || cotizacionEdicion.ciudad.idCiudad == Guid.Empty)
                {
                    this.Session[Constantes.VAR_SESSION_COTIZACION] = null;
                }

            }

            ViewBag.fechaDesde = cotizacionSearch.fechaDesde.ToString(Constantes.formatoFecha);
            ViewBag.fechaHasta = cotizacionSearch.fechaHasta.ToString(Constantes.formatoFecha);


            if (this.Session[Constantes.VAR_SESSION_COTIZACION_LISTA] == null)
            {
                this.Session[Constantes.VAR_SESSION_COTIZACION_LISTA] = new List<Cotizacion>();
            }

          
            int existeCliente = 0;
            //  if (cotizacion.cliente.idCliente != Guid.Empty || cotizacion.grupo.idGrupo != Guid.Empty)
            if(cotizacionSearch.cliente.idCliente != Guid.Empty)
            {
                existeCliente = 1;
            }

            if (cotizacionSearch.cliente.idCliente != Guid.Empty)
            {
                ViewBag.idClienteGrupo = cotizacionSearch.cliente.idCliente;
                ViewBag.clienteGrupo = cotizacionSearch.cliente.ToString();
            }

            ViewBag.idGrupoCliente = cotizacionSearch.grupo.idGrupoCliente;

            ViewBag.cotizacion = cotizacionSearch;

            ViewBag.cotizacionList =  this.Session[Constantes.VAR_SESSION_COTIZACION_LISTA];
            ViewBag.existeCliente = existeCliente;
            ViewBag.pagina = (int)Constantes.paginas.BusquedaCotizaciones;
            return View();
        }



        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<Cotizacion> list = (List<Cotizacion>)this.Session[Constantes.VAR_SESSION_COTIZACION_LISTA];

            CotizacionSearch excel = new CotizacionSearch();
            return excel.generateExcel(list);
        }

        /*Ejecución de la búsqueda de cotizaciones*/
        public String Search()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaCotizaciones;
            //Se recupera la cotizacion de la session
            Cotizacion cotizacion = this.CotizacionSession;
            CotizacionBL cotizacionBL = new CotizacionBL();
            List<Cotizacion> cotizacionList = cotizacionBL.GetCotizaciones(cotizacion);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_COTIZACION_LISTA] = cotizacionList;
            //Se retorna la cantidad de elementos encontrados

            String cotizacionListJson = JsonConvert.SerializeObject(ParserDTOsSearch.CotizacionToCotizacionDTO(cotizacionList));
            //String cotizacionListJson = JsonConvert.SerializeObject(cotizacionList);
            return cotizacionListJson;
            
        }


        public void CleanBusquedaCotizaciones()
        {
            instanciarCotizacionBusqueda();
            //Se retorna la cantidad de elementos encontrados
            //List<Cotizacion> cotizacionList = (List<Cotizacion>)this.Session[Constantes.VAR_SESSION_COTIZACION_LISTA];
            //return cotizacionList.Count();
        }



        public void instanciarCotizacion()
        {
            Cotizacion cotizacionTmp = new Cotizacion();
            cotizacionTmp.tipoCotizacion = Cotizacion.TiposCotizacion.Normal;
            cotizacionTmp.idCotizacion = Guid.Empty;
            cotizacionTmp.mostrarCodigoProveedor = true;
            cotizacionTmp.fecha = DateTime.Now;
            cotizacionTmp.fechaInicioVigenciaPrecios = null;
            cotizacionTmp.fechaFinVigenciaPrecios = null;
            cotizacionTmp.fechaLimiteValidezOferta = cotizacionTmp.fecha.AddDays(Constantes.PLAZO_OFERTA_DIAS);
            cotizacionTmp.fechaPrecios = cotizacionTmp.fecha.AddDays(Constantes.DIAS_MAX_BUSQUEDA_PRECIOS * -1);
            cotizacionTmp.ciudad = new Ciudad();
            cotizacionTmp.cliente = new Cliente();
            cotizacionTmp.grupo = new GrupoCliente();
            cotizacionTmp.cotizacionDetalleList = new List<CotizacionDetalle>();
            cotizacionTmp.igv = Constantes.IGV;
            cotizacionTmp.flete = 0;
            cotizacionTmp.validezOfertaEnDias = Constantes.PLAZO_OFERTA_DIAS;
            cotizacionTmp.considerarCantidades = Cotizacion.OpcionesConsiderarCantidades.Observaciones;
            Usuario usuario = (Usuario)this.Session["usuario"];
            cotizacionTmp.usuario = usuario;
            cotizacionTmp.observaciones = Constantes.OBSERVACION;
            cotizacionTmp.incluidoIGV = false;
            cotizacionTmp.aplicaSedes = false;
            cotizacionTmp.seguimientoCotizacion = new SeguimientoCotizacion();
            this.CotizacionSession = cotizacionTmp;
        }
        



        public String ConsultarSiExisteCotizacion()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION];
            if (cotizacion == null)
                return "{\"existe\":\"false\",\"numero\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"numero\":\""+ cotizacion.codigo+ "\"}";
        }

        public String ConsultarSiExisteCotizacionGrupal()
        {
            Cotizacion cotizacion = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION_GRUPAL];
            if (cotizacion == null)
                return "{\"existe\":\"false\",\"numero\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"numero\":\"" + cotizacion.codigo + "\"}";
        }

        public ActionResult CancelarCreacionCotizacion()
        {
            this.CotizacionSession = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("Index", "Cotizacion");
        }


        public ActionResult Cotizar()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoCotizacion;

            try
            {
                //Si no hay usuario, se dirige el logueo
                if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                    if (!usuario.creaCotizaciones)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                ViewBag.debug = Constantes.DEBUG;
                ViewBag.Si = Constantes.MENSAJE_SI;
                ViewBag.No = Constantes.MENSAJE_NO;
                ViewBag.IGV = Constantes.IGV;


                //Si no se está trabajando con una cotización se crea una y se agrega a la sesion

                if (this.CotizacionSession == null)
                {

                    instanciarCotizacion();
                }
                Cotizacion cotizacion = this.CotizacionSession;


                int existeCliente = 0;
                if (cotizacion.cliente.idCliente != Guid.Empty)// || cotizacion.grupo.idGrupoCliente != Guid.Empty)
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
                    ViewBag.idClienteGrupo = cotizacion.grupo.idGrupoCliente;
                    ViewBag.clienteGrupo = cotizacion.grupo.ToString();
                }

                ViewBag.fechaPrecios =DateTime.Now.AddDays(-Constantes.DIAS_MAX_BUSQUEDA_PRECIOS).ToString(Constantes.formatoFecha);
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
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }
            ViewBag.pagina = (int)Constantes.paginas.MantenimientoCotizacion;
            return View();
        }



        public ActionResult CotizarGrupo()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoCotizacionGrupal;

            try
            {
                //Si no hay usuario, se dirige el logueo
                if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                    if (!usuario.creaCotizacionesGrupales)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }

                ViewBag.debug = Constantes.DEBUG;
                ViewBag.Si = Constantes.MENSAJE_SI;
                ViewBag.No = Constantes.MENSAJE_NO;
                ViewBag.IGV = Constantes.IGV;


                //Si no se está trabajando con una cotización se crea una y se agrega a la sesion

                if (this.CotizacionSession == null)
                {

                    instanciarCotizacion();
                }
                Cotizacion cotizacion = this.CotizacionSession;


                int existeCliente = 0;
                if (cotizacion.cliente.idCliente != Guid.Empty)// || cotizacion.grupo.idGrupoCliente != Guid.Empty)
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
                    ViewBag.idClienteGrupo = cotizacion.grupo.idGrupoCliente;
                    ViewBag.clienteGrupo = cotizacion.grupo.ToString();
                }

                ViewBag.fechaPrecios = DateTime.Now.AddDays(-Constantes.DIAS_MAX_BUSQUEDA_PRECIOS).ToString(Constantes.formatoFecha);
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
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }
            ViewBag.pagina = (int)Constantes.paginas.MantenimientoCotizacionGrupal;
            return View();
        }


        public int updateSeleccionIGV()
        {
            int incluidoIGV = Int32.Parse(this.Request.Params["igv"]);
            return actualizarCotizacionDetalles(incluidoIGV, true);
        }

        
        public int updateSeleccionConsiderarCantidades()
        {
            Cotizacion cotizacion = this.CotizacionSession;
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
            HelperDocumento.calcularMontosTotales(cotizacion);

            this.CotizacionSession = cotizacion;

            return 1;
        }


        private int actualizarCotizacionDetalles(int incluidoIGVInt, bool recalcularIGV = false)//, int considerarCantidadesInt)
        {
            Cotizacion cotizacion = this.CotizacionSession;

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
             //   precioNetoEquivalente = precioNetoEquivalente + (precioNetoEquivalente * cotizacion.flete / 100);

                if (cotizacion.incluidoIGV)
                {
                    precioNeto = precioNeto + (precioNeto * Constantes.IGV);
                    costo = costo + (costo * Constantes.IGV);
                    //Si recalcularIGV es true quiere decir que se cambio la opcion de considerar IGV
                    //El cambio afecta el precio y el costo Anterior.
                    if (recalcularIGV)
                    {
                        cotizacionDetalle.precioNetoAnterior = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.precioNetoAnterior + cotizacionDetalle.precioNetoAnterior * Constantes.IGV));
                        cotizacionDetalle.producto.costoListaAnterior = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.costoListaAnterior + cotizacionDetalle.producto.costoListaAnterior * Constantes.IGV));
                    }
                }
                else
                {
                    //Si recalcularIGV es true quiere decir que se cambio la opcion de considerar IGV
                    //El cambio afecta el precio y el costo Anterior.
                    if (recalcularIGV)
                    {
                        cotizacionDetalle.precioNetoAnterior = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.precioNetoAnterior / (1 + Constantes.IGV)));
                        cotizacionDetalle.producto.costoListaAnterior = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacionDetalle.producto.costoListaAnterior / (1 + Constantes.IGV)));
                    }
                }

                
                //El precioLista no tiene el calculo de equivalencia
                //El flete no afecta al precioLista
                //cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNetoEquivalente));
                //Se define el costoLista del producto como el costo calculado

                //El precio y el costo se setean al final dado que si es equivalente en cada get se hará el recalculo

                cotizacionDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto));
                cotizacionDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, costo));
                //Se aplica descuenta al precio y se formatea a dos decimales el precio para un calculo exacto en el subtotal
                cotizacionDetalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto * (100 - cotizacionDetalle.porcentajeDescuento) / 100));
              
            }
            HelperDocumento.calcularMontosTotales(cotizacion);
            this.CotizacionSession = cotizacion;
            return detalles.Count();
        }
          




        public void updateObservaciones()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            cotizacion.observaciones = this.Request.Params["observaciones"];
            this.CotizacionSession = cotizacion;
        }


        public void updateMostrarCosto()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            cotizacion.mostrarCosto = Boolean.Parse(this.Request.Params["mostrarCosto"]);
            this.CotizacionSession = cotizacion;
        }

        public void updateConsiderarDescontinuados()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            cotizacion.considerarDescontinuados = Boolean.Parse(this.Request.Params["considerarDescontinuados"]);
            this.CotizacionSession = cotizacion;
        }

        public String updateIdGrupoCliente()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            GrupoClienteBL grupoClienteBL = new GrupoClienteBL();
            if (this.Request.Params["idGrupoCliente"] != null && this.Request.Params["idGrupoCliente"] != "")
            {
                cotizacion.grupo = grupoClienteBL.getGrupo(int.Parse(this.Request.Params["idGrupoCliente"]));
                cotizacion.contacto = cotizacion.grupo.contacto;
                cotizacion.ciudad = cotizacion.grupo.ciudad;
            }
            else
            {
                cotizacion.grupo = new GrupoCliente();
            }
            //Se crea un grupo vacío para no considerarlo al momento de grabar la cotización
            
            String resultado = "{" +
             /*   "\"descripcionCliente\":\"" + cotizacion.cliente.ToString() + "\"," +
                "\"idGrupoCliente\":\"" + cotizacion.cliente.idCliente + "\"," +*/
                "\"contacto\":\"" + cotizacion.grupo.contacto + "\"," +
                "\"textoCondicionesPago\":\"" + cotizacion.textoCondicionesPago + "\"" +
                "}";           

            this.CotizacionSession = cotizacion;
            return resultado;
        }


        public void updateFlete()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            cotizacion.flete = Decimal.Parse(this.Request.Params["flete"]);
            //actualizarCotizacionDetalles(cotizacion.incluidoIgv?1:0);
            this.CotizacionSession = cotizacion;
        }

        public void updateCodigoCotizacionBusqueda()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            try
            {
                cotizacion.codigo = int.Parse(this.Request.Params["codigo"]);
            }
            catch (Exception ex)
            {
                cotizacion.codigo = 0;
            }
            this.CotizacionSession = cotizacion;
        }


        public void updateEstadoCotizacionBusqueda()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            try
            {
                cotizacion.seguimientoCotizacion.estado = (SeguimientoCotizacion.estadosSeguimientoCotizacion)int.Parse(this.Request.Params["estado"]);
            }
            catch (Exception ex)
            {
            }
            this.CotizacionSession = cotizacion;
        }

        public String updateEsPagoContado()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            try
            {
                cotizacion.esPagoContado = Int32.Parse(this.Request.Params["esPagoContado"]) == 1; 
            }
            catch (Exception ex)
            {
            }
            this.CotizacionSession = cotizacion;

            return "{\"textoCondicionesPago\":\"" + cotizacion.textoCondicionesPago + "\"}";
        }

        public void updateMostrarCodigoProveedor()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            cotizacion.mostrarCodigoProveedor = Int32.Parse(this.Request.Params["mostrarcodproveedor"])==1;
            this.CotizacionSession = cotizacion;
        }

        public void updateContacto()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            cotizacion.contacto = this.Request.Params["contacto"];
            this.CotizacionSession = cotizacion;
        }


        #region CONTROL DE FECHAS

        public void updateFecha()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            String[] fecha = this.Request.Params["fecha"].Split('/');
            cotizacion.fecha = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.CotizacionSession = cotizacion;
        }

        public void updateFechaEsModificada()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            cotizacion.fechaEsModificada = int.Parse(this.Request.Params["fechaEsModificada"]) == 1 ;
            this.CotizacionSession = cotizacion;
        }

        public void updateMostrarValidezOfertaEnDias()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            cotizacion.mostrarValidezOfertaEnDias = int.Parse(this.Request.Params["mostrarValidezOfertaEnDias"]);
            this.CotizacionSession = cotizacion;
        }


        public void updateValidezOfertaEnDias()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            cotizacion.validezOfertaEnDias = int.Parse(this.Request.Params["validezOfertaEnDias"]);
            this.CotizacionSession = cotizacion;
        }


        public void updateFechaLimiteValidezOferta()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            String[] fecha = this.Request.Params["fechaLimiteValidezOferta"].Split('/');
            cotizacion.fechaLimiteValidezOferta = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.CotizacionSession = cotizacion;
        }



        public void updateFechaInicioVigenciaPrecios()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            if (this.Request.Params["fechaInicioVigenciaPrecios"] == null || this.Request.Params["fechaInicioVigenciaPrecios"].Equals(""))
                cotizacion.fechaInicioVigenciaPrecios = null;
            else
            { 
                String[] fecha = this.Request.Params["fechaInicioVigenciaPrecios"].Split('/');
                cotizacion.fechaInicioVigenciaPrecios = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            }
            this.CotizacionSession = cotizacion;
        }

        public void updateFechaFinVigenciaPrecios()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            if (this.Request.Params["fechaFinVigenciaPrecios"] == null || this.Request.Params["fechaFinVigenciaPrecios"].Equals(""))
                cotizacion.fechaFinVigenciaPrecios = null;
            else
            {
                String[] fecha = this.Request.Params["fechaFinVigenciaPrecios"].Split('/');
                cotizacion.fechaFinVigenciaPrecios = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            }
            this.CotizacionSession = cotizacion;
        }


        public void updateFechaDesde()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            String[] fecha = this.Request.Params["fechaDesde"].Split('/');
            cotizacion.fechaDesde = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.CotizacionSession = cotizacion;
        }

        public void updateFechaHasta()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            String[] fecha = this.Request.Params["fechaHasta"].Split('/');
           
            cotizacion.fechaHasta = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]), 23, 59, 59);//.AddDays(1);
            this.CotizacionSession = cotizacion;
        }

        public void updateUsuario()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            cotizacion.usuarioBusqueda = new Usuario { idUsuario = Guid.Parse(this.Request.Params["usuario"]) };
            this.CotizacionSession = cotizacion;
        }

        public void updateBuscarSedesGrupoCliente()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            if (this.Request.Params["buscarSedesGrupoCliente"] != null && Int32.Parse(this.Request.Params["buscarSedesGrupoCliente"]) == 1)
            {
                cotizacion.buscarSedesGrupoCliente = true;
            }
            else
            {
                cotizacion.buscarSedesGrupoCliente = false;
            }
            this.CotizacionSession = cotizacion;
        }

        public void updateBuscarSoloCotizacionesGrupales()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            if (this.Request.Params["buscarSoloCotizacionesGrupales"] != null && Int32.Parse(this.Request.Params["buscarSoloCotizacionesGrupales"]) == 1)
            {
                cotizacion.buscarSoloCotizacionesGrupales = true;
            }
            else
            {
                cotizacion.buscarSoloCotizacionesGrupales = false;
            }
            this.CotizacionSession = cotizacion;
        }

        #endregion





        public String ChangeIdCiudad()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            cotizacion.cliente = new Cliente();
           // cotizacion.grupo = new GrupoCliente();
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            { 
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);     
            }
            //Para realizar el cambio de ciudad ningun producto debe estar agregado
            if (cotizacion.cotizacionDetalleList != null && cotizacion.cotizacionDetalleList.Count > 0)
            {
                // throw new Exception("No se puede cambiar de ciudad");
                return "No se puede cambiar de ciudad";
            }
            else
            {
                CiudadBL ciudadBL = new CiudadBL();
                Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
                cotizacion.ciudad = ciudadNueva;
                this.CotizacionSession = cotizacion;
                return "{\"idCiudad\": \"" + idCiudad + "\"}";
            }
            
        }



        public void changeTipoCotizacion()
        {

            this.CotizacionSession.tipoCotizacion = (Cotizacion.TiposCotizacion)int.Parse(Request["tipoCotizacion"].ToString());
        }

        [HttpPost]
        public String ChangeDetalle(List<DocumentoDetalleJson> cotizacionDetalleJsonList)
        {
            IDocumento documento = this.CotizacionSession;
            List<DocumentoDetalle> documentoDetalle = HelperDocumento.updateDocumentoDetalle(documento, cotizacionDetalleJsonList);
            documento.documentoDetalle = documentoDetalle;
            HelperDocumento.calcularMontosTotales(documento);
            this.CotizacionSession = (Cotizacion)documento;
            return "{\"cantidad\":\""+ documento.documentoDetalle.Count + "\"}";
        }
        

        public String SearchClientes()
        {

            String data = this.Request.Params["data[q]"];

            ClienteBL clienteBL = new ClienteBL();
            Cotizacion cotizacion = this.CotizacionSession;

            List<Cliente> clienteList = clienteBL.getCLientesBusquedaCotizacion(data, cotizacion.ciudad.idCiudad);

            GrupoClienteBL grupoBL = new GrupoClienteBL();
            List<GrupoCliente> grupoList = grupoBL.getGruposBusqueda(data);

            String resultado = "{\"q\":\"" + data + "\",\"results\":[";
            Boolean existeClienteGrupo = false;
            foreach (Cliente cliente in clienteList)
            {
                resultado += "{\"id\":\"c" + cliente.idCliente + "\",\"text\":\"" + cliente.ToString() + "\"},";
                existeClienteGrupo = true;
            }
            foreach (GrupoCliente grupo in grupoList)
            {
                resultado += "{\"id\":\"g" + grupo.idGrupoCliente + "\",\"text\":\"" + grupo.ToString() + "\"},";
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
            Cotizacion cotizacion = this.CotizacionSession;
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            cotizacion.cliente = clienteBl.getCliente(idCliente);            
            cotizacion.contacto = cotizacion.cliente.contacto1;

            //Se crea un grupo vacío para no considerarlo al momento de grabar la cotización
            cotizacion.grupo = new GrupoCliente();

            String resultado = "{" +
                "\"descripcionCliente\":\"" + cotizacion.cliente.ToString() + "\"," +
                "\"idCliente\":\"" + cotizacion.cliente.idCliente + "\"," +
                "\"contacto\":\"" + cotizacion.cliente.contacto1 + "\"," +
                "\"sedePrincipal\":\"" + (cotizacion.cliente.sedePrincipal ? "1" : "0") + "\"," +
                "\"sedesString\":\"" + cotizacion.cliente.sedeListWebString + "\"," +
                "\"textoCondicionesPago\":\"" + cotizacion.textoCondicionesPago + "\"" +
                "}";

            this.CotizacionSession = cotizacion;
            return resultado;
        }

       /* public String GetGrupo()
        {
            Cotizacion cotizacion = this.CotizacionSession;


            Guid idGrupo = Guid.Parse(Request["idGrupo"].ToString());
            GrupoClienteBL grupoBl = new GrupoClienteBL();
            cotizacion.grupo = grupoBl.getGrupo(idGrupo);            
         //   cotizacion.contacto = cotizacion.grupo.contacto;

            //Se crea un cliente vacío para no considerarlo al momento de grabar la cotización
            cotizacion.cliente = new Cliente();

            String resultado = "{" +
                "\"descripcionGrupo\":\"" + cotizacion.grupo.ToString() + "\"," +
                "\"idGrupo\":\"" + cotizacion.grupo.idGrupoCliente + "\"," +
                //"\"contacto\":\"" + cotizacion.grupo.contacto + "\"" +
                "}";
            return resultado;
        }*/






        public String GetProducto()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            //Se recupera el producto y se guarda en Session
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            this.Session["idProducto"] = idProducto.ToString();
    
            //Para recuperar el producto se envia si la sede seleccionada es provincia o no
            ProductoBL bl = new ProductoBL();
            Producto producto = bl.getProducto(idProducto, cotizacion.ciudad.esProvincia , cotizacion.incluidoIGV, cotizacion.cliente.idCliente);

            //Se calcula el flete
            Decimal fleteDetalle = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.costoLista * (cotizacion.flete) / 100));
            //Se calcula el precio Unitario
            Decimal precioUnitario = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, fleteDetalle + producto.precioLista));


            //Se calcula el porcentaje de descuento
            Decimal porcentajeDescuento = 0;
            if (producto.precioClienteProducto.idPrecioClienteProducto != Guid.Empty)
            {
                //Solo en caso de que el precioNetoEquivalente sea distinto a 0 se calcula el porcentaje de descuento
                //si no se obtiene precioNetoEquivalente quiere decir que no hay precioRegistrado
                porcentajeDescuento = 100 - (producto.precioClienteProducto.precioNeto * 100 / producto.precioLista);
                fleteDetalle = producto.precioClienteProducto.flete;
            }

            String jsonPrecioLista = JsonConvert.SerializeObject(producto.precioListaList);
            String jsonProductoPresentacion = JsonConvert.SerializeObject(producto.ProductoPresentacionList);

            String resultado = "{" +
                "\"id\":\"" + producto.idProducto + "\"," +
                "\"nombre\":\"" + producto.descripcion + "\"," +
                "\"image\":\"data:image/png;base64, " + Convert.ToBase64String(producto.image) + "\"," +
                "\"unidad\":\"" + producto.unidad + "\"," +
                "\"unidad_alternativa\":\"" + producto.unidad_alternativa + "\"," +
                "\"proveedor\":\"" + producto.proveedor + "\"," +
                "\"familia\":\"" + producto.familia + "\"," +
                "\"precioUnitarioSinIGV\":\"" + producto.precioSinIgv + "\"," +
             //   "\"precioUnitarioAlternativoSinIGV\":\"" + producto.precioAlternativoSinIgv + "\"," +
                "\"precioLista\":\"" + producto.precioLista + "\"," +
                "\"costoSinIGV\":\"" + producto.costoSinIgv + "\"," +
            //    "\"costoAlternativoSinIGV\":\"" + producto.costoAlternativoSinIgv + "\"," +
                "\"fleteDetalle\":\"" + fleteDetalle + "\"," +
                "\"precioUnitario\":\"" + precioUnitario + "\"," +
                "\"porcentajeDescuento\":\"" + porcentajeDescuento + "\"," +
                "\"precioListaList\":" + jsonPrecioLista + "," +
                "\"productoPresentacionList\":" + jsonProductoPresentacion + "," +
                "\"costoLista\":\"" + producto.costoLista + "\"" +
                "}";





            return resultado;
        }






        public String AddProducto()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            Guid idProducto = Guid.Parse(this.Session["idProducto"].ToString());
            CotizacionDetalle cotizacionDetalle = cotizacion.cotizacionDetalleList.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
            if (cotizacionDetalle != null)
            {
                throw new System.Exception("Producto ya se encuentra en la lista");
            }
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            CotizacionDetalle detalle = new CotizacionDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
            ProductoBL productoBL = new ProductoBL();
            Producto producto = productoBL.getProducto(idProducto, cotizacion.ciudad.esProvincia, cotizacion.incluidoIGV, cotizacion.cliente.idCliente);
            detalle.producto = producto;

            detalle.cantidad = Int32.Parse(Request["cantidad"].ToString());
            detalle.porcentajeDescuento = Decimal.Parse(Request["porcentajeDescuento"].ToString());
            detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;
            int idProductoPresentacion = Int16.Parse(Request["idProductoPresentacion"].ToString());


            detalle.observacion = Request["observacion"].ToString();
            decimal precioNeto = Decimal.Parse(Request["precio"].ToString());
            decimal costo = Decimal.Parse(Request["costo"].ToString());
            decimal flete = Decimal.Parse(Request["flete"].ToString());

            decimal precioNetoAnterior = 0;
            detalle.unidad = detalle.producto.unidad;
            //si esPrecioAlternativo  se mostrará la unidad alternativa

            if (detalle.esPrecioAlternativo)
            {
                //Si es el precio Alternativo se multiplica por la equivalencia para que se registre el precio estandar
                //dado que cuando se hace get al precioNetoEquivalente se recupera diviendo entre la equivalencia
                detalle.ProductoPresentacion = producto.getProductoPresentacion(idProductoPresentacion);


                detalle.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, precioNeto * detalle.ProductoPresentacion.Equivalencia));


                precioNetoAnterior = detalle.producto.precioClienteProducto.precioNetoAlternativo;




                ProductoPresentacion productoPresentacion = producto.getProductoPresentacion(idProductoPresentacion);


                detalle.unidad = productoPresentacion.Presentacion;
            }
            else
            {
                detalle.precioNeto = precioNeto;
                precioNetoAnterior = detalle.producto.precioClienteProducto.precioNeto;
            }


            detalle.flete = flete;
            detalle.precioNetoAnterior = precioNetoAnterior;
            cotizacion.cotizacionDetalleList.Add(detalle);

            //Calcula los montos totales de la cabecera de la cotizacion
            HelperDocumento.calcularMontosTotales(cotizacion);

           


            var nombreProducto = detalle.producto.descripcion;
            if (cotizacion.mostrarCodigoProveedor)
            {
                nombreProducto = detalle.producto.skuProveedor + " - " + detalle.producto.descripcion;
            }

            /*       if (cotizacion.considerarCantidades == Cotizacion.OpcionesConsiderarCantidades.Ambos )
                   {
                       nombreProducto = nombreProducto + "\\n" + detalle.observacion;
                   }*/
            /*
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
            */

            var v = new
            {
                idProducto = detalle.producto.idProducto,
                codigoProducto = detalle.producto.sku,
                nombreProducto = nombreProducto,
                unidad = detalle.unidad,
                igv = cotizacion.montoIGV.ToString(),
                subTotal = cotizacion.montoSubTotal.ToString(),
                margen = detalle.margen,
                precioUnitario = detalle.precioUnitario,
                precioNetoAnt = precioNetoAnterior,
                observacion = detalle.observacion,
                total = cotizacion.montoTotal.ToString(),
                varprecioNetoAnterior = detalle.variacionPrecioAnterior,
                variacionPrecioListaAnterior = detalle.variacionPrecioListaAnterior,
                variacionCosto = detalle.variacionCosto,
                costoListaAnterior = detalle.costoListaAnterior
            };




            this.CotizacionSession = cotizacion ;

            String resultado = JsonConvert.SerializeObject(v);
            return resultado;
            
            
        }

        public String DelProducto()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            List<CotizacionDetalle> detalles = cotizacion.cotizacionDetalleList;
            Guid idProducto = Guid.Parse(this.Request.Params["idProducto"]);
            CotizacionDetalle cotizacionDetalle = detalles.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
            if(cotizacionDetalle != null)
            { 
                detalles.Remove(cotizacionDetalle);
                this.Session["detalles"] = detalles;
            }
            this.CotizacionSession = cotizacion;
            return detalles.AsEnumerable().Sum(o => o.subTotal).ToString();
        }

        


        public String Create()
        {
            try
            {
                UsuarioBL usuarioBL = new UsuarioBL();
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
                bool aplicaSedes = int.Parse(Request["aplicaSedes"].ToString()) == 1 ? true : false;

                Cotizacion cotizacion = this.CotizacionSession;
                cotizacion.aplicaSedes = aplicaSedes && !cotizacion.cliente.sedePrincipal ? false : aplicaSedes;
                cotizacion.usuario = usuario;
                CotizacionBL bl = new CotizacionBL();

                if (cotizacion.idCotizacion != Guid.Empty || cotizacion.codigo > 0)
                {
                    throw new System.Exception("Cotización ya se encuentra creada");
                }

                bl.InsertCotizacion(cotizacion);
                long codigo = cotizacion.codigo;
                int estado = (int)cotizacion.seguimientoCotizacion.estado;
                String observacion = cotizacion.seguimientoCotizacion.observacion;

                if (continuarLuego == 1)
                {
                    SeguimientoCotizacion.estadosSeguimientoCotizacion estadosSeguimientoCotizacion = SeguimientoCotizacion.estadosSeguimientoCotizacion.Edicion;
                    estado = (int)estadosSeguimientoCotizacion;
                    observacion = "Se continuará editando luego";
                    updateEstadoSeguimientoCotizacion(codigo, estadosSeguimientoCotizacion, observacion);
                }
                else
                {
                    if (cotizacion.seguimientoCotizacion.estado == SeguimientoCotizacion.estadosSeguimientoCotizacion.Edicion)
                    {
                        estado = (int)SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                    }

                }
                // cotizacion = null;
                // this.CotizacionSession = null;
                //usuarioBL.updateCotizacionSerializada(usuario, null);
                var v = new { codigo = codigo, estado = estado, observacion = observacion };
                String resultado = JsonConvert.SerializeObject(v);
                return resultado;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }

        }







        public String Update()
        {
            try { 
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            int continuarLuego = int.Parse(Request["continuarLuego"].ToString());
            bool aplicaSedes = int.Parse(Request["aplicaSedes"].ToString()) == 1 ? true : false;
            Cotizacion cotizacion = this.CotizacionSession;
            cotizacion.aplicaSedes = aplicaSedes && !cotizacion.cliente.sedePrincipal ? false : aplicaSedes;
            cotizacion.usuario = usuario;
            CotizacionBL bl = new CotizacionBL();

            Cotizacion cotizacionAprobada = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION_APROBADA];
            bl.UpdateCotizacion(cotizacion, cotizacionAprobada);
            long codigo = cotizacion.codigo;
            int estado = (int)cotizacion.seguimientoCotizacion.estado; 
            String observacion = cotizacion.seguimientoCotizacion.observacion;
            if (continuarLuego == 1)
            {
                SeguimientoCotizacion.estadosSeguimientoCotizacion estadosSeguimientoCotizacion = SeguimientoCotizacion.estadosSeguimientoCotizacion.Edicion;
                estado = (int)estadosSeguimientoCotizacion;
                observacion = "Se continuará editando luego";
                updateEstadoSeguimientoCotizacion(codigo, estadosSeguimientoCotizacion, observacion);
            }
            else
            {
                if (cotizacion.seguimientoCotizacion.estado == SeguimientoCotizacion.estadosSeguimientoCotizacion.Edicion)
                {
                    estado = (int)SeguimientoCotizacion.estadosSeguimientoCotizacion.Pendiente;
                }
            }

            //cotizacion = null;
            //this.CotizacionSession = null;
            //usuarioBL.updateCotizacionSerializada(usuario, null);

            var v = new { codigo = codigo, estado = estado, observacion = observacion };
            String resultado = JsonConvert.SerializeObject(v);
            return resultado;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }



        public void obtenerProductosAPartirdePreciosRegistrados()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
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

            cotizacion = cotizacionBL.obtenerProductosAPartirdePreciosRegistrados(cotizacion, familia, proveedor, usuario);
            HelperDocumento.calcularMontosTotales(cotizacion);
            this.CotizacionSession = cotizacion;
        }


        public void obtenerProductosAPartirdePreciosRegistradosParGrupo()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            cotizacion.usuario = usuario;
            CotizacionBL cotizacionBL = new CotizacionBL();

            String[] fechaPrecios = this.Request.Params["fecha"].Split('/');
            cotizacion.fechaPrecios = new DateTime(Int32.Parse(fechaPrecios[2]), Int32.Parse(fechaPrecios[1]), Int32.Parse(fechaPrecios[0]), 0, 0, 0);

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

            cotizacion = cotizacionBL.obtenerProductosAPartirdePreciosRegistrados (cotizacion, familia, proveedor, usuario);
            HelperDocumento.calcularMontosTotales(cotizacion);
            this.CotizacionSession = cotizacion;
        }

        public void iniciarEdicionCotizacion()
        {
            Cotizacion cotizacion = iniciarEdicionCotizacionGeneral();
            this.Session[Constantes.VAR_SESSION_COTIZACION] = cotizacion;
        }


        public void iniciarEdicionCotizacionGrupal()
        {
            Cotizacion cotizacion = iniciarEdicionCotizacionGeneral();
            this.Session[Constantes.VAR_SESSION_COTIZACION_GRUPAL] = cotizacion;
        }

        private Cotizacion iniciarEdicionCotizacionGeneral()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Cotizacion cotizacionVer = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION_VER];
            CotizacionBL cotizacionBL = new CotizacionBL();

            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = cotizacionVer.codigo;
            cotizacion.fechaModificacion = cotizacionVer.fechaModificacion;
            cotizacion.seguimientoCotizacion = new SeguimientoCotizacion();
            cotizacion.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            SeguimientoCotizacion.estadosSeguimientoCotizacion estadoAnteriorCotizacion = cotizacionVer.seguimientoCotizacion.estado;

            //Se cambia el estado de la cotizacion a Edición
            cotizacion.seguimientoCotizacion.estado = SeguimientoCotizacion.estadosSeguimientoCotizacion.Edicion;
            cotizacionBL.cambiarEstadoCotizacion(cotizacion);

            //Se obtiene los datos de la cotización ya modificada
            cotizacion = cotizacionBL.GetCotizacion(cotizacion, usuario, estadoAnteriorCotizacion);



            Cotizacion cotizacionAprobada = null;
            //Si la cotización se encuentra aprobada
            if (estadoAnteriorCotizacion == SeguimientoCotizacion.estadosSeguimientoCotizacion.Aprobada)
            {
                cotizacionAprobada = new Cotizacion();
                cotizacionAprobada.fecha = cotizacion.fecha;
                cotizacionAprobada.fechaEsModificada = cotizacion.fechaEsModificada;
                cotizacionAprobada.fechaInicioVigenciaPrecios = cotizacion.fechaInicioVigenciaPrecios;
                cotizacionAprobada.fechaFinVigenciaPrecios = cotizacion.fechaFinVigenciaPrecios;
            }

            this.Session[Constantes.VAR_SESSION_COTIZACION_APROBADA] = cotizacionAprobada;
            return cotizacion;
        }

        public String Show()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = Int64.Parse(Request["numero"].ToString());
            cotizacion = cotizacionBL.GetCotizacion(cotizacion, usuario);
            this.Session[Constantes.VAR_SESSION_COTIZACION_VER] = cotizacion;

            string jsonUsuario = JsonConvert.SerializeObject(usuario);

            string jsonCotizacion = JsonConvert.SerializeObject(ParserDTOsShow.CotizaciontoCotizacionDTO(cotizacion));

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
            Cotizacion cotizacionSession = this.CotizacionSession;
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













        public void recotizarCliente()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoCotizacion;
            Cotizacion cotizacion = recotizar(Int64.Parse(Request["numero"].ToString()), Boolean.Parse(this.Request.Params["mantenerPorcentajeDescuento"]));
            this.CotizacionSession = cotizacion;
        }

        public void recotizarGrupo()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.MantenimientoCotizacionGrupal;
            Cotizacion cotizacion = recotizar(Int64.Parse(Request["numero"].ToString()), Boolean.Parse(this.Request.Params["mantenerPorcentajeDescuento"]));
            this.CotizacionSession = cotizacion;
        }

        private Cotizacion recotizar(Int64 numeroCotizacion,Boolean mantenerPorcentajeDescuento)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = numeroCotizacion;

            

            cotizacion.esRecotizacion = true;

            cotizacion = cotizacionBL.GetReCotizacion(cotizacion, usuario, mantenerPorcentajeDescuento);
            cotizacion.usuario = (Usuario)this.Session["usuario"];
            //Se seta el codigo y estadoAprobacion en 0 porque una recotización es una nueva cotización
            cotizacion.codigo = 0;
            cotizacion.idCotizacion = Guid.Empty;
            cotizacion.fecha = DateTime.Now;
            cotizacion.fechaInicioVigenciaPrecios = null;
            cotizacion.fechaFinVigenciaPrecios = null;
            cotizacion.fechaLimiteValidezOferta = cotizacion.fecha.AddDays(Constantes.PLAZO_OFERTA_DIAS);


            return cotizacion;

           

        }


        [HttpGet]
        public ActionResult ExportLastViewExcel()
        {
            Cotizacion obj = (Cotizacion)this.Session[Constantes.VAR_SESSION_COTIZACION_VER];

            CotizacionDetalleExcel excel = new CotizacionDetalleExcel();
            return excel.generateExcel(obj);
        }



        [HttpPost]
        public String GenerarPDFdesdeIdCotizacion()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Int64 codigo = Int64.Parse(this.Request.Params["codigo"].ToString());

            CotizacionBL cotizacionBL = new CotizacionBL();
            Cotizacion cotizacion = new Cotizacion();
            cotizacion.codigo = codigo;
            cotizacion = cotizacionBL.GetCotizacion(cotizacion, usuario);
            GeneradorPDF gen = new GeneradorPDF();
            String nombreArchivo = gen.generarPDFExtended(cotizacion);
            return nombreArchivo;
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




        public void exportarExcel()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            CotizacionBL cotizacionBL = new CotizacionBL();

            Cotizacion cotizacion = new Cotizacion();


            cotizacion.codigo = 4260;
            cotizacion.idCotizacion = Guid.Parse("D33A328D-8C5C-4F1A-BF54-39B3DBC39479");


            cotizacion  = cotizacionBL.GetCotizacion(cotizacion, usuario);





            HSSFWorkbook wb;
            HSSFSheet shCabecera;
            HSSFSheet shDetalle;
            // create xls if not exists
            //  if (!File.Exists("test.xls"))
            {
                wb = HSSFWorkbook.Create(InternalWorkbook.CreateWorkbook());
                // create sheet
                shCabecera = (HSSFSheet)wb.CreateSheet("Cabecera");
                shDetalle = (HSSFSheet)wb.CreateSheet("Detalle");
               
                    // 5 rows, 8 columns
                    for (int r = 0; r < 7; r++)
                    {
                        var row = shCabecera.CreateRow(r);
                        for (int c = 0; c < 8; c++)
                        {
                            row.CreateCell(c);
                        }
                    }
                UtilesHelper.setValorCelda(shCabecera, 1, "A", "Número:");
                UtilesHelper.setValorCelda(shCabecera, 1, "B", cotizacion.codigo);
                UtilesHelper.setValorCelda(shCabecera, 1, "C", "Ciudad:");
                UtilesHelper.setValorCelda(shCabecera, 1, "D", cotizacion.ciudad.nombre);

                UtilesHelper.setValorCelda(shCabecera, 2, "A", "Cliente:");
                UtilesHelper.setValorCelda(shCabecera, 2, "B", cotizacion.cliente.razonSocial);
                UtilesHelper.setValorCelda(shCabecera, 2, "C", "RUC:");
                UtilesHelper.setValorCelda(shCabecera, 2, "D", cotizacion.cliente.ruc);
                UtilesHelper.setValorCelda(shCabecera, 2, "E", "Código MP");
                UtilesHelper.setValorCelda(shCabecera, 2, "F", cotizacion.cliente.codigo);

                UtilesHelper.setValorCelda(shCabecera, 3, "A", "Fecha Última Edición:");
           //     UtilesHelper.setValorCelda(shCabecera, 3, "B", cotizacion.fecha);
                UtilesHelper.setValorCelda(shCabecera, 3, "C", "Validez Oferta Hasta:");
            //    UtilesHelper.setValorCelda(shCabecera, 3, "D", cotizacion.fechaLimiteValidezOferta);
                UtilesHelper.setValorCelda(shCabecera, 3, "E", "Inicio Vigencia Precios:");
                if(cotizacion.fechaInicioVigenciaPrecios == null)
                    UtilesHelper.setValorCelda(shCabecera, 3, "F","No Definida");
                else
           //         UtilesHelper.setValorCelda(shCabecera, 3, "F", cotizacion.fechaInicioVigenciaPrecios.Value);
                UtilesHelper.setValorCelda(shCabecera, 3, "G", "Fin Vigencia Precios:");
           /*     if (cotizacion.fechaFinVigenciaPrecios == null)
                    UtilesHelper.setValorCelda(shCabecera, 3, "H", "No Definida");
                else
                    UtilesHelper.setValorCelda(shCabecera, 3, "H", cotizacion.fechaFinVigenciaPrecios.Value);*/

                UtilesHelper.setValorCelda(shCabecera, 4, "A", "Estado:");
                UtilesHelper.setValorCelda(shCabecera, 4, "B", cotizacion.seguimientoCotizacion.estadoString);
                UtilesHelper.setValorCelda(shCabecera, 4, "C", "Modificada Por:");
                UtilesHelper.setValorCelda(shCabecera, 4, "D", cotizacion.seguimientoCotizacion.usuario.nombre);
                UtilesHelper.setValorCelda(shCabecera, 4, "E", "Comentario:");
                UtilesHelper.setValorCelda(shCabecera, 4, "F", cotizacion.seguimientoCotizacion.observacion);

                UtilesHelper.setValorCelda(shCabecera, 5, "A", "Condiciones de Pago:");
                UtilesHelper.setValorCelda(shCabecera, 5, "B", cotizacion.observaciones);
                UtilesHelper.setValorCelda(shCabecera, 5, "C", "Observaciones:");
                UtilesHelper.setValorCelda(shCabecera, 5, "D", cotizacion.textoCondicionesPago);

                UtilesHelper.setValorCelda(shCabecera, 6, "A", "Sub Total:");
                UtilesHelper.setValorCelda(shCabecera, 6, "B", Convert.ToDouble(cotizacion.montoTotal));
                UtilesHelper.setValorCelda(shCabecera, 6, "C", "IGV:");
                UtilesHelper.setValorCelda(shCabecera, 6, "D", Convert.ToDouble(cotizacion.montoIGV));
                UtilesHelper.setValorCelda(shCabecera, 6, "E", "Total:");
                UtilesHelper.setValorCelda(shCabecera, 6, "F", Convert.ToDouble(cotizacion.montoTotal));

                //sh.GetRow(1).GetCell(1).SetCellValue(10);


                for (int r = 0; r < 3; r++)
                {
                    var row = shCabecera.CreateRow(r);
                    for (int c = 0; c < 8; c++)
                    {
                        row.CreateCell(c);
                    }
                }



                using (var fs = new FileStream("C:\\Users\\cesar\\Documents\\MP2\\cotizador\\cotizacion.xls", FileMode.Create, FileAccess.Write))
                    {
                        wb.Write(fs);

                    }
            }

            // get sheets list from xls
            /*    using (var fs = new FileStream("test.xls", FileMode.Open, FileAccess.Read))
                {
                    wb = new HSSFWorkbook(fs);

                    for (int i = 0; i < wb.Count; i++)
                    {
                        //comboBox1.Items.Add(wb.GetSheetAt(i).SheetName);
                    }
                }
                */




        }




        public String GetHistorial()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            CotizacionBL cotizacionBL = new CotizacionBL();
            List<SeguimientoCotizacion> historial = new List<SeguimientoCotizacion>();
            Guid idCotizacion = Guid.Parse(Request["id"].ToString());
            historial = cotizacionBL.GetHistorialSeguimiento(idCotizacion);

            String json = "";

            foreach(SeguimientoCotizacion seg in historial)
            {
                string jsonItem = JsonConvert.SerializeObject(seg);
                if (json.Equals("")) {
                    json = jsonItem;
                }
                else
                {
                    json = json + "," + jsonItem;
                }
            }

            json = "{\"result\": [" + json + "]}";
            return json;
        }


        public void ChangeInputBoolean()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            PropertyInfo propertyInfo = cotizacion.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(cotizacion, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.CotizacionSession = cotizacion;
        }

        public void ChangeInputString()
        {
            Cotizacion cotizacion = this.CotizacionSession;
            PropertyInfo propertyInfo = cotizacion.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(cotizacion, this.Request.Params["valor"]);
            this.CotizacionSession = cotizacion;
        }


        private bool necesitaAjusteDecimales(Cotizacion obj)
        {
            bool ajustaPrecios = false;
            decimal precioUnProv = 0;
            decimal precioUnMP = 0;
            decimal precioUnAlt = 0;

            if (obj.ajusteCalculoPrecios)
            {
                foreach (CotizacionDetalle det in obj.cotizacionDetalleList)
                {
                    if (det.esPrecioAlternativo)
                    {
                        
                    }

                    if (det.producto.equivalenciaAlternativa > 1)
                    {
                    }

                    if (det.producto.equivalenciaProveedor > 1)
                    {

                    } 
                }
            }

            return ajustaPrecios;
        }


    }
}