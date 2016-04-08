var app = angular.module('AngularAuthApp', ['ngSanitize', 'ngRoute', 'LocalStorageModule', 'angular-loading-bar', 'filters', 'SignalR', 'ui.bootstrap', 'chart.js']);

app.config(function ($routeProvider) {

    $routeProvider.when("/home", {
        controller: "homeController",
        templateUrl: "/app/views/home.html"
    });

    $routeProvider.when("/signin", {
        controller: "signinController",
        templateUrl: "/app/views/signin.html"
    });

    $routeProvider.when("/products", {
        controller: "productsController",
        templateUrl: "/app/views/products.html"
    });

    $routeProvider.when("/associate", {
        controller: "associateController",
        templateUrl: "/app/views/associate.html"
    });

    $routeProvider.when("/charts", {
        controller: "chartsController",
        templateUrl: "/app/views/charts.html"
    });

    $routeProvider.otherwise({ redirectTo: "/home" });
});

app.constant('ngSettings', {
    apiServiceBaseUri: 'http://localhost:52304/',
    signalRServerPath: 'http://localhost:52304//signalr',
    clientId: 'onlinerTrackerWebUI',
    dialogProvider: 'signalr' // Can be "zeromq" or "signalr" (default)
});

app.constant('ngEnums', {
    currencyType: Object.freeze({
        "BLR": 0,
        "USD": 1,
        "EUR": 2
    }),
    currencyName: Object.freeze({
        0: "BLR",
        1: "USD",
        2: "EUR"
    })
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService', 'localStorageService', 'currencyService', function (authService, localStorageService, currencyService) {
    authService.fillAuthData();
    //localStorageService.remove('currencies');
}]);
