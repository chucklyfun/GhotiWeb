using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using PagedList;
using MongoDB.Bson;
using Nancy.ModelBinding;

namespace ghoti.web.Controllers
{
    public class UtilitiesController : Nancy.NancyModule
    {
        public UtilitiesController(ISettingsManager settingsManager, ISerializationService serializationService)
        {
            Get["/api/Utilities/Configuration/Get"] = _ =>
            {
                return serializationService.Serialize(settingsManager.GetConfiguration());
            };

            Put["/api/Utilities/Configuration"] = _ =>
            {
                var conf = this.Bind<Configuration>();

                settingsManager.SetConfiguration(conf);

                return serializationService.Serialize(conf);
            };

            Get["/api/Utilities/Configuration/Reset"] = _ =>
            {
                settingsManager.ResetConfiguration();

                return true;
            };
        }
    }
}