using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using UniversityAdmission.Models.Entities;

namespace UniversityAdmission.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Името е задължително.")]
        [Display(Name = "Име")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Презимето е задължително.")]
        [Display(Name = "Презиме")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Фамилията е задължителна.")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Училището е задължително.")]
        [Display(Name = "Училище")]
        public string School { get; set; }

        [Required(ErrorMessage = "Градът е задължителен.")]
        [Display(Name = "Град")]
        public string City { get; set; }

        [Required(ErrorMessage = "Адресът е задължителен.")]
        [Display(Name = "Адрес")]
        public string Address { get; set; }

        public string FullName => $"{FirstName} {MiddleName} {LastName}";

        // Navigation

        public Grade? Grade { get; set; }

        public Application? Application { get; set; }

        public AdmissionResult? AdmissionResult { get; set; }
    }
}