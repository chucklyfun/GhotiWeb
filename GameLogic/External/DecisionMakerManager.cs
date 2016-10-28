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
using Utilities;
using Utilities.EventBroker;

namespace GameLogic.External
{
    public delegate void UpdatedViewEvent(object sender, ClientViewEventArgs eventArgs);

    public delegate void UpdatedGameViewEvent(object sender, GameViewEventArgs eventArgs);

    public delegate void MessageEventHandler(object sender, MessageEventArgs eventArgs);

    public interface IDecisionMakerManager
    {
        event UpdatedViewEvent UpdatedPlayerViewEvent;

        event MessageEventHandler MessageEvent;

        void MessageEventSafe(object sender, MessageEventArgs eventArgs);

        List<DecisionMaker> GetDecisionMakers(ObjectId gameId, ObjectId playerId);

        DecisionMaker Get(ConnectionType connectionType, ObjectId connectionId);

        DecisionMaker GetOrInsert(ObjectId gameId, ObjectId playerId, ObjectId persistentConnectionId, ConnectionType connectionType, TimeSpan timeoutPeriod);

        bool Update(DecisionMaker decisionMaker);

        void Remove(DecisionMaker decisionMaker);

        bool ClearAllConnections();
    }

    public class MessageEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    public class ClientViewEventArgs : GameViewEventArgs
    {
        public ObjectId PlayerId { get; set; }

        public ObjectId GameId { get; set; }

        public List<DecisionMaker> DecisionMakers { get; set; }

        public IView View { get; set; }
    }

    public class DecisionMakerManager : IDecisionMakerManager, ISubscriber<ClientPlayerEventArgsExt>
    {
        private IPlayerManager _playerManager;
        private IRepository<DecisionMaker> _decisionMakerRepository;
        private IRepository<GameLogic.Domain.Game> _gameRepository;
        private IRepository<GameLogic.Domain.Player> _playerRepository;
        private IGameManager _gameManager;
        private IGameViewManager _gameViewManager;
        private ICardManager _cardManager;
        private ICardLoader _cardLoader;
        private ICardService _cardService;
        private IGameUtilities _gameUtilities;
        private ISignalRConnectionManager _signalRConnectionManager;
        private IEventPublisher _eventPublisher;
        private ISubscriptionService _subscriptionService;

        public DecisionMakerManager(IPlayerManager playerManager, IRepository<DecisionMaker> decisionMakerRepository, IRepository<GameLogic.Domain.Game> gameRepository, IGameManager gameManager, ICardManager cardManager, IGameViewManager gameViewManager, ICardLoader cardLoader, IGameUtilities gameUtilities, IEventPublisher eventPublisher, ISignalRConnectionManager signalRConnectionManager, ISubscriptionService subscriptionService, ICardService cardService)
        {
            _playerManager = playerManager;
            _gameViewManager = gameViewManager;
            _gameRepository = gameRepository;
            _gameManager = gameManager;
            _decisionMakerRepository = decisionMakerRepository;
            _gameUtilities = gameUtilities;
            _signalRConnectionManager = signalRConnectionManager;
            _eventPublisher = eventPublisher;

            _cardManager = cardManager;
            _subscriptionService = subscriptionService;
            _cardService = cardService;

            _gameViewManager.UpdatedGameView += _gameViewManager_UpdatedView;

            _subscriptionService.RegisterSubscriber(this);
        }

        public event UpdatedViewEvent UpdatedPlayerViewEvent;

        public event MessageEventHandler MessageEvent;

        public void _gameViewManager_UpdatedView(object sender, GameViewEventArgs eventArgs)
        {
            foreach (var player in eventArgs.Game.Players)
            {
                var decisionMakers = GetDecisionMakers(eventArgs.Game.Id, player.Id);

                if (decisionMakers != null && decisionMakers.Any())
                {
                    foreach (var dm in decisionMakers)
                    {
                        if (dm.LastUpdated + dm.TimeoutPeriod > DateTime.UtcNow)
                        {
                            _decisionMakerRepository.Delete(dm);
                        }
                        else
                        {
                            _eventPublisher.Publish(new ClientViewEventArgs()
                            {
                                GameId = eventArgs.Game.Id,
                                PlayerId = player.Id,
                                DecisionMakers = decisionMakers,
                                View = _gameViewManager.CreateGameView(eventArgs.Game, player)
                            });
                        }
                    }
                }
            }

        }

        public void HandleEvent(ClientPlayerEventArgsExt eventArgs)
        {
            ProcessPlayerEvent(eventArgs);
        }

        public void MessageEventSafe(object sender, MessageEventArgs eventArgs)
        {
            if (MessageEvent != null)
            {
                MessageEvent(sender, eventArgs);
            }
        }

        public List<DecisionMaker> GetDecisionMakers(ObjectId gameId, ObjectId playerId)
        {
            return _decisionMakerRepository.AsQueryable()
                .Where(f => f.GameId.Equals(gameId) && f.PlayerId.Equals(playerId)).ToList();
        }

        public DecisionMaker Get(ConnectionType connectionType, ObjectId connectionId)
        {
            return _decisionMakerRepository.AsQueryable().Where(f => f.ConnectionType == connectionType && f.ConnectionId.Equals(connectionId)).FirstOrDefault();
        }

        public DecisionMaker GetOrInsert(ObjectId gameId, ObjectId playerId, ObjectId persistentConnectionId, ConnectionType connectionType, TimeSpan timeoutPeriod)
        {
            var result = Get(connectionType, persistentConnectionId);

            if (result == null)
            {
                result = new DecisionMaker()
                {
                    GameId = gameId,
                    PlayerId = playerId,
                    ConnectionId = persistentConnectionId,
                    ConnectionType = connectionType,
                    TimeoutPeriod = timeoutPeriod,
                    LastUpdated = DateTime.UtcNow
                };

                _decisionMakerRepository.Insert(result);

            }

            return result;
        }

        public bool Update(DecisionMaker decisionMaker)
        {
            return _decisionMakerRepository.Update(decisionMaker) != null;
        }

        public void Remove(DecisionMaker decisionMaker)
        {
            _decisionMakerRepository.Delete(decisionMaker);
        }

        public bool ClearAllConnections()
        {
            _decisionMakerRepository.RemoveAll();
            return true;
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
                f =>
                {
                    var ready = false;
                    game.PlayerReady.TryGetValue(f, out ready);
                    return ready;
                }))
            {
                _gameManager.StartGame(game);
                update = true;
            }
            else
            {
                _eventPublisher.Publish(new ClientPlayerEventArgs()
                {
                    Action = ClientToServerAction.Refresh
                });
            }
            return update;
        }

        public void ProcessPlayerEvent(ClientPlayerEventArgsExt eventArgs)
        {
            var game = _gameRepository.GetById(eventArgs.GameId);
            var player = game.Players.FirstOrDefault(f => f.Id.Equals(eventArgs.PlayerId));

            GetOrInsert(eventArgs.GameId, eventArgs.PlayerId, eventArgs.PermanentConnectionId, eventArgs.ConnectionType, eventArgs.TimeoutPeriod);

            if (game != null && player != null)
            {
                if (eventArgs.Action == ClientToServerAction.StartGame && game.CurrentState == GameState.Pregame)
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
                else if (eventArgs.Action == ClientToServerAction.Refresh)
                {
                    _eventPublisher.Publish(new ClientViewEventArgs()
                    {
                        PlayerId = eventArgs.PlayerId,
                        GameId = eventArgs.GameId,
                        DecisionMakers = GetDecisionMakers(eventArgs.GameId, eventArgs.PlayerId),
                        View = _gameViewManager.CreateGameView(game, player)
                    });
                }
            }
        }

        public bool PlayCard(ClientPlayerEventArgs eventArgs, Domain.Game game, Domain.Player player)
        {
            var card = eventArgs.Cards.FirstOrDefault();

            if (card == null) return false;

            _gameManager.PlayCardBlind(game, player, card, eventArgs.Action == ClientToServerAction.PlayAction);
                
            return true;
        }
    }
}