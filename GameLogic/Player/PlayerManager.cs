using System;
using System.Collections.Generic;
using System.Linq;

namespace GameLogic.Player
{
    public interface IPlayerManager
    {
        IList<IPlayer> Players { get; set; }
        IPlayer NextPlayer();
        void NextRound();
    }

    public class PlayerManager : IPlayerManager
    {
        public PlayerManager()
        {
        }

        public IList<IPlayer> Players { get; set; }

        public IPlayer NextPlayer()
        {
            var currentPlayer = Players.FirstOrDefault();
            Players.Remove(Players.FirstOrDefault());
            Players.Add(currentPlayer);

            currentPlayer = Players.FirstOrDefault();
            return currentPlayer;
        }

        public void NextRound()
        {
            NextPlayer();
        }
    }

}