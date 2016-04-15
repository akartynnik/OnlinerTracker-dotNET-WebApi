using NUnit.Framework;
using OnlinerTracker.Api.Controllers;
using System.Web.Http.Results;

namespace OnlinerTracker.UnitTests
{
    [TestFixture]
    public class ApiControllerBaseTests
    {
        [Test]
        public void Successful_CheckCustomOkHttpStatusCode_ShouldReturnOkText()
        {
            var controller = new ApiControllerBase();

            var result =  controller.Successful() as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result);
            Assert.AreEqual("OK", result.Content);
        }

        [Test]
        public void Duplicate_CheckCustomOkHttpStatusCode__ShouldReturnDuplicateText()
        {
            var controller = new ApiControllerBase();

            var result = controller.Duplicate() as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result);
            Assert.AreEqual("Duplicate", result.Content);
        }
    }
}
