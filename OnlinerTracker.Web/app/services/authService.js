'use strict';
app.factory('authService', ['$http', '$q', 'localStorageService', 'ngSettings', '$location', function ($http, $q, localStorageService, ngSettings, $location) {

    var serviceBase = ngSettings.apiServiceBaseUri;
    var authServiceFactory = {};

    var _authentication = {
        isAuth: false,
        userName: ""
    };

    var _externalAuthData = {
        provider: "",
        userName: "",
        externalAccessToken: "",
        userId:""
    };

    var _fillAuthData = function () {

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.userName = authData.userName;
        }

    };

    var _logOut = function () {
        localStorageService.remove('authorizationData');
        _authentication.isAuth = false;
        _authentication.userName = "";
        return $location.path('/signin');

    };

    var _obtainAccessToken = function (externalData) {
        var deferred = $q.defer();
        $http.get(serviceBase + 'api/account/ObtainLocalAccessToken', {
            params: {
                provider: externalData.provider,
                externalAccessToken: externalData.externalAccessToken,
                userId: externalData.userId,
            }
        }).success(function (response) {
            localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName });
            _authentication.isAuth = true;
            _authentication.userName = response.userName;
            deferred.resolve(response);
        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });
        return deferred.promise;
    };

    var _registerExternal = function (registerExternalData) {
        var deferred = $q.defer();
        $http.post(serviceBase + 'api/account/RegisterExternal', registerExternalData).success(function (response) {
            localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName });
            _authentication.isAuth = true;
            _authentication.userName = response.userName;
            deferred.resolve(response);
        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });
        return deferred.promise;
    };

    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.obtainAccessToken = _obtainAccessToken;
    authServiceFactory.externalAuthData = _externalAuthData;
    authServiceFactory.registerExternal = _registerExternal;

    return authServiceFactory;
}]);