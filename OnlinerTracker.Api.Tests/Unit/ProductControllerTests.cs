using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using NUnit.Framework;
using OnlinerTracker.Api.ApiViewModels;
using OnlinerTracker.Api.Controllers;
using OnlinerTracker.Api.Models.Configs;
using OnlinerTracker.Api.Tests.Base;
using OnlinerTracker.Data;
using OnlinerTracker.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace OnlinerTracker.Api.Tests.Unit
{
    [TestFixture]
    public class ProductControllerTests : TestsClassBase
    {

        [Test]
        public async Task Follow_IfProductAlreadyBeingTracked_ThenShouldCallDialogServiseWithWarningMessage()
        {
            ProductsControllerConfig fakeConfig;
            ProductsController testedController = ControllersFactory.GetProductController(out fakeConfig);
            IDialogService mockDialogService = fakeConfig.DialogService;
            fakeConfig.ProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Returns(new Product());
            
            await testedController.Follow(new ProductFollowModel());

            mockDialogService.Received()
                .SendInPopupForUser(
                    Arg.Is<PopupType>(popupType => popupType == PopupType.Warning),
                    Arg.Is<string>(message => message.Contains("This product is already being tracked!")),
                    Arg.Any<string>()
                );
        }

        [Test]
        public async Task Follow_IfProductAreFollow_ThenShouldCallDialogServiceWithSuccessMessage()
        {
            ProductsControllerConfig fakeConfig;
            ProductsController testedController = ControllersFactory.GetProductController(out fakeConfig);
            IDialogService mockDialogService = fakeConfig.DialogService;
            fakeConfig.ProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).ReturnsNull();

            await testedController.Follow(new ProductFollowModel());

            mockDialogService.Received()
                .SendInPopupForUser(
                    Arg.Is<PopupType>(popupType => popupType == PopupType.Success),
                    Arg.Is<string>(message => message.Contains("Now you are follow")),
                    Arg.Any<string>()
                );
        }

        [Test]
        public async Task Follow_IfThrowException_ThenShouldCallDialogServiceWithErrorMessage()
        {
            ProductsControllerConfig fakeConfig;
            ProductsController testedController = ControllersFactory.GetProductController(out fakeConfig);
            IDialogService mockDialogService = fakeConfig.DialogService;
            fakeConfig.ProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Throws(new Exception());

            await testedController.Follow(new ProductFollowModel());

            mockDialogService.Received()
                .SendInPopupForUser(
                    Arg.Is<PopupType>(popupType => popupType == PopupType.Error),
                    Arg.Is<string>(message => message.Contains("ERROR!")),
                    Arg.Any<string>()
                );
        }

        [Test]
        public async Task Follow_IfSuccessful_ThenShouldReturnOkHttpStatusWithOkMessage()
        {
            ProductsControllerConfig fakeConfig;
            ProductsController testedController = ControllersFactory.GetProductController(out fakeConfig);
            fakeConfig.ProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).ReturnsNull();

            var result = await testedController.Follow(new ProductFollowModel()) as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result);
            Assert.AreEqual("OK", result.Content);
        }

        [Test]
        public async Task Follow_IfProductAlreadyBeingTracked_ThenShouldReturnOkHttpStatusWithDuplicateMessage()
        {
            ProductsControllerConfig fakeConfig;
            ProductsController testedController = ControllersFactory.GetProductController(out fakeConfig);
            fakeConfig.ProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Returns(new Product());

            var result = await testedController.Follow(new ProductFollowModel()) as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result);
            Assert.AreEqual("Duplicate", result.Content);
        }

        [Test]
        public async Task Follow_IfThrowException_ThenShouldReturnExceptionResult()
        {
            ProductsControllerConfig fakeConfig;
            ProductsController testedController = ControllersFactory.GetProductController(out fakeConfig);
            fakeConfig.ProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Throws(new Exception());

            var result = await testedController.Follow(new ProductFollowModel()) as ExceptionResult;

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Exception.Message);
        }
    }
}