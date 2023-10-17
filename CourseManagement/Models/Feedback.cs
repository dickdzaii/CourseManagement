using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Models
{
    public class Feedback
    {
        public int FeedbackId { get; set; }
        [Required]
        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }
        public string Content { get; set; }
        [Required]
        public double RatingPoint { get; set; }
        public DateTime FeedbackDate { get; set; } = DateTime.Now;
    }
}
