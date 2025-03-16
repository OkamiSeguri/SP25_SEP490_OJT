using BusinessObject;
using CsvHelper;
using FOMSOData.Authorize;
using FOMSOData.Mappings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using System.Diagnostics;
using System.Globalization;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    [CustomAuthorize("1")]

    public class StudentGradeController : ControllerBase
    {
        private readonly IStudentGradeRepository studentGradeRepository;
        private readonly ICohortCurriculumRepository cohortCurriculumRepository;
        public StudentGradeController()
        {
            studentGradeRepository = new StudentGradeRepository();
            cohortCurriculumRepository = new CohortCurriculumRepository();
        }

        // GET: api/<StudentGradeController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentGrade>>> Get()
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

                var maxSemester = cohorts
                    .Where(cc => cc.CurriculumId == g.CurriculumId)
                    .Select(cc => cc.Semester)
                    .DefaultIfEmpty(0)
                    .Max();
                var minSemester = cohorts
                    .Where(cc => cc.CurriculumId == g.CurriculumId)
                    .Select(cc => cc.Semester)
                    .DefaultIfEmpty(0)
                    .Min();

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

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetGradeByUserId(int userId)
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

        // GET api/<StudentGradeController>/5
        [HttpGet("{userId}/{curriculumId}")]
        public async Task<IActionResult> Get(int userId, int curriculumId)
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


        // POST api/<StudentGradeController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StudentGrade studentGrade)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }

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


        // PUT api/<StudentGradeController>/5
        [HttpPut("{userId}/{curriculumId}")]
        public async Task<IActionResult> Put(int userId, int curriculumId, [FromBody] StudentGrade studentGrade)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }

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



        // DELETE api/<StudentGradeController>/5
        [HttpDelete("{userId}/{curriculumId}")]
        public async Task<IActionResult> DeleteGrade(int userId, int curriculumId)
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
        [HttpPost("import")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
          return BadRequest(new { message = "File is empty or missing." });

            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<StudentGradeMap>();
                var records = csv.GetRecords<StudentGrade>().ToList();

                var (missingUserIds, missingCurriculumIds) = await studentGradeRepository.ImportStudentGrades(records);

                if (missingUserIds.Count > 0 || missingCurriculumIds.Count > 0)
          {
                    return BadRequest(new
                    {
                        message = "Some UserId or CurriculumId does not exist!",
                        missingUsers = missingUserIds,
                        missingCurriculums = missingCurriculumIds
                    });
                }

                return Ok(new { message = "Student Grades CSV imported successfully!", count = records.Count });
            }
        }
    }
}