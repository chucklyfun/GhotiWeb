angular.module('routerApp').controller('IndexController', ['$scope', 'Global', function ($scope) {
    $scope.global = Global;
}]);