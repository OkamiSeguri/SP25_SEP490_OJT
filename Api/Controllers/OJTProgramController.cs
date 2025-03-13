using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using Repositories;

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class OJTProgramController : ControllerBase
    {
        private readonly IOJTProgramRepository ojtProgramRepository;
        public OJTProgramController()
        {
            ojtProgramRepository = new OJTProgramRepository();
        }
        // GET: api/<OJTProgramController>
        [HttpGet]
        public async Task<IEnumerable<OJTProgram>> Get()
        {
            var ojtProgram = await ojtProgramRepository.GetOJTProgramAll();
            return ojtProgram;
        }
        [HttpGet]
        public async Task<OJTProgram> Get(int id)
        {
            var ojtProgram = await ojtProgramRepository.GetOJTProgramById(id);
            return ojtProgram;
        }
        // POST api/<OJTProgramController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OJTProgram ojtProgram)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await ojtProgramRepository.Create(ojtProgram);
            return Ok(ojtProgram);
        }
        // PUT api/<OJTProgramController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] OJTProgram ojtProgram)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = await ojtProgramRepository.GetOJTProgramById(id);
            if (exist == null)
            {
                return NotFound();
            }
            ojtProgram.ProgramId = id;
            await ojtProgramRepository.Update(ojtProgram);
            return Ok(ojtProgram);
        }
        // DELETE api/<OJTProgramController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await ojtProgramRepository.GetOJTProgramById(id);
            if (exist == null)
            {
                return NotFound();
            }
            await ojtProgramRepository.Delete(id);
            return Ok("Delete Success");
        }
    }
}
