'use strict';
app.controller('homeController', ['$scope', '$http', 'productsService', 'ngAuthSettings', 'authService', '$location', function ($scope, $http, productsService, ngAuthSettings, authService, $location) {

    $scope.products = [];
    $scope.responser = [];
    $scope.textAlert = "";
    $scope.showAlert = false;
    $scope.alertClassName = "alert-success";

    /*
        if (!authService.authentication.isAuth) {
            $location.path('/signin');
        }
    */

    $scope.fetchdata = function () {
        if ($scope.searchQuery.length < 2) {
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
            //console.log(response);
            $scope.getedProducts = response.products;
        });
    }

    $scope.followProduct = function (product) {
        $http({
            url: ngAuthSettings.apiServiceBaseUri + 'api/products/follow',
            method: 'post',
            data: '{"OnlinerId":' + product.id + ',"Name":"' + product.full_name + '","ImageUrl":"' + product.images.header + '", "Description":"' + product.description + '"}'
        }).success(function (response) {
            console.log(response);
            if (response == "\"OK\"") {
                $scope.showAlert = true;
                $scope.alertClassName = "alert-success";
                $scope.textAlert = '<b>' + product.name + "</b> now is tracked!";
                $scope.switchBool = function (value) {
                    $scope[value] = !$scope[value];
                };
            }
            if (response == "\"Duplicate\"") {
                $scope.showAlert = true;
                $scope.alertClassName = "alert-warning";
                $scope.textAlert = "This product is already being tracked!";
                $scope.switchBool = function (value) {
                    $scope[value] = !$scope[value];
                };
            }
        }).error(function (msg) {
            $scope.showAlert = true;
            $scope.alertClassName = "alert-danger";
            $scope.textAlert = 'ERROR!';
            $scope.switchBool = function (value) {
                $scope[value] = !$scope[value];
            };
        });
    }
}]);