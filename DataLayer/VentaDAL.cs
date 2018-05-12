using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;

namespace DataLayer
{
    public class VentaDAL : DaoBase
    {
        public VentaDAL(IDalSettings settings) : base(settings)
        {
        }

        public VentaDAL() : this(new CotizadorSettings())
        {
        }

        public void InsertVenta(Venta venta)
        {
        }




        public void UpdateVenta(Venta venta)
        {
            foreach (PedidoDetalle ventaDetalle in venta.pedido.pedidoDetalleList)
            {
                this.UpdateVentaDetalle(ventaDetalle);
            }

            //Actualiza los totales de la venta
            var objCommand = GetSqlCommand("pu_venta");
            InputParameterAdd.Guid(objCommand, "idVenta", venta.idVenta);
            InputParameterAdd.Varchar(objCommand, "observaciones", venta.observaciones);
            ExecuteNonQuery(objCommand);

        }


        public void UpdateVentaDetalle(PedidoDetalle ventaDetalle)
        {
            var objCommand = GetSqlCommand("pu_ventaDetalle");
            InputParameterAdd.Guid(objCommand, "idVentaDetalle", ventaDetalle.idVentaDetalle);
            InputParameterAdd.Decimal(objCommand, "precioUnitario", ventaDetalle.precioUnitario);
            ExecuteNonQuery(objCommand);
        }



        public Venta SelectVenta(Venta venta)
        {
            return null;
        }

        public List<Venta> SelectVentas(Venta venta)
        {/*
            var objCommand = GetSqlCommand("ps_pedidos");
            InputParameterAdd.BigInt(objCommand, "numero", pedido.numeroPedido);
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", pedido.numeroGrupoPedido);
            InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuarioBusqueda.idUsuario);
           
            InputParameterAdd.DateTime(objCommand, "fechaSolicitudDesde", pedido.fechaSolicitudDesde);
            InputParameterAdd.DateTime(objCommand, "fechaSolicitudHasta", pedido.fechaSolicitudHasta);
 
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta);


            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            DataTable dataTable = Execute(objCommand);

            List<Pedido> pedidoList = new List<Pedido>();

            foreach (DataRow row in dataTable.Rows)
            {
                pedido = new Pedido();
                pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo_pedido");
                pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                pedido.incluidoIGV = Converter.GetBool(row, "incluido_igv");

                pedido.montoIGV = Converter.GetDecimal(row, "igv");
                pedido.montoTotal = Converter.GetDecimal(row, "total");
                pedido.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedido.montoTotal - pedido.montoIGV));

                pedido.observaciones = Converter.GetString(row, "observaciones");

                pedido.cliente = new Cliente();
                pedido.cliente.codigo = Converter.GetString(row, "codigo");
                pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                pedido.cliente.ruc = Converter.GetString(row, "ruc");

                pedido.usuario = new Usuario();
                pedido.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                pedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");

                //  cotizacion.usuario_aprobador = new Usuario();
                //  cotizacion.usuario_aprobador.nombre = Converter.GetString(row, "nombre_usuario_aprobador");

                pedido.ciudad = new Ciudad();
                pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                pedido.seguimientoPedido = new SeguimientoPedido();
                pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Converter.GetInt(row, "estado_seguimiento");
                pedido.seguimientoPedido.observacion = Converter.GetString(row, "observacion_seguimiento");
                pedido.seguimientoPedido.usuario = new Usuario();
                pedido.seguimientoPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                pedido.seguimientoPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");

                pedidoList.Add(pedido);
            }
            return pedidoList;*/
            return null;
        }


        public void insertSeguimientoVenta(Pedido pedido)
        {
            
        }
        
    }
}
