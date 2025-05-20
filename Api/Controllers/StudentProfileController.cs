using BusinessObject;
using CsvHelper;
using DataAccess;
using FOMSOData.Authorize;
using FOMSOData.Mappings;
using FOMSOData.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Services;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuthorize("1")]

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

                var student = await studentProfileRepository.GetStudentProfileByUserId(id);
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

            var exist = await studentProfileRepository.GetStudentProfileByUserId(id);
            if (exist == null)
            {
                return NotFound(new { code = 404, detail = "Student profile not found" });
            }

            studentProfile.UserId = id;
            studentProfile.StudentId = exist.StudentId;

            // Lấy CurriculumId tương ứng từ Cohort
            var cohortList = new List<string> { studentProfile.Cohort };
            var cohortCurriculums = await cohortCurriculumRepository.GetCohortCurriculumByCohort(cohortList);

            if (cohortCurriculums == null || cohortCurriculums.Count == 0)
            {
                return BadRequest(new { code = 400, detail = $"No curriculum found for cohort {studentProfile.Cohort}" });
            }

            studentProfile.CurriculumId = cohortCurriculums.First().CurriculumId;

            await studentGradeRepository.DeleteByUserId(studentProfile.UserId); // Nếu bạn muốn giống thằng import
            await studentProfileRepository.Update(studentProfile);

            // Thêm lại grade theo curriculum
            var grades = cohortCurriculums.Select(cc => new StudentGrade
            {
                UserId = studentProfile.UserId,
                CurriculumId = cc.CurriculumId,
                Semester = cc.Semester,
                Grade = 0,
                IsPassed = 0
            }).ToList();

            await studentGradeRepository.CreateMultiple(grades);

            return Ok(new { result = studentProfile, status = 200 });
        }



        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {

                var exist = await studentProfileRepository.GetStudentProfileByUserId(id);
                if (exist == null)
                {
                    return NotFound(new { code = 404, detail = "Student profile not found" });
                }

                await studentProfileRepository.DeleteByUserId(id);
                return Ok(new { status = 200, message = "Delete Success" });
            }


        [HttpPost("import")]
        public async Task<IActionResult> ImportFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is empty or missing." });

            var extension = Path.GetExtension(file.FileName).ToLower();
            var records = new List<StudentProfileImportDTO>();
            var invalidRows = new List<int>();

            try
            {
                if (extension == ".csv")
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
                    }
                }
                else if (extension == ".xlsx")
                {
                    using var stream = file.OpenReadStream();
                    using var workbook = new ClosedXML.Excel.XLWorkbook(stream);
                    var worksheet = workbook.Worksheet(1);

                    var headerMSSV = worksheet.Cell(1, 1).GetString().Trim();
                    var headerCohort = worksheet.Cell(1, 2).GetString().Trim();
                    var requiredHeaders = new List<string> { "MSSV", "Cohort" };

                    if (headerMSSV != "MSSV" || headerCohort != "Cohort")
                    {
                        return BadRequest(new
                        {
                            message = "XLSX file is missing required columns or wrong header names!",
                            missingColumns = requiredHeaders
                        });
                    }

                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                    int rowIndex = 2;
                    foreach (var row in rows)
                    {
                        var mssv = row.Cell(1).GetString();
                        var cohort = row.Cell(2).GetString();

                        bool isValid = !(string.IsNullOrWhiteSpace(mssv) || string.IsNullOrWhiteSpace(cohort));

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
                }
                else
                {
                    return BadRequest(new { message = "Unsupported file format. Please upload CSV or XLSX files only." });
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
                var duplicateMSSVs = new List<string>();

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

                    if (existingProfilesDict.ContainsKey(user.UserId))
                    {
                        duplicateMSSVs.Add(record.MSSV);
                        continue; // bỏ qua MSSV trùng để check hết tất cả MSSV khác
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

                if (duplicateMSSVs.Any())
                {
                    return BadRequest(new
                    {
                        message = "StudentProfile already exists for the following MSSVs:",
                        duplicates = duplicateMSSVs.Distinct().ToList()
                    });
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

                return Ok(new { message = "Student Profiles imported successfully!", count = studentProfiles.Count });
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
