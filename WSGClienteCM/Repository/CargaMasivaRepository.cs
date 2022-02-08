using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using WSGClienteCM.Connection;
using WSGClienteCM.Models;

namespace WSGClienteCM.Repository
{
    public class CargaMasivaRepository : ICargaMasivaRepository
    {
        private readonly IConnectionBase _connectionBase;
        public CargaMasivaRepository(IConnectionBase ConnectionBase)
        {

            _connectionBase = ConnectionBase;
        }
        private string Package3 = "PKG_BDU_CLIENTE_CM";



        public ResponseViewModel testQuartz()
        {

            ResponseViewModel res = new ResponseViewModel();
            try
            {
                OracleParameter P_NCODE = new OracleParameter("P_ROWS_UPDATED", OracleDbType.Int32, ParameterDirection.Output);
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NROPROCESO_CAB", OracleDbType.Varchar2, Guid.NewGuid().ToString().Substring(0, 5).GetHashCode(), ParameterDirection.Input));
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
                    res.P_MESSAGE = v_P_NCODE;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return res;

        }

        public async Task<ResponseViewModel> InsertHeader(ClientBindingModel item, DbConnection cn, DbTransaction trx)
        {

            ResponseViewModel res = new ResponseViewModel();
            try
            {

                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_MESSAGE.Size = 4000;
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_SNROPROCESO_CAB", OracleDbType.Varchar2, item.P_SNOPROCESO, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SCODAPLICACION", OracleDbType.Varchar2, item.P_CodAplicacion, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STITLE", OracleDbType.Varchar2, item.P_NTITLE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSPECIALITY", OracleDbType.Varchar2, item.P_NSPECIALITY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NUSERCODE", OracleDbType.Int32, item.P_NUSERCODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NDETAILCOUNT", OracleDbType.Int32, item.P_NDETAILCOUNT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SFILENAME", OracleDbType.Varchar2, item.P_SFILENAME, ParameterDirection.Input));
                //parameters.Add(new OracleParameter("P_SSTATE", OracleDbType.Char, '0', ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_TIPOPER", OracleDbType.Varchar2, item.P_TipOper, ParameterDirection.Input));
                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);

                using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync_TRX(string.Format("{0}.{1}", Package3, "INS_UPD_CLIENT_CM_CAB"), parameters, cn, trx, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    res.P_COD_ERR = P_COD_ERR.Value.ToString();
                    res.P_MESSAGE = P_MESSAGE.Value.ToString();
                }
            }
            catch (Exception ex)
            {
                res.P_COD_ERR = "2";
                res.P_MESSAGE = ex.Message;
                throw ex;
            }

            return res;

        }


        public async Task<ResponseViewModel> InsertDetail(List<ClientBindingModel> items, DbConnection cn, DbTransaction trx)
        {

            ResponseViewModel res = new ResponseViewModel();
            try
            {
                int iteration = 0;
                foreach (ClientBindingModel item in items)
                {
                    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                    P_MESSAGE.Size = 4000;
                    OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                    List<OracleParameter> parameters = new List<OracleParameter>();
                    parameters.Add(new OracleParameter("P_TIPOPER", OracleDbType.Varchar2, item.P_TipOper, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Char, item.P_NIDDOC_TYPE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, item.P_SIDDOC, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, item.P_SFIRSTNAME, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, item.P_SLASTNAME, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, item.P_SLASTNAME2, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SLEGALNAME", OracleDbType.Varchar2, item.P_SLEGALNAME, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.Char, item.P_SSEXCLIEN, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NCIVILSTA", OracleDbType.Char, item.P_NCIVILSTA, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NNATIONALITY", OracleDbType.Varchar2, item.P_NNATIONALITY, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, item.P_DBIRTHDAT, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("P_ADDRESSTYPE", OracleDbType.Char, item.EListAddresClient[0].P_ADDRESSTYPE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_STI_DIRE", OracleDbType.Char, item.EListAddresClient[0].P_STI_DIRE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SNOM_DIRECCION", OracleDbType.Varchar2, item.EListAddresClient[0].P_SNOM_DIRECCION, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SNUM_DIRECCION", OracleDbType.Varchar2, item.EListAddresClient[0].P_SNUM_DIRECCION, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_STI_BLOCKCHALET", OracleDbType.Char, item.EListAddresClient[0].P_STI_BLOCKCHALET, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SBLOCKCHALET", OracleDbType.Varchar2, item.EListAddresClient[0].P_SBLOCKCHALET, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_STI_INTERIOR", OracleDbType.Char, item.EListAddresClient[0].P_STI_INTERIOR, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SNUM_INTERIOR", OracleDbType.Varchar2, item.EListAddresClient[0].P_SNUM_INTERIOR, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_STI_CJHT", OracleDbType.Char, item.EListAddresClient[0].P_STI_CJHT, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SNOM_CJHT", OracleDbType.Varchar2, item.EListAddresClient[0].P_SNOM_CJHT, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SETAPA", OracleDbType.Varchar2, item.EListAddresClient[0].P_SETAPA, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SMANZANA", OracleDbType.Varchar2, item.EListAddresClient[0].P_SMANZANA, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SLOTE", OracleDbType.Varchar2, item.EListAddresClient[0].P_SLOTE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SREFERENCIA", OracleDbType.Varchar2, item.EListAddresClient[0].P_SREFERENCIA, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Varchar2, item.EListAddresClient[0].P_NMUNICIPALITY, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NCOUNTRY", OracleDbType.Varchar2, item.EListAddresClient[0].P_NCOUNTRY, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("P_NAREA_CODE", OracleDbType.Char, item.EListPhoneClient[0].P_NAREA_CODE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NPHONE_TYPE", OracleDbType.Char, item.EListPhoneClient[0].P_NPHONE_TYPE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, item.EListPhoneClient[0].P_SPHONE, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("P_SEMAILTYPE", OracleDbType.Char, item.EListEmailClient[0].P_SEMAILTYPE, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, item.EListEmailClient[0].P_SE_MAIL, ParameterDirection.Input));

                    parameters.Add(new OracleParameter("P_COD_CIIU", OracleDbType.Varchar2, item.P_COD_CIIU, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_COD_CUSPP", OracleDbType.Varchar2, item.P_COD_CUSPP, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SPROTEG_DATOS_IND", OracleDbType.Char, item.P_SPROTEG_DATOS_IND, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SBAJAMAIL_IND", OracleDbType.Char, item.P_SBAJAMAIL_IND, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SISCLIENT_GBD", OracleDbType.Char, item.P_SISCLIENT_GBD, ParameterDirection.Input));

                    //parameters.Add(new OracleParameter("P_SPROMOTIONS", OracleDbType.Char, item.P_SPROMOTIONS, ParameterDirection.Input));
                    //parameters.Add(new OracleParameter("P_SDATACONSENT", OracleDbType.Char, item.P_SDATACONSENT, ParameterDirection.Input));
                    //parameters.Add(new OracleParameter("P_SCLIENTGOB", OracleDbType.Char, item.P_SCLIENTGOB, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_SNROPROCESO", OracleDbType.Varchar2, item.P_SNOPROCESO, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_NNUMREG", OracleDbType.Int64, item.P_NNUMREG, ParameterDirection.Input));
                    //parameters.Add(new OracleParameter("P_SSTATE", OracleDbType.Char, '0', ParameterDirection.Input));
                    //parameters.Add(new OracleParameter("P_TIPOPER", OracleDbType.Varchar2, item.P_TipOper, ParameterDirection.Input));
                    parameters.Add(P_MESSAGE);
                    parameters.Add(P_COD_ERR);

                    using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync_TRX(string.Format("{0}.{1}", Package3, "INS_UPD_CLIENT_CM_DET"), parameters, cn, trx, ConnectionBase.enuTypeDataBase.OracleVTime))
                    {
                        if (P_COD_ERR.Value.ToString() == "1")
                        {
                            res.P_COD_ERR = P_COD_ERR.Value.ToString();
                            res.P_MESSAGE = P_MESSAGE.Value.ToString();
                            break;

                        }

                        iteration++;
                        if (iteration == items.Count)
                        {
                            res.P_COD_ERR = "0";
                            res.P_MESSAGE = "Registro exitoso";
                        }

                    }

                }

            }
            catch (Exception ex)
            {
                res.P_COD_ERR = "2";
                res.P_MESSAGE = ex.Message;
                throw ex;
            }

            return res;

        }
        public async Task<ResponseViewModel> GetByState(string code)
        {
            List<DetailBindingModel> detail = new List<DetailBindingModel>();
            ResponseViewModel res = new ResponseViewModel();
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
              
                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_SMESSAGE.Size = 4000;
                OracleParameter P_NCODE = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_TABLA = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(new OracleParameter("P_CODE", OracleDbType.Varchar2, code, ParameterDirection.Input));
                parameters.Add(P_SMESSAGE);
                parameters.Add(P_NCODE);
                parameters.Add(P_TABLA);
                using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "GET_BY_STATE"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {

                    detail = dr.ReadRowsList<DetailBindingModel>();

                }
                res.P_COD_ERR = P_NCODE.Value.ToString();
                res.P_MESSAGE = P_SMESSAGE.Value.ToString();
                res.ElistDetail = detail;

            }
            catch (Exception ex)
            {
                res.P_COD_ERR = "2";
                res.P_MESSAGE = ex.Message;
            }
            return res;
        }

        public async Task<ResponseViewModel> UpdateStateHeader(List<string> processCodeToUpdate, string value)
        {
            ResponseViewModel res = new ResponseViewModel();
            try
            {
                int iteration = 0;
                foreach (string code in processCodeToUpdate)
                {
                    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                    P_MESSAGE.Size = 4000;
                    OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                    List<OracleParameter> parameters = new List<OracleParameter>();

                    parameters.Add(new OracleParameter("P_SNROPROCESO", OracleDbType.Varchar2, code, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_VALUE", OracleDbType.Char, value, ParameterDirection.Input));

                    parameters.Add(P_MESSAGE);
                    parameters.Add(P_COD_ERR);

                    using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "UPD_STATE_CM_CAB"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                    {
                        if (P_COD_ERR.Value.ToString() == "1")
                        {
                            res.P_COD_ERR = P_COD_ERR.Value.ToString();
                            res.P_MESSAGE = P_MESSAGE.Value.ToString();
                            break;

                        }

                        iteration++;
                        if (iteration == processCodeToUpdate.Count)
                        {
                            res.P_COD_ERR = "0";
                            res.P_MESSAGE = "Modificación exitosa";
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                res.P_COD_ERR = "2";
                res.P_MESSAGE = ex.Message;

            }
            return res;


        }
        public async Task<ResponseViewModel> ValidateRow(DetailBindingModel item)
        {

            ResponseViewModel res = new ResponseViewModel();
            List<ListViewErrores> ElistErrores = new List<ListViewErrores>();
            try
            {


                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_MESSAGE.Size = 4000;
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_CAMPO = new OracleParameter("P_CAMPO", OracleDbType.Varchar2, ParameterDirection.Output);
                P_CAMPO.Size = 4000;
                OracleParameter P_TABLA = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ElistErrores, ParameterDirection.Output);

                List<OracleParameter> parameters = new List<OracleParameter>();

                parameters.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Char, item.NIDDOC_TYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, item.SIDDOC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, item.SFIRSTNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, item.SLASTNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, item.SLASTNAME2, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLEGALNAME", OracleDbType.Varchar2, item.SLEGALNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.Char, item.SSEXCLIEN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCIVILSTA", OracleDbType.Char, item.NCIVILSTA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNATIONALITY", OracleDbType.Varchar2, item.NNATIONALITY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, item.DBIRTHDAT, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_ADDRESSTYPE", OracleDbType.Char, item.ADDRESSTYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STI_DIRE", OracleDbType.Char, item.STI_DIRE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNOM_DIRECCION", OracleDbType.Varchar2, item.SNOM_DIRECCION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNUM_DIRECCION", OracleDbType.Varchar2, item.SNUM_DIRECCION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STI_BLOCKCHALET", OracleDbType.Char, item.STI_BLOCKCHALET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SBLOCKCHALET", OracleDbType.Varchar2, item.SBLOCKCHALET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STI_INTERIOR", OracleDbType.Char, item.STI_INTERIOR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNUM_INTERIOR", OracleDbType.Varchar2, item.SNUM_INTERIOR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STI_CJHT", OracleDbType.Char, item.STI_CJHT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNOM_CJHT", OracleDbType.Varchar2, item.SNOM_CJHT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SETAPA", OracleDbType.Varchar2, item.SETAPA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMANZANA", OracleDbType.Varchar2, item.SMANZANA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLOTE", OracleDbType.Varchar2, item.SLOTE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SREFERENCIA", OracleDbType.Varchar2, item.SREFERENCIA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Varchar2, item.NMUNICIPALITY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCOUNTRY", OracleDbType.Varchar2, item.NCOUNTRY, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_NAREA_CODE", OracleDbType.Char, item.NAREA_CODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPHONE_TYPE", OracleDbType.Char, item.NPHONE_TYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, item.SPHONE, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_SEMAILTYPE", OracleDbType.Char, item.SEMAILTYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, item.SE_MAIL, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_COD_CIIU", OracleDbType.Varchar2, item.COD_CIIU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_COD_CUSPP", OracleDbType.Varchar2, item.COD_CUSPP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPROTEG_DATOS_IND", OracleDbType.Char, item.SPROTEG_DATOS_IND, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SBAJAMAIL_IND", OracleDbType.Char, item.SBAJAMAIL_IND, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SISCLIENT_GBD", OracleDbType.Char, item.SISCLIENT_GBD, ParameterDirection.Input));

                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);
                parameters.Add(P_TABLA);
                parameters.Add(P_CAMPO);
                using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "VAL_CLIENT_CM_DET"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {

                    ElistErrores = dr.ReadRowsList<ListViewErrores>();

                }
                res.P_COD_ERR = P_COD_ERR.Value.ToString();
                res.P_MESSAGE = P_MESSAGE.Value.ToString();
                res.P_CAMPO = P_CAMPO.Value.ToString();
                res.EListErrores = ElistErrores;



             

            }catch (Exception ex)
            {
                res.P_COD_ERR = "2";
                res.P_MESSAGE = ex.Message;
               

            }
            return res;
            }


             

       
        public async Task<ResponseViewModel> SaveStateRow(DetailBindingModel item)
        {

            ResponseViewModel res = new ResponseViewModel();

            try
            {
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_MESSAGE.Size = 4000;
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_COD_INSERTED = new OracleParameter("P_INSERTED_ID_AUX", OracleDbType.Decimal, ParameterDirection.Output);



                List<OracleParameter> parameters = new List<OracleParameter>();
                parameters.Add(new OracleParameter("P_NNROPROCESO_DET", OracleDbType.Int64, item.NNROPROCESO_DET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NIDDOC_TYPE", OracleDbType.Varchar2, item.NIDDOC_TYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SIDDOC", OracleDbType.Varchar2, item.SIDDOC, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SFIRSTNAME", OracleDbType.Varchar2, item.SFIRSTNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME", OracleDbType.Varchar2, item.SLASTNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLASTNAME2", OracleDbType.Varchar2, item.SLASTNAME2, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLEGALNAME", OracleDbType.Varchar2, item.SLEGALNAME, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SSEXCLIEN", OracleDbType.Varchar2, item.SSEXCLIEN, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCIVILSTA", OracleDbType.Varchar2, item.NCIVILSTA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NNATIONALITY", OracleDbType.Varchar2, item.NNATIONALITY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_DBIRTHDAT", OracleDbType.Varchar2, item.DBIRTHDAT, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_ADDRESSTYPE", OracleDbType.Varchar2, item.ADDRESSTYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STI_DIRE", OracleDbType.Varchar2, item.STI_DIRE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNOM_DIRECCION", OracleDbType.Varchar2, item.SNOM_DIRECCION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNUM_DIRECCION", OracleDbType.Varchar2, item.SNUM_DIRECCION, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STI_BLOCKCHALET", OracleDbType.Varchar2, item.STI_BLOCKCHALET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SBLOCKCHALET", OracleDbType.Varchar2, item.SBLOCKCHALET, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STI_INTERIOR", OracleDbType.Varchar2, item.STI_INTERIOR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNUM_INTERIOR", OracleDbType.Varchar2, item.SNUM_INTERIOR, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_STI_CJHT", OracleDbType.Varchar2, item.STI_CJHT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SNOM_CJHT", OracleDbType.Varchar2, item.SNOM_CJHT, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SETAPA", OracleDbType.Varchar2, item.SETAPA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SMANZANA", OracleDbType.Varchar2, item.SMANZANA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SLOTE", OracleDbType.Varchar2, item.SLOTE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SREFERENCIA", OracleDbType.Varchar2, item.SREFERENCIA, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NMUNICIPALITY", OracleDbType.Varchar2, item.NMUNICIPALITY, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NCOUNTRY", OracleDbType.Varchar2, item.NCOUNTRY, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_NAREA_CODE", OracleDbType.Varchar2, item.NAREA_CODE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_NPHONE_TYPE", OracleDbType.Varchar2, item.NPHONE_TYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPHONE", OracleDbType.Varchar2, item.SPHONE, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_SEMAILTYPE", OracleDbType.Varchar2, item.SEMAILTYPE, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SE_MAIL", OracleDbType.Varchar2, item.SE_MAIL, ParameterDirection.Input));

                parameters.Add(new OracleParameter("P_COD_CIIU", OracleDbType.Varchar2, item.COD_CIIU, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_COD_CUSPP", OracleDbType.Varchar2, item.COD_CUSPP, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SPROTEG_DATOS_IND", OracleDbType.Varchar2, item.SPROTEG_DATOS_IND, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SBAJAMAIL_IND", OracleDbType.Varchar2, item.SBAJAMAIL_IND, ParameterDirection.Input));
                parameters.Add(new OracleParameter("P_SISCLIENT_GBD", OracleDbType.Varchar2, item.SISCLIENT_GBD, ParameterDirection.Input));
               

                parameters.Add(P_COD_INSERTED);
                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);


                using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "INS_STATE_CM_DET"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {
                    res.P_COD_ERR = P_COD_ERR.Value.ToString();
                    res.P_MESSAGE = P_MESSAGE.Value.ToString();
                    res.P_NIDCM = P_COD_INSERTED.Value.ToString();
                }


            }
            catch (Exception ex) {
                res.P_COD_ERR = "2";
                res.P_MESSAGE = ex.Message;
                res.P_NIDCM = "0";


            }
            return res;

           
        }
        public async Task<ResponseViewModel> UpdateStateResponse(int nid, string value,int state)
        {
            ResponseViewModel res = new ResponseViewModel();
            try
            {
               
               
                    OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                    P_MESSAGE.Size = 4000;
                    OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                    List<OracleParameter> parameters = new List<OracleParameter>();

                    parameters.Add(new OracleParameter("P_ID", OracleDbType.Int32, nid, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_VALUE", OracleDbType.Varchar2, value, ParameterDirection.Input));
                    parameters.Add(new OracleParameter("P_STATE", OracleDbType.Varchar2, state, ParameterDirection.Input));

                    parameters.Add(P_MESSAGE);
                    parameters.Add(P_COD_ERR);

                    using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "UPD_STATE_RESPONSE"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                    {
                            res.P_COD_ERR = P_COD_ERR.Value.ToString();
                            res.P_MESSAGE = P_MESSAGE.Value.ToString();
                    }

                
            }
            catch (Exception ex)
            {
                res.P_COD_ERR = "2";
                res.P_MESSAGE = ex.Message;

            }
            return res;


        }

        public async Task<ResponseViewModel> GetHeadersByState(string state)
        {
            List<HeaderBindingModel> headers = new List<HeaderBindingModel>();
            ResponseViewModel res = new ResponseViewModel();
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {

                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_SMESSAGE.Size = 4000;
                OracleParameter P_NCODE = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_TABLA = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);
                parameters.Add(new OracleParameter("P_STATE", OracleDbType.Varchar2, state, ParameterDirection.Input));
                parameters.Add(P_SMESSAGE);
                parameters.Add(P_NCODE);
                parameters.Add(P_TABLA);
                using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "GET_HEADERS_BY_STATE"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
                {

                    headers = dr.ReadRowsList<HeaderBindingModel>();

                }
                res.P_COD_ERR = P_NCODE.Value.ToString();
                res.P_MESSAGE = P_SMESSAGE.Value.ToString();
                res.ElistHeaders = headers;

            }
            catch (Exception ex)
            {
                res.P_COD_ERR = "2";
                res.P_MESSAGE = ex.Message;
            }
            return res;
        }

        //hcama@mg 26.01.2021 ini 
        public async Task<TramaRespuestaCargaMasivaResponse> ObtenerTramaEnvioExitosa(string P_SNOPROCESO)
        {
           // var sPackageName = "PKG_BDU_CLIENTE_CM_HCAMA.SP_OBTENER_TRAMA_ENVIO_EXITOSA";
            List<OracleParameter> parameter = new List<OracleParameter>();

            ClientBindingModel clientBindingModel;
            AddressBindingModel addressBindingModel;
            PhoneBindingModel phoneBindingModel;
            EmailBindingModel emailBindingModel;

            List<ClientBindingModel> ListClientBindingModel = new List<ClientBindingModel>();
            List<AddressBindingModel> ListAddressBindingModel = new List<AddressBindingModel>();
            List<PhoneBindingModel> ListPhoneBindingModel = new List<PhoneBindingModel>();
            List<EmailBindingModel> ListEmailBindingModel = new List<EmailBindingModel>();

            TramaRespuestaCargaMasivaResponse response = new TramaRespuestaCargaMasivaResponse();

            try
            {
                //INPUT     

                parameter.Add(new OracleParameter("P_SNOPROCESO", OracleDbType.Varchar2, P_SNOPROCESO, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(C_TABLE);

                try
                {
                    using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "SP_OBTENER_TRAMA_ENVIO_EXITOSA"), parameter, ConnectionBase.enuTypeDataBase.OracleVTime))
                    {
                        while (dr.Read())
                        {
                            clientBindingModel = new ClientBindingModel();
                            addressBindingModel = new AddressBindingModel();
                            phoneBindingModel = new PhoneBindingModel();
                            emailBindingModel = new EmailBindingModel();

                            clientBindingModel.P_SNOPROCESO = dr["P_SNOPROCESO"].ToString();
                            clientBindingModel.P_NNUMREG = Int64.Parse(dr["P_NNUMREG"].ToString());
                            clientBindingModel.P_SFILENAME = dr["P_SFILENAME"].ToString();

                            clientBindingModel.P_NIDDOC_TYPE = dr["P_NIDDOC_TYPE"].ToString();
                            clientBindingModel.P_SIDDOC = dr["P_SIDDOC"].ToString();
                            clientBindingModel.P_SFIRSTNAME = dr["P_SFIRSTNAME"].ToString();
                            clientBindingModel.P_SLASTNAME = dr["P_SLASTNAME"].ToString();
                            clientBindingModel.P_SLASTNAME2 = dr["P_SLASTNAME2"].ToString();
                            clientBindingModel.P_SLEGALNAME = dr["P_SLEGALNAME"].ToString();
                            clientBindingModel.P_SSEXCLIEN = dr["P_SSEXCLIEN"].ToString();
                            clientBindingModel.P_NCIVILSTA = dr["P_NCIVILSTA"].ToString();
                            clientBindingModel.P_NNATIONALITY = dr["P_NNATIONALITY"].ToString();
                            clientBindingModel.P_DBIRTHDAT = dr["P_DBIRTHDAT"].ToString();

                            addressBindingModel.P_ADDRESSTYPE = dr["P_ADDRESSTYPE"].ToString();
                            addressBindingModel.P_STI_DIRE = dr["P_STI_DIRE"].ToString();
                            addressBindingModel.P_SNOM_DIRECCION = dr["P_SNOM_DIRECCION"].ToString();
                            addressBindingModel.P_SNUM_DIRECCION = dr["P_SNUM_DIRECCION"].ToString();
                            addressBindingModel.P_STI_BLOCKCHALET = dr["P_STI_BLOCKCHALET"].ToString();
                            addressBindingModel.P_SBLOCKCHALET = dr["P_SBLOCKCHALET"].ToString();
                            addressBindingModel.P_STI_INTERIOR = dr["P_STI_INTERIOR"].ToString();
                            addressBindingModel.P_SNUM_INTERIOR = dr["P_SNUM_INTERIOR"].ToString();
                            addressBindingModel.P_STI_CJHT = dr["P_STI_CJHT"].ToString();
                            addressBindingModel.P_SNOM_CJHT = dr["P_SNOM_CJHT"].ToString();
                            addressBindingModel.P_SETAPA = dr["P_SETAPA"].ToString();
                            addressBindingModel.P_SMANZANA = dr["P_SMANZANA"].ToString();
                            addressBindingModel.P_SLOTE = dr["P_SLOTE"].ToString();
                            addressBindingModel.P_SREFERENCIA = dr["P_SREFERENCIA"].ToString();
                            addressBindingModel.P_NMUNICIPALITY = dr["P_NMUNICIPALITY"].ToString();
                            addressBindingModel.P_NCOUNTRY = dr["P_NCOUNTRY"].ToString();

                            phoneBindingModel.P_NAREA_CODE = dr["P_NAREA_CODE"].ToString();
                            phoneBindingModel.P_NPHONE_TYPE = dr["P_NPHONE_TYPE"].ToString();
                            phoneBindingModel.P_SPHONE = dr["P_SPHONE"].ToString();

                            emailBindingModel.P_SEMAILTYPE = dr["P_SEMAILTYPE"].ToString();
                            emailBindingModel.P_SE_MAIL = dr["P_SE_MAIL"].ToString();

                            clientBindingModel.P_COD_CIIU = dr["P_COD_CIIU"].ToString();
                            clientBindingModel.P_COD_CUSPP = dr["P_COD_CUSPP"].ToString();
                            //  clientBindingModel.P_SISCLIENT_IND = dr["P_SISCLIENT_IND"].ToString();
                            clientBindingModel.P_SBAJAMAIL_IND = dr["P_SBAJAMAIL_IND"].ToString();
                            clientBindingModel.P_SISCLIENT_GBD = dr["P_SISCLIENT_GBD"].ToString();
                            //  clientBindingModel.P_SPROMOTIONS = dr["P_SPROMOTIONS"].ToString();
                            //  clientBindingModel.P_SDATACONSENT = dr["P_SDATACONSENT"].ToString();
                            //  clientBindingModel.P_SCLIENTGOB = dr["P_SCLIENTGOB"].ToString();

                            ListAddressBindingModel.Add(addressBindingModel);
                            ListPhoneBindingModel.Add(phoneBindingModel);
                            ListEmailBindingModel.Add(emailBindingModel);

                            clientBindingModel.EListAddresClient = ListAddressBindingModel;
                            clientBindingModel.EListPhoneClient = ListPhoneBindingModel;
                            clientBindingModel.EListEmailClient = ListEmailBindingModel;

                            ListClientBindingModel.Add(clientBindingModel);
                        }


                        response.tramaExitosa = ListClientBindingModel;
                        dr.Close();
                    }
                    if (response.tramaExitosa.Count > 0)
                    {
                        response.respuesta = true;
                        response.mensajes.Add("Se encontraron registros");
                    }
                    else
                    {
                        response.respuesta = false;
                        response.mensajes.Add("No se encontraron registros");
                    }
                }
                catch (Exception ex)
                {
                    return new TramaRespuestaCargaMasivaResponse { respuesta = false, mensajes = new List<string> { "Hubo un error en la consulta de trama exitosa", ex.Message }, tramaExitosa = new List<ClientBindingModel>() };
                }


                response.codigoRespuesta = P_NCODE.Value.ToString();
                response.mensajeRespuesta = P_SMESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }


        public async Task<TramaRespuestaCargaMasivaResponse> ObtenerTramaEnvioErrores(string P_SNOPROCESO)
        {
            //var sPackageName = "PKG_BDU_CLIENTE_CM_HCAMA.SP_OBTENER_TRAMA_ENVIO_ERRORES";
            List<OracleParameter> parameter = new List<OracleParameter>();

            ClientBindingModel clientBindingModel;
            List<ClientBindingModel> ListClientBindingModel = new List<ClientBindingModel>();
            TramaRespuestaCargaMasivaResponse response = new TramaRespuestaCargaMasivaResponse();

            try
            {
                //INPUT     

                parameter.Add(new OracleParameter("P_SNOPROCESO", OracleDbType.Varchar2, P_SNOPROCESO, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(C_TABLE);

                try
                {
                    using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "SP_OBTENER_TRAMA_ENVIO_ERRORES"), parameter, ConnectionBase.enuTypeDataBase.OracleVTime))
                    {
                        while (dr.Read())
                        {
                            clientBindingModel = new ClientBindingModel();
                            clientBindingModel.P_SNOPROCESO = dr["P_SNOPROCESO"].ToString();
                            clientBindingModel.P_NNUMREG = Int64.Parse(dr["P_NNUMREG"].ToString());
                            clientBindingModel.P_SFILENAME = dr["P_SFILENAME"].ToString();
                            clientBindingModel.P_SCOLUMNNAME = dr["P_SCOLUMNNAME"].ToString();
                            clientBindingModel.P_SCOLUMNVALUE = dr["P_SCOLUMNVALUE"].ToString();
                            clientBindingModel.P_SERRORVALUE = dr["P_SERRORVALUE"].ToString();
                            clientBindingModel.P_NUSERNAME = dr["P_NUSERNAME"].ToString();

                            ListClientBindingModel.Add(clientBindingModel);
                        }


                        response.tramaErrores = ListClientBindingModel;
                        dr.Close();
                    }
                    if (response.tramaErrores.Count > 0)
                    {
                        response.respuesta = true;
                        response.mensajes.Add("Se encontraron registros");
                    }
                    else
                    {
                        response.respuesta = false;
                        response.mensajes.Add("No se encontraron registros");
                    }
                }
                catch (Exception ex)
                {
                    return new TramaRespuestaCargaMasivaResponse { respuesta = false, mensajes = new List<string> { "Hubo un error en la consulta de la trama de  errores", ex.Message }, tramaErrores = new List<ClientBindingModel>() };
                }


                response.codigoRespuesta = P_NCODE.Value.ToString();
                response.mensajeRespuesta = P_SMESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }

        public async Task<TramaRespuestaCargaMasivaResponse> ObtenerListaUsuariosEnvioTrama(string P_SNOPROCESO)
        {
            //var sPackageName = "PKG_BDU_CLIENTE_CM_HCAMA.SP_OBTENER_LISTA_USUARIOS_ENVIO_TRAMA";
            List<OracleParameter> parameter = new List<OracleParameter>();

            EmailViewModel emailViewModel;
            List<EmailViewModel> ListEmailViewModel = new List<EmailViewModel>();
            TramaRespuestaCargaMasivaResponse response = new TramaRespuestaCargaMasivaResponse();

            try
            {
                //INPUT     

                parameter.Add(new OracleParameter("P_SNOPROCESO", OracleDbType.Varchar2, P_SNOPROCESO, ParameterDirection.Input));

                OracleParameter P_NCODE = new OracleParameter("P_NCODE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter P_SMESSAGE = new OracleParameter("P_SMESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                OracleParameter C_TABLE = new OracleParameter("C_TABLE", OracleDbType.RefCursor, ParameterDirection.Output);

                P_NCODE.Size = 4000;
                P_SMESSAGE.Size = 4000;

                parameter.Add(P_NCODE);
                parameter.Add(P_SMESSAGE);
                parameter.Add(C_TABLE);

                try
                {
                    using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "SP_OBTENER_LISTA_USUARIOS_ENVIO_TRAMA"), parameter, ConnectionBase.enuTypeDataBase.OracleVTime))
                    {
                        while (dr.Read())
                        {
                            emailViewModel = new EmailViewModel();
                            emailViewModel.P_SE_MAIL = dr["P_SE_MAIL"].ToString();

                            ListEmailViewModel.Add(emailViewModel);
                        }


                        response.correoUsuarios = ListEmailViewModel;
                        dr.Close();
                    }
                    if (response.correoUsuarios.Count > 0)
                    {
                        response.respuesta = true;
                        response.mensajes.Add("Se encontraron registros");
                    }
                    else
                    {
                        response.respuesta = false;
                        response.mensajes.Add("No se encontraron registros");
                    }
                }
                catch (Exception ex)
                {
                    return new TramaRespuestaCargaMasivaResponse { respuesta = false, mensajes = new List<string> { "Hubo un error en la consulta de correos de los usuarios", ex.Message }, correoUsuarios = new List<EmailViewModel>() };
                }


                response.codigoRespuesta = P_NCODE.Value.ToString();
                response.mensajeRespuesta = P_SMESSAGE.Value.ToString();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return response;
        }
        //hcama@mg 26.01.2021 fin 


        //hcama@mg 26.01.2021 fin 
    }
}
