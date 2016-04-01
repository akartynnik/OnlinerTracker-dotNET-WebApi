using Newtonsoft.Json;
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

        public List<Product> ConvertToProducts(string externalProductsJsonString, RemoteServiceType providerType, Guid userId)
        {
            var productList = new List<Product>();
            dynamic externalProducts = JsonConvert.DeserializeObject(externalProductsJsonString);
            foreach (var externalProduct in externalProducts["products"])
            {
                var product = new Product();
                if (providerType == RemoteServiceType.Onliner)
                {
                    product.OnlinerId = externalProduct["id"];
                    product.Name = externalProduct["full_name"];
                    product.Description = externalProduct["description"];
                    product.ImageUrl = externalProduct["images"]["header"];
                    product.CurrentCost = externalProduct["prices"] != null ? externalProduct["prices"]["min"] : 0;
                }
                var sameProduct = _context.Products.Any(u => u.OnlinerId == product.OnlinerId && u.UserId == userId);
                if (sameProduct)
                    product.Tracking = true;
                productList.Add(product);
            }
            return productList;
        }
    }
}
