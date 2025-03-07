using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;
using System.Security.Claims;

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class StudentProfileController : ControllerBase
    {
        private readonly IStudentProfileRepository studentProfileRepository;
        private readonly JWTService jwtService;

        public StudentProfileController(JWTService jwtService)
        {
            studentProfileRepository = new StudentProfileRepository();
            this.jwtService = jwtService;

        }
        private bool IsAuthenticated()
        {
            return HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated;
        }

        private bool IsAuthorized()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role);

            return roleClaim != null && roleClaim.Value == "1";
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentProfile>>> Get()
        {

            if (!IsAuthenticated())
            {
                return StatusCode(401, new { code = 401, detail = "Authentication required" });
            }

            if (!IsAuthorized())
            {
                return StatusCode(403, new { code = 403, detail = "You do not have permission" });
            }

            try
            {

                var students = await studentProfileRepository.GetStudentProfileAll();
                if (students == null || !students.Any())
                {
                    return NotFound(new { code = 404, detail = "No student profiles found" });
                }
                return Ok(new
                {
                    results = students.Select(u => new
                    {
                        userId = u.UserId,
                        studentId = u.StudentId,
                        cohortCurriculumId = u.CohortCurriculumId,
                        totalCredits = u.TotalCredits,
                        debtCredits = u.DebtCredits,
                    }),
                    status = StatusCodes.Status200OK
                });
            }
            catch
            {
                return StatusCode(500, new { code = 500, detail = "Internal server error" });
            }
        }

        [HttpGet("{userId}/{studentId}")]
        public async Task<ActionResult<StudentProfile>> GetStudentProfile(int userId, int studentId)
        {
            if (!IsAuthenticated())
            {
                return StatusCode(401, new { code = 401, detail = "Authentication required" });
            }


            if (!IsAuthorized())
            {
                return StatusCode(403, new { code = 403, detail = "You do not have permission" });
            }

            try
            {
                var student = await studentProfileRepository.GetStudentProfile(userId, studentId);
                if (student == null)
                {
                    return NotFound(new { code = 404, detail = "Student profile not found" });
                }
                return Ok(new
                {
                    result = new
                    {
                        userId = student.UserId,
                        studentId = student.StudentId,
                        cohortCurriculumId = student.CohortCurriculumId,
                        totalCredits = student.TotalCredits,
                        debtCredits = student.DebtCredits,
                    },
                    status = StatusCodes.Status200OK
                });
            }
            catch
            {
                return StatusCode(500, new { code = 500, detail = "Internal server error" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] StudentProfile studentProfile)
        {
            if (!IsAuthenticated())
            {
                return StatusCode(401, new { code = 401, detail = "Authentication required" });
            }


            if (!IsAuthorized())
            {
                return StatusCode(403, new { code = 403, detail = "You do not have permission" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }

            try
            {
                var existingGrade = await studentProfileRepository.GetStudentProfile(studentProfile.UserId, studentProfile.StudentId);
                if (existingGrade != null)
                {
                    return Conflict(new
                    {
                        code = StatusCodes.Status409Conflict,
                        detail = "Student profile already exists"
                    });
                }
                await studentProfileRepository.Create(studentProfile);
                return Ok(new
                {
                    results =  new
                    {
                        userId = studentProfile.UserId,
                        studentId = studentProfile.StudentId,
                        cohortCurriculumId = studentProfile.CohortCurriculumId,
                        totalCredits = studentProfile.TotalCredits,
                        debtCredits = studentProfile.DebtCredits,
                    },
                    status = StatusCodes.Status200OK
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    code = StatusCodes.Status500InternalServerError,
                    detail = "Internal server error",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] StudentProfile studentProfile)
        {
            if (!IsAuthenticated())
            {
                return StatusCode(401, new { code = 401, detail = "Authentication required" });
            }


            if (!IsAuthorized())
            {
                return StatusCode(403, new { code = 403, detail = "You do not have permission" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }

            try
            {
                var exist = await studentProfileRepository.GetStudentProfileById(id);
                if (exist == null)
                {
                    return NotFound(new { code = 404, detail = "Student profile not found" });
                }

                studentProfile.StudentId = id;
                await studentProfileRepository.Update(studentProfile);
                return Ok(new { result = studentProfile, status = 200 });
            }
            catch
            {
                return StatusCode(500, new { code = 500, detail = "Internal server error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (!IsAuthenticated())
            {
                return StatusCode(401, new { code = 401, detail = "Authentication required" });
            }


            if (!IsAuthorized())
            {
                return StatusCode(403, new { code = 403, detail = "You do not have permission" });
            }

            try
            {
                var exist = await studentProfileRepository.GetStudentProfileById(id);
                if (exist == null)
                {
                    return NotFound(new { code = 404, detail = "Student profile not found" });
                }

                await studentProfileRepository.Delete(id);
                return Ok(new { status = 200, message = "Delete Success" });
            }
            catch
            {
                return StatusCode(500, new { code = 500, detail = "Internal server error" });
            }
        }
    }
}
