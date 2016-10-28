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
using Utilities.EventBroker;
using GameLogic.External;

namespace ghoti.web.Controllers
{
    public interface IGameHub
    {
        Task OnConnected();

        Task OnDisconnected(bool stopCalled);

        Task OnReconnected();

        void SendMessageToClient(IView view, ObjectId permanentId);

        void SendMessageToClient(string message);

        void send(ClientPlayerEventArgs eventArgs);
    }

    public class Gamehub : Hub, IGameHub, ISubscriber<ClientViewEventArgs>, ISubscriber<MessageEventArgs>
    {
        
        private ISerializationService _serializationService { get; set; }
        private ISettingsManager _settingsManager { get; set; }
        private ISignalRConnectionManager _signalRConnectionManager { get; set; }
        private ISubscriptionService _subscriptionService { get; set; }
        private IEventPublisher _eventPublisher { get; set; }

        public Gamehub(ISerializationService serializationService, ISettingsManager settingsManager, ISignalRConnectionManager signalRConnectionManager, ISubscriptionService subscriptionService, IEventPublisher eventPublisher)
        {
            _serializationService = serializationService;
            _settingsManager = settingsManager;
            _signalRConnectionManager = signalRConnectionManager;
            _subscriptionService = subscriptionService;
            _eventPublisher = eventPublisher;
        }

        public override Task OnConnected()
        {
            _subscriptionService.RegisterSubscriber((ISubscriber<ClientViewEventArgs>)this);
            _subscriptionService.RegisterSubscriber((ISubscriber<MessageEventArgs>)this);

            System.Console.WriteLine("OnConnected");

            var playerId = ObjectId.Parse(Context.QueryString["playerId"]);
            var gameId = ObjectId.Parse(Context.QueryString["gameId"]);
            var connectionId = ObjectId.Parse(Context.QueryString["connectionId"]);

            _signalRConnectionManager.UpdateConnectionId(connectionId, Context.ConnectionId);            

            _eventPublisher.Publish(new ClientPlayerEventArgsExt()
            {
                Action = ClientToServerAction.Refresh,
                GameId = gameId,
                PlayerId = playerId,
                PermanentConnectionId = connectionId,
                ConnectionType = GameLogic.Domain.ConnectionType.SignalR,
                TimeoutPeriod = TimeSpan.FromMinutes(10)
            });
            
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var connectionId = ObjectId.Parse(Context.QueryString["connectionId"]);


            _signalRConnectionManager.RemoveConnectionId(connectionId);
            

            return base.OnDisconnected(stopCalled);
        }

        public void HandleEvent(ClientViewEventArgs eventMessage)
        {
            foreach(var decisionMaker in eventMessage.DecisionMakers.Where(f => f.ConnectionType == ConnectionType.SignalR))
            {
                SendMessageToClient(eventMessage.View, decisionMaker.ConnectionId);
            }            
        }

        public void HandleEvent(MessageEventArgs eventMessage)
        {
            SendMessageToClient(eventMessage.Message);
        }


        public override Task OnReconnected()
        {
            _subscriptionService.RegisterSubscriber((ISubscriber<ClientViewEventArgs>)this);
            _subscriptionService.RegisterSubscriber((ISubscriber<MessageEventArgs>)this);

            var playerId = ObjectId.Parse(Context.QueryString["playerId"]);
            var gameId = ObjectId.Parse(Context.QueryString["gameId"]);
            var connectionId = ObjectId.Parse(Context.QueryString["connectionId"]);

            _signalRConnectionManager.UpdateConnectionId(connectionId, Context.ConnectionId);

            _eventPublisher.Publish(new ClientPlayerEventArgsExt()
            {
                Action = ClientToServerAction.Refresh,
                PermanentConnectionId = connectionId,
                GameId = gameId,
                PlayerId = playerId,
                ConnectionType = ConnectionType.SignalR,
                TimeoutPeriod = TimeSpan.FromMinutes(10)
            });

            return base.OnReconnected();            
        }

        public void SendMessageToClient(IView view, ObjectId permanentId)
        {
            var signalRId = _signalRConnectionManager.GetConnectionId(permanentId);

            if (!string.IsNullOrWhiteSpace(signalRId))
            {
                Clients.Client(signalRId).sendMessageFromServer(view);
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
            var connectionId = ObjectId.Parse(Context.QueryString["connectionId"]);

            _eventPublisher.Publish( new ClientPlayerEventArgsExt()
            {
                Action = eventArgs.Action,
                Cards = eventArgs.Cards,
                GameId = gameId, 
                PlayerId = playerId,
                PermanentConnectionId = connectionId,
                ConnectionType = ConnectionType.SignalR,
                TimeoutPeriod = TimeSpan.FromMinutes(10)
            });
        }
    }
}
