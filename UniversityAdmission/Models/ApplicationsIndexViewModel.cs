using UniversityAdmission.Models.Entities;

namespace UniversityAdmission.Models
{
    public class ApplicationsIndexViewModel
    {
        public List<Application> Applications { get; set; } = new();

        // Paging
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
    }
}
