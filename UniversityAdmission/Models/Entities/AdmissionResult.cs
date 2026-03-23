using System.ComponentModel.DataAnnotations;

namespace UniversityAdmission.Models.Entities
{
    public class AdmissionResult
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public int? SpecialityId { get; set; }

        public string Status { get; set; }

        public int? PreferenceOrder { get; set; }
    }
}
