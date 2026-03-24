using Microsoft.AspNetCore.Mvc.Rendering;
using UniversityAdmission.Models.Entities;
using UniversityAdmission.Models.Enums;

namespace UniversityAdmission.Models
{
    public class RankingViewModel
    {
        public List<Application> Applications { get; set; }

        // Filters
        public int? SpecialityId { get; set; }
        public int? PreferenceOrder { get; set; }
        public ApplicationStatus? Status { get; set; }

        // Dropdowns
        public List<SelectListItem> Specialities { get; set; }

        // Paging
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
    }
}
