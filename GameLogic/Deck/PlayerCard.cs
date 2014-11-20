using System.Collections;

using GameLogic.Player;
using GameLogic.Game;
using Utilities.Data;

namespace GameLogic.Deck
{
    public interface ICard : IEntity
    {
        string Name { get; set; }
        void OnDraw(Game.Game game);
        void OnPlay(Game.Game game);
    }

    public interface IPlayerCard : ICard
    {
        int Cost { get; set; }
        EquipmentType EquipmentType { get; set; }

        void OnReveal(Game.Game game, IPlayer player, bool actionSide);

        int ActionPowerBonus { get; set; }
        int ActionSpeedBonus { get; set; }
        int ActionDrawBonus { get; set; }
        int ActionKeepBonus { get; set; }
        int ActionAmbushMaxReward { get; set; }

        int EquipmentPowerBonus { get; set; }
        int EquipmentSpeedBonus { get; set; }
        int EquipmentDrawBonus { get; set; }
        int EquipmentKeepBonus { get; set; }
        int EquipmentHoldBonus { get; set; }

        int SingleUsePowerBonus { get; set; }
        int SingleUseSpeedBonus { get; set; }
        int SingleUseDrawBonus { get; set; }
        int SingleUseKeepBonus { get; set; }
        int SingleUseHoldBonus { get; set; }
    }

    public class PlayerCard : EntityBase, IPlayerCard
    {
        public string Name { get; set; }
        public int Cost { get; set; }
        public EquipmentType EquipmentType { get; set; }

        private IGameManager _gameManager;
        private IPlayerManager _playerManager;
        private ICardManager<IPlayerCard> _cardManager;
        private IPlayStrategy _strategy;

        public PlayerCard(IGameManager gameManager, IPlayerManager playerManager, ICardManager<IPlayerCard> cardManager, IPlayStrategy strategy)
        {
            _gameManager = gameManager;
            _playerManager = playerManager;
            _cardManager = cardManager;
            _strategy = strategy;
        }

        public void OnReveal(Game.Game game, IPlayer player, bool actionSide)
        {
            _strategy.OnReveal(player, this, game, actionSide);
        }

        public void OnDraw(Game.Game game)
        {

        }
        public void OnPlay(Game.Game game)
        {

        }

        public int ActionPowerBonus { get; set; }
        public int ActionSpeedBonus { get; set; }
        public int ActionDrawBonus { get; set; }
        public int ActionKeepBonus { get; set; }
        public int ActionAmbushMaxReward { get; set; }

        public int EquipmentPowerBonus { get; set; }
        public int EquipmentSpeedBonus { get; set; }
        public int EquipmentDrawBonus { get; set; }
        public int EquipmentKeepBonus { get; set; }
        public int EquipmentHoldBonus { get; set; }

        public int SingleUsePowerBonus { get; set; }
        public int SingleUseSpeedBonus { get; set; }
        public int SingleUseDrawBonus { get; set; }
        public int SingleUseKeepBonus { get; set; }
        public int SingleUseHoldBonus { get; set; }
    }
}