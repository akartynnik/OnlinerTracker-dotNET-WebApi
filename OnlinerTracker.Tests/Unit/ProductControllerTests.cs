using AutoMapper;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using OnlinerTracker.Api.ApiViewModels;
using OnlinerTracker.Api.Controllers;
using OnlinerTracker.Api.Tests.Base;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace OnlinerTracker.Api.Tests.Unit
{
    [TestFixture]
    public class ProductControllerTests : BaseTestsClass
    {
        private ProductsController _testedController;
        private IProductService _stubProductService;
        private IDialogService _mockDialogService;
        private IPrincipalService _stubPrincipleService;

        [SetUp]
        public void Setup()
        {
            var stubExternalPproductService = Substitute.For<IExternalProductService>();
            var stubMapper = Substitute.For<IMapper>();
            _mockDialogService = Substitute.For<IDialogService>();
            _stubProductService = Substitute.For<IProductService>();
            _stubPrincipleService = GetStubForPrincipalService();

            stubMapper.Map<ProductFollowModel, Product>(new ProductFollowModel())
                .ReturnsForAnyArgs(new Product() {Name = "ProductName"});
            stubMapper.Map<ProductFollowModel, Cost>(new ProductFollowModel()).ReturnsForAnyArgs(new Cost());
            _testedController = new ProductsController(_stubProductService, stubExternalPproductService,
                _mockDialogService, stubMapper, _stubPrincipleService);
        }

        [Test]
        public async Task Follow_IfProductAlreadyBeingTracked_ThenShouldCallDialogServiseWithWarningMessage()
        {
            _stubProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Returns(new Product());

            await _testedController.Follow(new ProductFollowModel());

            _mockDialogService.Received()
                .SendInPopupForUser(
                    Arg.Is<PopupType>(popupType => popupType == PopupType.Warning),
                    Arg.Is<string>(message => message.Contains("This product is already being tracked!")),
                    Arg.Is<string>(
                        connectionId => connectionId.Contains(_stubPrincipleService.GetSessionUser().DialogConnectionId))
                );
        }

        [Test]
        public async Task Follow_IfSuccessful_ThenShouldCallDialogServiceWithSuccessMessage()
        {
            _stubProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).ReturnsNull();

            await _testedController.Follow(new ProductFollowModel());

            _mockDialogService.Received()
                .SendInPopupForUser(
                    Arg.Is<PopupType>(popupType => popupType == PopupType.Success),
                    Arg.Is<string>(message => message.Contains("Now you are follow <b>ProductName</b>")),
                    Arg.Is<string>(
                        connectionId => connectionId.Contains(_stubPrincipleService.GetSessionUser().DialogConnectionId))
                );
        }

        [Test]
        public async Task Follow_IfThrowException_ThenShouldCallDialogServiceWithErrorMessage()
        {
            _stubProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Throws(new Exception());

            await _testedController.Follow(new ProductFollowModel());

            _mockDialogService.Received()
                .SendInPopupForUser(
                    Arg.Is<PopupType>(popupType => popupType == PopupType.Error),
                    Arg.Is<string>(message => message.Contains("ERROR!")),
                    Arg.Is<string>(
                        connectionId => connectionId.Contains(_stubPrincipleService.GetSessionUser().DialogConnectionId))
                );
        }

        [Test]
        public async Task Follow_IfSuccessful_ThenShouldReturnOkHttpStatusWithOkMessage()
        {
            _stubProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).ReturnsNull();

            var result = await _testedController.Follow(new ProductFollowModel()) as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result);
            Assert.AreEqual("OK", result.Content);
        }

        [Test]
        public async Task Follow_IfProductAlreadyBeingTracked_ThenShouldReturnOkHttpStatusWithDuplicateMessage()
        {
            _stubProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Returns(new Product());

            var result = await _testedController.Follow(new ProductFollowModel()) as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result);
            Assert.AreEqual("Duplicate", result.Content);
        }

        [Test]
        public async Task Follow_IfThrowException_ThenShouldReturnExceptionResult()
        {
            _stubProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Throws(new Exception());

            var result = await _testedController.Follow(new ProductFollowModel()) as ExceptionResult;

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Exception.Message);
        }
    }
}