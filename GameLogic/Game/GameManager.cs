using System;
using System.Collections.Generic;
using System.Linq;

using GameLogic.Player;
using GameLogic.Deck;
using GameLogic.External;

namespace GameLogic.Game
{
    public interface IGameManager
    {
        void PlayCardBlind(Game g, IPlayer player, IPlayerCard card, bool facing);
        bool CanReveal(Game g);
        void ProcessReveal(Game g);

        void ProcessTurnStart(Game g);
        void ProcessDraw(Game g);
        void ProcessEquip(Game g);
        void ProcessAmbush(Game g);
        void ProcessKeep(Game g, IPlayer player, IList<IPlayerCard> cards);
        void ProcessAttack(Game g, IDictionary<IPlayer, int> attackTotals);
        void ProcessAmbushAttackActions(Game g);

        void AddCardToQueue(Game game, IMonsterCard monsterCard);
    }


    public class GameManager : IGameManager
    {
        private ICardManager<IMonsterCard> _monsterCardManager;
        private ICardManager<IPlayerCard> _playerCardManager;
        private IPlayerManager _playerManager;
        private IDecisionMakerManager _decisionMakerManager;
        private IGameViewManager _gameViewManager;
        private IGameStateManager _gameStateManager;
        private IGameUtilities _gameUtilities;

        public GameManager(
            ICardManager<IMonsterCard> monsterCardManager,
            ICardManager<IPlayerCard> playerCardManager,
            IPlayerManager playerManager,
            IGameStateManager gameStateManager,
            IDecisionMakerManager decisionMakerManager,
            IGameViewManager gameViewManager,
            IGameUtilities gameUtilities)
        {
            _monsterCardManager = monsterCardManager;
            _playerCardManager = playerCardManager;
            _playerManager = playerManager;
            _gameStateManager = gameStateManager;
            _decisionMakerManager = decisionMakerManager;
            _gameViewManager = gameViewManager;
            _gameUtilities = gameUtilities;
        }

        public bool InitializeGame(Game game)
        {
            var result = false;

            return result;
        }

        public Game CreateGame()
        {
            return new Game();
        }

        public void ProcessTurnStart(Game game)
        {
            foreach (IPlayer p in _playerManager.Players)
            {
                var gv = _gameViewManager.CreateGameView(p);
                gv.CurrentAction = External.ExternalAction.PlayAction;
                _decisionMakerManager.ProcessGameView(gv);
            }
        }

        public void AddCardToQueue(Game game, IMonsterCard monsterCard)
        {
            game.MonsterQueue.Enqueue(monsterCard);
            monsterCard.OnAddToQueue(game);
        }

        public void PlayCardBlind(Game game, IPlayer player, IPlayerCard card, bool facing)
        {
            game.BlindActions.Add(player, card);
            game.ActionSide[player] = facing;

            _gameViewManager.SendHandView(player, ExternalAction.Wait);
        }

        public bool CanReveal(Game game)
        {
            return game.Players.Count == game.BlindActions.Count;
        }

        public void ProcessReveal(Game game)
        {
            foreach (var action in game.BlindActions)
            {
                bool v;
                if (game.ActionSide.TryGetValue(action.Key, out v))
                {
                    action.Value.OnReveal(game, action.Key, v);
                }
            }
            game.BlindActions.Clear();
        }

        public void ProcessDraw(Game game)
        {
            // Calculate draw and keep value
            var drawMax = _gameUtilities.CalculateDrawMax(game);
            foreach (var kvp in game.DrawActions)
            {
                for (int i = 0; i < drawMax; ++i)
                {
                    var card = _playerCardManager.DrawCard(game, game.PlayerCardDeck);
                    if (null != card)
                    {
                        kvp.Key.DrawCards.Add(card);
                    }
                }
            }

            var otherPlayers = new List<IPlayer>();
            foreach (var kvp in game.AttackActions)
            {
                otherPlayers.Add(kvp.Key);
            }
            foreach (var kvp in game.EquipActions)
            {
                otherPlayers.Add(kvp.Key);
            }
            foreach (var kvp in game.AmbushActions)
            {
                otherPlayers.Add(kvp.Key);
            }
            foreach (var p in otherPlayers)
            {
                var card = _playerCardManager.DrawCard(game, game.PlayerCardDeck);
                if (null != card)
                {
                    p.DrawCards.Add(card);
                }
                
            }
        }

        public void ProcessEquip(Game game)
        {
            foreach (var kvp in game.EquipActions)
            {
                if (kvp.Key.ValidateEquipment(kvp.Value))
                {
                    kvp.Key.Equipment.Add(kvp.Value);
                    var gv = _gameViewManager.CreateGameView(kvp.Key);
                    gv.CurrentAction = External.ExternalAction.Wait;
                    _decisionMakerManager.ProcessGameView(gv);
                }

                var otherPlayers = new List<IPlayer>();
                foreach (var p in game.AttackActions)
                {
                    otherPlayers.Add(p.Key);
                }
                foreach (var p in game.DrawActions)
                {
                    otherPlayers.Add(p.Key);
                }
                foreach (var p in game.AmbushActions)
                {
                    otherPlayers.Add(p.Key);
                }
                foreach (var p in otherPlayers)
                {
                    _gameViewManager.SendHandView(p, External.ExternalAction.PlayEquipment);
                }
            }
        }

        public void ProcessKeep(Game game, IPlayer player, IList<IPlayerCard> keepCards)
        {
            foreach (var card in keepCards)
            {
                if (player.Hand.Contains(card))
                {
                    player.Hand.Remove(card);
                    player.Hand.Add(card);
                }
            }

            _gameViewManager.SendKeepView(player, External.ExternalAction.PlayEquipment);
        }

        public void ProcessAmbush(Game game)
        {
            var firstMonster = game.MonsterQueue.FirstOrDefault();

            if (null != firstMonster)
            {
                // The attack has been ambushed! See who gets the ambush.
                var speeds = new Dictionary<IPlayer, int>();
                foreach (var kvp in game.AmbushActions)
                {
                    var currentSpeed = kvp.Key.CalculatePlayerSpeed();
                    currentSpeed += kvp.Value.ActionSpeedBonus;
                    speeds[kvp.Key] = currentSpeed;
                }

                var fastestAmbush = speeds.OrderBy(f => f.Value).FirstOrDefault();
                var reward = Math.Max(game.AmbushActions[fastestAmbush.Key].ActionAmbushMaxReward, firstMonster.Treasures.Sum());

                for (int i = 0; i < reward; ++i)
                {
                    var card = _playerCardManager.DrawCard(game, game.PlayerCardDeck);
                    if (null != card)
                    {
                        fastestAmbush.Key.DrawCards.Add(card);
                    }
                }

                _gameViewManager.SendKeepView(fastestAmbush.Key, External.ExternalAction.DrawAndDiscard);

                foreach (var player in _playerManager.Players.Where(f => f != fastestAmbush.Key))
                {
                    _gameViewManager.SendHandView(player, External.ExternalAction.Wait);
                }
            }
        }

        public void ProcessAttack(Game game, IDictionary<IPlayer, int> attackTotals)
        {
            // The attack has been ambushed! See who gets the ambush.
            var speeds = new Dictionary<IPlayer, int>();
            foreach (var kvp in game.AttackActions)
            {
                var currentSpeed = kvp.Key.CalculatePlayerSpeed();
                currentSpeed += kvp.Value.ActionSpeedBonus;
                speeds[kvp.Key] = currentSpeed;
            }

            int totalAttack = 0;
            var rewards = new Dictionary<IPlayer, int>();
            var currentMonster = game.MonsterQueue.FirstOrDefault();

            var treasures = currentMonster.Treasures.ToList();
            var fastestPlayer = speeds.FirstOrDefault();

            foreach (var kvp in speeds.OrderBy(f => f.Value))
            {
                if (null != currentMonster && attackTotals.Sum(f => f.Value) > currentMonster.Power)
                {

                    int currentAttack = 0;
                    if (attackTotals.TryGetValue(kvp.Key, out currentAttack))
                    {
                        totalAttack += currentAttack;
                    }

                    rewards[kvp.Key] = treasures.FirstOrDefault();
                    treasures.Remove(0);
                    game.AttackActions.Remove(kvp.Key);

                    if (totalAttack > currentMonster.Power)
                    {
                        rewards[fastestPlayer.Key] += treasures.Sum();

                        game.MonsterQueue.Dequeue();
                        currentMonster = game.MonsterQueue.FirstOrDefault();
                        treasures = currentMonster.Treasures.ToList();
                        fastestPlayer = default(KeyValuePair<IPlayer, int>);
                    }
                }
            }

            foreach (var player in _playerManager.Players)
            {
                int rewardValue = 0;
                if (rewards.TryGetValue(player, out rewardValue))
                {
                    for (int i = 0; i < rewardValue; ++i)
                    {
                        var card = _playerCardManager.DrawCard(game, game.PlayerCardDeck);
                        player.Hand.Add(card);
                    }
                }
            }

            foreach (var player in _playerManager.Players)
            {
                game.PlayerReady[player] = true;
                _gameViewManager.SendHandView(player, External.ExternalAction.Wait);
            }
        }

        public void ProcessAmbushAttackActions(Game game)
        {
            if (0 < game.AmbushActions.Count())
            {
                var firstMonster = game.MonsterQueue.FirstOrDefault();

                // Calculate Attack Totals
                var attackTotals = new Dictionary<IPlayer, int>();
                foreach (var action in game.AttackActions)
                {
                    var currentAttack = action.Key.CalculatePlayerAttack();
                    currentAttack += action.Value.ActionPowerBonus;
                    attackTotals[action.Key] = currentAttack;
                }

                // The attack has enough power to kill the monster
                if (firstMonster.Power > attackTotals.Values.Sum())
                {
                    // The attack has been ambushed! See who gets the ambush.
                    if (0 < game.AmbushActions.Count())
                    {
                        ProcessAmbush(game);
                    }
                    // The attack succeeds. Calculate rewards and whether more attacks succeed.
                    else
                    {
                        ProcessAttack(game, attackTotals);
                    }
                }
            }
        }
    }
}