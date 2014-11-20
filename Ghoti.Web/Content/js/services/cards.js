define(['angular'], function(angular) {
    //Articles service used for articles REST endpoint
    angular.module('mean.system').factory("Cards", ['$resource', function($resource) {
        return $resource('api/Card/Get/:cardid', {
                cardid: '@_id'
            }, {
                update: {
                    method: 'PUT'
                }
            });
    }]);
});