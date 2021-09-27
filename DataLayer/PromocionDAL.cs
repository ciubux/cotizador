using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class PromocionDAL : DaoBase
    {
        public PromocionDAL(IDalSettings settings) : base(settings)
        {
        }

        public PromocionDAL() : this(new CotizadorSettings())
        {
        }



        public Promocion getPromocion(Guid idPromocion)
        {
            var objCommand = GetSqlCommand("ps_promocion");
            InputParameterAdd.Guid(objCommand, "idPromocion", idPromocion);
            DataTable dataTable = Execute(objCommand);
            Promocion obj = new Promocion();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.idPromocion = Converter.GetGuid(row, "id_promocion");
                obj.codigo = Converter.GetString(row, "codigo");
                obj.titulo = Converter.GetString(row, "titulo");
                obj.descripcion = Converter.GetString(row, "descripcion");
                obj.descripcionPresentacion = Converter.GetString(row, "descripcion_presentacion");
                obj.Estado = Converter.GetInt(row, "estado");
                obj.fechaInicio = Converter.GetDateTime(row, "fecha_inicio");
                obj.fechaFin = Converter.GetDateTime(row, "fecha_fin");


            }

            return obj;
        }


        public List<Promocion> getPromociones(Promocion obj)
        {
            var objCommand = GetSqlCommand("ps_promociones");
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            DataTable dataTable = Execute(objCommand);
            List<Promocion> lista = new List<Promocion>();

            foreach (DataRow row in dataTable.Rows)
            {
                Promocion item = new Promocion();
                item.idPromocion = Converter.GetGuid(row, "id_promocion");
                item.codigo = Converter.GetString(row, "codigo");
                item.titulo = Converter.GetString(row, "titulo");
                item.Estado = Converter.GetInt(row, "estado");
                item.fechaInicio = Converter.GetDateTime(row, "fecha_inicio");
                item.fechaFin = Converter.GetDateTime(row, "fecha_fin");

                lista.Add(item);
            }

            return lista;
        }


        public Promocion insertPromocion(Promocion obj)
        {
            var objCommand = GetSqlCommand("pi_promocion");

            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Varchar(objCommand, "titulo", obj.titulo);
            InputParameterAdd.Varchar(objCommand, "descripcion", obj.descripcion);
            InputParameterAdd.Varchar(objCommand, "descripcionPresentacion", obj.descripcionPresentacion);
            InputParameterAdd.DateTime(objCommand, "fechaInicio", obj.fechaInicio);
            InputParameterAdd.DateTime(objCommand, "fechaFin", obj.fechaFin);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            OutputParameterAdd.UniqueIdentifier(objCommand, "newId");

            ExecuteNonQuery(objCommand);

            obj.idPromocion = (Guid)objCommand.Parameters["@newId"].Value;

            return obj;

        }


        public Promocion updatePromocion(Promocion obj)
        {
            var objCommand = GetSqlCommand("pu_promocion");
            InputParameterAdd.Guid(objCommand, "idPromocion", obj.idPromocion);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Varchar(objCommand, "titulo", obj.titulo);
            InputParameterAdd.Varchar(objCommand, "descripcion", obj.descripcion);
            InputParameterAdd.Varchar(objCommand, "descripcionPresentacion", obj.descripcionPresentacion);
            InputParameterAdd.DateTime(objCommand, "fechaInicio", obj.fechaInicio);
            InputParameterAdd.DateTime(objCommand, "fechaFin", obj.fechaFin);
            InputParameterAdd.Guid(objCommand, "idUsuario", obj.IdUsuarioRegistro);
            InputParameterAdd.Int(objCommand, "estado", obj.Estado);

            ExecuteNonQuery(objCommand);

            return obj;

        }
    }
}
