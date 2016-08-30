/**
* Create a gameview
*/
angular.module('routerApp').controller('GameController', ['$rootScope', '$scope', '$http', '$stateParams', '$gameRest', '$userRest', 'connection',
    function ($rootScope, $scope, $http, $stateParams, $gameRest, $userRest, connection) {
        $scope.Wait = 
            {
                id : 'Wait',
                value : 1
            };
        $scope.DrawAndDiscard =
            {
                id : 'DrawAndDiscard',
                value : 2
            };
        $scope.PlayAction =
            {
                id : 'PlayAction',
                value : 3
            };
        $scope.PlayEquipment = 
            {
                id : 'PlayEquipment',
                value : 4
            };
        $scope.StartGame =
            {
                id: 'StartGame',
                value: 5
            };

        $scope.createPlayerView = function(player, hubId)
        {
            return result =
            {
                Name: player.User.FullName,
                PlayerId: player.Id,
                SelectedCards: [],
                CurrentAction: $scope.Wait.value,
                HubId : hubId
            };
        }

        $scope.hubs = {};
        $scope.PlayersMap = {};
        $scope.Players = [];
        $scope.ServerViews = {};

        $scope.gameCols =
            [
                    {
                        field: 'Name',
                        displayName: 'Name',
                        enableCellEdit: 'false'
                    },
                    {
                        name: 'Current Action',
                        field: 'CurrentAction',
                        editDropdownOptionsArray: $scope.GameActions
                    },
                    {
                        name: 'Start Game',
                        cellTemplate: '<div><button id="btnStartGame" type="button" class="btn btn-primary" ng-click="grid.appScope.Action(row.entity.PlayerId, grid.appScope.StartGame)" >Start Game</button></div>'
                    },
                    {
                        name: 'Play Action',
                        cellTemplate: '<div><button id="btnPlayAction" type="button" class="btn btn-primary" ng-click="grid.appScope.Action(row.entity.PlayerId, grid.appScope.PlayAction)" >Play Action</button></div>'
                    },
                    {
                        name: 'Play Equipment',
                        cellTemplate: '<div><button id="btnPlayEquipment" type="button" class="btn btn-primary" ng-click="grid.appScope.Action(row.entity.PlayerId, grid.appScope.PlayEquipment)" >Play Equipment</button></div>'
                    },
                    {
                        name: 'Draw and Discard',
                        cellTemplate: '<div><button id="btnDrawAndDiscard" type="button" class="btn btn-primary" ng-click="grid.appScope.Action(row.entity.PlayerId, grid.appScope.DrawAndDiscard)" >Draw And Discard</button></div>'
                    },
                    {
                        name: 'Wait',
                        cellTemplate: '<div><button id="btnWait" type="button" class="btn btn-primary" ng-click="grid.appScope.Action(row.entity.PlayerId, grid.appScope.Wait)" >Wait</button></div>'
                    },
                    {
                        name: 'Hand',
                        cellTemplate: '<li data-ng-repeat="c in row.entity.Hand"><span>{{c.Name}}</span><span>{{c.CardNumber}}</span><img ng-src="{{c.ImageUrl}}" ng-click="grid.appScope.toggleCard(row.entity.PlayerId, c.Id)"/></li>'
                    }
            ];
            

        $rootScope.getNewConnectionId = function()
        {
            return $http({ method: 'GET', url: 'api/Utilities/CreateObjectId' });
        }

        $scope.AddHub = function (gameId, player, connectionId) {
            var h = connection.initialize(gameId, player.Id, connectionId);

            h.connection.start().done(function () {
                console.log('Now connected, connection ID=' + h.connection.id);

                $scope.hubs[player.Id] = h;
                $scope.PlayersMap[player.Id] = $scope.createPlayerView(player, h.connection.id);
                $scope.$apply();
            }).fail(function () {
                console.log('Could not Connect!');
            });
        };

        $scope.init = function ()
        {
            $scope.gameId = $stateParams.Id;

            $gameRest.getPlayers($scope.gameId)
                .success(function (data) {
                    $scope.Players = data;

                    for (var p in $scope.Players)
                    {
                        var player = data[p];
                        $scope.PlayersMap[player.Id] = player;

                        $rootScope.getNewConnectionId()
                            .success(function (newId) {
                                $scope.AddHub($scope.gameId, player, newId);
                            })
                        
                    }
                });
        };

        $scope.toggleCard = function(playerId, cardId)
        {
            var playerView = $scope.PlayersMap[playerId];
            if (playerView != undefined)
            {
                var index = playerView.SelectedCards.indexOf(cardId);

                if (index < 0) {
                    $scope.SelectedCards[playerId].push(c);
                }
                else {
                    $scope.SelectedCards[playerId].splice(index, 1);
                }
            }
            else
            {
                $scope.error = 'toggleCard: Unable to find player view.'
            }        
        }

        $scope.ping = function () {
            $http({ method: 'GET', url: 'api/game/Ping' }).success(function (data)
            {
                console.log(data);
            });
        }

        $scope.Action = function (playerId, action) {
            var playerView = $scope.PlayersMap[playerId];

            var selectedCards = [];

            if (playerView != undefined)
            {
                selectedCards = playerView.SelectedCards;

                var playerEvent =
                    {
                        Action: action.value,
                        Cards: selectedCards
                    };


                var hub = $scope.hubs[playerView.PlayerId];

                if (hub != undefined)
                {
                    hub.send(playerEvent);

                    playerView.SelectedCards = [];
                }
                else
                {
                    $scope.error = 'Unable to find hub: ' + playerView.HubId;
                }
            }
        
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