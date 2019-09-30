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


        [HttpGet]
        public ActionResult Index()
        {

            //this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BUsqueda;

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (this.Session[Constantes.VAR_SESSION_CLIENTE_BUSQUEDA] == null)
            {
                //instanciarClienteBusqueda();
            }

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

        public String GetDireccionesEntregaCliente()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            DireccionEntregaBL direccionEntregaBL = new DireccionEntregaBL();
            List<DireccionEntrega> direccionEntregaList = direccionEntregaBL.getDireccionesEntrega(usuario.idClienteSunat);

            DomicilioLegalBL domicilioLegalBL = new DomicilioLegalBL();
            List<DomicilioLegal> domicilioLegalList = domicilioLegalBL.getDomiciliosLegalesPorClienteSunat(usuario.idClienteSunat);

            String resultado = "{\"direccionEntregaList\":" + JsonConvert.SerializeObject(direccionEntregaList) +
                        ", \"domicilioLegalList\":" + JsonConvert.SerializeObject(domicilioLegalList) + "}";

            //   this.Session[Constantes.VAR_SESSION_CLIENTE_VER] = cliente;
            return resultado;
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