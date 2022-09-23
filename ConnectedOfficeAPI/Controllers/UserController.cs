using ConnectedOfficeAPI.Handler;
using ConnectedOfficeAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ConnectedOfficeAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ConnectedOfficeContexts _DBContext;
        private JwtSettings jwtSettings;
        private readonly IRefereshTokenGenerator refereshTokenGenerator;
        public UserController(ConnectedOfficeContexts _DBContext, IOptions<JwtSettings> options, IRefereshTokenGenerator referesh)
        {
            this._DBContext = _DBContext;
            this.refereshTokenGenerator = referesh;
            this.jwtSettings = options.Value;
        }
        [NonAction]
        public async Task<TokenResponse> TokenAuthenticate(string user, Claim[] claims)
        {
            var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.securitykey)), SecurityAlgorithms.HmacSha256)
            );
            var jwttoken = new JwtSecurityTokenHandler().WriteToken(token);
            return new TokenResponse() { jwttoken = jwttoken, refreshtoken = await refereshTokenGenerator.GenerateToken(user) };
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Authenticate([FromBody] UserCred userCred)
        {
            var user = await this._DBContext.User.FirstOrDefaultAsync(item => item.UserId == userCred.username && item.Password == userCred.password);
            if (user == null)
                return Unauthorized();
            /// Generate Token
            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securitykey);
            var tokendesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                new Claim[] { new Claim(ClaimTypes.Name, user.UserId), new Claim(ClaimTypes.Role, user.Role) }
                ),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenhandler.CreateToken(tokendesc);
            string finaltoken = tokenhandler.WriteToken(token);
            var response = new TokenResponse() { jwttoken = finaltoken, refreshtoken = await refereshTokenGenerator.GenerateToken(userCred.username) };
            return Ok(response);
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefToken([FromBody] TokenResponse tokenResponse)
        {

            /// Generate Token
            var tokenhandler = new JwtSecurityTokenHandler();
            var tokenkey = Encoding.UTF8.GetBytes(this.jwtSettings.securitykey);
            SecurityToken securityToken;
            var principal = tokenhandler.ValidateToken(tokenResponse.jwttoken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(tokenkey),
                ValidateIssuer = false,
                ValidateAudience = false,

            }, out securityToken);
            var token = securityToken as JwtSecurityToken;
            if (token != null && !token.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                return Unauthorized();
            }
            var username = principal.Identity?.Name;
            var user = await this._DBContext.Refreshtoken.FirstOrDefaultAsync(item => item.UserId == username && item.RefreshToken == tokenResponse.refreshtoken);
            if (user == null)
                return Unauthorized();
            var response = TokenAuthenticate(username, principal.Claims.ToArray()).Result;
            return Ok();
        }

        [Authorize(Roles = "admin")]
        [HttpPost("CreateUser")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (_DBContext.User == null)
            {
                return Problem("Entity set 'ConnectedOfficeContexts.Users'  is null.");
            }
            _DBContext.User.Add(user);
            try
            {
                await _DBContext.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.UserId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpPut("UpdateUser/{UserId}")]
        public async Task<IActionResult> PutUser(string UserId, User user)
        {
            if (UserId != user.UserId)
            {
                return BadRequest();
            }

            _DBContext.Entry(user).State = EntityState.Modified;

            try
            {
                await _DBContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Authorize(Roles = "admin")]
        [HttpDelete("RemoveUser/{UserId}")]
        public async Task<IActionResult> DeleteUser(string UserId)
        {
            if (_DBContext.User == null)
            {
                return NotFound();
            }
            var user = await _DBContext.User.FindAsync(UserId);
            if (user == null)
            {
                return NotFound();
            }

            _DBContext.User.Remove(user);
            await _DBContext.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(string id)
        {
            return (_DBContext.User?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
