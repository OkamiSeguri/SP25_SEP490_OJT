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
using FOMSOData.Mappings;
using System.Globalization;
using CsvHelper;
using FOMSOData.Mappings;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly JWTService jwtService;
        private readonly IConfiguration _configuration;
        private readonly IOJTConditionRepository ojtConditionRepository;
        private readonly IStudentProfileRepository studentProfileRepository;


        public UserController(JWTService jwtService, IConfiguration configuration)
        {
            studentProfileRepository = new StudentProfileRepository();
            ojtConditionRepository = new OJTConditionRepository();
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

            var filteredUsers = users.Where(u => u.Role != 3);


            return Ok(new
            {
                results = filteredUsers.Select(u => new
                {
                    id = u.UserId,
                    fullname = u.FullName,
                    mssv = u.MSSV,
                    email = u.Email,
                    password = u.Password,
                    role = u.Role
                }),
                status = StatusCodes.Status200OK
            });
        }

        [CustomAuthorize("1", "2")]
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

            var userIds = users.Select(u => u.UserId).ToList();
            var profiles = await studentProfileRepository.GetStudentProfilesByUserIds(userIds);
            var profileDict = profiles.ToDictionary(p => p.UserId);

            var results = new List<object>();
            foreach (var user in users)
            {
                profileDict.TryGetValue(user.UserId, out var profile);

                bool isEligible = false;
                if (profile != null)
                {
                    isEligible = await IsEligibleForOJT(user.UserId);
                }

                results.Add(new
                {
                    userid = user.UserId,
                    fullname = user.FullName,
                    mssv = user.MSSV,
                    email = user.Email,
                    studentId = profile?.StudentId,
                    cohort = profile?.Cohort,
                    totalCredits = profile?.TotalCredits,
                    debtCredits = profile?.DebtCredits,
                    ojtEligible = isEligible
                });
            }

            return Ok(new
            {
                results,
                status = StatusCodes.Status200OK
            });
        }
        private async Task<bool> IsEligibleForOJT(int userId)
        {
            var studentProfile = await studentProfileRepository.GetStudentProfileByUserId(userId);
            if (studentProfile == null) return false;

            double debtRatio = (double)studentProfile.DebtCredits.GetValueOrDefault()
                               / studentProfile.TotalCredits.GetValueOrDefault();

            var failedMandatorySubjects = await studentProfileRepository.GetFailedMandatorySubjectsAsync(userId);
            double maxDebtRatio = await ojtConditionRepository.GetMaxDebtRatioAsync();
            bool checkFailedSubjects = await ojtConditionRepository.ShouldCheckFailedSubjectsAsync();

            if (debtRatio > maxDebtRatio) return false;
            if (checkFailedSubjects && failedMandatorySubjects.Any()) return false;

            return true;
        }

        [CustomAuthorize("1", "2")]
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
                    detail = "You do not have permission"
                });
            }

            var profile = await studentProfileRepository.GetStudentProfileByUserId(user.UserId);
            bool isEligible = false;
            if (profile != null)
            {
                isEligible = await IsEligibleForOJT(user.UserId);
            }

            return Ok(new
            {
                result = new
                {
                    userid = user.UserId,
                    fullname = user.FullName,
                    mssv = user.MSSV,
                    email = user.Email,
                    studentId = profile?.StudentId,
                    cohort = profile?.Cohort,
                    totalCredits = profile?.TotalCredits,
                    debtCredits = profile?.DebtCredits,
                    ojtEligible = isEligible
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
                    mssv = user.MSSV,
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
                    mssv = user.MSSV,
                    email = user.Email,
                    password = user.Password,
                },
                status = StatusCodes.Status200OK
            });
        }
        [CustomAuthorize("1")]
        [HttpPost("staff")]
        public async Task<ActionResult> PostForStaff([FromBody] User user)
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
                    mssv = user.MSSV,
                    password = user.Password,
                    role = 0
                },
                status = StatusCodes.Status200OK
            });
        }
        [CustomAuthorize("1")]
        [HttpPut("staff/{id}")]
        public async Task<ActionResult> PutForStaff(int id, [FromBody] User user)
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

            if (exist.Role != 0)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = StatusCodes.Status403Forbidden,
                    detail = "You do not have permission"
                });
            }

            user.UserId = id;
            user.Role = 0; 
            await userRepository.Update(user);

            return Ok(new
            {
                code = StatusCodes.Status200OK,
                detail = "Update successful."
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
        [CustomAuthorize("1")]
        [HttpDelete("staff/{id}")]
        public async Task<ActionResult> DeleteForStaff(int id)
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
            if (exist.Role != 0)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = StatusCodes.Status403Forbidden,
                    detail = "You do not have permission"
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
                return Unauthorized(new
                {
                    message = "You don't have permission to access the system"
                });
            }

                var accessToken = jwtService.GenerateToken(user);

                return Ok(new
                {
                    token = accessToken,

                });
        }
        [CustomAuthorize("1")]
        [HttpPost("import")]
        public async Task<IActionResult> ImportUsersCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is empty or missing." });

            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<UserMap>();
                var requiredHeaders = new List<string> { "MSSV", "FullName", "Email", "Password" };
                csv.Read();
                csv.ReadHeader();
                var missingHeaders = requiredHeaders.Where(h => !csv.HeaderRecord.Contains(h)).ToList();

                if (missingHeaders.Any())
                {
                    return BadRequest(new
                    {
                        message = "CSV file is missing required columns!",
                        missingColumns = missingHeaders
                    });
                }
                var records = new List<User>();
                var invalidRows = new List<int>();
                int rowIndex = 1; // Bắt đầu từ dòng 1 (bỏ qua header)

                while (csv.Read())
                {
                    var user = new User();
                    bool isValid = true;

                    if (!csv.TryGetField("MSSV", out string mssv) || string.IsNullOrWhiteSpace(mssv))
                        isValid = false;
                    if (!csv.TryGetField("FullName", out string fullName) || string.IsNullOrWhiteSpace(fullName))
                        isValid = false;
                    if (!csv.TryGetField("Email", out string email) || string.IsNullOrWhiteSpace(email))
                        isValid = false;
                    if (!csv.TryGetField("Password", out string password) || string.IsNullOrWhiteSpace(password))
                        isValid = false;

                    if (!isValid)
                    {
                        invalidRows.Add(rowIndex);
                        rowIndex++;
                        continue;
                    }

                    records.Add(new User
                    {
                        MSSV = mssv.Trim(),
                        FullName = fullName.Trim(),
                        Email = email.Trim(),
                        Password = password.Trim(),
                    });

                    rowIndex++;
                }

                if (invalidRows.Any())
                {
                    return BadRequest(new
                    {
                        message = "Some rows have missing required fields!",
                        invalidRows = invalidRows
                    });
                }
                // Kiểm tra trùng MSSV và Email
                var (duplicateMSSVs, duplicateEmails) = await userRepository.ImportUsersAsync(records);

                if (duplicateMSSVs.Count > 0 || duplicateEmails.Count > 0)
                {
                    return BadRequest(new
                    {
                        message = "Some users already exist!",
                        duplicateMSSVs = duplicateMSSVs,
                        duplicateEmails = duplicateEmails
                    });
                }

                return Ok(new { message = "Users CSV imported successfully!", count = records.Count });
            }
        }

    }
}
