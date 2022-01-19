using System.Collections.Generic;
using System.Threading.Tasks;
using WSGClienteCM.Models;

namespace WSGClienteCM.Repository
{
    public interface ICargaMasivaRepository
    {
        ResponseViewModel SetTemporaryData(List<ClientBindingModel> request);
        ResponseViewModel testQuartz();
        Task<ResponseViewModel> InsertHeader(ClientBindingModel item);
        Task<ResponseViewModel> InsertDetail(List<ClientBindingModel> items);

    }
}
