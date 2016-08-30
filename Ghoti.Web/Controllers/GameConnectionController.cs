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

        public Dictionary<ObjectId, string> ConnectionIds { get; set; }
        
        public Gamehub(IDecisionMakerManager decisionMakerManager, ISignalRDecisionConnectionFactory signalRDecisionConnectionFactory, ISerializationService serializationService)
        {
            _decisionMakerManager = decisionMakerManager;
            _signalRDecisionConnectionFactory = signalRDecisionConnectionFactory;
            _serializationService = serializationService;
            
            _decisionMakerManager.UpdatedPlayerViewEvent +=_decisionMakerManager_UpdatedPlayerViewEvent;
            _decisionMakerManager.MessageEvent += _decisionMakerManager_MessageEvent;

            ConnectionIds = new Dictionary<ObjectId, string>();
        }


        public void _decisionMakerManager_UpdatedPlayerViewEvent(object sender, ClientViewEventArgs eventArgs)
        {
            var decisionMakers = _decisionMakerManager.GetDecisionMakers(eventArgs.GameId, eventArgs.PlayerId);

            foreach(var decisionMaker in decisionMakers)
            {
                SendMessageToClient(_serializationService.Serialize(eventArgs.View), decisionMaker.ConnectionId);
            }
            
        }

        public void _decisionMakerManager_MessageEvent(object sender, MessageEventArgs eventArgs)
        {
            SendMessageToClient(eventArgs.Message);
        }

        public override Task OnConnected()
        {
            System.Console.WriteLine("OnConnected");

            var playerId = ObjectId.Parse(Context.QueryString["playerId"]);
            var gameId = ObjectId.Parse(Context.QueryString["gameId"]);
            var connectionId = ObjectId.Parse(Context.QueryString["connectionId"]);

            ConnectionIds[connectionId] = Context.ConnectionId;

            var decisionMaker = _decisionMakerManager.GetOrInsert(gameId, playerId, connectionId, ConnectionType.SignalR);
            
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionId = ObjectId.Parse(Context.QueryString["connectionId"]);

            ConnectionIds.Remove(connectionId);

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var playerId = ObjectId.Parse(Context.QueryString["playerId"]);
            var gameId = ObjectId.Parse(Context.QueryString["gameId"]);
            var connectionId = ObjectId.Parse(Context.QueryString["connectionId"]);

            ConnectionIds[connectionId] = Context.ConnectionId;

            var decisionMaker = _decisionMakerManager.GetOrInsert(gameId, playerId, connectionId, ConnectionType.SignalR);

            return base.OnReconnected();            
        }

        public void SendMessageToClient(string message, ObjectId connectionId)
        {
            string signalRConnectionId;
            if (ConnectionIds.TryGetValue(connectionId, out signalRConnectionId))
            {
                Clients.Client(signalRConnectionId).sendMessageFromServer(message);
            }
            else
            {
                // TODO handle this somehow?
            }
        }

        public void SendMessageToClient(string message)
        {
            Clients.All.sendMessageFromServerAll(message);
        }

        public void send(ClientPlayerEventArgs eventArgs)
        {
            var playerId = ObjectId.Parse(Context.QueryString["playerId"]);
            var gameId = ObjectId.Parse(Context.QueryString["gameId"]);

            _decisionMakerManager.ProcessPlayerEvent(eventArgs, gameId, playerId);            
        }
    }
}
