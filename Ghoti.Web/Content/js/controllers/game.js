
define(['angular', 'nggrid', 'app/config', 'services/gameRest', 'services/cards', 'services/userRest', 'services/asyncConnectionManager'], function (angular, nggrid, config, gameRest, cards, userRest, AsyncConnectionManager) {
    /**
     * Create a gameview
     */
    angular.module('mean.system').controller('GameController', ['$rootScope', '$scope', '$http', '$routeParams', '$location', 'Global', '$gameRest', '$userRest', "connection",
        function ($rootScope, $scope, $http, $routeParams, $location, Global, $gameRest, $userRest, connection) {
        
            $scope.global = Global;

            $scope.GameActions = [
                {
                    Text : 'Wait',
                    Value : 1
                },
                {
                    Text : 'DrawAndDiscard',
                    Value : 2
                },
                {
                    Text : 'PlayAction',
                    Value : 3
                },
                {
                    Text : 'PlayEquipment',
                    Value : 4
                },
                {
                    Text: 'StartGame',
                    Value: 5
                }
            ];

            $scope.createPlayerView = function(player)
            {
                return result =
                {
                    Id: player.Id,
                    SelectedCards: [],
                    CurrentAction: $scope.GameActions[0]
                };
            }

            $scope.gdTestPlayersOptions = { data: 'players', columnDefs: 'gdTestPlayersCols', enableColumnResize: 'true' };
            $scope.gdTestPlayersCols = [
                { 'field': 'Name', displayName: 'Name' },
                { 'cellTemplate': '<select ng-model="row.entity.CurrentAction.Value" ng-options="g.Text for g in GameActions" />' },
                { 'cellTemplate': '<button id="btnAction" type="button" class="btn btn-primary" ng-click="Action(row.entity.Id)" >Action</button>' },
                { 'cellTemplate': '<li data-ng-repeat="c in row.entity.Hand"><span>{{c.Name}}</span><span>{{c.CardNumber}}</span><img ng-src="{{c.ImageUrl}}" ng-click="toggleCard(row.entity.Id, c.Id)"/></li>' }
            ];

            $scope.players = [];

            $scope.init = function ()
            {
                $gameRest.get($routeParams.id).success(function (data)
                {
                    $scope.gv = data;
                    $scope.hubs = [];

                    for (var p in $scope.gv.Players) {
                        var id = $scope.gv.Players[p].User.Id;

                        var h = connection.initialize($scope.gv.Id, id);
                        h.connection.start().done(function () {
                                console.log('Now connected, connection ID=' + $.connection.hub.id);
                                h.connected();
                            })
                            .fail(function(){ console.log('Could not Connect!'); });


                        $scope.hubs.push(h);

                        $scope.players.push($scope.createPlayerView($scope.gv.Players[p]));  
                    }
                });            
            };

        $scope.toggleCard = function(playerId, cardId)
        {
            $scope.SelectedCards[playerId].indexOf(cardId);

            if (index < 0)
            {
                $scope.SelectedCards[playerId].push(c);
            }
            else
            {
                $scope.SelectedCards[playerId].splice(index, 1);
            }
        }

        $scope.Action = function (playerId) {
            var playerEvent =
                    {
                        Action: $scope.CurrentAction[playerId].Value.Value,
                        Cards: $scope.SelectedCards[playerId]
                    };
            $scope.SelectedCards[playerId] = [];
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