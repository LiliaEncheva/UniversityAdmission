using System.ComponentModel.DataAnnotations;

namespace UniversityAdmission.Models.Entities
{
    public class Speciality
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(0, int.MaxValue)]
        public int Seats { get; set; }
        public ICollection<ApplicationSpeciality> ApplicationSpecialities { get; set; }
    }
}
