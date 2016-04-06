'use strict';
angular.module('filters', [])
    .filter('priceFormater', ['localStorageService', 'ngEnums', 'currencyService', function (localStorageService, ngEnums, currencyService) {
        var cache = {};
        function convertCost(cost) {
            if (cache[cost]) {
                return cache[cost];
            }
            cache[cost] = "fetching...";
            currencyService.getCurrent().then(function (currency) {
                var newcost = parseInt(cost / currency.value);
                cache[cost] = '<strong>' + newcost.toString().replace(/(\d)(?=(\d\d\d)+([^\d]|$))/g, '$1 ') + '</strong> ' + ngEnums.currencyName[currency.type];
            });
        }
        convertCost.$stateful = true;
        return convertCost;
    }])