using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using Utilities.Data.Cache;
using GameLogic.Data;

using MongoDB.Bson;
using System;
using GameLogic.Domain;
using GameLogic.Game;
using Utilities;

namespace GameLogic.Deck
{

    public interface ICardService
    {
        IEnumerable<PlayerCard> GetPlayerCards(string gameVersion);
        IEnumerable<MonsterCard> GetMonsterCards(string gameVersion);
        IEnumerable<PlayerCard> GetPlayerCards(IEnumerable<cardinstance> cardIds, string gameVersion);
        IEnumerable<PlayerCard> GetMonsterCards(IEnumerable<cardinstance> cardIds, string gameVersion);
        MonsterCard GetMonsterCard(ObjectId cardId, string gameVersion);
        PlayerCard GetPlayerCard(ObjectId cardId, string gameVersion);


        Dictionary<ObjectId, string> GetPlayerCardsDict(string gameVersion);
    }

    public class CardService : ICardService
    {
        private ICardManager _cardManager;
        private ICardManager _monsterCardManager;
        private ICardLoader _cardLoader;
        private IGameUtilities _gameUtilities;
        private ICollectionService _collectionService;

        public CardService(ICardManager cardManager, ICardLoader cardLoader, IGameUtilities gameUtilities, ICollectionService collectionService)
        {
            _cardManager = cardManager;
            _cardLoader = cardLoader;
            _gameUtilities = gameUtilities;
            _collectionService = collectionService;
        }

        public IEnumerable<PlayerCard> GetPlayerCards(string gameVersion)
        {
            return _cardManager.GetCardsFromCache(gameVersion, () => _gameUtilities.LoadPlayerCardDeck(_gameUtilities.GetPlayerCardFileName(gameVersion)));
        }

        public IEnumerable<MonsterCard> GetMonsterCards(string gameVersion)
        {
            return _cardManager.GetCardsFromCache(gameVersion, () => _gameUtilities.LoadMonsterCardDeck(_gameUtilities.GetMonsterCardFileName(gameVersion)));
        }

        public IEnumerable<PlayerCard> GetPlayerCards(IEnumerable<CardInstance> cardIds, string gameVersion)
        {
            return GetPlayerCards().Where(f => cardIds.Any(g => g.CardId == f.Id));
        }

        public IEnumerable<MonsterCard> GetMonsterCards(IEnumerable<CardInstance> cardIds, string gameVersion)
        {
            return GetMonsterCards().Where(f => cardIds.Any(g => g.CardId == f.Id));
        }
        
        public Dictionary<ObjectId, PlayerCard> GetPlayerCardsDict(string gameVersion)
        {
            return _collectionService.ToDictionary(GetPlayerCards(gameVersion), (f) => new Tuple<ObjectId, string>(f.Id, f.ImageUrl));
        }


        public MonsterCard GetMonsterCard(string gameVersion, ObjectId cardId)
        {
            return GetMonsterCards(gameVersion).FirstOrDefault(f => f.Id.Equals(cardId));
        }

        public PlayerCard GetPlayerCard(string gameVersion, ObjectId cardId)
        {
            return GetPlayerCards(gameVersion).FirstOrDefault(f => f.Id.Equals(cardId));
        }
    }
}