using OnlinerTracker.Core;
using OnlinerTracker.Data;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

namespace OnlinerTracker.Security.Migrations
{
    internal sealed class SecurityContextConfiguration : DbMigrationsConfiguration<SecurityContext>
    {
        public SecurityContextConfiguration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SecurityContext context)
        {
            if (context.Clients.Any())
            {
                return;
            }

            context.Clients.AddRange(BuildClientsList());
            context.SaveChanges();
        }

        private static List<Client> BuildClientsList()
        {

            List<Client> ClientsList = new List<Client>
            {
                new Client
                { Id = "onlinerTrackerWebUI",
                    Secret= Helper.GetHash("secret"),
                    Name="Onliner Tracker Web UI",
                    ApplicationType =  ApplicationTypes.JavaScript,
                    Active = true,
                    RefreshTokenLifeTime = 7200,
                    AllowedOrigin = "http://localhost:56366"
                }
            };

            return ClientsList;
        }
    }
}
