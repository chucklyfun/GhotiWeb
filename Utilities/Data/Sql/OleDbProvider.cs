using System.Data;
using System.Data.OleDb;

namespace Utilities.Data.Sql
{
    public class OleDbProvider : IDbProvider
    {
        public IDbConnection CreateDbConnection(string connectionString)
        {
            return new OleDbConnection(connectionString);
        }

        public IDataAdapter CreateDataAdapter(IDbCommand command)
        {
            return new OleDbDataAdapter(command as OleDbCommand);
        }

        public IDbCommand CreateCommand()
        {
            return new OleDbCommand();
        }

        public IDbDataParameter CreateDbDataParameter(string name, object value)
        {
            return new OleDbParameter(name, value);
        }
    }
}