
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

                    if (pedidoDetalle.esPrecioAlternativo)
                    {
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario =
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario / pedidoDetalle.producto.equivalencia;
                    }

                    pedidoDetalle.porcentajeDescuento = (1 - (pedidoDetalle.precioNeto / (pedidoDetalle.precioLista == 0 ? 1 : pedidoDetalle.precioLista)))*100;
                    pedidoDetalle.porcentajeMargen = (1 - (pedidoDetalle.costoLista / (pedidoDetalle.precioNeto == 0 ? 1 : pedidoDetalle.precioNeto))) * 100;
                }
                

                return items;
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

        public Cliente insertCliente(Cliente cliente)
        {
            using (var clienteDAL = new ClienteDAL())
            {
                return clienteDAL.insertCliente(cliente);
            }
        }


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
                return clienteDAL.insertClienteSunat(cliente);
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
                

                cliente = clienteDAL.updateClienteSunat(cliente);

                MailService mail = new MailService();
                /*Si existen cambios en la solicitud o aprobacion de créditos*/
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
    }
}
