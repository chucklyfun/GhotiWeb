using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Domain;
using GameLogic.Player;
using GameLogic.Game;
using Utilities.Data;
using MongoDB.Bson;
using GameLogic.Deck;
using GameLogic.Data;

namespace GameLogic.External
{
    public delegate void UpdatedViewEvent(object sender, ViewEventArgs eventArgs);

    public interface IDecisionMakerManager
    {	
        event UpdatedViewEvent UpdatedViewEvent;
        
        IList<DecisionMaker> GetDecisionMakers(ObjectId gameId, ObjectId playerId);

        DecisionMaker GetDecisionMaker(ConnectionType connectionType, string connectionId);

        List<DecisionMaker> GetOrInsertDecisionMakers(ObjectId gameId, ObjectId playerId, ConnectionType connectionType, string connectionId = "");

        bool ClearAllConnections();

        void ProcessPlayerEvent(ClientPlayerEventArgs playerEvent, ObjectId gameId, ObjectId playerId);
    }

    public class ViewEventArgs : EventArgs
    {
        public ObjectId GameId { get; set; }

        public ObjectId PlayerId { get; set; }

        public IView View { get; set; }
    }

    public class DecisionMakerManager : IDecisionMakerManager
    {
	    private IPlayerManager _playerManager;
        private IRepository<DecisionMaker> _decisionMakerRepository;
        private IRepository<GameLogic.Domain.Game> _gameRepository;
        private IRepository<GameLogic.Domain.Player> _playerRepository;
        private IGameManager _gameManager;
        private IGameViewManager _gameViewManager;
        private ICardManager<PlayerCard> _playerCardManager;
        private ICardManager<MonsterCard> _monsterCardManager;
        private ICardLoader _cardLoader;
        private IGameUtilities _gameUtilities;

	    public DecisionMakerManager (IPlayerManager playerManager, IRepository<DecisionMaker> decisionMakerRepository, IRepository<GameLogic.Domain.Game> gameRepository, IGameManager gameManager, ICardManager<PlayerCard> playerCardManager, ICardManager<MonsterCard> monsterCardManager, IGameViewManager gameViewManager, ICardLoader cardLoader, IGameUtilities gameUtilities)
	    {
		    _playerManager = playerManager;
            _gameViewManager = gameViewManager;
            _gameRepository = gameRepository;
            _gameManager = gameManager;
            _gameViewManager = gameViewManager;
            _decisionMakerRepository = decisionMakerRepository;
            _gameUtilities = gameUtilities;

            _playerCardManager = playerCardManager;
            _monsterCardManager = monsterCardManager;

            _gameViewManager.UpdatedGameView += _gameViewManager_UpdatedGameView;
	    }


        public event UpdatedViewEvent UpdatedViewEvent;

        public void _gameViewManager_UpdatedGameView(object sender, GameViewEventArgs eventArgs)
        {
            if (UpdatedViewEvent != null)
            {
                foreach (var player in eventArgs.Game.Players)
                {
                    UpdatedViewEvent(this, new ViewEventArgs()
                    {
                        GameId = eventArgs.Game.Id,
                        PlayerId = player.Id,
                        View = CreateGameView(eventArgs.Game, player)
                    });
                }
            }
        }

        public IList<DecisionMaker> GetDecisionMakers(ObjectId gameId, ObjectId playerId)
        {
            return _decisionMakerRepository.AsQueryable()
                .Where(f => f.GameId.Equals(gameId) && f.PlayerId.Equals(playerId)).ToList();
        }

        public DecisionMaker GetDecisionMaker(ConnectionType connectionType, string connectionId)
        {
            var data = _decisionMakerRepository.AsQueryable().ToList();
            return data.Where(f => f.ConnectionType == connectionType && f.ConnectionId.Equals(connectionId)).FirstOrDefault();
        }

        public List<DecisionMaker> GetOrInsertDecisionMakers(ObjectId gameId, ObjectId playerId, ConnectionType connectionType, string connectionId)
        {
            var result = GetDecisionMakers(gameId, playerId).ToList();

            if (result == null || !result.Any())
            {
                var decisionMaker = new DecisionMaker()
                {
                    GameId = gameId,
                    PlayerId = playerId,
                    ConnectionId = connectionId,
                    ConnectionType = connectionType
                };

                _decisionMakerRepository.Insert(decisionMaker);

                result.Add(decisionMaker);
            }

            return result;
        }

        public bool ClearAllConnections()
        {
            _decisionMakerRepository.RemoveAll();
            return true;
        }

        public GameView CreateGameView(Domain.Game game, Domain.Player p)
        {
            var result = new GameView();
            foreach (var card in p.Hand)
            {
                result.CardIds.Add(card.Id);
            }

            result.CurrentPlayer = CreatePlayerView(p);
            foreach (Domain.Player op in game.Players.Where(f => f.User.Id != p.User.Id))
            {
                result.OtherPlayers.Add(CreatePlayerView(op));
            }
            result.CurrentPlayer = CreatePlayerView(p);
            return result;
        }

        public PlayerView CreatePlayerView(Domain.Player p)
        {
            var pv = new PlayerView();
            pv.HandSize = p.Hand.Count();
            pv.PlayerAttack = _playerManager.CalculatePlayerAttack(p);
            pv.PlayerHold = _playerManager.CalculatePlayerHold(p);
            pv.PlayerDraw = _playerManager.CalculatePlayerDraw(p);
            pv.PlayerKeep = _playerManager.CalculatePlayerKeep(p);
            pv.PlayerSpeed = _playerManager.CalculatePlayerSpeed(p);

            return pv;
        }

        public GameView CreateEquipmentView(Domain.Game game, Domain.Player currentPlayer, Domain.Player targetPlayer)
        {
            var gv = CreateGameView(game, currentPlayer);
            foreach (var card in targetPlayer.Equipment)
            {
                gv.CardIds.Add(card.Id);
            }

            return gv;
        }

        public bool ReadyPlayer(Domain.Game game, Domain.Player player)
        {
            var update = false;
            var outValue = false;

            if (!game.PlayerReady.TryGetValue(player, out outValue) || !outValue)
            {
                game.PlayerReady[player] = true;
                update = true;
            }

            if (game.Players.Count >= 2 && game.Players.All(
                f => {
                    var ready = false;
                    game.PlayerReady.TryGetValue(f, out ready);
                    return ready;
                }))
            {
                _gameManager.StartGame(game);
                update = true;
            }

            return update;
        }

        public void ProcessPlayerEvent(ClientPlayerEventArgs eventArgs, ObjectId gameId, ObjectId playerId)
        {
            var game = _gameRepository.GetById(gameId);
            var player = game.Players.FirstOrDefault(f => f.Id.Equals(playerId));
            
            if (game != null && player != null)
            {
                if (eventArgs.Action == ClientToServerAction.StartGame)
                {
                    if (ReadyPlayer(game, player))
                    {
                        _gameRepository.Update(game);
                    }
                }
                else if (eventArgs.Action == ClientToServerAction.PlayEquipment 
                    || eventArgs.Action == ClientToServerAction.PlayAction)
                {
                    if (PlayCard(eventArgs, game, player))
                    {
                        _gameRepository.Update(game);
                    }
                }
            }
        }

        public bool PlayCard(ClientPlayerEventArgs eventArgs, Domain.Game game, Domain.Player player)
        {
            var update = false;

            var card = _playerCardManager.GetCard(game, eventArgs.Cards.FirstOrDefault(),
                () => _cardLoader.LoadPlayerCardFile(_gameUtilities.GetPlayerCardFileName(game)));

            if (card != null)
            {
                _gameManager.PlayCardBlind(game, player, card, eventArgs.Action == ClientToServerAction.PlayAction);
                update = true;    
            }

            return update;
        }    
    }
}