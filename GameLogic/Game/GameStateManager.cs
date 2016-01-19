using System;
using System.Collections.Generic;
using System.Linq;

using GameLogic.Player;


namespace GameLogic.Game
{
    public class GameStateEventArgs : EventArgs
    {
        public Domain.GameState PreviousGameState { get; set; }
        public Domain.GameState CurrentGameState { get; set; }
        public Domain.Game Game { get; set; }
    }

    public delegate void GameStateEventHandler(object sender, GameStateEventArgs e);

    public interface IGameStateManager
    {
        void UnreadyPlayers(Domain.Game game);

        Domain.Player CheckWinner(Domain.Game game);

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

        public void UnreadyPlayers(Domain.Game game)
        {
            foreach (var player in game.Players)
            {
                game.PlayerReady[player] = false;
            }
        }

        public void ProcessState(Domain.Game game)
        {
            if (game.PlayerReady.All(f => f.Value))
            {
                if (game.CurrentState == Domain.GameState.TurnStart)
                {
                    game.CurrentState = Domain.GameState.PlayBlind;
                    UnreadyPlayers(game);
                    _gameManager.ProcessTurnStart(game);
                }
                else if (game.CurrentState == Domain.GameState.PlayBlind)
                {
                    game.CurrentState = Domain.GameState.PlayEquip;
                    UnreadyPlayers(game);
                    _gameManager.ProcessEquip(game);
                }
                else if (game.CurrentState == Domain.GameState.PlayEquip)
                {
                    game.CurrentState = Domain.GameState.PlayAmbushAttack;
                    UnreadyPlayers(game);
                    _gameManager.ProcessAmbushAttackActions(game);
                    // check win?
                }
                else if (game.CurrentState == Domain.GameState.PlayAmbushAttack)
                {
                    game.CurrentState = Domain.GameState.PlayDraw;
                    UnreadyPlayers(game);
                    _gameManager.ProcessDraw(game);
                }
                else if (game.CurrentState == Domain.GameState.PlayDraw)
                {
                    game.CurrentState = Domain.GameState.TurnStart;
                    UnreadyPlayers(game);
                }
            }
        }

        public void ChangeState(Domain.Game game, Domain.GameState state)
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

        public void StartTurn(Domain.Game game)
        {
            ChangeState(game, Domain.GameState.TurnStart);
        }

        public void StartPlayBlind(Domain.Game game)
        {
            ChangeState(game, Domain.GameState.PlayBlind);
        }

        public void StartReveal(Domain.Game game)
        {
            ChangeState(game, Domain.GameState.Reveal);
        }

        public void StartEquip(Domain.Game game)
        {
            ChangeState(game, Domain.GameState.PlayEquip);
        }

        public void StartAmbushAttack(Domain.Game game)
        {
            ChangeState(game, Domain.GameState.PlayAmbushAttack);
        }

        public void StartDraw(Domain.Game game)
        {
            ChangeState(game, Domain.GameState.PlayDraw);
        }

        public Domain.Player CheckWinner(Domain.Game game)
        {
            Domain.Player result = null;
            if (Domain.GameState.PlayAmbushAttack == game.CurrentState)
            {
                var winningPlayers = game.Players.Where(f => f.Victories >= game.VictoryCondition);
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