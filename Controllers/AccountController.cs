using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MvcWebApp.Models;
using MvcWebApp.Services; // For IEmailSender
using MvcWebApp.ViewModels; // For RegisterViewModel
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities; // For WebEncoders
using System.Text; // For Encoding

namespace MvcWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender; // Inject email service

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender) // Add IEmailSender
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender; // Store injected service
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser 
                { 
                    UserName = model.Email, // Typically UserName is the email
                    Email = model.Email,
                    Name = model.Name // Our custom property
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // User created successfully
                    // Send confirmation email
                    var subject = "Welcome to Our Application!";
                    var message = $"Hello {user.Name},<br><br>Thank you for registering an account with us. We're excited to have you!<br><br>Best regards,<br>The Team";
                    
                    try
                    {
                        await _emailSender.SendEmailAsync(user.Email, subject, message);
                    }
                    catch (Exception ex)
                    {
                        // Log the email sending error, but don't let it block registration flow
                        // You might want to queue the email for retry in a real application
                        Console.WriteLine($"Failed to send welcome email to {user.Email}: {ex.Message}");
                    }


                    // Optional: Sign the user in immediately after registration
                    // await _signInManager.SignInAsync(user, isPersistent: false);
                    // return RedirectToAction("Index", "Home");

                    // For this example, we'll redirect to a registration confirmation page or login page
                    // You might want to implement email confirmation flow here as well
                    // For simplicity, we'll just show a success message.
                    ViewBag.Message = "Registration successful! A welcome email has been sent to your email address.";
                    return View("RegistrationConfirmation"); // Create a simple RegistrationConfirmation.cshtml view
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/Login (Example - usually scaffolded)
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        
        // POST: /Account/Login (Example - usually scaffolded)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            // ... (Login logic using _signInManager) ...
            // This is just a placeholder, actual login logic would be here
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                // ... (handle other cases like lockout, 2FA, failure) ...
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            return View(model);
        }

        // GET: /Account/Logout (Example - usually scaffolded)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}