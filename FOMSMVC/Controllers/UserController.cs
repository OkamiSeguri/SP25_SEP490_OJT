using FOMSDTO;
using FOMSMVC.Models;
using FOMSService;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BusinessObject;

namespace FOMSMVC.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserservice userservice;
        private readonly IStudentGradeService studentGradeService;
        private readonly ApplicationDbContext _context;

        public UserController(IUserservice userservice, ApplicationDbContext context, IStudentGradeService studentGradeService)
        {
            this.studentGradeService = studentGradeService;
            this._context = context;
            this.userservice = userservice;
        }

            // GET: UserController
        public async Task<ActionResult>  Index()
        {
            var user = await userservice.GetUserAll();
            var filteredUsers = user.Where(u => u.Role != 3).ToList();
            return View(filteredUsers);
        }
        public async Task<ActionResult> Grade(int userId)
        {
            var grade = await studentGradeService.GetGradeByUserId(userId);
            ViewBag.UserId = userId;
            return View(grade);
        }
        // GET: UserController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var user = await userservice.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserDTO user)
        {
            try
            {
                await userservice.Create(user);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var user = await userservice.GetUserById(id);
            return View(user);
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserDTO user)
        {
            try
            {
                await userservice.Update(user);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
           var user =  await userservice.GetUserById(id);

            return View(user);
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, IFormCollection collection)
        {
            try
            {
                await userservice.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public IActionResult Login()
        {
            return View();
        }

        // Xử lý đăng nhập
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await userservice.Login(email, password);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View();
            }

            HttpContext.Session.SetInt32("UserId", user.UserId);
            HttpContext.Session.SetString("FullName", user.FullName);
            HttpContext.Session.SetInt32("Role", user.Role);
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Register()
        {
            return View();
        }

        // POST: /User/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(registerDTO);
            }

            // Kiểm tra xác nhận mật khẩu
            if (registerDTO.Password != registerDTO.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu xác nhận không khớp.");
                return View(registerDTO);
            }

            // Gọi service để gửi thông tin đăng ký
            var user = await userservice.Register(registerDTO);
            if (user != null)
            {
                TempData["Success"] = "Đăng ký thành công!";
                return RedirectToAction("Login");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Email đã tồn tại hoặc có lỗi xảy ra.");
                return View(registerDTO);
            }
        }

        // Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Type");
            HttpContext.Session.Remove("Email");
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        
        }
        [HttpGet]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse") };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                var user = _context.Users.FirstOrDefault(u => u.Email == email);
                if (user == null)
                {
                    user = new User
                    {
                        FullName = name,
                        Email = email,
                        Password = Guid.NewGuid().ToString(),
                        Role = 0,
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                HttpContext.Session.SetString("Email", user.Email);
                HttpContext.Session.SetInt32("Role", user.Role);
                HttpContext.Session.SetInt32("AccountId", user.UserId);

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login");
        }

    }
}
