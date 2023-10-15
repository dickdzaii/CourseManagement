using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Models.ViewModels
{
	public class SignUpModel
	{
		[Required(ErrorMessage = "User Name is required")]
		public string Username { get; set; }

		[EmailAddress]
		[Required(ErrorMessage = "Email is required")]
		public string Email { get; set; }

		[Required(ErrorMessage = "Password is required")]
		public string Password { get; set; }
	}
}
