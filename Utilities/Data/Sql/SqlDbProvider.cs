using System.Data;
using System.Data.SqlClient;

namespace Utilities.Data.Sql
{
    public class SqlDbProvider : IDbProvider
    {
        public IDbConnection CreateDbConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public IDataAdapter CreateDataAdapter(IDbCommand command)
        {
            return new SqlDataAdapter(command as SqlCommand);
        }

        public IDbCommand CreateCommand()
        {
            return new SqlCommand();
        }

        public IDbDataParameter CreateDbDataParameter(string name, object value)
        {
            return new SqlParameter(name, value);
        }
    }
}