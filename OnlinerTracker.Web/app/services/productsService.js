'use strict';
app.factory('productsService', ['$http', '$q', '$location', 'ngSettings', function ($http, $q, $location, ngSettings) {
    var serviceBase = ngSettings.apiServiceBaseUri;
    var productsServiceFactory = {};

    var _getProducts = function () {

        return $http.get(serviceBase + 'api/product/getAll').then(function (results) {
            return results;
        });
    };

    productsServiceFactory.getProducts = _getProducts;
    return productsServiceFactory;
}]);