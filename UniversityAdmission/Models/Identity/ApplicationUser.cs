using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using UniversityAdmission.Models.Entities;

namespace UniversityAdmission.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
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

        public string FullName => $"{FirstName} {MiddleName} {LastName}";

        // Navigation

        public Grade? Grade { get; set; }

        public Application? Application { get; set; }

        public AdmissionResult? AdmissionResult { get; set; }
    }
}