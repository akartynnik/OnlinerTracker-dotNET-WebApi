'use strict';
app.controller('productsController', ['$scope', 'productsService', '$http', 'ngAuthSettings', '$timeout', function ($scope, productsService, $http, ngAuthSettings, $timeout) {

    $scope.products = [];
    $scope.textAlert = "";
    $scope.showAlert = false;
    $scope.alertClassName = "alert-success";
    var promise;

    productsService.getProducts().then(function (results) {
        $scope.products = results.data;
    }, function (error) {
        alert(error.data.message);
    });

    $scope.changeTrackingStatus = function (product) {
        //stop alerts timer
        $timeout.cancel(promise);
        //send request
        $http({
            url: ngAuthSettings.apiServiceBaseUri + 'api/product/ChangeTrackingStatus',
            method: 'post',
            data: product,
        }).success(function (response) {
            console.log(response);
            if (response == "OK") {
                $scope.showAlert = true;
                if (product.tracking) {
                    $scope.textAlert = '<b>' + product.name + "</b> tracking is started!";
                    $scope.alertClassName = "alert-success";
                } else {
                    $scope.textAlert = '<b>' + product.name + "</b> tracking is stopped!";
                    $scope.alertClassName = "alert-warning";
                }
                promise = $timeout(function () { $scope.showAlert = false; }, 3000);
            }
        }).error(function (msg) {
            onError();
        });
    }

    $scope.remove = function (product) {
        //clear alerts lifetimer
        $timeout.cancel(promise);
        //send request
        $http({
            url: ngAuthSettings.apiServiceBaseUri + 'api/product/remove',
            method: 'post',
            data: {
                id:product.id
            },
        }).success(function (response) {
            console.log(response);
            if (response == "OK") {
                $scope.showAlert = true;
                $scope.textAlert = '<b>' + product.name + "</b> was deleted.";
                $scope.alertClassName = "alert-warning";
                // update products in scope
                var deletedProductId = $scope.products.indexOf(product);
                $scope.products.splice(deletedProductId, 1);
                //add lifetimer to alert
                promise = $timeout(function () { $scope.showAlert = false; }, 3000);
            }
        }).error(function (msg) {
            onError();
        });
    }

    $scope.closeAlert = function () {
        $scope.showAlert = false;
    }

    function onError() {
        $scope.showAlert = true;
        $scope.alertClassName = "alert-danger";
        $scope.textAlert = 'ERROR!';
        promise = $timeout(function () { $scope.showAlert = false; }, 3000);
    }
}]);