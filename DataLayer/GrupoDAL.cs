using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using System;
using System.Collections.Generic;
using System.Data;
using Model;

namespace DataLayer
{
    public class GrupoClienteDAL : DaoBase
    {
        public GrupoClienteDAL(IDalSettings settings) : base(settings)
        {
        }

        public GrupoClienteDAL() : this(new CotizadorSettings())
        {
        }

    


        public List<GrupoCliente> getGruposBusqueda(String textoBusqueda)
        {
            var objCommand = GetSqlCommand("ps_getgrupos_search");
            InputParameterAdd.Varchar(objCommand, "textoBusqueda", textoBusqueda);
            DataTable dataTable = Execute(objCommand);
            List<GrupoCliente> grupoList = new List<GrupoCliente>();

            foreach (DataRow row in dataTable.Rows)
            {
                GrupoCliente grupo = new GrupoCliente
                {
                    idGrupoCliente = Converter.GetInt(row, "id_grupo"),
                    codigo = Converter.GetString(row, "codigo"),
                    nombre = Converter.GetString(row, "nombre"),
                };
                grupoList.Add(grupo);
            }
            return grupoList;
        }

        public GrupoCliente getGrupo(int idGrupoCliente)
        {
            var objCommand = GetSqlCommand("ps_grupoCliente");
            InputParameterAdd.Int(objCommand, "idGrupoCliente", idGrupoCliente);
            DataTable dataTable = Execute(objCommand);
            GrupoCliente grupoCliente = new GrupoCliente();

            foreach (DataRow row in dataTable.Rows)
            {
                grupoCliente.idGrupoCliente = Converter.GetInt(row, "id_grupo_cliente");
                grupoCliente.codigo = Converter.GetString(row, "codigo");
                grupoCliente.nombre = Converter.GetString(row, "nombre");

                grupoCliente.ciudad = new Ciudad();
                grupoCliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                grupoCliente.plazoCreditoSolicitado = (DocumentoVenta.TipoPago)Converter.GetInt(row, "plazo_credito_solicitado");
            }

            return grupoCliente;
        }


        public List<GrupoCliente> getGruposCliente()
        {
            var objCommand = GetSqlCommand("ps_gruposCliente");
            //InputParameterAdd.Guid(objCommand, "idGrupo", idGrupo);
            DataTable dataTable = Execute(objCommand);
            List<GrupoCliente> grupoClienteList = new List<GrupoCliente>();

            foreach (DataRow row in dataTable.Rows)
            {
                GrupoCliente grupoCliente = new GrupoCliente();
                grupoCliente.idGrupoCliente = Converter.GetInt(row, "id_grupo_cliente");
                grupoCliente.codigo = Converter.GetString(row, "codigo");
                grupoCliente.nombre = Converter.GetString(row, "nombre");

                grupoCliente.ciudad = new Ciudad();
                grupoCliente.ciudad.idCiudad = Converter.GetGuid(row, "id_ciudad");
                grupoCliente.plazoCreditoSolicitado = (DocumentoVenta.TipoPago)Converter.GetInt(row, "plazo_credito_solicitado");
                grupoCliente.plazoCreditoAprobado = (DocumentoVenta.TipoPago)Converter.GetInt(row, "plazo_credito_aprobado");

                grupoClienteList.Add(grupoCliente);
            }

            return grupoClienteList;
        }

        public GrupoCliente insertGrupoCliente(GrupoCliente grupoCliente)
        {
            var objCommand = GetSqlCommand("pi_grupoCliente");

            InputParameterAdd.Guid(objCommand, "idUsuario", grupoCliente.IdUsuarioRegistro);
            InputParameterAdd.Varchar(objCommand, "nombre", grupoCliente.nombre);
            InputParameterAdd.Varchar(objCommand, "contacto", grupoCliente.contacto);
            InputParameterAdd.Varchar(objCommand, "telefonoContacto", grupoCliente.telefonoContacto);
            InputParameterAdd.Varchar(objCommand, "emailContacto", grupoCliente.emailContacto);
            InputParameterAdd.Guid(objCommand, "idCiudad", grupoCliente.ciudad.idCiudad);

            /*Plazo credito*/
            InputParameterAdd.Int(objCommand, "plazoCreditoSolicitado", (int)grupoCliente.plazoCreditoSolicitado);
            InputParameterAdd.Int(objCommand, "plazoCreditoAprobado", (int)grupoCliente.plazoCreditoAprobado);
            InputParameterAdd.Int(objCommand, "sobrePlazo", grupoCliente.sobrePlazo);
            /*Monto Crédito*/
            InputParameterAdd.Decimal(objCommand, "creditoSolicitado", grupoCliente.creditoSolicitado);
            InputParameterAdd.Decimal(objCommand, "creditoAprobado", grupoCliente.creditoAprobado);
            InputParameterAdd.Decimal(objCommand, "sobreGiro", grupoCliente.sobreGiro);
      
            InputParameterAdd.Varchar(objCommand, "observacionesCredito", grupoCliente.observacionesCredito);
            InputParameterAdd.Varchar(objCommand, "observaciones", grupoCliente.observaciones);

            InputParameterAdd.Varchar(objCommand, "codigo", grupoCliente.codigo);

            OutputParameterAdd.Int(objCommand, "newId");
            //OutputParameterAdd.Varchar(objCommand, "codigo", 4);

            ExecuteNonQuery(objCommand);

            grupoCliente.idGrupoCliente = (int)objCommand.Parameters["@newId"].Value;


            foreach (GrupoClienteAdjunto grupoClienteAdjunto in grupoCliente.grupoClienteAdjuntoList)
            {
                grupoClienteAdjunto.idGrupoCliente = grupoCliente.idGrupoCliente;
                grupoClienteAdjunto.usuario = grupoCliente.usuario;
                this.InsertGrupoClienteAdjunto(grupoClienteAdjunto);
            }
            
            return grupoCliente;

        }

        public void InsertGrupoClienteAdjunto(GrupoClienteAdjunto grupoClienteAdjunto)
        {
            var objCommand = GetSqlCommand("pi_grupoClienteAdjunto");
            InputParameterAdd.Int(objCommand, "idGrupoCliente", grupoClienteAdjunto.idGrupoCliente);
            InputParameterAdd.Varchar(objCommand, "nombre", grupoClienteAdjunto.nombre);
            InputParameterAdd.VarBinary(objCommand, "adjunto", grupoClienteAdjunto.adjunto);

            InputParameterAdd.Guid(objCommand, "idUsuario", grupoClienteAdjunto.usuario.idUsuario);
            OutputParameterAdd.UniqueIdentifier(objCommand, "idArchivoAdjunto");
            ExecuteNonQuery(objCommand);

            grupoClienteAdjunto.idArchivoAdjunto = (Guid)objCommand.Parameters["@idArchivoAdjunto"].Value;
        }
    }
}
