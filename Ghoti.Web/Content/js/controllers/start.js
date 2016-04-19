/**
* Create a gameview
*/
angular.module('routerApp').controller('StartController', ['$scope', '$http', '$state', '$stateParams', '$location', '$gameRest', '$userRest',
    function ($scope, $http, $state, $stateParams, $location, $gameRest, $userRest) {
    $scope.loadGames = function()
    {
        $gameRest.getAll().success(function (data, status, headers, config) {
            $scope.games = data;
        }, function (data, status, headers, config) {
            $scope.error = data;
        });
    };

    $scope.loadUsers = function ()
    {
        $userRest.getAll().success(function (data, status, headers, config) {
            $scope.users = data;
        }, function (data, status, headers, config) {
            $scope.error = data;
        });
    }

    $scope.init = function ()
    {
        $scope.name = "test name";
        $scope.players = [];
        $scope.games = [];
        $scope.playerCols = [
            {   name: 'Add Player',
            cellTemplate: '<button id="btnAddPlayer" type="button" class="btn btn-primary ng-click="grid.appScope.addPlayer(row.entity)" >Add Player</button>'
            },
            {   name: 'Username',
                field: 'UserName',
                displayName: 'Username' },
            {   name: 'Full Name',
                field: 'FullName',
                displayName: 'Name' },
            {   name: 'E-Mail',
                field: 'Email',
                displayName: 'E-mail' }];

        $scope.gameCols = [
            {   name: 'Load Game',
            cellTemplate: '<button id="btnLoadGame" type="button" class="btn btn-primary" ng-click="grid.appScope.loadGame(row.entity)" >Load</button>'
            },
            {   field: 'Id',
                displayName: 'Id' },
            {   name: 'Players',
                cellTemplate: '<ul><li ng-repeat="p in row.Players">{{p.FullName}} ({{p.Email}})</li></ul>' },
            {   name: 'Delete',
            cellTemplate: '<button id="btnDeleteGame" type="button" class="btn btn-primary" ng-click="grid.appScope.deleteGame(row.entity)" >Delete</button>'
            }];

        $scope.loadUsers();

        $scope.loadGames();
    };
    

    $scope.createGame = function () {
        $gameRest.create().success(function (data, status, headers, config) {
            $scope.games.push(data);
            $scope.gv = data;
        }).error(function (data, status, headers, config) {
            $scope.error = data;
        });
    };

    $scope.addPlayer = function (entity) {
        $gameRest.addPlayer($scope.gv.Id, entity.Id).success(function (data, status, headers, config) {
            if (data == "true") {
                $scope.gv.Players.push(entity);
            } else {
                $scope.error = "Unable to add User " & entity.UserName;
            }
        }).error(function (data, status, headers, config) {
            $scope.error = data;
        });
    };

    $scope.loadGame = function (entity) {
        $gameRest.get(entity.Id).success(function (data, status, headers, config) {
            $scope.gv = data;
        }).error(function (data, status, headers, config) {
            $scope.error = data;
        });
    };

    $scope.deleteGame = function (entity) {
        $gameRest.delete(entity.Id).success(function (data, status, headers, config) {
            if (data == "true")
            {
                if ($scope.gv != undefined && entity.Id == $scope.gv.Id) {
                    $scope.gv = nothing;
                }

                $scope.loadGames();
                error = "Successfully deleted game: " & entity.Id;
            }
        }).error(function (data, status, headers, config) {
            $scope.error = data;
        });
    };

    $scope.startGame = function () {
        if ($scope.gv.Players.length >= 2) {
            $state.go('test-game', { id: $scope.gv.Id });
        }
    };
}]);