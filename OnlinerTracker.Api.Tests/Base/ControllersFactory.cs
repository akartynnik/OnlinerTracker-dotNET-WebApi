using AutoMapper;
using NSubstitute;
using OnlinerTracker.Api.ApiViewModels;
using OnlinerTracker.Api.Controllers;
using OnlinerTracker.Api.Models.Configs;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;

namespace OnlinerTracker.Api.Tests.Base
{
    public class ControllersFactory
    {
        public static ProductsController GetProductsController(out ProductsControllerConfig fakeConfig, Principal sessionUser = null)
        {
            var fakeDialogService = Substitute.For<IDialogService>();
            var fakeProductService = Substitute.For<IProductService>();
            var fakeExternalProductService = Substitute.For<IExternalProductService>();
            var fakeMapper = Substitute.For<IMapper>();
            var fakePrincipleService = Substitute.For<IPrincipalService>();

            SetDefaultMapperConfiguration(ref fakeMapper);
            SetDefaultPrincipalConfiguration(ref fakePrincipleService, sessionUser);
            fakeConfig = new ProductsControllerConfig
            {
                DialogService = fakeDialogService,
                ProductService = fakeProductService,
                ExternalProductService = fakeExternalProductService,
                Mapper = fakeMapper
            };
            return new ProductsController(fakeConfig, fakePrincipleService);
        }

        public static CurrencyController GetCurrencyController(out ICurrencyService fakeCurrancyService, Principal sessionUser = null)
        {
            fakeCurrancyService = Substitute.For<ICurrencyService>();
            var fakePrincipleService = Substitute.For<IPrincipalService>();

            SetDefaultPrincipalConfiguration(ref fakePrincipleService, sessionUser);

            return new CurrencyController(fakeCurrancyService, fakePrincipleService);
        }

        private static void SetDefaultPrincipalConfiguration(ref IPrincipalService stubPrincipleService, Principal sessionUser)
        {
            if (sessionUser == null)
            {
                sessionUser = new Principal(null)
                {
                    Id = TestsConstants.DefaultSessionUserId,
                    DialogConnectionId = TestsConstants.DefaultDialogId,
                };
            }
            stubPrincipleService.GetSessionUser().Returns(sessionUser);
        }

        private static void SetDefaultMapperConfiguration(ref IMapper stubMpper)
        {
            stubMpper.Map<ProductFollowModel, Product>(Arg.Any<ProductFollowModel>())
                .Returns(new Product());
            stubMpper.Map<ProductFollowModel, Cost>(Arg.Any<ProductFollowModel>())
                .Returns(new Cost());
        }
    }
}