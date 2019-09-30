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
    public class PeriodoSolicitudController : ParentController
    {
        [HttpGet]
        public ActionResult List()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaPeriodosSolicitud;
            
            if (this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD_BUSQUEDA] == null)
            {
                instanciarPeriodoSolicitudBusqueda();
            }
            

            PeriodoSolicitud objSearch = (PeriodoSolicitud)this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaPeriodosSolicitud;
            ViewBag.periodoSolicitud = objSearch;

            return View();
        }

        public String SearchList()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPeriodosSolicitud;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            PeriodoSolicitud obj = (PeriodoSolicitud)this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD_BUSQUEDA];

            PeriodoSolicitudBL bL = new PeriodoSolicitudBL();
            List<PeriodoSolicitud> list = bL.getPeriodosSolicitud(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);

        }

        public String Show()
        {
            Usuario usuerio = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Guid idPeriodoSolicitud = Guid.Parse(Request["idPeriodoSolicitud"].ToString());
            PeriodoSolicitudBL bL = new PeriodoSolicitudBL();
            PeriodoSolicitud obj = bL.getPeriodoSolicitud(idPeriodoSolicitud, usuerio.idUsuario);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado =  JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD_VER] = obj;
            return resultado;
        }

        public string EliminarPeriodoSolicitud ()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaPeriodoSolicitud)
            {
                return "";
            }

            Guid idPeriodoSolicitud = Guid.Parse(Request["idPeriodoSolicitud"].ToString());

            PeriodoSolicitudBL bL = new PeriodoSolicitudBL();
            PeriodoSolicitud obj = bL.getPeriodoSolicitud(idPeriodoSolicitud, usuario.idUsuario);
            obj.usuario = usuario;

            obj.nombre = obj.nombre + " " + DateTime.Now.ToFileTime().ToString();
            obj.Estado = 0;
            bL.updatePeriodoSolicitud(obj);

            int success = 1;
            string message = "";

            try
            {
                message = "Se eliminó el periodo";
            } catch(Exception ex)
            {
                message = "Error de sistema"; 
            }

            return "{\"success\": " + success.ToString() + ", \"message\": \"" + message + "\"}";
        }

        public ActionResult Editar(Guid? idPeriodoSolicitud = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoPeriodoSolicitud;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaPeriodoSolicitud)
            {
                return RedirectToAction("Login", "Account");
            }



            if (this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD] == null && idPeriodoSolicitud == null)
            {
                instanciarPeriodoSolicitud();
            }

            PeriodoSolicitud obj = (PeriodoSolicitud)this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD];
            
            if (idPeriodoSolicitud != null)
            {
                PeriodoSolicitudBL bL = new PeriodoSolicitudBL();
                obj = bL.getPeriodoSolicitud(idPeriodoSolicitud.Value, usuario.IdUsuarioRegistro);
                obj.IdUsuarioRegistro = usuario.idUsuario;
                obj.usuario = usuario;
                
                this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD] = obj;
            }
            

            ViewBag.periodoSolicitud = obj;
            return View();

        }

        private void instanciarPeriodoSolicitud()
        {
            PeriodoSolicitud obj = new PeriodoSolicitud();
            obj.idPeriodoSolicitud = Guid.Empty;
            obj.Estado = 2;
            obj.nombre = String.Empty;
            obj.fechaInicio = DateTime.Now;
            obj.fechaFin = DateTime.Now;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD] = obj;
        }

        private void instanciarPeriodoSolicitudBusqueda()
        {
            PeriodoSolicitud obj = new PeriodoSolicitud();
            obj.idPeriodoSolicitud = Guid.Empty;
            obj.Estado = -1;
            obj.nombre = String.Empty;
            obj.fechaInicio = DateTime.Now;
            obj.fechaFin = DateTime.Now;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD_BUSQUEDA] = obj;
        }


        //[HttpGet]
        //public ActionResult ExportLastSearchExcel()
        //{
        //    List<SubDistribuidor> list = (List<SubDistribuidor>)this.Session[Constantes.VAR_SESSION_SUBDISTRIBUIDOR_LISTA];

        //    SubDistribuidorSearch excel = new SubDistribuidorSearch();
        //    return excel.generateExcel(list, (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO]);
        //}

        

        // GET: SubDistribuidor
        [HttpGet]
        public ActionResult Index()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaPeriodoSolicitud)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();

        }

        public String ConsultarSiExistePeriodoSolicitud()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Guid idPeriodoSolicitud = Guid.Parse(Request["idPeriodoSolicitud"].ToString());
            PeriodoSolicitudBL bL = new PeriodoSolicitudBL();
            PeriodoSolicitud obj = bL.getPeriodoSolicitud(idPeriodoSolicitud, usuario.idUsuario);
            obj.usuario = usuario;
            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD_VER] = obj;

            obj = (PeriodoSolicitud)this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD];
            if (obj == null)
                return "{\"existe\":\"false\",\"idPeriodoSolicitud\":\"" + Guid.Empty + "\"}";
            else
                return "{\"existe\":\"true\",\"idPeriodoSolicitud\":\"" + obj.idPeriodoSolicitud + "\"}";
        }


        public void iniciarEdicionPeriodoSolicitud()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            PeriodoSolicitud obj = (PeriodoSolicitud)this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD_VER];
            this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD] = obj;
        }

        public String Create()
        {
            PeriodoSolicitudBL bL = new PeriodoSolicitudBL();
            PeriodoSolicitud obj = (PeriodoSolicitud)this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD];

            obj = bL.insertPeriodoSolicitud(obj);
            this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public String Update()
        {
            PeriodoSolicitudBL bL = new PeriodoSolicitudBL();
            PeriodoSolicitud obj = (PeriodoSolicitud)this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD];

            if (obj.idPeriodoSolicitud == Guid.Empty)
            {
                obj = bL.insertPeriodoSolicitud(obj);
                this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD] = null;
            }
            else
            {
                obj = bL.updatePeriodoSolicitud(obj);
                this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD] = null;
            }
            String resultado = JsonConvert.SerializeObject(obj);
            //this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            return resultado;
        }
        
        public void ChangeInputString()
        {
            PeriodoSolicitud obj = (PeriodoSolicitud) this.PeriodoSolicitudSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.PeriodoSolicitudSession = obj;
        }

        public void ChangeInputInt()
        {
            PeriodoSolicitud obj = (PeriodoSolicitud)this.PeriodoSolicitudSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.PeriodoSolicitudSession = obj;
        }

        public void ChangeInputDate()
        {
            PeriodoSolicitud obj = (PeriodoSolicitud)this.PeriodoSolicitudSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            
            String[] fechai = this.Request.Params["valor"].Split('/');
            DateTime fecha = new DateTime(Int32.Parse(fechai[2]), Int32.Parse(fechai[1]), Int32.Parse(fechai[0]), 0, 0, 0);

            propertyInfo.SetValue(obj, fecha);
            this.PeriodoSolicitudSession = obj;
        }

        public void ChangeInputBoolean()
        {
            PeriodoSolicitud obj = (PeriodoSolicitud)this.PeriodoSolicitudSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.PeriodoSolicitudSession = obj;
        }


        private PeriodoSolicitud PeriodoSolicitudSession
        {
            get
            {
                PeriodoSolicitud obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPeriodosSolicitud: obj = (PeriodoSolicitud)this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoPeriodoSolicitud: obj = (PeriodoSolicitud)this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPeriodosSolicitud: this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoPeriodoSolicitud: this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD] = value; break;
                }
            }
        }

        public ActionResult CancelarCreacionPeriodoSolicitud()
        {
            this.Session[Constantes.VAR_SESSION_PERIODOSOLICITUD] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("List", "PeriodoSolicitud");
        }


        public ActionResult GetPeriodosView(string periodoSelectId, string selectedValue = null, bool excluirPeriodosConRequerimientos = false)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            PeriodoSolicitudBL periodoSolicitudBL = new PeriodoSolicitudBL();
            PeriodoSolicitud periodoSolicitud = new PeriodoSolicitud { usuario = usuario };
            periodoSolicitud.IdUsuarioRegistro = usuario.idUsuario;

            List<PeriodoSolicitud> periodoList = periodoSolicitudBL.getPeriodosSolicitudVigentes(periodoSolicitud, false);
            var model = new PeriodoViewModels
            {
                Data = periodoList,
                PeriodoSelectId = periodoSelectId,
                SelectedValue = selectedValue
            };

            return PartialView("_Periodo", model);
        }
    }


}