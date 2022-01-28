using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using WSGClienteCM.Connection;
using WSGClienteCM.Models;
using WSGClienteCM.Repository;
using System.Linq;

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

        public async Task<ResponseViewModel> InitProcess()
        {
            DbConnection DataConnection = _connectionBase.ConnectionGet(ConnectionBase.enuTypeDataBase.OracleVTime);
            DbTransaction trx = null;
            ResponseViewModel responseViewModel = new ResponseViewModel();
            try
            {
                // selecciona registros con estado 0
                responseViewModel = await _cargaMasivaRepository.GetStateCero();
                if (responseViewModel.P_NCODE == "0" && responseViewModel.ElistDetail.Count > 0)
                {
                    //selecciona los codigos para modificar estado a 1=en proceso
                    List<string> processCodeToUpdate = new List<string>();
                    processCodeToUpdate = responseViewModel.ElistDetail.GroupBy(x=>x.P_SNOPROCESO).Select(p=>p.First().P_SNOPROCESO).ToList();
                    DataConnection.Open();
                    trx = DataConnection.BeginTransaction();
                    ResponseViewModel res = await _cargaMasivaRepository.UpdateStateHeader(processCodeToUpdate,"1",DataConnection,trx);
                    if (res.P_NCODE == "0")
                    {

                        foreach (DetailBindingModel row in responseViewModel.ElistDetail) {
                            DetailBindingModel detailState = new DetailBindingModel();
                            // primera validacion tipo y numero de documento
                            if (row.P_NIDDOC_TYPE == "" || row.P_SIDDOC == "")
                            {
                                detailState.P_NIDDOC_TYPE = "No tiene el tipo de documento o el número de documento";
                                detailState.P_SIDDOC = "No tiene el tipo de documento o el número de documento";

                            }
                            else { 
                               
                             
                            
                            }
                           
                          
                        
                        }

                    }
                    else {
                        responseViewModel = res;
                        trx.Rollback(); 
                    
                    }

                }
                else {

                    responseViewModel.P_NCODE = "0";
                    responseViewModel.P_SMESSAGE = "No hay registros para procesar";
                }

               
            }
            catch (Exception ex)
            {
                if (trx != null) trx.Rollback();
                responseViewModel.P_NCODE = "0";
                responseViewModel.P_SMESSAGE = ex.Message;
            }
            finally
            {
                if (DataConnection.State == ConnectionState.Open)
                {
                    DataConnection.Close();
                }
                trx.Dispose();
            }
            return responseViewModel;
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

        public TramaRespuestaCargaMasivaResponse ObtenerTramaEnvioExitosa(string P_SNOPROCESO)
        {
            return _cargaMasivaRepository.ObtenerTramaEnvioExitosa(P_SNOPROCESO);
        }        
        public TramaRespuestaCargaMasivaResponse ObtenerTramaEnvioErrores(string P_SNOPROCESO)
        {
            return _cargaMasivaRepository.ObtenerTramaEnvioErrores(P_SNOPROCESO);
        }
        public TramaRespuestaCargaMasivaResponse ObtenerListaUsuariosEnvioTrama(string P_SNOPROCESO)
        {
            return _cargaMasivaRepository.ObtenerListaUsuariosEnvioTrama(P_SNOPROCESO);
        }

    }
}
