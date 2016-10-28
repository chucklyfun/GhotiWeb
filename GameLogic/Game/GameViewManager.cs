using System;
using System.Linq;
using System.Collections.Generic;

using GameLogic.Player;
using GameLogic.Game;
using GameLogic.External;
using GameLogic.Domain;
using GameLogic.Deck;
using MongoDB.Bson;
using Utilities.Data;
using Newtonsoft.Json;
using Utilities.EventBroker;

namespace GameLogic.Game
{
    public interface IGameViewManager
    {
        event GameViewEvent UpdatedGameView;
        event PlayerViewEvent UpdatedPlayerView;
        
        void OnUpdatedGameView(object sender, GameViewEventArgs eventArgs);

        void OnUpdatedPlayerView(object sender, ServerPlayerEventArgs eventArgs);

        GameView CreateGameView(Domain.Game game, Domain.Player p);

        PlayerView CreatePlayerView(Domain.Player p);

        GameView CreateEquipmentView(Domain.Game game, Domain.Player currentPlayer, Domain.Player targetPlayer);
    }

    public delegate void GameEventHandler(object sender, GameEventArgs e);
    public delegate void PlayerEventHandler(ServerPlayerEventArgs e);

    public class GameViewEventArgs : EventArgs
    {
        public Domain.Game Game { get; set; }

        public Domain.ServerToClientAction Action { get; set; }
    }


    public class ServerPlayerEventArgs : EventArgs
    {
        public ObjectId GameId { get; set; }

        public Domain.Player Player { get; set; }

        public Domain.ServerToClientAction Action { get; set; }

        public List<Domain.CardInstance> Cards { get; set; }
    }

    public delegate void GameViewEvent(object sender, GameViewEventArgs eventArgs);

    public delegate void PlayerViewEvent(object sender, ServerPlayerEventArgs eventArgs);

    public class GameEventArgs : EventArgs
    {
        public GameView GameView { get; set; }
    }

    public class ClientPlayerEventArgs
    {
        public ClientToServerAction Action { get; set; }

        public List<CardInstance> Cards { get; set; }

        public ClientPlayerEventArgs()
        {
            Cards = new List<ObjectId>();
        }
    }

    public class ClientPlayerEventArgsExt : ClientPlayerEventArgs 
    {
        public ObjectId GameId { get; set; }

        public ObjectId PlayerId { get; set; }

        public ObjectId PermanentConnectionId { get; set; }

        public TimeSpan TimeoutPeriod { get; set; }

        public ConnectionType ConnectionType { get; set; }

    }


    public class GameViewManager : IGameViewManager
    {
        private IEventPublisher _eventPublisher;
        private ICardManager _cardManager;
        private Player.PlayerManager _playerManager;
        

        public GameViewManager(Player.PlayerManager playerManager, IEventPublisher eventPublisher, ICardManager cardManager)
        {
            _playerManager = playerManager;
            _eventPublisher = eventPublisher;
            _cardManager = _cardManager;
        }

        public void OnUpdatedGameView(object sender, GameViewEventArgs eventArgs)
        {
            if (UpdatedGameView != null)
            {
                UpdatedGameView(sender, eventArgs);
            }
        }


        public void OnUpdatedPlayerView(object sender, ServerPlayerEventArgs eventArgs)
        {
            if (UpdatedPlayerView != null)
            {
                UpdatedPlayerView(sender, eventArgs);
            }
        }

        public event GameViewEvent UpdatedGameView;

        public event PlayerViewEvent UpdatedPlayerView;


        public GameView CreateGameView(Domain.Game game, Domain.Player p)
        {
            var result = new GameView();
            result.GameId = game.Id;

            result.PlayerCardIds.AddRange(p.Hand.Select(f => f.Id));
            result.MonsterCardIds.AddRange(game.MonsterQueue.Select(f => f.Id));

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
            pv.PlayerId = p.Id;

            pv.HandSize = p.Hand.Count();
            pv.PlayerAttack = _playerManager.CalculatePlayerAttack(p);
            pv.PlayerHold = _playerManager.CalculatePlayerHold(p);
            pv.PlayerDraw = _playerManager.CalculatePlayerDraw(p);
            pv.PlayerKeep = _playerManager.CalculatePlayerKeep(p);
            pv.PlayerSpeed = _playerManager.CalculatePlayerSpeed(p);

            pv.ArmorCardIds.AddRange(p.Equipment.Where(f => f.EquipmentType == EquipmentType.Armor).Select(f => f.Id));
            pv.BootsCardIds.AddRange(p.Equipment.Where(f => f.EquipmentType == EquipmentType.Boots).Select(f => f.Id));
            pv.HelmetCardIds.AddRange(p.Equipment.Where(f => f.EquipmentType == EquipmentType.Helmet).Select(f => f.Id));
            pv.HandCardIds.AddRange(p.Equipment.Where(f => f.EquipmentType == EquipmentType.Hand || f.EquipmentType == EquipmentType.TwoHand).Select(f => f.Id));
            pv.OtherEquipmentCardIds.AddRange(p.Equipment.Where(f => f.EquipmentType == EquipmentType.Other).Select(f => f.Id));

            return pv;
        }

        public GameView CreateEquipmentView(Domain.Game game, Domain.Player currentPlayer, Domain.Player targetPlayer)
        {
            var gv = CreateGameView(game, currentPlayer);
            foreach (var card in targetPlayer.Equipment)
            {
                gv.ChooseCardIds.Add(card.Id);
            }

            return gv;
        }
    }
}