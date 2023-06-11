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
using Model.UTILES;
using NPOI.XWPF.UserModel;

namespace Cotizador.Controllers
{
    public class PrecioEspecialController : ParentController
    {
        private PrecioEspecialCabecera PrecioEspecialCabeceraSession
        {
            get
            {

                PrecioEspecialCabecera obj = null;
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPrecioEspecial: obj = (PrecioEspecialCabecera)this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL_BUSQUEDA]; break;
                    case Constantes.paginas.RegistrarPrecioEspecial: obj = (PrecioEspecialCabecera)this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL]; break;
                }
                return obj;
            }
            set
            {
                switch ((Constantes.paginas)this.Session[Constantes.VAR_SESSION_PAGINA])
                {
                    case Constantes.paginas.BusquedaPrecioEspecial: this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL_BUSQUEDA] = value; break;
                    case Constantes.paginas.RegistrarPrecioEspecial: this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL] = value; break;
                }
            }
        }


        [HttpGet]
        public ActionResult List()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (usuario == null || !usuario.modificaPreciosEspeciales)
            {
                return RedirectToAction("Login", "Account");
            }

            this.Session[Constantes.VAR_SESSION_PAGINA] = (int)Constantes.paginas.BusquedaPrecioEspecial;

            if (this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL_BUSQUEDA] == null)
            {
                instanciarPrecioEspecialBusqueda();
            }

            PrecioEspecialCabecera objSearch = this.PrecioEspecialCabeceraSession;

            PrecioEspecialBL bl = new PrecioEspecialBL();
            List<PrecioEspecialCabecera> lista = new List<PrecioEspecialCabecera>();

            ViewBag.pagina = (int)Constantes.paginas.BusquedaPrecioEspecial;
            ViewBag.objSearch = objSearch;

            return View();
        }


        public String SearchList()
        {
            //Se indica la página con la que se va a trabajar
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPrecioEspecial;
            //Se recupera el objeto cliente que contiene los criterios de Búsqueda de la session
            PrecioEspecialCabecera objSearch = (PrecioEspecialCabecera)this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL_BUSQUEDA];

            PrecioEspecialBL bL = new PrecioEspecialBL();
            List<PrecioEspecialCabecera> list = bL.BuscarCabeceras(objSearch);

            //Se coloca en session el resultado de la búsqueda
            this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL_LISTA] = list;
            //Se retorna la cantidad de elementos encontrados
            return JsonConvert.SerializeObject(list);
        }

        public String Show()
        {
            Guid idPrecioEspecial = Guid.Parse(Request["idPrecioEspecialCabecera"].ToString());
            PrecioEspecialBL bL = new PrecioEspecialBL();
            PrecioEspecialCabecera obj = new PrecioEspecialCabecera();
            obj.idPrecioEspecialCabecera = idPrecioEspecial;
            obj.usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            obj = bL.GetPrecioEspcial(obj.idPrecioEspecialCabecera, obj.usuario.idUsuario);

            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL_VER] = obj;
            return resultado;
        }

        [HttpGet]
        public ActionResult ExportListadoPrecios()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.modificaPreciosEspeciales)
            {
                return RedirectToAction("Login", "Account");
            }

            PrecioEspecialCabecera obj = (PrecioEspecialCabecera)this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL_VER];

            PrecioEspecialDetalleExcel excel = new PrecioEspecialDetalleExcel();

            return excel.generateExcel(obj);
        }

        [HttpGet]
        public ActionResult ExportListadoGlobal()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null || !usuario.modificaPreciosEspeciales)
            {
                return RedirectToAction("Login", "Account");
            }

            PrecioEspecialCabecera objSearch = (PrecioEspecialCabecera)this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL_BUSQUEDA];

            PrecioEspecialBL bL = new PrecioEspecialBL();
            List<PrecioEspecialCabecera> lista = bL.BuscarCabecerasDetalles(objSearch);


            PrecioEspecialCabecerasDetallesExcel excel = new PrecioEspecialCabecerasDetallesExcel();

            return excel.generateExcel(lista);
        }

        public ActionResult Editar(Guid? idPrecioEspecialCabecera = null)
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.RegistrarPrecioEspecial;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];

            if (usuario == null || !usuario.modificaPreciosEspeciales)
            {
                return RedirectToAction("Login", "Account");
            }


            if (this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL] == null && idPrecioEspecialCabecera == null)
            {
                instanciarPrecioEspecial();
            }

            PrecioEspecialCabecera obj = (PrecioEspecialCabecera)this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL];

            if (idPrecioEspecialCabecera != null)
            {
                PrecioEspecialBL bl = new PrecioEspecialBL();
                obj = bl.GetPrecioEspcial(idPrecioEspecialCabecera.Value, usuario.idUsuario);
                obj.IdUsuarioRegistro = usuario.idUsuario;
                obj.usuario = usuario;

                this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL] = obj;
            }


            ViewBag.obj = obj;
            ViewBag.pagina = (int)this.Session[Constantes.VAR_SESSION_PAGINA];
            if (this.Session["s_precioespecial_errorcargaprecios"] == null)
            {
                this.Session["s_precioespecial_errorcargaprecios"] = 0;
            }
               
            ViewBag.errorCargaPrecios = (int)this.Session["s_precioespecial_errorcargaprecios"];
            
            ViewBag.tieneDetallesConflicto = 0;
            ViewBag.tieneDetallesValidos = 0;

            foreach (PrecioEspecialDetalle det in obj.precios)
            {
                if(det.dataRelacionada != null && det.dataRelacionada.Count > 0)
                {
                    ViewBag.tieneDetallesConflicto = 1;
                } else
                {
                    ViewBag.tieneDetallesValidos = 1;
                }
            }
            
            
            return View();
        }

        private void instanciarPrecioEspecial()
        {
            PrecioEspecialCabecera obj = new PrecioEspecialCabecera();
            obj.idPrecioEspecialCabecera = Guid.Empty;
            obj.Estado = 1;
            obj.tipoNegociacion = "RUC";
            obj.codigo = "";
            obj.fechaInicio = DateTime.Now;
            obj.fechaFin = DateTime.Now;

            obj.precios = new List<PrecioEspecialDetalle>();
            obj.clienteSunat = new ClienteSunat();
            obj.clienteSunat.idClienteSunat = 0;

            obj.grupoCliente = new GrupoCliente();
            obj.grupoCliente.idGrupoCliente = 0;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL] = obj;
        }

        private void instanciarPrecioEspecialBusqueda()
        {
            PrecioEspecialCabecera obj = new PrecioEspecialCabecera();
            obj.idPrecioEspecialCabecera = Guid.Empty;
            obj.Estado = 1;
            obj.tipoNegociacion = "";
            obj.codigo = "";
            obj.fechaInicio = DateTime.Now;
            obj.fechaFin = DateTime.Now;

            obj.precios = new List<PrecioEspecialDetalle>();
            obj.clienteSunat = new ClienteSunat();
            obj.clienteSunat.idClienteSunat = 0;

            obj.grupoCliente = new GrupoCliente();
            obj.grupoCliente.idGrupoCliente = 0;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL_BUSQUEDA] = obj;
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


        //[HttpGet]
        //public ActionResult ExportLastSearchExcel()
        //{
        //    List<Rol> list = (List<Rol>)this.Session[Constantes.VAR_SESSION_ROL_LISTA];

        //    RolSearch excel = new RolSearch();
        //    return excel.generateExcel(list, (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO]);
        //}


        [HttpGet]
        public ActionResult PrepararAjusteAlmacen(Guid idCierreStock, string tipoAjuste)
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (this.Session[Constantes.VAR_SESSION_USUARIO] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            CierreStockBL bl = new CierreStockBL();
            CierreStock cierre = bl.SelectCierreStock(usuario.idUsuario, idCierreStock);

            GuiaRemision obj = new GuiaRemision();
            obj.idMovimientoAlmacen = Guid.Empty;
            obj.Estado = 1;
            obj.motivoAjuste = new MotivoAjusteAlmacen();
            obj.motivoTraslado = GuiaRemision.motivosTraslado.AjusteAlmacen;
            obj.ciudadOrigen = cierre.ciudad;
            obj.fechaEmision = cierre.fecha;
            obj.fechaTraslado = cierre.fecha;

            if (tipoAjuste.Equals("excedente"))
            {
                obj.tipoMovimiento = MovimientoAlmacen.tiposMovimiento.Entrada;
                obj.motivoAjuste = new MotivoAjusteAlmacen();
                obj.motivoAjuste.idMotivoAjusteAlmacen = Constantes.ID_MOTIVO_AJUSTE_EXCEDENTE_CIERRE_STOCK;
            }

            if (tipoAjuste.Equals("faltante"))
            {
                obj.tipoMovimiento = MovimientoAlmacen.tiposMovimiento.Salida;
                obj.motivoAjuste = new MotivoAjusteAlmacen();
                obj.motivoAjuste.idMotivoAjusteAlmacen = Constantes.ID_MOTIVO_AJUSTE_FALTANTE_CIERRE_STOCK;
            }

            obj.ajusteAprobado = 0;
            obj.documentoDetalle = new List<DocumentoDetalle>();
            obj.idCierreStock = idCierreStock;

            ProductoBL productoBL = new ProductoBL();

            foreach (RegistroCargaStock det in cierre.detalles)
            {
                if (det.stockValidable == 1 && det.diferenciaCantidadValidacion != 0)
                {
                    DocumentoDetalle detalle = new DocumentoDetalle();
                    detalle.producto = productoBL.getProducto(det.producto.idProducto, false, false, Guid.Empty, false, "PEN", null, false);

                    detalle.esPrecioAlternativo = true;

                    detalle.ProductoPresentacion = new ProductoPresentacion();
                    detalle.ProductoPresentacion.IdProductoPresentacion = 3;
                    detalle.ProductoPresentacion.Equivalencia = det.producto.equivalenciaUnidadEstandarUnidadConteo;
                    detalle.ProductoPresentacion.Presentacion = det.producto.unidadConteo;

                    detalle.unidad = det.producto.unidadConteo;

                    if (det.diferenciaCantidadValidacion > 0 && obj.tipoMovimiento == MovimientoAlmacen.tiposMovimiento.Entrada)
                    {
                        detalle.cantidad = det.diferenciaCantidadValidacion;
                        obj.documentoDetalle.Add(detalle);
                    }

                    if (det.diferenciaCantidadValidacion < 0 && obj.tipoMovimiento == MovimientoAlmacen.tiposMovimiento.Salida)
                    {
                        detalle.cantidad = -1 * det.diferenciaCantidadValidacion;
                        obj.documentoDetalle.Add(detalle);
                    }
                }
            }

            obj.IdUsuarioRegistro = usuario.idUsuario;
            obj.usuario = usuario;

            this.Session[Constantes.VAR_SESSION_AJUSTE_ALMACEN] = obj;


            return RedirectToAction("Editar", "AjusteAlmacen");
        }


        // GET: 
        [HttpGet]
        public ActionResult Index()
        {
            this.Session[Constantes.VAR_SESSION_PAGINA] = Constantes.paginas.BusquedaPrecioEspecial;

            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            if (!usuario.modificaPreciosEspeciales)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();

        }

        public String ConsultarSiExistePrecioEspecial()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            Guid idPrecioEspecialCabecera = Guid.Parse(Request["idPrecioEspecialCabecera"].ToString());
            PrecioEspecialBL bL = new PrecioEspecialBL();
            PrecioEspecialCabecera obj = bL.GetPrecioEspcial(idPrecioEspecialCabecera, usuario.idUsuario);
            obj.usuario = usuario;

            String resultado = JsonConvert.SerializeObject(obj);
            this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL_VER] = obj;

            obj = (PrecioEspecialCabecera)this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL];
            if (obj == null)
                return "{\"existe\":\"false\"}";
            else
                return "{\"existe\":\"true\"}";
        }


        public void iniciarEdicionPrecioEspecial()
        {
            Usuario usuario = (Usuario)this.Session[Constantes.VAR_SESSION_USUARIO];
            PrecioEspecialCabecera obj = (PrecioEspecialCabecera)this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL_VER];
            this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL] = obj;
        }

        public String Create()
        {
            PrecioEspecialCabecera obj = (PrecioEspecialCabecera)this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL];
            
            PrecioEspecialBL bL = new PrecioEspecialBL();

            List<PrecioEspecialDetalle> detallesValidos = new List<PrecioEspecialDetalle>();

            foreach (PrecioEspecialDetalle det in obj.precios)
            {
                if(det.dataRelacionada == null || det.dataRelacionada.Count == 0)
                {
                    detallesValidos.Add(det);
                }
            }
            
            obj.precios = detallesValidos;

            bL.InsertarCabecera(obj);

            this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public String Update()
        {
            PrecioEspecialCabecera obj = (PrecioEspecialCabecera)this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL];

            PrecioEspecialBL bL = new PrecioEspecialBL();

            List<PrecioEspecialDetalle> detallesValidos = new List<PrecioEspecialDetalle>();

            foreach (PrecioEspecialDetalle det in obj.precios)
            {
                if (det.dataRelacionada == null || det.dataRelacionada.Count == 0)
                {
                    detallesValidos.Add(det);
                }
            }

            obj.precios = detallesValidos;

            bL.ActualizarCabecera(obj);

            this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL] = null;
            String resultado = JsonConvert.SerializeObject(obj);
            return resultado;
        }


        public void ChangeGrupoCliente()
        {
            PrecioEspecialCabecera obj = this.PrecioEspecialCabeceraSession;
            obj.grupoCliente.idGrupoCliente = Int32.Parse(this.Request.Params["valor"]);
            this.PrecioEspecialCabeceraSession = obj;
            LimpiarDetallesPrecios();
        }

        public void ChangeClienteSunat()
        {
            PrecioEspecialCabecera obj = this.PrecioEspecialCabeceraSession;
            obj.clienteSunat.idClienteSunat = Int32.Parse(this.Request.Params["valor"]);
            this.PrecioEspecialCabeceraSession = obj;
            LimpiarDetallesPrecios();
        }

        public void ChangeInputString()
        {
            PrecioEspecialCabecera obj = this.PrecioEspecialCabeceraSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, this.Request.Params["valor"]);
            this.PrecioEspecialCabeceraSession = obj;
        }

        public void ChangeInputDate()
        {
            PrecioEspecialCabecera obj = this.PrecioEspecialCabeceraSession;

            string fechaParam = Request.Params["valor"].ToString();

            if (!fechaParam.Trim().Equals(""))
            {
                String[] fecha = fechaParam.Split('/');
                PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
                propertyInfo.SetValue(obj, new DateTime(Int32.Parse(fecha[2]), Int32.Parse(fecha[1]), Int32.Parse(fecha[0])));
                this.PrecioEspecialCabeceraSession = obj;
            }
        }


        public void ChangeInputInt()
        {
            PrecioEspecialCabecera obj = this.PrecioEspecialCabeceraSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]));
            this.PrecioEspecialCabeceraSession = obj;
        }

        public void ChangeInputDecimal()
        {
            PrecioEspecialCabecera obj = this.PrecioEspecialCabeceraSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Decimal.Parse(this.Request.Params["valor"]));
            this.PrecioEspecialCabeceraSession = obj;
        }
        public void ChangeInputBoolean()
        {
            PrecioEspecialCabecera obj = this.PrecioEspecialCabeceraSession;
            PropertyInfo propertyInfo = obj.GetType().GetProperty(this.Request.Params["propiedad"]);
            propertyInfo.SetValue(obj, Int32.Parse(this.Request.Params["valor"]) == 1);
            this.PrecioEspecialCabeceraSession = obj;
        }

        public void LimpiarDetallesPrecios()
        {
            if(((Constantes.paginas) this.Session[Constantes.VAR_SESSION_PAGINA]) == Constantes.paginas.RegistrarPrecioEspecial)
            {
                PrecioEspecialCabecera obj = this.PrecioEspecialCabeceraSession;
                obj.precios = new List<PrecioEspecialDetalle>();
                this.PrecioEspecialCabeceraSession = obj;
            }
        }

        public ActionResult CancelarCreacion()
        {
            PrecioEspecialCabecera obj = (PrecioEspecialCabecera)this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL];
            this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL] = null;
            return RedirectToAction("List", "PrecioEspecial");
        }

        [HttpPost]
        public ActionResult LoadPrecios(HttpPostedFileBase file)
        {
            Usuario usuario = (Usuario)this.Session["usuario"];

            HSSFWorkbook hssfwb;

            ProductoBL productoBL = new ProductoBL();

            hssfwb = new HSSFWorkbook(file.InputStream);

            ISheet sheet = hssfwb.GetSheetAt(0);
            int row = 1;

            int posicionInicial = 0;
            int pos = 0;
            bool agregar = false;
            bool finaliza = false;

            List<PrecioEspecialDetalle> items = new List<PrecioEspecialDetalle>();
            PrecioEspecialCabecera cabecera = (PrecioEspecialCabecera)this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL];

            if (cabecera.tipoNegociacion.Equals("") || 
                (cabecera.tipoNegociacion.Equals("GRUPO") && cabecera.grupoCliente.idGrupoCliente == 0) ||
                (cabecera.tipoNegociacion.Equals("RUC") && cabecera.clienteSunat.idClienteSunat == 0))
            {
                finaliza = true;
                this.Session["s_precioespecial_errorcargaprecios"] = 1;
            } else
            {
                this.Session["s_precioespecial_errorcargaprecios"] = 0;
            }

            while (!finaliza)
            {
                int a = 1;
                if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                {
                    PrecioEspecialDetalle obj = new PrecioEspecialDetalle();
                    obj.producto = new Producto();
                    obj.unidadPrecio = new ProductoPresentacion();
                    obj.unidadCosto = new ProductoPresentacion();
                    obj.moneda = new Moneda();

                    /*  A	SKU
                        C	Moneda
                        D	Tipo Unidad Precio
                        F	Precio
                        H	Tipo Unidad Costo
                        J	Costo
                        L	Fecha Inicio
                        M	Fecha Fin
                        N	Observaciones
                     */
                    agregar = true;
                    posicionInicial = 0;

                    try
                    {

                        pos = posicionInicial + 0;
                        if (sheet.GetRow(row).GetCell(pos) == null)
                        {
                            obj.producto.sku = null;
                            agregar = false;
                        }
                        else
                        {
                            obj.producto.sku = sheet.GetRow(row).GetCell(pos).ToString();
                            if (obj.producto.sku.Trim().Equals(""))
                            {
                                agregar = false;
                            }
                        }


                        if (agregar)
                        {
                            pos = posicionInicial + 2;
                            obj.moneda.codigo = "";
                            if (sheet.GetRow(row).GetCell(pos) != null)
                            {
                                obj.moneda.codigo = sheet.GetRow(row).GetCell(pos).ToString().Trim();
                            }

                            pos = posicionInicial + 3;
                            if (sheet.GetRow(row).GetCell(pos) == null)
                            {
                                obj.unidadPrecio.IdProductoPresentacion = 0;
                            }
                            else
                            {
                                string unidad = sheet.GetRow(row).GetCell(pos).ToString();
                                unidad = unidad.ToLower();
                                obj.unidadPrecio.IdProductoPresentacion = 0;

                                switch (unidad)
                                {
                                    case "alternativa": obj.unidadPrecio.IdProductoPresentacion = 1; break;
                                    case "proveedor": obj.unidadPrecio.IdProductoPresentacion = 2; break;
                                    case "conteo": obj.unidadPrecio.IdProductoPresentacion = 3; break;
                                }
                            }

                            pos = posicionInicial + 5;
                            obj.unidadPrecio.PrecioSinIGV = 0;
                            if (sheet.GetRow(row).GetCell(pos) != null && !sheet.GetRow(row).GetCell(pos).ToString().Trim().Equals(""))
                            {
                                obj.unidadPrecio.PrecioSinIGV = decimal.Parse(sheet.GetRow(row).GetCell(pos).ToString());
                            }

                            pos = posicionInicial + 7;
                            if (sheet.GetRow(row).GetCell(pos) == null)
                            {
                                obj.unidadCosto.IdProductoPresentacion = 0;
                            }
                            else
                            {
                                string unidad = sheet.GetRow(row).GetCell(pos).ToString();
                                unidad = unidad.ToLower();
                                obj.unidadCosto.IdProductoPresentacion = 0;

                                switch (unidad)
                                {
                                    case "alternativa": obj.unidadCosto.IdProductoPresentacion = 1; break;
                                    case "proveedor": obj.unidadCosto.IdProductoPresentacion = 2; break;
                                    case "conteo": obj.unidadCosto.IdProductoPresentacion = 3; break;
                                }
                            }

                            pos = posicionInicial + 9;
                            obj.unidadCosto.CostoSinIGV = 0;
                            if (sheet.GetRow(row).GetCell(pos) != null && !sheet.GetRow(row).GetCell(pos).ToString().Trim().Equals(""))
                            {
                                obj.unidadCosto.CostoSinIGV = decimal.Parse(sheet.GetRow(row).GetCell(pos).ToString());
                            }

                            pos = posicionInicial + 11;
                            obj.fechaInicio = DateTime.MinValue;
                            if (sheet.GetRow(row).GetCell(pos) != null && !sheet.GetRow(row).GetCell(pos).ToString().Trim().Equals("") 
                                && sheet.GetRow(row).GetCell(pos).CellType == CellType.Numeric 
                                && DateUtil.IsCellDateFormatted(sheet.GetRow(row).GetCell(pos)))
                            {
                                obj.fechaInicio = sheet.GetRow(row).GetCell(pos).DateCellValue;
                            } else
                            {
                                obj.fechaInicio = cabecera.fechaInicio;
                            }

                            pos = posicionInicial + 12;
                            obj.fechaFin = DateTime.MinValue;
                            if (sheet.GetRow(row).GetCell(pos) != null && !sheet.GetRow(row).GetCell(pos).ToString().Trim().Equals("")
                                && sheet.GetRow(row).GetCell(pos).CellType == CellType.Numeric 
                                && DateUtil.IsCellDateFormatted(sheet.GetRow(row).GetCell(pos)))
                            {
                                obj.fechaFin = sheet.GetRow(row).GetCell(pos).DateCellValue;
                            } else
                            {
                                obj.fechaFin = cabecera.fechaFin;
                            }

                            pos = posicionInicial + 13;
                            if (sheet.GetRow(row).GetCell(pos) == null)
                            {
                                obj.observaciones = "";
                            }
                            else
                            {
                                obj.observaciones = sheet.GetRow(row).GetCell(pos).ToString().Trim();
                            }                           

                            items.Add(obj);
                        }
                        else
                        {
                            finaliza = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log log = new Log("Excel Precio Especial De talle: " + ex.ToString() + " paso:" + pos, TipoLog.Error, usuario);
                        LogBL logBL = new LogBL();
                        logBL.insertLog(log);
                    }
                }
                else
                {
                    finaliza = true;
                }

                row++;
            }

            cabecera.precios = items;
            List<PrecioEspecialDetalle> results = new List<PrecioEspecialDetalle>();
            if (items.Count > 0)
            {
                PrecioEspecialBL bl = new PrecioEspecialBL();
                results = bl.ValidarDetalles(cabecera);
                cabecera.precios = results;
                this.Session[Constantes.VAR_SESSION_PRECIO_ESPECIAL] = cabecera;
            }


            return RedirectToAction("Editar", "PrecioEspecial");
        }
    }
}