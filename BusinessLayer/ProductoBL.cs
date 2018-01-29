
using DataLayer;
using System.Collections.Generic;
using System;
using Model;
using System.IO;

namespace BusinessLayer
{
    public class ProductoBL
    {
        public List<Producto> getProductosBusqueda(String textoBusqueda)
        {
            using (var dal = new ProductoDAL())
            {
                return dal.getProductosBusqueda(textoBusqueda);
            }
        }

        public Producto getProducto(Guid idProducto, Boolean esProvincia, Boolean incluidoIGV, Decimal Flete)
        {
            using (var dal = new ProductoDAL())
            {
                Producto producto = dal.getProducto(idProducto);
                //Si es provincia, se considera el precio de provincia
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

                //Se agrega el flete al precioLista
                //EL PRECIO LISTA YA INCLUTE FLETE
                producto.precioLista = producto.precioSinIgv + (producto.precioSinIgv * Flete / 100);

                producto.costoLista = producto.costoSinIgv;
                if (incluidoIGV)
                {
                    //Se agrega el IGV al costoLista
                    producto.costoLista = producto.costoSinIgv + (producto.costoSinIgv * Constantes.IGV);
                    //Se agrega el IGV al precioLista
                    producto.precioLista = producto.precioLista + (producto.precioLista * Constantes.IGV);
                }

                producto.costoLista = Decimal.Parse(String.Format(Constantes.decimalFormat, producto.costoLista));
                producto.precioLista = Decimal.Parse(String.Format(Constantes.decimalFormat, producto.precioLista));


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
    }
}
