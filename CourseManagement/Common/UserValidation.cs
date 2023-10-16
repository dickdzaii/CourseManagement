using CourseManagement.Models;
using System.ComponentModel.DataAnnotations;

namespace CourseManagement.Common
{
    public static class UserValidation
    {
        public static void ValidateUpsert(this DataContext context, User destination, User? source = null)
        {
            var userExist = context.Users.Where(u => u.UserName.ToLower() == destination.UserName.Trim().ToLower()
                                             || u.Email.ToLower() == destination.Email.Trim().ToLower());

            // is null means == null, is not null means != null
            // https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/is
            if (source is not null)
            {
                userExist = userExist.Where(u => u.UserId != source.UserId);
            }

            if (userExist.Any())
            {
                throw new ValidationException($"A user existed with username {destination.UserName} or email {destination.Email}.");
            }
        }

        public static void ValidateDelete(this DataContext context, User candidate)
        {
            var courseExist = context.Courses.Any(c => c.AccId == candidate.UserId);
            if (courseExist)
            {
                throw new ValidationException("Cannot delete user since this user is author of some courses.");
            }
        }
    }
}
