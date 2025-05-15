using BusinessObject;
using CsvHelper;
using FOMSOData.Authorize;
using FOMSOData.Mappings;
using FOMSOData.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services;
using System.Globalization;
using System.Text;

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuthorize("1", "2", "3")]

    public class StudentProfileController : ControllerBase
    {
        private readonly IStudentProfileRepository studentProfileRepository;
        private readonly IUserRepository userRepository;
        private readonly IStudentGradeRepository studentGradeRepository;
        private readonly ICohortCurriculumRepository cohortCurriculumRepository;
        private readonly IOJTConditionRepository ojtConditionRepository;

        private readonly JWTService jwtService;

        public StudentProfileController(JWTService jwtService)
        {
            userRepository = new UserRepository();
            studentProfileRepository = new StudentProfileRepository();
            studentGradeRepository = new StudentGradeRepository();
            cohortCurriculumRepository = new CohortCurriculumRepository();
            ojtConditionRepository = new OJTConditionRepository();

            this.jwtService = jwtService;

        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> Get()
        {
            var students = await studentProfileRepository.GetStudentProfileAll();
            if (students == null || !students.Any())
            {
                return NotFound(new { code = 404, detail = "No student profiles found" });
            }

            var users = await userRepository.GetUserAll();
            var userDict = users.ToDictionary(u => u.UserId, u => u.MSSV);

            // Chuẩn bị kết quả kèm Eligibility
            var resultList = new List<object>();
            foreach (var u in students)
            {
                bool isEligible = await IsEligibleForOJT(u.UserId); // Gọi logic kiểm tra OJT
                resultList.Add(new
                {
                    studentId = u.StudentId,
                    mssv = userDict.ContainsKey(u.UserId) ? userDict[u.UserId] : "Unknown",
                    cohort = u.Cohort,
                    totalCredits = u.TotalCredits,
                    debtCredits = u.DebtCredits,
                    ojtEligible = isEligible
                });
            }

            return Ok(new
            {
                results = resultList,
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

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentProfile>> GetStudentProfile(int id)
        {

            var student = await studentProfileRepository.GetStudentProfileById(id);
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
                    cohort = student.Cohort,
                    totalCredits = student.TotalCredits,
                    debtCredits = student.DebtCredits,
                },
                status = StatusCodes.Status200OK
            });
        }
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] StudentProfileDTO studentProfileDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }

            try
            {
                // Tìm UserId từ MSSV
                var user = await userRepository.GetUserByMSSV(studentProfileDTO.MSSV);
                if (user == null)
                {
                    return NotFound(new { code = 404, detail = "User not found with the provided MSSV." });
                }

                // Kiểm tra xem StudentProfile đã tồn tại chưa
                var existingProfile = await studentProfileRepository.GetStudentProfileByUserId(user.UserId);
                if (existingProfile != null)
                {
                    return Conflict(new { code = 409, detail = "Student profile already exists" });
                }

                var studentProfile = new StudentProfile
                {
                    UserId = user.UserId,
                    Cohort = studentProfileDTO.Cohort,
                    TotalCredits = 0,
                    DebtCredits = 0
                };

                await studentProfileRepository.Create(studentProfile);

                var cohortCurriculums = await cohortCurriculumRepository.GetCohortCurriculumByCohort(new List<string> { studentProfile.Cohort });

                if (cohortCurriculums.Any())
                {
                    var studentGrades = cohortCurriculums.Select(cc => new StudentGrade
                    {
                        UserId = user.UserId,
                        CurriculumId = cc.CurriculumId,
                        Semester = cc.Semester,
                        Grade = 0,
                        IsPassed = 0
                    }).ToList();

                    await studentGradeRepository.CreateMultiple(studentGrades);
                }

                return Ok(new
                {
                    results = new
                    {
                        userId = user.UserId,
                        mssv = studentProfileDTO.MSSV,
                        cohort = studentProfile.Cohort,
                        totalCredits = studentProfile.TotalCredits,
                        debtCredits = studentProfile.DebtCredits
                    },
                    status = 200
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = 500, detail = "Internal server error", error = ex.InnerException?.Message ?? ex.Message });
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] StudentProfile studentProfile)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }
            var exist = await studentProfileRepository.GetStudentProfileById(id);
            if (exist == null)
            {
                return NotFound(new { code = 404, detail = "Student profile not found" });
            }

            studentProfile.StudentId = id;
            await studentProfileRepository.Update(studentProfile);
            return Ok(new { result = studentProfile, status = 200 });
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {

            var exist = await studentProfileRepository.GetStudentProfileById(id);
            if (exist == null)
            {
                return NotFound(new { code = 404, detail = "Student profile not found" });
            }

            await studentProfileRepository.Delete(id);
            return Ok(new { status = 200, message = "Delete Success" });
        }


        [HttpPost("import")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is empty or missing." });

            try
            {
                using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
                using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<StudentProfileMap>();
                    csv.Read();
                    csv.ReadHeader();
                    var requiredHeaders = new List<string> { "MSSV", "Cohort" };
                    var missingHeaders = requiredHeaders.Where(h => !csv.HeaderRecord.Contains(h)).ToList();

                    if (missingHeaders.Any())
                    {
                        return BadRequest(new
                        {
                            message = "CSV file is missing required columns!",
                            missingColumns = missingHeaders
                        });
                    }

                    var records = new List<StudentProfileImportDTO>();
                    var invalidRows = new List<int>();
                    int rowIndex = 1;

                    while (csv.Read())
                    {
                        bool isValid = true;
                        if (!csv.TryGetField("MSSV", out string mssv) || string.IsNullOrWhiteSpace(mssv))
                            isValid = false;
                        if (!csv.TryGetField("Cohort", out string cohort) || string.IsNullOrWhiteSpace(cohort))
                            isValid = false;

                        if (!isValid)
                        {
                            invalidRows.Add(rowIndex);
                            rowIndex++;
                            continue;
                        }

                        records.Add(new StudentProfileImportDTO
                        {
                            MSSV = mssv.Trim(),
                            Cohort = cohort.Trim()
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
                    var cohortList = records.Select(r => r.Cohort).Distinct().ToList();

                    var users = await userRepository.GetUserByMSSVList(mssvList);
                    var userIds = users.Select(u => u.UserId).ToList();
                    var cohorts = await cohortCurriculumRepository.GetCohortCurriculumByCohort(cohortList);
                    var existingProfiles = await studentProfileRepository.GetStudentProfilesByUserIds(userIds);

                    var userDict = users.ToDictionary(u => u.MSSV, u => u);
                    var cohortDict = cohorts.GroupBy(c => c.Cohort)
                                            .ToDictionary(g => g.Key, g => g.ToList());
                    var existingProfilesDict = existingProfiles.ToDictionary(sp => sp.UserId, sp => sp);

                    var studentProfiles = new List<StudentProfile>();
                    var studentGrades = new List<StudentGrade>();
                    var missingUserIds = new List<string>();
                    var missingCohorts = new List<string>();

                    foreach (var record in records)
                    {
                        if (!userDict.TryGetValue(record.MSSV, out var user))
                        {
                            missingUserIds.Add(record.MSSV);
                            continue;
                        }

                        if (!cohortDict.TryGetValue(record.Cohort, out var cohortCurriculums))
                        {
                            missingCohorts.Add(record.Cohort);
                            continue;
                        }

                        if (existingProfilesDict.TryGetValue(user.UserId, out var existingProfile))
                        {
                            await studentGradeRepository.DeleteByUserId(existingProfile.UserId);
                            await studentProfileRepository.DeleteByUserId(existingProfile.UserId);
                        }

                        var studentProfile = new StudentProfile
                        {
                            UserId = user.UserId,
                            Cohort = record.Cohort,
                            TotalCredits = 0,
                            DebtCredits = 0
                        };

                        studentProfiles.Add(studentProfile);

                        studentGrades.AddRange(cohortCurriculums.Select(cc => new StudentGrade
                        {
                            UserId = user.UserId,
                            CurriculumId = cc.CurriculumId,
                            Semester = cc.Semester,
                            Grade = 0,
                            IsPassed = 0
                        }));
                    }

                    if (missingUserIds.Count > 0 || missingCohorts.Count > 0)
                    {
                        return BadRequest(new
                        {
                            message = "Some MSSV or Cohort does not exist!",
                            missingUsers = missingUserIds,
                            missingCohorts = missingCohorts
                        });
                    }

                    await studentProfileRepository.ImportStudentProfilesAsync(studentProfiles);

                    if (studentGrades.Any())
                    {
                        await studentGradeRepository.CreateMultiple(studentGrades);
                    }

                    return Ok(new { message = "Student Profiles CSV imported successfully!", count = studentProfiles.Count });
                }
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new
                {
                    message = "Database error occurred while saving student profiles.",
                    error = ex.InnerException?.Message ?? ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "An unexpected error occurred.",
                    error = ex.Message
                });
            }
        }

    }
}