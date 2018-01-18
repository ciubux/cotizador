
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
                    producto.precioLista = producto.precio_provincia;
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

                //Se calcula el precio unitario Alternativo sin IGV
                producto.precioSinIgv = producto.precioLista;
                //si la equivalencia es mayor a cero entonces quiere decir que existe una unidad alternativa
                if (producto.equivalencia > 1)
                {
                    producto.precioAlternativoSinIgv = Decimal.Parse(String.Format(Constantes.decimalFormat, producto.precioSinIgv / producto.equivalencia));
                }
                else
                {
                    producto.precioAlternativoSinIgv = 0;
                }

                producto.precioLista = producto.precioLista + (producto.precioLista * Flete / 100);

                //Si esta seleccionado el boton incluido igv entonces hay que realizar el calculo
                //se calcula utilizando el precioUnitarioSinIGV, dado que el precio estandar es el que se selecciona por defecto
                if (incluidoIGV)
                {
                    producto.precioLista = producto.precioLista + (producto.precioLista * Constantes.IGV);
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
    }
}
