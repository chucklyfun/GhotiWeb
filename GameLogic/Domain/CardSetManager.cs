using System.Collections.Generic;

using GameLogic.Game;
using Utilities.Data;
using GameLogic.Deck;
using MongoDB.Bson;

namespace GameLogic.Domain
{
    public class CardSetManager 
    {
        public void LoadCards<T>(IEnumerable<T> cards, CardSet cardSet) where T : ICardData
        {
            foreach (var c in cards)
            {
                var instances = new List<CardInstance>();

                for (int i = 0; i < c.Quantity; ++i)
                {
                    instances.Add(new CardInstance()
                    {
                        SetId = cardSet.SetId,
                        CardId = c.Id,
                        Id = ObjectId.GenerateNewId()
                    });
                }
                
                cardSet.CardInstances[c.Id] = instances;
            }
        }

        public void PullAllToDeck(CardSet cardSet, Deck deck)
        {
            foreach (var cardLists in cardSet.CardInstances)
            {
                foreach (var instance in cardLists.Value)
                {
                    deck.DrawPile.Push(instance);
                    cardLists.Value.Clear();
                }
                
            }
        }

        public CardInstance PullCard(CardSet cardSet, ObjectId cardId)
        {
            CardInstance result = null;
            List<CardInstance> cards = null;
            if (cardSet.CardInstances.TryGetValue(cardId, out cards))
            {
                result = cards.FirstOrDefault();
            }

            return result;
        }

        public void ReturnCard(CardSet cardSet, CardInstance card)
        {
            CardInstance result = null;
            List<CardInstance> cards = null;
            if (!cardSet.CardInstances.TryGetValue(card.CardId, out cards))
            {
                cards = new List<CardInstance>();
                cardSet[card.CardId] = cards;
            }

            cards.Add(card);

            return result;
        }
    }

    public class CardSet : EntityBase
    {
        public ObjectId GameId { get; set; }

        public ObjectId SetId { get; set; }

        public Dictionary<ObjectId, List<CardInstance>> CardInstances { get; set; }

        public CardSet()
        {
            CardInstances = new Dictionary<ObjectId, List<CardInstance>>();
        }
    }
}