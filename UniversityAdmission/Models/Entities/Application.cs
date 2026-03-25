using System.ComponentModel.DataAnnotations;
using UniversityAdmission.Models.Enums;
using UniversityAdmission.Models.Identity;

namespace UniversityAdmission.Models.Entities
{
    public class Application
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Display(Name = "Общ бал")]
        public decimal TotalScore { get; set; } 

        [Display(Name = "Дата на създаване")]
        public DateTime CreatedOn { get; set; }

        [Display(Name = "Статус")]
        public ApplicationStatus Status { get; set; }

        [Display(Name = "Потвърдено")]
        public bool IsConfirmed { get; set; }

        public ICollection<ApplicationSpeciality> ApplicationSpecialities { get; set; }
    }
}