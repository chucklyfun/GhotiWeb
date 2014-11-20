define(['angular', 'angularsignalr'], function (angular, angularsignalr) {
    //Articles service used for articles REST endpoint
    angular.module('mean.system')
    .factory('connection', ['$rootScope', 'Hub', function ($rootScope, Hub) {
        //Override socket.on to $apply the changes to angular
        return {
            initialize: function(gameId, playerId)
            {
                var hub = new Hub('GameHub',
                {
                    listeners:
                    {
                        'SendMessageFromServer': function (gameEvent)
                        {
                            $rootScope.Message = SendMessageFromServer(gameEvent);
                        }
                    },
                    methods: ['SendMessageFromClient'],
                    queryParams: {
                        'gameId': gameId,
                        'playerId': playerId
                    },
                    logging : true,
                    useSharedConnection : false

                });

                return hub;
            },           
        };
    }]);
});