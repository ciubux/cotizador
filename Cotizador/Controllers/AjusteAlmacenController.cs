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
    public class AjusteAlmacenController : ParentController
    {
        private GuiaRemision AjusteAlmacenSession
        {
            get
            {

                GuiaRemision obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaAjusteAlmacen: obj = (GuiaRemision)this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN_BUSQUEDA]; break;
                    case Constantes.paginas.RegistrarAjusteAlmacen: obj = (GuiaRemision)this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaAjusteAlmacen: this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN_BUSQUEDA] = value; break;
                    case Constantes.paginas.RegistrarAjusteAlmacen: this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = value; break;
                }
            }
        }


        [HttpGet]
        public ActionResult List()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.registraAjusteStock)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaAjusteAlmacen;

            if (this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN_BUSQUEDA] == null)
            {
                instanciarAjusteAlmacenBusqueda();
            }

            MotivoAjusteAlmacen mot = new MotivoAjusteAlmacen();
            mot.Estado = 1;
            MotivoAjusteAlmacenBL maBl = new MotivoAjusteAlmacenBL();
            List<MotivoAjusteAlmacen> motivos = new List<MotivoAjusteAlmacen>();
            motivos = maBl.getMotivos(mot);


            GuiaRemision objSearch = (GuiaRemision)this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN_BUSQUEDA];

            ViewBag.pagina = (int)Constantes.paginas.BusquedaAjusteAlmacen;
            ViewBag.objSearch = objSearch;
            ViewBag.motivos = motivos;

            return View();
        }


        public String SearchList()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaAjusteAlmacen;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            GuiaRemision obj = (GuiaRemision)this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN_BUSQUEDA];

            MovimientoAlmacenBL bL = new MovimientoAlmacenBL();
            List<GuiaRemision> list = bL.BuscarAjustesAlmacen(obj);

            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
        }

        public String Show()
        {
            Guid idAjuste = Guid.Parse(Request["idAjuste"].ToString());
            MovimientoAlmacenBL bL = new MovimientoAlmacenBL();
            GuiaRemision obj = new GuiaRemision();
            obj.idMovimientoAlmacen = idAjuste;
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            obj = bL.GetAjusteAlmacen(obj);

            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN_VER] = obj;
            return resultado;
        }

        public ActionResult Editar(Guid? idAjusteAlmacen = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.RegistrarAjusteAlmacen;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (usuario == null || !usuario.registraAjusteStock)
            {
                return RedirectToAction("Login", "Account");
            }

            MotivoAjusteAlmacen mot = new MotivoAjusteAlmacen();
            mot.Estado = 1;

            MotivoAjusteAlmacenBL maBl = new MotivoAjusteAlmacenBL();
            List<MotivoAjusteAlmacen> motivos = new List<MotivoAjusteAlmacen>();
            motivos = maBl.getMotivos(mot);


            if (this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] == null && idAjusteAlmacen == null)
            {
                instanciarAjusteAlmacen();
            }

            GuiaRemision obj = (GuiaRemision)this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN];

            if (idAjusteAlmacen != null)
            {
                MovimientoAlmacenBL bl = new MovimientoAlmacenBL();
                //obj = bl.GetGuiaRemision(idAjusteAlmacen);
                obj.IdUsuarioRegistro = usuario.idUsuario;
                obj.usuario = usuario;

                this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;
            }

            this.Session["familia"] = null;
            this.Session["proveedor"] = null;

            ViewBag.ajuste = obj;
            ViewBag.motivos = motivos;
            ViewBag.pagina = (int)this.Session[Constantes.VAR_SESSION_PAGINA];
           

            return View();
        }

        private void instanciarAjusteAlmacen()
        {
            GuiaRemision obj = new GuiaRemision();
            obj.idMovimientoAlmacen = Guid.Empty;
            obj.Estado = 1;
            obj.motivoAjuste = new MotivoAjusteAlmacen();
            obj.motivoTraslado = GuiaRemision.motivosTraslado.AjusteAlmacen;
            obj.ajusteAprobado = 0;
            obj.ciudadOrigen = new Ciudad();
            obj.ciudadOrigen.idCiudad = Guid.Empty;
            obj.documentoDetalle = new List<DocumentoDetalle>();

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;
        }

        private void instanciarAjusteAlmacenBusqueda()
        {
            GuiaRemision obj = new GuiaRemision();
            obj.idMovimientoAlmacen = Guid.Empty;
            obj.Estado = 1;
            obj.motivoAjuste = new MotivoAjusteAlmacen();
            obj.motivoAjuste.idMotivoAjusteAlmacen = 0;
            obj.motivoTraslado = GuiaRemision.motivosTraslado.AjusteAlmacen;
            obj.fechaEmisionDesde = DateTime.Now.AddDays(-14);
            obj.fechaEmisionHasta = DateTime.Now;

            obj.tipoMovimiento = MovimientoAlmacen.tiposMovimiento.Salida;
            obj.ajusteAprobado = -1;
            obj.ciudadOrigen = new Ciudad();
            obj.ciudadOrigen.idCiudad = Guid.Empty;
            obj.documentoDetalle = new List<DocumentoDetalle>();

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN_BUSQUEDA] = obj;
        }


        public String GetProducto()
        {
            //Se recupera el producto y se guarda en Session
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            this.Session["idProducto"] = idProducto.ToString();

            //Para recuperar el producto se envia si la sede seleccionada es provincia o no
            ProductoBL bl = new ProductoBL();
           
            Producto producto = bl.getProducto(idProducto, false, false, Guid.Empty, false);


            String jsonProductoPresentacion = JsonConvert.SerializeObject(producto.ProductoPresentacionList);


            var v = new
            {
                idProducto = producto.idProducto,
                sku = producto.sku,
                //descontinuado = producto.descontinuado,
                //motivoRestriccion = producto.motivoRestriccion,
                image = Convert.ToBase64String(producto.image),
                nombre = producto.descripcion,
                unidad = producto.unidad,
                productoPresentacionList = producto.ProductoPresentacionList
            };


            String resultado = JsonConvert.SerializeObject(v);
            return resultado;
        }

        public String ChangeIdCiudad()
        {
            GuiaRemision obj = this.AjusteAlmacenSession;

            // cotizacion.grupo = new GrupoCliente();
            Guid idCiudad = Guid.Empty;
            if (this.Request.Params["idCiudad"] != null && !this.Request.Params["idCiudad"].Equals(""))
            {
                idCiudad = Guid.Parse(this.Request.Params["idCiudad"]);
            }
            //Para realizar el cambio de ciudad ningun producto debe estar agregado
            
            CiudadBL ciudadBL = new CiudadBL();
            Ciudad ciudadNueva = ciudadBL.getCiudad(idCiudad);
            obj.ciudadOrigen = ciudadNueva;
            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;
            return "{\"idCiudad\": \"" + idCiudad + "\"}";
            
        }

        public String ChangeIdMotivoAjuste()
        {
            GuiaRemision obj = this.AjusteAlmacenSession;

            // cotizacion.grupo = new GrupoCliente();
            int idMotivoAjuste = 0;
            if (this.Request.Params["idMotivoAjuste"] != null && !this.Request.Params["idMotivoAjuste"].Equals(""))
            {
                idMotivoAjuste = int.Parse(this.Request.Params["idMotivoAjuste"].ToString());
            }
            //Para realizar el cambio de ciudad ningun producto debe estar agregado
            if (obj.documentoDetalle != null && obj.documentoDetalle.Count > 0)
            {
                // throw new Exception("No se puede cambiar de ciudad");
                return "No se puede cambiar de ciudad";
            }
            else
            {
                MotivoAjusteAlmacen mot = new MotivoAjusteAlmacen();
                mot.idMotivoAjusteAlmacen = idMotivoAjuste;
                obj.motivoAjuste = mot;
                this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;
                return "{\"idMotivoAjuste\": \"" + idMotivoAjuste + "\"}";
            }

        }


        public String ChangeTipoAjuste()
        {
            GuiaRemision obj = this.AjusteAlmacenSession;
            obj.tipoMovimiento = (MovimientoAlmacen.tiposMovimiento) Request["tipoMovimiento"].ToCharArray()[0];
            
            // cotizacion.grupo = new GrupoCliente();
            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;
            return "{\"tipoMovimiento\": \"" + obj.tipoMovimiento.ToString() + "\"}";
        }


        public void ChangeInputString()
        {
            GuiaRemision obj = this.AjusteAlmacenSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;
        }

        public void ChangeInputDate()
        {
            GuiaRemision obj = this.AjusteAlmacenSession;

            string fechaParam = Request.Params["valor"].ToString();

            if (!fechaParam.Trim().Equals(""))
            {
                String[] fecha = fechaParam.Split('/');   
                PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
                propertyInfo.SetValue(obj, new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0])));
                this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;
            }
        }

        public String AddProducto()
        {
            GuiaRemision obj = (GuiaRemision)this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN];
            Guid idProducto = Guid.Parse(this.Session["idProducto"].ToString());
            DocumentoDetalle detalle = obj.documentoDetalle.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
            if (detalle != null)
            {
                throw new System.Exception("Producto ya se encuentra en la lista");
            }
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            detalle = new DocumentoDetalle();
            ProductoBL productoBL = new ProductoBL();
            
            Producto producto = productoBL.getProducto(idProducto, false, false, Guid.Empty);
            detalle.producto = producto;

            detalle.cantidad = Int32.Parse(Request["cantidad"].ToString());
            detalle.esPrecioAlternativo = Int16.Parse(Request["esPrecioAlternativo"].ToString()) == 1;
            int idProductoPresentacion = Int16.Parse(Request["idProductoPresentacion"].ToString());


            detalle.observacion = Request["observacion"].ToString();

            detalle.unidad = detalle.producto.unidad;
            //si esPrecioAlternativo  se mostrará la unidad alternativa

            if (detalle.esPrecioAlternativo)
            {
                detalle.ProductoPresentacion = producto.getProductoPresentacion(idProductoPresentacion);

                ProductoPresentacion productoPresentacion = producto.getProductoPresentacion(idProductoPresentacion);

                detalle.unidad = productoPresentacion.Presentacion;
            }
            else
            {
            }

            obj.documentoDetalle.Add(detalle);

            var nombreProducto = detalle.producto.sku + " - " + detalle.producto.descripcion;            

            var v = new
            {
                idProducto = detalle.producto.idProducto,
                codigoProducto = detalle.producto.sku,
                motivoRestriccion = detalle.producto.motivoRestriccion,
                nombreProducto = nombreProducto,
                unidad = detalle.unidad,
                observacion = detalle.observacion,
                idProductoPresentacion = detalle.esPrecioAlternativo ? detalle.ProductoPresentacion.IdProductoPresentacion : 0
            };


            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;


            String resultado = JsonConvert.SerializeObject(v);
            return resultado;
        }

        public void RemoverProducto()
        {
            GuiaRemision obj = (GuiaRemision)this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN];
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            DocumentoDetalle detalle = obj.documentoDetalle.Where(p => p.producto.idProducto == idProducto).FirstOrDefault();
            if (detalle != null)
            {
                obj.documentoDetalle.Remove(detalle);
                this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;
            }

        }

        [HttpPost]
        public void AprobarAjusteAlmacen()
        {
            GuiaRemision obj = new GuiaRemision();
            obj.idMovimientoAlmacen = Guid.Parse(Request["idAjusteAlmacen"].ToString());
            obj.ajusteAprobado = 1;
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.usuario = usuario;

            MovimientoAlmacenBL bl = new MovimientoAlmacenBL();
            bl.UpdateAjusteEstadoAprobado(obj);
        }


        //[HttpGet]
        //public ActionResult ExportLastSearchExcel()
        //{
        //    List<Rol> list = (List<Rol>)this.Session[Constantes.VAR_SESSION_ROL_LISTA];

        //    RolSearch excel = new RolSearch();
        //    return excel.generateExcel(list, (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO]);
        //}



        // GET: 
        [HttpGet]
        public ActionResult Index()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaAjusteAlmacen;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.registraAjusteStock)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();

        }

        public String ConsultarSiExisteAjusteAlmacen()
        {
            Guid idAjusteAlmacen = Guid.Parse(Request["idAjusteAlmacen"].ToString());
            MovimientoAlmacenBL bL = new MovimientoAlmacenBL();
            GuiaRemision obj = new GuiaRemision();
            obj.idMovimientoAlmacen = idAjusteAlmacen;
            obj = bL.GetAjusteAlmacen(obj);
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            
            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN_VER] = obj;

            obj = (GuiaRemision)this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN];
            if (obj == null)
                return "{\"existe\":\"false\"}";
            else
                return "{\"existe\":\"true\"}";
        }


        public void iniciarEdicionAjusteAlmacen()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            GuiaRemision obj = (GuiaRemision)this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN_VER];
            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;
        }

        public String Create()
        {
            MovimientoAlmacenBL bL = new MovimientoAlmacenBL();
            GuiaRemision obj = (GuiaRemision)this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN];

            bL.InsertAjusteAlmacen(obj);

            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public String Update()
        {
            MovimientoAlmacenBL bL = new MovimientoAlmacenBL();
            GuiaRemision obj = (GuiaRemision)this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN];

            bL.InsertAjusteAlmacen(obj);

            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }

        

        public void ChangeInputInt()
        {
            GuiaRemision obj = this.AjusteAlmacenSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;
        }

        public void ChangeInputDecimal()
        {
            GuiaRemision obj = this.AjusteAlmacenSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Decimal.Parse(this.Request.Params["valor"]));
            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;
        }
        public void ChangeInputBoolean()
        {
            GuiaRemision obj = this.AjusteAlmacenSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;
        }

      
        public ActionResult CancelarCreacion()
        {
            GuiaRemision obj = (GuiaRemision)this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN];
            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = null;
            return RedirectToAction("List", "AjusteAlmacen");
        }
       
    }
}