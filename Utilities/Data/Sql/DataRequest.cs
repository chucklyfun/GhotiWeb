using System;
using System.Collections.Generic;
using System.Data;

namespace Utilities.Data.Sql
{
    public class DataRequest
    {
        public DataRequest()
        {
            CommandType = CommandType.Text;
            Exceptions = new List<Exception>();
            Parameters = new List<DataRequestParameter>();
        }

        public string ConnectionString { get; set; }

        public CommandType CommandType { get; set; }

        public string CommandText { get; set; }

        public IList<DataRequestParameter> Parameters { get; private set; }

        public IList<Exception> Exceptions { get; private set; }

        public DataRequestParameter AddParameter(string name, object value)
        {
            var result = new DataRequestParameter() { ParameterName = name, Value = value };
            Parameters.Add(result);
            return result;
        }
    }
}