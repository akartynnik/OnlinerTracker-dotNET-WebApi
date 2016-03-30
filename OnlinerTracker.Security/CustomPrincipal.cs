using System;
using System.Security.Principal;

namespace OnlinerTracker.Security
{
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return false; }

        public CustomPrincipal(IIdentity identity)
        {
            this.Identity = identity;
        }

        public Guid Id { get; set; }
    }
}
