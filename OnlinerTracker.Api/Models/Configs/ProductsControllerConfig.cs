using AutoMapper;
using OnlinerTracker.Interfaces;

namespace OnlinerTracker.Api.Models.Configs
{
    public class ProductsControllerConfig
    {
        public IProductService ProductService { get; set; }

        public IDialogService DialogService { get; set; }

        public IExternalProductService ExternalProductService { get; set; }

        public IMapper Mapper { get; set; }
    }
}