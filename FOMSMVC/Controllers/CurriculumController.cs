using BusinessObject;
using FOMSDTO;
using FOMSService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FOMSMVC.Controllers
{
    public class CurriculumController : Controller
    {
        private readonly ICurriculumService curriculumService;
        public CurriculumController(ICurriculumService curriculumService)
        {
            this.curriculumService = curriculumService;
        }

        // GET: CurriculumController
        public async Task<ActionResult> Index()
        {
            var curriculum = await curriculumService.GetCurriculumAll();
            return View(curriculum);
        }

        // GET: CurriculumController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var curriculum = await curriculumService.GetCurriculumById(id);
            if (curriculum == null)
            {
                return NotFound();
            }
            return View(curriculum);
        }

        // GET: CurriculumController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CurriculumController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CurriculumDTO curriculum)
        {
            try
            {
                await curriculumService.Create(curriculum);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CurriculumController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var curriculum = await curriculumService.GetCurriculumById(id);
            return View(curriculum);
        }

        // POST: CurriculumController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CurriculumDTO curriculum)
        {
            try
            {
                await curriculumService.Update(curriculum);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CurriculumController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CurriculumController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
