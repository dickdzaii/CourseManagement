namespace CourseManagement.Models.ViewModels
{
    public class CourseMaterialViewModel : CourseMaterial
    {
        public IFormFile? FileContent { get; set; }
        public string FullPath { get; set; }
    }
}
