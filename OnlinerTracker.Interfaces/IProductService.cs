using OnlinerTracker.Data;

namespace OnlinerTracker.Interfaces
{
    public interface IProductService
    {
        void Insert(Product product);

        Product GetByOnlinerId(string onlinerId);
    }
}
