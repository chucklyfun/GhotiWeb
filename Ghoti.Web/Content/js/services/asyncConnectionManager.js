
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
                    sendMessageFromServer: function (data) 
                    {
                        $rootScope.gv = data;
                        $rootScope.$apply();
                    },
                    sendMessageFromServerAll: function (data) {
                        console.log(data)
                    }
                },
                logging : true,
                useSharedConnection : false
            });

            return hub;
        },           
    };
}]);