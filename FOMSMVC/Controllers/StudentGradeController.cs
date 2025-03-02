using FOMSService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FOMSMVC.Controllers
{
    public class StudentGradeController : Controller
    {
        private readonly IStudentGradeService studentGradeService;
        public StudentGradeController(IStudentGradeService studentGradeService)
        {
            this.studentGradeService = studentGradeService;
        }

        // GET: StudentGradeController
        public async Task<ActionResult> Index()
        {
            var grade = await studentGradeService.GetGradesAll();
            return View(grade);
        }

        // GET: StudentGradeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: StudentGradeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StudentGradeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: StudentGradeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: StudentGradeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: StudentGradeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: StudentGradeController/Delete/5
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
