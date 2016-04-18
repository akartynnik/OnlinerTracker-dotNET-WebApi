using AutoMapper;
using OnlinerTracker.Api.ApiViewModels;
using OnlinerTracker.Data;
using System.Linq;

namespace OnlinerTracker.Api
{
    public class AutomapperConig
    {
        public class ProviderMappingProfile : Profile
        {
            protected override void Configure()
            {
                CreateMap<ProductFollowModel, Product>();
                CreateMap<ProductFollowModel, Cost>()
                    .ForMember(x => x.Value, x => x.MapFrom(m => m.Cost));
                CreateMap<Product, ExternalProduct>()
                    .ForMember(x => x.CurrentCost, x => x.MapFrom(m => m.Costs.OrderByDescending(u => u.CratedAt).FirstOrDefault().Value))
                    .ForMember(x => x.UpdatedAt, x => x.MapFrom(m => m.Costs.OrderByDescending(u => u.CratedAt).FirstOrDefault().CratedAt));
            }
        }
    }
}