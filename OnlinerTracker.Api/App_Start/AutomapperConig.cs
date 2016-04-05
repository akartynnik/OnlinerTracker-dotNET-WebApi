using AutoMapper;
using OnlinerTracker.Api.ApiViewModels;
using OnlinerTracker.Data;
using System.Linq;

namespace OnlinerTracker.Api
{
    public class AutomapperConig
    {
        public static void Register()
        {
            Mapper.CreateMap<ProductFollowModel, Product>();
            Mapper.CreateMap<ProductFollowModel, Cost>()
                .ForMember(x => x.Value, x => x.MapFrom(m => m.Cost));
            Mapper.CreateMap<Product, ExternalProduct>()
                .ForMember(x => x.CurrentCost, x => x.MapFrom(m => m.Costs.OrderByDescending(u => u.CratedAt).FirstOrDefault().Value))
                .ForMember(x => x.UpdatedAt, x => x.MapFrom(m => m.Costs.OrderByDescending(u => u.CratedAt).FirstOrDefault().CratedAt));
        }
    }
}