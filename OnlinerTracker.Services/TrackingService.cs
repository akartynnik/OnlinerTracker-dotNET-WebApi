using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlinerTracker.Services
{
    public class TrackingService : ITrackingService
    {
        #region Fields and Properties

        private readonly IProductService _productService;

        private readonly IExternalProductService _externalProductService;

        private readonly ILogService _logService;

        public string MinutesBeforeCheck { get; set; }

        #endregion

        #region Constructors

        public TrackingService(IProductService productService, IExternalProductService externalProductService, ILogService logService)
        {
            _productService = productService;
            _externalProductService = externalProductService;
            _logService = logService;
        }

        #endregion

        #region ITrackingService methods

        public async Task<string> CheckProducts()
        {
            var minutesBeforeCheck = 0;
            int.TryParse(MinutesBeforeCheck, out minutesBeforeCheck);
            var lastSuccessLog = _logService.GetLastSuccessLog(JobType.EmailSend);
            if (lastSuccessLog != null && (DateTime.Now - lastSuccessLog.CheckedAt).Minutes < minutesBeforeCheck)
                return string.Empty;
            try
            {
                var costsUpdateList = new List<Cost>();
                foreach (var product in _productService.GetAllTracking())
                {
                    var cost = await IsCostChanged(product);
                    if (cost != null)
                        costsUpdateList.Add(cost);
                }
                var updatedCostsCount = 0;
                foreach (var cost in costsUpdateList)
                {
                    updatedCostsCount++;
                    _productService.InsertCost(cost);
                }
                _logService.AddJobLog(JobType.CostCheck, string.Format("Number of updated costs: {0}", updatedCostsCount));
                return string.Empty;
            }
            catch (Exception ex)
            {
                _logService.AddJobLog(JobType.CostCheck, ex.Message, false);
                return ex.Message;
            }
            
        }

        #endregion

        #region Additional methods

        private async Task<Cost> IsCostChanged(Product product)
        {
            System.Threading.Thread.Sleep(2000);
            var externalProductJson = _externalProductService.Get(product.Name, 1);
            var externalProduct = _externalProductService.ConvertJsonToProducts(externalProductJson).FirstOrDefault();
            product.CurrentCost = _productService.GetCurrentProductCost(product.Id);
            if (externalProduct == null || externalProduct.CurrentCost == product.CurrentCost)
                return null;
            var cost = new Cost
            {
                CratedAt = DateTime.Now,
                ProductId = product.Id,
                Value = externalProduct.CurrentCost
            };
            return cost;
        }

        #endregion
    }
}