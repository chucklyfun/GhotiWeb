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

        $userRest.Get = function (userId) {
            return $http({ method: 'GET', url: 'api/User/Get/' + userId });
        };

        $userRest.GetAll = function () {
            return $http({ method: 'GET', url: 'api/User/Get' });
        };

        $userRest.Delete = function (userId) {
            return $http({ method: 'GET', url: 'api/User/Delete/' + userId  });
        };

        $userRest.Save = function (User) {
            return $http({ method: 'Put', url: 'api/User/Put', data: User });
        };

        return $userRest;
    }]);
});