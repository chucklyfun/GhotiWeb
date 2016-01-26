using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Data;

namespace Utilities
{
    public interface ISettingsManager
    {
        Configuration GetConfiguration();

        Configuration SetConfiguration(Configuration configuration);

        void ResetConfiguration();
    }

    public class SettingsManager : ISettingsManager
    {
        private IRepository<Configuration> _configurationRepository;

        public SettingsManager(IRepository<Configuration> configurationRepository)
        {
            _configurationRepository = configurationRepository;
        }

        public Configuration GetConfiguration()
        {
            var result = _configurationRepository.AsQueryable().FirstOrDefault();
            if (result == null)
            {
                result = new Configuration()
                {
                    SocketConnectionAddress = @"localhost",
                    SocketConnectionPort = 11000,
                    Name = "Test",
                    DefaultVersion = "0001",
                    DataPath = "~/Data/"
                };

                _configurationRepository.Insert(result);
            }


            return result;
        }

        public Configuration SetConfiguration(Configuration configuration)
        {
            return _configurationRepository.Update(configuration);
        }

        public void ResetConfiguration()
        {
            _configurationRepository.RemoveAll();
        }
    }
}
