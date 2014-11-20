using System.Collections;

using GameLogic.Player;
using GameLogic.Game;

namespace GameLogic.Deck
{
    public interface IPlayStrategy
    {
        void OnReveal(IPlayer player, IPlayerCard card, Game.Game game, bool actionSide);
    }

    public class AttackStrategy : IPlayStrategy
    {
        private IGameManager _gameManager;

        public AttackStrategy(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void OnReveal(IPlayer player, IPlayerCard card, Game.Game game, bool actionSide)
        {
            if (actionSide)
            {
                game.AttackActions[player] = card;
            }
            else
            {
                game.EquipActions[player] = card;
            }
        }
    }

    public class AmbushStrategy : IPlayStrategy
    {
        private IGameManager _gameManager;

        public AmbushStrategy(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void OnReveal(IPlayer player, IPlayerCard card, Game.Game game, bool actionSide)
        {
            if (actionSide)
            {
                game.AmbushActions[player] = card;
            }
            else
            {
                game.EquipActions[player] = card;
            }
        }
    }

    public class DrawStrategy : IPlayStrategy
    {
        private IGameManager _gameManager;

        public DrawStrategy(IGameManager gameManager)
        {
            _gameManager = gameManager;
        }

        public void OnReveal(IPlayer player, IPlayerCard card, Game.Game game, bool actionSide)
        {
            if (actionSide)
            {
                game.DrawActions[player] = card;
            }
            else
            {
                game.EquipActions[player] = card;
            }
        }
    }
}