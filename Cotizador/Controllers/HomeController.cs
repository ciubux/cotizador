using BusinessLayer;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cotizador.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            /*   String usuario = Request["txtUser"].ToString();
               String clave = Request["txtClave"].ToString();

               UsuarioDal dal = new UsuarioDal();
               Usuario user = dal.LoginUsuario(usuario, clave);
               if (user != null)
               {
                   this.Session["EstablecimientoLogin"] = dal.getEstablecimientoUsuario(user.idUsuario);
                   this.Session["UsuarioLogin"] = user;
                   return RedirectToAction("Index", "Home");
               }
               else
               {
                   if (usuario.Equals("")) this.Session["loginFault"] = 1;
                   else if (clave.Equals("")) this.Session["loginFault"] = 2;
                   else this.Session["loginFault"] = 3;
                   return RedirectToAction("Index", "Login");
               }*/

            PrecioBL precioBl = new PrecioBL();
            List<PrecioLista> precios = precioBl.getListas();
            ViewBag.Precios = precios;

            MonedaBL monedaBl = new MonedaBL();
            List<Moneda> monedas = monedaBl.getMonedas();
            ViewBag.Monedas = monedas;

            CategoriaBL categoriaBl = new CategoriaBL();
            List<Categoria> categorias = categoriaBl.getCategorias();
            ViewBag.Categorias = categorias;

            ProveedorBL proveedorBl = new ProveedorBL();
            List<Proveedor> proveedores = proveedorBl.getProveedores();
            ViewBag.Proveedores = proveedores;

            return View();
        }



        public String GetClientes()
        {

            String data = this.Request.Params["data[q]"];

            ClienteBL clienteBL = new ClienteBL();
            List<Cliente> clienteList = clienteBL.getCLientesBusqueda(data);

            String resultado = "{\"q\":\"" + data + "\",\"results\":[";
            Boolean existeCliente = false;
            foreach (Cliente cliente in clienteList)
            {
                cliente.razonSocial = cliente.codigo+  " - " +cliente.razonSocial + " - RUC: " + cliente ;
                resultado += "{\"id\":\"" + cliente.idCliente + "\",\"text\":\"" + cliente.razonSocial + "\"},";
                existeCliente = true;
            }

            if (existeCliente)
                resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
            else
                resultado = resultado.Substring(0, resultado.Length) + "]}";

            return resultado;
        }

        public String GetFamilias()
        {

            Guid idCategoria = Guid.Parse(Request["idCategoria"].ToString());
            this.Session["idCategoria"] = idCategoria.ToString();

            FamiliaBL bl = new FamiliaBL();
            List<Familia> lista = bl.getFamilias(idCategoria);

            String resultado = "{\"idCategoria\":\"" + idCategoria.ToString() + "\",\"results\":[";
            foreach (Familia fam in lista)
            {

                resultado += "{\"id\":\"" + fam.idFamilia + "\",\"text\":\"" + fam.nombre + "\"},";
            }

            resultado = resultado.Substring(0, resultado.Length - 1) + "]}";

            return resultado;
        }

        public String SetProveedor()
        {
            //Guid idProveedor = Guid.Parse(Request["idProveedor"].ToString());
            this.Session["idProveedor"] = Request["idProveedor"].ToString();
            return "";
        }

        public String SetFamilia()
        {
            //Guid idFamilia = Guid.Parse(Request["idFamilia"].ToString());
            this.Session["idFamilia"] = Request["idFamilia"].ToString();
            return "";
        }

        public String SetMoneda()
        {
            //Guid idFamilia = Guid.Parse(Request["idFamilia"].ToString());
            this.Session["idMoneda"] = Request["idMoneda"].ToString();
            return "";
        }
        public String GetProductos()
        {
            Guid idProveedor = Guid.Parse(this.Session["idProveedor"].ToString());
            Guid idFamilia = Guid.Parse(this.Session["idFamilia"].ToString());

            String data = this.Request.Params["data[q]"];

            ProductoBL bl = new ProductoBL();
            List<Producto> lista = bl.getProductosBusqueda(idProveedor, idFamilia, data);

            String resultado = "{\"q\":\"" + data + "\",\"results\":[";
            Boolean existe = false;
            foreach (Producto prod in lista)
            {
                resultado += "{\"id\":\"" + prod.idProducto + "\",\"text\":\"" + prod.descripcion + "(" + prod.sku.Trim() + ")" + "\"},";
                existe = true;
            }

            if (existe)
                resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
            else
                resultado = resultado.Substring(0, resultado.Length) + "]}";

            return resultado;
        }

        public String GetProducto()
        {
            Guid idProducto = Guid.Parse(Request["idProducto"].ToString());
            Guid idMoneda = Guid.Parse(this.Session["idMoneda"].ToString());

            this.Session["idProducto"] = idProducto.ToString();

            ProductoBL bl = new ProductoBL();
            Producto obj = bl.getProducto(idProducto);


            PrecioBL precioBl = new PrecioBL();
            List<PrecioLista> lista = precioBl.getPreciosProducto(idProducto, idMoneda);

            String precios = "";
            String resultado = "{" +
                "\"id\":\"" + obj.idProducto + "\"," +
                "\"nombre\":\"" + obj.descripcion + "\"," +
                "\"image\":\"data:image/png;base64, " + Convert.ToBase64String(obj.image) + "\"," +
                "\"presentacion\":\"" + obj.unidad.descripcion + "\"," +
                "\"precios\":[";
            foreach (PrecioLista p in lista)
            {
                if (precios.Length > 0)
                {
                    precios = precios + ",";
                }
                precios = precios + "{\"id\":\"" + p.idPrecioLista + "\", \"nombre\":\"" + p.codigo + "(" + p.nombre + ")\", \"precio\":\"" + p.precio.ToString() + "\"}";
            }

            resultado = resultado + precios + "]}";

            return resultado;
        }

        public String AddProducto()
        {
            PrecioBL precioBl = new PrecioBL();
            if (this.Session["detalles"] == null)
            {
                this.Session["detalles"] = new List<CotizacionDetalle>();
            }

            List<CotizacionDetalle> detalles = (List<CotizacionDetalle>)this.Session["detalles"];

            CotizacionDetalle det = new CotizacionDetalle();

            Proveedor prov = new Proveedor();
            prov.idProveedor = Guid.Parse(this.Session["idProveedor"].ToString());
            prov.nombre = Request["proveedor"].ToString();
            det.proveedor = prov;

            Familia fam = new Familia();
            fam.idFamilia = Guid.Parse(this.Session["idFamilia"].ToString());
            fam.nombre = Request["familia"].ToString();
            det.familia = fam;
            
            ProductoBL prodBl = new ProductoBL();
            Producto prod = prodBl.getProducto(Guid.Parse(this.Session["idProducto"].ToString()));
            det.producto = prod;
            det.idProducto = prod.idProducto;

            Categoria cat = new Categoria();
            cat.idCategoria = Guid.Parse(this.Session["idCategoria"].ToString());
            cat.nombre = Request["categoria"].ToString();
            det.categoria = cat;
            
            MonedaBL monedaBl = new MonedaBL();
            List<Moneda> monedas = monedaBl.getMonedas();
            Guid idMoneda = Guid.Parse(this.Session["idMoneda"].ToString());
            foreach (Moneda mo in monedas)
            {
                if (mo.idMoneda == idMoneda)
                {
                    det.moneda = mo;
                }
            }
                
            Guid idPrecioProducto = Guid.Parse(Request["idPrecioProducto"].ToString());
            det.precioLista = precioBl.getPrecioProducto(prod.idProducto, idPrecioProducto);
            det.idPrecio = det.precioLista.idPrecioLista;

            det.cantidad = int.Parse(Request["cantidad"].ToString());
            det.porcentajeDescuento = Decimal.Parse(Request["pDescuento"].ToString());

            det.valorUnitario = det.precioLista.precio;
            det.valorUnitarioFinal = det.valorUnitario * (100 - det.porcentajeDescuento) / 100;
            det.subTotal = det.valorUnitario * det.cantidad;
            
            String resultado = "{" +
                "\"proveedor\":\"" + det.proveedor.nombre + "\"," +
                "\"familia\":\"" + det.familia.nombre + "\"," +
                "\"categoria\":\"" + det.categoria.nombre + "\"," +
                "\"codigoProducto\":\"" + det.producto.sku + "\"," +
                "\"nombreProducto\":\"" + det.producto.descripcion + "\"," +
                "\"presentacion\":\"" + det.producto.unidad.descripcion + "\"," +
                "\"moneda\":\"" + det.moneda.simbolo + "\"," +
                "\"cantidad\":\"" + det.cantidad.ToString() + "\"," +
                "\"porcentajeDescuento\":\"" + det.porcentajeDescuento.ToString() + "\"," +
                "\"valorUnitario\":\"" + det.valorUnitario.ToString() + "\"," +
                "\"valorUnitarioFinal\":\"" + det.valorUnitarioFinal.ToString() + "\"," +
                "\"subTotal\":\"" + det.subTotal.ToString() + "\"," +
                "\"nombrePrecio\":\"" + det.precioLista.nombreVista + "\"}";

            detalles.Add(det);

            this.Session["detalles"] = detalles;

            return resultado;
        }

        public ActionResult GenerarPDF()
        {
            Cotizacion cot = new Cotizacion();

            String[] fecha = Request["fecha"].ToString().Split('/');
            cot.fecha = new DateTime(Convert.ToInt32(fecha[2]), Convert.ToInt32(fecha[1]), Convert.ToInt32(fecha[0]));
            
            cot.idCiudad = Guid.Parse(Request["idCiudad"].ToString());
            cot.idCliente = Guid.Parse(Request["idCliente"].ToString());
            cot.incluidoIgv = short.Parse(Request["igv"].ToString());
            cot.mostrarCodProveedor = short.Parse(Request["codigoproveedor"].ToString());
            cot.idMoneda = Guid.Parse(Request["moneda"].ToString());
            //cot.idTipoCambio = Guid.Parse(Request["tipocambio"].ToString());
            cot.idPrecio = Guid.Parse(Request["precio"].ToString());
            cot.flete = Decimal.Parse(Request["flete"].ToString());

            List<CotizacionDetalle> detalles = (List<CotizacionDetalle>)this.Session["detalles"];

            cot.detalles = detalles;

            CotizacionBL bl = new CotizacionBL();
            bl.InsertCotizacion(cot);

            return RedirectToAction("Index", "Home");
        }

        /*
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }*/
    }
}