using NSubstitute;
using NUnit.Framework;
using OnlinerTracker.Data;
using OnlinerTracker.Services.BaseServices;
using System;

namespace OnlinerTracker.Services.Tests.Unit.BaseServices
{
    [TestFixture]
    public class MessageSenderTests
    {
        [Test]
        public void SendEmail_SetNotConvertableToIntSmtpPortString_ShouldThrowException()
        {
            var stubConfig = Substitute.For<MessageSenderConfig>();
            stubConfig.SmtpPortString = "MUSTBECONVERTABLETOINT";
            var service = new MessageSender(stubConfig);
            
            var ex = Assert.Throws<FormatException>(() => service.SendEmail(string.Empty, string.Empty, string.Empty));

            StringAssert.Contains("Input string was not in a correct format", ex.Message);
        }
    }
}
