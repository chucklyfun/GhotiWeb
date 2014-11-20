using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security.Cryptography;

namespace GameLogic.Deck
{
    public interface ICardUtilities<T>
    {
        void Shuffle(Deck<T> deck);
        void RefillDrawPile(Deck<T> deck);
    }

    

    public class CardUtilities<T> : ICardUtilities<T>
    {
        public void RefillDrawPile(Deck<T> deck)
        {
            foreach (var card in deck.DiscardPile)
            {
                deck.DrawPile.Push(card);
            }

            deck.DiscardPile.Clear();
        }

        public void Shuffle(Deck<T> deck)
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
                T value = arr[k];
                arr[k] = arr[n];
                arr[n] = value;

            }
            deck.DrawPile = new Stack<T>(arr);
        }
    }
}
