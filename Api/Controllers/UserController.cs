using BusinessObject;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Repositories;
using Services;
using System.Data;
using Services;
using Microsoft.AspNetCore.Authorization;
using API.Models;
using FOMSOData.Models;
using System.Security.Claims;
using FOMSOData.Authorize;
using Google.Apis.Auth;
using System.Text;
using System.Security.Cryptography;
using FOMSOData.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly JWTService jwtService;
        private readonly IConfiguration _configuration;


        public UserController(  JWTService jwtService, IConfiguration configuration)
        {
            userRepository = new UserRepository();
            _configuration = configuration;
            this.jwtService = jwtService;
        }
        // GET: api/<UserController>
        [CustomAuthorize("3")]
        [HttpGet("admin")]
        public async Task<ActionResult> GetAllUsers()
        {


                var users = await userRepository.GetUserAll();

                if (users == null || !users.Any())
                {
                    return NotFound(new
                    {
                        status = StatusCodes.Status404NotFound,
                        detail = "No users found."
                    });
                }

                return Ok(new
                {
                    results = users.Select(u => new
                    {
                        id = u.UserId,
                        fullname = u.FullName,
                        email = u.Email,
                        password = u.Password,
                        role = u.Role
                    }),
                    status = StatusCodes.Status200OK
                });
            }
        [CustomAuthorize("1","2")]

        [HttpGet("staff-enter")]
        public async Task<ActionResult> GetUsersWithRole0()
        {


                var users = await userRepository.GetUserByRole(0);

                if (users == null || !users.Any())
                {
                    return NotFound(new
                    {
                        status = StatusCodes.Status404NotFound,
                        detail = "No users with role 0 found."
                    });
                }

                return Ok(new
                {
                    results = users.Select(u => new
                    {
                        id = u.UserId,
                        fullname = u.FullName,
                        email = u.Email,
                    }),
                    status = StatusCodes.Status200OK
                });
            }


        [CustomAuthorize("1","2")]

        // GET api/<UserController>/5
        [HttpGet("staff-enter/{id}")]
        public async Task<ActionResult> Get(int id)
        {

                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        code = StatusCodes.Status400BadRequest,
                        detail = "Invalid user ID."
                    });
                }
                var user = await userRepository.GetUserById(id);
                if (user == null)
                {
                    return NotFound(new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "User not found."
                    });
                }
                if (user.Role != 0)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        code = StatusCodes.Status403Forbidden,
                        detail = "You cannot access this user"
                    });
                }
                return Ok(new
                {
                    result = new
                    {
                        id = user.UserId,
                        fullname = user.FullName,
                        email = user.Email,

                    },
                    status = StatusCodes.Status200OK
                });
            }

        [CustomAuthorize("3")]
        [HttpGet("admin/{id}")]
        public async Task<ActionResult> GetForAdmin(int id)
        {

                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        code = StatusCodes.Status400BadRequest,
                        detail = "Invalid user ID."
                    });
                }      
                var user = await userRepository.GetUserById(id);
                if (user == null)
                {
                    return NotFound(new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "User not found."
                    });
                }

                return Ok(new
                {
                    result = new
                    {
                        id = user.UserId,
                        fullname = user.FullName,
                        email = user.Email,
                        password = user.Password, 
                        role = user.Role
                    },
                    status = StatusCodes.Status200OK
                });
            }

        // POST api/<UserController>
        [CustomAuthorize("3")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] User user)
        {

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        code = StatusCodes.Status400BadRequest,
                        detail = "Invalid request data."
                    });
                }
                await userRepository.Create(user);

                return Ok(new
                {
                    results = new
                    {
                        id = user.UserId,
                        name = user.FullName,
                        email = user.Email,
                        password = user.Password,
                        role = user.Role
                    },
                    status = StatusCodes.Status200OK
                });
            }

        // PUT api/<UserController>/5
        [CustomAuthorize("3")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] User user)
        {

                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        code = StatusCodes.Status400BadRequest,
                        detail = "Invalid request data."
                    });
                }
                var exist = await userRepository.GetUserById(id);
                if (exist == null)
                {
                    return NotFound(new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "User not found."
                    });
                }

                user.UserId = id;
                await userRepository.Update(user);

                return Ok(new
                {
                    code = StatusCodes.Status200OK,
                    detail = "Update successful."
                });
            }
        // DELETE api/<UserController>/5
        [CustomAuthorize("3")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {

                var exist = await userRepository.GetUserById(id);
                if (exist == null)
                {
                    return NotFound(new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "User not found."
                    });
                }

                await userRepository.Delete(id);
                return Ok(new
                {
                    code = StatusCodes.Status200OK,
                    detail = "Delete successful."
                });
            }

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {

                var user = await userRepository.ValidateUser(loginDTO.Email, loginDTO.Password);
                if (user == null)
                {
                    return BadRequest(new
                    {
                        code = StatusCodes.Status400BadRequest,
                        detail = "Invalid email or password."
                    });
                }

                var accessToken = jwtService.GenerateToken(user);
                await userRepository.Update(user);

                return Ok(new
                {
                    result = new
                    {
                        AccessToken = accessToken,
                        //RefreshToken = refreshToken
                    },
                    code = StatusCodes.Status200OK,

                });
            }

        [HttpPost("google-signin")]
        public async Task<ActionResult> GoogleSignIn([FromBody] GoogleLoginDTO googleLogin)
        {

                var googleClientId = _configuration["Authentication:Google:ClientId"];
                if (string.IsNullOrEmpty(googleClientId))
                {
                    return StatusCode(500, new { message = "Google Client ID not configured." });
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new List<string> { googleClientId } 
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(googleLogin.IdToken, settings);
                if (payload == null)
                {
                    return BadRequest(new
                    {
                        code = StatusCodes.Status400BadRequest,
                        detail = "Invalid Google token."
                    });
                }

                var user = await userRepository.GetUserByEmail(payload.Email);
                if (user == null)
                {
                    string randomPassword = PasswordService.GenerateRandomPassword(12);
                    string hashedPassword = PasswordService.HashPassword(randomPassword);

                    user = new User
                    {
                        FullName = payload.Name,
                        Email = payload.Email,
                        Password = hashedPassword, 
                        Role = 0 
                    };

                    await userRepository.Create(user);
                }

                var accessToken = jwtService.GenerateToken(user);

                return Ok(new
                {
                    token = accessToken,
                    user = new
                    {
                        id = user.UserId,
                        name = user.FullName,
                        email = user.Email,
                        role = user.Role

                    }
                });
            }

            
        


    }
}
