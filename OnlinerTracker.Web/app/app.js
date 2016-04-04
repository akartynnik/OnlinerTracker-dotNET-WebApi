var app = angular.module('AngularAuthApp', ['ngSanitize', 'ngRoute', 'LocalStorageModule', 'angular-loading-bar', 'filters', 'SignalR']);

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

    $routeProvider.otherwise({ redirectTo: "/home" });
});

app.constant('ngAuthSettings', {
    apiServiceBaseUri: 'http://localhost:52304/',
    clientId: 'onlinerTrackerWebUI'
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService', 'signalRService', function (authService, signalRService) {
    authService.fillAuthData();
}]);
