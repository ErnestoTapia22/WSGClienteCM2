using System.Collections.Generic;
using System.Threading.Tasks;
using WSGClienteCM.Models;

namespace WSGClienteCM.Services
{
    public interface ICargaMasivaService
    {
         Task<ResponseViewModel> ValidarClienteMasivo(List<ClientBindingModel> request);
        Task<ResponseViewModel> InsertData(List<ClientBindingModel> request);
    }
}
