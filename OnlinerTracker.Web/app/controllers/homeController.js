'use strict';
app.controller('homeController', ['$scope', '$http', 'productsService', 'ngSettings', 'currencyService', 'localStorageService',
    function ($scope, $http, productsService, ngSettings, currencyService, localStorageService) {

    $scope.products = [];
    $scope.alertClassName = "alert-success";
    $scope.getedProducts = [];
    $scope.searchQuery = "";

    $scope.$on('$viewContentLoaded', function() {
        //var curerncies = localStorageService.get('currencies');
        //currencyService.getCurrency(curerncies.current);
    });

    var page = 1;
    var lenghtForStartSearch = 2;
    var isLoding = true;

    $scope.$watch('searchQuery', function (typedString) {
        if ((!typedString || typedString.length < lenghtForStartSearch) && isLoding)
            return 0;
        if (typedString === $scope.searchQuery) {
            page = 1;
            isLoding = false;
            $http({
                url: ngSettings.apiServiceBaseUri + 'api/product/GetFromExternalServer',
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
        $http({
            url: ngSettings.apiServiceBaseUri + 'api/product/follow',
            method: 'post',
            data: {
                OnlinerId: product.onlinerId,
                Name: product.name,
                ImageUrl: product.imageUrl.replace(/"/g, "&quot;"),
                Description: product.description.replace(/"/g, "&quot;"),
                Cost: product.currentCost
            }
        }).success(function () {
            product.tracking = true;
        });
    }

    $scope.onScrollEnd = function () {
        if ($scope.searchQuery.length >= lenghtForStartSearch && isLoding) {
            page++;
            isLoding = false;
            $http({
                url: ngSettings.apiServiceBaseUri + 'api/product/GetFromExternalServer',
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

    $scope.test = function () {
        $http({
            url: ngSettings.apiServiceBaseUri + 'api/product/test',
            method: 'GET',
            params: {
                msg: "uuu"
            }
        });
    }


}]);