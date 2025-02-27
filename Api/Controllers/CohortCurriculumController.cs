using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class CohortCurriculumController : ControllerBase
    {
        private readonly ICohortCurriculumRepository cohortCurriculumRepository;
        public CohortCurriculumController()
        {
            cohortCurriculumRepository = new CohortCurriculumRepository();
        }
        // GET: api/<CohortCurriculumController>
        [HttpGet]
        public async Task<IEnumerable<CohortCurriculum>> Get()
        {
            var cohort = await cohortCurriculumRepository.GetCohortCurriculumAll();

            return cohort;
        }

        // GET api/<CohortCurriculumController>/5
        [HttpGet("{cohort}")]
        public async Task<CohortCurriculum> Get(string cohort)
        {
            var cohorts = await cohortCurriculumRepository.GetCohortCurriculumByCohort(cohort);
            return cohorts;
        }

        // POST api/<CohortCurriculumController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CohortCurriculum cohortCurriculum)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await cohortCurriculumRepository.Create(cohortCurriculum);
            return Ok(cohortCurriculum);
        }

        // PUT api/<CohortCurriculumController>/5
        [HttpPut("{cohort}")]
        public async Task<ActionResult> Put(string cohort, [FromBody] CohortCurriculum cohortCurriculum)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exist = await cohortCurriculumRepository.GetCohortCurriculumByCohort(cohort);
            if (exist == null)
            {
                return NotFound();
            }

            // Xóa bản ghi cũ
            await cohortCurriculumRepository.Delete(cohort);

            // Thêm bản ghi mới
            await cohortCurriculumRepository.Create(cohortCurriculum);

            return Ok("Update Success");
        }



        // DELETE api/<CohortCurriculumController>/5
        [HttpDelete("{cohort}")]
        public async Task<ActionResult> Delete(string cohort)
        {
            var exist = await cohortCurriculumRepository.GetCohortCurriculumByCohort(cohort);
            if (exist == null)
            {
                return NotFound();
            }
            await cohortCurriculumRepository.Delete(cohort);
            return Ok("Delete Success");
        }
    }
}
