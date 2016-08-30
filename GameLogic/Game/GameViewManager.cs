using System;
using System.Linq;
using System.Collections.Generic;

using GameLogic.Player;
using GameLogic.Game;
using GameLogic.External;
using GameLogic.Domain;
using MongoDB.Bson;
using Utilities.Data;
using Newtonsoft.Json;

namespace GameLogic.Game
{
    public interface IGameViewManager
    {
        event GameViewEvent UpdatedGameView;
        event PlayerViewEvent UpdatedPlayerView;
        
        void OnUpdatedGameView(object sender, GameViewEventArgs eventArgs);

        void OnUpdatedPlayerView(object sender, ServerPlayerEventArgs eventArgs);
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

        public List<Domain.PlayerCard> Cards { get; set; }
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

        [JsonConverter(typeof(ListObjectIdConverter))]
        public List<ObjectId> Cards { get; set; }
    }


    public class GameViewManager : IGameViewManager
    {
        private Player.PlayerManager _playerManager;

        public GameViewManager(Player.PlayerManager playerManager)
        {
            _playerManager = playerManager;
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
    }
}