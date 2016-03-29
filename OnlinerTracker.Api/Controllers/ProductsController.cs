using OnlinerTracker.Api.Models;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace OnlinerTracker.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/Product")]
    public class ProductsController : ApiController
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
            var user = await _securityRepo.FindUserAsync(User.Identity.Name);
            return Ok(_productService.GetAll(Guid.Parse(user.Id)));
        }
        
        [Route("Follow", Name = "Follow product")]
        [HttpPost]
        public async Task<IHttpActionResult> Follow(Product product)
        {
            var user = await _securityRepo.FindUserAsync(User.Identity.Name);
            product.Id = Guid.NewGuid();
            product.UserId = Guid.Parse(user.Id);
            product.Tracking = true;
            if (_productService.GetBy(product.OnlinerId, product.UserId) != null)
                return Ok("Duplicate");
            _productService.Insert(product);
            return Ok("OK");
        }

        [Route("ChangeTrackingStatus", Name = "Change tracking status")]
        [HttpPost]
        public IHttpActionResult ChangeTrackingStatus(Product product)
        {
            _productService.Update(product);
            return Ok("OK");
        }


        [Route("remove", Name = "Remove product")]
        [HttpPost]
        public IHttpActionResult Remove(RemoveObj obj)
        {
            _productService.Delete(obj.Id);
            return Ok("OK");
        }
    }
}