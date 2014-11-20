define(['angular', 'app/config', 'services/global'], function (angular, config, global) {
    angular.module('mean.system').controller('IndexController', ['$scope', 'Global', function ($scope) {
        $scope.global = Global;
    }]);
});