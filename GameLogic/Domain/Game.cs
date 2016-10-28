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

        public List<Domain.Player> Players { get; set; }
        public Domain.Player CurrentPlayer { get; set; }

        public Queue<CardInstance> MonsterQueue { get; set; }

        public Deck MonsterCardDeck { get; set; }
        public Deck PlayerCardDeck { get; set; }

        public IDictionary<Domain.Player, CardInstance> BlindActions { get; set; }

        public IDictionary<Domain.Player, CardInstance> EquipActions { get; set; }
        public IDictionary<Domain.Player, CardInstance> AttackActions { get; set; }
        public IDictionary<Domain.Player, CardInstance> AmbushActions { get; set; }
        public IDictionary<Domain.Player, CardInstance> DrawActions { get; set; }
        public IDictionary<Domain.Player, bool> PlayerReady { get; set; }
        public IDictionary<Domain.Player, bool> ActionSide { get; set; }


        public Domain.GameState CurrentState { get; set; }
        public int VictoryCondition { get; set; }

        public int KeepCardsMax { get; set; }
        public int DrawCardsMax { get; set; }

        public Game()
        {
            MonsterQueue = new Queue<Domain.CardInstance>();
            Players = new List<Domain.Player>();
            MonsterCardDeck = new Deck();
            PlayerCardDeck = new Deck();
            BlindActions = new Dictionary<Domain.Player, CardInstance>();
            EquipActions = new Dictionary<Domain.Player, CardInstance>();
            AttackActions = new Dictionary<Domain.Player, CardInstance>();
            AmbushActions = new Dictionary<Domain.Player, CardInstance>();
            DrawActions = new Dictionary<Domain.Player, CardInstance>();
            PlayerReady = new Dictionary<Domain.Player, bool>();
            ActionSide = new Dictionary<Domain.Player, bool>();
            CurrentState = GameState.Pregame;
        }
    }
}