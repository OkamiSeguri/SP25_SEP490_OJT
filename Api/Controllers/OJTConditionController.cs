using Microsoft.AspNetCore.Mvc;
using Repositories;
using FOMSOData.Models;
using Services;
using FOMSOData.Authorize;
using System.Security.Claims;
using DocumentFormat.OpenXml.Drawing.Charts;
using BusinessObject;
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
        [HttpGet]
        public async Task<IActionResult> GetAllOJTConditions()
        {
            var conditions = await ojtConditionRepository.GetOJTConditionsAll();

            var result = conditions.Select(c => new
            {
                id = c.ConditionId,
                key = c.ConditionKey,
                value = c.ConditionValue,
            }).ToList();

            return Ok(new
            {
                results = result,
                status = StatusCodes.Status200OK
            });
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

            double debtCredits = studentProfile.DebtCredits.GetValueOrDefault();
            double totalCredits = studentProfile.TotalCredits.GetValueOrDefault();
            double calculatedDebtRatio = totalCredits > 0 ? debtCredits / totalCredits : 0;

            var failedMandatorySubjects = await studentProfileRepository.GetFailedMandatorySubjectsAsync(userId);

            double maxDebtRatio = await ojtConditionRepository.GetMaxDebtRatioAsync();
            bool checkFailedSubjects = await ojtConditionRepository.ShouldCheckFailedSubjectsAsync();
            double debtRatio = 0;
            if (studentProfile.TotalCredits.GetValueOrDefault() > 0)
            {
                debtRatio = (double)studentProfile.DebtCredits.GetValueOrDefault()
                            / studentProfile.TotalCredits.GetValueOrDefault();
            }

            int debtPercent = (int)(debtRatio * 100);
            List<string> reasons = new List<string>();

            if (checkFailedSubjects && failedMandatorySubjects.Any())
                reasons.Add("Failed mandatory subjects.");
            if (debtRatio > maxDebtRatio)
            {
                reasons.Add($"Debt ratio exceeds allowed maximum of {(int)(maxDebtRatio * 100)}%.");
            }
            bool eligible = reasons.Count == 0;

            return Ok(new
            {
                TotalCredits = totalCredits,
                DebtCredits = debtCredits,
                DebtPercent = debtPercent,
                Eligible = eligible,
                Reasons = reasons
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCondition([FromBody] ConditionUpdateRequestDTO request)
        {
            if (request.Key == "MaxDebtRatio")
            {
                if (!double.TryParse(request.Value, out double value) || value <= 0 || value >= 1)
                {
                    return BadRequest(new { message = "MaxDebtRatio must be a number greater than 0 and less than 1." });
                }

                var parts = request.Value.Split('.');
                if (parts.Length == 2 && parts[1].Length > 2)
                {
                    return BadRequest(new { message = "MaxDebtRatio must have at most 2 decimal places." });
                }
            }

            await ojtConditionRepository.UpdateConditionAsync(request.Key, request.Value);
            return Ok(new { message = "Condition updated successfully." });
        }



    }
}
