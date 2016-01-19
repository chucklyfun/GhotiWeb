
define(['angular', 'nggrid', 'app/config', 'services/gameRest', 'services/cards', 'services/userRest', 'linqjs'], function (angular, nggrid, appconfig, gamerest, cards, userrest, Enumerable) {
    /**
     * Create a gameview
     */
    angular.module('mean.system').controller('StartController', ['$scope', '$http', '$routeParams', '$location', 'Global', '$gameRest', '$userRest',
        function ($scope, $http, $routeParams, $location, Global, $gameRest, $userRest) {
        
        $scope.global = Global;

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
            $scope.playerGridOptions = { data: 'users', columnDefs: 'playerCols' };
            $scope.gameGridOptions = { data: 'games', columnDefs: 'gameCols' };
            $scope.name = "test name";
            $scope.players = [];
            $scope.games = [];
            $scope.playerCols = [
                { 'cellTemplate': '<button id="btnAddPlayer" type="button" class="btn btn-primary" ng-click="addPlayer(row.entity)" >Add Player</button>' },
                { 'field': 'UserName', displayName: 'Username' },
                { 'field': 'FullName', displayName: 'Name' },
                { 'field': 'Email', displayName: 'E-mail' }];

            $scope.gameCols = [
                { 'cellTemplate': '<button id="btnLoadGame" type="button" class="btn btn-primary" ng-click="loadGame(row.entity)" >Load</button>' },
                { 'field': 'Id', displayName: 'Id' },
                { 'cellTemplate': '<ul><li ng-repeat="p in row.Players">{{p.FullName}} ({{p.Email}})</li></ul>' },
                { 'cellTemplate': '<button id="btnDeleteGame" type="button" class="btn btn-primary" ng-click="deleteGame(row.entity)" >Delete</button>' }];

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
                    if (entity.Id == $scope.gv.Id) {
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
                $location.path('/game').search({ id: $scope.gv.Id });
            }
        };
    }]);
});