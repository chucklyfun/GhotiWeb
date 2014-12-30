using System;
using System.Collections.Generic;
using System.Linq;
using GameLogic.User;
using GameLogic.Game;
using PagedList;
using Utilities.Data;
using MongoDB.Bson;
using Utilities;
using Microsoft.AspNet.SignalR;
using GameLogic.External;
using System.Threading.Tasks;
using Ninject;


namespace ghoti.web.Controllers
{
    

    public class Gamehub : Hub
    {
        private IDecisionMakerManager _decisionMakerManager{ get; set; }
        private ISignalRDecisionConnectionFactory _signalRDecisionConnectionFactory { get; set; }
        private IList<ISignalRDecisionConnection> _connections { get; set; }
        private ISerializationService _serializationService { get; set; }


        public Gamehub()
        {
            _decisionMakerManager = Ghoti.Web.Nancy.Bootstrapper.Kernel.Get<IDecisionMakerManager>();
            PlayerEvent += _decisionMakerManager.PlayerEvent;
        }


        public PlayerEventHandler PlayerEvent { get; set; }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            var playerId = new ObjectId(Context.QueryString["playerId"]);
            var gameId = new ObjectId(Context.QueryString["gameId"]);

            var connection = _signalRDecisionConnectionFactory.CreateISignalRDecisionConnection(
                Context.ConnectionId, Context.User.Identity.Name, playerId, gameId);

            _connections.Add(connection);

            return base.OnConnected();
        }


        public void SendMessageFromServer(string message, string connectionId)
        {
            Clients.Client(connectionId).SendMessage(message);
        }

        public void Send(string message)
        {
            var connection = _connections.FirstOrDefault(f => Context.ConnectionId.Equals(f.ConnectionId));

            var playerEvent = _serializationService.Deserialize<PlayerEvent>(message);

            var playerEventArgs = new PlayerEventArgs()
            {
                GameId = connection.GameId,
                PlayerId = connection.PlayerId,
                PlayerEvent = playerEvent
            };
        }
    }
}
