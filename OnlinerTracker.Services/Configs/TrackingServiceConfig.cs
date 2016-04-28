using OnlinerTracker.Interfaces;

namespace OnlinerTracker.Services.Configs
{
    public class TrackingServiceConfig
    {
        public IProductService ProductService { get; set; }

        public IExternalProductService ExternalProductService { get; set; }

        public ILogService LogService { get; set; }
    }
}
