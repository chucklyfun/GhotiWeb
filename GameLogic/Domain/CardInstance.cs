using System.Collections.Generic;

using GameLogic.Game;
using Utilities.Data;
using GameLogic.Deck;
using MongoDB.Bson;

namespace GameLogic.Domain
{
    public class CardInstance : EntityBase
    {
        public ObjectId SetId { get; set; }
        public ObjectId CardId { get; set; }
    }
}