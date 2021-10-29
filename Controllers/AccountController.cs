using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyChat.Models;
using MyChat.Services;
using MyChat.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyChat.Controllers
{
    public class AccountController : Controller
    {
        private ChatContext _db;
        private IMemoryCache cache;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;


        IWebHostEnvironment _appEnvironment;

        public AccountController(ChatContext db,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IWebHostEnvironment appEnvironment,
            IMemoryCache memoryCache)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _appEnvironment = appEnvironment;
            cache = memoryCache;
        }


        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByEmailAsync(model.Email);
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
                    user,
                    model.Password,
                    model.RememberMe,
                    false
                    );
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("UserCabinet");
                }
                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, IFormFile uploadedFile)
        {
            if (ModelState.IsValid)
            {
                if(uploadedFile == null)
                {
                    string path = "/Files/default_avatar.png";
                    FileModel file = new FileModel { Name = "default_avatar.png", Path = path };
                    _db.Files.Add(file);
                    model.Avatar = file.Path;
                }
                else
                {
                    string path = "/Files/" + uploadedFile.FileName;
                    using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                    {
                        await uploadedFile.CopyToAsync(fileStream);
                    }
                    FileModel file = new FileModel { Name = uploadedFile.FileName, Path = path };
                    _db.Files.Add(file);
                    model.Avatar = file.Path;
                }
                
                User user = new User
                {
                    Email = model.Email,
                    UserName = model.Login,
                    Avatar = model.Avatar,
                    BirthDate = model.DateOfBirth
                };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "user");
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }

                EmailService emailService = new EmailService();
                await emailService.SendEmailAsync(user.Email, "Приветствие от имени сайта", "Позвальте вас поздравить с успешеной регистарцией на сайте!");

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        private async Task<User> GetCurrentUser()
        {
            return await _userManager.GetUserAsync(HttpContext.User);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        public IActionResult UserCabinet()
        {
            string email = GetCurrentUser().Result.Email;
            User currentUser = _db.ChatUsers.FirstOrDefault(x => x.Email == email);
            return View(currentUser);
        }


        [HttpGet]
        public IActionResult Edit(string userId)
        {
            if(userId != null)
            {
                User user = _db.ChatUsers.FirstOrDefault(x => x.Id == userId);
                if(user != null)
                {
                    EditViewModel evm = new EditViewModel
                    {
                        Login = user.UserName,
                        Email = user.Email,
                        Avatar = user.Avatar,
                        DateOfBirth = user.BirthDate
                    };


                    return View(evm);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditViewModel model, IFormFile uploadedFile)
        {
            string avatar = GetCurrentUser().Result.Avatar;

            if (uploadedFile == null)
            {

                model.Avatar = avatar;
            }
            else
            {
                string path = "/Files/" + uploadedFile.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + path, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(fileStream);
                }
                FileModel file = new FileModel { Name = uploadedFile.FileName, Path = path };
                _db.Files.Add(file);
                model.Avatar = file.Path;
            }

            if (model.DateOfBirth == DateTime.MinValue)
            {
                model.DateOfBirth = GetCurrentUser().Result.BirthDate;
            }

            User user = new User
            {
                Email = model.Email,
                UserName = model.Login,
                Avatar = model.Avatar,
                BirthDate = model.DateOfBirth
            };

            _db.ChatUsers.Update(user);

            _db.SaveChanges();

            return RedirectToAction("UserCabinet");

        }


        public async Task<IActionResult> ShowCachedUser()
        {
            var userId = GetCurrentUser().Result.Id;

            UserService userService = new UserService(_db, cache);

            User user = await userService.GetUser(userId);
            if (user != null)
                return Content($"User: {user.UserName} ; Email: {user.Email}");
            return Content("User not found");
        }

        public async Task<IActionResult> SendDataToMail(string userEmail)
        {
            var user = _db.ChatUsers.FirstOrDefault(x => x.Email == userEmail);

            EmailService emailService = new EmailService();
            await emailService.SendEmailAsync
                (user.Email,
                "Данные о пользователе",
                $"Логин: {user.UserName}\n" +
                $"Электронная почта: {user.Email}\n" +
                $"Возраст: {DateTime.Now.Year - user.BirthDate.Year}");

            return RedirectToAction("UserCabinet");
        }
    }
}
