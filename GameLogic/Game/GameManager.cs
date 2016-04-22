using System;
using System.Collections.Generic;
using System.Linq;

using GameLogic.Player;
using GameLogic.Deck;
using GameLogic.External;
using GameLogic.Domain;
using Utilities;

namespace GameLogic.Game
{
    public interface IGameManager
    {
        void PlayCardBlind(Domain.Game g, Domain.Player player, Domain.PlayerCard card, bool actionSide);
        bool CanReveal(Domain.Game g);
        void ProcessReveal(Domain.Game g);

        void ProcessTurnStart(Domain.Game g);
        void ProcessDraw(Domain.Game g);
        void ProcessEquip(Domain.Game g);
        void ProcessAmbush(Domain.Game g);
        void ProcessKeep(Domain.Game g, Domain.Player player, IList<Domain.PlayerCard> cards);
        void ProcessAttack(Domain.Game g, IDictionary<Domain.Player, int> attackTotals);
        void ProcessAmbushAttackActions(Domain.Game g);

        void AddCardToQueue(Domain.Game game, Domain.MonsterCard monsterCard);

        
        void PlayerPlaysEquipment(Domain.Game game, Domain.Player player, Domain.PlayerCard card);

        void PlayerPlaysAction(Domain.Game game, Domain.Player player, Domain.PlayerCard card);

        void StartGame(Domain.Game game);
    }


    public class GameManager : IGameManager
    {
        private ICardManager<Domain.MonsterCard> _monsterCardManager;
        private ICardManager<Domain.PlayerCard> _playerCardManager;
        private IPlayerManager _playerManager;
        private IGameUtilities _gameUtilities;
        private IGameViewManager _gameViewManager;
        private ISettingsManager _settingsManager;
        private ICardUtilities<Domain.MonsterCard> _monsterCardUtilities;
        private ICardUtilities<Domain.PlayerCard> _playerCardUtilities;

        public GameManager(
            ICardManager<Domain.MonsterCard> monsterCardManager,
            ICardManager<Domain.PlayerCard> playerCardManager,
            IPlayerManager playerManager,
            IGameUtilities gameUtilities,
            IGameViewManager gameViewManager,
            ISettingsManager settingsManager)
        {
            _monsterCardManager = monsterCardManager;
            _playerCardManager = playerCardManager;
            _playerManager = playerManager;
            _gameViewManager = gameViewManager;
            _gameUtilities = gameUtilities;
            _settingsManager = settingsManager;
        }

        public bool InitializeGame(Domain.Game game)
        {
            var result = false;

            return result;
        }

        public Domain.Game CreateGame()
        {
            return new Domain.Game();
        }

        public void ProcessTurnStart(Domain.Game game)
        {
            _gameViewManager.OnUpdatedGameView(this, new GameViewEventArgs()
            {
                Game = game,
                Action = Domain.ServerToClientAction.PlayBlind
            });
        }

        public void PlayerPlaysEquipment(Domain.Game game, Domain.Player player, Domain.PlayerCard card)
        {
            game.BlindActions[player] = card;
            game.ActionSide[player] = false;
        }

        public void PlayerPlaysAction(Domain.Game game, Domain.Player player, Domain.PlayerCard card)
        {
            game.BlindActions[player] = card;
            game.ActionSide[player] = true;
        }

        public void AddCardToQueue(Domain.Game game, Domain.MonsterCard monsterCard)
        {
            game.MonsterQueue.Enqueue(monsterCard);
        }

        public void PlayCardBlind(Domain.Game game, Domain.Player player, Domain.PlayerCard card, bool actionSide)
        {
            game.BlindActions.Add(player, card);
            game.ActionSide[player] = actionSide;

            _gameViewManager.OnUpdatedPlayerView(this, new ServerPlayerEventArgs()
            {
                Action = Domain.ServerToClientAction.Wait,
                Cards = player.Hand,
                Player = player,
                GameId = game.Id
            });
        }

        public bool CanReveal(Domain.Game game)
        {
            return game.Players.Count == game.BlindActions.Count;
        }

        public void ProcessReveal(Domain.Game game)
        {
            foreach (var action in game.BlindActions)
            {
                bool v;
                if (game.ActionSide.TryGetValue(action.Key, out v))
                {
                    var card = action.Value;

                    if (v)
                    {
                        if (card.ActionType == Domain.ActionType.Draw)
                        {
                            game.DrawActions.Add(action.Key, card);
                        }
                        else if (card.ActionType == Domain.ActionType.Attack)
                        {
                            game.AttackActions.Add(action.Key, card);
                        }
                        else if (card.ActionType == Domain.ActionType.Ambush)
                        {
                            game.AmbushActions.Add(action.Key, card);
                        }
                    }
                    else
                    {
                        game.EquipActions.Add(action.Key, card);
                    }
                }
                
            }
            game.BlindActions.Clear();
        }

        public void ProcessDraw(Domain.Game game)
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

            var otherPlayers = new List<Domain.Player>();
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

            foreach (var p in game.Players)
            {
                _gameViewManager.OnUpdatedPlayerView(this, new ServerPlayerEventArgs()
                {
                    Player = p,
                    Action = Domain.ServerToClientAction.DrawAndDiscard,
                    Cards = p.DrawCards
                });
            }
            
        }

        public void ProcessEquip(Domain.Game game)
        {
            foreach (var kvp in game.EquipActions)
            {
                if (_playerManager.ValidateEquipment(kvp.Key, kvp.Value))
                {
                    kvp.Key.Equipment.Add(kvp.Value);
                    _gameViewManager.OnUpdatedPlayerView(this, new ServerPlayerEventArgs()
                    {
                        Action = ServerToClientAction.Wait,
                        GameId = game.Id,
                        Player = kvp.Key,
                        Cards = kvp.Key.Hand
                    });
                }

                var otherPlayers = new List<Domain.Player>();
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
                    _gameViewManager.OnUpdatedPlayerView(this, new ServerPlayerEventArgs()
                        {
                            Action = Domain.ServerToClientAction.PlayEquipment,
                            GameId = game.Id,
                            Player = p,
                            Cards = p.Hand
                        });
                }
            }
        }

        public void ProcessKeep(Domain.Game game, Domain.Player player, IList<Domain.PlayerCard> keepCards)
        {
            foreach (var card in keepCards)
            {
                if (player.Hand.Contains(card))
                {
                    player.Hand.Remove(card);
                    player.Hand.Add(card);
                }
            }
        }

        public void ProcessAmbush(Domain.Game game)
        {
            var firstMonster = game.MonsterQueue.FirstOrDefault();

            if (null != firstMonster)
            {
                // The attack has been ambushed! See who gets the ambush.
                var speeds = new Dictionary<Domain.Player, int>();
                foreach (var kvp in game.AmbushActions)
                {
                    var currentSpeed = _playerManager.CalculatePlayerSpeed(kvp.Key);
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

                _gameViewManager.OnUpdatedPlayerView(this, new ServerPlayerEventArgs()
                {
                    GameId = game.Id, 
                    Player = fastestAmbush.Key,
                    Action = ServerToClientAction.DrawAndDiscard,
                    Cards = fastestAmbush.Key.DrawCards
                });

                foreach (var player in game.Players.Where(f => f != fastestAmbush.Key))
                {
                    _gameViewManager.OnUpdatedPlayerView(this, new ServerPlayerEventArgs()
                    {
                        GameId = game.Id,
                        Player = player,
                        Action = ServerToClientAction.Wait,
                        Cards = player.Hand
                    });
                }
            }
        }

        public void ProcessAttack(Domain.Game game, IDictionary<Domain.Player, int> attackTotals)
        {
            // The attack has been ambushed! See who gets the ambush.
            var speeds = new Dictionary<Domain.Player, int>();
            foreach (var kvp in game.AttackActions)
            {
                var currentSpeed = _playerManager.CalculatePlayerSpeed(kvp.Key);
                currentSpeed += kvp.Value.ActionSpeedBonus;
                speeds[kvp.Key] = currentSpeed;
            }

            int totalAttack = 0;
            var rewards = new Dictionary<Domain.Player, int>();
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
                        fastestPlayer = default(KeyValuePair<Domain.Player, int>);
                    }
                }
            }

            foreach (var player in game.Players)
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

            foreach (var player in game.Players)
            {
                game.PlayerReady[player] = true;
            }

            _gameViewManager.OnUpdatedGameView(this, new GameViewEventArgs()
            {
                Game = game, 
                Action = ServerToClientAction.Wait
            });
        }

        public void ProcessAmbushAttackActions(Domain.Game game)
        {
            if (0 < game.AmbushActions.Count())
            {
                var firstMonster = game.MonsterQueue.FirstOrDefault();

                // Calculate Attack Totals
                var attackTotals = new Dictionary<Domain.Player, int>();
                foreach (var action in game.AttackActions)
                {
                    var currentAttack = _playerManager.CalculatePlayerAttack(action.Key);
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

        public void StartGame(Domain.Game game)
        {
            foreach (var p in game.Players)
            {
                game.PlayerReady[p] = false;
                game.ActionSide[p] = false;
            }

            if (string.IsNullOrEmpty(game.Version))
            {
                var configuration = _settingsManager.GetConfiguration();
                game.Version = configuration.DefaultVersion;
            }

            game.CurrentState = GameState.TurnStart;

            game.PlayerCardDeck = _gameUtilities.LoadPlayerCardDeck(_gameUtilities.GetPlayerCardFileName(game));
            game.MonsterCardDeck = _gameUtilities.LoadMonsterCardDeck(_gameUtilities.GetMonsterCardFileName(game));

            _playerCardUtilities.Shuffle(game.PlayerCardDeck);
            _monsterCardUtilities.Shuffle(game.MonsterCardDeck);

            foreach (var p in game.Players)
            {
                // TODO 4/21/2016 bdrosander - add starting hand to game settings.
                for (int i = 0; i < 5; ++i)
                {
                    p.Hand.Add(_playerCardManager.DrawCard(game, game.PlayerCardDeck));
                }
            }

            _gameViewManager.UpdatedGameView(new GameViewEventArgs()
            {
                Action = ServerToClientAction.PlayBlind,
                Game = game
            });
        }
    }
}