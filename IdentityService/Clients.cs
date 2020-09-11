using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServerTesting
{
    internal class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "oauthClient",
                    ClientName = "Example client application using client credentials",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret> {new Secret("Secret".Sha256())}, // change me!
                    AllowedScopes = new List<string> {"api1.write", "api1.read", "role"},
                    AlwaysIncludeUserClaimsInIdToken = true
                },

                new Client
                {
                    ClientId = "oidcClientApi",
                    ClientName = "Example Client Application",
                    ClientSecrets = new List<Secret> {new Secret("Secret".Sha256())}, // change me!
    
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = new List<string> {"https://localhost:44304/signin-oidc"},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "role",
                        "api1.read",
                        "api1",
                        "role"
                    },
                    
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequirePkce = true,
                    AllowPlainTextPkce = false
                }
            };
        }
    }
}
