using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace GameLogic.External
{ 
    public interface ISignalRDecisionConnection
    {
        string ConnectionId {get;set;}
        string ConnectionName {get;set;}
        ObjectId PlayerId {get;set;}
        ObjectId GameId {get;set;}
    }

    public interface ISignalRDecisionConnectionFactory
    {
        ISignalRDecisionConnection CreateISignalRDecisionConnection(string connectionId, string connectionName, ObjectId playerId, ObjectId gameId);
    }

    public class SignalRDecisionConnection : ISignalRDecisionConnection
    {
        private ISerializationService _serializationService;
        private IDecisionMakerManager _decisionMakerManager;

        public string ConnectionId {get;set;}
        public string ConnectionName {get;set;}
        public ObjectId PlayerId {get;set;}
        public ObjectId GameId { get; set; }

        public SignalRDecisionConnection(ISerializationService serializationService, IDecisionMakerManager decisionMakerManager, string connectionId, string connectionName, ObjectId playerId, ObjectId gameId)
        {
            _serializationService = serializationService;
            _decisionMakerManager = decisionMakerManager;
            ConnectionId = connectionId;
            ConnectionName = connectionName;
            PlayerId = playerId;
            GameId = gameId;
        }


    }
}
