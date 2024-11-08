using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    public AuthController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }    

    //This method was going to be used to authenticate the user and return a JWT token. 
    //It actually returns a token but I couldn't get the JWT to work with the API.
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] Login login)
    {
        var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Username == login.Username && c.Password == login.Password);
        if (customer == null)
        {
            return Unauthorized();
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("ThisIsMyTestSecretKeyThatIWillUseForTheRapidPayAPI");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, login.Username)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { Token = tokenString });
    }    
}