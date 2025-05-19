using Azure.Core;
using BusinessObject;
using ClosedXML.Excel;
using CsvHelper;
using FOMSOData.Authorize;
using FOMSOData.Mappings;
using FOMSOData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                    results = result,
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
            public async Task<IActionResult> ImportFile(IFormFile file)
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "File is empty or missing." });

                var extension = Path.GetExtension(file.FileName).ToLower();
                var records = new List<StudentGradeImportDTO>();
                var invalidRows = new List<int>();

                try
            {
                if (extension == ".csv")
                {
                    using var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
                    using var csv = new CsvReader(stream, CultureInfo.InvariantCulture);

                    csv.Context.RegisterClassMap<StudentGradeMap>();
                    csv.Read();
                    csv.ReadHeader();

                    var requiredHeaders = new List<string> { "MSSV", "SubjectCode", "Semester", "Grade" };
                    var missingHeaders = requiredHeaders.Where(h => !csv.HeaderRecord.Contains(h)).ToList();
                    if (missingHeaders.Any())
                        return BadRequest(new { message = "CSV file is missing required columns!", missingColumns = missingHeaders });

                    int rowIndex = 1;
                    while (csv.Read())
                    {
                        string mssv = csv.GetField("MSSV")?.Trim();
                        string subjectCode = csv.GetField("SubjectCode")?.Trim();
                        string semesterStr = csv.GetField("Semester")?.Trim();
                        string gradeStr = csv.GetField("Grade")?.Trim();

                        bool isValid = true;

                        isValid &= !string.IsNullOrWhiteSpace(mssv);
                        isValid &= !string.IsNullOrWhiteSpace(subjectCode);

                        bool validSemester = int.TryParse(semesterStr, out int semester);
                        bool validGrade = decimal.TryParse(gradeStr, out decimal grade);

                        isValid &= validSemester;
                        isValid &= validGrade;

                        if (!isValid)
                        {
                            invalidRows.Add(rowIndex++);
                            continue;
                        }
                        records.Add(new StudentGradeImportDTO
                        {
                            MSSV = mssv,
                            SubjectCode = subjectCode,
                            Semester = semester,
                            Grade = grade
                        });

                        rowIndex++;
                    }
                }

                else if (extension == ".xlsx")
            {
                using var stream = file.OpenReadStream();
                using var workbook = new XLWorkbook(stream);
                var worksheet = workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                    return BadRequest(new { message = "Excel file is empty." });

                var headers = new[] { "MSSV", "SubjectCode", "Semester", "Grade" };
                var headerMap = new Dictionary<string, int>();

                var headerRow = worksheet.Row(1);
                for (int col = 1; col <= worksheet.LastColumnUsed().ColumnNumber(); col++)
                {
                    var header = headerRow.Cell(col).GetString().Trim();
                    if (headers.Contains(header))
                        headerMap[header] = col;
                }

                var missingHeaders = headers.Where(h => !headerMap.ContainsKey(h)).ToList();
                if (missingHeaders.Any())
                    return BadRequest(new { message = "Excel file is missing required columns!", missingColumns = missingHeaders });

                for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                {
                    bool isValid = true;
                    var rowCells = worksheet.Row(row);

                    string mssv = rowCells.Cell(headerMap["MSSV"]).GetString().Trim();
                    string subjectCode = rowCells.Cell(headerMap["SubjectCode"]).GetString().Trim();
                    string semesterStr = rowCells.Cell(headerMap["Semester"]).GetString().Trim();
                    string gradeStr = rowCells.Cell(headerMap["Grade"]).GetString().Trim();

                    isValid &= !string.IsNullOrWhiteSpace(mssv);
                    isValid &= !string.IsNullOrWhiteSpace(subjectCode);
                    isValid &= int.TryParse(semesterStr, out int semester);
                    isValid &= decimal.TryParse(gradeStr, out decimal grade);

                    if (!isValid)
                    {
                        invalidRows.Add(row - 1); // Vì dòng header là dòng 1
                        continue;
                    }

                    records.Add(new StudentGradeImportDTO
                    {
                        MSSV = mssv,
                        SubjectCode = subjectCode,
                        Semester = semester,
                        Grade = grade
                    });
                }
            }
            else
            {
                return BadRequest(new { message = "Unsupported file format. Only CSV and XLSX are allowed." });
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error while reading file.", error = ex.Message });
        }

        if (invalidRows.Any())
        {
            return BadRequest(new
            {
                message = "Some rows have missing or invalid fields!",
                invalidRows = invalidRows
            });
        }

        // ✅ Giữ nguyên phần xử lý logic import
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

        if (missingUserIds.Any() || missingCurriculumIds.Any())
        {
            return BadRequest(new
            {
                message = "Some MSSV or SubjectCode does not exist!",
                missingUsers = missingUserIds,
                missingCurriculums = missingCurriculumIds
            });
        }

        try
        {
            await studentGradeRepository.ImportStudentGrades(studentGrades);
            return Ok(new
            {
                message = "Student Grades imported successfully!",
                count = studentGrades.Count
            });
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new
            {
                message = "Database update failed. Please check for duplicate records or constraint violations.",
                error = ex.InnerException?.Message ?? ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Unexpected error occurred.",
                error = ex.Message
            });
        }
    }






}
}