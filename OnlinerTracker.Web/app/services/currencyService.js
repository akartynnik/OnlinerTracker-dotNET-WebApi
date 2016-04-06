'use strict';
app.factory('currencyService', ['ngSettings', '$http', '$location', 'localStorageService', '$route', 'ngEnums', '$q',
function (ngSettings, $http, $location, localStorageService, $route, ngEnums, $q) {

    var currencyServiceFactory = {};

    var expireDateBLR = new Date();
    expireDateBLR.setFullYear(expireDateBLR.getFullYear() + 1); //expire of BLR is newer (1 year)

    var BLR = {
        type: ngEnums.currencyType.BLR,
        value: 1,
        expire: expireDateBLR
    };

    var writeDefaultCurrency = function() {
        localStorageService.set('currencies', {
            items: { BLR }, current : ngEnums.currencyType.BLR }); //currency by defauld: BLR
    }

    var getCurerntCurrency = function (currencies) {
        var current = {};
        angular.forEach(currencies.items, function(currency, key) {
            if (currency.type === currencies.current) {
                current = currency;
            }
        });
        return current;
    }

    var _getCurerntType = function() {
        if (localStorageService.get('currencies') == null) {
            writeDefaultCurrency(); 
        }
        return localStorageService.get('currencies').current;
    }

    var _getCurrent = function () {
        var deferred = $q.defer();

        var nowDate = new Date();
        var expireDate = new Date(nowDate);
        expireDate.setSeconds(nowDate.getSeconds() + 15); //add 2 minutes (time when currency expire)
        var currencies = localStorageService.get('currencies');
        
        //set default currency
        if (currencies === null) {
            writeDefaultCurrency();
            deferred.resolve(getCurerntCurrency(localStorageService.get('currencies')));
            return deferred.promise;
        }

        //search for needed currency
        var currentyValid = false;
        var currencyType = currencies.current;
        angular.forEach(currencies.items, function (currency, key) {
            var dateOfExpire = new Date(currency.expire);
            if (currency.type === currencyType & dateOfExpire > nowDate) {
                currentyValid = true; //needed curency exist in localStorage
            }
        });

        //if needed currency not found
        if (!currentyValid) {
            $http({
                url: ngSettings.apiServiceBaseUri + 'api/currency/get',
                method: 'GET',
                params: {
                    currencytype: currencyType
                }
            }).success(function (response) {
                currencies.items[ngEnums.currencyName[currencyType]] = { type: response.type, value: response.value, expire: expireDate };
                localStorageService.set('currencies', currencies); //update currencies
                deferred.resolve(getCurerntCurrency(currencies));
            });
        } else {
            localStorageService.set('currencies', currencies); //update currencies
            deferred.resolve(getCurerntCurrency(currencies));
        }
        return deferred.promise;
    }

    var _setCurrent = function (currentCurrency) {
        var currencies = localStorageService.get('currencies');
        //set default currency
        if (currencies === null) {
            writeDefaultCurrency();
        }
        currencies.current = currentCurrency;
        localStorageService.set('currencies', currencies);
        window.location.reload();
    }

    currencyServiceFactory.getCurrentType = _getCurerntType;
    currencyServiceFactory.getCurrent = _getCurrent;
    currencyServiceFactory.setCurrent = _setCurrent;
    return currencyServiceFactory;

}]);