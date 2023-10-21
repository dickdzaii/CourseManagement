namespace CourseManagement.Models.ViewModels
{
    public class CourseMaterialViewModel : CourseMaterial
    {
        // todo: getting file by its path
        public IFormFile? FileContent { get; set; }
        public string FullPath { get; set; }
    }
}
