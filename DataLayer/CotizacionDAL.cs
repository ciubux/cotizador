using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class CotizacionDAL : DaoBase
    {
        public CotizacionDAL(IDalSettings settings) : base(settings)
        {
        }

        public CotizacionDAL() : this(new CotizadorSettings())
        {
        }

        public void InsertCotizacion(Cotizacion cotizacion)
        {
            var objCommand = GetSqlCommand("pi_cotizacion");

           // InputParameterAdd.Varchar(objCommand, "codigo", "0000000001");
            InputParameterAdd.DateTime(objCommand, "fecha", cotizacion.fecha);
            InputParameterAdd.DateTime(objCommand, "fechaVigenciaInicio", cotizacion.fechaVigenciaInicio);
            InputParameterAdd.DateTime(objCommand, "fechaVigenciaLimite", cotizacion.fechaVigenciaLimite);
            InputParameterAdd.SmallInt(objCommand, "incluidoIgv", short.Parse((cotizacion.incluidoIgv?1:0).ToString()));
            InputParameterAdd.SmallInt(objCommand, "consideraCantidades", short.Parse((cotizacion.considerarCantidades ? 1 : 0).ToString()));

            //Si no se cuenta con idCliente entonces se registra el grupo
            if (cotizacion.cliente.idCliente == Guid.Empty)
            {
                InputParameterAdd.Guid(objCommand, "idCliente", null);
                InputParameterAdd.Guid(objCommand, "idGrupo", cotizacion.grupo.idGrupo);
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idCliente", cotizacion.cliente.idCliente);
                InputParameterAdd.Guid(objCommand, "idGrupo", null);
            }

            InputParameterAdd.Guid(objCommand, "idCiudad", cotizacion.ciudad.idCiudad);
            //porcentajeFlete
            InputParameterAdd.Decimal(objCommand, "porcentajeFlete", cotizacion.flete);
            InputParameterAdd.Decimal(objCommand, "igv", cotizacion.igv);
            InputParameterAdd.Decimal(objCommand, "total", cotizacion.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", cotizacion.observaciones);
            InputParameterAdd.Guid(objCommand, "idUsuario", cotizacion.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "contacto", cotizacion.contacto);
            InputParameterAdd.SmallInt(objCommand, "estadoAprobacion", cotizacion.estadoAprobacion);
            InputParameterAdd.SmallInt(objCommand, "mostrarCodigoProveedor", short.Parse((cotizacion.mostrarCodigoProveedor ? 1 : 0).ToString()));
            InputParameterAdd.Int(objCommand, "tipoVigencia", cotizacion.tipoVigencia);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            OutputParameterAdd.BigInt(objCommand, "codigo");
            ExecuteNonQuery(objCommand);

            cotizacion.idCotizacion = (Guid)objCommand.Parameters["@newId"].Value;
            cotizacion.codigo = (Int64)objCommand.Parameters["@codigo"].Value;


            foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
            {
                cotizacionDetalle.idCotizacion = cotizacion.idCotizacion;
                this.InsertCotizacionDetalle(cotizacionDetalle);
            }


        }

        public void UpdateCotizacion(Cotizacion cotizacion)
        {
            var objCommand = GetSqlCommand("pu_cotizacion");

            // InputParameterAdd.Varchar(objCommand, "codigo", "0000000001");
            InputParameterAdd.DateTime(objCommand, "fecha", cotizacion.fecha);
            InputParameterAdd.DateTime(objCommand, "fechaVigenciaInicio", cotizacion.fechaVigenciaInicio);
            InputParameterAdd.DateTime(objCommand, "fechaVigenciaLimite", cotizacion.fechaVigenciaLimite);
            InputParameterAdd.SmallInt(objCommand, "incluidoIgv", short.Parse((cotizacion.incluidoIgv ? 1 : 0).ToString()));
            InputParameterAdd.SmallInt(objCommand, "consideraCantidades", short.Parse((cotizacion.considerarCantidades ? 1 : 0).ToString()));

            //Si no se cuenta con idCliente entonces se registra el grupo
            if (cotizacion.cliente.idCliente == Guid.Empty)
            {
                InputParameterAdd.Guid(objCommand, "idCliente", null);
                InputParameterAdd.Guid(objCommand, "idGrupo", cotizacion.grupo.idGrupo);
            }
            else
            {
                InputParameterAdd.Guid(objCommand, "idCliente", cotizacion.cliente.idCliente);
                InputParameterAdd.Guid(objCommand, "idGrupo", null);
            }


            InputParameterAdd.Guid(objCommand, "idCiudad", cotizacion.ciudad.idCiudad);
            //porcentajeFlete
            InputParameterAdd.Decimal(objCommand, "porcentajeFlete", cotizacion.flete);
            InputParameterAdd.Decimal(objCommand, "igv", cotizacion.igv);
            InputParameterAdd.Decimal(objCommand, "total", cotizacion.montoTotal);
            InputParameterAdd.Varchar(objCommand, "observaciones", cotizacion.observaciones);
            InputParameterAdd.Guid(objCommand, "idUsuario", cotizacion.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "contacto", cotizacion.contacto);
            InputParameterAdd.BigInt(objCommand, "codigo", cotizacion.codigo);
            InputParameterAdd.SmallInt(objCommand, "estadoAprobacion", cotizacion.estadoAprobacion);
            InputParameterAdd.SmallInt(objCommand, "mostrarCodigoProveedor", short.Parse((cotizacion.mostrarCodigoProveedor ? 1 : 0).ToString()));
            InputParameterAdd.Int(objCommand, "tipoVigencia", cotizacion.tipoVigencia);

            ExecuteNonQuery(objCommand);

            foreach (CotizacionDetalle cotizacionDetalle in cotizacion.cotizacionDetalleList)
            {
                cotizacionDetalle.idCotizacion = cotizacion.idCotizacion;
                this.InsertCotizacionDetalle(cotizacionDetalle);
            }


        }


        public void InsertCotizacionDetalle(CotizacionDetalle cotizacionDetalle)
        {
            var objCommand = GetSqlCommand("pi_cotizacionDetalle");
            InputParameterAdd.Guid(objCommand, "idCotizacion", cotizacionDetalle.idCotizacion);
            InputParameterAdd.Guid(objCommand, "idProducto", cotizacionDetalle.producto.idProducto);
            InputParameterAdd.Int(objCommand, "cantidad", cotizacionDetalle.cantidad);
            //Siempre se almacena el precio sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "precioSinIGV", cotizacionDetalle.producto.precioSinIgv);
            //Siempre se almacena el costo sin igv de la unidad estandar
            InputParameterAdd.Decimal(objCommand, "costoSinIGV", cotizacionDetalle.producto.costoSinIgv);
            InputParameterAdd.Decimal(objCommand, "equivalencia", cotizacionDetalle.producto.equivalencia);
            InputParameterAdd.Varchar(objCommand, "unidad", cotizacionDetalle.unidad);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", cotizacionDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "precioNeto", cotizacionDetalle.precioNeto);
            InputParameterAdd.Int(objCommand, "esPrecioAlternativo", cotizacionDetalle.esPrecioAlternativo?1:0);
            InputParameterAdd.Guid(objCommand, "idUsuario", cotizacionDetalle.usuario.idUsuario);
            InputParameterAdd.Decimal(objCommand, "flete", cotizacionDetalle.flete);
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);

            cotizacionDetalle.idCotizacionDetalle = (Guid)objCommand.Parameters["@newId"].Value;
        }

        public Cotizacion aprobarCotizacion(Cotizacion cotizacion)
        {
            var objCommand = GetSqlCommand("pu_aprobarCotizacion");

            InputParameterAdd.BigInt(objCommand, "codigo", cotizacion.codigo);
            InputParameterAdd.Guid(objCommand, "idUsuario", cotizacion.usuario.idUsuario);
            InputParameterAdd.SmallInt(objCommand, "estadoAprobacion", cotizacion.estadoAprobacion);
            InputParameterAdd.Varchar(objCommand, "motivoRechazo", cotizacion.motivoRechazo);

            ExecuteNonQuery(objCommand);
            return cotizacion;

        }

        public Cotizacion SelectCotizacion(Cotizacion cotizacion)
        {
            var objCommand = GetSqlCommand("ps_cotizacion");
            InputParameterAdd.BigInt(objCommand, "codigo", cotizacion.codigo);
            DataSet dataSet = ExecuteDataSet(objCommand);
            DataTable cotizacionDataTable = dataSet.Tables[0];
            DataTable cotizacionDetalleDataTable = dataSet.Tables[1];

      
         //   DataTable dataTable = Execute(objCommand);
            //Datos de la cotizacion
            foreach (DataRow row in cotizacionDataTable.Rows)
            {
                cotizacion.idCotizacion = Converter.GetGuid(row,"id_cotizacion");
                cotizacion.fecha = Converter.GetDateTime(row, "fecha");
                cotizacion.fechaVigenciaInicio = Converter.GetDateTime(row, "fecha_vigencia_inicio");
                cotizacion.fechaVigenciaLimite = Converter.GetDateTime(row, "fecha_vigencia_limite");
                cotizacion.incluidoIgv = Converter.GetBool(row, "incluido_igv");
                cotizacion.considerarCantidades = Converter.GetBool(row, "considera_cantidades");
                cotizacion.flete = Converter.GetDecimal(row, "porcentaje_flete");
                cotizacion.igv = Converter.GetDecimal(row, "igv");
                cotizacion.montoTotal = Converter.GetDecimal(row, "total");
                cotizacion.estadoAprobacion = Converter.GetShort(row, "estado_aprobacion");
                cotizacion.tipoVigencia = Converter.GetInt(row, "tipo_vigencia");
                ///Mover "{0:0.00}" a clase de constantes
                cotizacion.montoSubTotal = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacion.montoTotal / (1+cotizacion.igv)));
                cotizacion.montoIGV = cotizacion.montoTotal - cotizacion.montoSubTotal;

                cotizacion.observaciones = Converter.GetString(row, "observaciones");

                //Si el cliente es Null
                if (row["id_cliente"] == DBNull.Value)
                {
                    cotizacion.cliente = new Cliente();

                    cotizacion.grupo = new Grupo();
                    cotizacion.grupo.codigo = Converter.GetString(row, "codigo_grupo");
                    cotizacion.grupo.idGrupo = Converter.GetGuid(row, "id_grupo");
                    cotizacion.grupo.nombre = Converter.GetString(row, "nombre_grupo");
                }
                else
                {
                    cotizacion.cliente = new Cliente();
                    cotizacion.cliente.codigo = Converter.GetString(row, "codigo");
                    cotizacion.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                    cotizacion.cliente.razonSocial = Converter.GetString(row, "razon_social");
                    cotizacion.cliente.ruc = Converter.GetString(row, "ruc");

                    cotizacion.grupo = new Grupo();
                }




                cotizacion.contacto = Converter.GetString(row, "contacto");           

                cotizacion.ciudad = new Ciudad();
                cotizacion.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                cotizacion.ciudad.nombre = Converter.GetString(row, "nombre");

                cotizacion.usuario = new Usuario();
                cotizacion.usuario.nombre = Converter.GetString(row, "nombre_usuario");
                cotizacion.usuario.cargo = Converter.GetString(row, "cargo");
                cotizacion.usuario.contacto = Converter.GetString(row, "contacto_usuario");
                cotizacion.usuario.email = Converter.GetString(row, "email");
                cotizacion.mostrarCodigoProveedor = Converter.GetBool(row, "mostrar_codigo_proveedor");



            }


            cotizacion.cotizacionDetalleList = new List<CotizacionDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in cotizacionDetalleDataTable.Rows)
            {
                CotizacionDetalle cotizacionDetalle = new CotizacionDetalle();
                cotizacionDetalle.producto = new Producto();

                cotizacionDetalle.idCotizacionDetalle = Converter.GetGuid(row, "id_cotizacion_detalle");
                cotizacionDetalle.cantidad = Converter.GetInt(row, "cantidad");
                cotizacionDetalle.producto.equivalencia = Convert.ToInt32(Converter.GetDecimal(row, "equivalencia"));
                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
                cotizacionDetalle.flete = Converter.GetDecimal(row, "flete");



                if (cotizacion.esRecotizacion)
                {
                    //Si es recotizacion entonces el precio y el costo son los tomados del producto
                    cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_producto");
                    cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_producto");
                    //El precio se pudo haber almacenado con IGV o sin IGV
                    
                    cotizacionDetalle.precioNetoAnterior = Converter.GetDecimal(row, "precio_neto");

                    //El costo anterior no contiene IGV
                    if (cotizacionDetalle.esPrecioAlternativo)
                    {
                        cotizacionDetalle.costoAnterior = Converter.GetDecimal(row, "costo_sin_igv") * cotizacionDetalle.producto.equivalencia;
                    }
                    else
                    {
                        cotizacionDetalle.costoAnterior = Converter.GetDecimal(row, "costo_sin_igv");
                    }
                    
                }
                else
                {
                    //Si NO es recotizacion se consideran los precios y el costo de lo guardado
                    cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_sin_igv");
                    cotizacionDetalle.producto.costoSinIgv = Converter.GetDecimal(row, "costo_sin_igv");
                    
                    //Este precio ya considera flete e igv
                    if (cotizacionDetalle.esPrecioAlternativo)
                    {
                        cotizacionDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto") * cotizacionDetalle.producto.equivalencia;
                    }
                    else
                    {
                        cotizacionDetalle.precioNeto = Converter.GetDecimal(row, "precio_neto");
                    }
                    
                }
                
                

                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");

                cotizacionDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                cotizacionDetalle.producto.sku = Converter.GetString(row, "sku");
                cotizacionDetalle.producto.skuProveedor = Converter.GetString(row, "sku_proveedor");
                cotizacionDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                cotizacionDetalle.producto.proveedor = Converter.GetString(row, "proveedor");


                cotizacionDetalle.producto.image = Converter.GetBytes(row, "imagen");

               
                cotizacionDetalle.porcentajeDescuento = Converter.GetDecimal(row, "porcentaje_descuento");

                cotizacion.cotizacionDetalleList.Add(cotizacionDetalle);
            }
            return cotizacion;
        }

        public List<Cotizacion> SelectCotizaciones(Cotizacion cotizacion)
        {
            var objCommand = GetSqlCommand("ps_cotizaciones");
            InputParameterAdd.BigInt(objCommand, "codigo", cotizacion.codigo);
            InputParameterAdd.Guid(objCommand, "id_cliente", cotizacion.cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "id_ciudad", cotizacion.ciudad.idCiudad);
            InputParameterAdd.DateTime(objCommand, "fecha", cotizacion.fecha);
            InputParameterAdd.DateTime(objCommand, "fechaHasta", cotizacion.fechaHasta);
            InputParameterAdd.SmallInt(objCommand, "estadoAprobacion", cotizacion.estadoAprobacion);
            DataTable dataTable = Execute(objCommand);

            List<Cotizacion> cotizacionList = new List<Cotizacion>();

            foreach (DataRow row in dataTable.Rows)
            {
                cotizacion = new Cotizacion();
                cotizacion.codigo = Converter.GetLong(row, "cod_cotizacion");
                cotizacion.idCotizacion = Converter.GetGuid(row, "id_cotizacion");
                cotizacion.fecha = Converter.GetDateTime(row, "fecha");
                cotizacion.incluidoIgv = Converter.GetBool(row, "incluido_igv");
                cotizacion.considerarCantidades = Converter.GetBool(row, "considera_cantidades");
                cotizacion.flete = Converter.GetDecimal(row, "porcentaje_flete");
                cotizacion.igv = Converter.GetDecimal(row, "igv");
                cotizacion.montoTotal = Converter.GetDecimal(row, "total");
                cotizacion.motivoRechazo = Converter.GetString(row, "motivo_rechazo");
                cotizacion.maximoPorcentajeDescuento = Converter.GetDecimal(row, "maximo_porcentaje_descuento");

                ///Mover "{0:0.00}" a clase de constantes
                cotizacion.montoSubTotal = Decimal.Parse(String.Format(Constantes.decimalFormat, cotizacion.montoTotal / (1 + cotizacion.igv)));
                cotizacion.montoIGV = cotizacion.montoTotal - cotizacion.montoSubTotal;
                cotizacion.estadoAprobacion = Converter.GetShort(row, "estado_aprobacion");

                cotizacion.observaciones = Converter.GetString(row, "observaciones");
                cotizacion.contacto = Converter.GetString(row, "contacto");
                cotizacion.cliente = new Cliente();
                cotizacion.cliente.codigo = Converter.GetString(row, "codigo");
                cotizacion.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                cotizacion.cliente.razonSocial = Converter.GetString(row, "razon_social");
                cotizacion.cliente.ruc = Converter.GetString(row, "ruc");

                cotizacion.usuario = new Usuario();
                cotizacion.usuario.nombre = Converter.GetString(row, "nombre_usuario");

                cotizacion.usuario_aprobador = new Usuario();
                cotizacion.usuario_aprobador.nombre = Converter.GetString(row, "nombre_usuario_aprobador");

                cotizacion.ciudad = new Ciudad();
                cotizacion.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                cotizacion.ciudad.nombre = Converter.GetString(row, "nombre");
                cotizacionList.Add(cotizacion);
            }
            return cotizacionList;
        }

    }
}
