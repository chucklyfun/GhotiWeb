using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Utilities
{
    public interface ISerializationService
    {
        string Serialize<t>(t obj);

        t Deserialize<t>(string data);
    }

    public class SerializationService : ISerializationService
    {
        public SerializationService()
        {

        }

        public string Serialize<t>(t obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public t Deserialize<t>(string data)
        {
            return JsonConvert.DeserializeObject<t>(data);
        }
    }
}
