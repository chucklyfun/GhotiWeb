/**
* Create a gameview
*/
angular.module('routerApp').controller('GameController', ['$rootScope', '$scope', '$http', '$stateParams', '$gameRest', '$userRest', '$utilitiesRest', 'connection',
    function ($rootScope, $scope, $http, $stateParams, $gameRest, $userRest, $utilitiesRest, connection) {
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
        $scope.Refresh =
            {
                id: 'Refresh',
                value: 6
            };

        $scope.createPlayerView = function(player)
        {
            return result =
            {
                Name: player.User.FullName,
                PlayerId: player.Id,
                SelectedCards: [],
                View: player
                
            };
        }

        $scope.Hubs = {};
        $rootScope.PlayersMap = {};
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
                        name: 'Refresh',
                        cellTemplate: '<div><button id="btnRefresh" type="button" class="btn btn-primary" ng-click="grid.appScope.Action(row.entity.PlayerId, grid.appScope.Refresh)" >Refresh</button></div>'
                    },
                    {
                        name: 'Hand',
                        cellTemplate: '<li data-ng-repeat="c in row.entity.View.PlayerCardIds"><img ng-src="{{grid.appScope.GetCardUrl(c)}}" ng-click="grid.appScope.toggleCard(row.entity.PlayerId, c)"/></li>'
                    }
            ];
            

        $rootScope.getNewConnectionId = function()
        {
            return $http({ method: 'GET', url: 'api/Utilities/CreateObjectId' });
        }

        $scope.AddHub = function (gameId, playerView) {
            
            $rootScope.getNewConnectionId()
                .success(function (newId) 
                {
                    var h = connection.initialize(gameId, playerView.PlayerId, newId);

                    h.connection.start().done(function () {
                        console.log('Now connected, connection ID=' + h.connection.id);

                        $scope.Hubs[playerView.PlayerId] = h;
                    }).fail(function () {
                        console.log('Could not Connect!');
                    });
                })
        };

        $scope.init = function ()
        {
            $scope.gameId = $stateParams.Id;

            $utilitiesRest.GetCards($scope.gameId)
                .success(function (data)
                {
                    $scope.CardImages = data;

                    $gameRest.getPlayers($scope.gameId)
                        .success(function (data)
                        {
                            for (var p in data) {
                                var player = data[p];
                                var playerView = $scope.createPlayerView(player);
                                $scope.PlayersMap[player.Id] = playerView;
                                $scope.Players.push(playerView);

                                $scope.AddHub($scope.gameId, playerView);
                            }

                        });
                });

            
        };

        $scope.GetCardUrl = function(id)
        {
            return $scope.CardImages[id];
        }

        $scope.toggleCard = function(playerId, cardId)
        {
            var playerView = $scope.PlayersMap[playerId];
            if (playerView != undefined)
            {
                var index = playerView.SelectedCards.indexOf(cardId);

                if (index < 0) {
                    playerView.SelectedCards.push(cardId);
                }
                else {
                    playerView.SelectedCards.splice(index, 1);
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
            var hub = $scope.Hubs[playerId];

            var selectedCards = [];

            if (playerView != undefined && hub != undefined)
            {
                selectedCards = playerView.SelectedCards;

                var playerEvent =
                    {
                        Action: action.value,
                        Cards: selectedCards
                    };

                hub.send(playerEvent);

                playerView.SelectedCards = [];
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