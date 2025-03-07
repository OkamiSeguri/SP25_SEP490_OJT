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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly JWTService jwtService;


        public UserController(  JWTService jwtService)
        {
            userRepository = new UserRepository();
            this.jwtService = jwtService;
        }
        // GET: api/<UserController>
        [HttpGet("Admin")]
        public async Task<ActionResult> GetAllUsers()
        {
            try
            {
                var roleClaim = User.FindFirst(ClaimTypes.Role);
                var expClaim = User.FindFirst("exp");

                if (roleClaim == null)
                {
                    return Unauthorized(new
                    {
                        code = StatusCodes.Status401Unauthorized,
                        detail = "Authentication required."
                    });
                }

                if (expClaim != null && long.TryParse(expClaim.Value, out long expTimestamp))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).UtcDateTime;
                    if (DateTime.UtcNow > expirationTime)
                    {
                        return Unauthorized(new
                        {
                            code = StatusCodes.Status401Unauthorized,
                            detail = "Token expired."
                        });
                    }
                }

                if (!int.TryParse(roleClaim.Value, out int role) || role != 3)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        code = StatusCodes.Status403Forbidden,
                        detail = "You do not have permission"
                    });
                }

                var users = await userRepository.GetUserAll();

                if (users == null || !users.Any())
                {
                    return NotFound(new
                    {
                        results = new List<object>(),
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = StatusCodes.Status500InternalServerError,
                    detail = "Internal server error",
                });
            }
        }
        [HttpGet("Staff-Enter")]
        public async Task<ActionResult> GetUsersWithRole0()
        {
            try
            {
                var roleClaim = User.FindFirst(ClaimTypes.Role);
                var expClaim = User.FindFirst("exp");

                if (roleClaim == null)
                {
                    return Unauthorized(new
                    {
                        code = StatusCodes.Status401Unauthorized,
                        detail = "Authentication required."
                    });
                }

                if (expClaim != null && long.TryParse(expClaim.Value, out long expTimestamp))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).UtcDateTime;
                    if (DateTime.UtcNow > expirationTime)
                    {
                        return Unauthorized(new
                        {
                            code = StatusCodes.Status401Unauthorized,
                            detail = "Token expired."
                        });
                    }
                }

                if (!int.TryParse(roleClaim.Value, out int role) || (role != 1 && role != 2))
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        code = StatusCodes.Status403Forbidden,
                        detail = "You do not have permission"
                    });
                }

                var users = await userRepository.GetUserByRole(0);

                if (users == null || !users.Any())
                {
                    return NotFound(new
                    {
                        results = new List<object>(),
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = StatusCodes.Status500InternalServerError,
                    detail = "Internal server error",
                });
            }
        }


        // GET api/<UserController>/5
        [HttpGet("Staff-Enter/{id}")]
        public async Task<ActionResult> Get(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        code = StatusCodes.Status400BadRequest,
                        detail = "Invalid user ID."
                    });
                }

                var roleClaim = User.FindFirst(ClaimTypes.Role);
                var expClaim = User.FindFirst("exp");

                if (roleClaim == null)
                {
                    return Unauthorized(new
                    {
                        code = StatusCodes.Status401Unauthorized,
                        detail = "Authentication required."
                    });
                }

                if (expClaim != null && long.TryParse(expClaim.Value, out long expTimestamp))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).UtcDateTime;
                    if (DateTime.UtcNow > expirationTime)
                    {
                        return Unauthorized(new
                        {
                            code = StatusCodes.Status401Unauthorized,
                            detail = "Token expired."
                        });
                    }
                }

                if (roleClaim.Value != "1" && roleClaim.Value != "2")
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        code = StatusCodes.Status403Forbidden,
                        detail = "You do not have permission."
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = StatusCodes.Status500InternalServerError,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }
        [HttpGet("Admin/{id}")]
        public async Task<ActionResult> GetForAdmin(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        code = StatusCodes.Status400BadRequest,
                        detail = "Invalid user ID."
                    });
                }

                var roleClaim = User.FindFirst(ClaimTypes.Role);
                var expClaim = User.FindFirst("exp");

                if (roleClaim == null)
                {
                    return Unauthorized(new
                    {
                        code = StatusCodes.Status401Unauthorized,
                        detail = "Authentication required."
                    });
                }

                if (expClaim != null && long.TryParse(expClaim.Value, out long expTimestamp))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).UtcDateTime;
                    if (DateTime.UtcNow > expirationTime)
                    {
                        return Unauthorized(new
                        {
                            code = StatusCodes.Status401Unauthorized,
                            detail = "Token expired."
                        });
                    }
                }

                if (roleClaim.Value != "3")  // Chỉ cho Admin (role = 3)
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        code = StatusCodes.Status403Forbidden,
                        detail = "You do not have permission."
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = StatusCodes.Status500InternalServerError,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        // POST api/<UserController>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        code = StatusCodes.Status400BadRequest,
                        detail = "Invalid request data."
                    });
                }

                var roleClaim = User.FindFirst(ClaimTypes.Role);
                var expClaim = User.FindFirst("exp");

                if (roleClaim == null)
                {
                    return Unauthorized(new
                    {
                        code = StatusCodes.Status401Unauthorized,
                        detail = "Authentication required."
                    });
                }

                if (expClaim != null && long.TryParse(expClaim.Value, out long expTimestamp))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).UtcDateTime;
                    if (DateTime.UtcNow > expirationTime)
                    {
                        return Unauthorized(new
                        {
                            code = StatusCodes.Status401Unauthorized,
                            detail = "Token expired."
                        });
                    }
                }

                if (roleClaim.Value != "3")
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        code = StatusCodes.Status403Forbidden,
                        detail = "You do not have permission."
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = StatusCodes.Status500InternalServerError,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }


        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        code = StatusCodes.Status400BadRequest,
                        detail = "Invalid request data."
                    });
                }

                var roleClaim = User.FindFirst(ClaimTypes.Role);
                var expClaim = User.FindFirst("exp");

                if (roleClaim == null)
                {
                    return Unauthorized(new
                    {
                        code = StatusCodes.Status401Unauthorized,
                        detail = "Authentication required."
                    });
                }

                if (expClaim != null && long.TryParse(expClaim.Value, out long expTimestamp))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).UtcDateTime;
                    if (DateTime.UtcNow > expirationTime)
                    {
                        return Unauthorized(new
                        {
                            code = StatusCodes.Status401Unauthorized,
                            detail = "Token expired."
                        });
                    }
                }

                if (roleClaim.Value != "3")
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        code = StatusCodes.Status403Forbidden,
                        detail = "You do not have permission."
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = StatusCodes.Status500InternalServerError,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var roleClaim = User.FindFirst(ClaimTypes.Role);
                var expClaim = User.FindFirst("exp");

                if (roleClaim == null)
                {
                    return Unauthorized(new
                    {
                        code = StatusCodes.Status401Unauthorized,
                        detail = "Authentication required."
                    });
                }

                if (expClaim != null && long.TryParse(expClaim.Value, out long expTimestamp))
                {
                    var expirationTime = DateTimeOffset.FromUnixTimeSeconds(expTimestamp).UtcDateTime;
                    if (DateTime.UtcNow > expirationTime)
                    {
                        return Unauthorized(new
                        {
                            code = StatusCodes.Status401Unauthorized,
                            detail = "Token expired."
                        });
                    }
                }

                if (roleClaim.Value != "3")
                {
                    return StatusCode(StatusCodes.Status403Forbidden, new
                    {
                        code = StatusCodes.Status403Forbidden,
                        detail = "You do not have permission."
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

                await userRepository.Delete(id);
                return Ok(new
                {
                    code = StatusCodes.Status200OK,
                    detail = "Delete successful."
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    status = StatusCodes.Status500InternalServerError,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        //[HttpPost("RefreshToken")]
        //public async Task<ActionResult> RefreshToken([FromBody] TokenRequestDTO tokenRequest)
        //{
        //    var principal = jwtService.GetPrincipalFromExpiredToken(tokenRequest.AccessToken);
        //    if (principal == null)
        //    {
        //        return BadRequest("Invalid access token");
        //    }

        //    var userId = principal.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
        //    if (userId == null)
        //    {
        //        return BadRequest("Invalid access token");
        //    }

        //    var user = await userRepository.GetUserById(int.Parse(userId));
        //    if (user == null || user.RefreshToken != tokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        //    {
        //        return Unauthorized("Invalid refresh token");
        //    }

        //    // Tạo token mới
        //    var newAccessToken = jwtService.GenerateToken(user);
        //    var newRefreshToken = jwtService.GenerateRefreshToken();

        //    // Lưu refresh token mới vào DB
        //    user.RefreshToken = newRefreshToken;
        //    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        //    await userRepository.Update(user);

        //    return Ok(new
        //    {
        //        AccessToken = newAccessToken,
        //        RefreshToken = newRefreshToken
        //    });
        //}
        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        code = StatusCodes.Status400BadRequest,
                        detail = "Invalid request data."
                    });
                }

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
                //var refreshToken = jwtService.GenerateRefreshToken();

                //user.RefreshToken = refreshToken;
                //user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = StatusCodes.Status500InternalServerError,
                    message = "Internal server error",
                    error = ex.Message
                });
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exist = await userRepository.GetUserByEmail(model.Email);
            if (exist != null)
            {
                return BadRequest("Email đã tồn tại.");
            }

            var newUser = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role
            };

            await userRepository.Create(newUser);
            return Ok(newUser);
        }
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var userIdString = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new
                {
                    code = StatusCodes.Status401Unauthorized,
                    detail = "User not authenticated."
                });
            }


            return Ok(new
            {
                code = StatusCodes.Status200OK,
                detail = "Logged out successfully."
            });
        }





    }
}
