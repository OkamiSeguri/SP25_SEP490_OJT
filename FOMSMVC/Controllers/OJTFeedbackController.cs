using Microsoft.AspNetCore.Mvc;
using FOMSDTO;
using FOMSService;
using Microsoft.AspNetCore.Http;

namespace FOMSMVC.Controllers
{
    public class OJTFeedbackController : Controller
    {
        private readonly IOJTFeedbackService ojtFeedbackService;
        public OJTFeedbackController(IOJTFeedbackService ojtFeedbackService)
        {
            this.ojtFeedbackService = ojtFeedbackService;
        }

        // GET: OJTFeedbackController
        public async Task<ActionResult> Index()
        {
            var ojtFeedback = await ojtFeedbackService.GetOJTFeedbackAll();
            return View(ojtFeedback);
        }

        // GET: OJTFeedbackController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var ojtFeedback = await ojtFeedbackService.GetOJTFeedbackById(id);
            if (ojtFeedback == null)
            {
                return NotFound();
            }
            return View(ojtFeedback);
        }

        // GET: OJTFeedbackController/Create
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Edit()
        {
            return View();
        }
        // POST: OJTFeedbackController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OJTFeedbackDTO ojtFeedback)
        {
            try
            {
                await ojtFeedbackService.Create(ojtFeedback);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: OJTFeedbackController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var ojtFeedback = await ojtFeedbackService.GetOJTFeedbackById(id);
            if (ojtFeedback == null)
            {
                return NotFound();
            }
            return View(ojtFeedback);
        }

        // POST: OJTFeedbackController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, OJTFeedbackDTO ojtFeedback)
        {
            try
            {
                await ojtFeedbackService.Update(ojtFeedback);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Delete()
        {
            return View();
        }
        // POST: OJTFeedbackController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await ojtFeedbackService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
