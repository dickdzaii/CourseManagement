using CourseManagement.Constants;
using CourseManagement.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }
        [Required]
        public int AccId { get; set; }
        public User Customer { get; set; }
        public UserViewModel CustomerView { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public EnrollmentState EnrollmentState { get; set; }
        public double? Score { get; set; }
        public double? TotalHour { get; set; }
        public bool? Passed { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
