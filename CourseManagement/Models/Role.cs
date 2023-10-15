namespace CourseManagement.Models
{
	public class Role
	{
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string? Description { get; set; }
		public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
	}
}
