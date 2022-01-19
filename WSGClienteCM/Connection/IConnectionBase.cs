using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;

namespace WSGClienteCM.Connection
{
    public interface IConnectionBase
    {
        DbParameterCollection ParamsCollectionResult
        {
            get;
            set;
        }

        /*DbConnection*/
        OracleConnection ConnectionGet(ConnectionBase.enuTypeDataBase typeDataBase = ConnectionBase.enuTypeDataBase.OracleVTime);

        DbDataReader ExecuteByStoredProcedure(
            string nameStore,
            //ref DbParameterCollection z, 
            IEnumerable<DbParameter> parameters = null,
            ConnectionBase.enuTypeDataBase typeDataBase = ConnectionBase.enuTypeDataBase.OracleVTime,
            ConnectionBase.enuTypeExecute typeExecute = ConnectionBase.enuTypeExecute.ExecuteReader
            );
        DbParameterCollection ExecuteByStoredProcedureNonQuery(
           string nameStore,
           //ref DbParameterCollection z, 
           IEnumerable<DbParameter> parameters = null,
           ConnectionBase.enuTypeDataBase typeDataBase = ConnectionBase.enuTypeDataBase.OracleVTime,
           ConnectionBase.enuTypeExecute typeExecute = ConnectionBase.enuTypeExecute.ExecuteNonQuery
           );

        Task<DbDataReader> ExecuteByStoredProcedureVTAsync(string nameStore,
          IEnumerable<DbParameter> parameters = null,
           ConnectionBase.enuTypeDataBase typeDataBase = ConnectionBase.enuTypeDataBase.OracleVTime,
           ConnectionBase.enuTypeExecute typeExecute = ConnectionBase.enuTypeExecute.ExecuteReader);

        Task<DbDataReader> ExecuteByStoredProcedureVTAsync_TRX(string nameStore,
        IEnumerable<DbParameter> parameters = null, DbConnection connection = null,
         DbTransaction trx = null,
        ConnectionBase.enuTypeDataBase typeDataBase = ConnectionBase.enuTypeDataBase.OracleVTime,
        ConnectionBase.enuTypeExecute typeExecute = ConnectionBase.enuTypeExecute.ExecuteReader);
    }
}