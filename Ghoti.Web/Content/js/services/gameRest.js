//Games service used for articles REST endpoint
angular.module('routerApp').factory("$gameRest", ['$resource', '$http', '$q', function ($resource, $http, $q) {
    //    return $resource('api/Game/:gameid',
    //        {
    //            gameid: '@id'
    //        })

    //return 
    var $gameRest = function(element, trigger, options) {
    };

    $gameRest.create = function() {
        return $http({ method: 'GET', url: 'api/Game/Get/0' });
    };
        
    $gameRest.get = function (gameId) {
        return $http({ method: 'GET', url: 'api/Game/Get/' + gameId });
    };
        
    $gameRest.getAll = function () {
        return $http({ method: 'GET', url: 'api/Game/Get' });
    };
        
    $gameRest.delete = function (gameId) {
        return $http({ method: 'GET', url: 'api/Game/Delete/' + gameId});
    };
        
    $gameRest.save = function (Game) {
        return $http({ method: 'Put', url: 'api/Game/Put', data: Game });
    };

    $gameRest.addPlayer = function (gameId, userId) {
        return $http({ method: 'GET', url: 'api/Game/' + gameId + '/AddPlayer/' + userId });
    };

    $gameRest.getPlayers = function (gameId) {
        return $http({ method: 'GET', url: 'api/Game/' + gameId + '/Players' });
    };

    return $gameRest;
}]);