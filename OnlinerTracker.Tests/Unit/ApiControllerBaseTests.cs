using NUnit.Framework;
using OnlinerTracker.Api.Controllers;
using OnlinerTracker.Api.Tests.Base;
using System.Web.Http.Results;

namespace OnlinerTracker.Api.Tests.Unit
{
    [TestFixture]
    public class ApiControllerBaseTests : BaseTestsClass
    {
        [Test]
        public void Successful_ReturnValue_ShouldBeOkHttpStatusWithOkMessage()
        {
            var testedController = new ApiControllerBase();

            var result = testedController.Successful() as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result); 
            Assert.AreEqual("OK", result.Content);
        }

        [Test]
        public void Duplicate_ReturnValue_ShouldBeOkHttpStatusWithDuplicateMessage()
        {
            var testedController = new ApiControllerBase();

            var result = testedController.Duplicate() as OkNegotiatedContentResult<string>;

            Assert.IsNotNull(result);
            Assert.AreEqual("Duplicate", result.Content);
        }

        [Test]
        public void User_ReturnValue_ShouldBeNotEmptyWithUserIdLikeSessionsUserId()
        {
            var stubPrincipalService = GetStubForPrincipalService();
            var testedController = new ApiControllerBase(stubPrincipalService);

            var result = testedController.User;

            Assert.IsNotNull(result);
            Assert.AreEqual(stubPrincipalService.GetSessionUser().Id, result.Id);
        }
    }
}
