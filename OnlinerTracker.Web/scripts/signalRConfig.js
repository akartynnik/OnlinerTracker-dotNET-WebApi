$(function () {
    var promise;

    var hub = $.connection.dialog;
    $.connection.hub.url = 'http://localhost:52304/signalr';
    hub.client.ShowDialogBox = function (type, message) {
        //console.log(type + ": " + message);
        clearTimeout(promise);
        $("#dialog_box").removeAttr('class').attr('class', 'alert');
        switch (type) {
            case "PopupSuccess":
                $("#dialog_box").addClass('alert-success');
                break;
            case "PopupWarning":
                $("#dialog_box").addClass('alert-warning');
                break;
            case "PopupError":
                $("#dialog_box").addClass('alert-danger');
                break;
            default:
                $("#dialog_box").addClass('alert-success');
                break;
        }
        $("#dialog_message").html(message);
        $("#dialog_box").slideDown(400);
        promise = setTimeout(function() {
            $("#dialog_box").slideUp(400);
        }, 5000);
    }

    $.connection.hub.start().done(function () {
        /** Listen for events **/
        //$("#send").click(function () {
            //hub.server.sendMessage("llll");
        //});
    });
});