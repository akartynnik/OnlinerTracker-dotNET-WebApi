'use strict';
app.controller('indexController', ['$scope', '$location', 'authService', function ($scope, $location, authService) {
    $scope.isActive = function (viewLocation) {
        var active = (viewLocation === $location.path());
        return active;
    };

    $scope.logOut = function () {
        authService.logOut();
    }

    $scope.authentication = authService.authentication;

}]);