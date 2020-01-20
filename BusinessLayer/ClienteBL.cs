
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using Model.ServiceSunatPadron;
using System.ServiceModel;
using System.Net;
using System.IO;
using System.Text;
using ServiceLayer;

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
                ClienteSunat clienteSunat = client.BuscarClienteSunat(cliente.ruc);

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

                        var rs2 = linea.Substring(0,fin);
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
        public String getCLientesBusqueda(String textoBusqueda,Guid idCiudad)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                List<Cliente> clienteList = clienteDAL.getClientesBusqueda(textoBusqueda, idCiudad);
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

        public String getCLientesBusqueda(String textoBusqueda)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                List<Cliente> clienteList = clienteDAL.getClientesBusqueda(textoBusqueda);
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

        public String getCLientesBusquedaRUC(String textoBusqueda)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                List<Cliente> clienteList = clienteDAL.getClientesBusquedaRUC(textoBusqueda);
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

        public List<Cliente> getCLientesBusquedaCotizacion(String textoBusqueda,Guid idCiudad)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                return clienteDAL.getClientesBusqueda(textoBusqueda, idCiudad);
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

                return clie;
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
        
        public List<DocumentoDetalle> getPreciosVigentesCliente(Guid idCliente)
        {
            using (var productoDal = new ProductoDAL())
            {
                List<DocumentoDetalle> items = productoDal.getPreciosVigentesCliente(idCliente);
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

                    pedidoDetalle.porcentajeDescuento = (1 - (pedidoDetalle.precioNeto / (pedidoDetalle.precioLista == 0 ? 1 : pedidoDetalle.precioLista)))*100;
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

        public Guid getClienteId(String ruc,String codigoSedeMP)
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

        public Cliente insertClienteSunat(Cliente cliente)
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
                if(cliente.responsableComercial.idVendedor > 0) {
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

                return cliente;
            }
        }

        public Cliente updateClienteSunat(Cliente cliente)
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

                if (!cliente.responsableComercial.idVendedor.ToString().Equals(cliente.chrAsesor.preValor))
                {
                    cliente.chrAsesor.idCliente = cliente.idCliente;
                    cliente.chrAsesor.usuario = cliente.usuario;
                    cliente.chrAsesor.valor = cliente.responsableComercial.idVendedor.ToString();
                    cliente.chrAsesor.campo = "responsableComercial";
                    clienteDAL.insertClienteReasignacionHistorico(cliente.chrAsesor);

                    // Notificar
                }

                if (!cliente.supervisorComercial.idVendedor.ToString().Equals(cliente.chrSupervisor.preValor))
                {
                    cliente.chrSupervisor.idCliente = cliente.idCliente;
                    cliente.chrSupervisor.usuario = cliente.usuario;
                    cliente.chrSupervisor.valor = cliente.supervisorComercial.idVendedor.ToString();
                    cliente.chrSupervisor.campo = "supervisorComercial";
                    clienteDAL.insertClienteReasignacionHistorico(cliente.chrSupervisor);
                }

                if (!cliente.asistenteServicioCliente.idVendedor.ToString().Equals(cliente.chrAsistente.preValor))
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
                    } else {
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
            
                /*Si existen cambios en la solicitud o aprobacion de créditos*/
                if (cliente.existenCambiosCreditos)
                {
                    enviarNotificacionSolicitudCredito(cliente);
                }

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
    }
}
