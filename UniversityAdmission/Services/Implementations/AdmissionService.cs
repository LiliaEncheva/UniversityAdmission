using UniversityAdmission.Data;
using UniversityAdmission.Services.Interfaces;

namespace UniversityAdmission.Services.Implementations
{
    public class AdmissionService : IAdmissionService
    {
        private readonly ApplicationDbContext _context;

        public AdmissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task RunAdmissionAsync()
        {
            // 1. Вземи всички потвърдени кандидатури
            // 2. Изчисли бал
            // 3. Сортирай по бал (низходящо)
            // 4. Провери желанията
            // 5. Намали местата
            // 6. Запиши резултатите
        }

        public async Task PublishResultsAsync()
        {
            // Маркира резултатите като публични
        }
        
    }
}
