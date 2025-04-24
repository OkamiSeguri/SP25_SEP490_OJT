using BusinessObject;
using CsvHelper;
using FOMSOData.Authorize;
using FOMSOData.Mappings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using Repositories;
using Services;
using System.Globalization;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FOMSOData.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CustomAuthorize("1")]

    public class CurriculumController : ControllerBase
    {
        private readonly ICurriculumRepository curriculumRepository;

        public CurriculumController()
        {
            curriculumRepository = new CurriculumRepository();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Curriculum>>> Get()
        {

                var curriculum = await curriculumRepository.GetCurriculumAll();

                if (curriculum == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "No curriculum found"
                    });
                }
                return Ok(new
                {
                    results = curriculum.Select(u => new
                    {
                        id = u.CurriculumId,
                        subjectCode = u.SubjectCode,
                        subjectName = u.SubjectName,
                        credits = u.Credits,
                        isMandatory = u.IsMandatory,
                    }),
                    status = StatusCodes.Status200OK
                });
            }


        [HttpGet("{id}")]
        public async Task<ActionResult<Curriculum>> Get(int id)
        {

                var curriculum = await curriculumRepository.GetCurriculumById(id);
                if (curriculum == null)
                {
                    return NotFound(new { code = 404, detail = "Curriculum not found." });
                }
                return Ok(curriculum);
            }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Curriculum curriculum)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new { code = 400, detail = "Invalid request data." });
            }

                await curriculumRepository.Create(curriculum);
                return Ok(new {  result = curriculum, status = 200});
            }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Curriculum curriculum)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    code = StatusCodes.Status400BadRequest,
                    detail = "Invalid request data."
                });
            }

                var exist = await curriculumRepository.GetCurriculumById(id);
                if (exist == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "Curriculum not found"
                    });
                }

                curriculum.CurriculumId = id;
                await curriculumRepository.Update(curriculum);
                return Ok(new { result = curriculum, status = 200 });
            }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {

                var exist = await curriculumRepository.GetCurriculumById(id);
                if (exist == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new
                    {
                        code = StatusCodes.Status404NotFound,
                        detail = "Curriculum not found"
                    });
                }
                await curriculumRepository.Delete(id);
                return Ok(new { status = 200, message = "Delete Success" });
            }


        [HttpPost("import")]
        public async Task<IActionResult> ImportCurriculumCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "File is empty or missing." });

            using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            using (var csv = new CsvReader(stream, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<CurriculumMap>();
                csv.Read();
                csv.ReadHeader();
                var requiredHeaders = new List<string> { "SubjectCode", "SubjectName" };
                var missingHeaders = requiredHeaders.Where(h => !csv.HeaderRecord.Contains(h)).ToList();

                if (missingHeaders.Any())
                {
                    return BadRequest(new
                    {
                        message = "CSV file is missing required columns!",
                        missingColumns = missingHeaders
                    });
                }

                var records = new List<Curriculum>();
                var invalidRows = new List<int>();
                int rowIndex = 1;

                while (csv.Read())
                {
                    bool isValid = true;
                    if (!csv.TryGetField("SubjectCode", out string subjectCode) || string.IsNullOrWhiteSpace(subjectCode))
                        isValid = false;
                    if (!csv.TryGetField("SubjectName", out string subjectName) || string.IsNullOrWhiteSpace(subjectName))
                        isValid = false;

                    if (!isValid)
                    {
                        invalidRows.Add(rowIndex);
                        rowIndex++;
                        continue;
                    }

                    records.Add(new Curriculum
                    {
                        SubjectCode = subjectCode.Trim(),
                        SubjectName = subjectName.Trim()
                    });

                    rowIndex++;
                }

                if (invalidRows.Any())
                {
                    return BadRequest(new
                    {
                        message = "Some rows have missing required fields!",
                        invalidRows = invalidRows
                    });
                }

                var duplicateSubjectCodes = await curriculumRepository.ImportCurriculums(records);
                if (duplicateSubjectCodes.Count > 0)
                {
                    return BadRequest(new
                    {
                        message = "Some SubjectCode already exists!",
                        duplicates = duplicateSubjectCodes
                    });
                }

                return Ok(new { message = "Curriculum CSV imported successfully!", count = records.Count });
            }
        }



    }
}