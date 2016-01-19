using System.Collections;

using GameLogic.Player;
using GameLogic.Game;
using Utilities.Data;

namespace GameLogic.Domain
{
    public interface ICard : IEntity
    {
        string Name { get; set; }
        int CardNumber { get; set; }
        string ImageUrl { get; set; }
    }

    public class PlayerCard : EntityBase, ICard
    {
        public string Name { get; set; }
        public int CardNumber { get; set; }
        public int Cost { get; set; }
        public EquipmentType EquipmentType { get; set; }
        public ActionType ActionType { get; set; }
        public string ImageUrl { get; set; }

        public PlayerCard()
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
    }
}