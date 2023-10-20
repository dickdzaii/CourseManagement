using CourseManagement.Constants;
using CourseManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Common
{
    public static class EnrollmentValidation
    {
        public static void ValidateUpsert(this DataContext context, Enrollment destination, Enrollment? source = null)
        {
            if (source is null)
            {
                ValidateInsert(context, destination);
            }
            else
            {
                ValidateUpdate(context, destination, source);
            }
        }

        private static void ValidateInsert(DataContext context, Enrollment destination)
        {
            // nếu user không tồn tại
            var customer = context.Users.FirstOrDefault(u => u.UserId == destination.AccId);
            if (customer == null)
            {
                throw new ValidationException($"Customer with id {destination.AccId} is not existed.");

            }

            // nếu course không tồn tại
            var course = context.Courses.FirstOrDefault(c => c.CourseId == destination.CourseId);
            if (course == null)
            {
                throw new ValidationException($"Course with id {destination.CourseId} is not existed.");
            }

            // nếu user chưa pay khóa học
            var payment = context.CoursePayments.FirstOrDefault(p => p.CourseId == destination.CourseId);
            if (payment == null)
            {
                throw new ValidationException($"User {customer.UserName} did not pay this course.");
            }

            var enrollmentOfUser = context.Enrollments.Where(e => e.AccId == destination.AccId && e.CourseId == destination.CourseId);

            // nếu user đã có enrollment chưa hoàn thành hoặc đã pass enrollment (pass course) (nếu hoàn thành mà fail thì có thể enroll lại)
            if (enrollmentOfUser.Any(e => e.EnrollmentState != EnrollmentState.Finished || e.Passed.Value == true))
            {
                throw new ValidationException($"User {customer.UserName} either passed or incompleted this course.");
            }

            //nếu user đã enroll bằng với giới hạn enrollment count thì không thể enroll được lữa
            if (enrollmentOfUser.Count() >= course.EnrollmentCount)
            {
                throw new ValidationException($"User {customer.UserName}'s enrollment exceeds the course's enrollment's limits.");
            }
        }

        private static void ValidateUpdate(DataContext context, Enrollment destination, Enrollment? source = null)
        {
            throw new NotImplementedException();
        }
    }
}
