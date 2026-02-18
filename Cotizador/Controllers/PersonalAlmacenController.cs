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
    public class PersonalAlmacenController : ParentController
    {
        [HttpGet]
        public ActionResult List()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            
            if (usuario != null || !usuario.modificaMaestroPersonalAlmacen)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaPersonalAlmacen;

            if (this.Session[Constantes.VAR_SESSION_PERSONALALMACEN_BUSQUEDA] == null)
            {
                instanciarPersonalBusqueda();
            }

            PersonalAlmacen objSearch = (PersonalAlmacen)this.Session[Constantes.VAR_SESSION_PERSONALALMACEN_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaPersonalAlmacen;
            ViewBag.personal = objSearch;

            return View();
        }

        public String SearchList()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPersonalAlmacen;

            PersonalAlmacen obj = (PersonalAlmacen)this.Session[Constantes.VAR_SESSION_PERSONALALMACEN_BUSQUEDA];

            PersonalAlmacenBL bL = new PersonalAlmacenBL();
            List<PersonalAlmacen> list = bL.getPersonalesAlmacen(obj);

            this.Session[Constantes.VAR_SESSION_PERSONALALMACEN_LISTA] = list;
            return JsonConvert.SerializeObject(list);
        }

        public String SearchAjax()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            Guid idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            string tipo = this.Request.Params["tipo"].ToString();
            PersonalAlmacen obj = new PersonalAlmacen();
            obj.Estado = 1;
            obj.tipo = tipo;
            obj.sedePrincipal = new Ciudad();
            obj.sedePrincipal.idCiudad = idCiudad;

            PersonalAlmacenBL bL = new PersonalAlmacenBL();
            List<PersonalAlmacen> list = bL.getPersonalesAlmacen(obj);

            return JsonConvert.SerializeObject(list);
        }


        private void instanciarPersonalBusqueda()
        {
            PersonalAlmacen obj = new PersonalAlmacen();
            obj.idPersonalAlmacen = 0;
            obj.Estado = 1;
            obj.nombres = String.Empty;
            obj.apellidoPaterno = String.Empty;
            obj.sedePrincipal = new Ciudad();

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;

            this.Session[Constantes.VAR_SESSION_PERSONALALMACEN_BUSQUEDA] = obj;
        }

        public String Create()
        {
            PersonalAlmacenBL bL = new PersonalAlmacenBL();
            PersonalAlmacen obj = new PersonalAlmacen();

            obj.nombres = Request["nombres"].ToString();
            obj.apellidoPaterno = Request["apellidoPaterno"].ToString();
            obj.apellidoMaterno = Request["apellidoMaterno"].ToString();
            obj.nroDocumento = Request["nroDocumento"].ToString();
            obj.brevete = Request["brevete"].ToString();
            obj.tipo = Request["tipo"].ToString();
            obj.Estado = 1;
            obj.sedePrincipal = new Ciudad { idCiudad = Guid.Parse(Request["idSede"].ToString()) };
            obj.IdUsuarioRegistro = Logueado.idUsuario;

            obj = bL.insertPersonalAlmacen(obj);
            return JsonConvert.SerializeObject(obj);
        }

        public String Update()
        {
            PersonalAlmacenBL bL = new PersonalAlmacenBL();
            PersonalAlmacen obj = new PersonalAlmacen();

            obj.idPersonalAlmacen = int.Parse(Request["idPersonalAlmacen"].ToString());
            obj.nombres = Request["nombres"].ToString();
            obj.apellidoPaterno = Request["apellidoPaterno"].ToString();
            obj.apellidoMaterno = Request["apellidoMaterno"].ToString();
            obj.nroDocumento = Request["nroDocumento"].ToString();
            obj.brevete = Request["brevete"].ToString();
            obj.tipo = Request["tipo"].ToString();

            if (!string.IsNullOrEmpty(Request["idSede"]))
            {
                obj.sedePrincipal = new Ciudad { idCiudad = Guid.Parse(Request["idSede"]) };
            }

            obj.Estado = 1;
            obj.IdUsuarioRegistro = Logueado.idUsuario;

            if (obj.idPersonalAlmacen == 0)
                obj = bL.insertPersonalAlmacen(obj);
            else
                obj = bL.updatePersonalAlmacen(obj);

            return JsonConvert.SerializeObject(obj);
        }
    }
}