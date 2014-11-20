using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLogic.Game
{

    public interface IGameUtilities
    {
        int CalculateDrawMax(Game game);
        int CalculateKeepMax(Game game);
    }

    public class GameUtilities : IGameUtilities
    {
        public int CalculateDrawMax(Game game)
        {
            return game.DrawActions.Max(f => f.Key.CalculatePlayerDraw() + f.Value.ActionDrawBonus);
        }
        public int CalculateKeepMax(Game game)
        {
            return game.DrawActions.Max(f => f.Key.CalculatePlayerKeep() + f.Value.ActionKeepBonus);
        }
    }
}
