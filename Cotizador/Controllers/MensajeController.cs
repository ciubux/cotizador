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
    public class MensajeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaVendedor)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Crear(int? idRol = null)
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaMensaje;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (this.Session[Constantes.VAR_SESSION_MENSAJE_BUSQUEDA] == null)
            {
                instanciarMensaje();
            }

            Rol obj2 = new Rol();
            obj2.Estado = 1;

            RolBL permisobl = new RolBL();
            List<Rol> permisos = new List<Rol>();

            permisos = permisobl.getRoles(obj2);

            this.Session[Constantes.VAR_SESSION_ROL_LISTA] = permisos;

            Mensaje obj = new Mensaje();
            obj.user = usuario;
            obj.roles = new List<Rol>();
            MensajeSession = obj;

            ViewBag.roles = permisos;

            return View();


        }
        private Mensaje MensajeSession
        {
            get
            {
                Mensaje obj = null;

                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {

                    case Constantes.paginas.BusquedaMensaje: obj = (Mensaje)this.Session[Constantes.VAR_SESSION_MENSAJE_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoMensaje: obj = (Mensaje)this.Session[Constantes.VAR_SESSION_MENSAJE]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaMensaje: this.Session[Constantes.VAR_SESSION_MENSAJE_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoMensaje: this.Session[Constantes.VAR_SESSION_MENSAJE] = value; break;
                }
            }
        }


        private void instanciarMensaje()
        {
            Mensaje obj = new Mensaje();
            obj.importancia = "";
            obj.mensaje = String.Empty;
            obj.titulo = String.Empty;
            MensajeSession = obj;
            //obj.fechaHasta = new DateTime(fechaHasta.Year, fechaHasta.Month, fechaHasta.Day, 23, 59, 59);
            obj.user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

        }

        public String Create()
        {
            MensajeBL bL = new MensajeBL();
            Mensaje obj = (Mensaje)this.MensajeSession;            
            obj = bL.insertMensaje(obj);
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }

        public void ChangeInputString()
        {
            Mensaje obj = (Mensaje)this.MensajeSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.MensajeSession = obj;
        }

        public void ChangeInputInt()
        {

            Mensaje obj = (Mensaje)this.MensajeSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.MensajeSession = obj;
        }


        public void Limpiar()
        {
            instanciarMensaje();

        }

        public void ChangeRol()
        {

            Mensaje obj = (Mensaje)this.MensajeSession;
            int valor = Int32.Parse(this.Request.Params["valor"]);
            int permiso = Int32.Parse(this.Request.Params["rol"].ToString().Replace("rol_", ""));
            List<Rol> listaRoles = (List<Rol>)this.Session[Constantes.VAR_SESSION_ROL_LISTA];
            if (valor == 0)
            {
                //Remove
                foreach (Rol rol in obj.roles)
                {
                    if (permiso == rol.idRol)
                    {
                        obj.roles.Remove(rol);
                        break;
                    }
                }
            }
            else
            {
                //ADD
                foreach (Rol rol in listaRoles)
                {
                    if (permiso == rol.idRol)
                    {
                        obj.roles.Add(rol);
                        break;
                    }
                }
            }

            this.MensajeSession = obj;
        }


        public String ShowMensaje()
        {
            Guid idUsuario;
            idUsuario = Guid.Parse(Request["idUsuario"].ToString());
            MensajeBL bL = new MensajeBL();
            List<Mensaje> list = bL.getMensajes(idUsuario);
            return JsonConvert.SerializeObject(list);
        }

        public void UpdateMensaje()
        {
            Mensaje obj = new Mensaje();
            obj.user = new Usuario();

            obj.id_mensaje = Guid.Parse(Request["idMensaje"].ToString());
            obj.user.idUsuario = Guid.Parse(Request["idUsuario"].ToString());
            MensajeBL bL = new MensajeBL();
            bL.updateMensaje(obj);
        }

        public void changeFecha()
        {
            Mensaje obj = this.MensajeSession;
            String[] fecha = this.Request.Params["fechaVencimiento"].Split('/');
            obj.fechaVencimiento = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.MensajeSession = obj;
        }
    }
}