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
            _signalRDecisionConnectionFactory = Ghoti.Web.Nancy.Bootstrapper.Kernel.Get<ISignalRDecisionConnectionFactory>();
            PlayerEvent += _decisionMakerManager.PlayerEvent;

            _connections = new List<ISignalRDecisionConnection>();
        }


        public PlayerEventHandler PlayerEvent { get; set; }

        public void connected()
        {
            string name = Context.User.Identity.Name;
            var playerId = new ObjectId(Context.QueryString["playerId"]);
            var gameId = new ObjectId(Context.QueryString["gameId"]);
        }

        public override Task OnConnected()
        {
            System.Console.WriteLine("OnConnected");
            return base.OnConnected();
        }

        public void SendMessageFromServer(string message, string connectionId)
        {
            Clients.Client(connectionId).SendMessage(message);
        }

        public Tuple<ObjectId, ObjectId> GetConnectionIds()
        {
            var playerId = new ObjectId(Context.QueryString["playerId"]);
            var gameId = new ObjectId(Context.QueryString["gameId"]);

            return new Tuple<ObjectId, ObjectId>(gameId, playerId);
        }

        public void send(PlayerEvent eventArgs)
        {
            var connectionId = GetConnectionIds();
            var playerEventArgs = new PlayerEventArgs()
            {
                GameId = connectionId.Item1,
                PlayerId = connectionId.Item2,
                PlayerEvent = eventArgs
            };
        }
    }
}
