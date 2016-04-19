
//Articles service used for articles REST endpoint
angular.module('routerApp').factory("UserResource", ['$resource', function ($resource) {
    return $resource('api/User/:userid', {
            userid: '@_id'
        });
}]);