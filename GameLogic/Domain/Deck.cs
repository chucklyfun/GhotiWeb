using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic.Domain
{
    public class Deck
    {
        public Stack<CardInstance> DrawPile { get; set; }
        public List<CardInstance> DiscardPile { get; set; }

        public Deck()
        {
            DrawPile = new Stack<CardInstance>();
            DiscardPile = new List<CardInstance>();
        }
    }
}