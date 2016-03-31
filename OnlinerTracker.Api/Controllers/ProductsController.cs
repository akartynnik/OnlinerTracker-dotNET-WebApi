using Autofac.Integration.WebApi;
using AutoMapper;
using OnlinerTracker.Api.ApiViewModels;
using OnlinerTracker.Api.Models;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace OnlinerTracker.Api.Controllers
{
    [AutofacControllerConfiguration]
    [RoutePrefix("api/Product")]
    public class ProductsController : ApiControllerBase
    {
        private IProductService _productService;
        private IExternalProductService _externalPproductService;

        public ProductsController (IProductService productService, IExternalProductService externalPproductService)
        {
            _productService = productService;
            _externalPproductService = externalPproductService;
        }

        [Route("GetAll", Name = "Get all products for current user")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            return Ok(Mapper.Map<IEnumerable<Product>, IEnumerable<ExternalProduct>>(_productService.GetAll(User.Id)));
        }

        [Route("GetFromExternalServer", Name = "Get products from external server (proxy)")]
        [HttpGet]
        public async Task<IHttpActionResult> GetExternal(string searchQuery)
        {

            return Ok(_externalPproductService.Get(searchQuery, User.Id));
        }

        [Route("Follow", Name = "Follow product")]
        [HttpPost]
        public async Task<IHttpActionResult> Follow(ProductFollowModel model)
        {
            var product = Mapper.Map<ProductFollowModel, Product>(model);
            product.Id = Guid.NewGuid();
            product.UserId = User.Id;
            product.Tracking = true;

            if (_productService.GetBy(product.OnlinerId, product.UserId) != null)
                return Duplicate();
            _productService.Insert(product);

            var cost = Mapper.Map<ProductFollowModel, Cost>(model);
            cost.Id = Guid.NewGuid();
            cost.ProductId = product.Id;
            cost.CratedAt = DateTime.Now;

            _productService.InsertCost(cost);
            return Successful();
        }

        [Route("ChangeTrackingStatus", Name = "Change tracking status")]
        [HttpPost]
        public IHttpActionResult ChangeTrackingStatus(Product product)
        {
            _productService.Update(product);
            return Successful();
        }


        [Route("remove", Name = "Remove product")]
        [HttpPost]
        public IHttpActionResult Remove(DeletedObject obj)
        {
            _productService.Delete(obj.Id);
            return Successful();
        }
    }
}