'use strict';
app.factory('authInterceptorService', ['$q', '$injector', '$location', 'localStorageService', 'signalRService',
    function ($q, $injector, $location, localStorageService, signalRService) {

    var authInterceptorServiceFactory = {};

    var _request = function (config) {

        //currencyService.getCurrency(); // check curracy in each request

        config.headers = config.headers || {};

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            config.headers.Authorization = 'Bearer ' + authData.token;
            config.headers.SignalRConnectionId = signalRService.connectionId();
        }
        if (!authData && $location.path() !== '/signin' && $location.path() !== '/associate') {
            $location.path('/signin');
        }
        return config;
    }

    var _responseError = function (rejection) {
        var deferred = $q.defer();
        var authService = $injector.get('authService');
        if (rejection.status === 401) {
            authService.logOut();
            $location.path('/signin');
            return $q.reject(rejection);
        } else {
            deferred.reject(rejection);
        }
        return deferred.promise;
    }

    authInterceptorServiceFactory.request = _request;
    authInterceptorServiceFactory.responseError = _responseError;

    return authInterceptorServiceFactory;
}]);