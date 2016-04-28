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
    public class ProductsControllerTests : TestsClassBase
    {

        [Test]
        public async Task Follow_IfProductAlreadyBeingTracked_ThenShouldCallDialogServiseWithWarningMessage()
        {
            ProductsControllerConfig fakeConfig;
            ProductsController controller = ControllersFactory.GetProductsController(out fakeConfig);
            IDialogService mockDialogService = fakeConfig.DialogService;
            fakeConfig.ProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Returns(new Product());
            
            await controller.Follow(new ProductFollowModel());

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
            ProductsController controller = ControllersFactory.GetProductsController(out fakeConfig);
            IDialogService mockDialogService = fakeConfig.DialogService;
            fakeConfig.ProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).ReturnsNull();

            await controller.Follow(new ProductFollowModel());

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
            ProductsController controller = ControllersFactory.GetProductsController(out fakeConfig);
            IDialogService mockDialogService = fakeConfig.DialogService;
            fakeConfig.ProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Throws(new Exception());

            await controller.Follow(new ProductFollowModel());

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
            ProductsController controller = ControllersFactory.GetProductsController(out fakeConfig);
            fakeConfig.ProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).ReturnsNull();

            var result = await controller.Follow(new ProductFollowModel()) as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result);
            Assert.AreEqual("OK", result.Content);
        }

        [Test]
        public async Task Follow_IfProductAlreadyBeingTracked_ThenShouldReturnOkHttpStatusWithDuplicateMessage()
        {
            ProductsControllerConfig fakeConfig;
            ProductsController controller = ControllersFactory.GetProductsController(out fakeConfig);
            fakeConfig.ProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Returns(new Product());

            var result = await controller.Follow(new ProductFollowModel()) as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result);
            Assert.AreEqual("Duplicate", result.Content);
        }

        [Test]
        public async Task Follow_IfThrowException_ThenShouldReturnExceptionResult()
        {
            ProductsControllerConfig fakeConfig;
            ProductsController controller = ControllersFactory.GetProductsController(out fakeConfig);
            fakeConfig.ProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Throws(new Exception());

            var result = await controller.Follow(new ProductFollowModel()) as ExceptionResult;

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Exception.Message);
        }

        [TestCase(true, PopupType.Success, "tracking is started!")]
        [TestCase(false, PopupType.Warning, "tracking is stopped!")]
        public void ChangeTrackingStatus_ChangeProductTrackingStatus_ShouldCallDialogService(bool trackingStatus, PopupType popupType, string dialogMessagePart)
        {
            ProductsControllerConfig fakeConfig;
            ProductsController controller = ControllersFactory.GetProductsController(out fakeConfig);
            fakeConfig.ProductService.GetById(Arg.Any<Guid>()).Returns(new Product());
            IDialogService mockDialogService = fakeConfig.DialogService;

            controller.ChangeTrackingStatus(Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a"), trackingStatus);

            mockDialogService.Received().SendInPopupForUser(popupType, Arg.Is<string>(u => u.Contains(dialogMessagePart)), Arg.Any<string>());
        }

        [TestCase(true, PopupType.Success, "add in comare")]
        [TestCase(false, PopupType.Warning, "remove from comare")]
        public void ChangeComparedStatus_ChangeProductComparedStatus_ShouldCallDialogService(bool comparedStatus, PopupType popupType, string dialogMessagePart)
        {
            ProductsControllerConfig fakeConfig;
            ProductsController controller = ControllersFactory.GetProductsController(out fakeConfig);
            fakeConfig.ProductService.GetById(Arg.Any<Guid>()).Returns(new Product());
            IDialogService mockDialogService = fakeConfig.DialogService;

            controller.ChangeComparedStatus(Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a"), comparedStatus);

            mockDialogService.Received().SendInPopupForUser(popupType, Arg.Is<string>(u => u.Contains(dialogMessagePart)), Arg.Any<string>());
        }
    }
}