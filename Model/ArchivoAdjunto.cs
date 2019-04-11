using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model
{
    public class ArchivoAdjunto 
    {
        public Guid idArchivoAdjunto { get; set; }
        public Usuario usuario { get; set; }
        public Byte[] adjunto { get; set; }
        public String nombre { get; set; }
        public long checksum { get; set; }
    }
}
