'use strict';
angular.module('filters', []).filter('moneySeparatorAdd', function () {
    return function (prices) {
        if (!prices)
            return "Price wasn't set...";
        return prices.min.toString().replace(/(\d)(?=(\d\d\d)+([^\d]|$))/g, '$1 ') + ' BLR';
    };
});