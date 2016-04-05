using OnlinerTracker.Core;
using OnlinerTracker.Data.SecurityModels;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace OnlinerTracker.Security
{
    public class SecurityInitializer : DropCreateDatabaseIfModelChanges<SecurityContext>
    {
        protected override void Seed(SecurityContext context)
        {
            if (context.Clients.Any())
            {
                return;
            }
            var clients = new List<Client>
            {
                new Client
                {
                    Id = "onlinerTrackerWebUI",
                    Secret= HashGenerator.GetHash("secret"),
                    Name="Onliner Tracker Web UI",
                    ApplicationType =  ApplicationType.JavaScript,
                    Active = true,
                    RefreshTokenLifeTime = 7200,
                    AllowedOrigin = "http://localhost:56366"
                }
            };
            context.Clients.AddRange(clients);
            context.SaveChanges();
        }
    }
}
