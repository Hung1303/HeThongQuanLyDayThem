using BusinessObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories.Interfaces;
using Services.DTO;
using Services.Interfaces;
using System.Security.Claims;
using Core.Base;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;

        public AuthController(IUnitOfWork unitOfWork, IUserService userService, ITokenService tokenService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        [NonAction]
        public TokenDTO GenerateToken(User user, string? RT)
        {
            var claims = new List<Claim>
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("UserName", user.Username),
                new Claim("Email", user.Email),
                new Claim("Role", user.UserRole.ToString()),
                new Claim("PhoneNumber", user.PhoneNumber ?? ""),
                new Claim("FullName", user.Fullname)
            };

            switch (user.UserRole)
            {
                case Role.Teacher:
                    var teacher = _unitOfWork.GetRepository<TeacherProfile>().Entities
                                    .FirstOrDefault(t => t.UserId == user.Id);
                    if(teacher != null) claims.Add(new Claim("TeacherProfileId", teacher.Id.ToString())); 
                    break;

                case Role.Center:
                    var center = _unitOfWork.GetRepository<CenterProfile>().Entities
                                    .FirstOrDefault(t => t.UserId == user.Id);
                    if (center != null) claims.Add(new Claim("CenterProfileId", center.Id.ToString()));
                    break;
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: credentials);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new TokenDTO()
            {
                AccessToken = accessToken,
                RefreshToken = RT ?? GenerateRefreshToken(user),
                ExpiredAt = _tokenService.GetRefreshTokenByUserID(user.Id).ExpiredTime
            };
        }

        [NonAction]
        public string GenerateRefreshToken(User user)
        {
            var randomnumber = new byte[32];
            using (var randomnumbergenerator = RandomNumberGenerator.Create())
            {
                randomnumbergenerator.GetBytes(randomnumber);
                string refreshtoken = Convert.ToBase64String(randomnumber);

                var refreshTokenEntity = new Token
                {
                    UserId = user.Id,
                    AccessToken = new Random().Next().ToString(),
                    RefreshToken = refreshtoken,
                    ExpiredTime = DateTime.Now.AddDays(7),
                    Status = 1
                };

                _tokenService.GenerateRefreshToken(refreshTokenEntity);
                return refreshtoken;
            }
        }


        [HttpPost]
        [Route("Login")]
        public IActionResult Login(string username, string password)
        {
            var user = _userService.Login(username).Result;

            if(user == null)
            {
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = "Incorrect email or password.",
                    Data = null
                });
            }

            if (user.IsDeleted)
            {
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = "Account has been deleted.",
                    Data = null
                });
            }

            if (user.AccountStatus != AccountStatus.Active)
            {
                return BadRequest(new ResultDTO
                {
                    IsSuccess = false,
                    Message = "Account has not been activated.",
                    Data = null
                });
            }

            if(user.Password == password)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                };

                //Reset refresh token (if there is one)
                _tokenService.ResetRefreshToken();

                var token = GenerateToken(user, null);

                return Ok(new ResultDTO
                {
                    IsSuccess = true,
                    Message = "Login successfully.",
                    Data = token
                });
            }

            return BadRequest(new ResultDTO
            {
                IsSuccess = false,
                Message = "Login failed.",
                Data = null
            });
        }



        [HttpGet("WhoAmI")]
        public IActionResult WhoAmI()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized(new { Message = "Unathorized user." });
            }

            try
            {
                var userIdClaim = User.FindFirst("UserId");
                var emailClaim = User.FindFirst("Email");
                var usernameClaim = User.FindFirst("UserName");
                var fullNameClaim = User.FindFirst("FullName");
                var phoneClaim = User.FindFirst("PhoneNumber");
                var roleClaim = User.FindFirst("Role");
                var statusClaim = User.FindFirst("AccountStatus");

                var teacherProfileClaim = User.FindFirst("TeacherProfileId");
                var centerProfileClaim = User.FindFirst("CenterProfileId");

                if (userIdClaim == null || emailClaim == null || usernameClaim == null || roleClaim == null)
                {
                    return Unauthorized(new { Message = "Missing important information in claim." });
                }

                Role role = Enum.TryParse(roleClaim.Value, out Role parsedRole) ? parsedRole : Role.Admin;

                AccountStatus status = Enum.TryParse(statusClaim?.Value, out AccountStatus parsedStatus) ? parsedStatus : AccountStatus.Active;

                string? profileId = teacherProfileClaim?.Value ?? centerProfileClaim?.Value;

                var userInfo = new Dictionary<string, object?>
                {
                    { "Id", userIdClaim.Value },
                    { "Email", emailClaim.Value },
                    { "UserName", usernameClaim.Value },
                    { "FullName", fullNameClaim?.Value },
                    { "PhoneNumber", phoneClaim?.Value },
                    { "Role", role.ToString() },
                    { "Status", status.ToString() },
                    { "TeacherProfileId", teacherProfileClaim?.Value },
                    { "CenterProfileId", centerProfileClaim?.Value }
                };

                //Filter out any key with null value
                var filteredInfo = userInfo.Where(kv => kv.Value != null)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                return Ok(filteredInfo);
            }
            catch(Exception e)
            {
                return BadRequest(new
                {
                    Message = "Error in retrieving user info.",
                    Detail = e.Message
                });
            }
        }
    }
}
