
define(['angular', 'nggrid', 'app/config', 'services/gameRest', 'services/cards', 'services/userRest', 'services/asyncConnectionManager'], function (angular, nggrid, config, gameRest, cards, userRest, AsyncConnectionManager) {
    /**
     * Create a gameview
     */
    angular.module('mean.system').controller('GameController', ['$rootScope', '$scope', '$http', '$routeParams', '$location', 'Global', '$gameRest', '$userRest', "connection",
        function ($rootScope, $scope, $http, $routeParams, $location, Global, $gameRest, $userRest, connection) {
        
        $scope.global = Global;

        $scope.init = function () {
            $scope.GameActions = {
                'Wait' : {
                    Text : 'Wait',
                    Value : 1
                },
                'DrawAndDiscard' : {
                    Text : 'DrawAndDiscard',
                    Value : 2
                },
                'PlayAction' : {
                    Text : 'PlayAction',
                    Value : 3
                },
                'PlayEquipment' : {
                    Text : 'PlayEquipment',
                    Value : 4
                },
                'ViewEquipment' : {
                    Text : 'ViewEquipment',
                    Value : 5
                },
                'ViewHand': {
                    Text: 'ViewHand',
                    Value: 6
                }
            };

            $gameRest.get($routeParams.id).success(function (data)
            {
                $scope.gv = data;
                $scope.hubs = [];

                for (var p in $scope.gv.Players) {
                    var h = connection.initialize($scope.gv.Id, $scope.gv.Players[p].User.Id);
                    var playerEvent =
                    {
                        Action: '1',
                        Cards: []
                    };
                    // h.SendMessageFromClient(playerEvent);
                    $scope.hubs.push(h);

                }
            });            
        };

        $scope.SendHello = function () {
            var playerEvent =
                    {
                        Action: '1',
                        Cards: []
                    };
            $scope.hubs[0].send(playerEvent);
        };

        $scope.ClientTest = function () {
            $http({ method: 'GET', url: 'api/game/clienttest' });
        };

        $scope.showcards = function()
        {
            $http({method: 'GET', url: 'api/game/getcard'})
                .success(function(data)
                    {
                        $scope.gv.Cards.push(data);
                    })
                .error(function(data)
                    {
                        $scope.error = data;
                    });
        };
    }]);
});