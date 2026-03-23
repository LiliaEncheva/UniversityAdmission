using UniversityAdmission.Models.Enums;
using UniversityAdmission.Models.Identity;

namespace UniversityAdmission.Models.Entities
{
    public class Application
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public decimal TotalScore { get; set; } 

        public DateTime CreatedOn { get; set; }

        public ApplicationStatus Status { get; set; }

        public bool IsConfirmed { get; set; }

        public ICollection<ApplicationSpeciality> ApplicationSpecialities { get; set; }
    }
}