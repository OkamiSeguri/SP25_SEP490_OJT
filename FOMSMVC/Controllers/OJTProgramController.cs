using FOMSDTO;
using FOMSService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FOMSMVC.Controllers
{
    public class OJTProgramController : Controller
    {
        private readonly IOJTProgramService ojtProgramService;
        public OJTProgramController(IOJTProgramService ojtProgramService)
        {
            this.ojtProgramService = ojtProgramService;
        }
        // GET: OJTProgramController
        public async Task<ActionResult> Index()
        {
            var ojtProgram = await ojtProgramService.GetOJTProgramAll();
            return View(ojtProgram);
        }
        public async Task<ActionResult> ListOJTProgram()
        {
            var ojtProgram = await ojtProgramService.GetOJTProgramAll();
            return View(ojtProgram);
        }
        // GET: OJTProgramController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var ojtProgram = await ojtProgramService.GetOJTProgramById(id);
            if (ojtProgram == null)
            {
                return NotFound();
            }
            return View(ojtProgram);
        }
        // GET: OJTProgramController/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: OJTProgramController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OJTProgramDTO ojtProgram)
        {
            try
            {
                await ojtProgramService.Create(ojtProgram);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        // GET: OJTProgramController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Update(OJTProgramDTO ojtProgram)
        {
            try
            {
                await ojtProgramService.Update(ojtProgram);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        // GET: OJTProgramController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var ojtProgram = await ojtProgramService.GetOJTProgramById(id);
            if (ojtProgram == null)
            {
                return NotFound();
            }
            return View(ojtProgram);
        }
        // GET: OJTProgramController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var ojtProgram = await ojtProgramService.GetOJTProgramById(id);
            if (ojtProgram == null)
            {
                return NotFound();
            }
            return View(ojtProgram);
        }
        // POST: OJTProgramController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await ojtProgramService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
