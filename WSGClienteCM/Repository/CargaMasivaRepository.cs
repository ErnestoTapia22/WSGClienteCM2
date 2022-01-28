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
        public async Task<ResponseViewModel> GetStateCero()
        {
            List<DetailBindingModel> detail = new List<DetailBindingModel>();
            ResponseViewModel res = new ResponseViewModel();
            List<OracleParameter> parameters = new List<OracleParameter>();

            try
            {
                OracleParameter P_SMESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_SMESSAGE.Size = 4000;
                OracleParameter P_NCODE = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
                OracleParameter P_TABLA = new OracleParameter("C_TABLE", OracleDbType.RefCursor, detail, ParameterDirection.Output);
                parameters.Add(P_SMESSAGE);
                parameters.Add(P_NCODE);
                parameters.Add(P_TABLA);
                using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync(string.Format("{0}.{1}", Package3, "GET_STATE_CERO"), parameters, ConnectionBase.enuTypeDataBase.OracleVTime))
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

       public async Task<ResponseViewModel> UpdateStateHeader(List<string> processCodeToUpdate, string value, DbConnection cn, DbTransaction trx) {
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

                    using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync_TRX(string.Format("{0}.{1}", Package3, "UPD_STATE_CM_CAB"), parameters, cn, trx, ConnectionBase.enuTypeDataBase.OracleVTime))
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
            catch (Exception ex) {
                res.P_COD_ERR = "2";
                res.P_MESSAGE = ex.Message;

            }
            return res;


        }
        public async Task<ResponseViewModel> ValidateRow(DetailBindingModel item, DbConnection cn, DbTransaction trx)
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
                    using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync_TRX(string.Format("{0}.{1}", Package3, "VAL_CLIENT_CM_DET"), parameters, cn, trx, ConnectionBase.enuTypeDataBase.OracleVTime))
                    {

                      ElistErrores = dr.ReadRowsList<ListViewErrores>();
                   
                    }
                    res.P_COD_ERR = P_COD_ERR.Value.ToString();
                    res.P_MESSAGE = P_MESSAGE.Value.ToString();
                    res.P_CAMPO = P_CAMPO.Value.ToString();
                    res.EListErrores = ElistErrores;



            }
            catch (Exception ex)
            {
                res.P_COD_ERR = "2";
                res.P_MESSAGE = ex.Message;
                throw ex;
            }

            return res;

        }

        public async Task<ResponseViewModel> SaveStateRow(DetailBindingModel item, DbConnection cn, DbTransaction trx) {

            ResponseViewModel res = new ResponseViewModel();
          
            try
            {
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, ParameterDirection.Output);
                P_MESSAGE.Size = 4000;
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, ParameterDirection.Output);
             
               

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

                parameters.Add(P_MESSAGE);
                parameters.Add(P_COD_ERR);
               
             
                using (OracleDataReader dr = (OracleDataReader)await _connectionBase.ExecuteByStoredProcedureVTAsync_TRX(string.Format("{0}.{1}", Package3, "INS_STATE_CM_DET"), parameters, cn, trx, ConnectionBase.enuTypeDataBase.OracleVTime))
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
    }
}
