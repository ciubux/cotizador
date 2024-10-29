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
using System.Linq.Expressions;

namespace Cotizador.Controllers
{
    public class EmpresaDescuentoController : ParentController
    {
        [HttpGet]
        public ActionResult List()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaMaestroEmpresaDescuento)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaEmpresaDescuento;
            
            if (this.Session[Constantes.VAR_SESSION_EMPRESADESCUENTO_BUSQUEDA] == null)
            {
                instanciarEmpresaDescuentoBusqueda();
            }
            
            EmpresaDescuento objSearch = (EmpresaDescuento)this.Session[Constantes.VAR_SESSION_EMPRESADESCUENTO_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaEmpresaDescuento;
            ViewBag.empresaDescuento = objSearch;

            return View();
        }

        public String SearchList()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaEmpresaDescuento;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            EmpresaDescuento obj = (EmpresaDescuento)this.Session[Constantes.VAR_SESSION_EMPRESADESCUENTO_BUSQUEDA];

            EmpresaDescuentoBL bL = new EmpresaDescuentoBL();
            List<EmpresaDescuento> list = bL.Listar(usuario.idEmpresa, usuario.idUsuario, obj.ciudad.idCiudad, obj.Estado);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_EMPRESADESCUENTO_LISTA] = list;

            List<String> tiposDescuento = new List<String>();

            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(new
            {
                lista = list,
                tiposDescuento = new string[] { EmpresaDescuento.TIPO_DESCUENTO_MONTO, EmpresaDescuento.TIPO_DESCUENTO_PORCENTAJE },
                ciudades = Logueado.sedesMP
            });
        }


        private void instanciarEmpresaDescuentoBusqueda()
        {
            EmpresaDescuento obj = new EmpresaDescuento();
            obj.empresa = new Empresa();
            obj.empresa.idEmpresa = this.Logueado.idEmpresa;
            obj.Estado = 1;

            obj.ciudad = new Ciudad();
            obj.ciudad.idCiudad = Guid.Empty;

            obj.IdUsuarioRegistro = this.Logueado.idUsuario;
            obj.usuario = this.Logueado;

            this.Session[Constantes.VAR_SESSION_EMPRESADESCUENTO_BUSQUEDA] = obj;
        }

        

        // GET: EmpresaDescuento
        [HttpGet]
        public ActionResult Index()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaMaestroEmpresaDescuento)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();

        }
        /*
        public String Create()
        {
            EmpresaDescuentoBL bL = new EmpresaDescuentoBL();
            EmpresaDescuento obj = new EmpresaDescuento();

            obj.codigo = Request["codigo"].ToString();
            obj.nombreUsual = Request["nombreUsual"].ToString();
            obj.IdUsuarioRegistro = Logueado.idUsuario; 

            obj = bL.Insert(obj);
            this.Session[Constantes.VAR_SESSION_EMPRESADESCUENTO] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }

        */

        private EmpresaDescuento EmpresaDescuentoSession
        {
            get
            {
                EmpresaDescuento obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaEmpresaDescuento: obj = (EmpresaDescuento)this.Session[Constantes.VAR_SESSION_EMPRESADESCUENTO_BUSQUEDA]; break;
                    //case Constantes.paginas.MantenimientoEmpresaDescuento: obj = (EmpresaDescuento)this.Session[Constantes.VAR_SESSION_EMPRESADESCUENTO]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaEmpresaDescuento: this.Session[Constantes.VAR_SESSION_EMPRESADESCUENTO_BUSQUEDA] = value; break;
                    //case Constantes.paginas.MantenimientoEmpresaDescuento: this.Session[Constantes.VAR_SESSION_EMPRESADESCUENTO] = value; break;
                }
            }
        }
    }
}