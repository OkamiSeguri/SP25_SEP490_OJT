using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using Repositories;

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class OJTFeedbackController : ControllerBase
    {
        private readonly IOJTFeedbackRepository ojtFeedbackRepository;
        public OJTFeedbackController()
        {
            ojtFeedbackRepository = new OJTFeedbackRepository();
        }
        // GET: api/<OJTFeedbackController>
        [HttpGet]
        public async Task<IEnumerable<OJTFeedback>> Get()
        {
            var ojtFeedback = await ojtFeedbackRepository.GetOJTFeedbackAll();
            return ojtFeedback;
        }
        [HttpGet]
        public async Task<OJTFeedback> Get(int id)
        {
            var ojtFeedback = await ojtFeedbackRepository.GetOJTFeedbackById(id);
            return ojtFeedback;
        }
        // POST api/<OJTFeedbackController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OJTFeedback ojtFeedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await ojtFeedbackRepository.Create(ojtFeedback);
            return Ok(ojtFeedback);
        }
        // PUT api/<OJTFeedbackController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] OJTFeedback ojtFeedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = await ojtFeedbackRepository.GetOJTFeedbackById(id);
            if (exist == null)
            {
                return NotFound();
            }
            ojtFeedback.FeedbackId = id;
            await ojtFeedbackRepository.Update(ojtFeedback);
            return Ok(ojtFeedback);
        }
        // DELETE api/<OJTFeedbackController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await ojtFeedbackRepository.GetOJTFeedbackById(id);
            if (exist == null)
            {
                return NotFound();
            }
            await ojtFeedbackRepository.Delete(id);
            return Ok("Delete Success");
        }
    }
}
