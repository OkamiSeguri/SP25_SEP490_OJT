using BusinessObject;
using FOMSOData.Authorize;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OJTFeedbackController : ControllerBase
    {
        private readonly IOJTFeedbackRepository ojtFeedbackRepository;

        public OJTFeedbackController()
        {
            ojtFeedbackRepository = new OJTFeedbackRepository();
        }

        [HttpGet]
        public async Task<IEnumerable<OJTFeedback>> Get()
        {
            return await ojtFeedbackRepository.GetOJTFeedbackAll();
        }

        [CustomAuthorize("0", "2")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OJTFeedback>> Get(int id)
        {
            var ojtFeedback = await ojtFeedbackRepository.GetOJTFeedbackById(id);
            if (ojtFeedback == null)
            {
                return NotFound();
            }
            return ojtFeedback;
        }

        //Lấy feedback theo OJTId
        // GET api/OJTFeedback/ojt/5
        [CustomAuthorize("0", "1", "2")]
        [HttpGet("ojt/{ojtId}")]
        public async Task<IEnumerable<OJTFeedback>> GetByOjtId(int ojtId)
        {
            return await ojtFeedbackRepository.GetFeedbacksByOJTId(ojtId);
        }

        // Lấy feedback theo chương trình OJT
        // GET api/OJTFeedback/program/5
        [CustomAuthorize("0", "1", "2")]
        [HttpGet("program/{programId}")]
        public async Task<IEnumerable<OJTFeedback>> GetByProgramId(int programId)
        {
            return await ojtFeedbackRepository.GetFeedbacksByProgramId(programId);
        }

        //	Lấy feedback theo doanh nghiệp
        // GET api/OJTFeedback/enterprise/5
        [CustomAuthorize("1", "2")]
        [HttpGet("enterprise/{enterpriseId}")]
        public async Task<IEnumerable<OJTFeedback>> GetByEnterpriseId(int enterpriseId)
        {
            return await ojtFeedbackRepository.GetFeedbacksByEnterpriseId(enterpriseId);
        }

        //Kiểm tra một feedback đã tồn tại chưa
        // GET api/OJTFeedback/exists?ojtId=1&programId=2
        [CustomAuthorize("0", "1", "2")]
        [HttpGet("exists")]
        public async Task<bool> FeedbackExists([FromQuery] int ojtId, [FromQuery] int programId)
        {
            return await ojtFeedbackRepository.FeedbackExists(ojtId, programId);
        }

        [CustomAuthorize("0", "2")]
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

        [CustomAuthorize("0", "2")]
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

        [CustomAuthorize("0", "2")]
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
