'use strict';
app.controller('signinController', ['$scope', '$location', 'authService', 'ngAuthSettings', function ($scope, $location, authService, ngAuthSettings) {

    $scope.loginData = {
        userName: "",
        password: "",
        useRefreshTokens: false
    };

    $scope.message = "";

    $scope.login = function () {
        authService.login($scope.loginData).then(function (response) {
            $location.path('/orders');
        },
         function (err) {
             $scope.message = err.error_description;
         });
    };

    $scope.authExternalProvider = function (provider) {
        var redirectUri = location.protocol + '//' + location.host + '/authcomplete.html';
        var externalProviderUrl = ngAuthSettings.apiServiceBaseUri + "api/Account/ExternalLogin?provider=" + provider
                                                                    + "&response_type=token&client_id=" + ngAuthSettings.clientId
                                                                    + "&redirect_uri=" + redirectUri;
        window.$windowScope = $scope;
        var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
    };

    $scope.authCompletedCB = function (fragment) {
        $scope.$apply(function () {
            if (fragment.haslocalaccount == 'False') {
                authService.logOut();
                authService.externalAuthData = {
                    provider: fragment.provider,
                    userName: fragment.external_user_name,
                    externalAccessToken: fragment.external_access_token,
                    userId: fragment.user_id
                };
                $location.path('/associate');
            }
            else {
                //Obtain access token and redirect to orders
                var externalData = {
                    provider: fragment.provider,
                    externalAccessToken: fragment.external_access_token,
                    userId: fragment.user_id
                };
                authService.obtainAccessToken(externalData).then(function (response) {
                    $location.path('/products');
                },
             function (err) {
                 $scope.message = err.error_description;
             });
            }
        });
    }

}]);
