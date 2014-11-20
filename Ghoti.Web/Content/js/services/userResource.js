define(['angular', 'app/config'], function(angular, config) {
    //Articles service used for articles REST endpoint
    angular.module('mean.system').factory("UserResource", ['$resource', function($resource) {
        return $resource('api/User/:userid', {
                userid: '@_id'
            });
    }]);
});