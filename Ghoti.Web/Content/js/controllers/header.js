define(['angular', 'app/config'], function (angular, config) {
    angular.module('mean.system').controller('HeaderController', ['$scope', 'Global', function ($scope, Global) {
        $scope.global = Global;

        $scope.menu = [{
            "title": "Articles",
            "link": "articles"
        }, {
            "title": "Create New Article",
            "link": "articles/create"
        },
        ];

        $scope.test = "This is a test";

        $scope.isCollapsed = false;
    }]);
});