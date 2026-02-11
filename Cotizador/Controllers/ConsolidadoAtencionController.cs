using BusinessLayer;
using Cotizador.Models;
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
    public class VehiculoController : ParentController
    {
        [HttpGet]
        public ActionResult List()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            // Asumiendo que existe este permiso en tu clase Usuario
            if (!usuario.modificaMaestroVehiculos)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaVehiculos;

            if (this.Session[Constantes.VAR_SESSION_VEHICULO_BUSQUEDA] == null)
            {
                instanciarVehiculoBusqueda();
            }

            Vehiculo objSearch = (Vehiculo)this.Session[Constantes.VAR_SESSION_VEHICULO_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaVehiculos;
            ViewBag.vehiculo = objSearch;

            return View();
        }

        public String SearchList()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaVehiculos;

            Vehiculo obj = (Vehiculo)this.Session[Constantes.VAR_SESSION_VEHICULO_BUSQUEDA];

            VehiculoBL bL = new VehiculoBL();
            // Se asume que el método Listar acepta los mismos parámetros base
            List<Vehiculo> list = bL.getVehiculos(obj);

            this.Session[Constantes.VAR_SESSION_VEHICULO_LISTA] = list;
            return JsonConvert.SerializeObject(list);
        }

        private void instanciarVehiculo()
        {
            Vehiculo obj = new Vehiculo();
            obj.idVehiculo = 0;
            obj.Estado = 1;
            obj.placa = String.Empty;
            obj.marca = String.Empty;
            obj.modelo = String.Empty;
            obj.ciudad = new Ciudad(); 

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_VEHICULO] = obj;
        }

        private void instanciarVehiculoBusqueda()
        {
            Vehiculo obj = new Vehiculo();
            obj.idVehiculo = 0;
            obj.Estado = 1;
            obj.placa = String.Empty;
            obj.marca = String.Empty;
            obj.modelo = String.Empty;
            obj.ciudad = new Ciudad();

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_VEHICULO_BUSQUEDA] = obj;
        }

        [HttpGet]
        public ActionResult Index()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaMaestroVehiculos)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        public String Create()
        {
            VehiculoBL bL = new VehiculoBL();
            Vehiculo obj = new Vehiculo();

            obj.placa = Request["placa"].ToString();
            obj.marca = Request["marca"].ToString();
            obj.modelo = Request["modelo"].ToString();
            obj.Estado = 1;

            // Mapeo de la ciudad desde el Select (ID 1 o 2)
            obj.ciudad = new Ciudad { idCiudad = Guid.Parse(Request["idCiudad"].ToString()) };

            obj.IdUsuarioRegistro = Logueado.idUsuario;

            obj = bL.insertVehiculo(obj);
            this.Session[Constantes.VAR_SESSION_VEHICULO] = null;
            return JsonConvert.SerializeObject(obj);
        }

        public String Update()
        {
            VehiculoBL bL = new VehiculoBL();
            Vehiculo obj = new Vehiculo();

            obj.idVehiculo = int.Parse(Request["idVehiculo"].ToString());
            obj.placa = Request["placa"].ToString();
            obj.marca = Request["marca"].ToString();
            obj.modelo = Request["modelo"].ToString();

            // Asignación de ciudad
            if (!string.IsNullOrEmpty(Request["idCiudad"]))
            {
                obj.ciudad = new Ciudad { idCiudad = Guid.Parse(Request["idCiudad"]) };
            }

            obj.Estado = 1;
            obj.IdUsuarioRegistro = Logueado.idUsuario;

            if (obj.idVehiculo == 0)
            {
                obj = bL.insertVehiculo(obj);
            }
            else
            {
                obj = bL.updateVehiculo(obj);
            }

            this.Session[Constantes.VAR_SESSION_VEHICULO] = null;
            return JsonConvert.SerializeObject(obj);
        }

        private Vehiculo VehiculoSession
        {
            get
            {
                Vehiculo obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaVehiculos:
                        obj = (Vehiculo)this.Session[Constantes.VAR_SESSION_VEHICULO_BUSQUEDA]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaVehiculos:
                        this.Session[Constantes.VAR_SESSION_VEHICULO_BUSQUEDA] = value; break;
                }
            }
        }
    }
}