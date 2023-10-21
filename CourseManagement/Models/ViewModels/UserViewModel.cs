using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Models.ViewModels
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
    }
}
