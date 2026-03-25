using System.ComponentModel.DataAnnotations;

namespace UniversityAdmission.Areas.Admin.ViewModels
{
    public class SpecialityViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Име на специалност")]
        public string Name { get; set; }

        [Display(Name = "Брой места")]
        public int Seats { get; set; }
    }
}
