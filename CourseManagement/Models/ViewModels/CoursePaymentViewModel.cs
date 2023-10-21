namespace CourseManagement.Models.ViewModels
{
    public class CoursePaymentViewModel
    {
        public int CoursePaymentId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public double CoursePrice { get; set; }
        public DateTime? PaymentDate { get; set; }
        public int AccId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }
}
