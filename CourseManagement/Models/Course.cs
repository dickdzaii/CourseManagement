using System;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Models
{
	public class Course
	{
		public int CourseId { get; set; }
        [Required]
        public int AccId  { get; set; }
        [Required]
        public string CourseName { get; set; }
		public string? Image { get; set; }
        [Required]
        public double Fee { get; set; }
        [Required]
        public bool Status { get; set; }
		public DateTime CreatedDate { get; set; }
		public int EnrollmentCount { get; set; }
	}
}
