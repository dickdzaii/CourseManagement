using CourseManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Common
{
    public static class CourseValidation
    {
        public static void ValidateUpsert(this DataContext context, Course destination, Course? source = null)
        {
            if ((destination.CourseName.ToLower().Equals(source?.CourseName.ToLower())
                && source?.CourseId != destination.CourseId)
                || (context.Courses.Any(c => c.CourseName.ToLower().Equals(destination.CourseName.ToLower()))
                    && source == null))
            {
                throw new ValidationException($"A course named {destination.CourseName} already existed");
            }

            if (destination.Fee <= 0)
            {
                throw new ValidationException($"Course's fee must be greater than 0");
            }
        }

        public static void ValidateDelete(this DataContext context, Course course)
        {

        }
    }
}
