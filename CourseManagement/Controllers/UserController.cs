using CourseManagement.Common.Helper;
using CourseManagement.Models;
using CourseManagement.Models.DataTransferObject;
using CourseManagement.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CourseManagement.Controllers
{
    [Route("api/[controller]s")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _configuration;

        public UserController(DataContext _dataContext, IConfiguration configuration)
        {
            this._dataContext = _dataContext;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var hashedPass = PasswordHelper.HashPassword(model.Password);
            var user = _dataContext.Users.FirstOrDefault(u => u.UserName.Trim().ToLower() == model.Username.ToLower() && u.Password == hashedPass);
            if (user != null)
            {
                var userRoles = _dataContext.UserRoles.Where(r => r.UserId == user.UserId);
                var roles = _dataContext.Roles.Where(r => userRoles.Select(ur => ur.RoleId).Contains(r.RoleId));

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var role in roles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role.RoleName));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return NotFound("Username or password is incorrect.");
        }

        [HttpPost]
        [Route("register")]
        public IActionResult Register([FromBody] SignUpModel model)
        {
            var userExists = _dataContext.Users.FirstOrDefault(u => u.UserName.Trim().ToLower() == model.Username.ToLower() || u.Email.Trim().ToLower() == model.Email.ToLower());
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "UserName or Email already exists!" });

            var user = new User()
            {
                Email = model.Email.Trim(),
                Password = PasswordHelper.HashPassword(model.Password),
                UserName = model.Username.Trim()
            };
            try
            {
                var result = _dataContext.Users.Add(user);
                _dataContext.SaveChanges();

                return Ok(new Response { Status = "Success", Message = "Created successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = $"User creation failed! Please check user details and try again. Error: { ex.Message }" });
            }
        }

        [HttpPost]
        [Route("role")]
        public IActionResult RegisterRole([FromBody] SignUpModel model)
        {
            var userExists = _dataContext.Users.FirstOrDefault(u => u.UserName.Trim().ToLower() == model.Username.ToLower() || u.Email.Trim().ToLower() == model.Email.ToLower());
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "UserName or Email already exists!" });

            var user = new User()
            {
                Email = model.Email,
                Password = PasswordHelper.HashPassword(model.Password),
                UserName = model.Username
            };
            try
            {
                var result = _dataContext.Users.Add(user);
                _dataContext.SaveChanges();

                return Ok(new Response { Status = "Success", Message = "Created user successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = $"User creation failed! Please check user details and try again. Error { ex.Message }" });
            }
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult EditUser(User entity)
        {
            try
            {
                _dataContext.Entry(entity).State = EntityState.Modified;
                _dataContext.SaveChanges();

                return Ok(new Response { Status = "Success", Message = "Updated user successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = $"User edition failed! Please check user details and try again. Error { ex.Message }" });
            }
        }
    }
}

