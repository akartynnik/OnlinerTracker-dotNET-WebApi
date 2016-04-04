'use strict';
app.factory('signalRService', ['$rootScope', 'Hub', '$timeout', function ($rootScope, Hub, $timeout) {
    var promise;
    $rootScope.popupVisible = false;
    $rootScope.popupMessage = "";
    $rootScope.popupClass = "";

    var hub = new Hub('dialog', {

        //client side methods
        listeners: {
            'showPopup': function (type, message) {
                //stop showPopup timer
                $timeout.cancel(promise);
                //choise popup class
                switch (type) {
                    case "Success":
                        $rootScope.popupClass = "alert-success";
                        break;
                    case "Warning":
                        $rootScope.popupClass = "alert-warning";
                        break;
                    case "Error":
                        $rootScope.popupClass = "alert-danger";
                        break;
                    default:
                        $rootScope.popupClass = "alert-success";
                        break;
                }
                $rootScope.popupMessage = message;
                $rootScope.popupVisible = true;
                promise = $timeout(function () { $rootScope.popupVisible = false; }, 5000);
            }
        },

        rootPath: "http://localhost:52304/signalr",

        errorHandler: function (error) {
            console.error(error);
        },

        //server side methods
        methods: []
    });

    var _connectionId = function () {
        return hub.connection.id;
    };

    return {
        connectionId: _connectionId
    };
}]);