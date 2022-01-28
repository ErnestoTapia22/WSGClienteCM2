using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WSGClienteCM.Models
{
    public class Archivo
    {
        public string nombre { get; set; }
        public string tipoMIME { get; set; }
        public byte[] tramaEnviar64 { get; set; }
    }
}
