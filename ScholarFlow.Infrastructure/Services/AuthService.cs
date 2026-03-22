using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ScholarFlow.Application.Common.Interfaces;
using ScholarFlow.Application.Common.Models;
using ScholarFlow.Domain.Entities;
using ScholarFlow.Domain.Enums;
using ScholarFlow.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ScholarFlow.Infrastructure.Services;

/// <summary>
/// JWT-based authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IApplicationDbContext _context;
    private readonly IEmailService _emailService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IConfiguration configuration,
        IApplicationDbContext context,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _context = context;
        _emailService = emailService;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(
        string email,
        string password,
        string role,
        string? fullName = null,
        Guid? subjectId = null,
        string? qualification = null,
        string? phoneNumber = null,
        string? bio = null)
    {
        // Check if user already exists
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            return Result<AuthResponse>.Failure("User with this email already exists");
        }

        // Create new user
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return Result<AuthResponse>.Failure($"Registration failed: {errors}");
        }

        // Assign role
        if (!string.IsNullOrEmpty(role))
        {
            // Normalize role name (capitalize first letter)
            var normalizedRole = char.ToUpper(role[0]) + role.Substring(1).ToLower();
            
            // Check if role exists, if not create it
            if (!await _roleManager.RoleExistsAsync(normalizedRole))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(normalizedRole));
            }
            
            await _userManager.AddToRoleAsync(user, normalizedRole);

            if (string.Equals(normalizedRole, "Teacher", StringComparison.OrdinalIgnoreCase))
            {
                if (!subjectId.HasValue)
                {
                    await _userManager.DeleteAsync(user);
                    return Result<AuthResponse>.Failure("Subject is required for teacher registration");
                }

                var subjectExists = await _context.Subjects.AnyAsync(s => s.Id == subjectId.Value);
                if (!subjectExists)
                {
                    await _userManager.DeleteAsync(user);
                    return Result<AuthResponse>.Failure("Selected subject does not exist");
                }

                var profile = new TeacherProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    FullName = fullName ?? string.Empty,
                    SubjectId = subjectId,
                    Qualification = qualification ?? string.Empty,
                    PhoneNumber = phoneNumber ?? string.Empty,
                    Bio = bio ?? string.Empty,
                    Status = TeacherRegistrationStatus.Pending,
                    TeacherCode = null,
                    RejectionReason = null,
                    ReviewedAt = null,
                };

                _context.TeacherProfiles.Add(profile);
                await _context.SaveChangesAsync();
            }
        }

        // Send welcome email (fire-and-forget, errors are logged inside service)
        await _emailService.SendWelcomeEmailAsync(email, fullName ?? email, role);

        // Generate token with all user details
        var token = await GenerateJwtToken(user);

        var requiresApproval = string.Equals(role, "Teacher", StringComparison.OrdinalIgnoreCase);
        return Result<AuthResponse>.Success(new AuthResponse
        {
            Token = token,
            RequiresApproval = requiresApproval,
            ApprovalStatus = requiresApproval ? TeacherRegistrationStatus.Pending.ToString() : null,
        });
    }

    public async Task<Result<AuthResponse>> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            return Result<AuthResponse>.Failure("Invalid email or password");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
        {
            return Result<AuthResponse>.Failure("Invalid email or password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Any(r => string.Equals(r, "Teacher", StringComparison.OrdinalIgnoreCase)))
        {
            var profile = await _context.TeacherProfiles
                .FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (profile == null)
            {
                return Result<AuthResponse>.Failure("Teacher profile not found. Please contact admin.");
            }

            if (profile.Status != TeacherRegistrationStatus.Accepted)
            {
                return Result<AuthResponse>.Failure("Your teacher account is pending admin approval.");
            }
        }

        // Generate token with all user details
        var token = await GenerateJwtToken(user);

        return Result<AuthResponse>.Success(new AuthResponse { Token = token });
    }

    private async Task<string> GenerateJwtToken(ApplicationUser user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"]!;
        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;
        var expiryInMinutes = int.Parse(jwtSettings["ExpiryInMinutes"]!);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await _userManager.GetRolesAsync(user);

        // Add all user details as claims in the token
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim("username", user.UserName!),
            new Claim("email", user.Email!)
        };

        // Add roles to claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim("role", role)); // Also add as custom claim for easier access
        }

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
