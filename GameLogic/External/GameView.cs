using MongoDB.Bson;
using System;
using System.Collections.Generic;

using GameLogic.Domain;

namespace GameLogic.External
{
    public interface IView
    {
    }

    public class MessageView : IView
    {
        public string Data { get; set; }
    }

    public class GameView : IView
    {
        public ObjectId GameId { get; set; }
        public PlayerView CurrentPlayer { get; set; }
        public List<ObjectId> ChooseCardIds { get; set; }
        public List<PlayerView> OtherPlayers { get; set; }
        public List<ObjectId> PlayerCardIds { get; set; }
        public List<ObjectId> MonsterCardIds { get; set; }

        public ServerToClientAction CurrentAction { get; set; }

        public GameView()
        {
            OtherPlayers = new List<PlayerView>();
            PlayerCardIds = new List<ObjectId>();
            MonsterCardIds = new List<ObjectId>();
        }
    }

    public class PlayerView
    {
        public ObjectId PlayerId { get; set; }

        public int HandSize { get; set; }

        public ObjectId RevealedCard { get; set; }
        public int PlayerAttack { get; set; }
        public int PlayerSpeed { get; set; }
        public int PlayerHold { get; set; }
        public int PlayerDraw { get; set; }
        public int PlayerKeep { get; set; }

        public List<ObjectId> HelmetCardIds { get; set; }
        public List<ObjectId> BootsCardIds { get; set; }
        public List<ObjectId> ArmorCardIds { get; set; }
        public List<ObjectId> HandCardIds { get; set; }
        public List<ObjectId> OtherEquipmentCardIds { get; set; }

        public PlayerView()
        {
            HelmetCardIds = new List<ObjectId>();
            BootsCardIds = new List<ObjectId>();
            ArmorCardIds = new List<ObjectId>();
            HandCardIds = new List<ObjectId>();
            OtherEquipmentCardIds = new List<ObjectId>();
        }
    }

    public class GameAction
    {
        public ObjectId PlayerId { get; set; }
        public ObjectId TargetPlayerId { get; set; }
        public List<ObjectId> CardIds { get; set; }
        public ClientToServerAction Action { get; set; }

        public GameAction()
        {
            CardIds = new List<ObjectId>();
        }
    }
}