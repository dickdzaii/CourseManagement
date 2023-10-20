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

            var mentorExists = context.Users.Any(u => u.UserId == destination.AccId);
            if (!mentorExists)
            {
                throw new ValidationException($"Mentor with id {destination.AccId} not found.");
            }

            if (destination.Fee < 0)
            {
                throw new ValidationException($"Course's fee must be greater or equal 0.");
            }

            if (destination.EnrollmentCount <= 0)
            {
                throw new ValidationException($"Course must allow at least one enrollment.");
            }
        }

        public static void ValidateDelete(this DataContext context, Course course)
        {

        }
    }
}
