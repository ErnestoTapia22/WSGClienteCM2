using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WSGClienteCM.Models
{
    public class TicketFields
    {
        public string DFIELD { get; set; } //customfield DFECHA

        public string IDSTATUS { get; set; } // ESTADO

        public string SDEAREA { get; set; } //customfield NOMBRE DE AREA DERIVADA

        public string DENVIO { get; set; } // FLAGO SI HAY ENVIO

        public string SATDATE { get; set; } //DIAS ATENDIDOS

        public string SOBS_FLAG { get; set; }//FLAGO SI SE UTILIZA METODO OBS
    }
}
