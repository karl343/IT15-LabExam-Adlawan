using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace crud_it15.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Position { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [Required(ErrorMessage = "Daily rate is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Daily rate must not be negative.")]
        [Display(Name = "Daily Rate")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DailyRate { get; set; }

        // Navigation property
        public ICollection<Payroll> Payrolls { get; set; } = new List<Payroll>();

        // Computed property
        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}";
    }
}
