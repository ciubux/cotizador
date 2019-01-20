﻿using BusinessLayer;
using Cotizador.ExcelExport;
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
                Cliente cliente = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaClientes: cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoCliente: cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE]; break;
                }
                return cliente;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaClientes: this.Session[Constantes.VAR_SESSION_CLIENTE_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoCliente: this.Session[Constantes.VAR_SESSION_CLIENTE] = value; break;
                }
            }
        }

        [HttpGet]
        public ActionResult Index()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaClientes;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (this.Session[Constantes.VAR_SESSION_CLIENTE_BUSQUEDA] == null)
            {
                instanciarClienteBusqueda();
            }

            Cliente clienteSearch = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaClientes;
            ViewBag.cliente = clienteSearch;
            ViewBag.Si = Constantes.MENSAJE_SI;
            ViewBag.No = Constantes.MENSAJE_NO;
            return View();

        }



        public String GetDatosSunat()
        {
            try
            { 
                this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoCliente;

                if (this.Session[Constantes.VAR_SESSION_CLIENTE] == null)
                {
                    instanciarCliente();
                }

                Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];

                ClienteBL clienteBL = new ClienteBL();
                cliente = clienteBL.getDatosPadronSunat(cliente);
                cliente.nombreComercial = clienteBL.getNombreComercial(cliente);
                cliente.nombreComercialSunat = cliente.nombreComercial;
                String resultado = JsonConvert.SerializeObject(cliente);
                return resultado;
            }
            catch (Exception ex)
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);
            }
            return "";
        }



        public void ChangeInputString()
        {
            Cliente cliente = this.ClienteSession;
            PropertyInfo propertyInfo = cliente.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(cliente, this.Request.Params["valor"]);
            this.ClienteSession = cliente;
        }

        public void ChangeInputInt()
        {
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];
            PropertyInfo propertyInfo = cliente.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(cliente, Int32.Parse(this.Request.Params["valor"]));
            this.Session[Constantes.VAR_SESSION_CLIENTE] = cliente;
        }

        public void ChangeInputDecimal()
        {
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];
            PropertyInfo propertyInfo = cliente.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(cliente, Decimal.Parse(this.Request.Params["valor"]));
            this.Session[Constantes.VAR_SESSION_CLIENTE] = cliente;
        }

        public void ChangeInputTime()
        {
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];
            PropertyInfo propertyInfo = cliente.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(cliente, Int32.Parse(this.Request.Params["valor"]));
            this.Session[Constantes.VAR_SESSION_CLIENTE] = cliente;
        }


        public void ChangeFormaPagoFactura()
        {
            this.ClienteSession.formaPagoFactura = (DocumentoVenta.FormaPago)Int32.Parse(this.Request.Params["formaPagoFactura"]);
        }

        public void ChangeTipoPagoFactura()
        {
            this.ClienteSession.tipoPagoFactura = (DocumentoVenta.TipoPago)Int32.Parse(this.Request.Params["tipoPagoFactura"]);
        }

        public void ChangePlazoCreditoSolicitado()
        {
            this.ClienteSession.plazoCreditoSolicitado = (DocumentoVenta.TipoPago)Int32.Parse(this.Request.Params["plazoCreditoSolicitado"]);
        }

        private void instanciarCliente()
        {
            Cliente cliente = new Cliente();
            cliente.idCliente = Guid.Empty;
            cliente.ciudad = new Ciudad();
            cliente.codigo = String.Empty;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            cliente.IdUsuarioRegistro = usuario.idUsuario;
            cliente.usuario = usuario;
            cliente.grupoCliente = new GrupoCliente();
            cliente.sedePrincipal = false;
            cliente.negociacionMultiregional = false;
            cliente.observacionHorarioEntrega = "";

            this.Session[Constantes.VAR_SESSION_CLIENTE] = cliente;
        }

        private void instanciarClienteBusqueda()
        {
            this.Session[Constantes.VAR_SESSION_CLIENTE_BUSQUEDA] = instanciarClienteBusquedaBasic();
        }

        private Cliente instanciarClienteBusquedaBasic()
        {
            Cliente cliente = new Cliente();
            cliente.idCliente = Guid.Empty;
            cliente.ciudad = new Ciudad();
            cliente.vendedoresAsignados = false;
            cliente.sinPlazoCreditoAprobado = false;
            cliente.bloqueado = false;
            cliente.codigo = String.Empty;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            cliente.IdUsuarioRegistro = usuario.idUsuario;
            cliente.usuario = usuario;
            cliente.grupoCliente = new GrupoCliente();

            return cliente;
        }

        public void CleanBusqueda()
        {
            instanciarClienteBusqueda();
        }

        public ActionResult CancelarCreacionCliente()
        {
            this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("Index", "Cliente");
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


            if (this.Session[Constantes.VAR_SESSION_CLIENTE] == null || idCliente == Guid.Empty)
            {

                instanciarCliente();
            }

            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];

            List<GrupoCliente> grupoClienteList = new List<GrupoCliente>();

            if (idCliente != null && idCliente != Guid.Empty)
            {
                ClienteBL clienteBL = new ClienteBL();
                cliente = clienteBL.getCliente(idCliente.Value);
                cliente.IdUsuarioRegistro = usuario.idUsuario;
                cliente.usuario = usuario;
                GrupoClienteBL grupoClienteBL = new GrupoClienteBL();
                grupoClienteList = grupoClienteBL.getGruposCliente();
                this.Session[Constantes.VAR_SESSION_CLIENTE] = cliente;
            }

            ViewBag.cliente = cliente;
            ViewBag.gruposCliente = grupoClienteList;
            return View();

        }

        [HttpGet]
        public ActionResult ExportLastSearchExcel()
        {
            List<Cliente> list = (List<Cliente>)this.Session[Constantes.VAR_SESSION_CLIENTE_LISTA];

            ClienteSearch excel = new ClienteSearch();
            return excel.generateExcel(list);
        }

        public String SearchClientes()
        {
            String data = this.Request.Params["data[q]"];
            ClienteBL clienteBL = new ClienteBL();
            return clienteBL.getCLientesBusqueda(data, this.ClienteSession.ciudad.idCiudad);
        }

        public String Search()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaClientes;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE_BUSQUEDA];
            ClienteBL clienteBL = new ClienteBL();
            List<Cliente> clienteList = clienteBL.getClientes(cliente);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_CLIENTE_LISTA] = clienteList;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(clienteList);
            //return pedidoList.Count();
        }


        public String GetCliente()
        {
            Cliente cliente = this.ClienteSession; 
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            Ciudad ciudad = cliente.ciudad;
            cliente = clienteBl.getCliente(idCliente);
            cliente.ciudad = ciudad;
            cliente.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            String resultado = JsonConvert.SerializeObject(cliente);
            this.ClienteSession = cliente;
            return resultado;
        }

        public String Show()
        {
            Guid idCliente = Guid.Parse(Request["idCliente"].ToString());
            ClienteBL clienteBl = new ClienteBL();
            Cliente cliente = clienteBl.getCliente(idCliente);
            cliente.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            List<DocumentoDetalle> listaPrecios = clienteBl.getPreciosVigentesCliente(idCliente);
            String resultado = "{\"cliente\":" + JsonConvert.SerializeObject(cliente) + ", \"precios\":" + JsonConvert.SerializeObject(listaPrecios) + "}";
            this.Session[Constantes.VAR_SESSION_CLIENTE_VER] = cliente;
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

        public String ChangeIdGrupoCliente()
        {
            Cliente cliente = this.ClienteSession;

            Int32 idGrupoCliente = 0;
            if (this.Request.Params["idGrupoCliente"] != null && !this.Request.Params["idGrupoCliente"].Equals(""))
            {
                idGrupoCliente = Int32.Parse(this.Request.Params["idGrupoCliente"]);
                GrupoClienteBL grupoClienteBL = new GrupoClienteBL();
                List<GrupoCliente> grupoClientList = grupoClienteBL.getGruposCliente();
                cliente.grupoCliente = grupoClientList.Where(g => g.idGrupoCliente == idGrupoCliente).FirstOrDefault();

            }
            else
            {
                cliente.grupoCliente.idGrupoCliente = 0;
                cliente.grupoCliente.nombre = String.Empty;
            }

            this.ClienteSession = cliente;
            return "{\"idGrupoCliente\": \"" + idGrupoCliente + "\"}";

        }

        public String Create()
        {
            ClienteBL clienteBL = new ClienteBL();
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];
            cliente = clienteBL.insertClienteSunat(cliente);
            if (cliente.codigo != null && !cliente.codigo.Equals(""))
            {
                this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            }
            String resultado = JsonConvert.SerializeObject(cliente);            
            return resultado;
        }


        public String Update()
        {
            ClienteBL clienteBL = new ClienteBL();
            Cliente cliente = this.ClienteSession;
            if (cliente.idCliente == Guid.Empty)
            {
                cliente = clienteBL.insertClienteSunat(cliente);
                if (cliente.codigo != null && !cliente.codigo.Equals(""))
                {
                    this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
                }
            }
            else
            {
                cliente = clienteBL.updateClienteSunat(cliente);
                this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            }
            String resultado = JsonConvert.SerializeObject(cliente);
            //this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
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
            Usuario usuarioSession = ((Usuario)this.Session[Constantes.VAR_SESSION_USUARIO]);
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


        public void changeTipoDocumentoIdentidad()
        {
            ClienteSession.tipoDocumentoIdentidad = (DocumentoVenta.TiposDocumentoIdentidad) Int32.Parse(this.Request.Params["tipoDocumentoIdentidad"]);
        }

        /*MANTENIMIENTO DE VENDEDORES*/
        public void ChangeIdResponsableComercial()
        {
            if (this.Request.Params["idResponsableComercial"] == null || this.Request.Params["idResponsableComercial"] == String.Empty)
                ClienteSession.responsableComercial.idVendedor = 0;
            else
                ClienteSession.responsableComercial.idVendedor = Int32.Parse(this.Request.Params["idResponsableComercial"]);
        }
        public void ChangeIdAsistenteServicioCliente()
        {
            if (this.Request.Params["idAsistenteServicioCliente"] == null || this.Request.Params["idAsistenteServicioCliente"] == String.Empty)
                ClienteSession.asistenteServicioCliente.idVendedor = 0;
            else
                ClienteSession.asistenteServicioCliente.idVendedor = Int32.Parse(this.Request.Params["idAsistenteServicioCliente"]);
        }
        public void ChangeIdSupervisorComercial()
        {
            if (this.Request.Params["idSupervisorComercial"] == null || this.Request.Params["idSupervisorComercial"] == String.Empty)
                ClienteSession.supervisorComercial.idVendedor = 0;
            else
                ClienteSession.supervisorComercial.idVendedor = Int32.Parse(this.Request.Params["idSupervisorComercial"]);
        }
      /*  public void ChangeBloqueado()
        {
            ClienteSession.bloqueado = Int32.Parse(this.Request.Params["bloqueado"]) == 1;
        }*/
        public void ChangeInputBoolean()
        {
            Cliente cliente = this.ClienteSession;
            PropertyInfo propertyInfo = cliente.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(cliente, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.ClienteSession = cliente;
        }
        

        public void ChangeInputBitBoolean()
        {
            Cliente cliente = this.ClienteSession;
            PropertyInfo propertyInfo = cliente.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(cliente, Boolean.Parse(this.Request.Params["valor"]) == true);
            this.ClienteSession = cliente;
        }


        public void ChangeSinPlazoCreditoAprobado()
        {
            ClienteSession.sinPlazoCreditoAprobado = Int32.Parse(this.Request.Params["sinPlazoCreditoAprobado"]) == 1;
        }

        public void ChangeSinAsesorValidado()
        {
            ClienteSession.vendedoresAsignados = Int32.Parse(this.Request.Params["sinAsesorValidado"]) == 1;
        }

        public String ConsultarSiExisteCliente()
        {
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];
            if (cliente == null)
                return "{\"existe\":\"false\",\"codigo\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"codigo\":\"" + cliente.codigo + "\"}";
        }


        public void iniciarEdicionCliente()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Cliente clienteVer = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE_VER];
            this.Session[Constantes.VAR_SESSION_CLIENTE] = clienteVer;
        }

        [HttpPost]
        public String UpdateByExcel(HttpPostedFileBase file)
        {
            /*     if (file.ContentLength > 0)
                 {
                     var fileName = Path.GetFileName(file.FileName);
                     var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                     file.SaveAs(path);
                 }
                 */
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            try
            {

                HSSFWorkbook hssfwb;

                ClienteBL clienteBL = new ClienteBL();

                hssfwb = new HSSFWorkbook(file.InputStream);

                ISheet sheet = hssfwb.GetSheetAt(0);
                int row = 1;
                int cantidad = sheet.LastRowNum;
                List<Cliente> clientesTemp = new List<Cliente>();
                string textoTemp = "";
                //   cantidad = 2008;
                //sheet.LastRowNum

                GrupoClienteBL grupoClienteBL = new GrupoClienteBL();
                List<GrupoCliente> grupoClienteList = grupoClienteBL.getGruposCliente();

                for (row = 1; row <= cantidad; row++)
                {
                    if (sheet.GetRow(row) != null && usuario.modificaMaestroClientes) //null is when the row only contains empty cells 
                    {

                        Cliente item = new Cliente();
                        item = instanciarClienteBusquedaBasic();

                        try
                        {
                            /* 0 codigo cliente
                             * 6 grupo, 
                             * 9 asesor comercial, 
                             * 11 supervisor, 
                             * 13 asistente servicio,   
                             * 15 plazo credito aprobado
                             * 16 monto credito aprobado
                             * 18 cotiza multiregional
                             * 19 sede principal
                             * 20 Canal Multiregional
                             * 21 Canal Lima
                             * 22 Canal Provincia
                             * 23 Canal PCP
                             * 24 Canal Subdistribuidor
                             * 25 Canal Ordon 
                            */
                            

                            if (sheet.GetRow(row).GetCell(0) != null)
                            {
                                item.codigo = sheet.GetRow(row).GetCell(0).ToString();
                                clientesTemp = clienteBL.getClientes(item);
                                if (clientesTemp.Count == 1)
                                {
                                    item = clienteBL.getCliente(clientesTemp.ElementAt(0).idCliente);
                                }
                            }


                            if (item.idCliente != Guid.Empty)
                            {
                                item.usuario = usuario;
                                if (sheet.GetRow(row).GetCell(6) != null)
                                {
                                    textoTemp = sheet.GetRow(row).GetCell(6).ToString().ToUpper().Trim();
                                    if (!textoTemp.Equals(item.grupoCliente.codigo))
                                    {
                                        foreach (GrupoCliente gc in grupoClienteList)
                                        {
                                            if (gc.codigo.Equals(textoTemp))
                                            {
                                                item.grupoCliente = gc;
                                            }
                                        }
                                    }
                                }

                                if (sheet.GetRow(row).GetCell(9) != null)
                                {
                                    textoTemp = sheet.GetRow(row).GetCell(9).ToString().ToUpper().Trim();
                                    if (!textoTemp.Equals(item.responsableComercial.codigo))
                                    {
                                        foreach (Vendedor rc in usuario.responsableComercialList)
                                        {
                                            if (rc.codigo.Equals(textoTemp))
                                            {
                                                item.responsableComercial = rc;
                                            }
                                        }
                                    }
                                }

                                if (sheet.GetRow(row).GetCell(11) != null)
                                {
                                    textoTemp = sheet.GetRow(row).GetCell(11).ToString().ToUpper().Trim();
                                    if (!textoTemp.Equals(item.supervisorComercial.codigo))
                                    {
                                        foreach (Vendedor sc in usuario.supervisorComercialList)
                                        {
                                            if (sc.codigo.Equals(textoTemp))
                                            {
                                                item.supervisorComercial = sc;
                                            }
                                        }
                                    }
                                }

                                if (sheet.GetRow(row).GetCell(13) != null)
                                {
                                    textoTemp = sheet.GetRow(row).GetCell(13).ToString().ToUpper().Trim();
                                    if (!textoTemp.Equals(item.asistenteServicioCliente.codigo))
                                    {
                                        foreach (Vendedor aser in usuario.asistenteServicioClienteList)
                                        {
                                            if (aser.codigo.Equals(textoTemp))
                                            {
                                                item.asistenteServicioCliente = aser;
                                            }
                                        }
                                    }
                                }

                                if (sheet.GetRow(row).GetCell(16) != null)
                                {
                                    //item.creditoAprobado = Decimal.Parse(sheet.GetRow(row).GetCell(16).ToString());
                                }

                                if (sheet.GetRow(row).GetCell(18) != null)
                                {
                                    item.negociacionMultiregional = sheet.GetRow(row).GetCell(18).ToString().ToUpper().Equals("SI");
                                }

                                if (sheet.GetRow(row).GetCell(19) != null)
                                {
                                    item.sedePrincipal = sheet.GetRow(row).GetCell(19).ToString().ToUpper().Equals("SI");
                                }

                                if (sheet.GetRow(row).GetCell(20) != null)
                                {
                                    item.perteneceCanalMultiregional = sheet.GetRow(row).GetCell(20).ToString().ToUpper().Equals("SI");
                                }

                                if (sheet.GetRow(row).GetCell(21) != null)
                                {
                                    item.perteneceCanalLima = sheet.GetRow(row).GetCell(21).ToString().ToUpper().Equals("SI");
                                }

                                if (sheet.GetRow(row).GetCell(22) != null)
                                {
                                    item.perteneceCanalProvincias = sheet.GetRow(row).GetCell(22).ToString().ToUpper().Equals("SI");
                                }

                                if (sheet.GetRow(row).GetCell(23) != null)
                                {
                                    item.perteneceCanalPCP = sheet.GetRow(row).GetCell(23).ToString().ToUpper().Equals("SI");
                                }

                                if (sheet.GetRow(row).GetCell(24) != null)
                                {
                                    item.esSubDistribuidor = sheet.GetRow(row).GetCell(24).ToString().ToUpper().Equals("SI");
                                }

                                if (sheet.GetRow(row).GetCell(25) != null)
                                {
                                    item.perteneceCanalOrdon = sheet.GetRow(row).GetCell(25).ToString().ToUpper().Equals("SI");
                                }

                                clienteBL.updateClienteSunat(item);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                            LogBL logBL = new LogBL();
                            logBL.insertLog(log);   
                        }
                    }
                }

                return "{\"success\":\"true\",\"message\":\"Se procesó el archivo correctamente.\"}";
            }
            catch (Exception ex)
            {
                Log log = new Log(ex.ToString(), TipoLog.Error, usuario);
                LogBL logBL = new LogBL();
                logBL.insertLog(log);

                return "{\"success\":\"false\",\"message\":\"Error al cargar el fichero.\"}";
            }
        }


        #region carga de imagenes

        public void ChangeFiles(List<HttpPostedFileBase> files)
        {
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];

            if ((int)this.Session[Constantes.VAR_SESSION_PAGINA] == (int)Constantes.paginas.BusquedaClientes)
                cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE_VER];

            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (cliente.clienteAdjuntoList.Where(p => p.nombre.Equals(file.FileName)).FirstOrDefault() != null)
                    {
                        continue;
                    }

                    ClienteAdjunto clienteAdjunto = new ClienteAdjunto();
                    using (Stream inputStream = file.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        clienteAdjunto.nombre = file.FileName;
                        clienteAdjunto.adjunto = memoryStream.ToArray();
                    }
                    cliente.clienteAdjuntoList.Add(clienteAdjunto);
                }
            }

        }


        public String DescartarArchivos()
        {
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];
            if ((int)this.Session[Constantes.VAR_SESSION_PAGINA] == (int)Constantes.paginas.BusquedaClientes)
                cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE_VER];

            String nombreArchivo = Request["nombreArchivo"].ToString();

            List<ClienteAdjunto> clienteAdjuntoList = new List<ClienteAdjunto>();
            foreach (ClienteAdjunto clienteAdjunto in cliente.clienteAdjuntoList)
            {
                if (!clienteAdjunto.nombre.Equals(nombreArchivo))
                    clienteAdjuntoList.Add(clienteAdjunto);
            }

            cliente.clienteAdjuntoList = clienteAdjuntoList;

            return JsonConvert.SerializeObject(cliente.clienteAdjuntoList);
        }

        public String Descargar()
        {
            String nombreArchivo = Request["nombreArchivo"].ToString();
            Cliente cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE];

            if ((int)this.Session[Constantes.VAR_SESSION_PAGINA] == (int)Constantes.paginas.BusquedaClientes)
            {
                cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE_VER];
            }

            ArchivoAdjunto archivoAdjunto = cliente.clienteAdjuntoList.Where(p => p.nombre.Equals(nombreArchivo)).FirstOrDefault();

            if (archivoAdjunto != null)
            {
                ArchivoAdjuntoBL archivoAdjuntoBL = new ArchivoAdjuntoBL();
                archivoAdjunto = archivoAdjuntoBL.GetArchivoAdjunto(archivoAdjunto);
                return JsonConvert.SerializeObject(archivoAdjunto);
            }
            else
            {
                return null;
            }

        }

        #endregion
    }
}