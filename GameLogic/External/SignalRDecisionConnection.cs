using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using GameLogic.Domain;

namespace GameLogic.External
{ 
    public interface ISignalRDecisionConnection
    {
        DecisionMaker DecisionMaker { get; set; }

        string ConnectionName { get; set; }
    }

    public interface ISignalRDecisionConnectionFactory
    {
        ISignalRDecisionConnection CreateISignalRDecisionConnection(string connectionId, string connectionName, ObjectId playerId, ObjectId gameId);
    }

    public class SignalRDecisionConnection : ISignalRDecisionConnection
    {
        private ISerializationService _serializationService;
        private IDecisionMakerManager _decisionMakerManager;

        public DecisionMaker DecisionMaker { get; set; }

        public string ConnectionName {get;set;}



        public SignalRDecisionConnection(ISerializationService serializationService, IDecisionMakerManager decisionMakerManager, string connectionId, string connectionName, ObjectId playerId, ObjectId gameId)
        {
            _serializationService = serializationService;
            _decisionMakerManager = decisionMakerManager;
            
            ConnectionName = connectionName;

            DecisionMaker = new DecisionMaker()
            {
                ConnectionType = ConnectionType.SignalR,
                GameId = gameId,
                Id = new ObjectId(connectionId),
                PlayerId = playerId
            };
        }
    }
}
