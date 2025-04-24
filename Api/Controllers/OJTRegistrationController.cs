using BusinessObject;
using FOMSOData.Authorize;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OJTRegistrationController : ControllerBase
    {
        private readonly IOJTRegistrationRepository ojtRegistrationRepository;
        public OJTRegistrationController()
        {
            ojtRegistrationRepository = new OJTRegistrationRepository();
        }
        // GET: api/<OJTRegistrationController>
        [CustomAuthorize("2")]
        [HttpGet]
        public async Task<IEnumerable<OJTRegistration>> Get()
        {
            var ojtRegistration = await ojtRegistrationRepository.GetOJTRegistrationAll();
            return ojtRegistration;
        }
        [CustomAuthorize("2")]
        [HttpGet("{id}")]
        public async Task<OJTRegistration> Get(int id)
        {
            var ojtRegistration = await ojtRegistrationRepository.GetOJTRegistrationById(id);
            return ojtRegistration;
        }
        // POST api/<OJTRegistrationController>
        [CustomAuthorize("0")]
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
        [CustomAuthorize("0")]
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

            // Kiểm tra xem sinh viên có quyền chỉnh sửa không
            var userId = User.Identity.Name; // Giả sử lấy User ID từ token
            if (exist.StudentId.ToString() != userId)
            {
                return Forbid(); // Không cho phép chỉnh sửa nếu không phải của chính mình
            }

            ojtRegistration.RegistrationId = id;
            await ojtRegistrationRepository.Update(ojtRegistration);
            return Ok(ojtRegistration);
        }
        // DELETE api/<OJTRegistrationController>/5
        [CustomAuthorize("0")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await ojtRegistrationRepository.GetOJTRegistrationById(id);
            if (exist == null)
            {
                return NotFound();
            }

            // Kiểm tra xem sinh viên có quyền xóa không
            var userId = User.Identity.Name;
            if (exist.StudentId.ToString() != userId)
            {
                return Forbid();
            }

            await ojtRegistrationRepository.Delete(id);
            return Ok("Delete Success");
        }
    }
}
