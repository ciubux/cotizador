using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class ClienteDAL : DaoBase
    {
        public ClienteDAL(IDalSettings settings) : base(settings)
        {
        }

        public ClienteDAL() : this(new CotizadorSettings())
        {
        }

        public void setClienteStaging(ClienteStaging clienteStaging)
        {
            var objCommand = GetSqlCommand("pi_clienteStaging");
            InputParameterAdd.Varchar(objCommand, "PlazaId", clienteStaging.PlazaId);
            InputParameterAdd.Varchar(objCommand, "Plaza", clienteStaging.Plaza);
            InputParameterAdd.Varchar(objCommand, "Id", clienteStaging.codigo);
            InputParameterAdd.Varchar(objCommand, "nombre", clienteStaging.nombre);
            InputParameterAdd.Varchar(objCommand, "documento", clienteStaging.documento);
            InputParameterAdd.Varchar(objCommand, "codVe", clienteStaging.codVe);
            InputParameterAdd.Varchar(objCommand, "nombreComercial", clienteStaging.nombreComercial);
            InputParameterAdd.Varchar(objCommand, "domicilioLegal", clienteStaging.domicilioLegal);
            InputParameterAdd.Varchar(objCommand, "distrito", clienteStaging.distrito);
            InputParameterAdd.Varchar(objCommand, "direccionDespacho", clienteStaging.direccionDespacho);
            InputParameterAdd.Varchar(objCommand, "distritoDespacho", clienteStaging.distritoDespacho);
            InputParameterAdd.Varchar(objCommand, "rubro", clienteStaging.rubro);
            InputParameterAdd.Char(objCommand, "sede", clienteStaging.sede);
            InputParameterAdd.Varchar(objCommand, "plazo", clienteStaging.plazo);
            ExecuteNonQuery(objCommand);
            
        }


        public void truncateClienteStaging(String sede)
        {
            var objCommand = GetSqlCommand("pt_clienteStaging");
            InputParameterAdd.Char(objCommand, "sede", sede);
            ExecuteNonQuery(objCommand);
        }

        public void mergeClienteStaging()
        {
            var objCommand = GetSqlCommand("pu_clienteStaging");
            ExecuteNonQuery(objCommand);
        }


        public List<Cliente> getClientesBusqueda(String textoBusqueda, Guid idCiudad)
        {
            var objCommand = GetSqlCommand("ps_getclientes_search");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            InputParameterAdd.Guid(objCommand, "idCiudad", idCiudad);
            
            DataTable dataTable = Execute(objCommand);
            List<Cliente> clienteList = new List<Cliente>();

            foreach (DataRow row in dataTable.Rows)
            {
                Cliente cliente = new Cliente
                {
                    idCliente = Converter.GetGuid(row, "id_cliente"),
                    codigo = Converter.GetString(row, "codigo"),
                    razonSocial = Converter.GetString(row, "razon_social"),
                    nombreComercial = Converter.GetString(row, "nombre_comercial"),
                    ruc = Converter.GetString(row, "ruc"),
                    contacto1 = Converter.GetString(row, "contacto1"),
                    contacto2 = Converter.GetString(row, "contacto2")
                };
                clienteList.Add(cliente);
            }
            return clienteList;
        }

        public Cliente getCliente(Guid idCliente)
        {
            var objCommand = GetSqlCommand("ps_getcliente");
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            DataTable dataTable = Execute(objCommand);
            Cliente obj = new Cliente();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idCliente = Converter.GetGuid(row, "id_cliente");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.razonSocial = Converter.GetString(row, "razon_social");
                obj.nombreComercial = Converter.GetString(row, "nombre_comercial");
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
                obj.tipoPagoFactura = (DocumentoVenta.TipoPago) Converter.GetInt(row, "tipo_pago_factura");
                obj.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                obj.ciudad = new Ciudad();
                obj.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");


                ///Nuevos Campos
                InputParameterAdd.Int(objCommand, "tipoDocumentoIdentidad", (int)cliente.tipoDocumentoIdentidad);
                InputParameterAdd.Decimal(objCommand, "creditoSolicitado", cliente.creditoSolicitado);
                InputParameterAdd.Decimal(objCommand, "creditoAprobado", cliente.creditoAprobado);
                InputParameterAdd.Decimal(objCommand, "sobreGiro", (int)cliente.sobreGiro);
                InputParameterAdd.Int(objCommand, "sobrePlazo", cliente.sobrePlazo);
                InputParameterAdd.Int(objCommand, "tipoPagoSolicitado", (int)cliente.tipoPagoSolicitado);
                InputParameterAdd.Int(objCommand, "idAsistenteServicioCliente", cliente.asistenteServicioCliente.idVendedor);
                InputParameterAdd.Int(objCommand, "idResponsableComercial", cliente.responsableComercial.idVendedor);
                InputParameterAdd.Int(objCommand, "idSupervisorComercial", cliente.supervisorComercial.idVendedor);


            }

            return obj;
        }

        public Cliente insertCliente(Cliente cliente)
        {
            var objCommand = GetSqlCommand("pi_cliente");

            InputParameterAdd.Guid(objCommand, "idUsuario", cliente.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "razonSocial", cliente.razonSocial);
            InputParameterAdd.Varchar(objCommand, "nombreComercial", cliente.nombreComercial);
            InputParameterAdd.Varchar(objCommand, "ruc", cliente.ruc);
            InputParameterAdd.Varchar(objCommand, "contacto1", cliente.contacto1);
            InputParameterAdd.Guid(objCommand, "idCiudad", cliente.ciudad.idCiudad);

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            OutputParameterAdd.Int(objCommand, "codigoAlterno");
            ExecuteNonQuery(objCommand);

            cliente.idCliente = (Guid)objCommand.Parameters["@newId"].Value;
            cliente.codigoAlterno = (Int32)objCommand.Parameters["@codigoAlterno"].Value;

            return cliente;

        }



        public Cliente insertClienteSunat(Cliente cliente)
        {
            var objCommand = GetSqlCommand("pi_clienteSunat");

            InputParameterAdd.Guid(objCommand, "idUsuario", cliente.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "razonSocial", cliente.razonSocialSunat);
            InputParameterAdd.Varchar(objCommand, "nombreComercial", cliente.nombreComercial);
            InputParameterAdd.Varchar(objCommand, "ruc", cliente.ruc);
            InputParameterAdd.Varchar(objCommand, "contacto1", cliente.contacto1);
            InputParameterAdd.Guid(objCommand, "idCiudad", cliente.ciudad.idCiudad);
            InputParameterAdd.Varchar(objCommand, "correoEnvioFactura", cliente.correoEnvioFactura);
            InputParameterAdd.Varchar(objCommand, "razonSocialSunat", cliente.razonSocialSunat);
            InputParameterAdd.Varchar(objCommand, "nombreComercialSunat", cliente.nombreComercial);
            InputParameterAdd.Varchar(objCommand, "direccionDomicilioLegalSunat", cliente.direccionDomicilioLegalSunat);
            InputParameterAdd.Varchar(objCommand, "estadoContribuyente", cliente.estadoContribuyente);
            InputParameterAdd.Varchar(objCommand, "condicionContribuyente", cliente.condicionContribuyente);
            InputParameterAdd.Varchar(objCommand, "ubigeo", cliente.ubigeo.Id);
            InputParameterAdd.Int(objCommand, "tipoPagoFactura", (int)cliente.tipoPagoFactura);
            InputParameterAdd.Int(objCommand, "formaPagoFactura", (int)cliente.formaPagoFactura);
            
            
            ///Nuevos Campos
            InputParameterAdd.Int(objCommand, "tipoDocumentoIdentidad", (int)cliente.tipoDocumentoIdentidad);
            InputParameterAdd.Decimal(objCommand, "creditoSolicitado", cliente.creditoSolicitado);
            InputParameterAdd.Decimal(objCommand, "creditoAprobado", cliente.creditoAprobado);
            InputParameterAdd.Decimal(objCommand, "sobreGiro", (int)cliente.sobreGiro);
            InputParameterAdd.Int(objCommand, "sobrePlazo", cliente.sobrePlazo);
            InputParameterAdd.Int(objCommand, "tipoPagoSolicitado", (int)cliente.tipoPagoSolicitado);
            InputParameterAdd.Int(objCommand, "idAsistenteServicioCliente", cliente.asistenteServicioCliente.idVendedor);
            InputParameterAdd.Int(objCommand, "idResponsableComercial", cliente.responsableComercial.idVendedor);
            InputParameterAdd.Int(objCommand, "idSupervisorComercial", cliente.supervisorComercial.idVendedor);

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            OutputParameterAdd.Int(objCommand, "codigoAlterno");


            ExecuteNonQuery(objCommand);

            cliente.idCliente = (Guid)objCommand.Parameters["@newId"].Value;
            cliente.codigoAlterno = (Int32)objCommand.Parameters["@codigoAlterno"].Value;

            return cliente;

        }



        public Cliente updateClienteSunat(Cliente cliente)
        {
            var objCommand = GetSqlCommand("pu_clienteSunat");
            //cliente.idCliente = (Guid)objCommand.Parameters["@newId"].Value;
            InputParameterAdd.Guid(objCommand, "idCliente", cliente.idCliente);
            InputParameterAdd.Guid(objCommand, "idUsuario", cliente.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "razonSocial", cliente.razonSocialSunat);
            InputParameterAdd.Varchar(objCommand, "nombreComercial", cliente.nombreComercial);
            InputParameterAdd.Varchar(objCommand, "contacto1", cliente.contacto1);
            InputParameterAdd.Guid(objCommand, "idCiudad", cliente.ciudad.idCiudad);            
            InputParameterAdd.Varchar(objCommand, "correoEnvioFactura", cliente.correoEnvioFactura);
            InputParameterAdd.Varchar(objCommand, "razonSocialSunat", cliente.razonSocialSunat);
            InputParameterAdd.Varchar(objCommand, "nombreComercialSunat", cliente.nombreComercialSunat);
            InputParameterAdd.Varchar(objCommand, "direccionDomicilioLegalSunat", cliente.direccionDomicilioLegalSunat);
            InputParameterAdd.Varchar(objCommand, "estadoContribuyente", cliente.estadoContribuyente);
            InputParameterAdd.Varchar(objCommand, "condicionContribuyente", cliente.condicionContribuyente);
            InputParameterAdd.Varchar(objCommand, "ubigeo", cliente.ubigeo.Id);
            InputParameterAdd.Int(objCommand, "tipoPagoFactura", (int)cliente.tipoPagoFactura);
            InputParameterAdd.Int(objCommand, "formaPagoFactura", (int)cliente.formaPagoFactura);

            ///Nuevos Campos
            InputParameterAdd.Int(objCommand, "tipoDocumentoIdentidad", (int)cliente.tipoDocumentoIdentidad);
            InputParameterAdd.Decimal(objCommand, "creditoSolicitado", cliente.creditoSolicitado);
            InputParameterAdd.Decimal(objCommand, "creditoAprobado", cliente.creditoAprobado);
            InputParameterAdd.Decimal(objCommand, "sobreGiro", (int)cliente.sobreGiro);
            InputParameterAdd.Int(objCommand, "sobrePlazo", cliente.sobrePlazo);
            InputParameterAdd.Int(objCommand, "tipoPagoSolicitado", (int)cliente.tipoPagoSolicitado);
            InputParameterAdd.Int(objCommand, "idAsistenteServicioCliente", cliente.asistenteServicioCliente.idVendedor);
            InputParameterAdd.Int(objCommand, "idResponsableComercial", cliente.responsableComercial.idVendedor);
            InputParameterAdd.Int(objCommand, "idSupervisorComercial", cliente.supervisorComercial.idVendedor);

            ExecuteNonQuery(objCommand);            
        //    cliente.codigoAlterno = (Int32)objCommand.Parameters["@codigoAlterno"].Value;

            return cliente;

        }


    }
}
