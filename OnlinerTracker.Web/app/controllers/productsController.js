'use strict';
app.controller('productsController', ['$scope', 'productsService', '$location', function ($scope, productsService, $location) {

    productsService.getProducts().then(function (results) {
        $scope.products = results.data;
    }, function (error) {
        alert(error.data.message);
    });
}]);