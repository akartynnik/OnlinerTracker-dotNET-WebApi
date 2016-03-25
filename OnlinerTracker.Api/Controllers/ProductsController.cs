using System.Collections.Generic;
using System.Web.Http;

namespace OnlinerTracker.Api.Controllers
{
    [RoutePrefix("api/Products")]
    public class ProductsController : ApiController
    {
        [Authorize]
        [Route("")]
        public IHttpActionResult Get()
        {
            return Ok(Product.CreateProducts());
        }

    }

    #region Helpers

    public class Product
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string UserName { get; set; }

        public bool IsTracked { get; set; }

        public static List<Product> CreateProducts()
        {
            List<Product> productList = new List<Product>
            {
                new Product {Id = 10248, UserName = "Taiseer Joudeh", Description = "Amman", IsTracked = true },
                new Product {Id = 10249, UserName = "Ahmad Hasan", Description = "Dubai", IsTracked = true},
                new Product {Id = 10250, UserName = "Tamer Yaser", Description = "Jeddah", IsTracked = false },
                new Product {Id = 10251, UserName = "Lina Majed", Description = "Abu Dhabi", IsTracked = false},
                new Product {Id = 10252, UserName = "Yasmeen Rami", Description = "Kuwait", IsTracked = true}
            };

            return productList;
        }
    }

    #endregion
}