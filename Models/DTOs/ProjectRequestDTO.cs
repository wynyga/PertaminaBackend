using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class ProjectRequestDTO : IValidatableObject
    {
        [Required]
        [StringLength(255)]
        public string ProjectName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(On Progress|Completed|Overdue)$", ErrorMessage = "Status harus berupa 'On Progress', 'Completed', atau 'Overdue'.")]
        public string Status { get; set; } = "On Progress";

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Range(0, 100, ErrorMessage = "Progress harus berada dalam rentang 0 hingga 100.")]
        public int Progress { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate < StartDate)
            {
                yield return new ValidationResult("End Date tidak boleh lebih awal dari Start Date.", new[] { nameof(EndDate) });
            }
        }
    }
}
