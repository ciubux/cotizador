using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Model;
using System.Linq;

namespace DataLayer
{
    public class PedidoDAL : DaoBase
    {
        public PedidoDAL(IDalSettings settings) : base(settings)
        {
        }

        public PedidoDAL() : this(new CotizadorSettings())
        {
        }

        #region Pedidos de Venta

        public void InsertPedido(Pedido pedido)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_pedido");
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", pedido.numeroGrupoPedido); //puede ser null

            if (pedido.cotizacion.idCotizacion == Guid.Empty)
                InputParameterAdd.Guid(objCommand, "idCotizacion", null); //puede ser null
            else
                InputParameterAdd.Guid(objCommand, "idCotizacion", pedido.cotizacion.idCotizacion); //puede ser null

            if (pedido.promocion == null || pedido.promocion.idPromocion == Guid.Empty)
                InputParameterAdd.Guid(objCommand, "idPromocion", null); //puede ser null
            else
                InputParameterAdd.Guid(objCommand, "idPromocion", pedido.promocion.idPromocion); //puede ser null

            if (pedido.ordenCompracliente == null || pedido.ordenCompracliente.idOrdenCompraCliente == Guid.Empty)
                InputParameterAdd.Guid(objCommand, "idOrdenCompraCliente", null); //puede ser null
            else
                InputParameterAdd.Guid(objCommand, "idOrdenCompraCliente", pedido.ordenCompracliente.idOrdenCompraCliente); //puede ser null

            InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", pedido.numeroReferenciaCliente); //puede ser null

            InputParameterAdd.Varchar(objCommand, "moneda", pedido.moneda == null ? null : pedido.moneda.codigo);

            if (pedido.clasePedido == Pedido.ClasesPedido.Venta)
            {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", pedido.direccionEntrega.idDireccionEntrega); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", pedido.direccionEntrega.descripcion);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", pedido.direccionEntrega.contacto); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", pedido.direccionEntrega.telefono); //puede ser null
                InputParameterAdd.Varchar(objCommand, "codigoCliente", pedido.direccionEntrega.codigoCliente); //puede ser null
                InputParameterAdd.Varchar(objCommand, "codigoMP", pedido.direccionEntrega.codigoMP); //puede ser null
                InputParameterAdd.Varchar(objCommand, "nombre", pedido.direccionEntrega.nombre); //puede ser null
                InputParameterAdd.Varchar(objCommand, "observacionesDireccionEntrega", pedido.direccionEntrega.observaciones); //puede ser null
            }
            else if (pedido.clasePedido == Pedido.ClasesPedido.Compra)
            {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "codigoCliente", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "codigoMP", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "nombre", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "observacionesDireccionEntrega", null); //puede ser null
            }
            else
            {
                if (pedido.tipoPedidoAlmacen == Pedido.tiposPedidoAlmacen.TrasladoInterno ||
                    pedido.tipoPedidoAlmacen == Pedido.tiposPedidoAlmacen.PrestamoEntregado 
                )
                {
                    InputParameterAdd.Guid(objCommand, "idDireccionEntrega", pedido.direccionEntrega.idDireccionEntrega); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "direccionEntrega", pedido.direccionEntrega.descripcion);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "contactoEntrega", pedido.direccionEntrega.contacto); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", pedido.direccionEntrega.telefono); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "codigoCliente", pedido.direccionEntrega.codigoCliente); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "codigoMP", pedido.direccionEntrega.codigoMP); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "nombre", pedido.direccionEntrega.nombre); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "observacionesDireccionEntrega", pedido.direccionEntrega.observaciones); //puede ser null
                }
                else
                {
                    InputParameterAdd.Guid(objCommand, "idDireccionEntrega", null); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "direccionEntrega", null);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "contactoEntrega", null); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", null); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "codigoCliente", null); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "codigoMP", null); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "nombre", null); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "observacionesDireccionEntrega", null); //puede ser null
                }
            }
            
            InputParameterAdd.DateTime(objCommand, "fechaSolicitud", pedido.fechaSolicitud);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde.Value);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta.Value);
            InputParameterAdd.Bit(objCommand, "esPagoContado", pedido.esPagoContado);

            InputParameterAdd.Int(objCommand, "facturaUnica", pedido.facturaUnica ? 1 : 0);

            DateTime dtTmp = DateTime.Now;
            String[] horaEntregaDesdeArray = pedido.horaEntregaDesde.Split(':');
            DateTime horaEntregaDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaDesdeArray[0]), Int32.Parse(horaEntregaDesdeArray[1]),0);
            String[] horaEntregaHastaArray = pedido.horaEntregaHasta.Split(':');
            DateTime horaEntregaHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaHastaArray[0]), Int32.Parse(horaEntregaHastaArray[1]),0);
            InputParameterAdd.DateTime(objCommand, "horaEntregaDesde", horaEntregaDesde);
            InputParameterAdd.DateTime(objCommand, "horaEntregaHasta", horaEntregaHasta);

            if (pedido.horaEntregaAdicionalDesde != null && !pedido.horaEntregaAdicionalDesde.Equals("")
                && pedido.horaEntregaAdicionalDesde != null && !pedido.horaEntregaAdicionalDesde.Equals(""))
            {
                String[] horaEntregaAdicionalDesdeArray = pedido.horaEntregaAdicionalDesde.Split(':');
                DateTime horaEntregaAdicionalDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaAdicionalDesdeArray[0]), Int32.Parse(horaEntregaAdicionalDesdeArray[1]), 0);
                String[] horaEntregaAdicionalHastaArray = pedido.horaEntregaAdicionalHasta.Split(':');
                DateTime horaEntregaAdicionalHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaAdicionalHastaArray[0]), Int32.Parse(horaEntregaAdicionalHastaArray[1]), 0);
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalDesde", horaEntregaAdicionalDesde);
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalHasta", horaEntregaAdicionalHasta);
            }
            else
            {
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalDesde", null);
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalHasta", null);
            }


            if (pedido.clasePedido == Pedido.ClasesPedido.Venta)
            {
                InputParameterAdd.Guid(objCommand, "idSolicitante", pedido.solicitante.idSolicitante);
                InputParameterAdd.Varchar(objCommand, "contactoPedido", pedido.solicitante.nombre);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", pedido.solicitante.telefono);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "correoContactoPedido", pedido.solicitante.correo);  //puede ser null
            }
            else if (pedido.clasePedido == Pedido.ClasesPedido.Compra)
            {
                InputParameterAdd.Guid(objCommand, "idSolicitante", null);
                InputParameterAdd.Varchar(objCommand, "contactoPedido", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "correoContactoPedido", null);  //puede ser null
            }
            else
            {
                if (pedido.tipoPedidoAlmacen == Pedido.tiposPedidoAlmacen.TrasladoInterno ||
                    pedido.tipoPedidoAlmacen == Pedido.tiposPedidoAlmacen.PrestamoEntregado 
                )
                {
                    InputParameterAdd.Guid(objCommand, "idSolicitante", pedido.solicitante.idSolicitante);
                    InputParameterAdd.Varchar(objCommand, "contactoPedido", pedido.solicitante.nombre);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", pedido.solicitante.telefono);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "correoContactoPedido", pedido.solicitante.correo);  //puede ser null
                }
                else
                {
                    InputParameterAdd.Guid(objCommand, "idSolicitante", null);
                    InputParameterAdd.Varchar(objCommand, "contactoPedido", null);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", null);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "correoContactoPedido", null);  //puede ser null
                }
            }

            InputParameterAdd.SmallInt(objCommand, "incluidoIGV", (short)(pedido.incluidoIGV?1:0));
            InputParameterAdd.Decimal(objCommand, "tasaIGV", Constantes.IGV);
            InputParameterAdd.Decimal(objCommand, "igv", pedido.montoIGV);
            InputParameterAdd.Decimal(objCommand, "total", pedido.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", pedido.observaciones);  //puede ser null
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            InputParameterAdd.Int(objCommand, "estadoCrediticio", (int)pedido.seguimientoCrediticioPedido.estado);
            InputParameterAdd.Varchar(objCommand, "observacionSeguimientoPedido", pedido.seguimientoPedido.observacion);
            InputParameterAdd.Varchar(objCommand, "observacionSeguimientoCrediticioPedido", pedido.seguimientoCrediticioPedido.observacion);
            InputParameterAdd.Varchar(objCommand, "ubigeoEntrega", pedido.ubigeoEntrega.Id);


            if (pedido.clasePedido == Pedido.ClasesPedido.Venta)
            {
                InputParameterAdd.Char(objCommand, "tipoPedido", ((char)pedido.tipoPedido).ToString());
                InputParameterAdd.Varchar(objCommand, "observacionesGuiaRemision", pedido.observacionesGuiaRemision);
                InputParameterAdd.Varchar(objCommand, "observacionesFactura", pedido.observacionesFactura);
            }
            else if (pedido.clasePedido == Pedido.ClasesPedido.Compra)
            {
                InputParameterAdd.Char(objCommand, "tipoPedido", ((char)pedido.tipoPedidoCompra).ToString());
                InputParameterAdd.Varchar(objCommand, "observacionesGuiaRemision", null);
                InputParameterAdd.Varchar(objCommand, "observacionesFactura", null);
            }
            else if (pedido.clasePedido == Pedido.ClasesPedido.Almacen)
            {
                InputParameterAdd.Char(objCommand, "tipoPedido", ((char)pedido.tipoPedidoAlmacen).ToString());
                InputParameterAdd.Varchar(objCommand, "observacionesGuiaRemision", pedido.observacionesGuiaRemision);
                InputParameterAdd.Varchar(objCommand, "observacionesFactura", null);
            }
           
            InputParameterAdd.Decimal(objCommand, "otrosCargos", pedido.otrosCargos);
            InputParameterAdd.Char(objCommand, "tipo", ((char)pedido.clasePedido).ToString());
            InputParameterAdd.Varchar(objCommand, "numeroRequerimiento", pedido.numeroRequerimiento);

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            OutputParameterAdd.BigInt(objCommand, "numero");
            ExecuteNonQuery(objCommand);

            pedido.idPedido = (Guid)objCommand.Parameters["@newId"].Value;
            pedido.numeroPedido = (Int64)objCommand.Parameters["@numero"].Value;


            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                pedidoDetalle.idPedido = pedido.idPedido;
                this.InsertPedidoDetalle(pedidoDetalle, pedido.usuario);
            }

            foreach (PedidoAdjunto pedidoAdjunto in pedido.pedidoAdjuntoList)
            {
                pedidoAdjunto.idPedido = pedido.idPedido;
                this.InsertPedidoAdjunto(pedidoAdjunto);
            }
            this.Commit();
        }
        

        public void UpdatePedido(Pedido pedido)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pu_pedido");

            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", pedido.numeroGrupoPedido); //puede ser null

            if (pedido.cotizacion.idCotizacion == Guid.Empty)
                InputParameterAdd.Guid(objCommand, "idCotizacion", null); //puede ser null
            else
                InputParameterAdd.Guid(objCommand, "idCotizacion", pedido.cotizacion.idCotizacion); //puede ser null

            if (pedido.promocion == null || pedido.promocion.idPromocion == Guid.Empty)
                InputParameterAdd.Guid(objCommand, "idPromocion", null); //puede ser null
            else
                InputParameterAdd.Guid(objCommand, "idPromocion", pedido.promocion.idPromocion); //puede ser null

            InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", pedido.numeroReferenciaCliente); //puede ser null
            InputParameterAdd.Varchar(objCommand, "moneda", pedido.moneda == null ? null : pedido.moneda.codigo);

            if (pedido.clasePedido == Pedido.ClasesPedido.Venta)
            {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", pedido.direccionEntrega.idDireccionEntrega); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", pedido.direccionEntrega.descripcion);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", pedido.direccionEntrega.contacto); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", pedido.direccionEntrega.telefono); //puede ser null
                InputParameterAdd.Varchar(objCommand, "codigoCliente", pedido.direccionEntrega.codigoCliente); //puede ser null
                InputParameterAdd.Varchar(objCommand, "codigoMP", pedido.direccionEntrega.codigoMP); //puede ser null
                InputParameterAdd.Varchar(objCommand, "nombre", pedido.direccionEntrega.nombre); //puede ser null
                InputParameterAdd.Varchar(objCommand, "observacionesDireccionEntrega", pedido.direccionEntrega.observaciones); //puede ser null
            }
            else if (pedido.clasePedido == Pedido.ClasesPedido.Compra)
            {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "codigoCliente", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "codigoMP", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "nombre", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "observacionesDireccionEntrega", null); //puede ser null
            }
            else
            {
                if (pedido.tipoPedidoAlmacen == Pedido.tiposPedidoAlmacen.TrasladoInterno ||
                    pedido.tipoPedidoAlmacen == Pedido.tiposPedidoAlmacen.PrestamoEntregado
                )
                {
                    InputParameterAdd.Guid(objCommand, "idDireccionEntrega", pedido.direccionEntrega.idDireccionEntrega); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "direccionEntrega", pedido.direccionEntrega.descripcion);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "contactoEntrega", pedido.direccionEntrega.contacto); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", pedido.direccionEntrega.telefono); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "codigoCliente", pedido.direccionEntrega.codigoCliente); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "codigoMP", pedido.direccionEntrega.codigoMP); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "nombre", pedido.direccionEntrega.nombre); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "observacionesDireccionEntrega", pedido.direccionEntrega.observaciones); //puede ser null
                }
                else
                {
                    InputParameterAdd.Guid(objCommand, "idDireccionEntrega", null); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "direccionEntrega", null);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "contactoEntrega", null); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", null); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "codigoCliente", null); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "codigoMP", null); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "nombre", null); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "observacionesDireccionEntrega", null); //puede ser null
                }
            }

            InputParameterAdd.DateTime(objCommand, "fechaSolicitud", pedido.fechaSolicitud);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde.Value);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta.Value);

            InputParameterAdd.Bit(objCommand, "esPagoContado", pedido.esPagoContado);
            InputParameterAdd.Int(objCommand, "facturaUnica", pedido.facturaUnica ? 1 : 0);

            DateTime dtTmp = DateTime.Now;
            String[] horaEntregaDesdeArray = pedido.horaEntregaDesde.Split(':');
            DateTime horaEntregaDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaDesdeArray[0]), Int32.Parse(horaEntregaDesdeArray[1]), 0);
            String[] horaEntregaHastaArray = pedido.horaEntregaHasta.Split(':');
            DateTime horaEntregaHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaHastaArray[0]), Int32.Parse(horaEntregaHastaArray[1]), 0);
            InputParameterAdd.DateTime(objCommand, "horaEntregaDesde", horaEntregaDesde);
            InputParameterAdd.DateTime(objCommand, "horaEntregaHasta", horaEntregaHasta);

            if (pedido.horaEntregaAdicionalDesde != null && !pedido.horaEntregaAdicionalDesde.Equals("")
                && pedido.horaEntregaAdicionalDesde != null && !pedido.horaEntregaAdicionalDesde.Equals(""))
            {
                String[] horaEntregaAdicionalDesdeArray = pedido.horaEntregaAdicionalDesde.Split(':');
                DateTime horaEntregaAdicionalDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaAdicionalDesdeArray[0]), Int32.Parse(horaEntregaAdicionalDesdeArray[1]), 0);
                String[] horaEntregaAdicionalHastaArray = pedido.horaEntregaAdicionalHasta.Split(':');
                DateTime horaEntregaAdicionalHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaAdicionalHastaArray[0]), Int32.Parse(horaEntregaAdicionalHastaArray[1]), 0);
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalDesde", horaEntregaAdicionalDesde);
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalHasta", horaEntregaAdicionalHasta);
            } else
            {
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalDesde", null);
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalHasta", null);
            }


            if (pedido.clasePedido == Pedido.ClasesPedido.Venta)
            {
                InputParameterAdd.Guid(objCommand, "idSolicitante", pedido.solicitante.idSolicitante);
                InputParameterAdd.Varchar(objCommand, "contactoPedido", pedido.solicitante.nombre);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", pedido.solicitante.telefono);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "correoContactoPedido", pedido.solicitante.correo);  //puede ser null
            }
            else if (pedido.clasePedido == Pedido.ClasesPedido.Compra)
            {
                InputParameterAdd.Guid(objCommand, "idSolicitante", null);
                InputParameterAdd.Varchar(objCommand, "contactoPedido", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "correoContactoPedido", null);  //puede ser null
            }
            else
            {
                if (pedido.tipoPedidoAlmacen == Pedido.tiposPedidoAlmacen.TrasladoInterno ||
                    pedido.tipoPedidoAlmacen == Pedido.tiposPedidoAlmacen.PrestamoEntregado
                )
                {
                    InputParameterAdd.Guid(objCommand, "idSolicitante", pedido.solicitante.idSolicitante);
                    InputParameterAdd.Varchar(objCommand, "contactoPedido", pedido.solicitante.nombre);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", pedido.solicitante.telefono);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "correoContactoPedido", pedido.solicitante.correo);  //puede ser null
                }
                else
                {
                    InputParameterAdd.Guid(objCommand, "idSolicitante", null);
                    InputParameterAdd.Varchar(objCommand, "contactoPedido", null);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", null);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "correoContactoPedido", null);  //puede ser null
                }
            }

            InputParameterAdd.SmallInt(objCommand, "incluidoIGV", (short)(pedido.incluidoIGV ? 1 : 0));
            InputParameterAdd.Decimal(objCommand, "tasaIGV", Constantes.IGV);
            InputParameterAdd.Decimal(objCommand, "igv", pedido.montoIGV);
            InputParameterAdd.Decimal(objCommand, "total", pedido.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", pedido.observaciones);  //puede ser null
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            InputParameterAdd.Int(objCommand, "estadoCrediticio", (int)pedido.seguimientoCrediticioPedido.estado);
            InputParameterAdd.Varchar(objCommand, "observacionSeguimientoPedido", pedido.seguimientoPedido.observacion);
            InputParameterAdd.Varchar(objCommand, "observacionSeguimientoCrediticioPedido", pedido.seguimientoCrediticioPedido.observacion);
            if (pedido.clasePedido == Pedido.ClasesPedido.Venta)
            {
                InputParameterAdd.Char(objCommand, "tipoPedido", ((char)pedido.tipoPedido).ToString());
                InputParameterAdd.Varchar(objCommand, "observacionesGuiaRemision", pedido.observacionesGuiaRemision);
                InputParameterAdd.Varchar(objCommand, "observacionesFactura", pedido.observacionesFactura);
            }
            else if (pedido.clasePedido == Pedido.ClasesPedido.Compra)
            {
                InputParameterAdd.Char(objCommand, "tipoPedido", ((char)pedido.tipoPedidoCompra).ToString());
                InputParameterAdd.Varchar(objCommand, "observacionesGuiaRemision", null);
                InputParameterAdd.Varchar(objCommand, "observacionesFactura", null);
            }
            else if (pedido.clasePedido == Pedido.ClasesPedido.Almacen)
            {
                InputParameterAdd.Char(objCommand, "tipoPedido", ((char)pedido.tipoPedidoAlmacen).ToString());
                InputParameterAdd.Varchar(objCommand, "observacionesGuiaRemision", pedido.observacionesGuiaRemision);
                InputParameterAdd.Varchar(objCommand, "observacionesFactura", null);
            }
            InputParameterAdd.Varchar(objCommand, "ubigeoEntrega", pedido.ubigeoEntrega.Id);
            InputParameterAdd.Decimal(objCommand, "otrosCargos", pedido.otrosCargos);
            InputParameterAdd.Varchar(objCommand, "numeroRequerimiento", pedido.numeroRequerimiento);
            

            ExecuteNonQuery(objCommand);


            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                pedidoDetalle.idPedido = pedido.idPedido;
                //pedidoDetalle.usuario = pedido.usuario;
                this.InsertPedidoDetalle(pedidoDetalle, pedido.usuario);
            }

            foreach (PedidoAdjunto pedidoAdjunto in pedido.pedidoAdjuntoList)
            {
                pedidoAdjunto.idPedido = pedido.idPedido;
                this.InsertPedidoAdjunto(pedidoAdjunto);
            }
            this.Commit();
        }           
    
        #endregion


        #region General

        public void ActualizarPedido(Pedido pedido)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pu_actualizarPedido");
            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", pedido.numeroReferenciaCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaAdicional", pedido.numeroReferenciaAdicional);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaExtendida", !pedido.fechaEntregaExtendida.HasValue ? null: pedido.fechaEntregaExtendida);
            InputParameterAdd.Varchar(objCommand, "observaciones", pedido.observaciones);
            InputParameterAdd.Varchar(objCommand, "observacionesAlmacen", pedido.observacionesAlmacen);
            InputParameterAdd.Varchar(objCommand, "observacionesGuiaRemision", pedido.observacionesGuiaRemision);
            InputParameterAdd.Varchar(objCommand, "observacionesFactura", pedido.observacionesFactura);
            InputParameterAdd.BigInt(objCommand, "numeroGrupoPedido", pedido.numeroGrupoPedido);
            InputParameterAdd.Varchar(objCommand, "numeroRequerimiento", pedido.numeroRequerimiento);
            InputParameterAdd.Int(objCommand, "facturaUnica", pedido.facturaUnica ? 1 : 0);

            ExecuteNonQuery(objCommand);

            foreach (PedidoAdjunto pedidoAdjunto in pedido.pedidoAdjuntoList)
            {
                pedidoAdjunto.usuario = pedido.usuario;
                pedidoAdjunto.idPedido = pedido.idPedido;
                pedidoAdjunto.idCliente = pedido.cliente.idCliente;
                InsertPedidoAdjunto(pedidoAdjunto);
            }
            this.Commit();
        }

        public void InsertPedidoAdjunto(PedidoAdjunto pedidoAdjunto)
        {
            var objCommand = GetSqlCommand("pi_pedidoAdjunto");
            InputParameterAdd.Guid(objCommand, "idPedido", pedidoAdjunto.idPedido);
            InputParameterAdd.Guid(objCommand, "idArchivoAdjunto", pedidoAdjunto.idArchivoAdjunto);
            InputParameterAdd.Guid(objCommand, "idCliente", pedidoAdjunto.idCliente);
            InputParameterAdd.Varchar(objCommand, "nombre", pedidoAdjunto.nombre);
            InputParameterAdd.VarBinary(objCommand, "adjunto", pedidoAdjunto.adjunto);

            InputParameterAdd.Guid(objCommand, "idUsuario", pedidoAdjunto.usuario.idUsuario);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);

            pedidoAdjunto.idArchivoAdjunto = (Guid)objCommand.Parameters["@newId"].Value;
        }

        public void InsertPedidoDetalle(PedidoDetalle pedidoDetalle, Usuario usuario)
        {
            var objCommand = GetSqlCommand("pi_pedidoDetalle");
            InputParameterAdd.Guid(objCommand, "idPedido", pedidoDetalle.idPedido);
            InputParameterAdd.Guid(objCommand, "idProducto", pedidoDetalle.producto.idProducto);
            InputParameterAdd.Int(objCommand, "cantidad", pedidoDetalle.cantidad);
            //Siempre se almacena el precio sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "precioSinIGV", pedidoDetalle.producto.precioSinIgv);
            //Siempre se almacena el costo sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "costoSinIGV", pedidoDetalle.producto.costoSinIgv);
            if(pedidoDetalle.esPrecioAlternativo)
                InputParameterAdd.Decimal(objCommand, "equivalencia", pedidoDetalle.ProductoPresentacion.Equivalencia);            
            else
                InputParameterAdd.Decimal(objCommand, "equivalencia", 1);
            InputParameterAdd.Varchar(objCommand, "unidad", pedidoDetalle.unidad);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", pedidoDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "precioNeto", pedidoDetalle.precioNeto);
            InputParameterAdd.Int(objCommand, "esPrecioAlternativo", pedidoDetalle.esPrecioAlternativo?1:0);
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Decimal(objCommand, "flete", pedidoDetalle.flete);
            InputParameterAdd.Varchar(objCommand, "observaciones", pedidoDetalle.observacion);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            if (pedidoDetalle.esPrecioAlternativo)
            {
                InputParameterAdd.Int(objCommand, "idProductoPresentacion", pedidoDetalle.ProductoPresentacion.IdProductoPresentacion);
            }
            else
            {
                InputParameterAdd.Int(objCommand, "idProductoPresentacion", 0);
            }

            InputParameterAdd.Int(objCommand, "indicadorAprobacion", (int)pedidoDetalle.indicadorAprobacion);

            ExecuteNonQuery(objCommand);
          
            pedidoDetalle.idPedidoDetalle = (Guid)objCommand.Parameters["@newId"].Value;
        }

        public Pedido ProgramarPedido(Pedido pedido, Usuario usuario)
        {
            var objCommand = GetSqlCommand("pu_programarPedido");

            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.DateTime(objCommand, "fechaProgramacion", pedido.fechaProgramacion);
            InputParameterAdd.Varchar(objCommand, "comentarioProgramacion", pedido.comentarioProgramacion);
            ExecuteNonQuery(objCommand);
            return pedido;
        }


        public Cotizacion obtenerProductosAPartirdePreciosRegistrados(Cotizacion cotizacion, String familia, String proveedor, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_generarPlantillaCotizacion");
            InputParameterAdd.Guid(objCommand, "idCliente", cotizacion.cliente.idCliente);
            InputParameterAdd.DateTime(objCommand, "fecha", cotizacion.fechaPrecios);
            InputParameterAdd.Varchar(objCommand, "familia", familia);
            InputParameterAdd.Varchar(objCommand, "proveedor", proveedor);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable cotizacionDataTable = dataSet.Tables[0];
            DataTable cotizacionDetalleDataTable = dataSet.Tables[1];

            //DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            foreach (DataRow row in cotizacionDataTable.Rows)
            {

                //No se cuenta con IdCotizacion
                /*       cotizacion.fecha = DateTime.Now;
                       cotizacion.fechaLimiteValidezOferta = DateTime.Now.AddDays(Constantes.PLAZO_OFERTA_DIAS);
                       cotizacion.fechaInicioVigenciaPrecios = null;
                       cotizacion.fechaFinVigenciaPrecios = null;
                       cotizacion.incluidoIgv = false;
                       cotizacion.considerarCantidades = Cotizacion.OpcionesConsiderarCantidades.Cantidades;
                       cotizacion.mostrarValidezOfertaEnDias = 1;
                       cotizacion.flete = 0;
                       cotizacion.igv = Constantes.IGV;
                       cotizacion.contacto = Converter.GetString(row, "contacto");
                       cotizacion.observaciones = Constantes.OBSERVACION;
                       cotizacion.mostrarCodigoProveedor = true;
                       cotizacion.fechaModificacion = DateTime.Now;


                       ///Falta agregar la búsqueda con Grupo
                       cotizacion.grupo = new Grupo();
                       Guid idCliente = cotizacion.cliente.idCliente;
                       cotizacion.cliente = new Cliente();
                       cotizacion.cliente.codigo = Converter.GetString(row, "codigo");
                       cotizacion.cliente.idCliente = idCliente;
                       cotizacion.cliente.razonSocial = Converter.GetString(row, "razon_social");
                       cotizacion.cliente.ruc = Converter.GetString(row, "ruc");


                       cotizacion.ciudad = new Ciudad();
                       cotizacion.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                       cotizacion.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");
                       cotizacion.seguimientoCotizacion = new SeguimientoCotizacion();
                       */
            }


            cotizacion.cotizacionDetalleList = new List<CotizacionDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in cotizacionDetalleDataTable.Rows)
            {
                CotizacionDetalle cotizacionDetalle = new CotizacionDetalle(usuario.visualizaCostos, usuario.visualizaMargen);
                cotizacionDetalle.producto = new Producto();


                //No se cuenta con IdCotizacionDetalle
                cotizacionDetalle.cantidad = 1;
                
                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");

                if (cotizacionDetalle.esPrecioAlternativo)
                {
                    cotizacionDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                }



                cotizacionDetalle.flete = Converter.GetDecimal(row, "flete");


                cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");



                //if (cotizacionDetalle.esPrecioAlternativo)
                // {
                cotizacionDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * cotizacionDetalle.ProductoPresentacion.Equivalencia;
                cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.precioNeto * 100 / cotizacionDetalle.producto.precioSinIgv);
                /*}
                else
                {
                    cotizacionDetalle.precioNetoEquivalente = Converter.GetDecimal(row, "precio_neto");
                    cotizacionDetalle.porcentajeDescuento = 100 - (cotizacionDetalle.producto.precioSinIgv * 100 / cotizacionDetalle.precioNetoEquivalente);
                }*/


                cotizacionDetalle.flete = Converter.GetDecimal(row, "flete");
                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");
                cotizacionDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                cotizacionDetalle.producto.sku = Converter.GetString(row, "sku");
                cotizacionDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                cotizacionDetalle.producto.topeDescuento = Converter.GetDecimal(row, "tope_descuento");
                cotizacionDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                cotizacionDetalle.producto.proveedor = Converter.GetString(row, "proveedor");
                cotizacionDetalle.producto.image = Converter.GetBytes(row, "imagen");


                cotizacionDetalle.observacion = null;

                cotizacion.cotizacionDetalleList.Add(cotizacionDetalle);
            }

            //POR REVISAR
            //  cotizacion.montoTotal = cotizacion.cotizacionDetalleList.AsEnumerable().Sum(o => o.subTotal);
            //  cotizacion.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, cotizacion.montoTotal / (1 + cotizacion.igv)));
            //cotizacion.montoIGV = cotizacion.montoTotal - cotizacion.montoSubTotal;

            return cotizacion;
        }


        public Int64 SelectSiguienteNumeroGrupoPedido()
        {
            var objCommand = GetSqlCommand("ps_siguienteNumeroGrupoPedido");
            OutputParameterAdd.BigInt(objCommand, "numeroGrupo");
            ExecuteNonQuery(objCommand);
            return (Int64)objCommand.Parameters["@numeroGrupo"].Value;
        }

        public List<Pedido> SelectPedidos(Pedido pedido)
        {
            var objCommand = GetSqlCommand("ps_pedidos");
            InputParameterAdd.BigInt(objCommand, "numero", pedido.numeroPedido);
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", pedido.numeroGrupoPedido);
            InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            InputParameterAdd.Guid(objCommand, "idUsuarioBusqueda", pedido.usuarioBusqueda.idUsuario);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", pedido.numeroReferenciaCliente);
            InputParameterAdd.Bit(objCommand, "buscaSedesGrupoCliente", pedido.buscarSedesGrupoCliente);
            InputParameterAdd.Int(objCommand, "idGrupoCliente", pedido.idGrupoCliente);
            InputParameterAdd.Int(objCommand, "truncado", pedido.truncado);

            switch (pedido.clasePedido)
            {
                case Pedido.ClasesPedido.Venta: InputParameterAdd.Char(objCommand, "tipoPedido", ((Char)pedido.tipoPedidoVentaBusqueda).ToString()); break;
                case Pedido.ClasesPedido.Compra: InputParameterAdd.Char(objCommand, "tipoPedido", ((Char)pedido.tipoPedidoCompraBusqueda).ToString()); break;
                case Pedido.ClasesPedido.Almacen: InputParameterAdd.Char(objCommand, "tipoPedido", ((Char)pedido.tipoPedidoAlmacenBusqueda).ToString()); break;
            }

            InputParameterAdd.DateTime(objCommand, "fechaCreacionDesde", new DateTime(pedido.fechaCreacionDesde.Year, pedido.fechaCreacionDesde.Month, pedido.fechaCreacionDesde.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaCreacionHasta", new DateTime(pedido.fechaCreacionHasta.Year, pedido.fechaCreacionHasta.Month, pedido.fechaCreacionHasta.Day, 23, 59, 59));
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde == null ? pedido.fechaEntregaDesde : new DateTime(pedido.fechaEntregaDesde.Value.Year, pedido.fechaEntregaDesde.Value.Month, pedido.fechaEntregaDesde.Value.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta == null ? pedido.fechaEntregaDesde : new DateTime(pedido.fechaEntregaHasta.Value.Year, pedido.fechaEntregaHasta.Value.Month, pedido.fechaEntregaHasta.Value.Day, 23, 59, 59));
            InputParameterAdd.DateTime(objCommand, "fechaProgramacionDesde", pedido.fechaProgramacionDesde == null ? pedido.fechaProgramacionDesde : new DateTime(pedido.fechaProgramacionDesde.Value.Year, pedido.fechaProgramacionDesde.Value.Month, pedido.fechaProgramacionDesde.Value.Day, 0, 0, 0));  //pedido.fechaProgramacionDesde);
            InputParameterAdd.DateTime(objCommand, "fechaProgramacionHasta", pedido.fechaProgramacionHasta == null ? pedido.fechaProgramacionHasta : new DateTime(pedido.fechaProgramacionHasta.Value.Year, pedido.fechaProgramacionHasta.Value.Month, pedido.fechaProgramacionHasta.Value.Day, 0, 0, 0));  //pedido.fechaProgramacionDesde); //pedido.fechaProgramacionHasta);
            InputParameterAdd.Char(objCommand, "tipo", ((char)pedido.clasePedido).ToString());
            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            InputParameterAdd.Int(objCommand, "estadoCrediticio", (int)pedido.seguimientoCrediticioPedido.estado);
            InputParameterAdd.Varchar(objCommand, "sku", pedido.sku);
            DataTable dataTable = Execute(objCommand);

            List<Pedido> pedidoList = new List<Pedido>();

            foreach (DataRow row in dataTable.Rows)
            {
                pedido = new Pedido(pedido.clasePedido);
                pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo_pedido");
                pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                pedido.horaEntregaAdicionalDesde = Converter.GetString(row, "hora_entrega_adicional_desde");
                pedido.horaEntregaAdicionalHasta = Converter.GetString(row, "hora_entrega_adicional_hasta");
                pedido.fechaEntregaExtendida = Converter.GetDateTimeNullable(row, "fecha_entrega_extendida");
                pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                pedido.truncado = Converter.GetInt(row, "truncado");

                pedido.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                //pedido.FechaRegistro = pedido.FechaRegistro.AddHours(-5);
                pedido.stockConfirmado = Converter.GetInt(row, "stock_confirmado");
                /*if (row["fecha_programacion"] == DBNull.Value)
                    pedido.fechaProgramacion = null;
                else*/
                pedido.fechaProgramacion = Converter.GetDateTimeNullable(row, "fecha_programacion");
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

                //pedido.cliente.tipoLiberacionCrediticia = (Persona.TipoLiberacionCrediticia)Converter.GetInt(row, "estado_liberacion_creditica"); 

                pedido.cliente.grupoCliente = new GrupoCliente();
                pedido.cliente.grupoCliente.nombre = Converter.GetString(row, "nombre_grupo");

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

                pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
                pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Converter.GetInt(row, "estado_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.observacion = Converter.GetString(row, "observacion_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.usuario = new Usuario();
                pedido.seguimientoCrediticioPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento_Crediticio");

                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "codigo_ubigeo");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                pedidoList.Add(pedido);
            }
            return pedidoList;
        }


        public Pedido SelectPedido(Pedido pedido, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_pedido");
            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable pedidoDataTable = dataSet.Tables[0];
            DataTable pedidoDetalleDataTable = dataSet.Tables[1];
            DataTable direccionEntregaDataTable = dataSet.Tables[2];
            DataTable movimientoAlmacenDataTable = dataSet.Tables[3];
            DataTable pedidoAdjuntoDataTable = dataSet.Tables[4];
            DataTable solicitanteDataTable = dataSet.Tables[5];
            DataTable pedidoGrupoDataTable = dataSet.Tables[6];
            DataTable preciosEspecialesDataTable = dataSet.Tables[7];

            //   DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            foreach (DataRow row in pedidoDataTable.Rows)
            {
                pedido.numeroPedido = Converter.GetLong(row, "numero");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo");
                pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                pedido.horaEntregaAdicionalDesde = Converter.GetString(row, "hora_entrega_adicional_desde");
                pedido.horaEntregaAdicionalHasta = Converter.GetString(row, "hora_entrega_adicional_hasta");

                pedido.moneda = new Moneda();
                pedido.moneda.codigo = Converter.GetString(row, "moneda");

                pedido.moneda = Moneda.ListaMonedasFija.Where(m => m.codigo == pedido.moneda.codigo).FirstOrDefault();

                pedido.fechaEntregaExtendida = Converter.GetDateTimeNullable(row, "fecha_entrega_extendida");
                pedido.truncado = Converter.GetInt(row, "truncado");

                pedido.stockConfirmado = Converter.GetInt(row, "stock_confirmado");
                pedido.incluidoIGV = Converter.GetBool(row, "incluido_igv");
                pedido.montoIGV = Converter.GetDecimal(row, "igv");
                pedido.montoTotal = Converter.GetDecimal(row, "total");
                pedido.observaciones = Converter.GetString(row, "observaciones");
                pedido.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedido.montoTotal - pedido.montoIGV));
                pedido.fechaModificacion = Converter.GetDateTime(row, "fecha_modificacion");
                pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                pedido.direccionEntrega = new DireccionEntrega();
                pedido.direccionEntrega.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega");
                pedido.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");
                pedido.direccionEntrega.contacto = Converter.GetString(row, "contacto_entrega");
                pedido.direccionEntrega.telefono = Converter.GetString(row, "telefono_contacto_entrega");
                pedido.direccionEntrega.codigoCliente = Converter.GetString(row, "direccion_entrega_codigo_cliente");
                pedido.direccionEntrega.codigoMP = Converter.GetString(row, "direccion_entrega_codigo_mp");
                pedido.direccionEntrega.nombre = Converter.GetString(row, "direccion_entrega_nombre");

                pedido.solicitante = new Solicitante();
                pedido.solicitante.idSolicitante = Converter.GetGuid(row, "id_solicitante");

                pedido.esPagoContado = Converter.GetBool(row, "es_pago_contado"); 

                pedido.contactoPedido = Converter.GetString(row, "contacto_pedido");
                pedido.telefonoContactoPedido = Converter.GetString(row, "telefono_contacto_pedido");
                pedido.correoContactoPedido = Converter.GetString(row, "correo_contacto_pedido");

                pedido.numeroRequerimiento = Converter.GetString(row, "numero_requerimiento");

                pedido.facturaUnica = Converter.GetInt(row, "factura_unica") == 1 ? true : false;

                pedido.solicitante.nombre = pedido.contactoPedido;
                pedido.solicitante.telefono = pedido.telefonoContactoPedido;
                pedido.solicitante.correo = pedido.correoContactoPedido;

                pedido.fechaProgramacion = Converter.GetDateTime(row, "fecha_programacion");
                pedido.observacionesAlmacen = Converter.GetString(row, "observaciones_almacen");
                pedido.observacionesFactura = Converter.GetString(row, "observaciones_factura");
                pedido.observacionesGuiaRemision = Converter.GetString(row, "observaciones_guia_remision");

                pedido.clasePedido = (Pedido.ClasesPedido)Char.Parse(Converter.GetString(row, "tipo"));
                if (pedido.clasePedido == Pedido.ClasesPedido.Venta)
                    pedido.tipoPedido = (Pedido.tiposPedido)Char.Parse(Converter.GetString(row, "tipo_pedido"));
                else if (pedido.clasePedido == Pedido.ClasesPedido.Compra)
                    pedido.tipoPedidoCompra = (Pedido.tiposPedidoCompra)Char.Parse(Converter.GetString(row, "tipo_pedido"));
                else
                    pedido.tipoPedidoAlmacen = (Pedido.tiposPedidoAlmacen)Char.Parse(Converter.GetString(row, "tipo_pedido"));

                pedido.otrosCargos = Converter.GetDecimal(row, "otros_cargos");
                pedido.numeroReferenciaAdicional = Converter.GetString(row, "numero_referencia_adicional");

                pedido.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                
                /* pedido.venta = new Venta();
                   pedido.venta.igv = Converter.GetDecimal(row, "igv_venta");
                   pedido.venta.subTotal = Converter.GetDecimal(row, "sub_total_venta");
                   pedido.venta.total = Converter.GetDecimal(row, "total_venta");
                   pedido.venta.idVenta = Converter.GetGuid(row, "id_Venta");*/

                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                pedido.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                pedido.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                pedido.documentoVenta = new DocumentoVenta();
                pedido.documentoVenta.serie = Converter.GetString(row, "serie_factura");
                pedido.documentoVenta.numero = Converter.GetString(row, "numero_factura");

                pedido.cotizacion = new Cotizacion();
                pedido.cotizacion.idCotizacion = Converter.GetGuid(row, "id_cotizacion");
                pedido.cotizacion.codigo = Converter.GetLong(row, "cotizacion_codigo");
                pedido.cotizacion.tipoCotizacion = (Cotizacion.TiposCotizacion)Converter.GetInt(row, "tipo_cotizacion");

                Guid idOrdenCompracliente = Converter.GetGuid(row, "id_orden_compra_cliente");
                if (idOrdenCompracliente != null && idOrdenCompracliente != Guid.Empty)
                {
                    pedido.ordenCompracliente = new OrdenCompraCliente();
                    pedido.ordenCompracliente.idOrdenCompraCliente = idOrdenCompracliente;
                    pedido.ordenCompracliente.numeroReferenciaCliente = pedido.numeroReferenciaCliente;
                }


                pedido.cliente = new Cliente();
                pedido.cliente.codigo = Converter.GetString(row, "codigo");
                pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                pedido.cliente.ruc = Converter.GetString(row, "ruc");
                pedido.cliente.ciudad = new Ciudad();
                pedido.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad_cliente");
                pedido.cliente.ciudad.nombre = Converter.GetString(row, "nombre_ciudad_cliente");
                pedido.cliente.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                pedido.cliente.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                pedido.cliente.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                pedido.cliente.plazoCredito = Converter.GetString(row, "plazo_credito");
                pedido.cliente.plazoCreditoSolicitado = (DocumentoVenta.TipoPago)Converter.GetInt(row, "plazo_credito_solicitado");
                pedido.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                pedido.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");

                /*Vendedores*/
                pedido.cliente.responsableComercial = new Vendedor();
                pedido.cliente.responsableComercial.codigo = Converter.GetString(row, "responsable_comercial_codigo");
                pedido.cliente.responsableComercial.descripcion = Converter.GetString(row, "responsable_comercial_descripcion");
                pedido.cliente.responsableComercial.usuario = new Usuario();
                pedido.cliente.responsableComercial.usuario.email = Converter.GetString(row, "responsable_comercial_email");

                pedido.cliente.supervisorComercial = new Vendedor();
                pedido.cliente.supervisorComercial.codigo = Converter.GetString(row, "supervisor_comercial_codigo");
                pedido.cliente.supervisorComercial.descripcion = Converter.GetString(row, "supervisor_comercial_descripcion");
                pedido.cliente.supervisorComercial.usuario = new Usuario();
                pedido.cliente.supervisorComercial.usuario.email = Converter.GetString(row, "supervisor_comercial_email");

                pedido.cliente.asistenteServicioCliente = new Vendedor();
                pedido.cliente.asistenteServicioCliente.codigo = Converter.GetString(row, "asistente_servicio_cliente_codigo");
                pedido.cliente.asistenteServicioCliente.descripcion = Converter.GetString(row, "asistente_servicio_cliente_descripcion");
                pedido.cliente.asistenteServicioCliente.usuario = new Usuario();
                pedido.cliente.asistenteServicioCliente.usuario.email = Converter.GetString(row, "asistente_servicio_cliente_email");

                pedido.cliente.grupoCliente = new GrupoCliente();
                pedido.cliente.grupoCliente.nombre = Converter.GetString(row, "grupo_nombre");

                pedido.promocion = new Promocion();
                pedido.promocion.idPromocion = Converter.GetGuid(row, "id_promocion");
                pedido.promocion.codigo = Converter.GetString(row, "codigo_promocion");
                pedido.promocion.titulo = Converter.GetString(row, "titulo_promocion");
                pedido.promocion.fechaInicio = Converter.GetDateTime(row, "fecha_inicio_promocion");
                pedido.promocion.fechaFin = Converter.GetDateTime(row, "fecha_fin_promocion");
                pedido.promocion.descripcion = Converter.GetString(row, "descripcion_promocion");

                pedido.ciudad = new Ciudad();
                pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");
                pedido.ciudad.direccionPuntoLlegada = Converter.GetString(row, "direccion_establecimiento");
                pedido.ciudad.esProvincia = Converter.GetBool(row, "es_provincia");
                pedido.ciudad.sede = Converter.GetString(row, "codigo_sede");

                pedido.usuario = new Usuario();
                pedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_creacion");
                pedido.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                pedido.usuario.cargo = Converter.GetString(row, "cargo");
                pedido.usuario.firmaImagen = Converter.GetBytes(row, "usuario_firma_imagen");
                pedido.usuario.contacto = Converter.GetString(row, "contacto_usuario");
                pedido.usuario.email = Converter.GetString(row, "email");
                pedido.IdUsuarioRegistro = Converter.GetGuid(row, "id_usuario_creacion");

                pedido.UsuarioRegistro = usuario;

                pedido.seguimientoPedido = new SeguimientoPedido();
                pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Converter.GetInt(row, "estado_seguimiento");
                pedido.seguimientoPedido.observacion = Converter.GetString(row, "observacion_seguimiento");
                pedido.seguimientoPedido.usuario = new Usuario();
                pedido.seguimientoPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                pedido.seguimientoPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");

                pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
                pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Converter.GetInt(row, "estado_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.observacion = Converter.GetString(row, "observacion_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.usuario = new Usuario();
                pedido.seguimientoCrediticioPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento_crediticio");

                
            }


            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in pedidoDetalleDataTable.Rows)
            {
                PedidoDetalle pedidoDetalle = new PedidoDetalle(usuario.visualizaCostos, usuario.visualizaMargen);

                pedidoDetalle.producto = new Producto();

                pedidoDetalle.idPedidoDetalle = Converter.GetGuid(row, "id_pedido_detalle");
                pedidoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                pedidoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");

                pedidoDetalle.cantidadPermitida = Converter.GetInt(row, "cantidad_permitida");
                pedidoDetalle.observacionRestriccion = Converter.GetString(row, "comentario_retencion");

                pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");

                pedidoDetalle.flete = Converter.GetDecimal(row, "flete");


                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (pedidoDetalle.esPrecioAlternativo)
                {
                   /* pedidoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    pedidoDetalle.ProductoPresentacion.Equivalencia = pedidoDetalle.producto.equivalencia;
                    */
                    pedidoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    pedidoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                    pedidoDetalle.ProductoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.ProductoPresentacion.Equivalencia;
                }
                else
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                }

                pedidoDetalle.unidad = Converter.GetString(row, "unidad");

                pedidoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                pedidoDetalle.producto.sku = Converter.GetString(row, "sku");
                pedidoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                pedidoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                pedidoDetalle.producto.descripcionLarga = Converter.GetString(row, "descripcion_larga");
                pedidoDetalle.producto.proveedor = Converter.GetString(row, "proveedor");
                pedidoDetalle.producto.tipoProducto = (Producto.TipoProducto)Converter.GetInt(row, "tipo_producto");
                pedidoDetalle.producto.ventaRestringida = (Producto.TipoVentaRestringida) Converter.GetInt(row, "descontinuado");
                pedidoDetalle.producto.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");
                pedidoDetalle.producto.compraRestringida = Converter.GetInt(row, "compra_restringida");
                pedidoDetalle.producto.topeDescuento = Converter.GetDecimal(row, "tope_descuento");
                pedidoDetalle.producto.costoOriginal = Converter.GetDecimal(row, "costo_original");
                pedidoDetalle.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                pedidoDetalle.producto.costoFleteProvincias = Converter.GetDecimal(row, "costo_flete_provincias");
                pedidoDetalle.producto.monedaFleteProvincias = Moneda.ListaMonedasFija.Where(m => m.codigo.Equals(Converter.GetString(row, "moneda_flete_provincias"))).First();

                pedidoDetalle.producto.image = Converter.GetBytes(row, "imagen");

                pedidoDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                pedidoDetalle.observacion = Converter.GetString(row, "observaciones");
                


                PrecioClienteProducto precioClienteProducto = new PrecioClienteProducto();

                precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto_vigente");
                precioClienteProducto.flete = Converter.GetDecimal(row, "flete_vigente");
                precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario_vigente");
                precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia_vigente");

                precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");

                precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");

             /*   if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    precioClienteProducto.fechaFinVigencia = null;
                }
                else
                {*/
                    precioClienteProducto.fechaFinVigencia = Converter.GetDateTimeNullable(row, "fecha_fin_vigencia");
                //}

                //  precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                precioClienteProducto.cliente = new Cliente();
                precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedidoDetalle.producto.precioClienteProducto = precioClienteProducto;


                pedidoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                pedidoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                pedidoDetalle.indicadorAprobacion = (PedidoDetalle.IndicadorAprobacion)Converter.GetShort(row, "indicador_aprobacion");

                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }

            List<DireccionEntrega> direccionEntregaList = new List<DireccionEntrega>();

            foreach (DataRow row in direccionEntregaDataTable.Rows)
            {
                DireccionEntrega obj = new DireccionEntrega
                {
                    idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega"),
                    descripcion = Converter.GetString(row, "descripcion"),
                    contacto = Converter.GetString(row, "contacto"),
                    telefono = Converter.GetString(row, "telefono"),
                    ubigeo = new Ubigeo { Id = Converter.GetString(row, "ubigeo") }
                };
                direccionEntregaList.Add(obj);
            }

            pedido.cliente.direccionEntregaList = direccionEntregaList;


            List<Solicitante> solicitanteList = new List<Solicitante>();

            foreach (DataRow row in solicitanteDataTable.Rows)
            {
                Solicitante obj = new Solicitante
                {
                    idSolicitante = Converter.GetGuid(row, "id_solicitante"),
                    nombre = Converter.GetString(row, "nombre"),
                    telefono = Converter.GetString(row, "telefono"),
                    correo = Converter.GetString(row, "correo")
                };
                solicitanteList.Add(obj);
            }

            pedido.cliente.solicitanteList = solicitanteList;


            pedido.guiaRemisionList = new List<GuiaRemision>();

            GuiaRemision movimientoAlmacen = new GuiaRemision();
            movimientoAlmacen.idMovimientoAlmacen = Guid.Empty;

            foreach (DataRow row in movimientoAlmacenDataTable.Rows)
            {
                Guid idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                if (movimientoAlmacen.idMovimientoAlmacen != idMovimientoAlmacen)
                {
                    //Si no coincide con el anterior se crea un nuevo movimiento Almacen
                    movimientoAlmacen = new GuiaRemision();
                    movimientoAlmacen.idMovimientoAlmacen = idMovimientoAlmacen;
                    movimientoAlmacen.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                    movimientoAlmacen.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                    movimientoAlmacen.numeroDocumento = Converter.GetInt(row, "numero_documento");
                    movimientoAlmacen.serieDocumento = Converter.GetString(row, "serie_documento");
                    movimientoAlmacen.documentoDetalle = new List<DocumentoDetalle>();

                    movimientoAlmacen.documentoVenta = new DocumentoVenta();
                    movimientoAlmacen.documentoVenta.idDocumentoVenta = Converter.GetGuid(row, "id_documento_venta");
                    movimientoAlmacen.documentoVenta.serie = Converter.GetString(row, "SERIE");
                    movimientoAlmacen.documentoVenta.numero = Converter.GetString(row, "CORRELATIVO");



                    /*if (row["fecha_emision_factura"] == DBNull.Value)
                        movimientoAlmacen.documentoVenta.fechaEmision = null;
                    else*/

                    movimientoAlmacen.documentoVenta.fechaEmision = Converter.GetDateTimeNullable(row, "fecha_emision_factura");



                    pedido.guiaRemisionList.Add(movimientoAlmacen);
                }

                DocumentoDetalle documentoDetalle = new DocumentoDetalle();
                documentoDetalle.idDocumentoDetalle = Converter.GetGuid(row, "id_movimiento_almacen_detalle");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                documentoDetalle.producto = new Producto();
                documentoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                documentoDetalle.producto.sku = Converter.GetString(row, "sku");
                documentoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                documentoDetalle.unidad = Converter.GetString(row, "unidad");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");

                movimientoAlmacen.documentoDetalle.Add(documentoDetalle);
            }


            /*mad.id_movimiento_almacen_detalle, mad.cantidad, 
            mad.unidad, pr.id_producto, pr.sku, pr.descripcion*/

            pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            //Detalle de la cotizacion
            foreach (DataRow row in pedidoAdjuntoDataTable.Rows)
            {
                PedidoAdjunto pedidoAdjunto = new PedidoAdjunto();
                pedidoAdjunto.idArchivoAdjunto = Converter.GetGuid(row, "id_archivo_adjunto");
                pedidoAdjunto.adjunto = Converter.GetBytes(row, "adjunto");
                pedidoAdjunto.nombre = Converter.GetString(row, "nombre");
                pedido.pedidoAdjuntoList.Add(pedidoAdjunto);
            }

            pedido.pedidoGrupoList = new List<PedidoGrupo>();

            foreach (DataRow row in pedidoGrupoDataTable.Rows)
            {
                PedidoGrupo pedidoGrupo = new PedidoGrupo();
                pedidoGrupo.numero = Converter.GetInt(row, "numero_grupo");
                pedidoGrupo.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.pedidoGrupoList.Add(pedidoGrupo);
            }

            foreach (DataRow row in preciosEspecialesDataTable.Rows)
            {
                Guid idProducto = Converter.GetGuid(row, "id_producto");
                decimal costoEspecial = Converter.GetDecimal(row, "precio_unitario");
                int idProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");

                PedidoDetalle det = pedido.pedidoDetalleList.Where(d => d.producto.idProducto.Equals(idProducto)).First();
                if (det != null)
                {
                    switch (idProductoPresentacion)
                    {
                        case 0:
                            det.costoEspecial = costoEspecial;
                            det.tieneCostoEspecial = true;
                            break;
                        case 1:
                            det.costoEspecial = costoEspecial * ((decimal)det.producto.equivalenciaProveedor);
                            det.tieneCostoEspecial = true;
                            break;
                        case 2:
                            det.costoEspecial = costoEspecial / ((decimal)det.producto.equivalenciaProveedor);
                            det.tieneCostoEspecial = true;
                            break;

                    }
                }
            }

            return pedido;
        }

        public Pedido SelectPedidoParaEditar(Pedido pedido, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_pedidoParaEditar");
            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable pedidoDataTable = dataSet.Tables[0];
            DataTable pedidoDetalleDataTable = dataSet.Tables[1];
            DataTable direccionEntregaDataTable = dataSet.Tables[2];
            DataTable movimientoAlmacenDataTable = dataSet.Tables[3];
            DataTable pedidoAdjuntoDataTable = dataSet.Tables[4];
            DataTable solicitanteDataTable = dataSet.Tables[5];

            //   DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            foreach (DataRow row in pedidoDataTable.Rows)
            {
                pedido.numeroPedido = Converter.GetLong(row, "numero");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo");
                pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                pedido.horaEntregaAdicionalDesde = Converter.GetString(row, "hora_entrega_adicional_desde");
                pedido.horaEntregaAdicionalHasta = Converter.GetString(row, "hora_entrega_adicional_hasta");
                pedido.incluidoIGV = Converter.GetBool(row, "incluido_igv");
                pedido.montoIGV = Converter.GetDecimal(row, "igv");
                pedido.montoTotal = Converter.GetDecimal(row, "total");
                pedido.observaciones = Converter.GetString(row, "observaciones");
                pedido.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedido.montoTotal - pedido.montoIGV));
                pedido.fechaModificacion = Converter.GetDateTime(row, "fecha_modificacion");
                pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                pedido.direccionEntrega = new DireccionEntrega();
                pedido.direccionEntrega.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega");
                pedido.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");
                pedido.direccionEntrega.contacto = Converter.GetString(row, "contacto_entrega");
                pedido.direccionEntrega.telefono = Converter.GetString(row, "telefono_contacto_entrega");
                pedido.direccionEntrega.codigoCliente = Converter.GetString(row, "direccion_entrega_codigo_cliente");
                pedido.direccionEntrega.codigoMP = Converter.GetString(row, "direccion_entrega_codigo_mp");
                pedido.direccionEntrega.nombre = Converter.GetString(row, "direccion_entrega_nombre");

                pedido.moneda = new Moneda();
                pedido.moneda.codigo = Converter.GetString(row, "moneda");

                pedido.moneda = Moneda.ListaMonedas.Where(m => m.codigo == pedido.moneda.codigo).FirstOrDefault();

                Guid idOrdenCompracliente = Converter.GetGuid(row, "id_orden_compra_cliente");
                if (idOrdenCompracliente != null && idOrdenCompracliente != Guid.Empty)
                {
                    pedido.ordenCompracliente = new OrdenCompraCliente();
                    pedido.ordenCompracliente.idOrdenCompraCliente = idOrdenCompracliente;
                    pedido.ordenCompracliente.numeroReferenciaCliente = pedido.numeroReferenciaCliente;
                }

                pedido.solicitante = new Solicitante();
                pedido.solicitante.idSolicitante = Converter.GetGuid(row, "id_solicitante");

                pedido.contactoPedido = Converter.GetString(row, "contacto_pedido");
                pedido.telefonoContactoPedido = Converter.GetString(row, "telefono_contacto_pedido");
                pedido.correoContactoPedido = Converter.GetString(row, "correo_contacto_pedido");
                pedido.numeroRequerimiento = Converter.GetString(row, "numero_requerimiento");
                pedido.esPagoContado = Converter.GetBool(row, "es_pago_contado");
                pedido.facturaUnica = Converter.GetInt(row, "factura_unica") == 1 ? true : false;

                pedido.solicitante.nombre = pedido.contactoPedido;
                pedido.solicitante.telefono = pedido.telefonoContactoPedido;
                pedido.solicitante.correo = pedido.correoContactoPedido;

                pedido.fechaProgramacion = Converter.GetDateTime(row, "fecha_programacion");
                pedido.observacionesFactura = Converter.GetString(row, "observaciones_factura");
                pedido.observacionesGuiaRemision = Converter.GetString(row, "observaciones_guia_remision");

                pedido.clasePedido = (Pedido.ClasesPedido)Char.Parse(Converter.GetString(row, "tipo"));
                if (pedido.clasePedido == Pedido.ClasesPedido.Venta)
                    pedido.tipoPedido = (Pedido.tiposPedido)Char.Parse(Converter.GetString(row, "tipo_pedido"));
                else if (pedido.clasePedido == Pedido.ClasesPedido.Compra)
                    pedido.tipoPedidoCompra = (Pedido.tiposPedidoCompra)Char.Parse(Converter.GetString(row, "tipo_pedido"));
                else
                    pedido.tipoPedidoAlmacen = (Pedido.tiposPedidoAlmacen)Char.Parse(Converter.GetString(row, "tipo_pedido"));



                pedido.otrosCargos = Converter.GetDecimal(row, "otros_cargos");
                pedido.numeroReferenciaAdicional = Converter.GetString(row, "numero_referencia_adicional");

                pedido.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                pedido.FechaRegistro = pedido.FechaRegistro.AddHours(-5);

                /* pedido.venta = new Venta();
                   pedido.venta.igv = Converter.GetDecimal(row, "igv_venta");
                   pedido.venta.subTotal = Converter.GetDecimal(row, "sub_total_venta");
                   pedido.venta.total = Converter.GetDecimal(row, "total_venta");
                   pedido.venta.idVenta = Converter.GetGuid(row, "id_Venta");*/

                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                pedido.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                pedido.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                pedido.documentoVenta = new DocumentoVenta();
                pedido.documentoVenta.serie = Converter.GetString(row, "serie_factura");
                pedido.documentoVenta.numero = Converter.GetString(row, "numero_factura");

                pedido.cotizacion = new Cotizacion();
                pedido.cotizacion.idCotizacion = Converter.GetGuid(row, "id_cotizacion");
                pedido.cotizacion.codigo = Converter.GetLong(row, "cotizacion_codigo");

                pedido.cliente = new Cliente();
                pedido.cliente.codigo = Converter.GetString(row, "codigo");
                pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                pedido.cliente.ruc = Converter.GetString(row, "ruc");
                pedido.cliente.ciudad = new Ciudad();
                pedido.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad_cliente");
                pedido.cliente.ciudad.nombre = Converter.GetString(row, "nombre_ciudad_cliente");
                pedido.cliente.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                pedido.cliente.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                pedido.cliente.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                pedido.cliente.plazoCredito = Converter.GetString(row, "plazo_credito");
                pedido.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                pedido.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                pedido.cliente.habilitadoModificarDireccionEntrega = Converter.GetBool(row, "habilitado_modificar_direccion_entrega");
                pedido.cliente.tipoLiberacionCrediticia = (Persona.TipoLiberacionCrediticia)Converter.GetInt(row, "estado_liberacion_creditica");

                pedido.ciudad = new Ciudad();
                pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                pedido.promocion = new Promocion();
                pedido.promocion.idPromocion = Converter.GetGuid(row, "id_promocion");
                pedido.promocion.codigo = Converter.GetString(row, "codigo_promocion");
                pedido.promocion.titulo = Converter.GetString(row, "titulo_promocion");
                pedido.promocion.fechaInicio = Converter.GetDateTime(row, "fecha_inicio_promocion");
                pedido.promocion.fechaFin = Converter.GetDateTime(row, "fecha_fin_promocion");
                pedido.promocion.descripcion = Converter.GetString(row, "descripcion_promocion");

                /*   pedido.usuario = new Usuario();
                   pedido.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                   pedido.usuario.cargo = Converter.GetString(row, "cargo");
                   pedido.usuario.contacto = Converter.GetString(row, "contacto_usuario");
                   pedido.usuario.email = Converter.GetString(row, "email");*/

                pedido.seguimientoPedido = new SeguimientoPedido();
                pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Converter.GetInt(row, "estado_seguimiento");
                pedido.seguimientoPedido.observacion = Converter.GetString(row, "observacion_seguimiento");
                pedido.seguimientoPedido.usuario = new Usuario();
                pedido.seguimientoPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                pedido.seguimientoPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");

                pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
                pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Converter.GetInt(row, "estado_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.observacion = Converter.GetString(row, "observacion_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.usuario = new Usuario();
                pedido.seguimientoCrediticioPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento_crediticio");

            }


            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in pedidoDetalleDataTable.Rows)
            {
                PedidoDetalle pedidoDetalle = new PedidoDetalle(usuario.visualizaCostos, usuario.visualizaMargen);

                pedidoDetalle.producto = new Producto();

                pedidoDetalle.idPedidoDetalle = Converter.GetGuid(row, "id_pedido_detalle");
                pedidoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                pedidoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");
                //pedidoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                pedidoDetalle.flete = Converter.GetDecimal(row, "flete");


                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");
                pedidoDetalle.producto.tipoProducto = (Producto.TipoProducto) Converter.GetInt(row, "tipo_producto");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    pedidoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                    pedidoDetalle.ProductoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.ProductoPresentacion.Equivalencia;
                }
                else
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                }

                pedidoDetalle.unidad = Converter.GetString(row, "unidad");

                pedidoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                pedidoDetalle.producto.sku = Converter.GetString(row, "sku");
                pedidoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                pedidoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                pedidoDetalle.producto.proveedor = Converter.GetString(row, "proveedor");
                pedidoDetalle.producto.ventaRestringida = (Producto.TipoVentaRestringida) Converter.GetInt(row, "descontinuado");
                pedidoDetalle.producto.motivoRestriccion = Converter.GetString(row, "motivo_restriccion");
                pedidoDetalle.producto.cantidadMaximaPedidoRestringido = Converter.GetInt(row, "cantidad_maxima_pedido_restringido");
                pedidoDetalle.producto.topeDescuento = Converter.GetDecimal(row, "tope_descuento");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_producto");
                pedidoDetalle.producto.costoLista = pedidoDetalle.producto.costoSinIgv;
                pedidoDetalle.producto.costoOriginal = Converter.GetDecimal(row, "costo_original");
                pedidoDetalle.producto.equivalenciaProveedor = Converter.GetInt(row, "equivalencia_proveedor");
                pedidoDetalle.producto.compraRestringida = Converter.GetInt(row, "compra_restringida");

                pedidoDetalle.producto.image = Converter.GetBytes(row, "imagen");

                pedidoDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                pedidoDetalle.observacion = Converter.GetString(row, "observaciones");


                PrecioClienteProducto precioClienteProducto = new PrecioClienteProducto();

                precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto_vigente");
                precioClienteProducto.flete = Converter.GetDecimal(row, "flete_vigente");
                precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario_vigente");
                precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia_vigente");

                precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");

                precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");

                /*if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    precioClienteProducto.fechaFinVigencia = null;
                }
                else
                {*/
                precioClienteProducto.fechaFinVigencia = Converter.GetDateTimeNullable(row, "fecha_fin_vigencia");
                //}

                //  precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                precioClienteProducto.cliente = new Cliente();
                precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedidoDetalle.producto.precioClienteProducto = precioClienteProducto;


                pedidoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                pedidoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }

            List<DireccionEntrega> direccionEntregaList = new List<DireccionEntrega>();

            foreach (DataRow row in direccionEntregaDataTable.Rows)
            {
                DireccionEntrega obj = new DireccionEntrega
                {
                    idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega"),
                    descripcion = Converter.GetString(row, "descripcion"),
                    contacto = Converter.GetString(row, "contacto"),
                    telefono = Converter.GetString(row, "telefono"),
                    ubigeo = new Ubigeo { Id = Converter.GetString(row, "ubigeo") },
                    nombre = Converter.GetString(row, "nombre"),
                    codigoCliente = Converter.GetString(row, "codigoCliente"),
                    codigoMP = Converter.GetString(row, "codigoMP")  
                };
                direccionEntregaList.Add(obj);
            }

            pedido.cliente.direccionEntregaList = direccionEntregaList;


            List<Solicitante> solicitanteList = new List<Solicitante>();

            foreach (DataRow row in solicitanteDataTable.Rows)
            {
                Solicitante obj = new Solicitante
                {
                    idSolicitante = Converter.GetGuid(row, "id_solicitante"),
                    nombre = Converter.GetString(row, "nombre"),
                    telefono = Converter.GetString(row, "telefono"),
                    correo = Converter.GetString(row, "correo")
                };
                solicitanteList.Add(obj);
            }

            pedido.cliente.solicitanteList = solicitanteList;


            pedido.guiaRemisionList = new List<GuiaRemision>();

            GuiaRemision movimientoAlmacen = new GuiaRemision();
            movimientoAlmacen.idMovimientoAlmacen = Guid.Empty;

            foreach (DataRow row in movimientoAlmacenDataTable.Rows)
            {
                Guid idMovimientoAlmacen = Converter.GetGuid(row, "id_movimiento_almacen");
                if (movimientoAlmacen.idMovimientoAlmacen != idMovimientoAlmacen)
                {
                    //Si no coincide con el anterior se crea un nuevo movimiento Almacen
                    movimientoAlmacen = new GuiaRemision();
                    movimientoAlmacen.idMovimientoAlmacen = idMovimientoAlmacen;
                    movimientoAlmacen.fechaEmision = Converter.GetDateTime(row, "fecha_emision");
                    movimientoAlmacen.fechaTraslado = Converter.GetDateTime(row, "fecha_traslado");
                    movimientoAlmacen.numeroDocumento = Converter.GetInt(row, "numero_documento");
                    movimientoAlmacen.serieDocumento = Converter.GetString(row, "serie_documento");
                    movimientoAlmacen.documentoDetalle = new List<DocumentoDetalle>();

                    movimientoAlmacen.documentoVenta = new DocumentoVenta();
                    movimientoAlmacen.documentoVenta.idDocumentoVenta = Converter.GetGuid(row, "id_documento_venta");
                    movimientoAlmacen.documentoVenta.serie = Converter.GetString(row, "SERIE");
                    movimientoAlmacen.documentoVenta.numero = Converter.GetString(row, "CORRELATIVO");



                    /*if (row["fecha_emision_factura"] == DBNull.Value)
                        movimientoAlmacen.documentoVenta.fechaEmision = null;
                    else*/
                    movimientoAlmacen.documentoVenta.fechaEmision = Converter.GetDateTimeNullable(row, "fecha_emision_factura");



                    pedido.guiaRemisionList.Add(movimientoAlmacen);
                }

                DocumentoDetalle documentoDetalle = new DocumentoDetalle();
                documentoDetalle.idDocumentoDetalle = Converter.GetGuid(row, "id_movimiento_almacen_detalle");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                documentoDetalle.producto = new Producto();
                documentoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                documentoDetalle.producto.sku = Converter.GetString(row, "sku");
                documentoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                documentoDetalle.unidad = Converter.GetString(row, "unidad");
                documentoDetalle.cantidad = Converter.GetInt(row, "cantidad");

                movimientoAlmacen.documentoDetalle.Add(documentoDetalle);
            }


            /*mad.id_movimiento_almacen_detalle, mad.cantidad, 
mad.unidad, pr.id_producto, pr.sku, pr.descripcion*/

            pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            //Detalle de la cotizacion
            foreach (DataRow row in pedidoAdjuntoDataTable.Rows)
            {
                PedidoAdjunto pedidoAdjunto = new PedidoAdjunto();
                pedidoAdjunto.idArchivoAdjunto = Converter.GetGuid(row, "id_archivo_adjunto");
                pedidoAdjunto.adjunto = Converter.GetBytes(row, "adjunto");
                pedidoAdjunto.nombre = Converter.GetString(row, "nombre");
                pedido.pedidoAdjuntoList.Add(pedidoAdjunto);
            }

            return pedido;
        }

      

        public void insertSeguimientoPedido(Pedido pedido)
        {
            var objCommand = GetSqlCommand("pi_seguimiento_pedido");

            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            InputParameterAdd.Varchar(objCommand, "observacion", pedido.seguimientoPedido.observacion);
            InputParameterAdd.DateTime(objCommand, "fechaModificacion", pedido.fechaModificacion);

            ExecuteNonQuery(objCommand);


        }
        public void insertSeguimientoCrediticioPedido(Pedido pedido)
        {
            var objCommand = GetSqlCommand("pi_seguimiento_crediticio_pedido");

            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoCrediticioPedido.estado);
            InputParameterAdd.Varchar(objCommand, "observacion", pedido.seguimientoCrediticioPedido.observacion);
            InputParameterAdd.DateTime(objCommand, "fechaModificacion", pedido.fechaModificacion);

            ExecuteNonQuery(objCommand);


        }
        public void UpdateStockConfirmado(Pedido pedido)
        {
            var objCommand = GetSqlCommand("pu_pedidoStockConfirmado");
            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.Int(objCommand, "stockConfirmado", pedido.stockConfirmado);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }

        public Pedido UpdateTruncado(Pedido pedido)
        {
            var objCommand = GetSqlCommand("pu_pedidotruncar");
            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.Int(objCommand, "truncado", pedido.truncado);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            ExecuteNonQuery(objCommand);

            return pedido;
        }

        #endregion


        public List<Pedido> SelectPedidosSinAtencion()
        {
            List<Pedido> pedidoIds = new List<Pedido>();

            var objCommand = GetSqlCommand("ps_pedidos_sin_atencion");
            DataTable dataTable = Execute(objCommand);

            int ultimoDia = 0;
            int esFechaExtendida = 0;
            int mod = 0;

            foreach (DataRow row in dataTable.Rows)
            {
                Pedido item = new Pedido();
                item.idPedido = Converter.GetGuid(row, "id_pedido");
                
                esFechaExtendida = Converter.GetInt(row, "es_fecha_extendida");

                if (esFechaExtendida == 1) {
                    ultimoDia = Converter.GetInt(row, "es_fecha_limite_extendida");
                    mod = Converter.GetInt(row, "dias_dif_mod_fecha_entrega_extendida");
                } else {
                    ultimoDia = Converter.GetInt(row, "es_fecha_limite");
                    mod = Converter.GetInt(row, "dias_dif_mod_fecha_entrega");
                }

                item.accion_truncar = false;
                item.accion_alertarNoAtendido = false;

                if (mod == 0)
                {
                    item.accion_alertarNoAtendido = true;
                }

                if (ultimoDia == 1)
                {
                    item.accion_truncar = true;
                }

                pedidoIds.Add(item);
            }
            
            return pedidoIds;
        }

        public bool TruncarPedidos(List<Guid> idPedidos)
        {
            var objCommand = GetSqlCommand("pu_truncar_pedidos");
            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("pedidos", typeof(Guid)));

            // populate DataTable from your List here
            foreach (var id in idPedidos)
                tvp.Rows.Add(id);

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@pedidos", tvp);
            // these next lines are important to map the C# DataTable object to the correct SQL User Defined Type
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            ExecuteNonQuery(objCommand);

            return true;
        }


        public List<SeguimientoPedido> GetHistorialSeguimiento(Guid idPedido)
        {
            var objCommand = GetSqlCommand("ps_pedido_seguimiento");
            InputParameterAdd.Guid(objCommand, "idPedido", idPedido);
            DataTable dataTable = Execute(objCommand);

            List<SeguimientoPedido> seguimientoPedidos = new List<SeguimientoPedido>();

            foreach (DataRow row in dataTable.Rows)
            {
                SeguimientoPedido seg = new SeguimientoPedido();
                seg.idSeguimientoPedido = Converter.GetGuid(row, "id_seguimiento_pedido");
                seg.observacion = Converter.GetString(row, "observacion");
                seg.FechaRegistro = Converter.GetDateTime(row, "fecha_creacion");
                seg.estado = (SeguimientoPedido.estadosSeguimientoPedido)Converter.GetInt(row, "estado_pedido");

                seg.usuario = new Usuario();
                seg.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                seg.usuario.nombre = Converter.GetString(row, "nombre_usuario");

                seguimientoPedidos.Add(seg);
            }

            return seguimientoPedidos;
        }

        public List<SeguimientoCrediticioPedido> GetHistorialCrediticioSeguimiento(Guid idPedido)
        {
            var objCommand = GetSqlCommand("ps_pedido_seguimiento_crediticio");
            InputParameterAdd.Guid(objCommand, "idPedido", idPedido);
            DataTable dataTable = Execute(objCommand);

            List<SeguimientoCrediticioPedido> seguimientoPedidos = new List<SeguimientoCrediticioPedido>();

            foreach (DataRow row in dataTable.Rows)
            {
                SeguimientoCrediticioPedido seg = new SeguimientoCrediticioPedido();
                //seg.id = Converter.GetGuid(row, "id_seguimiento_crediticio");
                seg.observacion = Converter.GetString(row, "observacion");
                seg.FechaRegistro = Converter.GetDateTime(row, "fecha_creacion");
                seg.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Converter.GetInt(row, "estado_pedido");

                seg.usuario = new Usuario();
                seg.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");
                seg.usuario.nombre = Converter.GetString(row, "nombre_usuario");

                seguimientoPedidos.Add(seg);
            }

            return seguimientoPedidos;

        }
        public Pedido SelectPedidoEmail(Guid idPedido)
        {
            var objCommand = GetSqlCommand("ps_pedido_email");
            InputParameterAdd.Guid(objCommand, "idPedido", idPedido);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable pedidoDataTable = dataSet.Tables[0];
            DataTable pedidoDetalleDataTable = dataSet.Tables[1];
            DataTable direccionEntregaDataTable = dataSet.Tables[2];

            //   DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            Pedido pedido = new Pedido();
            foreach (DataRow row in pedidoDataTable.Rows)
            {
                pedido.idPedido = idPedido;
                pedido.numeroPedido = Converter.GetLong(row, "numero");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo");
                pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                pedido.fechaEntregaExtendida = Converter.GetDateTime(row, "fecha_entrega_extendida");
                pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                pedido.incluidoIGV = Converter.GetBool(row, "incluido_igv");
                pedido.montoIGV = Converter.GetDecimal(row, "igv");
                pedido.montoTotal = Converter.GetDecimal(row, "total");
                pedido.observaciones = Converter.GetString(row, "observaciones");
                pedido.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, pedido.montoTotal - pedido.montoIGV));
                pedido.fechaModificacion = Converter.GetDateTime(row, "fecha_modificacion");
                pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                pedido.direccionEntrega = new DireccionEntrega();
                pedido.direccionEntrega.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega");
                pedido.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");
                pedido.direccionEntrega.contacto = Converter.GetString(row, "contacto_entrega");
                pedido.direccionEntrega.telefono = Converter.GetString(row, "telefono_contacto_entrega");

                pedido.solicitante = new Solicitante();
                pedido.solicitante.idSolicitante = Converter.GetGuid(row, "id_solicitante");

                pedido.contactoPedido = Converter.GetString(row, "contacto_pedido");
                pedido.telefonoContactoPedido = Converter.GetString(row, "telefono_contacto_pedido");
                pedido.correoContactoPedido = Converter.GetString(row, "correo_contacto_pedido");

                pedido.solicitante.nombre = pedido.contactoPedido;
                pedido.solicitante.telefono = pedido.telefonoContactoPedido;
                pedido.solicitante.correo = pedido.correoContactoPedido;

                pedido.fechaProgramacion = Converter.GetDateTime(row, "fecha_programacion");
                pedido.observacionesFactura = Converter.GetString(row, "observaciones_factura");
                pedido.observacionesGuiaRemision = Converter.GetString(row, "observaciones_guia_remision");

                
                pedido.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                pedido.FechaRegistro = pedido.FechaRegistro.AddHours(-5);

                /* pedido.venta = new Venta();
                   pedido.venta.igv = Converter.GetDecimal(row, "igv_venta");
                   pedido.venta.subTotal = Converter.GetDecimal(row, "sub_total_venta");
                   pedido.venta.total = Converter.GetDecimal(row, "total_venta");
                   pedido.venta.idVenta = Converter.GetGuid(row, "id_Venta");*/

                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                pedido.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                pedido.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

               
                pedido.cliente = new Cliente();
                pedido.cliente.codigo = Converter.GetString(row, "codigo");
                pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                pedido.cliente.ruc = Converter.GetString(row, "ruc");
                pedido.cliente.ciudad = new Ciudad();
                pedido.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad_cliente");
                pedido.cliente.ciudad.nombre = Converter.GetString(row, "nombre_ciudad_cliente");
                pedido.cliente.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                pedido.cliente.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                pedido.cliente.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                pedido.cliente.plazoCredito = Converter.GetString(row, "plazo_credito");
                pedido.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                pedido.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");

                /*Vendedores*/
                pedido.cliente.responsableComercial = new Vendedor();
                pedido.cliente.responsableComercial.codigo = Converter.GetString(row, "responsable_comercial_codigo");
                pedido.cliente.responsableComercial.descripcion = Converter.GetString(row, "responsable_comercial_descripcion");
                pedido.cliente.responsableComercial.usuario = new Usuario();
                pedido.cliente.responsableComercial.usuario.email = Converter.GetString(row, "responsable_comercial_email");

                pedido.cliente.supervisorComercial = new Vendedor();
                pedido.cliente.supervisorComercial.codigo = Converter.GetString(row, "supervisor_comercial_codigo");
                pedido.cliente.supervisorComercial.descripcion = Converter.GetString(row, "supervisor_comercial_descripcion");
                pedido.cliente.supervisorComercial.usuario = new Usuario();
                pedido.cliente.supervisorComercial.usuario.email = Converter.GetString(row, "supervisor_comercial_email");

                pedido.cliente.asistenteServicioCliente = new Vendedor();
                pedido.cliente.asistenteServicioCliente.codigo = Converter.GetString(row, "asistente_servicio_cliente_codigo");
                pedido.cliente.asistenteServicioCliente.descripcion = Converter.GetString(row, "asistente_servicio_cliente_descripcion");
                pedido.cliente.asistenteServicioCliente.usuario = new Usuario();
                pedido.cliente.asistenteServicioCliente.usuario.email = Converter.GetString(row, "asistente_servicio_cliente_email");

                pedido.ciudad = new Ciudad();
                pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");
                pedido.ciudad.esProvincia = Converter.GetBool(row, "es_provincia");

                pedido.usuario = new Usuario();
                pedido.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                pedido.usuario.cargo = Converter.GetString(row, "cargo");
                pedido.usuario.contacto = Converter.GetString(row, "contacto_usuario");
                pedido.usuario.email = Converter.GetString(row, "email");

                pedido.seguimientoPedido = new SeguimientoPedido();
                pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Converter.GetInt(row, "estado_seguimiento");
                pedido.seguimientoPedido.observacion = Converter.GetString(row, "observacion_seguimiento");
                pedido.seguimientoPedido.usuario = new Usuario();
                pedido.seguimientoPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento");
                pedido.seguimientoPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento");

                pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
                pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Converter.GetInt(row, "estado_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.observacion = Converter.GetString(row, "observacion_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.usuario = new Usuario();
                pedido.seguimientoCrediticioPedido.usuario.idUsuario = Converter.GetGuid(row, "id_usuario_seguimiento_crediticio");
                pedido.seguimientoCrediticioPedido.usuario.nombre = Converter.GetString(row, "usuario_seguimiento_crediticio");

            }


            pedido.pedidoDetalleList = new List<PedidoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in pedidoDetalleDataTable.Rows)
            {
                PedidoDetalle pedidoDetalle = new PedidoDetalle(false, false);

                pedidoDetalle.producto = new Producto();

                pedidoDetalle.idPedidoDetalle = Converter.GetGuid(row, "id_pedido_detalle");
                pedidoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                pedidoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                pedidoDetalle.flete = Converter.GetDecimal(row, "flete");

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    pedidoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                }

                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.ProductoPresentacion.Equivalencia;
                }
                else
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                }

                pedidoDetalle.unidad = Converter.GetString(row, "unidad");

                pedidoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                pedidoDetalle.producto.sku = Converter.GetString(row, "sku");
                pedidoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                pedidoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                pedidoDetalle.producto.proveedor = Converter.GetString(row, "proveedor");

                pedidoDetalle.producto.image = Converter.GetBytes(row, "imagen");

                pedidoDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                pedidoDetalle.observacion = Converter.GetString(row, "observaciones");


                PrecioClienteProducto precioClienteProducto = new PrecioClienteProducto();

                precioClienteProducto.precioNeto = Converter.GetDecimal(row, "precio_neto_vigente");
                precioClienteProducto.flete = Converter.GetDecimal(row, "flete_vigente");
                precioClienteProducto.precioUnitario = Converter.GetDecimal(row, "precio_unitario_vigente");
                precioClienteProducto.equivalencia = Converter.GetInt(row, "equivalencia_vigente");

                precioClienteProducto.idPrecioClienteProducto = Converter.GetGuid(row, "id_precio_cliente_producto");

                precioClienteProducto.fechaInicioVigencia = Converter.GetDateTime(row, "fecha_inicio_vigencia");

                /*if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    precioClienteProducto.fechaFinVigencia = null;
                }
                else
                {*/
                    precioClienteProducto.fechaFinVigencia = Converter.GetDateTimeNullable(row, "fecha_fin_vigencia");
                /*}*/

                //  precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                precioClienteProducto.cliente = new Cliente();
                precioClienteProducto.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedidoDetalle.producto.precioClienteProducto = precioClienteProducto;


                pedidoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                pedidoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                pedido.pedidoDetalleList.Add(pedidoDetalle);
            }

            List<DireccionEntrega> direccionEntregaList = new List<DireccionEntrega>();

            foreach (DataRow row in direccionEntregaDataTable.Rows)
            {
                DireccionEntrega obj = new DireccionEntrega
                {
                    idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega"),
                    descripcion = Converter.GetString(row, "descripcion"),
                    contacto = Converter.GetString(row, "contacto"),
                    telefono = Converter.GetString(row, "telefono"),
                    ubigeo = new Ubigeo { Id = Converter.GetString(row, "ubigeo") }
                };
                direccionEntregaList.Add(obj);
            }

            if (pedido.cliente != null)
            {
                pedido.cliente.direccionEntregaList = direccionEntregaList;
            }
            

            pedido.guiaRemisionList = new List<GuiaRemision>();

            GuiaRemision movimientoAlmacen = new GuiaRemision();
            movimientoAlmacen.idMovimientoAlmacen = Guid.Empty;



            /*mad.id_movimiento_almacen_detalle, mad.cantidad, 
            mad.unidad, pr.id_producto, pr.sku, pr.descripcion*/

            pedido.pedidoAdjuntoList = new List<PedidoAdjunto>();
            

            return pedido;
        }


        public bool UpdateDetallesRestriccion(Guid idPedido, List<Guid> idDetalles, List<int> cantidades, List<String> comentarios, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pu_restriccion_cantidad_pedido");
            InputParameterAdd.Guid(objCommand, "idPedido", idPedido);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));
            tvp.Columns.Add(new DataColumn("CANTIDAD", typeof(int)));
            tvp.Columns.Add(new DataColumn("COMENTARIO", typeof(String)));

            for (int i = 0; i < idDetalles.Count; i++)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = idDetalles[i];
                rowObj["CANTIDAD"] = cantidades[i];
                rowObj["COMENTARIO"] = comentarios[i];

                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@restricciones", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.DetalleCantidadList";


            ExecuteNonQuery(objCommand);

            return true;
        }

        public List<int> AprobarPedidosGrupo(long nroGrupo, Guid idUsuario) 
        {
            var objCommand = GetSqlCommand("pu_aprobar_pedidos_grupo");
            List<int> resultados = new List<int>();

            InputParameterAdd.BigInt(objCommand, "numeroGrupo", nroGrupo);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            OutputParameterAdd.BigInt(objCommand, "cantidadGrupo");
            OutputParameterAdd.BigInt(objCommand, "cantidadoAprobados");
            ExecuteNonQuery(objCommand);

            Int64 cantidadGrupo = (Int64)objCommand.Parameters["@cantidadGrupo"].Value;
            Int64 cantidadAplicados = (Int64)objCommand.Parameters["@cantidadoAprobados"].Value;



            resultados.Add((Int32)cantidadGrupo);
            resultados.Add((Int32)cantidadAplicados);

            return resultados;
        }

        public List<int> LiberarPedidosGrupo(long nroGrupo, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("pu_liberar_pedidos_grupo");
            List<int> resultados = new List<int>();
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", nroGrupo);
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            OutputParameterAdd.BigInt(objCommand, "cantidadGrupo");
            OutputParameterAdd.BigInt(objCommand, "cantidadoLiberados");
            ExecuteNonQuery(objCommand);

            Int64 cantidadGrupo = (Int64)objCommand.Parameters["@cantidadGrupo"].Value;
            Int64 cantidadAplicados = (Int64)objCommand.Parameters["@cantidadoLiberados"].Value;

            resultados.Add((Int32)cantidadGrupo);
            resultados.Add((Int32)cantidadAplicados);

            return resultados;
        }

        public List<Pedido> SelectPedidosGrupo(Pedido ped, int tipoOrdenamiento)
        {
            var objCommand = GetSqlCommand("ps_pedidos_grupo");
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", ped.numeroGrupoPedido);
            InputParameterAdd.Int(objCommand, "tipoOrdenamiento", tipoOrdenamiento);
            
            DataTable dataTable = Execute(objCommand);

            List<Pedido> pedidoList = new List<Pedido>();

            foreach (DataRow row in dataTable.Rows)
            {
                Pedido pedido = new Pedido(ped.clasePedido);
                pedido.numeroPedido = Converter.GetLong(row, "numero_pedido");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo_pedido");
                pedido.idPedido = Converter.GetGuid(row, "id_pedido");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                pedido.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                pedido.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                pedido.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                pedido.horaEntregaAdicionalDesde = Converter.GetString(row, "hora_entrega_adicional_desde");
                pedido.horaEntregaAdicionalHasta = Converter.GetString(row, "hora_entrega_adicional_hasta");
                pedido.fechaEntregaExtendida = Converter.GetDateTimeNullable(row, "fecha_entrega_extendida");
                pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                pedido.truncado = Converter.GetInt(row, "truncado");

                pedido.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                //pedido.FechaRegistro = pedido.FechaRegistro.AddHours(-5);
                pedido.montoTotal = Converter.GetDecimal(row, "total");
                
                pedido.cliente = new Cliente();
                pedido.cliente.codigo = Converter.GetString(row, "codigo");
                pedido.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                pedido.cliente.razonSocial = Converter.GetString(row, "razon_social");
                pedido.cliente.ruc = Converter.GetString(row, "ruc");

                pedido.cliente.grupoCliente = new GrupoCliente();

                pedido.ciudad = new Ciudad();
                pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                pedido.usuario = new Usuario();
                pedido.usuario.nombre = "";

                pedido.seguimientoPedido = new SeguimientoPedido();
                pedido.seguimientoPedido.estado = (SeguimientoPedido.estadosSeguimientoPedido)Converter.GetInt(row, "estado_seguimiento");

                pedido.seguimientoCrediticioPedido = new SeguimientoCrediticioPedido();
                pedido.seguimientoCrediticioPedido.estado = (SeguimientoCrediticioPedido.estadosSeguimientoCrediticioPedido)Converter.GetInt(row, "estado_seguimiento_crediticio");

                pedido.ubigeoEntrega = new Ubigeo();
                pedido.ubigeoEntrega.Id = Converter.GetString(row, "codigo_ubigeo");
                pedido.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                pedidoList.Add(pedido);
            }
            return pedidoList;
        }

        public List<Guid> soloPedidosALiberar(List<Guid> idPedidos, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_idsPedidosParaLiberar");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            for (int i = 0; i < idPedidos.Count; i++)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = idPedidos[i];

                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idPedidos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            DataTable dataTable = Execute(objCommand);

            List<Guid> idPedidosList = new List<Guid>();

            foreach (DataRow row in dataTable.Rows)
            {
                Guid idItem = Converter.GetGuid(row, "id_pedido");
            }

            return idPedidosList;
        }

        public List<Guid> soloPedidosAApropbar(List<Guid> idPedidos, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_idsPedidosParaAprobar");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            for (int i = 0; i < idPedidos.Count; i++)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = idPedidos[i];

                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idPedidos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            DataTable dataTable = Execute(objCommand);

            List<Guid> idPedidosList = new List<Guid>();

            foreach (DataRow row in dataTable.Rows)
            {
                Guid idItem = Converter.GetGuid(row, "id_pedido");
            }

            return idPedidosList;
        }


        public List<List<String>> totalesRazonSocial(List<Guid> idPedidos, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_totales_clientes_pedidos");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            for (int i = 0; i < idPedidos.Count; i++)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = idPedidos[i];

                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idPedidos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            DataTable dataTable = Execute(objCommand);

            List<List<String>> resultados = new List<List<String>>();

            foreach (DataRow row in dataTable.Rows)
            {
                List<String> item = new List<String>();

                item.Add(Converter.GetGuid(row, "id_cliente").ToString());
                item.Add(Converter.GetGuid(row, "id_ciudad").ToString());
                item.Add(Converter.GetString(row, "razon_social_sunat"));
                item.Add(Converter.GetString(row, "nombre_ciudad"));

                Decimal subtotal = Converter.GetDecimal(row, "subtotal");
                Decimal total = Converter.GetDecimal(row, "total");

                item.Add(String.Format(Constantes.formatoDosDecimales, subtotal));
                item.Add(String.Format(Constantes.formatoDosDecimales, total));
            }

            return resultados;
        }

        public List<List<String>> totalesProductos(List<Guid> idPedidos, Guid idUsuario)
        {
            var objCommand = GetSqlCommand("ps_totales_productos_pedidos");
            InputParameterAdd.Guid(objCommand, "idUsuario", idUsuario);

            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("ID", typeof(Guid)));

            for (int i = 0; i < idPedidos.Count; i++)
            {
                DataRow rowObj = tvp.NewRow();
                rowObj["ID"] = idPedidos[i];

                tvp.Rows.Add(rowObj);
            }

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idPedidos", tvp);
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.UniqueIdentifierList";

            DataTable dataTable = Execute(objCommand);

            List<List<String>> resultados = new List<List<String>>();

            foreach (DataRow row in dataTable.Rows)
            {
                List<String> item = new List<String>();

                item.Add(Converter.GetString(row, "sku"));
                item.Add(Converter.GetString(row, "descripcion"));
                item.Add(Converter.GetString(row, "unidad"));

                Decimal cantidad = Converter.GetDecimal(row, "cantidad_unidad_mp");
                Decimal subtotal = Converter.GetDecimal(row, "subtotal");
                Decimal precioUnitario = subtotal / cantidad;
                
                item.Add(String.Format(Constantes.formatoDosDecimales, precioUnitario));
                item.Add(String.Format(Constantes.formatoDosDecimales, cantidad));
                item.Add(String.Format(Constantes.formatoDosDecimales, subtotal));
            }

            return resultados;
        }
    }
}
