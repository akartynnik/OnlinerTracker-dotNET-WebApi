using AutoMapper;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using OnlinerTracker.Api.ApiViewModels;
using OnlinerTracker.Api.Controllers;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace OnlinerTracker.Api.Tests
{
    [TestFixture]
    public class ProductControllerTests
    {
        private ProductsController _testedController;
        private IProductService _fakeProductService;
        private IDialogService _fakeDialogService;
        private IPrincipalService _fakePrincipleService;

        [SetUp]
        public void Setup()
        {
            _fakePrincipleService = Substitute.For<IPrincipalService>();
            _fakePrincipleService.GetSessionUser().Returns(new Principal(null)
            {
                Id = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a"),
                DialogConnectionId = "000",
            });

            var fakeExternalPproductService = Substitute.For<IExternalProductService>();
            _fakeDialogService = Substitute.For<IDialogService>();
            _fakeProductService = Substitute.For<IProductService>();

            var fakeMapper = Substitute.For<IMapper>();
            fakeMapper.Map<ProductFollowModel, Product>(new ProductFollowModel()).ReturnsForAnyArgs(new Product() {Name = "ProductName"});
            fakeMapper.Map<ProductFollowModel, Cost>(new ProductFollowModel()).ReturnsForAnyArgs(new Cost());

            _testedController = new ProductsController(_fakeProductService, fakeExternalPproductService,
                _fakeDialogService, fakeMapper, _fakePrincipleService);
        }

        [Test]
        public async Task Follow_IfProductIsDublicateCallDialogServise_WithCorrectWarningMessage()
        {
            _fakeProductService.GetBy(string.Empty, Guid.Empty).ReturnsForAnyArgs(new Product());

            await _testedController.Follow(new ProductFollowModel());

            _fakeDialogService.Received()
                .SendInPopupForUser(PopupType.Warning, "This product is already being tracked!", _fakePrincipleService.GetSessionUser().DialogConnectionId);
        }

        [Test]
        public async Task Follow_IfSuccessfulCallDialogService_WithCorrectSuccessMessage()
        {
            _fakeProductService.GetBy(string.Empty, Guid.Empty).ReturnsNullForAnyArgs();

            await _testedController.Follow(new ProductFollowModel());

            _fakeDialogService.Received()
                .SendInPopupForUser(PopupType.Success, "Now you are follow <b>ProductName</b>", _fakePrincipleService.GetSessionUser().DialogConnectionId);
        }


        [Test]
        public async Task Follow_IfThrowExceptionCallDialogService_WithCorrectErrorMessage()
        {
            _fakeProductService.GetBy(string.Empty, Guid.Empty).ThrowsForAnyArgs(new Exception());

            await _testedController.Follow(new ProductFollowModel());

            _fakeDialogService.Received()
                .SendInPopupForUser(PopupType.Error, "ERROR!", _fakePrincipleService.GetSessionUser().DialogConnectionId);
        }
    }

    public static class FakeHttpFactory
    {
        public static HttpContext GetHttpContext()
        {
            return GetHttpContext(Guid.Empty, string.Empty);
        }

        public static HttpContext GetHttpContext(Guid userId, string dialogConnectionId)
        {
            var httpRequest = new HttpRequest("", "http://test/", "");
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                new HttpStaticObjectsCollection(), 10, true,
                HttpCookieMode.AutoDetect,
                SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof (HttpSessionState).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, CallingConventions.Standard,
                new[] {typeof (HttpSessionStateContainer)},
                null)
                .Invoke(new object[] {sessionContainer});
            httpContext.User = new Principal(null) {Id = userId, DialogConnectionId = dialogConnectionId};
            return httpContext;
        }
    }
}