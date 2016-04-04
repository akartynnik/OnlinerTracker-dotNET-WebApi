'use strict';
app.controller('productsController', ['$scope', 'productsService', '$http', 'ngAuthSettings', function ($scope, productsService, $http, ngAuthSettings) {

    $scope.products = [];

    productsService.getProducts().then(function (results) {
        $scope.products = results.data;
    }, function (error) {
        alert(error.data.message);
    });

    $scope.changeTrackingStatus = function (product) {
        $http({
            url: ngAuthSettings.apiServiceBaseUri + 'api/product/ChangeTrackingStatus',
            method: 'post',
            data: product
        });
    }

    $scope.remove = function (product) {
        $http({
            url: ngAuthSettings.apiServiceBaseUri + 'api/product/remove',
            method: 'post',
            data: {
                id: product.id,
                name: product.name
            }
        }).success(function (response) {
            if (response == "OK") {
                // update products in scope
                var deletedProductId = $scope.products.indexOf(product);
                $scope.products.splice(deletedProductId, 1);
            }
        });
    }
}]);