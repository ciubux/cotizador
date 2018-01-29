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
            InputParameterAdd.DateTime(objCommand, "fechaVigenciaLimite", cotizacion.fechaVigenciaLimite);
            InputParameterAdd.SmallInt(objCommand, "incluidoIgv", short.Parse((cotizacion.incluidoIgv?1:0).ToString()));
            InputParameterAdd.SmallInt(objCommand, "consideraCantidades", short.Parse((cotizacion.considerarCantidades ? 1 : 0).ToString()));
            InputParameterAdd.Guid(objCommand, "idCliente", cotizacion.cliente.idCliente);
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
            InputParameterAdd.DateTime(objCommand, "fechaVigenciaLimite", cotizacion.fechaVigenciaLimite);
            InputParameterAdd.SmallInt(objCommand, "incluidoIgv", short.Parse((cotizacion.incluidoIgv ? 1 : 0).ToString()));
            InputParameterAdd.SmallInt(objCommand, "consideraCantidades", short.Parse((cotizacion.considerarCantidades ? 1 : 0).ToString()));
            InputParameterAdd.Guid(objCommand, "idCliente", cotizacion.cliente.idCliente);
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
            
            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            ExecuteNonQuery(objCommand);

            cotizacionDetalle.idCotizacionDetalle = (Guid)objCommand.Parameters["@newId"].Value;
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
                cotizacion.fechaVigenciaLimite = Converter.GetDateTime(row, "fecha_vigencia_limite");
                cotizacion.incluidoIgv = Converter.GetBool(row, "incluido_igv");
                cotizacion.considerarCantidades = Converter.GetBool(row, "considera_cantidades");
                cotizacion.flete = Converter.GetDecimal(row, "porcentaje_flete");
                cotizacion.igv = Converter.GetDecimal(row, "igv");
                cotizacion.montoTotal = Converter.GetDecimal(row, "total");
                cotizacion.estadoAprobacion = Converter.GetShort(row, "estado_aprobacion");

                ///Mover "{0:0.00}" a clase de constantes
                cotizacion.montoSubTotal = Decimal.Parse(String.Format("{0:0.00}", cotizacion.montoTotal / (1+cotizacion.igv)));
                cotizacion.montoIGV = cotizacion.montoTotal - cotizacion.montoSubTotal;

                cotizacion.observaciones = Converter.GetString(row, "observaciones");
                cotizacion.contacto = Converter.GetString(row, "contacto");
                cotizacion.cliente = new Cliente();
                cotizacion.cliente.codigo = Converter.GetString(row, "codigo");
                cotizacion.cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                cotizacion.cliente.razonSocial = Converter.GetString(row, "razon_social");
                cotizacion.cliente.ruc = Converter.GetString(row, "ruc");

                cotizacion.ciudad = new Ciudad();
                cotizacion.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                cotizacion.ciudad.nombre = Converter.GetString(row, "nombre");

                cotizacion.usuario = new Usuario();
                cotizacion.usuario.nombre_mostrar = Converter.GetString(row, "nombre_mostrar");
                cotizacion.usuario.cargo = Converter.GetString(row, "cargo");
                cotizacion.usuario.anexo_empresa = Converter.GetInt(row, "anexo_empresa");
                cotizacion.usuario.celular = Converter.GetInt(row, "celular");
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

                ///Mover "{0:0.00}" a clase de constantes
                cotizacion.montoSubTotal = Decimal.Parse(String.Format("{0:0.00}", cotizacion.montoTotal / (1 + cotizacion.igv)));
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
                cotizacion.usuario.nombre_mostrar = Converter.GetString(row, "nombre_mostrar");

                cotizacion.ciudad = new Ciudad();
                cotizacion.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                cotizacion.ciudad.nombre = Converter.GetString(row, "nombre");
                cotizacionList.Add(cotizacion);
            }
            return cotizacionList;
        }

    }
}
