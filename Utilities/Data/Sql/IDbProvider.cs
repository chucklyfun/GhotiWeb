using System.Data;

namespace Utilities.Data.Sql
{
    public interface IDbProvider
    {
        IDbConnection CreateDbConnection(string connectionString);

        IDataAdapter CreateDataAdapter(IDbCommand command);

        IDbCommand CreateCommand();

        IDbDataParameter CreateDbDataParameter(string name, object value);
    }
}