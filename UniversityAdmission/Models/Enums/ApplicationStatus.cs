using System.ComponentModel.DataAnnotations;

namespace UniversityAdmission.Models.Enums
{
    public enum ApplicationStatus
    {
        [Display(Name = "В изчакване")]
        Pending = 0,

        [Display(Name = "Не е приет")]
        NotAccepted = 1,

        [Display(Name = "Потвърдена")]
        Confirmed = 2,

        [Display(Name = "Отхвърлена")]
        Rejected = 3
    }
}