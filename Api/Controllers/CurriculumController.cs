using BusinessObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class CurriculumController : ControllerBase
    {
        private readonly ICurriculumRepository curriculumRepository;

        public CurriculumController()
        {
            curriculumRepository = new CurriculumRepository();


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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Curriculum>>> Get()
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
                var curriculum = await curriculumRepository.GetCurriculumAll();

                if (curriculum == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "No curriculum found"
                    });
                }
                return Ok(new
                {
                    results = curriculum.Select(u => new
                    {
                        id = u.CurriculumId,
                        subjectCode = u.SubjectCode,
                        subjectName = u.SubjectName,
                        credits = u.Credits
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

        [HttpGet("{id}")]
        public async Task<ActionResult<Curriculum>> Get(int id)
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
                var curriculum = await curriculumRepository.GetCurriculumById(id);
                if (curriculum == null)
                {
                    return NotFound(new { code = 404, detail = "Curriculum not found." });
                }
                return Ok(curriculum);
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

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Curriculum curriculum)
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
                await curriculumRepository.Create(curriculum);
                return Ok(new {  result = curriculum, status = 200});
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

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Curriculum curriculum)
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
                return BadRequest(new
                {
                    code = StatusCodes.Status400BadRequest,
                    detail = "Invalid request data."
                });
            }

            try
            {
                var exist = await curriculumRepository.GetCurriculumById(id);
                if (exist == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "Curriculum not found"
                    });
                }

                curriculum.CurriculumId = id;
                await curriculumRepository.Update(curriculum);
                return Ok(new { result = curriculum, status = 200 });
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
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
                var exist = await curriculumRepository.GetCurriculumById(id);
                if (exist == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "Curriculum not found"
                    });
                }
                await curriculumRepository.Delete(id);
                return Ok(new { status = 200, message = "Delete Success" });
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
    }
}