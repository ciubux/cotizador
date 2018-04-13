using BusinessLayer;
using Model;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class ClienteController : Controller
    {
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

            clienteBL.truncateClienteStaging(sede);

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
    }
}