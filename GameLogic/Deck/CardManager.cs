using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using Utilities.Data.Cache;
using GameLogic.Data;
using GameLogic.Domain;

using MongoDB.Bson;
using System;

namespace GameLogic.Deck
{

    public interface ICardManager
    {
        CardInstance DrawCard(Domain.Game game, Domain.Deck deck);
        void AddToDiscard(Domain.Deck deck, CardInstance card);

        IEnumerable<T> GetCardsFromCache<T>(string gameVersion, Func<IEnumerable<T>> getter);
    }

    public class CardManager : ICardManager
    {
        private IRuntimeCache _cache;

        private ICardLoader _cardLoader;

        private ICardUtilities _cardUtilities;

        public CardManager(ICardUtilities cardUtilities, ICardLoader cardLoader, IRuntimeCache runtimeCache)
        {
            _cardUtilities = cardUtilities;
            _cardLoader = cardLoader;
            _cache = runtimeCache;
        }

        

        public CardInstance DrawCard(Domain.Game game, Domain.Deck deck)
        {
            CardInstance card = null;
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

        public void AddToDiscard(Domain.Deck deck, CardInstance card)
        {
            deck.DiscardPile.Add(card);
        }



        public IEnumerable<T> GetCardsFromCache<T>(string gameVersion, Func<IEnumerable<T>> getter)
        {
            return _cache.AddOrGetExisting(gameVersion + ":PlayerCardDeck", () => getter()) as IEnumerable<T> ?? new List<T>();
        }

    }
}