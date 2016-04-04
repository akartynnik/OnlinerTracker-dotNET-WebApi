'use strict';
angular.module('filters', [])
    .filter('priceFormater', function () {
        return function (cost) {
            if (cost === 0)
                return "Price wasn't set...";
            return '<strong>' + cost.toString().replace(/(\d)(?=(\d\d\d)+([^\d]|$))/g, '$1 ') + '</strong> BLR';
        };
    })