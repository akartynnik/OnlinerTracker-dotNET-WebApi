using Autofac.Integration.WebApi;
using AutoMapper;
using OnlinerTracker.Api.ApiViewModels;
using OnlinerTracker.Api.Models;
using OnlinerTracker.Api.Resources;
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
        private IDialogService _dialogService;
        private IExternalProductService _externalPproductService;

        public ProductsController(IProductService productService, IExternalProductService externalPproductService, IDialogService dialogService)
        {
            _productService = productService;
            _dialogService = dialogService;
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
        public async Task<IHttpActionResult> GetExternal(string searchQuery, int page)
        {

            return Ok(_externalPproductService.Get(searchQuery, User.Id, page));
        }

        [Route("Follow", Name = "Follow product")]
        [HttpPost]
        public async Task<IHttpActionResult> Follow(ProductFollowModel model)
        {
            try
            {
                var product = Mapper.Map<ProductFollowModel, Product>(model);
                product.Id = Guid.NewGuid();
                product.UserId = User.Id;
                product.Tracking = true;

                if (_productService.GetBy(product.OnlinerId, product.UserId) != null)
                {
                    _dialogService.SendInPopupForUser(PopupType.Warning, DialogResources.Warning_DuplicateTracking, User.SignalRConnectionId);
                    return Duplicate();
                }
                _productService.Insert(product);

                var cost = Mapper.Map<ProductFollowModel, Cost>(model);
                cost.Id = Guid.NewGuid();
                cost.ProductId = product.Id;
                cost.CratedAt = DateTime.Now;

                _productService.InsertCost(cost);
                _dialogService.SendInPopupForUser(PopupType.Success,
                    string.Format(DialogResources.Success_StartFollowProduct, product.Name), User.SignalRConnectionId);
                return Successful();
            }
            catch (Exception ex)
            {
                _dialogService.SendInPopupForUser(PopupType.Error, DialogResources.Error_ServerError, User.SignalRConnectionId);
                return InternalServerError(ex);
            }

        }

        [Route("ChangeTrackingStatus", Name = "Change tracking status")]
        [HttpPost]
        public IHttpActionResult ChangeTrackingStatus(Product product)
        {
            _productService.Update(product);
            if (product.Tracking)
            {
                _dialogService.SendInPopupForUser(PopupType.Success,
                    string.Format(DialogResources.Success_TrackingStarted, product.Name), User.SignalRConnectionId);
            }
            else
            {
                _dialogService.SendInPopupForUser(PopupType.Warning,
                   string.Format(DialogResources.Warning_TrackingStoped, product.Name), User.SignalRConnectionId);
            }
            return Successful();
        }


        [Route("remove", Name = "Remove product")]
        [HttpPost]
        public IHttpActionResult Remove(DeletedObject obj)
        {
            _dialogService.SendInPopupForUser(PopupType.Warning,
                    string.Format(DialogResources.Warning_ProductDeleted, obj.Name), User.SignalRConnectionId);
            _productService.Delete(obj.Id);
            return Successful();
        }
    }
}