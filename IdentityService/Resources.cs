using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerTesting
{
    internal class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = "role",
                    UserClaims = new List<string> {"role"}
                },
                new IdentityResource
                {
                    Name = "permission",
                    UserClaims = new List<string> {"permission"}
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new[]
            {
                new ApiResource
                {
                    Name = "doctorsPracticeApi",
                    DisplayName = "Doctors Practice Api",
                    Description = "Allow the application to access Doctors Practice API on your behalf",
                    Scopes = new List<string>{"api1.read","api1.write"},
                    ApiSecrets = new List<Secret>{new Secret("Secret".Sha256())},
                    UserClaims = new List<string>{"role", "permission"}
                }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope("api1.read", "Read Access to Api #1"),
                new ApiScope("api1.write", "Write Access to Api #1")
            };
        }
    }
}
