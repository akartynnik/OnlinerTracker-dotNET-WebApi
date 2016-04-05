using OnlinerTracker.Data;
using System;
using System.Collections.Generic;

namespace OnlinerTracker.Interfaces
{
    public interface IExternalProductService
    {
        string Get(string searchQuery, int page);

        List<Product> ConvertJsonToProducts(string externalProductsJsonString, Guid? userId = null);
    }
}
