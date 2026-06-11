// SafeNet.Web/Controllers/AccountController.cs
// REEMPLAZAR el archivo existente con este contenido completo
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SafeNet.Web.Models.ViewModels;

namespace SafeNet.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser>  _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(
            UserManager<IdentityUser>  userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager  = userManager;
            _signInManager = signInManager;
        }

        // ─── Login ────────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password,
                model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
                return LocalRedirect(returnUrl ?? "/");

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty,
                    "Cuenta bloqueada temporalmente. Intenta de nuevo en 10 minutos.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return View(model);
        }

        // ─── Register ─────────────────────────────────────────────────────────

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user   = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // ─── Logout ───────────────────────────────────────────────────────────

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ─── AccessDenied ─────────────────────────────────────────────────────

        public IActionResult AccessDenied() => View();

        // ─── Profile ──────────────────────────────────────────────────────────

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var vm = new ProfileViewModel
            {
                Email    = user.Email    ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            bool changed = false;

            // ── Cambiar contraseña ───────────────────────────────────────────
            if (!string.IsNullOrEmpty(model.CurrentPassword) &&
                !string.IsNullOrEmpty(model.NewPassword))
            {
                var passResult = await _userManager.ChangePasswordAsync(
                    user, model.CurrentPassword, model.NewPassword);

                if (!passResult.Succeeded)
                {
                    foreach (var error in passResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }

                await _signInManager.RefreshSignInAsync(user);
                changed = true;
            }

            TempData["Success"] = changed
                ? "Contraseña actualizada correctamente."
                : "No se realizaron cambios.";

            return RedirectToAction("Profile");
        }
    }
}
