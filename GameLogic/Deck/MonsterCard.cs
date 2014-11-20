using System.Collections.Generic;

using GameLogic.Game;
using Utilities.Data;

namespace GameLogic.Deck
{
    public interface IMonsterCard : ICard
    {
        int Power { get; set; }
        IList<int> Treasures { get; set; }

        void OnAddToQueue(Game.Game game);
        void ResolveAttack(Game.Game game);
        void ResolveAmbush(Game.Game game);
    }

    public class MonsterCard : EntityBase, IMonsterCard
    {
        private ICardManager<IMonsterCard> _cardManager;
        private IGameManager _gameManager;

        public int Power { get; set; }
        public string Name { get; set; }
        public IList<int> Treasures { get; set; }
        private IPlayStrategy _strategy { get; set; }

        public MonsterCard(ICardManager<IMonsterCard> cardManager, IGameManager gameManager)
        {
            _cardManager = cardManager;
            _gameManager = gameManager;
            Treasures = new List<int>();
        }

        public void OnAddToQueue(Game.Game game)
        {
        }

        public void OnDraw(Game.Game game)
        {

        }
        public void OnPlay(Game.Game game)
        {

        }

        public void ResolveAttack(Game.Game game)
        {

        }

        public void ResolveAmbush(Game.Game game)
        {

        }
    }
}