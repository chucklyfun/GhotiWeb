define(['angular', 'app/app'], function () {

    //Setting up route
    window.app.config(['$routeProvider',
        function ($routeProvider) {
            $routeProvider.
                //when('/game/creategame', {
                //    templateUrl: '/views/game/creategame.html'
            //}).
            //when('/game/showcards', {
            //    templateUrl: '/views/game/showcards.html'
            //}).
            //when('/game/wait', {
            //    templateUrl: '/views/game/wait.html'
            //}).
            when('/game', {
                templateUrl: '/content/public/views/game/showcards.html',
            }).
            when('/', {
                templateUrl: '/content/public/views/game/creategame.html'
            }).

            otherwise({
                redirectTo: '/'
            });
        }
    ]);

    //Setting HTML5 Location Mode
    window.app.config(['$locationProvider',
        function ($locationProvider) {
            $locationProvider.hashPrefix("!");
        }
    ]);
});