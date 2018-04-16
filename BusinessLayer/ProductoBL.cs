
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class ProductoBL
    {
        public String getProductosBusqueda(String textoBusqueda, bool considerarDescontinuados, String proveedor, String familia)
        {


            List<Producto> productoList = new List<Producto>();
            using (var dal = new ProductoDAL())
            {
                productoList = dal.getProductosBusqueda(textoBusqueda, considerarDescontinuados, proveedor, familia);
            }


            String resultado = "{\"q\":\"" + textoBusqueda + "\",\"results\":[";
            Boolean existe = false;
            foreach (Producto prod in productoList)
            {
                resultado += "{\"id\":\"" + prod.idProducto + "\",\"text\":\"" + prod.ToString() + "\"},";
                existe = true;
            }

            if (existe)
                resultado = resultado.Substring(0, resultado.Length - 1) + "]}";
            else
                resultado = resultado.Substring(0, resultado.Length) + "]}";


            return resultado;
        }

        public Producto getProducto(Guid idProducto, Boolean esProvincia, Boolean incluidoIGV, Guid idCliente)
        {
            using (var dal = new ProductoDAL())
            {
                Producto producto = dal.getProducto(idProducto, idCliente);
                //Si es Provincia automaticamente se considera el precioProvincia como precioSinIGV
                if (esProvincia)
                {
                    producto.precioSinIgv = producto.precioProvinciaSinIgv;
                }

                //Si no contiene imagen entonces se carga la imagen por defecto
                if (producto.image == null)
                {
                    FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                    MemoryStream storeStream = new MemoryStream();
                    storeStream.SetLength(inStream.Length);
                    inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                    storeStream.Flush();
                    inStream.Close();
                    producto.image = storeStream.GetBuffer();
                }

                //El precioSinIGV se convierte en precioLista
                producto.precioLista = producto.precioSinIgv;
                //El costoLista es el costo sin IGV
                producto.costoLista = producto.costoSinIgv;

                //En caso que se requiera hacer calculo incluido IGV entonces se agrega el IGV al precioSinGIV y al costoSinIGV/
                if (incluidoIGV)
                {
                    //Se agrega el IGV al costoLista
                    producto.costoLista = producto.costoSinIgv + (producto.costoSinIgv * Constantes.IGV);
                    //Se agrega el IGV al precioLista
                    producto.precioLista = producto.precioLista + (producto.precioLista * Constantes.IGV);

                    //Si el precioNetoEquivalente es distinto de null 
                    //es null cuando se obtiene de un precioRegistrado
                    if (producto.precioClienteProducto.idPrecioClienteProducto != Guid.Empty)
                    {
                        producto.precioClienteProducto.precioNeto = producto.precioClienteProducto.precioNeto + (producto.precioClienteProducto.precioNeto * Constantes.IGV);
                        producto.precioClienteProducto.precioUnitario = producto.precioClienteProducto.precioUnitario + (producto.precioClienteProducto.precioUnitario * Constantes.IGV);
                    }
                }

                //Se aplica formato al costo de Lista y al precio Lista a dos decimales
                producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.costoLista));
                producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.precioLista));
                //Se aplica formato al precioUnitario obtenido desde precioRegistrados
                if (producto.precioClienteProducto.idPrecioClienteProducto != Guid.Empty)
                {
                    producto.precioClienteProducto.precioNeto = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, producto.precioClienteProducto.precioNeto));
                }
                return producto;
            }
        }


        public void setProductoStaging(ProductoStaging productoStaging)
        {
            using (var productoDAL = new ProductoDAL())
            {
                productoDAL.setProductoStaging(productoStaging);
            }
        }

        public void truncateProductoStaging()
        {
            using (var productoDAL = new ProductoDAL())
            {
                productoDAL.truncateProductoStaging();
            }
        }

        public void mergeProductoStaging()
        {
            using (var productoDAL = new ProductoDAL())
            {
                productoDAL.mergeProductoStaging();
            }
        }
    }
}
