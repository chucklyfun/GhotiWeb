using System;
using System.Collections.Generic;
using System.Linq;

using GameLogic.Deck;
using GameLogic.Player;
using Utilities.Data;

namespace GameLogic.Domain
{
    public class Game : EntityBase
    {
        public string Version { get; set; }

        public IList<Domain.Player> Players { get; set; }
        public Domain.Player CurrentPlayer { get; set; }

        public Queue<MonsterCard> MonsterQueue { get; set; }

        public Deck<MonsterCard> MonsterDeck { get; set; }
        public Deck<PlayerCard> PlayerCardDeck { get; set; }

        public IDictionary<Domain.Player, PlayerCard> BlindActions { get; set; }

        public IDictionary<Domain.Player, PlayerCard> EquipActions { get; set; }
        public IDictionary<Domain.Player, PlayerCard> AttackActions { get; set; }
        public IDictionary<Domain.Player, PlayerCard> AmbushActions { get; set; }
        public IDictionary<Domain.Player, PlayerCard> DrawActions { get; set; }
        public IDictionary<Domain.Player, bool> PlayerReady { get; set; }
        public IDictionary<Domain.Player, bool> ActionSide { get; set; }


        public Domain.GameState CurrentState { get; set; }
        public int VictoryCondition { get; set; }

        public int KeepCardsMax { get; set; }
        public int DrawCardsMax { get; set; }

        public Game()
        {
            MonsterQueue = new Queue<Domain.MonsterCard>();
            Players = new List<Domain.Player>();
            MonsterDeck = new Deck<Domain.MonsterCard>();
            PlayerCardDeck = new Deck<Domain.PlayerCard>();
            BlindActions = new Dictionary<Domain.Player, PlayerCard>();
            EquipActions = new Dictionary<Domain.Player, PlayerCard>();
            AttackActions = new Dictionary<Domain.Player, PlayerCard>();
            AmbushActions = new Dictionary<Domain.Player, PlayerCard>();
            DrawActions = new Dictionary<Domain.Player, PlayerCard>();
            PlayerReady = new Dictionary<Domain.Player, bool>();
            ActionSide = new Dictionary<Domain.Player, bool>();
        }
    }
}