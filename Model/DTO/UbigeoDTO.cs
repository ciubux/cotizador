using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.DTO
{
    public class UbigeoDTO
    {
        public string codigo { get; set; }
        public string nombre { get; set; }

        //1 = Departamento
        //2 = Provincia
        //3 = Distrito
        public int tipo { get; set; }
        public List<UbigeoDTO> ubigeoDTOList { get; set; }

    }
}
