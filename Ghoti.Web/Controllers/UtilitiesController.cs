using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;
using PagedList;
using MongoDB.Bson;
using Nancy.ModelBinding;
using DataInitializer;
using GameLogic.External;

namespace ghoti.web.Controllers
{
    public class UtilitiesController : Nancy.NancyModule
    {
        public UtilitiesController(ISettingsManager settingsManager, ISerializationService serializationService, IInitializer initializer, IDecisionMakerManager decisionMakerManager)
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

            Get["/api/Utilities/Configuration/Initialize"] = _ =>
            {
                initializer.InitializeAdminUser();
                initializer.InitializePlayers(10);
                return true;
            };


            Get["/api/Utilities/CreateObjectId"] = _ =>
            {
                return ObjectId.GenerateNewId();
            };

            Get["/api/Utilities/Configuration/CreateIds/{count}"] = _ =>
            {
                return serializationService.Serialize(initializer.GenerateObjectIds(_.count));
            };
        }
    }
}