
define(['angular', 'nggrid', 'app/config', 'services/gameRest', 'services/cards', 'services/userRest', 'linqjs'], function (angular, nggrid, appconfig, gamerest, cards, userrest, Enumerable) {
    /**
     * Create a gameview
     */
    angular.module('mean.system').controller('StartController', ['$scope', '$http', '$routeParams', '$location', 'Global', '$gameRest', '$userRest',
        function ($scope, $http, $routeParams, $location, Global, $gameRest, $userRest) {
        
        $scope.global = Global;

        $scope.init = function () {
            $userRest.GetAll().success(function (data, status, headers, config) {
                $scope.Users = data;
            }, function (data, status, headers, config) {
                $scope.error = data;
            });

            $gameRest.create().success(function (data, status, headers, config) {
                $scope.gv = data;
            }).error(function (data, status, headers, config) {
                $scope.error = data;
            });

            $scope.gridOptions = { data: 'Users', columnDefs: 'cols' };
            $scope.name = "test name";
            $scope.players = [];
            $scope.cols = [
                { 'cellTemplate': '<button id="btnAddPlayer" type="button" class="btn btn-primary" ng-click="addPlayer(row)" >Add Player</button>' },
                { 'field': 'UserName', displayName: 'Username' },
                { 'field': 'FullName', displayName: 'Name' },
                { 'field': 'Email', displayName: 'E-mail' }];

        };
    
        $scope.addPlayer = function (row) {
            $gameRest.addPlayer($scope.gv.Id, row.entity.Id).success(function (data, status, headers, config) {
                if (data == "true") {
                    $scope.gv.Players.push(row.entity);
                } else {
                    $scope.error = "Unable to add User " & row.entity.UserName;
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