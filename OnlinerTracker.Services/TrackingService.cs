using OnlinerTracker.Core;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Services.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace OnlinerTracker.Services
{
    public class TrackingService : ITrackingService
    {
        #region Fields and Properties

        private readonly TrackingServiceConfig _config;

        #endregion

        #region Constructors

        public TrackingService(TrackingServiceConfig config)
        {
            _config = config;
        }

        #endregion

        #region ITrackingService methods

        public async Task CheckProducts(int minutesBeforeCheck)
        {
            if (HttpContext.Current == null)
                return;
            var lastSuccessLog = _config.LogService.GetLastSuccessLog(JobType.CostCheck);
            if (lastSuccessLog != null && (SystemTime.Now - lastSuccessLog.CheckedAt).TotalMinutes < minutesBeforeCheck)
                return;
            await CheckProducts();
        }

        public async Task CheckProducts()
        {
            try
            {
                var costsUpdateList = new List<Cost>();
                foreach (var product in _config.ProductService.GetAllTracking())
                {
                    var cost = await IsCostChanged(product);
                    if (cost != null)
                        costsUpdateList.Add(cost);
                }
                var updatedCostsCount = 0;
                foreach (var cost in costsUpdateList)
                {
                    updatedCostsCount++;
                    _config.ProductService.InsertCost(cost);
                }
                if (updatedCostsCount > 0)
                {
                    _config.LogService.AddJobLog(JobType.CostCheck, string.Format("Number of updated costs: {0}", updatedCostsCount));
                }
            }
            catch (Exception ex)
            {
                _config.LogService.AddJobLog(JobType.CostCheck, ex.Message, false);
            }
        }

        #endregion

        #region Additional methods

        private async Task<Cost> IsCostChanged(Product product)
        {
            SystemThread.Sleep(2000);
            var externalProductJson = _config.ExternalProductService.Get(product.Name, 1);
            var externalProduct = _config.ExternalProductService.ConvertJsonToProducts(externalProductJson).FirstOrDefault();
            product.CurrentCost = _config.ProductService.GetCurrentProductCost(product.Id);
            if (externalProduct == null || externalProduct.CurrentCost == product.CurrentCost)
                return null;
            var cost = new Cost
            {
                CratedAt = SystemTime.Now,
                ProductId = product.Id,
                Value = externalProduct.CurrentCost
            };
            return cost;
        }

        #endregion
    }
}