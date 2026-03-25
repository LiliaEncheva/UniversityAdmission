using System.ComponentModel.DataAnnotations;

namespace UniversityAdmission.Models.Entities
{
    public class Speciality
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Името на специалността е задължително.")]
        [Display(Name = "Име на специалност")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Броят места е задължителен.")]
        [Range(0, int.MaxValue, ErrorMessage = "Броят места трябва да е положително число.")]
        [Display(Name = "Брой места")]
        public int Seats { get; set; }

        public ICollection<ApplicationSpeciality> ApplicationSpecialities { get; set; }
    }
}
