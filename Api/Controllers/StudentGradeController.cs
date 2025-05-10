using Azure.Core;
using BusinessObject;
using CsvHelper;
using FOMSOData.Authorize;
using FOMSOData.Mappings;
using FOMSOData.Models;
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
    [Route("api/[controller]")]
    [ApiController]

    public class StudentGradeController : ControllerBase
    {
        private readonly IStudentGradeRepository studentGradeRepository;
        private readonly IUserRepository userRepository;
        private readonly ICohortCurriculumRepository cohortCurriculumRepository;
        private readonly ICurriculumRepository curriculumRepository;
        public StudentGradeController()
        {
            curriculumRepository = new CurriculumRepository();
            userRepository = new UserRepository();
            studentGradeRepository = new StudentGradeRepository();
            cohortCurriculumRepository = new CohortCurriculumRepository();
        }
        [CustomAuthorize("1")]

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
            var users = await userRepository.GetUserAll();
            var userDict = users.ToDictionary(u => u.UserId, u => u.MSSV);

            var curriculums = await curriculumRepository.GetCurriculumAll();
            var curriculumDict = curriculums.ToDictionary(c => c.CurriculumId, c => c.SubjectCode);

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
                        mssv = userDict.ContainsKey(g.UserId) ? userDict[g.UserId] : "Unknown",
                        subjectCode = curriculumDict.ContainsKey(g.CurriculumId) ? curriculumDict[g.CurriculumId] : "Unknown",
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
        [CustomAuthorize("1","0","2")]

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetGradeByUserId(int userId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int tokenUserId))
            {
                return Unauthorized(new { code = 401, detail = "Invalid or missing authentication token" });
            }
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
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if ((roleClaim != "1" && roleClaim != "2") && tokenUserId != userId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = 403,
                    detail = "You do not have permission"
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



        [CustomAuthorize("1")]

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

        [CustomAuthorize("1")]

        // POST api/<StudentGradeController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StudentGradeDTO studentGradeDTO)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }
            var user = await userRepository.GetUserByMSSV(studentGradeDTO.MSSV);
            if (user == null)
            {
                return NotFound(new { code = 404, detail = "User not found" });
            }
            var curriculum = await curriculumRepository.GetCurriculumBySubjectCode(studentGradeDTO.SubjectCode);
            if (curriculum == null)
            {
                return NotFound(new { code = 404, detail = "Subject not found" });
            }

            var existingGrade = await studentGradeRepository.GetGrade(user.UserId, curriculum.CurriculumId);
                if (existingGrade != null)
                {
                    return Conflict(new
                    {
                        code = StatusCodes.Status409Conflict,
                        detail = "Student grade already exists"
                    });
                }
            var studentGrade = new StudentGrade
            {
                UserId = user.UserId,
                CurriculumId = curriculum.CurriculumId,
                Grade = studentGradeDTO.Grade, 
                IsPassed = studentGradeDTO.IsPassed
            };
            await studentGradeRepository.Create(studentGrade);
            return Ok(new
            {
                result = new
                {
                    mssv = user.MSSV,
                    subjectCode = curriculum.SubjectCode,
                    semester = studentGrade.Semester,
                    grade = studentGrade.Grade,
                    isPassed = studentGrade.IsPassed
                },
                status = 200
            });

        }

        [CustomAuthorize("1")]

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


        [CustomAuthorize("1")]

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
        [CustomAuthorize("1")]

        [HttpPost("import")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is empty or missing." });

            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<StudentGradeMap>();
                csv.Read();
                csv.ReadHeader();
                var requiredHeaders = new List<string> { "MSSV", "SubjectCode", "Semester", "Grade" };
                var missingHeaders = requiredHeaders.Where(h => !csv.HeaderRecord.Contains(h)).ToList();

                if (missingHeaders.Any())
                {
                    return BadRequest(new
                    {
                        message = "CSV file is missing required columns!",
                        missingColumns = missingHeaders
                    });
                }

                var records = new List<StudentGradeImportDTO>();
                var invalidRows = new List<int>();
                int rowIndex = 1; // Bắt đầu từ dòng 1 (bỏ qua header)

                while (csv.Read())
                {
                    var record = new StudentGradeImportDTO();
                    bool isValid = true;
                    int semester = 0;
                    decimal grade = 0m;

                    if (!csv.TryGetField("MSSV", out string mssv) || string.IsNullOrWhiteSpace(mssv))
                        isValid = false;
                    if (!csv.TryGetField("SubjectCode", out string subjectCode) || string.IsNullOrWhiteSpace(subjectCode))
                        isValid = false;
                    if (!csv.TryGetField("Semester", out string semesterStr) || string.IsNullOrWhiteSpace(semesterStr) || !int.TryParse(semesterStr, out semester))
                        isValid = false;
                    if (!csv.TryGetField("Grade", out string gradeStr) || string.IsNullOrWhiteSpace(gradeStr) || !decimal.TryParse(gradeStr, out grade))
                        isValid = false;

                    if (!isValid)
                    {
                        invalidRows.Add(rowIndex);
                        rowIndex++;
                        continue;
                    }

                    records.Add(new StudentGradeImportDTO
                    {
                        MSSV = mssv.Trim(),
                        SubjectCode = subjectCode.Trim(),
                        Semester = semester,
                        Grade = grade
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

                var mssvList = records.Select(r => r.MSSV).Distinct().ToList();
                var subjectCodeList = records.Select(r => r.SubjectCode).Distinct().ToList();

                var users = await userRepository.GetUserByMSSVList(mssvList);
                var curriculums = await curriculumRepository.GetCurriculumBySubjectCodeList(subjectCodeList);

                var userDict = users.ToDictionary(u => u.MSSV, u => u);
                var curriculumDict = curriculums.ToDictionary(c => c.SubjectCode, c => c);

                var studentGrades = new List<StudentGrade>();
                var missingUserIds = new List<string>();
                var missingCurriculumIds = new List<string>();

                foreach (var record in records)
                {
                    if (!userDict.TryGetValue(record.MSSV, out var user))
                    {
                        missingUserIds.Add(record.MSSV);
                        continue;
                    }

                    if (!curriculumDict.TryGetValue(record.SubjectCode, out var curriculum))
                    {
                        missingCurriculumIds.Add(record.SubjectCode);
                        continue;
                    }

                    studentGrades.Add(new StudentGrade
                    {
                        UserId = user.UserId,
                        CurriculumId = curriculum.CurriculumId,
                        Semester = record.Semester,
                        Grade = record.Grade,
                    });
                }

                if (missingUserIds.Count > 0 || missingCurriculumIds.Count > 0)
                {
                    return BadRequest(new
                    {
                        message = "Some MSSV or SubjectCode does not exist!",
                        missingUsers = missingUserIds,
                        missingCurriculums = missingCurriculumIds
                    });
                }

                await studentGradeRepository.ImportStudentGrades(studentGrades);

                return Ok(new { message = "Student Grades CSV imported successfully!", count = studentGrades.Count });
            }
        }





    }
}