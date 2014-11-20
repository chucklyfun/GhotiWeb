using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.User;
using GameLogic.Game;
using PagedList;
using Utilities.Data;
using MongoDB.Bson;
using Utilities;

namespace ghoti.web.Controllers
{
    public class GameController : Nancy.NancyModule
    {

        private readonly IGameServices _gameServices;

        private readonly IGameManager _gameManager;
        private readonly IRepository<Game> _gameRepository;
        private readonly IRepository<User> _userRepository;
        private readonly ISerializationService _serializationService;

        public GameController(IGameServices gameServices, IRepository<User> userRepository, IRepository<Game> gameRepository, ISerializationService serializationService)
        {
            _gameServices = gameServices;
            _userRepository = userRepository;
            _gameRepository = gameRepository;
            _serializationService = serializationService;

            Get["/api/Game/Get"] = _ => serializationService.Serialize(_gameRepository.AsQueryable());

            Get["/api/Game/Get/0"] = _ =>
                {
                    var result = new Game();
                    _gameRepository.Insert(result);
                    return _serializationService.Serialize(result);
                };

            Get["/api/Game/Get/{Id}"] = _ =>
                {
                    return _serializationService.Serialize(_gameRepository.GetById(new ObjectId(_.Id)));
                };

            Get["/api/Game/{gameId}/AddPlayer/{userId}"] = (_) =>
                {
                    var result = false;
                    var user = _userRepository.GetById(new ObjectId(_.userId));
                    var game = _gameRepository.GetById(new ObjectId(_.gameId));
                    if (user != null && game != null)
                    {
                        result = _gameServices.AddPlayer(game, user);
                    }

                    return result;
                };

            Put["/api/Game"] = (_) =>
                {
                    var game = _ as Game;
                    return _gameRepository.Update(game);
                };

        }
    }
}
