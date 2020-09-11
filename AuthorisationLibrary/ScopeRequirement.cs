using Microsoft.AspNetCore.Authorization;
using System;

namespace AuthorisationLibrary
{
    public class ScopeRequirement : IAuthorizationRequirement
    {
        public string ScopeValue { get; }

        public ScopeRequirement(string scopeValue)
        {
            ScopeValue = scopeValue;
        }
    }
}
