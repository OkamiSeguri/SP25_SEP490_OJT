using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FOMSDTO;
using FOMSService;
using Microsoft.AspNetCore.Mvc.Rendering;
using BusinessObject;

namespace FOMSMVC.Controllers
{
    public class CohortCurriculumController : Controller
    {
        private readonly ICohortCurriculumService cohortCurriculumService;
        private readonly ICurriculumService curriculumService;

        public CohortCurriculumController(ICohortCurriculumService cohortCurriculumService, ICurriculumService curriculumService)
        {
            this.cohortCurriculumService = cohortCurriculumService;
            this.curriculumService = curriculumService;
        }

        // GET: CohortCurriculumController
        public async Task<ActionResult> Index()
        {
            var cohort = await cohortCurriculumService.GetCohortCurriculumAll();
            return View(cohort);
        }

        // GET: CohortCurriculumController/Details/5
        public async Task<ActionResult> Details(string cohort)
        {
            var cohorts = await cohortCurriculumService.GetCohortCurriculumByCohort(cohort);
            return View(cohorts);
        }

        // GET: CohortCurriculumController/Create
        public async Task<ActionResult> Create(string cohort)
        {
            var curriculums = await curriculumService.GetCurriculumAll();
            ViewBag.CurriculumList = new SelectList(curriculums, "CurriculumId", "SubjectCode");
            return View(cohort);
        }

        // POST: CohortCurriculumController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CohortCurriculumDTO cohortCurriculum)
        {
            try
            {
                await cohortCurriculumService.Create(cohortCurriculum);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CohortCurriculumController/Edit/5
        public async  Task<ActionResult> Edit(string cohort)
        {
            var cohorts = await cohortCurriculumService.GetCohortCurriculumByCohort(cohort);
            return View(cohorts);
        }

        // POST: CohortCurriculumController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(string cohort, CohortCurriculumDTO cohortCurriculumDTO)
        {
            try
            {
                Console.WriteLine($"Received Cohort: {cohortCurriculumDTO.Cohort}");
                Console.WriteLine($"Received CurriculumId: {cohortCurriculumDTO.CurriculumId}");
                Console.WriteLine($"Received Semester: {cohortCurriculumDTO.Semester}");

                // Tìm bản ghi cũ
                var exist = await cohortCurriculumService.GetCohortCurriculumByCohort(cohort);

                if (exist != null)
                {
                    await cohortCurriculumService.Delete(cohort); // Xóa bản ghi cũ
                }

                // Chuyển đổi từ DTO sang entity model
                var newCohort = new CohortCurriculumDTO
                {
                    Cohort = cohortCurriculumDTO.Cohort,
                    CurriculumId = cohortCurriculumDTO.CurriculumId,
                    Semester = cohortCurriculumDTO.Semester
                };

                await cohortCurriculumService.Create(newCohort); // Lưu bản ghi mới

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return View();
            }
        }



        // GET: CohortCurriculumController/Delete/5
        public async Task<ActionResult> Delete(string cohort)
        {
            var cohorts = await cohortCurriculumService.GetCohortCurriculumByCohort(cohort);
            return View(cohorts);
        }

        // POST: CohortCurriculumController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string cohort, IFormCollection collection)
        {
            try
            {
                await cohortCurriculumService.Delete(cohort);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
