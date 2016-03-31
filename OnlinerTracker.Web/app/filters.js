'use strict';
angular.module('filters', [])
    .filter('priceFromOnlinerSeparatorFormater', function () {
        return function (prices) {
            if (!prices)
                return "Price wasn't set...";
            return '<strong>' + prices.min.toString().replace(/(\d)(?=(\d\d\d)+([^\d]|$))/g, '$1 ') + '</strong> BLR';
        };
    })
    .filter('priceFromServerFormater', function () {
        return function (cost) {
            if (!cost)
                return "Price wasn't set...";
            return '<strong>' + cost.toString().replace(/(\d)(?=(\d\d\d)+([^\d]|$))/g, '$1 ') + '</strong> BLR';
        };
    })