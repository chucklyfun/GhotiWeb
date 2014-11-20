using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic.Deck
{
    public class Deck<T>
    {
        public Stack<T> DrawPile { get; set; }
        public IList<T> DiscardPile { get; set; }

        public Deck()
        {
            DrawPile = new Stack<T>();
            DiscardPile = new List<T>();
        }
    }
}