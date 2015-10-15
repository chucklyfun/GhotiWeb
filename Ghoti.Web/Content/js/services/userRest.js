define(['angular', 'app/config'], function(angular, config) {

    ////Articles service used for articles REST endpoint
    //angular.module('mean.system').factory("Users", ['$resource', function($resource) {
    //    return $resource('api/User/:userid', {
    //            userid: '@_id'
    //        });
    //}]);
    angular.module('mean.system').factory("$userRest", ['$resource', '$http', '$q', function ($resource, $http, $q) {

        var $userRest = function (element, trigger, options) {
        };

        $userRest.create = function () {
            return $http({ method: 'GET', url: 'api/User/Get/0' });
        };

        $userRest.get = function (userId) {
            return $http({ method: 'GET', url: 'api/User/Get/' + userId });
        };

        $userRest.getAll = function () {
            return $http({ method: 'GET', url: 'api/User/Get' });
        };

        $userRest.delete = function (userId) {
            return $http({ method: 'GET', url: 'api/User/Delete/' + userId  });
        };

        $userRest.save = function (User) {
            return $http({ method: 'Put', url: 'api/User/Put', data: User });
        };

        return $userRest;
    }]);
});