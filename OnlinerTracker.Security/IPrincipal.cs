using System;

namespace OnlinerTracker.Security
{
    public interface IPrincipal : System.Security.Principal.IPrincipal
    {
        Guid Id { get; }
    }
}
