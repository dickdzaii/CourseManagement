using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Models
{
    public class CourseMaterial
    {
        public int CourseMaterialId { get; set; }
        [Required]
        public string MaterialTitle { get; set; }
        public string MaterialDescription { get; set; }
        [Required]
        public string FilePath { get; set; }
        [Required]
        public string ContentType { get; set; }
        public int FileOrder { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [Required]
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
