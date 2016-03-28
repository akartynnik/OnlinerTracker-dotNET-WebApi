'use strict';
app.controller('productsController', ['$scope', '$http', 'productsService', function ($scope, $http, productsService) {

    $scope.products = [];
    $scope.responser = [];

    productsService.getProducts().then(function (results) {
        $scope.products = results.data;
    }, function (error) {
        alert(error.data.message);
    });

    $scope.fetchdata = function () {
        if ($scope.searchQuery.length < 3) {
            $scope.getedProducts = [];
            return;
        }
        $http({
            url: 'https://catalog.api.onliner.by/search/products',
            method: 'GET',
            params: {
                query: $scope.searchQuery
            }
        }).success(function (response) {
            console.log(response);
            $scope.getedProducts = response.products;
        });
    }
}]);