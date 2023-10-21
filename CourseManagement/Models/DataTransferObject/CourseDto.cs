using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Models.DataTransferObject
{
    public class CourseDto
    {
        [Required]
        public int AccId { get; set; }
        [Required]
        public string CourseName { get; set; }
        public string? Image { get; set; }
        [Required]
        public double Fee { get; set; }
        public bool? Status { get; set; } = true;
        public int? EnrollmentCount { get; set; }
    }
}
