using FOMSDTO;
using FOMSService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FOMSMVC.Controllers
{
    public class OJTRegistrationController : Controller
    {
        private readonly IOJTRegistrationService oJTRegistrationService;
        public OJTRegistrationController(IOJTRegistrationService oJTRegistrationService)
        {
            this.oJTRegistrationService = oJTRegistrationService;
        }
        // GET: OJTRegistrationController
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
        // GET: OJTRegistrationController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var oJTRegistration = await oJTRegistrationService.GetOJTRegistrationById(id);
            if (oJTRegistration == null)
            {
                return NotFound();
            }
            return View(oJTRegistration);
        }
        // GET: OJTRegistrationController/Create
        public ActionResult Create()
        {
            return View();
        }
        // POST: OJTRegistrationController/Create
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
        // GET: OJTRegistrationController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var oJTRegistration = await oJTRegistrationService.GetOJTRegistrationById(id);
            if (oJTRegistration == null)
            {
                return NotFound();
            }
            return View(oJTRegistration);
        }
        // POST: OJTRegistrationController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, OJTRegistrationDTO oJTRegistration)
        {
            try
            {
                oJTRegistration.OJTId = id;
                await oJTRegistrationService.Update(oJTRegistration);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(oJTRegistration);
            }
        }
        // GET: OJTRegistrationController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var oJTRegistration = await oJTRegistrationService.GetOJTRegistrationById(id);
            if (oJTRegistration == null)
            {
                return NotFound();
            }
            return View(oJTRegistration);
        }
        // POST: OJTRegistrationController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, OJTRegistrationDTO oJTRegistration)
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
