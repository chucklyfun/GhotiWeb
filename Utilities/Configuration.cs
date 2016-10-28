using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Data;

namespace Utilities
{
    public class Configuration : EntityBase
    {
        public string SocketConnectionAddress { get; set; }

        public int SocketConnectionPort { get; set; }

        public string Name { get; set; }

        public string DefaultVersion { get; set; }

        public string DataPath { get; set; }

        public Configuration()
        {
            SocketConnectionAddress = string.Empty;
            SocketConnectionPort = 0;
            Name = string.Empty;
            DefaultVersion = string.Empty;
            DataPath = string.Empty;
        }
    }
}
