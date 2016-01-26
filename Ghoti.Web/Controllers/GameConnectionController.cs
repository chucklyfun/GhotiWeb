using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.Game;
using PagedList;
using Utilities.Data;
using MongoDB.Bson;
using Utilities;
using Microsoft.AspNet.SignalR;
using GameLogic.External;
using System.Threading.Tasks;
using Ninject;
using GameLogic.Domain;


namespace ghoti.web.Controllers
{
    

    public class Gamehub : Hub
    {
        private IDecisionMakerManager _decisionMakerManager{ get; set; }
        private ISignalRDecisionConnectionFactory _signalRDecisionConnectionFactory { get; set; }
        private ISerializationService _serializationService { get; set; }
        
        public Gamehub()
        {
            _decisionMakerManager = Ghoti.Web.Nancy.Bootstrapper.Kernel.Get<IDecisionMakerManager>();
            _signalRDecisionConnectionFactory = Ghoti.Web.Nancy.Bootstrapper.Kernel.Get<ISignalRDecisionConnectionFactory>();
            
            _decisionMakerManager.UpdatedViewEvent += decisionMakerManager_UpdatedView;
        }

        public void decisionMakerManager_UpdatedView(object sender, ViewEventArgs eventArgs)
        {
            var decisionMakers = _decisionMakerManager.GetOrInsertDecisionMakers(eventArgs.GameId, eventArgs.PlayerId, ConnectionType.SignalR);

            foreach(var decisionMaker in decisionMakers)
            {
                SendMessageFromServer(_serializationService.Serialize(eventArgs.View), decisionMaker.Id.ToString());
            }
            
        }

        public void connected()
        {
            System.Console.WriteLine("OnConnected");

            var playerId = new ObjectId(Context.QueryString["playerId"]);
            var gameId = new ObjectId(Context.QueryString["gameId"]);

            var decisionMaker = _decisionMakerManager.GetOrInsertDecisionMakers(gameId, playerId, ConnectionType.SignalR, Context.ConnectionId);


            base.OnConnected();
        }

        public override Task OnConnected()
        {
            System.Console.WriteLine("OnConnected");

            var playerId = new ObjectId(Context.QueryString["playerId"]);
            var gameId = new ObjectId(Context.QueryString["gameId"]);

            var decisionMaker = _decisionMakerManager.GetOrInsertDecisionMakers(gameId, playerId, ConnectionType.SignalR, Context.ConnectionId);


            return base.OnConnected();
        }

        public void SendMessageFromServer(string message, string connectionId)
        {
            Clients.Client(connectionId).SendMessage(message);
        }

        public void send(ClientPlayerEventArgs eventArgs)
        {
            var playerId = new ObjectId(Context.QueryString["playerId"]);
            var gameId = new ObjectId(Context.QueryString["gameId"]);

            _decisionMakerManager.ProcessPlayerEvent(eventArgs, gameId, playerId);            
        }
    }
}
