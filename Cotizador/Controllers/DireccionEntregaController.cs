using BusinessLayer;
using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class DireccionEntregaController : Controller
    {
        // GET: DireccionEntrega
        public ActionResult Index()
        {
            return View();
        }

        public String GetDireccionesEntrega()
        {

            String ubigeo = Request["ubigeo"].ToString();
            Pedido pedido = (Pedido)this.Session[Constantes.VAR_SESSION_PEDIDO];
            DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
            List<DireccionEntrega> direccionEntrega = direccionEntregaBL.getDireccionesEntrega(pedido.cliente.idCliente, ubigeo);
            return JsonConvert.SerializeObject(direccionEntrega);
        }


        [HttpPost]
        public void Delete()
        {
            Guid idDireccionEntrega = Guid.Parse(Request["idDireccionEntrega"].ToString());
            DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
            DireccionEntrega direccionEntrega = new DireccionEntrega {
                idDireccionEntrega = idDireccionEntrega
            };
            direccionEntrega.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            direccionEntregaBL.deleteDireccionEntrega(direccionEntrega);
        }

        [HttpPost]
        public String Create()
        {
        //     Guid idDireccionEntrega = Guid.Parse(Request["idDireccionEntrega"].ToString());
            DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
            DireccionEntrega direccionEntrega = new DireccionEntrega();
            direccionEntrega.cliente = (Cliente)this.Session[Constantes.VAR_SESSION_CLIENTE_VER];

            direccionEntrega.ubigeo = new Ubigeo
            {
                Id = Request["ubigeo"].ToString()
            };
            direccionEntrega.descripcion = Request["direccion"].ToString();
            direccionEntrega.contacto = Request["contacto"].ToString();
            direccionEntrega.telefono = Request["telefono"].ToString();
            direccionEntrega.emailRecepcionFacturas = Request["emailRecepcionFacturas"].ToString();
            direccionEntrega.codigoCliente = Request["codigoCliente"].ToString();
            direccionEntrega.nombre = Request["nombre"].ToString();
            direccionEntrega.domicilioLegal = new DomicilioLegal
            {
                idDomicilioLegal = Int32.Parse(Request["idDomicilioLegal"].ToString())
            };
            direccionEntrega.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            direccionEntregaBL.insertDireccionEntrega(direccionEntrega);
            return JsonConvert.SerializeObject(direccionEntrega);
        }


        [HttpPost]
        public void Update()
        {
            Guid idDireccionEntrega = Guid.Parse(Request["idDireccionEntrega"].ToString());
            DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
            DireccionEntrega direccionEntrega = new DireccionEntrega();


            direccionEntrega.idDireccionEntrega = idDireccionEntrega;
            direccionEntrega.ubigeo = new Ubigeo
            {
                Id = Request["ubigeo"].ToString()
            };
            direccionEntrega.descripcion = Request["direccion"].ToString();
            direccionEntrega.contacto = Request["contacto"].ToString();
            direccionEntrega.telefono = Request["telefono"].ToString();
            direccionEntrega.emailRecepcionFacturas = Request["emailRecepcionFacturas"].ToString();
            direccionEntrega.codigoCliente = Request["codigoCliente"].ToString();
            direccionEntrega.nombre = Request["nombre"].ToString();
            direccionEntrega.domicilioLegal = new DomicilioLegal
            {
                idDomicilioLegal = Int32.Parse(Request["idDomicilioLegal"].ToString())
            };
            direccionEntrega.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];


            direccionEntregaBL.updateDireccionEntrega(direccionEntrega);
        }

    }
}