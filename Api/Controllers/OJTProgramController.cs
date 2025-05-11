using BusinessObject;
using FOMSOData.Authorize;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OJTProgramController : ControllerBase
    {
        private readonly IOJTProgramRepository ojtProgramRepository;

        public OJTProgramController()
        {
            ojtProgramRepository = new OJTProgramRepository();
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<OJTProgram>>> Get()
        {
            var ojtPrograms = await ojtProgramRepository.GetOJTProgramAll();
            return Ok(ojtPrograms);
        }

        [CustomAuthorize("0", "1", "2")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OJTProgram>> Get(int id)
        {
            var ojtProgram = await ojtProgramRepository.GetOJTProgramById(id);
            if (ojtProgram == null) return NotFound();
            return Ok(ojtProgram);
        }

        [CustomAuthorize("3")]
        [HttpPut("approve/{id}")]
        public async Task<ActionResult> ApproveRequest(int id)
        {
            var result = await ojtProgramRepository.ApproveRequest(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [CustomAuthorize("3")]
        [HttpPut("reject/{id}")]
        public async Task<ActionResult> RejectRequest(int id)
        {
            var result = await ojtProgramRepository.RejectRequest(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [CustomAuthorize("2", "3")]
        [HttpGet("approved")]
        public async Task<ActionResult<IEnumerable<OJTProgram>>> ListApproved()
        {
            var approvedPrograms = await ojtProgramRepository.ListApproved();
            return Ok(approvedPrograms);
        }

        [CustomAuthorize("2", "3")]
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<OJTProgram>>> ListPending()
        {
            var pendingPrograms = await ojtProgramRepository.ListPending();
            return Ok(pendingPrograms);
        }

        [CustomAuthorize("2", "3")]
        [HttpGet("rejected")]
        public async Task<ActionResult<IEnumerable<OJTProgram>>> ListRejected()
        {
            var rejectedPrograms = await ojtProgramRepository.ListRejected();
            return Ok(rejectedPrograms);
        }

        [CustomAuthorize("2")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OJTProgram ojtProgram)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            ojtProgram.Status = "Pending";

            await ojtProgramRepository.Create(ojtProgram);
            return CreatedAtAction(nameof(Get), new { id = ojtProgram.ProgramId }, ojtProgram);
        }

        [CustomAuthorize("2")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] OJTProgram ojtProgram)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var exist = await ojtProgramRepository.GetOJTProgramById(id);
            if (exist == null) return NotFound();
            ojtProgram.ProgramId = id;
            await ojtProgramRepository.Update(ojtProgram);
            return NoContent();
        }

        [CustomAuthorize("2")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await ojtProgramRepository.GetOJTProgramById(id);
            if (exist == null) return NotFound();
            await ojtProgramRepository.Delete(id);
            return Ok(new { message = "Delete Success" });
        }
    }
}