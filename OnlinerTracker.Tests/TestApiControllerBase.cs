using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnlinerTracker.Api.Controllers;
using System.Web.Http.Results;

namespace OnlinerTracker.Tests
{
    [TestClass]
    public class TestApiControllerBase
    {
        [TestMethod]
        public void Successful_ShouldReturnOkNegotiatedContentResultWithOkMessage()
        {
            var controller = new ApiControllerBase();
            var result =  controller.Successful() as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(result);
            Assert.AreEqual("OK", result.Content);
        }

        [TestMethod]
        public void Duplicate_ShouldReturnOkNegotiatedContentResultWithDuplicateMessage()
        {
            var controller = new ApiControllerBase();
            var result = controller.Duplicate() as OkNegotiatedContentResult<string>;
            Assert.IsNotNull(result);
            Assert.AreEqual("Duplicate", result.Content);
        }
    }
}
