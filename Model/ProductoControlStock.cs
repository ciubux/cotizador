using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ProductoControlStock : Auditoria
    {
        public Guid idProductoControlStock { get; set; } 
        public Empresa empresa { get; set; }
        public Producto producto { get; set; }
        public Ciudad ciudad { get; set; }

        public int cantidadMinima { get; set; }
        public int cantidadMaxima { get; set; }
        public int cantidadAlerta { get; set; }

        public DateTime fechaCs { get; set; }
        public int cantidadCs { get; set; }
        public int cantidadAtenderCs { get; set; }
        public int cantidadRecibirCs { get; set; }

        public int cantidadVirtualCs { get { return this.cantidadCs + this.cantidadRecibirCs - this.cantidadAtenderCs; } }
        public int cantidadSugeridaPedirCs { get { return this.cantidadVirtualCs > this.cantidadMaxima ? 0 : this.cantidadMaxima - this.cantidadVirtualCs; } }


        public string unidadCs { get; set; }

        public string FechaCsDesc
        {
            get { return fechaCs.ToString("dd/MM/yyyy HH:mm:ss"); }
        }

        public int idProductoPresentacion { get; set; }

        public string unidadPresentacion
        {
            get 
            {  
                string unidad = string.Empty;
                switch (this.idProductoPresentacion)
                {
                    case 0: unidad = producto.unidad; break;
                    case 1: unidad = producto.unidad_alternativa; break;
                    case 2: unidad = producto.unidadProveedor; break;
                    case 3: unidad = unidadCs; break;
                }

                return unidad;
            }
        }

        public decimal cantidadPresentacion
        {
            get
            {
                return ConvertirCantidadPresentacion(this.cantidadCs);
            }
        }

        public decimal cantidadAtenderPresentacion
        {
            get
            {
                return ConvertirCantidadPresentacion(this.cantidadAtenderCs);
            }
        }

        public decimal cantidadRecibirPresentacion
        {
            get
            {
                return ConvertirCantidadPresentacion(this.cantidadRecibirCs);
            }
        }

        public decimal cantidadVirtualPresentacion
        {
            get
            {
                return ConvertirCantidadPresentacion(this.cantidadVirtualCs);
            }
        }

        public decimal cantidadMinimaPresentacion
        {
            get
            {
                return ConvertirCantidadPresentacion(this.cantidadMinima);
            }
        }

        public decimal cantidadMaximaPresentacion
        {
            get
            {
                return ConvertirCantidadPresentacion(this.cantidadMaxima);
            }
        }

        public decimal cantidadAlertaPresentacion
        {
            get
            {
                return ConvertirCantidadPresentacion(this.cantidadAlerta);
            }
        }

        public decimal cantidadSugeridaPedirPresentacion
        {
            get
            {
                return ConvertirCantidadPresentacion(this.cantidadSugeridaPedirCs);
            }
        }

        public string estadoStockReal
        {
            get
            {
                string estado = "cubierto";
                
                if (this.cantidadCs <= this.cantidadMinima)
                {
                    estado = "critico";
                }
                else if(this.cantidadCs <= this.cantidadAlerta)
                {
                    estado = "alerta";
                }

                return estado;
            }
        }

        public string estadoStockVirtual
        {
            get
            {
                string estado = "cubierto";

                if (this.cantidadVirtualCs <= this.cantidadMinima)
                {
                    estado = "critico";
                }
                else if (this.cantidadVirtualCs <= this.cantidadAlerta)
                {
                    estado = "alerta";
                }

                return estado;
            }
        }

        private decimal ConvertirCantidadPresentacion(decimal cantidadOriginal)
        {
            decimal cantidadCalc = 0;

            switch (this.idProductoPresentacion)
            {
                case 0: cantidadCalc = ((decimal)cantidadOriginal) / ((decimal)producto.equivalenciaUnidadEstandarUnidadConteo); break;
                case 1: cantidadCalc = ((decimal)cantidadOriginal) / ((decimal)producto.equivalenciaUnidadAlternativaUnidadConteo); break;
                case 2: cantidadCalc = ((decimal)cantidadOriginal) / ((decimal)producto.equivalenciaUnidadProveedorUnidadConteo); break;
                case 3: cantidadCalc = (decimal)cantidadOriginal; break;
            }

            return cantidadCalc;
        }
    }
}
