using FOMSDTO;
using FOMSService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FOMSMVC.Controllers
{
    public class OJTResultController : Controller
    {
        private readonly IOJTRegistrationService oJTRegistrationService;
        public OJTResultController(IOJTRegistrationService oJTRegistrationService)
        {
            this.oJTRegistrationService = oJTRegistrationService;
        }
        // GET: OJTResultController
        public async Task<ActionResult> Index()
        {
            var oJTRegistration = await oJTRegistrationService.GetOJTRegistrationAll();
            return View(oJTRegistration);
        }
        public async Task<ActionResult> ListOJTRegistration()
        {
            var oJTRegistration = await oJTRegistrationService.GetOJTRegistrationAll();
            return View(oJTRegistration);
        }
        // GET: OJTResultController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var oJTRegistration = await oJTRegistrationService.GetOJTRegistrationById(id);
            if (oJTRegistration == null)
            {
                return NotFound();
            }
            return View(oJTRegistration);
        }
        // GET: OJTResultController/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: OJTResultController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OJTRegistrationDTO oJTRegistration)
        {
            try
            {
                await oJTRegistrationService.Create(oJTRegistration);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OJTResultController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var oJTRegistration = await oJTRegistrationService.GetOJTRegistrationById(id);
            if (oJTRegistration == null)
            {
                return NotFound();
            }
            return View(oJTRegistration);
        }
        // POST: OJTResultController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, OJTRegistrationDTO oJTRegistration)
        {
            try
            {
                await oJTRegistrationService.Update(oJTRegistration);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        // GET: OJTResultController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var oJTRegistration = await oJTRegistrationService.GetOJTRegistrationById(id);
            if (oJTRegistration == null)
            {
                return NotFound();
            }
            return View(oJTRegistration);
        }
        // POST: OJTResultController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await oJTRegistrationService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
