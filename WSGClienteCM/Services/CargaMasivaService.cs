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
            responseViewModel.EListErrores = new List<ListViewErrores>();

            try
            {
                // selecciona registros con estado 0
                responseViewModel = await _cargaMasivaRepository.GetStateCero();
                if (responseViewModel.P_COD_ERR == "0" && responseViewModel.ElistDetail.Count > 0)
                {
                    //selecciona los codigos para modificar estado a 1=en proceso
                    List<string> processCodeToUpdate = new List<string>();
                    processCodeToUpdate = responseViewModel.ElistDetail.GroupBy(x=>x.SNROPROCESO_CAB).Select(p=>p.First().SNROPROCESO_CAB).ToList();
                    DataConnection.Open();
                    trx = DataConnection.BeginTransaction();
                    ResponseViewModel res = await _cargaMasivaRepository.UpdateStateHeader(processCodeToUpdate,"1",DataConnection,trx);
                    if (res.P_COD_ERR == "0")
                    {

                        foreach (DetailBindingModel row in responseViewModel.ElistDetail) {
                            DetailBindingModel detailState = new DetailBindingModel();
                            ResponseViewModel resval = new ResponseViewModel();
                            // primera validacion tipo y numero de documento
                            if (row.NIDDOC_TYPE == "" || row.SIDDOC == "")// validar bien paso tipo doc =0
                            {
                                detailState.NIDDOC_TYPE = "No tiene el tipo de documento o el número de documento";
                                detailState.SIDDOC = "No tiene el tipo de documento o el número de documento";

                            }
                            else {
                                // validacion
                                resval = await _cargaMasivaRepository.ValidateRow(row, DataConnection, trx);

                                if (resval.EListErrores.Count > 0)
                                {
                                    DetailBindingModel detailStateParsed = new DetailBindingModel();
                                    ResponseViewModel resInsertState = new ResponseViewModel();
                                    ListViewErrores error = new ListViewErrores();
                                    detailStateParsed = ParseErrorToModel(resval.EListErrores);
                                    detailStateParsed.NNROPROCESO_DET = row.NNROPROCESO_DET;
                                    resInsertState = await _cargaMasivaRepository.SaveStateRow(detailStateParsed, DataConnection, trx);
                                    if (resInsertState.P_COD_ERR != "0") {
                                        error.SMENSAJE = "No se puedo insertar el estado del registro con id: " + row.NNROPROCESO_DET + "_" + resInsertState.P_MESSAGE;
                                        error.SGRUPO = "GES_CAR_MAS_CLI_DET_STATE";
                                        responseViewModel.EListErrores.Add(error);
                                    }
                                }
                                else { 
                                   //update

                                }
                            }
                           
                          
                        
                        }

                    }
                    else {
                        responseViewModel = res;
                        trx.Rollback(); 
                    
                    }

                }
                else {

                    responseViewModel.P_COD_ERR = "0";
                    responseViewModel.P_MESSAGE = "No hay registros para procesar";
                }

               
            }
            catch (Exception ex)
            {
                if (trx != null) trx.Rollback();
                responseViewModel.P_COD_ERR = "0";
                responseViewModel.P_MESSAGE = ex.Message;
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
                    if (responseViewModel.P_COD_ERR == "0")
                    {

                        responseViewModel = await _cargaMasivaRepository.InsertDetail(request, DataConnection, trx);

                        if (responseViewModel.P_COD_ERR == "0")
                        {
                            responseViewModel.P_MESSAGE = "Se registró correctamente la trama";
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
                    responseViewModel.P_MESSAGE = Constants.MsgGetError;
                   
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

        private DetailBindingModel ParseErrorToModel(List<ListViewErrores> items) { 
            DetailBindingModel model = new DetailBindingModel();
            foreach (ListViewErrores item in items)
            {
                if (model.GetType().GetProperty(item.SCAMPO) != null) {
                    model.GetType().GetProperty(item.SCAMPO).SetValue(model, item.SMENSAJE, null);
                };
            }
            return model;
        }
    }
}
