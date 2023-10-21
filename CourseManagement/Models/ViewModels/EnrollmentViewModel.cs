using CourseManagement.Constants;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Models.ViewModels
{
    public class EnrollmentViewModel
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int AccId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public EnrollmentState EnrollmentState { get; set; }
        public double? Score { get; set; }
        public double? TotalHour { get; set; }
        public bool? Passed { get; set; }
    }
}
