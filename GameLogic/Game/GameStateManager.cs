using System;
using System.Collections.Generic;
using System.Linq;

using GameLogic.Player;


namespace GameLogic.Game
{
    public class GameStateEventArgs : EventArgs
    {
        public GameState PreviousGameState { get; set; }
        public GameState CurrentGameState { get; set; }
        public Game Game { get; set; }
    }

    public delegate void GameStateEventHandler(object sender, GameStateEventArgs e);

    public interface IGameStateManager
    {
        void UnreadyPlayers(Game game);

        IPlayer CheckWinner(Game game);

        event GameStateEventHandler GameStateChanged;
    }

    public class GameStateManager : IGameStateManager
    {
        private IGameManager _gameManager;
        private IPlayerManager _playerManager;

        public event GameStateEventHandler GameStateChanged;


        public GameStateManager(IGameManager gameManager, IPlayerManager playerManager)
        {
            _gameManager = gameManager;
            _playerManager = playerManager;
        }

        public void UnreadyPlayers(Game game)
        {
            foreach (var player in game.Players)
            {
                game.PlayerReady[player] = false;
            }
        }

        public void ProcessState(Game game)
        {
            if (game.PlayerReady.All(f => f.Value))
            {
                if (game.CurrentState == GameState.TurnStart)
                {
                    game.CurrentState = GameState.PlayBlind;
                    UnreadyPlayers(game);
                    _gameManager.ProcessTurnStart(game);
                }
                else if (game.CurrentState == GameState.PlayBlind)
                {
                    game.CurrentState = GameState.PlayEquip;
                    UnreadyPlayers(game);
                    _gameManager.ProcessEquip(game);
                }
                else if (game.CurrentState == GameState.PlayEquip)
                {
                    game.CurrentState = GameState.PlayAmbushAttack;
                    UnreadyPlayers(game);
                    _gameManager.ProcessAmbushAttackActions(game);
                    // check win?
                }
                else if (game.CurrentState == GameState.PlayAmbushAttack)
                {
                    game.CurrentState = GameState.PlayDraw;
                    UnreadyPlayers(game);
                    _gameManager.ProcessDraw(game);
                }
                else if (game.CurrentState == GameState.PlayDraw)
                {
                    game.CurrentState = GameState.TurnStart;
                    UnreadyPlayers(game);
                }
            }
        }

        public void ChangeState(Game game, GameState state)
        {
            var eventArgs = new GameStateEventArgs()
            {
                Game = game,
                PreviousGameState = game.CurrentState,
                CurrentGameState = state
            };
            game.CurrentState = state;

            GameStateChanged(this, eventArgs);
        }

        public void StartTurn(Game game)
        {
            ChangeState(game, GameState.TurnStart);
        }

        public void StartPlayBlind(Game game)
        {
            ChangeState(game, GameState.PlayBlind);
        }

        public void StartReveal(Game game)
        {
            ChangeState(game, GameState.Reveal);
        }

        public void StartEquip(Game game)
        {
            ChangeState(game, GameState.PlayEquip);
        }

        public void StartAmbushAttack(Game game)
        {
            ChangeState(game, GameState.PlayAmbushAttack);
        }

        public void StartDraw(Game game)
        {
            ChangeState(game, GameState.PlayDraw);
        }

        public IPlayer CheckWinner(Game game)
        {
            IPlayer result = null;
            if (GameState.PlayAmbushAttack == game.CurrentState)
            {
                var winningPlayers = _playerManager.Players.Where(f => f.Victories >= game.VictoryCondition);
                var winningCount = 0;
                foreach (var p in winningPlayers)
                {
                    if (p.Victories == winningCount)
                    {
                        result = null;
                    }
                    else if (p.Victories > winningCount)
                    {
                        winningCount = p.Victories;
                        result = p;
                    }
                }
            }
            return result;
        }
    }
}