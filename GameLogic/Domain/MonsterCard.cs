using System.Collections.Generic;

using GameLogic.Game;
using Utilities.Data;
using GameLogic.Deck;
using MongoDB.Bson;

namespace GameLogic.Domain
{
    public class MonsterCard : EntityBase, ICardData
    {
        public ObjectId InstanceId { get; set; }

        public int Power { get; set; }
        public string Name { get; set; }
        public int CardNumber { get; set; }
        public List<int> Treasures { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }

        public MonsterCard()
        {
            Treasures = new List<int>();
        }
    }
}