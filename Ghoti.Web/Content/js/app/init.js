
define(['angular', 'app/app', 'app/config', 'controllers/game', 'controllers/start', 'controllers/header', 'controllers/index'], function(angular, app, config, game, start, header, index) {
    window.bootstrap = function() {
        angular.bootstrap(document, ['mean']);
    };

    window.init = function() {
        window.bootstrap();
    };

    angular.element(document).ready(function() {
        //Fixing facebook bug with redirect
        if (window.location.hash == "#_=_") window.location.hash = "";

        //Then init the app
        window.init();
    });
});