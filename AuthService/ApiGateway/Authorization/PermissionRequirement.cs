using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ApiGateway.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
         public string Permission { get; }

        public PermissionRequirement(string permission)
        {
            Permission = permission;
        }

    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
         private readonly ILogger<PermissionHandler> _logger;

        public PermissionHandler(ILogger<PermissionHandler> logger)
        {
            _logger = logger;
        }
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userRole = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userName = context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(userRole))
            {
                _logger.LogWarning("User {UserName} has no role claim", userName);
                return Task.CompletedTask;
            }

            var permissions = RolePermissions.GetPermissionsForRole(userRole);

            if (permissions.Contains(requirement.Permission))
            {
                _logger.LogInformation(
                    "User {UserName} with role {Role} has permission {Permission}",
                    userName, userRole, requirement.Permission);
                
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning(
                    "User {UserName} with role {Role} does not have permission {Permission}",
                    userName, userRole, requirement.Permission);
            }

            return Task.CompletedTask;
        }
    
    }
}