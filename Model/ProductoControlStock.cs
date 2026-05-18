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
                decimal cantidadCalc = 0;

                switch (this.idProductoPresentacion)
                {
                    case 0: cantidadCalc = ((decimal)this.cantidadCs) / ((decimal)producto.equivalenciaUnidadEstandarUnidadConteo); break;
                    case 1: cantidadCalc = ((decimal)this.cantidadCs) / ((decimal)producto.equivalenciaUnidadAlternativaUnidadConteo); break;
                    case 2: cantidadCalc = ((decimal)this.cantidadCs) / ((decimal)producto.equivalenciaUnidadProveedorUnidadConteo); break;
                    case 3: cantidadCalc = (decimal)this.cantidadCs; break;
                }

                return cantidadCalc;
            }
        }

        public decimal cantidadAtenderPresentacion
        {
            get
            {
                decimal cantidadCalc = 0;

                switch (this.idProductoPresentacion)
                {
                    case 0: cantidadCalc = ((decimal)this.cantidadAtenderCs) / ((decimal)producto.equivalenciaUnidadEstandarUnidadConteo); break;
                    case 1: cantidadCalc = ((decimal)this.cantidadAtenderCs) / ((decimal)producto.equivalenciaUnidadAlternativaUnidadConteo); break;
                    case 2: cantidadCalc = ((decimal)this.cantidadAtenderCs) / ((decimal)producto.equivalenciaUnidadProveedorUnidadConteo); break;
                    case 3: cantidadCalc = (decimal)this.cantidadAtenderCs; break;
                }

                return cantidadCalc;
            }
        }

        public decimal cantidadRecibirPresentacion
        {
            get
            {
                decimal cantidadCalc = 0;

                switch (this.idProductoPresentacion)
                {
                    case 0: cantidadCalc = ((decimal)this.cantidadRecibirCs) / ((decimal)producto.equivalenciaUnidadEstandarUnidadConteo); break;
                    case 1: cantidadCalc = ((decimal)this.cantidadRecibirCs) / ((decimal)producto.equivalenciaUnidadAlternativaUnidadConteo); break;
                    case 2: cantidadCalc = ((decimal)this.cantidadRecibirCs) / ((decimal)producto.equivalenciaUnidadProveedorUnidadConteo); break;
                    case 3: cantidadCalc = (decimal)this.cantidadRecibirCs; break;
                }

                return cantidadCalc;
            }
        }

        public decimal cantidadVirtualPresentacion
        {
            get
            {
                decimal cantidadCalc = 0;

                switch (this.idProductoPresentacion)
                {
                    case 0: cantidadCalc = ((decimal)this.cantidadVirtualCs) / ((decimal)producto.equivalenciaUnidadEstandarUnidadConteo); break;
                    case 1: cantidadCalc = ((decimal)this.cantidadVirtualCs) / ((decimal)producto.equivalenciaUnidadAlternativaUnidadConteo); break;
                    case 2: cantidadCalc = ((decimal)this.cantidadVirtualCs) / ((decimal)producto.equivalenciaUnidadProveedorUnidadConteo); break;
                    case 3: cantidadCalc = (decimal)this.cantidadVirtualCs; break;
                }

                return cantidadCalc;
            }
        }
    }
}
