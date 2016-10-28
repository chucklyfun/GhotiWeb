using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Player;
using GameLogic.Data;
using Utilities;
using System.IO;
using System.Web;
using GameLogic.Domain;
using System.Web.Hosting;

namespace GameLogic.Game
{

    public interface IGameUtilities
    {
        int CalculateDrawMax(Domain.Game game);
        int CalculateKeepMax(Domain.Game game);

        string GetPlayerCardFileName(string fileName);

        string GetMonsterCardFileName(string fileName);

        IEnumerable<Domain.PlayerCard> LoadPlayerCardDeck(string fileName);

        IEnumerable<Domain.MonsterCard> LoadMonsterCardDeck(string fileName);
    }

    public class GameUtilities : IGameUtilities
    {
        private IPlayerManager _playerManager;
        private ICardLoader _cardLoader;
        private ISettingsManager _settingsManager;

        public GameUtilities(IPlayerManager playerManager, ICardLoader cardLoader, ISettingsManager settingsManager)
        {
            _playerManager = playerManager;
            _cardLoader = cardLoader;
            _settingsManager = settingsManager;
        }

        public int CalculateDrawMax(Domain.Game game)
        {
            return game.DrawActions.Max(f =>  _playerManager.CalculatePlayerDraw(f.Key) + f.Value.ActionDrawBonus);
        }
        public int CalculateKeepMax(Domain.Game game)
        {
            return game.DrawActions.Max(f => _playerManager.CalculatePlayerKeep(f.Key) + f.Value.ActionKeepBonus);
        }

        public IEnumerable<Domain.PlayerCard> LoadPlayerCardDeck(string fileName)
        {
            var configuration = _settingsManager.GetConfiguration();
            var path = HostingEnvironment.MapPath(Path.Combine(configuration.DataPath, fileName));
            return _cardLoader.LoadPlayerCardFile(path);
        }

        public IEnumerable<Domain.MonsterCard> LoadMonsterCardDeck(string fileName)
        {
            var result = new Domain.Deck();

            var configuration = _settingsManager.GetConfiguration();
            var path = HostingEnvironment.MapPath(Path.Combine(configuration.DataPath, fileName));
            return _cardLoader.LoadMonsterCardFile(path);
        }

        public string GetPlayerCardFileName(string gameVersion)
        {
            return "PlayerCards_" + gameVersion + ".csv";
        }

        public string GetMonsterCardFileName(string gameVersion)
        {
            return "MonsterCards_" + gameVersion + ".csv";
        }
    }
}
