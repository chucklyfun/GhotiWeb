using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

namespace GameLogic.Deck
{

    public interface ICardManager<T>
    {
        T DrawCard(Game.Game game, Deck<T> deck);
        void AddToDiscard(Deck<T> deck, T card);
    }

    public class CardManager<T> : ICardManager<T> where T : ICard
    {
        public CardManager(ICardUtilities<T> cardUtilities)
        {
            _cardUtilities = cardUtilities;
        }
        private ICardUtilities<T> _cardUtilities { get; set; }

        public T DrawCard(Game.Game game, Deck<T> deck)
        {
            T card = default(T);
            if (!deck.DrawPile.Any())
            {
                _cardUtilities.RefillDrawPile(deck);
                _cardUtilities.Shuffle(deck);
            }

            if (deck.DrawPile.Any())
            {
                card = deck.DrawPile.Pop();
                card.OnDraw(game);
            }
            return card;
        }

        public void AddToDiscard(Deck<T> deck, T card)
        {
            deck.DiscardPile.Add(card);
        }
    }
}