using System;
using System.Data;
using System.Globalization;
using System.Linq;
using Ninject.Extensions.Logging;

namespace Utilities.Data.Sql
{
    public class SqlDataHelper : ISqlDataHelper
    {
        private readonly ILogger _logger;

        private readonly IDbProvider _dbProvider;

        private readonly IConnectionStringProvider _connectionStringProvider;

        public SqlDataHelper(ILogger logger, IDbProvider dbProvider, IConnectionStringProvider connectionStringProvider)
        {
            _logger = logger;
            _dbProvider = dbProvider;
            _connectionStringProvider = connectionStringProvider;

            CommandTimeout = 30;
        }

        public string ConnectionString
        {
            get
            {
                return _connectionStringProvider.ConnectionString;
            }
        }

        public int CommandTimeout { get; set; }

        /// <summary>
        /// Executes the given SQL query and returns a DataSet of the results
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string sql)
        {
            return ExecuteDataSet(new DataRequest()
                {
                    CommandText = sql, 
                    CommandType = CommandType.Text
                });
        }
        
     
        public DataSet ExecuteDataSet(DataRequest request)
        {
            request.Exceptions.Clear();

            var command = CreateCommandFromRequest(request);

            IDataAdapter adapter = null;
            
            var outputDataSet = new DataSet
            {
                Locale = CultureInfo.InvariantCulture
            };
            
            try
            {
                adapter = _dbProvider.CreateDataAdapter(command);

                // allow generic naming - NewDataSet
                adapter.Fill(outputDataSet);
            }
            catch (Exception ex)
            {
                request.Exceptions.Add(ex);
                _logger.Error(ex, ex.Message);
            }
            finally
            {
                command.Dispose();

                var disposableAdapter = adapter as IDisposable;
                if (disposableAdapter != null)
                {
                    disposableAdapter.Dispose();
                }
            }

            return outputDataSet;
        }

        public DataTable ExecuteDataTable(DataRequest request)
        {
            DataTable dt = null;
            if (request != null)
            {
                DataSet ds = ExecuteDataSet(request);
                if (ds != null && ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }
            }
            return dt;
        }

        public int ExecuteScalar(DataRequest request)
        {
            request.Exceptions.Clear();

            var command = CreateCommandFromRequest(request);
            int result = 0;
            
            try
            {
                result = (int) command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                request.Exceptions.Add(ex);
                _logger.Error(ex, ex.Message);
            }
            finally
            {
                command.Dispose();
            }

            return result; 
        }

        public int ExecuteNonQuery(DataRequest request)
        {
            request.Exceptions.Clear();

            var command = CreateCommandFromRequest(request);
            int result = 0;

            try
            {
                result = (int)command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                request.Exceptions.Add(ex);
                _logger.Error(ex, ex.Message);
            }
            finally
            {
                command.Dispose();
            }

            return result;
        }
        
        /// <summary>
        /// Creates a IDbCommand from a DataRequest
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public IDbCommand CreateCommandFromRequest(DataRequest request)
        {
            var connection = _dbProvider.CreateDbConnection(ConnectionString);
            var command = _dbProvider.CreateCommand();

            command.Connection = connection;
            command.CommandText = request.CommandText;
            command.CommandType = request.CommandType;
            command.CommandTimeout = CommandTimeout;

            // add parameters if they exist
            if (request.Parameters.Any())
            {
                foreach (var param in request.Parameters)
                {
                    command.Parameters[param.ParameterName] = param.Value;
                }
            }

            return command;
        }
    }


}
