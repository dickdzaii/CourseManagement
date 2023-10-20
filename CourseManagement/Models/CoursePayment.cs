namespace CourseManagement.Models
{
    public class CoursePayment
    {
        public int CoursePaymentId { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime? PaymentDate { get; set; } = DateTime.Now;
        public int AccId { get; set; }
        public User Customer { get; set; }
    }
}
