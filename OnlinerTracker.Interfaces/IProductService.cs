using OnlinerTracker.Data;
using System;

namespace OnlinerTracker.Interfaces
{
    public interface IProductService
    {
        void Insert(Product product);

        Product Get(string onlinerId, Guid userId);
    }
}
