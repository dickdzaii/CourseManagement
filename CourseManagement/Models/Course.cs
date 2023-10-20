using System;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Models
{
	public class Course
	{
		public int CourseId { get; set; }
        [Required]
        public int AccId  { get; set; }
        public User Mentor { get; set; }
        [Required]
        public string CourseName { get; set; }
		public string? Image { get; set; }
        [Required]
        public double Fee { get; set; }
        [Required]
        public bool Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int EnrollmentCount { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<CourseMaterial> CourseMaterials { get; set; }
        public ICollection<CoursePayment> CoursePayments { get; set; }

        public Course()
        {
            Enrollments = new List<Enrollment>();
            CourseMaterials = new List<CourseMaterial>();
            CoursePayments = new List<CoursePayment>();
        }
    }

}
