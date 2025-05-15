using BusinessObject;
using FOMSOData.Authorize;
using FOMSOData.Models;
using FOMSOData.Services;
using Microsoft.AspNetCore.Mvc;
using Repositories;

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OJTProgramController : ControllerBase
    {
        private readonly IOJTProgramRepository ojtProgramRepository;
        private readonly IPhotoService photoService;

        public OJTProgramController(IOJTProgramRepository repo, IPhotoService photoService)
        {
            ojtProgramRepository = repo;
            this.photoService = photoService;
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
        public async Task<ActionResult<OJTProgram>> ApproveRequest(int id)
        {
            var result = await ojtProgramRepository.ApproveRequest(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [CustomAuthorize("3")]
        [HttpPut("reject/{id}")]
        public async Task<ActionResult<OJTProgram>> RejectRequest(int id)
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

        //POST with image upload to Cloudinary
        [CustomAuthorize("2")]
        [HttpPost("with-image")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> PostWithImage([FromForm] OJTProgramCreateDTO dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (dto.StartDate >= dto.EndDate)
                return BadRequest("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");

            string imageUrl;
            try
            {
                imageUrl = await photoService.UploadPhotoAsync(dto.ImageFile, "ojt-programs");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Lỗi khi upload ảnh", detail = ex.Message });
            }

            var ojtProgram = new OJTProgram
            {
                EnterpriseId = dto.EnterpriseId,
                ProgramName = dto.ProgramName,
                Description = dto.Description,
                Requirements = dto.Requirements,
                Status = dto.Status ?? "Pending",
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ImageUrl = imageUrl
            };

            await ojtProgramRepository.Create(ojtProgram);
            return CreatedAtAction(nameof(Get), new { id = ojtProgram.ProgramId }, ojtProgram);
        }

        //PUT with image update
        [CustomAuthorize("2")]
        [HttpPut("with-image/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> PutWithImage(int id, [FromForm] OJTProgramUpdateDTO dto)
        {
            var existing = await ojtProgramRepository.GetOJTProgramById(id);
            if (existing == null) return NotFound();

            if (dto.StartDate >= dto.EndDate)
                return BadRequest("Ngày bắt đầu phải nhỏ hơn ngày kết thúc.");

            // Upload ảnh mới nếu có
            if (dto.ImageFile != null)
            {
                try
                {
                    var imageUrl = await photoService.UploadPhotoAsync(dto.ImageFile, "ojt-programs");
                    existing.ImageUrl = imageUrl;
                }
                catch (Exception ex)
                {
                    return BadRequest(new { error = "Lỗi khi upload ảnh", detail = ex.Message });
                }
            }

            // Cập nhật các thông tin còn lại
            existing.ProgramName = dto.ProgramName;
            existing.Description = dto.Description;
            existing.Requirements = dto.Requirements;
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;

            await ojtProgramRepository.Update(existing);
            return NoContent();
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
