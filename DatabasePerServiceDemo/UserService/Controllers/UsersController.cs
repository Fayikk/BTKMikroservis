using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Data;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
          private readonly UserDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

       [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        _logger.LogInformation("Getting all users from SQL Server database");
        return await _context.Users.ToListAsync();
    }


     [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        _logger.LogInformation("Creating new user: {UserEmail}", user.Email);
        
        user.CreatedAt = DateTime.UtcNow;
        _context.Users.Add(user);
        
        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("User created successfully with ID {UserId}", user.Id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error creating user");
            return Conflict(new { message = "User with this email already exists" });
        }

        return Ok(user);
    }
    }
}