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
    public class PromocionController : ParentController
    {
        public ActionResult GetPromociones(string selectId, string selectedValue = null)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Promocion obj = new Promocion();
            obj.Estado = 1;

            PromocionBL bL = new PromocionBL();
            List<Promocion> list = bL.getPromociones(obj);

            var model = new PromocionViewModels
            {
                Data = list,
                SelectId = selectId,
                SelectedValue = selectedValue
            };

            return PartialView("_Promocion", model);
        }


        [HttpGet]
        public ActionResult List()
        {

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaPromociones;
            
            if (this.Session[Constantes.VAR_SESSION_PROMOCION_BUSQUEDA] == null)
            {
                instanciarPromocionBusqueda();
            }
            

            Promocion objSearch = (Promocion)this.Session[Constantes.VAR_SESSION_PROMOCION_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaPromociones;
            ViewBag.promocion = objSearch;

            return View();
        }

        public String SearchList()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPromociones;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            Promocion obj = (Promocion)this.Session[Constantes.VAR_SESSION_PROMOCION_BUSQUEDA];

            PromocionBL bL = new PromocionBL();
            List<Promocion> list = bL.getPromociones(obj);
            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_PROMOCION_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);

        }

        public String Show()
        {
            Guid idPromocion = Guid.Parse(Request["idPromocion"].ToString());
            PromocionBL bL = new PromocionBL();
            Promocion obj = bL.getPromocion(idPromocion);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado =  JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_PROMOCION_VER] = obj;
            return resultado;
        }

        public String GetPromocion()
        {
            Guid idPromocion = Guid.Parse(Request["idPromocion"].ToString());
            PromocionBL bL = new PromocionBL();
            Promocion obj = bL.getPromocion(idPromocion);
            
            String resultado = JsonConvert.SerializeObject(obj);

            return resultado;
        }

        public ActionResult Editar(Guid? idPromocion = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.MantenimientoPromocion;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (!usuario.modificaPromocion)
            {
                return RedirectToAction("Login", "Account");
            }



            if (this.Session[Constantes.VAR_SESSION_PROMOCION] == null && idPromocion == null)
            {
                instanciarPromocion();
            }

            Promocion obj = (Promocion)this.Session[Constantes.VAR_SESSION_PROMOCION];
            
            if (idPromocion != null)
            {
                PromocionBL bL = new PromocionBL();
                obj = bL.getPromocion(idPromocion.Value);
                obj.IdUsuarioRegistro = usuario.idUsuario;
                obj.usuario = usuario;
                
                this.Session[Constantes.VAR_SESSION_PROMOCION] = obj;
            }
            

            ViewBag.promocion = obj;
            return View();

        }

        private void instanciarPromocion()
        {
            Promocion obj = new Promocion();
            obj.idPromocion = Guid.Empty;
            obj.Estado = 1;
            obj.titulo = String.Empty;
            obj.descripcion = String.Empty;
            obj.fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            obj.fechaFin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_PROMOCION] = obj;
        }

        private void instanciarPromocionBusqueda()
        {
            Promocion obj = new Promocion();
            obj.idPromocion = Guid.Empty;
            obj.Estado = 1;
            obj.codigo = String.Empty;
            obj.titulo = String.Empty;
            obj.descripcion = String.Empty;
            obj.fechaInicio = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            obj.fechaFin = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_PROMOCION_BUSQUEDA] = obj;
        }
        

        // GET: Promocion
        [HttpGet]
        public ActionResult Index()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaPromocion)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();

        }

        public String ConsultarSiExistePromocion()
        {
            Guid idPromocion = Guid.Parse(Request["idPromocion"].ToString());
            PromocionBL bL = new PromocionBL();
            Promocion obj = bL.getPromocion(idPromocion);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_PROMOCION_VER] = obj;

            obj = (Promocion)this.Session[Constantes.VAR_SESSION_PROMOCION];
            if (obj == null)
                return "{\"existe\":\"false\",\"idPromocion\":\"" + Guid.Empty + "\"}";
            else
                return "{\"existe\":\"true\",\"idPromocion\":\"" + obj.idPromocion + "\"}";
        }


        public void iniciarEdicionPromocion()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Promocion obj = (Promocion)this.Session[Constantes.VAR_SESSION_PROMOCION_VER];
            this.Session[Constantes.VAR_SESSION_PROMOCION] = obj;
        }

        public String Create()
        {
            PromocionBL bL = new PromocionBL();
            Promocion obj = (Promocion)this.Session[Constantes.VAR_SESSION_PROMOCION];

            obj = bL.insertar(obj);
            this.Session[Constantes.VAR_SESSION_PROMOCION] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public String Update()
        {
            PromocionBL bL = new PromocionBL();
            Promocion obj = (Promocion)this.Session[Constantes.VAR_SESSION_PROMOCION];

            if (obj.idPromocion == Guid.Empty)
            {
                obj = bL.insertar(obj);
                this.Session[Constantes.VAR_SESSION_PROMOCION] = null;
            }
            else
            {
                obj = bL.editar(obj);
                this.Session[Constantes.VAR_SESSION_PROMOCION] = null;
            }
            String resultado = JsonConvert.SerializeObject(obj);
            //this.Session[Constantes.VAR_SESSION_CLIENTE] = null;
            return resultado;
        }
        
        public void ChangeInputString()
        {
            Promocion obj = (Promocion) this.PromocionSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.PromocionSession = obj;
        }

        public void ChangeInputInt()
        {
            Promocion obj = (Promocion)this.PromocionSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.PromocionSession = obj;
        }

        public void ChangeInputDecimal()
        {
            Promocion obj = (Promocion)this.PromocionSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Decimal.Parse(this.Request.Params["valor"]));
            this.PromocionSession = obj;
        }
        public void ChangeInputBoolean()
        {
            Promocion obj = (Promocion)this.PromocionSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.PromocionSession = obj;
        }


        public void ChangeDate()
        {
            Promocion obj = (Promocion)this.PromocionSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            String[] ftmp = this.Request.Params["valor"].Split('/');
            propertyInfo.SetValue(obj, new DateTime(Int32.Parse(ftmp[2]), Int32.Parse(ftmp[1]), Int32.Parse(ftmp[0])));

            this.PromocionSession = obj;
        }



        private Promocion PromocionSession
        {
            get
            {
                Promocion obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPromociones: obj = (Promocion)this.Session[Constantes.VAR_SESSION_PROMOCION_BUSQUEDA]; break;
                    case Constantes.paginas.MantenimientoPromocion: obj = (Promocion)this.Session[Constantes.VAR_SESSION_PROMOCION]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPromociones: this.Session[Constantes.VAR_SESSION_PROMOCION_BUSQUEDA] = value; break;
                    case Constantes.paginas.MantenimientoPromocion: this.Session[Constantes.VAR_SESSION_PROMOCION] = value; break;
                }
            }
        }

        public ActionResult CancelarCreacionPromocion()
        {
            this.Session[Constantes.VAR_SESSION_PROMOCION] = null;
            UsuarioBL usuarioBL = new UsuarioBL();
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            //   usuarioBL.updateCotizacionSerializada(usuario, null);
            return RedirectToAction("List", "Promocion");

        }
    }


}