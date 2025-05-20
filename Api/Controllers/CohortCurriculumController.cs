using BusinessObject;
using CsvHelper;
using FOMSOData.Authorize;
using FOMSOData.Mappings;
using FOMSOData.Models;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using System.Globalization;
using System.Text;

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuthorize("1")]
    public class CohortCurriculumController : ControllerBase
    {
        private readonly ICohortCurriculumRepository cohortCurriculumRepository;
        private readonly ICurriculumRepository curriculumRepository;

        public CohortCurriculumController()
        {
            cohortCurriculumRepository = new CohortCurriculumRepository();
            curriculumRepository = new CurriculumRepository();
        }

        // 🔹 GET all CohortCurriculum
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CohortCurriculum>>> Get()
        {
            var cohort = await cohortCurriculumRepository.GetCohortCurriculumAll();
            if (cohort == null)
            {
                return NotFound(new { code = 404, detail = "No cohort curriculum found" });
            }

            return Ok(new
            {
                results = cohort.Select(u => new
                {
                    cohort = u.Cohort,
                    curriculumId = u.CurriculumId,
                    semester = u.Semester,
                }),
                status = StatusCodes.Status200OK
            });
        }

        // 🔹 GET CohortCurriculum by Cohort and CurriculumId
        [HttpGet("{cohort}/{curriculumId}")]
        public async Task<ActionResult<CohortCurriculum>> Get(string cohort, int curriculumId)
        {
            var cohorts = await cohortCurriculumRepository.GetCohortCurriculum(cohort, curriculumId);
            if (cohorts == null)
            {
                return NotFound(new { code = 404, detail = "Curriculum not found." });
            }

            return Ok(new
            {
                result = new
                {
                    cohort = cohorts.Cohort,
                    curriculumId = cohorts.CurriculumId,
                    semester = cohorts.Semester,
                },
                status = StatusCodes.Status200OK
            });
        }

        // 🔹 CREATE CohortCurriculum
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CohortCurriculumImportDTO dto)
        {
            if (!ModelState.IsValid || dto.Semester <= 0)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data. Semester must be greater than 0." });
            }

            var curriculum = await curriculumRepository.GetCurriculumBySubjectCode(dto.SubjectCode);
            if (curriculum == null)
            {
                return NotFound(new { code = 404, detail = "Not found with given subjectCode." });
            }

            var existing = await cohortCurriculumRepository
                .GetCohortCurriculum(dto.Cohort, curriculum.CurriculumId);

            if (existing != null)
            {
                return Conflict(new
                {
                    code = 409,
                    detail = "CohortCurriculum already exists with the same Cohort and CurriculumId."
                });
            }

            var cohortCurriculum = new CohortCurriculum
            {
                Cohort = dto.Cohort,
                CurriculumId = curriculum.CurriculumId,
                Semester = dto.Semester
            };

            await cohortCurriculumRepository.Create(cohortCurriculum);

            return Ok(new
            {
                result = new
                {
                    cohort = cohortCurriculum.Cohort,
                    subjectCode = dto.SubjectCode,
                    semester = cohortCurriculum.Semester,
                },
                status = StatusCodes.Status200OK
            });
        }


        // 🔹 UPDATE CohortCurriculum
        [HttpPut("{cohort}/{curriculumId}")]
        public async Task<ActionResult> Put(string cohort, int curriculumId, [FromBody] CohortCurriculum cohortCurriculum)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }

            var exist = await cohortCurriculumRepository.GetCohortCurriculum(cohort, curriculumId);
            if (exist == null)
            {
                return NotFound(new { code = 404, detail = "Curriculum not found." });
            }

            await cohortCurriculumRepository.Update(cohortCurriculum);
            return Ok(new { result = cohortCurriculum, status = StatusCodes.Status200OK });
        }

        // 🔹 DELETE CohortCurriculum
        [HttpDelete("{cohort}/{curriculumId}")]
        public async Task<ActionResult> Delete(string cohort, int curriculumId)
        {
            var exist = await cohortCurriculumRepository.GetCohortCurriculum(cohort, curriculumId);
            if (exist == null)
            {
                return NotFound(new { code = 404, detail = "Curriculum not found." });
            }

            await cohortCurriculumRepository.Delete(cohort, curriculumId);
            return Ok(new { message = "Delete Success", status = StatusCodes.Status200OK });
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportCohortCurriculumFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is empty or missing." });

            var extension = Path.GetExtension(file.FileName).ToLower();
            var importRecords = new List<CohortCurriculumImportDTO>();
            var invalidRows = new List<int>();

            if (extension == ".csv")
            {
                using var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
                using var csv = new CsvReader(stream, CultureInfo.InvariantCulture);
                csv.Context.RegisterClassMap<CohortCurriculumMap>();
                csv.Read();
                csv.ReadHeader();

                var requiredHeaders = new List<string> { "Cohort", "SubjectCode", "Semester" };
                var missingHeaders = requiredHeaders.Where(h => !csv.HeaderRecord.Contains(h)).ToList();
                if (missingHeaders.Any())
                    return BadRequest(new { message = "Missing required headers", missingColumns = missingHeaders });

                int rowIndex = 1;
                while (csv.Read())
                {
                    bool isValid = true;

                    string cohort = null;
                    string subjectCode = null;
                    int semester = 0;

                    if (!csv.TryGetField("Cohort", out cohort) || string.IsNullOrWhiteSpace(cohort))
                        isValid = false;

                    if (!csv.TryGetField("SubjectCode", out subjectCode) || string.IsNullOrWhiteSpace(subjectCode))
                        isValid = false;

                    if (!csv.TryGetField("Semester", out string semesterStr) || !int.TryParse(semesterStr, out semester))
                        isValid = false;

                    if (!isValid)
                    {
                        invalidRows.Add(rowIndex++);
                        continue;
                    }

                    importRecords.Add(new CohortCurriculumImportDTO
                    {
                        Cohort = cohort.Trim(),
                        SubjectCode = subjectCode.Trim(),
                        Semester = semester
                    });

                    rowIndex++;
                }
            }
            else if (extension == ".xlsx")
            {
                using var workbook = new ClosedXML.Excel.XLWorkbook(file.OpenReadStream());
                var worksheet = workbook.Worksheets.First();
                var requiredHeaders = new List<string> { "Cohort", "SubjectCode", "Semester" };

                // Lấy header từ dòng 1
                var headers = worksheet.Row(1).CellsUsed().Select(c => c.GetString().Trim()).ToList();
                var missingHeaders = requiredHeaders.Where(h => !headers.Contains(h)).ToList();
                if (missingHeaders.Any())
                    return BadRequest(new { message = "Missing required headers", missingColumns = missingHeaders });

                int rowIndex = 2; // bắt đầu từ dòng dữ liệu thứ 2
                while (true)
                {
                    var row = worksheet.Row(rowIndex);
                    if (row.IsEmpty())
                        break; // hết dữ liệu

                    string cohort = row.Cell(headers.IndexOf("Cohort") + 1).GetString();
                    string subjectCode = row.Cell(headers.IndexOf("SubjectCode") + 1).GetString();
                    string semesterStr = row.Cell(headers.IndexOf("Semester") + 1).GetString();

                    bool isValid = true;
                    if (string.IsNullOrWhiteSpace(cohort))
                        isValid = false;
                    if (string.IsNullOrWhiteSpace(subjectCode))
                        isValid = false;

                    if (!int.TryParse(semesterStr, out int semester))
                        isValid = false;

                    if (!isValid)
                    {
                        invalidRows.Add(rowIndex);
                        rowIndex++;
                        continue;
                    }

                    importRecords.Add(new CohortCurriculumImportDTO
                    {
                        Cohort = cohort.Trim(),
                        SubjectCode = subjectCode.Trim(),
                        Semester = semester
                    });

                    rowIndex++;
                }
            }
            else
            {
                return BadRequest(new { message = "Unsupported file format. Please upload CSV or XLSX." });
            }

            if (invalidRows.Any())
            {
                return BadRequest(new { message = "Invalid rows", invalidRows });
            }

            var subjectCodes = importRecords.Select(r => r.SubjectCode).Distinct().ToList();
            var curriculums = await curriculumRepository.GetCurriculumBySubjectCodeList(subjectCodes);
            var curriculumDict = curriculums.ToDictionary(c => c.SubjectCode, c => c.CurriculumId);

            var missingSubjectCodes = subjectCodes.Where(code => !curriculumDict.ContainsKey(code)).ToList();
            if (missingSubjectCodes.Any())
            {
                return BadRequest(new { message = "SubjectCode not found", missingSubjectCodes });
            }

            var cohortCurriculums = importRecords.Select(r => new CohortCurriculum
            {
                Cohort = r.Cohort,
                CurriculumId = curriculumDict[r.SubjectCode],
                Semester = r.Semester
            }).ToList();

            await cohortCurriculumRepository.ImportCohortCurriculum(cohortCurriculums);
            return Ok(new { message = "Import successful", count = cohortCurriculums.Count });
        }


    }
}
