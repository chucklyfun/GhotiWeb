using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLogic.Player;

namespace GameLogic.Game
{

    public interface IGameUtilities
    {
        int CalculateDrawMax(Domain.Game game);
        int CalculateKeepMax(Domain.Game game);
    }

    public class GameUtilities : IGameUtilities
    {
        private IPlayerManager _playerManager;
        public GameUtilities(IPlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        public int CalculateDrawMax(Domain.Game game)
        {
            return game.DrawActions.Max(f =>  _playerManager.CalculatePlayerDraw(f.Key) + f.Value.ActionDrawBonus);
        }
        public int CalculateKeepMax(Domain.Game game)
        {
            return game.DrawActions.Max(f => _playerManager.CalculatePlayerKeep(f.Key) + f.Value.ActionKeepBonus);
        }
    }
}
