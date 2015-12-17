using System;
using System.Collections.Generic;
using System.Linq;

using GameLogic.Player;
using GameLogic.Game;
using MongoDB.Bson;

namespace GameLogic.External
{
    public delegate void GameEventHandler(object sender, GameEventArgs e);
    public delegate void PlayerEventHandler(PlayerEventArgs e);

    public class GameEventArgs : EventArgs
    {
	    public GameView GameView{ get; set; }
    }
    
    public class PlayerEvent
    {
        public ExternalAction Action { get; set; }

        public IList<ObjectId> Cards { get; set; }
    }

    public class PlayerEventArgs
    {
        public ObjectId PlayerId { get; set; }

        public ObjectId GameId { get; set; }

        public PlayerEvent PlayerEvent { get; set; }
    }

    public interface IDecisionMakerManager
    {	
	    IDictionary<IPlayer, GameView > PlayerState{get;set;}
	
	    IDictionary<ObjectId, IDecisionMaker> DecisionMakers {get;set;}
	    bool ProcessGameView(GameView gv);
        void PlayerEvent(PlayerEventArgs eventArgs);
    }

    public class DecisionMakerManager : IDecisionMakerManager
    {
	    private IPlayerManager _playerManager;
	
	    public DecisionMakerManager (IPlayerManager playerManager)
	    {
            DecisionMakers = new Dictionary<ObjectId, IDecisionMaker>();
		    _playerManager = playerManager;
	    }

        public IDictionary<IPlayer, GameView> PlayerState { get; set; }
        public IDictionary<ObjectId, IDecisionMaker> DecisionMakers { get; set; }
        public bool ProcessGameView(GameView gv)
        {
            return true;
        }
        public void PlayerEvent(PlayerEventArgs eventArgs)
        {
            
        }
	
	    public bool SendGameView(GameView gv)
	    {
		    var result = true;
		
		    IDecisionMaker d = null;
		    if (DecisionMakers.TryGetValue(gv.CurrentPlayer.PlayerId, out d))
		    {
			    d.SendGameView(gv);
		    }
		    else
		    {
			    result = false;
		    }
		    return result;
	    }
	
	    public bool ProcessGameAction(GameView gv)
	    {
		    var result = true;
		
		    IDecisionMaker d = null;
		    if (DecisionMakers.TryGetValue(gv.CurrentPlayer.PlayerId, out d))
		    {
                d.SendGameView(gv);
		    }
		    else
		    {
			    result = false;
		    }
		    return result;
	    }
    }
}