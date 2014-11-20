define(['jquery', 'angular', 'angularbootstrap', 'angularroutejs', 'angularcookies', 'angularresource', 'angularsignalr'], function (jquery, angular, angularbootstrap, angularroutejs, angularcookies, angularresource, angularsignalr) {

    window.app = angular.module('mean', ['ngCookies', 'ngResource', 'ui.bootstrap', 'ui.route', 'mean.system', 'ngGrid', 'SignalR']);

    angular.module('mean.system', []);
    //angular.module('mean.games', []);

    require(["app/init"], function() {
    });

    return window.app;
});

