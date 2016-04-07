'use strict';
app.factory('productsService', ['$http', '$q', '$location', 'ngSettings', function ($http, $q, $location, ngSettings) {
    var serviceBase = ngSettings.apiServiceBaseUri;
    var productsServiceFactory = {};

    var _getAll = function () {

        return $http.get(serviceBase + 'api/product/getAll').then(function (results) {
            return results;
        });
    };

    var _getAllCompared = function () {

        return $http.get(serviceBase + 'api/product/getAllCompared').then(function (results) {
            return results;
        });
    };

    productsServiceFactory.getAll = _getAll;
    productsServiceFactory.getAllCompared = _getAllCompared;
    return productsServiceFactory;
}]);