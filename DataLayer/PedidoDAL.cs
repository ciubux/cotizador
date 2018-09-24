using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
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

        public void InsertPedido(Pedido pedido)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_pedido");
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", pedido.numeroGrupoPedido); //puede ser null

            if (pedido.cotizacion.idCotizacion == Guid.Empty)
                InputParameterAdd.Guid(objCommand, "idCotizacion", null); //puede ser null
            else
                InputParameterAdd.Guid(objCommand, "idCotizacion", pedido.cotizacion.idCotizacion); //puede ser null

            InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", pedido.numeroReferenciaCliente); //puede ser null
            if (pedido.tipoPedido == Pedido.tiposPedido.Venta ||
                pedido.tipoPedido == Pedido.tiposPedido.TrasladoInterno ||
                pedido.tipoPedido == Pedido.tiposPedido.TransferenciaGratuitaEntregada ||
                pedido.tipoPedido == Pedido.tiposPedido.ComodatoEntregado ||
                pedido.tipoPedido == Pedido.tiposPedido.PrestamoEntregado)
            {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", pedido.direccionEntrega.idDireccionEntrega); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", pedido.direccionEntrega.descripcion);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", pedido.direccionEntrega.contacto); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", pedido.direccionEntrega.telefono); //puede ser null
            }
            else {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", null); //puede ser null


            }

            InputParameterAdd.DateTime(objCommand, "fechaSolicitud", pedido.fechaSolicitud);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde.Value);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta.Value);

            DateTime dtTmp = DateTime.Now;
            String[] horaEntregaDesdeArray = pedido.horaEntregaDesde.Split(':');
            DateTime horaEntregaDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaDesdeArray[0]), Int32.Parse(horaEntregaDesdeArray[1]),0);
            String[] horaEntregaHastaArray = pedido.horaEntregaHasta.Split(':');
            DateTime horaEntregaHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaHastaArray[0]), Int32.Parse(horaEntregaHastaArray[1]),0);
            InputParameterAdd.DateTime(objCommand, "horaEntregaDesde", horaEntregaDesde);
            InputParameterAdd.DateTime(objCommand, "horaEntregaHasta", horaEntregaHasta);
            if (pedido.tipoPedido == Pedido.tiposPedido.Venta ||
                pedido.tipoPedido == Pedido.tiposPedido.TrasladoInterno ||
                pedido.tipoPedido == Pedido.tiposPedido.TransferenciaGratuitaEntregada ||
                pedido.tipoPedido == Pedido.tiposPedido.ComodatoEntregado ||
                pedido.tipoPedido == Pedido.tiposPedido.PrestamoEntregado)
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

            InputParameterAdd.Char(objCommand, "tipoPedido", ((char)pedido.tipoPedido).ToString());
            InputParameterAdd.Varchar(objCommand, "observacionesGuiaRemision", pedido.observacionesGuiaRemision);
            InputParameterAdd.Varchar(objCommand, "observacionesFactura", pedido.observacionesFactura);
            InputParameterAdd.Decimal(objCommand, "otrosCargos", pedido.otrosCargos);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            OutputParameterAdd.BigInt(objCommand, "numero");
            ExecuteNonQuery(objCommand);

            pedido.idPedido = (Guid)objCommand.Parameters["@newId"].Value;
            pedido.numeroPedido = (Int64)objCommand.Parameters["@numero"].Value;


            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                pedidoDetalle.idPedido = pedido.idPedido;
                this.InsertPedidoDetalle(pedidoDetalle);
            }

            foreach (PedidoAdjunto pedidoAdjunto in pedido.pedidoAdjuntoList)
            {
                pedidoAdjunto.idPedido = pedido.idPedido;
                this.InsertPedidoAdjunto(pedidoAdjunto);
            }
            this.Commit();
        }


        public void InsertPedidoCompra(Pedido pedido)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pi_pedidoCompra");

            InputParameterAdd.BigInt(objCommand, "numeroGrupo", pedido.numeroGrupoPedido); //puede ser null

            if (pedido.cotizacion.idCotizacion == Guid.Empty)
                InputParameterAdd.Guid(objCommand, "idCotizacion", null); //puede ser null
            else
                InputParameterAdd.Guid(objCommand, "idCotizacion", pedido.cotizacion.idCotizacion); //puede ser null

            InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", pedido.numeroReferenciaCliente); //puede ser null

            if (pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.Compra ||
               pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.TransferenciaGratuitaRecibida ||
               pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.ComodatoRecibido ||
               pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.PrestamoRecibido)
            {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", null); //puede ser null
                
            }
            else {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", pedido.direccionEntrega.idDireccionEntrega); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", pedido.direccionEntrega.descripcion);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", pedido.direccionEntrega.contacto); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", pedido.direccionEntrega.telefono); //puede ser null
            }
            InputParameterAdd.DateTime(objCommand, "fechaSolicitud", pedido.fechaSolicitud);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde.Value);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta.Value);

            DateTime dtTmp = DateTime.Now;
            String[] horaEntregaDesdeArray = pedido.horaEntregaDesde.Split(':');
            DateTime horaEntregaDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaDesdeArray[0]), Int32.Parse(horaEntregaDesdeArray[1]), 0);
            String[] horaEntregaHastaArray = pedido.horaEntregaHasta.Split(':');
            DateTime horaEntregaHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaHastaArray[0]), Int32.Parse(horaEntregaHastaArray[1]), 0);
            InputParameterAdd.DateTime(objCommand, "horaEntregaDesde", horaEntregaDesde);
            InputParameterAdd.DateTime(objCommand, "horaEntregaHasta", horaEntregaHasta);
            if (pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.Compra ||
               pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.TransferenciaGratuitaRecibida ||
               pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.ComodatoRecibido ||
               pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.PrestamoRecibido)
            {
                InputParameterAdd.Guid(objCommand, "idSolicitante", null);
                InputParameterAdd.Varchar(objCommand, "contactoPedido", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "correoContactoPedido", null);  //puede ser null
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idSolicitante", pedido.solicitante.idSolicitante);
                InputParameterAdd.Varchar(objCommand, "contactoPedido", pedido.solicitante.nombre);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", pedido.solicitante.telefono);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "correoContactoPedido", pedido.solicitante.correo);  //puede ser null


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
            InputParameterAdd.Varchar(objCommand, "ubigeoEntrega", pedido.ubigeoEntrega.Id);
            InputParameterAdd.Char(objCommand, "tipoPedido", ((char)pedido.tipoPedidoCompra).ToString());
            InputParameterAdd.Decimal(objCommand, "otrosCargos", pedido.otrosCargos);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            OutputParameterAdd.BigInt(objCommand, "numero");
            ExecuteNonQuery(objCommand);

            pedido.idPedido = (Guid)objCommand.Parameters["@newId"].Value;
            pedido.numeroPedido = (Int64)objCommand.Parameters["@numero"].Value;

            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                pedidoDetalle.idPedido = pedido.idPedido;
                this.InsertPedidoDetalle(pedidoDetalle);
            }

            foreach (PedidoAdjunto pedidoAdjunto in pedido.pedidoAdjuntoList)
            {
                pedidoAdjunto.idPedido = pedido.idPedido;
                this.InsertPedidoAdjunto(pedidoAdjunto);
            }
            this.Commit();
        }







        public void ActualizarPedido(Pedido pedido)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pu_actualizarPedido");
            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", pedido.numeroReferenciaCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaAdicional", pedido.numeroReferenciaAdicional);
            InputParameterAdd.Varchar(objCommand, "observaciones", pedido.observaciones);
            InputParameterAdd.Varchar(objCommand, "observacionesGuiaRemision", pedido.observacionesGuiaRemision);
            InputParameterAdd.Varchar(objCommand, "observacionesFactura", pedido.observacionesFactura);

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

            InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", pedido.numeroReferenciaCliente); //puede ser null
            if (pedido.tipoPedido == Pedido.tiposPedido.Venta ||
                pedido.tipoPedido == Pedido.tiposPedido.TrasladoInterno ||
                pedido.tipoPedido == Pedido.tiposPedido.TransferenciaGratuitaEntregada ||
                pedido.tipoPedido == Pedido.tiposPedido.ComodatoEntregado ||
                pedido.tipoPedido == Pedido.tiposPedido.PrestamoEntregado)
            {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", pedido.direccionEntrega.idDireccionEntrega); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", pedido.direccionEntrega.descripcion);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", pedido.direccionEntrega.contacto); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", pedido.direccionEntrega.telefono); //puede ser null
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", null); //puede ser null


            }

            InputParameterAdd.DateTime(objCommand, "fechaSolicitud", pedido.fechaSolicitud);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde.Value);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta.Value);

            DateTime dtTmp = DateTime.Now;
            String[] horaEntregaDesdeArray = pedido.horaEntregaDesde.Split(':');
            DateTime horaEntregaDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaDesdeArray[0]), Int32.Parse(horaEntregaDesdeArray[1]), 0);
            String[] horaEntregaHastaArray = pedido.horaEntregaHasta.Split(':');
            DateTime horaEntregaHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaHastaArray[0]), Int32.Parse(horaEntregaHastaArray[1]), 0);
            InputParameterAdd.DateTime(objCommand, "horaEntregaDesde", horaEntregaDesde);
            InputParameterAdd.DateTime(objCommand, "horaEntregaHasta", horaEntregaHasta);
            if (pedido.tipoPedido == Pedido.tiposPedido.Venta ||
               pedido.tipoPedido == Pedido.tiposPedido.TrasladoInterno ||
               pedido.tipoPedido == Pedido.tiposPedido.TransferenciaGratuitaEntregada ||
               pedido.tipoPedido == Pedido.tiposPedido.ComodatoEntregado ||
               pedido.tipoPedido == Pedido.tiposPedido.PrestamoEntregado)
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
            InputParameterAdd.Varchar(objCommand, "ubigeoEntrega", pedido.ubigeoEntrega.Id);
            InputParameterAdd.Decimal(objCommand, "otrosCargos", pedido.otrosCargos);
            InputParameterAdd.Char(objCommand, "tipoPedido", ((char)pedido.tipoPedido).ToString());
            InputParameterAdd.Varchar(objCommand, "observacionesGuiaRemision", pedido.observacionesGuiaRemision);
            InputParameterAdd.Varchar(objCommand, "observacionesFactura", pedido.observacionesFactura);


            ExecuteNonQuery(objCommand);


            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                pedidoDetalle.idPedido = pedido.idPedido;
                pedidoDetalle.usuario = pedido.usuario;
                this.InsertPedidoDetalle(pedidoDetalle);
            }

            foreach (PedidoAdjunto pedidoAdjunto in pedido.pedidoAdjuntoList)
            {
                pedidoAdjunto.idPedido = pedido.idPedido;
                this.InsertPedidoAdjunto(pedidoAdjunto);
            }
            this.Commit();
        }




        public void UpdatePedidoCompra(Pedido pedido)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pu_pedidoCompra");

            InputParameterAdd.Guid(objCommand, "idPedido", pedido.idPedido);
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", pedido.numeroGrupoPedido); //puede ser null

            if (pedido.cotizacion.idCotizacion == Guid.Empty)
                InputParameterAdd.Guid(objCommand, "idCotizacion", null); //puede ser null
            else
                InputParameterAdd.Guid(objCommand, "idCotizacion", pedido.cotizacion.idCotizacion); //puede ser null

            InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", pedido.numeroReferenciaCliente); //puede ser null
            if (pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.Compra ||
               pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.TransferenciaGratuitaRecibida ||
               pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.ComodatoRecibido ||
               pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.PrestamoRecibido)
            {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", null); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", null); //puede ser null
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", pedido.direccionEntrega.idDireccionEntrega); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", pedido.direccionEntrega.descripcion);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", pedido.direccionEntrega.contacto); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", pedido.direccionEntrega.telefono); //puede ser null
            }

            InputParameterAdd.DateTime(objCommand, "fechaSolicitud", pedido.fechaSolicitud);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde.Value);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta.Value);

            DateTime dtTmp = DateTime.Now;
            String[] horaEntregaDesdeArray = pedido.horaEntregaDesde.Split(':');
            DateTime horaEntregaDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaDesdeArray[0]), Int32.Parse(horaEntregaDesdeArray[1]), 0);
            String[] horaEntregaHastaArray = pedido.horaEntregaHasta.Split(':');
            DateTime horaEntregaHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaHastaArray[0]), Int32.Parse(horaEntregaHastaArray[1]), 0);
            InputParameterAdd.DateTime(objCommand, "horaEntregaDesde", horaEntregaDesde);
            InputParameterAdd.DateTime(objCommand, "horaEntregaHasta", horaEntregaHasta);
            if (pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.Compra ||
               pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.TransferenciaGratuitaRecibida ||
               pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.ComodatoRecibido ||
               pedido.tipoPedidoCompra == Pedido.tiposPedidoCompra.PrestamoRecibido)
            {
                InputParameterAdd.Guid(objCommand, "idSolicitante", null);
                InputParameterAdd.Varchar(objCommand, "contactoPedido", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "correoContactoPedido", null);  //puede ser null
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idSolicitante", pedido.solicitante.idSolicitante);
                InputParameterAdd.Varchar(objCommand, "contactoPedido", pedido.solicitante.nombre);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoPedido", pedido.solicitante.telefono);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "correoContactoPedido", pedido.solicitante.correo);  //puede ser null
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
            InputParameterAdd.Varchar(objCommand, "ubigeoEntrega", pedido.ubigeoEntrega.Id);
            InputParameterAdd.Decimal(objCommand, "otrosCargos", pedido.otrosCargos);
            InputParameterAdd.Char(objCommand, "tipoPedido", ((char)pedido.tipoPedidoCompra).ToString());

            ExecuteNonQuery(objCommand);


            foreach (PedidoDetalle pedidoDetalle in pedido.pedidoDetalleList)
            {
                pedidoDetalle.idPedido = pedido.idPedido;
                pedidoDetalle.usuario = pedido.usuario;
                this.InsertPedidoDetalle(pedidoDetalle);
            }

            foreach (PedidoAdjunto pedidoAdjunto in pedido.pedidoAdjuntoList)
            {
                pedidoAdjunto.idPedido = pedido.idPedido;
                this.InsertPedidoAdjunto(pedidoAdjunto);
            }
            this.Commit();
        }


        public void InsertPedidoDetalle(PedidoDetalle pedidoDetalle)
        {
            var objCommand = GetSqlCommand("pi_pedidoDetalle");
            InputParameterAdd.Guid(objCommand, "idPedido", pedidoDetalle.idPedido);
            InputParameterAdd.Guid(objCommand, "idProducto", pedidoDetalle.producto.idProducto);
            InputParameterAdd.Int(objCommand, "cantidad", pedidoDetalle.cantidad);
            //Siempre se almacena el precio sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "precioSinIGV", pedidoDetalle.producto.precioSinIgv);
            //Siempre se almacena el costo sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "costoSinIGV", pedidoDetalle.producto.costoSinIgv);
            InputParameterAdd.Decimal(objCommand, "equivalencia", pedidoDetalle.producto.equivalencia);
            InputParameterAdd.Varchar(objCommand, "unidad", pedidoDetalle.unidad);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", pedidoDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "precioNeto", pedidoDetalle.precioNeto);
            InputParameterAdd.Int(objCommand, "esPrecioAlternativo", pedidoDetalle.esPrecioAlternativo?1:0);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedidoDetalle.usuario.idUsuario);
            InputParameterAdd.Decimal(objCommand, "flete", pedidoDetalle.flete);
            InputParameterAdd.Varchar(objCommand, "observaciones", pedidoDetalle.observacion);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);
          
            pedidoDetalle.idPedidoDetalle = (Guid)objCommand.Parameters["@newId"].Value;
        }

        public void InsertPedidoAdjunto(PedidoAdjunto pedidoAdjunto)
        {
            var objCommand = GetSqlCommand("pi_pedidoAdjunto");
            InputParameterAdd.Guid(objCommand, "idPedido", pedidoAdjunto.idPedido);
            InputParameterAdd.Guid(objCommand, "idCliente", pedidoAdjunto.idCliente);
            InputParameterAdd.Varchar(objCommand, "nombre", pedidoAdjunto.nombre);
            InputParameterAdd.VarBinary(objCommand, "adjunto", pedidoAdjunto.adjunto);

            InputParameterAdd.Guid(objCommand, "idUsuario", pedidoAdjunto.usuario.idUsuario);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);

            pedidoAdjunto.idPedidoAdjunto = (Guid)objCommand.Parameters["@newId"].Value;
        }

        /*
        public Pedido aprobarPedido(Pedido pedido)
        {
            var objCommand = GetSqlCommand("pu_aprobarPedido");

            InputParameterAdd.BigInt(objCommand, "codigo", pedido.codigo);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoCotizacion.estado);
            InputParameterAdd.Varchar(objCommand, "observacion", pedido.seguimientoCotizacion.observacion);

            ExecuteNonQuery(objCommand);
            return pedido;

        }*/

        public Pedido ProgramarPedido(Pedido pedido,Usuario usuario)
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
                CotizacionDetalle cotizacionDetalle = new CotizacionDetalle(usuario);
                cotizacionDetalle.producto = new Producto();


                //No se cuenta con IdCotizacionDetalle
                cotizacionDetalle.cantidad = 1;
                cotizacionDetalle.producto.equivalencia = Convert.ToInt32(Converter.GetDecimal(row, "equivalencia"));
                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                cotizacionDetalle.flete = Converter.GetDecimal(row, "flete");


                cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");


    
                //if (cotizacionDetalle.esPrecioAlternativo)
               // {
                    cotizacionDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * cotizacionDetalle.producto.equivalencia;
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

        public Pedido SelectPedido(Pedido pedido, Usuario usuario)
        {
            var objCommand = GetSqlCommand("ps_pedido");
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

                pedido.tipo = (Pedido.tipos)Char.Parse(Converter.GetString(row, "tipo"));
                if (pedido.tipo == Pedido.tipos.Venta)
                    pedido.tipoPedido = (Pedido.tiposPedido)Char.Parse(Converter.GetString(row, "tipo_pedido"));
                else
                    pedido.tipoPedidoCompra = (Pedido.tiposPedidoCompra)Char.Parse(Converter.GetString(row, "tipo_pedido"));
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



                pedido.ciudad = new Ciudad();
                pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

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
                PedidoDetalle pedidoDetalle = new PedidoDetalle(usuario);

                pedidoDetalle.producto = new Producto();

                pedidoDetalle.idPedidoDetalle = Converter.GetGuid(row, "id_pedido_detalle");
                pedidoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                pedidoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.producto.equivalencia = Convert.ToInt32(Converter.GetDecimal(row, "equivalencia"));
                pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                pedidoDetalle.flete = Converter.GetDecimal(row, "flete");


                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.producto.equivalencia;
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

                if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    precioClienteProducto.fechaFinVigencia = null;
                }
                else
                {
                    precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                }

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



                    if (row["fecha_emision_factura"] == DBNull.Value)
                        movimientoAlmacen.documentoVenta.fechaEmision = null;
                    else
                        movimientoAlmacen.documentoVenta.fechaEmision = Converter.GetDateTime(row, "fecha_emision_factura");



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
                pedidoAdjunto.idPedidoAdjunto = Converter.GetGuid(row, "id_pedido_adjunto");
                pedidoAdjunto.adjunto = Converter.GetBytes(row, "adjunto");
                pedidoAdjunto.nombre = Converter.GetString(row, "nombre");
                pedido.pedidoAdjuntoList.Add(pedidoAdjunto);
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
                pedido.numeroPedido = Converter.GetLong(row,"numero");
                pedido.numeroGrupoPedido = Converter.GetLong(row, "numero_grupo");
                pedido.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                pedido.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
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

                pedido.tipo = (Pedido.tipos)Char.Parse(Converter.GetString(row, "tipo"));
                if(pedido.tipo == Pedido.tipos.Venta)
                    pedido.tipoPedido = (Pedido.tiposPedido)Char.Parse(Converter.GetString(row, "tipo_pedido"));
                else
                    pedido.tipoPedidoCompra = (Pedido.tiposPedidoCompra)Char.Parse(Converter.GetString(row, "tipo_pedido"));
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



                pedido.ciudad = new Ciudad();
                pedido.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                pedido.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

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
                PedidoDetalle pedidoDetalle = new PedidoDetalle(usuario);
             
                pedidoDetalle.producto = new Producto();

                pedidoDetalle.idPedidoDetalle = Converter.GetGuid(row, "id_pedido_detalle");
                pedidoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                pedidoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");
                pedidoDetalle.producto.equivalencia = Convert.ToInt32(Converter.GetDecimal(row, "equivalencia"));
                pedidoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                pedidoDetalle.flete = Converter.GetDecimal(row, "flete");


                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                pedidoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                pedidoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (pedidoDetalle.esPrecioAlternativo)
                {
                    pedidoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * pedidoDetalle.producto.equivalencia;
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

                if (row["fecha_fin_vigencia"] == DBNull.Value)
                {
                    precioClienteProducto.fechaFinVigencia = null;
                }
                else
                {
                    precioClienteProducto.fechaFinVigencia = Converter.GetDateTime(row, "fecha_fin_vigencia");
                }

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
                    ubigeo = new Ubigeo { Id = Converter.GetString(row, "ubigeo")  }
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


     
                    if (row["fecha_emision_factura"] == DBNull.Value)
                        movimientoAlmacen.documentoVenta.fechaEmision = null;
                    else
                        movimientoAlmacen.documentoVenta.fechaEmision = Converter.GetDateTime(row, "fecha_emision_factura");



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
                pedidoAdjunto.idPedidoAdjunto = Converter.GetGuid(row, "id_pedido_adjunto");
                pedidoAdjunto.adjunto = Converter.GetBytes(row, "adjunto");
                pedidoAdjunto.nombre = Converter.GetString(row, "nombre");
                pedido.pedidoAdjuntoList.Add(pedidoAdjunto);
            }

            return pedido;
        }




        public PedidoAdjunto SelectArchivoAdjunto(PedidoAdjunto pedidoAdjunto)
        {
            var objCommand = GetSqlCommand("ps_archivoAdjunto");
            InputParameterAdd.Guid(objCommand, "idArchivoAdjunto", pedidoAdjunto.idPedidoAdjunto);
            DataTable dataTable = Execute(objCommand);

            foreach (DataRow row in dataTable.Rows)
            {
                pedidoAdjunto.nombre = Converter.GetString(row, "nombre");
                pedidoAdjunto.adjunto = Converter.GetBytes(row, "adjunto");
            }

            return pedidoAdjunto;
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
            InputParameterAdd.Char(objCommand, "tipoPedido", ((Char)pedido.tipoPedidoVentaBusqueda).ToString());
            InputParameterAdd.DateTime(objCommand, "fechaSolicitudDesde", new DateTime(pedido.fechaSolicitudDesde.Year, pedido.fechaSolicitudDesde.Month, pedido.fechaSolicitudDesde.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaSolicitudHasta", new DateTime(pedido.fechaSolicitudHasta.Year, pedido.fechaSolicitudHasta.Month, pedido.fechaSolicitudHasta.Day, 23, 59, 59));
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", pedido.fechaEntregaDesde == null? pedido.fechaEntregaDesde : new DateTime(pedido.fechaEntregaDesde.Value.Year, pedido.fechaEntregaDesde.Value.Month, pedido.fechaEntregaDesde.Value.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", pedido.fechaEntregaHasta == null ? pedido.fechaEntregaDesde :  new DateTime(pedido.fechaEntregaHasta.Value.Year, pedido.fechaEntregaHasta.Value.Month, pedido.fechaEntregaHasta.Value.Day, 23, 59, 59)); 
            InputParameterAdd.DateTime(objCommand, "fechaProgramacionDesde", pedido.fechaProgramacionDesde == null ? pedido.fechaProgramacionDesde : new DateTime(pedido.fechaProgramacionDesde.Value.Year, pedido.fechaProgramacionDesde.Value.Month, pedido.fechaProgramacionDesde.Value.Day, 0, 0, 0));  //pedido.fechaProgramacionDesde);
            InputParameterAdd.DateTime(objCommand, "fechaProgramacionHasta", pedido.fechaProgramacionHasta == null ? pedido.fechaProgramacionHasta : new DateTime(pedido.fechaProgramacionHasta.Value.Year, pedido.fechaProgramacionHasta.Value.Month, pedido.fechaProgramacionHasta.Value.Day, 0, 0, 0));  //pedido.fechaProgramacionDesde); //pedido.fechaProgramacionHasta);



            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            InputParameterAdd.Int(objCommand, "estadoCrediticio", (int)pedido.seguimientoCrediticioPedido.estado);
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
                pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");

                pedido.FechaRegistro  = Converter.GetDateTime(row, "fecha_registro");
                pedido.FechaRegistro = pedido.FechaRegistro.AddHours(-5);
                pedido.stockConfirmado = Converter.GetBool(row, "stock_confirmado");
                if (row["fecha_programacion"] == DBNull.Value)
                    pedido.fechaProgramacion = null;
                else
                    pedido.fechaProgramacion = Converter.GetDateTime(row, "fecha_programacion");
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




        public List<Pedido> SelectPedidosCompra(Pedido pedido)
        {
            var objCommand = GetSqlCommand("ps_pedidosCompra");
            InputParameterAdd.BigInt(objCommand, "numero", pedido.numeroPedido);
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", pedido.numeroGrupoPedido);
            InputParameterAdd.Guid(objCommand, "idCliente", pedido.cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "idCiudad", pedido.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            InputParameterAdd.Guid(objCommand, "idUsuarioBusqueda", pedido.usuarioBusqueda.idUsuario);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", pedido.numeroReferenciaCliente);
            InputParameterAdd.Char(objCommand, "tipoPedido", ((Char)pedido.tipoPedidoCompraBusqueda).ToString());
            InputParameterAdd.DateTime(objCommand, "fechaSolicitudDesde", new DateTime(pedido.fechaSolicitudDesde.Year, pedido.fechaSolicitudDesde.Month, pedido.fechaSolicitudDesde.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaSolicitudHasta", new DateTime(pedido.fechaSolicitudHasta.Year, pedido.fechaSolicitudHasta.Month, pedido.fechaSolicitudHasta.Day, 23, 59, 59));
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", new DateTime(pedido.fechaEntregaDesde.Value.Year, pedido.fechaEntregaDesde.Value.Month, pedido.fechaEntregaDesde.Value.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", new DateTime(pedido.fechaEntregaHasta.Value.Year, pedido.fechaEntregaHasta.Value.Month, pedido.fechaEntregaHasta.Value.Day, 23, 59, 59));
            InputParameterAdd.DateTime(objCommand, "fechaProgramacionDesde", pedido.fechaProgramacionDesde);
            InputParameterAdd.DateTime(objCommand, "fechaProgramacionHasta", pedido.fechaProgramacionHasta);



            InputParameterAdd.Int(objCommand, "estado", (int)pedido.seguimientoPedido.estado);
            InputParameterAdd.Int(objCommand, "estadoCrediticio", (int)pedido.seguimientoCrediticioPedido.estado);
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
                pedido.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");

                pedido.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                pedido.FechaRegistro = pedido.FechaRegistro.AddHours(-5);

                if (row["fecha_programacion"] == DBNull.Value)
                    pedido.fechaProgramacion = null;
                else
                    pedido.fechaProgramacion = Converter.GetDateTime(row, "fecha_programacion");
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
            InputParameterAdd.Int(objCommand, "stockConfirmado", pedido.stockConfirmado ? 1 : 0);
            InputParameterAdd.Guid(objCommand, "idUsuario", pedido.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }



    }
}
