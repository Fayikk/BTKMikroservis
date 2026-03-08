using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ApiGateway.Authorization
{
    public class ResourceOwnerRequirement : IAuthorizationRequirement
    {
        
    }
     public class ResourceOwnerHandler : AuthorizationHandler<ResourceOwnerRequirement, string>
    {
        private readonly ILogger<ResourceOwnerHandler> _logger;

        public ResourceOwnerHandler(ILogger<ResourceOwnerHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ResourceOwnerRequirement requirement,
            string resourceOwnerId)
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userRole = context.User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var userName = context.User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;

            if (userRole == "Admin")
            {
                _logger.LogInformation(
                    "Admin user {UserName} granted access to resource owned by {ResourceOwnerId}",
                    userName, resourceOwnerId);
                
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if (userId == resourceOwnerId)
            {
                _logger.LogInformation(
                    "User {UserName} granted access to own resource",
                    userName);
                
                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning(
                    "User {UserName} (ID: {UserId}) denied access to resource owned by {ResourceOwnerId}",
                    userName, userId, resourceOwnerId);
            }

            return Task.CompletedTask;
        }
    }

}