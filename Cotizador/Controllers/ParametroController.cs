using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLayer;
using Newtonsoft.Json;
using System.Reflection;

namespace Cotizador.Controllers
{
    public class ParametroController : Controller
    {

        [HttpGet]
        public ActionResult Lista()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaParametro)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaParametro;

            if (this.Session[Constantes.VAR_SESSION_PARAMETRO_BUSQUEDA] == null)
            {
                instanciarBusquedaParametro();
            }

            Parametro objSearch = (Parametro)this.Session[Constantes.VAR_SESSION_PARAMETRO_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaParametro;
            ViewBag.parametro = objSearch;

            return View();
        }

        private void instanciarBusquedaParametro()
        {
            Parametro obj = new Parametro();
            obj.descripcion = "";
            obj.codigo = "";
            obj.valor = "";

            this.Session[Constantes.VAR_SESSION_PARAMETRO_BUSQUEDA] = obj;
        }

        public String SearchList()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaParametro;
            Parametro obj = (Parametro)this.Session[Constantes.VAR_SESSION_PARAMETRO_BUSQUEDA];
            //obj.user = (Parametro)this.Session[Constantes.VAR_SESSION_USUARIO];
            ParametroBL bL = new ParametroBL();
            List<Parametro> list = bL.getListParametro(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_MENSAJE_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
        }

        public void ChangeInputString()
        {
            Parametro obj = this.ParametroSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.ParametroSession = obj;
        }

        public String Update()
        {
            Parametro obj = new Parametro();
            ParametroBL bL = new ParametroBL();
            //Parametro obj = (Parametro)this.Session[Constantes.VAR_SESSION_PARAMETRO];

            obj.idParametro = Guid.Parse(Request["idParametro"].ToString());
            obj.valor = Request["valActual"].ToString();
            obj.descripcion = Request["desActual"].ToString();

            Usuario user = new Usuario();
            user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            bL.modificarParametro(obj, user);
            this.Session[Constantes.VAR_SESSION_PARAMETRO] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public void CleanBusqueda()
        {
            instanciarBusquedaParametro();
        }

        private Parametro ParametroSession
        {
            get
            {
                Parametro obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaParametro: obj = (Parametro)this.Session[Constantes.VAR_SESSION_PARAMETRO_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoParametro: obj = (Parametro)this.Session[Constantes.VAR_SESSION_PARAMETRO]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaParametro: this.Session[Constantes.VAR_SESSION_PARAMETRO_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoParametro: this.Session[Constantes.VAR_SESSION_PARAMETRO] = value; break;
                }
            }
        }




    }
}