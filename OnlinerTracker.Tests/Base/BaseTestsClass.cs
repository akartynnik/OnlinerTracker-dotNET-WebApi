using NSubstitute;
using OnlinerTracker.Interfaces;
using OnlinerTracker.Security;
using System;

namespace OnlinerTracker.Api.Tests.Base
{
    public class BaseTestsClass
    {
        public IPrincipalService GetStubForPrincipalService()
        {
            var stub = Substitute.For<IPrincipalService>();
            stub.GetSessionUser().Returns(new Principal(null)
            {
                Id = Guid.Parse("eea2925f-a3fc-e511-bc42-50e549caaf5a"),
                DialogConnectionId = "000",
            });
            return stub;
        }
    }
}
