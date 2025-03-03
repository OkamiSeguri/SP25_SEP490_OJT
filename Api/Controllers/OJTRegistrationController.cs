using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class OJTRegistrationController : ControllerBase
    {
        private readonly IOJTRegistrationRepository oJTRegistrationRepository;
        public OJTRegistrationController()
        {
            oJTRegistrationRepository = new OJTRegistrationRepository();
        }
        // GET: api/<OJTRegistrationController>
        [HttpGet]
        public async Task<IEnumerable<OJTRegistration>> Get()
        {
            var oJTRegistration = await oJTRegistrationRepository.GetOJTRegistrationAll();
            return oJTRegistration;
        }
        // GET api/<OJTRegistrationController>/5
        [HttpGet]
        [Route("GetOJTRegistrationById/{id}")]
        public async Task<OJTRegistration> Get(int id)
        {
            var oJTRegistration = await oJTRegistrationRepository.GetOJTRegistrationById(id);
            return oJTRegistration;
        }
        // POST api/<OJTRegistrationController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OJTRegistration oJTRegistration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await oJTRegistrationRepository.Create(oJTRegistration);
            return Ok(oJTRegistration);
        }
        // PUT api/<OJTRegistrationController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] OJTRegistration oJTRegistration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = await oJTRegistrationRepository.GetOJTRegistrationById(id);
            if (exist == null)
            {
                return NotFound();
            }
            oJTRegistration.OJTId = id;
            await oJTRegistrationRepository.Update(oJTRegistration);
            return Ok(oJTRegistration);
        }
        // DELETE api/<OJTRegistrationController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await oJTRegistrationRepository.GetOJTRegistrationById(id);
            if (exist == null)
            {
                return NotFound();
            }
            await oJTRegistrationRepository.Delete(id);
            return Ok("Delete Success");
        }
    }
}
