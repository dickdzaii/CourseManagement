using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Models.DataTransferObject
{
    public class PaymentModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public string CardName { get; set; }
        [Required]
        [RegularExpression(@"^[0-9]{12,19}$")]
        public string CardNumber { get; set; }
        [Required]
        [RegularExpression(@"\d{3}")]
        public string CVC { get; set; }
        [Required]
        [RegularExpression("^(0[1-9]|1[0-2]|[1-9])\\/[1-9][0-9][1-9][0-9]$")]
        public string ValidTo { get; set; }
    }
}
