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
        
        string GetPlayerCardFileName(GameLogic.Domain.Game game);

        string GetMonsterCardFileName(GameLogic.Domain.Game game);

        Deck<Domain.PlayerCard> LoadPlayerCardDeck(string fileName);

        Deck<Domain.MonsterCard> LoadMonsterCardDeck(string fileName);
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

        public Deck<Domain.PlayerCard> LoadPlayerCardDeck(string fileName)
        {
            var result = new Domain.Deck<Domain.PlayerCard>();

            var configuration = _settingsManager.GetConfiguration();
            var path = HostingEnvironment.MapPath(Path.Combine(configuration.DataPath, fileName));
            result.DrawPile = new Stack<Domain.PlayerCard>(_cardLoader.LoadPlayerCardFile(path));

            return result;
        }

        public Deck<Domain.MonsterCard> LoadMonsterCardDeck(string fileName)
        {
            var result = new Domain.Deck<Domain.MonsterCard>();

            var configuration = _settingsManager.GetConfiguration();
            var path = HostingEnvironment.MapPath(Path.Combine(configuration.DataPath, fileName));
            result.DrawPile = new Stack<Domain.MonsterCard>(_cardLoader.LoadMonsterCardFile(path));

            return result;
        }

        public string GetPlayerCardFileName(GameLogic.Domain.Game game)
        {
            return "PlayerCards_" + game.Version + ".csv";
        }

        public string GetMonsterCardFileName(GameLogic.Domain.Game game)
        {
            return "MonsterCards_" + game.Version + ".csv";
        }
    }
}
