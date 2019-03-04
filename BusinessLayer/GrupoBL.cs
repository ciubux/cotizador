
using DataLayer;
using System.Collections.Generic;
using System;
using System.IO;
using Model;

namespace BusinessLayer
{
    public class GrupoClienteBL
    {
        public List<GrupoCliente> getGruposBusqueda(String textoBusqueda)
        {
            using (var grupoDAL = new GrupoClienteDAL())
            {
                return grupoDAL.getGruposBusqueda(textoBusqueda);
            }
        }
        public GrupoCliente getGrupo(int idGrupoCliente)
        {
            using (var grupoDAL = new GrupoClienteDAL())
            {
                return grupoDAL.getGrupo(idGrupoCliente);
            }
        }

        public List<DocumentoDetalle> getPreciosVigentesGrupoCliente(int idGrupoCliente)
        {
            using (var productoDal = new ProductoDAL())
            {
                List<DocumentoDetalle> items = productoDal.getPreciosVigentesGrupoCliente(idGrupoCliente);
                foreach (DocumentoDetalle pedidoDetalle in items)
                {
                    if (pedidoDetalle.producto.image == null)
                    {
                        FileStream inStream = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\images\\NoDisponible.gif", FileMode.Open);
                        MemoryStream storeStream = new MemoryStream();
                        storeStream.SetLength(inStream.Length);
                        inStream.Read(storeStream.GetBuffer(), 0, (int)inStream.Length);
                        storeStream.Flush();
                        inStream.Close();
                        pedidoDetalle.producto.image = storeStream.GetBuffer();
                    }


                    //Se agrega el IGV al precioLista
                    pedidoDetalle.producto.precioLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedidoDetalle.producto.precioSinIgv));
                    pedidoDetalle.producto.costoLista = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedidoDetalle.producto.costoSinIgv));

                    if (pedidoDetalle.esPrecioAlternativo)
                    {
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario =
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario / pedidoDetalle.producto.equivalencia;
                    }

                    pedidoDetalle.porcentajeDescuento = (1 - (pedidoDetalle.precioNeto / (pedidoDetalle.precioLista == 0 ? 1 : pedidoDetalle.precioLista))) * 100;
                    pedidoDetalle.porcentajeMargen = (1 - (pedidoDetalle.costoLista / (pedidoDetalle.precioNeto == 0 ? 1 : pedidoDetalle.precioNeto))) * 100;
                }


                return items;
            }
        }

        public List<Cliente> getClientesGrupo(int idGrupo)
        {
            using (var dal = new ClienteDAL())
            {
                return dal.getClientesGrupo(idGrupo);
            }
        }

        public List<GrupoCliente> getGruposCliente()
        {
            using (var grupoDAL = new GrupoClienteDAL())
            {
                return grupoDAL.getGruposCliente();
            }
        }

        public List<GrupoCliente> getGruposCliente(GrupoCliente grupoCliente)
        {
            using (var grupoDAL = new GrupoClienteDAL())
            {
                return grupoDAL.getGruposCliente(grupoCliente);
            }
        }


        public GrupoCliente insertGrupoCliente(GrupoCliente grupoCliente)
        {
            using (var grupoClienteDAL = new GrupoClienteDAL())
            { 
                return grupoClienteDAL.insertGrupoCliente(grupoCliente);
            }
        }

        public GrupoCliente updateGrupoCliente(GrupoCliente grupoCliente)
        {
            using (var grupoClienteDAL = new GrupoClienteDAL())
            {
                return grupoClienteDAL.updateGrupoCliente(grupoCliente);
            }
        }

    }
}
