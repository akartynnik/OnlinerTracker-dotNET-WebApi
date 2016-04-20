using NSubstitute;
using NUnit.Framework;
using OnlinerTracker.Api.Controllers;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System;
using System.Security.Principal;
using System.Web.Http.Results;

namespace OnlinerTracker.Api.Tests
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

        [Test]
        public void User_CheckIfUserIdExist_ShouldBeNotEmptyGuid()
        {
            var userId = Guid.NewGuid();
            var fakePrincipalService = Substitute.For<IPrincipalService>();
            fakePrincipalService.GetSessionUser().Returns(new Principal(null) { Id = userId});
            var controller = new ApiControllerBase(fakePrincipalService);

            var result = controller.User;
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.Id);
        }
    }
}
