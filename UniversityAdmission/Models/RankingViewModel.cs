using Microsoft.AspNetCore.Mvc.Rendering;
using UniversityAdmission.Models.Entities;
using UniversityAdmission.Models.Enums;

namespace UniversityAdmission.Models
{
    public class RankingViewModel
    {
        public List<Application> Applications { get; set; } = new();

        // Filters
        public int? SpecialityId { get; set; }
        public int? PreferenceOrder { get; set; }
        public ApplicationStatus? Status { get; set; }

        // Dropdowns
        public List<SelectListItem> Specialities { get; set; } = new();
        public List<SelectListItem> Statuses { get; set; } = new();

        // Paging
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}
