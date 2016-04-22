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
    public class ProductControllerTests : TestsClassBase
    {

        [Test]
        public async Task Follow_IfProductAlreadyBeingTracked_ThenShouldCallDialogServiseWithWarningMessage()
        {
            IProductService stubProductService;
            IDialogService mockDialogService;
            ProductsController testedController = ControllersFactory.GetProductController(out stubProductService, out mockDialogService);
            stubProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Returns(new Product());
            
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
            IProductService stubProductService;
            IDialogService mockDialogService;
            ProductsController testedController = ControllersFactory.GetProductController(out stubProductService, out mockDialogService);
            stubProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).ReturnsNull();

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
            IProductService stubProductService;
            IDialogService mockDialogService;
            ProductsController testedController = ControllersFactory.GetProductController(out stubProductService, out mockDialogService);
            stubProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Throws(new Exception());

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
            IProductService stubProductService;
            ProductsController testedController = ControllersFactory.GetProductController(out stubProductService);
            stubProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).ReturnsNull();

            var result = await testedController.Follow(new ProductFollowModel()) as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result);
            Assert.AreEqual("OK", result.Content);
        }

        [Test]
        public async Task Follow_IfProductAlreadyBeingTracked_ThenShouldReturnOkHttpStatusWithDuplicateMessage()
        {
            IProductService stubProductService;
            ProductsController testedController = ControllersFactory.GetProductController(out stubProductService);
            stubProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Returns(new Product());

            var result = await testedController.Follow(new ProductFollowModel()) as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result);
            Assert.AreEqual("Duplicate", result.Content);
        }

        [Test]
        public async Task Follow_IfThrowException_ThenShouldReturnExceptionResult()
        {
            IProductService stubProductService;
            ProductsController testedController = ControllersFactory.GetProductController(out stubProductService);
            stubProductService.GetBy(Arg.Any<string>(), Arg.Any<Guid>()).Throws(new Exception());

            var result = await testedController.Follow(new ProductFollowModel()) as ExceptionResult;

            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result.Exception.Message);
        }
    }
}