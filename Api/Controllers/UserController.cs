using BusinessObject;
using FOMSDTO;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("odata/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        public UserController()
        {
            userRepository = new UserRepository();
        }
        // GET: api/<UserController>
        [HttpGet]
        public async Task<IEnumerable<User>> Get()
        {
            var user = await userRepository.GetUserAll();
            return user;
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<User>  Get(int id)
        {
            var user = await userRepository.GetUserById(id);
            return user;
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] User user)
        {
            if(!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            await userRepository.Create(user);
            return Ok(user);        
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = await userRepository.GetUserById(id);
            if(exist == null)
            {
                return NotFound();
            }
            user.UserId = id;
            await userRepository.Update(user); 
            return Ok("Updatse Success");
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await userRepository.GetUserById(id);
            if(exist == null)
            {
                return NotFound();
            }
            await userRepository.Delete(id);
            return Ok("Delete Success");
        }

        [HttpPost("Login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginDTO loginDTO)
        {
            var user = await userRepository.ValidateUser(loginDTO.Email, loginDTO.Password);
            if (user == null)
            {
                return Unauthorized("Invalid credentials");
            }
            return Ok(user);
        }
        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var exist = await userRepository.GetUserByEmail(model.Email);
            if (exist != null)
            {
                return BadRequest("Email đã tồn tại.");
            }

            var newUser = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                Password = model.Password,
                Role = model.Role 
            };

            await userRepository.Create(newUser);
            return Ok(newUser);
        }



    }
}
