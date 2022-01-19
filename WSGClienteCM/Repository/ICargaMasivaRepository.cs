using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using WSGClienteCM.Models;

namespace WSGClienteCM.Repository
{
    public interface ICargaMasivaRepository
    {
        ResponseViewModel SetTemporaryData(List<ClientBindingModel> request);
        ResponseViewModel testQuartz();
        Task<ResponseViewModel> InsertHeader(ClientBindingModel item , DbConnection cn, DbTransaction trx);
        Task<ResponseViewModel> InsertDetail(List<ClientBindingModel> items, DbConnection cn, DbTransaction trx);


    }
}
