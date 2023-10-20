using CourseManagement.Models;
using System.Net;

namespace CourseManagement.Middlewares
{
    public class UserMiddleware
    {
        internal const string ROUTE_KEY = "userId";

        readonly RequestDelegate _next;

        public UserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DataContext dbContext)
        {
            context.Request.RouteValues.TryGetValue(ROUTE_KEY, out var value);

            if (value is not null)
            {
                var validId = int.TryParse(value.ToString(), out var courseId);
                if (!validId)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return;
                }

                var vertex = dbContext.Users.FirstOrDefault(c => c.UserId == courseId);

                if (vertex is null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    return;
                }

            }

            await _next(context);
        }
    }
}
