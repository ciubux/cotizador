using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class DireccionEntregaDAL : DaoBase
    {
        public DireccionEntregaDAL(IDalSettings settings) : base(settings)
        {
        }

        public DireccionEntregaDAL() : this(new CotizadorSettings())
        {
        }

        public List<DireccionEntrega> getDireccionesEntrega(String ubigeo, Guid idCLiente)
        {
            var objCommand = GetSqlCommand("ps_DireccionesEntrega");

            InputParameterAdd.Varchar(objCommand, "ubigeo", ubigeo); 
            InputParameterAdd.Guid(objCommand, "idCLiente", idCLiente); 

            DataTable dataTable = Execute(objCommand);
            List<DireccionEntrega> lista = new List<DireccionEntrega>();

            foreach (DataRow row in dataTable.Rows)
            {
                DireccionEntrega obj = new DireccionEntrega
                {
                    idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega"),
                    descripcion = Converter.GetString(row, "descripcion"),
                    contacto = Converter.GetString(row, "contacto"),
                    telefono = Converter.GetString(row, "telefono"),
                    ubigeo = new Ubigeo { Id = Converter.GetString(row, "ubigeo"),
                        Departamento = Converter.GetString(row, "departamento"),
                        Provincia = Converter.GetString(row, "provincia"),
                        Distrito = Converter.GetString(row, "distrito")
                    },
                    codigoCliente = Converter.GetString(row, "codigo_cliente"),
                    codigoMP = Converter.GetString(row, "codigo_mp"),
                    observaciones = Converter.GetString(row, "observaciones"),
                    nombre = Converter.GetString(row, "nombre"),
                    direccionDomicilioLegal = Converter.GetString(row, "direccionDomicilioLegal"),
                    codigo = Converter.GetInt(row, "codigo"),
                    emailRecepcionFacturas  = Converter.GetString(row, "email_recepcion_facturas"),
                };
                obj.cliente = new Cliente
                {
                    idCliente = Converter.GetGuid(row, "id_cliente")
                };
                obj.cliente.ciudad = new Ciudad
                {
                    idCiudad = Converter.GetGuid(row, "id_ciudad"),
                    sede = Converter.GetString(row, "sede"),
                };
                obj.domicilioLegal = new DomicilioLegal();
                obj.domicilioLegal.idDomicilioLegal = Converter.GetInt(row, "id_domicilio_legal");
                obj.domicilioLegal.direccion = Converter.GetString(row, "direccionDomicilioLegal");

                lista.Add(obj);
            }
            return lista;
        }

        public DireccionEntrega getDireccionEntregaPorCodigo(int codigo)
        {
            var objCommand = GetSqlCommand("ps_DireccionEntregaPorCodigo");

            InputParameterAdd.Int(objCommand, "codigo", codigo);

            DataTable dataTable = Execute(objCommand);

            DireccionEntrega direccionEntrega = new DireccionEntrega();

            foreach (DataRow row in dataTable.Rows)
            {
                DireccionEntrega obj = new DireccionEntrega
                {
                    idDireccionEntrega = Converter.GetGuid(row, "id_direccion_entrega"),
                    descripcion = Converter.GetString(row, "descripcion"),
                    contacto = Converter.GetString(row, "contacto"),
                    telefono = Converter.GetString(row, "telefono"),
                    ubigeo = new Ubigeo
                    {
                        Id = Converter.GetString(row, "ubigeo"),
                        Departamento = Converter.GetString(row, "departamento"),
                        Provincia = Converter.GetString(row, "provincia"),
                        Distrito = Converter.GetString(row, "distrito")
                    },
                    codigoCliente = Converter.GetString(row, "codigo_cliente"),
                    codigoMP = Converter.GetString(row, "codigo_mp"),
                    observaciones = Converter.GetString(row, "observaciones"),
                    nombre = Converter.GetString(row, "nombre"),
                    direccionDomicilioLegal = Converter.GetString(row, "direccionDomicilioLegal"),
                    codigo = Converter.GetInt(row, "codigo"),
                    emailRecepcionFacturas = Converter.GetString(row, "email_recepcion_facturas"),
                };
                obj.cliente = new Cliente
                {
                    idCliente = Converter.GetGuid(row, "id_cliente")
                };
                obj.cliente.ciudad = new Ciudad
                {
                    idCiudad = Converter.GetGuid(row, "id_ciudad"),
                    sede = Converter.GetString(row, "sede"),
                };
                obj.domicilioLegal = new DomicilioLegal();
                obj.domicilioLegal.idDomicilioLegal = Converter.GetInt(row, "id_domicilio_legal");
                obj.domicilioLegal.direccion = Converter.GetString(row, "direccionDomicilioLegal");

                direccionEntrega = obj;
            }
            
            return direccionEntrega;
        }

        public void deleteDireccionEntrega(DireccionEntrega direccionEntrega)
        {
            var objCommand = GetSqlCommand("pd_direccionEntrega");
            InputParameterAdd.Guid(objCommand, "idDireccionEntrega", direccionEntrega.idDireccionEntrega);
            InputParameterAdd.Guid(objCommand, "idUsuario", direccionEntrega.usuario.idUsuario);
            ExecuteNonQuery(objCommand);
        }

        public void updateDireccionEntrega(DireccionEntrega direccionEntrega)
        {
            var objCommand = GetSqlCommand("pu_direccionEntrega");
            InputParameterAdd.Guid(objCommand, "idDireccionEntrega", direccionEntrega.idDireccionEntrega);
            InputParameterAdd.Guid(objCommand, "idUsuario", direccionEntrega.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "ubigeo", direccionEntrega.ubigeo.Id);
            InputParameterAdd.Varchar(objCommand, "descripcion", direccionEntrega.descripcion);
            InputParameterAdd.Varchar(objCommand, "contacto", direccionEntrega.contacto);
            InputParameterAdd.Varchar(objCommand, "telefono", direccionEntrega.telefono);
            InputParameterAdd.Varchar(objCommand, "emailRecepcionFacturas", direccionEntrega.emailRecepcionFacturas);
            InputParameterAdd.Varchar(objCommand, "codigoCliente", direccionEntrega.codigoCliente);
            InputParameterAdd.Varchar(objCommand, "nombre", direccionEntrega.nombre);
            InputParameterAdd.Int(objCommand, "idDomicilioLegal", direccionEntrega.domicilioLegal.idDomicilioLegal);
            ExecuteNonQuery(objCommand);
        }

        public void insertDireccionEntrega(DireccionEntrega direccionEntrega)
        {
            var objCommand = GetSqlCommand("pi_direccionEntrega");
            InputParameterAdd.Guid(objCommand, "idUsuario", direccionEntrega.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "ubigeo", direccionEntrega.ubigeo.Id);
            InputParameterAdd.Varchar(objCommand, "descripcion", direccionEntrega.descripcion);
            InputParameterAdd.Varchar(objCommand, "contacto", direccionEntrega.contacto);
            InputParameterAdd.Varchar(objCommand, "telefono", direccionEntrega.telefono);
            InputParameterAdd.Varchar(objCommand, "emailRecepcionFacturas", direccionEntrega.emailRecepcionFacturas);
            InputParameterAdd.Varchar(objCommand, "codigoCliente", direccionEntrega.codigoCliente);
            InputParameterAdd.Varchar(objCommand, "nombre", direccionEntrega.nombre);
            InputParameterAdd.Int(objCommand, "idDomicilioLegal", direccionEntrega.domicilioLegal.idDomicilioLegal);
            InputParameterAdd.Guid(objCommand, "idCliente", direccionEntrega.cliente.idCliente);
            OutputParameterAdd.UniqueIdentifier(objCommand, "idDireccionEntrega");
            OutputParameterAdd.Int(objCommand, "codigo");
            ExecuteNonQuery(objCommand);
            direccionEntrega.idDireccionEntrega = (Guid)objCommand.Parameters["@idDireccionEntrega"].Value;
            direccionEntrega.codigo = (Int32)objCommand.Parameters["@codigo"].Value;
        }
    }
}
