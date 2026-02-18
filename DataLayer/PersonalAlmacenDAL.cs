using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace DataLayer
{
    public class PersonalAlmacenDAL : DaoBase
    {
        public PersonalAlmacenDAL(IDalSettings settings) : base(settings)
        {
        }

        public PersonalAlmacenDAL() : this(new CotizadorSettings())
        {
        }

        public PersonalAlmacen getPersonalAlmacen(int idPersonalAlmacen)
        {
            var objCommand = GetSqlCommand("ps_personal_almacen");
            InputParameterAdd.Int(objCommand, "idPersonalAlmacen", idPersonalAlmacen);
            DataTable dataTable = Execute(objCommand);
            PersonalAlmacen obj = new PersonalAlmacen();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idPersonalAlmacen = Converter.GetInt(row, "id_personal_almacen");
                obj.nombres = Converter.GetString(row, "nombres");
                obj.apellidoPaterno = Converter.GetString(row, "apellido_paterno");
                obj.apellidoMaterno = Converter.GetString(row, "apellido_materno");
                obj.nroDocumento = Converter.GetString(row, "nro_documento");
                obj.brevete = Converter.GetString(row, "brevete");
                obj.tipo = Converter.GetString(row, "tipo");
                obj.Estado = Converter.GetInt(row, "estado");

                obj.sedePrincipal = new Ciudad
                {
                    idCiudad = Converter.GetGuid(row, "id_sede_principal"),
                    nombre = Converter.GetString(row, "nombre_ciudad")
                };
            }

            return obj;
        }

        public List<PersonalAlmacen> getPersonalesAlmacen(PersonalAlmacen personal)
        {
            var objCommand = GetSqlCommand("ps_personales_almacen");
            InputParameterAdd.Int(objCommand, "estado", personal.Estado);
            InputParameterAdd.VarcharEmpty(objCommand, "tipo", personal.tipo);

            if (personal.sedePrincipal != null && !personal.sedePrincipal.idCiudad.Equals(Guid.Empty))
            {
                InputParameterAdd.Guid(objCommand, "idCiudad", personal.sedePrincipal.idCiudad);
            }

            DataTable dataTable = Execute(objCommand);
            List<PersonalAlmacen> lista = new List<PersonalAlmacen>();

            foreach (DataRow row in dataTable.Rows)
            {
                PersonalAlmacen obj = new PersonalAlmacen();
                obj.idPersonalAlmacen = Converter.GetInt(row, "id_personal_almacen");
                obj.nombres = Converter.GetString(row, "nombres");
                obj.apellidoPaterno = Converter.GetString(row, "apellido_paterno");
                obj.apellidoMaterno = Converter.GetString(row, "apellido_materno");
                obj.nroDocumento = Converter.GetString(row, "nro_documento");
                obj.brevete = Converter.GetString(row, "brevete");
                obj.tipo = Converter.GetString(row, "tipo");
                obj.Estado = Converter.GetInt(row, "estado");

                obj.sedePrincipal = new Ciudad
                {
                    idCiudad = Converter.GetGuid(row, "id_sede_principal"),
                    nombre = Converter.GetString(row, "nombre_ciudad")
                };

                lista.Add(obj);
            }

            return lista;
        }

        public PersonalAlmacen insertPersonalAlmacen(PersonalAlmacen obj)
        {
            var objCommand = GetSqlCommand("pi_personal_almacen");

            InputParameterAdd.Varchar(objCommand, "nombres", obj.nombres?.Trim());
            InputParameterAdd.Varchar(objCommand, "apellidoPaterno", obj.apellidoPaterno?.Trim());
            InputParameterAdd.Varchar(objCommand, "apellidoMaterno", obj.apellidoMaterno?.Trim());
            InputParameterAdd.Varchar(objCommand, "nroDocumento", obj.nroDocumento?.Trim());
            InputParameterAdd.Varchar(objCommand, "brevete", obj.brevete?.Trim());
            InputParameterAdd.Varchar(objCommand, "tipo", obj.tipo?.Trim());
            InputParameterAdd.Guid(objCommand, "idSedePrincipal", obj.sedePrincipal.idCiudad);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);

            OutputParameterAdd.Int(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            obj.idPersonalAlmacen = (int)objCommand.Parameters["@newId"].Value;

            return obj;
        }

        public PersonalAlmacen updatePersonalAlmacen(PersonalAlmacen obj)
        {
            var objCommand = GetSqlCommand("pu_personal_almacen");

            InputParameterAdd.Int(objCommand, "idPersonalAlmacen", obj.idPersonalAlmacen);
            InputParameterAdd.Varchar(objCommand, "nombres", obj.nombres?.Trim());
            InputParameterAdd.Varchar(objCommand, "apellidoPaterno", obj.apellidoPaterno?.Trim());
            InputParameterAdd.Varchar(objCommand, "apellidoMaterno", obj.apellidoMaterno?.Trim());
            InputParameterAdd.Varchar(objCommand, "nroDocumento", obj.nroDocumento?.Trim());
            InputParameterAdd.Varchar(objCommand, "brevete", obj.brevete?.Trim());
            InputParameterAdd.Varchar(objCommand, "tipo", obj.tipo?.Trim());
            InputParameterAdd.Guid(objCommand, "idSedePrincipal", obj.sedePrincipal.idCiudad);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);

            ExecuteNonQuery(objCommand);

            return obj;
        }
    }
}
