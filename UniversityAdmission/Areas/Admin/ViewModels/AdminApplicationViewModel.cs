using System.ComponentModel.DataAnnotations;

namespace UniversityAdmission.Areas.Admin.ViewModels
{
    public class AdminApplicationViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Имейл на кандидат")]
        public string UserEmail { get; set; }

        [Display(Name = "Дата на създаване")]
        public DateTime CreatedOn { get; set; }
    }
}
