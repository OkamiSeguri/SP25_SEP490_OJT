using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class StudentGradeController : ControllerBase
    {
        private readonly IStudentGradeRepository studentGradeRepository;
        public StudentGradeController()
        {
            studentGradeRepository = new StudentGradeRepository();
        }

        // GET: api/<StudentGradeController>
        [HttpGet]
        public async Task<IEnumerable<StudentGrade>> Get()
        {
            var grade = await studentGradeRepository.GetGradesAll();
            return grade;
        }
        [HttpGet("{userId}")]
        public async Task<StudentGrade> GetByUserId(int userId)
        {
            var grade = await studentGradeRepository.GetGradeByUserId(userId);
            return grade; 
        }
        // GET api/<StudentGradeController>/5
        [HttpGet("{userId}/{curriculumId}")]
        public async Task<StudentGrade> Get(int userId, int curriculumId)
        {
            var grade = await studentGradeRepository.GetGrade(userId, curriculumId);
            return grade;
        }

        // POST api/<StudentGradeController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] StudentGrade studentGrade)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await studentGradeRepository.Create(studentGrade);
            return Ok(studentGrade);
        }

        // PUT api/<StudentGradeController>/5
        [HttpPut("{userId}/{curriculumId}")]
        public async Task<ActionResult> Put(int userId, int curriculumId, [FromBody] StudentGrade studentGrade)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = await studentGradeRepository.GetGrade(userId, curriculumId);
            if (exist == null)
            {
                return NotFound();
            }
            studentGrade.UserId = userId;
            studentGrade.CurriculumId = curriculumId;
            await studentGradeRepository.Update(studentGrade);
            return Ok("Updatse Success");
        }

        // DELETE api/<StudentGradeController>/5
        [HttpDelete("{userId}/{curriculumId}")]
        public async Task<ActionResult> DeleteGrade(int userId, int curriculumId)
        {
            await studentGradeRepository.Delete(userId, curriculumId);
            return NoContent();
        }
    }
}
