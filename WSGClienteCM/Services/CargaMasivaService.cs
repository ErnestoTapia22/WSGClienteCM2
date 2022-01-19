using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using WSGClienteCM.Connection;
using WSGClienteCM.Models;
using WSGClienteCM.Repository;

namespace WSGClienteCM.Services
{
    public class CargaMasivaService:ICargaMasivaService
    {
        private readonly ICargaMasivaRepository _cargaMasivaRepository;
        private readonly IConnectionBase _connectionBase;
        public CargaMasivaService(ICargaMasivaRepository cargaMasivaRepository, IConnectionBase connectionBase )
        {
            this._cargaMasivaRepository = cargaMasivaRepository;
            this._connectionBase = connectionBase;
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
            DbConnection DataConnection = _connectionBase.ConnectionGet(ConnectionBase.enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            try
            {
                if (processId != null && processId != "")

                {
                    DataConnection.Open();
                    trx = DataConnection.BeginTransaction();
                    responseViewModel = await _cargaMasivaRepository.InsertHeader(request[0], DataConnection, trx);
                    if (responseViewModel.P_NCODE == "0")
                    {

                        responseViewModel = await _cargaMasivaRepository.InsertDetail(request, DataConnection, trx);

                        if (responseViewModel.P_NCODE == "0")
                        {
                            responseViewModel.P_SMESSAGE = "Se registró correctamente la trama";
                            trx.Commit();
                        }
                        else
                        {

                            trx.Rollback();
                        }
                    }
                    else {

                        trx.Rollback();
                    }

                }
                else
                {
                    responseViewModel.P_SMESSAGE = Constants.MsgGetError;
                   
                }

             
            }
            catch (Exception ex)
            {
                if (trx != null) trx.Rollback();
               
                throw new WSGClienteCMException(ex.Message);
            }
            finally
            {
                if (DataConnection.State == ConnectionState.Open) {
                    DataConnection.Close();
                }
                trx.Dispose();
            }
            return responseViewModel;
        }
    }
}
