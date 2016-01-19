using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Utilities.Data
{
    public interface ICsvReader
    {
        IEnumerable<T> LoadFile<T>(string fileName);
    }

    public class CsvReader : ICsvReader
    {
        public IEnumerable<T> LoadFile<T>(string fileName)
        {
            IEnumerable<T> records = new List<T>();

            using (var f = File.Open(fileName, FileMode.Open))
            {
                var csvReader = new CsvHelper.CsvReader(new StreamReader(f));

                records = csvReader.GetRecords<T>();
            }

            return records;
        }
    }
}
