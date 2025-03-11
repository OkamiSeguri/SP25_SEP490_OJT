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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
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
        // GET: api/<CohortCurriculumController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CohortCurriculum>>> Get()
        {

                var cohort = await cohortCurriculumRepository.GetCohortCurriculumAll();
                if (cohort == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "No cohort curriculum found"
                    });
                }
                return Ok(new
                {
                    results = cohort.Select(u => new
                    {
                        cohortcurriculumId = u.CohortCurriculumId,
                        cohort = u.Cohort,
                        curriculumId = u.CurriculumId,
                        semester = u.Semester,
                    }),
                    status = StatusCodes.Status200OK
                });
            }


        // GET api/<CohortCurriculumController>/5
        [HttpGet("{cohortcurriculumId}")]
        public async Task<ActionResult<CohortCurriculum>> Get(int cohortcurriculumId)
        {

            var cohorts = await cohortCurriculumRepository.GetCohortCurriculum(cohortcurriculumId);
            if (cohorts == null)
            {
                return NotFound(new { code = 404, detail = "Curriculum not found." });
            }
            return Ok(new
                {
                    result = new
                    {
                        cohortcurriculumId = cohortcurriculumId,
                        cohort = cohorts.Cohort,
                        curriculumId = cohorts.CurriculumId,
                        semester = cohorts.Semester,

                    },
                    status = StatusCodes.Status200OK
                });
        }

        // POST api/<CohortCurriculumController>
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

            // PUT api/<CohortCurriculumController>/5
            [HttpPut("{cohortcurriculumId}")]
        public async Task<ActionResult> Put(int cohortcurriculumId, [FromBody] CohortCurriculum cohortCurriculum)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }

                var exist = await cohortCurriculumRepository.GetCohortCurriculum(cohortcurriculumId);
            if (exist == null)
            {
                return NotFound();
            }

            await cohortCurriculumRepository.Update(cohortCurriculum);
                return Ok(new { result = cohortCurriculum, status = 200 });

            }

        // DELETE api/<CohortCurriculumController>/5
        [HttpDelete("{cohortcurriculumId}")]
        public async Task<ActionResult> Delete(int cohortcurriculumId)
        {
            var exist = await cohortCurriculumRepository.GetCohortCurriculum(cohortcurriculumId);
            if (exist == null)
            {
                return NotFound();
            }
            await cohortCurriculumRepository.Delete(cohortcurriculumId);
            return Ok("Delete Success");
        }
        [HttpPost("import")]
        public async Task<IActionResult> ImportCsv(IFormFile file)
        {

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is empty or missing." });

            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<CohortCurriculumMap>();
                var records = csv.GetRecords<CohortCurriculum>().ToList();
                var existingCurriculumIds = (await curriculumRepository.GetAllIds()).ToHashSet();

                // 🚨 Kiểm tra xem dữ liệu import có CurriculumId nào không tồn tại không
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
