using Microsoft.AspNetCore.Mvc;
using Repositories;
using FOMSOData.Models;
using Services;
using FOMSOData.Authorize;
using System.Security.Claims;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuthorize("1","0")]

    public class OJTConditionController : ControllerBase
    {
        private readonly IOJTConditionRepository ojtConditionRepository;
        private readonly IStudentProfileRepository studentProfileRepository;
        private readonly JWTService jwtService;
        public OJTConditionController()
        {
            ojtConditionRepository = new OJTConditionRepository();
            studentProfileRepository = new StudentProfileRepository();

        }

        [HttpGet("check/{userId}")]
        public async Task<IActionResult> CheckOJT(int userId)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int tokenUserId))
            {
                return Unauthorized(new { code = 401, detail = "Invalid or missing authentication token" });
            }
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if ((roleClaim != "1" && roleClaim != "2") && tokenUserId != userId)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    code = 403,
                    detail = "You do not have permission"
                });
            }
            var studentProfile = await studentProfileRepository.GetStudentProfileByUserId(userId);
            if (studentProfile == null)
                return NotFound(new { message = "Student profile not found." });

            double debtRatio = (double)studentProfile.DebtCredits.GetValueOrDefault()
                               / studentProfile.TotalCredits.GetValueOrDefault();
            var failedMandatorySubjects = await studentProfileRepository.GetFailedMandatorySubjectsAsync(userId);
            Console.WriteLine($"[LOG] DebtCredits: {studentProfile.DebtCredits}");
            Console.WriteLine($"[LOG] TotalCredits: {studentProfile.TotalCredits}");
            Console.WriteLine($"[LOG] Calculated DebtRatio: {debtRatio}");
            double maxDebtRatio = await ojtConditionRepository.GetMaxDebtRatioAsync();
            bool checkFailedSubjects = await ojtConditionRepository.ShouldCheckFailedSubjectsAsync();
            Console.WriteLine($"[LOG] MaxDebtRatio from DB: {maxDebtRatio}");
            Console.WriteLine($"[LOG] CheckFailedSubjects from DB: {checkFailedSubjects}");
            List<string> errors = new List<string>();

            if (debtRatio > maxDebtRatio)
                errors.Add($"Outstanding credits exceed {maxDebtRatio * 100}% of the total credits.");

            if (checkFailedSubjects && failedMandatorySubjects.Any())
                errors.Add("Failed mandatory subjects.");

            if (errors.Count > 0)
                return Ok(new { message = "Not eligible for OJT", reasons = errors });

            return Ok(new { message = "Eligible for OJT." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCondition([FromBody] ConditionUpdateRequestDTO request)
        {
            await ojtConditionRepository.UpdateConditionAsync(request.Key, request.Value);
            return Ok(new { message = "Condition updated successfully." });
        }

    
    }
}
