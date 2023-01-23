using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class ProveedorDAL : DaoBase
    {
        public ProveedorDAL(IDalSettings settings) : base(settings)
        {
        }

        public ProveedorDAL() : this(new CotizadorSettings())
        {
        }

        public List<Proveedor> getProveedores()
        {
            var objCommand = GetSqlCommand("ps_getproveedores");
            DataTable dataTable = Execute(objCommand);
            List<Proveedor> lista = new List<Proveedor>();

            foreach (DataRow row in dataTable.Rows)
            {
                Proveedor obj = new Proveedor
                {
                    codigo = Converter.GetString(row, "codigo"),
                    nombre = Converter.GetString(row, "proveedor")
                };
                lista.Add(obj);
            }
            return lista;
        }




        public List<Proveedor> getProveedoresBusqueda(String textoBusqueda, Guid idCiudad)
        {
           var objCommand = GetSqlCommand("ps_proveedoresPorCiudad");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);

            DataTable dataTable = Execute(objCommand);
            List<Proveedor> proveedorList = new List<Proveedor>();

            foreach (DataRow row in dataTable.Rows)
            {
                Proveedor proveedor = new Proveedor
                {
                    idProveedor = Converter.GetGuid(row, "id_proveedor"),
                    codigo = Converter.GetString(row, "codigo"),
                    razonSocial = Converter.GetString(row, "razon_social"),
                    nombreComercial = Converter.GetString(row, "nombre_comercial"),
                    ruc = Converter.GetString(row, "ruc"),
                    contacto1 = Converter.GetString(row, "contacto1"),
                    contacto2 = Converter.GetString(row, "contacto2")
                };
                proveedorList.Add(proveedor);
            }
            return proveedorList;
        }

        public Proveedor getProveedor(Guid idProveedor)
        {
            var objCommand = GetSqlCommand("ps_proveedor");
            InputParameterAdd.Guid(objCommand, "idProveedor", idProveedor);
            DataTable dataTable = Execute(objCommand);
            Proveedor obj = new Proveedor();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idProveedor = Converter.GetGuid(row, "idProveedor");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.razonSocial = Converter.GetString(row, "razon_social");
                obj.ruc = Converter.GetString(row, "ruc");
                obj.contacto1 = Converter.GetString(row, "contacto1");
                obj.contacto2 = Converter.GetString(row, "contacto2");
                obj.domicilioLegal = Converter.GetString(row, "domicilio_legal");
                obj.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                obj.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                obj.nombreComercialSunat = Converter.GetString(row, "nombre_comercial_sunat");
                obj.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                obj.estadoContribuyente = Converter.GetString(row, "estado_contribuyente_sunat");
                obj.condicionContribuyente = Converter.GetString(row, "condicion_contribuyente_sunat");

                obj.ubigeo = new Ubigeo();
                obj.ubigeo.Id = Converter.GetString(row, "codigo_ubigeo");
                obj.ubigeo.Departamento = Converter.GetString(row, "departamento");
                obj.ubigeo.Provincia = Converter.GetString(row, "provincia");
                obj.ubigeo.Distrito = Converter.GetString(row, "distrito");
                obj.plazoCredito = Converter.GetString(row, "plazo_credito");
                obj.tipoPagoFactura = (DocumentoCompra.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                obj.formaPagoFactura = (DocumentoCompra.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                obj.ciudad = new Ciudad();
                obj.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
            }

            return obj;
        }






    }
}
