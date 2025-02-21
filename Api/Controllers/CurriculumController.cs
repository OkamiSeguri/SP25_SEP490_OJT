using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Repositories;

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
        // GET: api/<CurriculumController>
        [HttpGet]
        public async Task<IEnumerable<Curriculum>> Get()
        {
            var curriculum = await curriculumRepository.GetCurriculumAll();
            return curriculum;
        }

        // GET api/<CurriculumController>/5
        [HttpGet("{id}")]
        public async Task<Curriculum> Get(int id)
        {
            var curriculum = await curriculumRepository.GetCurriculumById(id);
            return curriculum;
        }

        // POST api/<CurriculumController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Curriculum curriculum)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await curriculumRepository.Create(curriculum);
            return Ok(curriculum);
        }
        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Curriculum curriculum)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = await curriculumRepository.GetCurriculumById(id);
            if (exist == null)
            {
                return NotFound();
            }
            curriculum.CurriculumId = id;
            await curriculumRepository.Update(curriculum);
            return Ok("Updatse Success");
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await curriculumRepository.GetCurriculumById(id);
            if (exist == null)
            {
                return NotFound();
            }
            await curriculumRepository.Delete(id);
            return Ok("Delete Success");
        }
    }
}
