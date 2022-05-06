using System.Collections.Generic;
using System.Threading.Tasks;
using WSGClienteCM.Models;

namespace WSGClienteCM.Services
{
    public interface ICargaMasivaService
    {
        Task<ResponseViewModel> InitProcess();
        Task<ResponseViewModel> InsertData(List<ClientBindingModel> request);
        Task<RespuestaMail> SendEmails(string snroprocess);

        Task<string> PostRequest(string url, object postObject, string token = null);
        Task<ResponseViewModel> updateJiraState(WebHookPayloadModel model);
        Task<ResponseViewModel> updateJiraStateSGC(WebHookPayloadModel model);
        //hcama@mg 26.01.2021 ini 
        //TramaRespuestaCargaMasivaResponse ObtenerTramaEnvioExitosa(string P_SNOPROCESO);
        //TramaRespuestaCargaMasivaResponse ObtenerTramaEnvioErrores(string P_SNOPROCESO);
        //TramaRespuestaCargaMasivaResponse ObtenerListaUsuariosEnvioTrama(string P_SNOPROCESO);
        //hcama@mg 26.01.2021 fin 
    }
}
