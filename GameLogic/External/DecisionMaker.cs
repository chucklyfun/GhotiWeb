using System;
using Utilities;

namespace GameLogic.External
{
    public interface IDecisionMaker
    {
        Player.IPlayer Player { get; set; }

        void SendGameView(GameView gv);
    }
}