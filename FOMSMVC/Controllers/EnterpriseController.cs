using FOMSDTO;
using FOMSService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FOMSMVC.Controllers
{
    public class EnterpriseController : Controller
    {
        private readonly IEnterpriseService enterpriseService;
        public EnterpriseController(IEnterpriseService enterpriseService)
        {
            this.enterpriseService = enterpriseService;
        }

        // GET: EnterpriseController
        public async Task<ActionResult> Index()
        {
            var enterprise = await enterpriseService.GetEnterpriseAll();
            return View(enterprise);
        }        
        public async Task<ActionResult> ListEnteprise()
        {
            var enterprise = await enterpriseService.GetEnterpriseAll();
            return View(enterprise);
        }

        // GET: EnterpriseController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var enterprise = await enterpriseService.GetEnterpriseById(id);
            if(enterprise == null)
            {
                return NotFound();
            }
            return View(enterprise);
        }

        // GET: EnterpriseController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EnterpriseController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EnterpriseDTO enterprise)
        {
            try
            {
                await enterpriseService.Create(enterprise);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EnterpriseController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var enterprise = await enterpriseService.GetEnterpriseById(id);
            return View(enterprise);
        }

        // POST: EnterpriseController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EnterpriseDTO enterprise)
        {
            try
            {
                await  enterpriseService.Update(enterprise);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EnterpriseController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EnterpriseController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await enterpriseService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
