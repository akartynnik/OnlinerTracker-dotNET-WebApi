'use strict';
app.controller('chartsController', ['$scope', '$location', 'productsService', '$filter', function ($scope, $location, productsService, $filter) {
    $scope.series = [];//продукты
    $scope.data = [];
    $scope.labels = [];
    $scope.options = {
    };

    productsService.getAllCompared().then(function (results) {
        var seriesArray = [];
        var labelArray = [];
        var dataArrays = [];

        //занесение названий графиков и лейблов оси X
        angular.forEach(results.data, function(product, key) {
            seriesArray.push(product.name); // заполнение названий графиков
            angular.forEach(product.costs, function (cost, key) {
                var createdAt = new Date(cost.cratedAt);
                labelArray.push(createdAt.withoutTime()); // заполнение лейблов (всеми значениями, втч дублями)
            });
        });

        labelArray.sordDate(); //сортировка лейблов (тип DateTime)
        labelArray.toArrayOfStrings(); //перобразование элеметнов типа DateTime в String
        labelArray.unique(true); //удаление дубликатов в массиве

        //построение массивов данных в соответствии с массивом лейблов (кол-во точек должно быть равным в обоих массивах)
        angular.forEach(results.data, function (product, key) {
            var dataArray = [];
            dataArray.length = labelArray.length; //уравнивание длинн массивов (длинна массива данных должна быть равна длинне массива лейблов)
            angular.forEach(product.costs, function (cost, key) {
                var createdAt = new Date(cost.cratedAt);
                dataArray[labelArray.indexOf(createdAt.withoutTime().toString())] = cost.value;
            });
            dataArrays.push(dataArray);
        });

        //преобразование массивов данных (замена null-элементов на их ближайшего не-null соседа слева)
        angular.forEach(dataArrays, function (dataArray, key) {
            var cache = 0;
            for (var index = 0; index < dataArray.length; index++) {
                if (index !== 0 && dataArray[index - 1] != null) {
                    cache = dataArray[index-1];
                }
                if (dataArray[index] == null) {
                    dataArray[index] = cache;
                }
            }
            dataArrays[key] = dataArray;
        });

        labelArray.toArrayOfDates(); //перобразование массива строк в массив времен
        labelArray.forEach(function (part, index, theArray) {
            theArray[index] = $filter('date')(theArray[index], "dd.MM.yyyy");
        }); // настройка output date format у лейблов

        $scope.series = seriesArray;
        $scope.data = dataArrays;
        $scope.labels = labelArray;
    });

    $scope.onClick = function (points, evt) {
        console.log(points, evt);
    };


    $scope.backToProductList = function () {
        return $location.path('/products');
    }
}]);