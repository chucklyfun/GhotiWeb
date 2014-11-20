using System;
using System.Collections.Generic;
using System.Linq;

using GameLogic.Deck;
using GameLogic.Player;
using Utilities.Data;

namespace GameLogic.Game
{
    public class Game : EntityBase
    {
        public IList<IPlayer> Players { get; set; }

        public Queue<IMonsterCard> MonsterQueue { get; set; }

        public Deck<IMonsterCard> MonsterDeck { get; set; }
        public Deck<IPlayerCard> PlayerCardDeck { get; set; }

        public IDictionary<IPlayer, IPlayerCard> BlindActions { get; set; }

        public IDictionary<IPlayer, IPlayerCard> EquipActions { get; set; }
        public IDictionary<IPlayer, IPlayerCard> AttackActions { get; set; }
        public IDictionary<IPlayer, IPlayerCard> AmbushActions { get; set; }
        public IDictionary<IPlayer, IPlayerCard> DrawActions { get; set; }
        public IDictionary<IPlayer, bool> PlayerReady { get; set; }
        public IDictionary<IPlayer, bool> ActionSide { get; set; }


        public GameState CurrentState { get; set; }
        public int VictoryCondition { get; set; }

        public int KeepCardsMax { get; set; }
        public int DrawCardsMax { get; set; }

        public Game()
        {
            MonsterQueue = new Queue<IMonsterCard>();
            Players = new List<IPlayer>();
            MonsterDeck = new Deck<IMonsterCard>();
            PlayerCardDeck = new Deck<IPlayerCard>();
            BlindActions = new Dictionary<IPlayer, IPlayerCard>();
            EquipActions = new Dictionary<IPlayer, IPlayerCard>();
            AttackActions = new Dictionary<IPlayer, IPlayerCard>();
            AmbushActions = new Dictionary<IPlayer, IPlayerCard>();
            DrawActions = new Dictionary<IPlayer, IPlayerCard>();
            PlayerReady = new Dictionary<IPlayer, bool>();
            ActionSide = new Dictionary<IPlayer, bool>();
        }
    }
}