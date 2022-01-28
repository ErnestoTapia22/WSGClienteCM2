using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using WSGClienteCM.Connection;
using WSGClienteCM.Models;
using WSGClienteCM.Repository;
using System.Linq;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Options;
using WSGClienteCM.Utils;

namespace WSGClienteCM.Services
{
    public class CargaMasivaService : ICargaMasivaService
    {
        private readonly ICargaMasivaRepository _cargaMasivaRepository;
        private readonly IConnectionBase _connectionBase;
        private readonly AppSettings _appSettings;
        public CargaMasivaService(ICargaMasivaRepository cargaMasivaRepository, IConnectionBase connectionBase, IOptions<AppSettings> appSettings)
        {
            this._cargaMasivaRepository = cargaMasivaRepository;
            this._connectionBase = connectionBase;
            this._appSettings = appSettings.Value;
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
                    processCodeToUpdate = responseViewModel.ElistDetail.GroupBy(x => x.SNROPROCESO_CAB).Select(p => p.First().SNROPROCESO_CAB).ToList();
                    DataConnection.Open();
                    trx = DataConnection.BeginTransaction();
                    ResponseViewModel res = await _cargaMasivaRepository.UpdateStateHeader(processCodeToUpdate, "1", DataConnection, trx);
                    if (res.P_COD_ERR == "0")
                    {

                        foreach (DetailBindingModel row in responseViewModel.ElistDetail)
                        {
                            DetailBindingModel detailState = new DetailBindingModel();
                            ResponseViewModel resval = new ResponseViewModel();
                            // primera validacion tipo y numero de documento
                            if (row.NIDDOC_TYPE == "" || row.SIDDOC == "")// validar bien paso tipo doc =0
                            {
                                detailState.NIDDOC_TYPE = "No tiene el tipo de documento o el número de documento";
                                detailState.SIDDOC = "No tiene el tipo de documento o el número de documento";

                            }
                            else
                            {
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
                                    if (resInsertState.P_COD_ERR != "0")
                                    {
                                        error.SMENSAJE = "No se puedo insertar el estado del registro con id: " + row.NNROPROCESO_DET + "_" + resInsertState.P_MESSAGE;
                                        error.SGRUPO = "GES_CAR_MAS_CLI_DET_STATE";
                                        responseViewModel.EListErrores.Add(error);
                                    }
                                }
                                else
                                {

                                    //Consultar
                                    PostRequest postRequest = new PostRequest
                                    {
                                        P_NUSERCODE = row.NUSERCODE.ToString(),
                                        P_NIDDOC_TYPE = row.NIDDOC_TYPE,
                                        P_SIDDOC = row.SIDDOC,
                                        P_CodAplicacion = row.SCODAPLICACION,
                                        P_TipOper = "CON"
                                    };
                                    string result = await PostRequest(_appSettings.ClientService, postRequest);
                                    ResponseViewModel resClientService = new ResponseViewModel();
                                    resClientService = JsonConvert.DeserializeObject<ResponseViewModel>(result);
                                    if (resClientService != null)
                                    {
                                        if (resClientService.P_COD_ERR != "0")
                                        {

                                        }

                                    }



                                }
                            }



                        }

                    }
                    else
                    {
                        responseViewModel = res;
                        trx.Rollback();

                    }

                }
                else
                {

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
                    else
                    {

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
                if (DataConnection.State == ConnectionState.Open)
                {
                    DataConnection.Close();
                }
                trx.Dispose();
            }
            return responseViewModel;
        }

        private DetailBindingModel ParseErrorToModel(List<ListViewErrores> items)
        {
            DetailBindingModel model = new DetailBindingModel();
            foreach (ListViewErrores item in items)
            {
                if (model.GetType().GetProperty(item.SCAMPO) != null)
                {
                    model.GetType().GetProperty(item.SCAMPO).SetValue(model, item.SMENSAJE, null);
                };
            }
            return model;
        }
        public async Task<string> PostRequest(string url, object postObject, string token = null)
        {

            string result = null;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                UriBuilder builder = new UriBuilder(url);
                string cadena = builder.Uri.ToString();
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    if (token != null)
                    {
                        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    }
                    string json = JsonConvert.SerializeObject(postObject);
                    StringContent stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(builder.Uri, stringContent);
                    result = await response.Content.ReadAsStringAsync();

                }

            }
            catch (Exception ex)
            {

                throw ex;

            }

            return result;

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
