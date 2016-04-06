'use strict';
app.controller('indexController', ['$scope', '$location', 'authService', 'currencyService', 'ngEnums',
    function ($scope, $location, authService, currencyService, ngEnums) {
    $scope.currencyTypes = ngEnums.currencyType;
    $scope.currentCurrency = ngEnums.currencyName[currencyService.getCurrentType()];

    $scope.changeCurrency = function (currencyType) {
        currencyService.setCurrent(currencyType);
        $scope.currentCurrency = ngEnums.currencyName[currencyType];
    }

    $scope.isActive = function (viewLocation) {
        var active = (viewLocation === $location.path());
        return active;
    };

    $scope.logOut = function () {
        authService.logOut();
    }

    $scope.authentication = authService.authentication;
}]);