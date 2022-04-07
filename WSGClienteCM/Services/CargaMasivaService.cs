using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using WSGClienteCM.Connection;
using WSGClienteCM.Controllers;
using WSGClienteCM.Helper;
using WSGClienteCM.Models;
using WSGClienteCM.Repository;
using WSGClienteCM.Utils;

namespace WSGClienteCM.Services
{
    public class CargaMasivaService : ICargaMasivaService
    {
        private readonly ICargaMasivaRepository _cargaMasivaRepository;
        private readonly IConnectionBase _connectionBase;
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _HostEnvironment;
        private readonly IMapper _mapper;
        private EmailController _emailController;
        public CargaMasivaService(ICargaMasivaRepository cargaMasivaRepository, IConnectionBase connectionBase, IOptions<AppSettings> appSettings, IHostingEnvironment HostEnvironment, IMapper mapper, EmailController emailController)
        {
            this._cargaMasivaRepository = cargaMasivaRepository;
            this._connectionBase = connectionBase;
            this._appSettings = appSettings.Value;
            this._HostEnvironment = HostEnvironment;
            this._mapper = mapper;
            this._emailController = emailController;
        }

        public async Task<ResponseViewModel> InitProcess()
        {

            ResponseViewModel responseViewModel = new ResponseViewModel();
            responseViewModel.EListErrores = new List<ListViewErrores>();


            try
            {
                // selecciona registros con estado 0
                responseViewModel = await _cargaMasivaRepository.GetHeadersByState("0");

                if (responseViewModel.P_COD_ERR == "0" && responseViewModel.ElistHeaders.Count > 0)
                {
                    List<string> processCodeToUpdate = new List<string>();
                    processCodeToUpdate = responseViewModel.ElistHeaders.GroupBy(x => x.SNROPROCESO_CAB).Select(p => p.First().SNROPROCESO_CAB).ToList();
                    ResponseViewModel res = await _cargaMasivaRepository.UpdateStateHeader(processCodeToUpdate, "1");
                    foreach (HeaderBindingModel header in responseViewModel.ElistHeaders)
                    {
                        ResponseViewModel responseDetail = new ResponseViewModel();
                        responseDetail = await _cargaMasivaRepository.GetByState(header.SNROPROCESO_CAB);
                        if (responseDetail.P_COD_ERR == "0" && responseDetail.ElistDetail.Count > 0)
                        {

                            foreach (DetailBindingModel row in responseDetail.ElistDetail)
                            {

                                DetailBindingModel detailState = new DetailBindingModel();
                                ResponseViewModel resval = new ResponseViewModel();
                                // primera validacion tipo y numero de documento
                                if (row.SIDDOC == "" || row.SIDDOC == null)// validar bien paso tipo doc =0
                                {
                                    DetailBindingModel detailStateParsed1 = new DetailBindingModel();
                                    detailStateParsed1.SIDDOC = "El número de documento es obligatorio";
                                    detailStateParsed1.NNROPROCESO_DET = row.NNROPROCESO_DET;
                                    await _cargaMasivaRepository.SaveStateRow(detailStateParsed1);

                                }
                                else
                                {
                                    int idInserted = 0;

                                    // validacion
                                    resval = await _cargaMasivaRepository.ValidateRow(row);

                                    if (resval.EListErrores.Count > 0)
                                    {
                                        DetailBindingModel detailStateParsed = new DetailBindingModel();
                                        ResponseViewModel resInsertState = new ResponseViewModel();
                                        ListViewErrores error = new ListViewErrores();
                                        detailStateParsed = ParseErrorToModel(resval.EListErrores);
                                        detailStateParsed.NNROPROCESO_DET = row.NNROPROCESO_DET;
                                        resInsertState = await _cargaMasivaRepository.SaveStateRow(detailStateParsed);
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
                                            P_CodAplicacion = "GESTORCLIENTE",//row.SCODAPLICACION.Trim(),
                                            P_TipOper = "CON"
                                        };
                                        try
                                        {
                                            string result = await PostRequest(_appSettings.ClientService, postRequest);
                                            ResponseViewModel resClientService = new ResponseViewModel();
                                            resClientService = JsonConvert.DeserializeObject<ResponseViewModel>(result);
                                            if (resClientService != null)
                                            {

                                                if (resClientService.P_NCODE == "0")
                                                {
                                                    ClientBindingModel resToSend = new ClientBindingModel();
                                                    ClientViewModel resToSend2 = new ClientViewModel();
                                                    ResponseViewModel resUpdateDB = new ResponseViewModel();
                                                    if (resClientService.EListClient.Count() > 0)
                                                    {
                                                        resToSend = CompleteFields(resClientService.EListClient[0], row);
                                                        string resultUpdate = await PostRequest(_appSettings.ClientService, resToSend);
                                                        ResponseViewModel resUpdate = JsonConvert.DeserializeObject<ResponseViewModel>(resultUpdate);


                                                        if (resUpdate.P_NCODE == "0")
                                                        {

                                                            resUpdateDB = await _cargaMasivaRepository.UpdateStateResponse(Convert.ToInt32(row.NNROPROCESO_DET), resToSend.P_IS_RENTAS ? resUpdate.P_SMESSAGE + "; " + resToSend.P_SMESSAGE_SEACSA : resUpdate.P_SMESSAGE, 1);
                                                        }
                                                        else
                                                        {
                                                            resUpdateDB = await _cargaMasivaRepository.UpdateStateResponse(Convert.ToInt32(row.NNROPROCESO_DET), resToSend.P_IS_RENTAS ? resUpdate.P_SMESSAGE + "; " + resToSend.P_SMESSAGE_SEACSA : resUpdate.P_SMESSAGE, 0);
                                                        }
                                                    }
                                                    else
                                                    {

                                                        resUpdateDB = await _cargaMasivaRepository.UpdateStateResponse(Convert.ToInt32(row.NNROPROCESO_DET), "No se encontró al cliente", 0);
                                                    }

                                                }

                                            }
                                            else
                                            {
                                                await _cargaMasivaRepository.UpdateStateResponse(Convert.ToInt32(row.NNROPROCESO_DET), "Error al cosultar datos en WSGClienteService", 1);

                                            }

                                        }
                                        catch (Exception ex)
                                        {
                                            await _cargaMasivaRepository.UpdateStateResponse(Convert.ToInt32(row.NNROPROCESO_DET), "Ocurrió un error en el proceso :" + ex.Message, 0);
                                            continue;

                                        }
                                    }
                                }

                            }
                            await _cargaMasivaRepository.UpdateStateHeader(new List<string> { header.SNROPROCESO_CAB }, "2");
                            RespuestaMail resp = await SendEmails(header.SNROPROCESO_CAB);
                            var a = _emailController.SendEmail(resp);
                            var okResult = a as OkObjectResult;
                            var actualConfiguration = okResult.Value as ResponseViewModel;
                            responseViewModel.P_MESSAGE = actualConfiguration.P_MESSAGE;
                            responseViewModel.P_NCODE = actualConfiguration.P_NCODE;
                        }
                        else
                        {
                            await _cargaMasivaRepository.UpdateStateHeader(new List<string> { header.SNROPROCESO_CAB }, "3");
                        }


                    }

                }
                else
                {

                    if (responseViewModel.P_COD_ERR == "0")
                    {
                        responseViewModel.P_MESSAGE = "No hay registros para procesar";
                    }

                }

            }
            catch (Exception ex)
            {
                //await _cargaMasivaRepository.UpdateStateHeader(processCodeToUpdate, "-1", DataConnection, trx);

                responseViewModel.P_COD_ERR = "0";
                responseViewModel.P_MESSAGE = ex.Message;

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
                if (trx != null)
                {
                    trx.Rollback();
                }

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

        public async Task<RespuestaMail> SendEmails(string snroprocess)

        {
            ResponseViewModel _objReturn = new ResponseViewModel();

            RespuestaMail respuestam = new RespuestaMail();
            respuestam.correos = new TramaRespuestaCargaMasivaResponse();
            respuestam.tramaslist = new List<Archivo>();


            if (snroprocess != null || snroprocess != "")
            {
                try
                {
                    TramaRespuestaCargaMasivaResponse tramaExistosa;
                    TramaRespuestaCargaMasivaResponse tramaError;
                    TramaRespuestaCargaMasivaResponse correoUsuarios;

                    List<Archivo> tramasList = new List<Archivo>();

                    tramaExistosa = await _cargaMasivaRepository.ObtenerTramaEnvioExitosa(snroprocess);
                    tramaError = await _cargaMasivaRepository.ObtenerTramaEnvioErrores(snroprocess);
                    correoUsuarios = await _cargaMasivaRepository.ObtenerListaUsuariosEnvioTrama(snroprocess);


                    if (!tramaExistosa.respuesta && (tramaExistosa.codigoRespuesta != "0"))
                    {
                        respuestam.P_NCODE = "1";
                        respuestam.P_SMESSAGE = tramaExistosa.mensajes[0];
                        //  return _objReturn;
                    }
                    if (!tramaError.respuesta && (tramaError.codigoRespuesta != "0"))
                    {
                        respuestam.P_NCODE = "1";
                        respuestam.P_SMESSAGE = tramaError.mensajes[0];
                        // return _objReturn;
                    }
                    if (!correoUsuarios.respuesta && (correoUsuarios.codigoRespuesta != "0"))
                    {
                        respuestam.P_NCODE = "1";
                        respuestam.P_SMESSAGE = correoUsuarios.mensajes[0];
                        // return _objReturn;
                    }
                    if (!correoUsuarios.respuesta)
                    {
                        respuestam.P_NCODE = "1";
                        respuestam.P_SMESSAGE = correoUsuarios.mensajes[0];
                        // return _objReturn;
                    }

                    string contentRootPath = _HostEnvironment.ContentRootPath;
                    string path_CuerpoCorreo = Path.Combine(contentRootPath, @"Templates\CorreoTramaCargaMasiva01.html");
                    string htmlCorreo = File.ReadAllText(path_CuerpoCorreo);




                    NotifyHelper objNotifyHelper = new NotifyHelper();
                    if (tramaError.tramaErrores.Count > 0)
                    {
                        tramasList.Add(objNotifyHelper.ComposeExcelErrores(contentRootPath, tramaError.tramaErrores));
                    }

                    if (tramaExistosa.tramaExitosa.Count > 0)
                    {
                        tramasList.Add(objNotifyHelper.ComposeExcelExitoso(contentRootPath, tramaExistosa.tramaExitosa));
                    }
                    respuestam.correos = new TramaRespuestaCargaMasivaResponse();
                    respuestam.correos = correoUsuarios;
                    respuestam.tramaslist = new List<Archivo>();
                    respuestam.tramaslist = tramasList;
                    respuestam.nroProces = snroprocess;



                }
                catch (SmtpException smtpEx)
                {

                    respuestam.P_NCODE = "2";
                    respuestam.P_SMESSAGE = smtpEx.Message;
                    return respuestam;
                }
                catch (Exception ex)
                {
                    respuestam.P_NCODE = "2";
                    respuestam.P_SMESSAGE = ex.Message;
                    return respuestam;
                }
            }
            return respuestam;


        }
        public ClientBindingModel CompleteFields(ClientViewModel resMaster, DetailBindingModel resToComplete)
        {
            string respRentas = "Para los clientes de rentas no se actualiza el ";
            ClientBindingModel clientBindingModel = new ClientBindingModel();
            clientBindingModel = _mapper.Map<ClientViewModel, ClientBindingModel>(resMaster);
            clientBindingModel.P_SNOPROCESO = resToComplete.SNROPROCESO_CAB?.Trim();
            clientBindingModel.P_NNUMREG = Convert.ToInt64(resToComplete.NNUMREG);
            clientBindingModel.P_SFILENAME = resToComplete.SFILENAME?.Trim();
            clientBindingModel.P_TipOper = "INS";
            clientBindingModel.P_CodAplicacion = resToComplete.SCODAPLICACION?.Trim();
            clientBindingModel.P_NIDDOC_TYPE = resToComplete.NIDDOC_TYPE?.Trim();
            clientBindingModel.P_SIDDOC = resToComplete.SIDDOC?.Trim();
            clientBindingModel.P_NUSERCODE = resToComplete.NUSERCODE.ToString();



            if (resToComplete.SFIRSTNAME != null)
            {
                if (resMaster.P_SISSEACSA_IND == "1")
                {
                    respRentas += "Nombre,";
                }
                if (resToComplete.SFIRSTNAME?.Trim() != resMaster.P_SFIRSTNAME?.Trim())
                {
                    clientBindingModel.P_SFIRSTNAME = resToComplete.SFIRSTNAME?.Trim();
                }
                else
                {
                    clientBindingModel.P_SFIRSTNAME = resMaster.P_SFIRSTNAME?.Trim();
                }
            }
            else
            {
                clientBindingModel.P_SFIRSTNAME = resMaster.P_SFIRSTNAME?.Trim();
            }



            if (resToComplete.SLASTNAME != null)
            {
                if (resMaster.P_SISSEACSA_IND == "1")
                {
                    respRentas += " Apellido,";
                }
                if (resToComplete.SLASTNAME?.Trim() != resMaster.P_SLASTNAME?.Trim())
                {
                    clientBindingModel.P_SLASTNAME = resToComplete.SLASTNAME?.Trim();
                }
                else
                {
                    clientBindingModel.P_SLASTNAME = resMaster.P_SLASTNAME?.Trim();
                }
            }
            else
            {
                clientBindingModel.P_SLASTNAME = resMaster.P_SLASTNAME?.Trim();
            }

            if (resToComplete.SLASTNAME2 != null)
            {
                if (resMaster.P_SISSEACSA_IND == "1")
                {
                    respRentas += " Apellido materno,";
                }
                if (resToComplete.SLASTNAME2?.Trim() != resMaster.P_SLASTNAME2?.Trim())
                {
                    clientBindingModel.P_SLASTNAME2 = resToComplete.SLASTNAME2?.Trim();
                }
                else
                {
                    clientBindingModel.P_SLASTNAME2 = resMaster.P_SLASTNAME2?.Trim();
                }
            }
            else
            {

                clientBindingModel.P_SLASTNAME2 = resMaster.P_SLASTNAME2?.Trim();
            }

            if (resToComplete.SLEGALNAME != null)
            {
                if (resMaster.P_SISSEACSA_IND == "1")
                {
                    respRentas += " Nombre legal,";
                }
                if (resToComplete.SLEGALNAME?.Trim() != resMaster.P_SLEGALNAME?.Trim())
                {
                    clientBindingModel.P_SLEGALNAME = resToComplete.SLEGALNAME?.Trim();
                }
                else
                {
                    clientBindingModel.P_SLEGALNAME = resMaster.P_SLEGALNAME?.Trim();
                }
            }
            else
            {
                clientBindingModel.P_SLEGALNAME = resMaster.P_SLEGALNAME?.Trim();

            }

            if (resToComplete.SSEXCLIEN != null && resToComplete.SSEXCLIEN?.Trim() != "0")
            {
                if (resMaster.P_SISSEACSA_IND == "1")
                {
                    respRentas += " sexo,";
                }
                if (resToComplete.SSEXCLIEN?.Trim() != resMaster.P_SSEXCLIEN?.Trim())
                {
                    clientBindingModel.P_SSEXCLIEN = resToComplete.SSEXCLIEN?.Trim();
                }
                else
                {
                    clientBindingModel.P_SSEXCLIEN = resMaster.P_SSEXCLIEN?.Trim();
                }
            }
            else
            {
                clientBindingModel.P_SSEXCLIEN = resMaster.P_SSEXCLIEN?.Trim();


            }

            if (resToComplete.NCIVILSTA != null && resToComplete.NCIVILSTA?.Trim() != "0")
            {
                if (resMaster.P_SISSEACSA_IND == "1")
                {
                    respRentas += " estado civil,";
                }
                if (resToComplete.NCIVILSTA?.Trim() != resMaster.P_NCIVILSTA?.Trim())
                {


                    clientBindingModel.P_NCIVILSTA = resToComplete.NCIVILSTA?.Trim();


                }
                else
                {
                    clientBindingModel.P_NCIVILSTA = resMaster.P_NCIVILSTA?.Trim();
                }

            }
            else
            {
                clientBindingModel.P_NCIVILSTA = resMaster.P_NCIVILSTA?.Trim();
            }

            if (resToComplete.NNATIONALITY != null && resToComplete.NNATIONALITY?.Trim() != "0")
            {
                if (resMaster.P_SISSEACSA_IND == "1")
                {
                    respRentas += " nacionalidad,";
                }
                if (resToComplete.NNATIONALITY?.Trim() != resMaster.P_NNATIONALITY?.Trim())
                {
                    clientBindingModel.P_NNATIONALITY = resToComplete.NNATIONALITY?.Trim();
                }
                else
                {
                    clientBindingModel.P_NNATIONALITY = resMaster.P_NNATIONALITY?.Trim();
                }
            }
            else
            {
                clientBindingModel.P_NNATIONALITY = resMaster.P_NNATIONALITY?.Trim();
            }

            if (resToComplete.DBIRTHDAT != null)
            {
                if (resMaster.P_SISSEACSA_IND == "1")
                {
                    respRentas += " fecha de nacimiento,";
                }
                if (resToComplete.DBIRTHDAT?.Trim() != resMaster.P_DBIRTHDAT?.Trim())
                {
                    clientBindingModel.P_DBIRTHDAT = resToComplete.DBIRTHDAT?.Trim();
                }
                else
                {
                    clientBindingModel.P_DBIRTHDAT = resMaster.P_DBIRTHDAT?.Trim();
                }
            }
            else
            {
                clientBindingModel.P_DBIRTHDAT = resMaster.P_DBIRTHDAT?.Trim();
            }



            if (resToComplete.COD_CUSPP != null)
            {
                if (resToComplete.COD_CUSPP?.Trim() != resMaster.P_COD_CUSPP?.Trim())
                {
                    clientBindingModel.P_COD_CUSPP = resToComplete.COD_CUSPP?.Trim();
                }
                else
                {
                    clientBindingModel.P_COD_CUSPP = resMaster.P_COD_CUSPP?.Trim();
                }

            }
            else
            {
                clientBindingModel.P_COD_CUSPP = resMaster.P_COD_CUSPP?.Trim();
            }

            if (resToComplete.SPROTEG_DATOS_IND != null && resToComplete.SPROTEG_DATOS_IND?.Trim() != "0")
            {
                if (resToComplete.SPROTEG_DATOS_IND?.Trim() != resMaster.P_SISCLIENT_IND?.Trim())
                {
                    clientBindingModel.P_SISCLIENT_IND = resToComplete.SPROTEG_DATOS_IND?.Trim();
                }
                else
                {
                    clientBindingModel.P_SISCLIENT_IND = resMaster.P_SISCLIENT_IND?.Trim();
                }
            }
            else
            {
                clientBindingModel.P_SISCLIENT_IND = resMaster.P_SISCLIENT_IND?.Trim();
            }



            if (resToComplete.SBAJAMAIL_IND != null && resToComplete.SBAJAMAIL_IND?.Trim() != "0")
            {
                if (resToComplete.SBAJAMAIL_IND?.Trim() != resMaster.P_SBAJAMAIL_IND?.Trim())
                {
                    clientBindingModel.P_SBAJAMAIL_IND = resToComplete.SBAJAMAIL_IND?.Trim();
                }
                else
                {
                    clientBindingModel.P_SBAJAMAIL_IND = resMaster.P_SBAJAMAIL_IND?.Trim();
                }



            }
            else
            {
                clientBindingModel.P_SBAJAMAIL_IND = resMaster.P_SBAJAMAIL_IND?.Trim();
            }

            if (resToComplete.SISCLIENT_GBD != null && resToComplete.SISCLIENT_GBD?.Trim() != "0")
            {

                if (resToComplete.SISCLIENT_GBD?.Trim() != resMaster.P_SISCLIENT_GBD?.Trim())
                {
                    clientBindingModel.P_SISCLIENT_GBD = resToComplete.SISCLIENT_GBD?.Trim();
                }
                else
                {
                    clientBindingModel.P_SISCLIENT_GBD = resMaster.P_SISCLIENT_GBD?.Trim();
                }
            }
            else
            {
                clientBindingModel.P_SISCLIENT_GBD = resMaster.P_SISCLIENT_GBD?.Trim();
            }
            CiiuBindingModel ciiu = new CiiuBindingModel();
            clientBindingModel.EListCIIUClient = new List<CiiuBindingModel>();
            //if (resMaster.EListCIIUClient.Count() > 0) {
            //    clientBindingModel.EListCIIUClient = _mapper.Map<List<CiiuViewModel>,List<CiiuBindingModel>>(resMaster.EListCIIUClient);
            //}
            //if (resToComplete.COD_CIIU != null)
            //{
            //    ciiu.P_NROW = "1";
            //    ciiu.P_TipOper = null;
            //    ciiu.P_DEFFECDATE = DateTime.Now.ToString("dd/MM/yyyy HH24:MI:SS");
            //    ciiu.P_SCIIU = resToComplete.COD_CIIU;


            //    clientBindingModel.EListCIIUClient.Add(ciiu);
            //}
            clientBindingModel.ElistDocumentClient = new List<DocumentosBindingModel>();
            clientBindingModel.ElistInfoBancariaClient = new List<InfoBancariaBindingModel>();
            if (resToComplete.COD_CIIU != null && resToComplete.COD_CIIU?.Trim() != "")
            {
                if (resMaster.P_NSPECIALITY != resToComplete.COD_CIIU?.Trim())
                {
                    clientBindingModel.P_NSPECIALITY = resToComplete.COD_CIIU?.Trim();
                }

            }

            clientBindingModel.EListAddresClient = new List<AddressBindingModel>();

            AddressBindingModel adr = new AddressBindingModel();
            clientBindingModel.EListContactClient = new List<ContactBindingModel>();

            //if (resMaster.EListAddresClient.Count > 0) {

            //    clientBindingModel.EListAddresClient = _mapper.Map<List<AddressViewModel>,List<AddressBindingModel>>(resMaster.EListAddresClient);
            //}
            if (resToComplete.STI_DIRE?.Trim() != "0" && resToComplete.STI_DIRE?.Trim() != null)
            {
                adr.P_ADDRESSTYPE = resToComplete.ADDRESSTYPE == null ? null : resToComplete.ADDRESSTYPE.Trim();
                adr.P_SRECTYPE = resToComplete.ADDRESSTYPE == null ? null : resToComplete.ADDRESSTYPE.Trim();
                adr.P_STI_DIRE = resToComplete.STI_DIRE == null ? null : resToComplete.STI_DIRE.Trim();
                adr.P_SNOM_DIRECCION = resToComplete.SNOM_DIRECCION == null ? null : resToComplete.SNOM_DIRECCION?.Trim();
                adr.P_SNUM_DIRECCION = resToComplete.SNUM_DIRECCION == null ? null : resToComplete.SNUM_DIRECCION?.Trim();
                adr.P_STI_BLOCKCHALET = resToComplete.STI_BLOCKCHALET?.Trim() == "0" ? null : resToComplete.STI_BLOCKCHALET?.Trim();
                adr.P_SBLOCKCHALET = resToComplete.SBLOCKCHALET == null ? null : resToComplete.SBLOCKCHALET?.Trim();

                adr.P_STI_INTERIOR = resToComplete.STI_INTERIOR?.Trim() == "0" ? null : resToComplete.STI_INTERIOR?.Trim();
                adr.P_SNUM_INTERIOR = resToComplete.SNUM_INTERIOR == null ? null : resToComplete.SNUM_INTERIOR?.Trim();
                adr.P_STI_CJHT = resToComplete.STI_CJHT?.Trim() == "0" ? null : resToComplete.STI_CJHT?.Trim();
                adr.P_SNOM_CJHT = resToComplete.SNOM_CJHT == null ? null : resToComplete.SNOM_CJHT?.Trim();
                adr.P_SETAPA = resToComplete.SETAPA == null ? null : resToComplete.SETAPA?.Trim();
                adr.P_SMANZANA = resToComplete.SMANZANA == null ? null : resToComplete.SMANZANA?.Trim();
                adr.P_SLOTE = resToComplete.SLOTE == null ? null : resToComplete.SLOTE?.Trim();
                adr.P_SREFERENCE = resToComplete.SREFERENCIA == null ? null : resToComplete.SREFERENCIA?.Trim();
                adr.P_NMUNICIPALITY = resToComplete.NMUNICIPALITY == null ? null : resToComplete.NMUNICIPALITY?.Trim();
                adr.P_NCOUNTRY = resToComplete.NCOUNTRY == null ? null : resToComplete.NCOUNTRY?.Trim();
                adr.P_NLOCAL = resToComplete?.NLOCAL == null ? null : resToComplete.NLOCAL.ToString();
                adr.P_NPROVINCE = resToComplete?.NPROVINCE == null ? null : resToComplete.NPROVINCE.ToString();
                clientBindingModel.EListAddresClient.Add(adr);
            }


            clientBindingModel.EListPhoneClient = new List<PhoneBindingModel>();

            PhoneBindingModel phone = new PhoneBindingModel();

            //if (resMaster.EListPhoneClient.Count > 0) {
            //    clientBindingModel.EListPhoneClient = _mapper.Map<List<PhoneViewModel>,List<PhoneBindingModel>>(resMaster.EListPhoneClient);
            //}
            if (resToComplete.NPHONE_TYPE?.Trim() != "0" && resToComplete.NPHONE_TYPE?.Trim() != null)
            {
                phone.P_NAREA_CODE = resToComplete.NAREA_CODE == null ? null : resToComplete.NAREA_CODE?.Trim();
                phone.P_NPHONE_TYPE = resToComplete.NPHONE_TYPE == null ? null : resToComplete.NPHONE_TYPE?.Trim();
                phone.P_SPHONE = resToComplete.SPHONE == null ? null : resToComplete.SPHONE?.Trim();
                clientBindingModel.EListPhoneClient.Add(phone);
            }


            clientBindingModel.EListEmailClient = new List<EmailBindingModel>();

            EmailBindingModel emailBindingModel = new EmailBindingModel();

            //if (resMaster.EListEmailClient.Count > 0) {
            //    clientBindingModel.EListEmailClient = _mapper.Map<List<EmailViewModel>,List<EmailBindingModel>>(resMaster.EListEmailClient);
            //}
            if (resToComplete.SEMAILTYPE?.Trim() != "0" && resToComplete.SEMAILTYPE?.Trim() != null)
            {
                emailBindingModel.P_SEMAILTYPE = resToComplete.SEMAILTYPE == null ? null : resToComplete.SEMAILTYPE?.Trim();
                emailBindingModel.P_SE_MAIL = resToComplete.SE_MAIL == null ? null : resToComplete.SE_MAIL?.Trim();
                emailBindingModel.P_SRECTYPE = resToComplete.SEMAILTYPE == null ? null : resToComplete.SEMAILTYPE?.Trim();
                clientBindingModel.EListEmailClient.Add(emailBindingModel);
            }
            if (resMaster.P_SISSEACSA_IND == "1")
            {
                clientBindingModel.P_SMESSAGE_SEACSA = respRentas;
                clientBindingModel.P_IS_RENTAS = true;
            }

            return clientBindingModel;
        }
        public async Task<ResponseViewModel> updateJiraState(WebHookPayloadModel model)
        {
            ResponseViewModel res = new ResponseViewModel();
            string[] statusValid = { "10212", "11400", "10211", "10400", "11401", "10700" };
            string derivationArea = "";
            string denvio = "0";
            string dateFired = "";
            string attendedDate = "0";



            if (!statusValid.Contains(model.issue.fields.status.id))
            {
                res.P_NCODE = "2";
                res.P_SMESSAGE = "Este estado del issue no se maneja : " + model.issue.fields.status.id;
                return res;
            }
            if (model.issue?.key == null || model.issue?.key == "")
            {
                res.P_NCODE = "2";
                res.P_SMESSAGE = "No existe el código de Jira";
                return res;
            }
            string dateParsed = "";
            string dates = "";

            try
            {

                switch (model.issue?.fields?.project?.key)
                {
                    case "TRA":
                        if (model.issue?.fields?.status?.id == "11400")//derivado
                        {
                            derivationArea = model.issue?.fields?.customfield_12229?.value;

                            //dateFired = parseFormatDate(model.issue?.fields?.customfield_12301);
                            if (model.issue?.fields?.customfield_12301 != null && model.issue?.fields?.customfield_12301 != "")
                            {
                                dates = model.issue?.fields?.customfield_12301;//Substring(0, 19);
                                if (dates.Length > 0)
                                {
                                    dateParsed = dates;
                                }
                            }
                            dateFired = parseFormatDate(dateParsed);
                        }
                        else if (model.issue?.fields?.status?.id == "10211")//cerrado 
                        {
                            denvio = "1";
                            model.issue.fields.status.id = "12001";

                            if (model.issue?.fields?.customfield_12319 != null && model.issue?.fields?.customfield_12319 != "")
                            {
                                dates = model.issue?.fields?.customfield_12319;//.Substring(0, 19);
                                if (dates.Length > 0)
                                {
                                    dateParsed = dates;
                                }
                            }
                            dateFired = parseFormatDate(dateParsed);
                        }
                        else if (model.issue?.fields?.status?.id == "10212")//cancelado
                        {
                            denvio = "1";
                            if (model.issue?.fields?.customfield_12427 != null && model.issue?.fields?.customfield_12427 != "")
                            {
                                dates = model.issue?.fields?.customfield_12427;//.Substring(0, 19);
                                if (dates.Length > 0)
                                {
                                    dateParsed = dates;
                                }
                            }
                            dateFired = parseFormatDate(dateParsed);
                        }
                        else if (model.issue?.fields?.status?.id == "11401")//atendido
                        {
                            attendedDate = "1";
                            if (model.issue?.fields?.customfield_12315 != null && model.issue?.fields?.customfield_12315 != "")
                            {
                                dates = model.issue?.fields?.customfield_12315;//.Substring(0, 19);
                                if (dates.Length > 0)
                                {
                                    dateParsed = dates;
                                }
                            }
                            dateFired = parseFormatDate(dateParsed);
                        }
                        break;

                    case "RYS":
                        if (model.issue?.fields?.status?.id == "11400")//derivado
                        {
                            if (model.issue?.fields?.customfield_12314 != null && model.issue?.fields?.customfield_12314 != "")
                            {
                                dates = model.issue?.fields?.customfield_12314;//.Substring(0,19);
                                if (dates.Length > 0)
                                {
                                    dateParsed = dates;
                                }
                            }
                            dateFired = parseFormatDate(dateParsed);
                            if (model.issue?.fields?.customfield_12308.Count > 0)
                            {
                                derivationArea = model.issue?.fields?.customfield_12308?[0].value;//model.issue?.fields?.subtasks[0].fields?.summary?.Substring(17, model.issue.fields.subtasks[0].fields.summary.Length - 17);
                            }

                        }
                        else if (model.issue?.fields?.status?.id == "10212") // cancelado
                        {
                            denvio = "1";
                            if (model.issue?.fields?.customfield_12427 != null && model.issue?.fields?.customfield_12427 != "")
                            {
                                dates = model.issue?.fields?.customfield_12427;//.Substring(0,19);
                                if (dates.Length > 0)
                                {
                                    dateParsed = dates;
                                }
                            }
                            dateFired = parseFormatDate(dateParsed);
                        }
                        else if (model.issue?.fields?.status?.id == "11401") // atendido
                        {
                            denvio = "1";
                            if (model.issue?.fields?.customfield_12427 != null && model.issue?.fields?.customfield_12427 != "")
                            {
                                dates = model.issue?.fields?.customfield_12427;//.Substring(0,19);
                                if (dates.Length > 0)
                                {
                                    dateParsed = dates;
                                }
                            }
                            dateFired = parseFormatDate(dateParsed);
                        }
                        //else if (model.issue?.fields?.status?.id == "10211") //cerrado
                        //{

                        //    dateFired = parseFormatDate(dateParsed);
                        //}
                        break;
                    //case "ADB":
                    //if (model.issue?.fields?.status?.id == "11400")//derivado
                    //{
                    //    derivationArea = model.issue?.fields?.customfield_12229?.value;

                    //    //dateFired = parseFormatDate(model.issue?.fields?.customfield_12301);
                    //    if (model.issue?.fields?.customfield_12301 != null && model.issue?.fields?.customfield_12301 != "")
                    //    {
                    //        dates = model.issue?.fields?.customfield_12301;//Substring(0, 19);
                    //        if (dates.Length > 0)
                    //        {
                    //            dateParsed = dates;
                    //        }
                    //    }
                    //    dateFired = parseFormatDate(dateParsed);
                    //}
                    //break;
                    default:

                        break;

                }
                res = await _cargaMasivaRepository.updateState(model.issue.key, model.issue.fields.status.id, derivationArea, denvio, dateFired, attendedDate);

            }
            catch (Exception ex)
            {
                res.P_COD_ERR = "2";
                res.P_MESSAGE = ex.Message;
            }
            res.P_CAMPO = dateFired;
            return res;
        }

        private string parseFormatDate(string datestring)
        {
            string resulDate = "";

            string[] allowedFormats = { "dd/MM/yyyy", "d/M/yyyy", "d/M/yyyy hh:mm:ss tt", "dd/MM/yyyy hh:mm:ss tt", "MM/dd/yyyy hh:mm:ss tt", "yyyy-MM-ddTHH:mm:ss", "yyyy-MM-ddTHH:mm:ss.fffK", "yyyy-MM-ddTHH:mm:ssK" };
            if (datestring != null && datestring != "")
            {
                try
                {
                    if (DateTime.TryParseExact(datestring, allowedFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        resulDate = date.ToString("MM/dd/yyyy HH:mm:ss");
                    }
                    else
                    {
                        resulDate = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(datestring)).DateTime.ToString("MM/dd/yyyy HH:mm:ss");
                    }
                }
                catch (Exception ex)
                {

                    resulDate = "";

                    return resulDate;
                }
            }



            return resulDate;
        }

        public async Task<ResponseViewModel> updateStateConcurrence(ResponseViewModel payload)
        {
            ResponseViewModel res = new ResponseViewModel();


            try
            {
                res = await _cargaMasivaRepository.updateStateConcurrence(payload);

            }
            catch (Exception ex)
            {
                res.P_COD_ERR = "2";
                res.P_MESSAGE = ex.Message;
            }

            return res;
        }


    }
}
