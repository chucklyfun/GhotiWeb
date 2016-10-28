
    //Articles service used for articles REST endpoint
angular.module('routerApp')
    .factory('connection', ['$rootScope', 'Hub', function ($rootScope, Hub) {
        //Override socket.on to $apply the changes to angular
    return {
        initialize: function(gameId, playerId, connectionId)
        {
            var hub = new Hub('gamehub',
            {
                methods: ['send'],
                queryParams: {
                    'gameId': gameId,
                    'playerId': playerId,
                    'connectionId' : connectionId
                },
                listeners:
                {
                    sendMessageFromServer: function (playerServerView) 
                    {
                        var playerView = $rootScope.PlayersMap[playerServerView.CurrentPlayer.PlayerId];

                        if (playerView != undefined)
                        {
                            playerView.View = playerServerView;
                            $rootScope.$apply();
                        }                        
                    },
                    sendMessageFromServerAll: function (data) {
                        console.log(data + ', gameId: ' + gameId + ', playerId: ' + playerId + ', permanentConnectionId: ' + connectionId)
                    }
                },
                logging : true,
                useSharedConnection : false
            });

            return hub;
        },           
    };
}]);