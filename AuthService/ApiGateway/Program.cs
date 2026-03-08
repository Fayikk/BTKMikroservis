
using System.Text;
using ApiGateway.Authorization;
using ApiGateway.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient<ProxyService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// JWT Authentication
var jwtSecret = builder.Configuration["JWT_SECRET"] ?? "SuperSecretKeyForJWTTokenAuth12345!";
var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? "ServiceAuthDemo";
var jwtAudience = builder.Configuration["JWT_AUDIENCE"] ?? "MicroserviceClients";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddSingleton<IAuthorizationHandler,PermissionHandler>();
builder.Services.AddSingleton<IAuthorizationHandler,ResourceOwnerHandler>();


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyNames.RequireAdminRole, policy => 
        policy.RequireRole("Admin"));
    
    options.AddPolicy(PolicyNames.RequireUserRole, policy => 
        policy.RequireRole("User", "Admin"));
    
    options.AddPolicy(PolicyNames.RequireManagerRole, policy => 
        policy.RequireRole("Manager", "Admin"));

    

    options.AddPolicy(PolicyNames.CanViewOwnOrders, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.ViewOwnOrders)));
    
    options.AddPolicy(PolicyNames.CanViewAllOrders, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.ViewAllOrders)));
    
    options.AddPolicy(PolicyNames.CanCreateOrder, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.CreateOrder)));
    
    options.AddPolicy(PolicyNames.CanUpdateOrderStatus, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.UpdateOrderStatus)));
    
    options.AddPolicy(PolicyNames.CanCancelOrder, policy =>
        policy.Requirements.Add(new PermissionRequirement(Permissions.CancelOrder)));

    
});



var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GatewaySecretMiddleware>();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    service = "ApiGateway",
    timestamp = DateTime.UtcNow
}));

app.Run();
