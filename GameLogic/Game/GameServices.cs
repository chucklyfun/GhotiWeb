using System;
using System.Collections.Generic;
using System.Linq;

using GameLogic.Player;
using GameLogic.Deck;
using GameLogic.External;
using GameLogic.User;
using Utilities.Data;

namespace GameLogic.Game
{
    public interface IGameServices
    {
        bool AddPlayer(Game game, User.User user);

    }


    public class GameServices : IGameServices
    {
        private IRepository<Game> _gameRepository { get; set; }

        public GameServices(IRepository<Game> gameRepository)
        {
            _gameRepository = gameRepository;
        }
        public bool AddPlayer(Game game, User.User user)
        {
            var playerNumber = 1;
            if (game.Players.Any())
            {
                playerNumber = game.Players.Max(f => f.PlayerNumber) + 1;
            }
            
            game.Players.Add(new Player.Player()
            {
                User = user,
                Name = user.UserName,
                PlayerNumber = playerNumber
            });

            _gameRepository.Update(game);

            return true;
        }
    }
}