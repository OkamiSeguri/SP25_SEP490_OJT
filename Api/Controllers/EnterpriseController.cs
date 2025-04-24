using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnterpriseController : ControllerBase
    {
        private readonly IEnterpriseRepository enterpriseRepository;
        public EnterpriseController()
        {
            enterpriseRepository = new EnterpriseRepository();
        }
        // GET: api/<EnterpriseController>
        [HttpGet]
        public async Task<IEnumerable<Enterprise>> Get()
        {
            var enterprise = await enterpriseRepository.GetEnterpriseAll();
            return enterprise;
        }

        // GET api/<EnterpriseController>/5
        [HttpGet("{id}")]
        public async Task<Enterprise> Get(int id)
        {
            var enterprise = await enterpriseRepository.GetEnterpriseById(id);
            return enterprise;
        }

        // POST api/<EnterpriseController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Enterprise enterprise)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await enterpriseRepository.Create(enterprise);
            return Ok(enterprise);
        }

        // PUT api/<EnterpriseController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Enterprise enterprise)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var exist = await enterpriseRepository.GetEnterpriseById(id);
            if (exist == null)
            {
                return NotFound();
            }
            enterprise.EnterpriseId = id;
            await enterpriseRepository.Update(enterprise);
            return Ok("Updatse Success");
        }
        // DELETE api/<EnterpriseController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exist = await enterpriseRepository.GetEnterpriseById(id);
            if (exist == null)
            {
                return NotFound();
            }
            await enterpriseRepository.Delete(id);
            return Ok("Delete Success");
        }
    }
}
