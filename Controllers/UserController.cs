using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManager.Models;
using UserManager.Services;

namespace UserManager.Controllers
{
    [TypeFilter(typeof(UserExceptionFilter))]
    public class UserController : BaseController
    {
        public UserController(IUserService userService) 
            : base(userService)
        {
        }

        public async Task<ActionResult> Index()
        {
            // Show list of users to all authenticated users
            // Admin user can edit and delete
            try
            {
                var users = await _userService.GetUsersAsync(GetTokenFromRequest());
                return View(users);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> LoginAsync([Bind("Email,Password")] UserLogin login)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Validate login via user service
                    var token = await _userService.ValidateUser(login);
                    this.Response.Cookies.Append("token", token);
                    ViewData["isLoggedIn"] = true;
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(login); // Bad input try again
        }


        public IActionResult Logout()
        {
            // Clear token
            this.Response.Cookies.Delete("token");
            ViewData["isLoggedIn"] = false;
            return RedirectToAction("Index", "Home");
        }

        [ActionName("Create")]
        public IActionResult CreateAsync()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(
            [Bind("FirstName,LastName,Email,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                // Save via user service
                user.CreatedDate = DateTime.Now;
                await _userService.RegisterUser(user);
                return RedirectToAction("Index", "Home");
            }

            return View(user);
        }

        [ActionName("Edit")]
        public async Task<ActionResult> GetEditAsync(string email)
        {
            if ((bool)ViewData["isAdmin"] != true)
            {
                return BadRequest();
            }

            if (email == null)
            {
                return BadRequest();
            }

            var item = await _userService.GetUserAsync(GetTokenFromRequest(), email);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(string email)
        {
            if ((bool)ViewData["isAdmin"] != true)
            {
                return BadRequest();
            }

            var user = await _userService.GetUserAsync(GetTokenFromRequest(), email);
            if (user == null)
                return NotFound();

            if (await TryUpdateModelAsync(user, "", m => m.FirstName, m => m.LastName, m => m.Email))
            {
                user.ModifiedDate = DateTime.Now;

                await _userService.UpdateUserAsync(GetTokenFromRequest(), user);
                return View("Details", user);
            }

            return View(user);
        }

        [ActionName("Delete")]
        public async Task<ActionResult> DeleteAsync(string email)
        {
            if ((bool)ViewData["isAdmin"] != true)
            {
                return BadRequest();
            }

            if (email == null)
            {
                return BadRequest();
            }

            var item = await _userService.GetUserAsync(GetTokenFromRequest(), email);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmedAsync([Bind("email")] string email)
        {
            if ((bool)ViewData["isAdmin"] != true)
            {
                return BadRequest();
            }

            var item = await _userService.GetUserAsync(GetTokenFromRequest(), email);
            if (item == null)
            {
                return NotFound();
            }

            await _userService.DeleteUserAsync(GetTokenFromRequest(), email);

            return RedirectToAction("Index");
        }
    }
}