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
        Task<ResponseViewModel> UpdateStateHeader(List<string> processCodeToUpdate, string value, DbConnection cn, DbTransaction trx);
        Task<ResponseViewModel> ValidateRow(DetailBindingModel item, DbConnection cn, DbTransaction trx);
        Task<ResponseViewModel> SaveStateRow(DetailBindingModel detailStateParsed, DbConnection dataConnection, DbTransaction trx);
        Task<ResponseViewModel> UpdateStateResponse(int nid, string value, int state, DbConnection cn, DbTransaction trx);
        //hcama@mg 26.01.2021 ini 
        Task<TramaRespuestaCargaMasivaResponse> ObtenerTramaEnvioExitosa(string P_SNOPROCESO);
        Task<TramaRespuestaCargaMasivaResponse> ObtenerTramaEnvioErrores(string P_SNOPROCESO);
        Task<TramaRespuestaCargaMasivaResponse> ObtenerListaUsuariosEnvioTrama(string P_SNOPROCESO);
        //hcama@mg 26.01.2021 fin 
    }
}
