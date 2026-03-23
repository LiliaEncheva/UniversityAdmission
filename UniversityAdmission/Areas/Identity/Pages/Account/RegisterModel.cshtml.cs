using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using UniversityAdmission.Models.Identity;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using System.Linq;

namespace UniversityAdmission.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Полето 'Имейл' е задължително.")]
            [EmailAddress(ErrorMessage = "Невалиден формат на имейл.")]
            [Display(Name = "Имейл")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Полето 'Парола' е задължително.")]
            [StringLength(100, ErrorMessage = "Паролата трябва да е поне {2} символа.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Парола")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Повтори паролата")]
            [Compare("Password", ErrorMessage = "Паролите не съвпадат.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Име")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Презиме")]
            public string MiddleName { get; set; }

            [Required]
            [Display(Name = "Фамилия")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "Училище")]
            public string School { get; set; }

            [Required]
            [Display(Name = "Град")]
            public string City { get; set; }

            [Required]
            [Display(Name = "Адрес")]
            public string Address { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Телефон")]
            public string PhoneNumber { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    MiddleName = Input.MiddleName,
                    LastName = Input.LastName,
                    School = Input.School,
                    City = Input.City,
                    Address = Input.Address,
                    PhoneNumber = Input.PhoneNumber
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Потребителят е създаден успешно.");

                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            return Page();
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            ExternalLogins = (await _signInManager
                .GetExternalAuthenticationSchemesAsync())
                .ToList();
        }
    }
}