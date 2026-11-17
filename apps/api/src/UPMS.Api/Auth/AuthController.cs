using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UPMS.Api.Data;

namespace UPMS.Api.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<AppUser> _users;
    private readonly JwtService _jwt;
    private readonly AppDbContext _db;

    public AuthController(UserManager<AppUser> users, JwtService jwt, AppDbContext db)
    {
        _users = users;
        _jwt = jwt;
        _db = db;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Email and password are required." });

        if (await _users.FindByEmailAsync(email) is not null)
            return Conflict(new { message = "An account with this email already exists." });

        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FullName = request.FullName.Trim(),
            PhoneNumber = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim(),
            EmailConfirmed = true
        };

        var result = await _users.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        await _users.AddToRoleAsync(user, "Passenger");
        return await BuildResponse(user);
    }

    [HttpPost("register-vendor")]
    public async Task<ActionResult<AuthResponse>> RegisterVendor(RegisterVendorRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var organizationName = request.OrganizationName.Trim();
        var contactPerson = request.ContactPerson.Trim();
        var phone = string.IsNullOrWhiteSpace(request.Phone) ? null : request.Phone.Trim();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = "Email and password are required." });
        if (string.IsNullOrWhiteSpace(organizationName))
            return BadRequest(new { message = "Organization name is required." });
        if (string.IsNullOrWhiteSpace(contactPerson))
            return BadRequest(new { message = "Contact person is required." });

        if (await _users.FindByEmailAsync(email) is not null)
            return Conflict(new { message = "An account with this email already exists." });

        await using var transaction = await _db.Database.BeginTransactionAsync();
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            FullName = contactPerson,
            PhoneNumber = phone,
            EmailConfirmed = true
        };

        var result = await _users.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return BadRequest(new { message = string.Join(" ", result.Errors.Select(e => e.Description)) });

        var roleResult = await _users.AddToRoleAsync(user, "Vendor");
        if (!roleResult.Succeeded)
            return BadRequest(new { message = string.Join(" ", roleResult.Errors.Select(e => e.Description)) });

        _db.Vendors.Add(new Vendor
        {
            Name = organizationName,
            Status = VendorStatus.Pending,
            UserId = user.Id,
            Contact = phone,
            MonthlySubscription = 0,
            SubscriptionStatus = SettlementStatus.Pending
        });
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return await BuildResponse(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var user = await _users.FindByEmailAsync(email);
        if (user is null || !await _users.CheckPasswordAsync(user, request.Password))
            return Unauthorized(new { message = "Invalid email or password." });

        return await BuildResponse(user);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult> Me()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(id, out var userId))
            return Unauthorized();

        var user = await _users.FindByIdAsync(userId.ToString());
        if (user is null)
            return Unauthorized();

        var roles = await _users.GetRolesAsync(user);
        return Ok(new { user.Id, user.Email, user.FullName, roles });
    }

    private async Task<AuthResponse> BuildResponse(AppUser user)
    {
        var roles = await _users.GetRolesAsync(user);
        var token = _jwt.CreateToken(user, roles);
        return new AuthResponse(token, user.Id, user.Email ?? string.Empty, user.FullName, roles.ToList());
    }
}
