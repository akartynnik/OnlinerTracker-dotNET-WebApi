'use strict';
app.factory('dialogService', ['$rootScope', 'Hub', '$timeout', 'ngSettings', 'localStorageService',
    function ($rootScope, Hub, $timeout, ngSettings, localStorageService) {


    var promise;
    $rootScope.popupVisible = false;
    $rootScope.popupMessage = "";
    $rootScope.popupClass = "";
    var _connectionId;

    function showPopup(type, message) {
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
        promise = $timeout(function() { $rootScope.popupVisible = false; }, 3000);
    }



    if (ngSettings.dialogProvider === "zeromq") {
        /* ZeroMQ realisation */
        var dealer = new JSMQ.Dealer();
        var subscriber = new JSMQ.Subscriber();

        dealer.connect("ws://localhost:98");
        subscriber.connect("ws://localhost:99");

        dealer.sendReady = function() {
            var message = new JSMQ.Message();
            message.addString("wantId");
            dealer.send(message);
        };

        dealer.onMessage = function(message) {
            var conId = message.popString();
            //set session connection id for subscriber
            subscriber.subscribe(conId);
            localStorageService.set('netMqConnectionId', conId);
            console.log("Start dialog (provided by NetMq). Session: " + conId);
        }

        subscriber.onMessage = function(message) {
            message.popString(); //ignore first frame with data - it's client connectionId
            showPopup("Success", message.popString());
        }

        _connectionId = function() {
            return localStorageService.get('netMqConnectionId');
        };
    } else {
        /* SignalR realisation */
        var hub = new Hub('dialog', {
            //client side methods
            listeners: {
                'showPopup': function (type, message) {
                    showPopup(type, message);
                }
            },

            rootPath: ngSettings.signalRServerPath,

            errorHandler: function (error) {
                console.error(error);
            },

            //server side methods
            methods: []
        });

        _connectionId = function () {
            return hub.connection.id;
        };
    }

    return {
        connectionId: _connectionId
    };

}]);