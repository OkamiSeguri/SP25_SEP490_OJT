using BusinessObject;
using FOMSOData.Authorize;
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
    [CustomAuthorize("1")]

    public class CurriculumController : ControllerBase
    {
        private readonly ICurriculumRepository curriculumRepository;

        public CurriculumController()
        {
            curriculumRepository = new CurriculumRepository();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Curriculum>>> Get()
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


        [HttpGet("{id}")]
        public async Task<ActionResult<Curriculum>> Get(int id)
        {

                var curriculum = await curriculumRepository.GetCurriculumById(id);
                if (curriculum == null)
                {
                    return NotFound(new { code = 404, detail = "Curriculum not found." });
                }
                return Ok(curriculum);
            }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Curriculum curriculum)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }

                await curriculumRepository.Create(curriculum);
                return Ok(new {  result = curriculum, status = 200});
            }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Curriculum curriculum)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    code = StatusCodes.Status400BadRequest,
                    detail = "Invalid request data."
                });
            }

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

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
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

    }
}