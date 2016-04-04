using OnlinerTracker.Data;
using OnlinerTracker.Data.Context;
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

        private readonly object _lock = new object();

        private readonly IProductService _productService;

        private readonly IExternalProductService _externalProductService;

        private readonly TrackerContext _context;
        public string MinutesBeforeCheck { get; set; }

        #endregion

        #region Constructors

        public TrackingService(IProductService productService, IExternalProductService externalProductService)
        {
            _productService = productService;
            _externalProductService = externalProductService;
            _context = new TrackerContext();
        }

        #endregion

        #region ITrackingService methods

        public async Task<string> CheckProduct()
        {
            var minutesBeforeCheck = 0;
            int.TryParse(MinutesBeforeCheck, out minutesBeforeCheck);
            var lastCheck = _context.JobsLogs.OrderByDescending(u => u.CheckedAt).FirstOrDefault(u => u.Type == JobType.CostCheck && u.IsSuccessed);
            if (lastCheck != null && (DateTime.Now - lastCheck.CheckedAt).Minutes < minutesBeforeCheck)
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

                foreach (var cost in costsUpdateList)
                    _productService.InsertCost(cost);

                _context.JobsLogs.Add(new JobLog
                {
                    Type = JobType.CostCheck,
                    CheckedAt = DateTime.Now,
                    IsSuccessed = true
                });
                _context.SaveChanges();
                return string.Empty;
            }
            catch (Exception ex)
            {
                _context.JobsLogs.Add(new JobLog
                {
                    Type = JobType.CostCheck,
                    CheckedAt = DateTime.Now,
                    IsSuccessed = false,
                    Info = ex.Message
                });
                _context.SaveChanges();
                return ex.Message;
            }
            
        }

        #endregion

        #region IJob methods 

        public void Execute()
        {
            lock (_lock)
                CheckProduct();
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