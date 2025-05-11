using BusinessObject;
using FOMSOData.Authorize;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OJTResultController : ControllerBase
    {
        private readonly IOJTResultRepository ojtResultRepository;

        public OJTResultController()
        {
            ojtResultRepository = new OJTResultRepository();
        }

        // Lấy tất cả kết quả OJT
        [CustomAuthorize("0", "1")]
        [HttpGet]
        public async Task<IEnumerable<OJTResult>> Get()
            => await ojtResultRepository.GetOJTResultAll();

        // Lấy chi tiết kết quả OJT theo ID (role "0", "1")
        [CustomAuthorize("0", "1")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OJTResult>> Get(int id)
        {
            var result = await ojtResultRepository.GetOJTResultById(id);
            return result == null ? NotFound() : Ok(result);
        }

        // Lấy danh sách kết quả theo chương trình OJT (ProgramId)
        [CustomAuthorize("0", "1", "2")]
        [HttpGet("program/{programId}")]
        public async Task<IEnumerable<OJTResult>> GetByProgramId(int programId)
            => await ojtResultRepository.GetByProgramId(programId);

        // Lấy danh sách kết quả OJT theo công ty (EnterpriseId)
        [CustomAuthorize("0", "1", "2")]
        [HttpGet("enterprise/{enterpriseId}")]
        public async Task<IEnumerable<OJTResult>> GetByEnterpriseId(int enterpriseId)
            => await ojtResultRepository.GetResultsByEnterpriseId(enterpriseId);

        // Lấy danh sách kết quả OJT theo trạng thái (Passed/Failed/etc.) 
        [CustomAuthorize("1", "2")]
        [HttpGet("status/{status}")]
        public async Task<IEnumerable<OJTResult>> GetByStatus(string status)
            => await ojtResultRepository.GetResultsByStatus(status);

        // Đếm số lượng kết quả theo trạng thái 
        [CustomAuthorize("1", "2")]
        [HttpGet("count/status/{status}")]
        public async Task<ActionResult<int>> CountByStatus(string status)
        {
            var count = await ojtResultRepository.CountByStatus(status);
            return Ok(count);
        }

        // Thêm mới kết quả OJT (chỉ cho phép doanh nghiệp thêm - role "2")
        [CustomAuthorize("2")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OJTResult ojtResult)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await ojtResultRepository.Create(ojtResult);
            return Ok(ojtResult);
        }

        // Cập nhật kết quả OJT theo ID (chỉ cho phép doanh nghiệp - role "2")
        [CustomAuthorize("2")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] OJTResult ojtResult)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exist = await ojtResultRepository.GetOJTResultById(id);
            if (exist == null)
                return NotFound();

            ojtResult.ResultId = id;
            await ojtResultRepository.Update(ojtResult);
            return Ok(ojtResult);
        }

        // Xoá kết quả OJT theo ID (chỉ cho phép doanh nghiệp - role "2")
        [CustomAuthorize("2")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await ojtResultRepository.GetOJTResultById(id);
            if (exist == null)
                return NotFound();

            await ojtResultRepository.Delete(id);
            return Ok("Delete Success");
        }
    }
}
