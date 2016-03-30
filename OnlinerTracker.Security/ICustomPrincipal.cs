using System;
using System.Security.Principal;

namespace OnlinerTracker.Security
{
    public interface ICustomPrincipal : IPrincipal
    {
        Guid Id { get; }
    }
}
