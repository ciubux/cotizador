using DataLayer.DALConverter;
using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;
using Model.DTO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class UbigeoDAL : DaoBase
    {
        public UbigeoDAL(IDalSettings settings) : base(settings)
        {
        }

        public UbigeoDAL() : this(new CotizadorSettings())
        {
        }

        public Ubigeo getUbigeoPorDistritoProvincia(String distrito, String provincia)
        {

            var objCommand = GetSqlCommand("ps_ubigeoPorDistritoYProvincia");
            InputParameterAdd.Varchar(objCommand, "distrito", distrito);
            InputParameterAdd.Varchar(objCommand, "provincia", provincia);
            DataTable dataTable = Execute(objCommand);
            Ubigeo obj = new Ubigeo();

            foreach (DataRow row in dataTable.Rows)
            {
                obj.Id = Converter.GetString(row, "codigo");
                obj.Provincia = Converter.GetString(row, "provincia");
                obj.Departamento = Converter.GetString(row, "departamento");
                obj.Distrito = Converter.GetString(row, "distrito");
            }

            return obj;

        }

        public List<UbigeoDTO> ObtenerUbigeos()
        {
            var objCommand = GetSqlCommand("ps_Ubigeo");
            //InputParameterAdd.Varchar(objCommand,"codDepartamento", codDepartamento);
            DataTable dataTableUbigeo = Execute(objCommand);

            List<UbigeoDTO> ubigeoDTODepartamentos = new List<UbigeoDTO>();
            UbigeoDTO ultimoDepartamento = new UbigeoDTO();
            ultimoDepartamento.codigo = "  ";
            UbigeoDTO ultimaProvincia = new UbigeoDTO();

            foreach (DataRow row in dataTableUbigeo.Rows)
            {

                String departamentoTmp = "";
                String provinciaTmp = "";
                String distritoTmp = "";

                UbigeoDTO ubigeoDTO = new UbigeoDTO
                {
                    codigo = Converter.GetString(row, "idUbigeo"),
                    nombre = Converter.GetString(row, "descripcion")
                };

                departamentoTmp = ubigeoDTO.codigo.Substring(0, 2);
                provinciaTmp = ubigeoDTO.codigo.Substring(2, 2);
                distritoTmp = ubigeoDTO.codigo.Substring(4, 2);

                //Si el departamento Cambia, la ubicacion se agrega como departamento
                if (!ultimoDepartamento.codigo.Equals(departamentoTmp))
                {
                    ultimoDepartamento = ubigeoDTO;
                    ultimoDepartamento.codigo = departamentoTmp;
                    ultimoDepartamento.tipo = 1;
                    //Se instancia la lista para las provincias
                    ultimoDepartamento.ubigeoDTOList = new List<UbigeoDTO>();
                    //Se agrega a la lista de departamentos
                    ubigeoDTODepartamentos.Add(ultimoDepartamento);
                    ultimaProvincia = new UbigeoDTO();
                    ultimaProvincia.codigo = "  ";
                }
                else if (!ultimaProvincia.codigo.Equals(provinciaTmp))
                {

                    ultimaProvincia = ubigeoDTO;
                    ultimaProvincia.codigo = provinciaTmp;
                    ultimaProvincia.tipo = 2;
                    //Se instancia la lista para los distritos
                    ultimaProvincia.ubigeoDTOList = new List<UbigeoDTO>();
                    //Se agrega a la lista de provincias
                    ultimoDepartamento.ubigeoDTOList.Add(ultimaProvincia);
                }
                else
                {
                    //Se agrega a la lista de distritos
                    ubigeoDTO.codigo = distritoTmp;
                    ubigeoDTO.tipo = 3;
                    ultimaProvincia.ubigeoDTOList.Add(ubigeoDTO);
                }
            }
            return ubigeoDTODepartamentos;
        }
       
        public List<Ubigeo> ObtenerDepartamentos()
        {
            var objCommand = GetSqlCommand("ps_Departamentos");

            return UbigeoConvertTo.Ubigeos(Execute(objCommand));
        }

        public List<Ubigeo> ObtenerProvincias(string idDepartamento)
        {
            var objCommand = GetSqlCommand("ps_Provincias");
            InputParameterAdd.Varchar(objCommand, "CodigoDepartamento", idDepartamento);

            return UbigeoConvertTo.Ubigeos(Execute(objCommand));
        }

        public List<Ubigeo> ObtenerDistritos(string idDepartamento, string idProvincia)
        {
            var objCommand = GetSqlCommand("ps_Distritos");
            InputParameterAdd.Varchar(objCommand, "CodigoProvincia", idProvincia);

            return UbigeoConvertTo.Ubigeos(Execute(objCommand));
        }
        /*
        public Ubigeo GetUbigeoById(string idUbigeo)
        {
            Ubigeo ubigeo = new Ubigeo();
            var objCommand = GetSqlCommand("pNLS_UbigeoById");
            InputParameterAdd.Varchar(objCommand, "idUbigeo", idUbigeo);
            OutputParameterAdd.Varchar(objCommand, "departamento", 500);
            OutputParameterAdd.Varchar(objCommand, "provincia", 500);
            OutputParameterAdd.Varchar(objCommand, "distrito", 500);
            ExecuteNonQuery(objCommand);
            ubigeo.Id = idUbigeo;
            ubigeo.Departamento = (string)objCommand.Parameters["@departamento"].Value;
            ubigeo.Provincia = (string)objCommand.Parameters["@provincia"].Value;
            ubigeo.Distrito = (string)objCommand.Parameters["@distrito"].Value;
            return ubigeo;
        }*/
    }
}

