
    //Articles service used for articles REST endpoint
angular.module('routerApp')
    .factory('connection', ['$rootScope', 'Hub', function ($rootScope, Hub) {
        //Override socket.on to $apply the changes to angular
    return {
        initialize: function(gameId, playerId)
        {
            var hub = new Hub('gamehub',
            {
                methods: ['send', 'connected'],
                queryParams: {
                    'gameId': gameId,
                    'playerId': playerId
                },
                listeners:
                {
                    sendSendMessageFromServer: function (id) 
                    {
                        var employee = find(id);
                        employee.Locked = true;
                        $rootScope.$apply();
                    },
                },
                logging : true,
                useSharedConnection : false
            });

            return hub;
        },           
    };
}]);