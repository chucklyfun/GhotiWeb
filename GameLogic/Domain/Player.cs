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
            Hand = new List<CardInstance>();
            DrawCards = new List<CardInstance>();
            Equipment = new List<CardInstance>();
            Victories = 0;
            Name = string.Empty;
        }

        public string Name { get; set; }
        public int PlayerNumber { get; set; }
        public List<CardInstance> Hand { get; set; }
        public List<CardInstance> DrawCards { get; set; }
        public int Victories { get; set; }
        public User User { get; set; }

        public List<CardInstance> Equipment { get; set; }   
    }
}