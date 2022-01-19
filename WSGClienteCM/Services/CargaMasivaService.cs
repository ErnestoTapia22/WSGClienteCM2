using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WSGClienteCM.Models;
using WSGClienteCM.Repository;

namespace WSGClienteCM.Services
{
    public class CargaMasivaService:ICargaMasivaService
    {
        private readonly ICargaMasivaRepository _cargaMasivaRepository;
        public CargaMasivaService(ICargaMasivaRepository cargaMasivaRepository)
        {
            this._cargaMasivaRepository = cargaMasivaRepository;
        }

        public async Task<ResponseViewModel> ValidarClienteMasivo(List<ClientBindingModel> request)
        {
            string processId = request[0].P_SNOPROCESO;
            ResponseViewModel responseViewModel = new ResponseViewModel();
            try
            {
                return await Task.FromResult(responseViewModel);
            }
            catch (Exception ex)
            {
                throw new WSGClienteCMException(Constants.MsgGetError, Convert.ToInt32(processId));
            }
        }
       public async Task<ResponseViewModel> InsertData(List<ClientBindingModel> request)
        {
            string processId = request[0].P_SNOPROCESO;
            ResponseViewModel responseViewModel = new ResponseViewModel();
            try
            {
                if (processId != null && processId != "")
                {
                    responseViewModel = await _cargaMasivaRepository.InsertHeader(request[0]);
                    if(responseViewModel.P_NCODE == "0")
                    {
                        responseViewModel = await _cargaMasivaRepository.InsertDetail(request);
                    }

                }
                else
                {
                    responseViewModel.P_SMESSAGE = Constants.MsgGetError;
                   
                }

             
            }
            catch (Exception ex)
            {
                throw new WSGClienteCMException(ex.Message);
            }
            return await Task.FromResult(responseViewModel);
        }
    }
}
