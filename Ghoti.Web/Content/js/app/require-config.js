require.config({
    baseUrl: "/content/js/",
    paths: {
        jquery: [
           'http://ajax.googleapis.com/ajax/libs/jquery/1.8.3/jquery.min',
           'lib/jquery/jquery-1.8.3.min'
        ],
        modernizer:['app/modernizr'],
        angular: ['lib/angular/angular'],
        angularbootstrap: ['lib/angular-bootstrap/ui-bootstrap'],
        angularbootstraptpls: ['lib/angular-bootstrap/ui-bootstrap-tpls'],
        angularcookies: ['lib/angular-cookies/angular-cookies'],
        angularresource: ['lib/angular-resource/angular-resource'],
        angularuiutils: ['lib/angular-ui-utils/modules/utils'],
        angularroutejs: ['lib/angular-ui-utils/modules/route/route'],
        nggrid: ['lib/ng-grid/ng-grid-2.0.7.debug'],
        linqjs: ['lib/linq'],
        signalr: ['lib/angular-signalr-hub/jquery.signalR-2.1.2'],
        angularsignalr: ['lib/angular-signalr-hub/signalr-hub']
    },
    shim: {
        jquery: { exports: '$' },
        linqjs: { exports: 'Enumerable' },
        angular: { exports: 'angular', deps: ['jquery'] },
        angularbootstraptpls: { deps: ['angular'] },
        angularbootstrap: { deps: ['angularbootstraptpls'] },
        angularcookies: { deps: ['angular'] },
        angularresource: { deps: ['angular'] },
        angularuiutils: { deps: ['angular'] },
        angularroutejs: { deps: ['angular'] },
        nggrid: { deps: ['jquery', 'angular'] },
        signalr: {deps: ['jquery']},
        angularsignalr: {deps: ['signalr', 'angular']},
    }
});

