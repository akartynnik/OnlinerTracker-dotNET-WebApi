using NUnit.Framework;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.SessionState;

namespace OnlinerTracker.UnitTests
{
    [TestFixture]
    public class ProductControllerTests
    {
        

        public static Principal FakeIdentityUser()
        {
            return new Principal(null)
            {
                Id = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a")
            };
        }

        public static HttpContext FakeHttpContext()
        {
            var httpRequest = new HttpRequest("", "http://stackoverflow/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null)
                                .Invoke(new object[] { sessionContainer });

            return httpContext;
        }

        private class FakeProductsService : IProductService
        {
            private readonly Product _product = new Product()
            {
                Id = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a"),
                UserId = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a"),
                Name = "Test product 1",
                Description = "Description 1",
                OnlinerId = "555",
                CurrentCost = 100,
                ImageUrl = string.Empty,
                Costs = new List<Cost>
                    {
                        new Cost()
                        {
                            Id = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a"),
                            ProductId = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a"),
                            Value = 30000,
                            CratedAt = DateTime.Parse("10/10/2010")
                        }
                    }
            };

            private readonly List<Product> _products = new List<Product>()
            {
                new Product
                {
                    Id = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a"),
                    UserId = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a"),
                    Name = "Test product 1",
                    Description = "Description 1",
                    OnlinerId = "555",
                    CurrentCost = 100,
                    ImageUrl = string.Empty,
                    Costs = new List<Cost>
                        {
                            new Cost()
                            {
                                Id = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a"),
                                ProductId = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a"),
                                Value = 30000,
                                CratedAt = DateTime.Parse("10/10/2010")
                            }
                        }
                }
            };

            private readonly IEnumerable<ProductForNotification> _productsForNotif = new List<ProductForNotification>()
            {
                new ProductForNotification
                {
                    Name = "Test product 1",
                    CurrentCost = 250000,
                },
                new ProductForNotification
                {
                    Name = "Test product 2",
                    CurrentCost = 400000,
                }
            };

            public void Insert(Product obj)
            {
            }

            public void Update(Product obj)
            {
            }

            public void Delete(Guid id)
            {
            }

            public void InsertCost(Cost obj)
            {
            }

            public Product GetById(Guid id)
            {
                return _product;
            }

            public Product GetBy(string onlinerId, Guid userId)
            {
                return _product;
            }

            public IEnumerable<Product> GetAll(Guid userId)
            {
                return _products;
            }

            public IEnumerable<Product> GetAllTracking()
            {
                return _products;
            }

            public IEnumerable<Product> GetAllCompared()
            {
                return _products;
            }

            public IEnumerable<ProductForNotification> GetAllChanges(Guid userId)
            {
                return _productsForNotif;
            }

            public bool IfSameProductExist(string onlinerId, Guid userId)
            {
                return true;
            }

            public decimal GetCurrentProductCost(Guid productId)
            {
                return 0;
            }
        }
    }
}
