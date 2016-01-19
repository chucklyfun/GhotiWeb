using System;
using System.Collections.Generic;
using System.Linq;

using GameLogic.Deck;
using Utilities.Data;

namespace GameLogic.Domain
{
    public class Player : EntityBase, IEntity
    {
        public Player()
        {
            Hand = new List<PlayerCard>();
            DrawCards = new List<PlayerCard>();
            Victories = 0;
            Name = string.Empty;
        }


        public string Name { get; set; }
        public int PlayerNumber { get; set; }
        public IList<PlayerCard> Hand { get; set; }
        public IList<PlayerCard> DrawCards { get; set; }
        public int Victories { get; set; }
        public User User { get; set; }

        public IList<PlayerCard> Equipment { get; set; }   
    }
}