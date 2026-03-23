using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using UniversityAdmission.Models.Identity;

namespace UniversityAdmission.Models.Entities
{
    public class Grade
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Display(Name = "Оценка от диплома")]
        [Required]
        [Range(2.00, 6.00)]
        public decimal Diploma { get; set; }

        [Display(Name = "Оценка по БЕЛ")]
        [Required]
        [Range(2.00, 6.00)]
        public decimal BEL { get; set; }

        [Display(Name = "Оценка по Математика")]
        [Required]
        [Range(2.00, 6.00)]
        public decimal Math { get; set; }

        [Display(Name = "Оценка по Английски език")]
        [Required]
        [Range(2.00, 6.00)]
        public decimal English { get; set; }

        public decimal TotalScore { get; set; }

        [ValidateNever]
        public ApplicationUser User { get; set; }   


    }
}
