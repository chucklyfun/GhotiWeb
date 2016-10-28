using System;
using Utilities;
using GameLogic.Domain;
using Utilities.Data;
using MongoDB.Bson;

namespace GameLogic.Domain
{
    public class DecisionMaker : IEntity
    {
        public ObjectId Id { get; set; }

        public ObjectId ConnectionId { get; set; }
        public ObjectId GameId { get; set; }
        public ObjectId PlayerId { get; set; }        

        public ConnectionType ConnectionType { get; set; }

        public DateTime LastUpdated { get; set; }
        public TimeSpan TimeoutPeriod { get; set; }

        public string AuthenticationToken { get; set; }
    }
}