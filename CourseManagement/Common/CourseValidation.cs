using CourseManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Common
{
    public static class CourseValidation
    {
        public static void ValidateUpsert(this DataContext context, Course destination, Course? source = null)
        {
            var courses = context.Courses.Where(c => c.CourseName.ToLower().Equals(destination.CourseName.Trim().ToLower()));
            if (source != null)
            {
                courses = courses.Where(c => c.CourseId != source.CourseId);
            }

            if (courses.Any())
            {
                throw new ValidationException($"A course named {destination.CourseName} already existed.");
            }

            if (destination.Fee <= 0)
            {
                throw new ValidationException($"Course's fee must be greater than 0.");
            }
        }

        public static void ValidateDelete(this DataContext context, Course course)
        {

        }
    }
}
