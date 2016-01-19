using System;
using System.Collections.Generic;
using System.Linq;

using GameLogic.Player;
using GameLogic.Deck;
using GameLogic.External;
using GameLogic.Domain;
using Utilities.Data;

namespace GameLogic.Game
{
    public interface IGameServices
    {
        bool AddPlayer(Domain.Game game, Domain.User user);

    }


    public class GameServices : IGameServices
    {
        private IRepository<Domain.Game> _gameRepository { get; set; }

        public GameServices(IRepository<Domain.Game> gameRepository)
        {
            _gameRepository = gameRepository;
        }
        public bool AddPlayer(Domain.Game game, Domain.User user)
        {
            var playerNumber = 1;
            if (game.Players.Any())
            {
                playerNumber = game.Players.Max(f => f.PlayerNumber) + 1;
            }
            
            game.Players.Add(new Domain.Player()
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