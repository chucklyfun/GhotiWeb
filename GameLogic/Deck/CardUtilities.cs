using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;
using GameLogic.Domain;

namespace GameLogic.Deck
{
    public interface ICardUtilities
    {
        void Shuffle(Domain.Deck deck);
        void RefillDrawPile(Domain.Deck deck);
    }

    

    public class CardUtilities : ICardUtilities
    {
        public void RefillDrawPile(Domain.Deck deck)
        {
            foreach (var card in deck.DiscardPile)
            {
                deck.DrawPile.Push(card);
            }

            deck.DiscardPile.Clear();
        }

        public void Shuffle(Domain.Deck deck)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            
            var arr = deck.DrawPile.ToArray();

            int n = deck.DrawPile.Count;
            while (n > 1)
            {

                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                var value = arr[k];
                arr[k] = arr[n];
                arr[n] = value;

            }
            deck.DrawPile = new Stack<CardInstance>(arr);
        }
    }
}
