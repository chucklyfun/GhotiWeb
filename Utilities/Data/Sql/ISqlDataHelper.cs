using System.Data;

namespace Utilities.Data.Sql
{
    public interface ISqlDataHelper
    {
        /// <summary>
        /// Executes the given SQL query and returns a DataSet of the results
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        DataSet ExecuteDataSet(string sql);

        DataTable ExecuteDataTable(DataRequest request);

        DataSet ExecuteDataSet(DataRequest request);

        int ExecuteScalar(DataRequest request);

        string ConnectionString { get; }

        int CommandTimeout { get; set; }

        int ExecuteNonQuery(DataRequest request);
    }
}