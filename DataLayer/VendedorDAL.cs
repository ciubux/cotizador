using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework.DAL;
using Framework.DAL.Settings.Implementations;
using Model;

namespace DataLayer
{
    public class VendedorDAL : DaoBase
    {

        public VendedorDAL(IDalSettings settings) : base(settings)
        {
        }
        public VendedorDAL() : this(new CotizadorSettings())
        {
        }


        public List<Vendedor> getVendedores(Vendedor vendedor)
        {
            var objCommand = GetSqlCommand("ps_vendedores");
            List<Vendedor> lista = new List<Vendedor>();
            
            InputParameterAdd.Int(objCommand, "estado", vendedor.estado);
                InputParameterAdd.VarcharEmpty(objCommand, "cod",vendedor.codigo);
                InputParameterAdd.VarcharEmpty(objCommand, "descripcion", vendedor.descripcion);
                InputParameterAdd.VarcharEmpty(objCommand, "email",vendedor.email);
                DataTable dataTable = Execute(objCommand);
                foreach (DataRow row in dataTable.Rows)
                {
                Vendedor obj = new Vendedor();
                
                    obj.idVendedor = Converter.GetInt(row, "id_vendedor");
                    obj.descripcion = Converter.GetString(row, "descripcion");
                    obj.codigo = Converter.GetString(row, "codigo");
                    obj.cargo = Converter.GetString(row, "cargo");
                    obj.email = Converter.GetString(row, "email");
                    obj.estado = Converter.GetInt(row, "estado");
                    lista.Add(obj);
                }    
            return lista;
        }


        public Vendedor getVendedor(int idVendedor)
        {
            var objCommand = GetSqlCommand("ps_detalle_vendedores");
            InputParameterAdd.Int(objCommand, "idUsuarioVenta", idVendedor);
            DataTable dataTable = Execute(objCommand);
            Vendedor obj = new Vendedor();
           
            foreach (DataRow row in dataTable.Rows)
            {
                
                obj.idVendedor = Converter.GetInt(row, "id_vendedor");
                obj.descripcion = Converter.GetString(row, "nombre");
                obj.codigo = Converter.GetString(row, "codigo");                
                obj.email = Converter.GetString(row, "email");
                obj.estado = Converter.GetInt(row, "estado");                
                obj.cargo = Converter.GetString(row, "cargo");
                obj.contacto = Converter.GetString(row, "contacto");                
                obj.idCiudad = Converter.GetGuid(row, "id_ciudad");                
                obj.maxdesapro = Converter.GetDecimal(row, "maximo_porcentaje_descuento_aprobacion");
                obj.pass = Converter.GetString(row, "pass");

            }

            return obj;
        }

        public Vendedor insertVendedor(Vendedor obj)
        {
             
            var objCommand = GetSqlCommand("pi_vendedor");
            InputParameterAdd.Guid(objCommand, "usuario_creacion", obj.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.descripcion);
            InputParameterAdd.Int(objCommand, "estado", obj.estado);
            InputParameterAdd.Varchar(objCommand, "email", obj.email);            
            InputParameterAdd.VarcharEmpty(objCommand, "pass", obj.pass);                       
            InputParameterAdd.Guid(objCommand, "id_ciudad", obj.idCiudad);
            InputParameterAdd.Varchar(objCommand, "cargo", obj.cargo);
            InputParameterAdd.Varchar(objCommand, "contacto", obj.contacto);
            InputParameterAdd.Decimal(objCommand, "maximo_descuento", obj.maxdesapro);

            //OutputParameterAdd.UniqueIdentifier(objCommand, "newid");
            InputParameterAdd.Guid(objCommand, "newid",null);
            ExecuteNonQuery(objCommand);
           

            return obj;

        }


        public Vendedor updateVendedor(Vendedor obj)
        {
            var objCommand = GetSqlCommand("pu_vendedores");
            InputParameterAdd.Int(objCommand, "id_vendedor", obj.idVendedor);
            InputParameterAdd.Guid(objCommand, "usuario_modificacion", obj.usuario.idUsuario);
            InputParameterAdd.Varchar(objCommand, "codigo", obj.codigo);
            InputParameterAdd.Varchar(objCommand, "email", obj.email);            
            InputParameterAdd.Varchar(objCommand, "Newpassword", obj.pass);                        
            InputParameterAdd.Varchar(objCommand, "cargo", obj.cargo);
            InputParameterAdd.Varchar(objCommand, "nombre", obj.descripcion);
            InputParameterAdd.Varchar(objCommand, "contacto", obj.contacto);
            InputParameterAdd.Decimal(objCommand, "max_por_des_apro", obj.maxdesapro);
            InputParameterAdd.Int(objCommand, "estado", obj.estado);
            InputParameterAdd.Guid(objCommand, "id_ciudad", obj.idCiudad);

            ExecuteNonQuery(objCommand);

            return obj;

        }

    }
}
