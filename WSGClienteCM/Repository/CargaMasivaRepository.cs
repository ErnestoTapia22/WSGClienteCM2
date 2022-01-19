using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using WSGClienteCM.Connection;
using WSGClienteCM.Models;

namespace WSGClienteCM.Repository
{
    public class CargaMasivaRepository:ICargaMasivaRepository
    {
        private readonly IConnectionBase _connectionBase;
        public CargaMasivaRepository(IConnectionBase ConnectionBase)
        {
          
            _connectionBase = ConnectionBase;
        }
        private string Package3 = "PKG_BDU_CLIENTE_CM";

        public ResponseViewModel SetTemporaryData(List<ClientBindingModel> request)
        {
            ResponseViewModel res = new ResponseViewModel();
           
            try
            {
                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("NTYPETICK", OracleDbType.Varchar2, request[0].P_APELLIDO_CASADA, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NTYPEDOCCLI", OracleDbType.Varchar2, request.tipoDocClient, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("SDOCNUMCLI", OracleDbType.Varchar2, request.docClient, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NTYPEDOCCON", OracleDbType.Varchar2, request.Contacto.tipodoc, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("SDOCNUMCON", OracleDbType.Varchar2, request.Contacto.documento, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("SEMAILTITULAR", OracleDbType.Varchar2, request.EmailTitular, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("SDIRECCIONTTITULAR", OracleDbType.Varchar2, request.DireccionTitular, ParameterDirection.Input));
                ////  parameters.Add(new OracleParameter("SUBIGEOTITULAR", OracleDbType.Varchar2, request.Contacto.documento, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("SUBIGEOTITULAR", OracleDbType.Varchar2, request.UbigeoTitular, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NBRANCH", OracleDbType.Varchar2, request.Ramo, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("P_NPRODUCT", OracleDbType.Varchar2, request.Producto, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NPOLICY", OracleDbType.Varchar2, request.Poliza, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NMONTO", OracleDbType.Varchar2, request.Monto, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("SDESCRIPCION", OracleDbType.Varchar2, request.Descripcion, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NREP", OracleDbType.Varchar2, request.Reconsideracion, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NVIARECP", OracleDbType.Varchar2, request.ViaRecepcion.Id, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NVIARESP", OracleDbType.Varchar2, request.ViaRespuesta.Id, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NMOTIVO", OracleDbType.Varchar2, request.SubMotivo.Motivo.Id, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NSUBMOTIVO", OracleDbType.Varchar2, request.SubMotivo.Id, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NUSER", OracleDbType.Varchar2, request.Usuario, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NCANAL", OracleDbType.Varchar2, request.Canal, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NRECON", OracleDbType.Varchar2, request.Reconsideracion, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("DFECRECEP", OracleDbType.Varchar2, request.FechaRecepcion, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("NESTADO", OracleDbType.Varchar2, request.Estado, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("SRESPUESTA", OracleDbType.Varchar2, request.Respuesta, ParameterDirection.Input));

                //parameters.Add(new OracleParameter("SSIMPLEPRODUCT", OracleDbType.Varchar2, request.ProductSimple, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("SPARTNERTYPE", OracleDbType.Varchar2, request.PartnerType, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("SSBSCODE", OracleDbType.Varchar2, request.SbsCode, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("SCREDITREQUIRE", OracleDbType.Varchar2, request.CreditRequire, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("SCONTRACTFIRM", OracleDbType.Varchar2, request.ContractFirm, ParameterDirection.Input));
                parameters.Add(P_NCODE);
                parameters.Add(P_SMESSAGE);
                parameters.Add(new OracleParameter("RC1", OracleDbType.RefCursor, ParameterDirection.Output));
                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "PRC_SET_TICKET"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    //var v_P_NCODE = P_NCODE.Value.ToString();
                    //List<String> lst_mensaje = new List<String>();
                    //lst_mensaje.Add(P_SMESSAGE.Value.ToString());
                    //var v_respuesta = true;
                    //if (v_P_NCODE != "0")
                    //{
                    //    v_respuesta = false;
                    //}
                    //else
                    //{
                    //    while (dr.Read())
                    //    {
                    //        ticket.Codigo = dr["STICKET"].ToString();

                    //    }
                    //}


                    //     response.mensajeRespuesta = 

                    //ticket.respuesta = v_respuesta;
                    //ticket.mensajes = lst_mensaje;
                    //ticket.codigoRespuestaError = v_P_NCODE;
                }
            }
            catch (Exception ex)
            {
                //WriteErrorLog(ex, "Catch");
                //ticket = new RegiTicketResponse();
                //ticket.respuesta = false;
                //ticket.mensaje = ex.Message;
            }
            return res;
        }

        public ResponseViewModel testQuartz()
        {

            ResponseViewModel res = new ResponseViewModel();
            try {
                OracleParameter P_NCODE = new OracleParameter("P_ROWS_UPDATED", OracleDbType.Int32, ParameterDirection.Output);
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NROPROCESO_CAB", OracleDbType.Varchar2,  Guid.NewGuid().ToString().Substring(0,5).GetHashCode(), ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCODAPLICACION", OracleDbType.Varchar2, "TEST_QUARTZ", ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STITLE", OracleDbType.Varchar2, "TEST_QUARTZ", ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSPECIALITY", OracleDbType.Varchar2, "TEST_QUARTZ", ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, 1, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMREG", OracleDbType.Int32, 3, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SFILENAME", OracleDbType.Varchar2, "TEST_QUARTZ", ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSTATE", OracleDbType.Char, '1', ParameterDirection.Input));
                parameters.Add(P_NCODE);

                using (OracleDataReader dr = (OracleDataReader)_connectionBase.ExecuteByStoredProcedure(string.Format("{0}.{1}", Package3, "INS_UPD_CLIENT_CM_CAB"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    var v_P_NCODE = P_NCODE.Value.ToString();
                    res.P_SMESSAGE = v_P_NCODE;

                }
            }
            catch(Exception ex) {
                throw ex;
            }
           
         return res;

        }

        public async Task<ResponseViewModel> InsertHeader(ClientBindingModel item)
        {

            ResponseViewModel res = new ResponseViewModel();
            try
            {
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_SROPROCESO_CAB", OracleDbType.Varchar2, item.P_SNOPROCESO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCODAPLICACION", OracleDbType.Varchar2, item.P_CodAplicacion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STITLE", OracleDbType.Varchar2, item.P_NTITLE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSPECIALITY", OracleDbType.Varchar2, item.P_NSPECIALITY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, item.P_NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNUMREG", OracleDbType.Int32, item.P_NNUMREG, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SFILENAME", OracleDbType.Varchar2, item.P_SFILENAME, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("P_SSTATE", OracleDbType.Char, '0', ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TIPOPER", OracleDbType.Varchar2, "INS", ParameterDirection.Input)); 
                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);

                using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "TEST_QUARTZ"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    res.P_NCODE = P_COD_ERR.Value.ToString();
                    res.P_SMESSAGE = P_MESSAGE.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                res.P_NCODE = "1";
                res.P_SMESSAGE = ex.Message;
                throw ex;
            }

            return res;

        }


        public async Task<ResponseViewModel> InsertDetail(List<ClientBindingModel> items)
        {

            ResponseViewModel res = new ResponseViewModel();
            try
            {
                foreach (ClientBindingModel item in items)
                {
                    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                    OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                    List<OracleParameter> parameters = new List<OracleParameter>();
                    parameters.Add(new OracleParameter("P_SNROPROCESO_CAB", OracleDbType.Varchar2, item.P_SNOPROCESO, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_TIPOPER", OracleDbType.Varchar2, item.P_TipOper, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, item.P_NIDDOC_TYPE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, item.P_SIDDOC, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, item.P_SFIRSTNAME, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, item.P_SLASTNAME, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, item.P_SLASTNAME2, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SLEGALNAME", OracleDbType.Varchar2, item.P_SLEGALNAME, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.Char, item.P_SSEXCLIEN, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NCIVILSTA", OracleDbType.Varchar2, item.P_NCIVILSTA, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NNATIONALITY", OracleDbType.Varchar2, item.P_NNATIONALITY, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, item.P_DBIRTHDAT, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, item.P_SLASTNAME, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, item.P_SLASTNAME, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, item.P_SLASTNAME, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, item.P_SLASTNAME, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, item.P_SLASTNAME, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, item.P_SLASTNAME, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SFILENAME", OracleDbType.Varchar2, item.P_SFILENAME, ParameterDirection.Input));
                    //parameters.Add(new OracleParameter("P_SSTATE", OracleDbType.Char, '0', ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_TIPOPER", OracleDbType.Varchar2, "INS", ParameterDirection.Input));
                    parameters.Add(P_MESSAGE);
                    parameters.Add(P_COD_ERR);

                    using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "TEST_QUARTZ"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                    {
                        res.P_NCODE = P_COD_ERR.Value.ToString();
                        res.P_SMESSAGE = P_MESSAGE.Value.ToString();
                    }

                }
             
            }
            catch (Exception ex)
            {
                res.P_NCODE = "1";
                res.P_SMESSAGE = ex.Message;
                throw ex;
            }

            return res;

        }
    }
}
