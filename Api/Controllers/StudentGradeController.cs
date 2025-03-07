using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using System.Diagnostics;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]

    public class StudentGradeController : ControllerBase
    {
        private readonly IStudentGradeRepository studentGradeRepository;
        private readonly ICohortCurriculumRepository cohortCurriculumRepository;
        public StudentGradeController()
        {
            studentGradeRepository = new StudentGradeRepository();
            cohortCurriculumRepository = new CohortCurriculumRepository();
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

        // GET: api/<StudentGradeController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentGrade>>> Get()
        {
            if (!IsAuthenticated())
            {
                return StatusCode(401, new { code = 401, detail = "Authentication required" });
            }

            if (!IsAuthorized())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = StatusCodes.Status403Forbidden,
                    detail = "You do not have permission"
                });
            }
            try
            {
                var grade = await studentGradeRepository.GetGradesAll();
                if (grade == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        code21 = StatusCodes.Status404NotFound,
                        detail = "No grade found"
                    });
                }
                var cohorts = await cohortCurriculumRepository.GetCohortCurriculumAll();

                var result = grade.Select(g =>
                {
                    // Lấy semester tối đa của 23
                    // curriculum trong cohort
                    var maxSemester = cohorts
                        .Where(cc => cc.CurriculumId == g.CurriculumId)
                        .Max(cc => cc.Semester);

                    var minSemester = cohorts
                        .Where(cc => cc.CurriculumId == g.CurriculumId)
                        .Min(cc => (int?)cc.Semester) ?? 0;
                    int slowBy = g.Semester - maxSemester;  
                    int fastBy = minSemester - g.Semester;  

                    bool isSlowStudy = slowBy > 0;
                    bool isFastStudy = fastBy > 0;


                    return new
                    {
                        userId = g.UserId,
                        curriculumId = g.CurriculumId,
                        semester = g.Semester,
                        grade = g.Grade,
                        ispass = g.IsPassed,
                        status = isSlowStudy
                    ? $"Slow study by {slowBy} semesters"
                    : (isFastStudy ? $"Fast study by {fastBy} semesters" : "Normal")
                    };
                });
                return Ok(new
                {
                    results = result,
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
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetGradeByUserId(int userId)
        {
            if (!IsAuthenticated())
            {
                return StatusCode(401, new { code = 401, detail = "Authentication required" });
            }

            if (!IsAuthorized())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = StatusCodes.Status403Forbidden,
                    detail = "You do not have permission"
                });
            }
            try
            {
                var grades = await studentGradeRepository.GetGradeByUserId(userId);
                var cohorts = await cohortCurriculumRepository.GetCohortCurriculumAll();

                if (grades == null || !grades.Any())
                {
                    return NotFound(new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "No grades found for this user."
                    });
                }
                var result = grades.Select(g =>
                {
                    // Lấy semester tối đa của curriculum trong cohort
                    var maxSemester = cohorts
                        .Where(cc => cc.CurriculumId == g.CurriculumId)
                        .Max(cc => cc.Semester);

                    var minSemester = cohorts
                        .Where(cc => cc.CurriculumId == g.CurriculumId)
                        .Min(cc => (int?)cc.Semester) ?? 0;
                    int slowBy = g.Semester - maxSemester;
                    int fastBy = minSemester - g.Semester;

                    bool isSlowStudy = slowBy > 0;
                    bool isFastStudy = fastBy > 0;


                    return new
                    {
                        userId = g.UserId,
                        curriculumId = g.CurriculumId,
                        semester = g.Semester,
                        grade = g.Grade,
                        ispass = g.IsPassed,
                        status = isSlowStudy
                    ? $"Slow study by {slowBy} semesters"
                    : (isFastStudy ? $"Fast study by {fastBy} semesters" : "Normal")
                    };
                });
                return Ok(new
                {
                    results = grades,
                    status = StatusCodes.Status200OK
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    code = StatusCodes.Status500InternalServerError,
                    detail = "Internal server error",
                    error = ex.Message
                });
            }
        }
        // GET api/<StudentGradeController>/5
        [HttpGet("{userId}/{curriculumId}")]
        public async Task<IActionResult> Get(int userId, int curriculumId)
        {
            if (!IsAuthenticated())
            {
                return StatusCode(401, new { code = 401, detail = "Authentication required" });
            }

            if (!IsAuthorized())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = StatusCodes.Status403Forbidden,
                    detail = "You do not have permission"
                });
            }
            try
            {
                var grade = await studentGradeRepository.GetGrade(userId, curriculumId);

                if (grade == null)
                {
                    return NotFound(new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "Grade not found."
                    });
                }

                return Ok(new
                {
                    result = grade,
                    status = StatusCodes.Status200OK
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    code = StatusCodes.Status500InternalServerError,
                    detail = "Internal server error",
                    error = ex.Message
                });
            }
        }

        // POST api/<StudentGradeController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StudentGrade studentGrade)
        {
            if (!IsAuthenticated())
            {
                return StatusCode(401, new { code = 401, detail = "Authentication required" });
            }

            if (!IsAuthorized())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = StatusCodes.Status403Forbidden,
                    detail = "You do not have permission"
                });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }

            try
            {
                var existingGrade = await studentGradeRepository.GetGrade(studentGrade.UserId, studentGrade.CurriculumId);
                if (existingGrade != null)
                {
                    return Conflict(new
                    {
                        code = StatusCodes.Status409Conflict,
                        detail = "Student grade already exists"
                    });
                }
                await studentGradeRepository.Create(studentGrade);
                return Ok(new { result = studentGrade, status = 200 });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    code = StatusCodes.Status500InternalServerError,
                    detail = "Internal server error",
                    error = ex.Message
                });
            }
        }

        // PUT api/<StudentGradeController>/5
        [HttpPut("{userId}/{curriculumId}")]
        public async Task<IActionResult> Put(int userId, int curriculumId, [FromBody] StudentGrade studentGrade)
        {
            if (!IsAuthenticated())
            {
                return StatusCode(401, new { code = 401, detail = "Authentication required" });
            }

            if (!IsAuthorized())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = StatusCodes.Status403Forbidden,
                    detail = "You do not have permission"
                });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }

            try
            {
                var exist = await studentGradeRepository.GetGrade(userId, curriculumId);
                if (exist == null)
                {
                    return NotFound(new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "Grade not found."
                    });
                }

                studentGrade.UserId = userId;
                studentGrade.CurriculumId = curriculumId;

                await studentGradeRepository.Update(studentGrade);

                return Ok(new
                {
                    result = new
                    {
                        userId = studentGrade.UserId,
                        curriculumId = studentGrade.CurriculumId,
                        semester = studentGrade.Semester,
                        grade = studentGrade.Grade,
                        ispass = studentGrade.IsPassed
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


        // DELETE api/<StudentGradeController>/5
        [HttpDelete("{userId}/{curriculumId}")]
        public async Task<IActionResult> DeleteGrade(int userId, int curriculumId)
        {
            if (!IsAuthenticated())
            {
                return StatusCode(401, new { code = 401, detail = "Authentication required" });
            }

            if (!IsAuthorized())
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = StatusCodes.Status403Forbidden,
                    detail = "You do not have permission"
                });
            }
            try
            {
                var exist = await studentGradeRepository.GetGrade(userId, curriculumId);
                if (exist == null)
                {
                    return NotFound(new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "Grade not found."
                    });
                }

                await studentGradeRepository.Delete(userId, curriculumId);

                return Ok(new
                {
                    message = "Grade deleted successfully",
                    status = StatusCodes.Status200OK
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    code = StatusCodes.Status500InternalServerError,
                    detail = "Internal server error",
                    error = ex.Message
                });
            }
        }
    }
}