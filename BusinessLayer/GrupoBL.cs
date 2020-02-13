
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
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario / pedidoDetalle.ProductoPresentacion.Equivalencia;
                    }

                    pedidoDetalle.porcentajeDescuento = (1 - (pedidoDetalle.precioNeto / (pedidoDetalle.precioLista == 0 ? 1 : pedidoDetalle.precioLista))) * 100;
                    pedidoDetalle.porcentajeMargen = (1 - (pedidoDetalle.costoLista / (pedidoDetalle.precioNeto == 0 ? 1 : pedidoDetalle.precioNeto))) * 100;
                }


                return items;
            }
        }

        public List<DocumentoDetalle> getPreciosHistoricoGrupoCliente(int idGrupoCliente)
        {
            using (var productoDal = new ProductoDAL())
            {
                List<DocumentoDetalle> items = productoDal.getPreciosHistoricoGrupoCliente(idGrupoCliente);
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
                        pedidoDetalle.producto.precioClienteProducto.precioUnitario / pedidoDetalle.ProductoPresentacion.Equivalencia;
                    }

                    pedidoDetalle.porcentajeDescuento = (1 - (pedidoDetalle.precioNeto / (pedidoDetalle.precioLista == 0 ? 1 : pedidoDetalle.precioLista))) * 100;
                    pedidoDetalle.porcentajeMargen = (1 - (pedidoDetalle.costoLista / (pedidoDetalle.precioNeto == 0 ? 1 : pedidoDetalle.precioNeto))) * 100;
                }


                return items;
            }
        }

        public bool setSKUCliente(String skuCliente, int idGrupo, Guid idUsuario, Guid idProducto, int replicarMiembros)
        {
            using (ProductoDAL productoDal = new ProductoDAL())
            {
                return productoDal.setSKUClienteGrupo(skuCliente, idGrupo, idUsuario, idProducto, replicarMiembros);
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
                GrupoCliente gc = grupoClienteDAL.insertGrupoCliente(grupoCliente);

                if ((gc.usuario == null || !gc.usuario.modificaGrupoClientes))
                {
                    AlertaValidacion alerta = new AlertaValidacion();
                    alerta.idRegistro = gc.idGrupoCliente.ToString();
                    alerta.nombreTabla = "GRUPO_CLIENTE";
                    alerta.Estado = 0;
                    alerta.UsuarioCreacion = gc.usuario;
                    alerta.IdUsuarioCreacion = gc.usuario.idUsuario;
                    alerta.tipo = AlertaValidacion.CREA_GRUPO_CLIENTE;
                    alerta.data = new DataAlertaValidacion();
                    
                    alerta.data.ObjData = gc.codigoNombre;

                    AlertaValidacionDAL alertaDal = new AlertaValidacionDAL();
                    alertaDal.insertAlertaValidacion(alerta);
                }

                return gc;
            }
        }

        public GrupoCliente updateGrupoCliente(GrupoCliente grupoCliente)
        {
            using (var grupoClienteDAL = new GrupoClienteDAL())
            {
                return grupoClienteDAL.updateGrupoCliente(grupoCliente);
            }
        }

        public bool agregarProductoCanasta(int idGrupoCliente, Guid idProducto, Usuario usuario)
        {
            using (var dal = new PrecioClienteProductoDAL())
            {

                return dal.agregaProductoCanastaGrupoCliente(idGrupoCliente, idProducto, usuario.idUsuario);
            }
        }


        public bool retiraProductoCanasta(int idGrupoCliente, Guid idProducto, Usuario usuario)
        {
            using (var dal = new PrecioClienteProductoDAL())
            {

                return dal.retiraProductoCanastaGrupoCliente(idGrupoCliente, idProducto, usuario.idUsuario);
            }
        }

        public bool limpiaCanasta(int idGrupoCliente, int aplicaMiembros)
        {
            using (var dal = new PrecioClienteProductoDAL())
            {

                return dal.limpiaCanastaCliente(idGrupoCliente, aplicaMiembros);
            }
        }
    }
}
