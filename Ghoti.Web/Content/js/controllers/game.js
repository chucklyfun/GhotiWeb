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

        $scope.createPlayerView = function(player)
        {
            return result =
            {
                Id: player.Id,
                SelectedCards: [],
                CurrentAction: $scope.Wait.value
            };
        }

        $scope.gdTestPlayersOptions =
            {
                columnDefs: [
                    { 'field': 'Name', displayName: 'Name', enableCellEdit: 'false' },
                    { 'field': 'row.entity.CurrentAction', editDropdownOptionsArray : $scope.GameActions   },
                    { name: 'Start Game',
                    cellTemplate: '<div><button id="btnStartGame" type="button" class="btn btn-primary" ng-click="grid.appScope.Action(row.entity.Id, StartGame)" >Start Game</button></div>'
                    },
                    { name: 'Play Action',
                    cellTemplate: '<div><button id="btnPlayAction" type="button" class="btn btn-primary" ng-click="grid.appScope.Action(row.entity.Id, PlayAction)" >Play Action</button></div>'
                    },
                    { name: 'Play Equipment',
                    cellTemplate: '<div><button id="btnPlayEquipment" type="button" class="btn btn-primary" ng-click="grid.appScope.Action(row.entity.Id, PlayEquipment)" >Play Equipment</button></div>'
                    },
                    { name: 'Draw and Discard',
                    cellTemplate: '<div><button id="btnDrawAndDiscard" type="button" class="btn btn-primary" ng-click="grid.appScope.Action(row.entity.Id, DrawAndDiscard)" >Draw And Discard</button></div>'
                    },
                    { name: 'Wait',
                    cellTemplate: '<div><button id="btnWait" type="button" class="btn btn-primary" ng-click="grid.appScope.Action(row.entity.Id, Wait)" >Wait</button></div>'
                    },
                    { name: 'Hand',
                    cellTemplate: '<li data-ng-repeat="c in row.entity.Hand"><span>{{c.Name}}</span><span>{{c.CardNumber}}</span><img ng-src="{{c.ImageUrl}}" ng-click="grid.appScope.toggleCard(row.entity.Id, c.Id)"/></li>'
                    }
                ],
                enableColumnResize: 'true'
            };
            

        $scope.players = [];

        $scope.init = function ()
        {
            $gameRest.get($stateParams.Id).success(function (data)
            {
                $scope.gv = data;
                $scope.hubs = [];

                for (var p in $scope.gv.Players) {
                    var id = $scope.gv.Players[p].User.Id;

                    var h = connection.initialize($scope.gv.Id, id);
                    h.connection.start().done(function () {
                            console.log('Now connected, connection ID=' + $.connection.hub.id);

                            h.connected();

                            $scope.hubs.push(h);
                            $scope.players.push($scope.createPlayerView($scope.gv.Players[p]));
                        })
                        .fail(function ()
                        {
                            console.log('Could not Connect!');
                        });


                    
                    
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

    $scope.Action = function (playerId, action) {
        var playerEvent =
                {
                    Action: action,
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