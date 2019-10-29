﻿using BusinessLayer;
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
            if (!usuario.visualizaMensaje)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpGet]
        public ActionResult Editar(Guid? id_mensaje = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoMensaje;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaMensaje)
            {
                return RedirectToAction("Lista", "Mensaje");
            }

            RolBL rolbl = new RolBL();
            List<Rol> roles = new List<Rol>();
            Rol rol = new Rol();
            rol.Estado = 1;
            roles = rolbl.getRoles(rol);
            this.Session[Constantes.VAR_SESSION_ROL_LISTA] = roles;

            if (this.Session[Constantes.VAR_SESSION_MENSAJE] == null && id_mensaje == null)
            {
                instanciarMensaje();
            }

            Mensaje mensaje = (Mensaje)this.Session[Constantes.VAR_SESSION_MENSAJE];

            if (id_mensaje != null)
            {
                MensajeBL bL = new MensajeBL();
                mensaje = bL.getMensajeById(id_mensaje.Value);
                mensaje.user.idUsuario = usuario.idUsuario;
                mensaje.user = usuario;

                this.Session[Constantes.VAR_SESSION_MENSAJE] = mensaje;
            }

            ViewBag.Mensaje = mensaje;
            ViewBag.roles = roles;

            return View();

        }

        private void instanciarMensaje()
        {
            Mensaje obj = new Mensaje();
            obj.user = new Usuario();
            obj.id_mensaje = Guid.Empty;
            obj.importancia = "";
            obj.mensaje = String.Empty;
            obj.titulo = String.Empty;
            obj.estado = 1;
            obj.fechaInicioMensaje = DateTime.Now;
            obj.fechaVencimientoMensaje = DateTime.Now;
            obj.listUsuario = new List<Usuario>();
            obj.user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            this.Session[Constantes.VAR_SESSION_MENSAJE] = obj;


        }

        [HttpGet]
        public ActionResult Lista()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaMensaje;

            if (this.Session[Constantes.VAR_SESSION_MENSAJE_BUSQUEDA] == null)
            {
                instanciarBusquedaMensaje();
            }

            Mensaje objSearch = (Mensaje)this.Session[Constantes.VAR_SESSION_MENSAJE_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaMensaje;
            ViewBag.Mensaje = objSearch;

            return View();
        }

        private void instanciarBusquedaMensaje()
        {
            Mensaje obj = new Mensaje();
            obj.user = new Usuario();
            obj.estado = 1;
            obj.id_mensaje = Guid.Empty;
            obj.fechaCreacionMensajeDesde = null;
            obj.fechaCreacionMensajeHasta = null;

            obj.fechaVencimientoMensajeDesde = null;
            obj.fechaVencimientoMensajeHasta = null;
         

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.user.idUsuario = usuario.idUsuario;

            this.Session[Constantes.VAR_SESSION_MENSAJE_BUSQUEDA] = obj;
        }

        public String SearchList()
        {
                       
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaMensaje;
            Mensaje obj = (Mensaje)this.Session[Constantes.VAR_SESSION_MENSAJE_BUSQUEDA];
            obj.user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            MensajeBL bL = new MensajeBL();
            List<Mensaje> list = bL.getListMensajes(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_MENSAJE_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
        }

        public String ConsultarSiExisteMensaje()
        {
            Guid idMensaje = Guid.Parse(Request["idMensaje"].ToString());
            MensajeBL bL = new MensajeBL();
            Mensaje obj = bL.getMensajeById(idMensaje);

            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_MENSAJE_VER] = obj;

            obj = (Mensaje)this.Session[Constantes.VAR_SESSION_MENSAJE];
            if (obj == null || obj.id_mensaje == Guid.Empty)
                return "{\"existe\":\"false\",\"idMensaje\":\"0\"}";
            else
                return "{\"existe\":\"true\",\"idMensaje\":\"" + obj.id_mensaje + "\"}";
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




        public String Create()
        {
            MensajeBL bL = new MensajeBL();
            Mensaje obj = (Mensaje)this.Session[Constantes.VAR_SESSION_MENSAJE];

            RolBL rolbl = new RolBL();
            List<Rol> roles = new List<Rol>();
            Rol rol = new Rol();
            rol.Estado = 1;
            roles = rolbl.getRoles(rol);

            obj = bL.insertMensaje(obj);
            this.Session[Constantes.VAR_SESSION_MENSAJE] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }

        public void iniciarEdicionMensaje()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Mensaje obj = (Mensaje)this.Session[Constantes.VAR_SESSION_MENSAJE_VER];
            this.Session[Constantes.VAR_SESSION_MENSAJE] = obj;
        }


        public void ChangeInputString()
        {
            Mensaje obj = (Mensaje)this.MensajeSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.MensajeSession = obj; ;

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
            instanciarBusquedaMensaje();

        }

        public void ChangeRol()
        {

            Mensaje obj = (Mensaje)this.MensajeSession;

            int valor = Int32.Parse(this.Request.Params["valor"]);
            int rol_select = Int32.Parse(this.Request.Params["rol"].ToString().Replace("rol_", ""));
            List<Rol> listaRoles = (List<Rol>)this.Session[Constantes.VAR_SESSION_ROL_LISTA];
            if (valor == 0)
            {
                //Remove
                foreach (Rol rol in obj.roles)
                {
                    if (rol_select == rol.idRol)
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
                    if (rol_select == rol.idRol)
                    {
                        obj.roles.Add(rol);
                        break;
                    }
                }
            }
            this.MensajeSession = obj;

        }


        public void ChangeUsuarioMensaje(Guid[] idUsuario)
        {
            Mensaje obj = (Mensaje)this.MensajeSession;
            obj.listUsuario = new List<Usuario>();
            if (idUsuario != null)
            {
                for (var i = 0; i < idUsuario.Length; i++)
                {
                    Usuario user = new Usuario();
                    UsuarioBL userbl= new UsuarioBL();
                    user = userbl.getUsuario(idUsuario[i]);                    
                    obj.listUsuario.Add(user);
                }
            }
            else
            {
                obj.listUsuario = new List<Usuario>();
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

        public void UpdateMensajeVisto()
        {
            Mensaje obj = new Mensaje();
            obj.user = new Usuario();

            obj.id_mensaje = Guid.Parse(Request["idMensaje"].ToString());
            obj.user.idUsuario = Guid.Parse(Request["idUsuario"].ToString());
            MensajeBL bL = new MensajeBL();
            bL.updateMensajeVisto(obj);
        }



        public String UpdateMensaje()
        {

            MensajeBL bL = new MensajeBL();
            Mensaje obj = (Mensaje)this.Session[Constantes.VAR_SESSION_MENSAJE];
            obj.user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //Mensaje update = (Mensaje)this.MensajeSession;
            //obj.titulo = update.titulo;
            //obj.importancia = update.importancia;
            //obj.fechaInicioMensaje = update.fechaInicioMensaje;
            //obj.fechaVencimientoMensaje = update.fechaVencimientoMensaje;
            //obj.mensaje = update.mensaje;
            if (obj.id_mensaje == null || obj.id_mensaje == Guid.Empty)
            {
                obj = bL.insertMensaje(obj);
                this.Session[Constantes.VAR_SESSION_MENSAJE] = null;
            }
            else
            {

                obj = bL.updateMensaje(obj);
                this.Session[Constantes.VAR_SESSION_MENSAJE] = null;
            }
            String resultado = JsonConvert.SerializeObject(obj);

            return resultado;

        }

        public void ChangeFechaInicio()
        {
            Mensaje obj = (Mensaje)this.MensajeSession;
            String[] fecha = this.Request.Params["fechaInicio"].Split('/');
            obj.fechaInicioMensaje = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.MensajeSession = obj;
        }       

        public void ChangeFechaVencimiento()
        {
            Mensaje obj = (Mensaje)this.MensajeSession;
            String[] fecha = this.Request.Params["fechaVencimiento"].Split('/');
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            if (fecha[0] == "")
            {
                propertyInfo.SetValue(obj,null);
            }
            else
            {
                propertyInfo.SetValue(obj, new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0])));
            }
           
            this.MensajeSession = obj;

        }

        public void ChangeFechaVencimientoEdit()
        {
            Mensaje obj = (Mensaje)this.MensajeSession;
            String[] fecha = this.Request.Params["fechaVencimiento"].Split('/');
            obj.fechaVencimientoMensaje = obj.fechaInicioMensaje = new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0]));
            this.MensajeSession = obj;

        }

        public void ChangeFechaCreacion()
        {
            Mensaje obj = (Mensaje)this.MensajeSession;
            String[] fecha = this.Request.Params["fechaCreacion"].Split('/');
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            if (fecha[0] == "")
            {
                propertyInfo.SetValue(obj,null);
            }
            else
            {
                propertyInfo.SetValue(obj,new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0])));
            }
            
            this.MensajeSession = obj;

        }

        public ActionResult CancelarCreacionMensaje()
        {
            this.Session[Constantes.VAR_SESSION_MENSAJE] = null;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            return RedirectToAction("Lista", "Mensaje");
        }
    }
}