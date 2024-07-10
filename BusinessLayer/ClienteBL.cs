
using DataLayer;
using System.Collections.Generic;
using System.Linq;
using System;
using Model;
using Model.ServiceSunatPadron;
using System.ServiceModel;
using System.Net;
using System.IO;
using System.Text;
using ServiceLayer;
using Model.NextSoft;
using Newtonsoft.Json;
using Framework.DAL;
using System.Data;

namespace BusinessLayer
{
    public class ClienteBL
    {


        public Cliente getDatosPadronSunat(Cliente cliente)
        {
            /*var RUC = txtRuc.Text;
            RUC = RUC.Replace(" ", "");*/
            using (IwsSunatPadronClient client = new IwsSunatPadronClient())
            {
                Model.ServiceSunatPadron.ClienteSunat clienteSunat = client.BuscarClienteSunat(cliente.ruc);

                List<string> direccion = new List<string>();

                if (clienteSunat.flagerror == false)
                {
                    cliente.razonSocialSunat = clienteSunat.razonsocial;
                    cliente.ubigeo = new Ubigeo();
                    cliente.ubigeo.Id = clienteSunat.ubigeo;
                    cliente.condicionContribuyente = clienteSunat.condicion;
                    cliente.estadoContribuyente = clienteSunat.estado;

                    if (clienteSunat.tipovia != "-")
                    {
                        direccion.Add(clienteSunat.tipovia);
                    }
                    if (clienteSunat.nombrevia != "-")
                    {
                        direccion.Add(" ");
                        direccion.Add(clienteSunat.nombrevia);
                    }

                    if (clienteSunat.numero != "-")
                    {
                        direccion.Add(" NRO. ");
                        direccion.Add(clienteSunat.numero);
                    }
                    if (clienteSunat.interior != "-")
                    {
                        direccion.Add(" INT. ");
                        direccion.Add(clienteSunat.interior);
                    }
                    if (clienteSunat.departamento != "-")
                    {
                        direccion.Add(" DPT. ");
                        direccion.Add(clienteSunat.departamento);
                    }


                    if (clienteSunat.codigozona != "-" && clienteSunat.codigozona != "----")
                    {
                        direccion.Add(" ");
                        direccion.Add(clienteSunat.codigozona);
                    }
                    if (clienteSunat.tipozona != "-")
                    {
                        direccion.Add(" ");
                        direccion.Add(clienteSunat.tipozona);
                    }

                    if (clienteSunat.manzana != "-")
                    {
                        direccion.Add(" MZ. ");
                        direccion.Add(clienteSunat.manzana);
                    }

                    if (clienteSunat.lote != "-")
                    {
                        direccion.Add(" LT. ");
                        direccion.Add(clienteSunat.lote);
                    }

                    if (clienteSunat.kilometro != "-")
                    {
                        direccion.Add(" KM.");
                        direccion.Add(clienteSunat.kilometro);
                    }


                }
                else
                {
                    throw new Exception(clienteSunat.Message);
                }

                cliente.direccionDomicilioLegalSunat = String.Join("", direccion);
            }

            UbigeoBL ubigeoBL = new UbigeoBL();
            cliente.ubigeo = ubigeoBL.getUbigeo(cliente.ubigeo.Id);

            cliente.direccionDomicilioLegalSunat = cliente.direccionDomicilioLegalSunat + " " +
                cliente.ubigeo.ToString;

            return cliente;
        }

        public String getNombreComercial(Cliente cliente)
        {
            String nombreComercial = String.Empty;


            try
            {
                string URI = @"http://personasperu.com/empresas-" + cliente.ruc + ".html";
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(URI);

                /*wc. .DownloadFile(URI, @"archivo.txt");
                FileStream stream = new FileStream(@"archivo.txt", FileMode.Open, FileAccess.Read);
                */

                StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));

                string linea;

                linea = reader.ReadLine();
                while (linea != null)
                {
                    bool ini;
                    linea = reader.ReadLine();
                    ini = linea.Contains("Nombre Comercial");


                    if (ini == true)
                    {
                        linea = reader.ReadLine().TrimEnd().TrimStart();

                        /*     if (linea.Contains("-"))
                                {

                                    break;
                                }*/



                        //  value =\"BANCO DE CREDITO DEL PERU\" />"

                        int inicio = linea.IndexOf("<td>");
                        linea = linea.Substring(inicio + 4);

                        int fin = linea.IndexOf("\t");

                        var rs2 = linea.Substring(0, fin);
                        //   var rs2 = rs1.Substring(0, 25);

                        nombreComercial = rs2.ToString();
                        break;

                    }
                }

            }
            catch (Exception ex)
            {
                nombreComercial = String.Empty;

            }
            return nombreComercial;
        }

        public String getClientesSunatBusqueda(String textoBusqueda)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                List<Model.ClienteSunat> clienteList = clienteDAL.getClientesSunatBusqueda(textoBusqueda);
                String resultado = "{\"q\":\"" + textoBusqueda + "\",\"results\":[";
                Boolean existeCliente = false;
                foreach (Model.ClienteSunat cliente in clienteList)
                {
                    var obj = new
                    {
                        id = cliente.idClienteSunat,
                        text = cliente.ToString()
                    };
                    resultado +=  JsonConvert.SerializeObject(obj) + ",";
                    existeCliente = true;
                }
                if (existeCliente)
                    resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
                else
                    resultado = resultado.Substring(0, resultado.Length) + "]}";
                return resultado;
            }
        }


        public String getCLientesBusqueda(String textoBusqueda, Guid idCiudad, Guid idUsuario, bool excluirLites = false)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                List<Cliente> clienteList = clienteDAL.getClientesBusqueda(textoBusqueda, idCiudad, idUsuario, excluirLites);
                String resultado = "{\"q\":\"" + textoBusqueda + "\",\"results\":[";
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
        }

        public String getCLientesBusqueda(String textoBusqueda, Guid idUsuario)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                List<Cliente> clienteList = clienteDAL.getClientesBusqueda(textoBusqueda, Guid.Empty, idUsuario, true);
                String resultado = "{\"q\":\"" + textoBusqueda + "\",\"results\":[";
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
        }

        public String getCLientesBusquedaRUC(String textoBusqueda, Guid idUsuario)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                List<Cliente> clienteList = clienteDAL.getClientesBusquedaRUC(textoBusqueda, idUsuario);
                String resultado = "{\"q\":\"" + textoBusqueda + "\",\"results\":[";
                Boolean existeCliente = false;
                foreach (Cliente cliente in clienteList)
                {
                    resultado += "{\"id\":\"" + cliente.ruc + "\",\"text\":\"" + cliente.ToStringRUC() + "\"},";
                    existeCliente = true;
                }
                if (existeCliente)
                    resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
                else
                    resultado = resultado.Substring(0, resultado.Length) + "]}";
                return resultado;
            }
        }

        public List<Cliente> getCLientesBusquedaCotizacion(String textoBusqueda, Guid idCiudad, Guid idUsuario, bool excluirLites = false)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                return clienteDAL.getClientesBusqueda(textoBusqueda, idCiudad, idUsuario, excluirLites);
            }
        }

        public int GetDataFacturacionEmpresaEOL(Guid idCliente)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                return clienteDAL.GetDataFacturacionEmpresaEOL(idCliente);
            }
        }
        
        public Model.ClienteSunat getClienteSunat(int idClienteSunat)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                Model.ClienteSunat clie = clienteDAL.getClienteSunat(idClienteSunat);

                return clie;
            }
        }

        public String exportarClienteEmpresa(Guid idCliente, int idEmpresaDestino, Guid idUsuario)
        {
            using (ClienteDAL dal = new ClienteDAL())
            {
                return dal.exportarClienteEmpresa(idCliente, idEmpresaDestino, idUsuario);
            }
        }

        public List<List<String>> getEmpresasExisteCliente(Guid idCliente)
        {
            using (ClienteDAL dal = new ClienteDAL())
            {
                return dal.getEmpresasExisteCliente(idCliente);
            }
        }

        public Cliente getCliente(Guid idCliente)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                Cliente clie = clienteDAL.getCliente(idCliente);

                if (clie.sedePrincipal)
                {
                    clie.sedeList = clienteDAL.getSedes(clie.ruc);
                }

                if (clie.esClienteLite && clie.tipoDocumento == null)
                {
                    clie.sedePrincipal = false;
                    clie.tipoLiberacionCrediticia = Persona.TipoLiberacionCrediticia.requiere;
                    clie.negociacionMultiregional = false;
                    clie.observacionHorarioEntrega = "";
                    clie.tipoPagoFactura = DocumentoVenta.TipoPago.Contado;
                    clie.plazoCreditoSolicitado = DocumentoVenta.TipoPago.Contado;
                    clie.FechaRegistro = DateTime.Now;
                    clie.horaInicioPrimerTurnoEntrega = "09:00:00";
                    clie.horaFinPrimerTurnoEntrega = "18:00:00";
                    clie.horaInicioSegundoTurnoEntrega = "";
                    clie.horaFinSegundoTurnoEntrega = "";

                    clie.direccionEntregaList = new List<DireccionEntrega>();
                    clie.solicitanteList = new List<Solicitante>();
                    clie.sedeList = new List<Cliente>();

                    clie.ubigeo = new Ubigeo();
                    clie.responsableComercial = new Vendedor();
                    clie.asistenteServicioCliente = new Vendedor();
                    clie.responsabelPortafolio = new Vendedor();
                    clie.supervisorComercial = new Vendedor();
                    clie.origen = new Origen();
                    clie.subDistribuidor = new SubDistribuidor();
                    clie.subDistribuidor.idSubDistribuidor = 0;


                    clie.CargaMasiva = false;
                    clie.clienteAdjuntoList = new List<ClienteAdjunto>();
                }

                return clie;
            }
        }

        public List<Ciudad> getCiudadesCliente(int idClienteSunat)
        {
            using (var dal = new ClienteDAL())
            {
                return dal.getCiudadesSedes(idClienteSunat);
            }
        }

        public List<Cliente> getClientesByRUC(string ruc)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                List<Cliente> lista = clienteDAL.getClientesByRUC(ruc);

                return lista;
            }
        }

        public List<DocumentoDetalle> getPreciosVigentesCliente(Guid idCliente, DateTime? fechaPreciosVigenciaDesde = null)
        {
            using (var productoDal = new ProductoDAL())
            {
                List<DocumentoDetalle> items = productoDal.getPreciosVigentesCliente(idCliente, fechaPreciosVigenciaDesde);
                foreach (DocumentoDetalle pedidoDetalle in items)
                {
                    if (pedidoDetalle.producto.image == null)
                    {
                        FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                        MemoryStream storeStream = new MemoryStream();
                        storeStream.SetLength(inStream.Length);
                        inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                        storeStream.Flush();
                        inStream.Close();
                        pedidoDetalle.producto.image = storeStream.GetBuffer();
                    }


                    //Se agrega el IGV al precioLista
                    pedidoDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedidoDetalle.producto.precioSinIgv));
                    pedidoDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedidoDetalle.producto.costoSinIgv));



                    /*      if (pedidoDetalle.esPrecioAlternativo)
                          {
                              pedidoDetalle.producto.precioClienteProducto.precioUnitario =
                              pedidoDetalle.producto.precioClienteProducto.precioUnitario / pedidoDetalle.ProductoPresentacion.Equivalencia;
                          }*/

                    pedidoDetalle.porcentajeDescuento = (1 - (pedidoDetalle.precioNeto / (pedidoDetalle.precioLista == 0 ? 1 : pedidoDetalle.precioLista))) * 100;
                    pedidoDetalle.porcentajeMargen = (1 - (pedidoDetalle.costoLista / (pedidoDetalle.precioNeto == 0 ? 1 : pedidoDetalle.precioNeto))) * 100;
                }


                return items;
            }
        }

        public List<DocumentoDetalle> getPreciosHistoricoCliente(Guid idCliente)
        {
            using (var productoDal = new ProductoDAL())
            {
                List<DocumentoDetalle> items = productoDal.getPreciosHistoricoCliente(idCliente);
                foreach (DocumentoDetalle pedidoDetalle in items)
                {
                    if (pedidoDetalle.producto.image == null)
                    {
                        FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                        MemoryStream storeStream = new MemoryStream();
                        storeStream.SetLength(inStream.Length);
                        inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                        storeStream.Flush();
                        inStream.Close();
                        pedidoDetalle.producto.image = storeStream.GetBuffer();
                    }


                    //Se agrega el IGV al precioLista
                    pedidoDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedidoDetalle.producto.precioSinIgv));
                    pedidoDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedidoDetalle.producto.costoSinIgv));



                    /*      if (pedidoDetalle.esPrecioAlternativo)
                          {
                              pedidoDetalle.producto.precioClienteProducto.precioUnitario =
                              pedidoDetalle.producto.precioClienteProducto.precioUnitario / pedidoDetalle.ProductoPresentacion.Equivalencia;
                          }*/

                    pedidoDetalle.porcentajeDescuento = (1 - (pedidoDetalle.precioNeto / (pedidoDetalle.precioLista == 0 ? 1 : pedidoDetalle.precioLista))) * 100;
                    pedidoDetalle.porcentajeMargen = (1 - (pedidoDetalle.costoLista / (pedidoDetalle.precioNeto == 0 ? 1 : pedidoDetalle.precioNeto))) * 100;
                }


                return items;
            }
        }

        public bool setSKUCliente(String skuCliente, Guid idCliente, Guid idUsuario, Guid idProducto)
        {
            using (ProductoDAL productoDal = new ProductoDAL())
            {
                return productoDal.setSKUCliente(skuCliente, idCliente, idUsuario, idProducto);
            }
        }

        public List<Cliente> getClientes(Cliente cliente)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                return clienteDAL.SelectClientes(cliente);
            }
        }

        public List<Cliente> getClientesLite(Cliente cliente)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                return clienteDAL.SelectClientesLite(cliente);
            }
        }

        public Guid getClienteId(String ruc, String codigoSedeMP)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                return clienteDAL.getClienteId(ruc, codigoSedeMP);
            }
        }
        /*
        public Cliente insertCliente(Cliente cliente)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                return clienteDAL.insertCliente(cliente);
            }
        }
        */

        public async System.Threading.Tasks.Task<Cliente> insertClienteSunatAsync(Cliente cliente)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                if (cliente.asistenteServicioCliente.idVendedor == 0)
                {
                    cliente.asistenteServicioCliente.idVendedor = Constantes.ID_VENDEDOR_POR_ASIGNAR;
                    cliente.vendedoresAsignados = false;
                }
                else
                {
                    cliente.vendedoresAsignados = cliente.usuario.modificaResponsableComercial;
                }

                if (cliente.supervisorComercial.idVendedor == 0)
                {
                    cliente.supervisorComercial.idVendedor = Constantes.ID_VENDEDOR_POR_ASIGNAR;
                }
                if (cliente.responsableComercial.idVendedor == 0)
                {
                    cliente.responsableComercial.idVendedor = Constantes.ID_VENDEDOR_POR_ASIGNAR;
                }

                if (!cliente.negociacionMultiregional)
                {
                    cliente.sedePrincipal = false;
                }


                cliente = clienteDAL.insertClienteSunat(cliente);
                if (cliente.responsableComercial.idVendedor > 0)
                {
                    cliente.chrAsesor.idCliente = cliente.idCliente;
                    cliente.chrAsesor.usuario = cliente.usuario;
                    cliente.chrAsesor.valor = cliente.responsableComercial.idVendedor.ToString();
                    cliente.chrAsesor.defaultValues("responsableComercial");
                    clienteDAL.insertClienteReasignacionHistorico(cliente.chrAsesor);
                }

                if (cliente.supervisorComercial.idVendedor > 0)
                {
                    cliente.chrSupervisor.idCliente = cliente.idCliente;
                    cliente.chrSupervisor.usuario = cliente.usuario;
                    cliente.chrSupervisor.valor = cliente.supervisorComercial.idVendedor.ToString();
                    cliente.chrSupervisor.defaultValues("supervisorComercial");
                    clienteDAL.insertClienteReasignacionHistorico(cliente.chrSupervisor);
                }

                if (cliente.asistenteServicioCliente.idVendedor > 0)
                {
                    cliente.chrAsistente.idCliente = cliente.idCliente;
                    cliente.chrAsistente.usuario = cliente.usuario;
                    cliente.chrAsistente.valor = cliente.asistenteServicioCliente.idVendedor.ToString();
                    cliente.chrAsistente.defaultValues("asistenteServicioCliente");
                    clienteDAL.insertClienteReasignacionHistorico(cliente.chrAsistente);
                }

                enviarNotificacionSolicitudCredito(cliente);


                if (cliente.esAgenteRetencion)
                {
                    UsuarioDAL usuarioDal = new UsuarioDAL();
                    RolDAL rolDal = new RolDAL();
                    Mensaje notificacion = new Mensaje();
                    notificacion.titulo = "SE REGISTRÓ CLIENTE COMO AGENTE DE RETENCIÓN";
                    notificacion.mensaje = "El usuario " + cliente.usuario.nombre + " registró el cliente \"" + cliente.razonSocialSunat + "\" como agente de retención.";
 
                    notificacion.fechaInicioMensaje = DateTime.Now;
                    notificacion.fechaVencimientoMensaje = DateTime.Now.AddDays(7);
                    notificacion.user = usuarioDal.getUsuario(Constantes.IDUSUARIOZAS);
                    notificacion.importancia = "Alta";

                    List<int> idRoles = new List<int>();
                    idRoles.Add(Constantes.IDROLJEFECREDITOS);
                    idRoles.Add(Constantes.IDROLJEFEFACTURACION);

                    notificacion.listUsuario = rolDal.getUsuariosRoles(idRoles);
                    notificacion.listUsuario.RemoveAll(item => item.idUsuario == cliente.usuario.idUsuario);

                    MensajeDAL mensajeDal = new MensajeDAL();
                    mensajeDal.insertMensaje(notificacion);
                }

                ClienteWS ws = new ClienteWS();
                ws.urlApi = Constantes.NEXTSOFT_API_URL;
                ws.apiToken = Constantes.NEXTSOFT_API_TOKEN;

                object result = await ws.crearCliente(ConverterMPToNextSoft.toCliente(cliente));

                cliente.FechaEdicion = cliente.FechaEdicion;

                return cliente;
            }
        }

        public Cliente insertClienteLite(Cliente cliente)
        {
            using (var clienteDAL = new ClienteDAL())
            { 
                cliente = clienteDAL.insertClienteLite(cliente);
                cliente.esClienteLite = true;
                return cliente;
            }
        }


        public async System.Threading.Tasks.Task<Cliente> updateClienteSunatAsync(Cliente cliente)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                if (cliente.asistenteServicioCliente.idVendedor == 0)
                {
                    cliente.asistenteServicioCliente.idVendedor = Constantes.ID_VENDEDOR_POR_ASIGNAR;
                    cliente.vendedoresAsignados = false;
                }
                if (cliente.asistenteServicioCliente.idVendedor == Constantes.ID_VENDEDOR_POR_ASIGNAR)
                {
                    cliente.vendedoresAsignados = false;
                }
                else if (!cliente.vendedoresAsignados)
                {
                    cliente.vendedoresAsignados = cliente.usuario.modificaResponsableComercial;
                }

                if (cliente.supervisorComercial.idVendedor == 0)
                {
                    cliente.supervisorComercial.idVendedor = Constantes.ID_VENDEDOR_POR_ASIGNAR;
                }
                if (cliente.responsableComercial.idVendedor == 0)
                {
                    cliente.responsableComercial.idVendedor = Constantes.ID_VENDEDOR_POR_ASIGNAR;
                }

                if (!cliente.negociacionMultiregional)
                {
                    cliente.sedePrincipal = false;
                }

                Cliente clientePrev = clienteDAL.getCliente(cliente.idCliente);

                cliente = clienteDAL.updateClienteSunat(cliente);
                
                if (cliente.responsableComercial.idVendedor != clientePrev.responsableComercial.idVendedor && clientePrev.responsableComercial.idVendedor != 0)
                {
                    cliente.chrAsesor.idCliente = cliente.idCliente;
                    cliente.chrAsesor.usuario = cliente.usuario;
                    cliente.chrAsesor.valor = cliente.responsableComercial.idVendedor.ToString();
                    cliente.chrAsesor.campo = "responsableComercial";
                    clienteDAL.insertClienteReasignacionHistorico(cliente.chrAsesor);

                    // Notificar
                    Vendedor asesorAnt = cliente.usuario.vendedorList.Where(item => item.idVendedor.Equals(clientePrev.responsableComercial.idVendedor)).FirstOrDefault();
                    Vendedor asesorNuevo = cliente.usuario.vendedorList.Where(item => item.idVendedor.Equals(cliente.responsableComercial.idVendedor)).FirstOrDefault();
                    Vendedor supervisor = cliente.usuario.vendedorList.Where(item => item.idVendedor.Equals(cliente.supervisorComercial.idVendedor)).FirstOrDefault();


                    Mensaje notificacion = new Mensaje();
                    notificacion.titulo = "REASIGNACIÓN DE ASESOR COMERCIAL";
                    notificacion.mensaje = "Se cambió el asesor comercial para el cliente \"" + cliente.razonSocialSunat + "\". <br/><br/> <b>Se cambió:</b>  " + asesorAnt.descripcion + " <br/> <b>Por:</b> " + asesorNuevo.descripcion + ".";
                    if (!cliente.chrAsesor.observacion.Trim().Equals(""))
                    {
                        notificacion.mensaje = notificacion.mensaje + "<br/><br/><b>Observación:</b> " + cliente.chrAsesor.observacion;
                    }
                    notificacion.fechaInicioMensaje = DateTime.Now;
                    notificacion.fechaVencimientoMensaje = DateTime.Now.AddDays(7);
                    notificacion.user = cliente.usuario;
                    notificacion.importancia = "Alta";

                    UsuarioDAL usuarioDal = new UsuarioDAL();
                    notificacion.listUsuario = usuarioDal.selectUsuariosPorPermiso(Constantes.VALIDA_RESPONSABLES_COMERCIALES_ASIGNADOS, 3);


                    if (asesorAnt != null && notificacion.listUsuario.Where(item => item.idUsuario.Equals(asesorAnt.usuario.idUsuario)).FirstOrDefault() == null)
                    {
                        notificacion.listUsuario.Add(asesorAnt.usuario);
                    }

                    if (asesorNuevo != null && notificacion.listUsuario.Where(item => item.idUsuario.Equals(asesorNuevo.usuario.idUsuario)).FirstOrDefault() == null)
                    {
                        notificacion.listUsuario.Add(asesorNuevo.usuario);
                    }

                    if (supervisor != null && notificacion.listUsuario.Where(item => item.idUsuario.Equals(supervisor.usuario.idUsuario)).FirstOrDefault() == null)
                    {
                        notificacion.listUsuario.Add(supervisor.usuario);
                    }

                    notificacion.listUsuario.RemoveAll(item => item.idUsuario == cliente.usuario.idUsuario);

                    MensajeDAL mensajeDal = new MensajeDAL();
                    mensajeDal.insertMensaje(notificacion);
                }

                if (cliente.supervisorComercial.idVendedor != clientePrev.supervisorComercial.idVendedor && clientePrev.supervisorComercial.idVendedor != 0)
                {
                    cliente.chrSupervisor.idCliente = cliente.idCliente;
                    cliente.chrSupervisor.usuario = cliente.usuario;
                    cliente.chrSupervisor.valor = cliente.supervisorComercial.idVendedor.ToString();
                    cliente.chrSupervisor.campo = "supervisorComercial";
                    clienteDAL.insertClienteReasignacionHistorico(cliente.chrSupervisor);
                }

                if (cliente.asistenteServicioCliente.idVendedor != clientePrev.asistenteServicioCliente.idVendedor && clientePrev.asistenteServicioCliente.idVendedor != 0)
                {
                    cliente.chrAsistente.idCliente = cliente.idCliente;
                    cliente.chrAsistente.usuario = cliente.usuario;
                    cliente.chrAsistente.valor = cliente.asistenteServicioCliente.idVendedor.ToString();
                    cliente.chrAsistente.campo = "asistenteServicioCliente";
                    clienteDAL.insertClienteReasignacionHistorico(cliente.chrAsistente);
                }

                if ((cliente.usuario == null || !cliente.usuario.modificaMiembrosGrupoCliente) &&
                    ((cliente.grupoCliente == null && clientePrev.grupoCliente != null) || (cliente.grupoCliente != null && clientePrev.grupoCliente == null) || cliente.grupoCliente.idGrupoCliente != clientePrev.grupoCliente.idGrupoCliente))
                {
                    AlertaValidacion alerta = new AlertaValidacion();
                    alerta.idRegistro = cliente.idCliente.ToString();
                    alerta.nombreTabla = "CLIENTE";
                    alerta.Estado = 0;
                    alerta.UsuarioCreacion = cliente.usuario;
                    alerta.IdUsuarioCreacion = cliente.usuario.idUsuario;
                    alerta.tipo = AlertaValidacion.CAMBIA_GRUPO_CLIENTE;
                    alerta.data = new DataAlertaValidacion();

                    if (clientePrev.grupoCliente != null && clientePrev.grupoCliente.idGrupoCliente != 0)
                    {
                        alerta.data.PrevData = clientePrev.grupoCliente.codigoNombre;
                    }
                    else
                    {
                        alerta.data.PrevData = AlertaValidacion.NO_DATA_TEXT;
                    }

                    if (cliente.grupoCliente != null && cliente.grupoCliente.idGrupoCliente != 0)
                    {
                        alerta.data.PostData = cliente.grupoCliente.codigoNombre;
                    }
                    else
                    {
                        alerta.data.PostData = AlertaValidacion.NO_DATA_TEXT;
                    }

                    alerta.data.ObjData = cliente.codigoRazonSocial;

                    AlertaValidacionDAL alertaDal = new AlertaValidacionDAL();
                    alertaDal.insertAlertaValidacion(alerta);
                }

                if (cliente.esAgenteRetencion != clientePrev.esAgenteRetencion)
                {
                    UsuarioDAL usuarioDal = new UsuarioDAL();
                    RolDAL rolDal = new RolDAL();
                    Mensaje notificacion = new Mensaje();
                    if (cliente.esAgenteRetencion)
                    {
                        notificacion.titulo = "SE REGISTRÓ CLIENTE COMO AGENTE DE RETENCIÓN";
                        notificacion.mensaje = "El usuario " + cliente.usuario.nombre + " registró el cliente \"" + cliente.razonSocialSunat + "\" como agente de retención.";
                    }
                    else
                    {
                        notificacion.titulo = "SE RETIRÓ REGISTRÓ DE CLIENTE COMO AGENTE DE RETENCIÓN";
                        notificacion.mensaje = "El usuario " + cliente.usuario.nombre + " retiró el registró el cliente \"" + cliente.razonSocialSunat + "\" como agente de retención.";
                    }


                    notificacion.fechaInicioMensaje = DateTime.Now;
                    notificacion.fechaVencimientoMensaje = DateTime.Now.AddDays(7);
                    notificacion.user = usuarioDal.getUsuario(Constantes.IDUSUARIOZAS);
                    notificacion.importancia = "Alta";

                    List<int> idRoles = new List<int>();
                    idRoles.Add(Constantes.IDROLJEFECREDITOS);
                    idRoles.Add(Constantes.IDROLJEFEFACTURACION);

                    notificacion.listUsuario = rolDal.getUsuariosRoles(idRoles);
                    notificacion.listUsuario.RemoveAll(item => item.idUsuario == cliente.usuario.idUsuario);

                    MensajeDAL mensajeDal = new MensajeDAL();
                    mensajeDal.insertMensaje(notificacion);
                }

                /*Si existen cambios en la solicitud o aprobacion de créditos*/
                if (cliente.existenCambiosCreditos)
                {
                    enviarNotificacionSolicitudCredito(cliente);
                }

                ClienteWS ws = new ClienteWS();
                ws.urlApi = Constantes.NEXTSOFT_API_URL;
                ws.apiToken = Constantes.NEXTSOFT_API_TOKEN;

                object result = await ws.crearCliente(ConverterMPToNextSoft.toCliente(cliente));


                return cliente;
            }
        }

        public bool agregarProductoCanasta(Guid idCliente, Guid idProducto, Usuario usuario)
        {
            using (var dal = new PrecioClienteProductoDAL())
            {

                return dal.agregaProductoCanastaCliente(idCliente, idProducto, usuario.idUsuario);
            }
        }


        public bool retiraProductoCanasta(Guid idCliente, Guid idProducto, Usuario usuario)
        {
            using (var dal = new PrecioClienteProductoDAL())
            {

                return dal.retiraProductoCanastaCliente(idCliente, idProducto, usuario.idUsuario);
            }
        }


        private void enviarNotificacionSolicitudCredito(Cliente cliente)
        {
            MailService mail = new MailService();
            List<String> destinatarios = new List<String>();
            /*Si el usuario es aprobador de créditos y aplicó el cambio entonces se notifica al solicitante*/
            if (cliente.usuario.apruebaPlazoCredito || cliente.usuario.apruebaMontoCredito)
            {
                if (cliente.usuarioSolicitante != null && cliente.usuarioSolicitante.email != null && !cliente.usuarioSolicitante.email.Equals(String.Empty))
                {
                    destinatarios.Add(cliente.usuarioSolicitante.email);
                    String asunto = "APROBACIÓN de Crédito - " + cliente.razonSocial + " (" + cliente.codigo + ")";
                    String bodyMail = String.Empty;
                    bodyMail = @"</p>Estimados, </p>" +
                        "</p>Se ha modificado el plazo de crédito aprobado, el monto de crédito aprobado o la forma de pago del cliente:" + cliente.razonSocial + "(" + cliente.codigo + ").</p>";
                    mail.enviar(destinatarios, asunto, bodyMail, Constantes.MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, Constantes.PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, new Usuario());
                }
            }
            /*Si el usuario NO es aprobador y aplicó el cambio entonces se notifica al aprobador de créditos*/
            else
            {
                //Enviar correo a Creditos
                destinatarios.Add(Constantes.MAIL_CREDITOS);
                String asunto = "SOLICITUD de Crédito - " + cliente.razonSocial + " (" + cliente.codigo + ")";
                String bodyMail = String.Empty;
                bodyMail = @"</p>Estimados, </p>" +
                    "</p>Se ha modificado el plazo de crédito solicitado, el monto de crédito solicitado o la forma de pago del cliente: " + cliente.razonSocial + " (" + cliente.codigo + ").</p>";
                mail.enviar(destinatarios, asunto, bodyMail, Constantes.MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, Constantes.PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, new Usuario());
            }
        }

        /*public Cliente updateClienteExcel(Cliente cliente)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                
                if (!cliente.negociacionMultiregional)
                {
                    cliente.sedePrincipal = false;
                }


                cliente = clienteDAL.updateClienteExcel(cliente);

                MailService mail = new MailService();

                if (cliente.existenCambiosCreditos)
                {
                    List<String> destinatarios = new List<String>();
                    if (cliente.usuario.apruebaPlazoCredito || cliente.usuario.apruebaMontoCredito)
                    {
                        if (cliente.usuarioSolicitante != null && cliente.usuarioSolicitante.email != null && !cliente.usuarioSolicitante.email.Equals(String.Empty))
                        {
                            destinatarios.Add(cliente.usuarioSolicitante.email);
                            String asunto = "APROBACIÓN de Crédito - " + cliente.razonSocial + " (" + cliente.codigo + ")";
                            String bodyMail = String.Empty;
                            bodyMail = @"</p>Estimados, </p>" +
                                "</p>Se ha modificado el plazo de crédito aprobado, el monto de crédito aprobado o la forma de pago del cliente:" + cliente.razonSocial + "(" + cliente.codigo + ").</p>";
                            mail.enviar(destinatarios, asunto, bodyMail, Constantes.MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, Constantes.PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, new Usuario());
                        }
                    }
                    else
                    {
                        //Enviar correo a Creditos
                        destinatarios.Add(Constantes.MAIL_CREDITOS);
                        String asunto = "SOLICITUD de Crédito - " + cliente.razonSocial + " (" + cliente.codigo + ")";
                        String bodyMail = String.Empty;
                        bodyMail = @"</p>Estimados, </p>" +
                            "</p>Se ha modificado el plazo de crédito solicitado, el monto de crédito solicitado o la forma de pago del cliente: " + cliente.razonSocial + " (" + cliente.codigo + ").</p>";
                        mail.enviar(destinatarios, asunto, bodyMail, Constantes.MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, Constantes.PASSWORD_MAIL_COMUNICACION_PEDIDOS_NO_ATENDIDOS, new Usuario());
                    }
                }

                return cliente;
            }
        }*/

        public void setClienteStaging(ClienteStaging clienteStaging)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                clienteDAL.setClienteStaging(clienteStaging);
            }
        }

        public void truncateClienteStaging(String sede)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                clienteDAL.truncateClienteStaging(sede);
            }
        }

        public void mergeClienteStaging()
        {
            using (var clienteDAL = new ClienteDAL())
            {
                clienteDAL.mergeClienteStaging();
            }
        }

        public void insertClienteReasignacionHistorico(ClienteReasignacionHistorico obj)
        {
            using (var dal = new ClienteDAL())
            {
                dal.insertClienteReasignacionHistorico(obj);
            }
        }


        public List<ClienteReasignacionHistorico> getHistorialReasignacionesClientePorCampo(String campo, Guid idCliente)
        {
            using (var dal = new ClienteDAL())
            {
                return dal.getHistorialReasignacionesClientePorCampo(campo, idCliente);
            }
        }

        public Boolean deleteClienteReasignacionHistorico(Guid idClienteHistorialReasignacion, Guid idCliente, Guid idUsuario)
        {
            using (var dal = new ClienteDAL())
            {
                return dal.deleteClienteReasignacionHistorico(idClienteHistorialReasignacion, idCliente, idUsuario);
            }
        }

        public Boolean insertarClienteReasignacionHistorico(ClienteReasignacionHistorico chr)
        {
            using (var dal = new ClienteDAL())
            {
                dal.insertClienteReasignacionHistorico(chr);
                return true;
            }
        }


        public List<Cliente> BusquedaClientesCartera(Cliente cliente)
        {
            using (var dal = new ClienteDAL())
            {
                return dal.BusquedaClientesCartera(cliente);
            }
        }

        public void UpdateReasignarCartera(List<Guid> idsCliente, List<int> idsVendedor, List<Guid> idsClienteSup, List<int> idsSupervisores,
                List<Guid> idsClienteAsis, List<int> idsAsistentes, DateTime fechaInicioVigencia, Guid idUsuario)
        {
            using (var dal = new ClienteDAL())
            {
                dal.UpdateReasignarCartera(idsCliente, idsVendedor, idsClienteSup, idsSupervisores, idsClienteAsis, idsAsistentes, fechaInicioVigencia, idUsuario);
            }
        }
    }
}
