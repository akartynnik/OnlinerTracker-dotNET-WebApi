'use strict';
app.controller('homeController', ['$scope', '$http', 'productsService', 'ngAuthSettings', '$timeout', function ($scope, $http, productsService, ngAuthSettings, $timeout) {

    $scope.products = [];
    $scope.responser = [];
    $scope.textAlert = "";
    $scope.showAlert = false;
    $scope.alertClassName = "alert-success";
    $scope.getedProducts = [];
    $scope.searchQuery = "";

    var page = 1;
    var lenghtForStartSearch = 2;
    var isLoding = true;
    var promise;

    $scope.$watch('searchQuery', function(typedString) {
        console.log(typedString);
        if ((!typedString || typedString.length < lenghtForStartSearch) && isLoding)
            return 0;
        if (typedString === $scope.searchQuery) {
            page = 1;
            isLoding = false;
            $http({
                url: ngAuthSettings.apiServiceBaseUri + 'api/product/GetFromExternalServer',
                method: 'GET',
                params: {
                    searchQuery: $scope.searchQuery,
                    page: 1
                }
            }).success(function(response) {
                $scope.getedProducts = response;
                isLoding = true;
            });
        }
    });
    
    $scope.followProduct = function (product) {
        //stop alerts timer
        $timeout.cancel(promise);
        //send request
        $http({
            url: ngAuthSettings.apiServiceBaseUri + 'api/product/follow',
            method: 'post',
            data: {
                OnlinerId: product.onlinerId,
                Name: product.name,
                ImageUrl: product.imageUrl.replace(/"/g, "&quot;"),
                Description: product.description.replace(/"/g, "&quot;"),
                Cost: product.currentCost
            }
        }).success(function (response) {
            console.log(response);
            if (response == "OK") {
                $scope.showAlert = true;
                $scope.alertClassName = "alert-success";
                $scope.textAlert = '<b>' + product.name + "</b> now is tracked!";
            }
            if (response == "Duplicate") {
                console.log("duplicate");
                $scope.showAlert = true;
                $scope.alertClassName = "alert-warning";
                $scope.textAlert = "This product is already being tracked!";
            }
            promise = $timeout(function () { $scope.showAlert = false; }, 3000);
            product.tracking = true;
        }).error(function (msg) {
            $scope.showAlert = true;
            $scope.alertClassName = "alert-danger";
            $scope.textAlert = 'ERROR!';
        });
    }

    $scope.onScrollEnd = function () {
        if ($scope.searchQuery.length >= lenghtForStartSearch && isLoding) {
            page++;
            isLoding = false;
            $http({
                url: ngAuthSettings.apiServiceBaseUri + 'api/product/GetFromExternalServer',
                method: 'GET',
                params: {
                searchQuery: $scope.searchQuery,
                page: page
            }
            }).success(function (response) {
                response.forEach(function (entry) {
                    $scope.getedProducts.push(entry);
                });
                isLoding = true;
            }); 
        }
    }

    $scope.closeAlert = function () {
        $scope.showAlert = false;
    }
}]);