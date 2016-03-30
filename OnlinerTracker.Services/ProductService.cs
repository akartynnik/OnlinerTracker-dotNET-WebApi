using OnlinerTracker.Data;
using OnlinerTracker.Data.Context;
using OnlinerTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        public void Update(Product obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(Guid id)
        {
            var product = GetById(id);
            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        public void InsertCost(Cost obj)
        {
            _context.Costs.Add(obj);
            _context.SaveChanges();
        }

        public Product GetById(Guid id)
        {
            return _context.Products.FirstOrDefault(u => u.Id == id);
        }

        public Product GetBy(string onlinerId, Guid userId)
        {
            return _context.Products.FirstOrDefault(u => u.OnlinerId == onlinerId && u.UserId == userId);
        }

        public IEnumerable<Product> GetAll(Guid userId)
        {
            return _context.Products.Where(u => u.UserId == userId);
        }
    }
}
