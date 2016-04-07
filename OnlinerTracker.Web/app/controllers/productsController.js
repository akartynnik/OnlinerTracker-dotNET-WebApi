'use strict';
app.controller('productsController', ['$scope', 'productsService', '$http', 'ngSettings', '$location', function ($scope, productsService, $http, ngSettings, $location) {

    $scope.products = [];
    $scope.productsComparedCount = 0;

    productsService.getAll().then(function (results) {
        $scope.products = results.data;
        angular.forEach(results.data, function (product, key) {
            if (product.compared) {
                $scope.productsComparedCount = $scope.productsComparedCount + 1;
            }
        });
    }, function (error) {
        alert(error.data.message);
    });

    $scope.changeTrackingStatus = function (product) {
        $http({
            url: ngSettings.apiServiceBaseUri + 'api/product/ChangeTrackingStatus',
            method: 'post',
            params: {
                id: product.id,
                tracking: product.tracking
            }
        });
    }

    $scope.changeComparedStatus = function (product) {
        $http({
            url: ngSettings.apiServiceBaseUri + 'api/product/ChangeComparedStatus',
            method: 'post',
            params: {
                id: product.id,
                compared: product.compared
            }
        });
        if (product.compared) {
            $scope.productsComparedCount = $scope.productsComparedCount + 1;
        } else {
            $scope.productsComparedCount = $scope.productsComparedCount - 1;
        }
    }

    $scope.remove = function (product) {
        $http({
            url: ngSettings.apiServiceBaseUri + 'api/product/remove',
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

    $scope.compare = function() {
        return $location.path('/charts');
    }
}]);