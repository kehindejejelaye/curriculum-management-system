using CMS.DATA.DTO;
using CMS.MVC.Models;
using CMS.MVC.Services.ServicesInterface;
using Microsoft.AspNetCore.Mvc;

namespace CMS.MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpGet]
        public IActionResult Login()
        {

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            var newUser = new LoginDto();

            if (login != null)
            {

                newUser.Password = login.Password;
                newUser.Email = login.Email;


            }

            var loginResponse = await _authService.Login(newUser);

            if (loginResponse.StatusCode == StatusCodes.Status200OK && loginResponse.Result.Role == "Student")
            {
                // Authentication successful
                return RedirectToAction("StudentDashboard", "Dashboard"); // Redirect to Student logged-in page
            }
            else if (loginResponse.StatusCode == StatusCodes.Status200OK && loginResponse.Result.Role == "Admin")
            {
                // Authentication successful
                return RedirectToAction("AdminDashboard", "Dashboard"); // Redirect to Admin logged-in page
            }
            else if (loginResponse.StatusCode == StatusCodes.Status200OK && loginResponse.Result.Role == "Facilitator")
            {
                // Authentication successful
                return RedirectToAction("FacilitatorDashboard", "Dashboard"); // Redirect to Facilitator logged-in page
            }
            else if (loginResponse.StatusCode == StatusCodes.Status400BadRequest)
            {
                // Invalid credentials
                ModelState.AddModelError(string.Empty, "Invalid credentials");
            }
            else
            {
                // Other errors
                ModelState.AddModelError(string.Empty, "An error occurred during login");
            }

            // Return to the login view with the updated model state
            return View(login);
        }

        public IActionResult FacilitatorLogin()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPasword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        public IActionResult ConfirmPassword()
        {
            return View();
        }

        public IActionResult PermissionDecadev()
        {
            return View();
        }

        public IActionResult PermissionFacilitator()
        {
            return View();
        }

        public IActionResult SuccessInvite()
        {
            return View();
        }
    }
}
