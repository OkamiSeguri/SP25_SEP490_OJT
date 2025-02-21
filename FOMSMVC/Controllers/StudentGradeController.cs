using FOMSDTO;
using FOMSService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FOMSMVC.Controllers
{
    public class StudentGradeController : Controller
    {
        private readonly IStudentGradeService studentGradeService;
        private readonly IUserservice userservice;
        private readonly ICurriculumService curriculumService;

        public StudentGradeController(IStudentGradeService studentGradeService, IUserservice userservice, ICurriculumService curriculumService)
        {
            this.studentGradeService = studentGradeService;
            this.userservice = userservice;
            this.curriculumService = curriculumService;
        }

        // GET: StudentGradeController
        public async Task<ActionResult> Index()
        {
            var user = await userservice.GetUserAll();
            var filteredUsers = user.Where(u => u.Role == 0).ToList();
            return View(filteredUsers);
        }

        public async Task<ActionResult> Grade(int userId)
        {
            var grade = await studentGradeService.GetGradeByUserId(userId);
            ViewBag.UserId = userId;
            return View(grade);
        }

        // GET: StudentGradeController/Details/5
        public async Task<ActionResult> Details(int userId, int curriculumId)
        {
            var grade = await studentGradeService.GetGrade(userId,curriculumId);
            if (grade == null)
            {
                return NotFound();
            }
            return View(grade);
        }

        // GET: StudentGradeController/Create
        public async Task<ActionResult> Create(int id)
        {
            var curriculums = await curriculumService.GetCurriculumAll(); 
            ViewBag.CurriculumList = new SelectList(curriculums, "CurriculumId", "SubjectCode");

            var model = new StudentGradeDTO { UserId = id }; 
            return View(model);
        }

        // POST: StudentGradeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StudentGradeDTO studentGradeDTO)
        {
            try
            {
                await studentGradeService.Create(studentGradeDTO);
                return RedirectToAction("Grade", new { userId = studentGradeDTO.UserId });
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
        public async Task<ActionResult> Delete(int userId, int curriculumId)
        {
            var grade = await studentGradeService.GetGrade(userId, curriculumId);
            return View(grade);
        }

        // POST: StudentGradeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int userId,int curriculumId, IFormCollection collection)
        {
            try
            {
                await studentGradeService.Delete(userId, curriculumId);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
