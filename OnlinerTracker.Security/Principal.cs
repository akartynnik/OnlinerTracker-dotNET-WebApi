using System;
using System.Security.Principal;

namespace OnlinerTracker.Security
{
    public class Principal : IPrincipal
    {
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role) { return false; }

        public Principal(IIdentity identity)
        {
            this.Identity = identity;
        }

        public Guid Id { get; set; }

        public string SignalRConnectionId { get; set; }
    }
}
