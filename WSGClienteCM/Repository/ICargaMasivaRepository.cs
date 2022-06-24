using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using WSGClienteCM.Models;

namespace WSGClienteCM.Repository
{
    public interface ICargaMasivaRepository
    {

        ResponseViewModel testQuartz();
        Task<ResponseViewModel> InsertHeader(ClientBindingModel item, DbConnection cn, DbTransaction trx);
        Task<ResponseViewModel> InsertDetail(List<ClientBindingModel> items, DbConnection cn, DbTransaction trx);
        Task<ResponseViewModel> GetByState(string state);
        Task<ResponseViewModel> UpdateStateHeader(List<string> processCodeToUpdate, string value);
        Task<ResponseViewModel> ValidateRow(DetailBindingModel item);
        Task<ResponseViewModel> SaveStateRow(DetailBindingModel detailStateParsed);
        Task<ResponseViewModel> UpdateStateResponse(int nid, string value, int state, string sorigendatos = null, string sclient = "sclient");
        Task<ResponseViewModel> GetHeadersByState(string state);
        //hcama@mg 26.01.2021 ini 
        Task<TramaRespuestaCargaMasivaResponse> ObtenerTramaEnvioExitosa(string P_SNOPROCESO);
        Task<TramaRespuestaCargaMasivaResponse> ObtenerTramaEnvioErrores(string P_SNOPROCESO);
        Task<TramaRespuestaCargaMasivaResponse> ObtenerListaUsuariosEnvioTrama(string P_SNOPROCESO);
        //hcama@mg 26.01.2021 fin 
        Task<ResponseViewModel> updateState(string jiraCode, string jiraStatus, string derivationArea, string denvio, string dateFired, string attendedDate);
        Task<ResponseViewModel> updateStateObservation(string jiraCode, string jiraStatus);
        Task<ResponseViewModel> updateState(string jiraCode, string jiraStatus);

        //DEVCY 11-04-22 ini
        Task<TicketState> GetTicketState(string code);
        //DEVCY 11-04-22 fin

        //DEV CY --INI
        Task<ResponseViewModel> ValidateStateJira(string value);

        Task<TicketFields> GetTicketFields(string code, string state);

        Task<ResponseViewModel> updateStateAllTickets(string jiraCode, string jiraStatus, string derivationArea, string denvio, string scomment);
        //DEV CY --FIN
    }
}
