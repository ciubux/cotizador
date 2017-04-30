﻿using BusinessLayer;
using Model;
using cotizadorPDF;
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
            if (this.Session["detalles"] == null)
            {
                this.Session["detalles"] = new List<CotizacionDetalle>();
            }

            if (this.Session["usuario"] != null)
            {
                PrecioBL precioBl = new PrecioBL();
                List<PrecioLista> precios = precioBl.getListas();
                ViewBag.Precios = precios;

                MonedaBL monedaBl = new MonedaBL();
                List<Moneda> monedas = monedaBl.getMonedas();

                int i = 0;
                foreach (Moneda mo in monedas)
                {
                    if (i == 0)
                    {
                        this.Session["idMoneda"] = mo.idMoneda.ToString();
                        this.Session["moneda"] = mo;
                    }
                    i++;
                }
                ViewBag.Monedas = monedas;

                CategoriaBL categoriaBl = new CategoriaBL();
                List<Categoria> categorias = categoriaBl.getCategorias();
                ViewBag.Categorias = categorias;

                ProveedorBL proveedorBl = new ProveedorBL();
                List<Proveedor> proveedores = proveedorBl.getProveedores();
                ViewBag.Proveedores = proveedores;

                TipoCambioBL tipoCambioBL = new TipoCambioBL();
                TipoCambio tipoCambio = tipoCambioBL.getTipoCambio();
                ViewBag.TipoCambio = tipoCambio.monto;

                Usuario usuario = (Usuario)this.Session["usuario"];
                ViewBag.nombreUsuario = usuario.apellidos + " " + usuario.nombres;
                
                List<CotizacionDetalle> detalles = (List<CotizacionDetalle>)this.Session["detalles"];
                ViewBag.Detalles = detalles;
       
                Boolean incluidoIgv = true;
                if (this.Session["incluidoIgv"] != null)
                {
                    incluidoIgv = (Boolean)this.Session["incluidoIgv"];
                }
                ViewBag.incluidoIgv = incluidoIgv;

                String monedaSimbolo = "S/.";
                if (this.Session["monedaSimbolo"] != null)
                {
                    monedaSimbolo = (String)this.Session["monedaSimbolo"];
                }
                ViewBag.monedaSimbolo = monedaSimbolo;

                Boolean mostrarCodProveedor = true;
                if (this.Session["mostrarCodProveedor"] != null)
                {
                    mostrarCodProveedor = (Boolean)this.Session["mostrarCodProveedor"];
                }
                ViewBag.mostrarCodProveedor = mostrarCodProveedor;

                Decimal pFlete = 0;
                if (this.Session["pFlete"] != null)
                {
                    pFlete = (Decimal)this.Session["pFlete"];
                }
                ViewBag.pFlete = pFlete;
                
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
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
            MonedaBL monedaBl = new MonedaBL();
            List<Moneda> monedas = monedaBl.getMonedas();
            Guid idMoneda = Guid.Parse(this.Session["idMoneda"].ToString());
            Moneda mon  = null;
            foreach (Moneda mo in monedas)
            {
                if (mo.idMoneda == idMoneda)
                {
                    mon = mo;
                }
            }

            this.Session["moneda"] = mon;
            return "";
        }
        public String GetProductos()
        {
            Guid idProveedor = Guid.Empty;
            if (this.Session["idProveedor"] != null && !this.Session["idProveedor"].ToString().Equals("0"))
            {
                idProveedor = Guid.Parse(this.Session["idProveedor"].ToString());
            }

            Guid idFamilia = Guid.Empty;
            if (this.Session["idFamilia"] != null && !this.Session["idFamilia"].ToString().Equals("0"))
            {
                idFamilia = Guid.Parse(this.Session["idFamilia"].ToString());
            }
 
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
            List<PrecioLista> lista = new List<PrecioLista>() ;// precioBl.getPreciosProducto(idProducto, idMoneda);

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
            
            det.moneda = (Moneda) this.Session["moneda"];
            
                
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
            cot.moneda = (Moneda)this.Session["moneda"];

            CiudadBL ciudadBl = new CiudadBL();
            cot.ciudad = ciudadBl.getCiudad(cot.idCiudad);

            Usuario usuario = (Usuario)this.Session["usuario"];
            cot.usuarioCreacion = usuario.apellidos + "_" + usuario.nombres;

            ClienteBL clienteBl = new ClienteBL();
            cot.cliente = clienteBl.getCliente(cot.idCliente);
            
            List<CotizacionDetalle> detalles = (List<CotizacionDetalle>)this.Session["detalles"];

            cot.detalles = detalles;

            cot.subtotal = 0;
            foreach (CotizacionDetalle det in cot.detalles)
            {
                cot.subtotal = cot.subtotal + (det.subTotal);
            }

            cot.igv = 0;

            if (cot.incluidoIgv == 1)
            {
                cot.igv = (decimal)0.18 * cot.subtotal;
            }

            cot.total = cot.subtotal + cot.igv;

            CotizacionBL bl = new CotizacionBL();
            bl.InsertCotizacion(cot);

            GeneradorPDF gen = new GeneradorPDF();
            gen.generarPDFExtended(cot);
            

            return Redirect("/pdfs/" + cot.usuarioCreacion + ".pdf");
        }

        public String TestPDF()
        {
            //Guid idFamilia = Guid.Parse(Request["idFamilia"].ToString());
            GeneradorPDF gen = new GeneradorPDF();
            gen.generar();
            return "";
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