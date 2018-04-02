using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public interface IDocumento
    {

        Decimal montoTotal
        {
            get;
            set;
        }
        Decimal montoSubTotal
        {
            get;
            set;
        }
        Decimal montoIGV
        {
            get;
            set;
        }
        Boolean incluidoIGV
        {
            get;
            set;
        }
        List<IDocumentoDetalle> documentoDetalle
        {
            get;
            set;
        }

        Ciudad ciudad
        {
            get;
            set;
        }

        Cliente cliente
        {
            get;
            set;
        }
    }
}
