using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Models
{
	public class User
	{
		public int UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
		public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
		public virtual ICollection<Course> Mentors { get; set; } = new List<Course>();
		public virtual ICollection<Enrollment> Customers { get; set; } = new List<Enrollment>();
	}
}
