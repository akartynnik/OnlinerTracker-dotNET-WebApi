using AutoMapper;
using NSubstitute;
using OnlinerTracker.Api.ApiViewModels;
using OnlinerTracker.Api.Controllers;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;

namespace OnlinerTracker.Api.Tests.Base
{
    public static class ControllersFactory
    {
        public static ProductsController GetProductController(out IProductService fakeProductService, Principal sessionUser = null)
        {
            IMapper fakeMapper;
            IExternalProductService fakeExternalProductService;
            IPrincipalService fakePrincipleService;
            IDialogService fakeDialogService;
            return GetProductController(out fakeExternalProductService, out fakeDialogService, out fakeMapper,
                out fakePrincipleService, out fakeProductService, sessionUser);
        }

        public static ProductsController GetProductController(
            out IProductService fakeProductService,
            out IDialogService fakeDialogService,
            Principal sessionUser = null)
        {
            IMapper fakeMapper;
            IExternalProductService fakeExternalProductService;
            IPrincipalService fakePrincipleService;
            return GetProductController(out fakeExternalProductService, out fakeDialogService, out fakeMapper,
                out fakePrincipleService, out fakeProductService, sessionUser);
        }

        public static ProductsController GetProductController(
            out IExternalProductService fakeExternalProductService,
            out IDialogService fakeDialogService,
            out IMapper fakeMapper,
            out IPrincipalService fakePrincipleService,
            out IProductService fakeProductService,
            Principal sessionUser)
        {
            fakeDialogService = Substitute.For<IDialogService>();
            fakeProductService = Substitute.For<IProductService>();
            fakePrincipleService = Substitute.For<IPrincipalService>();
            fakeExternalProductService = Substitute.For<IExternalProductService>();
            fakeMapper = Substitute.For<IMapper>();

            SetDefaultMapperConfiguration(ref fakeMapper);
            SetDefaultPrincipalConfiguration(ref fakePrincipleService, sessionUser);

            return new ProductsController(fakeProductService, fakeExternalProductService,
                fakeDialogService, fakeMapper, fakePrincipleService);
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