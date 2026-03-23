using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace UniversityAdmission.Areas.User.ViewModels
{
    public class ApplicationCreateViewModel : IValidatableObject
    {
        public decimal TotalScore { get; set; }

        [Required(ErrorMessage = "Трябва да изберете първа специалност.")]
        public int? FirstChoiceId { get; set; }

        public int? SecondChoiceId { get; set; }

        public int? ThirdChoiceId { get; set; }

        public List<SelectListItem> Specialities { get; set; } = new();

        // Проверка да няма еднакви специалности
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var choices = new List<int>();

            if (FirstChoiceId.HasValue)
                choices.Add(FirstChoiceId.Value);

            if (SecondChoiceId.HasValue)
                choices.Add(SecondChoiceId.Value);

            if (ThirdChoiceId.HasValue)
                choices.Add(ThirdChoiceId.Value);

            if (choices.Count != choices.Distinct().Count())
            {
                yield return new ValidationResult(
                    "Не можете да изберете една и съща специалност повече от веднъж.",
                    new[]
                    {
                        nameof(FirstChoiceId),
                        nameof(SecondChoiceId),
                        nameof(ThirdChoiceId)
                    });
            }
        }
    }
}