using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using Utilities.Data.Cache;
using GameLogic.Data;

using MongoDB.Bson;
using System;

namespace GameLogic.Deck
{

    public interface ICardManager<T>
    {
        T DrawCard(Domain.Game game, Deck<T> deck);
        void AddToDiscard(Deck<T> deck, T card);

        T GetCard(GameLogic.Domain.Game game, ObjectId cardId, Func<IEnumerable<T>> getter);
    }

    public class CardManager<T> : ICardManager<T> where T : Domain.ICard
    {
        private IRuntimeCache _cache;

        private ICardLoader _cardLoader;

        public CardManager(ICardUtilities<T> cardUtilities)
        {
            _cardUtilities = cardUtilities;
        }

        private ICardUtilities<T> _cardUtilities { get; set; }

        public T DrawCard(Domain.Game game, Deck<T> deck)
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

            }
            return card;
        }

        public void AddToDiscard(Deck<T> deck, T card)
        {
            deck.DiscardPile.Add(card);
        }

        public T GetCard(GameLogic.Domain.Game game, ObjectId cardId, Func<IEnumerable<T>> getter)
        {
            var cardList = _cache.AddOrGetExisting(game.Version + ":PlayerCardDeck", () => getter()) as IEnumerable<T> ?? new List<T>();

            return cardList.FirstOrDefault(f => f.Id.Equals(cardId));
        }
    }
}