using System;
using System.Linq;

using GameLogic.Player;

namespace GameLogic.External
{
    public interface IGameViewManager
    {
        GameView CreateGameView(IPlayer p);
        PlayerView CreatePlayerView(IPlayer p);
        GameView CreateEquipmentView(IPlayer currentPlayer, IPlayer targetPlayer);

        void SendHandView(IPlayer player, ExternalAction action);
        void SendKeepView(IPlayer player, ExternalAction action);
    }


    public class GameViewManager
    {
        private IPlayerManager _playerManager;
        private IDecisionMakerManager _decisionMakerManager;

        public GameViewManager(IPlayerManager playerManager, IDecisionMakerManager decisionMakerManager)
        {
            _playerManager = playerManager;
            _decisionMakerManager = decisionMakerManager;
        }


        public GameView CreateGameView(IPlayer p)
        {
            var result = new GameView();
            foreach (var card in p.Hand)
            {
                result.CardIds.Add(card.Id);
            }

            result.CurrentPlayer = CreatePlayerView(p);
            foreach (IPlayer op in _playerManager.Players.Where(f => f.User.Id != p.User.Id))
            {
                result.OtherPlayers.Add(CreatePlayerView(op));
            }
            result.CurrentPlayer = CreatePlayerView(p);
            return result;
        }

        public PlayerView CreatePlayerView(IPlayer p)
        {
            var pv = new PlayerView();
            pv.HandSize = p.Hand.Count();
            pv.PlayerAttack = p.CalculatePlayerAttack();
            pv.PlayerHold = p.CalculatePlayerHold();
            pv.PlayerDraw = p.CalculatePlayerDraw();
            pv.PlayerKeep = p.CalculatePlayerKeep();
            pv.PlayerSpeed = p.CalculatePlayerSpeed();

            return pv;
        }

        public GameView CreateEquipmentView(IPlayer currentPlayer, IPlayer targetPlayer)
        {
            var gv = CreateGameView(currentPlayer);
            foreach (var card in targetPlayer.Equipment)
            {
                gv.CardIds.Add(card.Id);
            }

            return gv;
        }

        public void SendHandView(IPlayer player, ExternalAction action)
        {
            var gv = CreateGameView(player);
            gv.CurrentAction = action;
            gv.CardIds = player.Hand.Select(f => f.Id).ToList();

            _decisionMakerManager.ProcessGameView(gv);
        }

        public void SendKeepView(IPlayer player, ExternalAction action)
        {
            var gv = CreateGameView(player);
            gv.CurrentAction = action;
            gv.CardIds = player.DrawCards.Select(f => f.Id).ToList();

            _decisionMakerManager.ProcessGameView(gv);
        }
    }
}