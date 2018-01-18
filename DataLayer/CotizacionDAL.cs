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


        public void InsertCotizacionDetalle(CotizacionDetalle cotizacionDetalle)
        {
            var objCommand = GetSqlCommand("pi_cotizacionDetalle");
            InputParameterAdd.Guid(objCommand, "idCotizacion", cotizacionDetalle.idCotizacion);
            InputParameterAdd.Guid(objCommand, "idProducto", cotizacionDetalle.producto.idProducto);
            InputParameterAdd.Int(objCommand, "cantidad", cotizacionDetalle.cantidad);
            //Precio Lista en Cotizacion Detalle almacena el precio sin IGV
            //Esto es independiente de si se escogió la unidad standar o alternativa
            InputParameterAdd.Decimal(objCommand, "precioLista", cotizacionDetalle.producto.precioSinIgv);
            InputParameterAdd.Decimal(objCommand, "equivalencia", cotizacionDetalle.producto.equivalencia);
            InputParameterAdd.Varchar(objCommand, "unidad", cotizacionDetalle.unidad);
            InputParameterAdd.Decimal(objCommand, "porcentajeDescuento", cotizacionDetalle.porcentajeDescuento);
            InputParameterAdd.Decimal(objCommand, "precioNeto", cotizacionDetalle.precio);
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
                cotizacion.incluidoIgv = Converter.GetBool(row, "incluido_igv");
                cotizacion.considerarCantidades = Converter.GetBool(row, "considera_cantidades");
                cotizacion.flete = Converter.GetDecimal(row, "porcentaje_flete");
                cotizacion.igv = Converter.GetDecimal(row, "igv");
                cotizacion.montoTotal = Converter.GetDecimal(row, "total");

                ///Mover "{0:0.00}" a clase de constantes
                cotizacion.montoIGV = Decimal.Parse(String.Format("{0:0.00}", cotizacion.montoTotal * cotizacion.igv));
                cotizacion.montoSubTotal = cotizacion.montoTotal - cotizacion.montoIGV ;

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
                
            }


            cotizacion.cotizacionDetalleList = new List<CotizacionDetalle>();
            //Detalle de la cotizacion
            foreach (DataRow row in cotizacionDetalleDataTable.Rows)
            {
                CotizacionDetalle cotizacionDetalle = new CotizacionDetalle();
                cotizacionDetalle.producto = new Producto();

                cotizacionDetalle.idCotizacionDetalle = Converter.GetGuid(row, "id_cotizacion_detalle");
                cotizacionDetalle.cantidad = Converter.GetInt(row, "cantidad");
                cotizacionDetalle.precio = Converter.GetDecimal(row, "precio_neto");

                cotizacionDetalle.producto.precioSinIgv = Converter.GetDecimal(row, "precio_lista");
                cotizacionDetalle.producto.equivalencia = Converter.GetInt(row, "equivalencia");
                cotizacionDetalle.unidad = Converter.GetString(row, "unidad");

                cotizacionDetalle.producto.idProducto = Converter.GetGuid(row, "id_producto");
                cotizacionDetalle.producto.sku = Converter.GetString(row, "sku");
                cotizacionDetalle.producto.descripcion = Converter.GetString(row, "descripcion");
                cotizacionDetalle.producto.proveedor = Converter.GetString(row, "proveedor");


                cotizacionDetalle.producto.image = Converter.GetBytes(row, "imagen");

                cotizacionDetalle.esPrecioAlternativo = Converter.GetBool(row, "es_precio_alternativo");
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
                cotizacion.montoIGV = Decimal.Parse(String.Format("{0:0.00}", cotizacion.montoTotal * cotizacion.igv));
                cotizacion.montoSubTotal = cotizacion.montoTotal - cotizacion.montoIGV;


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
