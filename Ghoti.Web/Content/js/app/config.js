//Setting up route

angular.module('routerApp').config(['$stateProvider', '$urlRouterProvider',
    function($stateProvider, $urlRouterProvider) {
        //
        // For any unmatched url, redirect to /state1
        //$urlRouterProvider.otherwise("/");

        //$stateProvider

        //        // HOME STATES AND NESTED VIEWS ========================================
        //        .state('home', {
        //            url: '/home',
        //            controller : 'GameController',
        //            templateUrl: 'public/views/game/creategame.html'
        //        })

        //        // nested list with custom controller
        //        .state('home.list', {
        //            url: '/list',
        //            templateUrl: 'partial-home-list.html',
        //            controller: function ($scope) {
        //                $scope.dogs = ['Bernese', 'Husky', 'Goldendoodle'];
        //            }
        //        })

        //        // nested list with just some random string data
        //        .state('home.paragraph', {
        //            url: '/paragraph',
        //            template: 'I could sure use a drink right now.'
        //        })

        //        // ABOUT PAGE AND MULTIPLE NAMED VIEWS =================================
        //        .state('about', {
        //            url: '/about',
        //            views: {
        //                '': { templateUrl: 'partial-about.html' },
        //                'columnOne@about': { template: 'Look I am a column!' },
        //                'columnTwo@about': {
        //                    templateUrl: 'table-data.html',
        //                    controller: 'scotchController'
        //                }
        //            }

        //        });

        //$stateProvider
        //    .state('game',
        //    {
        //        url: '/game',
        //        controller: 'GameController',
        //        templateUrl: 'public/views/game/showcards.html',
        //    }).state('default', {
        //        url: '/',
        //        controller: 'StartController',
        //        templateUrl: 'public/views/game/creategame.html'
        //    });
    }
]);