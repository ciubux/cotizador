using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;
using System.Linq;
using System.Data.SqlClient;

namespace DataLayer
{
    public class RequerimientoDAL : DaoBase
    {
        public RequerimientoDAL(IDalSettings settings) : base(settings)
        {
        }

        public RequerimientoDAL() : this(new CotizadorSettings())
        {
        }

        #region Requerimientos de Venta

        public void InsertRequerimiento(Requerimiento requerimiento)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("CLIENTE.pi_requerimiento");
    
            InputParameterAdd.Guid(objCommand, "idCiudad", requerimiento.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", requerimiento.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", requerimiento.numeroReferenciaCliente); //puede ser null

            if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Venta)
            {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", requerimiento.direccionEntrega.idDireccionEntrega); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", requerimiento.direccionEntrega.descripcion);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", requerimiento.direccionEntrega.contacto); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", requerimiento.direccionEntrega.telefono); //puede ser null
                InputParameterAdd.Varchar(objCommand, "codigoCliente", requerimiento.direccionEntrega.codigoCliente); //puede ser null
                InputParameterAdd.Varchar(objCommand, "codigoMP", requerimiento.direccionEntrega.codigoMP); //puede ser null
                InputParameterAdd.Varchar(objCommand, "nombre", requerimiento.direccionEntrega.nombre); //puede ser null
                InputParameterAdd.Varchar(objCommand, "observacionesDireccionEntrega", requerimiento.direccionEntrega.observaciones); //puede ser null
            }
            else if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Compra)
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
                if (requerimiento.tipoRequerimientoAlmacen == Requerimiento.tiposRequerimientoAlmacen.TrasladoInterno ||
                    requerimiento.tipoRequerimientoAlmacen == Requerimiento.tiposRequerimientoAlmacen.PrestamoEntregado 
                )
                {
                    InputParameterAdd.Guid(objCommand, "idDireccionEntrega", requerimiento.direccionEntrega.idDireccionEntrega); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "direccionEntrega", requerimiento.direccionEntrega.descripcion);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "contactoEntrega", requerimiento.direccionEntrega.contacto); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", requerimiento.direccionEntrega.telefono); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "codigoCliente", requerimiento.direccionEntrega.codigoCliente); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "codigoMP", requerimiento.direccionEntrega.codigoMP); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "nombre", requerimiento.direccionEntrega.nombre); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "observacionesDireccionEntrega", requerimiento.direccionEntrega.observaciones); //puede ser null
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
            
            InputParameterAdd.DateTime(objCommand, "fechaSolicitud", requerimiento.fechaSolicitud);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", requerimiento.fechaEntregaDesde.Value);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", requerimiento.fechaEntregaHasta.Value);
            

            DateTime dtTmp = DateTime.Now;
            String[] horaEntregaDesdeArray = requerimiento.horaEntregaDesde.Split(':');
            DateTime horaEntregaDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaDesdeArray[0]), Int32.Parse(horaEntregaDesdeArray[1]),0);
            String[] horaEntregaHastaArray = requerimiento.horaEntregaHasta.Split(':');
            DateTime horaEntregaHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaHastaArray[0]), Int32.Parse(horaEntregaHastaArray[1]),0);
            InputParameterAdd.DateTime(objCommand, "horaEntregaDesde", horaEntregaDesde);
            InputParameterAdd.DateTime(objCommand, "horaEntregaHasta", horaEntregaHasta);

            if (requerimiento.horaEntregaAdicionalDesde != null && !requerimiento.horaEntregaAdicionalDesde.Equals("")
                && requerimiento.horaEntregaAdicionalDesde != null && !requerimiento.horaEntregaAdicionalDesde.Equals(""))
            {
                String[] horaEntregaAdicionalDesdeArray = requerimiento.horaEntregaAdicionalDesde.Split(':');
                DateTime horaEntregaAdicionalDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaAdicionalDesdeArray[0]), Int32.Parse(horaEntregaAdicionalDesdeArray[1]), 0);
                String[] horaEntregaAdicionalHastaArray = requerimiento.horaEntregaAdicionalHasta.Split(':');
                DateTime horaEntregaAdicionalHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaAdicionalHastaArray[0]), Int32.Parse(horaEntregaAdicionalHastaArray[1]), 0);
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalDesde", horaEntregaAdicionalDesde);
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalHasta", horaEntregaAdicionalHasta);
            }
            else
            {
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalDesde", null);
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalHasta", null);
            }


            if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Venta)
            {
                InputParameterAdd.Guid(objCommand, "idSolicitante", requerimiento.solicitante.idSolicitante);
                InputParameterAdd.Varchar(objCommand, "contactoRequerimiento", requerimiento.solicitante.nombre);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoRequerimiento", requerimiento.solicitante.telefono);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "correoContactoRequerimiento", requerimiento.solicitante.correo);  //puede ser null
            }
            else if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Compra)
            {
                InputParameterAdd.Guid(objCommand, "idSolicitante", null);
                InputParameterAdd.Varchar(objCommand, "contactoRequerimiento", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoRequerimiento", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "correoContactoRequerimiento", null);  //puede ser null
            }
            else
            {
                if (requerimiento.tipoRequerimientoAlmacen == Requerimiento.tiposRequerimientoAlmacen.TrasladoInterno ||
                    requerimiento.tipoRequerimientoAlmacen == Requerimiento.tiposRequerimientoAlmacen.PrestamoEntregado 
                )
                {
                    InputParameterAdd.Guid(objCommand, "idSolicitante", requerimiento.solicitante.idSolicitante);
                    InputParameterAdd.Varchar(objCommand, "contactoRequerimiento", requerimiento.solicitante.nombre);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoRequerimiento", requerimiento.solicitante.telefono);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "correoContactoRequerimiento", requerimiento.solicitante.correo);  //puede ser null
                }
                else
                {
                    InputParameterAdd.Guid(objCommand, "idSolicitante", null);
                    InputParameterAdd.Varchar(objCommand, "contactoRequerimiento", null);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoRequerimiento", null);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "correoContactoRequerimiento", null);  //puede ser null
                }
            }

            InputParameterAdd.SmallInt(objCommand, "incluidoIGV", (short)(requerimiento.incluidoIGV?1:0));
            InputParameterAdd.Decimal(objCommand, "tasaIGV", Constantes.IGV);
            InputParameterAdd.Decimal(objCommand, "igv", requerimiento.montoIGV);
            InputParameterAdd.Decimal(objCommand, "total", requerimiento.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", requerimiento.observaciones);  //puede ser null
            InputParameterAdd.Guid(objCommand, "idUsuario", requerimiento.usuario.idUsuario);
            InputParameterAdd.Bit(objCommand, "esPagoContado", requerimiento.esPagoContado);
            InputParameterAdd.Varchar(objCommand, "ubigeoEntrega", requerimiento.ubigeoEntrega.Id);
            InputParameterAdd.Char(objCommand, "tipoRequerimiento", ((char)requerimiento.tipoRequerimiento).ToString());
            InputParameterAdd.Decimal(objCommand, "otrosCargos", requerimiento.otrosCargos);
            InputParameterAdd.Char(objCommand, "tipo", ((char)requerimiento.claseRequerimiento).ToString());
            InputParameterAdd.Int(objCommand, "idClienteSunat", requerimiento.usuario.idClienteSunat);
            InputParameterAdd.Guid(objCommand, "idDireccionEntregaAlmacen", requerimiento.direccionEntrega.direccionEntregaAlmacen.idDireccionEntrega);
            InputParameterAdd.Bit(objCommand, "excedioPresupuesto", requerimiento.excedioPresupuesto);
            InputParameterAdd.Decimal(objCommand, "topePresupuesto", requerimiento.topePresupuesto);
            OutputParameterAdd.Int(objCommand, "idRequerimiento");
            ExecuteNonQuery(objCommand);

            requerimiento.idRequerimiento = (Int32)objCommand.Parameters["@idRequerimiento"].Value;

            foreach (RequerimientoDetalle requerimientoDetalle in requerimiento.requerimientoDetalleList)
            {
                requerimientoDetalle.idRequerimiento = requerimiento.idRequerimiento;
                this.InsertRequerimientoDetalle(requerimientoDetalle, requerimiento.usuario);
            }

            this.Commit();
        }
        

        public void UpdateRequerimiento(Requerimiento requerimiento)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("CLIENTE.pu_requerimiento");
            
            InputParameterAdd.Guid(objCommand, "idCiudad", requerimiento.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idCliente", requerimiento.cliente.idCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", requerimiento.numeroReferenciaCliente); //puede ser null

            if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Venta)
            {
                InputParameterAdd.Guid(objCommand, "idDireccionEntrega", requerimiento.direccionEntrega.idDireccionEntrega); //puede ser null
                InputParameterAdd.Varchar(objCommand, "direccionEntrega", requerimiento.direccionEntrega.descripcion);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "contactoEntrega", requerimiento.direccionEntrega.contacto); //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", requerimiento.direccionEntrega.telefono); //puede ser null
                InputParameterAdd.Varchar(objCommand, "codigoCliente", requerimiento.direccionEntrega.codigoCliente); //puede ser null
                InputParameterAdd.Varchar(objCommand, "codigoMP", requerimiento.direccionEntrega.codigoMP); //puede ser null
                InputParameterAdd.Varchar(objCommand, "nombre", requerimiento.direccionEntrega.nombre); //puede ser null
                InputParameterAdd.Varchar(objCommand, "observacionesDireccionEntrega", requerimiento.direccionEntrega.observaciones); //puede ser null
            }
            else if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Compra)
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
                if (requerimiento.tipoRequerimientoAlmacen == Requerimiento.tiposRequerimientoAlmacen.TrasladoInterno ||
                    requerimiento.tipoRequerimientoAlmacen == Requerimiento.tiposRequerimientoAlmacen.PrestamoEntregado
                )
                {
                    InputParameterAdd.Guid(objCommand, "idDireccionEntrega", requerimiento.direccionEntrega.idDireccionEntrega); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "direccionEntrega", requerimiento.direccionEntrega.descripcion);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "contactoEntrega", requerimiento.direccionEntrega.contacto); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoEntrega", requerimiento.direccionEntrega.telefono); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "codigoCliente", requerimiento.direccionEntrega.codigoCliente); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "codigoMP", requerimiento.direccionEntrega.codigoMP); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "nombre", requerimiento.direccionEntrega.nombre); //puede ser null
                    InputParameterAdd.Varchar(objCommand, "observacionesDireccionEntrega", requerimiento.direccionEntrega.observaciones); //puede ser null
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

            InputParameterAdd.DateTime(objCommand, "fechaSolicitud", requerimiento.fechaSolicitud);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", requerimiento.fechaEntregaDesde.Value);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", requerimiento.fechaEntregaHasta.Value);

         

            DateTime dtTmp = DateTime.Now;
            String[] horaEntregaDesdeArray = requerimiento.horaEntregaDesde.Split(':');
            DateTime horaEntregaDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaDesdeArray[0]), Int32.Parse(horaEntregaDesdeArray[1]), 0);
            String[] horaEntregaHastaArray = requerimiento.horaEntregaHasta.Split(':');
            DateTime horaEntregaHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaHastaArray[0]), Int32.Parse(horaEntregaHastaArray[1]), 0);
            InputParameterAdd.DateTime(objCommand, "horaEntregaDesde", horaEntregaDesde);
            InputParameterAdd.DateTime(objCommand, "horaEntregaHasta", horaEntregaHasta);

            if (requerimiento.horaEntregaAdicionalDesde != null && !requerimiento.horaEntregaAdicionalDesde.Equals("")
                && requerimiento.horaEntregaAdicionalDesde != null && !requerimiento.horaEntregaAdicionalDesde.Equals(""))
            {
                String[] horaEntregaAdicionalDesdeArray = requerimiento.horaEntregaAdicionalDesde.Split(':');
                DateTime horaEntregaAdicionalDesde = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaAdicionalDesdeArray[0]), Int32.Parse(horaEntregaAdicionalDesdeArray[1]), 0);
                String[] horaEntregaAdicionalHastaArray = requerimiento.horaEntregaAdicionalHasta.Split(':');
                DateTime horaEntregaAdicionalHasta = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaEntregaAdicionalHastaArray[0]), Int32.Parse(horaEntregaAdicionalHastaArray[1]), 0);
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalDesde", horaEntregaAdicionalDesde);
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalHasta", horaEntregaAdicionalHasta);
            } else
            {
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalDesde", null);
                InputParameterAdd.DateTime(objCommand, "horaEntregaAdicionalHasta", null);
            }


            if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Venta)
            {
                InputParameterAdd.Guid(objCommand, "idSolicitante", requerimiento.solicitante.idSolicitante);
                InputParameterAdd.Varchar(objCommand, "contactoRequerimiento", requerimiento.solicitante.nombre);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoRequerimiento", requerimiento.solicitante.telefono);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "correoContactoRequerimiento", requerimiento.solicitante.correo);  //puede ser null
            }
            else if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Compra)
            {
                InputParameterAdd.Guid(objCommand, "idSolicitante", null);
                InputParameterAdd.Varchar(objCommand, "contactoRequerimiento", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "telefonoContactoRequerimiento", null);  //puede ser null
                InputParameterAdd.Varchar(objCommand, "correoContactoRequerimiento", null);  //puede ser null
            }
            else
            {
                if (requerimiento.tipoRequerimientoAlmacen == Requerimiento.tiposRequerimientoAlmacen.TrasladoInterno ||
                    requerimiento.tipoRequerimientoAlmacen == Requerimiento.tiposRequerimientoAlmacen.PrestamoEntregado
                )
                {
                    InputParameterAdd.Guid(objCommand, "idSolicitante", requerimiento.solicitante.idSolicitante);
                    InputParameterAdd.Varchar(objCommand, "contactoRequerimiento", requerimiento.solicitante.nombre);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoRequerimiento", requerimiento.solicitante.telefono);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "correoContactoRequerimiento", requerimiento.solicitante.correo);  //puede ser null
                }
                else
                {
                    InputParameterAdd.Guid(objCommand, "idSolicitante", null);
                    InputParameterAdd.Varchar(objCommand, "contactoRequerimiento", null);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "telefonoContactoRequerimiento", null);  //puede ser null
                    InputParameterAdd.Varchar(objCommand, "correoContactoRequerimiento", null);  //puede ser null
                }
            }

            InputParameterAdd.SmallInt(objCommand, "incluidoIGV", (short)(requerimiento.incluidoIGV ? 1 : 0));
            InputParameterAdd.Decimal(objCommand, "tasaIGV", Constantes.IGV);
            InputParameterAdd.Decimal(objCommand, "igv", requerimiento.montoIGV);
            InputParameterAdd.Decimal(objCommand, "total", requerimiento.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", requerimiento.observaciones);  //puede ser null
            InputParameterAdd.Guid(objCommand, "idUsuario", requerimiento.usuario.idUsuario);
            InputParameterAdd.Bit(objCommand, "esPagoContado", requerimiento.esPagoContado);

            /*if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Venta)
            {
                InputParameterAdd.Char(objCommand, "tipoRequerimiento", ((char)requerimiento.tipoRequerimiento).ToString());
            }
            else if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Compra)
            {
                InputParameterAdd.Char(objCommand, "tipoRequerimiento", ((char)requerimiento.tipoRequerimientoCompra).ToString());
            }
            else if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Almacen)
            {
                
            }*/
            InputParameterAdd.Varchar(objCommand, "ubigeoEntrega", requerimiento.ubigeoEntrega.Id);
            InputParameterAdd.Char(objCommand, "tipoRequerimiento", ((char)requerimiento.tipoRequerimientoAlmacen).ToString());            
            InputParameterAdd.Decimal(objCommand, "otrosCargos", requerimiento.otrosCargos);
            InputParameterAdd.Char(objCommand, "tipo", ((char)requerimiento.claseRequerimiento).ToString());
            InputParameterAdd.Int(objCommand, "idClienteSunat", requerimiento.usuario.idClienteSunat);
            InputParameterAdd.Guid(objCommand, "idDireccionEntregaAlmacen", requerimiento.direccionEntrega.direccionEntregaAlmacen.idDireccionEntrega);
            InputParameterAdd.Bit(objCommand, "excedioPresupuesto", requerimiento.excedioPresupuesto);
            InputParameterAdd.Decimal(objCommand, "topePresupuesto", requerimiento.topePresupuesto);
            InputParameterAdd.Int(objCommand, "idRequerimiento", requerimiento.idRequerimiento);
            ExecuteNonQuery(objCommand);


            foreach (RequerimientoDetalle requerimientoDetalle in requerimiento.requerimientoDetalleList)
            {
                requerimientoDetalle.idRequerimiento = requerimiento.idRequerimiento;
                //requerimientoDetalle.usuario = requerimiento.usuario;
                this.InsertRequerimientoDetalle(requerimientoDetalle, requerimiento.usuario);
            }
       this.Commit();
        }           
    
        #endregion


        #region General

        public void ActualizarRequerimiento(Requerimiento requerimiento)
        {
            this.BeginTransaction(IsolationLevel.ReadCommitted);
            var objCommand = GetSqlCommand("pu_actualizarRequerimiento");
            InputParameterAdd.Int(objCommand, "idRequerimiento", requerimiento.idRequerimiento);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaCliente", requerimiento.numeroReferenciaCliente);
            InputParameterAdd.Varchar(objCommand, "numeroReferenciaAdicional", requerimiento.numeroReferenciaAdicional);
            InputParameterAdd.DateTime(objCommand, "fechaEntregaExtendida", !requerimiento.fechaEntregaExtendida.HasValue ? null: requerimiento.fechaEntregaExtendida);
            InputParameterAdd.Varchar(objCommand, "observaciones", requerimiento.observaciones);
            InputParameterAdd.Varchar(objCommand, "observacionesGuiaRemision", requerimiento.observacionesGuiaRemision);
            InputParameterAdd.Varchar(objCommand, "observacionesFactura", requerimiento.observacionesFactura);
   
            ExecuteNonQuery(objCommand);

         
            this.Commit();
        }


        public void AprobarRequerimientos(List<Requerimiento> requerimientoList, Int64 numeroGrupo)
        {

            this.BeginTransaction(IsolationLevel.ReadCommitted);


            var objCommand = GetSqlCommand("CLIENTE.pu_AprobarRequerimientos");



            DataTable tvp = new DataTable();
            tvp.Columns.Add(new DataColumn("idRequerimientoList", typeof(Int32)));

            // populate DataTable from your List here
            foreach (Requerimiento requerimiento in requerimientoList)
                tvp.Rows.Add(requerimiento.idRequerimiento);

            SqlParameter tvparam = objCommand.Parameters.AddWithValue("@idRequerimientoList", tvp);
            
            

            // these next lines are important to map the C# DataTable object to the correct SQL User Defined Type
            tvparam.SqlDbType = SqlDbType.Structured;
            tvparam.TypeName = "dbo.IntegerList";

            //InputParameterAdd.Varchar(objCommand, "idProductoList", tvparam);
            InputParameterAdd.BigInt(objCommand, "numeroGrupo", numeroGrupo);

            //InputParameterAdd.Varchar(objCommand, "idMovimientoAlmacenList", idMovimientoAlmacenList);
            ExecuteNonQuery(objCommand);

            this.Commit();
        }


        public void InsertRequerimientoDetalle(RequerimientoDetalle requerimientoDetalle, Usuario usuario)
        {
            var objCommand = GetSqlCommand("CLIENTE.pi_requerimientoDetalle");
            InputParameterAdd.Int(objCommand, "idRequerimiento", requerimientoDetalle.idRequerimiento);
            InputParameterAdd.Guid(objCommand, "idProducto", requerimientoDetalle.producto.idProducto);
            InputParameterAdd.Int(objCommand, "cantidad", requerimientoDetalle.cantidad);
            //Siempre se almacena el precio sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "precioSinIGV", requerimientoDetalle.producto.precioSinIgv);
            //Siempre se almacena el costo sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "costoSinIGV", requerimientoDetalle.producto.costoSinIgv);
            if(requerimientoDetalle.esPrecioAlternativo)
                InputParameterAdd.Decimal(objCommand, "equivalencia", requerimientoDetalle.ProductoPresentacion.Equivalencia);            
            else
                InputParameterAdd.Decimal(objCommand, "equivalencia", 1);
            InputParameterAdd.Varchar(objCommand, "unidad", requerimientoDetalle.unidad);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", requerimientoDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "precioNeto", requerimientoDetalle.precioNeto);
            InputParameterAdd.Int(objCommand, "esPrecioAlternativo", requerimientoDetalle.esPrecioAlternativo?1:0);
            InputParameterAdd.Guid(objCommand, "idUsuario", usuario.idUsuario);
            InputParameterAdd.Decimal(objCommand, "flete", requerimientoDetalle.flete);
            InputParameterAdd.Varchar(objCommand, "observaciones", requerimientoDetalle.observacion);
            OutputParameterAdd.BigInt(objCommand, "idRequerimientoDetalle");
            if (requerimientoDetalle.esPrecioAlternativo)
            {
                InputParameterAdd.Int(objCommand, "idProductoPresentacion", requerimientoDetalle.ProductoPresentacion.IdProductoPresentacion);
            }
            else
            {
                InputParameterAdd.Int(objCommand, "idProductoPresentacion", 0);
            }

            InputParameterAdd.Int(objCommand, "indicadorAprobacion", (int)requerimientoDetalle.indicadorAprobacion);

            ExecuteNonQuery(objCommand);
          
            requerimientoDetalle.idRequerimientoDetalle = (Int64)objCommand.Parameters["@idRequerimientoDetalle"].Value;
        }

     
        public List<Requerimiento> SelectRequerimientos(Requerimiento requerimiento)
        {
            var objCommand = GetSqlCommand("CLIENTE.ps_requerimientos");
            InputParameterAdd.Int(objCommand, "idRequerimiento", requerimiento.idRequerimiento);
            InputParameterAdd.Guid(objCommand, "idUsuario", requerimiento.usuario.idUsuario);
            InputParameterAdd.Guid(objCommand, "idUsuarioBusqueda", requerimiento.usuarioBusqueda.idUsuario);

            switch (requerimiento.claseRequerimiento)
            {
                case Requerimiento.ClasesRequerimiento.Venta: InputParameterAdd.Char(objCommand, "tipoRequerimiento", ((Char)requerimiento.tipoRequerimientoVentaBusqueda).ToString()); break;
                case Requerimiento.ClasesRequerimiento.Compra: InputParameterAdd.Char(objCommand, "tipoRequerimiento", ((Char)requerimiento.tipoRequerimientoCompraBusqueda).ToString()); break;
                case Requerimiento.ClasesRequerimiento.Almacen: InputParameterAdd.Char(objCommand, "tipoRequerimiento", ((Char)requerimiento.tipoRequerimientoAlmacenBusqueda).ToString()); break;
            }           

            InputParameterAdd.DateTime(objCommand, "fechaCreacionDesde", new DateTime(requerimiento.fechaCreacionDesde.Year, 1, 1, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaCreacionHasta", new DateTime(requerimiento.fechaCreacionHasta.Year, 12, 30, 23, 59, 59));
            InputParameterAdd.DateTime(objCommand, "fechaEntregaDesde", requerimiento.fechaEntregaDesde == null ? requerimiento.fechaEntregaDesde : new DateTime(requerimiento.fechaEntregaDesde.Value.Year, requerimiento.fechaEntregaDesde.Value.Month, requerimiento.fechaEntregaDesde.Value.Day, 0, 0, 0));
            InputParameterAdd.DateTime(objCommand, "fechaEntregaHasta", requerimiento.fechaEntregaHasta == null ? requerimiento.fechaEntregaDesde : new DateTime(requerimiento.fechaEntregaHasta.Value.Year, requerimiento.fechaEntregaHasta.Value.Month, requerimiento.fechaEntregaHasta.Value.Day, 23, 59, 59));
            InputParameterAdd.DateTime(objCommand, "fechaProgramacionDesde", requerimiento.fechaProgramacionDesde == null ? requerimiento.fechaProgramacionDesde : new DateTime(requerimiento.fechaProgramacionDesde.Value.Year, requerimiento.fechaProgramacionDesde.Value.Month, requerimiento.fechaProgramacionDesde.Value.Day, 0, 0, 0));  //requerimiento.fechaProgramacionDesde);
            InputParameterAdd.DateTime(objCommand, "fechaProgramacionHasta", requerimiento.fechaProgramacionHasta == null ? requerimiento.fechaProgramacionHasta : new DateTime(requerimiento.fechaProgramacionHasta.Value.Year, requerimiento.fechaProgramacionHasta.Value.Month, requerimiento.fechaProgramacionHasta.Value.Day, 0, 0, 0));  //requerimiento.fechaProgramacionDesde); //requerimiento.fechaProgramacionHasta);

            InputParameterAdd.Char(objCommand, "tipo", ((char)requerimiento.claseRequerimiento).ToString());
            InputParameterAdd.Varchar(objCommand, "sku", requerimiento.sku);
            InputParameterAdd.Int(objCommand, "idClienteSunat", requerimiento.usuario.idClienteSunat);
            InputParameterAdd.Guid(objCommand, "idPeriodo", requerimiento.periodo.idPeriodoSolicitud);

            DataTable dataTable = Execute(objCommand);

            List<Requerimiento> requerimientoList = new List<Requerimiento>();

            int idRequerimientoActual = 0;

           /// Dictionary<Guid, string> productosCabecera = new Dictionary<Guid, string>();


            foreach (DataRow row in dataTable.Rows)
            {
                if (Converter.GetInt(row, "id_requerimiento") != idRequerimientoActual)
                {
                    idRequerimientoActual = Converter.GetInt(row, "id_requerimiento");
                    requerimiento = new Requerimiento(requerimiento.claseRequerimiento);
                    requerimiento.idRequerimiento = Converter.GetInt(row, "id_requerimiento");

                    requerimiento.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                    requerimiento.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                    requerimiento.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                    requerimiento.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                    requerimiento.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                    requerimiento.horaEntregaAdicionalDesde = Converter.GetString(row, "hora_entrega_adicional_desde");
                    requerimiento.horaEntregaAdicionalHasta = Converter.GetString(row, "hora_entrega_adicional_hasta");
                    requerimiento.fechaEntregaExtendida = Converter.GetDateTimeNullable(row, "fecha_entrega_extendida");
                    requerimiento.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");

                    requerimiento.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                    requerimiento.FechaRegistro = requerimiento.FechaRegistro.AddHours(-5);
                    requerimiento.stockConfirmado = Converter.GetBool(row, "stock_confirmado");
                    /*if (row["fecha_programacion"] == DBNull.Value)
                        requerimiento.fechaProgramacion = null;
                    else*/
                    requerimiento.fechaProgramacion = Converter.GetDateTimeNullable(row, "fecha_programacion");
                    requerimiento.incluidoIGV = Converter.GetBool(row, "incluido_igv");

                    requerimiento.montoIGV = Converter.GetDecimal(row, "igv");
                    requerimiento.montoTotal = Converter.GetDecimal(row, "total");
                    requerimiento.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, requerimiento.montoTotal - requerimiento.montoIGV));

                    requerimiento.observaciones = Converter.GetString(row, "observaciones");

                    requerimiento.cliente = new Cliente();
                    requerimiento.cliente.codigo = Converter.GetString(row, "codigo");
                    requerimiento.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                    requerimiento.cliente.razonSocial = Converter.GetString(row, "razon_social");
                    requerimiento.cliente.ruc = Converter.GetString(row, "ruc");
                    requerimiento.cliente.bloqueado = Converter.GetBool(row, "bloqueado");

                    requerimiento.cliente.grupoCliente = new GrupoCliente();
                    requerimiento.cliente.grupoCliente.nombre = Converter.GetString(row, "nombre_grupo");

                    requerimiento.usuario = new Usuario();
                    requerimiento.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                    requerimiento.usuario.idUsuario = Converter.GetGuid(row, "id_usuario");

                    //  cotizacion.usuario_aprobador = new Usuario();
                    //  cotizacion.usuario_aprobador.nombre = Converter.GetString(row, "nombre_usuario_aprobador");

                    requerimiento.ciudad = new Ciudad();
                    requerimiento.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                    requerimiento.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

                    requerimiento.ubigeoEntrega = new Ubigeo();
                    requerimiento.ubigeoEntrega.Id = Converter.GetString(row, "codigo_ubigeo");
                    requerimiento.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");
                    requerimiento.requerimientoDetalleList = new List<RequerimientoDetalle>();
                    requerimiento.estadoRequerimiento = (Requerimiento.estadosRequerimiento)Converter.GetInt(row, "estado_requerimiento");
                    requerimientoList.Add(requerimiento);
                }

                RequerimientoDetalle requerimientoDetalle = new RequerimientoDetalle(false,false);

                
                requerimientoDetalle.producto = new Producto();
                requerimientoDetalle.producto.sku = Converter.GetString(row, "sku");
                requerimientoDetalle.unidad = Converter.GetString(row, "unidad");
                requerimientoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                requerimientoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                requerimientoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                requerimiento.requerimientoDetalleList.Add(requerimientoDetalle);

            }
            return requerimientoList;
        }



        public Requerimiento SelectRequerimiento(Requerimiento requerimiento, Usuario usuario)
        {
            var objCommand = GetSqlCommand("CLIENTE.ps_requerimiento");
            InputParameterAdd.Int(objCommand, "idRequerimiento", requerimiento.idRequerimiento);
            InputParameterAdd.Int(objCommand, "idClienteSunat", usuario.idClienteSunat);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable requerimientoDataTable = dataSet.Tables[0];
            DataTable requerimientoDetalleDataTable = dataSet.Tables[1];
            DataTable direccionEntregaDataTable = dataSet.Tables[2];
        

            //   DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            foreach (DataRow row in requerimientoDataTable.Rows)
            {
                requerimiento.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                requerimiento.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                requerimiento.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                requerimiento.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                requerimiento.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                requerimiento.horaEntregaAdicionalDesde = Converter.GetString(row, "hora_entrega_adicional_desde");
                requerimiento.horaEntregaAdicionalHasta = Converter.GetString(row, "hora_entrega_adicional_hasta");

                requerimiento.fechaEntregaExtendida = Converter.GetDateTimeNullable(row, "fecha_entrega_extendida");
                

                requerimiento.incluidoIGV = Converter.GetBool(row, "incluido_igv");
                requerimiento.montoIGV = Converter.GetDecimal(row, "igv");
                requerimiento.montoTotal = Converter.GetDecimal(row, "total");
                requerimiento.observaciones = Converter.GetString(row, "observaciones");
                requerimiento.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, requerimiento.montoTotal - requerimiento.montoIGV));
                requerimiento.fechaModificacion = Converter.GetDateTime(row, "fecha_modificacion");
                requerimiento.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                requerimiento.direccionEntrega = new DireccionEntrega();
                requerimiento.direccionEntrega.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega");
                requerimiento.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");
                requerimiento.direccionEntrega.contacto = Converter.GetString(row, "contacto_entrega");
                requerimiento.direccionEntrega.telefono = Converter.GetString(row, "telefono_contacto_entrega");
                requerimiento.direccionEntrega.codigoCliente = Converter.GetString(row, "direccion_entrega_codigo_cliente");
                requerimiento.direccionEntrega.codigoMP = Converter.GetString(row, "direccion_entrega_codigo_mp");
                requerimiento.direccionEntrega.nombre = Converter.GetString(row, "direccion_entrega_nombre");
                requerimiento.direccionEntrega.direccionEntregaAlmacen = new DireccionEntrega();
                requerimiento.direccionEntrega.direccionEntregaAlmacen.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega_almacen");
                requerimiento.direccionEntrega.direccionEntregaAlmacen.descripcion = Converter.GetString(row, "direccion_entrega_almacen_descripcion");
                requerimiento.direccionEntrega.direccionEntregaAlmacen.contacto = Converter.GetString(row, "direccion_entrega_almacen_contacto_entrega");
                requerimiento.direccionEntrega.direccionEntregaAlmacen.telefono = Converter.GetString(row, "direccion_entrega_almacen_telefono_contacto_entrega");
                requerimiento.direccionEntrega.direccionEntregaAlmacen.codigoCliente = Converter.GetString(row, "direccion_entrega_almacen_codigo_cliente");
                requerimiento.direccionEntrega.direccionEntregaAlmacen.codigoMP = Converter.GetString(row, "direccion_entrega_almacen_codigo_mp");
                requerimiento.direccionEntrega.direccionEntregaAlmacen.nombre = Converter.GetString(row, "direccion_entrega_almacen_nombre");

                requerimiento.direccionEntrega.direccionEntregaAlmacen.ubigeo = new Ubigeo();
                requerimiento.direccionEntrega.direccionEntregaAlmacen.ubigeo.Id = Converter.GetString(row, "codigo_ubigeo_entrega_almacen");
                requerimiento.direccionEntrega.direccionEntregaAlmacen.ubigeo.Departamento = Converter.GetString(row, "direccion_entrega_almacen_departamento");
                requerimiento.direccionEntrega.direccionEntregaAlmacen.ubigeo.Provincia = Converter.GetString(row, "direccion_entrega_almacen_provincia");
                requerimiento.direccionEntrega.direccionEntregaAlmacen.ubigeo.Distrito = Converter.GetString(row, "direccion_entrega_almacen_distrito");


                requerimiento.solicitante = new Solicitante();
                requerimiento.solicitante.idSolicitante = Converter.GetGuid(row, "id_solicitante");

                requerimiento.esPagoContado = Converter.GetBool(row, "es_pago_contado"); 

                requerimiento.contactoRequerimiento = Converter.GetString(row, "contacto_requerimiento");
                requerimiento.telefonoContactoRequerimiento = Converter.GetString(row, "telefono_contacto_requerimiento");
                requerimiento.correoContactoRequerimiento = Converter.GetString(row, "correo_contacto_requerimiento");
                requerimiento.solicitante.nombre = requerimiento.contactoRequerimiento;
                requerimiento.solicitante.telefono = requerimiento.telefonoContactoRequerimiento;
                requerimiento.solicitante.correo = requerimiento.correoContactoRequerimiento;

                requerimiento.fechaProgramacion = Converter.GetDateTime(row, "fecha_programacion");
                requerimiento.observacionesFactura = Converter.GetString(row, "observaciones_factura");
                requerimiento.observacionesGuiaRemision = Converter.GetString(row, "observaciones_guia_remision");

                requerimiento.claseRequerimiento = (Requerimiento.ClasesRequerimiento)Char.Parse(Converter.GetString(row, "tipo"));
                if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Venta)
                    requerimiento.tipoRequerimiento = (Requerimiento.tiposRequerimiento)Char.Parse(Converter.GetString(row, "tipo_requerimiento"));
                else if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Compra)
                    requerimiento.tipoRequerimientoCompra = (Requerimiento.tiposRequerimientoCompra)Char.Parse(Converter.GetString(row, "tipo_requerimiento"));
                else
                    requerimiento.tipoRequerimientoAlmacen = (Requerimiento.tiposRequerimientoAlmacen)Char.Parse(Converter.GetString(row, "tipo_requerimiento"));

                requerimiento.otrosCargos = Converter.GetDecimal(row, "otros_cargos");
                requerimiento.numeroReferenciaAdicional = Converter.GetString(row, "numero_referencia_adicional");

                requerimiento.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                requerimiento.FechaRegistro = requerimiento.FechaRegistro.AddHours(-5);

                /* requerimiento.venta = new Venta();
                   requerimiento.venta.igv = Converter.GetDecimal(row, "igv_venta");
                   requerimiento.venta.subTotal = Converter.GetDecimal(row, "sub_total_venta");
                   requerimiento.venta.total = Converter.GetDecimal(row, "total_venta");
                   requerimiento.venta.idVenta = Converter.GetGuid(row, "id_Venta");*/

                requerimiento.ubigeoEntrega = new Ubigeo();
                requerimiento.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                requerimiento.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                requerimiento.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                requerimiento.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                requerimiento.documentoVenta = new DocumentoVenta();
                requerimiento.documentoVenta.serie = Converter.GetString(row, "serie_factura");
                requerimiento.documentoVenta.numero = Converter.GetString(row, "numero_factura");

                requerimiento.cotizacion = new Cotizacion();
                requerimiento.cotizacion.idCotizacion = Converter.GetGuid(row, "id_cotizacion");
                requerimiento.cotizacion.codigo = Converter.GetLong(row, "cotizacion_codigo");
                requerimiento.cotizacion.tipoCotizacion = (Cotizacion.TiposCotizacion)Converter.GetInt(row, "tipo_cotizacion");

                requerimiento.cliente = new Cliente();
                requerimiento.cliente.codigo = Converter.GetString(row, "codigo");
                requerimiento.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                requerimiento.cliente.razonSocial = Converter.GetString(row, "razon_social");
                requerimiento.cliente.ruc = Converter.GetString(row, "ruc");
                requerimiento.cliente.ciudad = new Ciudad();
                requerimiento.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad_cliente");
                requerimiento.cliente.ciudad.nombre = Converter.GetString(row, "nombre_ciudad_cliente");
                requerimiento.cliente.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                requerimiento.cliente.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                requerimiento.cliente.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                requerimiento.cliente.plazoCredito = Converter.GetString(row, "plazo_credito");
                requerimiento.cliente.plazoCreditoSolicitado = (DocumentoVenta.TipoPago)Converter.GetInt(row, "plazo_credito_solicitado");
                requerimiento.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                requerimiento.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");

                /*Vendedores*/
                requerimiento.cliente.responsableComercial = new Vendedor();
                requerimiento.cliente.responsableComercial.codigo = Converter.GetString(row, "responsable_comercial_codigo");
                requerimiento.cliente.responsableComercial.descripcion = Converter.GetString(row, "responsable_comercial_descripcion");
                requerimiento.cliente.responsableComercial.usuario = new Usuario();
                requerimiento.cliente.responsableComercial.usuario.email = Converter.GetString(row, "responsable_comercial_email");

                requerimiento.cliente.supervisorComercial = new Vendedor();
                requerimiento.cliente.supervisorComercial.codigo = Converter.GetString(row, "supervisor_comercial_codigo");
                requerimiento.cliente.supervisorComercial.descripcion = Converter.GetString(row, "supervisor_comercial_descripcion");
                requerimiento.cliente.supervisorComercial.usuario = new Usuario();
                requerimiento.cliente.supervisorComercial.usuario.email = Converter.GetString(row, "supervisor_comercial_email");

                requerimiento.cliente.asistenteServicioCliente = new Vendedor();
                requerimiento.cliente.asistenteServicioCliente.codigo = Converter.GetString(row, "asistente_servicio_cliente_codigo");
                requerimiento.cliente.asistenteServicioCliente.descripcion = Converter.GetString(row, "asistente_servicio_cliente_descripcion");
                requerimiento.cliente.asistenteServicioCliente.usuario = new Usuario();
                requerimiento.cliente.asistenteServicioCliente.usuario.email = Converter.GetString(row, "asistente_servicio_cliente_email");

                requerimiento.cliente.grupoCliente = new GrupoCliente();
                requerimiento.cliente.grupoCliente.nombre = Converter.GetString(row, "grupo_nombre");

                requerimiento.ciudad = new Ciudad();
                requerimiento.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                requerimiento.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");
                requerimiento.ciudad.direccionPuntoLlegada = Converter.GetString(row, "direccion_establecimiento");
                requerimiento.ciudad.esProvincia = Converter.GetBool(row, "es_provincia");

                requerimiento.usuario = new Usuario();
                requerimiento.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                requerimiento.usuario.cargo = Converter.GetString(row, "cargo");
                requerimiento.usuario.contacto = Converter.GetString(row, "contacto_usuario");
                requerimiento.usuario.email = Converter.GetString(row, "email");


                requerimiento.usuario.nombre = Converter.GetString(row, "nombre_usuario");


            }


            requerimiento.requerimientoDetalleList = new List<RequerimientoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in requerimientoDetalleDataTable.Rows)
            {
                RequerimientoDetalle requerimientoDetalle = new RequerimientoDetalle(usuario.visualizaCostos, usuario.visualizaMargen);

                requerimientoDetalle.producto = new Producto();

                requerimientoDetalle.idRequerimientoDetalle = Converter.GetInt(row, "id_requerimiento_detalle");
                requerimientoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                requerimientoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                requerimientoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");

            
                requerimientoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");

                requerimientoDetalle.flete = Converter.GetDecimal(row, "flete");


                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                requerimientoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                requerimientoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (requerimientoDetalle.esPrecioAlternativo)
                {
                   /* requerimientoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    requerimientoDetalle.ProductoPresentacion.Equivalencia = requerimientoDetalle.producto.equivalencia;
                    */
                    requerimientoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    requerimientoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                    requerimientoDetalle.ProductoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                    requerimientoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * requerimientoDetalle.ProductoPresentacion.Equivalencia;
                }
                else
                {
                    requerimientoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                }

                requerimientoDetalle.unidad = Converter.GetString(row, "unidad");

                requerimientoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                requerimientoDetalle.producto.sku = Converter.GetString(row, "sku");
                requerimientoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                requerimientoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                requerimientoDetalle.producto.proveedor = Converter.GetString(row, "proveedor");
                requerimientoDetalle.producto.tipoProducto = (Producto.TipoProducto)Converter.GetInt(row, "tipo_producto");

                requerimientoDetalle.producto.image = Converter.GetBytes(row, "imagen");

                requerimientoDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                requerimientoDetalle.observacion = Converter.GetString(row, "observaciones");


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
                requerimientoDetalle.producto.precioClienteProducto = precioClienteProducto;


                requerimientoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                requerimientoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                requerimientoDetalle.indicadorAprobacion = (RequerimientoDetalle.IndicadorAprobacion)Converter.GetShort(row, "indicador_aprobacion");

                requerimiento.requerimientoDetalleList.Add(requerimientoDetalle);
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

            requerimiento.cliente.direccionEntregaList = direccionEntregaList;

            
            
         

            return requerimiento;
        }

        public Requerimiento SelectRequerimientoParaEditar(Requerimiento requerimiento, Usuario usuario)
        {
            var objCommand = GetSqlCommand("CLIENTE.ps_requerimientoParaEditar");
            InputParameterAdd.Int(objCommand, "idRequerimiento", requerimiento.idRequerimiento);
            InputParameterAdd.Int(objCommand, "idClienteSunat", usuario.idClienteSunat);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable requerimientoDataTable = dataSet.Tables[0];
            DataTable requerimientoDetalleDataTable = dataSet.Tables[1];
            DataTable direccionEntregaDataTable = dataSet.Tables[2];

            //   DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            foreach (DataRow row in requerimientoDataTable.Rows)
            {
                requerimiento.fechaSolicitud = Converter.GetDateTime(row, "fecha_solicitud");
                requerimiento.fechaEntregaDesde = Converter.GetDateTime(row, "fecha_entrega_desde");
                requerimiento.fechaEntregaHasta = Converter.GetDateTime(row, "fecha_entrega_hasta");
                requerimiento.horaEntregaDesde = Converter.GetString(row, "hora_entrega_desde");
                requerimiento.horaEntregaHasta = Converter.GetString(row, "hora_entrega_hasta");
                requerimiento.horaEntregaAdicionalDesde = Converter.GetString(row, "hora_entrega_adicional_desde");
                requerimiento.horaEntregaAdicionalHasta = Converter.GetString(row, "hora_entrega_adicional_hasta");
                requerimiento.incluidoIGV = Converter.GetBool(row, "incluido_igv");
                requerimiento.montoIGV = Converter.GetDecimal(row, "igv");
                requerimiento.montoTotal = Converter.GetDecimal(row, "total");
                requerimiento.observaciones = Converter.GetString(row, "observaciones");
                requerimiento.montoSubTotal = Decimal.Parse(String.Format(Constantes.formatoDosDecimales, requerimiento.montoTotal - requerimiento.montoIGV));
                requerimiento.fechaModificacion = Converter.GetDateTime(row, "fecha_modificacion");
                requerimiento.numeroReferenciaCliente = Converter.GetString(row, "numero_referencia_cliente");
                requerimiento.direccionEntrega = new DireccionEntrega();
                requerimiento.direccionEntrega.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega");
                requerimiento.direccionEntrega.descripcion = Converter.GetString(row, "direccion_entrega");
                requerimiento.direccionEntrega.contacto = Converter.GetString(row, "contacto_entrega");
                requerimiento.direccionEntrega.telefono = Converter.GetString(row, "telefono_contacto_entrega");
                requerimiento.direccionEntrega.codigoCliente = Converter.GetString(row, "direccion_entrega_codigo_cliente");
                requerimiento.direccionEntrega.codigoMP = Converter.GetString(row, "direccion_entrega_codigo_mp");
                requerimiento.direccionEntrega.nombre = Converter.GetString(row, "direccion_entrega_nombre");
                requerimiento.direccionEntrega.direccionEntregaAlmacen = new DireccionEntrega();
                requerimiento.direccionEntrega.direccionEntregaAlmacen.idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega_almacen");             
	            requerimiento.direccionEntrega.limitePresupuesto = Converter.GetDecimal(row, "limite_presupuesto"); 



                requerimiento.solicitante = new Solicitante();
                requerimiento.solicitante.idSolicitante = Converter.GetGuid(row, "id_solicitante");

                requerimiento.contactoRequerimiento = Converter.GetString(row, "contacto_requerimiento");
                requerimiento.telefonoContactoRequerimiento = Converter.GetString(row, "telefono_contacto_requerimiento");
                requerimiento.correoContactoRequerimiento = Converter.GetString(row, "correo_contacto_requerimiento");
                requerimiento.esPagoContado = Converter.GetBool(row, "es_pago_contado");

                requerimiento.solicitante.nombre = requerimiento.contactoRequerimiento;
                requerimiento.solicitante.telefono = requerimiento.telefonoContactoRequerimiento;
                requerimiento.solicitante.correo = requerimiento.correoContactoRequerimiento;

                requerimiento.fechaProgramacion = Converter.GetDateTime(row, "fecha_programacion");
                requerimiento.observacionesFactura = Converter.GetString(row, "observaciones_factura");
                requerimiento.observacionesGuiaRemision = Converter.GetString(row, "observaciones_guia_remision");

                requerimiento.claseRequerimiento = (Requerimiento.ClasesRequerimiento)Char.Parse(Converter.GetString(row, "tipo"));


                if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Venta)
                    requerimiento.tipoRequerimiento = (Requerimiento.tiposRequerimiento)Char.Parse(Converter.GetString(row, "tipo_requerimiento"));
                else if (requerimiento.claseRequerimiento == Requerimiento.ClasesRequerimiento.Compra)
                    requerimiento.tipoRequerimientoCompra = (Requerimiento.tiposRequerimientoCompra)Char.Parse(Converter.GetString(row, "tipo_requerimiento"));
                else
                    requerimiento.tipoRequerimientoAlmacen = (Requerimiento.tiposRequerimientoAlmacen)Char.Parse(Converter.GetString(row, "tipo_requerimiento"));
             


                requerimiento.otrosCargos = Converter.GetDecimal(row, "otros_cargos");
                requerimiento.numeroReferenciaAdicional = Converter.GetString(row, "numero_referencia_adicional");

                requerimiento.FechaRegistro = Converter.GetDateTime(row, "fecha_registro");
                requerimiento.FechaRegistro = requerimiento.FechaRegistro.AddHours(-5);

                /* requerimiento.venta = new Venta();
                   requerimiento.venta.igv = Converter.GetDecimal(row, "igv_venta");
                   requerimiento.venta.subTotal = Converter.GetDecimal(row, "sub_total_venta");
                   requerimiento.venta.total = Converter.GetDecimal(row, "total_venta");
                   requerimiento.venta.idVenta = Converter.GetGuid(row, "id_Venta");*/

                requerimiento.ubigeoEntrega = new Ubigeo();
                requerimiento.ubigeoEntrega.Id = Converter.GetString(row, "ubigeo_entrega");
                requerimiento.ubigeoEntrega.Departamento = Converter.GetString(row, "departamento");
                requerimiento.ubigeoEntrega.Provincia = Converter.GetString(row, "provincia");
                requerimiento.ubigeoEntrega.Distrito = Converter.GetString(row, "distrito");

                requerimiento.documentoVenta = new DocumentoVenta();
                requerimiento.documentoVenta.serie = Converter.GetString(row, "serie_factura");
                requerimiento.documentoVenta.numero = Converter.GetString(row, "numero_factura");

                requerimiento.cotizacion = new Cotizacion();
                requerimiento.cotizacion.idCotizacion = Converter.GetGuid(row, "id_cotizacion");
                requerimiento.cotizacion.codigo = Converter.GetLong(row, "cotizacion_codigo");

                requerimiento.cliente = new Cliente();
                requerimiento.cliente.codigo = Converter.GetString(row, "codigo");
                requerimiento.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                requerimiento.cliente.razonSocial = Converter.GetString(row, "razon_social");
                requerimiento.cliente.ruc = Converter.GetString(row, "ruc");
                requerimiento.cliente.ciudad = new Ciudad();
                requerimiento.cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad_cliente");
                requerimiento.cliente.ciudad.nombre = Converter.GetString(row, "nombre_ciudad_cliente");
                requerimiento.cliente.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                requerimiento.cliente.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                requerimiento.cliente.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                requerimiento.cliente.plazoCredito = Converter.GetString(row, "plazo_credito");
                requerimiento.cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                requerimiento.cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                requerimiento.cliente.habilitadoModificarDireccionEntrega = Converter.GetBool(row, "habilitado_modificar_direccion_entrega");
                requerimiento.cliente.exoneradoValidacionLiberacionCrediticia = Converter.GetBool(row, "exonerado_validacion_liberacion_crediticia");

                requerimiento.ciudad = new Ciudad();
                requerimiento.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                requerimiento.ciudad.nombre = Converter.GetString(row, "nombre_ciudad");

             /*   requerimiento.usuario = new Usuario();
                requerimiento.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                requerimiento.usuario.cargo = Converter.GetString(row, "cargo");
                requerimiento.usuario.contacto = Converter.GetString(row, "contacto_usuario");
                requerimiento.usuario.email = Converter.GetString(row, "email");*/

            }


            requerimiento.requerimientoDetalleList = new List<RequerimientoDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in requerimientoDetalleDataTable.Rows)
            {
                RequerimientoDetalle requerimientoDetalle = new RequerimientoDetalle(usuario.visualizaCostos, usuario.visualizaMargen);

                requerimientoDetalle.producto = new Producto();

                requerimientoDetalle.idRequerimientoDetalle = Converter.GetInt(row, "id_requerimiento_detalle");
                requerimientoDetalle.cantidad = Converter.GetInt(row, "cantidad");
                requerimientoDetalle.cantidadPendienteAtencion = Converter.GetInt(row, "cantidadPendienteAtencion");
                requerimientoDetalle.cantidadPorAtender = Converter.GetInt(row, "cantidadPendienteAtencion");
                //requerimientoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                requerimientoDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                requerimientoDetalle.flete = Converter.GetDecimal(row, "flete");


                //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                requerimientoDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                requerimientoDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");
                requerimientoDetalle.producto.tipoProducto = (Producto.TipoProducto) Converter.GetInt(row, "tipo_producto");

                //Si la unidad es alternativa se múltiplica por la equivalencia, dado que la capa de negocio se encarga de hacer los calculos y espera siempre el precio estándar

                if (requerimientoDetalle.esPrecioAlternativo)
                {
                    requerimientoDetalle.ProductoPresentacion = new ProductoPresentacion();
                    requerimientoDetalle.ProductoPresentacion.Equivalencia = Converter.GetDecimal(row, "equivalencia");
                    requerimientoDetalle.ProductoPresentacion.IdProductoPresentacion = Converter.GetInt(row, "id_producto_presentacion");
                    requerimientoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * requerimientoDetalle.ProductoPresentacion.Equivalencia;
                }
                else
                {
                    requerimientoDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                }

                requerimientoDetalle.unidad = Converter.GetString(row, "unidad");

                requerimientoDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                requerimientoDetalle.producto.sku = Converter.GetString(row, "sku");
                requerimientoDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                requerimientoDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                requerimientoDetalle.producto.proveedor = Converter.GetString(row, "proveedor");

                requerimientoDetalle.producto.image = Converter.GetBytes(row, "imagen");

                requerimientoDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                requerimientoDetalle.observacion = Converter.GetString(row, "observaciones");


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
                requerimientoDetalle.producto.precioClienteProducto = precioClienteProducto;


                requerimientoDetalle.precioUnitarioVenta = Converter.GetDecimal(row, "precio_unitario_venta");
                requerimientoDetalle.idVentaDetalle = Converter.GetGuid(row, "id_venta_detalle");

                requerimiento.requerimientoDetalleList.Add(requerimientoDetalle);
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

            requerimiento.cliente.direccionEntregaList = direccionEntregaList;

            
     

            /*mad.id_movimiento_almacen_detalle, mad.cantidad, 
mad.unidad, pr.id_producto, pr.sku, pr.descripcion*/

        

            return requerimiento;
        }

      

        #endregion        
         
        
    }
}
