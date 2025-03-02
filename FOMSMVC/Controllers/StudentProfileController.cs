using BusinessObject;
using FOMSDTO;
using FOMSService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FOMSMVC.Controllers
{
    public class StudentProfileController : Controller
    {
        private readonly IStudentProfileService studentProfileService;
        public StudentProfileController(IStudentProfileService studentProfileService)
        {
            this.studentProfileService = studentProfileService;
        }

        public async  Task<ActionResult> Index()
        {
            var student = await studentProfileService.GetStudentProfileAll();
            return View(student);
        }

        // GET: StudentProfileController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var student = await studentProfileService.GetStudentProfileById(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // GET: StudentProfileController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StudentProfileController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StudentProfileDTO studentProfile)
        {
            try
            {
                await studentProfileService.Create(studentProfile);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: StudentProfileController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var student = await studentProfileService.GetStudentProfileById(id); 
            return View(student);
        }

        // POST: StudentProfileController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StudentProfileDTO studentProfile)
        {
            try
            {
                await studentProfileService.Update(studentProfile);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: StudentProfileController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var student = await studentProfileService.GetStudentProfileById(id);
            return View(student);
        }

        // POST: StudentProfileController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await studentProfileService.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
