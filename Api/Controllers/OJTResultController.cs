using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class OJTResultController : ControllerBase
    {
        private readonly IOJTResultRepository ojtResultRepository;
        public OJTResultController()
        {
            ojtResultRepository = new OJTResultRepository();
        }
        // GET: api/<OJTResultController>
        [HttpGet]
        public async Task<IEnumerable<OJTResult>> Get()
        {
            var ojtResult = await ojtResultRepository.GetOJTResultAll();
            return ojtResult;
        }
        // GET api/<OJTResultController>/5
        [HttpGet("{id}")]
        public async Task<OJTResult> Get(int id)
        {
            var ojtResult = await ojtResultRepository.GetOJTResultById(id);
            return ojtResult;
        }
        // POST api/<OJTResultController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OJTResult ojtResult)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await ojtResultRepository.Create(ojtResult);
            return Ok(ojtResult);
        }
        // PUT api/<OJTResultController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] OJTResult ojtResult)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = await ojtResultRepository.GetOJTResultById(id);
            if (exist == null)
            {
                return NotFound();
            }
            ojtResult.OJTId = id;
            await ojtResultRepository.Update(ojtResult);
            return Ok(ojtResult);
        }
        // DELETE api/<OJTResultController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await ojtResultRepository.GetOJTResultById(id);
            if (exist == null)
            {
                return NotFound();
            }
            await ojtResultRepository.Delete(id);
            return Ok();
        }
    }
}
