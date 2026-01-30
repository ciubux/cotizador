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
    public class VehiculoDAL : DaoBase
    {
        public VehiculoDAL(IDalSettings settings) : base(settings)
        {
        }

        public VehiculoDAL() : this(new CotizadorSettings())
        {
        }

        public Vehiculo getVehiculo(int idVehiculo)
        {
            var objCommand = GetSqlCommand("ps_vehiculo");
            InputParameterAdd.Int(objCommand, "idVehiculo", idVehiculo);
            DataTable dataTable = Execute(objCommand);
            Vehiculo obj = new Vehiculo();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idVehiculo = Converter.GetInt(row, "id_vehiculo");
                obj.placa = Converter.GetString(row, "placa");
                obj.marca = Converter.GetString(row, "marca");
                obj.modelo = Converter.GetString(row, "modelo");
                // Inicializamos el objeto ciudad para asignar su ID
                obj.ciudad = new Ciudad { idCiudad = Converter.GetGuid(row, "id_ciudad"), nombre = Converter.GetString(row, "nombre_ciudad") };
                obj.Estado = Converter.GetInt(row, "estado");
            }

            return obj;
        }

        public List<Vehiculo> getVehiculos(Vehiculo vehiculo)
        {
            var objCommand = GetSqlCommand("ps_vehiculos");
            InputParameterAdd.Int(objCommand, "estado", vehiculo.Estado);
            DataTable dataTable = Execute(objCommand);
            List<Vehiculo> lista = new List<Vehiculo>();

            foreach (DataRow row in dataTable.Rows)
            {
                Vehiculo obj = new Vehiculo();
                obj.idVehiculo = Converter.GetInt(row, "id_vehiculo");
                obj.placa = Converter.GetString(row, "placa");
                obj.marca = Converter.GetString(row, "marca");
                obj.modelo = Converter.GetString(row, "modelo");
                obj.ciudad = new Ciudad { idCiudad = Converter.GetGuid(row, "id_ciudad"), nombre = Converter.GetString(row, "nombre_ciudad") };
                obj.Estado = Converter.GetInt(row, "estado");
                lista.Add(obj);
            }

            return lista;
        }

        public Vehiculo insertVehiculo(Vehiculo obj)
        {
            var objCommand = GetSqlCommand("pi_vehiculo");

            InputParameterAdd.Varchar(objCommand, "placa", obj.placa?.Trim());
            InputParameterAdd.Varchar(objCommand, "marca", obj.marca?.Trim());
            InputParameterAdd.Varchar(objCommand, "modelo", obj.modelo?.Trim());
            InputParameterAdd.Guid(objCommand, "idCiudad", obj.ciudad.idCiudad);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);

            OutputParameterAdd.Int(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            obj.idVehiculo = (int)objCommand.Parameters["@newId"].Value;

            return obj;
        }

        public Vehiculo updateVehiculo(Vehiculo obj)
        {
            var objCommand = GetSqlCommand("pu_vehiculo");

            InputParameterAdd.Int(objCommand, "idVehiculo", obj.idVehiculo);
            InputParameterAdd.Varchar(objCommand, "placa", obj.placa?.Trim());
            InputParameterAdd.Varchar(objCommand, "marca", obj.marca?.Trim());
            InputParameterAdd.Varchar(objCommand, "modelo", obj.modelo?.Trim());
            InputParameterAdd.Guid(objCommand, "idCiudad", obj.ciudad.idCiudad);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);

            ExecuteNonQuery(objCommand);

            return obj;
        }
    }
}
