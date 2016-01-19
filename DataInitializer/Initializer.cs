using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLogic.Game;
using Utilities.Data;
using GameLogic.Domain;

namespace DataInitializer
{
    public interface IInitializer
    {
        void InitializeAdminUser();

        void InitializePlayers(int count);
    }

    public class Initializer : IInitializer
    {
        public IRepository<User> _userRepository;

        public IRepository<Game> _gameRepository;

        public ILoremIpsum _loremIpsum;

        public Initializer(IRepository<User> userRepository, ILoremIpsum loremIpsum)
        {
            _loremIpsum = loremIpsum;
            _userRepository = userRepository;
        }

        public void InitializeAdminUser()
        {
            _userRepository.Insert(new User()
            {
                Email = "change-this",
                FullName = "admin",
                UserName = "admin",
                ShortName = "admin"
            });
        }

        public void InitializePlayers(int count)
        {
            for (int i = 0; i < count; ++i)
            {
                var username = string.Format("{0}@ghoti.com", _loremIpsum.GetName(1));

                _userRepository.Insert(
                    new User() { Email = username, FullName = _loremIpsum.GetName(2), UserName = username, ShortName = _loremIpsum.GetName(1) });
            }
        }
    }
}
