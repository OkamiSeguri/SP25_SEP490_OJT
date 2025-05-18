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
        public async Task<ActionResult> Get()
        {
            var ojtPrograms = await ojtProgramRepository.GetOJTProgramAll();
            return Ok(new
            {
                status = 200,
                message = "List of OJT programs",
                data = ojtPrograms
            });
        }

        [CustomAuthorize("0", "1", "2")]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var ojtProgram = await ojtProgramRepository.GetOJTProgramById(id);
            if (ojtProgram == null)
                return NotFound(new { status = 404, message = "OJT program not found" });

            return Ok(new
            {
                status = 200,
                message = "OJT program retrieved successfully",
                data = ojtProgram
            });
        }

        [CustomAuthorize("3")]
        [HttpPut("approve/{id}")]
        public async Task<ActionResult> ApproveRequest(int id)
        {
            var result = await ojtProgramRepository.ApproveRequest(id);
            if (result == null)
                return NotFound(new { status = 404, message = "Program to approve not found" });

            return Ok(new
            {
                status = 200,
                message = "Program approved successfully",
                data = result
            });
        }

        [CustomAuthorize("3")]
        [HttpPut("reject/{id}")]
        public async Task<ActionResult> RejectRequest(int id)
        {
            var result = await ojtProgramRepository.RejectRequest(id);
            if (result == null)
                return NotFound(new { status = 404, message = "Program to reject not found" });

            return Ok(new
            {
                status = 200,
                message = "Program rejected successfully",
                data = result
            });
        }

        [CustomAuthorize("2", "3")]
        [HttpGet("approved")]
        public async Task<ActionResult> ListApproved()
        {
            var approvedPrograms = await ojtProgramRepository.ListApproved();
            return Ok(new
            {
                status = 200,
                message = "List of approved programs",
                data = approvedPrograms
            });
        }

        [CustomAuthorize("2", "3")]
        [HttpGet("pending")]
        public async Task<ActionResult> ListPending()
        {
            var pendingPrograms = await ojtProgramRepository.ListPending();
            return Ok(new
            {
                status = 200,
                message = "List of pending programs",
                data = pendingPrograms
            });
        }

        [CustomAuthorize("2", "3")]
        [HttpGet("rejected")]
        public async Task<ActionResult> ListRejected()
        {
            var rejectedPrograms = await ojtProgramRepository.ListRejected();
            return Ok(new
            {
                status = 200,
                message = "List of rejected programs",
                data = rejectedPrograms
            });
        }

        [CustomAuthorize("2")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OJTProgram ojtProgram)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { status = 400, message = "Invalid data" });

            ojtProgram.Status = "Pending";
            await ojtProgramRepository.Create(ojtProgram);

            return CreatedAtAction(nameof(Get), new { id = ojtProgram.ProgramId }, new
            {
                status = 201,
                message = "Program created successfully",
                data = ojtProgram
            });
        }

        [CustomAuthorize("2")]
        [HttpPost("with-image")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> PostWithImage([FromForm] OJTProgramCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { status = 400, message = "Invalid data" });

            if (dto.StartDate >= dto.EndDate)
                return BadRequest(new { status = 400, message = "Start date must be earlier than end date." });

            string imageUrl;
            try
            {
                imageUrl = await photoService.UploadPhotoAsync(dto.ImageFile, "ojt-programs");
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = 400, message = "Image upload error", detail = ex.Message });
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
            return CreatedAtAction(nameof(Get), new { id = ojtProgram.ProgramId }, new
            {
                status = 201,
                message = "OJT program with image created successfully",
                data = ojtProgram
            });
        }

        [CustomAuthorize("2")]
        [HttpPut("with-image/{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> PutWithImage(int id, [FromForm] OJTProgramUpdateDTO dto)
        {
            var existing = await ojtProgramRepository.GetOJTProgramById(id);
            if (existing == null)
                return NotFound(new { status = 404, message = "Program to update not found" });

            if (dto.StartDate >= dto.EndDate)
                return BadRequest(new { status = 400, message = "Start date must be earlier than end date." });

            if (dto.ImageFile != null)
            {
                try
                {
                    var imageUrl = await photoService.UploadPhotoAsync(dto.ImageFile, "ojt-programs");
                    existing.ImageUrl = imageUrl;
                }
                catch (Exception ex)
                {
                    return BadRequest(new { status = 400, message = "Image upload error", detail = ex.Message });
                }
            }

            existing.ProgramName = dto.ProgramName;
            existing.Description = dto.Description;
            existing.Requirements = dto.Requirements;
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;

            await ojtProgramRepository.Update(existing);
            return Ok(new { status = 200, message = "OJT program updated successfully" });
        }

        [CustomAuthorize("2")]
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] OJTProgram ojtProgram)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { status = 400, message = "Invalid data" });

            var exist = await ojtProgramRepository.GetOJTProgramById(id);
            if (exist == null)
                return NotFound(new { status = 404, message = "Program to update not found" });

            ojtProgram.ProgramId = id;
            await ojtProgramRepository.Update(ojtProgram);
            return Ok(new { status = 200, message = "Program updated successfully" });
        }

        [CustomAuthorize("2")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await ojtProgramRepository.GetOJTProgramById(id);
            if (exist == null)
                return NotFound(new { status = 404, message = "Program to delete not found" });

            await ojtProgramRepository.Delete(id);
            return Ok(new { status = 200, message = "Program deleted successfully" });
        }
    }
}
