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
    public class TipoClienteContactoController : Controller
    {

        [HttpGet]
        public ActionResult Lista()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaParametro)
            {
                return RedirectToAction("Login", "Account");
            }


            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoClienteContactoTipo;
            ParametroBL parametrobl = new ParametroBL();
            int maxTipos = parametrobl.getParametroInt("MAX_TIPOS_CONTACTO_CLIENTE");

            ClienteContactoTipoBL bL = new ClienteContactoTipoBL();
            List<ClienteContactoTipo> list = bL.getTipos(-1);

            ViewBag.pagina = (int)Constantes.paginas.MantenimientoClienteContactoTipo;

            ViewBag.tipos = list;
            ViewBag.maxTipos = maxTipos;

            return View();
        }


        public String SearchList()
        {
            //obj.user = (Parametro)this.Session[Constantes.VAR_SESSION_USUARIO];
            ClienteContactoTipoBL bL = new ClienteContactoTipoBL();
            List<ClienteContactoTipo> list = bL.getTipos(-1);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_CLIENTE_CONTACTO_TIPO_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
        }

        public String Update()
        {
            ClienteContactoTipo obj = new ClienteContactoTipo();
            ClienteContactoTipoBL bL = new ClienteContactoTipoBL();
            
            obj.idClienteContactoTipo = Guid.Parse(Request["idTipo"].ToString());
            obj.nombre = Request["nombre"].ToString();
            obj.descripcion = Request["descripcion"].ToString();
            obj.Estado = 1;

            Usuario user = new Usuario();
            user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = user.idUsuario;
            bL.update(obj);

            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }

        public String Insert()
        {
            ClienteContactoTipo obj = new ClienteContactoTipo();
            ClienteContactoTipoBL bL = new ClienteContactoTipoBL();

            obj.nombre = Request["nombre"].ToString();
            obj.descripcion = Request["descripcion"].ToString();
            obj.Estado = 1;

            Usuario user = new Usuario();
            user = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = user.idUsuario;
            bL.insert(obj);

            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public void CleanBusqueda()
        {
        }
    }
}