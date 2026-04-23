using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace crud_it15.Models
{
    public class Payroll
    {
        [Key]
        public int PayrollId { get; set; }

        [Required(ErrorMessage = "Employee is required.")]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Payroll Date")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Days worked is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Days worked must not be negative.")]
        [Display(Name = "Days Worked")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DaysWorked { get; set; }

        [Display(Name = "Gross Pay")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GrossPay { get; set; }

        [Required(ErrorMessage = "Deduction is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Deduction must not be negative.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Deduction { get; set; }

        [Display(Name = "Net Pay")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NetPay { get; set; }

        // Navigation property
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }
    }
}
