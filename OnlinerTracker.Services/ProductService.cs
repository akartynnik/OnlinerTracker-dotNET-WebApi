using OnlinerTracker.Data;
using OnlinerTracker.Data.Context;
using OnlinerTracker.Interfaces;
using System.Linq;

namespace OnlinerTracker.Services
{
    public class ProductService : IProductService
    {
        private readonly TrackerContext _context;
        public ProductService()
        {
            _context = new TrackerContext();
        }

        public void Insert(Product obj)
        {
            _context.Products.Add(obj);
            _context.SaveChanges();
        }

        public Product GetByOnlinerId(string onlinerId)
        {
            return _context.Products.FirstOrDefault(u => u.OnlinerId == onlinerId);
        }
    }
}
