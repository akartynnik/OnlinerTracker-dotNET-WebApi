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

        IEnumerable<Product> GetAllTracking();

        IEnumerable<ProductForNotification> GetAllChanges(Guid userId);

        bool IfSameProductExist(string onlinerId, Guid userId);
        
        decimal GetCurrentProductCost(Guid productId);
    }
}
