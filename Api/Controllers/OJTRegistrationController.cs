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

        // Lấy toàn bộ danh sách đăng ký OJT
        [CustomAuthorize("1", "2", "3")]
        [HttpGet]
        public async Task<IEnumerable<OJTRegistration>> Get()
            => await ojtRegistrationRepository.GetOJTRegistrationAll();

        // Lấy thông tin chi tiết một bản đăng ký OJT theo ID (Chỉ dành cho quyền role "2")
        [CustomAuthorize("1", "2", "3")]
        [HttpGet("{id}")]
        public async Task<ActionResult<OJTRegistration>> GetById(int id)
        {
            var item = await ojtRegistrationRepository.GetOJTRegistrationById(id);
            return item == null ? NotFound() : Ok(item);
        }

        // Lấy danh sách đăng ký OJT đã được duyệt (role "2")
        [CustomAuthorize("1", "2", "3")]
        [HttpGet("approved")]
        public async Task<IEnumerable<OJTRegistration>> GetApproved()
            => await ojtRegistrationRepository.ListApproved();

        // Lấy danh sách đăng ký OJT bị từ chối (role "2")
        [CustomAuthorize("1", "2", "3")]
        [HttpGet("rejected")]
        public async Task<IEnumerable<OJTRegistration>> GetRejected()
            => await ojtRegistrationRepository.ListRejected();

        // Lấy danh sách đăng ký OJT đang chờ duyệt (role "2")
        [CustomAuthorize("1", "2", "3")]
        [HttpGet("pending")]
        public async Task<IEnumerable<OJTRegistration>> GetPending()
            => await ojtRegistrationRepository.ListPending();

        // Lấy danh sách đăng ký OJT theo doanh nghiệp (EnterpriseId) (role "2")
        [CustomAuthorize("1", "2", "3")]
        [HttpGet("enterprise/{enterpriseId}")]
        public async Task<IEnumerable<OJTRegistration>> GetByEnterpriseId(int enterpriseId)
            => await ojtRegistrationRepository.GetByEnterpriseId(enterpriseId);

        // Lấy danh sách đăng ký OJT theo sinh viên (StudentId) (role "2")
        [CustomAuthorize("1", "2", "3")]
        [HttpGet("student/{studentId}")]
        public async Task<IEnumerable<OJTRegistration>> GetByStudentId(int studentId)
            => await ojtRegistrationRepository.GetByStudentId(studentId);

        // Lấy danh sách đăng ký OJT theo chương trình OJT (ProgramId) (role "2")
        [CustomAuthorize("1", "2", "3")]
        [HttpGet("program/{programId}")]
        public async Task<IEnumerable<OJTRegistration>> GetByProgramId(int programId)
            => await ojtRegistrationRepository.GetByProgramId(programId);

        // Đếm số lượng đăng ký OJT theo doanh nghiệp (role "2")
        [CustomAuthorize("1", "2", "3")]
        [HttpGet("count/enterprise/{enterpriseId}")]
        public async Task<int> CountByCompanyId(int enterpriseId)
            => await ojtRegistrationRepository.CountByEnterpriseId(enterpriseId);

        // Đếm số lượng đăng ký OJT theo chương trình OJT (role "2")
        [CustomAuthorize("1", "2", "3")]
        [HttpGet("count/program/{programId}")]
        public async Task<int> CountByProgramId(int programId)
            => await ojtRegistrationRepository.CountByProgramId(programId);

        // Lấy trạng thái OJT hiện tại của sinh viên (dành cho role "0" - sinh viên)
        [CustomAuthorize("0")]
        [HttpGet("status/student/{studentId}")]
        public async Task<string> GetCurrentStatusByStudent(int studentId)
            => await ojtRegistrationRepository.GetCurrentStatusByStudentId(studentId);

        // Tạo mới một bản đăng ký OJT (dành cho role "0" - sinh viên)
        [CustomAuthorize("0")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OJTRegistration ojtRegistration)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await ojtRegistrationRepository.Create(ojtRegistration);
            return Ok(ojtRegistration);
        }

        // Cập nhật thông tin đăng ký OJT (chỉ cho phép sinh viên cập nhật bản ghi của chính họ)
        [CustomAuthorize("0")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OJTRegistration ojtRegistration)
        {
            var existing = await ojtRegistrationRepository.GetOJTRegistrationById(id);
            if (existing == null) return NotFound();

            var studentId = User.Identity?.Name;
            if (existing.StudentId.ToString() != studentId) return Forbid();

            ojtRegistration.OJTId = id;
            await ojtRegistrationRepository.Update(ojtRegistration);
            return Ok(ojtRegistration);
        }

        // Thay đổi trạng thái của bản đăng ký OJT (dành cho role "2")
        [CustomAuthorize("2")]
        [HttpPatch("status/{ojtId}")]
        public async Task<IActionResult> ChangeStatus(int ojtId, [FromBody] string newStatus)
        {
            var success = await ojtRegistrationRepository.ChangeStatus(ojtId, newStatus);
            return success ? Ok("Status Updated") : NotFound();
        }

        // Xoá một bản đăng ký OJT (chỉ cho phép sinh viên xoá bản ghi của chính họ)
        [CustomAuthorize("0")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await ojtRegistrationRepository.GetOJTRegistrationById(id);
            if (existing == null) return NotFound();

            var studentId = User.Identity?.Name;
            if (existing.StudentId.ToString() != studentId) return Forbid();

            await ojtRegistrationRepository.Delete(id);
            return Ok("Delete Success");
        }
    }
}
