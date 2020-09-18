using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers
{
    [ApiController]
    public class RolesController : ControllerBase
    {
        private RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        [Route("roles")]
        public IEnumerable<IdentityRole> GetIdentityRoles()
        {
            List<IdentityRole> roles = _roleManager.Roles.ToList();

            return roles;
        }

        [HttpGet]
        [Route("roles/{roleName}")]
        public IEnumerable<Claim> GetIdentityRoleClaims(string roleName)
        {
            var identityRole = GetIdentityRoleByName(roleName);

            if (identityRole != null)
            {
                return _roleManager.GetClaimsAsync(identityRole).Result.ToList();
            }
            else
            {
                return null;
            }
        }

        [HttpPost]
        [Route("roles/{roleName}")]
        public StatusCodeResult PostIdentityRoleClaim(string roleName, string claimType, string claimValue)
        {
            var identityRole = GetIdentityRoleByName(roleName);
            if (identityRole != null)
            {
                _roleManager.AddClaimAsync(identityRole, new Claim(claimType, claimValue)).Wait();
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete]
        [Route("roles/{roleName}/{claimValue}")]
        public StatusCodeResult DeleteIdentityRoleClaim(string roleName, string claimValue)
        {
            var identityRole = GetIdentityRoleByName(roleName);
            if (identityRole != null)
            {
                var roleClaims = _roleManager.GetClaimsAsync(identityRole).Result.ToList();
                foreach (var claim in roleClaims)
                {
                    if (claim.Value == claimValue)
                    {
                        _roleManager.RemoveClaimAsync(identityRole, claim).Wait();
                        return Ok();
                    }
                }
            }
            return BadRequest();
        }

        private IdentityRole GetIdentityRoleByName(string name)
        {
            var identityRoles = _roleManager.Roles.ToList();

            foreach (var identityRole in identityRoles)
            {
                if (identityRole.Name == name)
                {
                    return identityRole;
                }
            }
            return null;
        }
    }
}
