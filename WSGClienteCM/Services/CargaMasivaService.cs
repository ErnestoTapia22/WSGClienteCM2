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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using WSGClienteCM.Helper;

namespace WSGClienteCM.Services
{
    public class CargaMasivaService : ICargaMasivaService
    {
        private readonly ICargaMasivaRepository _cargaMasivaRepository;
        private readonly IConnectionBase _connectionBase;
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _HostEnvironment;
        public CargaMasivaService(ICargaMasivaRepository cargaMasivaRepository, IConnectionBase connectionBase, IOptions<AppSettings> appSettings, IHostingEnvironment HostEnvironment)
        {
            this._cargaMasivaRepository = cargaMasivaRepository;
            this._connectionBase = connectionBase;
            this._appSettings = appSettings.Value;
            this._HostEnvironment = HostEnvironment;
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
                responseViewModel = await _cargaMasivaRepository.GetByState("0");
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
                                int idInserted = 0;
                                
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
                                    idInserted = Convert.ToInt32(resInsertState.P_NIDCM);
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
                                        P_NUSERCODE = row.NUSERCODE.ToString().Trim(),
                                        P_NIDDOC_TYPE = row.NIDDOC_TYPE.Trim(),
                                        P_SIDDOC = row.SIDDOC.Trim(),
                                        P_CodAplicacion = row.SCODAPLICACION.Trim(),
                                        P_TipOper = "CON"
                                    };
                                    string result = await PostRequest(_appSettings.ClientService, postRequest);
                                    ResponseViewModel resClientService = new ResponseViewModel();
                                    resClientService = JsonConvert.DeserializeObject<ResponseViewModel>(result);
                                    if (resClientService != null)
                                    {
                                        if (resClientService.P_NCODE == "0")
                                        {
                                            ClientBindingModel resToSend = new ClientBindingModel();
                                            resToSend = CompleteFields(resClientService.EListClient[0], row);
                                            
                                            string resultUpdate = await PostRequest(_appSettings.ClientService, resToSend);
                                            ResponseViewModel resUpdate = JsonConvert.DeserializeObject<ResponseViewModel>(resultUpdate);
                                            ResponseViewModel resUpdateDB = new ResponseViewModel();
                                            if (resUpdate.P_NCODE == "0")
                                            {
                                                resUpdateDB = await _cargaMasivaRepository.UpdateStateResponse(Convert.ToInt32(row.NNROPROCESO_DET), resUpdate.P_SMESSAGE, 1, DataConnection, trx);
                                            }
                                            else {
                                                resUpdateDB = await _cargaMasivaRepository.UpdateStateResponse(Convert.ToInt32(row.NNROPROCESO_DET), resUpdate.P_SMESSAGE, 0, DataConnection, trx);
                                            }
                                           
                                         
                                        }

                                    }



                                }
                            }



                        }
                        ResponseViewModel resgetState1   = new ResponseViewModel();
                        List<string> processCodeToEmail = new List<string>();

                        ResponseViewModel res2 = await _cargaMasivaRepository.UpdateStateHeader(processCodeToUpdate, "2", DataConnection, trx);
                        if (res2.P_COD_ERR == "0")
                        {
                            trx.Commit();
                            DataConnection.Close();
                            resgetState1 = await _cargaMasivaRepository.GetByState("2");

                            processCodeToEmail = resgetState1.ElistDetail.GroupBy(x => x.SNROPROCESO_CAB).Select(p => p.First().SNROPROCESO_CAB).ToList();
                            //foreach (string processCode in processCodeToEmail) { 
                            //    await SendEmails(processCode);
                            //}

                           
                        }
                        else {
                            res.P_COD_ERR = res2.P_COD_ERR;
                            res.P_MESSAGE = res2.P_MESSAGE;
                            responseViewModel = res;
                            trx.Rollback();
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
                //await _cargaMasivaRepository.UpdateStateHeader(processCodeToUpdate, "-1", DataConnection, trx);
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
                if (trx != null)
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


        }

        public async Task<ResponseViewModel> SendEmails(string snroprocess) {
            ResponseViewModel _objReturn = null;
            _objReturn = new ResponseViewModel();
            if (snroprocess != null || snroprocess != "") {
                try {
                    TramaRespuestaCargaMasivaResponse tramaExistosa;
                    TramaRespuestaCargaMasivaResponse tramaError;
                    TramaRespuestaCargaMasivaResponse correoUsuarios;

                    List<Archivo> tramasList = new List<Archivo>();

                    tramaExistosa = await _cargaMasivaRepository.ObtenerTramaEnvioExitosa(snroprocess);
                    tramaError = await _cargaMasivaRepository.ObtenerTramaEnvioErrores(snroprocess);
                    correoUsuarios = await _cargaMasivaRepository.ObtenerListaUsuariosEnvioTrama(snroprocess);


                    if (!tramaExistosa.respuesta && (tramaExistosa.codigoRespuesta != "0"))
                    {
                        _objReturn.P_NCODE = "1";
                        _objReturn.P_SMESSAGE = tramaExistosa.mensajes[0];
                        return _objReturn;
                    }
                    if (!tramaError.respuesta && (tramaError.codigoRespuesta != "0"))
                    {
                        _objReturn.P_NCODE = "1";
                        _objReturn.P_SMESSAGE = tramaError.mensajes[0];
                        return _objReturn;
                    }
                    if (!correoUsuarios.respuesta && (correoUsuarios.codigoRespuesta != "0"))
                    {
                        _objReturn.P_NCODE = "1";
                        _objReturn.P_SMESSAGE = correoUsuarios.mensajes[0];
                        return _objReturn;
                    }
                    if (!correoUsuarios.respuesta)
                    {
                        _objReturn.P_NCODE = "1";
                        _objReturn.P_SMESSAGE = correoUsuarios.mensajes[0];
                        return _objReturn;
                    }

                    string contentRootPath = _HostEnvironment.ContentRootPath;
                    string path_CuerpoCorreo = Path.Combine(contentRootPath, @"Templates\CorreoTramaCargaMasiva01.html");
                    string htmlCorreo = File.ReadAllText(path_CuerpoCorreo);
                    string addressFrom = "cotizacionesdigitales@protectasecurity.pe";
                    string pwdFrom = "0perac10nesSCTR$$_";

                    string addressTo;
                    string subject = "Detalle  Carga  Masiva - Cliente  360";


                    NotifyHelper objNotifyHelper = new NotifyHelper();
                    tramasList.Add(objNotifyHelper.ComposeExcelErrores(contentRootPath, tramaError.tramaErrores));
                    tramasList.Add(objNotifyHelper.ComposeExcelExitoso(contentRootPath, tramaExistosa.tramaExitosa));

                    foreach (EmailViewModel email in correoUsuarios.correoUsuarios)
                    {
                        addressTo = email.P_SE_MAIL;
                        await objNotifyHelper.SendMail(addressFrom, pwdFrom, addressTo, subject, htmlCorreo, tramasList);
                    }

                    if (_objReturn == null)
                    {
                        return new ResponseViewModel();
                    }
                    _objReturn.P_NCODE = "0";
                    _objReturn.P_SMESSAGE = "Se notificaron las tramas  con �xito";

                }
                catch (Exception ex){
                    _objReturn.P_NCODE = "2";
                    _objReturn.P_SMESSAGE = ex.Message;
                   
                }



            }
            return _objReturn;


        }
        public ClientBindingModel CompleteFields(ClientViewModel resMaster,DetailBindingModel resToComplete) {

            ClientBindingModel clientBindingModel = new ClientBindingModel();
            clientBindingModel.P_SNOPROCESO = resToComplete.SNROPROCESO_CAB.Trim();
            clientBindingModel.P_NNUMREG = Convert.ToInt64(resToComplete.NNUMREG);
            clientBindingModel.P_SFILENAME = resToComplete.SFILENAME.Trim();

            if (clientBindingModel.P_SFIRSTNAME != null) {
                if (resToComplete.SFIRSTNAME.Trim() != clientBindingModel.P_SFIRSTNAME.Trim())
                {
                    clientBindingModel.P_SFIRSTNAME = resToComplete.SFIRSTNAME.Trim();
                }
            }

            if (clientBindingModel.P_SLASTNAME != null)
            {
                if (resToComplete.SLASTNAME.Trim() != clientBindingModel.P_SLASTNAME.Trim())
                {
                    clientBindingModel.P_SLASTNAME = resToComplete.SLASTNAME.Trim();
                }
            }

            if (clientBindingModel.P_SLASTNAME2 != null)
            {
                if (resToComplete.SLASTNAME2.Trim() != clientBindingModel.P_SLASTNAME2.Trim())
                {
                    clientBindingModel.P_SLASTNAME2 = resToComplete.SLASTNAME2.Trim();
                }
            }

            if (clientBindingModel.P_SLEGALNAME != null)
            {
                if (resToComplete.SLEGALNAME.Trim() != clientBindingModel.P_SLEGALNAME.Trim())
                {
                    clientBindingModel.P_SLEGALNAME = resToComplete.SLEGALNAME.Trim();
                }
            }

            if (clientBindingModel.P_SSEXCLIEN != null)
            {
                if (resToComplete.SSEXCLIEN.Trim() != clientBindingModel.P_SSEXCLIEN.Trim())
                {
                    clientBindingModel.P_SSEXCLIEN = resToComplete.SSEXCLIEN.Trim();
                }
            }

            if (clientBindingModel.P_NCIVILSTA != null)
            {
                if (resToComplete.NCIVILSTA.Trim() != clientBindingModel.P_NCIVILSTA.Trim())
                {
                    clientBindingModel.P_NCIVILSTA = resToComplete.NCIVILSTA.Trim();
                }
            }

            if (clientBindingModel.P_NNATIONALITY != null)
            {
                if (resToComplete.NNATIONALITY.Trim() != clientBindingModel.P_NNATIONALITY.Trim())
                {
                    clientBindingModel.P_NNATIONALITY = resToComplete.NNATIONALITY.Trim();
                }
            }

            if (clientBindingModel.P_DBIRTHDAT != null)
            {
                if (resToComplete.DBIRTHDAT.Trim() != clientBindingModel.P_DBIRTHDAT.Trim())
                {
                    clientBindingModel.P_DBIRTHDAT = resToComplete.DBIRTHDAT.Trim();
                }
            }

            if (clientBindingModel.P_COD_CIIU != null)
            {
                if (resToComplete.COD_CIIU.Trim() != clientBindingModel.P_COD_CIIU.Trim())
                {
                    clientBindingModel.P_COD_CIIU = resToComplete.COD_CIIU.Trim();
                }
            }

            if (clientBindingModel.P_COD_CUSPP != null)
            {
                if (resToComplete.COD_CUSPP.Trim() != clientBindingModel.P_COD_CUSPP.Trim())
                {
                    clientBindingModel.P_COD_CUSPP = resToComplete.COD_CUSPP.Trim();
                }

            }

            if (clientBindingModel.P_SISCLIENT_IND != null)
            {
                if (resToComplete.SPROTEG_DATOS_IND.Trim() != clientBindingModel.P_SISCLIENT_IND.Trim())
                {
                    clientBindingModel.P_SISCLIENT_IND = resToComplete.SPROTEG_DATOS_IND.Trim();
                }
            }

            if (clientBindingModel.P_SISCLIENT_IND != null)
            {
                if (resToComplete.SPROTEG_DATOS_IND.Trim() != clientBindingModel.P_SISCLIENT_IND.Trim())
                {
                    clientBindingModel.P_SISCLIENT_IND = resToComplete.SPROTEG_DATOS_IND.Trim();
                }
            }

            if (clientBindingModel.P_SBAJAMAIL_IND != null)
            {
                if (resToComplete.SBAJAMAIL_IND.Trim() != clientBindingModel.P_SBAJAMAIL_IND.Trim())
                {
                    clientBindingModel.P_SBAJAMAIL_IND = resToComplete.SBAJAMAIL_IND.Trim();
                }
            }

            if (clientBindingModel.P_SISCLIENT_GBD != null)
            {

                if (resToComplete.SISCLIENT_GBD.Trim() != clientBindingModel.P_SISCLIENT_GBD.Trim())
                {
                    clientBindingModel.P_SISCLIENT_GBD = resToComplete.SISCLIENT_GBD.Trim();
                }
            }
           

            clientBindingModel.EListAddresClient = new List<AddressBindingModel>();

            AddressBindingModel adr = new AddressBindingModel();
            adr.P_ADDRESSTYPE = resToComplete.ADDRESSTYPE == null ? null : resToComplete.ADDRESSTYPE.Trim();
            adr.P_STI_DIRE = resToComplete.STI_DIRE == null ? null : resToComplete.STI_DIRE.Trim();
            adr.P_SNOM_DIRECCION = resToComplete.SNOM_DIRECCION == null ? null: resToComplete.SNOM_DIRECCION.Trim();
            adr.P_SNUM_DIRECCION = resToComplete.SNUM_DIRECCION == null ? null : resToComplete.SNUM_DIRECCION.Trim();
            adr.P_STI_BLOCKCHALET = resToComplete.STI_BLOCKCHALET == null ? null :resToComplete.STI_BLOCKCHALET.Trim();
            adr.P_SBLOCKCHALET = resToComplete.SBLOCKCHALET == null ? null : resToComplete.SBLOCKCHALET.Trim();
            adr.P_STI_BLOCKCHALET = resToComplete.STI_BLOCKCHALET == null ? null: resToComplete.STI_BLOCKCHALET.Trim();
            adr.P_STI_INTERIOR = resToComplete.STI_INTERIOR == null ? null :  resToComplete.STI_INTERIOR.Trim();
            adr.P_STI_CJHT = resToComplete.STI_CJHT == null ? null :  resToComplete.STI_CJHT.Trim();
            adr.P_SNOM_CJHT = resToComplete.SNOM_CJHT == null ? null : resToComplete.SNOM_CJHT.Trim();
            adr.P_SETAPA = resToComplete.SETAPA == null ? null : resToComplete.SETAPA.Trim();
            adr.P_SMANZANA = resToComplete.SMANZANA == null ? null : resToComplete.SMANZANA.Trim();
            adr.P_SLOTE = resToComplete.SLOTE  == null ? null : resToComplete.SLOTE.Trim();
            adr.P_SREFERENCE = resToComplete.SREFERENCIA == null ? null :  resToComplete.SREFERENCIA.Trim();
            adr.P_NMUNICIPALITY = resToComplete.NMUNICIPALITY == null ? null: resToComplete.NMUNICIPALITY.Trim();

            clientBindingModel.EListAddresClient.Add(adr);

            clientBindingModel.EListPhoneClient = new List<PhoneBindingModel>();

            PhoneBindingModel phone = new PhoneBindingModel();
            phone.P_NAREA_CODE = resToComplete.NAREA_CODE == null ? null : resToComplete.NAREA_CODE.Trim();
            phone.P_NPHONE_TYPE = resToComplete.NPHONE_TYPE == null ? null : resToComplete.NPHONE_TYPE.Trim();
            phone.P_SPHONE = resToComplete.SPHONE == null ? null :resToComplete.SPHONE.Trim();

            clientBindingModel.EListPhoneClient.Add(phone);

            clientBindingModel.EListEmailClient = new List<EmailBindingModel>();

            EmailBindingModel emailBindingModel = new EmailBindingModel();
            emailBindingModel.P_SEMAILTYPE = resToComplete.SEMAILTYPE == null ? null : resToComplete.SEMAILTYPE.Trim();
            emailBindingModel.P_SE_MAIL = resToComplete.SE_MAIL == null ? null : resToComplete.SE_MAIL.Trim();

            clientBindingModel.EListEmailClient.Add(emailBindingModel);





            return clientBindingModel;
        }

        //public TramaRespuestaCargaMasivaResponse ObtenerTramaEnvioExitosa(string P_SNOPROCESO)
        //{
        //    return _cargaMasivaRepository.ObtenerTramaEnvioExitosa(P_SNOPROCESO);
        //}
        //public TramaRespuestaCargaMasivaResponse ObtenerTramaEnvioErrores(string P_SNOPROCESO)
        //{
        //    return _cargaMasivaRepository.ObtenerTramaEnvioErrores(P_SNOPROCESO);
        //}
        //public TramaRespuestaCargaMasivaResponse ObtenerListaUsuariosEnvioTrama(string P_SNOPROCESO)
        //{
        //    return _cargaMasivaRepository.ObtenerListaUsuariosEnvioTrama(P_SNOPROCESO);
        //}
    }
}
