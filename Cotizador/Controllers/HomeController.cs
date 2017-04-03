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

            foreach (Cliente cliente in clienteList)
            {
                cliente.razonSocial = cliente.codigo+  " - " +cliente.razonSocial + " - RUC: " + cliente ;
                resultado += "{\"id\":\"" + cliente.idCliente + "\",\"text\":\"" + cliente.razonSocial + "\"},";
            }

            resultado = resultado.Substring(0, resultado.Length - 1) + "]}";

            return resultado;
        }

        public String GetFamilias()
        {

            Guid idCategoria = Guid.Parse(Request["idCategoria"].ToString());

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
        public ActionResult GenerarPDF()
        {
            Cotizacion cot = new Cotizacion();

            cot.fecha = DateTime.ParseExact(Request["fecha"].ToString(), "dd/MM/YYYY", null);
            cot.idCiudad = Guid.Parse(Request["idCiudad"].ToString());
            cot.idCliente = Guid.Parse(Request["idCliente"].ToString());
            cot.incluidoIgv = short.Parse(Request["igv"].ToString());
            cot.mostrarCodProveedor = short.Parse(Request["codigoproveedor"].ToString());
            cot.idMoneda = Guid.Parse(Request["moneda"].ToString());
            cot.idTipoCambio = Guid.Parse(Request["tipocambio"].ToString());
            cot.idPrecio = Guid.Parse(Request["precio"].ToString());
            cot.flete = Decimal.Parse(Request["flete"].ToString());

            
            
            CotizacionBL bl = new CotizacionBL();

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