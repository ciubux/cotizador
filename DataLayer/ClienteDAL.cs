﻿using Framework.DAL;
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


        public Guid getClienteId(String ruc, String codigoSedeMP)
        {
            var objCommand = GetSqlCommand("ps_clienteId");
            InputParameterAdd.Varchar(objCommand, "ruc", ruc);
            InputParameterAdd.Varchar(objCommand, "codigoSede", codigoSedeMP);
            DataTable dataTable = Execute(objCommand);

            Guid idCliente = Guid.Empty;
            foreach (DataRow row in dataTable.Rows)
            {
                idCliente = Converter.GetGuid(row, "id_cliente");
            }
            return idCliente;
        }


        public Cliente getCliente(Guid idCliente)
        {
            var objCommand = GetSqlCommand("ps_cliente");
            InputParameterAdd.Guid(objCommand, "idCliente", idCliente);
            DataTable dataTable = Execute(objCommand);
            Cliente cliente = new Cliente();

            foreach (DataRow row in dataTable.Rows)
            {
                cliente.idCliente = Converter.GetGuid(row, "id_cliente");
                cliente.codigo = Converter.GetString(row, "codigo");
                cliente.razonSocial = Converter.GetString(row, "razon_social");
                cliente.nombreComercial = Converter.GetString(row, "nombre_comercial");
                cliente.ruc = Converter.GetString(row, "ruc");
                cliente.contacto1 = Converter.GetString(row, "contacto1");
                cliente.contacto2 = Converter.GetString(row, "contacto2");
                cliente.domicilioLegal = Converter.GetString(row, "domicilio_legal");
                cliente.correoEnvioFactura = Converter.GetString(row, "correo_envio_factura");
                cliente.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                cliente.nombreComercialSunat = Converter.GetString(row, "nombre_comercial_sunat");
                cliente.direccionDomicilioLegalSunat = Converter.GetString(row, "direccion_domicilio_legal_sunat");
                cliente.estadoContribuyente = Converter.GetString(row, "estado_contribuyente_sunat");
                cliente.condicionContribuyente = Converter.GetString(row, "condicion_contribuyente_sunat");

                cliente.ubigeo = new Ubigeo();
                cliente.ubigeo.Id = Converter.GetString(row, "codigo_ubigeo");
                cliente.ubigeo.Departamento = Converter.GetString(row, "departamento");
                cliente.ubigeo.Provincia = Converter.GetString(row, "provincia");
                cliente.ubigeo.Distrito = Converter.GetString(row, "distrito");
                cliente.plazoCredito = Converter.GetString(row, "plazo_credito");

                cliente.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");
                cliente.ciudad = new Ciudad();
                cliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                cliente.ciudad.nombre = Converter.GetString(row, "ciudad_nombre");

                /*Plazo Crédito*/
                cliente.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                cliente.plazoCreditoSolicitado = (DocumentoVenta.TipoPago)Converter.GetInt(row, "plazo_credito_solicitado");
                cliente.sobrePlazo = Converter.GetInt(row, "sobre_plazo");

                /*Monto Crédito*/
                cliente.creditoSolicitado = Converter.GetDecimal(row, "credito_solicitado");
                cliente.creditoAprobado = Converter.GetDecimal(row, "credito_aprobado");
                cliente.sobreGiro = Converter.GetDecimal(row, "sobre_giro");

                cliente.vendedoresAsignados = Converter.GetBool(row, "vendedores_asignados");
                cliente.tipoDocumentoIdentidad = (DocumentoVenta.TiposDocumentoIdentidad)Char.Parse(Converter.GetString(row, "tipo_documento"));

                /*Vendedores*/
                cliente.responsableComercial = new Vendedor();
                cliente.responsableComercial.idVendedor = Converter.GetInt(row, "responsable_comercial_id_vendedor");
                cliente.responsableComercial.codigo = Converter.GetString(row, "responsable_comercial_codigo");
                cliente.responsableComercial.descripcion = Converter.GetString(row, "responsable_comercial_descripcion");

                cliente.supervisorComercial = new Vendedor();
                cliente.supervisorComercial.idVendedor = Converter.GetInt(row, "supervisor_comercial_id_vendedor");
                cliente.supervisorComercial.codigo = Converter.GetString(row, "supervisor_comercial_codigo");
                cliente.supervisorComercial.descripcion = Converter.GetString(row, "supervisor_comercial_descripcion");

                cliente.asistenteServicioCliente = new Vendedor();
                cliente.asistenteServicioCliente.idVendedor = Converter.GetInt(row, "asistente_servicio_cliente_id_vendedor");
                cliente.asistenteServicioCliente.codigo = Converter.GetString(row, "asistente_servicio_cliente_codigo");
                cliente.asistenteServicioCliente.descripcion = Converter.GetString(row, "asistente_servicio_cliente_descripcion");

                cliente.observacionesCredito = Converter.GetString(row, "observaciones_credito");
                cliente.observaciones = Converter.GetString(row, "observaciones");

                cliente.bloqueado = Converter.GetBool(row, "bloqueado");

                cliente.perteneceCanalMultiregional = Converter.GetBool(row, "pertenece_canal_multiregional");
                cliente.perteneceCanalLima = Converter.GetBool(row, "pertenece_canal_lima");
                cliente.perteneceCanalProvincias = Converter.GetBool(row, "pertenece_canal_provincia");
                cliente.perteneceCanalPCP = Converter.GetBool(row, "pertenece_canal_pcp");
                cliente.perteneceCanalOrdon = Converter.GetBool(row, "pertenece_canal_ordon");
                cliente.esSubDistribuidor = Converter.GetBool(row, "es_sub_distribuidor");

                cliente.grupoCliente = new GrupoCliente();
                cliente.grupoCliente.idGrupoCliente = Converter.GetInt(row, "id_grupo_cliente");
                cliente.grupoCliente.nombre = Converter.GetString(row, "grupo_nombre");
            }

            return cliente;
        }

        public List<Cliente> SelectClientes(Cliente cliente)
        {
            var objCommand = GetSqlCommand("ps_clientes");
            InputParameterAdd.Varchar(objCommand, "codigo", cliente.codigo);
            InputParameterAdd.Guid(objCommand, "idCiudad", cliente.ciudad.idCiudad);
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", cliente.textoBusqueda);
            InputParameterAdd.Int(objCommand, "idResponsableComercial", cliente.responsableComercial.idVendedor);
            InputParameterAdd.Int(objCommand, "idSupervisorComercial", cliente.supervisorComercial.idVendedor);
            InputParameterAdd.Int(objCommand, "idAsistenteServicioCliente", cliente.asistenteServicioCliente.idVendedor);
            InputParameterAdd.Int(objCommand, "sinPlazoCreditoAprobado", cliente.sinPlazoCreditoAprobado ? 1 : 0);
            InputParameterAdd.Int(objCommand, "bloqueado", cliente.bloqueado ? 1 : 0);
            InputParameterAdd.Int(objCommand, "idGrupoCliente", cliente.grupoCliente.idGrupoCliente);
            DataTable dataTable = Execute(objCommand);

            List<Cliente> clienteList = new List<Cliente>();
            foreach (DataRow row in dataTable.Rows)
            {
                Cliente ClienteResultado = new Cliente();
                ClienteResultado.idCliente = Converter.GetGuid(row, "id_cliente");
                ClienteResultado.codigo = Converter.GetString(row, "codigo");
                ClienteResultado.ciudad = new Ciudad();
                ClienteResultado.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                ClienteResultado.ciudad.nombre = Converter.GetString(row, "ciudad_nombre");

                ClienteResultado.razonSocialSunat = Converter.GetString(row, "razon_social_sunat");
                ClienteResultado.nombreComercial = Converter.GetString(row, "nombre_comercial");
                ClienteResultado.tipoDocumentoIdentidad = (DocumentoVenta.TiposDocumentoIdentidad)Converter.GetInt(row, "tipo_documento");
                ClienteResultado.ruc = Converter.GetString(row, "ruc");

                /*Vendedores*/
                ClienteResultado.responsableComercial = new Vendedor();
                ClienteResultado.responsableComercial.codigo = Converter.GetString(row, "responsable_comercial_codigo");
                ClienteResultado.responsableComercial.descripcion = Converter.GetString(row, "responsable_comercial_descripcion");

                ClienteResultado.supervisorComercial = new Vendedor();
                ClienteResultado.supervisorComercial.codigo = Converter.GetString(row, "supervisor_comercial_codigo");
                ClienteResultado.supervisorComercial.descripcion = Converter.GetString(row, "supervisor_comercial_descripcion");

                ClienteResultado.asistenteServicioCliente = new Vendedor();
                ClienteResultado.asistenteServicioCliente.codigo = Converter.GetString(row, "asistente_servicio_cliente_codigo");
                ClienteResultado.asistenteServicioCliente.descripcion = Converter.GetString(row, "asistente_servicio_cliente_descripcion");

                /*Plazo de Crédito Aprobado*/
                ClienteResultado.plazoCredito = Converter.GetString(row, "plazo_credito");
                /*Forma de Pago Aprobado*/
                ClienteResultado.formaPagoFactura = (DocumentoVenta.FormaPago)Converter.GetInt(row, "forma_pago_factura");

                /*Plazo Crédito*/
                ClienteResultado.tipoPagoFactura = (DocumentoVenta.TipoPago)Converter.GetInt(row, "tipo_pago_factura");
                ClienteResultado.creditoAprobado = Converter.GetDecimal(row, "credito_aprobado");
                ClienteResultado.sobrePlazo = Converter.GetInt(row, "sobre_plazo");

                /*Monto Crédito*/
                ClienteResultado.creditoSolicitado = Converter.GetDecimal(row, "credito_solicitado");
                ClienteResultado.creditoAprobado = Converter.GetDecimal(row, "credito_aprobado");
                ClienteResultado.sobreGiro = Converter.GetDecimal(row, "sobre_giro");

                ClienteResultado.vendedoresAsignados = Converter.GetBool(row, "vendedores_asignados");
                ClienteResultado.tipoDocumentoIdentidad = (DocumentoVenta.TiposDocumentoIdentidad)Char.Parse(Converter.GetString(row, "tipo_documento"));

                ClienteResultado.bloqueado = Converter.GetBool(row, "bloqueado");

                ClienteResultado.grupoCliente = new GrupoCliente();
                ClienteResultado.grupoCliente.idGrupoCliente = Converter.GetInt(row, "id_grupo_cliente");
                ClienteResultado.grupoCliente.nombre = Converter.GetString(row, "grupo");

                clienteList.Add(ClienteResultado);
            }

            return clienteList;
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
            InputParameterAdd.Varchar(objCommand, "nombreComercialSunat", cliente.nombreComercialSunat);
            InputParameterAdd.Varchar(objCommand, "direccionDomicilioLegalSunat", cliente.direccionDomicilioLegalSunat);
            InputParameterAdd.Varchar(objCommand, "estadoContribuyente", cliente.estadoContribuyente);
            InputParameterAdd.Varchar(objCommand, "condicionContribuyente", cliente.condicionContribuyente);
            InputParameterAdd.Varchar(objCommand, "ubigeo", cliente.ubigeo.Id);
            //InputParameterAdd.Int(objCommand, "tipoPagoFactura", (int)cliente.tipoPagoFactura);
            InputParameterAdd.Int(objCommand, "formaPagoFactura", (int)cliente.formaPagoFactura);
            
            /*Plazo credito*/
            InputParameterAdd.Int(objCommand, "plazoCreditoSolicitado", (int)cliente.plazoCreditoSolicitado);
            InputParameterAdd.Int(objCommand, "tipoPagoFactura", (int)cliente.tipoPagoFactura);
            InputParameterAdd.Int(objCommand, "sobrePlazo", cliente.sobrePlazo);
            /*Monto Crédito*/
            InputParameterAdd.Decimal(objCommand, "creditoSolicitado", cliente.creditoSolicitado);
            InputParameterAdd.Decimal(objCommand, "creditoAprobado", cliente.creditoAprobado);
            InputParameterAdd.Decimal(objCommand, "sobreGiro", cliente.sobreGiro);
            /*Vendedores*/
            InputParameterAdd.Decimal(objCommand, "idResponsableComercial", cliente.responsableComercial.idVendedor);
            InputParameterAdd.Decimal(objCommand, "idAsistenteServicioCliente", cliente.asistenteServicioCliente.idVendedor);
            InputParameterAdd.Decimal(objCommand, "idSupervisorComercial", cliente.supervisorComercial.idVendedor);

            InputParameterAdd.Char(objCommand, "tipoDocumento", ((char)cliente.tipoDocumentoIdentidad).ToString());

            InputParameterAdd.Varchar(objCommand, "observacionesCredito", cliente.observacionesCredito);
            InputParameterAdd.Varchar(objCommand, "observaciones", cliente.observaciones);


            InputParameterAdd.SmallInt(objCommand, "vendedoresAsignados", (short)(cliente.vendedoresAsignados?1:0));

            InputParameterAdd.SmallInt(objCommand, "bloqueado", (short)(cliente.bloqueado ? 1 : 0));

            InputParameterAdd.SmallInt(objCommand, "perteneceCanalMultiregional", (short)(cliente.perteneceCanalMultiregional ? 1 : 0));
            InputParameterAdd.SmallInt(objCommand, "perteneceCanalLima", (short)(cliente.perteneceCanalLima ? 1 : 0));
            InputParameterAdd.SmallInt(objCommand, "perteneceCanalProvincias", (short)(cliente.perteneceCanalProvincias ? 1 : 0));
            InputParameterAdd.SmallInt(objCommand, "perteneceCanalPCP", (short)(cliente.perteneceCanalPCP ? 1 : 0));
            InputParameterAdd.SmallInt(objCommand, "esSubDistribuidor", (short)(cliente.esSubDistribuidor ? 1 : 0));

            InputParameterAdd.Int(objCommand, "idGrupoCliente", cliente.grupoCliente == null ? 0 : cliente.grupoCliente.idGrupoCliente);

            DateTime dtTmp = DateTime.Now;
            String[] horaTmp = cliente.horaInicioPrimerTurnoEntrega.Split(':');
            DateTime horaInicioPrimerTurnoEntrega = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaTmp[0]), Int32.Parse(horaTmp[1]), 0);
            InputParameterAdd.DateTime(objCommand, "horaInicioPrimerTurnoEntrega", horaInicioPrimerTurnoEntrega);
            horaTmp = cliente.horaFinPrimerTurnoEntrega.Split(':');
            DateTime horaFinPrimerTurnoEntrega = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaTmp[0]), Int32.Parse(horaTmp[1]), 0);            
            InputParameterAdd.DateTime(objCommand, "horaFinPrimerTurnoEntrega", horaFinPrimerTurnoEntrega);
            if (cliente.horaInicioSegundoTurnoEntrega == null || cliente.horaFinSegundoTurnoEntrega == null
                || cliente.horaInicioSegundoTurnoEntrega.Equals(String.Empty)
                || cliente.horaFinSegundoTurnoEntrega.Equals(String.Empty))
            {
                InputParameterAdd.DateTime(objCommand, "horaInicioSegundoTurnoEntrega", null);
                InputParameterAdd.DateTime(objCommand, "horaFinSegundoTurnoEntrega", null);
            }
            else
            {
                horaTmp = cliente.horaInicioSegundoTurnoEntrega.Split(':');
                DateTime horaInicioSegundoTurnoEntrega = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaTmp[0]), Int32.Parse(horaTmp[1]), 0);
                InputParameterAdd.DateTime(objCommand, "horaInicioSegundoTurnoEntrega", horaInicioSegundoTurnoEntrega);
                horaTmp = cliente.horaFinSegundoTurnoEntrega.Split(':');
                DateTime horaFinSegundoTurnoEntrega = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaTmp[0]), Int32.Parse(horaTmp[1]), 0);
                InputParameterAdd.DateTime(objCommand, "horaFinSegundoTurnoEntrega", horaFinSegundoTurnoEntrega);
            }

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");
            OutputParameterAdd.Int(objCommand, "codigoAlterno");
            OutputParameterAdd.Varchar(objCommand, "codigo",4);

            ExecuteNonQuery(objCommand);

            cliente.idCliente = (Guid)objCommand.Parameters["@newId"].Value;
            cliente.codigoAlterno = (Int32)objCommand.Parameters["@codigoAlterno"].Value;
            cliente.codigo = (String)objCommand.Parameters["@codigo"].Value;

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
            //InputParameterAdd.Int(objCommand, "tipoPagoFactura", (int)cliente.tipoPagoFactura);
            InputParameterAdd.Int(objCommand, "formaPagoFactura", (int)cliente.formaPagoFactura);

            /*Plazo credito*/
            InputParameterAdd.Int(objCommand, "plazoCreditoSolicitado", (int)cliente.plazoCreditoSolicitado);
            InputParameterAdd.Int(objCommand, "tipoPagoFactura", (int)cliente.tipoPagoFactura);
            InputParameterAdd.Int(objCommand, "sobrePlazo", cliente.sobrePlazo);
            /*Monto Crédito*/
            InputParameterAdd.Decimal(objCommand, "creditoSolicitado", cliente.creditoSolicitado);
            InputParameterAdd.Decimal(objCommand, "creditoAprobado", cliente.creditoAprobado);
            InputParameterAdd.Decimal(objCommand, "sobreGiro", cliente.sobreGiro);
            /*Vendedores*/
            InputParameterAdd.Decimal(objCommand, "idResponsableComercial", cliente.responsableComercial.idVendedor);
            InputParameterAdd.Decimal(objCommand, "idAsistenteServicioCliente", cliente.asistenteServicioCliente.idVendedor);
            InputParameterAdd.Decimal(objCommand, "idSupervisorComercial", cliente.supervisorComercial.idVendedor);

            InputParameterAdd.Varchar(objCommand, "observacionesCredito", cliente.observacionesCredito);
            InputParameterAdd.Varchar(objCommand, "observaciones", cliente.observaciones);
            InputParameterAdd.SmallInt(objCommand, "vendedoresAsignados", (short)(cliente.vendedoresAsignados ? 1 : 0));
            InputParameterAdd.SmallInt(objCommand, "bloqueado", (short)(cliente.bloqueado ? 1 : 0));

            InputParameterAdd.SmallInt(objCommand, "perteneceCanalMultiregional", (short)(cliente.perteneceCanalMultiregional ? 1 : 0));
            InputParameterAdd.SmallInt(objCommand, "perteneceCanalLima", (short)(cliente.perteneceCanalLima ? 1 : 0));
            InputParameterAdd.SmallInt(objCommand, "perteneceCanalProvincias", (short)(cliente.perteneceCanalProvincias ? 1 : 0));
            InputParameterAdd.SmallInt(objCommand, "perteneceCanalPCP", (short)(cliente.perteneceCanalPCP ? 1 : 0));
            InputParameterAdd.SmallInt(objCommand, "esSubDistribuidor", (short)(cliente.esSubDistribuidor ? 1 : 0));


            InputParameterAdd.Int(objCommand, "idGrupoCliente", cliente.grupoCliente==null?0: cliente.grupoCliente.idGrupoCliente);
            DateTime dtTmp = DateTime.Now;
            String[] horaTmp = cliente.horaInicioPrimerTurnoEntrega.Split(':');
            DateTime horaInicioPrimerTurnoEntrega = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaTmp[0]), Int32.Parse(horaTmp[1]), 0);
            InputParameterAdd.DateTime(objCommand, "horaInicioPrimerTurnoEntrega", horaInicioPrimerTurnoEntrega);
            horaTmp = cliente.horaFinPrimerTurnoEntrega.Split(':');
            DateTime horaFinPrimerTurnoEntrega = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaTmp[0]), Int32.Parse(horaTmp[1]), 0);
            InputParameterAdd.DateTime(objCommand, "horaFinPrimerTurnoEntrega", horaFinPrimerTurnoEntrega);
            if (cliente.horaInicioSegundoTurnoEntrega == null || cliente.horaFinSegundoTurnoEntrega == null
                || cliente.horaInicioSegundoTurnoEntrega.Equals(String.Empty)
                || cliente.horaFinSegundoTurnoEntrega.Equals(String.Empty))
            {
                InputParameterAdd.DateTime(objCommand, "horaInicioSegundoTurnoEntrega", null);
                InputParameterAdd.DateTime(objCommand, "horaFinSegundoTurnoEntrega", null);
            }
            else
            {
                horaTmp = cliente.horaInicioSegundoTurnoEntrega.Split(':');
                DateTime horaInicioSegundoTurnoEntrega = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaTmp[0]), Int32.Parse(horaTmp[1]), 0);
                InputParameterAdd.DateTime(objCommand, "horaInicioSegundoTurnoEntrega", horaInicioSegundoTurnoEntrega);
                horaTmp = cliente.horaFinSegundoTurnoEntrega.Split(':');
                DateTime horaFinSegundoTurnoEntrega = new DateTime(dtTmp.Year, dtTmp.Month, dtTmp.Day, Int32.Parse(horaTmp[0]), Int32.Parse(horaTmp[1]), 0);
                InputParameterAdd.DateTime(objCommand, "horaFinSegundoTurnoEntrega", horaFinSegundoTurnoEntrega);
            }
            ExecuteNonQuery(objCommand);            

            return cliente;

        }


    }
}
