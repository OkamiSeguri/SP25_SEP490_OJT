using Microsoft.AspNetCore.Mvc;
using BusinessObject;
using Repositories;

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class OJTRegistrationController : ControllerBase
    {
        private readonly IOJTRegistrationRepository ojtRegistrationRepository;
        public OJTRegistrationController()
        {
            ojtRegistrationRepository = new OJTRegistrationRepository();
        }
        // GET: api/<OJTRegistrationController>
        [HttpGet]
        public async Task<IEnumerable<OJTRegistration>> Get()
        {
            var ojtRegistration = await ojtRegistrationRepository.GetOJTRegistrationAll();
            return ojtRegistration;
        }
        [HttpGet]
        public async Task<OJTRegistration> Get(int id)
        {
            var ojtRegistration = await ojtRegistrationRepository.GetOJTRegistrationById(id);
            return ojtRegistration;
        }
        // POST api/<OJTRegistrationController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OJTRegistration ojtRegistration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await ojtRegistrationRepository.Create(ojtRegistration);
            return Ok(ojtRegistration);
        }
        // PUT api/<OJTRegistrationController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] OJTRegistration ojtRegistration)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = await ojtRegistrationRepository.GetOJTRegistrationById(id);
            if (exist == null)
            {
                return NotFound();
            }
            ojtRegistration.RegistrationId = id;
            await ojtRegistrationRepository.Update(ojtRegistration);
            return Ok(ojtRegistration);
        }
        // DELETE api/<OJTRegistrationController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await ojtRegistrationRepository.Delete(id);
            return Ok("Delete Success");
        }
    }
}
