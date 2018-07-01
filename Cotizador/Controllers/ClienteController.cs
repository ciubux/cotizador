using BusinessLayer;
using Model;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class ClienteController : Controller
    {
        private Cliente ClienteSession
        {
            get
            {

             /*   Cliente cliente = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    //case Constantes.paginas.BusquedaCli: cliente = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoCliente: cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE]; break;
                }
                return cliente;*/

                return (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];
            }
            set
            {/*
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    //case Constantes.paginas.BusquedaPedidos: this.Session[Constantes.VAR_SESSION_PEDIDO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoCliente: this.Session[Constantes.VAR_SESSION_CLIENTE] = value; break;
                }*/

                this.Session[Constantes.VAR_SESSION_CLIENTE] = value;
            }
        }

        // GET: Cliente
        [HttpGet]
        public ActionResult Index()
        {
            if (this.Session["usuario"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();

        }

        public void ChangeInputString()
        {
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];
            PropertyInfo propertyInfo = cliente.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(cliente, this.Request.Params["valor"]);
            this.Session[Constantes.VAR_SESSION_CLIENTE] = cliente;
        }


        public void ChangeFormaPagoFactura()
        {
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];
            cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Int32.Parse(this.Request.Params["formaPagoFactura"]);
            this.Session[Constantes.VAR_SESSION_CLIENTE] = cliente;
        }

        public void ChangeTipoPagoFactura()
        {
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];
            cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Int32.Parse(this.Request.Params["tipoPagoFactura"]);
            this.Session[Constantes.VAR_SESSION_CLIENTE] = cliente;
        }

        private void instanciarCliente()
        {
            Cliente cliente = new Cliente();
            cliente.idCliente = Guid.Empty;
            cliente.ciudad = new Ciudad();
            cliente.codigo = String.Empty;
            Usuario usuario = (Usuario)this.Session["usuario"];
            cliente.IdUsuarioRegistro = usuario.idUsuario;
            this.Session[Constantes.VAR_SESSION_CLIENTE] = cliente;
        }

        public ActionResult CancelarCreacionCliente()
        {
            this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session["usuario"];

            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("Editar", "Cliente");
        }

        public ActionResult Editar(Guid? idCliente = null )
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoCliente;
            Usuario usuario = null;
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                if (!usuario.modificaMaestroClientes)
                {
                    return RedirectToAction("Login", "Account");
                }
            }


            if (this.Session[Constantes.VAR_SESSION_CLIENTE] == null)
            {

                instanciarCliente();
            }

            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];



            if (idCliente != null)
            {
                ClienteBL clienteBL = new ClienteBL();
                cliente = clienteBL.getCliente(idCliente.Value);
                cliente.IdUsuarioRegistro = usuario.idUsuario;
                this.Session[Constantes.VAR_SESSION_CLIENTE] = cliente;
            }
                

            if (cliente.idCliente == Guid.Empty)
            {
                ViewBag.existeCliente = 0;
            }
            else {
                ViewBag.existeCliente = 1;
            }

            ViewBag.cliente = cliente;

            return View();

        }

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];
            return clienteBL.getCLientesBusqueda(data, cliente.ciudad.idCiudad);
        }


        public String GetCliente()
        {


            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE]; 
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            Ciudad ciudad = cliente.ciudad;
            cliente = clienteBl.getCliente(idCliente);
            cliente.ciudad = ciudad;
            String resultado = JsonConvert.SerializeObject(cliente);
            this.ClienteSession = cliente;
            return resultado;
        }


        public String ChangeDireccionDomicilioLegalSunat()
        {
            String direccionDomicilioLegalSunat = Request["direccionDomicilioLegalSunat"].ToString();

            String[] dirArray = direccionDomicilioLegalSunat.Split('-');

            int cantidad = dirArray.Length;

            String distrito = dirArray[cantidad - 1].Trim();
            String provincia = dirArray[cantidad - 2].Trim();
            direccionDomicilioLegalSunat = String.Empty;
            for (int i = 0; i < cantidad; i++)
            {
                if (i == cantidad - 1 || i == cantidad - 2)
                {
                    break;

                }
                else {
                    direccionDomicilioLegalSunat = direccionDomicilioLegalSunat + (dirArray[i]).Trim() + " - ";

                }
            }

            direccionDomicilioLegalSunat = direccionDomicilioLegalSunat.Trim()+ " " + provincia+ " - "+distrito;

            UbigeoBL ubigeoBL = new UbigeoBL();

            this.ClienteSession.ubigeo = ubigeoBL.getUbigeoPorDistritoProvincia(distrito,provincia);
            this.ClienteSession.direccionDomicilioLegalSunat = direccionDomicilioLegalSunat;

            var obj = new 
            {
                ubigeo = this.ClienteSession.ubigeo,
                direccionDomicilioLegalSunat = direccionDomicilioLegalSunat
            };

            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }



        public String ChangeIdCiudad()
        {
            Cliente cliente = this.ClienteSession;
            cliente = new Cliente();
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }
           
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
            cliente.ciudad = ciudadNueva;
            this.ClienteSession = cliente;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";
          
        }


        public String Create()
        {
            String controller = Request["controller"].ToString();
            IDocumento documento = (IDocumento)this.Session[controller];
            Cliente cliente = new Cliente();
            Usuario usuario = (Usuario)this.Session["usuario"];
            cliente.IdUsuarioRegistro = usuario.idUsuario;
            cliente.razonSocial = Request["razonSocial"].ToString();
            cliente.nombreComercial = Request["nombreComercial"].ToString();
            cliente.ruc = Request["ruc"].ToString();
            cliente.contacto1 = Request["contacto"].ToString();

            ClienteBL clienteBL = new ClienteBL();
            cliente.ciudad = documento.ciudad;
            documento.cliente = clienteBL.insertCliente(cliente);

            if (controller.Equals("cotizacion"))
            {
                Cotizacion cotizacion = (Cotizacion)documento;
                cotizacion.contacto = cotizacion.cliente.contacto1;
            }
            else 
            {


            }


            this.Session[controller] = documento;


            String resultado = "{" +
                "\"idCLiente\":\"" + documento.cliente.idCliente + "\"," +
                "\"codigoAlterno\":\"" + documento.cliente.codigoAlterno + "\"}";

            return resultado;
        }


        public String Update()
        {
            ClienteBL clienteBL = new ClienteBL();
            Cliente cliente = this.ClienteSession;
            if (cliente.idCliente == Guid.Empty)
            {
                cliente = clienteBL.insertClienteSunat(cliente);
            }
            else
            {
                cliente = clienteBL.updateClienteSunat(cliente);
            }
            String resultado = JsonConvert.SerializeObject(cliente);
            this.ClienteSession = null;
            return resultado;
        }


        [HttpPost]
        public ActionResult Load(HttpPostedFileBase file)
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

          //  clienteBL.truncateClienteStaging(sede);


            if (cantidad == 0)
                cantidad = sheet.LastRowNum;

            for (row = 4; row <= cantidad; row++)
            {
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    int paso = 0;
                    try
                    {
                        ClienteStaging clienteStaging = new ClienteStaging();
                        //A
                        try
                        {
                            clienteStaging.sede = sheet.GetRow(row).GetCell(0).StringCellValue.Substring(2,1);
                        }
                        catch (Exception ex)
                        {
                            clienteStaging.sede = sheet.GetRow(row).GetCell(0).ToString().Substring(2, 1);
                        }

                        //B
                        try
                        {
                            if (sheet.GetRow(row).GetCell(1).NumericCellValue == 0)
                                continue;
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }





                        //C
                        try
                        {
                            clienteStaging.codigo = sheet.GetRow(row).GetCell(2).StringCellValue;
                        }
                        catch (Exception ex)
                        {
                            clienteStaging.codigo = sheet.GetRow(row).GetCell(2).NumericCellValue.ToString();
                        }
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
                        //I
                        paso = 41;
                        try
                        {
                            clienteStaging.plazo = sheet.GetRow(row).GetCell(8).StringCellValue;
                        }
                        catch (Exception ex)
                        {
                            clienteStaging.plazo = sheet.GetRow(row).GetCell(8).NumericCellValue.ToString();
                        }
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

                      //  clienteStaging.sede = sede;

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
                        Log log = new Log(ex.ToString() + " paso: " + paso, TipoLog.Error, usuario);
                        LogBL logBL = new LogBL();
                        logBL.insertLog(log);
                    }
                }
            }

            // clienteBL.mergeClienteStaging();
            row = row;
            return RedirectToAction("Index", "Cotizacion");

        }

        public ActionResult clientesPedidoList()
        {
            Usuario usuarioSession = ((Usuario)this.Session["usuario"]);
            List<Cliente> clienteList = new List<Cliente>();
            List<Cliente> clienteListTmp = usuarioSession.clienteList;

            Cliente clienteTodos = new Cliente { razonSocial = "Todos", idCliente = Guid.Empty };
            clienteList.Add(clienteTodos);
            foreach (Cliente cliente in clienteListTmp)
            {
                clienteList.Add(cliente);
            }
            var model = clienteList;

            return PartialView("_SelectCliente", model);
        }


        
        public ActionResult clientesCargarPedidoList()
        {
            Usuario usuarioSession = ((Usuario)this.Session["usuario"]);
            List<Cliente> clienteList = new List<Cliente>();
            List<Cliente> clienteListTmp = usuarioSession.clienteList;

            Cliente clienteTodos = new Cliente { razonSocial = "-Seleccione Cliente-", idCliente = Guid.Empty };
            clienteList.Add(clienteTodos);
            foreach (Cliente cliente in clienteListTmp)
            {
                clienteList.Add(cliente);
            }
            var model = clienteList;

            return PartialView("_SelectCliente", model);
        }


    }
}