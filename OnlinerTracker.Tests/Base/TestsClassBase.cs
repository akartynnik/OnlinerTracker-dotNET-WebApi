using OnlinerTracker.Data;
using OnlinerTracker.Security;
using System;

namespace OnlinerTracker.Api.Tests.Base
{
    public abstract class TestsClassBase
    {

        protected Currency DefaultCurrency => new Currency
        {
            Type = CurrencyType.USD,
            Value = 20000
        };

        protected Principal DefaultSessionUser => new Principal(null)
        {
            Id = TestsConstants.DefaultSessionUserId,
            DialogConnectionId = TestsConstants.DefaultDialogId,
        };
    }
}