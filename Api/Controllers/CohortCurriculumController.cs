using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using System.Security.Claims;

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
        private bool IsAuthenticated()
        {
            return HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated;
        }

        private bool IsAuthorized()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            return roleClaim != null && roleClaim.Value == "1";
        }
        // GET: api/<CohortCurriculumController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CohortCurriculum>>> Get()
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
                        cohort = u.Cohort,
                        curriculumId = u.CurriculumId,
                        semester = u.Semester,
                    }),
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

        // GET api/<CohortCurriculumController>/5
        [HttpGet("{cohort}/{curriculumId}")]
        public async Task<ActionResult<CohortCurriculum>> Get(string cohort,int curriculumId)
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = StatusCodes.Status500InternalServerError,
                    detail = "Internal server error",
                });
            }
        }

        // POST api/<CohortCurriculumController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CohortCurriculum cohortCurriculum)
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
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    code = StatusCodes.Status500InternalServerError,
                    detail = "Internal server error",
                });
            }
        }
            // PUT api/<CohortCurriculumController>/5
            [HttpPut("{cohort}/{curriculumId}")]
        public async Task<ActionResult> Put(string cohort,int curriculumId, [FromBody] CohortCurriculum cohortCurriculum)
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
                var exist = await cohortCurriculumRepository.GetCohortCurriculum(cohort,curriculumId);
            if (exist == null)
            {
                return NotFound();
            }

            await cohortCurriculumRepository.Delete(cohort,curriculumId);

            await cohortCurriculumRepository.Create(cohortCurriculum);
                return Ok(new { result = cohortCurriculum, status = 200 });

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




        // DELETE api/<CohortCurriculumController>/5
        [HttpDelete("{cohort}/{curriculumId}")]
        public async Task<ActionResult> Delete(string cohort,int curriculumId)
        {
            var exist = await cohortCurriculumRepository.GetCohortCurriculum(cohort,curriculumId);
            if (exist == null)
            {
                return NotFound();
            }
            await cohortCurriculumRepository.Delete(cohort, curriculumId);
            return Ok("Delete Success");
        }
    }
}
