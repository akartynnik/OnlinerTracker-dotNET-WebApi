/* Чистка массива строк от дублей */
Array.prototype.unique = function (mutate) {
    var unique = this.reduce(function (accum, current) {
        if (accum.indexOf(current) < 0) {
            accum.push(current);
        }
        return accum;
    }, []);
    if (mutate) {
        this.length = 0;
        for (var i = 0; i < unique.length; ++i) {
            this.push(unique[i]);
        }
        return this;
    }
    return unique;
}

Array.prototype.toArrayOfStrings = function () {
    this.forEach(function (part, index, theArray) {
        theArray[index] = theArray[index].toString();
    });
    return this;
}

Array.prototype.toArrayOfDates = function () {
    this.forEach(function (part, index, theArray) {
        var dateTimeItem = new Date(theArray[index]);
        theArray[index] = dateTimeItem;
    });
    return this;
}

Array.prototype.sordDate = function () {
    return this.sort(function (a, b) { return a.getTime() - b.getTime() });
}

/* Расширение, аппроксимирующее дату до точности в день */
Date.prototype.withoutTime = function () {
    var d = new Date(this);
    d.setHours(0, 0, 0, 0, 0);
    return d;
}

