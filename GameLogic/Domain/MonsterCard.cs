using System.Collections.Generic;

using GameLogic.Game;
using Utilities.Data;
using GameLogic.Deck;

namespace GameLogic.Domain
{
    public class MonsterCard : EntityBase, ICard
    {
        private ICardManager<MonsterCard> _cardManager;
        private IGameManager _gameManager;

        public int Power { get; set; }
        public string Name { get; set; }
        public int CardNumber { get; set; }
        public IList<int> Treasures { get; set; }
        public string ImageUrl { get; set; }

        public MonsterCard(ICardManager<MonsterCard> cardManager, IGameManager gameManager)
        {
            _cardManager = cardManager;
            _gameManager = gameManager;
            Treasures = new List<int>();
        }
    }
}