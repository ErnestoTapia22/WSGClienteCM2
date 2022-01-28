using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WSGClienteCM.Models
{
    public class TramaRespuestaCargaMasivaResponse : CommonResponse
    {
        public List<ClientBindingModel> tramaExitosa { get; set; }
        public List<ClientBindingModel> tramaErrores { get; set; }

        public List<EmailViewModel> correoUsuarios { get; set; }

        public string codigoRespuesta { get; set; }
        public string mensajeRespuesta { get; set; }

        public TramaRespuestaCargaMasivaResponse()
        {
            mensajes = new List<string>();
            tramaExitosa = new List<ClientBindingModel>();
            tramaErrores = new List<ClientBindingModel>();
            correoUsuarios = new List<EmailViewModel>();
        }
    }
}
