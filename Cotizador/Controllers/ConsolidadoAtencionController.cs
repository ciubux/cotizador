using BusinessLayer;
using Cotizador.ExcelExport;
using Cotizador.Models;
using Model;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class ConsolidadoAtencionController : ParentController
    {
        [HttpGet]
        public ActionResult List()
        {
            if (!Logueado.visualizaConsolidadoAtencion && !Logueado.modificaMaestroConsolidadoAtencion) 
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaConsolidadoAtencion;

            if (this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION_BUSQUEDA] == null)
            {
                instanciarConsolidadoAtencionBusqueda();
            }

            ConsolidadoAtencion objSearch = (ConsolidadoAtencion)this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaConsolidadoAtencion;
            ViewBag.consolidado = objSearch;

            return View();
        }

        public String SearchList()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaConsolidadoAtencion;
            ConsolidadoAtencion obj = (ConsolidadoAtencion)this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION_BUSQUEDA];

            ConsolidadoAtencionBL bL = new ConsolidadoAtencionBL();
            List<ConsolidadoAtencion> list = bL.getConsolidadosAtencion(obj);

            this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION_LISTA] = list;
            return JsonConvert.SerializeObject(list);
        }

        private void instanciarConsolidadoAtencionBusqueda()
        {
            ConsolidadoAtencion obj = new ConsolidadoAtencion();
            obj.idConsolidadoAtencion = Guid.Empty;
            obj.fecha = DateTime.Now;
            obj.Estado = 1;
            this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION_BUSQUEDA] = obj;
        }

        private void instanciarConsolidadoAtencion()
        {
            ConsolidadoAtencion obj = new ConsolidadoAtencion();
            obj.idConsolidadoAtencion = Guid.Empty;
            obj.fecha = DateTime.Now;
            obj.Estado = 1;
            obj.vehiculo = new Vehiculo();
            obj.chofer = new PersonalAlmacen();
            obj.asistente = new PersonalAlmacen();
            obj.ciudad = Logueado.sedeMP;

            this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION] = obj;
        }

        [HttpGet]
        public ActionResult Editar()
        {
            if (!Logueado.modificaMaestroConsolidadoAtencion) 
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.RegistroConsolidadoAtencion;

            if (this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION] == null)
            {
                instanciarConsolidadoAtencion();
            }

            ConsolidadoAtencion item = (ConsolidadoAtencion)this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION];

            List<Vehiculo> vehiculos = new List<Vehiculo>();
            List<PersonalAlmacen> choferes = new List<PersonalAlmacen> ();
            List<PersonalAlmacen> asistentes = new List<PersonalAlmacen>();
            List<Pedido> pedidos = new List<Pedido>();
            if (item.ciudad != null && !item.ciudad.idCiudad.Equals(Guid.Empty))
            {
                VehiculoBL blVehiculo = new VehiculoBL();
                PersonalAlmacenBL blPersonal = new PersonalAlmacenBL();

                Vehiculo vehiculoSe = new Vehiculo { Estado = 1, ciudad = item.ciudad };

                vehiculos = blVehiculo.getVehiculos(vehiculoSe);

                PersonalAlmacen choferSe = new PersonalAlmacen { Estado = 1, sedePrincipal = item.ciudad, tipo = "CHOFER" };
                PersonalAlmacen asistenteSe = new PersonalAlmacen { Estado = 1, sedePrincipal = item.ciudad, tipo = "ASISTENTE" };

                choferes = blPersonal.getPersonalesAlmacen(choferSe);
                asistentes = blPersonal.getPersonalesAlmacen(asistenteSe);

                //PedidoBL blPedido = new PedidoBL();
                //pedidos = blPedido.SelectPedidosConsolidar(item.ciudad.idCiudad, Logueado.idUsuario, item.fecha);
            }


            ViewBag.vehiculos = vehiculos;
            ViewBag.choferes = choferes;
            ViewBag.asistentes = asistentes;
            //ViewBag.pedidos = pedidos;

            ViewBag.pagina = (int)Constantes.paginas.RegistroConsolidadoAtencion;
            ViewBag.item = item;

            return View();
        }


        [HttpPost]
        public string GetPedidosConsolidar()
        {
            ConsolidadoAtencion obj = (ConsolidadoAtencion)this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION];

            //SelectPedidosConsolidar
            PedidoBL blPedido = new PedidoBL();
            List<Pedido> lista = blPedido.SelectPedidosConsolidar(obj.ciudad.idCiudad, Logueado.idUsuario, obj.fecha);

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.RegistroConsolidadoAtencion;

            return JsonConvert.SerializeObject(lista);
        }
        

        public String Create()
        {
            ConsolidadoAtencionBL bL = new ConsolidadoAtencionBL();
            ConsolidadoAtencion obj = new ConsolidadoAtencion();

            obj.fecha = DateTime.Parse(Request["fecha"]);
            obj.observaciones = Request["observaciones"] ?? "";
            obj.vehiculo = new Vehiculo { idVehiculo = int.Parse(Request["idVehiculo"]) };
            obj.chofer = new PersonalAlmacen { idPersonalAlmacen = int.Parse(Request["idChofer"]) };
            obj.asistente = new PersonalAlmacen { idPersonalAlmacen = int.Parse(Request["idAsistente"]) };
            obj.Estado = 1;
            obj.IdUsuarioRegistro = Logueado.idUsuario;

            obj = bL.insertConsolidadoAtencion(obj);
            return JsonConvert.SerializeObject(obj);
        }

        public String Update()
        {
            ConsolidadoAtencionBL bL = new ConsolidadoAtencionBL();
            ConsolidadoAtencion obj = new ConsolidadoAtencion();

            obj.idConsolidadoAtencion = Guid.Parse(Request["idConsolidadoAtencion"]);
            obj.fecha = DateTime.Parse(Request["fecha"]);
            obj.observaciones = Request["observaciones"] ?? "";
            obj.vehiculo = new Vehiculo { idVehiculo = int.Parse(Request["idVehiculo"]) };
            obj.chofer = new PersonalAlmacen { idPersonalAlmacen = int.Parse(Request["idChofer"]) };
            obj.asistente = new PersonalAlmacen { idPersonalAlmacen = int.Parse(Request["idAsistente"]) };

            obj.IdUsuarioRegistro = Logueado.idUsuario;

            obj = bL.updateConsolidadoAtencion(obj);
            return JsonConvert.SerializeObject(obj);
        }

        public void ChangeInputForm()
        {
            string tipo = this.Request.Params["tipo"].ToString();
            ConsolidadoAtencion obj = (ConsolidadoAtencion)this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION];
            PropertyInfo propertyInfo = null;
            switch (tipo)
            {
                case "string":
                    propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
                    propertyInfo.SetValue(obj, this.Request.Params["valor"]);
                    break;

                case "int":
                    propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
                    propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
                    break;

                case "date":
                    string fechaParam = Request.Params["valor"].ToString();

                    if (!fechaParam.Trim().Equals(""))
                    {
                        String[] fecha = fechaParam.Split('/');
                        propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
                        propertyInfo.SetValue(obj, new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0])));
                    }
                    break;

                case "ciudad":
                    Guid idCiudad = Guid.Parse(this.Request.Params["valor"]);
                    if (!idCiudad.Equals(obj.ciudad.idCiudad)) {
                        obj.ciudad.idCiudad = idCiudad;
                        obj.vehiculo = new Vehiculo();
                        obj.chofer = new PersonalAlmacen();
                        obj.asistente = new PersonalAlmacen();
                    }
                    break;

                case "vehiculo":
                    obj.vehiculo.idVehiculo = Int32.Parse(this.Request.Params["valor"]);
                    break;

                case "chofer":
                    obj.chofer.idPersonalAlmacen = Int32.Parse(this.Request.Params["valor"]);
                    break;

                case "asistente":
                    obj.asistente.idPersonalAlmacen = Int32.Parse(this.Request.Params["valor"]);
                    break;
            }
            
            this.Session[Constantes.VAR_SESSION_CONSOLIDADOATENCION] = obj;
        }
    }
}
