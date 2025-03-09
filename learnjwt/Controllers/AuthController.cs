using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using learnjwt.AppContext;
using learnjwt.Models;
using learnjwt.Models.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace learnjwt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : Controller
{
    private readonly MyDbContext _context;

    public AuthController(MyDbContext context)
    {
        _context = context;
    }


    [HttpPost("login")]
    public IActionResult Login([FromBody] DtoUserLogin user)
    {
        var checkusername = _context.Users.FirstOrDefault(u => u.Username == user.Username);
        if (checkusername == null)
            return Unauthorized(new
            {
                message = "Username or password wrong"
            });

        var passwordHasher = new PasswordHasher<object>();

        var checkPass = passwordHasher.VerifyHashedPassword(null, checkusername.Password, user.Password);

        if (checkPass == PasswordVerificationResult.Failed)
            return Unauthorized(new
            {
                message = "Username or password wrong"
            });
        var token = GenerateJwtToken(checkusername);
        return Ok(new { token });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] DtoUserRegister userRegister)
    {
        var user = new User();
        var passwordHasher = new PasswordHasher<object>();
        user.Username = userRegister.Username;
        user.Password = passwordHasher.HashPassword(null, userRegister.Password);
        user.Name = userRegister.Name;
        user.Address = userRegister.Address;

        _context.Users.Add(user);
        _context.SaveChanges();

        return Ok(new
        {
            status = "success"
        });
    }

    [HttpGet("userdata")]
    [Authorize]
    public IActionResult GetUserData()
    {
        var userId = User.FindFirst("UserID")?.Value;
        var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(userId));
        if (user != null)
            return Ok(new
            {
                status = "success",
                username = user.Username
            });

        return Unauthorized();
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("UserID", user.Id.ToString())
        };

        // Kunci minimal 32 karakter
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes("MySuperSecureJWTSecretKeyThatIsAtLeast32Characters"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            "your-app",
            "your-app-users",
            claims,
            // expires: DateTime.UtcNow.AddMinutes(1),
            expires: DateTime.UtcNow.AddMinutes(24 * 60),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}