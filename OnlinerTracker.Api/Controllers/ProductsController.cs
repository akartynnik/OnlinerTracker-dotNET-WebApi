using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace OnlinerTracker.Api.Controllers
{
    [Authorize]
    [RoutePrefix("api/Products")]
    public class ProductsController : ApiController
    {
        private IProductService _productService;
        private SecurityRepository _securityRepo;

        public ProductsController (IProductService productService)
        {
            _productService = productService;
            _securityRepo = new SecurityRepository();
        }
        [Route("")]
        [HttpGet]
        public IHttpActionResult Get()
        {
            return Ok();
        }
        
        [Route("Follow", Name = "Follow Product")]
        [HttpPost]
        public async Task<IHttpActionResult> Follow(Product product)
        {
            if (_productService.GetByOnlinerId(product.OnlinerId) != null)
                return Ok("Duplicate");
            product.Id = Guid.NewGuid();
            var user = await _securityRepo.FindUserAsync(User.Identity.Name);
            product.UserId = Guid.Parse(user.Id);
            _productService.Insert(product);
            return Ok("OK");
        }
        
        [Route("")]
        [HttpPost]
        public IHttpActionResult Update()
        {
            return Ok();
        }

    }
}