using OnlinerTracker.Data;
using System;
using System.Collections.Generic;

namespace OnlinerTracker.Interfaces
{
    public interface IProductService
    {
        void Insert(Product obj);

        void InsertCost(Cost obj);

        void Update(Product obj);

        void Delete(Guid id);

        Product GetById(Guid id);

        Product GetBy(string onlinerId, Guid userId);

        IEnumerable<Product> GetAll(Guid userId);

        List<Product> ConvertToProducts(string externalProductsJsonString, RemoteServiceType providerType,
            Guid userId);
    }
}
