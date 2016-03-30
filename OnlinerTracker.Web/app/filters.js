'use strict';
angular.module('filters', []).filter('moneySeparatorAdd', function () {
    return function (input) {
        return input.toString().replace(/(\d)(?=(\d\d\d)+([^\d]|$))/g, '$1 ');
    };
});