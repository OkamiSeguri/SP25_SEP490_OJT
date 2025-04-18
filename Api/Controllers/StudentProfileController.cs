﻿using BusinessObject;
using FOMSOData.Authorize;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;
using System.Security.Claims;

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuthorize("1")]

    public class StudentProfileController : ControllerBase
    {
        private readonly IStudentProfileRepository studentProfileRepository;
        private readonly JWTService jwtService;

        public StudentProfileController(JWTService jwtService)
        {
            studentProfileRepository = new StudentProfileRepository();
            this.jwtService = jwtService;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentProfile>>> Get()
        {
                var students = await studentProfileRepository.GetStudentProfileAll();
                if (students == null || !students.Any())
                {
                    return NotFound(new { code = 404, detail = "No student profiles found" });
                }
                return Ok(new
                {
                    results = students.Select(u => new
                    {
                        userId = u.UserId,
                        studentId = u.StudentId,
                        cohortCurriculumId = u.CohortCurriculumId,
                        totalCredits = u.TotalCredits,
                        debtCredits = u.DebtCredits,
                    }),
                    status = StatusCodes.Status200OK
                });
            }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentProfile>> GetStudentProfile(int id)
        {

                var student = await studentProfileRepository.GetStudentProfileById(id);
                if (student == null)
                {
                    return NotFound(new { code = 404, detail = "Student profile not found" });
                }
                return Ok(new
                {
                    result = new
                    {
                        userId = student.UserId,
                        studentId = student.StudentId,
                        cohortCurriculumId = student.CohortCurriculumId,
                        totalCredits = student.TotalCredits,
                        debtCredits = student.DebtCredits,
                    },
                    status = StatusCodes.Status200OK
                });
            }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] StudentProfile studentProfile)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }
                var existingGrade = await studentProfileRepository.GetStudentProfile(studentProfile.UserId, studentProfile.StudentId);
                if (existingGrade != null)
                {
                    return Conflict(new
                    {
                        code = StatusCodes.Status409Conflict,
                        detail = "Student profile already exists"
                    });
                }
                await studentProfileRepository.Create(studentProfile);
                return Ok(new
                {
                    results =  new
                    {
                        userId = studentProfile.UserId,
                        studentId = studentProfile.StudentId,
                        cohortCurriculumId = studentProfile.CohortCurriculumId,
                        totalCredits = studentProfile.TotalCredits,
                        debtCredits = studentProfile.DebtCredits,
                    },
                    status = StatusCodes.Status200OK
                });
            }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] StudentProfile studentProfile)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }
                var exist = await studentProfileRepository.GetStudentProfileById(id);
                if (exist == null)
                {
                    return NotFound(new { code = 404, detail = "Student profile not found" });
                }

                studentProfile.StudentId = id;
                await studentProfileRepository.Update(studentProfile);
                return Ok(new { result = studentProfile, status = 200 });
            }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {

                var exist = await studentProfileRepository.GetStudentProfileById(id);
                if (exist == null)
                {
                    return NotFound(new { code = 404, detail = "Student profile not found" });
                }

                await studentProfileRepository.Delete(id);
                return Ok(new { status = 200, message = "Delete Success" });
            }

        [HttpGet("check/{userId}")]
        public async Task<IActionResult> CheckOJT(int userId)
        {
            var studentProfile = await studentProfileRepository.GetStudentProfileById(userId);
            if (studentProfile == null)
                return NotFound(new { message = "Student profile not found." });

            double debtRatio = (double)studentProfile.DebtCredits.GetValueOrDefault()
                               / studentProfile.TotalCredits.GetValueOrDefault();
            var failedMandatorySubjects = await studentProfileRepository.GetFailedMandatorySubjectsAsync(userId);

            // List of errors
            List<string> errors = new List<string>();

            if (debtRatio > 0.1)
                errors.Add("Outstanding credits exceed 10% of the total credits.");

            if (failedMandatorySubjects.Any())
                errors.Add("Failed mandatory subjects.");

            if (errors.Count > 0)
                return Ok(new { message = "Not eligible for OJT", reasons = errors });

            return Ok(new { message = "Eligible for OJT." });
        }

    }
}
