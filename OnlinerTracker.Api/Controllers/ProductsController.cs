using Autofac.Integration.WebApi;
using OnlinerTracker.Api.Models;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace OnlinerTracker.Api.Controllers
{
    [AutofacControllerConfiguration]
    [RoutePrefix("api/Product")]
    public class ProductsController : BaseController
    {
        private IProductService _productService;
        private SecurityRepository _securityRepo;

        public ProductsController (IProductService productService)
        {
            _productService = productService;
            _securityRepo = new SecurityRepository();
        }

        [Route("GetAll", Name = "Get all products for current user")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            return Ok(_productService.GetAll(User.Id));
        }
        
        [Route("Follow", Name = "Follow product")]
        [HttpPost]
        public async Task<IHttpActionResult> Follow(Product product)
        {
            product.Id = Guid.NewGuid();
            product.UserId = User.Id;
            product.Tracking = true;
            if (_productService.GetBy(product.OnlinerId, product.UserId) != null)
                return Duplicate();
            _productService.Insert(product);
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