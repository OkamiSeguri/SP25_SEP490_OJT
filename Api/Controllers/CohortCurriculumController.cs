using BusinessObject;
using CsvHelper;
using FOMSOData.Authorize;
using FOMSOData.Mappings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
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
        public async Task<ActionResult> Post([FromBody] CohortCurriculum cohortCurriculum)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }

            await cohortCurriculumRepository.Create(cohortCurriculum);
            return Ok(new
            {
                result = new
                {
                    cohort = cohortCurriculum.Cohort,
                    curriculumId = cohortCurriculum.CurriculumId,
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

        // 🔹 IMPORT CSV
        [HttpPost("import")]
        public async Task<IActionResult> ImportCohortCurriculumCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is empty or missing." });

            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<CohortCurriculumMap>();
                csv.Read();
                csv.ReadHeader();
                var requiredHeaders = new List<string> { "Cohort", "CurriculumId" };
                var missingHeaders = requiredHeaders.Where(h => !csv.HeaderRecord.Contains(h)).ToList();

                if (missingHeaders.Any())
                {
                    return BadRequest(new
                    {
                        message = "CSV file is missing required columns!",
                        missingColumns = missingHeaders
                    });
                }

                var records = new List<CohortCurriculum>();
                var invalidRows = new List<int>();
                int rowIndex = 1;

                while (csv.Read())
                {
                    bool isValid = true;
                    int curriculumId = 0;

                    if (!csv.TryGetField("Cohort", out string cohort) || string.IsNullOrWhiteSpace(cohort))
                        isValid = false;
                    if (!csv.TryGetField("CurriculumId", out string curriculumIdStr) ||
                        string.IsNullOrWhiteSpace(curriculumIdStr) ||
                        !int.TryParse(curriculumIdStr, out curriculumId))
                        isValid = false;

                    if (!isValid)
                    {
                        invalidRows.Add(rowIndex);
                        rowIndex++;
                        continue;
                    }

                    records.Add(new CohortCurriculum
                    {
                        Cohort = cohort.Trim(),
                        CurriculumId = curriculumId
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

                var existingCurriculumIds = (await curriculumRepository.GetAllIds()).ToHashSet();
                var invalidRecords = records.Where(r => !existingCurriculumIds.Contains(r.CurriculumId)).ToList();
                if (invalidRecords.Any())
                {
                    return BadRequest(new
                    {
                        message = "CurriculumId does not exist in the system.",
                        invalidCurriculumIds = invalidRecords.Select(r => r.CurriculumId).Distinct()
                    });
                }

                await cohortCurriculumRepository.ImportCohortCurriculum(records);
                return Ok(new { message = "Cohort Curriculum CSV imported successfully!", count = records.Count });
            }
        }

    }
}
