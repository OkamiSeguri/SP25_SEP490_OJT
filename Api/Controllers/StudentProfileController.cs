using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class StudentProfileController : ControllerBase
    {
        private readonly IStudentProfileRepository studentProfileRepository;
        public StudentProfileController()
        {
            studentProfileRepository = new StudentProfileRepository();
        }
        // GET: api/<StudentProfileController>
        [HttpGet]
        public async Task<IEnumerable<StudentProfile>> Get()
        {
            var student = await studentProfileRepository.GetStudentProfileAll();
            return student;
        }

        // GET api/<StudentProfileController>/5
        [HttpGet("{userId}/{studentId}")]
        public async Task<StudentProfile> GetStudentProfile(int userId, int studentId)
        {
            var student = await studentProfileRepository.GetStudentProfile(userId,studentId);
            return student;
        }     
        [HttpGet("profile/{id}")]
        public async Task<StudentProfile> GetStudentProfileById(int id)
        {
            var student = await studentProfileRepository.GetStudentProfileById(id);
            return student;
        }      
        [HttpGet("major/{major}")]
        public async Task<StudentProfile> GetStudentByMajor(string major)
        {
            var student = await studentProfileRepository.GetStudentProfileByMajor(major);
            return student;
        }

        // POST api/<StudentProfileController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] StudentProfile studentProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await studentProfileRepository.Create(studentProfile);
            return Ok(studentProfile);
        }

        // PUT api/<StudentProfileController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] StudentProfile studentProfile)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = await studentProfileRepository.GetStudentProfileById(id);
            if (exist == null)
            {
                return NotFound();
            }
            studentProfile.StudentId = id;
            await studentProfileRepository.Update(studentProfile);
            return Ok("Updatse Success");
        }

        // DELETE api/<StudentProfileController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await studentProfileRepository.GetStudentProfileById(id);
            if (exist == null)
            {
                return NotFound();
            }
            await studentProfileRepository.Delete(id);
            return Ok("Delete Success");
        }
    }
}
