using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace GameLogic.External
{
    public class GameView
    {
        public IList<ObjectId> CardIds { get; set; }
        public PlayerView CurrentPlayer { get; set; }
        public IList<PlayerView> OtherPlayers { get; set; }

        public IList<ObjectId> HandCardIds { get; set; }
        public IList<ObjectId> HelmetCardIds { get; set; }
        public IList<ObjectId> BootsCardIds { get; set; }
        public IList<ObjectId> ArmorCardIds { get; set; }
        public IList<ObjectId> OtherEquipmentCardIds { get; set; }


        public ExternalAction CurrentAction { get; set; }
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
    }

    public class GameAction
    {
        public ObjectId PlayerId { get; set; }
        public ObjectId TargetPlayerId { get; set; }
        public IList<ObjectId> CardIds { get; set; }
        public ExternalAction Action { get; set; }
    }
}