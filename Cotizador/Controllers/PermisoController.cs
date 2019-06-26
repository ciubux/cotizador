using BusinessLayer;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class PermisoController : ParentController
    {
        private Permiso PermisoSession
        {
            get
            {
                Permiso permiso = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.AsignacionPermisos: permiso = (Permiso)this.Session[Constantes.VAR_SESSION_ASIGNACION_PERMISOS]; break;
                }
                return permiso;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.AsignacionPermisos: this.Session[Constantes.VAR_SESSION_ASIGNACION_PERMISOS] = value; break;
                }
            }
        }
        // GET: Permiso
        public ActionResult Index()
        {
            return View();
        }

        private void instanciarPermisoBusqueda()
        {
            Permiso obj = new Permiso();
            obj.idPermiso = 0;
            obj.Estado = 1;
            obj.descripcion_corta = String.Empty;
            obj.descripcion_larga = String.Empty;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_PERMISO_BUSQUEDA] = obj;
        }

        private void instanciarPermiso()
        {
            Permiso obj = new Permiso();
            obj.idPermiso = 0;
            obj.Estado = 1;
            obj.descripcion_corta = String.Empty;
            obj.descripcion_larga = String.Empty;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_PERMISO_MANTENEDOR] = obj;
        }

        public ActionResult AsignarPermisos()
        {
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                if (!usuario.administraPermisos)
                {
                    return RedirectToAction("Login", "Account");
                }
            }            

            try
            { 
                this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.AsignacionPermisos;
                this.Session[Constantes.VAR_SESSION_USUARIO_LISTA] = new List<Usuario>();
                Usuario usuarioSession = ((Usuario)this.Session["usuario"]);
                PermisoBL permisoBL = new PermisoBL();
                List<Permiso> permisoList = permisoBL.getPermisos();

                List<CategoriaPermiso> categoriaPermisoList = permisoList.GroupBy(x => new  { idCategoriaPermiso=  x.categoriaPermiso.idCategoriaPermiso, descripcion=   x.categoriaPermiso.descripcion })
                          .Select(y => new CategoriaPermiso { idCategoriaPermiso = y.Key.idCategoriaPermiso, descripcion = y.Key.descripcion } //y.Sum(x => x.Quantity)
                          ).ToList();

                foreach (CategoriaPermiso categoriaPermiso in categoriaPermisoList)
                {
                    categoriaPermiso.permisoList = permisoList.Where(p => p.categoriaPermiso.idCategoriaPermiso == categoriaPermiso.idCategoriaPermiso).ToList();
                }

                UsuarioBL usuarioBL = new UsuarioBL();
                if (PermisoSession == null)
                {
                    PermisoSession = permisoList[0];
                    PermisoSession.usuarioList = usuarioBL.getUsuariosPorPermiso(PermisoSession);
                }

           
                this.Session[Constantes.VAR_SESSION_USUARIO_LISTA] = usuarioBL.getUsuarios();

                var idUsuarioArray = PermisoSession.usuarioList.Select(s => s.idUsuario).ToArray();

                List<Usuario> usuarioList = (List<Usuario>)this.Session[Constantes.VAR_SESSION_USUARIO_LISTA];
                List<Usuario> usuarioSinPermisoList = usuarioList.Where(u => !idUsuarioArray.Contains(u.idUsuario)).ToList();

                //  var model = categoriaPermisoList;
                ViewBag.permiso = PermisoSession;
                ViewBag.categoriaPermisoList = categoriaPermisoList;
                ViewBag.usuarioSinPermisoList = usuarioSinPermisoList;
                return View();
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }


        public void ChangeIdPermiso()
        {
            int idPermiso = int.Parse(this.Request.Params["idPermiso"]);
            PermisoBL permisoBL = new PermisoBL();
            List<Permiso> permisoList = permisoBL.getPermisos();
            Permiso permiso = permisoList.Where(p => p.idPermiso == idPermiso).FirstOrDefault();
            UsuarioBL usuarioBL = new UsuarioBL();
            permiso.usuarioList = usuarioBL.getUsuariosPorPermiso(permiso);
            this.PermisoSession = permiso;
            this.Session[Constantes.VAR_SESSION_CAMBIO_ASIGNACION_PERMISOS] = false;
        }


        public void changeUsuarios(String[] idsUsuario)
        {
            List<Usuario> usuarioList = (List<Usuario>)this.Session[Constantes.VAR_SESSION_USUARIO_LISTA];
            List<Guid> idUsuarioList = new List<Guid>();
            foreach (String idUsuarioString in idsUsuario)
            {
                idUsuarioList.Add(Guid.Parse(idUsuarioString));
            }

            this.PermisoSession.usuarioList = usuarioList.Where(u => idUsuarioList.Contains(u.idUsuario)).ToList();

            this.Session[Constantes.VAR_SESSION_CAMBIO_ASIGNACION_PERMISOS] = true;
        }

        public bool consultaSiExistenCambios()
        {
            if (this.Session[Constantes.VAR_SESSION_CAMBIO_ASIGNACION_PERMISOS] == null)
                return false;
            else
                return (bool)this.Session[Constantes.VAR_SESSION_CAMBIO_ASIGNACION_PERMISOS];
        }

        public void deshacerCambiosUsuario()
        {
            UsuarioBL usuarioBL = new UsuarioBL();
            this.PermisoSession.usuarioList = usuarioBL.getUsuariosPorPermiso(this.PermisoSession);
            this.Session[Constantes.VAR_SESSION_CAMBIO_ASIGNACION_PERMISOS] = false;
        }

        public void UpdatePermiso()
        {
            try
            {
                Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
                UsuarioBL usuarioBL = new UsuarioBL();
                usuarioBL.setPermiso(this.PermisoSession.usuarioList, this.PermisoSession, usuario);
                this.Session[Constantes.VAR_SESSION_CAMBIO_ASIGNACION_PERMISOS] = false;
            }
            catch (Exception e)
            {
                logger.Error(e, agregarUsuarioAlMensaje(e.Message));
                throw e;
            }
        }



        [HttpGet]
        public ActionResult List()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            PermisoBL bl = new PermisoBL();

            if (!usuario.administraPermisos)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaPermisos;

            if (this.Session[Constantes.VAR_SESSION_PERMISO_BUSQUEDA] == null)
            {
                instanciarPermisoBusqueda();
            }


            Permiso objSearch = (Permiso)this.Session[Constantes.VAR_SESSION_PERMISO_BUSQUEDA];
            List<Permiso> permisos = bl.getPermisos();

            ViewBag.pagina = (int)Constantes.paginas.BusquedaPermisos;
            ViewBag.usuario = objSearch;
            ViewBag.permisos = permisos;

            return View();
        }


        public ActionResult Editar(int? idPermiso = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoPermisos;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.administraPermisos)
            {
                return RedirectToAction("List", "Permiso");
            }


            if (this.Session[Constantes.VAR_SESSION_PERMISO_MANTENEDOR] == null && idPermiso == null)
            {
                instanciarPermiso();
            }

            Permiso obj = (Permiso)this.Session[Constantes.VAR_SESSION_PERMISO_MANTENEDOR];

            if (idPermiso != null)
            {
                PermisoBL bL = new PermisoBL();
                obj = bL.getPermiso(idPermiso.Value);
                obj.IdUsuarioRegistro = usuario.idUsuario;
                obj.usuario = usuario;

                this.Session[Constantes.VAR_SESSION_PERMISO_MANTENEDOR] = obj;
            }


            ViewBag.permiso = obj;

            return View();

        }
    }
}